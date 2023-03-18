public class AckShuffleCardsMessage : ShsEventMessage
{
	public readonly int PlayerId;

	public readonly int SourceId;

	public AckShuffleCardsMessage(int ActingPlayerId, int SourceStackId)
	{
		PlayerId = ActingPlayerId;
		SourceId = SourceStackId;
	}
}
