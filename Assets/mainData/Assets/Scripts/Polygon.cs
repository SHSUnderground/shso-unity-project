using UnityEngine;

public struct Polygon
{
	public Vector2[] PolyPoints;

	public Polygon(Vector2[] PolyPoints)
	{
		this.PolyPoints = PolyPoints;
	}

	public bool Contains(Vector2 PointToTest)
	{
		bool flag = false;
		if (PolyPoints.Length < 3)
		{
			return flag;
		}
		Vector2 vector = new Vector2(PolyPoints[PolyPoints.Length - 1].x, PolyPoints[PolyPoints.Length - 1].y);
		for (int i = 0; i < PolyPoints.Length; i++)
		{
			Vector2 vector2 = new Vector2(PolyPoints[i].x, PolyPoints[i].y);
			Vector2 vector3;
			Vector2 vector4;
			if (vector2.x > vector.x)
			{
				vector3 = vector;
				vector4 = vector2;
			}
			else
			{
				vector3 = vector2;
				vector4 = vector;
			}
			if (vector2.x < PointToTest.x == PointToTest.x <= vector.x && ((long)PointToTest.y - (long)vector3.y) * (long)(vector4.x - vector3.x) < ((long)vector4.y - (long)vector3.y) * (long)(PointToTest.x - vector3.x))
			{
				flag = !flag;
			}
			vector = vector2;
		}
		return flag;
	}
}
