public class BattleInviteMessage : ShsEventMessage
{
	public readonly string opponentName;

	public readonly int playerId;

	public BattleInviteMessage(int PlayerId)
	{
		playerId = PlayerId;
		opponentName = string.Empty;
	}

	public BattleInviteMessage(string OpponentName)
	{
		playerId = -1;
		opponentName = OpponentName;
	}
}
