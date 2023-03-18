public class ScenarioEventScoreMessage : ShsEventMessage
{
	public int eventScore;

	public CombatController eventScoreTarget;

	public ScenarioEventScoreMessage(int scoreForEvent)
	{
		eventScore = scoreForEvent;
	}

	public ScenarioEventScoreMessage(int scoreForEvent, CombatController scoreOnTarget)
	{
		eventScore = scoreForEvent;
		eventScoreTarget = scoreOnTarget;
	}
}
