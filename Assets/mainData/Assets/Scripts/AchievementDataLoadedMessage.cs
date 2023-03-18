public class AchievementDataLoadedMessage : ShsEventMessage
{
	public PlayerAchievementData data;

	public AchievementDataLoadedMessage(PlayerAchievementData data)
	{
		this.data = data;
	}
}
