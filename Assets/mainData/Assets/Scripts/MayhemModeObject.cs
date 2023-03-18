using System;
using UnityEngine;

public class MayhemModeObject : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private static DateTime epoch = new DateTime(2013, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	private string counterKey;

	private ISHSCounterType counterType;

	public void Start()
	{
		counterKey = "MayhemMode." + base.gameObject.name;
		counterType = AppShell.Instance.CounterManager.GetCounter(counterKey);
		long num = Convert.ToInt64((DateTime.Now - epoch).TotalSeconds);
		if (num - 3600 < counterType.GetCurrentValue() && counterType.GetCurrentValue() != 0L)
		{
			base.gameObject.SetActiveRecursively(false);
		}
	}

	public void MarkUsed()
	{
		// CSP commented out this block so that mayhem missions will always be available.
		//long counter = Convert.ToInt64((DateTime.Now - epoch).TotalSeconds);
		//ISHSCounterType counter2 = AppShell.Instance.CounterManager.GetCounter(counterKey);
		//counter2.SetCounter(counter);
	}
}
