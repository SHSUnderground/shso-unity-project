using UnityEngine;

public class EntityFactionChangeMessage : ShsEventMessage
{
	public GameObject go;

	public CombatController.Faction oldFaction;

	public CombatController.Faction newFaction;

	public EntityFactionChangeMessage(GameObject go, CombatController.Faction oldFaction, CombatController.Faction newFaction)
	{
		this.go = go;
		this.oldFaction = oldFaction;
		this.newFaction = newFaction;
	}
}
