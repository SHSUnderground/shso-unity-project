using UnityEngine;

public class CombatEffectAttachedProjectile : CombatEffectBase
{
	protected ImpactProjectile[] activeProjectiles;

	protected string projectileAttackName;

	protected int impactCount;

	protected CombatController sourceCombatController;

	protected CombatController combatController;

	public new void Initialize(CombatEffectData newCombatEffectData, CombatController SourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		GameObject gameObject = base.transform.root.gameObject;
		combatController = (gameObject.GetComponent(typeof(CombatController)) as CombatController);
		projectileAttackName = newCombatEffectData.attachedProjectile;
		impactCount = newCombatEffectData.attachedProjectileCount;
		activeProjectiles = new ImpactProjectile[impactCount];
		sourceCombatController = SourceCombatController;
		if (combatController != null && sourceCombatController != null)
		{
			for (int i = 0; i < impactCount; i++)
			{
				CombatController.AttackData attackData = AttackDataManager.Instance.getAttackData(projectileAttackName);
				activeProjectiles[i] = (CombatController.ImpactData.CreateImpact(attackData, i, sourceCombatController.CharGlobals, combatController) as ImpactProjectile);
				GameObject gameObject2 = base.gameObject;
				activeProjectiles[i].CreateProjectile(sourceCombatController.CharGlobals, gameObject2, gameObject2);
				activeProjectiles[i].LaunchProjectile(null);
			}
		}
	}

	private new void OnRemove(bool doRemoveEffect)
	{
		if (activeProjectiles != null)
		{
			ImpactProjectile[] array = activeProjectiles;
			foreach (ImpactProjectile impactProjectile in array)
			{
				impactProjectile.ImpactEnd();
			}
			activeProjectiles = null;
		}
	}
}
