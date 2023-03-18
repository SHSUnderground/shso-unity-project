using System.Collections.Generic;

public class NPCStartCommand : NPCCommandBase
{
	private NPCPath appointedPath;

	public NPCStartCommand()
	{
		interruptable = false;
		type = NPCCommandTypeEnum.Start;
	}

	public override void Start()
	{
		Start(null);
	}

	public override void Start(params object[] initValues)
	{
		SpawnData component = Utils.GetComponent<SpawnData>(gameObject);
		if (!(component == null) && !(component.spawner == null) && component.spawner is NPCSpawn)
		{
			appointedPath = ((NPCSpawn)component.spawner).path;
			if (appointedPath == null)
			{
				manager.ProblemHistory.Add(new KeyValuePair<int, string>(commandSet, " No path assigned. Must assign a path to NPCs"));
				return;
			}
			aiController.AssignedPath = appointedPath;
			aiController.CurrentNode = appointedPath.GetStartNode();
			NPCMoveToCurrentNodeCommand command = new NPCMoveToCurrentNodeCommand();
			manager.AddCommand(command);
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

	public override void Completed()
	{
		base.Completed();
	}

	public override NPCCommandResultEnum Update()
	{
		return NPCCommandResultEnum.Completed;
	}

	public override string ToString()
	{
		return type.ToString() + ": " + ((!(appointedPath != null)) ? "null" : appointedPath.gameObject.name);
	}
}
