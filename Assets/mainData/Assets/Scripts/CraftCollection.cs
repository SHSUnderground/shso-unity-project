public class CraftCollection : ShsCollectionBase<GenericCollectionItem>
{
	protected const string ELEMENT_NAME = "item";

	protected const string KEY_NAME = "type";

	public CraftCollection()
	{
		collectionElementName = "item";
		keyName = "type";
	}

	public CraftCollection(DataWarehouse xmlData)
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
