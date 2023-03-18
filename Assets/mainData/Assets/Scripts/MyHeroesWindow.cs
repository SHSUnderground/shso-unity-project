using MySquadChallenge;
using System.Collections.Generic;
using UnityEngine;

public class MyHeroesWindow : GUIDialogWindow
{
	private GUIImage _background;

	private GUIImage _underlay;

	private GUISimpleControlWindow _content;

	public static MyHeroesWindow instance;

	private SHSMyHeroesWindow _myHeroesWindow;

	private MySquadDataManager _dataManager;

	protected string _currentDisplayHero = string.Empty;

	protected string _defaultHero = string.Empty;

	private HeroViewer _heroViewer;

	private List<GUIStrokeTextLabel> _tabLabels = new List<GUIStrokeTextLabel>();

	private Dictionary<string, GUIButton> _tabButtons = new Dictionary<string, GUIButton>();

	private Dictionary<string, GUIImage> _tabSelectedImages = new Dictionary<string, GUIImage>();

	private GUIButton PlayButton;

	public static int forceSelectedID = -1;

	private bool _profileLoaded;

	public MyHeroesWindow(MySquadDataManager dataManager)
	{
		_dataManager = dataManager;
	}

	public override void Update()
	{
		base.Update();
		if (!_profileLoaded && _dataManager.Profile != null)
		{
			_profileLoaded = true;
			init();
		}
	}

	public new void init()
	{
		instance = this;
		Traits.FullScreenOpaqueBackgroundTrait = ControlTraits.FullScreenOpaqueBackgroundTraitEnum.HasFullScreenOpaqueBackground;
		Traits.EventListenerRegistrationTrait = ControlTraits.EventListenerRegistrationTraitEnum.Ignore;
		SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		SetPosition(QuickSizingHint.Centered);
		_background = new GUIImage();
		_background.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		_background.Position = Vector2.zero;
		_background.TextureSource = "mysquadgadget_bundle|bg";
		Add(_background);
		_content = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(974f, 547f), new Vector2(0f, 5f));
		_content.IsVisible = true;
		Add(_content);
		_underlay = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(974f, 547f), Vector2.zero);
		_underlay.TextureSource = "mysquadgadget_bundle|notabs";
		_content.Add(_underlay);
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, -290f), new Vector2(200f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.BackColorAlpha = 1f;
		gUIStrokeTextLabel.StrokeColorAlpha = 1f;
		gUIStrokeTextLabel.WordWrap = true;
		if (_dataManager.isLocalPlayer)
		{
			gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 24, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 43, 68), GUILabel.GenColor(0, 43, 68), new Vector2(3f, 4f), TextAnchor.MiddleCenter);
			gUIStrokeTextLabel.Text = "#HEROLIST_MYHEROES";
		}
		else
		{
			gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 14, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 43, 68), GUILabel.GenColor(0, 43, 68), new Vector2(3f, 4f), TextAnchor.MiddleCenter);
			gUIStrokeTextLabel.VerticalKerning = 15;
			gUIStrokeTextLabel.Text = AchievementManager.formatAchievementString(_dataManager.SquadName + "'s #MYHEROES_OTHER_SQUAD_HEROES", 0, 0, string.Empty);
			gUIStrokeTextLabel.VerticalKerning = 15;
		}
		gUIStrokeTextLabel.IsVisible = true;
		Add(gUIStrokeTextLabel);
		GUIStrokeTextLabel gUIStrokeTextLabel2 = new GUIStrokeTextLabel();
		gUIStrokeTextLabel2.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(369f, 25f), new Vector2(200f, 40f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel2.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(2, 34, 90), GUILabel.GenColor(2, 34, 90), new Vector2(2f, 3f), TextAnchor.UpperLeft);
		gUIStrokeTextLabel2.BackColorAlpha = 1f;
		gUIStrokeTextLabel2.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUIStrokeTextLabel2.Text = "#MYSQUAD_ABOUT_THIS_HERO";
		_content.Add(gUIStrokeTextLabel2);
		addTab("info", "#MYSQUAD_INFO", 0);
		addTab("powers", "#MYSQUAD_POWERS", 1);
		string currentDisplayHero = createHeroSelector();
		_heroViewer = new HeroViewer(_dataManager);
		_heroViewer.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, new Vector2(369f, 63f));
		_heroViewer.SetSize(new Vector2(590f, 450f));
		_heroViewer.init();
		_content.Add(_heroViewer);
		GUIButton gUIButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(64f, 64f), new Vector2(933f, 6f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|button_mysquad_close");
		gUIButton.Click += delegate
		{
			_currentDisplayHero = _defaultHero;
			Close();
		};
		gUIButton.ToolTip = new NamedToolTipInfo("#TT_COMMON_1");
		Add(gUIButton);
		if (_dataManager.isLocalPlayer)
		{
			PlayButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(325f, 50f), new Vector2(300f, 450f));
			PlayButton.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -1f), new Vector2(325f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			PlayButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|playbutton");
			PlayButton.Click += delegate
			{
				Close();
			};
			PlayButton.ToolTip = new NamedToolTipInfo("#TT_COMMON_1");
			Add(PlayButton);
		}
		else
		{
			CspUtils.DebugLog("viewing squad for player " + _dataManager.SquadName);
			GUIButton gUIButton2 = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(325f, 50f), new Vector2(300f, 450f));
			gUIButton2.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -1f), new Vector2(325f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIButton2.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|L_mshs_mysquad_challenge_quit");
			gUIButton2.Click += delegate
			{
				Close();
			};
			Add(gUIButton2);
		}
		if (_dataManager.isLocalPlayer)
		{
			AppShell.Instance.EventMgr.AddListener<ShoppingItemPurchasedMessage>(OnShoppingItemPurchasedMessage);
		}
		HeroDefinition heroDefinition = null;
		if (forceSelectedID != -1)
		{
			heroDefinition = OwnableDefinition.getHeroDef(forceSelectedID);
		}
		forceSelectedID = -1;
		if (_dataManager.isLocalPlayer && heroDefinition != null && AppShell.Instance.Profile.AvailableCostumes.ContainsKey(heroDefinition.name))
		{
			_currentDisplayHero = heroDefinition.name;
		}
		else if (_dataManager.isLocalPlayer && AppShell.Instance.Profile.SelectedCostume != string.Empty)
		{
			_currentDisplayHero = _dataManager.Profile.LastSelectedCostume;
		}
		else
		{
			_currentDisplayHero = currentDisplayHero;
		}
		_defaultHero = _currentDisplayHero;
		_heroViewer.setHero(_currentDisplayHero);
		showTab("info");
	}

	private void addTab(string ident, string labelText, int offset)
	{
		Vector2 a = new Vector2(620f, 17f);
		GUIButton gUIButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(84f, 35f), a + new Vector2(84 * offset, 0f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|pah_tab", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		gUIButton.Click += delegate
		{
			showTab(ident);
		};
		_content.Add(gUIButton);
		_tabButtons.Add(ident, gUIButton);
		GUIImage gUIImage = GUIControl.CreateControlTopLeftFrame<GUIImage>(gUIButton.Size, gUIButton.Position);
		gUIImage.TextureSource = "mysquadgadget_bundle|pah_tab_highlight";
		_content.Add(gUIImage);
		gUIImage.IsVisible = false;
		_tabSelectedImages.Add(ident, gUIImage);
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, gUIButton.Position + new Vector2(0f, 0f), gUIButton.Size, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel.BackColorAlpha = 1f;
		gUIStrokeTextLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUIStrokeTextLabel.Text = labelText;
		_content.Add(gUIStrokeTextLabel);
	}

	private void hideTabs()
	{
		foreach (string key in _tabButtons.Keys)
		{
			GUIButton gUIButton = _tabButtons[key];
			gUIButton.IsVisible = true;
			gUIButton.IsEnabled = false;
		}
		foreach (string key2 in _tabSelectedImages.Keys)
		{
			GUIImage gUIImage = _tabSelectedImages[key2];
			gUIImage.IsVisible = false;
		}
	}

	private void showTab(string tab, bool broadcast = true)
	{
		if (broadcast)
		{
			_heroViewer.showTab(tab);
		}
		foreach (string key in _tabButtons.Keys)
		{
			GUIButton gUIButton = _tabButtons[key];
			gUIButton.IsEnabled = true;
			if (key == tab)
			{
				gUIButton.IsVisible = false;
			}
			else
			{
				gUIButton.IsVisible = true;
			}
		}
		foreach (string key2 in _tabSelectedImages.Keys)
		{
			GUIImage gUIImage = _tabSelectedImages[key2];
			if (key2 != tab)
			{
				gUIImage.IsVisible = false;
			}
			else
			{
				gUIImage.IsVisible = true;
			}
		}
	}

	private string createHeroSelector()
	{
		if (_myHeroesWindow != null)
		{
			_content.Remove(_myHeroesWindow);
			_myHeroesWindow.Dispose();
		}
		List<SHSHeroSelectionItem> heroListFromProfile = SHSMyHeroesWindow.GetHeroListFromProfile(_dataManager.Profile, OnHeroClick);
		if (_dataManager.isLocalPlayer)
		{
			// SHSMyHeroesWindow.AddShopHeroesToHeroList(OnHeroClick, heroListFromProfile);  // commented out by CSP
		}
		_myHeroesWindow = new SHSMyHeroesWindow(heroListFromProfile);
		_myHeroesWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(19f, 10f));
		_content.Add(_myHeroesWindow);
		return heroListFromProfile[0].HeroName;
	}

	private void OnShoppingItemPurchasedMessage(ShoppingItemPurchasedMessage msg)
	{
		OwnableDefinition def = OwnableDefinition.getDef(int.Parse(msg.OwnableId));
		if (def == null)
		{
			return;
		}
		if (def.category == OwnableDefinition.Category.Hero)
		{
			createHeroSelector();
		}
		if (def.name == _currentDisplayHero)
		{
			_heroViewer.setHero(_currentDisplayHero);
		}
		else if (def.category == OwnableDefinition.Category.Badge)
		{
			OwnableDefinition def2 = OwnableDefinition.getDef(int.Parse(def.metadata));
			if (def2 != null && def2.name == _currentDisplayHero)
			{
				_heroViewer.setHero(_currentDisplayHero);
			}
		}
	}

	public void Close()
	{
		if (_dataManager.isLocalPlayer)
		{
			if (AppShell.Instance.Profile.LastSelectedCostume != _currentDisplayHero && AchievementManager.shouldReportAchievementEvent("generic_event", "swap_hero", string.Empty))
			{
				AppShell.Instance.delayedAchievementEvent(_currentDisplayHero, "generic_event", "swap_hero", string.Empty, 3f);
			}
			if (AppShell.Instance.Profile != null)
			{
				AppShell.Instance.Profile.LastSelectedCostume = _currentDisplayHero;
				AppShell.Instance.Profile.SelectedCostume = _currentDisplayHero;
				AppShell.Instance.Profile.PersistExtendedData();
			}
			AppShell.Instance.EventMgr.Fire(this, new CharacterSelectedMessage(_currentDisplayHero));
			AppShell.Instance.EventMgr.RemoveListener<ShoppingItemPurchasedMessage>(this, OnShoppingItemPurchasedMessage);
		}
		IsVisible = false;
		VOManager.Instance.StopAll();
		OnHide();
		Dispose();
	}

	public void OnHeroClick(string hero)
	{
		_heroViewer.setHero(hero);
		showTab("info", false);
		bool flag = Random.value <= 0.2f;
		bool flag2 = false;
		if (flag)
		{
			flag2 = (VOManager.Instance.PlayVO("alt_character_name", new VOInputString(hero)) != null);
		}
		if (!flag || !flag2)
		{
			VOManager.Instance.PlayVO("character_name", new VOInputString(hero));
		}
		if (!_dataManager.Profile.AvailableCostumes.ContainsKey(hero))
		{
			hideTabs();
		}
		if (PlayButton != null)
		{
			if (_dataManager.Profile.AvailableCostumes.ContainsKey(hero))
			{
				PlayButton.IsEnabled = true;
			}
			else
			{
				PlayButton.IsEnabled = false;
			}
		}
		_currentDisplayHero = hero;
	}
}
