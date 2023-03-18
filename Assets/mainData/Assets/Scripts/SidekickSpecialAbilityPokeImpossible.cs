using UnityEngine;

public class SidekickSpecialAbilityPokeImpossible : SidekickSpecialAbility
{
	public int radius;

	public SidekickSpecialAbilityPokeImpossible(PetUpgradeXMLDefinitionPokeImpossible def)
		: base(def)
	{
		radius = def.radius;
		icon = "shopping_bundle|poke_impossible";
	}

	public override void attachToPetObject(GameObject petObject)
	{
		PetPokeImpossibleEmitter petPokeImpossibleEmitter = petObject.AddComponent<PetPokeImpossibleEmitter>();
		petPokeImpossibleEmitter.radius = radius;
	}
}
