public class PotionFetchCompleteMessage : ShsEventMessage
{
	public bool success;

	public PotionFetchCompleteMessage(bool success)
	{
		this.success = success;
	}
}
