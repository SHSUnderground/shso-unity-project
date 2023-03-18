public class NPCWaitCommand : NPCCommandBase
{
	private bool finished;

	public NPCWaitCommand()
	{
		type = NPCCommandTypeEnum.Wait;
	}

	public void Finish()
	{
		finished = true;
	}

	public override NPCCommandResultEnum Update()
	{
		return (!finished) ? NPCCommandResultEnum.InProgress : NPCCommandResultEnum.Completed;
	}

	public override void Suspend()
	{
		finished = true;
		base.Suspend();
	}
}
