public class BadgeCollection : ShsCollectionBase<Badge>
{
	protected const string ELEMENT_NAME = "item";

	protected const string KEY_NAME = "type";

	public BadgeCollection()
	{
		collectionElementName = "item";
		keyName = "type";
	}

	public BadgeCollection(DataWarehouse xmlData)
		: this()
	{
		DataWarehouse data = xmlData.GetData("//inventory");
		InitializeFromData(data);
	}

	protected override string GetKeyFromItemData(DataWarehouse itemData)
	{
		return itemData.GetString(".", keyName);
	}
}
