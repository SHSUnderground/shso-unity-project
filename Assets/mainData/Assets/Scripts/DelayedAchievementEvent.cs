using UnityEngine;

public class DelayedAchievementEvent
{
	private float endTime;

	private string heroStr = string.Empty;

	private string type = string.Empty;

	private string subtype = string.Empty;

	private string str1 = string.Empty;

	public DelayedAchievementEvent(string heroStr, string type, string subtype, string str1, float delay = 3f)
	{
		this.heroStr = heroStr;
		this.type = type;
		this.subtype = subtype;
		this.str1 = str1;
		endTime = Time.time + delay;
	}

	public bool tick(float time)
	{
		if (time > endTime)
		{
			AppShell.Instance.EventReporter.ReportAchievementEvent(heroStr, type, subtype, 1, str1);
			return true;
		}
		return false;
	}
}
