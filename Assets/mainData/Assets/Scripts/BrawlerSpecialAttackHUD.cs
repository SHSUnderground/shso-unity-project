using UnityEngine;

public class BrawlerSpecialAttackHUD : GUISimpleControlWindow
{
	private GUIImage _background;

	public BrawlerSpecialAttackHUD()
	{
		AppShell.Instance.EventMgr.AddListener<PlayerSpecialAttacksChanged>(OnSpecialAttacksChanged);
	}

	public void Init()
	{
		RemoveAllControls();
		int num = 0;
		int num2 = 48;
		int num3 = 0;
		foreach (SpecialAbility brawlerAbility in AppShell.Instance.Profile.brawlerAbilities)
		{
			GUIHotSpotButton gUIHotSpotButton = GUIControl.CreateControlTopLeftFrame<GUIHotSpotButton>(new Vector2(48f, 48f), new Vector2(num * num3, num2 * num3));
			gUIHotSpotButton.HitTestType = HitTestTypeEnum.Circular;
			gUIHotSpotButton.BlockTestType = BlockTestTypeEnum.Circular;
			gUIHotSpotButton.BlockTestSize = new Vector2(1f, 1f);
			gUIHotSpotButton.IsVisible = false;
			Add(gUIHotSpotButton);
			gUIHotSpotButton.IsVisible = true;
			GUIImage gUIImage = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(48f, 48f), new Vector2(num * num3, num2 * num3));
			gUIImage.TextureSource = "brawlergadget_bundle|brawler_gadget_small_power_selection";
			Add(gUIImage);
			CspUtils.DebugLog("adding ability " + brawlerAbility.ToString());
			Vector2 vector = new Vector2(48f, 48f);
			Vector2 vector2 = new Vector2(0f, 0f);
			if (brawlerAbility.iconSize != Vector2.zero)
			{
				vector = brawlerAbility.iconSize;
				vector2.x = (48f - vector.x) / 2f;
				vector2.y = (48f - vector.y) / 2f;
				CspUtils.DebugLog("icon size is " + vector + " offset is " + vector2);
			}
			SpecialAttackSelectorButton specialAttackSelectorButton = GUIControl.CreateControlTopLeftFrame<SpecialAttackSelectorButton>(vector, new Vector2(num * num3, num2 * num3) + vector2);
			specialAttackSelectorButton.TextureSource = brawlerAbility.icon;
			Add(specialAttackSelectorButton);
			specialAttackSelectorButton.attackID = brawlerAbility.specialAbilityID;
			specialAttackSelectorButton.IsVisible = true;
			specialAttackSelectorButton.ToolTip = new NamedToolTipInfo(brawlerAbility.name, new Vector2(0f, 20f));
			if (brawlerAbility.uses != SpecialAbility.PASSIVE_USES)
			{
				specialAttackSelectorButton.Click += iconClicked;
				GUIDropShadowTextLabel gUIDropShadowTextLabel = GUIControl.CreateControlTopLeftFrame<GUIDropShadowTextLabel>(new Vector2(48f, 24f), new Vector2(30 + num * num3, 33 + num2 * num3));
				gUIDropShadowTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, Color.white, TextAnchor.UpperLeft);
				gUIDropShadowTextLabel.FrontColor = GUILabel.GenColor(255, 255, 255);
				gUIDropShadowTextLabel.BackColor = GUILabel.GenColor(0, 0, 0);
				gUIDropShadowTextLabel.Text = "x" + brawlerAbility.usesLeft;
				Add(gUIDropShadowTextLabel);
			}
			num3++;
		}
	}

	private void OnSpecialAttacksChanged(PlayerSpecialAttacksChanged msg)
	{
		Init();
	}

	private void iconClicked(GUIControl sender, GUIClickEvent eventArgs)
	{
		int attackID = (sender as SpecialAttackSelectorButton).attackID;
		AppShell.Instance.Profile.useSpecialAbility(attackID);
	}
}
