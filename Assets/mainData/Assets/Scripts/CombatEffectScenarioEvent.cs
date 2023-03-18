public class CombatEffectScenarioEvent : CombatEffectBase
{
	public override void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		if (combatEffectData.startScenarioEvent != string.Empty)
		{
			ScenarioEventManager.Instance.FireScenarioEvent(combatEffectData.startScenarioEvent, true);
		}
	}

	protected override void ReleaseEffect()
	{
		if (combatEffectData.endScenarioEvent != string.Empty)
		{
			ScenarioEventManager.Instance.FireScenarioEvent(combatEffectData.endScenarioEvent, true);
		}
		base.ReleaseEffect();
	}
}
