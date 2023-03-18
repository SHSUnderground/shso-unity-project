using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSAutoDeckBuilderWindow : GUITopLevelWindow
{
	public enum DeckThemeType
	{
		Factor,
		Team,
		Hero
	}

	private class ThemeDatum
	{
		public DeckThemeType themeType;

		public string iconId;

		public string name;

		public ThemeDatum(DeckThemeType type, string iconId, string name)
		{
			themeType = type;
			this.iconId = iconId;
			this.name = name;
		}
	}

	private GUISimpleControlWindow root;

	private GUISimpleControlWindow introductionPanel;

	private GUISimpleControlWindow deckPanel;

	private GUISimpleControlWindow middlePanel;

	private GUISimpleControlWindow largeCardPane;

	private GUIButton buildADeckButton;

	private SHSCharacterSelect.CharacterSelection characterSelection;

	private GUIDrawTexture largeCard;

	private bool largeCardSet;

	private CardZoomComponent zoomComponent;

	private static readonly Vector2 VIEW_CARD_SIZE = new Vector2(244f, 340f);

	private static readonly Vector2 ZOOM_CARD_SIZE = new Vector2(366f, 512f);

	private static readonly Vector2 VIEW_CARD_OFFSET = new Vector2(-4f, 0f);

	private static readonly Vector2 ZOOM_CARD_OFFSET = new Vector2(-4f, 0f);

	private static readonly int MIN_CARD_COUNT_FOR_THEME = 4;

	private GUIImage deckRenderTexture;

	private GUIStrokeTextLabel myCardsLabel;

	private GUISlider deckSlider;

	private int deckCardCount;

	private float deckScrollSpeed;

	private CardProperties lastPickedCard;

	private bool deckMouseOver;

	private bool suppressZoomedCard;

	private Dictionary<string, Vector2> FactorOffsets;

	private Dictionary<string, Vector2> TeamTooltipOffsets;

	private Dictionary<string, DeckTheme> ThemeList;

	private List<GUIAnimatedButton> ThemeButtonList;

	private Dictionary<string, GUIAnimatedButton> TeamToggleButtons;

	private Dictionary<string, GUIAnimatedButton> FactorToggleButtons;

	private List<ThemeDatum> ThemeData;

	public SHSAutoDeckBuilderWindow()
		: base("SHSAutoDeckBuilderWindow")
	{
		Traits.ResourceLoadingPhaseTrait = ControlTraits.ResourceLoadingPhaseTraitEnum.Active;
		ThemeList = new Dictionary<string, DeckTheme>();
		ThemeButtonList = new List<GUIAnimatedButton>();
		ThemeData = new List<ThemeDatum>();
		TeamTooltipOffsets = new Dictionary<string, Vector2>();
		TeamTooltipOffsets["F4"] = new Vector2(-9f, 0f);
		TeamTooltipOffsets["Avengers"] = new Vector2(0f, 0f);
		TeamTooltipOffsets["SpideyFriends"] = new Vector2(-30f, 0f);
		TeamTooltipOffsets["Xmen"] = new Vector2(0f, 0f);
		TeamTooltipOffsets["SS"] = new Vector2(-37f, 0f);
		TeamTooltipOffsets["Brotherhood"] = new Vector2(-62f, 0f);
	}

	protected override void InitializeBundleList()
	{
		base.InitializeBundleList();
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("cardgame_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("brawler_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("deckbuilder_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
	}

	public override void OnSceneLoaded(AppShell.GameControllerTypeData currentGameData)
	{
		if (characterSelection.items.Count == 0)
		{
			PopulateCharacterSelector();
		}
		base.OnSceneLoaded(currentGameData);
	}

	private AnimClip AddPulseAnimation(GUIAnimatedButton ctrl)
	{
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		AnimClip animClip = (AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(1f, 1.25f, 0.5f), ctrl.ModPercX) ^ AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(1f, 1.25f, 0.5f), ctrl.ModPercY)) | (AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(1.25f, 1f, 0.5f), ctrl.ModPercX) ^ AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(1.25f, 1f, 0.5f), ctrl.ModPercY));
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			AddPulseAnimation(ctrl);
		};
		base.AnimationPieceManager.Add(animClip);
		return animClip;
	}

	private void AddMouseoverAudio(GUIControl control)
	{
		control.MouseOver += delegate
		{
			ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("hover_over"));
		};
		control.MouseDown += delegate
		{
			ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("click_down"));
		};
		control.MouseUp += delegate
		{
			ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("click_up"));
		};
	}

	public override bool InitializeResources(bool reload)
	{
		return base.InitializeResources(reload);
	}

	public void CreateWindow()
	{
		GUILabel gUILabel = null;
		GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(Vector2.zero, Vector2.zero);
		gUIImage.Id = "bgImage";
		gUIImage.SetPositionAndSize(QuickSizingHint.ScreenSize);
		gUIImage.TextureSource = "persistent_bundle|loading_bg_bluecircles";
		Add(gUIImage);
		root = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(1022f, 650f), Vector2.zero);
		root.Id = "root";
		Add(root);
		root.BringToFront();
		GUISimpleControlWindow gUISimpleControlWindow = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(1020f, 650f), new Vector2(0f, 8f));
		gUISimpleControlWindow.Id = "background";
		root.Add(gUISimpleControlWindow);
		gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(1020f, 630f), Vector2.zero);
		gUIImage.Id = "bgImage";
		gUIImage.TextureSource = "deckbuilder_bundle|autodeckbuilder_main_frame";
		gUISimpleControlWindow.Add(gUIImage);
		GUISimpleControlWindow gUISimpleControlWindow2 = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(512f, 128f), new Vector2(0f, -258f));
		gUISimpleControlWindow2.Id = "gadgetTitle";
		root.Add(gUISimpleControlWindow2);
		GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(512f, 116f), new Vector2(0f, -6f));
		gUIImage2.Id = "gadgetTopModule";
		gUIImage2.TextureSource = "persistent_bundle|gadget_topmodule";
		gUISimpleControlWindow2.Add(gUIImage2);
		GUIImage gUIImage3 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(213f, 38f), new Vector2(0f, -12f));
		gUIImage3.Id = "titleImage";
		gUIImage3.TextureSource = "deckbuilder_bundle|L_autodeckbuilder_title_deck_builder";
		gUISimpleControlWindow2.Add(gUIImage3);
		introductionPanel = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(627f, 617f), Vector2.zero);
		introductionPanel.Id = "introductionPanel";
		introductionPanel.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		gUISimpleControlWindow.Add(introductionPanel);
		introductionPanel.IsVisible = true;
		GUIImage gUIImage4 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(627f, 617f), Vector2.zero);
		gUIImage4.Id = "introImage";
		gUIImage4.TextureSource = "deckbuilder_bundle|autodeckbuilder_introduction";
		introductionPanel.Add(gUIImage4);
		gUILabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(196f, 77f), new Vector2(92f, -160f));
		gUILabel.Id = "Intro1";
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 20, GUILabel.GenColor(52, 75, 87), TextAnchor.UpperLeft);
		gUILabel.Text = "#DECKBUILDER_AUTO_INTRO_1";
		introductionPanel.Add(gUILabel);
		gUILabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(215f, 77f), new Vector2(-99f, 40f));
		gUILabel.Id = "Intro2";
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 21, GUILabel.GenColor(52, 75, 87), TextAnchor.UpperLeft);
		gUILabel.Text = "#DECKBUILDER_AUTO_INTRO_2";
		introductionPanel.Add(gUILabel);
		GUISimpleControlWindow gUISimpleControlWindow3 = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(258f, 630f), new Vector2(-370f, 0f));
		gUISimpleControlWindow3.Id = "leftPanel";
		gUISimpleControlWindow.Add(gUISimpleControlWindow3);
		GUIStrokeTextLabel gUIStrokeTextLabel = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(205f, 29f), new Vector2(7f, 39f));
		gUIStrokeTextLabel.Id = "pickThemesLabel";
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 25, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 114, 255), GUILabel.GenColor(22, 77, 195), new Vector2(4f, 3f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel.Text = "#DECKBUILDER_AUTO_PICK_DECK_THEMES";
		gUIStrokeTextLabel.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		gUIStrokeTextLabel.ToolTip = new NamedToolTipInfo("#TT_AUTODECK_PICKED_THEMES");
		gUISimpleControlWindow3.Add(gUIStrokeTextLabel);
		GUIStrokeTextLabel gUIStrokeTextLabel2 = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(71f, 23f), new Vector2(-78f, 62f));
		gUIStrokeTextLabel2.Id = "factorsLabel";
		gUIStrokeTextLabel2.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(98, 96, 50), GUILabel.GenColor(55, 53, 15), new Vector2(1f, 2f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel2.Text = "#DECKBUILDER_AUTO_FACTORS";
		gUISimpleControlWindow3.Add(gUIStrokeTextLabel2);
		GUIStrokeTextLabel gUIStrokeTextLabel3 = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(71f, 23f), new Vector2(-78f, 127f));
		gUIStrokeTextLabel3.Id = "teamsLabel";
		gUIStrokeTextLabel3.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(98, 96, 50), GUILabel.GenColor(55, 53, 15), new Vector2(1f, 2f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel3.Text = "#DECKBUILDER_AUTO_TEAMS";
		gUISimpleControlWindow3.Add(gUIStrokeTextLabel3);
		GUIStrokeTextLabel gUIStrokeTextLabel4 = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(71f, 23f), new Vector2(-78f, 198f));
		gUIStrokeTextLabel4.Id = "heroesLabel";
		gUIStrokeTextLabel4.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(98, 96, 50), GUILabel.GenColor(55, 53, 15), new Vector2(1f, 2f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel4.Text = "#DECKBUILDER_AUTO_HEROES";
		gUISimpleControlWindow3.Add(gUIStrokeTextLabel4);
		GUISimpleControlWindow gUISimpleControlWindow4 = GUIControl.CreateControlTopFrameCentered<GUISimpleControlWindow>(new Vector2(235f, 48f), new Vector2(-4f, 95f));
		gUISimpleControlWindow4.Id = "factorPanel";
		gUISimpleControlWindow3.Add(gUISimpleControlWindow4);
		FactorOffsets = new Dictionary<string, Vector2>();
		FactorOffsets["Animal"] = new Vector2(24f, 0f);
		FactorOffsets["Strength"] = new Vector2(61f, 0f);
		FactorOffsets["Elemental"] = new Vector2(98f, 0f);
		FactorOffsets["Energy"] = new Vector2(132f, 0f);
		FactorOffsets["Tech"] = new Vector2(172f, 0f);
		FactorOffsets["Speed"] = new Vector2(209f, 0f);
		int num = 0;
		FactorToggleButtons = new Dictionary<string, GUIAnimatedButton>();
		foreach (KeyValuePair<char, string> item in DeckBuilderController.Instance.FactorLookup)
		{
			char key = item.Key;
			GUIAnimatedButton gUIAnimatedButton = new GUIAnimatedButton();
			gUIAnimatedButton.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, FactorOffsets[item.Value], new Vector2(47f, 47f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIAnimatedButton.TextureSource = "deckbuilder_bundle|deck_theme_factor_" + key;
			gUIAnimatedButton.ToolTip = new NamedToolTipInfo("#TT_CARDGAME_FACTOR_" + key);
			gUIAnimatedButton.SetupButton(1f, 1.25f, 0.9f);
			gUIAnimatedButton.Id = "factorToggle_" + key;
			string iconId = key.ToString();
			string name = item.Value.ToLower();
			ThemeDatum td = new ThemeDatum(DeckThemeType.Factor, iconId, name);
			ThemeData.Add(td);
			gUIAnimatedButton.Click += delegate
			{
				if (AddTheme(td))
				{
					ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("click_down"));
				}
			};
			bool isEnabled = IsValidFactor(key.ToString());
			FactorToggleButtons[key.ToString()] = gUIAnimatedButton;
			gUIAnimatedButton.IsEnabled = isEnabled;
			AddMouseoverAudio(gUIAnimatedButton);
			gUISimpleControlWindow4.Add(gUIAnimatedButton);
			num++;
		}
		GUISimpleControlWindow gUISimpleControlWindow5 = GUIControl.CreateControlTopFrameCentered<GUISimpleControlWindow>(new Vector2(235f, 48f), new Vector2(-5f, 162f));
		gUISimpleControlWindow5.Id = "teamPanel";
		gUISimpleControlWindow3.Add(gUISimpleControlWindow5);
		int num2 = 0;
		TeamToggleButtons = new Dictionary<string, GUIAnimatedButton>();
		foreach (KeyValuePair<char, string> item2 in DeckBuilderController.Instance.TeamLookup)
		{
			char key2 = item2.Key;
			GUIAnimatedButton gUIAnimatedButton2 = new GUIAnimatedButton();
			gUIAnimatedButton2.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(24f + (float)num2 * 37.2f, 0f), new Vector2(46f, 46f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIAnimatedButton2.TextureSource = "deckbuilder_bundle|deck_theme_team_" + key2;
			gUIAnimatedButton2.Id = "teamToggle_" + key2;
			gUIAnimatedButton2.SetupButton(1f, 1.25f, 0.9f);
			gUIAnimatedButton2.ToolTip = new NamedToolTipInfo("#TT_CARDGAME_TEAM_" + key2);
			gUIAnimatedButton2.ToolTip.Offset = TeamTooltipOffsets[item2.Value];
			string iconId2 = key2.ToString();
			string name2 = item2.Value.ToLower();
			ThemeDatum td2 = new ThemeDatum(DeckThemeType.Team, iconId2, name2);
			ThemeData.Add(td2);
			gUIAnimatedButton2.Click += delegate
			{
				if (AddTheme(td2))
				{
					ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("click_down"));
				}
			};
			AddMouseoverAudio(gUIAnimatedButton2);
			bool isEnabled2 = IsValidTeam(key2.ToString());
			TeamToggleButtons[key2.ToString()] = gUIAnimatedButton2;
			gUIAnimatedButton2.IsEnabled = isEnabled2;
			gUISimpleControlWindow5.Add(gUIAnimatedButton2);
			num2++;
		}
		GUISlider gUISlider = GUIControl.CreateControlFrameCentered<GUISlider>(new Vector2(50f, 370f), new Vector2(110f, 110f));
		gUISlider.Id = "slider";
		characterSelection = new SHSCharacterSelect.CharacterSelection(gUISlider);
		characterSelection.TopOffsetAdjustHeight = 0f;
		characterSelection.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(18f, 105f), new Vector2(227f, 350f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUISimpleControlWindow3.Add(gUISlider);
		gUISimpleControlWindow3.Add(characterSelection);
		GUITextField searchField = new GUITextField();
		searchField.Id = "searchField";
		searchField.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(19f, -91f), new Vector2(160f, 30f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		searchField.FontFace = GUIFontManager.SupportedFontEnum.Komica;
		searchField.FontSize = 16;
		searchField.TextColor = GUILabel.GenColor(65, 112, 160);
		searchField.Text = string.Empty;
		searchField.TextAlignment = TextAnchor.MiddleLeft;
		searchField.WordWrap = false;
		searchField.ToolTip = new NamedToolTipInfo("#characterselect_text_search_tt");
		searchField.Changed += delegate
		{
			characterSelection.Sort(searchField.Text);
		};
		searchField.GotFocus += delegate
		{
			searchField.Text = string.Empty;
			characterSelection.Sort(searchField.Text);
		};
		gUISimpleControlWindow3.Add(searchField);
		middlePanel = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(474f, 488f), new Vector2(0f, 20f));
		middlePanel.Id = "middlePanel";
		gUISimpleControlWindow.Add(middlePanel);
		GUIImage gUIImage5 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(474f, 488f), Vector2.zero);
		gUIImage5.Id = "deckPanelBackground";
		gUIImage5.TextureSource = "deckbuilder_bundle|autodeckbuilder_deck_frame";
		middlePanel.Add(gUIImage5);
		GUIStrokeTextLabel gUIStrokeTextLabel5 = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(106f, 90f), new Vector2(-165f, -208f));
		gUIStrokeTextLabel5.Id = "pickedThemesLabel";
		gUIStrokeTextLabel5.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 26, GUILabel.GenColor(236, 250, 255), GUILabel.GenColor(31, 75, 174), GUILabel.GenColor(27, 61, 134), new Vector2(4f, 3f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel5.Text = "#DECKBUILDER_AUTO_PICKED_THEMES";
		gUIStrokeTextLabel5.VerticalKerning = 20;
		middlePanel.Add(gUIStrokeTextLabel5);
		GUISimpleControlWindow gUISimpleControlWindow6 = new GUISimpleControlWindow();
		gUISimpleControlWindow6.Id = "deckPane";
		gUISimpleControlWindow6.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-7f, 13f), new Vector2(405f, 363f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		middlePanel.Add(gUISimpleControlWindow6);
		deckRenderTexture = new GUIImage();
		deckRenderTexture.Id = "deckRenderTexture";
		deckRenderTexture.SetPositionAndSize(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(390f, 390f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUISimpleControlWindow6.Add(deckRenderTexture);
		gUISimpleControlWindow6.HitTestType = HitTestTypeEnum.Rect;
		gUISimpleControlWindow6.MouseOver += delegate
		{
			deckMouseOver = true;
		};
		gUISimpleControlWindow6.MouseOut += delegate
		{
			deckMouseOver = false;
		};
		gUISimpleControlWindow6.RightMouseDown += delegate
		{
			DeckBuilderController deckBuilderController2 = (DeckBuilderController)GameController.GetController();
			GameObject x2 = deckBuilderController2.PickCardPanel(deckBuilderController2.deckCamera, deckRenderTexture);
			if (x2 != null)
			{
				if (!zoomComponent.IsZooming)
				{
					zoomComponent.ZoomInOnCard(false);
				}
				else
				{
					zoomComponent.ZoomOutOnCard();
				}
			}
		};
		deckSlider = GUIControl.CreateControlFrameCentered<GUISlider>(new Vector2(70f, 440f), new Vector2(213f, 11f));
		deckSlider.Id = "deckSlider";
		deckSlider.Orientation = GUISlider.SliderOrientationEnum.Vertical;
		if (DeckBuilderController.Instance.DeckCards != null && DeckBuilderController.Instance.DeckCards.Count > 0)
		{
			deckSlider.IsVisible = true;
		}
		else
		{
			deckSlider.IsVisible = false;
		}
		deckSlider.ArrowsEnabled = true;
		deckSlider.IsEnabled = true;
		deckSlider.Min = 0f;
		deckSlider.Max = 100f;
		deckSlider.Value = 0f;
		deckSlider.StyleInfo = SHSInheritedStyleInfo.Instance;
		deckSlider.Changed += delegate(GUIControl sender, GUIChangedEvent eventData)
		{
			AppShell.Instance.EventMgr.Fire(this, new CardCollectionUIMessage(CardCollectionUIMessage.CCUIEvent.DeckGridScroll, sender));
		};
		middlePanel.Add(deckSlider);
		deckSlider.Alpha = 0f;
		middlePanel.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		middlePanel.IsVisible = false;
		GUIButton clearThemesButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(-187f, 260f));
		clearThemesButton.Id = "clearThemesButton";
		clearThemesButton.HitTestType = HitTestTypeEnum.Alpha;
		clearThemesButton.StyleInfo = new SHSButtonStyleInfo("deckbuilder_bundle|autodeckbuilder_clearthemes_button");
		clearThemesButton.ToolTip = new NamedToolTipInfo("#TT_AUTODECK_CLEAR", new Vector2(-38f, 26f));
		clearThemesButton.MouseDown += delegate(GUIControl sender, GUIMouseEvent eventArgs)
		{
			GUIDialogNotificationSink notificationSink = new GUIDialogNotificationSink(delegate(string Id, GUIDialogWindow.DialogState state)
			{
				if (state == GUIDialogWindow.DialogState.Ok)
				{
					AppShell.Instance.EventMgr.Fire(sender, new CardCollectionUIMessage(CardCollectionUIMessage.CCUIEvent.NewClicked, clearThemesButton));
					largeCard.TextureSource = "deckbuilder_bundle|mshs_card_rear_noURL";
					if (buildADeckButton != null && buildADeckButton.ToolTip != null)
					{
						buildADeckButton.ToolTip = new NamedToolTipInfo("#TT_AUTODECK_BUILD_DECK_NO_THEMES", new Vector2(20f, 200f));
					}
					largeCardSet = false;
					ClearThemeList();
				}
			});
			SHSCommonDialogWindow sHSCommonDialogWindow = new SHSCommonDialogWindow("common_bundle|notification_icon_cleardeck", new Vector2(256f, 247f), new Vector2(-5f, 0f), string.Empty, "common_bundle|L_mshs_button_ok", "common_bundle|L_mshs_button_cancel", typeof(SHSDialogOkButton), typeof(SHSDialogCancelButton), false);
			sHSCommonDialogWindow.TitleText = "#deckbuilder_clear_themes_title";
			sHSCommonDialogWindow.Text = "#deckbuilder_new_deck_warning";
			sHSCommonDialogWindow.NotificationSink = notificationSink;
			GUIManager.Instance.ShowDynamicWindow(sHSCommonDialogWindow, ModalLevelEnum.Full);
		};
		gUISimpleControlWindow.Add(clearThemesButton);
		GUIStrokeTextLabel gUIStrokeTextLabel6 = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(80f, 60f), new Vector2(-189f, 260f));
		gUIStrokeTextLabel6.Id = "clearThemesLabel";
		gUIStrokeTextLabel6.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 15, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(102, 129, 21), GUILabel.GenColor(71, 92, 8), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel6.Text = "#DECKBUILDER_AUTO_CLEAR_THEMES";
		gUIStrokeTextLabel6.VerticalKerning = 15;
		gUISimpleControlWindow.Add(gUIStrokeTextLabel6);
		buildADeckButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(512f, 512f), new Vector2(0f, 260f));
		buildADeckButton.Id = "buildADeckButton";
		buildADeckButton.HitTestType = HitTestTypeEnum.Alpha;
		buildADeckButton.ToolTip = new NamedToolTipInfo("#TT_AUTODECK_BUILD_DECK_NO_THEMES", new Vector2(78f, 200f));
		buildADeckButton.StyleInfo = new SHSButtonStyleInfo("deckbuilder_bundle|builddeck_button");
		buildADeckButton.Click += delegate
		{
			BuilAutoDeck();
		};
		gUISimpleControlWindow.Add(buildADeckButton);
		GUIStrokeTextLabel gUIStrokeTextLabel7 = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(250f, 30f), new Vector2(41f, 260f));
		gUIStrokeTextLabel7.Id = "buildADeckLabel";
		gUIStrokeTextLabel7.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 27, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(102, 129, 21), GUILabel.GenColor(71, 92, 8), new Vector2(1f, 2f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel7.Text = "#DECKBUILDER_AUTO_BUILD_A_DECK";
		gUISimpleControlWindow.Add(gUIStrokeTextLabel7);
		GUISimpleControlWindow gUISimpleControlWindow7 = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(250f, 575f), new Vector2(368f, 15f));
		gUISimpleControlWindow7.Id = "rightPanel";
		gUISimpleControlWindow.Add(gUISimpleControlWindow7);
		GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(3f, 99f));
		gUIButton.Id = "myCardsButton";
		gUIButton.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton.StyleInfo = new SHSButtonStyleInfo("deckbuilder_bundle|autodeckbuilder_wide_button");
		gUIButton.ToolTip = new NamedToolTipInfo("#TT_AUTODECK_EDIT_DECK", new Vector2(82f, 100f));
		gUIButton.Click += delegate
		{
			HideRoot();
			GUIManager.Instance["/SHSMainWindow/SHSAutoDeckBuilderWindow/SHSDeckBuilderWindow"].Show();
		};
		gUISimpleControlWindow7.Add(gUIButton);
		myCardsLabel = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(210f, 30f), new Vector2(3f, 99f));
		myCardsLabel.Id = "myCardsLabel";
		myCardsLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 22, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(102, 129, 21), GUILabel.GenColor(71, 92, 8), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
		myCardsLabel.Text = "#DECKBUILDER_AUTO_MY_CARDS";
		myCardsLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUISimpleControlWindow7.Add(myCardsLabel);
		GUIButton gUIButton2 = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(3f, 147f));
		gUIButton2.Id = "boosterButton";
		gUIButton2.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton2.StyleInfo = new SHSButtonStyleInfo("deckbuilder_bundle|autodeckbuilder_wide_button");
		gUIButton2.ToolTip = new NamedToolTipInfo("#TT_AUTODECK_BUY_CARDS", new Vector2(30f, 100f));
		gUIButton2.Click += delegate
		{
			suppressZoomedCard = true;
			DeckBuilderController DeckBuilder = (DeckBuilderController)GameController.GetController();
			if (DeckBuilder != null && DeckBuilder.DeckCards != null && DeckBuilder.DeckCards.Count > 0 && !DeckBuilder.DeckCards.saved)
			{
				GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, "#DECKBUILDER_EXIT_SAVE_DECK_ PROMPT", delegate(string Id, GUIDialogWindow.DialogState state)
				{
					if (state == GUIDialogWindow.DialogState.Ok)
					{
						ShsEventMgr.GenericDelegate<CardCollectionSavedMessage> t = null;
						t = delegate
						{
							AppShell.Instance.EventMgr.RemoveListener(t);
							ShoppingWindow shoppingWindow3 = new ShoppingWindow(NewShoppingManager.ShoppingCategory.Card);
							shoppingWindow3.OnHidden += delegate
							{
								CspUtils.DebugLog("New shop hidden, refreshing");
								DeckBuilder.FetchCardCollection();
								suppressZoomedCard = false;
							};
							shoppingWindow3.launch();
						};
						AppShell.Instance.EventMgr.AddListener(t);
						AppShell.Instance.EventMgr.Fire(this, new CardCollectionUIMessage(CardCollectionUIMessage.CCUIEvent.SaveClicked, this));
					}
					else
					{
						ShoppingWindow shoppingWindow2 = new ShoppingWindow(NewShoppingManager.ShoppingCategory.Card);
						shoppingWindow2.OnHidden += delegate
						{
							CspUtils.DebugLog("New shop hidden2, refreshing");
							DeckBuilder.FetchCardCollection();
							suppressZoomedCard = false;
						};
						shoppingWindow2.launch();
					}
				}, ModalLevelEnum.Default);
			}
			else
			{
				ShoppingWindow shoppingWindow = new ShoppingWindow(NewShoppingManager.ShoppingCategory.Card);
				shoppingWindow.OnHidden += delegate
				{
					CspUtils.DebugLog("New shop hidden3, refreshing");
					DeckBuilder.FetchCardCollection();
				};
				shoppingWindow.launch();
			}
		};
		AppShell.Instance.EventMgr.AddListener<ShoppingItemPurchasedMessage>(OnCardPurchased);
		gUISimpleControlWindow7.Add(gUIButton2);
		GUIStrokeTextLabel gUIStrokeTextLabel8 = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(250f, 30f), new Vector2(3f, 147f));
		gUIStrokeTextLabel8.Id = "boosterLabel";
		gUIStrokeTextLabel8.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 22, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(102, 129, 21), GUILabel.GenColor(71, 92, 8), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel8.Text = "#DECKBUILDER_BOOSTER_PACKS";
		gUIStrokeTextLabel8.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUISimpleControlWindow7.Add(gUIStrokeTextLabel8);
		GUIButton gUIButton3 = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(-46f, 193f));
		gUIButton3.Id = "loadButton";
		gUIButton3.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton3.StyleInfo = new SHSButtonStyleInfo("deckbuilder_bundle|deckbuilder_load_button");
		gUIButton3.ToolTip = new NamedToolTipInfo("#TT_DECKBUILDER_2", new Vector2(69f, 18f));
		gUIButton3.Click += delegate(GUIControl sender, GUIClickEvent eventArgs)
		{
			AppShell.Instance.EventMgr.Fire(sender, new CardCollectionUIMessage(CardCollectionUIMessage.CCUIEvent.LoadClicked, sender));
		};
		gUISimpleControlWindow7.Add(gUIButton3);
		GUIStrokeTextLabel gUIStrokeTextLabel9 = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(250f, 30f), new Vector2(-46f, 193f));
		gUIStrokeTextLabel9.Id = "loadLabel";
		gUIStrokeTextLabel9.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(102, 129, 21), GUILabel.GenColor(71, 92, 8), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel9.Text = "#DECKBUILDER_LOAD";
		gUIStrokeTextLabel9.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUISimpleControlWindow7.Add(gUIStrokeTextLabel9);
		GUIButton gUIButton4 = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(54f, 193f));
		gUIButton4.Id = "saveButton";
		gUIButton4.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton4.StyleInfo = new SHSButtonStyleInfo("deckbuilder_bundle|deckbuilder_save_button");
		gUIButton4.ToolTip = new NamedToolTipInfo("#TT_DECKBUILDER_4", new Vector2(-9f, 27f));
		gUIButton4.ToolTip.VerticalAlignmentOverride = SHSTooltip.ToolTipVerticalAlignment.Bottom;
		gUIButton4.Click += delegate(GUIControl sender, GUIClickEvent eventArgs)
		{
			AppShell.Instance.EventMgr.Fire(sender, new CardCollectionUIMessage(CardCollectionUIMessage.CCUIEvent.SaveClicked, sender));
		};
		gUISimpleControlWindow7.Add(gUIButton4);
		GUIStrokeTextLabel gUIStrokeTextLabel10 = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(250f, 30f), new Vector2(54f, 193f));
		gUIStrokeTextLabel10.Id = "saveLabel";
		gUIStrokeTextLabel10.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(102, 129, 21), GUILabel.GenColor(71, 92, 8), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel10.Text = "#DECKBUILDER_SAVE";
		gUIStrokeTextLabel10.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUISimpleControlWindow7.Add(gUIStrokeTextLabel10);
		GUIButton gUIButton5 = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(-72f, 245f));
		gUIButton5.Id = "backButton";
		gUIButton5.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton5.StyleInfo = new SHSButtonStyleInfo("deckbuilder_bundle|deckbuilder_back_button");
		gUIButton5.ToolTip = new NamedToolTipInfo("#TT_DECKBUILDER_BACK", new Vector2(85f, 22f));
		gUIButton5.Click += delegate
		{
			OpenCardGadgetWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.ScreenWithBigPlayButtonThatQuoteEverybodyQuoteLoves);
		};
		gUISimpleControlWindow7.Add(gUIButton5);
		GUIStrokeTextLabel gUIStrokeTextLabel11 = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(60f, 30f), new Vector2(-75f, 245f));
		gUIStrokeTextLabel11.Id = "backLabel";
		gUIStrokeTextLabel11.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(102, 129, 21), GUILabel.GenColor(71, 92, 8), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel11.Text = "#DECKBUILDER_BACK";
		gUIStrokeTextLabel11.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUISimpleControlWindow7.Add(gUIStrokeTextLabel11);
		GUIButton gUIButton6 = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(6f, 245f));
		gUIButton6.Id = "playButton";
		gUIButton6.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton6.StyleInfo = new SHSButtonStyleInfo("deckbuilder_bundle|deckbuilder_play_button");
		gUIButton6.ToolTip = new NamedToolTipInfo("#TT_DECKBUILDER_PLAY", new Vector2(67f, 22f));
		gUIButton6.Click += delegate
		{
			OpenCardGadgetWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.Main);
		};
		gUISimpleControlWindow7.Add(gUIButton6);
		GUIStrokeTextLabel gUIStrokeTextLabel12 = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(80f, 30f), new Vector2(3f, 245f));
		gUIStrokeTextLabel12.Id = "playLabel";
		gUIStrokeTextLabel12.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 24, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(102, 129, 21), GUILabel.GenColor(71, 92, 8), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel12.Text = "#DECKBUILDER_PLAY";
		gUIStrokeTextLabel12.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUISimpleControlWindow7.Add(gUIStrokeTextLabel12);
		GUIButton gUIButton7 = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(87f, 244f));
		gUIButton7.Id = "quitButton";
		gUIButton7.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton7.StyleInfo = new SHSButtonStyleInfo("deckbuilder_bundle|deckbuilder_quit_button");
		gUIButton7.ToolTip = new NamedToolTipInfo("#TT_DECKBUILDER_QUIT", new Vector2(-9f, 27f));
		gUIButton7.ToolTip.VerticalAlignmentOverride = SHSTooltip.ToolTipVerticalAlignment.Bottom;
		gUIButton7.Click += delegate
		{
			Close();
		};
		gUISimpleControlWindow7.Add(gUIButton7);
		GUIStrokeTextLabel gUIStrokeTextLabel13 = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(60f, 30f), new Vector2(85f, 245f));
		gUIStrokeTextLabel13.Id = "quitLabel";
		gUIStrokeTextLabel13.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(169, 44, 19), GUILabel.GenColor(137, 18, 0), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel13.Text = "#DECKBUILDER_QUIT";
		gUIStrokeTextLabel13.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUISimpleControlWindow7.Add(gUIStrokeTextLabel13);
		largeCardPane = new GUISimpleControlWindow();
		largeCardPane.Id = "largeCardPane";
		largeCardPane.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(118f, -5f), new Vector2(380f, 520f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Add(largeCardPane);
		largeCard = GUIControl.CreateControl<GUIDrawTexture>(VIEW_CARD_SIZE, VIEW_CARD_OFFSET, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		largeCard.Id = "largeCard";
		largeCard.TextureSource = "deckbuilder_bundle|mshs_card_rear_noURL";
		largeCard.RightMouseDown += delegate
		{
			if (largeCardSet)
			{
				if (!zoomComponent.IsZooming)
				{
					zoomComponent.ZoomInOnCard(true);
				}
				else
				{
					zoomComponent.ZoomOutOnCard();
				}
			}
		};
		largeCardPane.Add(largeCard);
		GUIButton gUIButton8 = new GUIButton();
		gUIButton8.Id = "closeButton";
		gUIButton8.SetPositionAndSize(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(-25f, -4f), new Vector2(45f, 45f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton8.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton8.StyleInfo = new SHSButtonStyleInfo("common_bundle|button_close");
		gUIButton8.ToolTip = new NamedToolTipInfo(SHSTooltip.GetCommonToolTipText(SHSTooltip.CommonToolTipText.Close));
		gUIButton8.Click += delegate
		{
			Close();
		};
		gUISimpleControlWindow.Add(gUIButton8);
		DeckBuilderController deckBuilderController = (DeckBuilderController)GameController.GetController();
		deckRenderTexture.Texture = deckBuilderController.deckCamera.targetTexture;
		zoomComponent = new CardZoomComponent(base.AnimationPieceManager, largeCard, new CardZoomComponent.ZoomData(VIEW_CARD_SIZE, VIEW_CARD_OFFSET, ZOOM_CARD_SIZE, ZOOM_CARD_OFFSET, 3f, 0.5f));
	}

	private static bool IsValidTeam(string teamType)
	{
		int num = 0;
		bool flag = false;
		if (DeckBuilderController.Instance.MyCards != null)
		{
			foreach (CardListCard myCard in DeckBuilderController.Instance.MyCards)
			{
				if (myCard.BelongsToTeam(teamType))
				{
					num += myCard.Available;
					if (num >= MIN_CARD_COUNT_FOR_THEME)
					{
						flag = true;
						break;
					}
				}
			}
		}
		if (!flag && DeckBuilderController.Instance.DeckCards != null)
		{
			foreach (CardListCard deckCard in DeckBuilderController.Instance.DeckCards)
			{
				if (deckCard.BelongsToTeam(teamType))
				{
					num += deckCard.Available;
					if (num >= MIN_CARD_COUNT_FOR_THEME)
					{
						return true;
					}
				}
			}
			return flag;
		}
		return flag;
	}

	private static bool IsValidFactor(string factorType)
	{
		int num = 0;
		bool flag = false;
		if (DeckBuilderController.Instance.MyCards != null)
		{
			foreach (CardListCard myCard in DeckBuilderController.Instance.MyCards)
			{
				if (myCard.HasFactor(factorType))
				{
					num += myCard.Available;
					if (num >= MIN_CARD_COUNT_FOR_THEME)
					{
						flag = true;
						break;
					}
				}
			}
		}
		if (!flag && DeckBuilderController.Instance.DeckCards != null)
		{
			foreach (CardListCard deckCard in DeckBuilderController.Instance.DeckCards)
			{
				if (deckCard.HasFactor(factorType))
				{
					num += deckCard.Available;
					if (num >= MIN_CARD_COUNT_FOR_THEME)
					{
						return true;
					}
				}
			}
			return flag;
		}
		return flag;
	}

	public override void OnActive()
	{
		CreateWindow();
		if (DeckBuilderController.Instance.MyCards != null)
		{
			PopulateCharacterSelector();
		}
		base.OnActive();
	}

	public void ShowRoot()
	{
		root.Show();
		if (DeckBuilderController.Instance.DeckCards != null)
		{
			deckCardCount = DeckBuilderController.Instance.DeckCards.Count;
		}
		else
		{
			deckCardCount = 0;
		}
		if (ThemeList.Count > 0 || deckCardCount > 0)
		{
			deckSlider.Value = 0f;
			deckSlider.FireChanged();
			middlePanel.IsVisible = true;
			introductionPanel.IsVisible = false;
		}
		else
		{
			middlePanel.IsVisible = false;
			introductionPanel.IsVisible = true;
		}
		largeCard.Show();
		UpdateSliderVisibility();
		AppShell.Instance.EventMgr.AddListener<CardCollectionDataMessage>(OnCardCountChanged);
	}

	public void HideRoot()
	{
		root.Hide();
		largeCard.Hide();
		AppShell.Instance.EventMgr.RemoveListener<CardCollectionDataMessage>(OnCardCountChanged);
	}

	public override void OnShow()
	{
		SHSInput.SetInputBlockingMode(this, SHSInput.InputBlockType.BlockWorld);
		GUIManager.Instance.TogglePersistentGUI(GUIManager.PersistentGUIToggleOption.Visibility, true);
		ShowRoot();
		if (AppShell.Instance.Profile != null && AppShell.Instance.Profile.AvailableBoosterPacks != null && AppShell.Instance.Profile.AvailableBoosterPacks.Count > 0)
		{
			GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, "#DECKBUILDER_OPEN_BOOSTER_PROMPT", delegate(string Id, GUIDialogWindow.DialogState state)
			{
				if (state == GUIDialogWindow.DialogState.Ok)
				{
					OpenAvailableBoosterPacks();
				}
			}, ModalLevelEnum.Default);
		}
		base.OnShow();
	}

	public override void OnHide()
	{
		SHSInput.RevertInputBlockingMode(this);
		GUIManager.Instance.TogglePersistentGUI(GUIManager.PersistentGUIToggleOption.Visibility, false);
		AppShell.Instance.EventMgr.RemoveListener<ShoppingItemPurchasedMessage>(OnCardPurchased);
		base.OnHide();
	}

	public override void Update()
	{
		base.Update();
		if (!IsActive)
		{
			return;
		}
		if (DeckBuilderController.Instance.DeckCards != null && DeckBuilderController.Instance.DeckCards.Count == 0 && ThemeList.Count == 0)
		{
			introductionPanel.IsVisible = true;
			middlePanel.IsVisible = false;
		}
		else
		{
			introductionPanel.IsVisible = false;
			middlePanel.IsVisible = true;
		}
		if (deckMouseOver)
		{
			DeckBuilderController deckBuilderController = (DeckBuilderController)GameController.GetController();
			GameObject gameObject = null;
			gameObject = deckBuilderController.PickCardPanel(deckBuilderController.deckCamera, deckRenderTexture);
			float mouseWheelDelta = SHSInput.GetMouseWheelDelta(middlePanel);
			if (mouseWheelDelta != 0f)
			{
				deckSlider.Value += mouseWheelDelta * (0f - deckScrollSpeed);
			}
			if (!suppressZoomedCard)
			{
				if (gameObject != null)
				{
					zoomComponent.IsMouseOver = true;
					CardProperties cardProperties = gameObject.GetComponent(typeof(CardProperties)) as CardProperties;
					if (cardProperties != null)
					{
						largeCard.Texture = DeckBuilderController.Instance.GetFullSizeTexture(cardProperties.Card);
						largeCard.Alpha = 1f;
						largeCardSet = true;
						if (cardProperties != lastPickedCard)
						{
							ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("hover_over"));
						}
					}
					lastPickedCard = cardProperties;
				}
				else
				{
					zoomComponent.IsMouseOver = false;
				}
			}
			else
			{
				zoomComponent.IsMouseOver = false;
			}
		}
		zoomComponent.Update();
	}

	private void Close()
	{
		DeckBuilderController deckBuilderController = (DeckBuilderController)GameController.GetController();
		if (deckBuilderController != null && deckBuilderController.DeckCards != null && deckBuilderController.DeckCards.Count > 0 && !deckBuilderController.DeckCards.saved)
		{
			GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, "#DECKBUILDER_EXIT_SAVE_DECK_ PROMPT", delegate(string Id, GUIDialogWindow.DialogState state)
			{
				if (state == GUIDialogWindow.DialogState.Ok)
				{
					SHSAutoDeckBuilderWindow sHSAutoDeckBuilderWindow = this;
					ShsEventMgr.GenericDelegate<CardCollectionSavedMessage> t = null;
					t = delegate
					{
						AppShell.Instance.EventMgr.RemoveListener(t);
						sHSAutoDeckBuilderWindow.Hide();
						AppShell.Instance.Transition(GameController.ControllerType.SocialSpace);
					};
					AppShell.Instance.EventMgr.AddListener(t);
					AppShell.Instance.EventMgr.Fire(this, new CardCollectionUIMessage(CardCollectionUIMessage.CCUIEvent.SaveClicked, this));
				}
				else
				{
					Hide();
					AppShell.Instance.Transition(GameController.ControllerType.SocialSpace);
				}
			}, ModalLevelEnum.Default);
			return;
		}
		Hide();
		AppShell.Instance.Transition(GameController.ControllerType.SocialSpace);
	}

	private void OpenCardGadgetWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum windowType)
	{
		DeckBuilderController deckBuilderController = (DeckBuilderController)GameController.GetController();
		if (deckBuilderController != null && deckBuilderController.DeckCards != null && deckBuilderController.DeckCards.Count > 0 && !deckBuilderController.DeckCards.saved)
		{
			GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, "#DECKBUILDER_EXIT_SAVE_DECK_ PROMPT", delegate(string Id, GUIDialogWindow.DialogState state)
			{
				if (state == GUIDialogWindow.DialogState.Ok)
				{
					ShsEventMgr.GenericDelegate<CardCollectionSavedMessage> t = null;
					t = delegate
					{
						AppShell.Instance.EventMgr.RemoveListener(t);
						SHSCardGameGadgetWindow cardWindow3 = new SHSCardGameGadgetWindow(windowType);
						cardWindow3.CloseButton.Click += delegate
						{
							cardWindow3.CloseGadget();
						};
						GUIManager.Instance.ShowDynamicWindow(cardWindow3, ModalLevelEnum.Default);
					};
					AppShell.Instance.EventMgr.AddListener(t);
					AppShell.Instance.EventMgr.Fire(this, new CardCollectionUIMessage(CardCollectionUIMessage.CCUIEvent.SaveClicked, this));
				}
				else
				{
					SHSCardGameGadgetWindow cardWindow2 = new SHSCardGameGadgetWindow(windowType);
					cardWindow2.CloseButton.Click += delegate
					{
						cardWindow2.CloseGadget();
					};
					GUIManager.Instance.ShowDynamicWindow(cardWindow2, ModalLevelEnum.Default);
				}
			}, ModalLevelEnum.Default);
			return;
		}
		SHSCardGameGadgetWindow cardWindow = new SHSCardGameGadgetWindow(windowType);
		cardWindow.CloseButton.Click += delegate
		{
			cardWindow.CloseGadget();
		};
		GUIManager.Instance.ShowDynamicWindow(cardWindow, ModalLevelEnum.Default);
	}

	private void OpenAvailableBoosterPacks()
	{
		if (AppShell.Instance.Profile != null && AppShell.Instance.Profile.AvailableBoosterPacks != null && AppShell.Instance.Profile.AvailableBoosterPacks.Count > 0)
		{
			foreach (AvailableBoosterPack value in AppShell.Instance.Profile.AvailableBoosterPacks.Values)
			{
				int num = int.Parse(value.BoosterPackId);
				OwnableDefinition def = OwnableDefinition.getDef(num);
				if (def != null)
				{
					SHSBoosterPackOpeningWindow dialogWindow = new SHSBoosterPackOpeningWindow(num, def.name, delegate
					{
						AppShell.Instance.Profile.StartBoosterPacksFetch(delegate
						{
							DeckBuilderController deckBuilderController = (DeckBuilderController)GameController.GetController();
							deckBuilderController.FetchCardCollection();
							OpenAvailableBoosterPacks();
						});
					});
					GUIManager.Instance.ShowDynamicWindow(dialogWindow, ModalLevelEnum.Full);
					break;
				}
			}
		}
	}

	protected void OnCardPurchased(ShoppingItemPurchasedMessage message)
	{
	}

	private bool AddTheme(ThemeDatum theme)
	{
		return AddTheme(theme.themeType, theme.iconId, theme.name);
	}

	private bool AddTheme(DeckThemeType themeType, string iconId, string name)
	{
		introductionPanel.IsVisible = false;
		middlePanel.IsVisible = true;
		int count = ThemeList.Count;
		if (ThemeList.ContainsKey(name) || count >= 6)
		{
			return false;
		}
		string textureSource = string.Empty;
		Vector2 size = Vector2.zero;
		switch (themeType)
		{
		case DeckThemeType.Factor:
			textureSource = "deckbuilder_bundle|deck_theme_factor_" + iconId;
			size = new Vector2(50f, 50f);
			break;
		case DeckThemeType.Team:
			textureSource = "deckbuilder_bundle|deck_theme_team_" + iconId;
			size = new Vector2(50f, 50f);
			break;
		case DeckThemeType.Hero:
			textureSource = "characters_bundle|inventory_character_" + iconId + "_normal";
			size = new Vector2(64f, 64f);
			break;
		default:
			CspUtils.DebugLog("Tried to add an unknown theme");
			break;
		}
		GUIAnimatedButton themeToggle = new GUIAnimatedButton();
		themeToggle.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, Vector2.zero, size, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		themeToggle.TextureSource = textureSource;
		themeToggle.SetupButton(1f, 1.25f, 0.9f);
		themeToggle.Tag = name;
		themeToggle.Click += delegate
		{
			ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("click_up"));
			RemoveTheme(name);
			themeToggle.Hide();
		};
		middlePanel.Add(themeToggle);
		ThemeButtonList.Add(themeToggle);
		UpdateThemeButtons();
		DeckTheme deckTheme = DeckBuilderConfigDefinition.Instance.FindTheme(name);
		if (deckTheme == null)
		{
			deckTheme = new DeckTheme();
			deckTheme.name = name;
			switch (themeType)
			{
			case DeckThemeType.Factor:
				deckTheme.affinity.Add("factor" + iconId, 50);
				break;
			case DeckThemeType.Team:
				deckTheme.affinity.Add("team" + iconId, 50);
				break;
			case DeckThemeType.Hero:
			{
				string characterFamily = AppShell.Instance.CharacterDescriptionManager[iconId].CharacterFamily;
				characterFamily = characterFamily.Replace(' ', '_');
				deckTheme.affinity.Add("hero" + characterFamily, 100);
				break;
			}
			}
		}
		ThemeList.Add(name, deckTheme);
		if (buildADeckButton != null)
		{
			buildADeckButton.ToolTip = new NamedToolTipInfo("#TT_AUTODECK_BUILD_DECK_WITH_THEMES", new Vector2(20f, 200f));
		}
		return true;
	}

	private void RemoveTheme(string themeName)
	{
		if (ThemeList.ContainsKey(themeName))
		{
			ThemeList.Remove(themeName);
			foreach (GUIAnimatedButton themeButton in ThemeButtonList)
			{
				if (themeButton.Tag.ToString() == themeName)
				{
					themeButton.Hide();
					ThemeButtonList.Remove(themeButton);
					middlePanel.Remove(themeButton);
					break;
				}
			}
			UpdateThemeButtons();
		}
	}

	private void ClearThemeList()
	{
		ThemeList.Clear();
		foreach (GUIAnimatedButton themeButton in ThemeButtonList)
		{
			themeButton.Hide();
			middlePanel.Remove(themeButton);
		}
		ThemeButtonList.Clear();
	}

	private void UpdateThemeButtons()
	{
		int num = 0;
		foreach (GUIAnimatedButton themeButton in ThemeButtonList)
		{
			themeButton.SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(125 + num * 50, -208f));
			num++;
		}
	}

	private void BuilAutoDeck()
	{
		introductionPanel.IsVisible = false;
		middlePanel.IsVisible = true;
		DeckBuilderController.Instance.DoNewDeck();
		DeckTheme deckTheme = new DeckTheme();
		if (ThemeList.Count == 0)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < ThemeData.Count; i++)
			{
				ThemeDatum themeDatum = ThemeData[i];
				bool flag = false;
				if (TeamToggleButtons.ContainsKey(themeDatum.iconId))
				{
					flag = TeamToggleButtons[themeDatum.iconId].IsEnabled;
				}
				else if (FactorToggleButtons.ContainsKey(themeDatum.iconId))
				{
					flag = FactorToggleButtons[themeDatum.iconId].IsEnabled;
				}
				if (flag)
				{
					list.Add(i);
				}
			}
			if (list.Count <= 0)
			{
				CspUtils.DebugLog("No valid themes to be able to build a deck from.");
				return;
			}
			CspUtils.DebugLog("Randomly selecting a deck theme since none was chosen.");
			int index = UnityEngine.Random.Range(0, list.Count);
			ThemeDatum theme = ThemeData[list[index]];
			AddTheme(theme);
		}
		foreach (KeyValuePair<string, DeckTheme> theme2 in ThemeList)
		{
			foreach (KeyValuePair<string, int> item in theme2.Value.affinity)
			{
				deckTheme.affinity.Add(item);
			}
			if (!string.IsNullOrEmpty(theme2.Value.decklist))
			{
				deckTheme.decklist += theme2.Value.decklist;
			}
		}
		CspUtils.DebugLog("Building a deck with the following data...");
		foreach (KeyValuePair<string, int> item2 in deckTheme.affinity)
		{
			CspUtils.DebugLog(string.Format("affinity - {0}:{1}", item2.Value, item2.Key));
		}
		CspUtils.DebugLog(string.Format("decklist - {0}", deckTheme.decklist));
		buildADeckButton.IsEnabled = false;
		AppShell.Instance.StartCoroutine(DeckBuilderController.Instance.BuildAutoDeck(deckTheme));
	}

	private void OnCardCountChanged(CardCollectionDataMessage msg)
	{
		buildADeckButton.IsEnabled = true;
		if (msg.Event == CardCollectionDataMessage.CCDataEvent.DeckCountChanged)
		{
			if (msg.ScrollToTop)
			{
				deckSlider.Value = 0f;
				deckSlider.FireChanged();
			}
			else if (msg.CardTypeCount > deckCardCount)
			{
				deckSlider.Value = 100f;
				deckSlider.FireChanged();
			}
			deckCardCount = msg.CardTypeCount;
			UpdateSliderVisibility();
			if (deckCardCount > 0)
			{
				myCardsLabel.Text = "#DECKBUILDER_AUTO_EDIT_CARDS";
			}
			else
			{
				myCardsLabel.Text = "#DECKBUILDER_AUTO_MY_CARDS";
			}
		}
		else if (msg.Event == CardCollectionDataMessage.CCDataEvent.CollectionCountChanged)
		{
			if (TeamToggleButtons != null)
			{
				foreach (KeyValuePair<string, GUIAnimatedButton> teamToggleButton in TeamToggleButtons)
				{
					teamToggleButton.Value.IsEnabled = IsValidTeam(teamToggleButton.Key);
				}
			}
			if (FactorToggleButtons != null)
			{
				foreach (KeyValuePair<string, GUIAnimatedButton> factorToggleButton in FactorToggleButtons)
				{
					factorToggleButton.Value.IsEnabled = IsValidFactor(factorToggleButton.Key);
				}
			}
		}
	}

	private void OnReturnKeypress(SHSKeyCode code)
	{
	}

	private void PopulateCharacterSelector()
	{
		List<SHSCharacterSelect.CharacterItem> heroListFromCards = GetHeroListFromCards();
		characterSelection.AddList(heroListFromCards);
		characterSelection.SortItemList();
		foreach (SHSCharacterSelect.CharacterItem item in heroListFromCards)
		{
			string text = AppShell.Instance.CharacterDescriptionManager[item.Name].CharacterFamily.ToLower();
			text = text.Replace(' ', '_');
			ThemeDatum td = new ThemeDatum(DeckThemeType.Hero, item.Name, text);
			ThemeData.Add(td);
			item.HeroClicked += delegate
			{
				AddTheme(td);
			};
		}
		characterSelection.Slider.FireChanged();
	}

	private void UpdateSliderVisibility()
	{
		if (deckCardCount > 8)
		{
			deckSlider.Alpha = 1f;
			deckScrollSpeed = 1f / (float)(deckCardCount / 4) * 250f;
			deckScrollSpeed += deckScrollSpeed * 0.2f;
		}
		else
		{
			deckSlider.Alpha = 0f;
			deckSlider.Value = 0f;
			deckSlider.FireChanged();
		}
	}

	private List<SHSCharacterSelect.CharacterItem> GetHeroListFromCards()
	{
		List<SHSCharacterSelect.CharacterItem> list = new List<SHSCharacterSelect.CharacterItem>();
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (CardListCard myCard in DeckBuilderController.Instance.MyCards)
		{
			string[] internalHeroName = myCard.InternalHeroName;
			foreach (string characterKey in internalHeroName)
			{
				string characterFamilyInternal = AppShell.Instance.CharacterDescriptionManager[characterKey].CharacterFamilyInternal;
				if (dictionary.ContainsKey(characterFamilyInternal))
				{
					Dictionary<string, int> dictionary2;
					Dictionary<string, int> dictionary3 = dictionary2 = dictionary;
					string key;
					string key2 = key = characterFamilyInternal;
					int num = dictionary2[key];
					dictionary3[key2] = num + myCard.Available;
				}
				else
				{
					dictionary.Add(characterFamilyInternal, 1);
				}
			}
		}
		foreach (KeyValuePair<string, int> item in dictionary)
		{
			if (item.Value >= 4)
			{
				HeroPersisted hero = new HeroPersisted(item.Key);
				list.Add(new SHSCharacterSelect.CharacterItem(hero, (SHSCharacterSelect.HeroClickedDelegate)null, SHSSelectionItem<SHSItemLoadingWindow>.SelectionState.Active));
			}
		}
		return list;
	}

	public override void ConfigureKeyBanks()
	{
		base.ConfigureKeyBanks();
		keyBanks[KeyInputState.Active].AddKey(new KeyCodeEntry(KeyCode.Return, false, false, false), OnReturnKeypress);
	}

	public void CaptureHandlerGotInput(ICaptureHandler handler)
	{
	}
}
