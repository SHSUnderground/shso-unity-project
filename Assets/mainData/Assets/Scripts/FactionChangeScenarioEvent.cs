using UnityEngine;

[AddComponentMenu("ScenarioEvent/FactionChange")]
public class FactionChangeScenarioEvent : ScenarioEventHandlerEnableBase
{
	public CombatController.Faction newFaction = CombatController.Faction.Enemy;

	protected override void OnEnableEvent(string eventName)
	{
		CombatController combatController = GetComponent(typeof(CombatController)) as CombatController;
		if (combatController == null)
		{
			CspUtils.DebugLog("Combat controller not found in OnEnableEvent of CombatEnableScenarioEvent");
		}
		else
		{
			combatController.faction = newFaction;
		}
	}
}
