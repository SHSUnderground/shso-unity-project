public class DailyRewardNotificationData : NotificationData
{
	public string rewardData;

	public DailyRewardNotificationData(string rewardData)
		: base(NotificationType.DailyReward, NotificationOrientation.Center)
	{
		this.rewardData = rewardData;
	}
}
