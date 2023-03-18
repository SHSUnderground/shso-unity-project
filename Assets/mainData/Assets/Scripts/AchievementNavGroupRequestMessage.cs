public class AchievementNavGroupRequestMessage : ShsEventMessage
{
	public int groupID;

	public AchievementNavGroupRequestMessage(int groupID)
	{
		this.groupID = groupID;
	}
}
