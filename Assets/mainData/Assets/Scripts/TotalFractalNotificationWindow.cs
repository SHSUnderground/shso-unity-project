using UnityEngine;

public class TotalFractalNotificationWindow : GeneralNotificationWindow
{
	private GUIImage _icon2;

	public TotalFractalNotificationWindow()
		: base(NotificationData.NotificationType.TotalFractals, "GUI/Notifications/baseWindow", "GUI/Notifications/globe", new Vector2(60f, 61f), new Vector2(0f, 0f), string.Empty, "Total Fractals")
	{
		_icon2 = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(64f, 64f), Vector2.zero);
		_icon2.TextureSource = "common_bundle|fractal";
		_icon2.Position = new Vector2(16f, 16f);
		Add(_icon2);
		_mainLabel.FontSize += 6;
		_mainLabel.Position += new Vector2(0f, -10f);
	}

	public override void init(NotificationHUD parent, NotificationData data)
	{
		base.init(parent, data);
		TotalFractalNotificationData totalFractalNotificationData = (TotalFractalNotificationData)data;
		if (totalFractalNotificationData == null)
		{
			CspUtils.DebugLog("ERROR: TotalFractalNotificationWindow called with an invalid data record " + data);
			return;
		}
		_mainLabel.Text = string.Empty + totalFractalNotificationData.getTotalFractals();
		recenterLabel(_mainLabel);
	}
}
