using System;

public class CountFrames : AutomationCmd
{
	private int duration;

	private int fpsmin;

	private int totalFrames;

	private int fpsmax;

	private int fpsavg;

	private int sec;

	public CountFrames(string cmdline, int dur)
		: base(cmdline)
	{
		totalFrames = 0;
		sec = 1;
		duration = dur;
		AutomationManager.Instance.nOther++;
	}

	public CountFrames(string cmdline)
		: base(cmdline)
	{
		totalFrames = 0;
		sec = 1;
		duration = 1;
		AutomationManager.Instance.nOther++;
	}

	public override bool isReady()
	{
		fpsmin = Convert.ToInt32(AppShell.Instance.FPS);
		fpsmax = Convert.ToInt32(AppShell.Instance.FPS);
		fpsavg = Convert.ToInt32(AppShell.Instance.FPS);
		return base.isReady();
	}

	public override bool execute()
	{
		return base.execute();
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		if (flag)
		{
			CspUtils.DebugLog("Real Time FPS: " + AppShell.Instance.FPS.ToString());
			if (DateTime.Now - base.StartTime <= TimeSpan.FromSeconds(duration))
			{
				RecordFPS();
				sec++;
				flag = false;
			}
			else
			{
				RecordFPS();
				fpsavg = totalFrames / sec;
				base.ErrorMsg = "[fpsMin:" + ((fpsmin != 0) ? fpsmin.ToString() : "undef") + "][fpsAvg:" + fpsavg + "][fpsMax:" + fpsmax + "]";
				if (fpsmin <= 30)
				{
					AutomationManager.Instance.errOther++;
					base.ErrorCode = "CFP001";
				}
			}
		}
		return flag;
	}

	public void RecordFPS()
	{
		int num = Convert.ToInt32(AppShell.Instance.FPS);
		totalFrames += num;
		if (fpsmin > num)
		{
			fpsmin = num;
		}
		if (fpsmax < num)
		{
			fpsmax = num;
		}
	}
}
