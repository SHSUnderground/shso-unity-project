using UnityEngine;

public class SidekickSpecialAbilityBrawlerPassiveBuff : SidekickSpecialAbilityBrawler
{
	public string buffName;

	public SidekickSpecialAbilityBrawlerPassiveBuff(PetUpgradeXMLDefinitionBrawlerPassiveBuff def)
		: base(def)
	{
		buffName = def.buff;
		switch (buffName)
		{
		case "SidekickPassiveDamageIncrease":
			icon = "shopping_bundle|buff_damage_passive";
			break;
		case "SidekickPassiveSpeedIncrease":
			icon = "shopping_bundle|buff_speed_passive";
			break;
		case "SidekickPassiveArmorIncrease":
			icon = "shopping_bundle|buff_armor_passive";
			break;
		}
		iconSize = new Vector2(32f, 32f);
	}

	public override void beginBrawler(PlayerCombatController playerCC)
	{
		playerCC.createCombatEffect(buffName, playerCC, true);
	}
}
