public class ChallengeServer : ChallengeBase
{
	public ChallengeServer()
	{
		challengeValidationSource = ChallengeValidationEnum.Server;
	}

	public override void HandleChallengeEvent(ChallengeEventMessage msg)
	{
	}
}
