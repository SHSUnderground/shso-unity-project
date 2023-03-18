using System;
using UnityEngine;

public class CountersTriggerAdaptor : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string counterType;

	private string counterTypeCached;

	public int counters = 1;

	public int cooldown = 5;

	private DateTime lastTriggered;

	private string counterTypeParsed;

	public string CounterTypeParsed
	{
		get
		{
			if ((counterTypeParsed == null || counterType != counterTypeCached) && counterType.Contains("{zone}"))
			{
				string text = AppShell.Instance.SharedHashTable["SocialSpaceLevelCurrent"] as string;
				if (text == null)
				{
					CspUtils.DebugLog("Cant associate zone with this counter type. Faking it.");
					counterTypeParsed = "TROPHYCOUNTERS.Daily_Bugle";
					return counterTypeParsed;
				}
				counterTypeParsed = counterType.Replace("{zone}", text).ToUpper();
				counterTypeCached = counterType;
			}
			return counterTypeParsed;
		}
	}

	public void Triggered()
	{
		if (DateTime.Now - lastTriggered < TimeSpan.FromSeconds(5.0))
		{
			CspUtils.DebugLog("Cooldown period (" + DateTime.Now + " : " + lastTriggered + "). not triggering.");
			return;
		}
		AppShell.Instance.CounterManager.AddCounter(CounterTypeParsed, counters);
		lastTriggered = DateTime.Now;
	}
}
