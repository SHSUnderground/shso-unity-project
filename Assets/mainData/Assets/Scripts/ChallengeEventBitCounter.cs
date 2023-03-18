public class ChallengeEventBitCounter : ChallengeBitCounter
{
	public override void Initialize(ChallengeManager manager, ChallengeInfo info, ISHSCounterType counter, ChallengeManager.ChallengeCompleteDelegate onChallengeComplete)
	{
		base.Initialize(manager, info, counter, onChallengeComplete);
		foreach (ChallengeInfoParameters parameter in info.Parameters)
		{
			if (parameter.key == "event_trigger")
			{
				object[] objArray = parameter.value.Split('|');
				AssignBit(objArray);
			}
		}
	}
}
