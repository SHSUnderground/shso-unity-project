using System;
using UnityEngine;

[Serializable]
public class ColorAnimation
{
	[Serializable]
	public class ColorInterpolation
	{
		public AnimationCurve curve;

		public Color finalColor = Color.black;

		public float Length
		{
			get
			{
				return curve[curve.length - 1].time;
			}
		}

		public Color Blend(Color original, float time)
		{
			return Color.Lerp(original, finalColor, curve.Evaluate(time));
		}
	}

	public bool loop = true;

	public Color initialColor = Color.black;

	public ColorInterpolation[] colors;

	public float Duration
	{
		get
		{
			float num = 0f;
			ColorInterpolation[] array = colors;
			foreach (ColorInterpolation colorInterpolation in array)
			{
				num += colorInterpolation.Length;
			}
			return num;
		}
	}

	public Color GetColor(float time)
	{
		Color color = initialColor;
		if (loop)
		{
			float duration = Duration;
			if (duration > 0f)
			{
				time %= duration;
			}
		}
		ColorInterpolation[] array = colors;
		foreach (ColorInterpolation colorInterpolation in array)
		{
			float length = colorInterpolation.Length;
			if (time < length)
			{
				color = colorInterpolation.Blend(color, time);
				break;
			}
			color = colorInterpolation.Blend(color, length);
			time -= length;
		}
		return color;
	}
}
