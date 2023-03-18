public class GlobalNavButtonVisibleMessage : ShsEventMessage
{
	public GlobalNav.GlobalNavType type;

	public GlobalNavButtonVisibleMessage(GlobalNav.GlobalNavType type)
	{
		this.type = type;
	}
}
