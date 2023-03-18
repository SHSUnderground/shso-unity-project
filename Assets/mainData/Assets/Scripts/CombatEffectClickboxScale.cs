using UnityEngine;

public class CombatEffectClickboxScale : CombatEffectBase
{
	private GameObject clickBox;

	private float heightScale;

	private float radiusScale;

	public override void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		foreach (Transform item in charGlobals.transform)
		{
			if (item.name == "ClickBox")
			{
				clickBox = item.gameObject;
				break;
			}
		}
		if (clickBox != null)
		{
			heightScale = newCombatEffectData.clickboxHeightScale;
			radiusScale = newCombatEffectData.clickboxRadiusScale;
			SetClickboxSize(heightScale, radiusScale);
		}
	}

	protected override void ReleaseEffect()
	{
		if (clickBox != null)
		{
			SetClickboxSize(1f / heightScale, 1f / radiusScale);
		}
		base.ReleaseEffect();
	}

	private void SetClickboxSize(float heightScale, float radiusScale)
	{
		CapsuleCollider component = clickBox.GetComponent<CapsuleCollider>();
		component.height *= heightScale;
		component.radius *= radiusScale;
	}
}
