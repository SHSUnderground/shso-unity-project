public class PlayerTargetedMessage : ShsEventMessage
{
	public readonly int targetUserId;

	public readonly string targetUserName;

	public PlayerTargetedMessage(int netId, string userName)
	{
		targetUserId = netId;
		targetUserName = userName;
	}
}
