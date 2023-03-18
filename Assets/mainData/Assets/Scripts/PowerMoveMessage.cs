internal class PowerMoveMessage : ShsEventMessage
{
	public readonly int PowerMoveId;

	public PowerMoveMessage(int PowerMoveId)
	{
		this.PowerMoveId = PowerMoveId;
	}
}
