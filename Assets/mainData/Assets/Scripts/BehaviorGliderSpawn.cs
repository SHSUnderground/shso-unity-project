using System;
using UnityEngine;

public class BehaviorGliderSpawn : BehaviorBase
{
	protected float behaviorTime;

	protected float movementTime = 1f;

	protected GliderCharacterSpawn spawner;

	protected bool finished;

	protected ShsCharacterController charController;

	protected CharacterMotionController motionController;

	protected Quaternion lookDirection;

	public void Initialize(GliderCharacterSpawn spawnSource)
	{
		spawner = spawnSource;
		animationComponent.Play(spawner.GliderAnimation);
		movementTime = spawner.pathLength / spawner.GliderSpeed;
		lookDirection = Quaternion.LookRotation(spawner.pathVec);
		charGlobals.combatController.createCombatEffect(spawner.GliderCombatEffect, charGlobals.combatController, true);
	}

	public override void behaviorBegin()
	{
		base.behaviorBegin();
	}

	public override void behaviorUpdate()
	{
		behaviorTime += Time.deltaTime;
		Vector3 pathStart = spawner.pathStart;
		if (behaviorTime >= spawner.AttackDelay)
		{
			float num = (behaviorTime - spawner.AttackDelay) / movementTime;
			if (num > 1f)
			{
				num = 1f;
				EndState();
			}
			pathStart += spawner.pathVec * num * spawner.pathLength;
		}
		pathStart.y += spawner.heightOffset;
		owningObject.transform.position = pathStart;
		owningObject.transform.rotation = lookDirection;
		base.behaviorUpdate();
	}

	public override bool allowUserInput()
	{
		return false;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return finished;
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

	protected void EndState()
	{
		finished = true;
		charGlobals.characterController.enabled = true;
		charGlobals.combatController.removeCombatEffect(spawner.GliderCombatEffect);
		if (spawner.DestroyAtEndOfPath)
		{
			if (charGlobals.spawnData != null)
			{
				charGlobals.spawnData.Die();
				charGlobals.spawnData.Despawn(EntityDespawnMessage.despawnType.effect);
			}
			UnityEngine.Object.Destroy(owningObject);
			return;
		}
		if (spawner.spawnInSource != null)
		{
			BehaviorSpawnAnimate behaviorSpawnAnimate = charGlobals.behaviorManager.requestChangeBehavior(typeof(BehaviorSpawnAnimate), true) as BehaviorSpawnAnimate;
			if (behaviorSpawnAnimate != null)
			{
				behaviorSpawnAnimate.Initialize(spawner.spawnInSource, spawner.transform.position, true, null, true);
			}
			return;
		}
		BehaviorFreeFall behaviorFreeFall = charGlobals.behaviorManager.requestChangeBehavior(typeof(BehaviorFreeFall), false) as BehaviorFreeFall;
		if (behaviorFreeFall != null)
		{
			behaviorFreeFall.Initialize(default(Vector3), null);
			behaviorFreeFall.behaviorUpdate();
			return;
		}
		CspUtils.DebugLog("Failed to switch to free fall behavior");
		if (charGlobals.behaviorManager.getBehavior() == this)
		{
			charGlobals.behaviorManager.endBehavior();
		}
	}
}
