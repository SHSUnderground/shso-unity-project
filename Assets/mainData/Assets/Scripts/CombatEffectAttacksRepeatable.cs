public class CombatEffectAttacksRepeatable : CombatEffectBase
{
	private bool originalAttacksRepeatable;

	public override void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		originalAttacksRepeatable = GetAttacksRepeatable();
		SetAttacksRepeatable(combatEffectData.attacksRepeatable == true);
	}

	protected override void ReleaseEffect()
	{
		base.ReleaseEffect();
		SetAttacksRepeatable(originalAttacksRepeatable);
	}

	protected void SetAttacksRepeatable(bool attacksRepeatable)
	{
		AICombatController aICombatController = charGlobals.combatController as AICombatController;
		if (aICombatController != null)
		{
			aICombatController.AttacksRepeatable = attacksRepeatable;
		}
	}

	protected bool GetAttacksRepeatable()
	{
		AICombatController aICombatController = charGlobals.combatController as AICombatController;
		return aICombatController != null && aICombatController.AttacksRepeatable;
	}
}
