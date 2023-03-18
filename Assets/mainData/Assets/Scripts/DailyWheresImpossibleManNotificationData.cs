public class DailyWheresImpossibleManNotificationData : NotificationData
{
	public DailyWheresImpossibleManNotificationData(int amount, int max)
		: base(NotificationType.DailyWheresImpossibleMan, NotificationOrientation.Left)
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
