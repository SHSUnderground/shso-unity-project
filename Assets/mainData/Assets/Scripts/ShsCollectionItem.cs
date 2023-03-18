public abstract class ShsCollectionItem
{
	protected bool shieldAgentOnly;

	public virtual bool ShieldAgentOnly
	{
		get
		{
			return shieldAgentOnly;
		}
	}

	public ShsCollectionItem()
	{
	}

	public ShsCollectionItem(DataWarehouse data)
	{
		InitializeFromData(data);
	}

	public abstract string GetKey();

	public virtual bool InitializeFromData(DataWarehouse data)
	{
		if (data.TryGetInt("subscriber_only", 0) == 1)
		{
			shieldAgentOnly = true;
		}
		return true;
	}

	public abstract void UpdateFromData(DataWarehouse data);
}
