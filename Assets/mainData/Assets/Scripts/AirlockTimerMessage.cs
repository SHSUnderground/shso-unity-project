public class AirlockTimerMessage : ShsEventMessage
{
	public enum AirlockTimerType
	{
		Start,
		Warning,
		Complete
	}

	public readonly AirlockTimerType Type;

	public AirlockTimerMessage(AirlockTimerType Type)
	{
		this.Type = Type;
	}
}
