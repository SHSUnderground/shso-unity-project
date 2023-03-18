public class MissionCompletionDetailRequest : ShsEventMessage
{
	public readonly bool hero;

	public readonly AchievementDetailWindow requester;

	public readonly string content;

	public readonly string data;

	public MissionCompletionDetailRequest(bool hero, AchievementDetailWindow requester, string content, string data)
	{
		this.hero = hero;
		this.requester = requester;
		this.content = content;
		this.data = data;
	}
}
