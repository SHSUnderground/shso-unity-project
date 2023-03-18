using System;

public class ChallengeBase : IDisposable, IChallenge
{
	public const int COUNTER_UNINITIALIZED = 0;

	protected int id;

	protected string name;

	protected string description;

	protected string messageType;

	protected ChallengeStatus challengeStatus;

	protected ISHSCounterType challengeCounter;

	protected ChallengeValidationEnum challengeValidationSource;

	protected ChallengeManager.ChallengeCompleteDelegate onChallengeComplete;

	protected ChallengeRewardType rewardType;

	protected string rewardValue;

	protected string rewardIcon;

	protected bool forceClientEvents;

	public virtual int Id
	{
		get
		{
			return id;
		}
	}

	public virtual string Name
	{
		get
		{
			return name;
		}
	}

	public virtual string Description
	{
		get
		{
			return description;
		}
	}

	public virtual ChallengeRewardType RewardType
	{
		get
		{
			return rewardType;
		}
	}

	public virtual string RewardValue
	{
		get
		{
			return rewardValue;
		}
	}

	public virtual string RewardIcon
	{
		get
		{
			return rewardIcon;
		}
	}

	public virtual bool ForceClientEvents
	{
		get
		{
			return forceClientEvents;
		}
	}

	public virtual ChallengeStatus Status
	{
		get
		{
			return challengeStatus;
		}
	}

	public virtual ChallengeValidationEnum ChallengeValidationSource
	{
		get
		{
			return challengeValidationSource;
		}
	}

	public ChallengeBase()
	{
		challengeStatus = ChallengeStatus.NotStarted;
	}

	public virtual void Initialize(ChallengeManager manager, ChallengeInfo info, ISHSCounterType counter, ChallengeManager.ChallengeCompleteDelegate onChallengeComplete)
	{
		id = info.ChallengeId;
		name = info.Name;
		description = info.Description;
		messageType = info.MessageType;
		rewardIcon = info.IconPath;
		this.onChallengeComplete = onChallengeComplete;
		forceClientEvents = info.ForceClientEvents;
		challengeCounter = counter;
		challengeStatus = ChallengeStatus.InProgress;
		if (!Singleton<ChallengeMessageType>.instance.IsType(messageType))
		{
			CspUtils.DebugLog("ChallengeBase::Initialize() - message id <" + messageType + "> is not a valid message id");
		}
	}

	public virtual void HandleChallengeEvent(ChallengeEventMessage msg)
	{
		throw new NotImplementedException();
	}

	public virtual void ChallengeVerifiedComplete()
	{
		challengeStatus = ChallengeStatus.Completed;
	}

	public void LogChallengeStatus()
	{
		if (ChallengeManager.LogChallenges)
		{
			string text = Name;
			if (AppShell.Instance != null && AppShell.Instance.stringTable != null)
			{
				text = AppShell.Instance.stringTable[Name];
			}
			if (Status == ChallengeStatus.InProgress)
			{
				CspUtils.DebugLog(string.Format("Challenge Log: <Class={0}> <Name={1}> <Id={2}> <Status={3}> <Progress={4}>", GetType(), text, Id, Status, GetProgressLogString()));
			}
			else
			{
				CspUtils.DebugLog(string.Format("Challenge Log: <Class={0}> <Name={1}> <Id={2}> <Status={3}>", GetType(), text, Id, Status));
			}
		}
	}

	public virtual void Ready()
	{
	}

	protected virtual string GetProgressLogString()
	{
		return string.Empty;
	}

	public virtual void Dispose()
	{
		challengeCounter.SetCounter(0L, SHSCounterType.ReportingMethodEnum.WebService);
	}
}
