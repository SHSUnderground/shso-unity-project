public class NotificationServerDisconnectMessage : ServerDisconnectMessage
{
	public NotificationServerDisconnectMessage(NetworkManager.ConnectionState state)
		: base(state)
	{
	}
}
