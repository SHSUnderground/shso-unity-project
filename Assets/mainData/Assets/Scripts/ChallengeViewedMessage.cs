public class ChallengeViewedMessage : ShsEventMessage
{
	public int ChallengeId;

	public ChallengeViewedMessage(int challengeId)
	{
		ChallengeId = challengeId;
	}
}
