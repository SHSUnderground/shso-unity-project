using System.Collections.Generic;

public class TargetedEmoteChallenge : ChallengeEventCounter
{
	protected List<int> playerIds;

	public override void Initialize(ChallengeManager manager, ChallengeInfo info, ISHSCounterType counter, ChallengeManager.ChallengeCompleteDelegate onChallengeComplete)
	{
		base.Initialize(manager, info, counter, onChallengeComplete);
		playerIds = new List<int>();
	}

	protected override void OnClientChallengeEvent(object[] data)
	{
		if (data != null && data.Length != 0 && data[0] != null && IsTrackedEvent(data[0].ToString()) && data.Length > 1 && data[1] != null)
		{
			int item = int.Parse(data[1].ToString());
			if (!playerIds.Contains(item))
			{
				playerIds.Add(item);
				base.CounterValue++;
			}
		}
	}
}
