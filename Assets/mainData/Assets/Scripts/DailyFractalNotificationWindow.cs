using UnityEngine;

public class DailyFractalNotificationWindow : GeneralNotificationWindow
{
	private GUIImage _icon2;

	public DailyFractalNotificationWindow()
		: base(NotificationData.NotificationType.DailyFractals, "GUI/Notifications/baseWindow", "GUI/Notifications/globe", new Vector2(60f, 61f), new Vector2(0f, 0f), "Fractals", "Collected")
	{
		_icon2 = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(64f, 64f), Vector2.zero);
		_icon2.TextureSource = "common_bundle|fractal";
		_icon2.Position = new Vector2(16f, 16f);
		Add(_icon2);
		_helpInfo = HelpNotificationWindow.HelpInfo.DailyFractal;
	}

	public override void init(NotificationHUD parent, NotificationData data)
	{
		base.init(parent, data);
		DailyFractalNotificationData dailyFractalNotificationData = (DailyFractalNotificationData)data;
		if (dailyFractalNotificationData == null)
		{
			CspUtils.DebugLog("ERROR: DailyFractalNotificationWindow called with an invalid data record " + data);
			return;
		}
		_mainLabel.Text = dailyFractalNotificationData.getFractalsFound().ToString() + "/" + dailyFractalNotificationData.getMaxFractals().ToString();
		recenterLabel(_mainLabel);
	}
}
