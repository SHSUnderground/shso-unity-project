public class PrizeWheelSlotResultMessage : ShsEventMessage
{
	public PrizeWheelStop slotAwarded;

	public PrizeWheelSlotResultMessage(PrizeWheelStop slotAwarded)
	{
		this.slotAwarded = slotAwarded;
	}
}
