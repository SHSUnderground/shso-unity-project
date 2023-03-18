public class AvailableBoosterPacksCollection : ShsCollectionBase<AvailableBoosterPack>
{
	protected const string ELEMENT_NAME = "item";

	protected const string KEY_NAME = "type";

	public AvailableBoosterPacksCollection()
	{
		collectionElementName = "item";
		keyName = "type";
	}

	public AvailableBoosterPacksCollection(DataWarehouse xmlData)
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
