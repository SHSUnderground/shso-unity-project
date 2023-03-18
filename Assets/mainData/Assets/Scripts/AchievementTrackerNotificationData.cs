public class AchievementTrackerNotificationData : NotificationData
{
	public int achievementID;

	public bool isCollapsed;

	public AchievementTrackerNotificationData(int achievementID, bool isCollapsed)
		: base(NotificationType.AchievementTracker, NotificationOrientation.AchievementTracker)
	{
		this.achievementID = achievementID;
		this.isCollapsed = isCollapsed;
	}
}
