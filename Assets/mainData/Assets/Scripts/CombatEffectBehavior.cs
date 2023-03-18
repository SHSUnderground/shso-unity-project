public class CombatEffectBehavior : CombatEffectBase
{
	public override void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		if (combatEffectData.startBehavior != string.Empty)
		{
			changeBehavior(combatEffectData.startBehavior);
		}
	}

	protected override void ReleaseEffect()
	{
		if (combatEffectData.endBehavior != string.Empty)
		{
			BehaviorBase behaviorBase = changeBehavior(combatEffectData.endBehavior);
			BehaviorAttackBase behaviorAttackBase = behaviorBase as BehaviorAttackBase;
			if (behaviorAttackBase != null)
			{
				behaviorAttackBase.suppressBroadcast = true;
			}
			if (combatEffectData.getupAttack != string.Empty && charGlobals.behaviorManager.currentBehaviorName == "BehaviorGenericGetup")
			{
				BehaviorGenericGetup behaviorGenericGetup = charGlobals.behaviorManager.getBehavior() as BehaviorGenericGetup;
				behaviorGenericGetup.chainToAttack = combatEffectData.getupAttack;
			}
		}
		base.ReleaseEffect();
	}
}
