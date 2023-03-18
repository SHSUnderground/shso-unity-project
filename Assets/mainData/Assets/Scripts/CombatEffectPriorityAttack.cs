public class CombatEffectPriorityAttack : CombatEffectBase
{
	public override void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		if (!string.IsNullOrEmpty(combatEffectData.prioritizedAttackName))
		{
			SetPrioritizedAttack(combatEffectData.prioritizedAttackName);
		}
	}

	protected override void ReleaseEffect()
	{
		base.ReleaseEffect();
		SetPrioritizedAttack(null);
	}

	protected void SetPrioritizedAttack(string attackName)
	{
		AICombatController aICombatController = charGlobals.combatController as AICombatController;
		if (aICombatController != null)
		{
			CombatController.AttackData attackData2 = aICombatController.PrioritizedAttack = ((!string.IsNullOrEmpty(attackName)) ? AttackDataManager.Instance.getAttackData(attackName) : null);
		}
	}
}
