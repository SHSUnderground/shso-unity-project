public class BundleCollection : ShsCollectionBase<Bundle>
{
	protected const string ELEMENT_NAME = "item";

	protected const string KEY_NAME = "type";

	public BundleCollection()
	{
		collectionElementName = "item";
		keyName = "type";
	}

	public BundleCollection(DataWarehouse xmlData)
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
