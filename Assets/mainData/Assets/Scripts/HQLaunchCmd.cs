using System;

public class HQLaunchCmd : AutomationCmd
{
	public HQLaunchCmd(string cmdline)
		: base(cmdline)
	{
		AutomationManager.Instance.nHeadQuarters++;
	}

	public override bool precheckOk()
	{
		bool flag = AutomationManager.Instance.activeController != GameController.ControllerType.HeadQuarters;
		if (!flag)
		{
			AutomationManager.Instance.errHeadQuareter++;
			base.ErrorCode = "P01";
			base.ErrorMsg = "Precheck failed : HeadQuarters space not launched!";
		}
		else
		{
			flag = base.precheckOk();
		}
		return flag;
	}

	public override bool execute()
	{
		try
		{
			AppShell.Instance.Transition(GameController.ControllerType.HeadQuarters);
		}
		catch (Exception ex)
		{
			AutomationManager.Instance.errHeadQuareter++;
			base.ErrorCode = "E001";
			base.ErrorMsg += ex.Message;
		}
		return base.execute();
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		if (flag)
		{
			flag = AutomationManager.Instance.isHeadQuarters;
			CspUtils.DebugLog(AutomationManager.Instance.activeController);
			if (!flag)
			{
				base.ErrorMsg = "HQ has not completed loading...";
			}
		}
		else
		{
			AutomationManager.Instance.errHeadQuareter++;
			base.ErrorCode = "C001";
			base.ErrorMsg = "HQ failed to launch";
		}
		return flag;
	}
}
