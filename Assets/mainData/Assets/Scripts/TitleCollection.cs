public class TitleCollection : ShsCollectionBase<GenericCollectionItem>
{
	protected const string ELEMENT_NAME = "item";

	protected const string KEY_NAME = "type";

	public TitleCollection()
	{
		collectionElementName = "item";
		keyName = "type";
	}

	public TitleCollection(DataWarehouse xmlData)
		: this()
	{
		DataWarehouse data = xmlData.GetData("//inventory");
		InitializeFromData(data);
		foreach (GenericCollectionItem value in base.Values)
		{
			TitleManager.awardTitle(int.Parse(value.GetKey()), false);
		}
	}

	protected override string GetKeyFromItemData(DataWarehouse itemData)
	{
		return itemData.GetString(".", keyName);
	}
}
