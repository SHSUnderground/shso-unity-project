public class CollectionResetMessage : ShsEventMessage
{
	public enum ActionType
	{
		Add,
		Remove
	}

	public string[] keys;

	public string collectionId;

	public ActionType actionType;

	public CollectionResetMessage(string[] keys, ActionType actionType, string collectionId)
	{
		this.keys = keys;
		this.collectionId = collectionId;
		this.actionType = actionType;
	}
}
