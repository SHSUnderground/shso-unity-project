public class TutorialActionMessage : ShsEventMessage
{
	public enum ActionTypeEnum
	{
		Intro,
		Movement,
		MovementJump,
		MovementTickets,
		ShowHub,
		ActivateHub,
		Complete
	}

	private object caller;

	private ActionTypeEnum completedActionType;

	public object Caller
	{
		get
		{
			return caller;
		}
	}

	public ActionTypeEnum CompletedActionType
	{
		get
		{
			return completedActionType;
		}
		set
		{
			completedActionType = value;
		}
	}

	public TutorialActionMessage(object Caller, ActionTypeEnum ActionType)
	{
		caller = Caller;
		completedActionType = ActionType;
	}
}
