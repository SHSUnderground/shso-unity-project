using System;
using System.Collections.Generic;

public class ShsFSM : IDisposable
{
	protected bool disposed;

	protected IShsState current;

	protected List<Type> stack;

	protected Dictionary<Type, IShsState> states;

	public IShsState CurrentStateInstance
	{
		get
		{
			return current;
		}
	}

	public ShsFSM()
	{
		current = null;
		stack = new List<Type>();
		states = new Dictionary<Type, IShsState>();
	}

	~ShsFSM()
	{
		Dispose(false);
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				foreach (IShsState value in states.Values)
				{
					IDisposable disposable = value as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
				states.Clear();
				states = null;
				current = null;
			}
			disposed = true;
		}
	}

	public void AddState(IShsState state)
	{
		if (states.ContainsKey(state.GetType()))
		{
			throw new Exception("state id already exists");
		}
		states.Add(state.GetType(), state);
	}

	public void GotoState<T>() where T : IShsState
	{
		GotoState(typeof(T));
	}

	public void GotoState(Type t)
	{
		IShsState value = null;
		if (states.TryGetValue(t, out value))
		{
			SetCurrentState(value);
			return;
		}
		throw new Exception("not found");
	}

	public void PushState(IShsState state)
	{
		AddState(state);
		stack.Add(state.GetType());
	}

	public void PushState<T>() where T : IShsState
	{
		if (current == null)
		{
			GotoState<T>();
		}
		else if (current.GetType() != typeof(T))
		{
			stack.Add(current.GetType());
			GotoState<T>();
		}
	}

	public void PopState()
	{
		if (stack.Count > 0)
		{
			Type key = stack[stack.Count - 1];
			stack.RemoveAt(stack.Count - 1);
			SetCurrentState(states[key]);
		}
		else
		{
			SetCurrentState(null);
		}
	}

	public void ClearState()
	{
		SetCurrentState(null);
	}

	public T GetState<T>() where T : IShsState
	{
		IShsState value = null;
		if (states.TryGetValue(typeof(T), out value))
		{
			return (T)value;
		}
		throw new Exception("not found");
	}

	public object GetCurrentStateObject()
	{
		return current;
	}

	public Type GetCurrentState()
	{
		if (current != null)
		{
			return current.GetType();
		}
		return null;
	}

	public void Update()
	{
		if (current != null)
		{
			current.Update();
		}
	}

	protected void SetCurrentState(IShsState state)
	{
		if (current != state)
		{
			IShsState shsState = current;
			current = state;
			Type previousState = null;
			if (shsState != null)
			{
				previousState = shsState.GetType();
				shsState.Leave((state == null) ? null : state.GetType());
			}
			if (current != null)
			{
				current.Enter(previousState);
			}
		}
	}
}
