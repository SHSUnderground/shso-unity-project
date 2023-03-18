using UnityEngine;

public class BehaviorAttackTeleportAway : BehaviorAttackBase
{
	public override void Initialize(GameObject newTargetObject, CombatController.AttackData newAttackData, bool newSecondaryAttack, bool chainAttack, float emoteBroadcastRadius)
	{
		if (networkComponent == null || networkComponent.IsOwner())
		{
			BrawlerTeleportGrid[] array = Utils.FindObjectsOfType<BrawlerTeleportGrid>();
			if (array == null || array.Length == 0)
			{
				CspUtils.DebugLog(charGlobals.name + " is attempting to teleport but there is no BrawlerTeleportGrid in the scene to teleport to!");
			}
			else
			{
				BrawlerTeleportGrid brawlerTeleportGrid = array[0];
				BrawlerTeleportGrid[] array2 = array;
				foreach (BrawlerTeleportGrid brawlerTeleportGrid2 in array2)
				{
					if ((brawlerTeleportGrid2.transform.position - charGlobals.transform.position).sqrMagnitude < (brawlerTeleportGrid.transform.position - charGlobals.transform.position).sqrMagnitude)
					{
						brawlerTeleportGrid = brawlerTeleportGrid2;
					}
				}
				Vector3 safeLocation = brawlerTeleportGrid.GetSafeLocation(CombatController.Faction.Enemy);
				RaycastHit hitInfo = default(RaycastHit);
				if (Physics.Raycast(safeLocation + new Vector3(0f, 5f, 0f), Vector3.down, out hitInfo, charGlobals.characterController.height + 5f, 804756969))
				{
					charGlobals.motionController.teleportTo(hitInfo.point + new Vector3(0f, 0.1f, 0f));
					Vector3 forward = newTargetObject.transform.position - safeLocation;
					forward.y = 0f;
					charGlobals.transform.rotation = Quaternion.LookRotation(forward);
					charGlobals.motionController.updateLookDirection();
				}
			}
		}
		else
		{
			charGlobals.motionController.teleportToDestination();
		}
		base.Initialize(newTargetObject, newAttackData, newSecondaryAttack, chainAttack, emoteBroadcastRadius);
	}
}
