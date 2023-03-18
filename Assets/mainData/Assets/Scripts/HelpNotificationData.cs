using System.Collections.Generic;

public class HelpNotificationData : NotificationData
{
	public HelpNotificationWindow.HelpInfo helpInfo;

	public List<string> helpStrings = new List<string>();

	public string iconPath = string.Empty;

	public string iconUnderlayPath = string.Empty;

	public string iconOverlayPath = string.Empty;

	public HelpNotificationData(HelpNotificationWindow.HelpInfo helpInfo)
		: base(NotificationType.Help, NotificationOrientation.Center)
	{
		switch (helpInfo)
		{
		case HelpNotificationWindow.HelpInfo.DailyFractal:
			iconPath = "common_bundle|fractal";
			iconUnderlayPath = "GUI/Notifications/globe";
			helpStrings.Add("#HELP_DAILYFRACTAL_1");
			helpStrings.Add("#HELP_DAILYFRACTAL_2");
			helpStrings.Add("#HELP_DAILY_HEROMOVES");
			break;
		case HelpNotificationWindow.HelpInfo.DailyToken:
			iconPath = "achievement_bundle|tokens";
			helpStrings.Add("#HELP_DAILYTOKEN_1");
			helpStrings.Add("#HELP_DAILYTOKEN_2");
			helpStrings.Add("#HELP_DAILY_HEROMOVES");
			break;
		case HelpNotificationWindow.HelpInfo.DailyGoldenFractal:
			iconPath = "common_bundle|golden_fractal";
			iconUnderlayPath = "GUI/Notifications/globe";
			helpStrings.Add("#HELP_DAILYGOLDEN_1");
			helpStrings.Add("#HELP_DAILYGOLDEN_2");
			helpStrings.Add("#HELP_DAILY_HEROMOVES");
			break;
		case HelpNotificationWindow.HelpInfo.DailyScavenger:
			iconPath = "shopping_bundle|craft_generic";
			helpStrings.Add("#HELP_DAILYCRAFTING_1");
			helpStrings.Add("#HELP_DAILYCRAFTING_2");
			helpStrings.Add("#HELP_DAILY_HEROMOVES");
			break;
		case HelpNotificationWindow.HelpInfo.DailyImpossibleMan:
			iconPath = "characters_bundle|token_impossible_man_playable";
			iconUnderlayPath = "GUI/Notifications/globe";
			helpStrings.Add("#HELP_DAILYIMPY_1");
			helpStrings.Add("#HELP_DAILYIMPY_2");
			helpStrings.Add("#HELP_DAILYIMPY_3");
			break;
		case HelpNotificationWindow.HelpInfo.DailySeasonal_Halloween:
			iconPath = "common_bundle|seasonal_taco";
			helpStrings.Add("#HELP_DAILYSEASONAL_HALLOWEEN_1");
			helpStrings.Add("#HELP_DAILYSEASONAL_HALLOWEEN_2");
			helpStrings.Add("#HELP_DAILY_HEROMOVES");
			break;
		case HelpNotificationWindow.HelpInfo.DailyRareSeasonal_Halloween:
			iconPath = "common_bundle|rare_seasonal_taco";
			helpStrings.Add("#HELP_DAILYRARESEASONAL_HALLOWEEN_1");
			helpStrings.Add("#HELP_DAILYRARESEASONAL_HALLOWEEN_2");
			helpStrings.Add("#HELP_DAILY_HEROMOVES");
			break;
		}
	}

	public HelpNotificationData(string iconPath, List<string> helpStrings, string iconUnderlayPath = "", string iconOverlayPath = "")
		: base(NotificationType.Help, NotificationOrientation.Center)
	{
		this.helpStrings = helpStrings;
		this.iconPath = iconPath;
		this.iconUnderlayPath = iconUnderlayPath;
		this.iconOverlayPath = iconOverlayPath;
	}
}
