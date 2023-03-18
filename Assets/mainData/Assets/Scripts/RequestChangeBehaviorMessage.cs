public class RequestChangeBehaviorMessage : ShsEventMessage
{
	private int playerId;

	public int PlayerId
	{
		get
		{
			return playerId;
		}
	}

	public RequestChangeBehaviorMessage(int playerId)
	{
		this.playerId = playerId;
	}
}
