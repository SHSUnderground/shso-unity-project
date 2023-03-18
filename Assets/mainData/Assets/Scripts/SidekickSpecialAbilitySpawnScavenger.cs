public class SidekickSpecialAbilitySpawnScavenger : SidekickSpecialAbilitySpawn
{
	public SidekickSpecialAbilitySpawnScavenger(PetUpgradeXMLDefinitionSpawnScavenger def)
		: base(def)
	{
		objectType = "scavenger";
		icon = "shopping_bundle|create_scavenger";
	}
}
