public class DailyTokenNotificationData : NotificationData
{
	public DailyTokenNotificationData(string hero, int amount, int max)
		: base(NotificationType.DailyTokens, NotificationOrientation.Left)
	{
		str1 = hero;
		data1 = amount;
		data2 = max;
	}

	public string getHero()
	{
		return str1;
	}

	public int getTokensFound()
	{
		return data1;
	}

	public int getMaxTokens()
	{
		return data2;
	}
}
