public class ChallengeCelebrationHideMessage : ShsEventMessage
{
	public readonly bool ShowMySquad = true;

	public ChallengeCelebrationHideMessage()
	{
		ShowMySquad = true;
	}

	public ChallengeCelebrationHideMessage(bool ShowMySquad)
	{
		this.ShowMySquad = ShowMySquad;
	}
}
