using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Camera/Camera Lite Multi Target")]
public class CameraLiteMultiTarget : CameraLiteOffset
{
	protected List<CombatController> combatTargets;

	protected List<GameObject> targets;

	protected override void computeDestination()
	{
		if (targets != null)
		{
			Vector3 zero = Vector3.zero;
			float num = 0f;
			int num2 = 0;
			foreach (GameObject target2 in targets)
			{
				if (!(target2 == null))
				{
					zero += target2.transform.position + Vector3.up;
					num2++;
					foreach (GameObject target3 in targets)
					{
						num = Mathf.Max(num, Vector3.Distance(target2.transform.position, target3.transform.position));
					}
					foreach (CombatController combatTarget in combatTargets)
					{
						num = Mathf.Max(num, Vector3.Distance(target2.transform.position, combatTarget.TargetPosition));
					}
				}
			}
			foreach (CombatController combatTarget2 in combatTargets)
			{
				if (!(combatTarget2 == null))
				{
					zero += combatTarget2.TargetPosition + Vector3.up;
					num2++;
					foreach (GameObject target4 in targets)
					{
						num = Mathf.Max(num, Vector3.Distance(combatTarget2.TargetPosition, target4.transform.position));
					}
					foreach (CombatController combatTarget3 in combatTargets)
					{
						num = Mathf.Max(num, Vector3.Distance(combatTarget2.TargetPosition, combatTarget3.TargetPosition));
					}
				}
			}
			if (num2 > 0)
			{
				zero /= (float)num2;
			}
			float num3 = (!reverseAngle) ? 1 : (-1);
			Vector3 eulerAngles = new Vector3(0f - verticalAngle, rotationAngle * num3, 0f);
			destination.transform.position = zero;
			destination.transform.eulerAngles = eulerAngles;
			destination.transform.position = destination.transform.position + destination.transform.forward * (num + distance);
			destination.transform.LookAt(zero);
		}
	}

	public void assignTargets(List<GameObject> newTargets)
	{
		targets = new List<GameObject>();
		combatTargets = new List<CombatController>();
		foreach (GameObject newTarget in newTargets)
		{
			CombatController combatController = newTarget.GetComponent(typeof(CombatController)) as CombatController;
			if (combatController != null)
			{
				combatTargets.Add(combatController);
			}
			else
			{
				targets.Add(newTarget);
			}
		}
	}
}
