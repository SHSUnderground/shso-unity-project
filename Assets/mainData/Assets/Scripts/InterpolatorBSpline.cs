using System.Collections.Generic;
using UnityEngine;

public class InterpolatorBSpline : SplineInterpolator
{
	protected List<SplinePoint> points;

	protected float[] times;

	protected float[] lengths;

	protected float totArcLength;

	public bool Initialize(List<SplinePoint> ctrlPoints)
	{
		if (ctrlPoints.Count <= 2)
		{
			Reset();
			return false;
		}
		points = ctrlPoints;
		points.Insert(0, new SplinePoint(ctrlPoints[0]));
		points.Insert(0, new SplinePoint(ctrlPoints[0]));
		points.Add(new SplinePoint(ctrlPoints[ctrlPoints.Count - 1]));
		points.Add(new SplinePoint(ctrlPoints[ctrlPoints.Count - 1]));
		times = new float[points.Count - 2];
		float num = 1f / (float)(points.Count - 3);
		times[0] = 0f;
		for (int i = 0; i < points.Count - 4; i++)
		{
			times[i + 1] = times[i] + num;
		}
		times[points.Count - 3] = 1f;
		lengths = new float[points.Count - 3];
		for (int j = 0; j < lengths.Length; j++)
		{
			float num2 = SegmentArcLength(j, 0f, 1f);
			lengths[j] = num2;
			totArcLength += num2;
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
		int segment;
		float u;
		GetSegment(ref t, out segment, out u);
		Vector3 A;
		Vector3 B;
		Vector3 C;
		Vector3 D;
		SplineParametersForSegment(segment, out A, out B, out C, out D);
		return (C + u * (2f * B + 3f * u * A)) / 6f;
	}

	public float TimeFromDistance(float t1, float s)
	{
		float num = t1;
		float num2 = times[times.Length - 1];
		if (s >= ArcLength(t1, num2))
		{
			return num2;
		}
		if (s <= 0f)
		{
			return num;
		}
		float num3 = t1 + s * (times[times.Length - 1] - times[0]) / totArcLength;
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
		int segment;
		float u;
		GetSegment(ref t, out segment, out u);
		Vector3 A;
		Vector3 B;
		Vector3 C;
		Vector3 D;
		SplineParametersForSegment(segment, out A, out B, out C, out D);
		return CalcPoint(u, A, B, C, D);
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
		Vector3 A;
		Vector3 B;
		Vector3 C;
		Vector3 D;
		SplineParametersForSegment(i, out A, out B, out C, out D);
		i += 3;
		float num = 0f;
		for (int j = 0; j < 5; j++)
		{
			float num2 = 0.5f * ((u2 - u1) * array[j] + u2 + u1);
			Vector3 vector = (C + num2 * (2f * B + 3f * num2 * A)) / 6f;
			num += array2[j] * vector.magnitude;
		}
		return num * (0.5f * (u2 - u1));
	}

	protected void GetSegment(ref float t, out int segment, out float u)
	{
		int num = times.Length - 1;
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

	protected void SplineParametersForSegment(int segment, out Vector3 A, out Vector3 B, out Vector3 C, out Vector3 D)
	{
		segment += 3;
		SplinePoint splinePoint = points[segment];
		Vector3 pt = splinePoint.pt;
		SplinePoint splinePoint2 = points[segment - 1];
		Vector3 pt2 = splinePoint2.pt;
		SplinePoint splinePoint3 = points[segment - 2];
		Vector3 pt3 = splinePoint3.pt;
		SplinePoint splinePoint4 = points[segment - 3];
		Vector3 pt4 = splinePoint4.pt;
		A = pt - 3f * pt2 + 3f * pt3 - pt4;
		B = 3f * pt2 - 6f * pt3 + 3f * pt4;
		C = 3f * pt2 - 3f * pt4;
		D = pt2 + 4f * pt3 + pt4;
	}

	protected Vector3 CalcPoint(float u, Vector3 A, Vector3 B, Vector3 C, Vector3 D)
	{
		return (D + u * (C + u * (B + u * A))) / 6f;
	}
}
