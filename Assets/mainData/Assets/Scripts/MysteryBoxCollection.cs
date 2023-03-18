public class MysteryBoxCollection : ShsCollectionBase<MysteryBox>
{
	protected const string ELEMENT_NAME = "item";

	protected const string KEY_NAME = "type";

	public MysteryBoxCollection()
	{
		collectionElementName = "item";
		keyName = "type";
	}

	public MysteryBoxCollection(DataWarehouse xmlData)
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
