using System;

public class BrawlerLaunchCmd : AutomationCmd
{
	private string missionName;

	private string playMode;

	private string playType;

	private GUIManager currState;

	public BrawlerLaunchCmd(string cmdline, string sname, string mode, string type)
		: base(cmdline)
	{
		missionName = sname;
		playMode = mode;
		playType = type;
		currState = new GUIManager();
		AutomationManager.Instance.missionName = sname;
		AutomationManager.Instance.currentStage = 0;
		AutomationManager.Instance.spawnObjCount = 1;
		AutomationManager.Instance.nBrawler++;
	}

	public override bool execute()
	{
		try
		{
			if (playMode == "solo")
			{
				AutomationBrawler.instance.PlaySolo(missionName);
			}
			else if (playMode == "multi")
			{
				if (playType == "friends")
				{
					AutomationBrawler.instance.InviteFriends(missionName);
				}
				else if (playType == "all")
				{
					AutomationBrawler.instance.InviteEveryone(missionName);
				}
				else
				{
					AutomationManager.Instance.errBrawler++;
					base.ErrorCode = "E001";
					base.ErrorMsg = "Invalid Paramter - Correct(friends,all)";
				}
			}
			else
			{
				AutomationManager.Instance.errBrawler++;
				base.ErrorCode = "E001";
				base.ErrorMsg = "Invalid Paramter - Correct(solo,multi)";
			}
		}
		catch (Exception ex)
		{
			AutomationManager.Instance.errBrawler++;
			base.ErrorCode = "E001";
			base.ErrorMsg += ex.Message;
		}
		return base.execute();
	}

	public override bool precheckOk()
	{
		bool flag = AutomationManager.Instance.activeController != GameController.ControllerType.Brawler;
		if (!flag)
		{
			AutomationManager.Instance.errBrawler++;
			base.ErrorCode = "P01";
			base.ErrorMsg = "Precheck failed : Brawler is already launched!";
		}
		else
		{
			flag = base.precheckOk();
		}
		return flag;
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		if (flag)
		{
			CspUtils.DebugLog("Waiting for Brawler Launch to Complete...");
			CspUtils.DebugLog("Active Controler: " + currState.CurrentState);
			flag = (GameController.ControllerType.Brawler == AutomationManager.Instance.activeController);
			if (!flag)
			{
				base.ErrorMsg = "Active Controller is not set to Brawler ";
			}
		}
		else
		{
			AutomationManager.Instance.errBrawler++;
			base.ErrorCode = "C001";
			base.ErrorMsg += "BrawlerLaunchCmd Timed out <isComplete>";
		}
		return flag;
	}

	public override bool isReady()
	{
		bool flag = base.isReady();
		if (flag)
		{
			CspUtils.DebugLog("Waiting for Brawler To Load...");
			if (!flag)
			{
				base.ErrorMsg = "Brawler Loading has not completed ";
			}
		}
		else
		{
			AutomationManager.Instance.errBrawler++;
			base.ErrorCode = "R001";
			base.ErrorMsg += "BrawlerLaunchCmd Timed Out";
		}
		return flag;
	}
}
