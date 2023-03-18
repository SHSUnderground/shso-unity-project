public class GlobalNavStateMessage : ShsEventMessage
{
	public bool open;

	public GlobalNavStateMessage(bool open)
	{
		this.open = open;
	}
}
