using System;
using UnityEngine;

public class BehaviorRecoilStun : BehaviorRecoil
{
	public bool disallowInterrupt;

	protected string introAnim;

	protected bool idleTransitionPlayed;

	protected float duration;

	private GameObject stunFX;

	private CombatController.AttackData.RecoilType stunRecoilType;

	public void InitializeWithIntro(float newDuration, string newIntroAnim)
	{
		minAnimLength = ActionTimesDefinition.Instance.StunMin;
		maxAnimLength = ActionTimesDefinition.Instance.StunMax;
		duration = newDuration;
		introAnim = newIntroAnim;
		stunRecoilType = CombatController.AttackData.RecoilType.Stun;
		animationComponent.CrossFade(introAnim);
	}

	public override void Initialize(GameObject source, Vector3 newImpactPosition, CombatController.ImpactResultData newImpactResultData)
	{
		introAnim = null;
		if (newImpactResultData != null)
		{
			stunRecoilType = newImpactResultData.recoil;
		}
		if (stunRecoilType == CombatController.AttackData.RecoilType.Dance)
		{
			if (animationComponent["recoil_dance"] != null)
			{
				animName = "recoil_dance";
			}
			else if (animationComponent["emote_dance"] != null)
			{
				animName = "emote_dance";
			}
			else
			{
				animName = "recoil_back";
			}
		}
		else if (animationComponent["recoil_stun"] != null)
		{
			animName = "recoil_stun";
		}
		else
		{
			animName = "recoil_small";
		}
		base.Initialize(source, newImpactPosition, newImpactResultData);
		if (impactResultData != null && impactResultData.stunAnimSpeed >= 0f)
		{
			animationComponent[animName].speed = impactResultData.stunAnimSpeed;
		}
		else
		{
			animationComponent[animName].speed = UnityEngine.Random.Range(0.95f, 1.05f);
		}
		if (animName == "recoil_small" || animName == "recoil_back")
		{
			animationComponent[animName].wrapMode = WrapMode.PingPong;
			animName = getAnimationName(animName);
			animationComponent[animName].wrapMode = WrapMode.PingPong;
		}
		else
		{
			animationComponent[animName].wrapMode = WrapMode.Loop;
		}
		if (animationComponent["recoil_stun_to_idle"] != null)
		{
			idleTransitionPlayed = false;
		}
		else
		{
			idleTransitionPlayed = true;
		}
		if (impactResultData != null && impactResultData != null && impactResultData.knockdownDuration != null)
		{
			duration = impactResultData.knockdownDuration.getValue(sourceCombatController);
		}
		stunFX = combatController.playStunEffect();
	}

	public override CombatController.AttackData.RecoilType recoilType()
	{
		return stunRecoilType;
	}

	public override void behaviorUpdate()
	{
		if (introAnim != null)
		{
			if (elapsedTime > animationComponent[introAnim].length)
			{
				Initialize(null, Vector3.zero, null);
			}
		}
		else if (!idleTransitionPlayed && elapsedTime > duration - animationComponent["recoil_stun_to_idle"].length)
		{
			animationComponent.CrossFade("recoil_stun_to_idle");
			idleTransitionPlayed = true;
		}
		base.behaviorUpdate();
	}

	public override bool checkRecoilEnd()
	{
		if (elapsedTime > duration)
		{
			return true;
		}
		return false;
	}

	public override void behaviorEnd()
	{
		if (animName != null)
		{
			animationComponent[animName].speed = 1f;
			animationComponent[animName].wrapMode = WrapMode.ClampForever;
		}
		if (stunFX != null)
		{
			UnityEngine.Object.Destroy(stunFX);
		}
		base.behaviorEnd();
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		if (!disallowInterrupt)
		{
			return base.allowInterrupt(newBehaviorType);
		}
		return false;
	}
}
