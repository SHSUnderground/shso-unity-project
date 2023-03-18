using System;

public class MysteryBox : ShsCollectionItem
{
	private string type;

	private int quantity;

	private OwnableDefinition definition;

	public OwnableDefinition Definition
	{
		get
		{
			if (definition == null)
			{
				definition = OwnableDefinition.getDef(int.Parse(type));
			}
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
				AppShell.Instance.EventMgr.Fire(this, new InventoryCollectionUpdateMessage(type));
			}
		}
	}

	public MysteryBox()
	{
	}

	public MysteryBox(DataWarehouse xmlData)
	{
		InitializeFromData(xmlData);
	}

	public override bool InitializeFromData(DataWarehouse data)
	{
		base.InitializeFromData(data);
		type = data.TryGetString("type", null);
		quantity = data.TryGetInt("quantity", -1);
		if (type == null)
		{
			CspUtils.DebugLog(string.Format("No item type specified for item <type=null, quantity={0}, >.", quantity));
			return false;
		}
		return true;
	}

	public override void UpdateFromData(DataWarehouse data)
	{
		throw new NotImplementedException();
	}

	public override string GetKey()
	{
		return type;
	}
}
