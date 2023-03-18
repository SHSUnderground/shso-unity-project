using System;
using System.Collections;
using System.Collections.Generic;

public class PrizeWheelBase : IEnumerable, IEnumerable<PrizeWheelStop>
{
	public const int STOP_COUNT = 24;

	private List<PrizeWheelStop> stops;

	public PrizeWheelStop this[int i]
	{
		get
		{
			return stops[i];
		}
		set
		{
			stops[i] = value;
		}
	}

	public int Count
	{
		get
		{
			return Math.Max(stops.Count - 1, 0);
		}
	}

	public PrizeWheelBase()
	{
		stops = new List<PrizeWheelStop>(24);
		for (int i = 0; i <= 24; i++)
		{
			stops.Add(null);
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		for (int i = 1; i <= 24; i++)
		{
			yield return stops[i];
		}
	}

	public PrizeWheelStop GetNextStop(PrizeWheelStop stop)
	{
		if (!stops.Contains(stop))
		{
			return null;
		}
		int index = stop.index;
		foreach (PrizeWheelStop stop2 in stops)
		{
			if (stop2 != null && stop2.index == index + 1)
			{
				return stop2;
			}
		}
		return stops[1];
	}

	public PrizeWheelStop GetPrevStop(PrizeWheelStop stop)
	{
		if (!stops.Contains(stop))
		{
			return null;
		}
		int index = stop.index;
		foreach (PrizeWheelStop stop2 in stops)
		{
			if (stop2 != null && stop2.index == index - 1)
			{
				return stop2;
			}
		}
		return stops[24];
	}

	public IEnumerator<PrizeWheelStop> GetEnumerator()
	{
		for (int i = 1; i <= 24; i++)
		{
			yield return stops[i];
		}
	}
}
