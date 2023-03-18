public class PrizeWheelLoadedMessage : ShsEventMessage
{
	public PrizeWheelManager prizeWheelData;

	public PrizeWheelLoadedMessage(PrizeWheelManager PrizeWheelData)
	{
		prizeWheelData = PrizeWheelData;
	}
}
