public class AckFakePlayerMessage : ShsEventMessage
{
	public readonly int FakePlayerId;

	public AckFakePlayerMessage(int PlayerId)
	{
		FakePlayerId = PlayerId;
	}
}
