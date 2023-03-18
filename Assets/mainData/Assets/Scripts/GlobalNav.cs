using System;
using System.Collections.Generic;
using UnityEngine;

public class GlobalNav : GUISimpleControlWindow
{
	public enum GlobalNavType
	{
		Mission,
		Store,
		Explore,
		Card,
		Arcade,
		Craft,
		Achievements,
		Squad,
		Inventory,
		Friends,
		HQ,
		Settings
	}

	private GUISimpleControlWindow _mainContainer;

	private GUIImage _background;

	private GUIImage _goldIcon;

	private GUIImage _fractalIcon;

	private GUILabel _totalGoldLabel;

	private GUILabel _totalFractalsLabel;

	private GUIButton _toggleOn;

	private GUIButton _toggleOff;

	private GUISimpleControlWindow _navButtonContainer;

	private List<GlobalNavButton> _navButtons = new List<GlobalNavButton>();

	private int _currY;

	private int _deltY = 40;

	private Vector2 _navButtonContainerOffPosition = new Vector2(0f, -1000f);

	private Vector2 _navButtonContainerOnPosition = new Vector2(0f, 0f);

	private bool _onBottom;

	public static SHSStyle GlobalNavStyle;

	public GlobalNav(bool showMinimalNavButtons, bool onBottom = false)
	{
		AppShell.Instance.EventMgr.AddListener<CurrencyUpdateMessage>(OnCurrencyUpdateMessage);
		if (GlobalNavStyle == null)
		{
			GUIStyleManager styleManager = GUIManager.Instance.StyleManager;
			GUIStyle gUIStyle = new GUIStyle();
			gUIStyle.name = "GlobalNavStyle";
			SHSStyle sHSStyle = new SHSStyle(gUIStyle);
			sHSStyle.AudioDown = styleManager.GetUISound("large_click_down");
			sHSStyle.AudioOver = styleManager.GetUISound("large_hover_over");
			GlobalNavStyle = sHSStyle;
		}
		_onBottom = onBottom;
		_mainContainer = new GUISimpleControlWindow();
		Add(_mainContainer);
		_background = new GUIImage();
		_background.SetSize(new Vector2(175f, 71f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_background.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, Vector2.zero);
		_background.TextureSource = "hud_bundle|globalnav_bg";
		_background.Id = "Background";
		_background.HitTestType = HitTestTypeEnum.Rect;
		_background.BlockTestType = BlockTestTypeEnum.Rect;
		_background.HitTestSize = new Vector2(1f, 1f);
		_background.IsVisible = false;
		_mainContainer.Add(_background);
		_background.IsVisible = true;
		_mainContainer.SetSize(_background.Size + new Vector2(100f, 0f));
		Vector2 size = new Vector2(100f, 100f);
		Vector2 size2 = _background.Size;
		_navButtonContainer = GUIControl.CreateControlTopRightFrame<GUISimpleControlWindow>(size, new Vector2(0f, size2.y));
		_navButtonContainer.SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		_navButtonContainer.HitTestType = HitTestTypeEnum.Rect;
		_navButtonContainer.BlockTestType = BlockTestTypeEnum.Rect;
		_navButtonContainer.HitTestSize = new Vector2(1f, 1f);
		Add(_navButtonContainer);
		if (showMinimalNavButtons)
		{
			_navButtons.Add(createButton("#GLOBALNAV_MISSION", "hud_bundle|globalnav_mission", GlobalNavType.Mission));
			_navButtons.Add(createButton("#GLOBALNAV_TRAVEL", "hud_bundle|globalnav_explore", GlobalNavType.Explore));
			_navButtons.Add(createButton("#GLOBALNAV_CARDGAME", "hud_bundle|globalnav_cards", GlobalNavType.Card));
			_navButtons.Add(createButton("#GLOBALNAV_ARCADE", "hud_bundle|globalnav_arcade", GlobalNavType.Arcade));
			_navButtons.Add(createButton("#GLOBALNAV_SETTINGS", "hud_bundle|globalnav_settings", GlobalNavType.Settings));
		}
		else
		{
			_navButtons.Add(createButton("#GLOBALNAV_SHOP", "hud_bundle|globalnav_store", GlobalNavType.Store));
			_navButtons.Add(createButton("#GLOBALNAV_MISSION", "hud_bundle|globalnav_mission", GlobalNavType.Mission));
			_navButtons.Add(createButton("#GLOBALNAV_TRAVEL", "hud_bundle|globalnav_explore", GlobalNavType.Explore));
			_navButtons.Add(createButton("#GLOBALNAV_ACHIEVEMENTS", "hud_bundle|globalnav_achievements", GlobalNavType.Achievements));
			_navButtons.Add(createButton("#GLOBALNAV_CRAFTING", "hud_bundle|globalnav_craft", GlobalNavType.Craft));
			_navButtons.Add(createButton("#GLOBALNAV_MYSQUAD", "hud_bundle|globalnav_mysquad", GlobalNavType.Squad));
			_navButtons.Add(createButton("#GLOBALNAV_CARDGAME", "hud_bundle|globalnav_cards", GlobalNavType.Card));
			_navButtons.Add(createButton("#GLOBALNAV_BACKPACK", "hud_bundle|globalnav_inventory", GlobalNavType.Inventory));
			_navButtons.Add(createButton("#GLOBALNAV_ARCADE", "hud_bundle|globalnav_arcade", GlobalNavType.Arcade));
			_navButtons.Add(createButton("#GLOBALNAV_FRIENDS", "hud_bundle|globalnav_friends", GlobalNavType.Friends));
			_navButtons.Add(createButton("#GLOBALNAV_SETTINGS", "hud_bundle|globalnav_settings", GlobalNavType.Settings));
		}
		_fractalIcon = new GUIImage();
		_fractalIcon.SetSize(new Vector2(23f, 27f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_fractalIcon.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, _background.Position + new Vector2(125f, 12f));
		_fractalIcon.TextureSource = "hud_bundle|fractal_icon";
		_fractalIcon.Id = "fractalIcon";
		_mainContainer.Add(_fractalIcon);
		_totalFractalsLabel = new GUILabel();
		_totalFractalsLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, _fractalIcon.Position + new Vector2(-90f, 2f), new Vector2(80f, 21f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_totalFractalsLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(104, 160, 9), TextAnchor.MiddleRight);
		_mainContainer.Add(_totalFractalsLabel);
		_goldIcon = new GUIImage();
		_goldIcon.SetSize(new Vector2(26f, 24f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_goldIcon.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, _background.Position + new Vector2(125f, 39f));
		_goldIcon.TextureSource = "hud_bundle|gold_icon";
		_goldIcon.Id = "goldIcon";
		_mainContainer.Add(_goldIcon);
		_totalGoldLabel = new GUILabel();
		_totalGoldLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, _goldIcon.Position + new Vector2(-90f, 2f), new Vector2(80f, 21f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_totalGoldLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(104, 160, 9), TextAnchor.MiddleRight);
		_mainContainer.Add(_totalGoldLabel);
		_toggleOn = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(114f, 114f), _background.Position + new Vector2(138f, -22f));
		_toggleOn.StyleInfo = new SHSButtonStyleInfo("hud_bundle|button_drop_down", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		_toggleOn.HitTestType = HitTestTypeEnum.Rect;
		_toggleOn.BlockTestType = BlockTestTypeEnum.Rect;
		_toggleOn.HitTestSize = new Vector2(1f, 1f);
		_toggleOn.Click += delegate
		{
			toggle(true);
		};
		_toggleOn.IsVisible = true;
		_mainContainer.Add(_toggleOn);
		_toggleOff = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(114f, 114f), _toggleOn.Position);
		_toggleOff.StyleInfo = new SHSButtonStyleInfo("hud_bundle|button_drop_down_on", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		_toggleOff.HitTestType = HitTestTypeEnum.Rect;
		_toggleOff.BlockTestType = BlockTestTypeEnum.Rect;
		_toggleOff.HitTestSize = new Vector2(1f, 1f);
		_toggleOff.Click += delegate
		{
			toggle(false);
		};
		_toggleOff.IsVisible = false;
		_mainContainer.Add(_toggleOff);
		int num = 0;
		foreach (GlobalNavButton navButton in _navButtons)
		{
			Vector2 size3 = navButton.Size;
			if (size3.x > (float)num)
			{
				Vector2 size4 = navButton.Size;
				num = (int)size4.x;
			}
		}
		_navButtonContainer.SetSize(num, _currY + 20);
		foreach (GlobalNavButton navButton2 in _navButtons)
		{
			Vector2 position = navButton2.Position;
			navButton2.SetPosition(0f, position.y);
			navButton2.adjustWidth(num);
		}
		float num2 = num;
		Vector2 size5 = _mainContainer.Size;
		if (num2 < size5.x)
		{
			Vector2 size6 = _mainContainer.Size;
			num = (int)size6.x;
		}
		float width = num;
		Vector2 size7 = _background.Size;
		SetSize(width, size7.y + (float)_currY + 20f);
		Vector2 offset = new Vector2(50f, 0f);
		if (onBottom)
		{
			GUISimpleControlWindow navButtonContainer = _navButtonContainer;
			Vector2 position2 = _navButtonContainer.Position;
			navButtonContainer.SetPosition(position2.x, -30f);
			_mainContainer.SetPosition(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, offset);
		}
		else
		{
			_mainContainer.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, offset);
		}
		_navButtonContainerOnPosition = _navButtonContainer.Position;
		GUISimpleControlWindow navButtonContainer2 = _navButtonContainer;
		Vector2 position3 = _navButtonContainer.Position;
		navButtonContainer2.SetPosition(new Vector2(position3.x, -2000f));
		OnCurrencyUpdateMessage(null);
		AppShell.Instance.EventMgr.AddListener<GlobalNavStateMessage>(OnGlobalNavStateMessage);
	}

	public override void OnActive()
	{
		base.OnActive();
		AppShell.Instance.EventMgr.AddListener<GlobalNavButtonVisibleMessage>(EnsureButtonVisible);
	}

	public override void OnInactive()
	{
		base.OnInactive();
		AppShell.Instance.EventMgr.RemoveListener<GlobalNavButtonVisibleMessage>(EnsureButtonVisible);
	}

	public void hideCurrency()
	{
		_background.SetVisible(false);
		_fractalIcon.IsVisible = false;
		_goldIcon.IsVisible = false;
		_totalGoldLabel.IsVisible = false;
		_totalFractalsLabel.IsVisible = false;
	}

	private void OnGlobalNavStateMessage(GlobalNavStateMessage msg)
	{
		toggle(msg.open);
	}

	private void clickedNav(GlobalNavType type)
	{
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Expected O, but got Unknown
		toggle(false);
		switch (type)
		{
		case GlobalNavType.Mission:
			LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.Missions);
			break;
		case GlobalNavType.Explore:
			GUIManager.Instance.ShowDialog(typeof(SHSZoneSelectorGadget), string.Empty, "SHSMainWindow", null, ModalLevelEnum.Default);
			break;
		case GlobalNavType.Store:
		{
			ShoppingWindow shoppingWindow = new ShoppingWindow();
			shoppingWindow.launch();
			break;
		}
		case GlobalNavType.Card:
			LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.CardGame);
			break;
		case GlobalNavType.Arcade:
			LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.Arcade);
			break;
		case GlobalNavType.Craft:
			CraftingWindow.requestCraftingWindow(-1);
			break;
		case GlobalNavType.Achievements:
		{
			AchievementWindow dialogWindow2 = new AchievementWindow((int)AppShell.Instance.Profile.UserId);
			GUIManager.Instance.ShowDynamicWindow(dialogWindow2, ModalLevelEnum.Default);
			break;
		}
		case GlobalNavType.Squad:
		{
			MySquadDataManager dataManager = new MySquadDataManager(AppShell.Instance.Profile);
			MySquadWindow dialogWindow = new MySquadWindow(dataManager);
			GUIManager.Instance.ShowDynamicWindow(dialogWindow, ModalLevelEnum.Default);
			break;
		}
		case GlobalNavType.Inventory:
		{
			if (SHSInventoryAnimatedWindow.instance != null)
			{
				SHSInventoryAnimatedWindow.instance.ToggleClosed();
				break;
			}
			SHSInventoryAnimatedWindow newInventoryWindow = new SHSInventoryAnimatedWindow();
			newInventoryWindow.OnToggleClose += (Action)(object)(Action)delegate
			{
				newInventoryWindow.Hide();
				newInventoryWindow.Dispose();
			};
			newInventoryWindow.AnimationOpenFinished += delegate
			{
				GUIManager.Instance.SetModal(newInventoryWindow, ModalLevelEnum.None);
			};
			GUIManager.Instance.ShowDynamicWindow(newInventoryWindow, "SHSMainWindow", DrawOrder.DrawFirst, DrawPhaseHintEnum.PreDraw, ModalLevelEnum.Default);
			break;
		}
		case GlobalNavType.Friends:
		{
			SHSHudWindows friendsWindow = (SHSHudWindows)Activator.CreateInstance(typeof(SHSFriendsListWindow));
			friendsWindow.OnToggleClose += (Action)(object)(Action)delegate
			{
				friendsWindow.Hide();
				friendsWindow.Dispose();
			};
			friendsWindow.AnimationOpenFinished += delegate
			{
				GUIManager.Instance.SetModal(friendsWindow, ModalLevelEnum.None);
			};
			GUIManager.Instance.ShowDynamicWindow(friendsWindow, "SHSMainWindow", DrawOrder.DrawFirst, DrawPhaseHintEnum.PreDraw, ModalLevelEnum.Default);
			break;
		}
		case GlobalNavType.HQ:
			LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.Hq);
			break;
		case GlobalNavType.Settings:
			GUIManager.Instance.ShowDialog(typeof(SHSOptionsGadget), null, ModalLevelEnum.Default);
			break;
		}
	}

	private GlobalNavButton createButton(string label, string iconPath, GlobalNavType type)
	{
		GlobalNavButton globalNavButton = new GlobalNavButton(label, iconPath, type);
		globalNavButton.background.MouseDown += delegate
		{
			ShsAudioSource.PlayAutoSound(GlobalNavStyle.AudioDown);
			clickedNav(type);
		};
		globalNavButton.IsVisible = true;
		_navButtonContainer.Add(globalNavButton);
		globalNavButton.SetPosition(0f, _currY);
		_currY += _deltY;
		return globalNavButton;
	}

	private void toggle(bool on)
	{
		if (on)
		{
			_toggleOn.IsVisible = false;
			_toggleOff.IsVisible = true;
			_navButtonContainer.IsVisible = true;
			if (_onBottom)
			{
				_navButtonContainer.SetPosition(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, new Vector2(0f, -55f));
				return;
			}
			GUISimpleControlWindow navButtonContainer = _navButtonContainer;
			Vector2 size = _background.Size;
			navButtonContainer.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, new Vector2(0f, size.y));
		}
		else
		{
			_toggleOn.IsVisible = true;
			_toggleOff.IsVisible = false;
			_navButtonContainer.IsVisible = false;
			if (_onBottom)
			{
				_navButtonContainer.SetPosition(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, new Vector2(0f, -2000f));
			}
			else
			{
				_navButtonContainer.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, new Vector2(0f, -2000f));
			}
		}
	}

	private void OnCurrencyUpdateMessage(CurrencyUpdateMessage msg)
	{
		CspUtils.DebugLog("AppShell.Instance: " + AppShell.Instance);
		CspUtils.DebugLog("AppShell.Instance.Profile: " + AppShell.Instance.Profile);

		if (AppShell.Instance != null && AppShell.Instance.Profile != null)
		{
			CspUtils.DebugLog("AppShell.Instance.Profile.Shards: " + AppShell.Instance.Profile.Shards);

			_totalGoldLabel.Text = string.Format("{0:n0}", AppShell.Instance.Profile.Gold);
			_totalFractalsLabel.Text = string.Format("{0:n0}", AppShell.Instance.Profile.Shards);
		}
	}

	private void EnsureButtonVisible(GlobalNavButtonVisibleMessage message)
	{
		if (isVisible)
		{
			clickedNav(message.type);
		}
	}
}
