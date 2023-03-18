using System;
using UnityEngine;

public class LoginCmd : AutomationCmd
{
	private string username;

	private string password;

	private WaitForInit WindowScene;

	public LoginCmd(string cmdline)
		: base(cmdline)
	{
		username = string.Empty;
		password = string.Empty;
		AutomationManager.Instance.nOther++;
	}

	public LoginCmd(string cmdline, string uname, string pwd)
		: base(cmdline)
	{
		username = uname;
		password = pwd;
	}

	public override bool execute()
	{
		try
		{
			AutomationManager.Instance.LogAttribute("username", username);
			AutomationManager.Instance.LogAttribute("password", password);
			AutomationManager.Instance.LoginStatus = AutomationManager.LoginState.inProgress;
			AppShell.Instance.ServerConnection.Login(username, password);
			CspUtils.DebugLogWarning("Executing Login..........");
		}
		catch (Exception ex)
		{
			base.ErrorCode = "E001";
			base.ErrorMsg += ex.Message;
			AutomationManager.Instance.errOther++;
		}
		return base.execute();
	}

	public override bool precheckOk()
	{
		bool flag = true;
		WindowScene = new WaitForInit();
		if (AutomationManager.Instance.LoginStatus == AutomationManager.LoginState.Succeeded || AutomationManager.Instance.LoginStatus == AutomationManager.LoginState.inProgress)
		{
			AutomationManager.Instance.errOther++;
			flag = false;
			base.ErrorCode = "P01";
			base.ErrorMsg = "Precheck failed : Cannot log in because Automation already logged in or in progress.";
			CspUtils.DebugLog(base.ErrorMsg);
		}
		else
		{
			flag = base.precheckOk();
		}
		return flag;
	}

	public override bool isReady()
	{
		bool flag = base.isReady();
		if (flag)
		{
			if (WindowScene.isReady())
			{
				flag = (AutomationManager.Instance.LoginStatus == AutomationManager.LoginState.None);
				if (!flag)
				{
					AutomationManager.Instance.errOther++;
					base.ErrorCode = "R001";
					base.ErrorMsg = "Login ready conditions not met";
				}
				else
				{
					flag = (AutomationManager.Instance.activeController == GameController.ControllerType.FrontEnd);
				}
			}
			else
			{
				flag = false;
			}
		}
		else
		{
			AutomationManager.Instance.errOther++;
			base.ErrorCode = "P001";
			base.ErrorMsg = "Timeout - Command was never in ReadyState";
		}
		CspUtils.DebugLog("LoginCmd::isReady = " + flag);
		return flag;
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		if (flag)
		{
			base.isLoggedOut = false;
			if (AutomationManager.Instance.LoginStatus == AutomationManager.LoginState.Failed)
			{
				AutomationManager.Instance.errOther++;
				base.ErrorCode = "C001";
				base.ErrorMsg = "Login failed: unable to complete";
			}
		}
		else
		{
			AutomationManager.Instance.errOther++;
			base.ErrorCode = "C002";
			base.ErrorMsg = "Timeout Error : User unable to Login";
			CspUtils.DebugLogWarning("Login Command has Timed Out!");
		}
		return flag;
	}
}
