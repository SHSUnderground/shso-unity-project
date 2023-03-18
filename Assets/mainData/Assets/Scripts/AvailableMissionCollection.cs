public class AvailableMissionCollection : ShsCollectionBase<AvailableMission>
{
	protected const string ELEMENT_NAME = "item";

	protected const string KEY_NAME = "type";

	public AvailableMissionCollection()
	{
		collectionElementName = "item";
		keyName = "type";
	}

	public AvailableMissionCollection(DataWarehouse xmlData)
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
