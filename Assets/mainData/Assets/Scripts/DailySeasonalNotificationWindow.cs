using UnityEngine;

public class DailySeasonalNotificationWindow : GeneralNotificationWindow
{
	private GUIImage _icon2;

	public DailySeasonalNotificationWindow()
		: base(NotificationData.NotificationType.DailySeasonals, "GUI/Notifications/baseWindow", "GUI/Notifications/globe", new Vector2(60f, 61f), new Vector2(0f, 0f), "Tacos", "Collected")
	{
		_icon2 = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(64f, 64f), Vector2.zero);
		_icon2.Position = new Vector2(16f, 16f);
		_icon2.IsVisible = false;
		Add(_icon2);
		_helpInfo = HelpNotificationWindow.HelpInfo.DailySeasonal_Halloween;
	}

	public override void init(NotificationHUD parent, NotificationData data)
	{
		base.init(parent, data);
		DailySeasonalNotificationData dailySeasonalNotificationData = (DailySeasonalNotificationData)data;
		if (dailySeasonalNotificationData == null)
		{
			CspUtils.DebugLog("ERROR: DailySeasonalNotificationData called with an invalid data record " + data);
			return;
		}
		_icon2.TextureSource = dailySeasonalNotificationData.icon;
		_icon2.IsVisible = true;
		_mainLabel.Text = dailySeasonalNotificationData.getSeasonalsFound().ToString() + "/" + dailySeasonalNotificationData.getMaxSeasonalsObjects().ToString();
		recenterLabel(_mainLabel);
	}
}
