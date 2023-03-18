using System;
using UnityEngine;

public class SHSHeroUnlockedWindow : GUIDynamicWindow
{
	protected GUIImage bgImage;

	protected GUIStrokeTextLabel _titleText;

	protected GUIImage heroImage;

	protected GUIButton okButton;

	protected GUIButton switchHeroButton;

	protected GUIStrokeTextLabel _text;

	protected bool _topScore;

	protected int heroID;

	public SHSHeroUnlockedWindow(int heroID)
	{
		this.heroID = heroID;
	}

	public override bool InitializeResources(bool reload)
	{
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Expected O, but got Unknown
		SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero, new Vector2(593f, 366f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		bgImage = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(593f, 356f), new Vector2(0f, 0f));
		bgImage.TextureSource = "common_bundle|world_event_congratulations_box";
		Add(bgImage);
		OwnableDefinition def = OwnableDefinition.getDef(heroID);
		heroImage = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(153f, 163f), new Vector2(15f, 125f));
		heroImage.TextureSource = "characters_bundle|expandedtooltip_render_" + def.name;
		heroImage.IsVisible = false;
		Add(heroImage);
		_titleText = new GUIStrokeTextLabel();
		_titleText.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 28, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(189, 44, 57), GUILabel.GenColor(23, 23, 23), new Vector2(1f, 3f), TextAnchor.MiddleCenter);
		_titleText.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(80f, 30f), new Vector2(465f, 80f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_titleText.VerticalKerning = 20;
		_titleText.Text = "New Hero Unlocked!";
		_titleText.VerticalKerning = 20;
		_titleText.IsVisible = false;
		Add(_titleText);
		okButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(38f, 146f));
		okButton.Click += delegate
		{
			onClick();
		};
		okButton.HitTestSize = new Vector2(0.5f, 0.5f);
		okButton.HitTestType = HitTestTypeEnum.Circular;
		okButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|L_mshs_button_ok");
		okButton.IsVisible = false;
		Add(okButton);
		switchHeroButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(70f, 70f), new Vector2(-38f, 146f));
		switchHeroButton.Click += delegate
		{
			SwitchHero();
		};
		switchHeroButton.HitTestSize = new Vector2(0.5f, 0.5f);
		switchHeroButton.HitTestType = HitTestTypeEnum.Circular;
		switchHeroButton.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|mshs_gameworld_HUD_changehero");
		switchHeroButton.IsVisible = false;
		Add(switchHeroButton);
		_text = new GUIStrokeTextLabel();
		_text.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 22, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(23, 23, 23), GUILabel.GenColor(23, 23, 23), new Vector2(1f, 3f), TextAnchor.MiddleLeft);
		_text.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(190f, 100f), new Vector2(330f, 160f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_text.VerticalKerning = 24;
		_text.Text = AppShell.Instance.CharacterDescriptionManager[def.name].LongDescription;
		_text.VerticalKerning = 24;
		_text.IsVisible = false;
		Add(_text);
		AnimClip animClip = AnimClipBuilder.Absolute.SizeX(AnimClipBuilder.Path.Linear(0f, 593f, 1f), bgImage) ^ AnimClipBuilder.Absolute.SizeY(AnimClipBuilder.Path.Linear(0f, 356f, 1f), bgImage) ^ AnimClipBuilder.Absolute.OffsetX(AnimClipBuilder.Path.Linear(190f, 0f, 1f), this);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			heroImage.IsVisible = true;
			_titleText.IsVisible = true;
			okButton.IsVisible = true;
			switchHeroButton.IsVisible = true;
			_text.IsVisible = true;
		};
		base.AnimationPieceManager.Add(animClip);
		return base.InitializeResources(reload);
	}

	protected void onClick()
	{
		Hide();
	}

	protected void SwitchHero()
	{
		MyHeroesWindow.forceSelectedID = heroID;
		SocialSpaceController.Instance.Controller.ChangeCharacters();
		Hide();
	}

	protected override void dispose(bool disposing)
	{
		base.dispose(disposing);
		bgImage = null;
		heroImage = null;
	}
}
