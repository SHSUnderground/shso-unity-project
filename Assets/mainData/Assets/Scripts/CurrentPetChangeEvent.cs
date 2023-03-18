public class CurrentPetChangeEvent : ShsEventMessage
{
	public int id;

	public CurrentPetChangeEvent(int id)
	{
		this.id = id;
	}
}
