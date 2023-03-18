using UnityEngine;

public class CombatEffectClickboxSize : CombatEffectBase
{
	private GameObject clickBox;

	private bool created;

	private float oldHeight;

	private float oldRadius;

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
		if (clickBox == null)
		{
			AttachClickBox();
			created = true;
		}
		SetClickboxSize(newCombatEffectData.clickboxHeight, newCombatEffectData.clickboxRadius);
	}

	protected override void ReleaseEffect()
	{
		if (created)
		{
			Object.Destroy(clickBox);
		}
		else
		{
			SetClickboxSize(oldHeight, oldRadius);
		}
		base.ReleaseEffect();
	}

	private void AttachClickBox()
	{
		GameObject original = Resources.Load("Character/ClickBoxCapsule") as GameObject;
		clickBox = (Object.Instantiate(original) as GameObject);
		clickBox.name = "ClickBox";
		Utils.AttachGameObject(charGlobals.gameObject, clickBox);
	}

	private void SetClickboxSize(float height, float radius)
	{
		if (radius == height / 2f)
		{
			height += 0.01f;
		}
		CapsuleCollider component = clickBox.GetComponent<CapsuleCollider>();
		oldHeight = component.height;
		oldRadius = component.radius;
		component.height = height;
		component.radius = radius;
	}
}
