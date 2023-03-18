public class WindowStatusMessage : ShsEventMessage
{
	public bool isActive;

	public WindowStatusMessage(bool active)
	{
		isActive = active;
	}
}
