public class AckPassMessage : ShsEventMessage
{
	public readonly int PlayerId;

	public AckPassMessage(int PassingPlayerId)
	{
		PlayerId = PassingPlayerId;
	}
}
