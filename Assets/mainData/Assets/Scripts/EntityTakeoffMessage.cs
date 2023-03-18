using UnityEngine;

public class EntityTakeoffMessage : ShsEventMessage
{
	public GameObject entity;

	public SplineController spline;

	public EntityTakeoffMessage(GameObject entity, SplineController spline)
	{
		this.entity = entity;
		this.spline = spline;
	}
}
