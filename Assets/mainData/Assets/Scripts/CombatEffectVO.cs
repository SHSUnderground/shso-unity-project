using UnityEngine;

public class CombatEffectVO : CombatEffectBase
{
	private SuppressVO addedSuppressVO;

	public override void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		if (newCombatEffectData.suppressVO)
		{
			addedSuppressVO = charGlobals.gameObject.AddComponent<SuppressVO>();
		}
	}

	protected override void ReleaseEffect()
	{
		base.ReleaseEffect();
		if (addedSuppressVO != null)
		{
			Object.Destroy(addedSuppressVO);
		}
	}
}
