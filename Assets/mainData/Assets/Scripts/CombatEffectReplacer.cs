public class CombatEffectReplacer : CombatEffectBase
{
	private new void OnRemove(bool doRemoveEffect)
	{
		if (doRemoveEffect)
		{
			CombatController combatController = base.transform.root.GetComponent(typeof(CombatController)) as CombatController;
			if (combatController == null)
			{
				CspUtils.DebugLog("Unable to find CombatController in CombatEffectReplacer");
			}
			else
			{
				combatController.createCombatEffect(combatEffectData.replacementCombatEffect, combatController, true);
			}
		}
	}
}
