using System;
using UnityEngine;

public class BehaviorRecoilSmall : BehaviorRecoil
{
	public override void Initialize(GameObject source, Vector3 newImpactPosition, CombatController.ImpactResultData newImpactResultData)
	{
		minAnimLength = ActionTimesDefinition.Instance.SmallMin;
		maxAnimLength = ActionTimesDefinition.Instance.SmallMax;
		float num = Vector3.Dot((newImpactPosition - owningObject.transform.position).normalized, owningObject.transform.forward);
		if (num < 0f)
		{
			animName = getAnimationName("recoil_back");
		}
		else
		{
			animName = getAnimationName("recoil_small");
			if (animationComponent[animName + "_2"] != null && UnityEngine.Random.Range(1, 100) <= 50)
			{
				animName += "_2";
			}
		}
		if (UnityEngine.Random.value < 0.25f)
		{
			VOManager.Instance.PlayVO("damage_small", owningObject);
		}
		charGlobals.effectsList.TryOneShot("recoil_small_sequence", owningObject);
		base.Initialize(source, newImpactPosition, newImpactResultData);
	}

	public override CombatController.AttackData.RecoilType recoilType()
	{
		return CombatController.AttackData.RecoilType.Small;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		if (newBehaviorType == typeof(BehaviorAttackSpecial) && charGlobals.spawnData != null && charGlobals.spawnData.spawnType == CharacterSpawn.Type.LocalPlayer)
		{
			return true;
		}
		return base.allowInterrupt(newBehaviorType);
	}
}
