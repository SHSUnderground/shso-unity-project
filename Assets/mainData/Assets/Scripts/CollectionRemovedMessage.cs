public class CollectionRemovedMessage : ShsEventMessage
{
	public string key;

	public CollectionRemovedMessage(string key)
	{
		this.key = key;
	}
}
