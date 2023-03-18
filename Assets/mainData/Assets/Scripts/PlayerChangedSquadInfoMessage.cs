public class PlayerChangedSquadInfoMessage : ShsEventMessage
{
	public readonly int targetUserId;

	public readonly string title;

	public readonly string medallionSource;

	public PlayerChangedSquadInfoMessage(int netId, string newTitle, string newMedallionSource)
	{
		targetUserId = netId;
		title = newTitle;
		medallionSource = newMedallionSource;
	}
}
