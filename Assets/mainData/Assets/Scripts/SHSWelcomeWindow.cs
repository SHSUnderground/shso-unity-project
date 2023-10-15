using UnityEngine;

public class SHSWelcomeWindow : GUIDynamicWindow
{
	private GUIImage loadingBg;

	private GUIImage heroBgd;

	private GUIImage contentBgd;

	private GUIImage contentBubble;

	private GUIImage shsLogo;

	private GUIImage buttonInset;

	private GUIButton yesButton;

	private GUIButton noButton;

	private GUIButton becomeAgentButton;

	private GUILabel welcomeLabel;

	private GUILabel fullscreenLabel;

	private GUIImage tasLogo;

	private GUIImage gazLogo;

	private GUIImage marvelImage;

	private AudioManager.MuteContext mutingContext;

	public SHSWelcomeWindow()
	{
		SetPositionAndSize(QuickSizingHint.ScreenSize);
	}

	public override bool InitializeResources(bool reload)
	{
		if (!base.InitializeResources(reload))
		{
			return false;
		}
		loadingBg = new GUIImage();
		loadingBg.SetPositionAndSize(QuickSizingHint.ParentSize);
		loadingBg.TextureSource = "GUI/loading/welcome/mshs_welcome_screen_bg";
		loadingBg.Traits.RespectDisabledAlphaTrait = ControlTraits.RespectDisabledAlphaTraitEnum.DisrespectDisabledAlpha;
		loadingBg.Id = "loading background";
		Add(loadingBg);
		contentBgd = new GUIImage();
		contentBgd.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero, new Vector2(432f, 464f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		contentBgd.TextureSource = "GUI/loading/welcome/mshs_central_content_panel_bg";
		contentBgd.Id = "content bgd";
		Add(contentBgd);
		heroBgd = new GUIImage();
		heroBgd.SetPositionAndSize(QuickSizingHint.ParentSize);
		heroBgd.Id = "hero characters";
		heroBgd.TextureSource = "GUI/loading/welcome/mshs_welcome_screen_characters_r2";
		Add(heroBgd);
		contentBubble = new GUIImage();
		contentBubble.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, -16f), new Vector2(274f, 183f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		contentBubble.TextureSource = "GUI/loading/welcome/mshs_central_content_panel_speechbubble";
		contentBubble.Id = "content Bubble";
		Add(contentBubble);
		welcomeLabel = new GUILabel();
		welcomeLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 20, ColorUtil.FromRGB255(26, 28, 71), TextAnchor.MiddleCenter);
		welcomeLabel.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-27f, -73f), new Vector2(141f, 55f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		welcomeLabel.Id = "welcome Label";
		welcomeLabel.Text = "#welcome_greeting";
		Add(welcomeLabel);
		fullscreenLabel = new GUILabel();
		fullscreenLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 20, ColorUtil.FromRGB255(26, 28, 71), TextAnchor.MiddleCenter);
		fullscreenLabel.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(11f, 5f), new Vector2(196f, 56f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		fullscreenLabel.Id = "fullscreen Label";
		fullscreenLabel.Text = "#welcome_question";
		Add(fullscreenLabel);
		buttonInset = new GUIImage();
		buttonInset.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero, new Vector2(432f, 464f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		buttonInset.TextureSource = "GUI/loading/welcome/mshs_central_content_panel_indents_for_buttons";
		buttonInset.Id = "button indents";
		Add(buttonInset);
		yesButton = new GUIButton();
		yesButton.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-42f, 110f), new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		yesButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|L_mshs_button_yes");
		yesButton.Text = string.Empty;
		yesButton.Click += yesButton_Click;
		Add(yesButton);
		noButton = new GUIButton();
		noButton.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(44f, 107f), new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		noButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|L_mshs_button_no");
		noButton.Text = string.Empty;
		noButton.Click += noButton_Click;
		Add(noButton);
		becomeAgentButton = new GUIButton();
		becomeAgentButton.SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(17f, -5f), new Vector2(206f, 137f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		becomeAgentButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|L_button_becomeanagent");
		becomeAgentButton.Text = string.Empty;
		becomeAgentButton.Click += becomeAgentButton_Click;
		Add(becomeAgentButton);
		shsLogo = new GUIImage();
		shsLogo.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, -210f), new Vector2(231f, 209f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		shsLogo.TextureSource = "GUI/loading/welcome/shsu_game_logo";
		shsLogo.Id = "logo";
		Add(shsLogo);
		gazLogo = new GUIImage();
		gazLogo.SetPositionAndSize(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, OffsetType.Absolute, new Vector2(-240f, 0f), new Vector2(127f, 64f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gazLogo.TextureSource = "GUI/loading/welcome_gaz_logo";
		gazLogo.Id = "gazlogo";
		Add(gazLogo);
		tasLogo = new GUIImage();
		tasLogo.SetPositionAndSize(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, OffsetType.Absolute, new Vector2(-110f, 6f), new Vector2(130f, 75f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		tasLogo.TextureSource = "GUI/loading/welcome_tas_logo";
		tasLogo.Id = "taslogo";
		Add(tasLogo);
		marvelImage = new GUIImage();
		marvelImage.SetPositionAndSize(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, OffsetType.Absolute, new Vector2(0f, 12f), new Vector2(117f, 81f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		marvelImage.TextureSource = "GUI/loading/welcome_marvel_logo_normal";
		marvelImage.Id = "marvelImage";
		Add(marvelImage);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		return true;
	}

	private void marvelButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		Application.ExternalEval("document.location='http://www.marvel.com';");
	}

	private void yesButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		WelcomeResponse(true);
	}

	private void noButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		WelcomeResponse(false);
	}

	private void WelcomeResponse(bool fullscreen)
	{
		if (fullscreen)
		{
			AppShell.Instance.AutoFullScreenToggle();
		}
		UnMute();
		AppShell.Instance.SharedHashTable["WelcomeScreenViewed"] = true;
		AppShell.Instance.EventMgr.Fire(this, new WelcomeResponseMessage(false, null));
		Hide();
	}

	private void becomeAgentButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.Subscribe);
	}

	public override void OnShow()
	{
		mutingContext = AppShell.Instance.AudioManager.MuteAllExcept(new string[4]
		{
			"HUD_UI",
			"VO_UI",
			"VO_UI_Boss",
			"Music"
		});
		becomeAgentButton.IsVisible = !Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.IsPayingSubscriber);
		base.OnShow();
	}

	private void UnMute()
	{
		AppShell.Instance.AudioManager.UnMute(mutingContext);
	}
}
