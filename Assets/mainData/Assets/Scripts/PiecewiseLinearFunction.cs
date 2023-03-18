using System.Collections.Generic;
using UnityEngine;

public class PiecewiseLinearFunction
{
	protected List<Vector2> points;

	protected bool dirty;

	public PiecewiseLinearFunction()
	{
		points = new List<Vector2>();
		dirty = false;
	}

	public PiecewiseLinearFunction(Vector2[] points)
	{
		this.points = new List<Vector2>(points);
		sort();
	}

	public void AddKey(float time, float value)
	{
		points.Add(new Vector2(time, value));
		dirty = true;
	}

	public float Eval(float time)
	{
		if (dirty)
		{
			sort();
		}
		Vector2 vector = points[0];
		if (time <= vector.x)
		{
			Vector2 vector2 = points[0];
			return vector2.y;
		}
		for (int i = 1; i < points.Count; i++)
		{
			Vector2 vector3 = points[i];
			if (time <= vector3.x)
			{
				Vector2 vector4 = points[i - 1];
				float num = time - vector4.x;
				Vector2 vector5 = points[i];
				float x = vector5.x;
				Vector2 vector6 = points[i - 1];
				float t = num / (x - vector6.x);
				Vector2 vector7 = points[i - 1];
				float y = vector7.y;
				Vector2 vector8 = points[i];
				return Mathf.Lerp(y, vector8.y, t);
			}
		}
		Vector2 vector9 = points[points.Count - 1];
		return vector9.y;
	}

	private void sort()
	{
		dirty = false;
		points.Sort(delegate(Vector2 a, Vector2 b)
		{
			return a.x.CompareTo(b.x);
		});
	}
}
