public class NPCTurnToCommand : NPCCommandBase
{
	public NPCTurnToCommand()
	{
		type = NPCCommandTypeEnum.TurnTo;
		interruptable = false;
	}

	public override void Start()
	{
		Start(null);
	}

	public override void Start(params object[] initValues)
	{
		if (target == null)
		{
			CspUtils.DebugLog("No target to turn towards.");
			isDone = true;
		}
		else
		{
			BehaviorTurnTo behaviorTurnTo = behaviorManager.requestChangeBehavior(typeof(BehaviorTurnTo), false) as BehaviorTurnTo;
			behaviorTurnTo.Initialize(target.transform.position, delegate
			{
				isDone = true;
			});
		}
	}

	public override void Suspend()
	{
		base.Suspend();
	}

	public override void Resume()
	{
		base.Resume();
	}

	public override NPCCommandResultEnum Update()
	{
		return (!isDone) ? NPCCommandResultEnum.InProgress : NPCCommandResultEnum.Completed;
	}

	public override string ToString()
	{
		return type.ToString() + ": " + ((!(target != null)) ? "null" : target.gameObject.name);
	}
}
