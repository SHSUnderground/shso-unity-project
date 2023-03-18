using System.Collections.Generic;
using UnityEngine;

internal class InterpolatorLinear : SplineInterpolator
{
	protected List<SplinePoint> points;

	protected float[] times;

	protected float[] lengths;

	protected float totArcLength;

	protected float invVelocity;

	public bool Initialize(List<SplinePoint> ctrlPoints)
	{
		if (ctrlPoints.Count <= 1)
		{
			Reset();
			return false;
		}
		points = ctrlPoints;
		totArcLength = 0f;
		lengths = new float[points.Count - 1];
		for (int i = 0; i < lengths.Length; i++)
		{
			SplinePoint splinePoint = points[i + 1];
			Vector3 pt = splinePoint.pt;
			SplinePoint splinePoint2 = points[i];
			float magnitude = (pt - splinePoint2.pt).magnitude;
			lengths[i] = magnitude;
			totArcLength += magnitude;
		}
		if (totArcLength <= 0f)
		{
			Reset();
			return false;
		}
		invVelocity = 1f / totArcLength;
		times = new float[points.Count];
		times[0] = 0f;
		for (int j = 1; j < times.Length - 1; j++)
		{
			float num = lengths[j - 1] / totArcLength;
			times[j] = times[j - 1] + num;
		}
		times[times.Length - 1] = 1f;
		return true;
	}

	public float ArcLength()
	{
		return totArcLength;
	}

	public float ArcLength(float t0, float t1)
	{
		int segment;
		GetSegment(ref t0, out segment);
		int segment2;
		GetSegment(ref t1, out segment2);
		if (t0 >= t1)
		{
			return 0f;
		}
		Vector3 point = GetPoint(t0, segment);
		Vector3 point2 = GetPoint(t1, segment2);
		if (segment == segment2)
		{
			return (point2 - point).magnitude;
		}
		float num = 0f;
		float num2 = num;
		SplinePoint splinePoint = points[segment + 1];
		num = num2 + (point - splinePoint.pt).magnitude;
		float num3 = num;
		SplinePoint splinePoint2 = points[segment2];
		num = num3 + (point2 - splinePoint2.pt).magnitude;
		for (int i = segment + 1; i < segment2; i++)
		{
			num += lengths[i];
		}
		return num;
	}

	public Vector3 Derivative(float t)
	{
		int segment;
		GetSegment(ref t, out segment);
		SplinePoint splinePoint = points[segment];
		SplinePoint splinePoint2 = points[segment + 1];
		return (splinePoint2.pt - splinePoint.pt) / (times[segment + 1] - times[segment]);
	}

	public float TimeFromDistance(float t1, float s)
	{
		float value = t1 + s * invVelocity;
		return Mathf.Clamp(value, times[0], times[times.Length - 1]);
	}

	public Vector3 GetVector(float t)
	{
		int segment;
		GetSegment(ref t, out segment);
		SplinePoint splinePoint = points[segment];
		SplinePoint splinePoint2 = points[segment + 1];
		float t2 = (t - times[segment]) / (times[segment + 1] - times[segment]);
		return Vector3.Lerp(splinePoint.pt, splinePoint2.pt, t2);
	}

	public void DrawGizmos()
	{
		for (int i = 1; i < points.Count; i++)
		{
			Gizmos.color = Color.blue;
			SplinePoint splinePoint = points[i - 1];
			Vector3 pt = splinePoint.pt;
			SplinePoint splinePoint2 = points[i];
			Gizmos.DrawLine(pt, splinePoint2.pt);
		}
	}

	protected void Reset()
	{
		points = null;
		times = null;
		lengths = null;
		totArcLength = 0f;
	}

	protected void GetSegment(ref float t, out int segment)
	{
		if (t <= times[0])
		{
			t = times[0];
			segment = 0;
			return;
		}
		int num = times.Length - 1;
		if (t >= times[num])
		{
			t = times[num];
			segment = num - 1;
			return;
		}
		segment = 0;
		while (segment < num && !(t <= times[segment + 1]))
		{
			segment++;
		}
	}

	protected Vector3 GetPoint(float t, int segment)
	{
		SplinePoint splinePoint = points[segment];
		SplinePoint splinePoint2 = points[segment + 1];
		float t2 = (t - times[segment]) / (times[segment + 1] - times[segment]);
		return Vector3.Lerp(splinePoint.pt, splinePoint2.pt, t2);
	}
}
