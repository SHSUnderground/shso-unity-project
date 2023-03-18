public class AvailableQuestCollection : ShsCollectionBase<AvailableQuest>
{
	protected const string ELEMENT_NAME = "item";

	protected const string KEY_NAME = "type";

	public AvailableQuestCollection()
	{
		collectionElementName = "item";
		keyName = "type";
	}

	public AvailableQuestCollection(DataWarehouse xmlData)
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
