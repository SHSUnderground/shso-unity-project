using System;

public class WaitForInit
{
	private DateTime start;

	private int ELAPSED_TIME = 25;

	public static WaitForInit instance
	{
		get
		{
			return new WaitForInit();
		}
	}

	public WaitForInit()
	{
		start = DateTime.Now;
	}

	public bool isReady()
	{
		if (DateTime.Now - start > TimeSpan.FromSeconds(ELAPSED_TIME))
		{
			return true;
		}
		return false;
	}
}
