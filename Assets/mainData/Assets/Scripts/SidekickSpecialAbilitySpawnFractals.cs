public class SidekickSpecialAbilitySpawnFractals : SidekickSpecialAbilitySpawn
{
	public SidekickSpecialAbilitySpawnFractals(PetUpgradeXMLDefinitionSpawnFractals def)
		: base(def)
	{
		objectType = "fractals";
		icon = "shopping_bundle|create_fractals";
	}
}
