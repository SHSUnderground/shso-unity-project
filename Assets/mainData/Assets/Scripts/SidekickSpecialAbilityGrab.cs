public class SidekickSpecialAbilityGrab : SidekickSpecialAbility
{
	public string activityType = string.Empty;

	public int radius = 10;

	public SidekickSpecialAbilityGrab(PetUpgradeXMLDefinitionGrab def)
		: base(def)
	{
		activityType = def.activityName;
		radius = def.radius;
		switch (activityType)
		{
		case "feather":
			icon = "shopping_bundle|attract_tokens";
			break;
		case "fractal":
			icon = "shopping_bundle|attract_fractals";
			break;
		case "scavenger":
			icon = "shopping_bundle|attract_scavenger";
			break;
		}
	}
}
