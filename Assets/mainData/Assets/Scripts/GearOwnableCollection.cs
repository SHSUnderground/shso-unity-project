public class GearOwnableCollection : ShsCollectionBase<GearCollectionItem>
{
	protected const string ELEMENT_NAME = "item";

	protected const string KEY_NAME = "type";

	public GearOwnableCollection()
	{
		collectionElementName = "item";
		keyName = "type";
	}

	public GearOwnableCollection(DataWarehouse xmlData)
		: this()
	{
		DataWarehouse data = xmlData.GetData("//inventory");
		InitializeFromData(data);
		if (Count > 0)
		{
			CardManager.LoadTextureBundle(true);
		}
	}

	protected override string GetKeyFromItemData(DataWarehouse itemData)
	{
		return itemData.GetString(".", keyName);
	}
}
