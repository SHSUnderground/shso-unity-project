public class HeroCollectionUpdateMessage : ShsEventMessage
{
	public string[] keys;

	public HeroCollectionUpdateMessage(params string[] keys)
	{
		this.keys = keys;
	}
}
