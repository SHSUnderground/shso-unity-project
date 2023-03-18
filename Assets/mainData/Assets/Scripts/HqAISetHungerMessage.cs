public class HqAISetHungerMessage : ShsEventMessage
{
	public string Character;

	public float HungerValue;

	public HqAISetHungerMessage(string Character, float HungerValue)
	{
		this.Character = Character;
		this.HungerValue = HungerValue;
	}
}
