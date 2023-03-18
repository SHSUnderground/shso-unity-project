public class HQHeroPlacedMessage : ShsEventMessage
{
	public string key;

	public bool add = true;

	public HQHeroPlacedMessage(string key, bool add)
	{
		this.key = key;
		this.add = add;
	}
}
