using System;
using UnityEngine;

public class FractalNoise
{
	private Perlin m_Noise;

	private float[] m_Exponent;

	private int m_IntOctaves;

	private float m_Octaves;

	private float m_Lacunarity;

	public FractalNoise(float inH, float inLacunarity, float inOctaves)
		: this(inH, inLacunarity, inOctaves, null)
	{
	}

	public FractalNoise(float inH, float inLacunarity, float inOctaves, Perlin noise)
	{
		m_Lacunarity = inLacunarity;
		m_Octaves = inOctaves;
		m_IntOctaves = (int)inOctaves;
		m_Exponent = new float[m_IntOctaves + 1];
		float num = 1f;
		for (int i = 0; i < m_IntOctaves + 1; i++)
		{
			m_Exponent[i] = (float)Math.Pow(m_Lacunarity, 0f - inH);
			num *= m_Lacunarity;
		}
		if (noise == null)
		{
			m_Noise = new Perlin();
		}
		else
		{
			m_Noise = noise;
		}
	}

	public float HybridMultifractal(float x, float y, float offset)
	{
		float num = (m_Noise.Noise(x, y) + offset) * m_Exponent[0];
		float num2 = num;
		x *= m_Lacunarity;
		y *= m_Lacunarity;
		int i;
		for (i = 1; i < m_IntOctaves; i++)
		{
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			float num3 = (m_Noise.Noise(x, y) + offset) * m_Exponent[i];
			num += num2 * num3;
			num2 *= num3;
			x *= m_Lacunarity;
			y *= m_Lacunarity;
		}
		float num4 = m_Octaves - (float)m_IntOctaves;
		return num + num4 * m_Noise.Noise(x, y) * m_Exponent[i];
	}

	public float RidgedMultifractal(float x, float y, float offset, float gain)
	{
		float num = Mathf.Abs(m_Noise.Noise(x, y));
		num = offset - num;
		num *= num;
		float num2 = num;
		float num3 = 1f;
		for (int i = 1; i < m_IntOctaves; i++)
		{
			x *= m_Lacunarity;
			y *= m_Lacunarity;
			num3 = num * gain;
			num3 = Mathf.Clamp01(num3);
			num = Mathf.Abs(m_Noise.Noise(x, y));
			num = offset - num;
			num *= num;
			num *= num3;
			num2 += num * m_Exponent[i];
		}
		return num2;
	}

	public float BrownianMotion(float x, float y)
	{
		float num = 0f;
		long num2;
		for (num2 = 0L; num2 < m_IntOctaves; num2++)
		{
			num = m_Noise.Noise(x, y) * m_Exponent[num2];
			x *= m_Lacunarity;
			y *= m_Lacunarity;
		}
		float num3 = m_Octaves - (float)m_IntOctaves;
		return num + num3 * m_Noise.Noise(x, y) * m_Exponent[num2];
	}
}
