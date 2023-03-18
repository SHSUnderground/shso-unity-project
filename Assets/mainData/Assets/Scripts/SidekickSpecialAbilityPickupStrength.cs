using UnityEngine;

public class SidekickSpecialAbilityPickupStrength : SidekickSpecialAbility
{
	public float strength = 5f;

	public SidekickSpecialAbilityPickupStrength(PetUpgradeXMLDefinitionPickupStrength def)
		: base(def)
	{
		strength = def.strength;
		icon = "shopping_bundle|social_strength";
	}

	public override void attachToPetObject(GameObject petObject)
	{
		CspUtils.DebugLog("SidekickSpecialAbilityPickupStrength attachToPet " + strength);
		CharacterGlobals component = petObject.GetComponent<CharacterGlobals>();
		component.motionController.pickupStrength = strength;
	}
}
