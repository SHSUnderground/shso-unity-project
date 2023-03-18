using UnityEngine;

public class BehaviorLoopAnimate : BehaviorBase
{
	public delegate bool LoopEndCondition();

	protected string animation;

	protected OnBehaviorDone onAnimationDone;

	protected LoopEndCondition loopEndCondition;

	protected int loopCount = -1;

	private float lastAnimTime = -1f;

	private int numTimesLooped;

	private WrapMode originalWrapMode;

	public void Initialize(string animation, OnBehaviorDone onDone)
	{
		Initialize(animation, 1, onDone);
	}

	public void Initialize(string animation, LoopEndCondition endCondition, OnBehaviorDone onDone)
	{
		loopEndCondition = endCondition;
		Initialize(animation, loopCount, onDone);
	}

	public void Initialize(string animation, int loopCount, OnBehaviorDone onDone)
	{
		this.animation = animation;
		this.loopCount = loopCount;
		onAnimationDone = onDone;
		if (animationComponent[this.animation] != null)
		{
			originalWrapMode = animationComponent[this.animation].wrapMode;
			animationComponent[this.animation].wrapMode = WrapMode.Loop;
			animationComponent.CrossFade(this.animation);
		}
		else
		{
			CspUtils.DebugLog("Object: " + charGlobals.gameObject.name + " Animation: " + this.animation + " does not exist on entity: " + this);
			charGlobals.behaviorManager.endBehavior();
		}
	}

	public override void behaviorUpdate()
	{
		float num = animationComponent[animation].length * (float)(numTimesLooped + 1);
		float time = animationComponent[animation].time;
		if (time >= num && lastAnimTime < num)
		{
			numTimesLooped++;
			if (loopCount == 0 || (loopEndCondition != null && loopEndCondition()))
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
			else if (loopCount > 0)
			{
				loopCount--;
			}
		}
		lastAnimTime = time;
		base.behaviorUpdate();
	}

	public override void behaviorEnd()
	{
		if (animationComponent[animation] != null)
		{
			animationComponent[animation].wrapMode = originalWrapMode;
			animationComponent[animation].time = 0f;
		}
		base.behaviorEnd();
	}
}
