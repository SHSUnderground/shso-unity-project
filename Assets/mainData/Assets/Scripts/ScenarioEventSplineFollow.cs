using UnityEngine;

[AddComponentMenu("ScenarioEvent/SplineFollow")]
public class ScenarioEventSplineFollow : ScenarioEventHandlerEnableBase
{
	protected override void OnEnableEvent(string eventName)
	{
		base.OnEnableEvent(eventName);
		EnableSplineFollow(true);
	}

	protected void Awake()
	{
		EnableSplineFollow(false);
	}

	protected void EnableSplineFollow(bool enable)
	{
		SplineFollow splineFollowComponent = GetSplineFollowComponent();
		if (splineFollowComponent != null)
		{
			splineFollowComponent.enabled = enable;
		}
	}

	protected SplineFollow GetSplineFollowComponent()
	{
		SplineFollow component = base.gameObject.GetComponent<SplineFollow>();
		if (component == null)
		{
			CspUtils.DebugLog("spline follow component not found on game object <" + base.gameObject.name + "> with spline follow scenario event");
		}
		return component;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "ScenarioEventSplineFollow.png");
	}
}
