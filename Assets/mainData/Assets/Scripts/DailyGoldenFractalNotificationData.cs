public class DailyGoldenFractalNotificationData : NotificationData
{
	public DailyGoldenFractalNotificationData(int amount, int max)
		: base(NotificationType.DailyGoldenFractal, NotificationOrientation.Left)
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
