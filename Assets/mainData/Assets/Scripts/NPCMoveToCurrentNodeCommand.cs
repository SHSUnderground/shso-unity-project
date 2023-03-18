public class NPCMoveToCurrentNodeCommand : NPCCommandBase
{
	public bool run;

	public float moveSpeed = 2.6f;

	public NPCMoveToCurrentNodeCommand()
	{
		type = NPCCommandTypeEnum.MoveToCurrentNode;
	}

	public override void Start()
	{
		Start(null);
	}

	public override void Start(params object[] initValues)
	{
		NPCMoveToNodeCommand nPCMoveToNodeCommand = new NPCMoveToNodeCommand();
		nPCMoveToNodeCommand.targetNode = aiController.CurrentNode;
		nPCMoveToNodeCommand.currentNode = aiController.CurrentNode;
		nPCMoveToNodeCommand.run = run;
		nPCMoveToNodeCommand.moveSpeed = moveSpeed;
		manager.AddCommand(nPCMoveToNodeCommand);
	}

	public override void Suspend()
	{
		base.Suspend();
	}

	public override void Resume()
	{
		base.Resume();
	}

	public override void Completed()
	{
		base.Completed();
	}

	public override NPCCommandResultEnum Update()
	{
		return NPCCommandResultEnum.Completed;
	}
}
