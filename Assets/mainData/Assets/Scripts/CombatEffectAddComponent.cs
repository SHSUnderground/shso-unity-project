using UnityEngine;

public class CombatEffectAddComponent : CombatEffectBase
{
	private Component addedComponent;

	public override void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		if (charGlobals.GetComponent(newCombatEffectData.addComponent) == null)
		{
			addedComponent = charGlobals.gameObject.AddComponent(newCombatEffectData.addComponent);
		}
	}

	protected override void ReleaseEffect()
	{
		base.ReleaseEffect();
		if (addedComponent != null)
		{
			Object.Destroy(addedComponent);
		}
	}
}
