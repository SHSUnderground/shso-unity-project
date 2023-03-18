public class CombatEffectAblative : CombatEffectBase
{
	protected float healthRemaining;

	protected float healthMax;

	public override void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		healthRemaining = combatEffectData.ablativeHealth;
		healthMax = healthRemaining;
	}

	public void OnDamage(float damage)
	{
		if (!(healthRemaining > 0f))
		{
			return;
		}
		healthRemaining -= damage;
		if (!(healthRemaining <= 0f))
		{
			return;
		}
		if (combatEffectData.ablativeEndBehavior != string.Empty)
		{
			CspUtils.DebugLog("ablative end behavior : " + combatEffectData.ablativeEndBehavior);
			changeBehavior(combatEffectData.ablativeEndBehavior);
		}
		if (combatEffectData.ablativeReplacementCombatEffect != string.Empty)
		{
			CombatController combatController = base.transform.root.GetComponent(typeof(CombatController)) as CombatController;
			if (combatController == null)
			{
				CspUtils.DebugLog("Unable to find CombatController in CombatEffectReplacer");
			}
			else
			{
				combatController.createCombatEffect(combatEffectData.ablativeReplacementCombatEffect, combatController, true);
			}
		}
		removeCombatEffect(true);
	}
}
