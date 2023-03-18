public class ActivityCompletedMessage : ShsEventMessage
{
	public ISHSActivity activity;

	public ActivityResultEnum state;

	public ActivityCompletedMessage(ISHSActivity activity, ActivityResultEnum state)
	{
		this.activity = activity;
		this.state = state;
	}
}
