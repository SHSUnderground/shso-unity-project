using System.Collections.Generic;
using System.Linq;

public class ChallengeIOEventCounter : ChallengeEventCounter
{
	protected List<string> filters;

	public override void Initialize(ChallengeManager manager, ChallengeInfo info, ISHSCounterType counter, ChallengeManager.ChallengeCompleteDelegate onChallengeComplete)
	{
		base.Initialize(manager, info, counter, onChallengeComplete);
		filters = new List<string>();
		foreach (ChallengeInfoParameters parameter in info.Parameters)
		{
			if (parameter.key == "triggerFilter")
			{
				filters.Add(parameter.value);
			}
		}
	}

	protected override void OnClientChallengeEvent(object[] data)
	{
		if (data != null && data.Length != 0 && data[0] != null && data.Length > 0 && data[0] != null)
		{
			string text = data[0].ToString();
			if (Enumerable.Any(filters, text.StartsWith))
			{
				base.CounterValue++;
			}
		}
	}

	public override bool IsClientChallengeMet()
	{
		return base.CounterGoal <= challengeCounter.GetCurrentValue();
	}
}
