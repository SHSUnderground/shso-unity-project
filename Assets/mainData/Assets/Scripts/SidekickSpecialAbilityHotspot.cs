public class SidekickSpecialAbilityHotspot : SidekickSpecialAbility
{
	public string hotSpotType = string.Empty;

	public SidekickSpecialAbilityHotspot(PetUpgradeXMLDefinitionHotspot def)
		: base(def)
	{
		hotSpotType = def.hotSpotType;
		switch (hotSpotType)
		{
		case "flying":
			icon = "shopping_bundle|flight1";
			break;
		case "teleport":
			icon = "shopping_bundle|teleport";
			break;
		case "GroundSpeed":
			icon = "shopping_bundle|social_racetrack";
			break;
		}
	}
}
