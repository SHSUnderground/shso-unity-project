public class WelcomeResponseMessage : ShsEventMessage
{
	public readonly bool FullScreenRequested;

	public IGUIControl ControlRef;

	public WelcomeResponseMessage(bool fullScreen, IGUIControl controlRef)
	{
		FullScreenRequested = fullScreen;
		ControlRef = controlRef;
	}
}
