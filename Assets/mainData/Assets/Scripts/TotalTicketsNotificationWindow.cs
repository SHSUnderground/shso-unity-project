using UnityEngine;

public class TotalTicketsNotificationWindow : GeneralNotificationWindow
{
	public TotalTicketsNotificationWindow()
		: base(NotificationData.NotificationType.TotalTickets, "GUI/Notifications/gameworld_pickup_toast_tickets", string.Empty, Vector2.zero, Vector2.zero, string.Empty, "#TICKET_COUNT_TOAST")
	{
		_mainLabel.FontSize += 6;
		_mainLabel.Position += new Vector2(0f, -10f);
	}

	public override void init(NotificationHUD parent, NotificationData data)
	{
		base.init(parent, data);
		TotalTicketsNotificationData totalTicketsNotificationData = (TotalTicketsNotificationData)data;
		if (totalTicketsNotificationData == null)
		{
			CspUtils.DebugLog("ERROR: TotalTicketsNotificationWindow called with an invalid data record " + data);
			return;
		}
		_mainLabel.Text = string.Empty + totalTicketsNotificationData.getTotal();
		recenterLabel(_mainLabel);
	}
}
