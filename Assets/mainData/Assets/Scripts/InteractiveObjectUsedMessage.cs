public class InteractiveObjectUsedMessage : ShsEventMessage
{
	public readonly InteractiveObject interactiveObject;

	public InteractiveObjectUsedMessage(InteractiveObject interactiveObject)
	{
		this.interactiveObject = interactiveObject;
	}
}
