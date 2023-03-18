public class SidekickSpecialAbilitySpawnXP : SidekickSpecialAbilitySpawn
{
	public SidekickSpecialAbilitySpawnXP(PetUpgradeXMLDefinitionSpawnXP def)
		: base(def)
	{
		objectType = "xp";
		icon = "shopping_bundle|create_xp";
	}
}
