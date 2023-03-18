public abstract class ServerDisconnectMessage : ShsEventMessage
{
	private readonly DisconnectReason reason;

	public DisconnectReason DisconnectReason
	{
		get
		{
			return reason;
		}
	}

	protected ServerDisconnectMessage(NetworkManager.ConnectionState state)
	{
		reason = (((state & NetworkManager.ConnectionState.DisconnectingFromGame) == 0) ? DisconnectReason.ConnectionLost : DisconnectReason.UserRequested);
	}
}
