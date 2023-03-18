public class AvailableHQRoomsCollection : ShsCollectionBase<AvailableHQRoom>
{
	protected const string ELEMENT_NAME = "item";

	protected const string KEY_NAME = "type";

	public AvailableHQRoomsCollection()
	{
		collectionElementName = "item";
		keyName = "type";
	}

	public AvailableHQRoomsCollection(DataWarehouse xmlData)
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
