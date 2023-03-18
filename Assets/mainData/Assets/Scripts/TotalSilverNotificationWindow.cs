using UnityEngine;

public class TotalSilverNotificationWindow : GeneralNotificationWindow
{
	public TotalSilverNotificationWindow()
		: base(NotificationData.NotificationType.TotalSilver, "GUI/Notifications/gameworld_pickup_toast_silver", string.Empty, Vector2.zero, Vector2.zero, string.Empty, "#SILVER_COUNT_TOAST")
	{
		_mainLabel.FontSize += 6;
		_mainLabel.Position += new Vector2(0f, -10f);
	}

	public override void init(NotificationHUD parent, NotificationData data)
	{
		base.init(parent, data);
		TotalSilverNotificationData totalSilverNotificationData = (TotalSilverNotificationData)data;
		if (totalSilverNotificationData == null)
		{
			CspUtils.DebugLog("ERROR: TotalSilverNotificationWindow called with an invalid data record " + data);
			return;
		}
		_mainLabel.Text = string.Empty + totalSilverNotificationData.getTotal();
		recenterLabel(_mainLabel);
	}
}
