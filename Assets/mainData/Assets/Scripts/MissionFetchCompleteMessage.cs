public class MissionFetchCompleteMessage : ShsEventMessage
{
	public bool success;

	public MissionFetchCompleteMessage(bool success)
	{
		this.success = success;
	}
}
