public class PetCollection : ShsCollectionBase<GenericCollectionItem>
{
	protected const string ELEMENT_NAME = "item";

	protected const string KEY_NAME = "type";

	public PetCollection()
	{
		collectionElementName = "item";
		keyName = "type";
	}

	public PetCollection(DataWarehouse xmlData)
		: this()
	{
		DataWarehouse data = xmlData.GetData("//inventory");
		InitializeFromData(data);
		foreach (GenericCollectionItem value in base.Values)
		{
			PetDataManager.purchasedPetByID(int.Parse(value.GetKey()));
		}
	}

	protected override string GetKeyFromItemData(DataWarehouse itemData)
	{
		return itemData.GetString(".", keyName);
	}
}
