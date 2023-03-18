public class MedallionCollection : ShsCollectionBase<GenericCollectionItem>
{
	protected const string ELEMENT_NAME = "item";

	protected const string KEY_NAME = "type";

	public MedallionCollection()
	{
		collectionElementName = "item";
		keyName = "type";
	}

	public MedallionCollection(DataWarehouse xmlData)
		: this()
	{
		DataWarehouse data = xmlData.GetData("//inventory");
		InitializeFromData(data);
		foreach (GenericCollectionItem value in base.Values)
		{
			TitleManager.awardMedallion(int.Parse(value.GetKey()), false);
		}
	}

	protected override string GetKeyFromItemData(DataWarehouse itemData)
	{
		return itemData.GetString(".", keyName);
	}
}
