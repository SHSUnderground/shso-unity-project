using System;
using UnityEngine;

[Serializable]
public class SplineRotation : SplineEvent
{
	public Quaternion rot = Quaternion.identity;

	public SplineRotation(SplineRotation old)
	{
		time = old.time;
		rot = old.rot;
	}

	public SplineRotation(Quaternion r, float t)
	{
		time = t;
		rot = r;
	}
}
