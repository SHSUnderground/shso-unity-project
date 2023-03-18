public class AckGetFirstPlayerMessage : ShsEventMessage
{
	public readonly int firstPlayerId;

	public AckGetFirstPlayerMessage(int FirstPlayer)
	{
		firstPlayerId = FirstPlayer;
	}
}
