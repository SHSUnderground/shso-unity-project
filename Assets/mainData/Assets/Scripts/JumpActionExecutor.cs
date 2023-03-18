using UnityEngine;

public class JumpActionExecutor
{
	private string jumpActionMissionName = string.Empty;

	private string forcedHeroName = string.Empty;

	public JumpActionExecutor(string jumpAction = "")
	{
		if (jumpAction == string.Empty)
		{
			return;
		}
		string[] array = jumpAction.Split(',');
		if (array[0] == "mission")
		{
			if (array.Length <= 1)
			{
				CspUtils.DebugLog("Got a jumpAction of " + jumpAction + " but was missing the mission name");
				return;
			}
			jumpActionMissionName = array[1];
			AppShell.Instance.DataManager.LoadGameData("Missions/" + jumpActionMissionName, OnMissionDefinitionLoaded, new MissionDefinition());
		}
		else if (array[0] == "craft")
		{
			if (array.Length <= 1)
			{
				CspUtils.DebugLog("Got a jumpAction of " + jumpAction + " but was missing the ownable ID");
				return;
			}
			int initialOwnableID = int.Parse(array[1]);
			CraftingWindow.requestCraftingWindow(initialOwnableID);
		}
	}

	public void jumpToMission(string missionName, string heroName = "")
	{
		jumpActionMissionName = missionName;
		forcedHeroName = heroName;
		AppShell.Instance.DataManager.LoadGameData("Missions/" + jumpActionMissionName, OnMissionDefinitionLoaded, new MissionDefinition());
	}

	protected void OnMissionDefinitionLoaded(GameDataLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("SHSBRawlerGadget: The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			return;
		}
		MissionDefinition missionDefinition = response.DataDefinition as MissionDefinition;
		string lastSelectedCostume = AppShell.Instance.Profile.LastSelectedCostume;
		if (forcedHeroName != string.Empty)
		{
			lastSelectedCostume = forcedHeroName;
		}
		foreach (string blacklistCharacter in missionDefinition.blacklistCharacters)
		{
			bool flag = true;
			if (blacklistCharacter == "non_villains")
			{
				HeroDefinition heroDef = OwnableDefinition.getHeroDef(lastSelectedCostume);
				if (!heroDef.charDef.isVillain)
				{
					CspUtils.DebugLogWarning("This mission only allows villains, and " + lastSelectedCostume + " is not a villain, check their XML.");
					flag = false;
				}
			}
			if (blacklistCharacter == AppShell.Instance.Profile.LastSelectedCostume || !flag)
			{
				SHSErrorNotificationWindow sHSErrorNotificationWindow = new SHSErrorNotificationWindow(SHSErrorNotificationWindow.ErrorIcons.MissionNoGo);
				sHSErrorNotificationWindow.TitleText = "#error_oops";
				sHSErrorNotificationWindow.Text = "#BRAWLER_BANNED_HERO_NOTIF";
				GUIManager.Instance.ShowDynamicWindow(sHSErrorNotificationWindow, GUIControl.ModalLevelEnum.Full);
				return;
			}
		}
		AppShell.Instance.Matchmaker2.SoloBrawler(jumpActionMissionName, OnRecievedTicket);
	}

	private void OnRecievedTicket(Matchmaker2.Ticket ticket)
	{
		if (ticket != null && ticket.status == Matchmaker2.Ticket.Status.SUCCESS)
		{
			ticket.local = true;
			AppShell.Instance.SharedHashTable["BrawlerTicket"] = ticket;
			AppShell.Instance.SharedHashTable["BrawlerAirlockPlaySolo"] = true;
			AppShell.Instance.SharedHashTable["BrawlerAirlockReturnsToGadget"] = false;
			AppShell.Instance.QueueLocationInfo();
			if (forcedHeroName != string.Empty)
			{
				AppShell.Instance.Profile.LastSelectedCostume = forcedHeroName;
			}
			ActiveMission activeMission = new ActiveMission(jumpActionMissionName);
			activeMission.BecomeActiveMission();
			AppShell.Instance.Transition(GameController.ControllerType.Brawler);
		}
		else
		{
			CspUtils.DebugLog("bad ticket " + ticket.status + " " + ticket.server + " " + ticket.ticket);
			SHSErrorNotificationWindow sHSErrorNotificationWindow = new SHSErrorNotificationWindow(SHSErrorNotificationWindow.ErrorIcons.MissionNoGo);
			sHSErrorNotificationWindow.TitleText = "#error_oops";
			sHSErrorNotificationWindow.Text = "#missioninvite_error";
			GUIManager.Instance.ShowDynamicWindow(sHSErrorNotificationWindow, GUIControl.ModalLevelEnum.Full);
		}
	}
}
