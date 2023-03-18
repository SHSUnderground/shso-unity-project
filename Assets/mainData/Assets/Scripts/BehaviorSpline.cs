using System.Collections.Generic;

public class BehaviorSpline : BehaviorSplineBase
{
	public void Initialize(SplineController spline, bool followRotations, OnBehaviorDone onDone, bool ignoreCollision, bool freeFall)
	{
		base.spline = spline;
		base.followRotations = followRotations;
		charController = charGlobals.characterController;
		effectsList = charGlobals.effectsList;
		activeEffects = new Dictionary<string, EffectSequence>();
		onBehaviorDone = onDone;
		base.ignoreCollision = ignoreCollision;
		base.freeFall = freeFall;
		state = States.Spline;
		InitSplineMovement();
	}

	public void SnapToGroundOnLand(bool snap)
	{
		snapToGroundOnLand = snap;
	}

	public override void behaviorUpdate()
	{
		States state = base.state;
		if (state == States.Spline)
		{
			StateSpline(owningObject);
		}
		else
		{
			CspUtils.DebugLog("Unexpected state");
			if (onBehaviorDone != null)
			{
				onBehaviorDone(owningObject);
			}
			if (charGlobals.behaviorManager.getBehavior() == this)
			{
				charGlobals.behaviorManager.endBehavior();
			}
		}
		base.behaviorUpdate();
	}
}
