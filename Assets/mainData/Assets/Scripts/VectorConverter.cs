using UnityEngine;

public class VectorConverter
{
	public static Vector2 Convert(Vector3 vector)
	{
		return new Vector2(vector.x, vector.z);
	}

	public static Vector3 Convert(Vector2 vector)
	{
		return new Vector3(vector.x, 0f, vector.y);
	}

	public static Vector3 Convert(Vector2 vector, float y)
	{
		return new Vector3(vector.x, y, vector.y);
	}
}
