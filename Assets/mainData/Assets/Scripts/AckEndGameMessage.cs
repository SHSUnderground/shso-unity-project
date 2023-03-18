public class AckEndGameMessage : ShsEventMessage
{
	public readonly int WinnerId;

	public AckEndGameMessage(int WinningPlayerId)
	{
		WinnerId = WinningPlayerId;
	}
}
