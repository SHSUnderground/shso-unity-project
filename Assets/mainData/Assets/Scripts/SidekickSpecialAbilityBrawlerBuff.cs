using UnityEngine;

public class SidekickSpecialAbilityBrawlerBuff : SidekickSpecialAbilityBrawler
{
	public string buffName;

	public SidekickSpecialAbilityBrawlerBuff(PetUpgradeXMLDefinitionBrawlerBuff def)
		: base(def)
	{
		buffName = def.buff;
		switch (buffName)
		{
		case "DamageIncreasePickupBuff":
			icon = "shopping_bundle|buff_damage";
			break;
		case "SuperArmorPickupBuff":
			icon = "shopping_bundle|buff_armor";
			break;
		case "MoveSpeedPickupBuff":
			icon = "shopping_bundle|buff_speed";
			break;
		case "SuperGainPickupBuff":
			icon = "shopping_bundle|buff_energy";
			break;
		case "HealthRegenPickupBuff":
			icon = "shopping_bundle|buff_regen";
			break;
		}
		iconSize = new Vector2(32f, 32f);
	}

	public override bool sameAs(SpecialAbility ab)
	{
		if (!base.sameAs(ab))
		{
			return false;
		}
		SidekickSpecialAbilityBrawlerBuff sidekickSpecialAbilityBrawlerBuff = ab as SidekickSpecialAbilityBrawlerBuff;
		if (sidekickSpecialAbilityBrawlerBuff == null)
		{
			return false;
		}
		if (buffName != sidekickSpecialAbilityBrawlerBuff.buffName)
		{
			return false;
		}
		return true;
	}

	public override void execute()
	{
		if (BrawlerController.Instance == null || BrawlerController.Instance.LocalPlayer == null)
		{
			CspUtils.DebugLog("cannot execute SidekickSpecialAbilityBrawlerBuff - not in a brawler");
			return;
		}
		GameObject localPlayer = BrawlerController.Instance.LocalPlayer;
		PlayerCombatController component = localPlayer.GetComponent<PlayerCombatController>();
		component.createCombatEffect(buffName, component, true);
	}
}
