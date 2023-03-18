public class DailyScavengerNotificationData : NotificationData
{
	public DailyScavengerNotificationData(int amount, int max)
		: base(NotificationType.DailyScavenger, NotificationOrientation.Left)
	{
		data1 = amount;
		data2 = max;
	}

	public int getItemsFound()
	{
		return data1;
	}

	public int getMaxItems()
	{
		return data2;
	}
}
