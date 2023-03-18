using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MenuChatMainWindow : GUIWindow, ICaptureHandler, ICaptureManager
{
	private int DelayedHideMenuTicks = 10;

	[CompilerGenerated]
	private Dictionary<int, MenuLevelInfo> _003CLevelInfo_003Ek__BackingField;

	[CompilerGenerated]
	private CaptureManager _003CManager_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CMenuShowing_003Ek__BackingField;

	public Dictionary<int, MenuLevelInfo> LevelInfo
	{
		[CompilerGenerated]
		get
		{
			return _003CLevelInfo_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CLevelInfo_003Ek__BackingField = value;
		}
	}

	public new CaptureManager Manager
	{
		[CompilerGenerated]
		get
		{
			return _003CManager_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CManager_003Ek__BackingField = value;
		}
	}

	public bool MenuShowing
	{
		[CompilerGenerated]
		get
		{
			return _003CMenuShowing_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CMenuShowing_003Ek__BackingField = value;
		}
	}

	public MenuChatMainWindow()
	{
		LevelInfo = new Dictionary<int, MenuLevelInfo>();
		SetPositionAndSize(QuickSizingHint.ScreenSize);
		Traits.HitTestType = HitTestTypeEnum.Transparent;
		Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Inherit;
	}

	public void ShowMenu(GameController.ControllerType controllerType, Rect originRect)
	{
		if (LevelInfo.ContainsKey(1))
		{
			HideMenu();
			return;
		}
		MenuChatGroup menuForGameArea = AppShell.Instance.MenuChatManager.GetMenuForGameArea(controllerType);
		MenuLevelInfo level = new MenuLevelInfo(0);
		ShowMenu(level, menuForGameArea, originRect);
	}

	public void HideMenu()
	{
		if (MenuShowing)
		{
			foreach (IGUIControl control in controlList)
			{
				control.Hide();
			}
			LevelInfo.Clear();
			MenuShowing = false;
		}
	}

	public void ShowMenu(MenuLevelInfo level, MenuChatGroup group, Rect originRect)
	{
		HidePreviousMenus(level);
		foreach (IGUIControl item in controlList.FindAll(delegate(IGUIControl c)
		{
			return ((MenuChatItemWindow)c).MenuLevel == level;
		}))
		{
			((MenuChatItemWindow)item).MouseOverConfigure(false);
		}
		if (level.CurrentMenuChatItemWindow != null)
		{
			level.CurrentMenuChatItemWindow.MouseOverConfigure(true);
		}
		if (group.MenuChatGroups.Count == 0)
		{
			SortMenus();
			return;
		}
		MenuLevelInfo menuLevelInfo = new MenuLevelInfo(level.Ordinal + 1);
		LevelInfo[menuLevelInfo.Ordinal] = menuLevelInfo;
		float num = 0f;
		int num2 = 0;
		List<GUIControl> list = new List<GUIControl>();
		foreach (MenuChatGroup menuChatGroup in group.MenuChatGroups)
		{
			MenuChatItemWindow menuChatItemWindow = new MenuChatItemWindow(menuChatGroup);
			menuChatItemWindow.Id = menuChatGroup.PhraseKey.ToString();
			menuChatItemWindow.MenuLevel = LevelInfo[menuLevelInfo.Ordinal];
			Add(menuChatItemWindow, DrawOrder.DrawFirst, DrawPhaseHintEnum.PostDraw);
			list.Add(menuChatItemWindow);
			num = Math.Max(num, menuChatItemWindow.TextWidth);
		}
		num = (menuLevelInfo.MenuWidth = num + 46f);
		Vector2 origin;
		int vDirection;
		configureDynamicMenuLocation(originRect, num, group.MenuChatGroups.Count * 28, out origin, out vDirection);
		for (int i = 0; i < list.Count; i++)
		{
			GUIControl gUIControl = list[i];
			gUIControl.SetPosition(new Vector2(origin.x, origin.y + (float)(num2 * vDirection)));
			num2 += 28;
		}
		list.Clear();
		SortMenus();
		MenuShowing = true;
		DrawPhaseHint = DrawPhaseHintEnum.PostDraw;
	}

	private void SortMenus()
	{
		List<IGUIControl> list = new List<IGUIControl>();
		List<MenuLevelInfo> list2 = Enumerable.ToList(LevelInfo.Values);
		list2.Sort(delegate(MenuLevelInfo a, MenuLevelInfo b)
		{
			return (a.Ordinal <= b.Ordinal) ? 1 : (-1);
		});
		using (List<MenuLevelInfo>.Enumerator enumerator = list2.GetEnumerator())
		{
			MenuLevelInfo level;
			while (enumerator.MoveNext())
			{
				level = enumerator.Current;
				List<IGUIControl> list3 = ControlList.FindAll(delegate(IGUIControl control)
				{
					return ((MenuChatItemWindow)control).MenuLevel == level;
				});
				list3.Sort(delegate(IGUIControl a, IGUIControl b)
				{
					return (!(a.Rect.y < b.Rect.y)) ? 1 : (-1);
				});
				foreach (IGUIControl item in list3)
				{
					if (((MenuChatItemWindow)item).Highlighted)
					{
						list.Add(item);
					}
					else
					{
						item.SendToBack();
					}
				}
			}
		}
		for (int num = list.Count - 1; num >= 0; num--)
		{
			list[num].BringToFront();
		}
	}

	private void configureDynamicMenuLocation(Rect originRect, float largestWidth, int targetHeight, out Vector2 origin, out int vDirection)
	{
		if (!(originRect.x + originRect.width + largestWidth > Rect.x + Rect.width))
		{
			origin = new Vector2(originRect.x + originRect.width, originRect.y);
		}
		else
		{
			origin = new Vector2(originRect.x - largestWidth, originRect.y);
		}
		if (originRect.y + (float)targetHeight > Rect.y + Rect.height)
		{
			vDirection = -1;
		}
		else
		{
			vDirection = 1;
		}
	}

	private void HidePreviousMenus(MenuLevelInfo level)
	{
		foreach (IGUIControl item in controlList.FindAll(delegate(IGUIControl c)
		{
			return ((MenuChatItemWindow)c).MenuLevel > level;
		}))
		{
			item.Hide();
		}
	}

	public override void OnShow()
	{
		AppShell.Instance.EventMgr.AddListener<MenuChatActivateMessage>(OnMenuActivate);
		AppShell.Instance.EventMgr.AddListener<MenuChatSelectedMessage>(OnMenuSelected);
		AppShell.Instance.OnNewControllerReady += OnNewControllerReady;
		base.OnShow();
	}

	public override void OnHide()
	{
		AppShell.Instance.OnNewControllerReady -= OnNewControllerReady;
		AppShell.Instance.EventMgr.RemoveListener<MenuChatActivateMessage>(OnMenuActivate);
		AppShell.Instance.EventMgr.RemoveListener<MenuChatSelectedMessage>(OnMenuSelected);
		base.OnHide();
	}

	public override void OnUpdate()
	{
		if (DelayedHideMenuTicks > 0)
		{
			DelayedHideMenuTicks--;
			if (DelayedHideMenuTicks == 0)
			{
				HideMenu();
			}
		}
		base.OnUpdate();
	}

	private void OnMenuSelected(MenuChatSelectedMessage message)
	{
		HideMenu();
		GameObject targetPlayer = null;
		SocialSpaceController socialSpaceController = GameController.GetController() as SocialSpaceController;
		if (socialSpaceController != null)
		{
			targetPlayer = socialSpaceController.SelectedPlayer;
		}
		DirectedMenuChat.DirectMenuChatEmote(GameController.GetController().LocalPlayer, message.Group, targetPlayer);
	}

	private void OnMenuActivate(MenuChatActivateMessage message)
	{
		if (message.Group == null)
		{
			ShowMenu(GameController.GetController().controllerType, message.OriginRect);
		}
		else
		{
			ShowMenu(message.MenuLevel, message.Group, message.OriginRect);
		}
	}

	private void OnNewControllerReady(AppShell.GameControllerTypeData newGameTypeData, GameController controller)
	{
		HideMenu();
	}

	public void CaptureHandlerGotInput(ICaptureHandler handler)
	{
		CspUtils.DebugLog("got capture handler: " + handler + " and it's this?" + (handler == this));
		if (handler != this)
		{
		}
	}

	public override CaptureHandlerResponse HandleCapture(SHSKeyCode code)
	{
		if (code.code == KeyCode.Mouse0 && code.source is MenuChatItemWindow)
		{
			return CaptureHandlerResponse.Passthrough;
		}
		if (code.code == KeyCode.Mouse0)
		{
			DelayedHideMenuTicks = 1;
		}
		return CaptureHandlerResponse.Passthrough;
	}
}
