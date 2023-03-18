using UnityEngine;

public struct Circle
{
	public Vector2 CenterPoint;

	public float Radius;

	public Circle(Vector2 CenterPoint, float Radius)
	{
		this.CenterPoint = CenterPoint;
		this.Radius = Radius;
	}

	public bool Contains(Vector2 PointToTest)
	{
		return (PointToTest - CenterPoint).magnitude <= Radius;
	}
}
