using System;
using UnityEngine;

[Serializable]
public class FRange
{
	public float minimum;

	public float maximum;

	public float RandomValue
	{
		get
		{
			return UnityEngine.Random.Range(minimum, maximum);
		}
	}

	public FRange(float minimum, float maximum)
	{
		this.minimum = minimum;
		this.maximum = maximum;
	}
}
