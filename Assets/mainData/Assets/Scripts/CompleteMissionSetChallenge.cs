using UnityEngine;

public class CompleteMissionSetChallenge : ChallengeBitCounter
{
	private string _eventTrigger;

	public bool IsTrackedEvent(string evt)
	{
		return evt == _eventTrigger;
	}

	protected override void OnClientChallengeEvent(object[] data)
	{
		CspUtils.DebugLogWarning("CompleteMissionSetChallenge.OnClientChallengeEvent data[0]:" + data[0].ToString());
		if (data != null && data.Length != 0 && data[0] != null && IsTrackedEvent(data[0].ToString()) && data.Length > 1 && data[1] != null)
		{
			CspUtils.DebugLogWarning("CompleteMissionSetChallenge.OnClientChallengeEvent data[1]:" + data[1].ToString());
			if (!IsBitSet(data[1]))
			{
				CspUtils.DebugLogWarning("CompleteMissionSetChallenge - Setting bit for " + data[1]);
				SetBit(data[1]);
				CspUtils.DebugLogWarning("CompleteMissionSetChallenge - All bits set? " + AreAllBitsSet());
			}
		}
	}

	public override void Initialize(ChallengeManager manager, ChallengeInfo info, ISHSCounterType counter, ChallengeManager.ChallengeCompleteDelegate onChallengeComplete)
	{
		base.Initialize(manager, info, counter, onChallengeComplete);
		foreach (ChallengeInfoParameters parameter in info.Parameters)
		{
			if (parameter.key == "event_trigger")
			{
				_eventTrigger = parameter.value;
			}
			if (parameter.key == "mission_name")
			{
				AssignBit(parameter.value);
			}
		}
	}
}
