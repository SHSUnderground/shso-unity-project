using UnityEngine;

public class DynamicPoint
{
	private Vector2 _v = default(Vector2);

	private string _ident = string.Empty;

	public float x
	{
		get
		{
			if (_ident == string.Empty)
			{
				return _v.x;
			}
			return _v.x;
		}
		set
		{
			_v.x = value;
		}
	}

	public float y
	{
		get
		{
			if (_ident == string.Empty)
			{
				return _v.y;
			}
			return _v.y;
		}
		set
		{
			_v.y = value;
		}
	}

	public DynamicPoint(float x = 0f, float y = 0f)
	{
		_v.x = x;
		_v.y = y;
	}

	public void setIdent(string id)
	{
		_ident = id;
	}

	public DynamicPoint offset(DynamicPoint off)
	{
		DynamicPoint dynamicPoint = new DynamicPoint(_v.x + off.x, _v.y + off.y);
		dynamicPoint.setIdent(_ident);
		return dynamicPoint;
	}

	public DynamicPoint center(DynamicPoint off)
	{
		DynamicPoint dynamicPoint = new DynamicPoint(_v.x - off.x / 2f, _v.y - off.y / 2f);
		dynamicPoint.setIdent(_ident);
		return dynamicPoint;
	}

	public static DynamicPoint toIdent(string id)
	{
		DynamicPoint dynamicPoint = new DynamicPoint(0f, 0f);
		dynamicPoint.setIdent(id);
		return dynamicPoint;
	}
}
