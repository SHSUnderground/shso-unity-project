using UnityEngine;

public class SidekickSpecialAbilityPokeStar : SidekickSpecialAbility
{
	public int radius;

	public SidekickSpecialAbilityPokeStar(PetUpgradeXMLDefinitionPokeStar def)
		: base(def)
	{
		radius = def.radius;
		icon = "shopping_bundle|poke_star";
	}

	public override void attachToPetObject(GameObject petObject)
	{
		PetPokeStarEmitter petPokeStarEmitter = petObject.AddComponent<PetPokeStarEmitter>();
		petPokeStarEmitter.radius = radius;
	}
}
