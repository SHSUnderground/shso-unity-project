using System.Collections.Generic;
using UnityEngine;

public class ImpactObject : ImpactBase
{
	public override void ImpactUpdate(float elapsedTime)
	{
		if (fireNow || (impactData.firingTime > 0f && elapsedTime > impactData.firingTime && !fired))
		{
			ImpactFired();
			if (charGlobals.networkComponent != null && charGlobals.networkComponent.IsOwner())
			{
				List<ProjectileConverter> list = new List<ProjectileConverter>();
				ProjectileConverter[] array = Object.FindObjectsOfType(typeof(ProjectileConverter)) as ProjectileConverter[];
				ProjectileConverter[] array2 = array;
				foreach (ProjectileConverter projectileConverter in array2)
				{
					if (projectileConverter.objectName == impactData.projectileName)
					{
						list.Add(projectileConverter);
					}
				}
				int num = 0;
				while (list.Count > 0)
				{
					ProjectileConverter projectileConverter2 = list[0];
					PlayerCombatController playerCombatController = PlayerCombatController.PlayerList[num];
					num++;
					if (num >= PlayerCombatController.PlayerCount)
					{
						num = 0;
					}
					NetActionProjectile action = new NetActionProjectile(projectileConverter2.gameObject, playerCombatController.gameObject, attackData.attackName, impactData.index);
					charGlobals.networkComponent.QueueNetAction(action);
					launchObjectAt(projectileConverter2, playerCombatController);
					list.RemoveAt(0);
				}
			}
		}
		base.ImpactUpdate(elapsedTime);
	}

	public void launchObjectAt(ProjectileConverter converter, CombatController targetController)
	{
		GameObject gameObject = converter.CreateProjectile();
		ProjectileColliderController projectileColliderController = gameObject.GetComponent(typeof(ProjectileColliderController)) as ProjectileColliderController;
		if (projectileColliderController == null)
		{
			CspUtils.DebugLog("Projectile " + gameObject.name + " does not have a projectile collider controller and will not be created");
			Object.Destroy(gameObject);
			return;
		}
		projectileColliderController.Initialize(charGlobals, attackData, impactData, false, colliderObject, targetController);
		Vector3 force = targetController.TargetPosition - gameObject.transform.position;
		force.Normalize();
		force *= impactData.projectileSpeed;
		gameObject.rigidbody.AddForce(force, ForceMode.VelocityChange);
	}
}
