public class TotalSilverNotificationData : NotificationData
{
	public TotalSilverNotificationData(int amount)
		: base(NotificationType.TotalSilver, NotificationOrientation.Left)
	{
		data1 = amount;
	}

	public int getTotal()
	{
		return data1;
	}
}
