public class DailyRewardMessage : ShsEventMessage
{
	public int consecutiveDays;

	public bool rewardReceived;

	public string countdown;

	public DailyRewardMessage(int consecutiveDays, bool rewardReceived, string countdown)
	{
		this.consecutiveDays = consecutiveDays;
		this.rewardReceived = rewardReceived;
		this.countdown = countdown;
	}
}
