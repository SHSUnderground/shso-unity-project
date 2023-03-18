using UnityEngine;

public struct Bounds2
{
	private bool initialized;

	private Vector2 _center;

	private Vector2 _extents;

	public Vector2 center
	{
		get
		{
			return _center;
		}
		set
		{
			_center = value;
		}
	}

	public Vector2 extents
	{
		get
		{
			return _extents;
		}
	}

	public Vector2 max
	{
		get
		{
			return _center + _extents;
		}
	}

	public Vector2 min
	{
		get
		{
			return _center - _extents;
		}
	}

	public Vector2 size
	{
		get
		{
			return _extents * 2f;
		}
	}

	public Bounds2(Vector2 center, Vector2 size)
	{
		initialized = true;
		_center = center;
		_extents = size * 0.5f;
	}

	public void SetMinMax(Vector2 min, Vector2 max)
	{
		_extents = (max - min) * 0.5f;
		_center = min + _extents;
	}

	public void Encapsulate(Vector2 point)
	{
		if (initialized)
		{
			Vector2 min = this.min;
			Vector2 max = this.max;
			Vector2 min2 = new Vector2(Mathf.Min(min.x, point.x), Mathf.Min(min.y, point.y));
			Vector2 max2 = new Vector2(Mathf.Max(max.x, point.x), Mathf.Max(max.y, point.y));
			SetMinMax(min2, max2);
		}
		else
		{
			initialized = true;
			_center = point;
			_extents = Vector2.zero;
		}
	}

	public void EncapsulateXY(Vector3 point)
	{
		Encapsulate(new Vector2(point.x, point.y));
	}

	public void Encapsulate(Vector3 point)
	{
		Encapsulate(new Vector2(point.x, point.z));
	}

	public bool Contains(Vector2 point)
	{
		if (initialized)
		{
			Vector2 min = this.min;
			Vector2 max = this.max;
			if (point.x < min.x)
			{
				return false;
			}
			if (point.x > max.x)
			{
				return false;
			}
			if (point.y < min.y)
			{
				return false;
			}
			if (point.y > max.y)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public bool Contains(Vector3 point)
	{
		return Contains(new Vector2(point.x, point.z));
	}

	public bool ContainsXY(Vector3 point)
	{
		return Contains(new Vector2(point.x, point.y));
	}

	public bool Intersects(Bounds2 check)
	{
		if (initialized && check.initialized)
		{
			Vector2 min = this.min;
			Vector2 max = this.max;
			Vector2 min2 = check.min;
			Vector2 max2 = check.max;
			return min.x < max2.x && max.x > min2.x && min.y < max2.y && max.y > min2.y;
		}
		return false;
	}

	public override string ToString()
	{
		if (initialized)
		{
			object[] obj = new object[9]
			{
				"[",
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			Vector2 center = this.center;
			obj[1] = center.x;
			obj[2] = ",";
			Vector2 center2 = this.center;
			obj[3] = center2.y;
			obj[4] = ":";
			Vector2 extents = this.extents;
			obj[5] = extents.x;
			obj[6] = ",";
			Vector2 extents2 = this.extents;
			obj[7] = extents2.y;
			obj[8] = "]";
			return string.Concat(obj);
		}
		return "[unintialized]";
	}
}
