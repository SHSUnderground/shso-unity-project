using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorSplineBase : BehaviorBase
{
	protected enum States
	{
		Uninitialized,
		Spline,
		SplineTeleport
	}

	protected States state;

	protected ShsCharacterController charController;

	protected SplineController spline;

	protected EffectSequenceList effectsList;

	protected bool followRotations;

	protected float speed;

	protected float time;

	protected List<Vector3> velocities;

	protected int idxEffect;

	protected int idxAnim;

	protected Vector3 velocity = Vector3.zero;

	protected Dictionary<string, EffectSequence> activeEffects;

	protected OnBehaviorDone onBehaviorDone;

	protected bool ignoreCollision;

	protected bool freeFall = true;

	protected bool snapToGroundOnLand;

	protected readonly float snapRayLength = 10f;

	protected EffectSequence idleSequence;

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		idleSequence = charGlobals.effectsList.PlaySequence("emote_idle_sequence");
	}

	public override void behaviorEnd()
	{
		if (idleSequence != null)
		{
			UnityEngine.Object.Destroy(idleSequence.gameObject);
		}
		if (activeEffects != null)
		{
			foreach (EffectSequence value in activeEffects.Values)
			{
				if (value != null && (value.Lifetime <= 0f || value.Looping))
				{
					value.Cancel();
				}
			}
		}
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

	protected void InitSplineMovement()
	{
		time = 0f;
		speed = spline.ArcLength() / spline.traversalTime;
		velocities = new List<Vector3>(5);
		velocity = Vector3.zero;
		idxAnim = 0;
		idxEffect = 0;
	}

	protected void StateSpline(GameObject obj)
	{
		float desiredSpeed = spline.GetDesiredSpeed(time * spline.traversalTime);
		time = spline.TimeForDistance(time, desiredSpeed * Time.deltaTime);
		List<SplineEffectSequence> effects = spline.GetEffects();
		if (effects != null)
		{
			while (idxEffect < effects.Count)
			{
				SplineEffectSequence splineEffectSequence = effects[idxEffect];
				if (!(time * spline.traversalTime >= splineEffectSequence.time))
				{
					break;
				}
				if (!splineEffectSequence.turnOff)
				{
					EffectSequence sequence;
					if (effectsList.TryGetLogicalEffectSequence(splineEffectSequence.name, out sequence) || effectsList.TryGetEffectSequenceByName(splineEffectSequence.name, out sequence))
					{
						sequence.Initialize(null, null, null);
						sequence.AssignCreator(charGlobals);
						sequence.StartSequence();
						try
						{
							activeEffects.Add(splineEffectSequence.name, sequence);
						}
						catch (ArgumentException)
						{
							CspUtils.DebugLog("Unable to add: " + splineEffectSequence.name);
						}
					}
				}
				else
				{
					EffectSequence value = null;
					if (activeEffects.TryGetValue(splineEffectSequence.name, out value))
					{
						activeEffects.Remove(splineEffectSequence.name);
						if (value != null)
						{
							value.Cancel();
						}
					}
				}
				idxEffect++;
			}
		}
		List<SplineAnimation> animations = spline.GetAnimations();
		if (animations != null)
		{
			while (idxAnim < animations.Count)
			{
				SplineAnimation splineAnimation = animations[idxAnim];
				if (!(time * spline.traversalTime >= splineAnimation.time))
				{
					break;
				}
				AnimationState animationState = animationComponent[splineAnimation.animation];
				if (animationState != null)
				{
					if (splineAnimation.looping)
					{
						animationState.wrapMode = WrapMode.Loop;
					}
					else
					{
						animationState.wrapMode = WrapMode.ClampForever;
					}
					animationComponent.Rewind(splineAnimation.animation);
					animationComponent.CrossFade(splineAnimation.animation, splineAnimation.blendTime);
					if (splineAnimation.scaleTime)
					{
						float traversalTime = spline.traversalTime;
						if (idxAnim + 1 < animations.Count)
						{
							traversalTime = animations[idxAnim + 1].time;
						}
						float num = spline.ArcLength(time, traversalTime);
						float num2 = (desiredSpeed + spline.GetDesiredSpeed(traversalTime)) / 2f;
						animationState.speed = animationState.length / (num / num2);
					}
				}
				idxAnim++;
			}
		}
		if (time >= 1f)
		{
			foreach (SplineAnimation item in animations)
			{
				AnimationState animationState2 = animationComponent[item.animation];
				if (animationState2 != null)
				{
					animationState2.speed = 1f;
				}
			}
			InitStateFalling(obj);
			return;
		}
		if (followRotations)
		{
			obj.transform.rotation = spline.EvalRotation(time);
		}
		else
		{
			obj.transform.rotation = Quaternion.identity;
		}
		Vector3 position = obj.transform.position;
		Vector3 a = spline.EvalPosition(time);
		if (ignoreCollision)
		{
			obj.transform.localPosition += a - position;
		}
		else
		{
			charController.Move(a - position);
		}
		velocities.Add((obj.transform.position - position) / Time.deltaTime);
		if (velocities.Count > velocities.Capacity - 1)
		{
			velocities.RemoveAt(0);
		}
	}

	protected void InitStateFalling(GameObject obj)
	{
		if (!freeFall)
		{
			Vector3 vector = obj.transform.position;
			if (snapToGroundOnLand)
			{
				Vector3 position = vector + Vector3.up * (snapRayLength / 2f);
				RaycastHit hitInfo;
				if (ShsCharacterController.FindGround(position, snapRayLength, out hitInfo))
				{
					vector = ShsCharacterController.FindPositionOnGround(obj.GetComponent<CharacterController>(), hitInfo.point, ShsCharacterController.GroundOffset);
					charGlobals.motionController.teleportTo(vector);
				}
			}
			charGlobals.motionController.setDestination(vector);
			if (onBehaviorDone != null)
			{
				onBehaviorDone(owningObject);
			}
			if (charGlobals.behaviorManager.getBehavior() == this)
			{
				charGlobals.behaviorManager.endBehavior();
			}
			return;
		}
		velocity = Vector3.zero;
		foreach (Vector3 velocity2 in velocities)
		{
			velocity += velocity2;
		}
		velocity /= (float)velocities.Count;
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
}
