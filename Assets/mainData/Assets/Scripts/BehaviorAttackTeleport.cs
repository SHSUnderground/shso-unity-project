using UnityEngine;

public class BehaviorAttackTeleport : BehaviorAttackBase
{
	public override void Initialize(GameObject newTargetObject, CombatController.AttackData newAttackData, bool newSecondaryAttack, bool chainAttack, float emoteBroadcastRadius)
	{
		if (networkComponent == null || networkComponent.IsOwner())
		{
			Vector3 vector = FindTeleportableLocation(newTargetObject.transform.position, newAttackData.desiredRange, charGlobals.characterController.radius, charGlobals.characterController.height);
			charGlobals.motionController.teleportTo(vector + new Vector3(0f, 0.1f, 0f));
			Vector3 forward = newTargetObject.transform.position - vector;
			forward.y = 0f;
			charGlobals.transform.rotation = Quaternion.LookRotation(forward);
			charGlobals.motionController.updateLookDirection();
		}
		else
		{
			charGlobals.motionController.teleportToDestination();
		}
		base.Initialize(newTargetObject, newAttackData, newSecondaryAttack, chainAttack, emoteBroadcastRadius);
	}

	public static Vector3 FindTeleportableLocation(CharacterGlobals character, float range)
	{
		if (character != null)
		{
			return FindTeleportableLocation(character.transform.position, range, character.characterController.radius, character.characterController.height);
		}
		return Vector3.zero;
	}

	public static Vector3 FindTeleportableLocation(Vector3 startPosition, float range, float requiredRadius, float requiredHeight)
	{
		for (int i = 0; i < 5; i++)
		{
			float angle = Random.Range(0f, 360f);
			Vector3 vector = Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward;
			for (float num = Random.Range(range / 2f, range); num > 0.5f + requiredRadius; num -= 0.5f)
			{
				Vector3 vector2 = vector * num + startPosition + new Vector3(0f, 0.1f, 0f);
				RaycastHit hitInfo;
				if (!Physics.CheckSphere(vector2 + new Vector3(0f, requiredHeight, 0f), requiredRadius, 804756969) && !Physics.Raycast(startPosition + new Vector3(0f, requiredHeight * 0.5f, 0f), vector, out hitInfo, num, 804756969) && Physics.Raycast(vector2, Vector3.down, out hitInfo, requiredHeight, 804756969))
				{
					return hitInfo.point;
				}
			}
		}
		return startPosition;
	}
}
