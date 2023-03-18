public class AvailableBoosterPack : ShsCollectionItem
{
	protected string boosterPackId;

	protected int quantity;

	public string BoosterPackId
	{
		get
		{
			return boosterPackId;
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
		}
	}

	public AvailableBoosterPack()
	{
	}

	public AvailableBoosterPack(DataWarehouse xmlData)
	{
		InitializeFromData(xmlData);
	}

	public override bool InitializeFromData(DataWarehouse data)
	{
		base.InitializeFromData(data);
		boosterPackId = data.TryGetString("type", null);
		quantity = data.TryGetInt("quantity", -1);
		if (quantity == -1)
		{
			quantity = data.TryGetInt("stack_size", 1);
		}
		return true;
	}

	public override void UpdateFromData(DataWarehouse data)
	{
		CspUtils.DebugLog("Needs Implementation!");
	}

	public override string GetKey()
	{
		return boosterPackId;
	}
}
