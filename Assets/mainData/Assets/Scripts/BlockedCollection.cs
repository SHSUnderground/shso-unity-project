public class BlockedCollection : ShsCollectionBase<Friend>
{
	public delegate void SentInviteStatusChecked(bool isInSentList);

	protected const string ELEMENT_NAME = "ignore";

	protected const string KEY_NAME = "ignores";

	public BlockedCollection()
	{
		collectionElementName = "ignore";
		keyName = "ignores";
	}

	public BlockedCollection(DataWarehouse data)
		: this()
	{
		InitializeFromData(data);
	}

	public void OnBlockedLoaded(DataWarehouse data)
	{
		Clear();
		DataWarehouse data2 = data.GetData("//ignores");
		InitializeFromData(data2);
	}
}
