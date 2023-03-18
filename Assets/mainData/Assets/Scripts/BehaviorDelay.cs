using System;
using UnityEngine;

public class BehaviorDelay : BehaviorBase
{
	public delegate void OnDoneDelegate();

	protected float delay = -1f;

	protected OnDoneDelegate onDone;

	public virtual void Initialize(float delay, Vector3 lookAtDir, OnDoneDelegate onDone)
	{
		this.delay = delay;
		this.onDone = onDone;
		charGlobals.motionController.setNewFacing(lookAtDir);
	}

	public override void behaviorUpdate()
	{
		base.behaviorUpdate();
		if (elapsedTime >= delay)
		{
			if (onDone != null)
			{
				onDone();
			}
			if (charGlobals.behaviorManager.getBehavior() == this)
			{
				charGlobals.behaviorManager.endBehavior();
			}
		}
	}

	public override bool allowUserInput()
	{
		return false;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return false;
	}
}
