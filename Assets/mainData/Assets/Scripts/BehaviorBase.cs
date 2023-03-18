using System;
using UnityEngine;

public class BehaviorBase
{
	public delegate void OnBehaviorDone(GameObject obj);

	protected GameObject owningObject;

	protected GameObject targetObject;

	protected CharacterGlobals charGlobals;

	protected Animation animationComponent;

	protected CombatController combatController;

	protected NetworkComponent networkComponent;

	protected float elapsedTime;

	protected float startTime;

	protected bool paused;

	public virtual bool Paused
	{
		get
		{
			return paused;
		}
		set
		{
			paused = value;
		}
	}

	public void behaviorSetup(GameObject newOwningObject, CharacterGlobals newCharGlobals)
	{
		owningObject = newOwningObject;
		charGlobals = newCharGlobals;
		animationComponent = charGlobals.animationComponent;
		combatController = charGlobals.combatController;
		networkComponent = charGlobals.networkComponent;
		paused = false;
	}

	public void setTarget(GameObject newTargetObject)
	{
		targetObject = newTargetObject;
	}

	public GameObject getTarget()
	{
		return targetObject;
	}

	public string getAnimationName(string animationKey)
	{
		return charGlobals.behaviorManager.GetAnimationName(animationKey);
	}

	public float clampAnimationSpeed(string animationKey, float minTime, float maxTime)
	{
		if (animationComponent != null)
		{
			AnimationState animationState = animationComponent[animationKey];
			if (animationState != null)
			{
				float speed = animationState.speed;
				if (charGlobals.spawnData != null && (charGlobals.spawnData.spawnType & CharacterSpawn.Type.Player) != 0)
				{
					animationState.speed = ActionTimesDefinition.Instance.ClampActionTime(animationState.speed, animationState.length, minTime, maxTime);
				}
				return speed;
			}
			return 1f;
		}
		return 1f;
	}

	public void restoreAnimationSpeed(string animationKey, float speed)
	{
		if (animationComponent != null)
		{
			AnimationState animationState = animationComponent[animationKey];
			if (animationState != null)
			{
				animationState.speed = speed;
			}
		}
	}

	public virtual void behaviorBegin(BehaviorBase oldBehavior)
	{
		behaviorBegin();
	}

	public virtual void behaviorBegin()
	{
		startTime = Time.time;
		elapsedTime = 0f;
	}

	public virtual void behaviorUpdate()
	{
		elapsedTime += Time.deltaTime;
	}

	public virtual void behaviorLateUpdate()
	{
	}

	public virtual void behaviorEnd(BehaviorBase newBehavior)
	{
		behaviorEnd();
	}

	public virtual void behaviorEnd()
	{
	}

	public virtual void behaviorCancel()
	{
	}

	public virtual bool behaviorEndOnCutScene()
	{
		return false;
	}

	public virtual bool allowUserInput()
	{
		return true;
	}

	public virtual void userInputOverride()
	{
	}

	public virtual bool allowInterrupt(Type newBehaviorType)
	{
		return true;
	}

	public virtual bool allowForcedInterrupt(Type newBehaviorType)
	{
		return true;
	}

	public virtual bool useMotionController()
	{
		return !Paused;
	}

	public virtual bool useMotionControllerRotate()
	{
		return !Paused;
	}

	public virtual bool useMotionControllerGravity()
	{
		return !Paused;
	}

	public virtual float GetBehaviorDuration()
	{
		return 0f;
	}

	public virtual void motionBeganMoving()
	{
	}

	public virtual void motionStoppedMoving()
	{
	}

	public virtual void motionArrived()
	{
	}

	public virtual void motionJumped()
	{
	}

	public virtual void motionJumpCancelled()
	{
	}

	public virtual void motionBeganFalling()
	{
	}

	public virtual void motionLanded()
	{
	}

	public virtual void motionCollided()
	{
	}

	public virtual void destinationChanged()
	{
	}

	public virtual void animationOverriden(string baseAnimation, string overrideAnimation)
	{
		if (animationComponent.IsPlaying(baseAnimation))
		{
			animationComponent.CrossFade(overrideAnimation, 0.2f);
		}
	}

	public virtual void HitByEnemy(CombatController enemy)
	{
	}
}
