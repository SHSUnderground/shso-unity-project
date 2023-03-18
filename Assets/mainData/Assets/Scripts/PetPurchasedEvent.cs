public class PetPurchasedEvent : ShsEventMessage
{
	public int id;

	public PetPurchasedEvent(int id)
	{
		this.id = id;
	}
}
