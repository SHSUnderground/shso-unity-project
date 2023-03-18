public class ChallengeEventCounter : ChallengeCounter
{
	private string _eventTrigger;

	public string EventTrigger
	{
		get
		{
			return _eventTrigger;
		}
	}

	public bool IsTrackedEvent(string evt)
	{
		return evt != null && evt == EventTrigger;
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
		}
	}

	protected override void OnClientChallengeEvent(object[] data)
	{
		if (data != null && data.Length != 0 && data[0] != null && IsTrackedEvent(data[0].ToString()))
		{
			long num = 1L;
			if (data.Length > 1 && data[1] != null)
			{
				num = long.Parse(data[1].ToString());
			}
			base.CounterValue += num;
		}
	}
}
