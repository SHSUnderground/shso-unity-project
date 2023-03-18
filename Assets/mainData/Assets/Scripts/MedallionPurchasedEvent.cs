public class MedallionPurchasedEvent : ShsEventMessage
{
	public int id;

	public MedallionPurchasedEvent(int id)
	{
		this.id = id;
	}
}
