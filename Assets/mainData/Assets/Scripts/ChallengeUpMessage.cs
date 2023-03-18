public class ChallengeUpMessage : ShsEventMessage
{
	public int ChallengeId;

	public ChallengeUpMessage(int challengeId)
	{
		ChallengeId = challengeId;
	}
}
