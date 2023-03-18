using UnityEngine;

public class SidekickSpecialAbilityKillTroublebot : SidekickSpecialAbility
{
	public int radius;

	public SidekickSpecialAbilityKillTroublebot(PetUpgradeXMLDefinitionKillTroublebot def)
		: base(def)
	{
		radius = def.radius;
		icon = "shopping_bundle|kill_troublebots";
	}

	public override void attachToPetObject(GameObject petObject)
	{
		PetKillTroublebotEmitter petKillTroublebotEmitter = petObject.AddComponent<PetKillTroublebotEmitter>();
		petKillTroublebotEmitter.radius = radius;
	}
}
