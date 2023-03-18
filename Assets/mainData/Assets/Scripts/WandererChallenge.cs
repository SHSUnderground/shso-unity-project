public class WandererChallenge : ChallengeEventCollector
{
	public override void Initialize(ChallengeManager manager, ChallengeInfo info, ISHSCounterType counter, ChallengeManager.ChallengeCompleteDelegate onChallengeComplete)
	{
		base.Initialize(manager, info, counter, onChallengeComplete);
		if (SocialSpaceController.Instance != null && !string.IsNullOrEmpty(SocialSpaceController.Instance.ZoneName))
		{
			AddEventToCollection(SocialSpaceController.Instance.ZoneName.ToLower());
		}
	}
}
