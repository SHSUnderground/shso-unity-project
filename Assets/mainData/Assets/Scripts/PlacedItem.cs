using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlacedItem : HqItem
{
	protected Item inventoryItem;

	protected int placedId;

	public override ItemDefinition ItemDefinition
	{
		get
		{
			if (InventoryItem != null)
			{
				return InventoryItem.Definition;
			}
			return base.ItemDefinition;
		}
	}

	public Item InventoryItem
	{
		get
		{
			return inventoryItem;
		}
	}

	public int PlacedId
	{
		get
		{
			return placedId;
		}
		set
		{
			placedId = value;
		}
	}

	public void Initialize(int placedId, Item inventoryItem, Vector3 position, Quaternion rotation, HqRoom2 room)
	{
		if (inventoryItem != null)
		{
			Initialize(room, inventoryItem.Definition);
		}
		else
		{
			Initialize(room, null);
		}
		PlacedId = placedId;
		this.inventoryItem = inventoryItem;
		base.Position = position;
		base.Rotation = rotation;
		defaultRotation = rotation;
		loadedOrSaved = false;
	}

	public void AppendToSave(StringBuilder sb)
	{
		Vector3 eulerAngles = base.Rotation.eulerAngles;
		if (inventoryItem == null)
		{
			sb.Append("-1");
		}
		else
		{
			sb.Append(inventoryItem.Id);
		}
		sb.Append("|");
		sb.Append(PlacedId);
		sb.Append("|");
		Vector3 position = base.Position;
		sb.Append(position.x);
		sb.Append("|");
		Vector3 position2 = base.Position;
		sb.Append(position2.y);
		sb.Append("|");
		Vector3 position3 = base.Position;
		sb.Append(position3.z);
		sb.Append("|");
		sb.Append(eulerAngles.x);
		sb.Append("|");
		sb.Append(eulerAngles.y);
		sb.Append("|");
		sb.Append(eulerAngles.z);
		sb.AppendLine(string.Empty);
		loadedOrSaved = true;
	}

	public static PlacedItem LoadFromSave(string line, HqRoom2 hqRoom)
	{
		//Discarded unreachable code: IL_02cf
		PlacedItem placedItem = null;
		try
		{
			string[] array = line.Split('|');
			Item value;
			HqController2.Instance.Profile.AvailableItems.TryGetValue(array[0], out value);
			if (value == null)
			{
				CspUtils.DebugLog("Trying to create a placed item from an inventory item the user does not have: " + array[0]);
				return null;
			}
			if (value.Placed >= value.Quantity)
			{
				CspUtils.DebugLog("Trying to place more of an item type <" + array[0] + "> than are available.");
				return null;
			}
			GameObject gameObject = HqController2.Instance.GetTempObjectPrefab(0);
			if (value.Definition.PlacedObjectAssetBundle != null)
			{
				AssetBundle assetBundle = HqController2.Instance.GetAssetBundle(value.Definition.PlacedObjectAssetBundle);
				if (assetBundle == null)
				{
					CspUtils.DebugLog("Could not get bundle " + value.Definition.PlacedObjectAssetBundle);
					return null;
				}
				gameObject = (assetBundle.Load(value.Definition.PlacedObjectPrefab) as GameObject);
				if (gameObject == null)
				{
					CspUtils.DebugLog("Could not load bundle " + value.Definition.Name + " prefab:" + value.Definition.PlacedObjectPrefab);
					return null;
				}
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject) as GameObject;
			placedItem = gameObject2.AddComponent<PlacedItem>();
			if (!(placedItem != null))
			{
				return placedItem;
			}
			placedItem.inventoryItem = value;
			placedItem.PlacedId = int.Parse(array[1]);
			placedItem.Position = new Vector3(float.Parse(array[2]), float.Parse(array[3]), float.Parse(array[4]));
			Vector3 euler = default(Vector3);
			euler.x = float.Parse(array[5]);
			euler.y = float.Parse(array[6]);
			euler.z = float.Parse(array[7]);
			placedItem.Rotation = Quaternion.Euler(euler);
			placedItem.Room = hqRoom;
			placedItem.sequencesPlaying = new Dictionary<string, SequencePlayInfo>();
			placedItem.defaultRotation = gameObject.transform.rotation;
			gameObject2.transform.parent = placedItem.Room.transform;
			gameObject2.transform.position = placedItem.Position;
			gameObject2.transform.rotation = placedItem.Rotation;
			HqObject2 component = Utils.GetComponent<HqObject2>(gameObject2);
			component.PlacedId = placedItem.PlacedId;
			component.InventoryId = value.Id;
			placedItem.loadedOrSaved = true;
			placedItem.CreateClaims();
			value.Placed++;
			return placedItem;
		}
		catch (Exception ex)
		{
			CspUtils.DebugLog(hqRoom.Id + ": unable to load object from saved data <" + line + "> with error <" + ex + ">.");
			return null;
		}
	}

	public bool Consume()
	{
		if (!HqController2.Instance.Visiting)
		{
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("instance_id", inventoryItem.Id);
			string uri = "resources$users/inventory_consume.py";
			AppShell.Instance.WebService.StartRequest(uri, delegate(ShsWebResponse response)
			{
				OnRemoveFromInventoryResponse(response);
			}, wWWForm.data, ShsWebService.ShsWebServiceType.RASP);
		}
		return true;
	}

	private void OnRemoveFromInventoryResponse(ShsWebResponse response)
	{
		if (response.Status == 200 && response.Body.ToLower().Contains("ok"))
		{
			bool flag = false;
			foreach (KeyValuePair<string, Item> availableItem in HqController2.Instance.Profile.AvailableItems)
			{
				if (availableItem.Value.Id == inventoryItem.Id)
				{
					availableItem.Value.Quantity--;
					availableItem.Value.Placed--;
					flag = (availableItem.Value.Quantity <= 0);
					break;
				}
			}
			if (flag)
			{
				HqController2.Instance.Profile.AvailableItems.Remove(inventoryItem.Id);
			}
		}
		else
		{
			CspUtils.DebugLog("Error removing item from inventory: " + response.Status);
			CspUtils.DebugLog("  " + response.Body);
		}
	}

	public void SetThrowableLayer()
	{
		Utils.SetLayerTree(base.gameObject, 13);
		if (base.EntryPoints != null)
		{
			Component[] entryPoints = base.EntryPoints;
			for (int i = 0; i < entryPoints.Length; i++)
			{
				EntryPoint entryPoint = (EntryPoint)entryPoints[i];
				entryPoint.gameObject.layer = 2;
			}
		}
		if (base.DockPoints != null)
		{
			Component[] dockPoints = base.DockPoints;
			for (int j = 0; j < dockPoints.Length; j++)
			{
				DockPoint dockPoint = (DockPoint)dockPoints[j];
				dockPoint.gameObject.layer = 2;
			}
		}
		HqTrigger[] components = Utils.GetComponents<HqTrigger>(base.gameObject, Utils.SearchChildren);
		if (components != null)
		{
			HqTrigger[] array = components;
			foreach (HqTrigger hqTrigger in array)
			{
				hqTrigger.gameObject.layer = 2;
			}
		}
	}
}
