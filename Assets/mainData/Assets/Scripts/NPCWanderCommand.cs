using System;
using UnityEngine;

[Serializable]
public class NPCWanderCommand : NPCCommandBase
{
	public float wanderRadius = 12f;

	public float wanderTime = 2f;

	public NPCWanderCommand()
	{
		type = NPCCommandTypeEnum.Wander;
	}

	public override void Start()
	{
		Start(null);
	}

	public override void Start(params object[] initValues)
	{
		base.Start(initValues);
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
		return (!(Time.time - startTime > wanderTime)) ? NPCCommandResultEnum.InProgress : NPCCommandResultEnum.Completed;
	}

	private void wanderCompleted(GameObject objMoving)
	{
	}

	public override string ToString()
	{
		return type.ToString() + ": Radius=" + wanderRadius + " Time=" + wanderTime;
	}
}
