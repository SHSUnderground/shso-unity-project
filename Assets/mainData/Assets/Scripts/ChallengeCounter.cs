public class ChallengeCounter : ChallengeClient
{
	private long _counterGoal;

	private long _counterSimple;

	private bool _useCounterSimple;

	public long CounterValue
	{
		get
		{
			return (!_useCounterSimple) ? challengeCounter.GetCurrentValue() : _counterSimple;
		}
		set
		{
			if (_useCounterSimple)
			{
				_counterSimple = value;
			}
			else
			{
				challengeCounter.SetCounter(value, SHSCounterType.ReportingMethodEnum.WebService);
			}
			LogChallengeStatus();
			if (IsClientChallengeMet())
			{
				NotifyOnClientChallengeMet();
			}
		}
	}

	public long CounterGoal
	{
		get
		{
			return _counterGoal;
		}
	}

	public override bool IsClientChallengeMet()
	{
		return CounterValue >= CounterGoal;
	}

	protected override void OnClientChallengeEvent(object[] data)
	{
		if (data != null)
		{
			if (data.Length > 0 && data[0] != null)
			{
				CounterValue += long.Parse(data[0].ToString());
			}
			else
			{
				CounterValue++;
			}
		}
	}

	public override void Initialize(ChallengeManager manager, ChallengeInfo info, ISHSCounterType counter, ChallengeManager.ChallengeCompleteDelegate onChallengeComplete)
	{
		base.Initialize(manager, info, counter, onChallengeComplete);
		_useCounterSimple = false;
		foreach (ChallengeInfoParameters parameter in info.Parameters)
		{
			if (parameter.key == "counter_goal")
			{
				_counterGoal = long.Parse(parameter.value);
			}
			if (parameter.key == "counter_simple")
			{
				_useCounterSimple = bool.Parse(parameter.value);
			}
		}
	}

	protected override string GetProgressLogString()
	{
		return CounterValue.ToString() + "/" + CounterGoal.ToString();
	}
}
