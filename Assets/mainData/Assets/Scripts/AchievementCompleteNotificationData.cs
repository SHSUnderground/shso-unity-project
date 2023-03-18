using System.Collections.Generic;

public class AchievementCompleteNotificationData : NotificationData
{
	public int achievementID;

	public List<OwnableSet> rewards;

	public AchievementCompleteNotificationData(int achievementID, List<OwnableSet> rewards)
		: base(NotificationType.AchievementComplete, NotificationOrientation.Center)
	{
		this.achievementID = achievementID;
		this.rewards = rewards;
	}
}
