using UnityEngine;

public class BehaviorRecoilGetup : BehaviorRecoil
{
	protected int motionFrames;

	public override void Initialize(GameObject source, Vector3 newImpactPosition, CombatController.ImpactResultData newImpactResultData)
	{
		minAnimLength = ActionTimesDefinition.Instance.GetupMin;
		maxAnimLength = ActionTimesDefinition.Instance.GetupMax;
		if (animationComponent["recoil_getup"] == null)
		{
			animName = "recoil_big";
		}
		else
		{
			animName = "recoil_getup";
		}
		animationComponent.Stop();
		charGlobals.effectsList.TryOneShot("recoil_getup_sequence", charGlobals.gameObject);
		base.Initialize(source, newImpactPosition, newImpactResultData);
		motionFrames = 0;
	}

	public override void behaviorBegin()
	{
		combatController.playGetupEffect();
		base.behaviorBegin();
	}

	public override CombatController.AttackData.RecoilType recoilType()
	{
		return CombatController.AttackData.RecoilType.Getup;
	}

	public override void behaviorLateUpdate()
	{
		if (motionFrames < 2)
		{
			motionFrames++;
		}
		else
		{
			charGlobals.motionController.performRootMotion();
		}
		base.behaviorLateUpdate();
	}
}
