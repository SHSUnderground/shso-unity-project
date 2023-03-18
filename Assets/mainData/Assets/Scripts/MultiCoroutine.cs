using System;
using System.Collections;
using System.Collections.Generic;

internal class MultiCoroutine : IEnumerator
{
	private Queue<IEnumerator> coroutines = new Queue<IEnumerator>();

	private Exception exception;

	private IEnumerator _currentEnumerator;

	object IEnumerator.Current
	{
		get
		{
			return CurrentEnumerator.Current;
		}
	}

	private IEnumerator CurrentEnumerator
	{
		get
		{
			if (_currentEnumerator != null)
			{
				return _currentEnumerator;
			}
			_currentEnumerator = WaitAll();
			return _currentEnumerator;
		}
		set
		{
			_currentEnumerator = value;
		}
	}

	public MultiCoroutine()
	{
	}

	public MultiCoroutine(IEnumerator co)
	{
		Add(co);
	}

	public MultiCoroutine(IEnumerator co0, IEnumerator co1)
		: this(co0)
	{
		Add(co1);
	}

	public MultiCoroutine(IEnumerator co0, IEnumerator co1, IEnumerator co2)
		: this(co0, co1)
	{
		Add(co2);
	}

	public MultiCoroutine(IEnumerator[] coArray)
	{
		foreach (IEnumerator co in coArray)
		{
			Add(co);
		}
	}

	public void Add(IEnumerator co)
	{
		coroutines.Enqueue(co);
	}

	public IEnumerator WaitAll()
	{
		while (coroutines.Count > 0)
		{
			IEnumerator c = coroutines.Dequeue();
			bool bContinue;
			try
			{
				bContinue = c.MoveNext();
			}
			catch (Exception ex2)
			{
				Exception ex = exception = ex2;
				yield break;
			}
			if (bContinue)
			{
				yield return c.Current;
				coroutines.Enqueue(c);
			}
		}
	}

	public void Throw()
	{
		if (exception != null)
		{
			Exception ex = exception;
			exception = null;
			throw ex;
		}
	}

	public bool MoveNext()
	{
		if (CurrentEnumerator.MoveNext())
		{
			return true;
		}
		CurrentEnumerator = null;
		return false;
	}

	public void Reset()
	{
		CurrentEnumerator = null;
	}
}
