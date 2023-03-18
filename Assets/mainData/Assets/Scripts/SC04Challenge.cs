public class SC04Challenge : ChallengeCounter
{
	public const int STARTING_HERO_COUNT = 4;

	public override void InitializeCounterValue()
	{
		if (AppShell.Instance.Profile != null)
		{
			int num = AppShell.Instance.Profile.AvailableCostumes.Count;
			if (num < 4)
			{
				CspUtils.DebugLog("Hero count is lower than starting hero count. This should not be possible!");
				num = 4;
			}
			challengeCounter.SetCounter(num, SHSCounterType.ReportingMethodEnum.WebService);
		}
		else
		{
			CspUtils.DebugLog("No user profile, so no ability to set the initial counter for this challenge.");
		}
	}

	public override void Ready()
	{
		if (AppShell.Instance.Profile == null)
		{
			return;
		}
		int num = AppShell.Instance.Profile.AvailableCostumes.Count;
		if (num >= base.CounterGoal)
		{
			NotifyOnClientChallengeMet();
			return;
		}
		if (num < 4)
		{
			num = 4;
		}
		if (num != base.CounterValue)
		{
			base.CounterValue = num;
		}
	}
}
