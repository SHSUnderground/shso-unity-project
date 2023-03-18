using System;
using UnityEngine;

public abstract class BehaviorRecoil : BehaviorBase
{
	protected string animName;

	protected CombatController.ImpactResultData impactResultData;

	protected float minAnimLength;

	protected float maxAnimLength = 0.5f;

	protected float oldAnimSpeed = 1f;

	protected EffectSequence idleSequence;

	protected CombatController sourceCombatController;

	public virtual void Initialize(GameObject source, Vector3 newImpactPosition, CombatController.ImpactResultData newImpactResultData)
	{
		sourceCombatController = source.GetComponent<CombatController>();
		if (animationComponent[animName] == null)
		{
			charGlobals.behaviorManager.endBehavior();
			return;
		}
		impactResultData = newImpactResultData;
		if (animationComponent.IsPlaying(animName))
		{
			animationComponent.Rewind(animName);
			animationComponent.Play(animName);
		}
		else
		{
			animationComponent[animName].time = 0f;
			animationComponent[animName].wrapMode = WrapMode.ClampForever;
			animationComponent.CrossFade(animName, 0.05f);
		}
		oldAnimSpeed = clampAnimationSpeed(animName, minAnimLength, maxAnimLength);
	}

	public override void behaviorBegin()
	{
		base.behaviorBegin();
		idleSequence = charGlobals.effectsList.PlaySequence("emote_idle_sequence");
	}

	public override void behaviorUpdate()
	{
		base.behaviorUpdate();
		if (checkRecoilEnd())
		{
			charGlobals.behaviorManager.endBehavior();
		}
	}

	public override void behaviorLateUpdate()
	{
		base.behaviorLateUpdate();
		if (Time.time > startTime + 0.15f)
		{
			charGlobals.motionController.performRootMotion();
		}
	}

	public override void behaviorEnd()
	{
		if (idleSequence != null)
		{
			UnityEngine.Object.Destroy(idleSequence.gameObject);
		}
		restoreAnimationSpeed(animName, oldAnimSpeed);
		base.behaviorEnd();
	}

	public virtual bool checkRecoilEnd()
	{
		if (animName != null && elapsedTime * animationComponent[animName].speed >= animationComponent[animName].length)
		{
			return true;
		}
		return false;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		if (newBehaviorType.IsSubclassOf(typeof(BehaviorRecoil)))
		{
			return true;
		}
		if (charGlobals != null && charGlobals.spawnData != null && (charGlobals.spawnData.spawnType & CharacterSpawn.Type.Boss) != 0 && (newBehaviorType == typeof(BehaviorAttackBase) || newBehaviorType.IsSubclassOf(typeof(BehaviorAttackBase))))
		{
			return true;
		}
		return false;
	}

	public override bool allowUserInput()
	{
		return true;
	}

	public override bool useMotionController()
	{
		if (networkComponent != null && networkComponent.IsOwner())
		{
			return false;
		}
		return true;
	}

	public override void animationOverriden(string baseAnimation, string overrideAnimation)
	{
		base.animationOverriden(baseAnimation, overrideAnimation);
		if (animName == baseAnimation)
		{
			animName = overrideAnimation;
		}
	}

	public virtual void tookDamage(float damage)
	{
	}

	public abstract CombatController.AttackData.RecoilType recoilType();
}
