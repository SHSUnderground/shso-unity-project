public class SHSBarEventArgs
{
	public enum SHSBarEventType
	{
		Start,
		Progress,
		End
	}

	public readonly float Min;

	public readonly float Max;

	public readonly float Value;

	public readonly float CurrentValue;

	public readonly SHSBarEventType EventType;

	public SHSBarEventArgs(SHSBarEventType EventType, float Min, float Max, float Value, float CurrentValue)
	{
		this.EventType = EventType;
		this.Min = Min;
		this.Max = Max;
		this.Value = Value;
		this.CurrentValue = CurrentValue;
	}
}
