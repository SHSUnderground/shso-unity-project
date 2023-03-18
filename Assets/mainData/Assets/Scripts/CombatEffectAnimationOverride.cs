using System.Collections.Generic;

public class CombatEffectAnimationOverride : CombatEffectBase
{
	public override void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		foreach (KeyValuePair<string, string> animationOverride in newCombatEffectData.animationOverrides)
		{
			charGlobals.behaviorManager.OverrideAnimation(animationOverride.Key, animationOverride.Value);
		}
	}

	protected override void ReleaseEffect()
	{
		foreach (KeyValuePair<string, string> animationOverride in combatEffectData.animationOverrides)
		{
			charGlobals.behaviorManager.RemoveAnimationOverride(animationOverride.Key, animationOverride.Value);
		}
		base.ReleaseEffect();
	}
}
