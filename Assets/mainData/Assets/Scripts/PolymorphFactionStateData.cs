using System;

public class PolymorphFactionStateData : PolymorphStateData
{
	private CombatController.Faction _newFaction;

	private CombatController.Faction _oldFaction;

	private bool _canFactionDamage;

	private bool _canCharmBreak;

	private CombatController _charmer;

	public CombatController.Faction NewFaction
	{
		get
		{
			return _newFaction;
		}
	}

	public CombatController.Faction OldFaction
	{
		get
		{
			return _oldFaction;
		}
	}

	public bool CanFactionDamage
	{
		get
		{
			return _canFactionDamage;
		}
	}

	public bool CanCharmBreak
	{
		get
		{
			return _canCharmBreak;
		}
	}

	public CombatController Charmer
	{
		get
		{
			return _charmer;
		}
	}

	public void Initialize(CharacterGlobals characterToPolymorph, CombatController.Faction newFaction, bool canFactionDamage, bool canCharmBreak, CombatController charmer)
	{
		Initialize(characterToPolymorph, string.Empty, string.Empty);
		_newFaction = newFaction;
		_oldFaction = characterToPolymorph.combatController.faction;
		_canFactionDamage = canFactionDamage;
		_canCharmBreak = canCharmBreak;
		_charmer = charmer;
	}

	public override Type GetPolymorphState()
	{
		return typeof(PolymorphFactionState);
	}

	public override Type GetRevertState()
	{
		return typeof(PolymorphNullState);
	}
}
