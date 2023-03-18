using UnityEngine;

[AddComponentMenu("Interactive Object/Teleport Controller")]
public class TeleportInteractiveObjectController : SplineInteractiveObjectBaseController
{
	public float teleportStartTime;

	public GameObject teleportStartEffectPrefab;

	public GameObject teleportEndEffectPrefab;

	public GameObject teleportPurgatory;

	public GameObject teleportDestination;

	public float teleportTime;

	public bool useSpline = true;

	public override bool IsControllerHotSpotType(GameObject player)
	{
		return true;
	}

	protected override void ApproachArrived(GameObject obj)
	{
		base.ApproachArrived(obj);
		Teleport(obj);
	}

	protected override void SplineDone(GameObject obj)
	{
		base.SplineDone(obj);
		BehaviorManager component = obj.GetComponent<BehaviorManager>();
		if (component != null)
		{
			BehaviorTeleport behaviorTeleport = component.getBehavior() as BehaviorTeleport;
			if (behaviorTeleport != null)
			{
				component.endBehavior();
			}
		}
		if (teleportEndEffectPrefab != null)
		{
			Object.Instantiate(teleportEndEffectPrefab, obj.transform.position, obj.transform.rotation);
		}
	}

	protected void Teleport(GameObject obj)
	{
		if (!useSpline && teleportDestination == null)
		{
			CspUtils.DebugLog("Teleport Destination needed if splines are not being used for teleportation");
			return;
		}
		if (teleportStartEffectPrefab != null)
		{
			Object.Instantiate(teleportStartEffectPrefab, obj.transform.position, obj.transform.rotation);
		}
		BehaviorManager component = obj.GetComponent<BehaviorManager>();
		if (component != null)
		{
			BehaviorTeleport behaviorTeleport = component.requestChangeBehavior(typeof(BehaviorTeleport), true) as BehaviorTeleport;
			if (behaviorTeleport != null)
			{
				behaviorTeleport.Initialize(SplineDone, teleportPurgatory, teleportStartTime, useSpline, spline, teleportDestination, teleportTime, true);
			}
		}
	}
}
