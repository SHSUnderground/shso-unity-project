using System.Collections.Generic;
using UnityEngine;

public class NPCMoveToNextNodeCommand : NPCCommandBase
{
	public bool run;

	public float moveSpeed = 2.6f;

	public NPCMoveToNextNodeCommand()
	{
		type = NPCCommandTypeEnum.MoveToNextNode;
	}

	public override void Start()
	{
		Start(null);
	}

	public override void Start(params object[] initValues)
	{
		NPCPathNode currentNode = aiController.CurrentNode;
		if (currentNode.nextNodes.Length > 0)
		{
			int num = Random.Range(0, currentNode.nextNodes.Length);
			NPCPathNode targetNode = currentNode.nextNodes[num];
			NPCMoveToNodeCommand nPCMoveToNodeCommand = new NPCMoveToNodeCommand();
			nPCMoveToNodeCommand.targetNode = targetNode;
			nPCMoveToNodeCommand.currentNode = aiController.CurrentNode;
			nPCMoveToNodeCommand.run = run;
			nPCMoveToNodeCommand.moveSpeed = moveSpeed;
			manager.AddCommand(nPCMoveToNodeCommand);
		}
		else
		{
			manager.ProblemHistory.Add(new KeyValuePair<int, string>(commandSet, " Current Node <" + currentNode.name + "> has no next node to traverse to!"));
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
}
