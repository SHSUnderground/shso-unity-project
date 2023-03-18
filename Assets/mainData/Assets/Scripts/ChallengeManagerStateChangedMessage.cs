using System.Runtime.CompilerServices;

public class ChallengeManagerStateChangedMessage : ShsEventMessage
{
	[CompilerGenerated]
	private ChallengeManager.ChallengeManagerStateEnum _003CLastState_003Ek__BackingField;

	[CompilerGenerated]
	private ChallengeManager.ChallengeManagerStateEnum _003CNewState_003Ek__BackingField;

	public ChallengeManager.ChallengeManagerStateEnum LastState
	{
		[CompilerGenerated]
		get
		{
			return _003CLastState_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CLastState_003Ek__BackingField = value;
		}
	}

	public ChallengeManager.ChallengeManagerStateEnum NewState
	{
		[CompilerGenerated]
		get
		{
			return _003CNewState_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CNewState_003Ek__BackingField = value;
		}
	}

	public ChallengeManagerStateChangedMessage(ChallengeManager.ChallengeManagerStateEnum lastState, ChallengeManager.ChallengeManagerStateEnum newState)
	{
		LastState = lastState;
		NewState = newState;
	}
}
