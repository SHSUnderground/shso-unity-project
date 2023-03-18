public class TotalTicketsNotificationData : NotificationData
{
	public TotalTicketsNotificationData(int amount)
		: base(NotificationType.TotalTickets, NotificationOrientation.Left)
	{
		data1 = amount;
	}

	public int getTotal()
	{
		return data1;
	}
}
