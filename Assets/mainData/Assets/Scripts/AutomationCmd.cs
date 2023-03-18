using System;

public class AutomationCmd : IAutomationCmdAPI
{
	private const int DEFAULT_COMPLETION_TIMEOUT = 60;

	private const int DEFAULT_STARTUP_TIMEOUT = 60;

	public const int DEFAULT_FIGHT_TIMEOUT = 1;

	public const int FPS_TIMEOUT = 1;

	public const int FPS_THRESHOLD = 30;

	public const float OFFSET = 2f;

	protected string line;

	private string errormsg;

	private string errorcode;

	private bool logoutstatus;

	private DateTime startupTime;

	private DateTime completeTime;

	private bool criticalCmdFlag;

	public bool isBrawlerMultiplayer;

	public string ErrorMsg
	{
		get
		{
			return errormsg;
		}
		set
		{
			errormsg = value;
		}
	}

	public string ErrorCode
	{
		get
		{
			return errorcode;
		}
		set
		{
			errorcode = value;
		}
	}

	public DateTime StartTime
	{
		get
		{
			return startupTime;
		}
		set
		{
			startupTime = value;
		}
	}

	public DateTime CompleteTime
	{
		get
		{
			return completeTime;
		}
		set
		{
			completeTime = value;
		}
	}

	public bool isLoggedOut
	{
		get
		{
			return logoutstatus;
		}
		set
		{
			logoutstatus = value;
		}
	}

	public bool isCriticalCmd
	{
		get
		{
			return criticalCmdFlag;
		}
		set
		{
			criticalCmdFlag = value;
		}
	}

	public AutomationCmd(string cmdline)
	{
		errormsg = "Success";
		errorcode = "OK";
		line = cmdline;
		logoutstatus = true;
		criticalCmdFlag = false;
		isBrawlerMultiplayer = false;
	}

	public override string ToString()
	{
		return line;
	}

	public virtual bool precheckOk()
	{
		errormsg = "Success";
		errorcode = "OK";
		StartTime = DateTime.Now;
		return true;
	}

	public virtual bool isReady()
	{
		errormsg = "Success";
		errorcode = "OK";
		if (DateTime.Now - StartTime > TimeSpan.FromSeconds(60.0))
		{
			ErrorCode = "P001";
			ErrorMsg = "Timeout! ";
			CspUtils.DebugLog("Base:isReady() - false");
			return false;
		}
		CspUtils.DebugLog("Base:isReady() - true");
		return true;
	}

	public virtual bool execute()
	{
		CspUtils.DebugLog("Executing :" + line);
		CompleteTime = DateTime.Now;
		return true;
	}

	public virtual bool isCompleted()
	{
		errormsg = "Success";
		errorcode = "OK";
		if (DateTime.Now - CompleteTime > TimeSpan.FromSeconds(60.0))
		{
			ErrorCode = "C001";
			ErrorMsg = "Timeout! ";
			return false;
		}
		return true;
	}

	public virtual void results()
	{
		AutomationManager.Instance.LogElementString("Result", ErrorMsg);
		AutomationManager.Instance.LogElementString("Code", ErrorCode);
	}
}
