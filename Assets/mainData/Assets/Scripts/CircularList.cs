using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class CircularList<T> : List<T>
{
	[CompilerGenerated]
	private double _003CBasePosition_003Ek__BackingField;

	public double BasePosition
	{
		[CompilerGenerated]
		get
		{
			return _003CBasePosition_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CBasePosition_003Ek__BackingField = value;
		}
	}

	public double GetPosition(T index)
	{
		if (!Contains(index))
		{
			return double.NaN;
		}
		int num = IndexOf(index);
		return GetCorrectPosition(num);
	}

	public List<double> GetMap()
	{
		List<double> list = new List<double>(Count);
		for (int i = 0; i < Count; i++)
		{
			list.Add(GetCorrectPosition(i));
		}
		return list;
	}

	private double GetCorrectPosition(double index)
	{
		double num = SignCorrectMod(BasePosition, Count);
		double num2 = SignCorrectMod(index - num, Count);
		double num3 = (!(num2 > 0.0)) ? ((double)Count + num2) : (num2 - (double)Count);
		return (!(Math.Abs(num2) < Math.Abs(num3))) ? num3 : num2;
	}

	private double SignCorrectMod(double i, double j)
	{
		return Math.Abs(i) % j * (double)Math.Sign(i);
	}
}
