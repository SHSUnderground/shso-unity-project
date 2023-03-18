using System;

public class SocialSpaceLaunchCmd : AutomationCmd
{
	private string spaceName;

	public SocialSpaceLaunchCmd(string cmdline, string sname)
		: base(cmdline)
	{
		spaceName = sname;
		AutomationManager.Instance.nGameWorld++;
	}

	public override bool execute()
	{
		try
		{
			AutomationManager.Instance.LogAttribute("spaceName", spaceName);
			CspUtils.DebugLog("SHT_KEY_SOCIAL_LEVEL  SocialSpaceLevel SHT_KEY_SOCIAL_SPAWNPOINT SocialSpaceSpawnPoint SHT_KEY_SOCIAL_CHARACTER   SocialSpaceCharacter SHT_KEY_SOCIAL_TICKET SocialSpaceTicket");
			string value = null;
			if (GameController.GetController() != null && GameController.GetController().LocalPlayer != null)
			{
				value = GameController.GetController().LocalPlayer.name;
			}
			AppShell.Instance.SharedHashTable["SocialSpaceLevel"] = spaceName;
			AppShell.Instance.SharedHashTable["SocialSpaceSpawnPoint"] = null;
			AppShell.Instance.SharedHashTable["SocialSpaceCharacter"] = value;
			AppShell.Instance.Transition(GameController.ControllerType.SocialSpace);
		}
		catch (Exception ex)
		{
			base.ErrorCode = "E001";
			base.ErrorMsg += ex.Message;
		}
		return base.execute();
	}

	public override bool precheckOk()
	{
		bool flag = AutomationManager.Instance.activeController != GameController.ControllerType.SocialSpace;
		if (!flag)
		{
			base.ErrorCode = "P01";
			base.ErrorMsg = "Precheck failed : Active Controler is not set to Social Space";
		}
		else
		{
			flag = base.precheckOk();
		}
		return flag;
	}

	public override bool isReady()
	{
		CspUtils.DebugLog("isReady - SocialSpaceLaunchCmd");
		bool flag = base.isReady();
		if (flag)
		{
			if (!flag)
			{
				base.ErrorMsg = "SocialSpace is not ready to load";
			}
		}
		else
		{
			base.ErrorCode = "R001";
			base.ErrorMsg += "SocialSpace Lunch Timed Out";
		}
		return flag;
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		if (flag)
		{
			flag = (AutomationManager.Instance.activeController == GameController.ControllerType.SocialSpace);
			if (!flag)
			{
				base.ErrorMsg = "SocialSpaceLaunchCmd has not loaded";
			}
		}
		else
		{
			base.ErrorCode = "C001";
			base.ErrorMsg = "SocialSpaceLaunchCmd Timed Out";
		}
		return flag;
	}
}
