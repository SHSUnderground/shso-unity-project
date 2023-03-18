using System.Collections.Generic;

public class ChallengeEventCollector : ChallengeCounter
{
	private HashSet<string> _eventCollection;

	public void AddEventToCollection(string evt)
	{
		if (_eventCollection.Add(evt))
		{
			base.CounterValue++;
		}
	}

	protected override void OnClientChallengeEvent(object[] data)
	{
		if (data == null || data.Length <= 0)
		{
			return;
		}
		foreach (object obj in data)
		{
			if (obj != null && !IsClientChallengeMet())
			{
				AddEventToCollection(obj.ToString());
			}
		}
	}

	public override void Initialize(ChallengeManager manager, ChallengeInfo info, ISHSCounterType counter, ChallengeManager.ChallengeCompleteDelegate onChallengeComplete)
	{
		base.Initialize(manager, info, counter, onChallengeComplete);
		_eventCollection = new HashSet<string>();
	}

	public override void Dispose()
	{
		base.Dispose();
		_eventCollection = null;
	}

	protected override string GetProgressLogString()
	{
		string text = string.Empty;
		if (_eventCollection != null)
		{
			foreach (string item in _eventCollection)
			{
				text = text + "[" + item + "]";
			}
			return text;
		}
		return text;
	}
}
