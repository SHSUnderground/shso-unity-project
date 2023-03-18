using System.Collections.Generic;
using UnityEngine;

public class InterpolatorCatmull : SplineInterpolator
{
	internal List<SplinePoint> points;

	protected List<float> times;

	protected List<float> lengths;

	protected float totArcLength;

	public bool Initialize(List<SplinePoint> ctrlPoints)
	{
		if (ctrlPoints.Count <= 3)
		{
			Reset();
			return false;
		}
		points = ctrlPoints;
		times = new List<float>(points.Count);
		float num = 0f;
		float num2 = 1f / (float)(points.Count - 1);
		for (int i = 0; i < points.Count; i++)
		{
			times.Add(num);
			num += num2;
		}
		lengths = new List<float>(points.Count - 1);
		totArcLength = 0f;
		for (int j = 0; j < points.Count - 1; j++)
		{
			float num3 = SegmentArcLength(j, 0f, 1f);
			lengths.Add(num3);
			totArcLength += num3;
		}
		return true;
	}

	public float ArcLength()
	{
		return totArcLength;
	}

	public float ArcLength(float t1, float t2)
	{
		int segment;
		float u;
		GetSegment(ref t1, out segment, out u);
		int segment2;
		float u2;
		GetSegment(ref t2, out segment2, out u2);
		if (segment == segment2)
		{
			return SegmentArcLength(segment, u, u2);
		}
		float num = 0f;
		num += SegmentArcLength(segment, u, 1f);
		num += SegmentArcLength(segment2, 0f, u2);
		for (int i = segment + 1; i < segment2; i++)
		{
			num += lengths[i];
		}
		return num;
	}

	public Vector3 Derivative(float t)
	{
		float u;
		int segment;
		GetSegment(ref t, out segment, out u);
		if (segment == 0)
		{
			SplinePoint splinePoint = points[0];
			Vector3 pt = splinePoint.pt;
			SplinePoint splinePoint2 = points[1];
			Vector3 a = pt - 2f * splinePoint2.pt;
			SplinePoint splinePoint3 = points[2];
			Vector3 a2 = a + splinePoint3.pt;
			SplinePoint splinePoint4 = points[1];
			Vector3 a3 = 4f * splinePoint4.pt;
			SplinePoint splinePoint5 = points[0];
			Vector3 a4 = a3 - 3f * splinePoint5.pt;
			SplinePoint splinePoint6 = points[2];
			Vector3 a5 = a4 - splinePoint6.pt;
			return 0.5f * a5 + u * a2;
		}
		if (segment >= points.Count - 2)
		{
			segment = points.Count - 2;
			SplinePoint splinePoint7 = points[segment - 1];
			Vector3 pt2 = splinePoint7.pt;
			SplinePoint splinePoint8 = points[segment];
			Vector3 a6 = pt2 - 2f * splinePoint8.pt;
			SplinePoint splinePoint9 = points[segment + 1];
			Vector3 a7 = a6 + splinePoint9.pt;
			SplinePoint splinePoint10 = points[segment + 1];
			Vector3 pt3 = splinePoint10.pt;
			SplinePoint splinePoint11 = points[segment - 1];
			Vector3 a8 = pt3 - splinePoint11.pt;
			return 0.5f * a8 + u * a7;
		}
		SplinePoint splinePoint12 = points[segment];
		Vector3 a9 = 3f * splinePoint12.pt;
		SplinePoint splinePoint13 = points[segment - 1];
		Vector3 a10 = a9 - splinePoint13.pt;
		SplinePoint splinePoint14 = points[segment + 1];
		Vector3 a11 = a10 - 3f * splinePoint14.pt;
		SplinePoint splinePoint15 = points[segment + 2];
		Vector3 a12 = a11 + splinePoint15.pt;
		SplinePoint splinePoint16 = points[segment - 1];
		Vector3 a13 = 2f * splinePoint16.pt;
		SplinePoint splinePoint17 = points[segment];
		Vector3 a14 = a13 - 5f * splinePoint17.pt;
		SplinePoint splinePoint18 = points[segment + 1];
		Vector3 a15 = a14 + 4f * splinePoint18.pt;
		SplinePoint splinePoint19 = points[segment + 2];
		Vector3 a16 = a15 - splinePoint19.pt;
		SplinePoint splinePoint20 = points[segment + 1];
		Vector3 pt4 = splinePoint20.pt;
		SplinePoint splinePoint21 = points[segment - 1];
		Vector3 a17 = pt4 - splinePoint21.pt;
		return 0.5f * a17 + u * (a16 + 1.5f * u * a12);
	}

	public float TimeFromDistance(float t1, float s)
	{
		float num = t1;
		float num2 = times[times.Count - 1];
		if (s >= ArcLength(t1, num2))
		{
			return num2;
		}
		if (s <= 0f)
		{
			return num;
		}
		float num3 = t1 + s * (times[times.Count - 1] - times[0]) / totArcLength;
		for (int i = 0; i < 32; i++)
		{
			float num4 = ArcLength(t1, num3) - s;
			if (Mathf.Abs(num4) < 0.001f)
			{
				return num3;
			}
			if (num4 < 0f)
			{
				num = num3;
			}
			else
			{
				num2 = num3;
			}
			float magnitude = Derivative(num3).magnitude;
			num3 = ((!(((num3 - num) * magnitude - num4) * ((num3 - num2) * magnitude - num4) > -0.001f)) ? (num3 - num4 / magnitude) : (0.5f * (num + num2)));
		}
		CspUtils.DebugLog("TimeFromDistance: Failed to find a value");
		return float.NaN;
	}

	public Vector3 GetVector(float t)
	{
		float u;
		int segment;
		GetSegment(ref t, out segment, out u);
		if (segment == 0)
		{
			SplinePoint splinePoint = points[0];
			Vector3 pt = splinePoint.pt;
			SplinePoint splinePoint2 = points[1];
			Vector3 a = pt - 2f * splinePoint2.pt;
			SplinePoint splinePoint3 = points[2];
			Vector3 a2 = a + splinePoint3.pt;
			SplinePoint splinePoint4 = points[1];
			Vector3 a3 = 4f * splinePoint4.pt;
			SplinePoint splinePoint5 = points[0];
			Vector3 a4 = a3 - 3f * splinePoint5.pt;
			SplinePoint splinePoint6 = points[2];
			Vector3 a5 = a4 - splinePoint6.pt;
			SplinePoint splinePoint7 = points[0];
			return splinePoint7.pt + 0.5f * u * (a5 + u * a2);
		}
		if (segment >= points.Count - 2)
		{
			segment = points.Count - 2;
			SplinePoint splinePoint8 = points[segment - 1];
			Vector3 pt2 = splinePoint8.pt;
			SplinePoint splinePoint9 = points[segment];
			Vector3 a6 = pt2 - 2f * splinePoint9.pt;
			SplinePoint splinePoint10 = points[segment + 1];
			Vector3 a7 = a6 + splinePoint10.pt;
			SplinePoint splinePoint11 = points[segment + 1];
			Vector3 pt3 = splinePoint11.pt;
			SplinePoint splinePoint12 = points[segment - 1];
			Vector3 a8 = pt3 - splinePoint12.pt;
			SplinePoint splinePoint13 = points[segment];
			return splinePoint13.pt + 0.5f * u * (a8 + u * a7);
		}
		SplinePoint splinePoint14 = points[segment];
		Vector3 a9 = 3f * splinePoint14.pt;
		SplinePoint splinePoint15 = points[segment - 1];
		Vector3 a10 = a9 - splinePoint15.pt;
		SplinePoint splinePoint16 = points[segment + 1];
		Vector3 a11 = a10 - 3f * splinePoint16.pt;
		SplinePoint splinePoint17 = points[segment + 2];
		Vector3 a12 = a11 + splinePoint17.pt;
		SplinePoint splinePoint18 = points[segment - 1];
		Vector3 a13 = 2f * splinePoint18.pt;
		SplinePoint splinePoint19 = points[segment];
		Vector3 a14 = a13 - 5f * splinePoint19.pt;
		SplinePoint splinePoint20 = points[segment + 1];
		Vector3 a15 = a14 + 4f * splinePoint20.pt;
		SplinePoint splinePoint21 = points[segment + 2];
		Vector3 a16 = a15 - splinePoint21.pt;
		SplinePoint splinePoint22 = points[segment + 1];
		Vector3 pt4 = splinePoint22.pt;
		SplinePoint splinePoint23 = points[segment - 1];
		Vector3 a17 = pt4 - splinePoint23.pt;
		SplinePoint splinePoint24 = points[segment];
		return splinePoint24.pt + 0.5f * u * (a17 + u * (a16 + u * a12));
	}

	public void DrawGizmos()
	{
		float num = 0.01f;
		Vector3 from = GetVector(0f);
		for (float num2 = num; num2 < 1f; num2 += num)
		{
			Gizmos.color = Color.blue;
			Vector3 vector = GetVector(num2);
			Gizmos.DrawLine(from, vector);
			from = vector;
		}
	}

	protected void Reset()
	{
		points = null;
		times = null;
		lengths = null;
		totArcLength = 0f;
	}

	protected float SegmentArcLength(int i, float u1, float u2)
	{
		float[] array = new float[5]
		{
			0f,
			0.5384693f,
			-0.5384693f,
			0.906179845f,
			-0.906179845f
		};
		float[] array2 = new float[5]
		{
			128f / 225f,
			0.478628665f,
			0.478628665f,
			0.236926883f,
			0.236926883f
		};
		if (u2 <= u1)
		{
			return 0f;
		}
		if (u1 < 0f)
		{
			u1 = 0f;
		}
		if (u2 > 1f)
		{
			u2 = 1f;
		}
		float num = 0f;
		Vector3 a2;
		Vector3 a5;
		Vector3 a6;
		if (i == 0)
		{
			SplinePoint splinePoint = points[0];
			Vector3 pt = splinePoint.pt;
			SplinePoint splinePoint2 = points[1];
			Vector3 a = pt - 2f * splinePoint2.pt;
			SplinePoint splinePoint3 = points[2];
			a2 = a + splinePoint3.pt;
			SplinePoint splinePoint4 = points[1];
			Vector3 a3 = 4f * splinePoint4.pt;
			SplinePoint splinePoint5 = points[0];
			Vector3 a4 = a3 - 3f * splinePoint5.pt;
			SplinePoint splinePoint6 = points[2];
			a5 = a4 - splinePoint6.pt;
			a6 = Vector3.zero;
		}
		else if (i >= points.Count - 2)
		{
			i = points.Count - 2;
			SplinePoint splinePoint7 = points[i - 1];
			Vector3 pt2 = splinePoint7.pt;
			SplinePoint splinePoint8 = points[i];
			Vector3 a7 = pt2 - 2f * splinePoint8.pt;
			SplinePoint splinePoint9 = points[i + 1];
			a2 = a7 + splinePoint9.pt;
			SplinePoint splinePoint10 = points[i + 1];
			Vector3 pt3 = splinePoint10.pt;
			SplinePoint splinePoint11 = points[i - 1];
			a5 = pt3 - splinePoint11.pt;
			a6 = Vector3.zero;
		}
		else
		{
			SplinePoint splinePoint12 = points[i];
			Vector3 a8 = 3f * splinePoint12.pt;
			SplinePoint splinePoint13 = points[i - 1];
			Vector3 a9 = a8 - splinePoint13.pt;
			SplinePoint splinePoint14 = points[i + 1];
			Vector3 a10 = a9 - 3f * splinePoint14.pt;
			SplinePoint splinePoint15 = points[i + 2];
			a2 = a10 + splinePoint15.pt;
			SplinePoint splinePoint16 = points[i - 1];
			Vector3 a11 = 2f * splinePoint16.pt;
			SplinePoint splinePoint17 = points[i];
			Vector3 a12 = a11 - 5f * splinePoint17.pt;
			SplinePoint splinePoint18 = points[i + 1];
			Vector3 a13 = a12 + 4f * splinePoint18.pt;
			SplinePoint splinePoint19 = points[i + 2];
			a5 = a13 - splinePoint19.pt;
			SplinePoint splinePoint20 = points[i + 1];
			Vector3 pt4 = splinePoint20.pt;
			SplinePoint splinePoint21 = points[i - 1];
			a6 = pt4 - splinePoint21.pt;
		}
		for (int j = 0; j < 5; j++)
		{
			float num2 = 0.5f * ((u2 - u1) * array[j] + u2 + u1);
			Vector3 vector = (i != 0 && i < points.Count - 2) ? (0.5f * a6 + num2 * (a5 + 1.5f * num2 * a2)) : (0.5f * a5 + num2 * a2);
			num += array2[j] * vector.magnitude;
		}
		return num * (0.5f * (u2 - u1));
	}

	protected void GetSegment(ref float t, out int segment, out float u)
	{
		int num = times.Count - 1;
		if (t <= times[0])
		{
			t = times[0];
			segment = 0;
			u = 0f;
			return;
		}
		if (t >= times[num])
		{
			t = times[num];
			segment = num - 1;
			u = 1f;
			return;
		}
		segment = 0;
		while (segment < num && !(t <= times[segment + 1]))
		{
			segment++;
		}
		float num2 = times[segment];
		float num3 = times[segment + 1];
		u = (t - num2) / (num3 - num2);
	}
}
