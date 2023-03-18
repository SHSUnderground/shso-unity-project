using System;
using UnityEngine;

public class BehaviorAnimate : BehaviorBase
{
	protected OnBehaviorDone onAnimationDone;

	protected string animation;

	public void Initialize(string animation, OnBehaviorDone onDone)
	{
		this.animation = animation;
		onAnimationDone = onDone;
		try
		{
			animationComponent[this.animation].wrapMode = WrapMode.ClampForever;
			animationComponent.CrossFade(this.animation);
		}
		catch (Exception ex)
		{
			CspUtils.DebugLog("Object: " + charGlobals.gameObject.name + " Animation: " + this.animation + " does not exist on entity: " + this + ". Error: " + ex.Message);
			charGlobals.behaviorManager.endBehavior();
		}
	}

	public override void behaviorUpdate()
	{
		if (animationComponent[animation].time >= animationComponent[animation].length)
		{
			if (onAnimationDone != null)
			{
				onAnimationDone(owningObject);
			}
			if (charGlobals.behaviorManager.getBehavior() == this)
			{
				charGlobals.behaviorManager.endBehavior();
				return;
			}
		}
		base.behaviorUpdate();
	}

	public override bool allowUserInput()
	{
		return false;
	}

	public override bool useMotionController()
	{
		return false;
	}
}
