using UnityEngine;

public class AttackColliderConeController : AttackColliderController
{
	protected override void OnTriggerStay(Collider other)
	{
		if (!hitTargets.ContainsKey(other.gameObject.GetInstanceID()))
		{
			base.OnTriggerEnter(other);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		int instanceID = other.gameObject.GetInstanceID();
		if (hitTargets.ContainsKey(instanceID))
		{
			hitTargets.Remove(instanceID);
		}
	}

	protected override bool checkExtraConditions(GameObject targetObject)
	{
		Vector3 from = targetObject.transform.position - base.gameObject.transform.position;
		from.y = 0f;
		Vector3 forward = base.gameObject.transform.forward;
		forward.y = 0f;
		float num = Vector3.Angle(from, forward);
		if (num < impactData.colliderAngleLimit)
		{
			return base.checkExtraConditions(targetObject);
		}
		return false;
	}
}
