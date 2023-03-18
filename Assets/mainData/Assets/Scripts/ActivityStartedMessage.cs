public class ActivityStartedMessage : ShsEventMessage
{
	public ISHSActivity activity;

	public ActivityStartedMessage(ISHSActivity activity)
	{
		this.activity = activity;
	}
}
