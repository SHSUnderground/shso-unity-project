using System.Collections.Generic;
using UnityEngine;

public class HqAssignableRoom : HqRoom2
{
	protected DormPortrait portrait;

	protected bool isInAssignmentMode;

	protected DormPortrait Portrait
	{
		get
		{
			if (portrait == null)
			{
				DormPortrait[] components = Utils.GetComponents<DormPortrait>(this, Utils.SearchChildren, true);
				if (components != null && components.Length > 0)
				{
					portrait = components[0];
				}
			}
			return portrait;
		}
	}

	public bool IsInAssignmentMode
	{
		get
		{
			return isInAssignmentMode;
		}
		set
		{
			isInAssignmentMode = value;
		}
	}

	public string Owner
	{
		get
		{
			if (roomKeyValues.ContainsKey("owner"))
			{
				return roomKeyValues["owner"] as string;
			}
			return null;
		}
	}

	public void AssignRoomTo(string characterName)
	{
		ThemeStaticData heroTheme = HqController2.Instance.GetHeroTheme(characterName);
		if (heroTheme != null)
		{
			ApplyTheme(heroTheme, true);
		}
		ClearItems(placedItems);
		aiUsableItems.Clear();
		SetPortrait(characterName);
		HqPlacedItemInfo[] components = Utils.GetComponents<HqPlacedItemInfo>(this, Utils.SearchChildren, true);
		List<string> list = new List<string>();
		HqPlacedItemInfo[] array = components;
		foreach (HqPlacedItemInfo hqPlacedItemInfo in array)
		{
			foreach (KeyValuePair<string, Item> availableItem in HqController2.Instance.Profile.AvailableItems)
			{
				Item value = availableItem.Value;
				if (value.Definition != null)
				{
					ItemDefinition definition = value.Definition;
					if (definition.HeroSet == characterName && definition.Name.Contains(hqPlacedItemInfo.ItemName))
					{
						PlacedItem placedItem = null;
						HqObject2 hqObject = null;
						if (value.Placed > 0)
						{
							placedItem = HqController2.Instance.FindPlacedItem(value);
							if (placedItem != null)
							{
								placedItem.Room.DelItem(placedItem.gameObject);
								hqObject = Utils.GetComponent<HqObject2>(placedItem.gameObject);
								placedItem.Room = this;
								placedItem.Position = hqPlacedItemInfo.transform.position;
								placedItem.Rotation = hqPlacedItemInfo.transform.rotation;
								if (hqObject != null)
								{
									hqObject.gameObject.transform.position = placedItem.Position;
									hqObject.gameObject.transform.rotation = placedItem.Rotation;
									hqObject.gameObject.transform.parent = base.transform;
									Utils.ActivateTree(hqObject.gameObject, true);
									hqObject.PlacedId = -1;
									hqObject.PlacedId = GetNewPlacedId();
									placedItem.PlacedId = hqObject.PlacedId;
								}
							}
						}
						if (placedItem == null)
						{
							GameObject original = HqController2.Instance.GetTempObjectPrefab(0);
							if (definition.PlacedObjectAssetBundle != null)
							{
								AssetBundle assetBundle = HqController2.Instance.GetAssetBundle(definition.PlacedObjectAssetBundle);
								original = (assetBundle.Load(definition.PlacedObjectPrefab) as GameObject);
							}
							GameObject gameObject = Object.Instantiate(original, hqPlacedItemInfo.transform.position, hqPlacedItemInfo.transform.rotation) as GameObject;
							if (gameObject != null)
							{
								hqObject = Utils.GetComponent<HqObject2>(gameObject);
								if (hqObject == null)
								{
									return;
								}
								hqObject.InventoryId = value.Id;
								hqObject.PlacedId = GetNewPlacedId();
								gameObject.transform.parent = base.transform;
								placedItem = gameObject.AddComponent<PlacedItem>();
								if (placedItem != null)
								{
									placedItem.Initialize(hqObject.PlacedId, value, gameObject.transform.position, gameObject.transform.rotation, this);
								}
								list.Add(value.Id);
							}
						}
						if (placedItem != null)
						{
							if (placedItems.ContainsKey(hqObject.PlacedId))
							{
								CspUtils.DebugLog("Placed Items already contains " + hqObject.gameObject.name);
							}
							else
							{
								placedItems.Add(hqObject.PlacedId, placedItem);
								if (placedItem.IsUsableItem)
								{
									aiUsableItems.Add(placedItem);
								}
							}
						}
					}
				}
			}
		}
		AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(list.ToArray(), CollectionResetMessage.ActionType.Remove, "Items"));
		foreach (HqRoom2 value2 in HqController2.Instance.Rooms.Values)
		{
			HqAssignableRoom hqAssignableRoom = value2 as HqAssignableRoom;
			if (hqAssignableRoom != null && hqAssignableRoom.Owner == characterName)
			{
				hqAssignableRoom.UnAssignRooom();
			}
		}
		roomKeyValues["owner"] = characterName;
		Save();
	}

	public void SetPortrait(string characterName)
	{
		if (Portrait != null)
		{
			Portrait.AssignedCharacter = characterName;
			if (HqController2.Instance.ActiveRoom == this)
			{
				Utils.ActivateTree(Portrait.gameObject, true);
			}
		}
	}

	public void UnAssignRooom()
	{
		roomKeyValues.Remove("owner");
		if (Portrait != null)
		{
			Portrait.AssignedCharacter = null;
			Utils.ActivateTree(Portrait.gameObject, false);
		}
		ApplyTheme(HqController2.Instance.DefaultThemeName, true);
		ImplicitSave();
	}

	protected void ClearItems(Dictionary<int, HqItem> items)
	{
		List<string> list = new List<string>();
		foreach (HqItem value in items.Values)
		{
			HqAIProxy component = Utils.GetComponent<HqAIProxy>(value.gameObject);
			if (!(component != null))
			{
				Object.Destroy(value.gameObject);
				PlacedItem placedItem = value as PlacedItem;
				if (placedItem != null && placedItem.InventoryItem != null)
				{
					list.Add(placedItem.InventoryItem.Id);
				}
			}
		}
		AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(list.ToArray(), CollectionResetMessage.ActionType.Add, "Items"));
		items.Clear();
	}

	public override void LoadSaveData(string[] lines)
	{
		base.LoadSaveData(lines);
		if (Owner == null || HqController2.Instance.IsAllowableCharacter(Owner))
		{
			SetPortrait(Owner);
		}
	}

	public override void Activate()
	{
		base.Activate();
		if (Portrait != null)
		{
			if (Portrait.AssignedCharacter != null)
			{
				Utils.ActivateTree(Portrait.gameObject, true);
			}
			else
			{
				Utils.ActivateTree(Portrait.gameObject, false);
			}
		}
	}

	public override void Deactivate()
	{
		base.Deactivate();
		if (Portrait != null)
		{
			Utils.ActivateTree(Portrait.gameObject, false);
		}
	}

	public override void Reset()
	{
		base.Reset();
		UnAssignRooom();
	}
}
