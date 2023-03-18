public class Item : ShsCollectionItem
{
	private string type;

	private int quantity;

	private int placed;

	private ItemDefinition definition;

	public string Name
	{
		get
		{
			return definition.Name;
		}
	}

	public string Type
	{
		get
		{
			return definition.Id;
		}
	}

	public string Id
	{
		get
		{
			return type;
		}
	}

	public string Description
	{
		get
		{
			return definition.Description;
		}
	}

	public ItemDefinition Definition
	{
		get
		{
			return definition;
		}
	}

	public int Quantity
	{
		get
		{
			return quantity;
		}
		set
		{
			quantity = value;
			if (AppShell.Instance != null)
			{
				AppShell.Instance.EventMgr.Fire(this, new InventoryCollectionUpdateMessage(Id));
			}
		}
	}

	public int Placed
	{
		get
		{
			return placed;
		}
		set
		{
			placed = value;
			if (AppShell.Instance != null)
			{
				AppShell.Instance.EventMgr.Fire(this, new InventoryCollectionUpdateMessage(Id));
			}
		}
	}

	public Item()
	{
	}

	public Item(DataWarehouse xmlData)
	{
		InitializeFromData(xmlData);
	}

	public override bool InitializeFromData(DataWarehouse data)
	{
		base.InitializeFromData(data);
		type = data.TryGetString("type", null);
		quantity = data.TryGetInt("quantity", -1);
		placed = 0;
		if (type == null)
		{
			CspUtils.DebugLog(string.Format("No item type specified for item <type=null, quantity={0}, # placed={1}>.", quantity, placed));
			return false;
		}
		definition = null;
		AppShell.Instance.ItemDictionary.TryGetValue(type, out definition);
		if (definition == null)
		{
			CspUtils.DebugLog(string.Format("No item definition found for item <type id={0}, quantity={1}, # placed={2}>.", type, quantity, placed));
			return false;
		}
		return true;
	}

	public override void UpdateFromData(DataWarehouse data)
	{
		CspUtils.DebugLog("Needs Implementation!");
	}

	public override string GetKey()
	{
		return type;
	}
}
