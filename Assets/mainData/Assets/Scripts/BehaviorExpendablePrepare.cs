using System;

public class BehaviorExpendablePrepare : BehaviorBase
{
	private float kTimeoutDurationSeconds = 3f;

	public override bool allowUserInput()
	{
		return false;
	}

	public override bool useMotionController()
	{
		return false;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		if (newBehaviorType == typeof(BehaviorEffectExpendable))
		{
			return true;
		}
		return false;
	}

	public override void behaviorUpdate()
	{
		base.behaviorUpdate();
		if (elapsedTime > kTimeoutDurationSeconds)
		{
			charGlobals.behaviorManager.endBehavior();
		}
	}
}
