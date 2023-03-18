using UnityEngine;

public class DailyGoldenFractalNotificationWindow : GeneralNotificationWindow
{
	private GUIImage _icon2;

	public DailyGoldenFractalNotificationWindow()
		: base(NotificationData.NotificationType.DailyGoldenFractal, "GUI/Notifications/baseWindow", "GUI/Notifications/globe", new Vector2(60f, 61f), new Vector2(0f, 0f), "Golden Fractal", "Collected")
	{
		_icon2 = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(64f, 64f), Vector2.zero);
		_icon2.TextureSource = "common_bundle|golden_fractal";
		_icon2.Position = new Vector2(16f, 16f);
		Add(_icon2);
		_helpInfo = HelpNotificationWindow.HelpInfo.DailyGoldenFractal;
	}

	public override void init(NotificationHUD parent, NotificationData data)
	{
		base.init(parent, data);
		DailyGoldenFractalNotificationData dailyGoldenFractalNotificationData = (DailyGoldenFractalNotificationData)data;
		if (dailyGoldenFractalNotificationData == null)
		{
			CspUtils.DebugLog("ERROR: DailyGoldenFractalNotificationWindow called with an invalid data record " + data);
			return;
		}
		_mainLabel.Text = dailyGoldenFractalNotificationData.getFractalsFound().ToString() + "/" + dailyGoldenFractalNotificationData.getMaxFractals().ToString();
		recenterLabel(_mainLabel);
	}
}
