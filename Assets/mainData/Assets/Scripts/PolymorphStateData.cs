using System;
using UnityEngine;

public abstract class PolymorphStateData
{
	private CharacterGlobals _characterToPolymorph;

	private CharacterGlobals _polymorph;

	private GameObject _polymorphObject;

	private NetworkComponent _polymorphNetCmp;

	private string _polymorphEffect;

	private string _revertEffect;

	private bool _remotePolymorphEnabled;

	public CharacterGlobals Original
	{
		get
		{
			return _characterToPolymorph;
		}
	}

	public CharacterGlobals Polymorph
	{
		get
		{
			return _polymorph;
		}
		set
		{
			_polymorph = value;
		}
	}

	public GameObject OriginalObject
	{
		get
		{
			return _characterToPolymorph.gameObject;
		}
	}

	public GameObject PolymorphObject
	{
		get
		{
			return _polymorphObject;
		}
		set
		{
			_polymorphObject = value;
		}
	}

	public string PolymorphEffect
	{
		get
		{
			return _polymorphEffect;
		}
	}

	public string RevertEffect
	{
		get
		{
			return _revertEffect;
		}
		set
		{
			_revertEffect = value;
		}
	}

	public bool RemotePolymorphEnabled
	{
		get
		{
			return _remotePolymorphEnabled;
		}
		set
		{
			_remotePolymorphEnabled = value;
		}
	}

	public void Initialize(CharacterGlobals characterToPolymorph, string polymorphEffect, string revertEffect)
	{
		_characterToPolymorph = characterToPolymorph;
		_polymorph = null;
		_polymorphObject = null;
		_polymorphEffect = polymorphEffect;
		_revertEffect = revertEffect;
		_remotePolymorphEnabled = false;
	}

	public abstract Type GetPolymorphState();

	public abstract Type GetRevertState();

	public virtual GameObject CreateEffect(string effect, GameObject owningObject)
	{
		return Original.combatController.createEffect(effect, owningObject);
	}

	public NetworkComponent GetPolymorphNetworkComponent()
	{
		if (Polymorph != null && Polymorph.networkComponent != null)
		{
			return Polymorph.networkComponent;
		}
		if (_polymorphNetCmp == null && PolymorphObject != null)
		{
			_polymorphNetCmp = PolymorphObject.GetComponent<NetworkComponent>();
		}
		return _polymorphNetCmp;
	}
}
