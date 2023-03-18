public class TitlePurchasedEvent : ShsEventMessage
{
	public int id;

	public TitlePurchasedEvent(int id)
	{
		this.id = id;
	}
}
