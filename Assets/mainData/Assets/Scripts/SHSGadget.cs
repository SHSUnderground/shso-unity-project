using System.Collections.Generic;
using UnityEngine;

public abstract class SHSGadget : GUIDialogWindow
{
	public enum BackgroundType
	{
		OnePanel,
		TwoPanel
	}

	public struct CustomCloseData
	{
		public GameController.ControllerType placeToGo;

		public static CustomCloseData NoData
		{
			get
			{
				return new CustomCloseData(GameController.ControllerType.None);
			}
		}

		public CustomCloseData(GameController.ControllerType placeToGo)
		{
			this.placeToGo = placeToGo;
		}

		public override bool Equals(object obj)
		{
			//Discarded unreachable code: IL_0017, IL_0024
			try
			{
				return this == (CustomCloseData)obj;
			}
			catch
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public static bool operator ==(CustomCloseData dataOne, CustomCloseData dataTwo)
		{
			return dataOne.placeToGo == dataTwo.placeToGo;
		}

		public static bool operator !=(CustomCloseData dataOne, CustomCloseData dataTwo)
		{
			return dataOne.placeToGo != dataTwo.placeToGo;
		}
	}

	public abstract class GadgetWindow : GUIControlWindow
	{
		private CustomCloseData closeData = new CustomCloseData(GameController.ControllerType.None);

		public CustomCloseData CloseData
		{
			get
			{
				return closeData;
			}
			set
			{
				closeData = value;
			}
		}

		public GadgetWindow()
		{
			Traits.HitTestType = HitTestTypeEnum.Transparent;
			SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
			base.AnimationOnOpen = SHSAnimations.WindowOpenCloseDelegates.FadeIn(0.3f, this);
			base.AnimationOnClose = SHSAnimations.WindowOpenCloseDelegates.FadeOut(0.3f, this);
		}

		public void PerformCustomClose()
		{
			if (closeData != CustomCloseData.NoData)
			{
				AppShell.Instance.Transition(closeData.placeToGo);
			}
		}
	}

	public abstract class GadgetLeftWindow : GadgetWindow
	{
		public GadgetLeftWindow()
		{
			SetSize(LEFT_WINDOW_SIZE);
		}
	}

	public abstract class GadgetRightWindow : GadgetWindow
	{
		public GadgetRightWindow()
		{
			SetSize(RIGHT_WINDOW_SIZE);
		}
	}

	public abstract class GadgetCenterWindow : GadgetWindow
	{
		public GadgetCenterWindow()
		{
			SetSize(CENTER_WINDOW_SIZE);
		}
	}

	public abstract class GadgetTopWindow : GadgetWindow
	{
		public GadgetTopWindow()
		{
			SetSize(TOP_WINDOW_SIZE);
		}
	}

	protected GUIImage Background = new GUIImage();

	private GUIControlWindow ContentWindow = new GUIControlWindow();

	private GUIControlWindow blockTestZone;

	protected List<GUIControl> bounce = new List<GUIControl>();

	protected List<GUIControl> fade = new List<GUIControl>();

	private bool IsAnimationOpenFinished;

	public GUITBCloseButton CloseButton = new GUITBCloseButton();

	protected MouseClickDelegate CloseDelegate;

	public static readonly Vector2 GADGET_SIZE = new Vector2(2000f, 2000f);

	public Vector2 backgroundSize = new Vector2(938f, 636f);

	public static readonly Vector2 TOP_WINDOW_SIZE = new Vector2(592f, 141f);

	public static readonly Vector2 RIGHT_WINDOW_SIZE = new Vector2(459f, 459f);

	public static readonly Vector2 LEFT_WINDOW_SIZE = new Vector2(314f, 492f);

	public static readonly Vector2 CENTER_WINDOW_SIZE = new Vector2(938f, 636f);

	public static readonly Vector2 TOP_WINDOW_OFFSET = new Vector2(0f, -590f);

	public static readonly Vector2 RIGHT_WINDOW_OFFSET = new Vector2(169f, -315f);

	public static readonly Vector2 LEFT_WINDOW_OFFSET = new Vector2(-235f, -284f);

	public static readonly Vector2 CENTER_WINDOW_OFFSET = new Vector2(0f, -318f);

	public static readonly Vector2 ONE_PANEL_CLOSE_BUTTON_OFFSET = new Vector2(423f, -555f);

	public static readonly Vector2 TWO_PANEL_CLOSE_BUTTON_OFFSET = new Vector2(413f, -555f);

	private GadgetWindow currentTopWindow;

	private GadgetWindow lastTopWindow;

	private GadgetWindow currentRightWindow;

	private GadgetWindow lastRightWindow;

	private GadgetWindow currentLeftWindow;

	private GadgetWindow lastLeftWindow;

	private GadgetWindow currentCenterWindow;

	private GadgetWindow lastCenterWindow;

	public SHSGadget()
	{
		Traits.EventListenerRegistrationTrait = ControlTraits.EventListenerRegistrationTraitEnum.Ignore;
		Traits.FullScreenOpaqueBackgroundTrait = ControlTraits.FullScreenOpaqueBackgroundTraitEnum.HasFullScreenOpaqueBackground;
		Traits.HitTestType = HitTestTypeEnum.Transparent;
		blockTestZone = GUIControl.CreateControlBottomFrame<GUIControlWindow>(backgroundSize, Vector2.zero);
		Add(blockTestZone);
		SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, -673f));
		SetSize(GADGET_SIZE);
		Background.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle);
		Add(Background);
		ContentWindow.SetPosition(0f, 0f);
		ContentWindow.SetSize(Size);
		ContentWindow.HitTestType = HitTestTypeEnum.Transparent;
		Add(ContentWindow);
		CloseDelegate = CloseTheGadget;
		CloseButton.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(413f, -555f));
		CloseButton.SetSize(64f, 64f);
		CloseButton.Click += CloseDelegate;
		Add(CloseButton);
		bounce.Add(Background);
		fade.Add(CloseButton);
		base.AnimationOnOpen = SHSAnimations.WindowOpenCloseDelegates.GetOpenAnimation(bounce, fade, backgroundSize, 0.3f);
		base.AnimationOnClose = SHSAnimations.WindowOpenCloseDelegates.GetCloseAnimation(bounce, fade, backgroundSize, 0.3f);
		IsAnimationOpenFinished = false;
		base.AnimationOpenFinished += delegate
		{
			IsAnimationOpenFinished = true;
		};
	}

	public void CloseTheGadget(GUIControl sender, GUIClickEvent eventArgs)
	{
		CloseGadget();
		if (currentLeftWindow != null)
		{
			currentLeftWindow.PerformCustomClose();
		}
		if (currentRightWindow != null)
		{
			currentRightWindow.PerformCustomClose();
		}
		if (currentTopWindow != null)
		{
			currentTopWindow.PerformCustomClose();
		}
		if (currentCenterWindow != null)
		{
			currentCenterWindow.PerformCustomClose();
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		PlayOpenSFX();
		SHSInput.SetInputBlockingMode(this, SHSInput.InputBlockType.BlockWorld);
	}

	public virtual void CloseGadget()
	{
		SHSInput.RevertInputBlockingMode(this);
		Hide();
		if (currentLeftWindow != null)
		{
			currentLeftWindow.Hide();
		}
		if (currentRightWindow != null)
		{
			currentRightWindow.Hide();
		}
		if (currentTopWindow != null)
		{
			currentTopWindow.Hide();
		}
		if (currentCenterWindow != null)
		{
			currentCenterWindow.Hide();
		}
		PlayCloseSFX();
	}

	public override void OnInactive()
	{
		base.OnInactive();
		SHSInput.RevertInputBlockingMode(this);
	}

	public void SetBackgroundSize(Vector2 backgroundSize)
	{
		this.backgroundSize = backgroundSize;
		blockTestZone.SetSize(backgroundSize);
		Vector2 gADGET_SIZE = GADGET_SIZE;
		Offset = new Vector2(0f, (0f - (gADGET_SIZE.y - backgroundSize.y)) * 0.5f);
		base.AnimationOnOpen = SHSAnimations.WindowOpenCloseDelegates.GetOpenAnimation(bounce, fade, backgroundSize, 0.3f);
		base.AnimationOnClose = SHSAnimations.WindowOpenCloseDelegates.GetCloseAnimation(bounce, fade, backgroundSize, 0.3f);
	}

	public void SetBackgroundImage(string backgroundImageLocation)
	{
		Background.TextureSource = backgroundImageLocation;
	}

	public void SetupOpeningWindow(BackgroundType backgroundType, params GadgetWindow[] gadgetWindows)
	{
		AnimationFinishDelegate value;
		switch (backgroundType)
		{
		case BackgroundType.OnePanel:
			value = setupOnePanelBackground(gadgetWindows);
			break;
		case BackgroundType.TwoPanel:
			value = setupTwoPanelBackground(gadgetWindows);
			break;
		default:
			value = delegate
			{
			};
			break;
		}
		base.AnimationOpenFinished += value;
	}

	public void SetupOpeningTopWindow(GadgetTopWindow topWindow)
	{
		base.AnimationOpenFinished += delegate
		{
			SetTopWindow(topWindow);
		};
	}

	private AnimationFinishDelegate setupOnePanelBackground(GadgetWindow[] gadgetWindows)
	{
		Background.TextureSource = "persistent_bundle|gadget_base_one_panel";
		CloseButton.Offset = ONE_PANEL_CLOSE_BUTTON_OFFSET;
		if (gadgetWindows.Length != 1 || !(gadgetWindows[0] is GadgetCenterWindow))
		{
			CspUtils.DebugLog("cannot create a One Panel Gadget window because was not given exactly one GadgetCenterWindow");
			return delegate
			{
			};
		}
		return delegate
		{
			SetCenterWindow(gadgetWindows[0] as GadgetCenterWindow);
		};
	}

	private AnimationFinishDelegate setupTwoPanelBackground(GadgetWindow[] gadgetWindows)
	{
		Background.TextureSource = "persistent_bundle|gadget_base_two_panel";
		CloseButton.Offset = TWO_PANEL_CLOSE_BUTTON_OFFSET;
		if (gadgetWindows.Length != 2 || !(gadgetWindows[0] is GadgetLeftWindow) || !(gadgetWindows[1] is GadgetRightWindow))
		{
			CspUtils.DebugLog("cannot create a Two Panel Gadget window because was not given exactly one GadgetLeftWindow and one GadgetRightWindow (in that order)");
			return delegate
			{
			};
		}
		return delegate
		{
			SetLeftWindow(gadgetWindows[0] as GadgetLeftWindow);
			SetRightWindow(gadgetWindows[1] as GadgetRightWindow);
		};
	}

	private bool SetWindow(ref GadgetWindow currentWindow, ref GadgetWindow lastWindow, GadgetWindow win, Vector2 loc, DrawOrder drawOrder, AnimationFinishDelegate animFin)
	{
		if (!IsAnimationOpenFinished)
		{
			return false;
		}
		if (win == currentWindow)
		{
			return false;
		}
		if (currentWindow != null)
		{
			if (currentWindow.AnimationInProgress)
			{
				return false;
			}
			if (lastWindow != null && lastWindow.AnimationInProgress)
			{
				return false;
			}
			currentWindow.AnimationCloseFinished += animFin;
			currentWindow.IsVisible = false;
			lastWindow = currentWindow;
			currentWindow = win;
			return true;
		}
		ContentWindow.Add(win, drawOrder);
		win.SetVisibilityAndSkipAnimation(false);
		win.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, loc);
		win.IsVisible = true;
		lastWindow = currentWindow;
		currentWindow = win;
		return true;
	}

	public bool SetTopWindow(GadgetTopWindow win)
	{
		return SetWindow(ref currentTopWindow, ref lastTopWindow, win, TOP_WINDOW_OFFSET, DrawOrder.DrawFirst, delegate
		{
			ContentWindow.Remove(currentTopWindow);
			currentTopWindow.AnimationCloseFinishedClear();
			currentTopWindow = null;
			SetTopWindow(win);
		});
	}

	public void SetTopWindowImmediate(GadgetTopWindow win)
	{
		if (win != currentTopWindow)
		{
			if (currentTopWindow != null)
			{
				ContentWindow.Remove(currentTopWindow);
				currentTopWindow.SetSkipAnimation(true);
				currentTopWindow.IsVisible = false;
			}
			ContentWindow.Add(win, DrawOrder.DrawFirst);
			win.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, TOP_WINDOW_OFFSET);
			win.SetSkipAnimation(true);
			win.IsVisible = true;
			lastTopWindow = currentTopWindow;
			currentTopWindow = win;
		}
	}

	public void SetCenterWindowImmediate(GadgetCenterWindow win)
	{
		if (win != currentCenterWindow)
		{
			if (currentCenterWindow != null)
			{
				currentCenterWindow.SetSkipAnimation(true);
				currentCenterWindow.IsVisible = false;
				ContentWindow.Remove(currentCenterWindow);
			}
			ContentWindow.Add(win, DrawOrder.DrawLast);
			win.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, CENTER_WINDOW_OFFSET);
			win.SetSkipAnimation(true);
			win.IsVisible = true;
			lastCenterWindow = currentCenterWindow;
			currentCenterWindow = win;
			currentCenterWindow.AnimationCloseFinished += delegate
			{
				ContentWindow.Remove(win);
				win.AnimationCloseFinishedClear();
				currentCenterWindow = null;
			};
		}
	}

	public bool SetRightWindow(GadgetRightWindow win)
	{
		return SetWindow(ref currentRightWindow, ref lastRightWindow, win, RIGHT_WINDOW_OFFSET, DrawOrder.DrawLast, delegate
		{
			ContentWindow.Remove(currentRightWindow);
			currentRightWindow.AnimationCloseFinishedClear();
			currentRightWindow = null;
			SetRightWindow(win);
		});
	}

	public bool SetLeftWindow(GadgetLeftWindow win)
	{
		return SetWindow(ref currentLeftWindow, ref lastLeftWindow, win, LEFT_WINDOW_OFFSET, DrawOrder.DrawLast, delegate
		{
			ContentWindow.Remove(currentLeftWindow);
			currentLeftWindow.AnimationCloseFinishedClear();
			currentLeftWindow = null;
			SetLeftWindow(win);
		});
	}

	public bool SetCenterWindow(GadgetCenterWindow win)
	{
		return SetWindow(ref currentCenterWindow, ref lastCenterWindow, win, CENTER_WINDOW_OFFSET, DrawOrder.DrawLast, delegate
		{
			ContentWindow.Remove(currentCenterWindow);
			currentCenterWindow.AnimationCloseFinishedClear();
			currentCenterWindow = null;
			SetCenterWindow(win);
		});
	}

	private static void PlayOpenSFX()
	{
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("global_window_in"));
	}

	private static void PlayCloseSFX()
	{
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("global_window_out"));
	}
}
