public class SidekickSpecialAbilitySpawnTickets : SidekickSpecialAbilitySpawn
{
	public SidekickSpecialAbilitySpawnTickets(PetUpgradeXMLDefinitionSpawnTickets def)
		: base(def)
	{
		objectType = "tickets";
		icon = "shopping_bundle|create_tickets";
	}
}
