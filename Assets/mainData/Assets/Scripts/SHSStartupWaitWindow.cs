using System;
using UnityEngine;

public class SHSStartupWaitWindow : SHSWaitWindow
{
	private const int PROGRESS_CAP_WIDTH = 8;

	private const int PROGRESS_CAP_HEIGHT = 25;

	private GUIImage loadingBg;

	private GUIImage heroBgd;

	private GUIImage contentBgd;

	private GUIImage capnImage;

	private GUIImage shsLogo;

	private GUIImage loadingInset;

	private GUIChildWindow progressWindow;

	private GUIImage progressInset;

	private GUIImage progressOverlay;

	private GUIImage progressLeftCap;

	private GUIImage progressMiddle;

	private GUIImage progressRightCap;

	private GUILabel percentLabel;

	private GUIImage tasLogo;

	private GUIImage gazLogo;

	private GUIImage marvelImage;

	private GUILabel versionLabel;

	public override bool InitializeResources(bool reload)
	{
		loadingBg = new GUIImage();
		loadingBg.SetPositionAndSize(QuickSizingHint.ParentSize);
		loadingBg.TextureSource = "GUI/loading/welcome/mshs_welcome_screen_bg";
		loadingBg.Id = "loading background";
		Add(loadingBg);
		contentBgd = new GUIImage();
		contentBgd.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero, new Vector2(432f, 464f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		contentBgd.TextureSource = "GUI/loading/welcome/mshs_central_content_panel_bg";
		contentBgd.Id = "content bgd";
		Add(contentBgd);
		capnImage = new GUIImage();
		capnImage.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 3f), new Vector2(171f, 201f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		capnImage.TextureSource = "GUI/loading/welcome/mshs_central_content_panel_shieldposter";
		capnImage.Id = "capn image";
		Add(capnImage);
		loadingInset = new GUIImage();
		loadingInset.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(432f, 464f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		loadingInset.TextureSource = "GUI/loading/welcome/mshs_central_content_panel_indents_for_loadingbar";
		loadingInset.Id = "loading inset";
		Add(loadingInset);
		progressInset = new GUIImage();
		progressInset.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-0.4f, 119f), new Vector2(203f, 29f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		progressInset.TextureSource = "GUI/loading/progressbar/loading_screen_loading_bar_empty";
		progressInset.Id = "progress inset";
		Add(progressInset);
		progressWindow = new GUIChildWindow();
		progressWindow.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.MiddleLeft);
		progressWindow.Offset = new Vector2(-98f, 120f);
		progressWindow.Size = new Vector2(194f, 29f);
		progressWindow.Id = "progress window container";
		Add(progressWindow);
		progressMiddle = new GUIImage();
		progressMiddle.SetSize(50f, 25f);
		progressMiddle.SetPosition(8f, 0f);
		progressMiddle.TextureSource = "GUI/loading/progressbar/loading_screen_loading_bar_middle";
		progressMiddle.Id = "loading bar";
		progressWindow.Add(progressMiddle);
		progressLeftCap = new GUIImage();
		progressLeftCap.TextureSource = "GUI/loading/progressbar/loading_screen_loading_bar_left";
		progressLeftCap.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
		progressLeftCap.Offset = new Vector2(0f, 0f);
		progressLeftCap.SetSize(8f, 25f);
		progressLeftCap.IsVisible = false;
		progressWindow.Add(progressLeftCap);
		progressRightCap = new GUIImage();
		progressRightCap.TextureSource = "GUI/loading/progressbar/loading_screen_loading_bar_right";
		progressRightCap.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		progressRightCap.SetSize(8f, 25f);
		progressRightCap.Offset = new Vector2(0f, 0f);
		progressWindow.Add(progressRightCap);
		progressOverlay = new GUIImage();
		progressOverlay.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-0.8f, 118f), new Vector2(203f, 29f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		progressOverlay.TextureSource = "GUI/loading/progressbar/loading_screen_loading_bar_highlight";
		progressOverlay.Id = "progress overlay";
		Add(progressOverlay);
		percentLabel = new GUILabel();
		percentLabel.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight);
		percentLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, ColorUtil.FromRGB255(32, 44, 7), TextAnchor.MiddleRight);
		percentLabel.WordWrap = false;
		percentLabel.Bold = true;
		percentLabel.SetSize(50f, 25f);
		percentLabel.Offset = new Vector2(0f, -3f);
		progressWindow.Add(percentLabel);
		heroBgd = new GUIImage();
		heroBgd.SetPositionAndSize(QuickSizingHint.ParentSize);
		heroBgd.Id = "hero characters";
		heroBgd.TextureSource = "GUI/loading/welcome/mshs_welcome_screen_characters_r2";
		Add(heroBgd);
		shsLogo = new GUIImage();
		shsLogo.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, -210f), new Vector2(231f, 209f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		shsLogo.TextureSource = "GUI/loading/welcome/mshs_welcome_screen_game_logo";
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

		/////// block added by CSP ///////////
		versionLabel = new GUILabel();
		versionLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 20, ColorUtil.FromRGB255(222, 222, 222), TextAnchor.MiddleCenter);
		versionLabel.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-27f, 220f), new Vector2(141f, 55f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		//versionLabel.Id = "welcome Label";
		versionLabel.Text = "client version: " + CspUtils.version;
		Add(versionLabel);
		//////////////////

		base.AnimationOnOpen = SHSAnimations.WindowOpenCloseDelegates.FadeIn(0f, this);
		base.AnimationOnClose = SHSAnimations.WindowOpenCloseDelegates.FadeOut(0.5f, this);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		Traits.LifeSpan = ControlTraits.LifeSpanTraitEnum.DestroyOnHide;
		return true;
		}

	private void UpdateProgressBar(float value)
	{
		progressLeftCap.IsVisible = true;
		Vector2 size = progressWindow.Size;
		float val = size.x - 16f;
		Vector2 size2 = progressWindow.Size;
		float num = Math.Min(val, size2.x * (1f - value / 100f));
		progressRightCap.Offset = new Vector2(0f - num, 0f);
		percentLabel.Offset = new Vector2(0f - num - 6f, -3f);
		GUIImage gUIImage = progressMiddle;
		Vector2 size3 = progressWindow.Size;
		float x = size3.x;
		Vector2 offset = progressRightCap.Offset;
		gUIImage.SetSize(new Vector2(Math.Max(0f, x + offset.x - 16f), 25f));
		percentLabel.IsVisible = (value > 10f);
		if (percentLabel.IsVisible)
		{
			percentLabel.Text = string.Format("{0}%  ", ((int)value).ToString());
		}
	}

	public override void Update()
	{
		BaseUpdate();
		ProgressUpdate();
		UpdateProgressBar(base.ProgressValue);
	}
}
