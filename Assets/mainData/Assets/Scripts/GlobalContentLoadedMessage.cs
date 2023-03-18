internal class GlobalContentLoadedMessage : ShsEventMessage
{
	public bool ForceComplete;

	public GlobalContentLoadedMessage()
		: this(false)
	{
	}

	public GlobalContentLoadedMessage(bool ForceComplete)
	{
		this.ForceComplete = ForceComplete;
	}
}
