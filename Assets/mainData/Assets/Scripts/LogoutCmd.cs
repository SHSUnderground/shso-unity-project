using System;

public class LogoutCmd : AutomationCmd
{
	public LogoutCmd(string cmdline)
		: base(cmdline)
	{
		AutomationManager.Instance.nOther++;
	}

	public override bool execute()
	{
		try
		{
			AppShell.Instance.ServerConnection.Logout();
			AppShell.Instance.Quit();
		}
		catch (Exception ex)
		{
			AutomationManager.Instance.errOther++;
			base.ErrorCode = "E001";
			base.ErrorMsg = ex.Message;
		}
		return base.execute();
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		if (flag)
		{
			flag = base.isLoggedOut;
		}
		if (!flag)
		{
			AutomationManager.Instance.errOther++;
			base.ErrorCode = "C001";
			base.ErrorMsg = "Logout Failed";
		}
		return flag;
	}

	public override bool precheckOk()
	{
		bool flag = AutomationManager.Instance.GUI_LoginComplete;
		if (!flag)
		{
			AutomationManager.Instance.errOther++;
			base.ErrorCode = "P02";
			base.ErrorMsg = "Automation cannot logout since it was not logged in";
		}
		else
		{
			flag = base.precheckOk();
		}
		return flag;
	}
}
