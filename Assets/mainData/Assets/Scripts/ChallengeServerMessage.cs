public class ChallengeServerMessage : ShsEventMessage
{
	protected int challengeId;

	protected int nextChallengeId;

	protected ChallengeRewardType rewardType;

	protected string rewardValue;

	protected string success;

	protected string failReason;

	public int ChallengeId
	{
		get
		{
			return challengeId;
		}
	}

	public int NextChallengeId
	{
		get
		{
			return nextChallengeId;
		}
	}

	public ChallengeRewardType RewardType
	{
		get
		{
			return rewardType;
		}
	}

	public string RewardValue
	{
		get
		{
			return rewardValue;
		}
	}

	public string Success
	{
		get
		{
			return success;
		}
	}

	public string FailReason
	{
		get
		{
			return failReason;
		}
	}

	public ChallengeServerMessage(int challengeId, int nextChallengeId, ChallengeRewardType rewardType, string reward, string success)
		: this(challengeId, nextChallengeId, rewardType, reward, success, string.Empty)
	{
	}

	public ChallengeServerMessage(int challengeId, int nextChallengeId, ChallengeRewardType rewardType, string reward, string success, string failReason)
	{
		this.challengeId = challengeId;
		this.nextChallengeId = nextChallengeId;
		this.rewardType = rewardType;
		rewardValue = reward;
		this.success = success;
		this.failReason = failReason;
	}
}
