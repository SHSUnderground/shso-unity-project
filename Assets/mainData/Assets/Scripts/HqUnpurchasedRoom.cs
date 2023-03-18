using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class HqUnpurchasedRoom : HqRoom2
{
	private TransactionMonitor loadTransaction;

	private RoomStaticData roomData;

	public RoomStaticData RoomData
	{
		get
		{
			if (roomData == null)
			{
				roomData = HqController2.Instance.GetRoomData(id);
			}
			return roomData;
		}
	}

	public override AccessState RoomState
	{
		get
		{
			return AccessState.Unpurchased;
		}
		set
		{
		}
	}

	public override void Initialize(RoomStaticData roomData)
	{
		base.Initialize(roomData);
		AppShell.Instance.EventMgr.AddListener<ShoppingItemPurchasedMessage>(OnRoomPurchasedMessage);
		AppShell.Instance.EventMgr.AddListener<HQRoomLayoutSaveResponseMessage>(OnRoomLayoutResponseMessage);
	}

	public void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<ShoppingItemPurchasedMessage>(OnRoomPurchasedMessage);
		AppShell.Instance.EventMgr.RemoveListener<HQRoomLayoutSaveResponseMessage>(OnRoomLayoutResponseMessage);
	}

	public override void Update()
	{
	}

	public override void Activate()
	{
		if (HqController2.Instance.LoadScreen != null)
		{
			HqController2.Instance.LoadScreen.TextureSource = string.Format("hq_bundle|mshs_hq_loading_{0}", id);
			HqController2.Instance.LoadScreen.IsVisible = true;
		}
	}

	public override void Deactivate()
	{
		if (HqController2.Instance.LoadScreen != null)
		{
			HqController2.Instance.LoadScreen.IsVisible = false;
		}
	}

	public override void ScrollLeft(float speed)
	{
	}

	public override void ScrollRight(float speed)
	{
	}

	public override void ScrollDown(float speed)
	{
	}

	public override void ScrollUp(float speed)
	{
	}

	public override void ScrollVector(Vector2 dir)
	{
	}

	public override void ZoomUp()
	{
	}

	public override void ZoomDown()
	{
	}

	public override void GotoViewMode()
	{
	}

	public override void GotoPlacementMode()
	{
	}

	public override void GotoFlingaMode()
	{
	}

	public override void ApplyTheme(string themeName, bool makeDirty)
	{
	}

	public override HqItem AddItem(GameObject go)
	{
		return null;
	}

	public override void DelItem(GameObject go)
	{
	}

	public override void MoveItem(GameObject go)
	{
	}

	public override void RevertToLastSave()
	{
	}

	public override void Save()
	{
	}

	public override void Reset()
	{
	}

	protected override void LoadGeometry()
	{
		fixedItems = new List<HqFixedItem>();
		animations = new List<Animation>();
		emitters = new List<ParticleEmitter>();
		themeColors = new List<Color>();
	}

	protected void StartBundleLoad(string name)
	{
		if (loadTransaction != null && !(HqController2.Instance.GetAssetBundle(name) != null))
		{
			if (!loadTransaction.HasStep(name))
			{
				loadTransaction.AddStep(name);
			}
			loadTransaction.AddStepBundle(name, name);
			AppShell.Instance.BundleLoader.FetchAssetBundle(name, OnAssetBundleLoaded, loadTransaction);
		}
	}

	protected void OnAssetBundleLoaded(AssetBundleLoadResponse response, object extraData)
	{
		if (loadTransaction == extraData)
		{
			if ((response.Error != null && response.Error != string.Empty) || response.Bundle == null)
			{
				loadTransaction.Fail(response.Error);
				return;
			}
			HqController2.Instance.AddAssetBundle(response.Path, response.Bundle);
			loadTransaction.CompleteStep(response.Path);
		}
	}

	protected void OnBundleLoadComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		loadTransaction = null;
		if (exit != 0 || RoomData == null)
		{
			return;
		}
		AssetBundle assetBundle = HqController2.Instance.GetAssetBundle(RoomData.bundleName);
		if (!(assetBundle != null))
		{
			return;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(assetBundle.mainAsset, Vector3.zero, Quaternion.identity) as GameObject;
		HqRoom2 component = Utils.GetComponent<HqRoom2>(gameObject, Utils.SearchChildren);
		component.transform.parent = null;
		string[] array = AppShell.Instance.SharedHashTable["HQ_" + RoomData.id] as string[];
		if (array != null)
		{
			component.LoadSaveData(array);
		}
		component.Initialize(RoomData);
		component.LoadItems();
		UnityEngine.Object.Destroy(gameObject);
		Utils.ActivateTree(component.gameObject, false);
		HqController2.Instance.SwitchRooms(this, component);
		if (HqController2.Instance.ActiveRoom == this)
		{
			HqController2.Instance.SetActiveRoom(component);
			if (HqController2.Instance.LoadScreen != null)
			{
				HqController2.Instance.LoadScreen.IsVisible = false;
			}
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	protected void OnRoomPurchasedMessage(ShoppingItemPurchasedMessage msg)
	{
		if (msg.ItemType == OwnableDefinition.Category.HQRoom && RoomData != null && msg.OwnableId == RoomData.typeid)
		{
			loadTransaction = TransactionMonitor.CreateTransactionMonitor("Room purchased Transaction", OnBundleLoadComplete, 0f, null);
			if (!AppShell.Instance.SharedHashTable.ContainsKey("HQ_" + id))
			{
				loadTransaction.AddStep("fetch_and_load_updated_inventory");
				HqController2.Instance.Profile.StartInventoryFetch(OnInventoryFetched);
			}
			StartBundleLoad(RoomData.bundleName);
			if (RoomData.fixedBundleNames != null)
			{
				foreach (string fixedBundleName in RoomData.fixedBundleNames)
				{
					StartBundleLoad(fixedBundleName);
				}
			}
		}
	}

	protected void OnRoomLayoutResponseMessage(HQRoomLayoutSaveResponseMessage msg)
	{
		if (msg.roomId == id && HqController2.Instance.Profile != null)
		{
			string uri = "resources$users/" + HqController2.Instance.Profile.UserId + "/hq/room/" + id + "/";
			AppShell.Instance.WebService.StartRequest(uri, delegate(ShsWebResponse r)
			{
				RoomPlacementLoaded(r, loadTransaction);
			}, null, ShsWebService.ShsWebServiceType.RASP);
		}
	}

	protected void RoomPlacementLoaded(ShsWebResponse response, object extraData)
	{
		if (response.Status != 200)
		{
			CspUtils.DebugLog("Could not get room placement for " + id + " response.Status = " + (HttpStatusCode)response.Status);
			if (loadTransaction != null)
			{
				loadTransaction.CompleteStep("room_placement_load");
			}
		}
		else
		{
			try
			{
				string[] array = response.Body.Split('\n');
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = array[i].Trim();
				}
				int j = 0;
				while (j < array.Length)
				{
					int num = j;
					string text = array[j++];
					if (j >= array.Length)
					{
						break;
					}
					j++;
					if (j >= array.Length)
					{
						break;
					}
					for (; j < array.Length && array[j].Length > 0; j++)
					{
					}
					string[] array2 = new string[j - num];
					for (int k = 0; k < array2.Length; k++)
					{
						array2[k] = array[num + k];
					}
					AppShell.Instance.SharedHashTable["HQ_" + text.ToLower()] = array2;
					for (; j < array.Length && array[j].Length == 0; j++)
					{
					}
				}
				if (loadTransaction != null)
				{
					loadTransaction.CompleteStep("room_placement_load");
				}
			}
			catch (Exception message)
			{
				CspUtils.DebugLog(message);
				if (loadTransaction != null)
				{
					loadTransaction.FailStep("room_placement_load", "Exception");
				}
			}
		}
	}

	protected void OnInventoryFetched(bool succeeded)
	{
		List<string> list = new List<string>();
		foreach (Item value in HqController2.Instance.Profile.AvailableItems.Values)
		{
			string placedObjectAssetBundle = value.Definition.PlacedObjectAssetBundle;
			if (placedObjectAssetBundle != null && !HqController2.Instance.IsAssetBundleLoaded(value.Definition) && !list.Contains(placedObjectAssetBundle))
			{
				list.Add(placedObjectAssetBundle);
				StartBundleLoad(placedObjectAssetBundle);
			}
		}
		loadTransaction.CompleteStep("fetch_and_load_updated_inventory");
	}
}
