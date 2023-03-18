using System;
using UnityEngine;

public class MathfEx
{
	public static float Hermite(float start, float end, float t)
	{
		return Mathf.Lerp(start, end, t * t * (3f - 2f * t));
	}

	public static Vector3 Hermite(Vector3 start, Vector3 end, float value)
	{
		float x = Hermite(start.x, end.x, value);
		float y = Hermite(start.y, end.y, value);
		float z = Hermite(start.z, end.z, value);
		return new Vector3(x, y, z);
	}

	public static float Sinerp(float start, float end, float t)
	{
		return Mathf.Lerp(start, end, Mathf.Sin(t * (float)Math.PI * 0.5f));
	}

	public static Vector3 Sinerp(Vector3 start, Vector3 end, float value)
	{
		float x = Sinerp(start.x, end.x, value);
		float y = Sinerp(start.y, end.y, value);
		float z = Sinerp(start.z, end.z, value);
		return new Vector3(x, y, z);
	}

	public static float Coserp(float start, float end, float t)
	{
		return Mathf.Lerp(start, end, 1f - Mathf.Cos(t * (float)Math.PI * 0.5f));
	}

	public static Vector3 Coserp(Vector3 start, Vector3 end, float t)
	{
		float x = Coserp(start.x, end.x, t);
		float y = Coserp(start.y, end.y, t);
		float z = Coserp(start.z, end.z, t);
		return new Vector3(x, y, z);
	}

	public static float Berp(float start, float end, float t)
	{
		t = Mathf.Clamp01(t);
		t = (Mathf.Sin(t * (float)Math.PI * (0.2f + 2.5f * t * t * t)) * Mathf.Pow(1f - t, 2.2f) + t) * (1f + 1.2f * (1f - t));
		return start + (end - start) * t;
	}

	public static Vector3 Berp(Vector3 start, Vector3 end, float t)
	{
		float x = Berp(start.x, end.x, t);
		float y = Berp(start.y, end.y, t);
		float z = Berp(start.z, end.z, t);
		return new Vector3(x, y, z);
	}

	public static float Lerp(float start, float end, float t)
	{
		return (1f - t) * start + t * end;
	}

	public static float Bounce(float x)
	{
		return Mathf.Abs(Mathf.Sin(6.28f * (x + 1f) * (x + 1f)) * (1f - x));
	}

	public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref float currentSpeed, float smoothTime, float maxSpeed, float deltaTime)
	{
		Vector3 vector = target - current;
		float d = Mathf.SmoothDamp(vector.magnitude, 0f, ref currentSpeed, smoothTime, maxSpeed, deltaTime);
		return current + vector.normalized * d;
	}

	public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
	{
		Vector3 vector = Vector3.Normalize(lineEnd - lineStart);
		float d = Vector3.Dot(point - lineStart, vector) / Vector3.Dot(vector, vector);
		return lineStart + d * vector;
	}

	public static Vector3 NearestPointStrict(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
	{
		Vector3 vector = lineEnd - lineStart;
		Vector3 vector2 = Vector3.Normalize(vector);
		float value = Vector3.Dot(point - lineStart, vector2) / Vector3.Dot(vector2, vector2);
		return lineStart + Mathf.Clamp(value, 0f, Vector3.Magnitude(vector)) * vector2;
	}

	public static bool Approx(float val, float about, float range)
	{
		return Mathf.Abs(val - about) < range;
	}

	public static bool Approx(Vector3 val, Vector3 about, float range)
	{
		return (val - about).sqrMagnitude < range * range;
	}

	public static float Clerp(float start, float end, float value)
	{
		float num = 0f;
		float num2 = 360f;
		float num3 = Mathf.Abs((num2 - num) / 2f);
		float num4 = 0f;
		float num5 = 0f;
		if (end - start < 0f - num3)
		{
			num5 = (num2 - start + end) * value;
			return start + num5;
		}
		if (end - start > num3)
		{
			num5 = (0f - (num2 - end + start)) * value;
			return start + num5;
		}
		return start + (end - start) * value;
	}

	public static T Clamp<T>(T Value, T Min, T Max) where T : IComparable<T>
	{
		if (Value.CompareTo(Max) > 0)
		{
			return Max;
		}
		if (Value.CompareTo(Min) < 0)
		{
			return Min;
		}
		return Value;
	}

	public static Plane NormalizedPlaneFromVector(ref Vector4 v)
	{
		float num = 1f / Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
		v.x *= num;
		v.y *= num;
		v.z *= num;
		v.w *= num;
		return new Plane(v, v.w);
	}

	public static Plane[] GetFrustrumPlanes(ref Matrix4x4 mat)
	{
		Plane[] array = new Plane[6];
		Vector4 v = mat.GetRow(3) + mat.GetRow(0);
		array[0] = NormalizedPlaneFromVector(ref v);
		v = mat.GetRow(3) - mat.GetRow(0);
		array[1] = NormalizedPlaneFromVector(ref v);
		v = mat.GetRow(3) + mat.GetRow(1);
		array[2] = NormalizedPlaneFromVector(ref v);
		v = mat.GetRow(3) - mat.GetRow(1);
		array[3] = NormalizedPlaneFromVector(ref v);
		v = mat.GetRow(3) + mat.GetRow(2);
		array[4] = NormalizedPlaneFromVector(ref v);
		v = mat.GetRow(3) - mat.GetRow(2);
		array[5] = NormalizedPlaneFromVector(ref v);
		return array;
	}
}
