using System;

public class BrawlerCharacterSelectCmd : AutomationCmd
{
	private string characterName;

	public BrawlerCharacterSelectCmd(string cmdline, string name)
		: base(cmdline)
	{
		characterName = name;
		AutomationManager.Instance.nBrawler++;
	}

	public override bool execute()
	{
		try
		{
			CspUtils.DebugLog("Char Name: " + characterName);
			AutomationManager.Instance.LogAttribute("characterName", characterName);
			if (AutomationManager.Instance.playerCharacterSelected != characterName)
			{
				AppShell.Instance.Profile.LastSelectedCostume = characterName;
				AutomationManager.Instance.playerCharacterSelected = characterName;
			}
			else
			{
				AppShell.Instance.Profile.LastSelectedCostume = AutomationManager.Instance.playerCharacterSelected;
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

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		if (flag)
		{
			CspUtils.DebugLog("Waiting for Character Selection To Complete...");
			flag = (AutomationManager.Instance.playerCharacterSelected == characterName);
		}
		else
		{
			AutomationManager.Instance.errBrawler++;
			CspUtils.DebugLog("Brawler Character Selected: " + AutomationManager.Instance.playerCharacterSelected);
			base.ErrorCode = "C001";
			base.ErrorMsg = "BrawlerCharacterSelectCmd Timed Out";
		}
		return flag;
	}

	public override bool isReady()
	{
		bool flag = base.isReady();
		CspUtils.DebugLog("Waiting for Character Selectiong to be ready...");
		if (flag)
		{
			flag = (AutomationManager.Instance.activeController != GameController.ControllerType.Brawler);
			if (!flag)
			{
				CspUtils.DebugLog("Active Controler: " + AutomationManager.Instance.activeController);
				base.ErrorMsg = base.ErrorMsg + "Active Controler is set to: " + AutomationManager.Instance.activeController;
			}
		}
		else
		{
			AutomationManager.Instance.errBrawler++;
			base.ErrorCode = "P001";
		}
		return flag;
	}
}
