using System.Collections.Generic;

public class ChallengeEvent : ChallengeClient
{
	protected Dictionary<string, bool> completedEventMap;

	public ChallengeEvent()
	{
		completedEventMap = new Dictionary<string, bool>();
	}

	public void CompleteEvent(string evt)
	{
		if (evt == null)
		{
			CspUtils.DebugLog("ChallengeEvent::CompleteEvent() - string event argument is null");
			return;
		}
		if (IsTrackedEvent(evt) && !IsCompletedEvent(evt))
		{
			completedEventMap[evt] = true;
			LogChallengeStatus();
		}
		if (IsClientChallengeMet())
		{
			NotifyOnClientChallengeMet();
		}
	}

	public bool IsTrackedEvent(string evt)
	{
		return evt != null && completedEventMap.ContainsKey(evt);
	}

	public bool IsCompletedEvent(string evt)
	{
		return IsTrackedEvent(evt) && completedEventMap[evt];
	}

	public override bool IsClientChallengeMet()
	{
		foreach (KeyValuePair<string, bool> item in completedEventMap)
		{
			if (!item.Value)
			{
				return false;
			}
		}
		return true;
	}

	protected override void OnClientChallengeEvent(object[] data)
	{
		if (data != null && data.Length > 0 && data[0] != null)
		{
			CompleteEvent(data[0].ToString());
		}
	}

	public override void Initialize(ChallengeManager manager, ChallengeInfo info, ISHSCounterType counter, ChallengeManager.ChallengeCompleteDelegate onChallengeComplete)
	{
		base.Initialize(manager, info, counter, onChallengeComplete);
		foreach (ChallengeInfoParameters parameter in info.Parameters)
		{
			if (parameter.key == "event_trigger")
			{
				completedEventMap.Add(parameter.value, false);
			}
		}
	}

	public override void Dispose()
	{
		base.Dispose();
		completedEventMap = null;
	}

	protected override string GetProgressLogString()
	{
		string text = string.Empty;
		foreach (KeyValuePair<string, bool> item in completedEventMap)
		{
			text += item.ToString();
		}
		return text;
	}
}
