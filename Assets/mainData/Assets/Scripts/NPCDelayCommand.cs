using System;
using UnityEngine;

[Serializable]
public class NPCDelayCommand : NPCCommandBase
{
	public float delayTime = 2f;

	public NPCDelayCommand()
	{
		type = NPCCommandTypeEnum.Delay;
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
		return (!(Time.time - startTime > delayTime)) ? NPCCommandResultEnum.InProgress : NPCCommandResultEnum.Completed;
	}

	public override string ToString()
	{
		return type.ToString() + ": (" + delayTime + ")";
	}
}
