using System;
using UnityEngine;

public class PolymorphBaseState : IShsState
{
	protected bool done;

	private PolymorphStateProxy _proxy;

	public PolymorphStateProxy stateProxy
	{
		get
		{
			return _proxy;
		}
	}

	public bool RemotePolymorphEnabled
	{
		get
		{
			return _proxy != null && _proxy.Data != null && _proxy.Data.RemotePolymorphEnabled;
		}
		set
		{
			if (_proxy != null && _proxy.Data != null)
			{
				_proxy.Data.RemotePolymorphEnabled = value;
			}
		}
	}

	protected PolymorphStateData stateData
	{
		get
		{
			return _proxy.Data;
		}
	}

	public PolymorphBaseState(PolymorphStateProxy proxy)
	{
		_proxy = proxy;
	}

	public virtual void Enter(Type previousState)
	{
		done = false;
	}

	public virtual void Update()
	{
	}

	public virtual void Leave(Type nextState)
	{
	}

	public void SetPolymorph(GameObject polymorph)
	{
		if (polymorph != null)
		{
			stateData.Polymorph = polymorph.GetComponent<CharacterGlobals>();
			stateData.PolymorphObject = polymorph;
		}
	}

	public virtual bool Done()
	{
		return done;
	}

	public virtual Type GetNextState()
	{
		throw new NotImplementedException();
	}
}
