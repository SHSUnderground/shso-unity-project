public class HeroFetchCompleteMessage : ShsEventMessage
{
	public bool success;

	public HeroFetchCompleteMessage(bool success)
	{
		this.success = success;
	}
}
