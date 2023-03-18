public class DailyRareSeasonalNotificationData : NotificationData
{
	public string icon;

	public DailyRareSeasonalNotificationData(int amount, int max, string icon)
		: base(NotificationType.DailyRareSeasonal, NotificationOrientation.Left)
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
