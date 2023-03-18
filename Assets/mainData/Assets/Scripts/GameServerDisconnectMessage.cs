public class GameServerDisconnectMessage : ServerDisconnectMessage
{
	public GameServerDisconnectMessage(NetworkManager.ConnectionState state)
		: base(state)
	{
	}
}
