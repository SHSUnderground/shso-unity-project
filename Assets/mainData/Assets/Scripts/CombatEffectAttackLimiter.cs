using UnityEngine;

public class CombatEffectAttackLimiter : CombatEffectBase
{
	protected CombatController combatController;

	public override void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		GameObject gameObject = base.transform.root.gameObject;
		combatController = gameObject.GetComponent<CombatController>();
		if (combatEffectData.maxRegularAttackChain >= 0)
		{
			combatController.SetMaxRegularAttackChain(combatEffectData.maxRegularAttackChain);
		}
		if (combatEffectData.attackLimiter == 0)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			if ((combatEffectData.attackLimiter & (1 << i)) != 0)
			{
				combatController.LockSecondaryAttack(i);
			}
		}
		if ((combatEffectData.attackLimiter & 8) != 0)
		{
			combatController.SetCombatState(CombatController.CombatState.AttacksRestricted);
		}
		if ((combatEffectData.attackLimiter & 0x10) != 0)
		{
			combatController.SetCombatState(CombatController.CombatState.InteractsRestricted);
		}
		if ((combatEffectData.attackLimiter & 0x20) != 0)
		{
			combatController.SetCombatState(CombatController.CombatState.JumpRestricted);
		}
	}

	protected override void ReleaseEffect()
	{
		base.ReleaseEffect();
		if (combatEffectData.maxRegularAttackChain >= 0)
		{
			combatController.RemoveMaxRegularAttackChain(combatEffectData.maxRegularAttackChain);
		}
		if (combatEffectData.attackLimiter == 0)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			if ((combatEffectData.attackLimiter & (1 << i)) != 0)
			{
				combatController.RemoveSecondaryAttackLock(i);
			}
		}
		if ((combatEffectData.attackLimiter & 8) != 0)
		{
			combatController.ClearCombatState(CombatController.CombatState.AttacksRestricted);
		}
		if ((combatEffectData.attackLimiter & 0x10) != 0)
		{
			combatController.ClearCombatState(CombatController.CombatState.InteractsRestricted);
		}
		if ((combatEffectData.attackLimiter & 0x20) != 0)
		{
			combatController.ClearCombatState(CombatController.CombatState.JumpRestricted);
		}
	}
}
