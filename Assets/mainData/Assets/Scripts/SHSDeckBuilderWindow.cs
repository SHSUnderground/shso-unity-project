using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSDeckBuilderWindow : GUITopLevelWindow
{
	private struct SortMethodInfo
	{
		public string label;

		public string toolTip;

		public SortMethodInfo(string label, string toolTip)
		{
			this.label = label;
			this.toolTip = toolTip;
		}
	}

	private const int NUM_LEVELS = 10;

	public const int MAX_DECK_COUNT = 40;

	protected ShsEventMgr eventMgr;

	private GUIImage collectionRenderTexture;

	private GUIImage deckRenderTexture;

	private GUIDrawTexture largeCard;

	private GUISimpleControlWindow leftPane;

	private GUISimpleControlWindow midTopPane;

	private GUISimpleControlWindow midBottomPane;

	private GUISimpleControlWindow rightPane;

	private GUISimpleControlWindow collectionPane;

	private GUISimpleControlWindow largeCardPane;

	private GUIRadioButtonList sortRadioButtons;

	private GUISlider collectionSlider;

	private GUISlider deckSlider;

	private GUIStrokeTextLabel deckCounterLabel;

	private GUIStrokeTextLabel collectionCounterLabel;

	private GUITextField searchField;

	private GUIDropShadowTextLabel collectionScoreValueLabel;

	private GUIVerticalBar[] levelBars = new GUIVerticalBar[10];

	private GUIDeckStatBar attackFactorBar;

	private GUIDeckStatBar blockFactorBar;

	private List<GUIToggleButton> SortToggleButtons;

	private List<GUIButton> FactorToggleButtons;

	private List<GUIButton> BlockToggleButtons;

	private List<GUIButton> TeamToggleButtons;

	protected CardProperties lastPickedCard;

	protected bool largeCardSet;

	private bool collectionMouseOver;

	private bool deckMouseOver;

	private float deckScrollSpeed;

	private float collectionScrollSpeed;

	private int deckCardCount;

	private int collectionCardCount;

	private CardZoomComponent zoomComponent;

	private static readonly Vector2 VIEW_CARD_SIZE = new Vector2(244f, 340f);

	private static readonly Vector2 ZOOM_CARD_SIZE = new Vector2(366f, 512f);

	private static readonly Vector2 VIEW_CARD_OFFSET = new Vector2(0f, 0f);

	private static readonly Vector2 ZOOM_CARD_OFFSET = new Vector2(0f, 0f);

	private Dictionary<SortedCardList.SortMethod, SortMethodInfo> sortMethodsToLabels;

	private Dictionary<string, List<Vector2>> FactorOffsets;

	private Dictionary<string, List<Vector2>> BlockOffsets;

	private Dictionary<string, List<Vector2>> TeamOffsets;

	public SHSDeckBuilderWindow()
		: base("SHSDeckBuilderWindow")
	{
		Traits.ResourceLoadingPhaseTrait = ControlTraits.ResourceLoadingPhaseTraitEnum.Active;
		Traits.VisibleAncestryTrait = ControlTraits.VisibilityAncestryTraitEnum.DetachedVisibility;
		SetSize(1022f, 700f);
		SetPosition(QuickSizingHint.Centered);
		Offset = new Vector2(0f, 0f);
	}

	protected override void InitializeBundleList()
	{
		base.InitializeBundleList();
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("cardgame_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("brawler_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("deckbuilder_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
	}

	public override bool InitializeResources(bool reload)
	{
		InitializeWindow();
		return base.InitializeResources(reload);
	}

	private void InitializeWindow()
	{
		FontInfo = new GUIFontInfo(GUIFontManager.SupportedFontEnum.Komica, 16);
		FactorOffsets = new Dictionary<string, List<Vector2>>();
		Dictionary<string, List<Vector2>> factorOffsets = FactorOffsets;
		List<Vector2> list = new List<Vector2>();
		list.Add(new Vector2(39.31f, -76f));
		list.Add(new Vector2(17f, 0f));
		factorOffsets["Animal"] = list;
		Dictionary<string, List<Vector2>> factorOffsets2 = FactorOffsets;
		list = new List<Vector2>();
		list.Add(new Vector2(77.33f, -76f));
		list.Add(new Vector2(5f, 0f));
		factorOffsets2["Strength"] = list;
		Dictionary<string, List<Vector2>> factorOffsets3 = FactorOffsets;
		list = new List<Vector2>();
		list.Add(new Vector2(115f, -76f));
		list.Add(new Vector2(10f, 0f));
		factorOffsets3["Elemental"] = list;
		Dictionary<string, List<Vector2>> factorOffsets4 = FactorOffsets;
		list = new List<Vector2>();
		list.Add(new Vector2(147.7f, -77.6f));
		list.Add(new Vector2(10f, 0f));
		factorOffsets4["Energy"] = list;
		Dictionary<string, List<Vector2>> factorOffsets5 = FactorOffsets;
		list = new List<Vector2>();
		list.Add(new Vector2(187.46f, -76.5f));
		list.Add(new Vector2(10f, 0f));
		factorOffsets5["Tech"] = list;
		Dictionary<string, List<Vector2>> factorOffsets6 = FactorOffsets;
		list = new List<Vector2>();
		list.Add(new Vector2(224f, -76f));
		list.Add(new Vector2(10f, 0f));
		factorOffsets6["Speed"] = list;
		BlockOffsets = new Dictionary<string, List<Vector2>>();
		Dictionary<string, List<Vector2>> blockOffsets = BlockOffsets;
		list = new List<Vector2>();
		list.Add(new Vector2(36f, -16f));
		list.Add(new Vector2(8f, 0f));
		blockOffsets["Animal"] = list;
		Dictionary<string, List<Vector2>> blockOffsets2 = BlockOffsets;
		list = new List<Vector2>();
		list.Add(new Vector2(73f, -16f));
		list.Add(new Vector2(35f, 0f));
		blockOffsets2["Strength"] = list;
		Dictionary<string, List<Vector2>> blockOffsets3 = BlockOffsets;
		list = new List<Vector2>();
		list.Add(new Vector2(111.8f, -16f));
		list.Add(new Vector2(65f, 0f));
		blockOffsets3["Elemental"] = list;
		Dictionary<string, List<Vector2>> blockOffsets4 = BlockOffsets;
		list = new List<Vector2>();
		list.Add(new Vector2(149.5f, -16f));
		list.Add(new Vector2(52f, 0f));
		blockOffsets4["Energy"] = list;
		Dictionary<string, List<Vector2>> blockOffsets5 = BlockOffsets;
		list = new List<Vector2>();
		list.Add(new Vector2(187.7f, -16f));
		list.Add(new Vector2(59f, 0f));
		blockOffsets5["Tech"] = list;
		Dictionary<string, List<Vector2>> blockOffsets6 = BlockOffsets;
		list = new List<Vector2>();
		list.Add(new Vector2(226f, -16f));
		list.Add(new Vector2(56f, 0f));
		blockOffsets6["Speed"] = list;
		TeamOffsets = new Dictionary<string, List<Vector2>>();
		Dictionary<string, List<Vector2>> teamOffsets = TeamOffsets;
		list = new List<Vector2>();
		list.Add(new Vector2(37f, 49f));
		list.Add(new Vector2(8f, 0f));
		teamOffsets["F4"] = list;
		Dictionary<string, List<Vector2>> teamOffsets2 = TeamOffsets;
		list = new List<Vector2>();
		list.Add(new Vector2(72.7f, 48.49f));
		list.Add(new Vector2(18f, 0f));
		teamOffsets2["Avengers"] = list;
		Dictionary<string, List<Vector2>> teamOffsets3 = TeamOffsets;
		list = new List<Vector2>();
		list.Add(new Vector2(111.47f, 49f));
		list.Add(new Vector2(33f, 0f));
		teamOffsets3["SpideyFriends"] = list;
		Dictionary<string, List<Vector2>> teamOffsets4 = TeamOffsets;
		list = new List<Vector2>();
		list.Add(new Vector2(149.2f, 49f));
		list.Add(new Vector2(8f, 0f));
		teamOffsets4["Xmen"] = list;
		Dictionary<string, List<Vector2>> teamOffsets5 = TeamOffsets;
		list = new List<Vector2>();
		list.Add(new Vector2(188.2f, 49f));
		list.Add(new Vector2(43f, 0f));
		teamOffsets5["SS"] = list;
		Dictionary<string, List<Vector2>> teamOffsets6 = TeamOffsets;
		list = new List<Vector2>();
		list.Add(new Vector2(224.8f, 49f));
		list.Add(new Vector2(75f, 0f));
		teamOffsets6["Brotherhood"] = list;
		sortMethodsToLabels = new Dictionary<SortedCardList.SortMethod, SortMethodInfo>();
		sortMethodsToLabels[SortedCardList.SortMethod.CardName] = new SortMethodInfo("#DECKBUILDER_NAME", "#TT_DECKBUILDER_SORT_NAME");
		sortMethodsToLabels[SortedCardList.SortMethod.HeroName] = new SortMethodInfo("#DECKBUILDER_HERO", "#TT_DECKBUILDER_SORT_HERO");
		sortMethodsToLabels[SortedCardList.SortMethod.Level] = new SortMethodInfo("#DECKBUILDER_LEVEL", "#TT_DECKBUILDER_SORT_LEVEL");
		sortMethodsToLabels[SortedCardList.SortMethod.Factor] = new SortMethodInfo("#DECKBUILDER_FACTOR", "#TT_DECKBUILDER_SORT_FACTOR");
		sortMethodsToLabels[SortedCardList.SortMethod.Block] = new SortMethodInfo("#DECKBUILDER_BLOCK", "#TT_DECKBUILDER_SORT_BLOCK");
		sortMethodsToLabels[SortedCardList.SortMethod.Effects] = new SortMethodInfo("#DECKBUILDER_EFFECTS", "#TT_DECKBUILDER_SORT_EFFECTS");
		DeckBuilderController controller = (DeckBuilderController)GameController.GetController();
		leftPane = new GUISimpleControlWindow();
		leftPane.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(-240f, 0f), new Vector2(269f, 637f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Add(leftPane);
		attackFactorBar = new GUIDeckStatBar();
		attackFactorBar.SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(21f, -77f), new Vector2(210f, 21f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		leftPane.Add(attackFactorBar);
		blockFactorBar = new GUIDeckStatBar();
		blockFactorBar.SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(21f, -27f), new Vector2(210f, 21f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		leftPane.Add(blockFactorBar);
		GUIImage gUIImage = new GUIImage();
		gUIImage.TextureSource = "deckbuilder_bundle|deckbuilder_left_panel";
		gUIImage.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, leftPane.Size);
		leftPane.Add(gUIImage);
		GUIStrokeTextLabel gUIStrokeTextLabel = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(200f, 35f), new Vector2(40f, 24f));
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 25, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 114, 255), GUILabel.GenColor(22, 77, 195), new Vector2(4f, 3f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel.Text = "#DECKBUILDER_SORT_BY";
		leftPane.Add(gUIStrokeTextLabel);
		sortRadioButtons = new GUIRadioButtonList();
		sortRadioButtons.SetPositionAndSize(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-8f, 82f), new Vector2(210f, 92f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		leftPane.Add(sortRadioButtons);
		SortToggleButtons = new List<GUIToggleButton>();
		int num = 3;
		int num2 = 2;
		int num3 = 0;
		foreach (KeyValuePair<SortedCardList.SortMethod, SortMethodInfo> sortMethodsToLabel in sortMethodsToLabels)
		{
			int num4 = (int)Math.Floor((float)num3 / (float)(num2 + 1));
			int num5 = num3 % num;
			GUIToggleButton sortToggle = new GUIToggleButton();
			GUILabel label = sortToggle.Label;
			SortMethodInfo value = sortMethodsToLabel.Value;
			label.Text = value.label;
			sortToggle.Tag = sortMethodsToLabel.Key;
			sortToggle.Label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(65, 112, 160), TextAnchor.MiddleLeft);
			sortToggle.Spacing = 40f;
			GUIToggleButton gUIToggleButton = sortToggle;
			SortMethodInfo value2 = sortMethodsToLabel.Value;
			gUIToggleButton.ToolTip = new NamedToolTipInfo(value2.toolTip);
			sortToggle.ButtonStyleInfo = new SHSButtonStyleInfo("deckbuilder_bundle|deckbuilder_filter");
			sortToggle.SetButtonSize(new Vector2(100f, 31f));
			sortToggle.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(num4 * 105, num5 * 31), new Vector2(99f, 29f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			sortToggle.Changed += delegate(GUIControl sender, GUIChangedEvent eventArgs)
			{
				if (eventArgs.NewValue == 1f)
				{
					sortToggle.Label.Bold = true;
					sortToggle.Label.FontSize = 15;
					Vector2 size = sortToggle.Label.Style.UnityStyle.CalcSize(new GUIContent(sortToggle.Text));
					sortToggle.Label.SetSize(size);
					AppShell.Instance.EventMgr.Fire(sender, new CardCollectionUIMessage(CardCollectionUIMessage.CCUIEvent.SortTypeSelected, sortToggle));
				}
				else
				{
					sortToggle.Label.Bold = false;
					sortToggle.Label.FontSize = 14;
				}
			};
			SortToggleButtons.Add(sortToggle);
			sortRadioButtons.AddButton(sortToggle);
			num3++;
		}
		GUISimpleControlWindow gUISimpleControlWindow = new GUISimpleControlWindow();
		gUISimpleControlWindow.SetPositionAndSize(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(7f, 175f), new Vector2(245f, 60f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		leftPane.Add(gUISimpleControlWindow);
		searchField = new GUITextField();
		searchField.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(15f, 2f), new Vector2(160f, 30f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		searchField.FontFace = GUIFontManager.SupportedFontEnum.Komica;
		searchField.FontSize = 14;
		searchField.TextColor = GUILabel.GenColor(65, 112, 160);
		searchField.Text = string.Empty;
		searchField.WordWrap = false;
		searchField.TextAlignment = TextAnchor.MiddleLeft;
		searchField.ToolTip = new NamedToolTipInfo("#TT_DECKBUILDER_SEARCH");
		searchField.GotFocus += delegate
		{
			searchField.Text = string.Empty;
			SearchCardText(searchField.Text);
		};
		searchField.Changed += delegate
		{
			SearchCardText(searchField.Text);
		};
		gUISimpleControlWindow.Add(searchField);
		FactorToggleButtons = new List<GUIButton>();
		BlockToggleButtons = new List<GUIButton>();
		TeamToggleButtons = new List<GUIButton>();
		int num6 = 0;
		foreach (KeyValuePair<char, string> item in DeckBuilderController.Instance.FactorLookup)
		{
			char key = item.Key;
			string value3 = item.Value;
			GUIButton gUIButton = new GUIButton();
			gUIButton.StyleInfo = new SHSButtonNoHighlightStyleInfo("deckbuilder_bundle|show_factors_" + value3.ToLower());
			gUIButton.Id = "AttackToggle_" + value3;
			gUIButton.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, FactorOffsets[value3][0], new Vector2(64f, 64f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIButton.HitTestType = HitTestTypeEnum.Alpha;
			gUIButton.ToolTip = new NamedToolTipInfo("#TT_CARDGAME_FACTOR_" + key, FactorOffsets[value3][1]);
			gUIButton.ToolTip.VerticalAlignmentOverride = SHSTooltip.ToolTipVerticalAlignment.Bottom;
			gUIButton.ToolTip.HorizontalAlignmentOverride = SHSTooltip.ToolTipHorizontalAlignment.Right;
			CardFilterType attackFilter = (CardFilterType)(int)Enum.Parse(typeof(CardFilterType), "Factor" + value3);
			gUIButton.IsSelected = controller.IsFilterActive(attackFilter);
			gUIButton.Click += delegate(GUIControl sender, GUIClickEvent eventArgs)
			{
				((GUIButton)sender).IsSelected = !((GUIButton)sender).IsSelected;
				AppShell.Instance.EventMgr.Fire(sender, new CardCollectionFilterUIMessage(sender, attackFilter));
			};
			AddMouseoverAudio(gUIButton);
			FactorToggleButtons.Add(gUIButton);
			leftPane.Add(gUIButton);
			GUIButton gUIButton2 = new GUIButton();
			gUIButton2.StyleInfo = new SHSButtonNoHighlightStyleInfo("deckbuilder_bundle|show_blocks_" + value3.ToLower());
			gUIButton2.Id = "BlockToggle_" + value3;
			gUIButton2.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, BlockOffsets[value3][0], new Vector2(64f, 64f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIButton2.HitTestType = HitTestTypeEnum.Alpha;
			gUIButton2.ToolTip = new NamedToolTipInfo("#TT_DECKBUILDER_BLOCK_" + key, BlockOffsets[value3][1]);
			gUIButton2.ToolTip.VerticalAlignmentOverride = SHSTooltip.ToolTipVerticalAlignment.Bottom;
			gUIButton2.ToolTip.HorizontalAlignmentOverride = SHSTooltip.ToolTipHorizontalAlignment.Right;
			CardFilterType blockFilter = (CardFilterType)(int)Enum.Parse(typeof(CardFilterType), "Block" + value3);
			gUIButton2.IsSelected = controller.IsFilterActive(blockFilter);
			gUIButton2.Click += delegate(GUIControl sender, GUIClickEvent eventArgs)
			{
				((GUIButton)sender).IsSelected = !((GUIButton)sender).IsSelected;
				AppShell.Instance.EventMgr.Fire(sender, new CardCollectionFilterUIMessage(sender, blockFilter));
			};
			AddMouseoverAudio(gUIButton2);
			BlockToggleButtons.Add(gUIButton2);
			leftPane.Add(gUIButton2);
			num6++;
		}
		foreach (KeyValuePair<char, string> item2 in DeckBuilderController.Instance.TeamLookup)
		{
			char key2 = item2.Key;
			string value4 = item2.Value;
			GUIButton gUIButton3 = new GUIButton();
			gUIButton3.StyleInfo = new SHSButtonNoHighlightStyleInfo("deckbuilder_bundle|show_teams_" + value4);
			gUIButton3.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, TeamOffsets[value4][0], new Vector2(64f, 64f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIButton3.HitTestType = HitTestTypeEnum.Alpha;
			gUIButton3.ToolTip = new NamedToolTipInfo("#TT_CARDGAME_TEAM_" + key2, TeamOffsets[value4][1]);
			gUIButton3.ToolTip.VerticalAlignmentOverride = SHSTooltip.ToolTipVerticalAlignment.Bottom;
			gUIButton3.ToolTip.HorizontalAlignmentOverride = SHSTooltip.ToolTipHorizontalAlignment.Right;
			CardFilterType teamFilter = (CardFilterType)(int)Enum.Parse(typeof(CardFilterType), "Team" + value4);
			gUIButton3.IsSelected = controller.IsFilterActive(teamFilter);
			gUIButton3.Click += delegate(GUIControl sender, GUIClickEvent eventArgs)
			{
				((GUIButton)sender).IsSelected = !((GUIButton)sender).IsSelected;
				AppShell.Instance.EventMgr.Fire(sender, new CardCollectionFilterUIMessage(sender, teamFilter));
			};
			AddMouseoverAudio(gUIButton3);
			TeamToggleButtons.Add(gUIButton3);
			leftPane.Add(gUIButton3);
		}
		GUIStrokeTextLabel gUIStrokeTextLabel2 = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(200f, 35f), new Vector2(-10f, 212f));
		gUIStrokeTextLabel2.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(98, 96, 50), GUILabel.GenColor(55, 53, 15), new Vector2(1f, 2f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel2.Text = "#DECKBUILDER_SHOW_FACTORS";
		gUIStrokeTextLabel2.Bold = true;
		leftPane.Add(gUIStrokeTextLabel2);
		GUIStrokeTextLabel gUIStrokeTextLabel3 = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(200f, 35f), new Vector2(-10f, 272f));
		gUIStrokeTextLabel3.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(98, 96, 50), GUILabel.GenColor(55, 53, 15), new Vector2(1f, 2f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel3.Text = "#DECKBUILDER_SHOW_BLOCKS";
		gUIStrokeTextLabel3.Bold = true;
		leftPane.Add(gUIStrokeTextLabel3);
		GUIStrokeTextLabel gUIStrokeTextLabel4 = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(200f, 35f), new Vector2(-10f, 338f));
		gUIStrokeTextLabel4.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(98, 96, 50), GUILabel.GenColor(55, 53, 15), new Vector2(1f, 2f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel4.Text = "#DECKBUILDER_SHOW_TEAMS";
		gUIStrokeTextLabel4.Bold = true;
		leftPane.Add(gUIStrokeTextLabel4);
		GUIStrokeTextLabel gUIStrokeTextLabel5 = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(200f, 35f), new Vector2(35f, 419f));
		gUIStrokeTextLabel5.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 25, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 114, 255), GUILabel.GenColor(22, 77, 195), new Vector2(4f, 3f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel5.Text = "#DECKBUILDER_DECK_STATS";
		leftPane.Add(gUIStrokeTextLabel5);
		GUIStrokeTextLabel gUIStrokeTextLabel6 = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(200f, 35f), new Vector2(-10f, 433f));
		gUIStrokeTextLabel6.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(98, 96, 50), GUILabel.GenColor(55, 53, 15), new Vector2(1f, 2f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel6.Text = "#DECKBUILDER_LEVELS";
		gUIStrokeTextLabel6.ToolTip = new NamedToolTipInfo("#TT_DECKBUILDER_DECK_STATS_LEVELS", new Vector2(53f, -8f));
		gUIStrokeTextLabel6.ToolTip.VerticalAlignmentOverride = SHSTooltip.ToolTipVerticalAlignment.Bottom;
		gUIStrokeTextLabel6.ToolTip.HorizontalAlignmentOverride = SHSTooltip.ToolTipHorizontalAlignment.Right;
		gUIStrokeTextLabel6.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		gUIStrokeTextLabel6.Bold = true;
		leftPane.Add(gUIStrokeTextLabel6);
		for (int i = 0; i < 10; i++)
		{
			GUIVerticalBar gUIVerticalBar = new GUIVerticalBar();
			gUIVerticalBar.SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(i * 21 + 31, -145f), new Vector2(20f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIVerticalBar.Percent = 0f;
			gUIVerticalBar.IsVisible = false;
			gUIVerticalBar.Id = "LevelBar_" + i;
			gUIVerticalBar.Text = "0";
			leftPane.Add(gUIVerticalBar);
			levelBars[i] = gUIVerticalBar;
			GUIStrokeTextLabel gUIStrokeTextLabel7 = new GUIStrokeTextLabel();
			gUIStrokeTextLabel7.SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(i * 21 + 13, -110f), new Vector2(30f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIStrokeTextLabel7.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 14, GUILabel.GenColor(255, 246, 172), GUILabel.GenColor(182, 125, 14), GUILabel.GenColor(156, 101, 17), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
			gUIStrokeTextLabel7.Id = "Level_" + i;
			if (i < 9)
			{
				gUIStrokeTextLabel7.Text = (i + 1).ToString();
			}
			else
			{
				Vector2 offset = gUIStrokeTextLabel7.Offset;
				float x = offset.x + 7f;
				Vector2 offset2 = gUIStrokeTextLabel7.Offset;
				gUIStrokeTextLabel7.SetPosition(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(x, offset2.y));
				gUIStrokeTextLabel7.Text = (i + 1).ToString() + "+";
			}
			leftPane.Add(gUIStrokeTextLabel7);
		}
		GUIStrokeTextLabel gUIStrokeTextLabel8 = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(200f, 35f), new Vector2(-10f, 525f));
		gUIStrokeTextLabel8.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(98, 96, 50), GUILabel.GenColor(55, 53, 15), new Vector2(1f, 2f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel8.Text = "#DECKBUILDER_FACTORS";
		gUIStrokeTextLabel8.ToolTip = new NamedToolTipInfo("#TT_DECKBUILDER_DECK_STATS_FACTOR", new Vector2(65f, -10f));
		gUIStrokeTextLabel8.ToolTip.VerticalAlignmentOverride = SHSTooltip.ToolTipVerticalAlignment.Bottom;
		gUIStrokeTextLabel8.ToolTip.HorizontalAlignmentOverride = SHSTooltip.ToolTipHorizontalAlignment.Right;
		gUIStrokeTextLabel8.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		gUIStrokeTextLabel8.Bold = true;
		leftPane.Add(gUIStrokeTextLabel8);
		GUIStrokeTextLabel gUIStrokeTextLabel9 = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(200f, 35f), new Vector2(-10f, 575f));
		gUIStrokeTextLabel9.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(98, 96, 50), GUILabel.GenColor(55, 53, 15), new Vector2(1f, 2f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel9.Text = "#DECKBUILDER_BLOCKS";
		gUIStrokeTextLabel9.ToolTip = new NamedToolTipInfo("#TT_DECKBUILDER_DECK_STATS_BLOCKS", new Vector2(58f, -10f));
		gUIStrokeTextLabel9.ToolTip.VerticalAlignmentOverride = SHSTooltip.ToolTipVerticalAlignment.Bottom;
		gUIStrokeTextLabel9.ToolTip.HorizontalAlignmentOverride = SHSTooltip.ToolTipHorizontalAlignment.Right;
		gUIStrokeTextLabel9.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		gUIStrokeTextLabel9.Bold = true;
		leftPane.Add(gUIStrokeTextLabel9);
		midTopPane = new GUISimpleControlWindow();
		midTopPane.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(-2f, -2f), new Vector2(482f, 319f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Add(midTopPane);
		gUIImage = new GUIImage();
		gUIImage.TextureSource = "deckbuilder_bundle|deckbuilder_current_deck_panel";
		gUIImage.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, midTopPane.Size);
		midTopPane.Add(gUIImage);
		GUIStrokeTextLabel gUIStrokeTextLabel10 = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(200f, 35f), new Vector2(-105f, 25f));
		gUIStrokeTextLabel10.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 30, GUILabel.GenColor(236, 250, 255), GUILabel.GenColor(31, 75, 174), GUILabel.GenColor(31, 70, 155), new Vector2(4f, 3f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel10.Text = "#DECKBUILDER_CURRENT_DECK";
		gUIStrokeTextLabel10.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		gUIStrokeTextLabel10.ToolTip = new NamedToolTipInfo("#TT_DECKBUILDER_DECK_TITLE", new Vector2(32f, 0f));
		gUIStrokeTextLabel10.ToolTip.IgnoreCursor = true;
		gUIStrokeTextLabel10.ToolTip.VerticalAlignmentOverride = SHSTooltip.ToolTipVerticalAlignment.Top;
		gUIStrokeTextLabel10.ToolTip.HorizontalAlignmentOverride = SHSTooltip.ToolTipHorizontalAlignment.Right;
		midTopPane.Add(gUIStrokeTextLabel10);
		GUIStrokeTextLabel gUIStrokeTextLabel11 = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(200f, 19f), new Vector2(230f, 22f));
		gUIStrokeTextLabel11.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(51, 116, 183), GUILabel.GenColor(20, 81, 143), new Vector2(2f, 2f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel11.Text = "#DECKBUILDER_CARDS";
		gUIStrokeTextLabel11.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		gUIStrokeTextLabel11.ToolTip = new NamedToolTipInfo("#TT_CARDGAME_DECK_COUNT", new Vector2(10f, 20f));
		gUIStrokeTextLabel11.ToolTip.IgnoreCursor = true;
		gUIStrokeTextLabel11.ToolTip.VerticalAlignmentOverride = SHSTooltip.ToolTipVerticalAlignment.Top;
		gUIStrokeTextLabel11.ToolTip.HorizontalAlignmentOverride = SHSTooltip.ToolTipHorizontalAlignment.Right;
		midTopPane.Add(gUIStrokeTextLabel11);
		deckCounterLabel = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(130f, 19f), new Vector2(60f, 22f));
		deckCounterLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(51, 116, 183), GUILabel.GenColor(20, 81, 143), new Vector2(2f, 2f), TextAnchor.MiddleRight);
		deckCounterLabel.Text = DeckBuilderController.Instance.DeckCards.TotalCards.ToString() + "/" + Math.Max(DeckBuilderController.Instance.DeckCards.TotalCards, 40).ToString();
		deckCounterLabel.Id = "deckCounterLabel";
		deckCounterLabel.ToolTip = new NamedToolTipInfo("#TT_CARDGAME_DECK_COUNT", new Vector2(145f, 20f));
		deckCounterLabel.ToolTip.IgnoreCursor = true;
		deckCounterLabel.ToolTip.VerticalAlignmentOverride = SHSTooltip.ToolTipVerticalAlignment.Top;
		deckCounterLabel.ToolTip.HorizontalAlignmentOverride = SHSTooltip.ToolTipHorizontalAlignment.Right;
		deckCounterLabel.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		midTopPane.Add(deckCounterLabel);
		GUISimpleControlWindow gUISimpleControlWindow2 = new GUISimpleControlWindow();
		gUISimpleControlWindow2.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 13f), new Vector2(405f, 250f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		midTopPane.Add(gUISimpleControlWindow2);
		deckRenderTexture = new GUIImage();
		deckRenderTexture.SetPositionAndSize(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(390f, 390f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUISimpleControlWindow2.Add(deckRenderTexture);
		gUISimpleControlWindow2.HitTestType = HitTestTypeEnum.Rect;
		gUISimpleControlWindow2.MouseOver += delegate
		{
			deckMouseOver = true;
		};
		gUISimpleControlWindow2.MouseOut += delegate
		{
			deckMouseOver = false;
		};
		gUISimpleControlWindow2.MouseDown += delegate
		{
			GameObject exists2 = controller.PickCardPanel(controller.deckCamera, deckRenderTexture);
			if ((bool)exists2)
			{
				ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("click_down"));
			}
		};
		gUISimpleControlWindow2.Click += delegate
		{
			GameObject gameObject2 = controller.PickCardPanel(controller.deckCamera, deckRenderTexture);
			if ((bool)gameObject2)
			{
				AppShell.Instance.EventMgr.Fire(this, new CardCollection3DMessage(CardCollection3DMessage.CC3DEvent.CardMouseLeftClick, gameObject2));
				ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("click_up"));
			}
		};
		gUISimpleControlWindow2.RightMouseDown += delegate
		{
			GameObject x3 = controller.PickCardPanel(controller.deckCamera, deckRenderTexture);
			if (x3 != null)
			{
				if (zoomComponent.IsZooming)
				{
					zoomComponent.ZoomOutOnCard();
				}
				else
				{
					zoomComponent.ZoomInOnCard(false);
				}
			}
		};
		deckSlider = new GUISlider();
		deckSlider.Orientation = GUISlider.SliderOrientationEnum.Vertical;
		deckSlider.SetPositionAndSize(AnchorAlignmentEnum.Middle, new Vector2(457f, 175f), new Vector2(40f, 287f));
		deckSlider.IsVisible = false;
		deckSlider.IsEnabled = true;
		deckSlider.Min = 0f;
		deckSlider.Max = 100f;
		deckSlider.Value = 0f;
		deckSlider.ArrowsEnabled = true;
		deckSlider.StyleInfo = SHSInheritedStyleInfo.Instance;
		deckSlider.Changed += delegate(GUIControl sender, GUIChangedEvent eventData)
		{
			AppShell.Instance.EventMgr.Fire(this, new CardCollectionUIMessage(CardCollectionUIMessage.CCUIEvent.DeckGridScroll, sender));
		};
		midTopPane.Add(deckSlider);
		midBottomPane = new GUISimpleControlWindow();
		midBottomPane.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(-2f, -2f), new Vector2(482f, 319f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Add(midBottomPane);
		gUIImage = new GUIImage();
		gUIImage.TextureSource = "deckbuilder_bundle|deckbuilder_collection_panel";
		gUIImage.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, -2f), midBottomPane.Size, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		midBottomPane.Add(gUIImage);
		GUIStrokeTextLabel gUIStrokeTextLabel12 = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(200f, 35f), new Vector2(-105f, 25f));
		gUIStrokeTextLabel12.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 30, GUILabel.GenColor(255, 250, 195), GUILabel.GenColor(210, 148, 13), GUILabel.GenColor(173, 123, 32), new Vector2(4f, 3f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel12.Text = "#DECKBUILDER_COLLECTION";
		gUIStrokeTextLabel12.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		gUIStrokeTextLabel12.ToolTip = new NamedToolTipInfo("#TT_DECKBUILDER_COLLECTION_TITLE", new Vector2(-84f, 0f));
		midBottomPane.Add(gUIStrokeTextLabel12);
		collectionCounterLabel = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(130f, 19f), new Vector2(30f, 22f));
		collectionCounterLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(177, 120, 10), GUILabel.GenColor(144, 99, 17), new Vector2(2f, 2f), TextAnchor.MiddleRight);
		collectionCounterLabel.Text = "0";
		collectionCounterLabel.ToolTip = new NamedToolTipInfo("#TT_CARDGAME_COLLECTION_COUNT", new Vector2(140f, -5f));
		collectionCounterLabel.ToolTip.IgnoreCursor = true;
		collectionCounterLabel.ToolTip.VerticalAlignmentOverride = SHSTooltip.ToolTipVerticalAlignment.Bottom;
		collectionCounterLabel.ToolTip.HorizontalAlignmentOverride = SHSTooltip.ToolTipHorizontalAlignment.Right;
		collectionCounterLabel.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		collectionCounterLabel.Id = "collectionCounterLabel";
		midBottomPane.Add(collectionCounterLabel);
		GUIStrokeTextLabel gUIStrokeTextLabel13 = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(200f, 19f), new Vector2(200f, 22f));
		gUIStrokeTextLabel13.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(177, 120, 10), GUILabel.GenColor(144, 99, 17), new Vector2(2f, 2f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel13.Text = "#DECKBUILDER_TOTAL_CARDS";
		gUIStrokeTextLabel13.ToolTip = new NamedToolTipInfo("#TT_CARDGAME_COLLECTION_COUNT", new Vector2(0f, -5f));
		gUIStrokeTextLabel13.ToolTip.IgnoreCursor = true;
		gUIStrokeTextLabel13.ToolTip.VerticalAlignmentOverride = SHSTooltip.ToolTipVerticalAlignment.Bottom;
		gUIStrokeTextLabel13.ToolTip.HorizontalAlignmentOverride = SHSTooltip.ToolTipHorizontalAlignment.Right;
		gUIStrokeTextLabel13.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		midBottomPane.Add(gUIStrokeTextLabel13);
		collectionPane = new GUISimpleControlWindow();
		collectionPane.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 15f), new Vector2(405f, 250f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		midBottomPane.Add(collectionPane);
		collectionRenderTexture = new GUIImage();
		collectionRenderTexture.SetPositionAndSize(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(390f, 390f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		collectionPane.Add(collectionRenderTexture);
		collectionPane.HitTestType = HitTestTypeEnum.Rect;
		collectionPane.MouseOver += delegate
		{
			collectionMouseOver = true;
		};
		collectionPane.MouseOut += delegate
		{
			collectionMouseOver = false;
		};
		collectionPane.MouseDown += delegate
		{
			GameObject exists = controller.PickCardPanel(controller.collectionCamera, collectionRenderTexture);
			if ((bool)exists)
			{
				ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("click_down"));
			}
		};
		collectionPane.RightMouseDown += delegate
		{
			GameObject x2 = controller.PickCardPanel(controller.collectionCamera, collectionRenderTexture);
			if (x2 != null)
			{
				if (zoomComponent.IsZooming)
				{
					zoomComponent.ZoomOutOnCard();
				}
				else
				{
					zoomComponent.ZoomInOnCard(false);
				}
			}
		};
		collectionPane.Click += delegate
		{
			GameObject gameObject = controller.PickCardPanel(controller.collectionCamera, collectionRenderTexture);
			if ((bool)gameObject)
			{
				AppShell.Instance.EventMgr.Fire(this, new CardCollection3DMessage(CardCollection3DMessage.CC3DEvent.CardMouseLeftClick, gameObject));
				ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("click_up"));
			}
		};
		collectionSlider = new GUISlider();
		collectionSlider.Orientation = GUISlider.SliderOrientationEnum.Vertical;
		collectionSlider.SetPositionAndSize(AnchorAlignmentEnum.Middle, new Vector2(457f, 175f), new Vector2(40f, 287f));
		collectionSlider.Min = 0f;
		collectionSlider.Max = 100f;
		collectionSlider.Value = 0f;
		collectionSlider.ArrowsEnabled = true;
		collectionSlider.Changed += delegate(GUIControl sender, GUIChangedEvent eventData)
		{
			AppShell.Instance.EventMgr.Fire(this, new CardCollectionUIMessage(CardCollectionUIMessage.CCUIEvent.CollectionGridScroll, sender));
		};
		midBottomPane.Add(collectionSlider);
		rightPane = new GUISimpleControlWindow();
		rightPane.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(240f, 0f), new Vector2(269f, 634f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Add(rightPane);
		gUIImage = new GUIImage();
		gUIImage.TextureSource = "deckbuilder_bundle|deckbuilder_right_panel";
		gUIImage.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, rightPane.Size);
		rightPane.Add(gUIImage);
		GUISimpleControlWindow gUISimpleControlWindow3 = GUIControl.CreateControlTopFrameCentered<GUISimpleControlWindow>(new Vector2(270f, 50f), new Vector2(-15f, 27f));
		gUISimpleControlWindow3.HitTestType = HitTestTypeEnum.Rect;
		gUISimpleControlWindow3.ToolTip = new NamedToolTipInfo("#TT_DECKBUILDER_COLLECTION_SCORE", new Vector2(80f, 0f));
		gUISimpleControlWindow3.ToolTip.IgnoreCursor = true;
		GUILabel gUILabel = GUIControl.CreateControlTopFrameCentered<GUILabel>(new Vector2(200f, 35f), new Vector2(0f, 0f));
		gUILabel.Docking = DockingAlignmentEnum.Middle;
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, GUILabel.GenColor(252, 242, 199), TextAnchor.MiddleLeft);
		gUILabel.Text = "#DECKBUILDER_COLLECTION_SCORE";
		gUISimpleControlWindow3.Add(gUILabel);
		collectionScoreValueLabel = GUIControl.CreateControlTopFrameCentered<GUIDropShadowTextLabel>(new Vector2(70f, 35f), new Vector2(58f, 0f));
		collectionScoreValueLabel.Docking = DockingAlignmentEnum.Middle;
		collectionScoreValueLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, GUILabel.GenColor(172, 214, 9), GUILabel.GenColor(0, 21, 105), new Vector2(1f, 1f), TextAnchor.MiddleRight);
		collectionScoreValueLabel.Text = "0";
		gUISimpleControlWindow3.Add(collectionScoreValueLabel);
		rightPane.Add(gUISimpleControlWindow3);
		GUIButton gUIButton4 = new GUIButton();
		gUIButton4.Id = "buildDeckButton";
		gUIButton4.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(4f, -185f), new Vector2(512f, 512f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton4.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton4.ToolTip = new NamedToolTipInfo("#TT_DECKBUILDER_1", new Vector2(196f, 220f));
		gUIButton4.StyleInfo = new SHSButtonStyleInfo("deckbuilder_bundle|deckbuilder_build_deck_button");
		gUIButton4.Click += delegate
		{
			DeckBuilderController deckBuilderController3 = (DeckBuilderController)GameController.GetController();
			if (deckBuilderController3 != null && deckBuilderController3.DeckCards != null && deckBuilderController3.DeckCards.Count > 0 && !deckBuilderController3.DeckCards.saved)
			{
				GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, "#DECKBUILDER_EXIT_SAVE_DECK_ PROMPT", delegate(string Id, GUIDialogWindow.DialogState state)
				{
					if (state == GUIDialogWindow.DialogState.Ok)
					{
						ShsEventMgr.GenericDelegate<CardCollectionSavedMessage> t3 = null;
						t3 = delegate
						{
							AppShell.Instance.EventMgr.RemoveListener(t3);
							Hide();
							(GUIManager.Instance["/SHSMainWindow/SHSAutoDeckBuilderWindow"] as SHSAutoDeckBuilderWindow).ShowRoot();
						};
						AppShell.Instance.EventMgr.AddListener(t3);
						AppShell.Instance.EventMgr.Fire(this, new CardCollectionUIMessage(CardCollectionUIMessage.CCUIEvent.SaveClicked, this));
					}
					else
					{
						Hide();
						(GUIManager.Instance["/SHSMainWindow/SHSAutoDeckBuilderWindow"] as SHSAutoDeckBuilderWindow).ShowRoot();
					}
				}, ModalLevelEnum.Default);
			}
			else
			{
				Hide();
				(GUIManager.Instance["/SHSMainWindow/SHSAutoDeckBuilderWindow"] as SHSAutoDeckBuilderWindow).ShowRoot();
			}
		};
		rightPane.Add(gUIButton4);
		GUIStrokeTextLabel gUIStrokeTextLabel14 = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(200f, 35f), new Vector2(20f, 450f));
		gUIStrokeTextLabel14.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 26, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(102, 129, 21), GUILabel.GenColor(71, 92, 8), new Vector2(2f, 2f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel14.Text = "#DECKBUILDER_AUTO_BUILD_A_DECK";
		rightPane.Add(gUIStrokeTextLabel14);
		gUIButton4 = new GUIButton();
		gUIButton4.Id = "openDeckButton";
		gUIButton4.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-50f, -125f), new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton4.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton4.ToolTip = new NamedToolTipInfo("#TT_DECKBUILDER_2", new Vector2(69f, 18f));
		gUIButton4.StyleInfo = new SHSButtonStyleInfo("deckbuilder_bundle|deckbuilder_load_button");
		gUIButton4.Click += delegate(GUIControl sender, GUIClickEvent eventArgs)
		{
			AppShell.Instance.EventMgr.Fire(sender, new CardCollectionUIMessage(CardCollectionUIMessage.CCUIEvent.LoadClicked, sender));
		};
		rightPane.Add(gUIButton4);
		GUIStrokeTextLabel gUIStrokeTextLabel15 = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(200f, 35f), new Vector2(25f, 508f));
		gUIStrokeTextLabel15.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(102, 129, 21), GUILabel.GenColor(71, 92, 8), new Vector2(2f, 2f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel15.Text = "#DECKBUILDER_LOAD";
		rightPane.Add(gUIStrokeTextLabel15);
		gUIButton4 = new GUIButton();
		gUIButton4.Id = "saveDeckButton";
		gUIButton4.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(55f, -125f), new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton4.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton4.ToolTip = new NamedToolTipInfo("#TT_DECKBUILDER_4", new Vector2(-9f, 27f));
		gUIButton4.ToolTip.VerticalAlignmentOverride = SHSTooltip.ToolTipVerticalAlignment.Bottom;
		gUIButton4.StyleInfo = new SHSButtonStyleInfo("deckbuilder_bundle|deckbuilder_save_button");
		gUIButton4.Click += delegate(GUIControl sender, GUIClickEvent eventArgs)
		{
			AppShell.Instance.EventMgr.Fire(sender, new CardCollectionUIMessage(CardCollectionUIMessage.CCUIEvent.SaveClicked, sender));
		};
		rightPane.Add(gUIButton4);
		GUIStrokeTextLabel gUIStrokeTextLabel16 = new GUIStrokeTextLabel();
		gUIStrokeTextLabel16.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(53f, -125f), new Vector2(128f, 35f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel16.Id = "saveButtonLabel";
		gUIStrokeTextLabel16.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(102, 129, 21), GUILabel.GenColor(71, 92, 8), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel16.Text = "#DECKBUILDER_SAVE";
		gUIStrokeTextLabel16.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		rightPane.Add(gUIStrokeTextLabel16);
		gUIButton4 = new GUIButton();
		gUIButton4.Id = "backButton";
		gUIButton4.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-73f, -65f), new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton4.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton4.ToolTip = new NamedToolTipInfo("#TT_DECKBUILDER_BACK", new Vector2(85f, 22f));
		gUIButton4.StyleInfo = new SHSButtonStyleInfo("deckbuilder_bundle|deckbuilder_back_button");
		gUIButton4.Click += delegate
		{
			DeckBuilderController deckBuilderController2 = (DeckBuilderController)GameController.GetController();
			if (deckBuilderController2 != null && deckBuilderController2.DeckCards != null && deckBuilderController2.DeckCards.Count > 0 && !deckBuilderController2.DeckCards.saved)
			{
				GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, "#DECKBUILDER_EXIT_SAVE_DECK_ PROMPT", delegate(string Id, GUIDialogWindow.DialogState state)
				{
					if (state == GUIDialogWindow.DialogState.Ok)
					{
						ShsEventMgr.GenericDelegate<CardCollectionSavedMessage> t2 = null;
						t2 = delegate
						{
							AppShell.Instance.EventMgr.RemoveListener(t2);
							Hide();
							(GUIManager.Instance["/SHSMainWindow/SHSAutoDeckBuilderWindow"] as SHSAutoDeckBuilderWindow).ShowRoot();
						};
						AppShell.Instance.EventMgr.AddListener(t2);
						AppShell.Instance.EventMgr.Fire(this, new CardCollectionUIMessage(CardCollectionUIMessage.CCUIEvent.SaveClicked, this));
					}
					else
					{
						Hide();
						(GUIManager.Instance["/SHSMainWindow/SHSAutoDeckBuilderWindow"] as SHSAutoDeckBuilderWindow).ShowRoot();
					}
				}, ModalLevelEnum.Default);
			}
			else
			{
				Hide();
				(GUIManager.Instance["/SHSMainWindow/SHSAutoDeckBuilderWindow"] as SHSAutoDeckBuilderWindow).ShowRoot();
			}
		};
		rightPane.Add(gUIButton4);
		GUIStrokeTextLabel gUIStrokeTextLabel17 = new GUIStrokeTextLabel();
		gUIStrokeTextLabel17.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-75f, -65f), new Vector2(60f, 35f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel17.Id = "backButtonLabel";
		gUIStrokeTextLabel17.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(102, 129, 21), GUILabel.GenColor(71, 92, 8), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel17.Text = "#DECKBUILDER_BACK";
		gUIStrokeTextLabel17.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		rightPane.Add(gUIStrokeTextLabel17);
		gUIButton4 = new GUIButton();
		gUIButton4.Id = "playButton";
		gUIButton4.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(4f, -65f), new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton4.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton4.ToolTip = new NamedToolTipInfo("#TT_DECKBUILDER_PLAY", new Vector2(67f, 22f));
		gUIButton4.StyleInfo = new SHSButtonStyleInfo("deckbuilder_bundle|deckbuilder_play_button");
		gUIButton4.Click += delegate
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
							SHSCardGameGadgetWindow cardWindow3 = new SHSCardGameGadgetWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.Main);
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
						SHSCardGameGadgetWindow cardWindow2 = new SHSCardGameGadgetWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.Main);
						cardWindow2.CloseButton.Click += delegate
						{
							cardWindow2.CloseGadget();
						};
						GUIManager.Instance.ShowDynamicWindow(cardWindow2, ModalLevelEnum.Default);
					}
				}, ModalLevelEnum.Default);
			}
			else
			{
				SHSCardGameGadgetWindow cardWindow = new SHSCardGameGadgetWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.Main);
				cardWindow.CloseButton.Click += delegate
				{
					cardWindow.CloseGadget();
				};
				GUIManager.Instance.ShowDynamicWindow(cardWindow, ModalLevelEnum.Default);
			}
		};
		rightPane.Add(gUIButton4);
		GUIStrokeTextLabel gUIStrokeTextLabel18 = new GUIStrokeTextLabel();
		gUIStrokeTextLabel18.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(1f, -65f), new Vector2(80f, 35f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel18.Id = "playButtonLabel";
		gUIStrokeTextLabel18.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 24, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(102, 129, 21), GUILabel.GenColor(71, 92, 8), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel18.Text = "#DECKBUILDER_PLAY";
		gUIStrokeTextLabel18.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		rightPane.Add(gUIStrokeTextLabel18);
		gUIButton4 = new GUIButton();
		gUIButton4.Id = "quitButton";
		gUIButton4.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(85f, 252f), new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton4.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton4.ToolTip = new NamedToolTipInfo("#TT_DECKBUILDER_QUIT", new Vector2(-9f, 27f));
		gUIButton4.ToolTip.VerticalAlignmentOverride = SHSTooltip.ToolTipVerticalAlignment.Bottom;
		gUIButton4.StyleInfo = new SHSButtonStyleInfo("deckbuilder_bundle|deckbuilder_quit_button");
		gUIButton4.Click += delegate
		{
			Close();
		};
		rightPane.Add(gUIButton4);
		GUIStrokeTextLabel gUIStrokeTextLabel19 = new GUIStrokeTextLabel();
		gUIStrokeTextLabel19.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(82f, 252f), new Vector2(60f, 35f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel19.Id = "quitButtonLabel";
		gUIStrokeTextLabel19.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(169, 44, 19), GUILabel.GenColor(137, 18, 0), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel19.Text = "#DECKBUILDER_QUIT";
		gUIStrokeTextLabel19.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		rightPane.Add(gUIStrokeTextLabel19);
		gUIButton4 = new GUIButton();
		gUIButton4.Id = "closeDeckBuilderButton";
		gUIButton4.SetPositionAndSize(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(-18f, -5f), new Vector2(45f, 45f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton4.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton4.StyleInfo = new SHSButtonStyleInfo("common_bundle|button_close");
		gUIButton4.ToolTip = new NamedToolTipInfo(SHSTooltip.GetCommonToolTipText(SHSTooltip.CommonToolTipText.Close));
		gUIButton4.Click += delegate
		{
			Close();
		};
		rightPane.Add(gUIButton4);
		largeCardPane = new GUISimpleControlWindow();
		largeCardPane.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(118f, -5f), new Vector2(380f, 520f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Add(largeCardPane);
		largeCard = GUIControl.CreateControl<GUIDrawTexture>(VIEW_CARD_SIZE, VIEW_CARD_OFFSET, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		largeCard.TextureSource = "deckbuilder_bundle|mshs_card_rear_noURL";
		largeCard.RightMouseDown += delegate
		{
			if (largeCardSet)
			{
				if (zoomComponent.IsZooming)
				{
					zoomComponent.ZoomOutOnCard();
				}
				else
				{
					zoomComponent.ZoomInOnCard(true);
				}
			}
		};
		largeCardPane.Add(largeCard);
		zoomComponent = new CardZoomComponent(base.AnimationPieceManager, largeCard, new CardZoomComponent.ZoomData(VIEW_CARD_SIZE, VIEW_CARD_OFFSET, ZOOM_CARD_SIZE, ZOOM_CARD_OFFSET, 3f, 0.5f));
	}

	public override void OnShow()
	{
		base.OnShow();
		BringToFront();
		DeckBuilderController deckBuilderController = (DeckBuilderController)GameController.GetController();
		collectionRenderTexture.Texture = deckBuilderController.collectionCamera.targetTexture;
		deckRenderTexture.Texture = deckBuilderController.deckCamera.targetTexture;
		eventMgr = AppShell.Instance.EventMgr;
		eventMgr.AddListener<CardCollectionDataMessage>(OnCardCountChanged);
		deckCardCount = deckBuilderController.DeckCards.CountVisible();
		collectionCardCount = deckBuilderController.MyCards.CountVisible();
		OnCardCountChanged(new CardCollectionDataMessage(CardCollectionDataMessage.CCDataEvent.DeckCountChanged, deckBuilderController.DeckCards.Count, deckCardCount));
		OnCardCountChanged(new CardCollectionDataMessage(CardCollectionDataMessage.CCDataEvent.CollectionCountChanged, deckBuilderController.MyCards.TotalCards, collectionCardCount));
		UpdateSliderVisibility();
		UpdateCollectionScore();
		SHSInput.SetInputBlockingMode(this, SHSInput.InputBlockType.BlockWorld);
	}

	public override void OnHide()
	{
		base.OnHide();
		eventMgr.RemoveListener<CardCollectionDataMessage>(OnCardCountChanged);
		SHSInput.RevertInputBlockingMode(this);
	}

	private void Reset()
	{
		sortRadioButtons.Select(2);
		searchField.Text = string.Empty;
		SearchCardText(string.Empty);
		collectionSlider.Value = 0f;
		collectionSlider.FireChanged();
		deckSlider.Value = 0f;
		deckSlider.FireChanged();
		foreach (GUIButton factorToggleButton in FactorToggleButtons)
		{
			factorToggleButton.IsSelected = false;
		}
		foreach (GUIButton blockToggleButton in BlockToggleButtons)
		{
			blockToggleButton.IsSelected = false;
		}
		foreach (GUIButton teamToggleButton in TeamToggleButtons)
		{
			teamToggleButton.IsSelected = false;
		}
	}

	public void Close()
	{
		DeckBuilderController deckBuilderController = (DeckBuilderController)GameController.GetController();
		if (deckBuilderController != null && deckBuilderController.DeckCards != null && deckBuilderController.DeckCards.Count > 0 && !deckBuilderController.DeckCards.saved)
		{
			GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, "#DECKBUILDER_EXIT_SAVE_DECK_ PROMPT", delegate(string Id, GUIDialogWindow.DialogState state)
			{
				if (state == GUIDialogWindow.DialogState.Ok)
				{
					SHSDeckBuilderWindow sHSDeckBuilderWindow = this;
					ShsEventMgr.GenericDelegate<CardCollectionSavedMessage> t = null;
					t = delegate
					{
						AppShell.Instance.EventMgr.RemoveListener(t);
						sHSDeckBuilderWindow.Hide();
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

	private void UpdateDeckStats()
	{
		DeckBuilderController deckBuilderController = (DeckBuilderController)GameController.GetController();
		SortedCardList deckCards = deckBuilderController.DeckCards;
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		Dictionary<BattleCard.Factor, int> dictionary2 = new Dictionary<BattleCard.Factor, int>();
		Dictionary<BattleCard.Factor, int> dictionary3 = new Dictionary<BattleCard.Factor, int>();
		int num = 0;
		int num2 = 0;
		if (deckCards != null)
		{
			foreach (CardListCard item in deckCards)
			{
				int num3 = (item.Level <= 10) ? item.Level : 10;
				int available = item.Available;
				if (!dictionary.ContainsKey(num3))
				{
					dictionary.Add(num3, available);
				}
				else
				{
					Dictionary<int, int> dictionary4;
					Dictionary<int, int> dictionary5 = dictionary4 = dictionary;
					int key;
					int key2 = key = num3;
					key = dictionary4[key];
					dictionary5[key2] = key + available;
				}
				BattleCard.Factor[] attackFactors = item.AttackFactors;
				foreach (BattleCard.Factor factor in attackFactors)
				{
					if (!dictionary2.ContainsKey(factor))
					{
						dictionary2.Add(factor, available);
					}
					else
					{
						Dictionary<BattleCard.Factor, int> dictionary6;
						Dictionary<BattleCard.Factor, int> dictionary7 = dictionary6 = dictionary2;
						BattleCard.Factor key3;
						BattleCard.Factor key4 = key3 = factor;
						int key = dictionary6[key3];
						dictionary7[key4] = key + available;
					}
				}
				num += item.AttackFactors.Length * available;
				BattleCard.Factor[] blockFactors = item.BlockFactors;
				foreach (BattleCard.Factor factor2 in blockFactors)
				{
					if (!dictionary3.ContainsKey(factor2))
					{
						dictionary3.Add(factor2, available);
					}
					else
					{
						Dictionary<BattleCard.Factor, int> dictionary8;
						Dictionary<BattleCard.Factor, int> dictionary9 = dictionary8 = dictionary3;
						BattleCard.Factor key3;
						BattleCard.Factor key5 = key3 = factor2;
						int key = dictionary8[key3];
						dictionary9[key5] = key + available;
					}
				}
				num2 += item.BlockFactors.Length * available;
			}
		}
		float num4 = 10f;
		for (int k = 0; k < 10; k++)
		{
			if (dictionary.ContainsKey(k + 1))
			{
				levelBars[k].Percent = (float)dictionary[k + 1] / num4;
				levelBars[k].Text = dictionary[k + 1].ToString();
				levelBars[k].IsVisible = true;
			}
			else
			{
				levelBars[k].IsVisible = false;
			}
		}
		if (attackFactorBar != null)
		{
			attackFactorBar.DeckCount = ((num >= 40) ? num : 40);
			attackFactorBar.CardCounts = dictionary2;
		}
		if (blockFactorBar != null)
		{
			blockFactorBar.DeckCount = ((num2 >= 40) ? num2 : 40);
			blockFactorBar.CardCounts = dictionary3;
		}
	}

	private void UpdateSliderVisibility()
	{
		if (deckCardCount > 8)
		{
			deckSlider.Alpha = 1f;
			deckScrollSpeed = 125f;
		}
		else
		{
			deckSlider.Alpha = 0f;
			deckSlider.Value = 0f;
			deckSlider.FireChanged();
		}
		if (collectionCardCount > 8)
		{
			collectionSlider.Alpha = 1f;
			collectionScrollSpeed = 5f;
		}
		else
		{
			collectionSlider.Alpha = 0f;
		}
	}

	private void UpdateCollectionScore()
	{
		if (collectionScoreValueLabel != null)
		{
			DeckBuilderController deckBuilderController = (DeckBuilderController)GameController.GetController();
			int num = 0;
			foreach (int value in Enum.GetValues(typeof(BattleCard.CardRarity)))
			{
				num += BattleCard.RarityToScore((BattleCard.CardRarity)value) * deckBuilderController.MyCards.CountByRarity((BattleCard.CardRarity)value);
			}
			collectionScoreValueLabel.Text = num.ToString();
		}
	}

	private void OnCardCountChanged(CardCollectionDataMessage msg)
	{
		if (msg.Event == CardCollectionDataMessage.CCDataEvent.DeckCountChanged)
		{
			if (DeckBuilderController.Instance.DeckCards.TotalCards > 40)
			{
				deckCounterLabel.Text = string.Format("{0}", DeckBuilderController.Instance.DeckCards.TotalCards);
			}
			else
			{
				deckCounterLabel.Text = string.Format("{0}/{1}", DeckBuilderController.Instance.DeckCards.TotalCards, 40);
			}
			UpdateDeckStats();
			if (msg.ScrollToTop)
			{
				deckSlider.Value = 0f;
				deckSlider.FireChanged();
			}
			else if (msg.CardTypeCount > deckCardCount || (msg.CardTypeCount < deckCardCount && deckSlider.Value == 100f))
			{
				deckSlider.Value = 100f;
				deckSlider.FireChanged();
			}
			deckCardCount = msg.CardTypeCount;
			UpdateSliderVisibility();
		}
		else if (msg.Event == CardCollectionDataMessage.CCDataEvent.CollectionCountChanged)
		{
			collectionCardCount = DeckBuilderController.Instance.MyCards.TotalCards + DeckBuilderController.Instance.DeckCards.TotalCards;
			UpdateSliderVisibility();
			collectionSlider.Value = 0f;
			collectionSlider.FireChanged();
			collectionCounterLabel.Text = collectionCardCount.ToString();
		}
	}

	private void SearchCardText(string searchText)
	{
		AppShell.Instance.EventMgr.Fire(this, new CardCollectionFilterUIMessage(this, CardFilterType.TextSearch, searchText));
	}

	private void OnReturnKeypress(SHSKeyCode code)
	{
		SearchCardText(searchField.Text);
	}

	public override void ConfigureKeyBanks()
	{
		base.ConfigureKeyBanks();
		keyBanks[KeyInputState.Active].AddKey(new KeyCodeEntry(KeyCode.Return, false, false, false), OnReturnKeypress);
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
	}

	public override void OnActive()
	{
		base.OnActive();
		UserProfile profile = AppShell.Instance.Profile;
		if (profile == null)
		{
			CspUtils.DebugLog("No profile. Offline?Controller is:" + GameController.GetController());
		}
	}

	public override void OnInactive()
	{
		base.OnInactive();
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

	public override void Update()
	{
		base.Update();
		if (!IsActive)
		{
			return;
		}
		DeckBuilderController deckBuilderController = (DeckBuilderController)GameController.GetController();
		GameObject gameObject = null;
		if (collectionMouseOver)
		{
			gameObject = deckBuilderController.PickCardPanel(deckBuilderController.collectionCamera, collectionRenderTexture);
			float mouseWheelDelta = SHSInput.GetMouseWheelDelta(midBottomPane);
			if (mouseWheelDelta != 0f)
			{
				collectionSlider.Value += mouseWheelDelta * (0f - collectionScrollSpeed);
			}
		}
		else if (deckMouseOver)
		{
			gameObject = deckBuilderController.PickCardPanel(deckBuilderController.deckCamera, deckRenderTexture);
			float mouseWheelDelta2 = SHSInput.GetMouseWheelDelta(midTopPane);
			if (mouseWheelDelta2 != 0f)
			{
				deckSlider.Value += mouseWheelDelta2 * (0f - deckScrollSpeed);
			}
		}
		if ((bool)gameObject)
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
		zoomComponent.Update();
	}
}
