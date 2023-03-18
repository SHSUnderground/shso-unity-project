using UnityEngine;

public class SidekickSpecialAbilitySpawn : SidekickSpecialAbility
{
	public string objectType = string.Empty;

	public int refresh = 60;

	public int amount = 1;

	private float lastSpawnTime;

	private int spawnCount;

	public SidekickSpecialAbilitySpawn(PetUpgradeXMLDefinitionSpawn def)
		: base(def)
	{
		objectType = def.objectType;
		refresh = def.refresh;
		amount = def.amount;
		switch (objectType)
		{
		case "tickets":
			name = "#ABILITY_NAME_CREATE_TICKETS";
			break;
		case "silver":
			name = "#ABILITY_NAME_CREATE_SILVER";
			break;
		case "xp":
			name = "#ABILITY_NAME_CREATE_XP";
			break;
		case "fractals":
			name = "#ABILITY_NAME_CREATE_FRACTALS";
			break;
		case "scavenger":
			name = "#ABILITY_NAME_CREATE_SCAVENGER";
			break;
		}
	}

	public override void begin()
	{
		lastSpawnTime = Time.time;
		spawnCount = 0;
	}

	public override void update()
	{
		if (!(Time.time - lastSpawnTime > (float)refresh))
		{
			return;
		}
		CspUtils.DebugLog("it is time!");
		lastSpawnTime = Time.time;
		if (SocialSpaceControllerImpl.playerIsIdle)
		{
			CspUtils.DebugLog("totally ignoring SidekickAbilitySpawn because idle hacks");
			return;
		}
		spawnCount++;
		switch (objectType)
		{
		case "tickets":
			AppShell.Instance.EventReporter.ReportAwardTokens(amount);
			NotificationHUD.addNotification(new TotalSilverNotificationData(AppShell.Instance.Profile.Silver));
			break;
		case "silver":
			AppShell.Instance.EventReporter.ReportAwardTokens(amount);
			NotificationHUD.addNotification(new TotalSilverNotificationData(AppShell.Instance.Profile.Silver));
			break;
		case "xp":
			AppShell.Instance.EventReporter.ReportAchievementEvent(SocialSpaceController.Instance.LocalPlayer.name, "ri", "sidekick_spawn_xp", 1, string.Empty);
			break;
		case "fractals":
			AppShell.Instance.EventReporter.ReportAchievementEvent(SocialSpaceController.Instance.LocalPlayer.name, "ri", "sidekick_spawn_fractal", 1, string.Empty);
			GUIManager.Instance.ShowDynamicWindow(new SHSFractalholioWindow(new Vector2(GUIManager.ScreenRect.width - 100f, 50f), 1, false), GUIControl.ModalLevelEnum.None);
			break;
		case "scavenger":
		{
			SHSSocialMainWindow sHSSocialMainWindow = GUIManager.Instance["SHSMainWindow/SHSSocialMainWindow"] as SHSSocialMainWindow;
			if (sHSSocialMainWindow != null)
			{
				sHSSocialMainWindow.OnCollectScavengerObject(-27, false);
			}
			break;
		}
		}
	}
}
