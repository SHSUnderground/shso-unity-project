using System;
using UnityEngine;

public class BehaviorRecoilLarge : BehaviorRecoil
{
	public override void Initialize(GameObject source, Vector3 newImpactPosition, CombatController.ImpactResultData newImpactResultData)
	{
		minAnimLength = ActionTimesDefinition.Instance.LargeMin;
		maxAnimLength = ActionTimesDefinition.Instance.LargeMax;
		animName = "recoil_big";
		if (UnityEngine.Random.value < 0.5f)
		{
			VOManager.Instance.PlayVO("damage_large", owningObject);
		}
		charGlobals.effectsList.TryOneShot("recoil_large_sequence", owningObject);
		base.Initialize(source, newImpactPosition, newImpactResultData);
	}

	public override CombatController.AttackData.RecoilType recoilType()
	{
		return CombatController.AttackData.RecoilType.Large;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		if (newBehaviorType.BaseType == typeof(BehaviorAttackSpecial) && charGlobals.spawnData != null && charGlobals.spawnData.spawnType == CharacterSpawn.Type.LocalPlayer)
		{
			return true;
		}
		return base.allowInterrupt(newBehaviorType);
	}
}
