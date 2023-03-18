using System;

public class Expendable : ShsCollectionItem
{
	private string type;

	private int quantity;

	private ExpendableDefinition definition;

	public ExpendableDefinition Definition
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
				AppShell.Instance.EventMgr.Fire(this, new InventoryCollectionUpdateMessage(type));
			}
		}
	}

	public Expendable()
	{
	}

	public Expendable(DataWarehouse xmlData)
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
		if (AppShell.Instance != null && AppShell.Instance.ExpendablesManager != null)
		{
			if (!AppShell.Instance.ExpendablesManager.ExpendableTypes.TryGetValue(type, out definition))
			{
				CspUtils.DebugLog("No definition for item: " + type);
			}
			return true;
		}
		CspUtils.DebugLog("No AppShell global.");
		return false;
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
