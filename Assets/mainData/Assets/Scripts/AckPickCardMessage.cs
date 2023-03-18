public class AckPickCardMessage : ShsEventMessage
{
	public readonly int PlayerId;

	public readonly int SourceId;

	public readonly int DestId;

	public readonly int CardIndex;

	public readonly string CardId;

	public readonly int Facing;

	public AckPickCardMessage(int ActingPlayerId, int SourceStackId, int DestStackId, int PickedIndex, string PickedCard, int FinalFacing)
	{
		PlayerId = ActingPlayerId;
		SourceId = SourceStackId;
		DestId = DestStackId;
		CardIndex = PickedIndex;
		CardId = PickedCard;
		Facing = FinalFacing;
	}
}
