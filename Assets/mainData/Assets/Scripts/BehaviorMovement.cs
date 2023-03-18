using System.Collections.Generic;
using UnityEngine;

public class BehaviorMovement : BehaviorBase
{
	public enum MovementState
	{
		Undefined,
		Idle,
		Running,
		Jumping,
		Airborne,
		Landing,
		Sleeping
	}

	private const float customJumpProbability = 0.4f;

	public bool useTransitionAnims;

	public IdleEmote idleEmote;

	protected MovementState movementState;

	protected bool jumped;

	protected int jumpIndex;

	protected bool fallLoopStarted;

	protected float fallStartTime = -1f;

	protected float fallStartHeight;

	protected int counter;

	protected string runAnimName;

	protected string idleAnimName;

	protected string sleepAnimName;

	protected string wakeAnimName;

	protected Vector3 lastPosition;

	protected float stalledTime;

	protected EffectSequence movementSequence;

	protected EffectSequence pickupSequence;

	public bool npcRunMode;

	private float cachedRunSpeed;

	public MovementState CurrentMovementState
	{
		get
		{
			return movementState;
		}
	}

	public virtual void configureMovementBehaviors()
	{
		if (charGlobals.spawnData != null && charGlobals.spawnData.spawner as NPCSpawn != null)
		{
			runAnimName = GetNPCRunAnimName();
			idleAnimName = getAnimationName("movement_idle");
		}
		else if (charGlobals.motionController != null && charGlobals.motionController.carriedThrowable != null)
		{
			runAnimName = "pickup_run";
			idleAnimName = "pickup_idle";
		}
		else
		{
			runAnimName = getAnimationName("movement_run");
			idleAnimName = getAnimationName("movement_idle");
		}
	}

	public override void behaviorBegin()
	{
		configureMovementBehaviors();
		sleepAnimName = "sleep_idle";
		wakeAnimName = "sleep_wake_up";
		movementState = MovementState.Undefined;
		base.behaviorBegin();
		jumped = false;
		fallLoopStarted = false;
		playStandardAnim();
		movementSequence = charGlobals.effectsList.PlaySequence("emote_idle_sequence");
	}

	public override void behaviorUpdate()
	{
		base.behaviorUpdate();
		updateAnimation();
		if (movementState == MovementState.Idle && idleEmote != null && idleEmote.Update(Time.deltaTime))
		{
			idleEmote.Emote(charGlobals.behaviorManager);
		}
		if ((lastPosition - owningObject.transform.position).sqrMagnitude >= 1E-09f)
		{
			stalledTime = Time.time;
		}
		lastPosition = owningObject.transform.position;
	}

	protected void updateAnimation()
	{
		if (movementState == MovementState.Landing)
		{
			if (HasAnimationCompleted("jump_land") || HasAnimationCompleted("jump_run_land"))
			{
				jumped = false;
				movementState = MovementState.Undefined;
				animationComponent.Rewind(runAnimName);
				playStandardAnim();
			}
		}
		else if (movementState == MovementState.Idle)
		{
			if (HasAnimationCompleted("run_to_idle"))
			{
				animationComponent.Play(idleAnimName);
			}
		}
		else if (movementState == MovementState.Running)
		{
			if (HasAnimationCompleted("idle_to_run"))
			{
				animationComponent.Play(runAnimName);
			}
		}
		else if (movementState == MovementState.Airborne && fallStartTime > 0f)
		{
			if (Time.time >= fallStartTime)
			{
				playFallAnim();
			}
		}
		else if (movementState == MovementState.Airborne && animationComponent["jump_fall_loop"] != null && !fallLoopStarted)
		{
			if (HasAnimationCompleted("jump_run_fall") || HasAnimationCompleted("jump_fall"))
			{
				fallLoopStarted = true;
				animationComponent.CrossFade("jump_fall_loop", 0.4f);
			}
		}
		else if (movementState == MovementState.Sleeping && HasAnimationCompleted(wakeAnimName))
		{
			playStandardAnim();
		}
	}

	public override void motionBeganMoving()
	{
		if (movementState == MovementState.Jumping || movementState == MovementState.Airborne)
		{
			return;
		}
		if (movementState == MovementState.Idle && animationComponent["idle_to_run"] != null && useTransitionAnims)
		{
			animationComponent.CrossFade("idle_to_run");
		}
		else
		{
			animationComponent.CrossFade(runAnimName, 0.2f);
		}
		movementState = MovementState.Running;
		if (charGlobals.motionController.carriedThrowable != null)
		{
			if (pickupSequence != null)
			{
				Object.Destroy(pickupSequence.gameObject);
			}
			pickupSequence = charGlobals.effectsList.PlaySequence("pickup_run_sequence");
		}
	}

	public override void motionStoppedMoving()
	{
		if (movementState == MovementState.Jumping || movementState == MovementState.Airborne)
		{
			return;
		}
		if (movementState == MovementState.Running && animationComponent["run_to_idle"] != null && useTransitionAnims)
		{
			animationComponent.CrossFade("run_to_idle");
		}
		else
		{
			if (ShouldSleep())
			{
				Sleep();
				return;
			}
			animationComponent.CrossFade(idleAnimName, 0.5f);
		}
		Idle();
		if (charGlobals.motionController.carriedThrowable != null)
		{
			if (pickupSequence != null)
			{
				Object.Destroy(pickupSequence.gameObject);
			}
			pickupSequence = charGlobals.effectsList.PlaySequence("pickup_idle_sequence");
		}
	}

	protected void Idle()
	{
		movementState = MovementState.Idle;
		if (idleEmote != null)
		{
			idleEmote.elapsed = 0f;
		}
	}

	protected string GetJumpAnimation(string name)
	{
		return (jumpIndex != 0) ? (name + "_" + (jumpIndex + 1)) : name;
	}

	public override void motionJumped()
	{
		jumped = true;
		string name = (!(charGlobals.motionController.getVelocityFlat().sqrMagnitude > 0.1f)) ? "jump_up" : "jump_run_up";
		if (charGlobals.motionController.doubleJump)
		{
			if (charGlobals.motionController.DoubleJumping)
			{
				if (charGlobals.motionController.doubleJumpAnimIndex >= 0)
				{
					jumpIndex = charGlobals.motionController.doubleJumpAnimIndex - 1;
				}
				else
				{
					jumpIndex = 1;
				}
			}
			else
			{
				jumpIndex = 0;
			}
		}
		else
		{
			jumpIndex = ((Random.value < 0.4f) ? 1 : 0);
		}
		if (jumpIndex != 0 && animationComponent[GetJumpAnimation(name)] == null)
		{
			jumpIndex = 0;
		}
		movementState = MovementState.Jumping;
		string jumpAnimation = GetJumpAnimation(name);
		if (animationComponent[jumpAnimation] != null)
		{
			animationComponent.CrossFade(jumpAnimation, 0.1f);
			AnimationState animationState = animationComponent[jumpAnimation];
			float lastJumpDuration = charGlobals.motionController.LastJumpDuration;
			animationState.speed = ((lastJumpDuration == 0f) ? 1f : (0.5f / lastJumpDuration));
		}
		if (charGlobals.motionController.DoubleJumping || charGlobals.motionController.holdJump)
		{
			VOManager.Instance.PlayVO("jump", owningObject);
		}
		if (charGlobals.motionController.UseJumpSequence)
		{
			charGlobals.effectsList.OneShot("jump_sequence");
		}
	}

	public override void motionJumpCancelled()
	{
		string name = animationComponent.clip.name;
		float time = animationComponent[name].time;
		animationComponent.clip = animationComponent.GetClip(name);
		animationComponent[name].speed = 1f;
		animationComponent[name].time = time;
	}

	public override void motionBeganFalling()
	{
		if (IsSleeping())
		{
			animationComponent.Play(idleAnimName);
		}
		movementState = MovementState.Airborne;
		fallLoopStarted = false;
		Vector3 position = charGlobals.transform.position;
		fallStartHeight = position.y;
		if (charGlobals.motionController.carriedThrowable == null)
		{
			charGlobals.motionController.dropThrowable();
			float num = 0f;
			AnimationState animationState = animationComponent[GetJumpAnimation("jump_up")];
			AnimationState animationState2 = animationComponent[GetJumpAnimation("jump_run_up")];
			if (animationState != null && animationComponent.IsPlaying(GetJumpAnimation("jump_up")))
			{
				num = animationState.length - animationState.time;
			}
			else if (animationState2 != null && animationComponent.IsPlaying(GetJumpAnimation("jump_run_up")))
			{
				num = animationState2.length - animationState2.time;
			}
			if (num > 0.3f)
			{
				fallStartTime = Time.time + (num - 0.3f);
			}
			else
			{
				playFallAnim();
			}
		}
	}

	public override void motionLanded()
	{
		movementState = MovementState.Landing;
		if (charGlobals.motionController.carriedThrowable == null)
		{
			if (animationComponent["jump_run_land"] != null && (animationComponent["jump_land"] == null || charGlobals.motionController.getVelocityFlat().sqrMagnitude > 0.1f))
			{
				animationComponent.CrossFade("jump_run_land", 0.1f);
			}
			else if (animationComponent["jump_land"] != null)
			{
				animationComponent.CrossFade("jump_land", 0.1f);
			}
			else
			{
				playStandardAnim();
			}
		}
		float num = charGlobals.motionController.JumpPotential + charGlobals.motionController.SecondJumpPotential;
		float num2 = 0.5f;
		float num3 = fallStartHeight;
		Vector3 position = charGlobals.transform.position;
		if (num3 - position.y > num + num2)
		{
			VOManager.Instance.PlayVO("land", owningObject);
		}
	}

	protected virtual void playStandardAnim()
	{
		if (charGlobals.motionController != null && (double)charGlobals.motionController.getVelocityFlat().sqrMagnitude > 0.01)
		{
			motionBeganMoving();
		}
		else
		{
			motionStoppedMoving();
		}
	}

	protected void playFallAnim()
	{
		fallStartTime = -1f;
		if (animationComponent["jump_run_fall"] != null && (animationComponent["jump_fall"] == null || charGlobals.motionController.getVelocityFlat().sqrMagnitude > 0.1f))
		{
			animationComponent.CrossFade("jump_run_fall", 0.3f);
		}
		else if (animationComponent["jump_fall"] != null)
		{
			animationComponent.CrossFade("jump_fall", 0.3f);
		}
	}

	public override void behaviorEnd()
	{
		if (npcRunMode)
		{
			charGlobals.motionController.speed = cachedRunSpeed;
		}
		StopIdleSequence();
		base.behaviorEnd();
	}

	protected void StopIdleSequence()
	{
		if (movementSequence != null)
		{
			Object.Destroy(movementSequence.gameObject);
		}
		if (pickupSequence != null)
		{
			Object.Destroy(pickupSequence.gameObject);
		}
	}

	public override bool useMotionController()
	{
		return true;
	}

	protected void DebugDrawPath(List<Vector3> path)
	{
		if (path != null && path.Count > 0)
		{
			for (int i = 1; i < path.Count; i++)
			{
				Debug.DrawLine(path[i - 1], path[i], Color.green);
			}
			if (path.Count <= 1)
			{
			}
		}
	}

	protected void DebugDrawPath(List<PathNode> path)
	{
		if (path != null && path.Count > 0)
		{
			for (int i = 1; i < path.Count; i++)
			{
				Debug.DrawLine(path[i - 1].transform.position, path[i].transform.position, Color.green);
			}
			if (path.Count <= 1)
			{
			}
		}
	}

	private bool HasAnimationCompleted(string animationName)
	{
		return !animationComponent.isPlaying || (animationComponent.IsPlaying(animationName) && animationComponent[animationName].time >= animationComponent[animationName].length);
	}

	public void setPetAnimMode(float speed)
	{
		npcRunMode = true;
		cachedRunSpeed = charGlobals.motionController.speed;
		CspUtils.DebugLog("setPetAnimMode " + speed + " " + charGlobals.motionController.speed);
	}

	public void SetNPCAnimMode(bool run, float speed)
	{
		npcRunMode = run;
		cachedRunSpeed = charGlobals.motionController.speed;
		runAnimName = GetNPCRunAnimName();
		if (npcRunMode)
		{
			charGlobals.motionController.speed = speed;
		}
	}

	protected string GetNPCRunAnimName()
	{
		string animationName;
		if (npcRunMode)
		{
			animationName = getAnimationName("movement_run");
			if (animationComponent[animationName] == null)
			{
				animationName = getAnimationName("movement_walk");
			}
		}
		else
		{
			animationName = getAnimationName("movement_walk");
			if (animationComponent[animationName] == null)
			{
				animationName = getAnimationName("movement_run");
			}
		}
		return animationName;
	}

	protected bool ShouldSleep()
	{
		if (animationComponent[sleepAnimName] == null)
		{
			return false;
		}
		if (charGlobals.combatController != null && charGlobals.combatController.IsCharmed)
		{
			return false;
		}
		AIControllerBrawler brawlerCharacterAI = charGlobals.brawlerCharacterAI;
		if (brawlerCharacterAI == null)
		{
			return false;
		}
		if (brawlerCharacterAI.HasTarget() || brawlerCharacterAI.HasTargetedSomeone())
		{
			return false;
		}
		if (brawlerCharacterAI.CanWakeUpOnAggro && brawlerCharacterAI.HasTargetInRange())
		{
			return false;
		}
		return true;
	}

	public bool IsSleeping()
	{
		return movementState == MovementState.Sleeping;
	}

	public void Sleep()
	{
		if (animationComponent[sleepAnimName] != null)
		{
			animationComponent.CrossFade(sleepAnimName, 0.5f);
			movementState = MovementState.Sleeping;
		}
	}

	public void WakeUp()
	{
		if (animationComponent[wakeAnimName] != null)
		{
			animationComponent.CrossFade(wakeAnimName, 0.1f);
			charGlobals.effectsList.TryOneShot("wake_up_sequence", charGlobals.gameObject);
		}
		else
		{
			playStandardAnim();
		}
	}

	public override void animationOverriden(string baseAnimation, string overrideAnimation)
	{
		base.animationOverriden(baseAnimation, overrideAnimation);
		if (idleAnimName == baseAnimation)
		{
			idleAnimName = overrideAnimation;
			if (movementState == MovementState.Idle)
			{
				animationComponent.Play(idleAnimName);
			}
		}
		else if (runAnimName == baseAnimation)
		{
			runAnimName = overrideAnimation;
			if (movementState == MovementState.Running)
			{
				animationComponent.Play(runAnimName);
			}
		}
	}
}
