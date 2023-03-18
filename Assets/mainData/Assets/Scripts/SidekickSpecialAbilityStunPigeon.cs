using UnityEngine;

public class SidekickSpecialAbilityStunPigeon : SidekickSpecialAbility
{
	public int radius;

	public SidekickSpecialAbilityStunPigeon(PetUpgradeXMLDefinitionStunPigeon def)
		: base(def)
	{
		radius = def.radius;
		icon = "shopping_bundle|knock_pigeons";
	}

	public override void attachToPetObject(GameObject petObject)
	{
		PetStunPigeonEmitter petStunPigeonEmitter = petObject.AddComponent<PetStunPigeonEmitter>();
		petStunPigeonEmitter.radius = radius;
	}
}
