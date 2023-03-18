using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorSpawnAnimate : BehaviorBase
{
	protected enum States
	{
		Uninitialized,
		Spawning,
		Done
	}

	protected States state;

	protected ShsCharacterController charController;

	protected CharacterMotionController motionController;

	protected GameObject source;

	protected Vector3 destination;

	protected EffectSequenceList effectsList;

	protected bool followRotations;

	protected float speed = 1f;

	protected float timeSpeed = 1f;

	protected float time;

	protected int idxEffect;

	protected int idxAnim;

	protected Dictionary<string, EffectSequence> activeEffects;

	protected OnBehaviorDone onBehaviorDone;

	protected bool ignoreCollision;

	protected Vector3 spawnSourceUp;

	protected Vector3 toSpawnSource;

	protected Vector3 lookVector;

	protected bool useExtraRotation;

	protected Quaternion startRotation;

	protected Quaternion endRotation;

	protected List<Vector3> velocities;

	protected Vector3 velocity = Vector3.zero;

	protected BrawlerSpawnSource spawnSourceComp;

	protected float animationOriginalSpeed;

	public void Initialize(GameObject Source, Vector3 Destination, bool FollowRotations, OnBehaviorDone OnDone, bool IgnoreCollision)
	{
		source = Source;
		destination = Destination;
		followRotations = FollowRotations;
		charController = charGlobals.characterController;
		motionController = charGlobals.motionController;
		effectsList = charGlobals.effectsList;
		activeEffects = new Dictionary<string, EffectSequence>();
		onBehaviorDone = OnDone;
		ignoreCollision = IgnoreCollision;
		useExtraRotation = false;
		InitStateSpawning();
	}

	public override void behaviorBegin()
	{
		base.behaviorBegin();
	}

	public override void behaviorUpdate()
	{
		switch (state)
		{
		case States.Spawning:
			StateSpawning();
			break;
		case States.Done:
			CheckEnd();
			break;
		default:
			CspUtils.DebugLog("Unexpected state");
			EndState();
			break;
		case States.Uninitialized:
			break;
		}
		base.behaviorUpdate();
	}

	public override void behaviorEnd()
	{
		if (spawnSourceComp != null && spawnSourceComp.spawnAnimation != string.Empty && animationComponent[spawnSourceComp.spawnAnimation] != null && spawnSourceComp.spawnAnimationSpeed != 1f)
		{
			animationComponent[spawnSourceComp.spawnAnimation].speed = animationOriginalSpeed;
		}
		if ((charGlobals.spawnData.spawnType & CharacterSpawn.Type.AI) != 0 && (charGlobals.spawnData.spawnType & CharacterSpawn.Type.Boss) == 0)
		{
			AppShell.Instance.EventMgr.Fire(owningObject, new EntitySpawnAnimationCompleteMessage(owningObject));
		}
		motionController.setForcedVelocityAllow(true);
		combatController.setRecoilAllow(true);
		base.behaviorEnd();
	}

	public override bool allowUserInput()
	{
		return false;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return newBehaviorType == typeof(BehaviorFreeFall);
	}

	public override bool useMotionController()
	{
		return false;
	}

	public override bool useMotionControllerGravity()
	{
		return false;
	}

	public override void motionBeganMoving()
	{
	}

	public override void motionStoppedMoving()
	{
	}

	public override void motionJumped()
	{
	}

	public override void motionBeganFalling()
	{
	}

	public override void motionLanded()
	{
	}

	protected void InitStateSpawning()
	{
		motionController.setForcedVelocityAllow(false);
		combatController.setRecoilAllow(false);
		spawnSourceUp = default(Vector3);
		toSpawnSource = default(Vector3);
		float vectorLength = 0f;
		BrawlerSpawnSource.InitMotionVectors(source, destination, ref spawnSourceUp, ref toSpawnSource, ref vectorLength);
		lookVector = new Vector3(toSpawnSource.x, 0f, toSpawnSource.z);
		state = States.Spawning;
		idxAnim = 0;
		idxEffect = 0;
		time = 0f;
		float f = Vector3.Angle(spawnSourceUp, toSpawnSource);
		float num = Mathf.Abs(f) / 180f;
		float num2 = vectorLength * (1f - num) + Mathf.Sqrt(2f * vectorLength * vectorLength) * num;
		if (num2 < 0.001f)
		{
			num2 = 0.001f;
		}
		timeSpeed = 1f / num2;
		spawnSourceComp = (source.GetComponent(typeof(BrawlerSpawnSource)) as BrawlerSpawnSource);
		if (spawnSourceComp != null)
		{
			speed = spawnSourceComp.spawnInSpeed;
			if (spawnSourceComp.spawnAnimation != string.Empty)
			{
				if (animationComponent[spawnSourceComp.spawnAnimation] == null)
				{
					CspUtils.DebugLog("Spawn source " + spawnSourceComp.gameObject.name + " calls for animation " + spawnSourceComp.spawnAnimation + " that " + owningObject.name + " does not have");
				}
				else
				{
					if (spawnSourceComp.spawnAnimationSpeed != 1f)
					{
						animationOriginalSpeed = animationComponent[spawnSourceComp.spawnAnimation].speed;
						animationComponent[spawnSourceComp.spawnAnimation].speed *= spawnSourceComp.spawnAnimationSpeed;
					}
					if (spawnSourceComp.playFullAnimation && animationComponent[spawnSourceComp.spawnAnimation].speed / animationComponent[spawnSourceComp.spawnAnimation].length < speed * timeSpeed)
					{
						timeSpeed = animationComponent[spawnSourceComp.spawnAnimation].speed / (animationComponent[spawnSourceComp.spawnAnimation].length * speed);
					}
					animationComponent.Rewind(spawnSourceComp.spawnAnimation);
					animationComponent.Play(spawnSourceComp.spawnAnimation);
				}
			}
			if (spawnSourceComp.startAngle != 0f || spawnSourceComp.endAngle != 0f)
			{
				useExtraRotation = true;
				startRotation = Quaternion.Euler(spawnSourceComp.startAngle, 0f, 0f);
				endRotation = Quaternion.Euler(spawnSourceComp.endAngle, 0f, 0f);
			}
			if (spawnSourceComp.startRotation != Vector3.zero || spawnSourceComp.endRotation != Vector3.zero)
			{
				useExtraRotation = true;
				startRotation = Quaternion.Euler(spawnSourceComp.startRotation);
				endRotation = Quaternion.Euler(spawnSourceComp.endRotation);
			}
		}
		velocities = new List<Vector3>(5);
	}

	protected void StateSpawning()
	{
		time += Time.deltaTime * speed * timeSpeed;
		if (time >= 1f)
		{
			EndState();
			return;
		}
		Vector3 position = owningObject.transform.position;
		Vector3 a = BrawlerSpawnSource.EvalPosition(time, source.transform.position, spawnSourceUp, toSpawnSource);
		velocities.Add((a - position) / Time.deltaTime);
		if (velocities.Count > velocities.Capacity - 1)
		{
			velocities.RemoveAt(0);
		}
		if (followRotations)
		{
			owningObject.transform.rotation = Quaternion.LookRotation(lookVector);
			if (useExtraRotation)
			{
				Quaternion rhs = Quaternion.Lerp(startRotation, endRotation, time);
				owningObject.transform.rotation = owningObject.transform.rotation * rhs;
			}
		}
		if (ignoreCollision)
		{
			owningObject.transform.localPosition += a - position;
		}
		else
		{
			charController.Move(a - position);
		}
	}

	protected void EndState()
	{
		if (motionController != null)
		{
			motionController.updateLookDirection();
			motionController.setSpawnAnimation(null);
			motionController.setDestination(destination);
		}
		velocity = Vector3.zero;
		foreach (Vector3 velocity2 in velocities)
		{
			velocity += velocity2;
		}
		velocity /= (float)velocities.Count;
		if (spawnSourceComp != null && spawnSourceComp.freeFallOnCompletion)
		{
			BehaviorFreeFall behaviorFreeFall = charGlobals.behaviorManager.requestChangeBehavior(typeof(BehaviorFreeFall), false) as BehaviorFreeFall;
			if (behaviorFreeFall != null)
			{
				behaviorFreeFall.Initialize(velocity, onBehaviorDone);
				behaviorFreeFall.behaviorUpdate();
				return;
			}
			CspUtils.DebugLog("Failed to switch to free fall behavior");
			if (onBehaviorDone != null)
			{
				onBehaviorDone(owningObject);
			}
			if (charGlobals.behaviorManager.getBehavior() == this)
			{
				charGlobals.behaviorManager.endBehavior();
			}
		}
		else
		{
			state = States.Done;
		}
	}

	protected void CheckEnd()
	{
		if (spawnSourceComp == null || spawnSourceComp.spawnAnimation == string.Empty || animationComponent[spawnSourceComp.spawnAnimation] == null || !animationComponent.IsPlaying(spawnSourceComp.spawnAnimation))
		{
			if (onBehaviorDone != null)
			{
				onBehaviorDone(owningObject);
			}
			charGlobals.behaviorManager.endBehavior();
		}
	}
}
