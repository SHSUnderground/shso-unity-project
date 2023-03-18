public class TotalFractalNotificationData : NotificationData
{
	public TotalFractalNotificationData(int amount)
		: base(NotificationType.TotalFractals, NotificationOrientation.Left)
	{
		data1 = amount;
	}

	public int getTotalFractals()
	{
		return data1;
	}
}
