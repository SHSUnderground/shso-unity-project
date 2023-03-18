using UnityEngine;

public class CombatEffectClickboxCenter : CombatEffectBase
{
	private CapsuleCollider clickBoxCollider;

	private Vector3 centerOffset;

	public override void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		GameObject gameObject = null;
		foreach (Transform item in charGlobals.transform)
		{
			if (item.name == "ClickBox")
			{
				gameObject = item.gameObject;
				break;
			}
		}
		if (!(gameObject != null))
		{
			return;
		}
		clickBoxCollider = gameObject.GetComponent<CapsuleCollider>();
		if (clickBoxCollider != null)
		{
			centerOffset = Vector3.zero;
			if (newCombatEffectData.clickboxCenter.x != float.MaxValue)
			{
				
				float x = newCombatEffectData.clickboxCenter.x;
				Vector3 center = clickBoxCollider.center;
				centerOffset.x = x - center.x;
			}
			if (newCombatEffectData.clickboxCenter.y != float.MaxValue)
			{
			
				float y = newCombatEffectData.clickboxCenter.y;
				Vector3 center2 = clickBoxCollider.center;
				centerOffset.y = y - center2.y;
			}
			if (newCombatEffectData.clickboxCenter.z != float.MaxValue)
			{
				
				float z = newCombatEffectData.clickboxCenter.z;
				Vector3 center3 = clickBoxCollider.center;
				centerOffset.z = z - center3.z;
			}
			clickBoxCollider.center += centerOffset;
		}
	}

	protected override void ReleaseEffect()
	{
		if (clickBoxCollider != null)
		{
			clickBoxCollider.center -= centerOffset;
		}
		base.ReleaseEffect();
	}
}
