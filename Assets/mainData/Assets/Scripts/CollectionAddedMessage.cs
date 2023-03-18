public class CollectionAddedMessage : ShsEventMessage
{
	public string key;

	public CollectionAddedMessage(string key)
	{
		this.key = key;
	}
}
