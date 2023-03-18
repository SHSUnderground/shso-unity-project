public class HeroXPUpdateMessage : ShsEventMessage
{
	public string heroName;

	public HeroXPUpdateMessage(string heroName)
	{
		this.heroName = heroName;
	}
}
