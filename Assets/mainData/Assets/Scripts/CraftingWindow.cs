using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingWindow : GUIDialogWindow
{
	private const int _blueprintBaseX = 20;

	private const int _blueprintBaseY = 0;

	private const int _pieceBaseX = 395;

	private const int _pieceBaseY = 0;

	private GUIImage _frame;

	private GUISimpleControlWindow _content;

	private GUISimpleControlWindow _blueprintScrollPaneMask;

	private GUISimpleControlWindow _blueprintScrollPane;

	private GUISlider _blueprintScrollPaneSlider;

	private int _blueprintCurrX = 20;

	private int _blueprintCurrY;

	private int _blueprintDeltY = 40;

	private GUISimpleControlWindow _blueprintImage;

	private GUIStrokeTextLabel _blueprintName;

	private GUILabel _blueprintDesc;

	private GUIImage _blueprintOutputOwned;

	private GUIButton _masterCraftButton;

	private GUIButton _masterCraftButtonDisabled;

	private GUISimpleControlWindow _pieces;

	private GUIImage _topTierItemQuantityHolder;

	private GUILabel _topTierItemQuantityLabel;

	private GUIButton _shoppingButton;

	private GUISimpleControlWindow _upgradeDisplay;

	private List<GUIImageWithEvents> _upgradeIcons = new List<GUIImageWithEvents>();

	private List<GUILabel> _upgradeLabels = new List<GUILabel>();

	private GUILabel _upgradeHeader;

	private List<CraftingWindowPrimeContainer> _primeContainers = new List<CraftingWindowPrimeContainer>();

	private int _selectedPieceOwnableTypeID = -1;

	private int _pieceCurrX = 395;

	private int _pieceCurrY;

	private int _pieceDeltX = -125;

	private int _pieceCraftOffsetX = -83;

	private int _pieceCraftOffsetY = 54;

	private int _baseContentX = 370;

	private int _baseContentY = 50;

	private int _currentBlueprintID = -1;

	private bool _craftUnlocked;

	private List<GUIControl> _pieceList;

	private GUIButton _tabButtonGoodies;

	private GUIImage _tabImageGoodies;

	private GUIButton _tabButtonSidekicks;

	private GUIImage _tabImageSidekicks;

	private GUIButton _tabButtonParts;

	private GUIImage _tabImageParts;

	private GUITextField _searchText;

	private GUISimpleControlWindow _disabledInfoWindow;

	private GUIImage _background;

	private GameObject _craftSFX;

	private bool craftLocked;

	private string currentRecipeList = string.Empty;

	private List<LeftNavBlueprintContainer> _blueprintVisList = new List<LeftNavBlueprintContainer>();

	public CraftingWindow(int initialOwnableID = -1)
	{
		Traits.FullScreenOpaqueBackgroundTrait = ControlTraits.FullScreenOpaqueBackgroundTraitEnum.HasFullScreenOpaqueBackground;
		Traits.EventListenerRegistrationTrait = ControlTraits.EventListenerRegistrationTraitEnum.Ignore;
		SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		SetPosition(QuickSizingHint.Centered);
		_frame = new GUIImage();
		_frame.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		_frame.Position = Vector2.zero;
		_frame.TextureSource = "mysquadgadget_bundle|bg";
		Add(_frame);
		_content = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(974f, 547f), new Vector2(0f, -10f));
		_content.IsVisible = true;
		Add(_content);
		_background = new GUIImage();
		_background.SetSize(974f, 502f);
		_background.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 17f));
		_background.TextureSource = "mysquadgadget_bundle|mysquad_2panels_challengeandheroespage_01_crafting_tab01";
		_content.Add(_background);
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(0f, 20f), new Vector2(200f, 30f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 24, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 43, 68), GUILabel.GenColor(0, 43, 68), new Vector2(3f, 4f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel.BackColorAlpha = 1f;
		gUIStrokeTextLabel.StrokeColorAlpha = 1f;
		gUIStrokeTextLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUIStrokeTextLabel.Text = "#CRAFTING_WINDOW_TITLE";
		gUIStrokeTextLabel.IsVisible = true;
		Add(gUIStrokeTextLabel);
		int num = 47;
		int num2 = 47;
		int num3 = 57;
		_tabButtonGoodies = new GUIButton();
		_tabButtonGoodies.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(num, num2), new Vector2(38f, 38f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_tabButtonGoodies.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|item_type_goodies", SHSButtonStyleInfo.SupportedStatesEnum.Normal);
		_tabButtonGoodies.ToolTip = new NamedToolTipInfo("#CRAFT_TAB_GOODIES");
		_tabButtonGoodies.Click += tabGoodies;
		_tabButtonGoodies.IsVisible = true;
		_content.Add(_tabButtonGoodies);
		_tabImageGoodies = new GUIImage();
		_tabImageGoodies.SetSize(50f, 50f);
		_tabImageGoodies.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(num - 5, num2 - 5));
		_tabImageGoodies.TextureSource = "mysquadgadget_bundle|item_type_goodies_highlight";
		_content.Add(_tabImageGoodies);
		_tabButtonSidekicks = new GUIButton();
		_tabButtonSidekicks.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(num + num3, num2), new Vector2(38f, 38f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_tabButtonSidekicks.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|item_type_sidekicks", SHSButtonStyleInfo.SupportedStatesEnum.Normal);
		_tabButtonSidekicks.ToolTip = new NamedToolTipInfo("#CRAFT_TAB_SIDEKICKS");
		_tabButtonSidekicks.Click += tabSidekicks;
		_tabButtonSidekicks.IsVisible = true;
		_content.Add(_tabButtonSidekicks);
		_tabImageSidekicks = new GUIImage();
		_tabImageSidekicks.SetSize(50f, 50f);
		_tabImageSidekicks.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(num + num3 - 5, num2 - 5));
		_tabImageSidekicks.TextureSource = "mysquadgadget_bundle|item_type_sidekicks_highlight";
		_content.Add(_tabImageSidekicks);
		_tabButtonParts = new GUIButton();
		_tabButtonParts.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(num + num3 * 2, num2), new Vector2(38f, 38f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_tabButtonParts.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|item_type_parts", SHSButtonStyleInfo.SupportedStatesEnum.Normal);
		_tabButtonParts.ToolTip = new NamedToolTipInfo("#CRAFT_TAB_PARTS");
		_tabButtonParts.Click += tabParts;
		_content.Add(_tabButtonParts);
		_tabImageParts = new GUIImage();
		_tabImageParts.SetSize(50f, 50f);
		_tabImageParts.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(num + num3 * 2 - 5, num2 - 5));
		_tabImageParts.TextureSource = "mysquadgadget_bundle|item_type_parts_highlight";
		_content.Add(_tabImageParts);
		Vector2 a = new Vector2(25f, 80f);
		GUIImage gUIImage = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(266f, 54f), a + new Vector2(47f, 55f));
		gUIImage.TextureSource = "mysquadgadget_bundle|mshs_mysquad_heroes_searchfield";
		Add(gUIImage);
		GUIImage gUIImage2 = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(75f, 75f), gUIImage.Position + new Vector2(195f, -12f));
		gUIImage2.TextureSource = "persistent_bundle|gadget_searchbutton_normal";
		Add(gUIImage2);
		_searchText = GUIControl.CreateControlTopLeftFrame<GUITextField>(new Vector2(192f, 54f), gUIImage.Position + new Vector2(15f, -2f));
		_searchText.ControlName = "searchFieldForMySquadGadget";
		_searchText.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, GUILabel.GenColor(16, 44, 57), TextAnchor.MiddleLeft);
		_searchText.WordWrap = false;
		_searchText.Changed += delegate
		{
			filterByText(_searchText.Text);
		};
		Add(_searchText);
		_blueprintScrollPaneMask = new GUISimpleControlWindow();
		_blueprintScrollPaneMask.SetSize(279f, 370f);
		_blueprintScrollPaneMask.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, a + new Vector2(0f, 65f));
		_content.Add(_blueprintScrollPaneMask);
		_blueprintScrollPane = new GUISimpleControlWindow();
		_blueprintScrollPane.SetSize(300f, 6400f);
		_blueprintScrollPane.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f));
		_blueprintScrollPaneMask.Add(_blueprintScrollPane);
		_blueprintScrollPaneSlider = new GUISlider();
		_blueprintScrollPaneSlider.Changed += blueprintSliderChanged;
		_blueprintScrollPaneSlider.UseMouseWheelScroll = true;
		_blueprintScrollPaneSlider.MouseScrollWheelAmount = 2f;
		_blueprintScrollPaneSlider.TickValue = 40f;
		_blueprintScrollPaneSlider.ArrowsEnabled = true;
		_blueprintScrollPaneSlider.SetSize(40f, 370f);
		_blueprintScrollPaneSlider.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, _blueprintScrollPaneMask.Position + new Vector2(280f, -8f));
		_content.Add(_blueprintScrollPaneSlider);
		GUIImage gUIImage3 = new GUIImage();
		gUIImage3.SetSize(274f, 22f);
		gUIImage3.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(41f, 498f));
		gUIImage3.TextureSource = "mysquadgadget_bundle|navpanel_bottommask";
		_content.Add(gUIImage3);
		_blueprintImage = new GUISimpleControlWindow();
		_blueprintImage.SetSize(256f, 256f);
		_blueprintImage.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(_baseContentX - 20, _baseContentY));
		_content.Add(_blueprintImage);
		_blueprintOutputOwned = new GUIImage();
		_blueprintOutputOwned.SetSize(78f, 71f);
		_blueprintOutputOwned.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(_baseContentX + 487, _baseContentY + 212));
		_blueprintOutputOwned.TextureSource = "shopping_bundle|L_shopping_owned_indicator";
		_content.Add(_blueprintOutputOwned);
		_blueprintOutputOwned.IsVisible = false;
		_blueprintName = new GUIStrokeTextLabel();
		_blueprintName.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 20, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(63, 86, 0), GUILabel.GenColor(120, 170, 4), new Vector2(2f, 3f), TextAnchor.MiddleLeft);
		_blueprintName.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(_baseContentX + 200, _baseContentY + 40), new Vector2(350f, 65f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_blueprintName.WordWrap = true;
		_blueprintName.VerticalKerning += 8;
		_content.Add(_blueprintName);
		_blueprintDesc = new GUILabel();
		_blueprintDesc.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
		_blueprintDesc.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(_baseContentX + 200, _baseContentY + 100), new Vector2(350f, 130f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_content.Add(_blueprintDesc);
		_upgradeDisplay = new GUISimpleControlWindow();
		_upgradeDisplay.SetSize(500f, 256f);
		_upgradeDisplay.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(_baseContentX + 200, _baseContentY + 100));
		_content.Add(_upgradeDisplay);
		_upgradeHeader = new GUILabel();
		_upgradeHeader.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 14, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
		_upgradeHeader.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(300f, 130f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_upgradeHeader.WordWrap = true;
		_upgradeHeader.Text = "#CRAFTING_WINDOW_ABILITIES_HEADER";
		_upgradeDisplay.Add(_upgradeHeader);
		_upgradeHeader.IsVisible = false;
		for (int i = 0; i < 4; i++)
		{
			GUIImageWithEvents gUIImageWithEvents = new GUIImageWithEvents();
			gUIImageWithEvents.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 20 + i * 30), new Vector2(32f, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			_upgradeDisplay.Add(gUIImageWithEvents);
			gUIImageWithEvents.IsVisible = false;
			_upgradeIcons.Add(gUIImageWithEvents);
			GUILabel gUILabel = new GUILabel();
			gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 14, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
			gUILabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(40f, 25 + i * 30), new Vector2(300f, 130f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUILabel.WordWrap = true;
			_upgradeDisplay.Add(gUILabel);
			gUILabel.IsVisible = false;
			_upgradeLabels.Add(gUILabel);
		}
		_upgradeDisplay.IsVisible = false;
		for (int j = 0; j < 6; j++)
		{
			CraftingWindowPrimeContainer craftingWindowPrimeContainer = new CraftingWindowPrimeContainer(this);
			craftingWindowPrimeContainer.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(_baseContentX + 395 - j * 77, _baseContentY + 206));
			craftingWindowPrimeContainer.IsVisible = false;
			_content.Add(craftingWindowPrimeContainer);
			_primeContainers.Add(craftingWindowPrimeContainer);
		}
		_pieces = new GUISimpleControlWindow();
		_pieces.SetSize(600f, 800f);
		_pieces.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(_baseContentX, _baseContentY + 315));
		_content.Add(_pieces);
		_pieceList = new List<GUIControl>();
		_masterCraftButton = new GUIButton();
		_masterCraftButton.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(_baseContentX + 457, _baseContentY + 207), new Vector2(114f, 74f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_masterCraftButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|craft_item_prime_button", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		_masterCraftButton.ToolTip = new NamedToolTipInfo("#CRAFT_PRIME_CRAFTBUTTON");
		_masterCraftButton.Click += craftMaster;
		_masterCraftButton.IsVisible = false;
		_content.Add(_masterCraftButton);
		_masterCraftButtonDisabled = new GUIButton();
		_masterCraftButtonDisabled.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(_baseContentX + 457, _baseContentY + 207), new Vector2(114f, 74f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_masterCraftButtonDisabled.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|craft_item_prime_button_inactive", SHSButtonStyleInfo.SupportedStatesEnum.Normal);
		_masterCraftButtonDisabled.ToolTip = new NamedToolTipInfo("#CRAFT_PRIME_MISSING_ITEMS", new Vector2(50f, 0f));
		_masterCraftButtonDisabled.IsVisible = false;
		_masterCraftButtonDisabled.Click += craftMasterDisabled;
		_content.Add(_masterCraftButtonDisabled);
		GUIBundleManager bundleManager = GUIManager.Instance.BundleManager;
		bundleManager.LoadAsset("hud_bundle", "HUD_UI_CraftButton_Press_audio", null, delegate(UnityEngine.Object obj, AssetBundle bundle, object extraData)
		{
			_craftSFX = (obj as GameObject);
		});
		_disabledInfoWindow = new GUISimpleControlWindow();
		_disabledInfoWindow.SetSize(600f, 300f);
		_disabledInfoWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(_baseContentX, _baseContentY + 295));
		_content.Add(_disabledInfoWindow);
		_disabledInfoWindow.IsVisible = false;
		GUIButton gUIButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(64f, 64f), new Vector2(933f, 6f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|button_mysquad_close");
		gUIButton.Click += delegate
		{
			Close();
		};
		gUIButton.ToolTip = new NamedToolTipInfo("#TT_COMMON_1");
		Add(gUIButton);
		AppShell.Instance.EventMgr.AddListener<CraftCollectionUpdateMessage>(OnUpdateCraft);
		changeRecipeList("goodies");
		if (initialOwnableID != -1)
		{
			Blueprint blueprintFromOutput = BlueprintManager.getBlueprintFromOutput(initialOwnableID);
			if (blueprintFromOutput != null)
			{
				selectBlueprintByID(blueprintFromOutput.id);
				return;
			}
		}
		selectBlueprintByID(46);
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("global_window_in"));
	}

	public static void requestCraftingWindow(int initialOwnableID = -1)
	{
		if (!BlueprintManager.blueprintsLoaded)
		{
			BlueprintManager.loadBlueprints(true, initialOwnableID);
			CspUtils.DebugLogWarning("CraftingWindow requested, but the blueprints aren't loaded.  Loading now.");
		}
		else
		{
			new CraftingWindow(initialOwnableID).launch();
		}
	}

	public void launch()
	{
		GUIManager.Instance.ShowDynamicWindow(this, ModalLevelEnum.Default);
	}

	public void filterByText(string text)
	{
		_blueprintCurrY = 0;
		text = text.ToLower();
		int num = 0;
		foreach (LeftNavBlueprintContainer blueprintVis in _blueprintVisList)
		{
			blueprintVis.IsVisible = false;
			if (blueprintVis.blueprintName.Contains(text))
			{
				blueprintVis.IsVisible = true;
				blueprintVis.SetPosition(new Vector2(_blueprintCurrX, _blueprintCurrY));
				_blueprintCurrY += _blueprintDeltY;
				num++;
			}
		}
		_blueprintScrollPane.SetSize(300f, _blueprintCurrY + _blueprintDeltY);
		_blueprintScrollPaneSlider.Value = 0f;
		blueprintSliderChanged(null, null);
	}

	public void Close()
	{
		AppShell.Instance.EventMgr.RemoveListener<CraftCollectionUpdateMessage>(OnUpdateCraft);
		IsVisible = false;
		SHSInput.RevertInputBlockingMode(this);
		OnHide();
		Dispose();
	}

	private void changeRecipeList(string recipeListName)
	{
		if (!(currentRecipeList == recipeListName))
		{
			_tabButtonGoodies.IsVisible = true;
			_tabImageGoodies.IsVisible = false;
			_tabButtonSidekicks.IsVisible = true;
			_tabImageSidekicks.IsVisible = false;
			_tabButtonParts.IsVisible = true;
			_tabImageParts.IsVisible = false;
			currentRecipeList = recipeListName;
			switch (currentRecipeList)
			{
			case "goodies":
				_background.TextureSource = "mysquadgadget_bundle|mysquad_2panels_challengeandheroespage_01_crafting_tab01";
				_tabButtonGoodies.IsVisible = false;
				_tabImageGoodies.IsVisible = true;
				break;
			case "sidekicks":
				_background.TextureSource = "mysquadgadget_bundle|mysquad_2panels_challengeandheroespage_01_crafting_tab02";
				_tabButtonSidekicks.IsVisible = false;
				_tabImageSidekicks.IsVisible = true;
				break;
			case "parts":
				_background.TextureSource = "mysquadgadget_bundle|mysquad_2panels_challengeandheroespage_01_crafting_tab03";
				_tabButtonParts.IsVisible = false;
				_tabImageParts.IsVisible = true;
				break;
			}
			_searchText.Text = string.Empty;
			foreach (LeftNavBlueprintContainer blueprintVis in _blueprintVisList)
			{
				blueprintVis.Dispose();
			}
			_blueprintVisList.Clear();
			_blueprintScrollPane.RemoveAllControls();
			_blueprintCurrY = 0;
			_blueprintScrollPane.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, _blueprintCurrY));
			_blueprintScrollPaneSlider.Value = 0f;
			List<Blueprint> blueprints = BlueprintManager.getBlueprints(true);
			blueprints.Sort(new BlueprintSorter());
			foreach (Blueprint item in blueprints)
			{
				OwnableDefinition def = OwnableDefinition.getDef(item.outputOwnableTypeID);
				switch (def.category)
				{
				case OwnableDefinition.Category.Hero:
				case OwnableDefinition.Category.Badge:
				case OwnableDefinition.Category.Sidekick:
				case OwnableDefinition.Category.SidekickUpgrade:
					if (currentRecipeList != "sidekicks")
					{
						continue;
					}
					break;
				case OwnableDefinition.Category.Craft:
					if (currentRecipeList != "parts")
					{
						continue;
					}
					break;
				default:
					if (currentRecipeList != "goodies")
					{
						continue;
					}
					break;
				}
				addBlueprint(item);
			}
			_blueprintScrollPane.SetSize(300f, _blueprintCurrY + _blueprintDeltY);
			_blueprintScrollPaneSlider.Value = 0f;
			blueprintSliderChanged(null, null);
		}
	}

	private void addBlueprint(Blueprint blueprint)
	{
		OwnableDefinition def = OwnableDefinition.getDef(blueprint.outputOwnableTypeID);
		if (def == null)
		{
			CspUtils.DebugLog("couldn't find blueprint output ownable " + blueprint.outputOwnableTypeID);
			return;
		}
		if (def.released != 1)
		{
			CspUtils.DebugLog("hiding blueprint (" + blueprint.name + ") of unreleased ownable " + def.name);
			return;
		}
		if (blueprint.restricted == 1)
		{
			CspUtils.DebugLog("hiding restricted blueprint " + blueprint.name);
			return;
		}
		LeftNavBlueprintContainer leftNavBlueprintContainer = new LeftNavBlueprintContainer();
		leftNavBlueprintContainer.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(_blueprintCurrX, _blueprintCurrY), new Vector2(250f, 45f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		leftNavBlueprintContainer.blueprintID = blueprint.id;
		leftNavBlueprintContainer.blueprintName = AppShell.Instance.stringTable.GetString(blueprint.name).ToLower();
		BlueprintButton blueprintButton = new BlueprintButton();
		blueprintButton.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(260f, 45f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		blueprintButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|green_button", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		blueprintButton.ToolTip = new NamedToolTipInfo(AppShell.Instance.stringTable.GetString(blueprint.name));
		blueprintButton.HitTestSize = new Vector2(1f, 1f);
		blueprintButton.HitTestType = HitTestTypeEnum.Circular;
		blueprintButton.Click += selectBlueprint;
		blueprintButton.blueprintID = blueprint.id;
		leftNavBlueprintContainer.Add(blueprintButton);
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 14, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(63, 86, 0), GUILabel.GenColor(120, 170, 4), new Vector2(2f, 3f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(22f, -3f), new Vector2(210f, 45f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.VerticalKerning += 4;
		gUIStrokeTextLabel.WordWrap = true;
		gUIStrokeTextLabel.Text = AppShell.Instance.stringTable.GetString(blueprint.name);
		leftNavBlueprintContainer.Add(gUIStrokeTextLabel);
		_blueprintScrollPane.Add(leftNavBlueprintContainer);
		_blueprintVisList.Add(leftNavBlueprintContainer);
		_blueprintCurrY += _blueprintDeltY;
	}

	private void OnUpdateCraft(CraftCollectionUpdateMessage msg)
	{
		update();
	}

	private bool canCraftBlueprint(int blueprintID)
	{
		Blueprint blueprint = BlueprintManager.getBlueprint(blueprintID);
		return canCraftBlueprint(blueprint);
	}

	private bool canCraftBlueprint(Blueprint blueprint)
	{
		bool result = true;
		foreach (BlueprintPiece piece in blueprint.pieces)
		{
			if (getInventoryOfCraft(piece.ownableTypeID) < piece.quantity)
			{
				result = false;
			}
		}
		return result;
	}

	private void update()
	{
		if (_currentBlueprintID == -1)
		{
			_masterCraftButton.IsVisible = false;
			_masterCraftButtonDisabled.IsVisible = false;
			return;
		}
		Blueprint blueprint = BlueprintManager.getBlueprint(_currentBlueprintID);
		if (blueprint == null)
		{
			_masterCraftButton.IsVisible = false;
			_masterCraftButtonDisabled.IsVisible = false;
			_currentBlueprintID = -1;
			return;
		}
		_craftUnlocked = false;
		_masterCraftButtonDisabled.IsVisible = true;
		_masterCraftButton.IsVisible = false;
		_blueprintImage.RemoveAllControls();
		_disabledInfoWindow.RemoveAllControls();
		OwnableDefinition def = OwnableDefinition.getDef(blueprint.outputOwnableTypeID);
		_blueprintName.Text = AppShell.Instance.stringTable.GetString(blueprint.name);
		if (def.category == OwnableDefinition.Category.SidekickUpgrade)
		{
			GUIImage gUIImage = new GUIImage();
			gUIImage.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			CspUtils.DebugLog(def.name.Substring(def.name.Length - 1, 1));
			if (def.name.Substring(def.name.Length - 1, 1) == "2")
			{
				gUIImage.TextureSource = "shopping_bundle|badge";
			}
			else
			{
				gUIImage.TextureSource = "shopping_bundle|badge_silver";
			}
			_blueprintImage.Add(gUIImage);
			PetData data = PetDataManager.getData(Convert.ToInt32(def.metadata));
			gUIImage = new GUIImage();
			gUIImage.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(103f, 103f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIImage.TextureSource = "shopping_bundle|" + data.inventoryIconBase + "_normal";
			_blueprintImage.Add(gUIImage);
			_blueprintDesc.Text = string.Empty;
			_upgradeDisplay.IsVisible = true;
			_upgradeHeader.IsVisible = true;
			int i = 0;
			foreach (SpecialAbility ability in data.abilities)
			{
				if (ability.requiredOwnable == def.ownableTypeID)
				{
					_upgradeIcons[i].IsVisible = true;
					_upgradeIcons[i].TextureSource = ability.icon;
					if (ability.iconSize != Vector2.zero)
					{
						_upgradeIcons[i].Size = ability.iconSize;
					}
					_upgradeLabels[i].IsVisible = true;
					_upgradeLabels[i].Text = ability.name;
					i++;
				}
			}
			for (; i < 4; i++)
			{
				_upgradeIcons[i].IsVisible = false;
				_upgradeLabels[i].IsVisible = false;
			}
		}
		else
		{
			_upgradeDisplay.IsVisible = false;
			_upgradeHeader.IsVisible = false;
			GUIImage gUIImage;
			if (blueprint.id == 112)
			{
				gUIImage = new GUIImage();
				gUIImage.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(90f, 95f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				gUIImage.TextureSource = "shopping_bundle|destroyer_summon_portal_01";
			}
			else
			{
				gUIImage = new GUIImage();
				gUIImage.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f), def.shoppingIconSize, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				gUIImage.TextureSource = def.shoppingIcon;
			}
			_blueprintImage.Add(gUIImage);
			_blueprintDesc.Text = AppShell.Instance.stringTable.GetString(blueprint.description);
			if (def.category == OwnableDefinition.Category.Badge)
			{
				gUIImage.Size = new Vector2(128f, 128f);
				OwnableDefinition def2 = OwnableDefinition.getDef(Convert.ToInt32(def.metadata));
				if (def2 != null)
				{
					GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(103f, 103f), new Vector2(0f, 0f));
					gUIImage2.TextureSource = "characters_bundle|token_" + def2.name + string.Empty;
					gUIImage2.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
					_blueprintImage.Add(gUIImage2);
				}
			}
		}
		_pieceCurrY = 0;
		int num = 0;
		bool flag = true;
		BlueprintPiece blueprintPiece = null;
		int num2 = 0;
		foreach (BlueprintPiece piece in blueprint.pieces)
		{
			num2 = getInventoryOfCraft(piece.ownableTypeID);
			_primeContainers[num].setQuantity(num2);
			_primeContainers[num].setState(num2 >= piece.quantity, piece.ownableTypeID == _selectedPieceOwnableTypeID);
			if (num2 < piece.quantity)
			{
				flag = false;
			}
			num++;
			if (piece.ownableTypeID == _selectedPieceOwnableTypeID)
			{
				blueprintPiece = piece;
			}
		}
		foreach (int prereq in blueprint.prereqs)
		{
			if (!OwnableDefinition.isOwned(prereq, AppShell.Instance.Profile))
			{
				flag = false;
				break;
			}
		}
		_pieces.RemoveAllControls();
		foreach (GUIControl piece2 in _pieceList)
		{
			piece2.Dispose();
		}
		_pieceList.Clear();
		List<GUISimpleControlWindow> list = new List<GUISimpleControlWindow>();
		_topTierItemQuantityHolder = new GUIImage();
		_topTierItemQuantityHolder.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(446f, 60f), new Vector2(101f, 67f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_topTierItemQuantityHolder.TextureSource = "mysquadgadget_bundle|craft_item_bobble";
		_pieceList.Add(_topTierItemQuantityHolder);
		_topTierItemQuantityLabel = new GUILabel();
		_topTierItemQuantityLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 20, GUILabel.GenColor(114, 166, 24), TextAnchor.MiddleCenter);
		_topTierItemQuantityLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(446f, 60f), new Vector2(101f, 67f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_topTierItemQuantityLabel.Text = "3/7";
		_pieceList.Add(_topTierItemQuantityLabel);
		if (blueprintPiece != null && NewShoppingManager.CatalogOwnableMap.ContainsKey(blueprintPiece.ownableTypeID))
		{
			_shoppingButton = new GUIButton();
			_shoppingButton.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(496f, -3f), new Vector2(64f, 76f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			_shoppingButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|shopping_button", SHSButtonStyleInfo.SupportedStatesEnum.Normal);
			_shoppingButton.ToolTip = new NamedToolTipInfo("#CRAFT_BUY_IN_SHOP");
			_shoppingButton.Click += clickedBuy;
			_pieces.Add(_shoppingButton);
			_pieceList.Add(_shoppingButton);
		}
		if (blueprintPiece != null)
		{
			_pieceCurrX = 395;
			_pieceCurrY = 0;
			GUISimpleControlWindow gUISimpleControlWindow = createCraftUI(blueprintPiece.ownableTypeID);
			gUISimpleControlWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(_pieceCurrX, _pieceCurrY));
			_pieces.Add(gUISimpleControlWindow);
			_pieceList.Add(gUISimpleControlWindow);
			_topTierItemQuantityLabel.Text = getInventoryOfCraft(blueprintPiece.ownableTypeID) + "/" + blueprintPiece.quantity;
			Blueprint blueprint2 = BlueprintManager.getBlueprintFromOutput(blueprintPiece.ownableTypeID);
			if (blueprint2 != null)
			{
				gUISimpleControlWindow = createSingleCraftButtonUI(blueprint2);
				gUISimpleControlWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(_pieceCurrX + _pieceCraftOffsetX, _pieceCurrY + _pieceCraftOffsetY));
				_pieceList.Add(gUISimpleControlWindow);
				list.Add(gUISimpleControlWindow);
			}
			while (blueprint2 != null)
			{
				_pieceCurrX += _pieceDeltX;
				gUISimpleControlWindow = createCraftUI(blueprint2.pieces[0].ownableTypeID);
				gUISimpleControlWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(_pieceCurrX, _pieceCurrY));
				_pieceList.Add(gUISimpleControlWindow);
				_pieces.Add(gUISimpleControlWindow);
				Blueprint blueprintFromOutput = BlueprintManager.getBlueprintFromOutput(blueprint2.pieces[0].ownableTypeID);
				if (blueprintFromOutput != null && blueprintFromOutput.outputOwnableTypeID == blueprint2.outputOwnableTypeID)
				{
					CspUtils.DebugLog("ERROR:  circular reference detected in blueprints");
					break;
				}
				blueprint2 = blueprintFromOutput;
				if (blueprint2 != null)
				{
					gUISimpleControlWindow = createSingleCraftButtonUI(blueprint2);
					gUISimpleControlWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(_pieceCurrX + _pieceCraftOffsetX, _pieceCurrY + _pieceCraftOffsetY));
					_pieceList.Add(gUISimpleControlWindow);
					list.Add(gUISimpleControlWindow);
				}
			}
			foreach (GUISimpleControlWindow item in list)
			{
				_pieces.Add(item);
			}
			list.Clear();
			_pieces.Add(_topTierItemQuantityHolder);
			_pieces.Add(_topTierItemQuantityLabel);
		}
		_blueprintOutputOwned.IsVisible = false;
		if (OwnableDefinition.isUniqueAndOwned(blueprint.outputOwnableTypeID, AppShell.Instance.Profile))
		{
			_masterCraftButtonDisabled.IsVisible = false;
			_masterCraftButton.IsVisible = false;
			_blueprintOutputOwned.IsVisible = true;
			return;
		}
		if (flag)
		{
			_craftUnlocked = true;
			_masterCraftButtonDisabled.IsVisible = false;
			_masterCraftButton.IsVisible = true;
			return;
		}
		_masterCraftButtonDisabled.IsVisible = true;
		_masterCraftButtonDisabled.ToolTip = new NamedToolTipInfo("#CRAFT_PRIME_MISSING_ITEMS", new Vector2(50f, 0f));
		_masterCraftButton.IsVisible = false;
		if (blueprintPiece == null)
		{
			craftMasterDisabled(null, null);
		}
	}

	private int getInventoryOfCraft(int ownableTypeID)
	{
		GenericCollectionItem value = null;
		if (!AppShell.Instance.Profile.Crafts.TryGetValue(string.Empty + ownableTypeID, out value))
		{
			return 0;
		}
		return value.Quantity;
	}

	private void blueprintSliderChanged(GUIControl sender, GUIChangedEvent eventData)
	{
		float num = (float)_blueprintCurrY / (float)_blueprintDeltY;
		_blueprintScrollPane.Offset = new Vector2(0f, (0f - _blueprintScrollPaneSlider.Value) * (num / 100f) * (float)_blueprintDeltY);
	}

	protected void tabGoodies(GUIControl sender, GUIClickEvent EventData)
	{
		changeRecipeList("goodies");
	}

	protected void tabSidekicks(GUIControl sender, GUIClickEvent EventData)
	{
		changeRecipeList("sidekicks");
	}

	protected void tabParts(GUIControl sender, GUIClickEvent EventData)
	{
		changeRecipeList("parts");
	}

	protected void craftMasterDisabled(GUIControl sender, GUIClickEvent EventData)
	{
		_disabledInfoWindow.RemoveAllControls();
		_pieces.RemoveAllControls();
		foreach (GUIControl piece in _pieceList)
		{
			piece.Dispose();
		}
		_pieceList.Clear();
		Blueprint blueprint = BlueprintManager.getBlueprint(_currentBlueprintID);
		bool flag = false;
		foreach (int prereq in blueprint.prereqs)
		{
			if (!OwnableDefinition.isOwned(prereq, AppShell.Instance.Profile))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			_masterCraftButtonDisabled.ToolTip = new NamedToolTipInfo("#CRAFT_PRIME_MISSING_PREREQ", new Vector2(50f, 0f));
			GUILabel gUILabel = new GUILabel();
			gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 13, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
			gUILabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(5f, 8f), new Vector2(530f, 130f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			_disabledInfoWindow.Add(gUILabel);
			gUILabel.Text = "#CRAFTING_WINDOW_PREREQ_LABEL";
			int num = 0;
			foreach (int prereq2 in blueprint.prereqs)
			{
				if (!OwnableDefinition.isOwned(prereq2, AppShell.Instance.Profile))
				{
					OwnableDefinition def = OwnableDefinition.getDef(prereq2);
					Vector2 size = def.shoppingIconSize;
					float num2 = 1f;
					if (size.x > 70f || size.y > 70f)
					{
						num2 = size.x / 100f;
						if (size.y > size.x)
						{
							num2 = size.y / 100f;
						}
						size = new Vector2(size.x / num2, size.y / num2);
					}
					if (def.category == OwnableDefinition.Category.SidekickUpgrade)
					{
						GUIImage gUIImage = new GUIImage();
						gUIImage.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-200 + num * 100, -70f), new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
						gUIImage.TextureSource = "shopping_bundle|badge_silver";
						_disabledInfoWindow.Add(gUIImage);
					}
					GUIImageWithEvents gUIImageWithEvents = new GUIImageWithEvents();
					gUIImageWithEvents.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-200 + num * 100, -70f), size, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
					gUIImageWithEvents.TextureSource = def.iconFullPath;
					gUIImageWithEvents.ToolTip = new NamedToolTipInfo(def.name, Vector2.zero);
					gUIImageWithEvents.Click += clickedDisabledItem;
					gUIImageWithEvents.data = string.Empty + prereq2;
					_disabledInfoWindow.Add(gUIImageWithEvents);
					num++;
				}
			}
		}
		else if (canCraftBlueprint(blueprint))
		{
		}
		_disabledInfoWindow.IsVisible = true;
	}

	protected void clickedDisabledItem(GUIControl sender, GUIClickEvent EventData)
	{
		string data = (sender as GUIImageWithEvents).data;
		int ownableTypeID = Convert.ToInt32(data);
		Blueprint blueprintFromOutput = BlueprintManager.getBlueprintFromOutput(ownableTypeID);
		if (blueprintFromOutput == null)
		{
			CspUtils.DebugLog("\tclickedDisabledItem got a null blueprint, oops?");
		}
		else
		{
			selectBlueprintByID(blueprintFromOutput.id);
		}
	}

	protected void craftMaster(GUIControl sender, GUIClickEvent EventData)
	{
		AppShell.Instance.StartCoroutine(ToggleIconLocks());
		CraftBlueprintService.CraftBlueprint(_currentBlueprintID, null);
		ShsAudioSource.PlayAutoSound(_craftSFX);
	}

	protected void clickedBuy(GUIControl sender, GUIClickEvent EventData)
	{
		if (_selectedPieceOwnableTypeID != -1)
		{
			ShoppingWindow shoppingWindow = new ShoppingWindow(_selectedPieceOwnableTypeID);
			shoppingWindow.launch();
		}
	}

	protected void selectBlueprintByID(int id)
	{
		_currentBlueprintID = id;
		_selectedPieceOwnableTypeID = -1;
		for (int i = 0; i < _primeContainers.Count; i++)
		{
			_primeContainers[i].IsVisible = false;
		}
		Blueprint blueprint = BlueprintManager.getBlueprint(_currentBlueprintID);
		if (blueprint != null)
		{
			int num = 0;
			foreach (BlueprintPiece piece in blueprint.pieces)
			{
				_primeContainers[num].setup(OwnableDefinition.getDef(piece.ownableTypeID), piece.quantity, getInventoryOfCraft(piece.ownableTypeID));
				num++;
			}
		}
		update();
	}

	protected void selectBlueprint(GUIControl sender, GUIClickEvent EventData)
	{
		if (sender is BlueprintButton)
		{
			selectBlueprintByID((sender as BlueprintButton).blueprintID);
		}
		else
		{
			CspUtils.DebugLog("Got selectBlueprint from a control that isn't a proper button?");
		}
	}

	public void selectPiece(int ownableTypeID)
	{
		_disabledInfoWindow.IsVisible = false;
		_selectedPieceOwnableTypeID = ownableTypeID;
		update();
	}

	public GUISimpleControlWindow createCraftUI(int pieceOwnableID)
	{
		GUISimpleControlWindow gUISimpleControlWindow = new GUISimpleControlWindow();
		gUISimpleControlWindow.SetSize(100f, 100f);
		OwnableDefinition def = OwnableDefinition.getDef(pieceOwnableID);
		Vector2 offset = new Vector2(50f, 0f);
		GUIButton gUIButton = new GUIButton();
		gUIButton.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(90f, 95f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|container_crafting_items", SHSButtonStyleInfo.SupportedStatesEnum.Normal);
		gUISimpleControlWindow.Add(gUIButton);
		gUIButton.ToolTip = new NamedToolTipInfo(def.name, offset);
		GUIImage gUIImage = new GUIImage();
		gUIImage.SetSize(90f, 95f);
		gUIImage.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-5f, -5f));
		gUIImage.TextureSource = def.iconBase;
		gUISimpleControlWindow.Add(gUIImage);
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 14, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(63, 86, 0), GUILabel.GenColor(120, 170, 4), new Vector2(2f, 2f), TextAnchor.UpperRight);
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(2f, 80f), new Vector2(92f, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.WordWrap = true;
		gUIStrokeTextLabel.Text = "x" + getInventoryOfCraft(pieceOwnableID);
		gUISimpleControlWindow.Add(gUIStrokeTextLabel);
		return gUISimpleControlWindow;
	}

	public GUISimpleControlWindow createSingleCraftButtonUI(Blueprint blueprint)
	{
		GUISimpleControlWindow gUISimpleControlWindow = new GUISimpleControlWindow();
		gUISimpleControlWindow.SetSize(134f, 72f);
		int num = 0;
		int num2 = 0;
		string text = string.Empty;
		bool flag = true;
		OwnableDefinition def;
		foreach (BlueprintPiece piece in blueprint.pieces)
		{
			def = OwnableDefinition.getDef(piece.ownableTypeID);
			if (def != null)
			{
				text = AppShell.Instance.stringTable.GetString(def.name);
			}
			num = piece.quantity;
			num2 = getInventoryOfCraft(piece.ownableTypeID);
			if (num2 < num)
			{
				flag = false;
				break;
			}
		}
		string text2 = string.Empty;
		def = OwnableDefinition.getDef(blueprint.outputOwnableTypeID);
		if (def != null)
		{
			text2 = AppShell.Instance.stringTable.GetString(def.name);
		}
		string @string = AppShell.Instance.stringTable.GetString("#CRAFT_SUB_BUTTON_TEXT");
		@string = string.Format(@string, num, text, blueprint.outputQuantity, text2);
		if (flag)
		{
			CraftBlueprintButton craftBlueprintButton = GUIControl.CreateControlFrameCentered<CraftBlueprintButton>(new Vector2(134f, 72f), new Vector2(0f, 0f));
			craftBlueprintButton.blueprintID = blueprint.id;
			craftBlueprintButton.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f));
			craftBlueprintButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|craft_item_button", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
			gUISimpleControlWindow.Add(craftBlueprintButton);
			craftBlueprintButton.ToolTip = new NamedToolTipInfo(@string, new Vector2(50f, 0f));
			craftBlueprintButton.Click += craftBlueprint;
		}
		else
		{
			GUIButton gUIButton = new GUIButton();
			gUIButton.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(134f, 72f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|craft_item_button_inactive", SHSButtonStyleInfo.SupportedStatesEnum.Normal);
			gUIButton.ToolTip = new NamedToolTipInfo("#CRAFT_PRIME_CRAFTBUTTON");
			gUISimpleControlWindow.Add(gUIButton);
			gUIButton.ToolTip = new NamedToolTipInfo(@string, new Vector2(50f, 0f));
		}
		GUILabel gUILabel = new GUILabel();
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(114, 166, 24), TextAnchor.MiddleCenter);
		gUILabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(5f, 20f), new Vector2(80f, 30f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUILabel.Text = num2 + "/" + num;
		gUISimpleControlWindow.Add(gUILabel);
		return gUISimpleControlWindow;
	}

	protected void craftBlueprint(GUIControl sender, GUIClickEvent EventData)
	{
		if (sender is CraftBlueprintButton)
		{
			if (!craftLocked)
			{
				AppShell.Instance.StartCoroutine(ToggleIconLocks());
				int blueprintID = (sender as CraftBlueprintButton).blueprintID;
				CraftBlueprintService.CraftBlueprint(blueprintID, null);
				ShsAudioSource.PlayAutoSound(_craftSFX);
			}
		}
		else
		{
			CspUtils.DebugLog("Got craftBlueprint from a control that isn't a CraftBlueprintButton button?");
		}
	}

	protected IEnumerator ToggleIconLocks()
	{
		setIconLock(true);
		yield return new WaitForSeconds(1f);
		setIconLock(false);
	}

	protected void setIconLock(bool locked)
	{
		craftLocked = locked;
		Blueprint blueprint = BlueprintManager.getBlueprint(_currentBlueprintID);
		if (blueprint != null && !OwnableDefinition.isUniqueAndOwned(blueprint.outputOwnableTypeID, AppShell.Instance.Profile))
		{
			_masterCraftButtonDisabled.IsVisible = !_craftUnlocked;
			_masterCraftButton.IsVisible = _craftUnlocked;
		}
	}
}
