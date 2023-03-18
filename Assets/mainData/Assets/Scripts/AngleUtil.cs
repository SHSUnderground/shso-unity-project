using UnityEngine;

public class AngleUtil
{
	public enum RotateDirection
	{
		Clockwise,
		Counterclockwise
	}

	public static float ClampAngle(int angle)
	{
		return ClampAngle(angle, -2.14748365E+09f, 2.14748365E+09f);
	}

	public static float ClampAngle(float angle)
	{
		return ClampAngle(angle, float.MinValue, float.MaxValue);
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		while (angle < 0f)
		{
			angle += 360f;
		}
		while (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	public static float Distance(float sourceAngle, float targetAngle, RotateDirection direction)
	{
		float num;
		if (direction == RotateDirection.Counterclockwise)
		{
			num = sourceAngle - targetAngle;
			if (num < 0f)
			{
				num = 360f + num;
			}
			num = 0f - num;
		}
		else
		{
			num = targetAngle - sourceAngle;
			if (num < 0f)
			{
				num = 360f + num;
			}
		}
		return num;
	}
}
