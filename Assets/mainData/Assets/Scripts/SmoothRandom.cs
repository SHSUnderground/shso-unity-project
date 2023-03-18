using UnityEngine;

public class SmoothRandom
{
	private static FractalNoise s_Noise;

	public static Vector3 GetVector3(float speed)
	{
		float x = Time.time * 0.01f * speed;
		return new Vector3(Get().HybridMultifractal(x, 15.73f, 0.58f), Get().HybridMultifractal(x, 63.94f, 0.58f), Get().HybridMultifractal(x, 0.2f, 0.58f));
	}

	public static float Get(float speed)
	{
		float num = Time.time * 0.01f * speed;
		return Get().HybridMultifractal(num * 0.01f, 15.7f, 0.65f);
	}

	private static FractalNoise Get()
	{
		if (s_Noise == null)
		{
			s_Noise = new FractalNoise(1.27f, 2.04f, 8.36f);
		}
		return s_Noise;
	}
}
