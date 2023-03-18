using System;
using UnityEngine;

public class SidekickSpecialAbilityAlly : SidekickSpecialAbility
{
	public string characterName = string.Empty;

	public int rAttack = 1;

	public int duration = 30;

	public int numberToSpawn = 1;

	public string deathAnimOverride = string.Empty;

	public SidekickSpecialAbilityAlly(PetUpgradeXMLDefinitionAlly def)
		: base(def)
	{
		characterName = def.character;
		rAttack = def.r2;
		duration = def.duration;
		numberToSpawn = def.numberToSpawn;
		deathAnimOverride = def.deathAnimOverride;
	}

	public override bool sameAs(SpecialAbility ab)
	{
		if (!base.sameAs(ab))
		{
			return false;
		}
		SidekickSpecialAbilityAlly sidekickSpecialAbilityAlly = ab as SidekickSpecialAbilityAlly;
		if (sidekickSpecialAbilityAlly == null)
		{
			return false;
		}
		if (characterName != sidekickSpecialAbilityAlly.characterName || rAttack != sidekickSpecialAbilityAlly.rAttack || duration != sidekickSpecialAbilityAlly.duration || numberToSpawn != sidekickSpecialAbilityAlly.numberToSpawn)
		{
			return false;
		}
		return true;
	}

	public override void execute()
	{
		CspUtils.DebugLog("execute SidekickSmartBombSpecialAbility");
		if (BrawlerController.Instance == null || BrawlerController.Instance.LocalPlayer == null)
		{
			CspUtils.DebugLog("cannot execute SidekickSmartBombSpecialAbility - not in a brawler");
			return;
		}
		GameObject localPlayer = BrawlerController.Instance.LocalPlayer;
		double num = Math.PI * 2.0 / (double)numberToSpawn;
		double num2 = 2.0;
		double num3 = numberToSpawn - 1;
		while (num3 >= 0.0)
		{
			double num4 = Math.Sin(num * num3) * num2;
			double num5 = Math.Cos(num * num3) * num2;
			num3 -= 1.0;
			CspUtils.DebugLog(new Vector3((float)num4, -2f, (float)num5).ToString());
			CharacterSpawn.SpawnAlly(characterName, localPlayer.transform.localPosition + new Vector3((float)num4, 0f, (float)num5), CombatController.Faction.Player, rAttack, duration, false, string.Empty, deathAnimOverride);
		}
	}

	public override string ToString()
	{
		return "SidekickAllySpecialAbility " + characterName + " " + rAttack + " " + duration + " ";
	}
}
