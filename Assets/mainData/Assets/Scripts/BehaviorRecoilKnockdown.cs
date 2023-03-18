using UnityEngine;

public class BehaviorRecoilKnockdown : BehaviorRecoil
{
	protected float landedTime;

	protected float getupTime;

	protected Vector3 lookTarget;

	public override void Initialize(GameObject source, Vector3 newImpactPosition, CombatController.ImpactResultData newImpactResultData)
	{
		minAnimLength = ActionTimesDefinition.Instance.KnockdownMin;
		maxAnimLength = ActionTimesDefinition.Instance.KnockdownMax;
		if (animationComponent["recoil_knockdown"] != null)
		{
			animName = "recoil_knockdown";
		}
		else
		{
			animName = getAnimationName("recoil_death");
		}
		base.Initialize(source, newImpactPosition, newImpactResultData);
		landedTime = animationComponent[animName].length;
		float num = 0f;
		if (impactResultData != null && impactResultData.knockdownDuration != null)
		{
			num = impactResultData.knockdownDuration.getValue(sourceCombatController);
		}
		getupTime = landedTime + num;
		lookTarget = newImpactPosition - owningObject.transform.position;
		if (Random.value < 0.75f)
		{
			VOManager.Instance.PlayVO("damage_knockdown", owningObject);
		}
		combatController.playKnockdownEffect();
	}

	public override bool checkRecoilEnd()
	{
		return false;
	}

	public override void behaviorUpdate()
	{
		if (elapsedTime < landedTime)
		{
			if (impactResultData.rotateTargetToImpact)
			{
				charGlobals.motionController.rotateTowards(lookTarget);
			}
		}
		else if (elapsedTime >= getupTime)
		{
			if (charGlobals.squadBattleCharacterAI != null && charGlobals.squadBattleCharacterAI.homeLocator == null)
			{
				charGlobals.squadBattleCharacterAI.EndAttack();
				charGlobals.combatController.playDespawnEffect();
				SquadBattleCharacterController.Instance.RemoveCharacter(owningObject);
			}
			else
			{
				BehaviorRecoilGetup behaviorRecoilGetup = charGlobals.behaviorManager.forceChangeBehavior(typeof(BehaviorRecoilGetup)) as BehaviorRecoilGetup;
				behaviorRecoilGetup.Initialize(owningObject, owningObject.transform.position, impactResultData);
			}
			return;
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

	public override bool allowUserInput()
	{
		return false;
	}

	public override void tookDamage(float damage)
	{
		if (animationComponent["recoil_ground"] != null && !animationComponent.IsPlaying("recoil_ground") && damage > 0f && elapsedTime > animationComponent[animName].length)
		{
			if (getupTime < elapsedTime + animationComponent["recoil_ground"].length)
			{
				getupTime = elapsedTime + animationComponent["recoil_ground"].length;
			}
			animationComponent.Play("recoil_ground");
		}
	}

	public override CombatController.AttackData.RecoilType recoilType()
	{
		return CombatController.AttackData.RecoilType.Knockdown;
	}
}
