using UnityEngine;

public class GoFullScreen : AutomationCmd
{
	public GoFullScreen(string cmdline)
		: base(cmdline)
	{
		AutomationManager.Instance.nOther++;
	}

	public override bool isReady()
	{
		CspUtils.DebugLog("Full Screen Ready Meth");
		bool flag = base.isReady();
		if (flag)
		{
			flag = !Screen.fullScreen;
			CspUtils.DebugLog(flag);
		}
		return flag;
	}

	public override bool execute()
	{
		bool result = base.execute();
		CspUtils.DebugLog("Full Screen Exec Meth");
		try
		{
			AppShell.Instance.AutoFullScreenToggle();
			return result;
		}
		catch
		{
			AutomationManager.Instance.errOther++;
			base.ErrorMsg = "Failed to Toggle Full Screen";
			base.ErrorCode = "E001";
			CspUtils.DebugLog("Failed to Switch Screen Mode");
			return false;
		}
	}

	public override bool isCompleted()
	{
		CspUtils.DebugLog("Full Screen Complete Meth");
		bool flag = base.isCompleted();
		if (flag)
		{
			flag = Screen.fullScreen;
		}
		CspUtils.DebugLog("status: " + flag);
		return flag;
	}
}
