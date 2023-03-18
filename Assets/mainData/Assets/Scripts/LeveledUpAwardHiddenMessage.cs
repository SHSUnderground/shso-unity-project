public class LeveledUpAwardHiddenMessage : ShsEventMessage
{
	public LeveledUpMessage message;

	public LeveledUpAwardHiddenMessage(LeveledUpMessage message)
	{
		this.message = message;
	}
}
