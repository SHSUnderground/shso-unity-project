using System;
using UnityEngine;

public class PolymorphCharacterStateData : PolymorphStateData
{
	private string _form;

	private string _combatEffect;

	private bool _polymorphIsCharacter;

	private GameObject _combatTarget;

	public string Form
	{
		get
		{
			return _form;
		}
	}

	public string CombatEffect
	{
		get
		{
			return _combatEffect;
		}
	}

	public bool PolymorphIsCharacter
	{
		get
		{
			return _polymorphIsCharacter;
		}
	}

	public GameObject CombatTarget
	{
		get
		{
			return _combatTarget;
		}
	}

	public void Initialize(CharacterGlobals characterToPolymorph, string polymorphEffect, string revertEffect, string form, bool polymorphIsCharacter, string combatEffect, GameObject combatTarget)
	{
		Initialize(characterToPolymorph, polymorphEffect, revertEffect);
		_form = form;
		_combatEffect = combatEffect;
		_polymorphIsCharacter = polymorphIsCharacter;
		_combatTarget = combatTarget;
	}

	public override Type GetPolymorphState()
	{
		if (base.Original.networkComponent.IsOwner())
		{
			return typeof(PolymorphSpawnState);
		}
		return typeof(PolymorphRemoteState);
	}

	public override Type GetRevertState()
	{
		NetworkComponent polymorphNetworkComponent = GetPolymorphNetworkComponent();
		if (polymorphNetworkComponent == null || polymorphNetworkComponent.IsOwner())
		{
			return typeof(RevertCharacterState);
		}
		return typeof(RevertRemoteState);
	}

	public override GameObject CreateEffect(string effect, GameObject owningObject)
	{
		GameObject gameObject = base.CreateEffect(effect, owningObject);
		if (gameObject == null && CombatTarget != null)
		{
			return base.Original.combatController.createEffect(effect, owningObject, CombatTarget.GetComponent<EffectSequenceList>());
		}
		return gameObject;
	}
}
