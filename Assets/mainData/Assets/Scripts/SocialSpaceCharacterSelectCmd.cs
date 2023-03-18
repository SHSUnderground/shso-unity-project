using System;
using System.Collections.Generic;

public class SocialSpaceCharacterSelectCmd : AutomationCmd
{
	private string characterName;

	private WaitForInit transition;

	public SocialSpaceCharacterSelectCmd(string cmdline, string name)
		: base(cmdline)
	{
		characterName = name;
		AutomationManager.Instance.nGameWorld++;
	}

	private void HideFullScreenMsg()
	{
		CspUtils.DebugLog("Automation is forcing the Welcome Window to Hide");
		List<SHSWelcomeWindow> controlsOfType = ((IGUIContainer)GUIManager.Instance.Root["SHSMainWindow/SHSSocialMainWindow"]).GetControlsOfType<SHSWelcomeWindow>();
		if (controlsOfType.Count > 0)
		{
			AppShell.Instance.EventMgr.Fire(this, new WelcomeResponseMessage(false, controlsOfType[0]));
		}
	}

	public override bool isReady()
	{
		bool flag = base.isReady();
		if (flag)
		{
			flag = ((!AutomationManager.Instance.isWebPlayer) ? (AutomationManager.Instance.activeController == GameController.ControllerType.SocialSpace) : (AutomationManager.Instance.activeController == GameController.ControllerType.FrontEnd));
			CspUtils.DebugLog("isRead: " + flag + " Active Controler: " + AutomationManager.Instance.activeController);
		}
		else
		{
			base.ErrorCode = "P001";
			base.ErrorMsg = "SocialSpaceCharacterSelectCmd failed";
			AutomationManager.Instance.errGameWorld++;
		}
		return flag;
	}

	public override bool execute()
	{
		try
		{
			if (!AutomationManager.Instance.isWebPlayer)
			{
				HideFullScreenMsg();
			}
			AutomationManager.Instance.LogAttribute("characterName", characterName);
			if (AutomationManager.Instance.playerCharacterSelected != characterName)
			{
				UserProfile profile = AppShell.Instance.Profile;
				if (profile != null)
				{
					profile.LastSelectedCostume = characterName;
					profile.SelectedCostume = characterName;
					profile.PersistExtendedData();
				}
				AppShell.Instance.EventMgr.Fire(null, new NewsClosedMessage());
				AppShell.Instance.EventMgr.Fire(null, new CharacterSelectedMessage(characterName));
				transition = new WaitForInit();
			}
		}
		catch (Exception ex)
		{
			base.ErrorCode = "E001";
			base.ErrorMsg = ex.Message;
		}
		return base.execute();
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		if (transition.isReady())
		{
			if (flag)
			{
				flag = (AutomationManager.Instance.playerCharacterSelected == characterName);
				if (!flag)
				{
					base.ErrorCode = "C001";
					base.ErrorMsg = "SocialSpaceCharacterSelectCmd failed";
					AutomationManager.Instance.errGameWorld++;
				}
			}
		}
		else
		{
			flag = false;
		}
		return flag;
	}
}
