public class ChallengeRewardSelectedMessage : ShsEventMessage
{
	public string hero;

	public ChallengeRewardSelectedMessage(string hero)
	{
		this.hero = hero;
	}
}
