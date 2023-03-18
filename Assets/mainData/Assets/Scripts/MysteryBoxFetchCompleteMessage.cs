public class MysteryBoxFetchCompleteMessage : ShsEventMessage
{
	public bool success;

	public MysteryBoxFetchCompleteMessage(bool success)
	{
		this.success = success;
	}
}
