using System.Collections.Generic;
using UnityEngine;

public class HeroViewer : GUISimpleControlWindow
{
	private HeroPersisted _currentHero;

	private OwnableDefinition _currentHeroDef;

	private GUIStrokeTextLabel _heroNameLabel;

	private GUISimpleControlWindow _iconHolder;

	private GUISimpleControlWindow _icon;

	private GUILabel _heroDescLabel;

	private Dictionary<string, GUISimpleControlWindow> _tabs = new Dictionary<string, GUISimpleControlWindow>();

	private GUILabel _infoHeroLevelLabel;

	private MyHeroesXPBar _infoHeroXPBar;

	private MyHeroesHealthBar _infoHeroHealthBar;

	private MyHeroesPowerEmoteBar _infoHeroPowerEmoteBar;

	private GUIDrawTexture _infoHeroLore;

	private GUIImage _infoKeywordBG;

	private List<GUIImageWithEvents> _infoKeywordImages = new List<GUIImageWithEvents>();

	private BadgeGUI _infoBadgeIconSilver;

	private GUIDrawTexture _infoBadgeLockSilver;

	private BadgeGUI _infoBadgeIconGold;

	private GUIDrawTexture _infoBadgeLockGold;

	private GUIStrokeTextLabel _powersR1Name;

	private GUIStrokeTextLabel _powersR2Name;

	private GUIStrokeTextLabel _powersR3Name;

	private MyHeroesPowerAttackBar _powersAttack1;

	private MyHeroesPowerAttackBar _powersAttack2;

	private MyHeroesPowerAttackBar _powersAttack3;

	private MyHeroesPowerAttackBar _powersHeroUps;

	private GUIStrokeTextLabel _unownedLabel;

	private GUIStrokeTextLabel _unownedPurchaseLabel;

	private GUIButton _unownedPurchaseButton;

	private MySquadDataManager _dataManager;

	private Vector2 MAX_ICON_SIZE = new Vector2(180f, 180f);

	public HeroViewer(MySquadDataManager dataManager)
	{
		_dataManager = dataManager;
	}

	public string selectedHero()
	{
		return _currentHero.Name;
	}

	public void init()
	{
		_heroNameLabel = new GUIStrokeTextLabel();
		_heroNameLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(15f, 0f), new Vector2(200f, 60f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_heroNameLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 20, GUILabel.GenColor(255, 255, 106), GUILabel.GenColor(2, 34, 90), GUILabel.GenColor(2, 34, 90), new Vector2(2f, 3f), TextAnchor.MiddleCenter);
		_heroNameLabel.BackColorAlpha = 1f;
		_heroNameLabel.WordWrap = true;
		Add(_heroNameLabel);
		_iconHolder = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(MAX_ICON_SIZE, new Vector2(10f, 42f));
		Add(_iconHolder);
		_iconHolder.IsVisible = true;
		_heroDescLabel = new GUILabel();
		_heroDescLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(7f, 220f), new Vector2(200f, 150f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_heroDescLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
		_heroDescLabel.WordWrap = true;
		Add(_heroDescLabel);
		Vector2 vector = new Vector2(0f, 10f);
		GUISimpleControlWindow gUISimpleControlWindow = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(Size - new Vector2(220f, 0f), new Vector2(200f, 0f));
		Add(gUISimpleControlWindow);
		gUISimpleControlWindow.SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		gUISimpleControlWindow.IsVisible = true;
		_infoHeroLevelLabel = new GUILabel();
		_infoHeroLevelLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, vector + new Vector2(20f, 0f), new Vector2(300f, 100f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_infoHeroLevelLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
		gUISimpleControlWindow.Add(_infoHeroLevelLabel);
		_infoHeroLore = GUIControl.CreateControlAbsolute<GUIDrawTexture>(new Vector2(82f, 33f), vector + new Vector2(260f, 0f));
		_infoHeroLore.ToolTip = new NamedToolTipInfo("#HEROLIST_LOREVALUE", new Vector2(0f, 0f));
		gUISimpleControlWindow.Add(_infoHeroLore);
		_infoHeroXPBar = GUIControl.CreateControlTopLeftFrame<MyHeroesXPBar>(new Vector2(378f, 55f), vector + new Vector2(-5f, 30f));
		_infoHeroXPBar.IsVisible = true;
		gUISimpleControlWindow.Add(_infoHeroXPBar);
		_infoHeroHealthBar = GUIControl.CreateControlTopLeftFrame<MyHeroesHealthBar>(new Vector2(378f, 55f), vector + new Vector2(-5f, 80f));
		_infoHeroHealthBar.IsVisible = true;
		gUISimpleControlWindow.Add(_infoHeroHealthBar);
		_infoHeroPowerEmoteBar = GUIControl.CreateControlTopLeftFrame<MyHeroesPowerEmoteBar>(new Vector2(378f, 55f), vector + new Vector2(-5f, 130f));
		_infoHeroPowerEmoteBar.IsVisible = true;
		gUISimpleControlWindow.Add(_infoHeroPowerEmoteBar);
		_infoKeywordBG = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(356f, 36f), vector + new Vector2(5f, 185f));
		_infoKeywordBG.TextureSource = "mysquadgadget_bundle|generic_fillbar_bg";
		gUISimpleControlWindow.Add(_infoKeywordBG);
		_infoKeywordBG.IsVisible = true;
		GUIStrokeTextLabel gUIStrokeTextLabel = GUIControl.CreateControlAbsolute<GUIStrokeTextLabel>(new Vector2(370f, 55f), new Vector2(5f, 0f));
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(134, 208, 255), GUILabel.GenColor(0, 33, 101), GUILabel.GenColor(1, 33, 85), new Vector2(-2f, 2f), TextAnchor.UpperLeft);
		gUIStrokeTextLabel.SetPosition(_infoKeywordBG.Position + new Vector2(13f, 7f));
		gUIStrokeTextLabel.Text = "#MYSQUAD_ABILITIES";
		gUIStrokeTextLabel.Bold = true;
		gUISimpleControlWindow.Add(gUIStrokeTextLabel);
		_infoBadgeIconSilver = new BadgeGUI(1);
		_infoBadgeIconSilver.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, vector + new Vector2(5f, 225f));
		_infoBadgeIconSilver.SetSize(new Vector2(60f, 60f));
		_infoBadgeIconSilver.Traits.ActivationTrait = ControlTraits.ActivationTraitEnum.Manual;
		_infoBadgeIconSilver.HitTestType = HitTestTypeEnum.Circular;
		_infoBadgeIconSilver.IsVisible = true;
		gUISimpleControlWindow.Add(_infoBadgeIconSilver);
		_infoBadgeLockSilver = GUIControl.CreateControlTopLeftFrame<GUIDrawTexture>(new Vector2(32f, 35f) * 0.91f, _infoBadgeIconSilver.Position + new Vector2(30f, 0f));
		_infoBadgeLockSilver.TextureSource = "mysquadgadget_bundle|mysquad_gadget_charactericon_lock";
		_infoBadgeLockSilver.HitTestSize = new Vector2(0.875f, 0.875f);
		_infoBadgeLockSilver.Traits.ActivationTrait = ControlTraits.ActivationTraitEnum.Manual;
		_infoBadgeLockSilver.HitTestType = HitTestTypeEnum.Circular;
		_infoBadgeLockSilver.IsVisible = true;
		gUISimpleControlWindow.Add(_infoBadgeLockSilver);
		_infoBadgeIconGold = new BadgeGUI(2);
		_infoBadgeIconGold.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, vector + new Vector2(65f, 225f));
		_infoBadgeIconGold.SetSize(new Vector2(60f, 60f));
		_infoBadgeIconGold.Traits.ActivationTrait = ControlTraits.ActivationTraitEnum.Manual;
		_infoBadgeIconGold.HitTestType = HitTestTypeEnum.Circular;
		_infoBadgeIconGold.IsVisible = true;
		gUISimpleControlWindow.Add(_infoBadgeIconGold);
		_infoBadgeLockGold = GUIControl.CreateControlTopLeftFrame<GUIDrawTexture>(new Vector2(32f, 35f) * 0.91f, _infoBadgeIconGold.Position + new Vector2(30f, 0f));
		_infoBadgeLockGold.TextureSource = "mysquadgadget_bundle|mysquad_gadget_charactericon_lock";
		_infoBadgeLockGold.HitTestSize = new Vector2(0.875f, 0.875f);
		_infoBadgeLockGold.Traits.ActivationTrait = ControlTraits.ActivationTraitEnum.Manual;
		_infoBadgeLockGold.HitTestType = HitTestTypeEnum.Circular;
		_infoBadgeLockGold.IsVisible = true;
		gUISimpleControlWindow.Add(_infoBadgeLockGold);
		_tabs.Add("info", gUISimpleControlWindow);
		GUISimpleControlWindow gUISimpleControlWindow2 = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(Size - new Vector2(210f, 0f), new Vector2(210f, 0f));
		Add(gUISimpleControlWindow2);
		gUISimpleControlWindow2.SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		gUISimpleControlWindow2.IsVisible = false;
		GUIStrokeTextLabel gUIStrokeTextLabel2 = GUIControl.CreateControlAbsolute<GUIStrokeTextLabel>(new Vector2(370f, 55f), vector);
		gUIStrokeTextLabel2.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(134, 208, 255), GUILabel.GenColor(0, 33, 101), GUILabel.GenColor(1, 33, 85), new Vector2(-2f, 2f), TextAnchor.UpperLeft);
		gUIStrokeTextLabel2.Text = "#HEROLIST_POWERATTACKS";
		gUIStrokeTextLabel2.Bold = true;
		gUISimpleControlWindow2.Add(gUIStrokeTextLabel2);
		_powersR1Name = GUIControl.CreateControlAbsolute<GUIStrokeTextLabel>(new Vector2(370f, 55f), vector + new Vector2(20f, 30f));
		_powersR1Name.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(134, 208, 255), GUILabel.GenColor(0, 33, 101), GUILabel.GenColor(1, 33, 85), new Vector2(-2f, 2f), TextAnchor.UpperLeft);
		_powersR1Name.Text = string.Empty;
		_powersR1Name.Bold = true;
		gUISimpleControlWindow2.Add(_powersR1Name);
		_powersAttack1 = GUIControl.CreateControlTopLeftFrame<MyHeroesPowerAttackBar>(new Vector2(378f, 55f), _powersR1Name.Position + new Vector2(20f, 15f));
		_powersAttack1.IsVisible = true;
		gUISimpleControlWindow2.Add(_powersAttack1);
		_powersR2Name = GUIControl.CreateControlAbsolute<GUIStrokeTextLabel>(new Vector2(370f, 55f), vector + new Vector2(20f, 100f));
		_powersR2Name.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(134, 208, 255), GUILabel.GenColor(0, 33, 101), GUILabel.GenColor(1, 33, 85), new Vector2(-2f, 2f), TextAnchor.UpperLeft);
		_powersR2Name.Text = string.Empty;
		_powersR2Name.Bold = true;
		gUISimpleControlWindow2.Add(_powersR2Name);
		_powersAttack2 = GUIControl.CreateControlTopLeftFrame<MyHeroesPowerAttackBar>(new Vector2(378f, 55f), _powersR2Name.Position + new Vector2(20f, 15f));
		_powersAttack2.IsVisible = true;
		gUISimpleControlWindow2.Add(_powersAttack2);
		_powersR3Name = GUIControl.CreateControlAbsolute<GUIStrokeTextLabel>(new Vector2(370f, 55f), vector + new Vector2(20f, 170f));
		_powersR3Name.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(134, 208, 255), GUILabel.GenColor(0, 33, 101), GUILabel.GenColor(1, 33, 85), new Vector2(-2f, 2f), TextAnchor.UpperLeft);
		_powersR3Name.Text = string.Empty;
		_powersR3Name.Bold = true;
		gUISimpleControlWindow2.Add(_powersR3Name);
		_powersAttack3 = GUIControl.CreateControlTopLeftFrame<MyHeroesPowerAttackBar>(new Vector2(378f, 55f), _powersR3Name.Position + new Vector2(20f, 15f));
		_powersAttack3.IsVisible = true;
		gUISimpleControlWindow2.Add(_powersAttack3);
		GUIStrokeTextLabel gUIStrokeTextLabel3 = GUIControl.CreateControlAbsolute<GUIStrokeTextLabel>(new Vector2(370f, 55f), vector + new Vector2(0f, 240f));
		gUIStrokeTextLabel3.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(134, 208, 255), GUILabel.GenColor(0, 33, 101), GUILabel.GenColor(1, 33, 85), new Vector2(-2f, 2f), TextAnchor.UpperLeft);
		gUIStrokeTextLabel3.Text = "#HEROLIST_HEROUP";
		gUIStrokeTextLabel3.Bold = true;
		gUISimpleControlWindow2.Add(gUIStrokeTextLabel3);
		_powersHeroUps = GUIControl.CreateControlTopLeftFrame<MyHeroesPowerAttackBar>(new Vector2(378f, 55f), gUIStrokeTextLabel3.Position + new Vector2(40f, 20f));
		_powersHeroUps.IsVisible = true;
		gUISimpleControlWindow2.Add(_powersHeroUps);
		_tabs.Add("powers", gUISimpleControlWindow2);
		GUISimpleControlWindow gUISimpleControlWindow3 = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(Size - new Vector2(210f, 0f), new Vector2(210f, 0f));
		Add(gUISimpleControlWindow3);
		gUISimpleControlWindow3.SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		gUISimpleControlWindow3.IsVisible = false;
		GUIImage gUIImage = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(159f, 176f) * 0.9f, vector + new Vector2(65f, 30f));
		gUIImage.TextureSource = "mysquadgadget_bundle|biglock";
		gUISimpleControlWindow3.Add(gUIImage);
		gUIImage.IsVisible = true;
		Vector2 size = gUIImage.Size;
		Vector2 size2 = new Vector2(size.x + 100f, 75f);
		Vector2 position = gUIImage.Position;
		Vector2 size3 = gUIImage.Size;
		_unownedLabel = GUIControl.CreateControlAbsolute<GUIStrokeTextLabel>(size2, position + new Vector2(-50f, size3.y));
		_unownedLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 22, GUILabel.GenColor(134, 208, 255), GUILabel.GenColor(0, 33, 101), GUILabel.GenColor(1, 33, 85), new Vector2(-2f, 2f), TextAnchor.UpperCenter);
		_unownedLabel.Text = "#MYSQUAD_DONT_OWN_HERO";
		_unownedLabel.Bold = true;
		_unownedLabel.WordWrap = true;
		gUISimpleControlWindow3.Add(_unownedLabel);
		Vector2 size4 = new Vector2(325f, 50f);
		Vector2 position2 = gUIImage.Position;
		Vector2 size5 = gUIImage.Size;
		_unownedPurchaseButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(size4, position2 + new Vector2(-50f, size5.y + 70f));
		_unownedPurchaseButton.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|widegoldbutton", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		_unownedPurchaseButton.IsVisible = true;
		_unownedPurchaseButton.Click += delegate
		{
			tryToFindHero();
		};
		gUISimpleControlWindow3.Add(_unownedPurchaseButton);
		_unownedPurchaseLabel = new GUIStrokeTextLabel();
		_unownedPurchaseLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, _unownedPurchaseButton.Position + new Vector2(0f, -5f), _unownedPurchaseButton.Size, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_unownedPurchaseLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 226, 90), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.MiddleCenter);
		_unownedPurchaseLabel.BackColorAlpha = 1f;
		_unownedPurchaseLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		_unownedPurchaseLabel.IsVisible = true;
		gUISimpleControlWindow3.Add(_unownedPurchaseLabel);
		_tabs.Add("unowned", gUISimpleControlWindow3);
		GUISimpleControlWindow gUISimpleControlWindow4 = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(Size - new Vector2(210f, 0f), new Vector2(210f, 0f));
		Add(gUISimpleControlWindow4);
		gUISimpleControlWindow4.SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		gUISimpleControlWindow4.IsVisible = false;
		_tabs.Add("achievements", gUISimpleControlWindow4);
		GUISimpleControlWindow gUISimpleControlWindow5 = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(Size - new Vector2(210f, 0f), new Vector2(210f, 0f));
		Add(gUISimpleControlWindow5);
		gUISimpleControlWindow5.SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		gUISimpleControlWindow5.IsVisible = false;
		_tabs.Add("gear", gUISimpleControlWindow5);
	}

	private void tryToFindHero()
	{
		OwnableDefinition.goToOwnableLocation(_currentHeroDef.ownableTypeID);
	}

	public void showTab(string whatTab)
	{
		foreach (GUISimpleControlWindow value in _tabs.Values)
		{
			value.IsVisible = false;
		}
		_tabs[whatTab].IsVisible = true;
	}

	public void setHero(string heroName)
	{
		_currentHeroDef = OwnableDefinition.getDef(OwnableDefinition.HeroNameToHeroID[heroName]);
		if (_icon != null)
		{
			_iconHolder.Remove(_icon);
			_icon.Dispose();
		}
		_icon = _currentHeroDef.getIcon(MAX_ICON_SIZE);
		GUISimpleControlWindow icon = _icon;
		float x = MAX_ICON_SIZE.x;
		Vector2 size = _icon.Size;
		float x2 = (x - size.x) / 2f;
		float y = MAX_ICON_SIZE.y;
		Vector2 size2 = _icon.Size;
		icon.Position = new Vector2(x2, (y - size2.y) / 2f);
		_icon.IsVisible = true;
		_iconHolder.Add(_icon);
		_heroNameLabel.Text = _currentHeroDef.shoppingName;
		_heroDescLabel.Text = _currentHeroDef.shoppingDesc;
		HeroPersisted heroPersisted = _dataManager.Profile.AvailableCostumes[heroName];
		if (heroPersisted == null)
		{
			_unownedLabel.Text = "#MYSQUAD_DONT_OWN_HERO";
			switch (OwnableDefinition.getLocation(_currentHeroDef.ownableTypeID))
			{
			case OwnableDefinition.OwnableLocation.Store:
				_unownedPurchaseLabel.Text = "#MYSQUAD_HERO_IN_SHOP";
				break;
			case OwnableDefinition.OwnableLocation.MysteryBox:
				_unownedPurchaseLabel.Text = "#MYSQUAD_HERO_IN_BOX";
				break;
			case OwnableDefinition.OwnableLocation.Crafting:
				_unownedPurchaseLabel.Text = "#MYSQUAD_HERO_IN_CRAFTING";
				break;
			case OwnableDefinition.OwnableLocation.Achievement:
				_unownedPurchaseLabel.Text = "#MYSQUAD_HERO_IN_ACHIEVEMENT";
				break;
			case OwnableDefinition.OwnableLocation.Mission:
				_unownedPurchaseLabel.Text = "#MYSQUAD_HERO_IN_MISSION";
				break;
			default:
				_unownedPurchaseLabel.Text = "#MYSQUAD_VIEW_SIMILAR";
				_unownedLabel.Text = "#MYSQUAD_HERO_UNAVAILABLE";
				break;
			}
			showTab("unowned");
			return;
		}
		_currentHero = heroPersisted;
		_infoHeroLevelLabel.Text = string.Format(AppShell.Instance.stringTable["#airlock_hero_level"], _currentHero.Level);
		int heroLore = AppShell.Instance.HeroLoreManager.GetHeroLore(_currentHero.Name);
		if (heroLore < 0 || heroLore > 3)
		{
			CspUtils.DebugLog("HeroViewer - hiding lore icon due to unexpected lore value <" + heroLore + "> for hero " + _currentHero.Name);
			_infoHeroLore.IsVisible = false;
			return;
		}
		_infoHeroLore.IsVisible = true;
		_infoHeroLore.TextureSource = "common_bundle|marvel_lore_icon_mysquad_plus" + heroLore;
		_infoHeroXPBar.SetExperience(MySquadDataManager.GetPercToNextLevel(_dataManager.Profile, heroPersisted.Name), MySquadDataManager.GetExpTextRaw(_dataManager.Profile, heroPersisted.Name));
		HeroDefinition heroDef = OwnableDefinition.getHeroDef(heroPersisted.Name);
		float percent = (float)(heroDef.getHealthAtLevel(heroPersisted.Level) - (heroDef.getBaseHealth() - 20)) / (float)(heroDef.getMaxHealth() - (heroDef.getBaseHealth() - 20));
		_infoHeroHealthBar.SetHealth(percent, string.Empty + heroDef.getHealthAtLevel(heroPersisted.Level) + "hp");
		_infoHeroPowerEmoteBar.SetPowerEmotes(heroPersisted.Name, _dataManager.Profile);
		foreach (GUIImageWithEvents infoKeywordImage in _infoKeywordImages)
		{
			_tabs["info"].Remove(infoKeywordImage);
			infoKeywordImage.Dispose();
		}
		_infoKeywordImages.Clear();
		int num = 0;
		int num2 = 1;
		int num3 = 12;
		foreach (Keyword keyword in heroDef.ownableDef.getKeywords())
		{
			if (keyword.icon.Length > 0)
			{
				GUIImageWithEvents gUIImageWithEvents = GUIControl.CreateControlTopLeftFrame<GUIImageWithEvents>(new Vector2(26f, 26f), _infoKeywordBG.Position + new Vector2(65f, 5f) + new Vector2(num % num3 * 32, num / num3 * 28));
				gUIImageWithEvents.TextureSource = keyword.icon;
				_infoKeywordImages.Add(gUIImageWithEvents);
				_tabs["info"].Add(gUIImageWithEvents);
				gUIImageWithEvents.Id = keyword.keyword;
				gUIImageWithEvents.ToolTip = new NamedToolTipInfo(keyword.tooltip, new Vector2(20f, 0f));
				gUIImageWithEvents.IsVisible = true;
				num++;
				if (num % num3 == 0)
				{
					num2++;
				}
			}
		}
		GUIImage infoKeywordBG = _infoKeywordBG;
		Vector2 size3 = _infoKeywordBG.Size;
		infoKeywordBG.SetSize(new Vector2(size3.x, 36 * num2));
		OwnableDefinition badgeDef = heroDef.getBadgeDef(1);
		if (badgeDef != null)
		{
			_infoBadgeIconSilver.IsVisible = true;
			_infoBadgeIconSilver.setHero(heroPersisted.Name);
			_infoBadgeIconSilver.ownableTypeID = badgeDef.ownableTypeID;
			_infoBadgeIconSilver.IsVisible = true;
			_infoBadgeLockSilver.IsVisible = false;
			if (AppShell.Instance.Profile.Badges.ContainsKey(string.Empty + badgeDef.ownableTypeID))
			{
				_infoBadgeIconSilver.IsEnabled = false;
				_infoBadgeIconSilver.setOwned(true);
				_infoBadgeLockSilver.IsVisible = false;
			}
			else
			{
				_infoBadgeLockSilver.ToolTip = new NamedToolTipInfo("#BADGE_PURCHASE_PROMPT", new Vector2(20f, 0f));
				_infoBadgeIconSilver.IsEnabled = true;
				_infoBadgeLockSilver.IsVisible = true;
			}
		}
		else
		{
			_infoBadgeIconSilver.IsVisible = false;
			_infoBadgeLockSilver.IsVisible = false;
		}
		badgeDef = heroDef.getBadgeDef(2);
		if (badgeDef != null)
		{
			_infoBadgeIconGold.IsVisible = true;
			_infoBadgeIconGold.setHero(heroPersisted.Name);
			_infoBadgeIconGold.ownableTypeID = badgeDef.ownableTypeID;
			_infoBadgeIconGold.IsVisible = true;
			_infoBadgeIconGold.IsVisible = false;
			if (AppShell.Instance.Profile.Badges.ContainsKey(string.Empty + badgeDef.ownableTypeID))
			{
				_infoBadgeIconGold.IsEnabled = false;
				_infoBadgeIconGold.setOwned(true);
				_infoBadgeLockGold.IsVisible = false;
			}
			else
			{
				_infoBadgeLockGold.ToolTip = new NamedToolTipInfo("#BADGE_PURCHASE_PROMPT", new Vector2(20f, 0f));
				_infoBadgeIconGold.IsEnabled = true;
				_infoBadgeLockGold.IsVisible = true;
			}
		}
		else
		{
			_infoBadgeIconGold.IsVisible = false;
			_infoBadgeLockGold.IsVisible = false;
		}
		_powersAttack1.config(heroPersisted.Name, 1, heroPersisted.Level);
		_powersAttack2.config(heroPersisted.Name, 2, heroPersisted.Level);
		_powersAttack3.config(heroPersisted.Name, 3, heroPersisted.Level);
		_powersHeroUps.config(heroPersisted.Name, 0, heroPersisted.Level);
		_powersR1Name.Text = MySquadDataManager.GetPowerAttackName(heroPersisted.Name, 1, 1);
		_powersR2Name.Text = MySquadDataManager.GetPowerAttackName(heroPersisted.Name, 2, 1);
		_powersR3Name.Text = MySquadDataManager.GetPowerAttackName(heroPersisted.Name, 3, 1);
		showTab("info");
	}
}
