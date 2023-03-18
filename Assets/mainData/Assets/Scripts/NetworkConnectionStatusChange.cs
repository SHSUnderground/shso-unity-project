public class NetworkConnectionStatusChange : ShsEventMessage
{
	public NetworkManager.ConnectionState prevState;

	public NetworkManager.ConnectionState state;

	public NetworkConnectionStatusChange(NetworkManager.ConnectionState state, NetworkManager.ConnectionState prevState)
	{
		this.state = state;
		this.prevState = prevState;
	}
}
