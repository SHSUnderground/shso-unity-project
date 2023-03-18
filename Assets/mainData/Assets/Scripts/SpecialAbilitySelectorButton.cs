using UnityEngine;

public class SpecialAbilitySelectorButton : GUISimpleControlWindow
{
	public int _abilityID = -1;

	private GUIDropShadowTextLabel _usesText;

	private SocialSpecialAbilityHUD _parentHUD;

	private GUIHotSpotButton _clicker;

	private GUIImageWithEvents _icon;

	private bool needsUpdate;

	public SpecialAbilitySelectorButton()
	{
		_clicker = GUIControl.CreateControlTopLeftFrame<GUIHotSpotButton>(new Vector2(48f, 48f), new Vector2(0f, 0f));
		_clicker.HitTestType = HitTestTypeEnum.Rect;
		_clicker.BlockTestType = BlockTestTypeEnum.Rect;
		_clicker.HitTestSize = new Vector2(1f, 1f);
		_clicker.IsVisible = false;
		Add(_clicker);
		_clicker.IsVisible = true;
		GUIImage gUIImage = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(48f, 48f), new Vector2(0f, 0f));
		gUIImage.TextureSource = "brawlergadget_bundle|brawler_gadget_small_power_selection";
		gUIImage.IsVisible = true;
		Add(gUIImage);
		_icon = GUIControl.CreateControlTopLeftFrame<GUIImageWithEvents>(new Vector2(48f, 48f), new Vector2(0f, 0f));
		_icon.Traits.HitTestType = HitTestTypeEnum.Circular;
		_icon.Traits.BlockTestType = BlockTestTypeEnum.Circular;
		_icon.IsVisible = true;
		Add(_icon);
		_usesText = GUIControl.CreateControlTopLeftFrame<GUIDropShadowTextLabel>(new Vector2(48f, 24f), new Vector2(-1f, 32f));
		_usesText.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, Color.white, TextAnchor.UpperRight);
		_usesText.FrontColor = GUILabel.GenColor(255, 255, 255);
		_usesText.BackColor = GUILabel.GenColor(0, 0, 0);
		_usesText.IsVisible = false;
		Add(_usesText);
	}

	public void setAbility(SpecialAbility ability, SocialSpecialAbilityHUD parentHUD)
	{
		_parentHUD = parentHUD;
		_abilityID = ability.specialAbilityID;
		_icon.ToolTip = new NamedToolTipInfo(ability.name, new Vector2(-160f, 40f));
		Vector2 position = new Vector2(0f, 0f);
		if (ability.iconSize != Vector2.zero)
		{
			position = (_icon.Size - ability.iconSize) * 0.5f;
			_icon.Size = ability.iconSize;
		}
		_icon.TextureSource = ability.icon;
		_icon.SetPosition(position);
		if (ability.uses != SpecialAbility.PASSIVE_USES)
		{
			_clicker.Click += iconClicked;
			_icon.Click += iconClicked;
			_usesText.Text = "x" + ability.usesLeft;
			_usesText.IsVisible = true;
		}
		if (ability is SidekickSpecialAbilityCooldown)
		{
			needsUpdate = true;
		}
	}

	private void iconClicked(GUIControl sender, GUIClickEvent eventArgs)
	{
		_parentHUD.iconClicked(_abilityID);
	}

	public void update()
	{
		if (needsUpdate)
		{
			foreach (SpecialAbility socialAbility in AppShell.Instance.Profile.socialAbilities)
			{
				if (socialAbility.specialAbilityID == _abilityID)
				{
					socialAbility.update();
					if (socialAbility is SidekickSpecialAbilityCooldown)
					{
						SidekickSpecialAbilityCooldown sidekickSpecialAbilityCooldown = socialAbility as SidekickSpecialAbilityCooldown;
						if (sidekickSpecialAbilityCooldown.cooldownRemaining() <= 0f)
						{
							_usesText.Text = "x1";
						}
						else
						{
							string empty = string.Empty;
							int num = (int)sidekickSpecialAbilityCooldown.cooldownRemaining();
							if (num < 60)
							{
								empty += "0:";
							}
							else
							{
								string text = empty;
								empty = text + string.Empty + num / 60 + ":";
							}
							if (num % 60 < 10)
							{
								empty += "0";
							}
							empty = empty + string.Empty + num % 60;
							_usesText.Text = empty;
						}
					}
					break;
				}
			}
		}
	}
}
