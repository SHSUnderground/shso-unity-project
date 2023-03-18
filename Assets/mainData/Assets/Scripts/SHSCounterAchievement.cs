using System;
using System.Xml.Serialization;

[XmlRoot(ElementName = "achievement")]
public class SHSCounterAchievement : Achievement
{
	public override void Initialize(object initData)
	{
		base.Initialize(initData);
		if (Manager != null)
		{
			Manager.RegisterCounterNotification(counters);
		}
	}

	public override void ProcessSourceUpdate(object sourceData)
	{
		base.ProcessSourceUpdate(sourceData);
		if (!(sourceData is CounterUpdatedMessage))
		{
			CspUtils.DebugLog("Processing counter update, but sourceData passed in isn't a counter update message");
			return;
		}
		CounterUpdatedMessage counterUpdatedMessage = (CounterUpdatedMessage)sourceData;
		if (counterUpdatedMessage.Path.ToUpper() == counters[0].ToUpper())
		{
			OnSourceUpdated(sourceData);
		}
	}

	public override void OnSourceUpdated(object sourceData)
	{
		base.OnSourceUpdated(sourceData);
		if (!(sourceData is CounterUpdatedMessage))
		{
			CspUtils.DebugLog("Processing counter update, but sourceData passed in isn't a counter update message");
			return;
		}
		CounterUpdatedMessage counterUpdatedMessage = (CounterUpdatedMessage)sourceData;
		AchievementLevelEnum achievementLevelEnum = EvaluateCounterValue(counterUpdatedMessage.PreviousValue, counterUpdatedMessage.Counter);
		AchievementLevelEnum achievementLevelEnum2 = EvaluateCounterValue(counterUpdatedMessage.NewValue, counterUpdatedMessage.Counter);
		if (AchievementUpTest(achievementLevelEnum, achievementLevelEnum2))
		{
			if (counterUpdatedMessage.Key.Equals("BESTTHEREISCOUNTER"))
			{
				AppShell.Instance.EventReporter.ReportEnemyDefeatedAllUpdate();
			}
			CspUtils.DebugLog("ACHIEVEMENT FOR: " + name + " from " + achievementLevelEnum + " to " + achievementLevelEnum2);
		}
	}

	public virtual AchievementLevelEnum EvaluateCounterValue(long value, ISHSCounterType counter)
	{
		AchievementLevelEnum result = AchievementLevelEnum.NotAchieved;
		if (thresholds.Length > 0 && thresholds[thresholds.Length - 1] < value)
		{
			return AchievementLevelEnum.Adamantium;
		}
		Array values = Enum.GetValues(typeof(AchievementLevelEnum));
		for (int i = 0; i < values.Length; i++)
		{
			if (thresholds.Length > i)
			{
				if (thresholds[i] > value)
				{
					break;
				}
				result = (AchievementLevelEnum)(int)values.GetValue(i);
			}
		}
		return result;
	}

	public override AchievementLevelEnum GetLevel(string heroKey, object bank)
	{
		if (bank is SHSCounterBank)
		{
			return getLevel(heroKey, (SHSCounterBank)bank);
		}
		throw new NotSupportedException("Only a CounterBank may be passed into this method as its second parameter");
	}

	public override AchievementLevelEnum GetLevel(string heroKey)
	{
		return getLevel(heroKey, AppShell.Instance.Profile.CounterBank);
	}

	private AchievementLevelEnum getLevel(string heroKey, SHSCounterBank bank)
	{
		if (counters.Length == 0)
		{
			CspUtils.DebugLog("No counter specified for this counter based achievement.");
			return AchievementLevelEnum.Unknown;
		}
		ISHSCounterType counter = AppShell.Instance.CounterManager.GetCounter(counters[0]);
		if (counter == null)
		{
			return AchievementLevelEnum.Unknown;
		}
		return EvaluateCounterValue(counter.GetCurrentValue(bank, heroKey), counter);
	}

	public override long GetCharacterLevelValue(string heroKey, object bank)
	{
		if (bank is SHSCounterBank)
		{
			return getCharacterLevelValue(heroKey, (SHSCounterBank)bank);
		}
		throw new NotSupportedException("Only a CounterBank may be passed into this method as its second parameter");
	}

	public override long GetCharacterLevelValue(string qualifierKey)
	{
		return GetCharacterLevelValue(qualifierKey, AppShell.Instance.Profile.CounterBank);
	}

	private long getCharacterLevelValue(string qualifierKey, SHSCounterBank bank)
	{
		if (counters.Length == 0)
		{
			CspUtils.DebugLog("No counter specified for this counter based achievement.");
			return -1L;
		}
		ISHSCounterType counter = AppShell.Instance.CounterManager.GetCounter(counters[0]);
		if (counter == null)
		{
			return -1L;
		}
		return counter.GetCurrentValue(bank, qualifierKey);
	}

	public override void DebugSetSourceValue(string qualifierKey, long value)
	{
		base.DebugSetSourceValue(qualifierKey, value);
		if (counters.Length == 0)
		{
			CspUtils.DebugLog("No counter specified for this counter based achievement.");
			return;
		}
		ISHSCounterType counter = AppShell.Instance.CounterManager.GetCounter(counters[0]);
		if (counter != null)
		{
			counter.SetCounter(qualifierKey, value);
		}
	}

	public override string ToString()
	{
		string text = id + " (" + name + "," + description + ")" + Environment.NewLine;
		string text2 = text;
		text = text2 + type.ToString() + "," + source.ToString() + Environment.NewLine;
		text = text + "Thresholds: " + Environment.NewLine;
		int[] thresholds = base.thresholds;
		foreach (int num in thresholds)
		{
			text2 = text;
			text = text2 + "  " + num + Environment.NewLine;
		}
		text = text + "Counters: " + Environment.NewLine;
		string[] counters = base.counters;
		foreach (string str in counters)
		{
			text = text + "  " + str + Environment.NewLine;
		}
		return text;
	}
}
