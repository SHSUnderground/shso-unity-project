using System;

public class CompleteMissionMultipleTimesChallenge : ChallengeEventCounter
{
	private string missionName;

	public override void Initialize(ChallengeManager manager, ChallengeInfo info, ISHSCounterType counter, ChallengeManager.ChallengeCompleteDelegate onChallengeComplete)
	{
		base.Initialize(manager, info, counter, onChallengeComplete);
		challengeValidationSource = info.Authority;
		foreach (ChallengeInfoParameters parameter in info.Parameters)
		{
			if (parameter.key == "mission_name")
			{
				missionName = parameter.value;
			}
		}
	}

	protected override void OnClientChallengeEvent(object[] data)
	{
		if (data != null && data.Length != 0 && data[0] != null && IsTrackedEvent(data[0].ToString()) && data.Length > 1 && data[1] != null && missionName.Equals(data[1].ToString(), StringComparison.OrdinalIgnoreCase))
		{
			base.CounterValue++;
		}
	}
}
