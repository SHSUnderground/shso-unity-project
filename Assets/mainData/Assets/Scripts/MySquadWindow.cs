using MySquadChallenge;
using System.Collections.Generic;
using UnityEngine;

public class MySquadWindow : GUIDialogWindow
{
	private GUIImage _background;

	private GUIImage _underlay;

	private GUISimpleControlWindow _content;

	public static MySquadWindow instance;

	private SHSMyHeroesWindow _myHeroesWindow;

	private MySquadDataManager _dataManager;

	protected string _currentDisplayHero = string.Empty;

	protected string _defaultHero = string.Empty;

	private GUISimpleControlWindow _iconHolder;

	private GUISimpleControlWindow _icon;

	private GUISimpleControlWindow _sidekickIconHolder;

	private GUISimpleControlWindow _sidekickIcon;

	private List<GUIStrokeTextLabel> _tabLabels = new List<GUIStrokeTextLabel>();

	private Dictionary<string, GUIButton> _tabButtons = new Dictionary<string, GUIButton>();

	private Dictionary<string, GUIImage> _tabSelectedImages = new Dictionary<string, GUIImage>();

	private Vector2 HeroIconSize = new Vector2(180f, 180f);

	private Vector2 SidekickIconSize = new Vector2(100f, 100f);

	private ChooseTitleHUD chooseTitleHUD;

	public static Vector2 TITLE_WINDOW_OPENED = new Vector2(400f, 120f);

	public static Vector2 TITLE_WINDOW_CLOSED = new Vector2(-800f, 120f);

	private GUIImageWithEvents changeMedallionButton;

	private ChooseMedallionHUD chooseMedallionHUD;

	public static Vector2 MEDALLION_WINDOW_OPENED = new Vector2(0f, 190f);

	public static Vector2 MEDALLION_WINDOW_CLOSED = new Vector2(-500f, 190f);

	protected ChoosePetHUD petHUD;

	public static Vector2 PET_WINDOW_OPENED = new Vector2(0f, 190f);

	public static Vector2 PET_WINDOW_CLOSED = new Vector2(-500f, 190f);

	private GUIButton PlayButton;

	private GUIStrokeTextLabel titleLabel;

	private GUIStrokeTextLabel _heroFamilyLabel;

	private bool _profileLoaded;

	public MySquadWindow(MySquadDataManager dataManager)
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
		PlayerStatus.SetLocalStatus(PlayerStatusDefinition.Instance.GetStatus("MySquad"));
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
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(0f, 20f), new Vector2(200f, 30f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 24, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 43, 68), GUILabel.GenColor(0, 43, 68), new Vector2(3f, 4f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel.BackColorAlpha = 1f;
		gUIStrokeTextLabel.StrokeColorAlpha = 1f;
		gUIStrokeTextLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUIStrokeTextLabel.Text = "#MYSQUAD_WINDOW_TITLE";
		gUIStrokeTextLabel.IsVisible = true;
		Add(gUIStrokeTextLabel);
		PlayButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(325f, 50f), new Vector2(300f, 450f));
		PlayButton.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -1f), new Vector2(325f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		PlayButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|playbutton");
		PlayButton.Click += delegate
		{
			Close();
		};
		PlayButton.ToolTip = new NamedToolTipInfo("#TT_COMMON_1");
		Add(PlayButton);
		GUIButton gUIButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(64f, 64f), new Vector2(933f, 6f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|button_mysquad_close");
		gUIButton.Click += delegate
		{
			_currentDisplayHero = _defaultHero;
			Close();
		};
		gUIButton.ToolTip = new NamedToolTipInfo("#TT_COMMON_1");
		Add(gUIButton);
		int id = -1;
		using (Dictionary<string, HeroPersisted>.ValueCollection.Enumerator enumerator = _dataManager.Profile.AvailableCostumes.Values.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				HeroPersisted current = enumerator.Current;
				id = OwnableDefinition.HeroNameToHeroID[current.Name];
			}
		}
		if (_dataManager.Profile.SelectedCostume != null && _dataManager.Profile.SelectedCostume.Length > 0)
		{
			id = OwnableDefinition.HeroNameToHeroID[_dataManager.Profile.SelectedCostume];
		}
		HeroDefinition heroDef = OwnableDefinition.getHeroDef(id);
		titleLabel = new GUIStrokeTextLabel();
		titleLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(94f, 80f), new Vector2(200f, 40f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		titleLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 14, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(2, 34, 90), GUILabel.GenColor(2, 34, 90), new Vector2(2f, 3f), TextAnchor.UpperCenter);
		titleLabel.BackColorAlpha = 1f;
		titleLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		if (_dataManager.Profile.titleID != -1 || _dataManager.isLocalPlayer)
		{
			titleLabel.Text = _dataManager.Profile.Title;
		}
		else
		{
			titleLabel.Text = "<No Title Displayed>";
		}
		_content.Add(titleLabel);
		GUIButton gUIButton2 = new GUIButton();
		gUIButton2.Id = "changeTitleButton";
		gUIButton2.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|changetitle_icon");
		gUIButton2.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, titleLabel.Position + new Vector2(-25f, -10f), new Vector2(32f, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton2.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton2.MouseDown += delegate
		{
			chooseTitleHUD.IsVisible = true;
			chooseTitleHUD.SetPosition(TITLE_WINDOW_OPENED);
		};
		if (_dataManager.isLocalPlayer)
		{
			_content.Add(gUIButton2);
		}
		GUIStrokeTextLabel gUIStrokeTextLabel2 = new GUIStrokeTextLabel();
		gUIStrokeTextLabel2.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(69f, 105f), new Vector2(250f, 30f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel2.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(2, 34, 90), GUILabel.GenColor(2, 34, 90), new Vector2(2f, 3f), TextAnchor.UpperCenter);
		gUIStrokeTextLabel2.BackColorAlpha = 1f;
		gUIStrokeTextLabel2.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUIStrokeTextLabel2.Text = _dataManager.Profile.PlayerName;
		_content.Add(gUIStrokeTextLabel2);
		string text = _dataManager.Profile.Medallion;
		if (text == string.Empty)
		{
			text = "brawlergadget_bundle|brawler_gadget_powerON_placeholder_normal";
		}
		CspUtils.DebugLog("currentMedallion " + text + " " + _dataManager.Profile.medallionID);
		changeMedallionButton = new GUIImageWithEvents();
		changeMedallionButton.Id = "changeMedallionButton";
		changeMedallionButton.TextureSource = text;
		changeMedallionButton.ToolTip = new NamedToolTipInfo("#MYSQUAD_CHANGE_MEDALLION");
		changeMedallionButton.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, gUIStrokeTextLabel2.Position + new Vector2(-20f, -7f), new Vector2(32f, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		if (_dataManager.isLocalPlayer)
		{
			changeMedallionButton.MouseDown += delegate
			{
				chooseMedallionHUD.IsVisible = true;
				chooseMedallionHUD.SetPosition(MEDALLION_WINDOW_OPENED);
			};
		}
		if (_dataManager.isLocalPlayer || _dataManager.Profile.medallionID != -1)
		{
			_content.Add(changeMedallionButton);
		}
		GUIImage gUIImage = new GUIImage();
		gUIImage.SetSize(239f, 33f);
		gUIImage.SetPosition(50f, 140f);
		gUIImage.TextureSource = "achievement_bundle|leftnav_underlay";
		_content.Add(gUIImage);
		GUIStrokeTextLabel gUIStrokeTextLabel3 = new GUIStrokeTextLabel();
		gUIStrokeTextLabel3.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, gUIImage.Position, gUIImage.Size, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel3.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(2, 0, 0), GUILabel.GenColor(2, 0, 0), new Vector2(2f, 3f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel3.BackColorAlpha = 1f;
		gUIStrokeTextLabel3.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		_content.Add(gUIStrokeTextLabel3);
		gUIStrokeTextLabel3.Text = "Current Hero";
		_iconHolder = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(new Vector2(180f, 180f), new Vector2(80f, 170f));
		_content.Add(_iconHolder);
		_iconHolder.IsVisible = true;
		_icon = heroDef.ownableDef.getIcon(HeroIconSize);
		GUISimpleControlWindow icon = _icon;
		float x = HeroIconSize.x;
		Vector2 size = _icon.Size;
		float x2 = (x - size.x) / 2f;
		float y = HeroIconSize.y;
		Vector2 size2 = _icon.Size;
		icon.Position = new Vector2(x2, (y - size2.y) / 2f);
		_icon.IsVisible = true;
		_iconHolder.Add(_icon);
		if (_dataManager.isLocalPlayer)
		{
			GUIButton gUIButton3 = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(48f, 48f), gUIImage.Position + new Vector2(200f, -7f));
			gUIButton3.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|mshs_gameworld_HUD_changehero");
			gUIButton3.ToolTip = new NamedToolTipInfo("#GAMEWORLD_PICKHERO_BUTTON");
			gUIButton3.HitTestSize = new Vector2(1f, 1f);
			gUIButton3.HitTestType = HitTestTypeEnum.Circular;
			gUIButton3.Click += ChangeHero_Click;
			_content.Add(gUIButton3);
		}
		GUIImage gUIImage2 = new GUIImage();
		gUIImage2.SetSize(239f, 33f);
		gUIImage2.SetPosition(50f, 360f);
		gUIImage2.TextureSource = "achievement_bundle|leftnav_underlay";
		_content.Add(gUIImage2);
		GUIStrokeTextLabel gUIStrokeTextLabel4 = new GUIStrokeTextLabel();
		gUIStrokeTextLabel4.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, gUIImage2.Position, gUIImage2.Size, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel4.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(2, 0, 0), GUILabel.GenColor(2, 0, 0), new Vector2(2f, 3f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel4.BackColorAlpha = 1f;
		gUIStrokeTextLabel4.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		_content.Add(gUIStrokeTextLabel4);
		gUIStrokeTextLabel4.Text = "Current Sidekick";
		OwnableDefinition def = OwnableDefinition.getDef(_dataManager.Profile.SidekickID);
		CspUtils.DebugLog("sidekickID is " + _dataManager.Profile.SidekickID);
		_sidekickIconHolder = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(SidekickIconSize, new Vector2(120f, 390f));
		_content.Add(_sidekickIconHolder);
		_sidekickIconHolder.IsVisible = true;
		if (def != null)
		{
			_sidekickIcon = def.getIcon(SidekickIconSize);
			GUISimpleControlWindow sidekickIcon = _sidekickIcon;
			float x3 = SidekickIconSize.x;
			Vector2 size3 = _sidekickIcon.Size;
			float x4 = (x3 - size3.x) / 2f;
			float y2 = SidekickIconSize.y;
			Vector2 size4 = _sidekickIcon.Size;
			sidekickIcon.Position = new Vector2(x4, (y2 - size4.y) / 2f);
			_sidekickIcon.IsVisible = true;
			_sidekickIconHolder.Add(_sidekickIcon);
		}
		if (_dataManager.isLocalPlayer)
		{
			GUIButton gUIButton4 = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(48f, 48f), gUIImage2.Position + new Vector2(200f, -7f));
			gUIButton4.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|icon_changesidekick");
			gUIButton4.ToolTip = new NamedToolTipInfo("#GAMEWORLD_PICKSIDEKICK_BUTTON");
			gUIButton4.HitTestSize = new Vector2(1f, 1f);
			gUIButton4.HitTestType = HitTestTypeEnum.Circular;
			gUIButton4.BlockTestType = BlockTestTypeEnum.Circular;
			gUIButton4.Click += ChangePet_Click;
			_content.Add(gUIButton4);
		}
		Vector2 vector = new Vector2(400f, 120f);
		GUIStrokeTextLabel gUIStrokeTextLabel5 = new GUIStrokeTextLabel();
		gUIStrokeTextLabel5.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, vector, new Vector2(250f, 30f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel5.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 22, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(2, 34, 90), GUILabel.GenColor(2, 34, 90), new Vector2(2f, 3f), TextAnchor.UpperLeft);
		gUIStrokeTextLabel5.BackColorAlpha = 1f;
		gUIStrokeTextLabel5.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUIStrokeTextLabel5.Text = _dataManager.Profile.PlayerName;
		Add(gUIStrokeTextLabel5);
		GUIImageWithEvents gUIImageWithEvents = new GUIImageWithEvents();
		gUIImageWithEvents.SetSize(new Vector2(102f, 102f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImageWithEvents.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, vector + new Vector2(-30f, 0f));
		gUIImageWithEvents.TextureSource = "hud_bundle|globalnav_achievements";
		Add(gUIImageWithEvents);
		gUIImageWithEvents.MouseDown += delegate
		{
			AchievementWindow dialogWindow = new AchievementWindow((int)_dataManager.Profile.UserId);
			GUIManager.Instance.ShowDynamicWindow(dialogWindow, ModalLevelEnum.Default);
		};
		GUIStrokeTextLabel gUIStrokeTextLabel6 = new GUIStrokeTextLabel();
		gUIStrokeTextLabel6.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, vector + new Vector2(40f, 40f), new Vector2(250f, 30f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel6.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(2, 34, 90), GUILabel.GenColor(2, 34, 90), new Vector2(2f, 3f), TextAnchor.UpperLeft);
		gUIStrokeTextLabel6.BackColorAlpha = 1f;
		gUIStrokeTextLabel6.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUIStrokeTextLabel6.Text = "Achievement Points: " + _dataManager.Profile.achievementPoints;
		Add(gUIStrokeTextLabel6);
		GUIStrokeTextLabel gUIStrokeTextLabel7 = new GUIStrokeTextLabel();
		gUIStrokeTextLabel7.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, vector + new Vector2(40f, 80f), new Vector2(250f, 30f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel7.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(2, 34, 90), GUILabel.GenColor(2, 34, 90), new Vector2(2f, 3f), TextAnchor.UpperLeft);
		gUIStrokeTextLabel7.BackColorAlpha = 1f;
		gUIStrokeTextLabel7.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUIStrokeTextLabel7.Text = "Total Heroes in Squad: " + _dataManager.Profile.AvailableCostumes.Count;
		Add(gUIStrokeTextLabel7);
		chooseTitleHUD = new ChooseTitleHUD();
		chooseTitleHUD.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, TITLE_WINDOW_CLOSED);
		Add(chooseTitleHUD);
		chooseTitleHUD.IsVisible = false;
		chooseMedallionHUD = new ChooseMedallionHUD();
		chooseMedallionHUD.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, MEDALLION_WINDOW_CLOSED);
		Add(chooseMedallionHUD);
		chooseMedallionHUD.IsVisible = false;
		petHUD = new ChoosePetHUD();
		petHUD.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, PET_WINDOW_CLOSED);
		Add(petHUD);
		petHUD.IsVisible = false;
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("global_window_in"));
		AppShell.Instance.EventMgr.AddListener<PlayerChangedSquadInfoMessage>(OnSquadInfoChanged);
		AppShell.Instance.EventMgr.AddListener<CharacterSelectedMessage>(OnCharacterSelected);
		AppShell.Instance.EventMgr.AddListener<CurrentPetChangeEvent>(OnCurrentPetChangeEvent);
	}

	private void OnCurrentPetChangeEvent(CurrentPetChangeEvent msg)
	{
		if (_sidekickIcon != null)
		{
			_sidekickIconHolder.Remove(_sidekickIcon);
			_sidekickIcon.Dispose();
			_sidekickIcon = null;
		}
		if (msg.id != -1)
		{
			_sidekickIcon = OwnableDefinition.getDef(msg.id).getIcon(SidekickIconSize);
			GUISimpleControlWindow sidekickIcon = _sidekickIcon;
			float x = SidekickIconSize.x;
			Vector2 size = _sidekickIcon.Size;
			float x2 = (x - size.x) / 2f;
			float y = SidekickIconSize.y;
			Vector2 size2 = _sidekickIcon.Size;
			sidekickIcon.Position = new Vector2(x2, (y - size2.y) / 2f);
			_sidekickIcon.IsVisible = true;
			_sidekickIconHolder.Add(_sidekickIcon);
		}
		petHUD.state = 0;
		petHUD.IsVisible = false;
		petHUD.SetPosition(PET_WINDOW_CLOSED);
		base.AnimationPieceManager.Add(SHSAnimations.WindowOpenCloseDelegates.FadeOut(0.2f, petHUD)());
	}

	protected void ChangePet_Click(GUIControl sender, GUIClickEvent EventData)
	{
		if (petHUD.state == 0)
		{
			petHUD.state = 1;
			petHUD.IsVisible = true;
			petHUD.SetPosition(PET_WINDOW_OPENED);
			base.AnimationPieceManager.Add(SHSAnimations.WindowOpenCloseDelegates.FadeIn(0.3f, petHUD)());
		}
		else
		{
			petHUD.state = 0;
			petHUD.IsVisible = false;
			petHUD.SetPosition(PET_WINDOW_CLOSED);
			base.AnimationPieceManager.Add(SHSAnimations.WindowOpenCloseDelegates.FadeOut(0.2f, petHUD)());
		}
	}

	private void OnCharacterSelected(CharacterSelectedMessage msg)
	{
		CspUtils.DebugLog("OnCharacterSelected " + _dataManager.isLocalPlayer);
		if (_dataManager.isLocalPlayer)
		{
			int id = OwnableDefinition.HeroNameToHeroID[msg.CharacterName];
			HeroDefinition heroDef = OwnableDefinition.getHeroDef(id);
			if (_icon != null)
			{
				_iconHolder.Remove(_icon);
				_icon.Dispose();
				_icon = null;
			}
			_icon = heroDef.ownableDef.getIcon(HeroIconSize);
			GUISimpleControlWindow icon = _icon;
			float x = HeroIconSize.x;
			Vector2 size = _icon.Size;
			float x2 = (x - size.x) / 2f;
			float y = HeroIconSize.y;
			Vector2 size2 = _icon.Size;
			icon.Position = new Vector2(x2, (y - size2.y) / 2f);
			_icon.IsVisible = true;
			_iconHolder.Add(_icon);
		}
	}

	protected void ChangeHero_Click(GUIControl sender, GUIClickEvent EventData)
	{
		if (_dataManager.isLocalPlayer)
		{
			SocialSpaceController.Instance.Controller.ChangeCharacters();
		}
	}

	private void OnSquadInfoChanged(PlayerChangedSquadInfoMessage message)
	{
		if (_dataManager.isLocalPlayer)
		{
			titleLabel.Text = AppShell.Instance.Profile.Title;
			string text = _dataManager.Profile.Medallion;
			if (text == string.Empty)
			{
				text = "brawlergadget_bundle|brawler_gadget_powerON_placeholder_normal";
			}
			changeMedallionButton.TextureSource = text;
		}
	}

	public void Close()
	{
		IsVisible = false;
		if (PlayerStatus.GetLocalStatus() == PlayerStatusDefinition.Instance.GetStatus("MySquad"))
		{
			PlayerStatus.ClearLocalStatus();
		}
		AppShell.Instance.EventMgr.RemoveListener<PlayerChangedSquadInfoMessage>(OnSquadInfoChanged);
		AppShell.Instance.EventMgr.RemoveListener<CharacterSelectedMessage>(OnCharacterSelected);
		SHSInput.RevertInputBlockingMode(this);
		VOManager.Instance.StopAll();
		OnHide();
		Dispose();
	}
}
