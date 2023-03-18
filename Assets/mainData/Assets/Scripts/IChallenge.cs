using System;

public interface IChallenge : IDisposable
{
	int Id
	{
		get;
	}

	string Name
	{
		get;
	}

	string Description
	{
		get;
	}

	ChallengeRewardType RewardType
	{
		get;
	}

	string RewardIcon
	{
		get;
	}

	ChallengeStatus Status
	{
		get;
	}

	ChallengeValidationEnum ChallengeValidationSource
	{
		get;
	}

	bool ForceClientEvents
	{
		get;
	}

	void Initialize(ChallengeManager manager, ChallengeInfo info, ISHSCounterType counter, ChallengeManager.ChallengeCompleteDelegate onChallengeComplete);

	void HandleChallengeEvent(ChallengeEventMessage msg);

	void ChallengeVerifiedComplete();

	void LogChallengeStatus();

	void Ready();
}
