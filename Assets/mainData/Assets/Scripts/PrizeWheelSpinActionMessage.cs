public class PrizeWheelSpinActionMessage : ShsEventMessage
{
	public enum PrizeWheelSpinActionEnum
	{
		Started,
		Stopped,
		Rewarded,
		Aborted
	}

	public PrizeWheelSpinActionEnum action;

	public PrizeWheelSpinActionMessage(PrizeWheelSpinActionEnum action)
	{
		this.action = action;
	}
}
