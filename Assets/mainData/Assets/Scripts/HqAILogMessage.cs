public class HqAILogMessage : ShsEventMessage
{
	public string Character;

	public string LogChannel;

	public HqAILogMessage(string Character, string LogChannel)
	{
		this.Character = Character;
		this.LogChannel = LogChannel;
	}
}
