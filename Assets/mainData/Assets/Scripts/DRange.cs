using System;
using UnityEngine;

[Serializable]
public class DRange
{
	public int minimum;

	public int maximum;

	public int RandomValue
	{
		get
		{
			return UnityEngine.Random.Range(minimum, maximum);
		}
	}

	public DRange(int minimum, int maximum)
	{
		this.minimum = minimum;
		this.maximum = maximum;
	}

	public bool Contains(int value)
	{
		return value >= minimum && value <= maximum;
	}
}
