public class AckTransferCardsMessage : ShsEventMessage
{
	public readonly int PlayerId;

	public readonly int SourceId;

	public readonly int DestId;

	public readonly int NumCards;

	public readonly string[] Cards;

	public readonly bool Individually;

	public readonly int Facing;

	public AckTransferCardsMessage(int ActingPlayerId, int SourceStackId, int DestStackId, int NumberOfCards, string[] ArrayOfCards, bool TransferredIndividually, int FinalFacing)
	{
		PlayerId = ActingPlayerId;
		SourceId = SourceStackId;
		DestId = DestStackId;
		NumCards = NumberOfCards;
		Cards = ArrayOfCards;
		Individually = TransferredIndividually;
		Facing = FinalFacing;
	}
}
