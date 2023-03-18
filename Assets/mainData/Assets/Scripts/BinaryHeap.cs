using System;
using System.Collections.Generic;

public class BinaryHeap<T> where T : IComparable<T>
{
	private List<T> _list;

	public int Count
	{
		get
		{
			return _list.Count;
		}
	}

	public BinaryHeap()
	{
		_list = new List<T>();
	}

	public void Push(T item)
	{
		_list.Add(item);
		BubbleUp(_list.Count - 1);
	}

	public T Pop()
	{
		if (_list.Count <= 0)
		{
			throw new InvalidOperationException("BinaryHeap is empty");
		}
		T result = _list[0];
		_list[0] = _list[_list.Count - 1];
		_list.RemoveAt(_list.Count - 1);
		BubbleDown(0);
		return result;
	}

	public T Peak()
	{
		if (_list.Count <= 0)
		{
			throw new InvalidOperationException("BinaryHeap is empty");
		}
		return _list[0];
	}

	public void Remove(T item)
	{
		for (int i = 0; i < _list.Count; i++)
		{
			if (_list[i].Equals(item))
			{
				_list[i] = _list[_list.Count - 1];
				_list.RemoveAt(_list.Count - 1);
				BubbleDown(i);
				return;
			}
		}
		throw new Exception("Item not found");
	}

	public void Clear()
	{
		_list.Clear();
	}

	private void BubbleUp(int idx)
	{
		while (idx > 0)
		{
			int num = Parent(idx);
			if (_list[idx].CompareTo(_list[num]) < 0)
			{
				T value = _list[num];
				_list[num] = _list[idx];
				_list[idx] = value;
				idx = num;
				continue;
			}
			break;
		}
	}

	private void BubbleDown(int parentIdx)
	{
		int num = 0;
		while (true)
		{
			int num2 = Child1(parentIdx);
			if (num2 >= _list.Count)
			{
				break;
			}
			int num3 = Child2(parentIdx);
			num = ((num3 < _list.Count) ? ((_list[num2].CompareTo(_list[num3]) >= 0) ? num3 : num2) : num2);
			if (_list[parentIdx].CompareTo(_list[num]) > 0)
			{
				T value = _list[parentIdx];
				_list[parentIdx] = _list[num];
				_list[num] = value;
				parentIdx = num;
				continue;
			}
			break;
		}
	}

	private int Parent(int i)
	{
		return i - 1 >> 1;
	}

	private int Child1(int i)
	{
		return (i << 1) + 1;
	}

	private int Child2(int i)
	{
		return (i << 1) + 2;
	}
}
