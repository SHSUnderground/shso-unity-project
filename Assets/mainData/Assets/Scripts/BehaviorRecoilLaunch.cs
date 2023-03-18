using UnityEngine;

public class BehaviorRecoilLaunch : BehaviorRecoil
{
	protected float landedTime;

	protected bool landed;

	protected Vector3 lookTarget;

	public override void Initialize(GameObject source, Vector3 newImpactPosition, CombatController.ImpactResultData newImpactResultData)
	{
		minAnimLength = ActionTimesDefinition.Instance.LaunchMin;
		maxAnimLength = ActionTimesDefinition.Instance.LaunchMax;
		landedTime = 0f;
		landed = false;
		charGlobals.effectsList.TryOneShot("recoil_launch_sequence", charGlobals.gameObject);
		if (animationComponent["recoil_launch"] != null)
		{
			animName = "recoil_launch";
		}
		else if (animationComponent["recoil_knockdown"] != null)
		{
			animName = "recoil_knockdown";
		}
		else
		{
			animName = getAnimationName("recoil_death");
		}
		base.Initialize(source, newImpactPosition, newImpactResultData);
		animationComponent[animName].wrapMode = WrapMode.ClampForever;
		oldAnimSpeed = clampAnimationSpeed(animName, minAnimLength, maxAnimLength);
		lookTarget = newImpactPosition - owningObject.transform.position;
		if (Random.value < 0.75f)
		{
			VOManager.Instance.PlayVO("damage_knockdown", owningObject);
		}
	}

	public override void behaviorUpdate()
	{
		if (!landed)
		{
			charGlobals.motionController.setForcedVelocityDuration(0.2f);
			if (impactResultData.rotateTargetToImpact)
			{
				charGlobals.motionController.rotateTowards(lookTarget);
			}
			if ((double)elapsedTime > 0.5 && charGlobals.motionController.IsOnGround())
			{
				motionLanded();
			}
		}
		else
		{
			float num = 0f;
			if (impactResultData.knockdownDuration != null && impactResultData.knockdownDuration != null)
			{
				num = impactResultData.knockdownDuration.getValue(sourceCombatController);
			}
			if (landedTime > 0f && Time.time > landedTime + num)
			{
				BehaviorRecoilGetup behaviorRecoilGetup = charGlobals.behaviorManager.forceChangeBehavior(typeof(BehaviorRecoilGetup)) as BehaviorRecoilGetup;
				behaviorRecoilGetup.Initialize(owningObject, owningObject.transform.position, impactResultData);
				return;
			}
		}
		base.behaviorUpdate();
	}

	public override bool checkRecoilEnd()
	{
		return false;
	}

	public override void motionLanded()
	{
		landed = true;
		landedTime = Time.time;
		if (animationComponent["recoil_launch_land"] != null)
		{
			animationComponent.CrossFade("recoil_launch_land", 0.1f);
		}
		combatController.playLaunchLandEffect();
	}

	public override bool allowUserInput()
	{
		return false;
	}

	public override CombatController.AttackData.RecoilType recoilType()
	{
		return CombatController.AttackData.RecoilType.Launch;
	}
}
