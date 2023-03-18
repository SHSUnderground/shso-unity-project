using System;
using System.Collections.Generic;
using UnityEngine;

public class ShsTimerMgr
{
	public class TimerData : IComparable<TimerData>
	{
		public long id;

		public float timeToFire;

		public OnTimer callback;

		public string category;

		public int CompareTo(TimerData other)
		{
			return timeToFire.CompareTo(other.timeToFire);
		}
	}

	public delegate void OnTimer(long timerId, bool canceled);

	protected long ids;

	protected Dictionary<long, TimerData> timers;

	protected BinaryHeap<TimerData> heap;

	public ShsTimerMgr()
	{
		ids = 0L;
		timers = new Dictionary<long, TimerData>();
		heap = new BinaryHeap<TimerData>();
	}

	public long CreateTimer(float seconds, OnTimer callback)
	{
		return CreateTimer(seconds, callback, null);
	}

	public long CreateTimer(float seconds, OnTimer callback, string categoryName)
	{
		if (callback == null)
		{
			throw new InvalidOperationException("callback is null");
		}
		TimerData timerData = new TimerData();
		timerData.id = ++ids;
		timerData.timeToFire = Time.time + seconds;
		timerData.callback = callback;
		timerData.category = categoryName;
		timers.Add(timerData.id, timerData);
		heap.Push(timerData);
		return timerData.id;
	}

	public void CancelTimer(long timerId, bool fireCallback)
	{
		TimerData value;
		if (timers.TryGetValue(timerId, out value))
		{
			timers.Remove(value.id);
			heap.Remove(value);
			if (fireCallback)
			{
				value.callback(value.id, true);
			}
		}
	}

	public void CancelTimer(string categoryName, bool fireCallback)
	{
		List<TimerData> list = new List<TimerData>();
		foreach (TimerData value in timers.Values)
		{
			if (value.category == categoryName)
			{
				list.Add(value);
			}
		}
		foreach (TimerData item in list)
		{
			timers.Remove(item.id);
			heap.Remove(item);
		}
		if (fireCallback)
		{
			foreach (TimerData item2 in list)
			{
				item2.callback(item2.id, true);
			}
		}
	}

	public void Update()
	{
		while (heap.Count > 0)
		{
			TimerData timerData = heap.Peak();
			if (timerData.timeToFire > Time.time)
			{
				break;
			}
			heap.Pop();
			timers.Remove(timerData.id);
			timerData.callback(timerData.id, false);
		}
	}
}
