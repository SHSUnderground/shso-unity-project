public class DailyFractalNotificationData : NotificationData
{
	public DailyFractalNotificationData(int amount, int max)
		: base(NotificationType.DailyFractals, NotificationOrientation.Left)
	{
		data1 = amount;
		data2 = max;
	}

	public int getFractalsFound()
	{
		return data1;
	}

	public int getMaxFractals()
	{
		return data2;
	}
}
