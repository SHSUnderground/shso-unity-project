public class ItemConsumedMessage : ShsEventMessage
{
	public string key;

	public int numConsumed;

	public ItemConsumedMessage(string key, int numConsumed)
	{
		this.key = key;
		this.numConsumed = numConsumed;
	}
}
