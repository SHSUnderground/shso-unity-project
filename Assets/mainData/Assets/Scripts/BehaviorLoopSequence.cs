using System;
using UnityEngine;

public class BehaviorLoopSequence : BehaviorBase
{
	protected OnBehaviorDone onAnimationDone;

	protected OnBehaviorDone onAnimationCancel;

	protected WrapMode oldWrapMode;

	protected string animation;

	protected float animateTime = 1f;

	protected float currentTime;

	protected bool allowInput = true;

	public void Initialize(string animation, OnBehaviorDone onDone, float duration)
	{
		Initialize(animation, onDone, null, duration);
	}

	public void Initialize(string animation, OnBehaviorDone onDone, float duration, bool allowInput)
	{
		Initialize(animation, onDone, null, duration, allowInput);
	}

	public void Initialize(string animation, OnBehaviorDone onDone, OnBehaviorDone onCancel, float duration)
	{
		Initialize(animation, onDone, onCancel, duration, true);
	}

	public void Initialize(string animation, OnBehaviorDone onDone, OnBehaviorDone onCancel, float duration, bool allowInput)
	{
		this.animation = animation;
		onAnimationDone = onDone;
		onAnimationCancel = onCancel;
		animateTime = duration;
		currentTime = 0f;
		this.allowInput = allowInput;
		try
		{
			oldWrapMode = animationComponent[this.animation].wrapMode;
			animationComponent[this.animation].wrapMode = WrapMode.Loop;
			animationComponent.CrossFade(this.animation);
		}
		catch (Exception ex)
		{
			CspUtils.DebugLog("Animation: " + this.animation + " does not exist on entity: " + this + ". Error: " + ex.Message);
		}
	}

	public override void behaviorUpdate()
	{
		currentTime += Time.deltaTime;
		if (currentTime >= animateTime)
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

	public override void behaviorEnd()
	{
		if (currentTime < animateTime && onAnimationCancel != null)
		{
			onAnimationCancel(owningObject);
		}
		base.behaviorEnd();
	}

	public override bool allowUserInput()
	{
		return allowInput;
	}

	public override bool useMotionController()
	{
		return false;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		if (newBehaviorType.BaseType == typeof(BehaviorRecoil) || newBehaviorType.BaseType == typeof(BehaviorAttackBase))
		{
			return true;
		}
		return false;
	}
}
