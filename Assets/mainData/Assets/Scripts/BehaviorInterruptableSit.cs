using System;
using UnityEngine;

public class BehaviorInterruptableSit : BehaviorSit
{
	protected bool listenForInput;

	protected Vector3 positionOverride;

	protected Quaternion rotationOverride;

	private bool interrupted;

	public void Initialize(bool localOnly, FinishedDelegate onFinishedStanding)
	{
		Initialize(localOnly);
		base.OnFinishedStanding += onFinishedStanding;
	}

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		positionOverride = owningObject.transform.position;
		rotationOverride = owningObject.transform.rotation;
	}

	public override void behaviorUpdate()
	{
		base.behaviorUpdate();
		if (sitting && !standing)
		{
			listenForInput = true;
		}
		if (!interrupted && charGlobals.behaviorManager.getQueuedBehavior() != null)
		{
			InterruptSit();
		}
	}

	public override bool allowUserInput()
	{
		return listenForInput;
	}

	public override void destinationChanged()
	{
		InterruptSit();
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return false;
	}

	protected void InterruptSit()
	{
		interrupted = true;
		listenForInput = false;
		stand();
	}
}
