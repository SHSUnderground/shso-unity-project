using System;
using UnityEngine;

public class BehaviorHitByObject : BehaviorBase
{
	public enum ReactionSize
	{
		Small,
		Large,
		XtraLarge
	}

	protected const float GETUP_TIME = 0.45f;

	protected float landedTime;

	protected float getupTime;

	protected Vector3 lookTarget;

	protected bool isGettingUp;

	protected float endTime;

	protected OnBehaviorDone onBehaviorDone;

	protected ReactionSize reactionSize;

	protected bool lookAtTarget;

	private bool isDone;

	public override bool Paused
	{
		get
		{
			return paused;
		}
		set
		{
			paused = value;
			if (paused)
			{
				charGlobals.motionController.setForcedVelocity(Vector3.zero, 0f);
			}
		}
	}

	public bool Initialize(Vector3 impactPosition, float force, Vector3 direction, bool useDirection, bool applyMotion, OnBehaviorDone doneCallback)
	{
		string text = "recoil_small";
		lookAtTarget = true;
		float num = Vector3.Dot(owningObject.transform.forward, direction);
		if (useDirection && num >= 0f)
		{
			text = "recoil_back";
			lookAtTarget = false;
		}
		else
		{
			reactionSize = ReactionSize.Small;
			if (force > 20f)
			{
				reactionSize = ReactionSize.Large;
			}
			if (force > 50f)
			{
				reactionSize = ReactionSize.XtraLarge;
			}
			switch (reactionSize)
			{
			case ReactionSize.Small:
				text = "recoil_small";
				break;
			case ReactionSize.Large:
				text = "recoil_big";
				break;
			case ReactionSize.XtraLarge:
				text = ((!(animationComponent["recoil_knockdown"] != null)) ? "recoil_death" : "recoil_knockdown");
				break;
			}
		}
		if (animationComponent[text] == null)
		{
			return false;
		}
		if (animationComponent.IsPlaying(text))
		{
			animationComponent.Rewind(text);
			animationComponent.Play(text);
		}
		else
		{
			animationComponent[text].time = 0f;
			animationComponent[text].wrapMode = WrapMode.ClampForever;
			animationComponent.CrossFade(text, 0.15f);
		}
		landedTime = animationComponent[text].length;
		getupTime = landedTime + 0.45f;
		if (lookAtTarget)
		{
			lookTarget = impactPosition - owningObject.transform.position;
		}
		isGettingUp = false;
		onBehaviorDone = doneCallback;
		isDone = false;
		charGlobals.motionController.setDestination(owningObject.gameObject.transform.position);
		if (applyMotion)
		{
			charGlobals.motionController.setForcedVelocity(direction, landedTime);
		}
		return true;
	}

	public override void behaviorUpdate()
	{
		if (Paused)
		{
			return;
		}
		if (elapsedTime < landedTime && lookAtTarget)
		{
			charGlobals.motionController.rotateTowards(lookTarget);
		}
		else if (isGettingUp && elapsedTime >= endTime)
		{
			isDone = true;
			if (onBehaviorDone != null)
			{
				onBehaviorDone(owningObject.gameObject);
			}
			else
			{
				charGlobals.behaviorManager.endBehavior();
			}
		}
		else if (elapsedTime >= getupTime && !isGettingUp)
		{
			isGettingUp = true;
			if (reactionSize == ReactionSize.XtraLarge)
			{
				string text = null;
				text = ((!(animationComponent["recoil_getup"] == null)) ? "recoil_getup" : "recoil_big");
				if (text == null || animationComponent[text] == null)
				{
					isDone = true;
					if (onBehaviorDone != null)
					{
						onBehaviorDone(owningObject.gameObject);
					}
					else
					{
						charGlobals.behaviorManager.endBehavior();
					}
				}
				else
				{
					endTime = getupTime + animationComponent[text].length;
					animationComponent.Rewind(text);
					animationComponent.Play(text);
				}
			}
			else
			{
				isDone = true;
				if (onBehaviorDone != null)
				{
					onBehaviorDone(owningObject.gameObject);
				}
				else
				{
					charGlobals.behaviorManager.endBehavior();
				}
			}
		}
		base.behaviorUpdate();
	}

	public override void behaviorLateUpdate()
	{
		if (!charGlobals.motionController.IsForcedVelocity())
		{
			charGlobals.motionController.performRootMotion();
		}
		base.behaviorLateUpdate();
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return isDone;
	}
}
