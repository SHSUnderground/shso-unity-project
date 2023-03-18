public class DailySeasonalNotificationData : NotificationData
{
	public string icon;

	public DailySeasonalNotificationData(int amount, int max, string icon)
		: base(NotificationType.DailySeasonals, NotificationOrientation.Left)
	{
		data1 = amount;
		data2 = max;
		this.icon = icon;
	}

	public int getSeasonalsFound()
	{
		return data1;
	}

	public int getMaxSeasonalsObjects()
	{
		return data2;
	}
}
