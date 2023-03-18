public class NotificationData
{
	public enum NotificationOrientation
	{
		Left,
		Center,
		Right,
		AchievementTracker
	}

	public enum NotificationType
	{
		TotalFractals,
		DailyFractals,
		DailyGoldenFractal,
		DailyTokens,
		DailyScavenger,
		DailyWheresImpossibleMan,
		DailySeasonals,
		DailyRareSeasonal,
		TotalScavenger,
		TotalSilver,
		TotalTickets,
		BrawlerInvite,
		CardGameInvite,
		OldAchievement,
		NewAchievement,
		FriendInvite,
		GameAreaAvailable,
		AchievementTracker,
		AchievementComplete,
		Help,
		DailyReward
	}

	public NotificationOrientation orientation;

	public NotificationType notificationType;

	protected int data1 = -10000;

	protected int data2 = -10000;

	protected string str1 = string.Empty;

	protected string str2 = string.Empty;

	public NotificationData(NotificationType type, NotificationOrientation orientation)
	{
		notificationType = type;
		this.orientation = orientation;
	}

	public void genericInit(int data1, int data2 = -10000, string str1 = "", string str2 = "")
	{
		this.data1 = data1;
		this.data2 = data2;
		this.str1 = str1;
		this.str2 = str2;
	}
}
