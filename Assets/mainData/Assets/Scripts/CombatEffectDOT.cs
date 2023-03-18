using UnityEngine;

public class CombatEffectDOT : CombatEffectBase
{
	protected CombatController.AttackData attackData;

	protected CombatController.ImpactData activeAttack;

	protected string dotAttackName;

	protected CombatController sourceCombatController;

	protected CombatController combatController;

	private float dotPeriod = 1f;

	private float dotTimer;

	protected override void ReleaseEffect()
	{
		base.ReleaseEffect();
	}

	public new void Initialize(CombatEffectData newCombatEffectData, CombatController SourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		GameObject gameObject = base.transform.root.gameObject;
		combatController = (gameObject.GetComponent(typeof(CombatController)) as CombatController);
		dotAttackName = newCombatEffectData.dotAttack;
		sourceCombatController = SourceCombatController;
		dotPeriod = newCombatEffectData.dotPeriod;
		attackData = AttackDataManager.Instance.getAttackData(dotAttackName);
		if (attackData.impacts.Length > 0)
		{
			activeAttack = attackData.impacts[0];
		}
	}

	private void Update()
	{
		dotTimer += Time.deltaTime;
		if (!(dotTimer > dotPeriod))
		{
			return;
		}
		if (attackData != null && activeAttack != null)
		{
			float damage = activeAttack.impactResult.damageData.getValue(sourceCombatController) * combatController.incomingDamageMultiplier;
			if (combatController.canShowPopupDamage())
			{
				combatController.showPopupDamage(damage, true);
			}
			if (sourceCombatController == null)
			{
				combatController.hitByAttack(Vector3.zero, combatController, base.gameObject, damage, activeAttack.impactResult);
			}
			else
			{
				if (sourceCombatController.canShowPopupDamage())
				{
					combatController.showPopupDamage(damage, false);
				}
				combatController.hitByAttack(Vector3.zero, sourceCombatController, base.gameObject, damage, activeAttack.impactResult);
			}
		}
		dotTimer %= dotPeriod;
	}
}
