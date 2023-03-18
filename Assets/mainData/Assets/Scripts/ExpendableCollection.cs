public class ExpendableCollection : ShsCollectionBase<Expendable>
{
	protected const string ELEMENT_NAME = "item";

	protected const string KEY_NAME = "type";

	public ExpendableCollection()
	{
		collectionElementName = "item";
		keyName = "type";
	}

	public ExpendableCollection(DataWarehouse xmlData)
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
