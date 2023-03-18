using UnityEngine;

public struct SplinePoint
{
	public Vector3 pt;

	public Quaternion rot;

	public SplinePoint(SplineControlPoint pt)
	{
		this.pt = pt.transform.position;
		rot = pt.transform.rotation;
	}

	public SplinePoint(SplinePoint pt)
	{
		this.pt = pt.pt;
		rot = pt.rot;
	}

	public SplinePoint(Vector3 pt, Quaternion rot)
	{
		this.pt = pt;
		this.rot = rot;
	}
}
