using UnityEngine;

public class DailyRareSeasonalNotificationWindow : GeneralNotificationWindow
{
	private GUIImage _icon2;

	public DailyRareSeasonalNotificationWindow()
		: base(NotificationData.NotificationType.DailyRareSeasonal, "GUI/Notifications/baseWindow", "GUI/Notifications/globe", new Vector2(60f, 61f), new Vector2(0f, 0f), "Chimichangas", "Collected")
	{
		_icon2 = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(64f, 64f), Vector2.zero);
		_icon2.Position = new Vector2(16f, 16f);
		_icon2.IsVisible = false;
		Add(_icon2);
		_helpInfo = HelpNotificationWindow.HelpInfo.DailyRareSeasonal_Halloween;
	}

	public override void init(NotificationHUD parent, NotificationData data)
	{
		base.init(parent, data);
		DailyRareSeasonalNotificationData dailyRareSeasonalNotificationData = (DailyRareSeasonalNotificationData)data;
		if (dailyRareSeasonalNotificationData == null)
		{
			CspUtils.DebugLog("ERROR: DailyRareSeasonalNotificationData called with an invalid data record " + data);
			return;
		}
		_icon2.TextureSource = dailyRareSeasonalNotificationData.icon;
		_icon2.IsVisible = true;
		_mainLabel.Text = dailyRareSeasonalNotificationData.getSeasonalsFound().ToString() + "/" + dailyRareSeasonalNotificationData.getMaxSeasonalsObjects().ToString();
		recenterLabel(_mainLabel);
	}
}
