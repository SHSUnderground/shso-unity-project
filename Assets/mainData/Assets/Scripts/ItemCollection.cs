using System.Collections.Generic;

public class ItemCollection : ShsCollectionBase<Item>
{
	private const string ELEMENT_NAME = "item";

	private const string KEY_NAME = "type";

	public ItemCollection()
	{
		collectionElementName = "item";
		keyName = "type";
		AppShell.Instance.EventMgr.AddListener<CollectionResetMessage>(OnItemsReset);
	}

	public ItemCollection(DataWarehouse data)
		: this()
	{
		InitializeFromData(data);
	}

	~ItemCollection()
	{
		AppShell.Instance.EventMgr.RemoveListener<CollectionResetMessage>(OnItemsReset);
	}

	public void OnInventoryLoaded(ShsWebResponse response)
	{
		if (response.Status != 200)
		{
			CspUtils.DebugLog("Inventory request failed for <" + response.RequestUri + "> with status " + response.Status);
			return;
		}
		Clear();
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		DataWarehouse data = dataWarehouse.GetData("//inventory");
		InitializeFromData(data);
	}

	public void ResetPlacedValues()
	{
		HqController2 instance = HqController2.Instance;
		if (instance != null)
		{
			foreach (Item value in base.Values)
			{
				value.Placed = instance.PlacedCount(value);
			}
		}
	}

	private void resetItems(string[] keys, bool add)
	{
		foreach (string b in keys)
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, Item> current = enumerator.Current;
					if (current.Value.Id == b)
					{
						if (add)
						{
							current.Value.Placed--;
						}
						else
						{
							current.Value.Placed++;
						}
						break;
					}
				}
			}
		}
	}

	private void OnItemsReset(CollectionResetMessage message)
	{
		if (message.collectionId == "Items")
		{
			resetItems(message.keys, message.actionType == CollectionResetMessage.ActionType.Add);
		}
	}
}
