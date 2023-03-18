using UnityEngine;

public class SidekickSpecialAbilitySmartBomb : SidekickSpecialAbility
{
	public string characterName = string.Empty;

	public string attackName = string.Empty;

	public string deathAnimOverride = string.Empty;

	public SidekickSpecialAbilitySmartBomb(PetUpgradeXMLDefinitionSmartbomb def)
		: base(def)
	{
		characterName = def.character;
		attackName = def.attackName;
		deathAnimOverride = def.deathAnimOverride;
	}

	public override bool sameAs(SpecialAbility ab)
	{
		if (!base.sameAs(ab))
		{
			return false;
		}
		SidekickSpecialAbilitySmartBomb sidekickSpecialAbilitySmartBomb = ab as SidekickSpecialAbilitySmartBomb;
		if (sidekickSpecialAbilitySmartBomb == null)
		{
			return false;
		}
		if (characterName != sidekickSpecialAbilitySmartBomb.characterName || attackName != sidekickSpecialAbilitySmartBomb.attackName)
		{
			return false;
		}
		return true;
	}

	public override void execute()
	{
		if (BrawlerController.Instance == null || BrawlerController.Instance.LocalPlayer == null)
		{
			CspUtils.DebugLog("cannot execute SidekickSmartBombSpecialAbility - not in a brawler");
			return;
		}
		GameObject localPlayer = BrawlerController.Instance.LocalPlayer;
		CharacterSpawn.SpawnAlly(characterName, localPlayer.transform.localPosition, CombatController.Faction.Player, 0, 30, true, attackName, deathAnimOverride);
	}

	public override string ToString()
	{
		return "SidekickSmartBombSpecialAbility " + characterName + " " + attackName;
	}
}
