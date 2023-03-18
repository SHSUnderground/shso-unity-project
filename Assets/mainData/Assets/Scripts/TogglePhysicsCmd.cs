using System;

public class TogglePhysicsCmd : AutomationCmd
{
	public TogglePhysicsCmd(string cmdline)
		: base(cmdline)
	{
		AutomationManager.Instance.nHeadQuarters++;
	}

	public override bool execute()
	{
		try
		{
			AppShell.Instance.EventMgr.Fire(null, new TogglePhysicsMessage());
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
		return true;
	}

	public override bool isReady()
	{
		bool flag = base.isReady();
		if (flag)
		{
			flag = (AutomationManager.Instance.activeController == GameController.ControllerType.HeadQuarters);
		}
		if (!flag)
		{
			AutomationManager.Instance.errHeadQuareter++;
			base.ErrorCode = "P001";
			base.ErrorMsg = "TogglePhysicsCmd Error";
		}
		return flag;
	}
}
