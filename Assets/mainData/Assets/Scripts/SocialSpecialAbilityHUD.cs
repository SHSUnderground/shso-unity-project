using System.Collections.Generic;
using UnityEngine;

public class SocialSpecialAbilityHUD : GUISimpleControlWindow
{
	private GUIImage _background;

	private List<SpecialAbilitySelectorButton> _icons = new List<SpecialAbilitySelectorButton>();

	public SocialSpecialAbilityHUD()
	{
		AppShell.Instance.EventMgr.AddListener<PlayerSpecialAttacksChanged>(OnSpecialAttacksChanged);
	}

	public void Init()
	{
		foreach (SpecialAbilitySelectorButton icon in _icons)
		{
			Remove(icon);
			icon.Dispose();
		}
		_icons.Clear();
		RemoveAllControls();
		int num = 0;
		if (AppShell.Instance.Profile != null)
		{
			foreach (SpecialAbility socialAbility in AppShell.Instance.Profile.socialAbilities)
			{
				SpecialAbilitySelectorButton specialAbilitySelectorButton = GUIControl.CreateControlTopLeftFrame<SpecialAbilitySelectorButton>(new Vector2(48f, 48f), new Vector2(48 * num, 5f));
				specialAbilitySelectorButton.setAbility(socialAbility, this);
				_icons.Add(specialAbilitySelectorButton);
				Add(specialAbilitySelectorButton);
				specialAbilitySelectorButton.IsVisible = true;
				num++;
			}
		}
	}

	private void OnSpecialAttacksChanged(PlayerSpecialAttacksChanged msg)
	{
		Init();
	}

	public void iconClicked(int abilityID)
	{
		CspUtils.DebugLog("SocialSpecialAbilityHUD iconClicked " + abilityID);
		AppShell.Instance.Profile.useSpecialAbility(abilityID);
	}

	public override void Update()
	{
		base.Update();
		foreach (SpecialAbilitySelectorButton icon in _icons)
		{
			icon.update();
		}
	}
}
