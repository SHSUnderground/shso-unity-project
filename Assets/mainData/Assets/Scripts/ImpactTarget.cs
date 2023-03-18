using UnityEngine;

public class ImpactTarget : ImpactBase
{
	public GameObject CreateProjectilePrefab()
	{
		GameObject gameObject = null;
		if (impactData.projectileName != null)
		{
			if (impactData.projectileIsEnvironmental)
			{
				EnvironmentalObjectReference[] array = Object.FindObjectsOfType(typeof(EnvironmentalObjectReference)) as EnvironmentalObjectReference[];
				EnvironmentalObjectReference[] array2 = array;
				foreach (EnvironmentalObjectReference environmentalObjectReference in array2)
				{
					if (environmentalObjectReference.objectName == impactData.projectileName)
					{
						gameObject = environmentalObjectReference.reference;
					}
				}
			}
			if (gameObject == null)
			{
				gameObject = (charGlobals.combatController.effectSequenceSource.GetEffectSequencePrefabByName(impactData.projectileName) as GameObject);
			}
		}
		return gameObject;
	}

	public override void ImpactUpdate(float elapsedTime)
	{
		if (fireNow || (impactData.firingTime > 0f && elapsedTime > impactData.firingTime && !fired))
		{
			ImpactFired();
			if (impactData.projectileName != null)
			{
				GameObject gameObject = CreateProjectilePrefab();
				GameObject gameObject2 = null;
				if (gameObject != null)
				{
					gameObject2 = (Object.Instantiate(gameObject, targetCombatController.transform.position, targetCombatController.transform.rotation) as GameObject);
					ProjectileColliderController projectileColliderController = gameObject2.GetComponent(typeof(ProjectileColliderController)) as ProjectileColliderController;
					if ((bool)projectileColliderController)
					{
						bool newNonImpacting = charGlobals != null && charGlobals.spawnData != null && (charGlobals.spawnData.spawnType & CharacterSpawn.Type.Remote) != 0 && charGlobals.combatController.faction != CombatController.Faction.Enemy;
						projectileColliderController.Initialize(charGlobals, attackData, impactData, newNonImpacting, targetCombatController.gameObject, targetCombatController);
					}
				}
				else
				{
					CspUtils.DebugLog("Failed to spawn projectile " + impactData.projectileName + " for attack " + attackData.attackName);
				}
			}
			else if (targetCombatController != null && (charGlobals.networkComponent == null || charGlobals.networkComponent.IsOwner()))
			{
				charGlobals.combatController.attackHit(charGlobals.gameObject.transform.position, targetCombatController, attackData, impactData);
			}
		}
		base.ImpactUpdate(elapsedTime);
	}
}
