using System;
using System.Collections.Generic;
//using System.Runtime.CompilerServices;
using UnityEngine;

public class SHSHudWheels : GUISimpleControlWindow, IDisposable
{
	public class SHSHudWheelAnimated : GUIControlWindow
	{
		public class IdleButton : GUISubScalingWindow
		{
			private float sizeMod = 1f;

			private float setSize = 128f;

			private GUIHotSpotButton hotSpot;

			private GUIAnimatedFrameButton animation;

			public float SizeMod
			{
				get
				{
					return this.sizeMod;
				}
				set
				{
					this.sizeMod = value;
					this.AnimateSize(this.setSize);
				}
			}

			public IdleButton(string location, int numberOfFrames, bool flip, string tooltipText) : base(128f, 128f)
			{
				this.animation = new GUIAnimatedFrameButton(location, numberOfFrames);
				this.animation.SetSize((float)((!flip) ? 128 : (-128)), 128f);
				this.animation.SetPosition(GUIControl.DockingAlignmentEnum.Middle, GUIControl.AnchorAlignmentEnum.Middle);
				base.SetPosition(GUIControl.DockingAlignmentEnum.Middle, GUIControl.AnchorAlignmentEnum.Middle);
				this.hotSpot = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(new Vector2(128f, 128f), Vector2.zero);
				this.hotSpot.MouseOver += delegate(GUIControl sender, GUIMouseEvent EventData)
				{
					this.animation.CurrentCustomAnimation = GUIAnimatedFrameButton.CustomAnimation.AnimateToFinalStateAndHold;
				};
				this.hotSpot.MouseOut += delegate(GUIControl sender, GUIMouseEvent EventData)
				{
					this.animation.CurrentCustomAnimation = GUIAnimatedFrameButton.CustomAnimation.Idle;
				};
				this.hotSpot.ToolTip = new GUIControl.NamedToolTipInfo(tooltipText);
				this.hotSpot.HitTestType = GUIControl.HitTestTypeEnum.Circular;
				this.hotSpot.HitTestSize = new Vector2(0.75f, 0.75f);
				base.AddItem(this.animation);
				base.AddItem(this.hotSpot);
			}

			public void AnimateSize(float x)
			{
				this.setSize = x;
				this.SetSize(new Vector2(x, x) * this.sizeMod);
			}

			public void AddClick(GUIControl.MouseClickDelegate del)
			{
				this.hotSpot.Click += del;
			}
		}

		public class HUDAnimations : SHSAnimations
		{
			private sealed class _AnimateOpen_c__AnonStorey23E
			{
				internal SHSHudWheels.SHSHudWheelAnimated headWindow;

				internal void __m__364()
				{
					for (int i = 0; i < this.headWindow.hudButtons.Count; i++)
					{
						SHSHudWheels.SHSHudWheelAnimated.HUDButton hUDButton = this.headWindow.hudButtons[i];
						hUDButton.DisableButtonDueToAnimationInProgress = false;
					}
				}
			}

			private sealed class _CloseIfIdle_c__AnonStorey23F
			{
				internal SHSHudWheels.SHSHudWheelAnimated headWindow;

				internal void __m__365()
				{
					if (!this.headWindow.CloseIfIdleValid() || AppShell.Instance.TransitionHandler.CurrentWaitWindow != null)
					{
						return;
					}
					this.headWindow.Close();
				}
			}

			private const float HUB_MOVE_OUT_TIME = 0.38f;

			private const float ANIMATION_TIME = 0.4f;

			private const float FADE_TIME = 0.1f;

			private const float SELECT_TIME = 0.3f;

			private const float START_POS = 0f;

			public static AnimClip AnimateOpen(SHSHudWheels.SHSHudWheelAnimated headWindow)
			{
				float num = (float)((headWindow.loc != SHSHudWheels.WheelLocation.Left) ? (-1) : 1);
				AnimClip animClip = SHSAnimations.Generic.Blank();
				for (int i = 0; i < headWindow.hudButtons.Count; i++)
				{
					SHSHudWheels.SHSHudWheelAnimated.HUDButton hUDButton = headWindow.hudButtons[i];
					hUDButton.DisableButtonDueToAnimationInProgress = true;
					animClip ^= AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Constant(1f, 0.38f) | AnimClipBuilder.Path.Sin(0f, 1f, 0.3f) * AnimClipBuilder.Path.Linear(15f, 5f, 0.3f) * -0.006666667f + 1f, new Action<float>(hUDButton.AdditionalBounceOffset));
				}
				animClip ^= AnimClipBuilder.Delta.SizeXY(AnimClipBuilder.Path.Constant(1f, 0.38f) | AnimClipBuilder.Path.Sin(0f, 1f, 0.3f) * AnimClipBuilder.Path.Linear(15f, 5f, 0.3f), new GUIControl[]
				{
					headWindow.bkg
				});
				animClip = (animClip ^ AnimClipBuilder.Absolute.OffsetX(AnimClipBuilder.Path.Linear(48f * num, 147f * num, 0.38f), new GUIControl[]
				{
					headWindow
				}) ^ AnimClipBuilder.Absolute.OffsetY(AnimClipBuilder.Path.Linear(-50f, -115f, 0.38f), new GUIControl[]
				{
					headWindow
				}) ^ AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(128f, 0f, 0.38f), new Action<float>(headWindow.idleButton.AnimateSize)) ^ AnimClipBuilder.Absolute.SizeXY(AnimClipBuilder.Path.Linear(0f, 300f, 0.38f), new GUIControl[]
				{
					headWindow.HubWheelWindow
				}) ^ AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, 0.38f), new GUIControl[]
				{
					headWindow.HubWheelWindow
				}) ^ AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(1f, 0f, 0.38f), new GUIControl[]
				{
					headWindow.idleButton
				}));
				animClip.OnFinished += delegate
				{
					for (int j = 0; j < headWindow.hudButtons.Count; j++)
					{
						SHSHudWheels.SHSHudWheelAnimated.HUDButton hUDButton2 = headWindow.hudButtons[j];
						hUDButton2.DisableButtonDueToAnimationInProgress = false;
					}
				};
				return animClip;
			}

			public static AnimClip AnimateClose(SHSHudWheels.SHSHudWheelAnimated headWindow)
			{
				for (int i = 0; i < headWindow.hudButtons.Count; i++)
				{
					SHSHudWheels.SHSHudWheelAnimated.HUDButton hUDButton = headWindow.hudButtons[i];
					hUDButton.DisableButtonDueToAnimationInProgress = true;
				}
				float num = (float)((headWindow.loc != SHSHudWheels.WheelLocation.Left) ? (-1) : 1);
				return AnimClipBuilder.Absolute.OffsetX(AnimClipBuilder.Path.Linear(147f * num, 48f * num, 0.38f), new GUIControl[]
				{
					headWindow
				}) ^ AnimClipBuilder.Absolute.OffsetY(AnimClipBuilder.Path.Linear(-115f, -50f, 0.38f), new GUIControl[]
				{
					headWindow
				}) ^ AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(0f, 128f, 0.38f) | AnimClipBuilder.Path.Sin(0f, 1f, 0.25f) * AnimClipBuilder.Path.Linear(10f, 2f, 0.25f) + 128f, new Action<float>(headWindow.idleButton.AnimateSize)) ^ AnimClipBuilder.Absolute.SizeXY(AnimClipBuilder.Path.Linear(300f, 0f, 0.38f), new GUIControl[]
				{
					headWindow.HubWheelWindow
				}) ^ AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, 0.38f), new GUIControl[]
				{
					headWindow.idleButton
				}) ^ AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(1f, 0f, 0.38f), new GUIControl[]
				{
					headWindow.HubWheelWindow
				});
			}

			public static AnimClip ToSelected(SHSHudWheels.SHSHudWheelAnimated.HUDButton hb)
			{
				hb.ToLastFrameHold(true);
				return AnimClipBuilder.Absolute.OffsetX(SHSAnimations.GenericPaths.LinearWithBounce(hb.Offset.x, 0f, 0.3f), new GUIControl[]
				{
					hb
				}) ^ AnimClipBuilder.Absolute.OffsetY(SHSAnimations.GenericPaths.LinearWithBounce(hb.Offset.y, 0f, 0.3f), new GUIControl[]
				{
					hb
				}) ^ AnimClipBuilder.Custom.Function(SHSAnimations.GenericPaths.LinearWithBounce(1f, 1.4f, 0.3f), new Action<float>(hb.UpdateSizeFunction)) ^ SHSAnimations.Generic.FadeOut(hb.bkg, 0.3f);
			}

			public static AnimClip ToUnselected(SHSHudWheels.SHSHudWheelAnimated.HUDButton hb)
			{
				hb.ToLastFrameHold(false);
				return AnimClipBuilder.Absolute.OffsetX(SHSAnimations.GenericPaths.LinearWithBounce(0f, hb.ReturnOffset.x, 0.3f), new GUIControl[]
				{
					hb
				}) ^ AnimClipBuilder.Absolute.OffsetY(SHSAnimations.GenericPaths.LinearWithBounce(0f, hb.ReturnOffset.y, 0.3f), new GUIControl[]
				{
					hb
				}) ^ AnimClipBuilder.Custom.Function(SHSAnimations.GenericPaths.LinearWithBounce(1.4f, 1f, 0.3f), new Action<float>(hb.UpdateSizeFunction)) ^ SHSAnimations.Generic.FadeIn(hb.bkg, 0.3f);
			}

			public static AnimClip CloseIfIdle(SHSHudWheels.SHSHudWheelAnimated headWindow)
			{
				AnimClip animClip = SHSAnimations.Generic.Wait(10f);
				animClip.OnFinished += delegate
				{
					if (!headWindow.CloseIfIdleValid() || AppShell.Instance.TransitionHandler.CurrentWaitWindow != null)
					{
						return;
					}
					headWindow.Close();
				};
				return animClip;
			}
		}

		public class HUDButton : GUISubScalingWindow
		{
			private sealed class _HUDButton_c__AnonStorey240
			{
				internal SHSHudWheels.SHSHudWheelAnimated ParentWindow;

				internal SHSHudWheels.SHSHudWheelAnimated.HUDButton __f__this;

				internal void __m__366(GUIControl x, GUIClickEvent y)
				{
					this.ParentWindow.RecievedButtonClick(this.__f__this);
					this.__f__this.TestCustomTooltips();
				}
			}

			public const float WINDOW_SIZE = 180f;

			public GUIImage bkg;

			private GUIImage bkg2;

			public GUIAnimatedFrameButton button;

			public SHSHudWheels.CloseType closeType;

			public SHSHudWheels.ButtonType buttonType;

			public float startPosition;

			public float finalPosition;

			public Vector2 ReturnOffset;

			public bool DisableButtonDueToAnimationInProgress;

			public Action launchDelegate;

			public Type HudWindowType;

			public bool HudWindowRemoveOnHudClosed;

			public AnimClip SelectUnselectPiece;

			public GUIControl.DrawPhaseHintEnum HudWindowDrawPhaseHint;

			public HUDButton(SHSHudWheels.ButtonType buttonType, SHSHudWheels.CloseType closeType, SHSHudWheels.SHSHudWheelAnimated ParentWindow) : base(180f, 180f)
			{
				SHSHudWheels.SHSHudWheelAnimated.HUDButton __f__this = this;
				this.closeType = closeType;
				this.buttonType = buttonType;
				this.Traits.VisibilityTrait = GUIControl.ControlTraits.VisibilityTraitEnum.Manual;
				this.SetControlFlag(GUIControl.ControlFlagSetting.AlphaCascade, true, true);
				this.SetSize(180f, 180f);
				base.SetPosition(GUIControl.DockingAlignmentEnum.Middle, GUIControl.AnchorAlignmentEnum.Middle, GUIControl.OffsetType.Absolute, new Vector2(0f, 0f));
				this.bkg2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(53f, 53f), new Vector2(0f, 0f));
				this.bkg2.TextureSource = "hud_bundle|mshs_persistent_hub_active_button";
				base.AddItem(this.bkg2);
				this.bkg = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(53f, 53f), new Vector2(0f, 0f));
				this.bkg.TextureSource = "hud_bundle|mshs_persistent_hub_inactive_button";
				base.AddItem(this.bkg);
				this.button = SHSHudWheels.GetAnimatedButton(buttonType);
				this.button.SetPosition(GUIControl.DockingAlignmentEnum.Middle, GUIControl.AnchorAlignmentEnum.Middle, GUIControl.OffsetType.Absolute, new Vector2(0f, 0f));
				this.button.SetSize(180f, 180f);
				this.button.HitTestType = GUIControl.HitTestTypeEnum.Circular;
				this.button.HitTestSize = new Vector2(0.294444442f, 0.294444442f);
				this.button.Click += delegate(GUIControl x, GUIClickEvent y)
				{
					ParentWindow.RecievedButtonClick(__f__this);
					__f__this.TestCustomTooltips();
				};
				this.button.SetControlFlag(GUIControl.ControlFlagSetting.EnableInherited, true, true);
				base.AddItem(this.button);
				this.button.EntitlementFlag = SHSHudWheels.GetEntitlementFlag(buttonType);
			}

			public override void OnShow()
			{
				base.OnShow();
				this.TestCustomTooltips();
			}

			public void TestCustomTooltips()
			{
				if (this.buttonType == SHSHudWheels.ButtonType.Chat)
				{
					this.SetupChatTooltips();
				}
			}

			public void SetupChatTooltips()
			{
				this.button.ToolTip.ContextStringLookup[GUIControl.GUIContext.Status.Default] = ((!SHSEmoteChatBar.IsOpenChatVisible) ? SHSHudWheels.GetTooltipText(SHSHudWheels.ButtonType.Chat) : "#TT_HUDWHEEL_16");
			}

			public override void ConfigureRequiredContent(List<ContentReference> ContentReferenceList)
			{
				this.button.ConfigureRequiredContent(ContentReferenceList);
			}

			public void ToLastFrameHold(bool enable)
			{
				this.button.CurrentCustomAnimation = ((!enable) ? GUIAnimatedFrameButton.CustomAnimation.None : GUIAnimatedFrameButton.CustomAnimation.AnimateToFinalStateAndHold);
			}

			public void FireInternalClick()
			{
				this.button.FireMouseClick(null);
			}

			public void AdditionalBounceOffset(float value)
			{
				this.Offset = this.ReturnOffset * value;
			}

			public void UpdateSizeFunction(float value)
			{
				this.bkg.SetSize(53f * value, 53f * value);
				this.bkg2.SetSize(53f * value, 53f * value);
				this.button.SetSize(180f * value, 180f * value);
				this.SetSize(180f * value, 180f * value);
			}

			public bool IsHovering()
			{
				return this.button.Hover;
			}
		}

		private sealed class _AddHudWindow_c__AnonStorey239
		{
			internal SHSHudWheels.SHSHudWheelAnimated.HUDButton button;

			internal SHSHudWheels.SHSHudWheelAnimated __f__this;

			internal void __m__355()
			{
				this.__f__this.Close();
				this.__f__this.RemoveHudWindow(this.button);
			}
		}

		private sealed class _ChangeButtonEnabledStatus_c__AnonStorey23A
		{
			internal SHSHudWheels.ButtonType buttonType;

			internal bool __m__356(SHSHudWheels.SHSHudWheelAnimated.HUDButton button)
			{
				return button.buttonType == this.buttonType;
			}
		}

		private sealed class _ChangeButtonEnabledStatus_c__AnonStorey23B
		{
			internal SHSHudWheels.ButtonType buttonType;

			internal bool __m__357(SHSHudWheels.SHSHudWheelAnimated.HUDButton button)
			{
				return button.buttonType == this.buttonType;
			}
		}

		private sealed class _EnsureButtonVisible_c__AnonStorey23C
		{
			internal EnsureButtonVisibleMessage message;

			internal bool __m__358(SHSHudWheels.SHSHudWheelAnimated.HUDButton button)
			{
				return button.buttonType == this.message.button;
			}
		}

		private sealed class _ToggleButtonVisibility_c__AnonStorey23D
		{
			internal ToggleButtonVisibilityMessage message;

			internal bool __m__359(SHSHudWheels.SHSHudWheelAnimated.HUDButton button)
			{
				return button.buttonType == this.message.button;
			}
		}

		public const float CIRCLE_RADIUS = 75f;

		public const float OFFSET_AMOUNT_X = 147f;

		public const float OFFSET_AMOUNT_Y = 115f;

		private const float BUTTON_SIZE = 180f;

		public const float BACKGROUND_SIZE = 53f;

		public const float IDLE_BUTTON_OFFSET_X = 48f;

		public const float IDLE_BUTTON_OFFSET_Y = 50f;

		public const float BLOCK_TEST_SIZE_OPEN = 0.7f;

		public const float BLOCK_TEST_SIZE_CLOSED = 0.35f;

		private Dictionary<SHSHudWheels.SHSHudWheelAnimated.HUDButton, SHSHudWindows> HudWindows = new Dictionary<SHSHudWheels.SHSHudWheelAnimated.HUDButton, SHSHudWindows>();

		private EnsureButtonVisibleMessage SavedMessageForCallback;

		private SHSHudWheels.OpenOrClosed openOrClosed;

		private SHSHudWheels.SHSHudWheelAnimated.IdleButton idleButton;

		private GUISubScalingWindow HubWheelWindow;

		private GUIDrawTexture bkg;

		public List<SHSHudWheels.SHSHudWheelAnimated.HUDButton> hudButtons;

		private SHSHudWheels.SHSHudWheelAnimated.HUDButton selectedButton;

		private SHSHudWheels.WheelLocation loc;

		private GameObject openSFX;

		private GameObject closeSFX;

		private bool queuedOpen;

		private bool queuedClose;

		private bool queuedClicked;

		private SHSHudWheels.SHSHudWheelAnimated.HUDButton queuedButton;

		private AnimClip CloseIfIdle;

		private Action OnOpened;

		private static Action __f__am_cache11;

		private static Action __f__am_cache12;

		private static Action __f__am_cache13;

		private static Action __f__am_cache14;

		private static Action __f__am_cache15;

		private static Action __f__am_cache16;

		private static Action __f__am_cache17;

		private static Action __f__am_cache18;

		private static Action __f__am_cache19;

		private static Action __f__am_cache1A;

		private static Action __f__am_cache1B;

		private static Action __f__am_cache1C;

		//private event Action OnOpened;

		public SHSHudWheelAnimated(SHSHudWheels.WheelLocation loc)
		{
			this.loc = loc;
			this.hudButtons = new List<SHSHudWheels.SHSHudWheelAnimated.HUDButton>();
			this.SetSize(300f, 300f);
			this.openOrClosed = SHSHudWheels.OpenOrClosed.Closed;
			this.Traits.BlockTestType = GUIControl.BlockTestTypeEnum.Circular;
			this.Traits.HitTestType = GUIControl.HitTestTypeEnum.Circular;
			base.HitTestSize = Vector2.zero;
			base.BlockTestSize = new Vector2(0.35f, 0.35f);
			this.HubWheelWindow = new GUISubScalingWindow(300f, 300f);
			this.HubWheelWindow.SetPosition(GUIControl.DockingAlignmentEnum.Middle, GUIControl.AnchorAlignmentEnum.Middle);
			this.Add(this.HubWheelWindow);
			if (loc == SHSHudWheels.WheelLocation.Left)
			{
				this.idleButton = new SHSHudWheels.SHSHudWheelAnimated.IdleButton("hud_bundle|hud_ball_", 8, false, "#TT_HUDWHEEL_13");
			}
			else
			{
				this.idleButton = new SHSHudWheels.SHSHudWheelAnimated.IdleButton("hud_bundle|hud_ball_", 8, true, "#TT_HUDWHEEL_13");
			}
			this.idleButton.AddClick(delegate(GUIControl sender, GUIClickEvent EventData)
			{
				this.OpenAndClose();
			});
			this.Add(this.idleButton);
			this.bkg = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(new Vector2(174f, 174f), new Vector2(0f, 0f));
			this.bkg.TextureSource = "hud_bundle|mshs_persistent_hub_backdrop";
			this.HubWheelWindow.AddItem(this.bkg);
			this.bkg.HitTestType = GUIControl.HitTestTypeEnum.Circular;
			this.bkg.Click += delegate(GUIControl sender, GUIClickEvent EventData)
			{
				this.OpenAndClose();
			};
			this.bkg.MouseOver += delegate(GUIControl sender, GUIMouseEvent EventData)
			{
				this.RemoveCloseIfIdle();
			};
			this.bkg.MouseOut += delegate(GUIControl sender, GUIMouseEvent EventData)
			{
				this.RefreshCloseIfIdle();
			};
			this.SetupWheel();
			this.SetupMaxData();
		}

		private void SetupWheel()
		{
			if (this.loc == SHSHudWheels.WheelLocation.Left)
			{
				this.SetupLeftWheel();
			}
			else
			{
				this.SetupRightWheel();
			}
		}

		private void SetupLeftWheel()
		{
			Action onButtonClicked = delegate
			{
			};
			this.AddButton(SHSHudWheels.ButtonType.Inventory, SHSHudWheels.CloseType.StayOpen, typeof(SHSInventoryAnimatedWindow), true, GUIControl.DrawPhaseHintEnum.PostDraw);
			onButtonClicked = delegate
			{
			};
			this.AddButton(SHSHudWheels.ButtonType.Friends, SHSHudWheels.CloseType.StayOpen, typeof(SHSFriendsListWindow), true, GUIControl.DrawPhaseHintEnum.PostDraw);
			onButtonClicked = delegate
			{
			};
			this.AddButton(SHSHudWheels.ButtonType.Chat, SHSHudWheels.CloseType.CloseOnClick, typeof(SHSEmoteChatBar), false);
			onButtonClicked = delegate
			{
				GUIManager.Instance.ShowDialog(typeof(SHSOptionsGadget), null, GUIControl.ModalLevelEnum.Default);
			};
			this.AddButton(SHSHudWheels.ButtonType.Settings, SHSHudWheels.CloseType.CloseOnClick, onButtonClicked, false);
			onButtonClicked = delegate
			{
				AppShell.Instance.PromptQuit();
			};
			this.AddButton(SHSHudWheels.ButtonType.Exit, SHSHudWheels.CloseType.CloseOnClick, onButtonClicked, false);
			onButtonClicked = delegate
			{
			};
			this.AddButton(SHSHudWheels.ButtonType.MySquad, SHSHudWheels.CloseType.CloseOnClick, onButtonClicked, true, AssetBundleLoader.BundleGroup.Characters);
		}

		private void SetupRightWheel()
		{
			Action onButtonClicked = delegate
			{
				LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.Missions);
			};
			this.AddButton(SHSHudWheels.ButtonType.Brawler, SHSHudWheels.CloseType.CloseOnClick, onButtonClicked, true, AssetBundleLoader.BundleGroup.Any);
			onButtonClicked = delegate
			{
				GUIManager.Instance.ShowDialog(typeof(SHSZoneSelectorGadget), string.Empty, "SHSMainWindow", null, GUIControl.ModalLevelEnum.Default);
			};
			this.AddButton(SHSHudWheels.ButtonType.WorldMap, SHSHudWheels.CloseType.CloseOnClick, onButtonClicked, false);
			onButtonClicked = delegate
			{
				ShoppingWindow shoppingWindow = new ShoppingWindow();
				shoppingWindow.launch();
			};
			this.AddButton(SHSHudWheels.ButtonType.Shopping, SHSHudWheels.CloseType.CloseOnClick, onButtonClicked, true, AssetBundleLoader.BundleGroup.Characters);
			onButtonClicked = delegate
			{
				LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.CardGame);
			};
			this.AddButton(SHSHudWheels.ButtonType.CardGame, SHSHudWheels.CloseType.CloseOnClick, onButtonClicked, true, AssetBundleLoader.BundleGroup.CardGame);
			onButtonClicked = delegate
			{
				LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.Arcade);
			};
			this.AddButton(SHSHudWheels.ButtonType.Arcade, SHSHudWheels.CloseType.CloseOnClick, onButtonClicked, true);
			onButtonClicked = delegate
			{
				LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.Hq);
			};
			this.AddButton(SHSHudWheels.ButtonType.HQ, SHSHudWheels.CloseType.CloseOnClick, onButtonClicked, true, AssetBundleLoader.BundleGroup.HQ);
		}

		public virtual void RecievedButtonClick(SHSHudWheels.SHSHudWheelAnimated.HUDButton button)
		{
			if (this.openOrClosed != SHSHudWheels.OpenOrClosed.Open)
			{
				return;
			}
			if (this.queuedClose)
			{
				return;
			}
			if (button.launchDelegate != null)
			{
				button.launchDelegate();
				if (button.closeType == SHSHudWheels.CloseType.CloseOnClick)
				{
					this.Close();
				}
			}
			else if (this.selectedButton == null && this.WindowIsUp(button))
			{
				this.RemoveHudWindow(button);
				this.Close();
			}
			else if (this.selectedButton == button)
			{
				if (button.SelectUnselectPiece.Done)
				{
					this.RemoveHudWindow(button);
					this.Unselect();
					this.queuedClose = true;
				}
			}
			else
			{
				this.CloseAllHudWindows();
				if (button.closeType == SHSHudWheels.CloseType.CloseOnClick)
				{
					this.Close();
				}
				else
				{
					this.Select(button);
				}
				this.AddHudWindow(button);
			}
		}

		public void EnsureOpen(SHSHudWheels.SHSHudWheelAnimated.HUDButton button)
		{
			if (button.closeType == SHSHudWheels.CloseType.CloseOnClick)
			{
				this.AddHudWindow(button);
			}
			else if (this.selectedButton != button)
			{
				this.RecievedButtonClick(button);
			}
		}

		public void EnsureClosed(SHSHudWheels.SHSHudWheelAnimated.HUDButton button)
		{
			this.RemoveHudWindow(button);
		}

		private bool WindowIsUp(SHSHudWheels.SHSHudWheelAnimated.HUDButton button)
		{
			return this.HudWindows.ContainsKey(button) && this.HudWindows[button].IsVisible;
		}

		private void AddHudWindow(SHSHudWheels.SHSHudWheelAnimated.HUDButton button)
		{
			if (!this.HudWindows.ContainsKey(button))
			{
				SHSHudWindows sHSHudWindows = (SHSHudWindows)Activator.CreateInstance(button.HudWindowType);
				sHSHudWindows.OnToggleClose += delegate
				{
					this.Close();
					this.RemoveHudWindow(button);
				};
				sHSHudWindows.buttonType = button.buttonType;
				sHSHudWindows.RemoveOnHudClose = button.HudWindowRemoveOnHudClosed;
				GUIManager.Instance.ShowDynamicWindow(sHSHudWindows, "SHSMainWindow", GUIControl.DrawOrder.DrawFirst, button.HudWindowDrawPhaseHint, GUIControl.ModalLevelEnum.None);
				this.HudWindows.Add(button, sHSHudWindows);
			}
			else if (!this.HudWindows[button].IsVisible)
			{
				this.HudWindows.Remove(button);
				this.AddHudWindow(button);
			}
		}

		private void RemoveHudWindow(SHSHudWheels.SHSHudWheelAnimated.HUDButton button)
		{
			if (this.HudWindows.ContainsKey(button))
			{
				SHSHudWindows sHSHudWindows = this.HudWindows[button];
				sHSHudWindows.IsVisible = false;
				this.HudWindows.Remove(button);
			}
		}

		public bool HasHudWindowOpen()
		{
			foreach (SHSHudWindows current in this.HudWindows.Values)
			{
				if (current.RemoveOnHudClose)
				{
					return true;
				}
			}
			return false;
		}

		public void CloseAllHudWindows()
		{
			List<SHSHudWheels.SHSHudWheelAnimated.HUDButton> list = new List<SHSHudWheels.SHSHudWheelAnimated.HUDButton>();
			foreach (SHSHudWheels.SHSHudWheelAnimated.HUDButton current in this.HudWindows.Keys)
			{
				SHSHudWindows sHSHudWindows = this.HudWindows[current];
				if (sHSHudWindows.RemoveOnHudClose)
				{
					sHSHudWindows.Hide();
					list.Add(current);
				}
			}
			foreach (SHSHudWheels.SHSHudWheelAnimated.HUDButton current2 in list)
			{
				this.HudWindows.Remove(current2);
			}
		}

		public void ChangeButtonEnabledStatus(ChangeHudButtonsAndTraitsMessage message)
		{
			foreach (SHSHudWheels.ButtonType buttonType2 in message.toEnable)
			{
				SHSHudWheels.SHSHudWheelAnimated.HUDButton hUDButton = this.hudButtons.Find((SHSHudWheels.SHSHudWheelAnimated.HUDButton button) => button.buttonType == buttonType2);
				if (hUDButton != null)
				{
					hUDButton.IsEnabled = true;
				}
			}
			foreach (SHSHudWheels.ButtonType buttonType in message.toDisable)
			{
				SHSHudWheels.SHSHudWheelAnimated.HUDButton hUDButton2 = this.hudButtons.Find((SHSHudWheels.SHSHudWheelAnimated.HUDButton button) => button.buttonType == buttonType);
				if (hUDButton2 != null)
				{
					if (hUDButton2.buttonType == SHSHudWheels.ButtonType.WorldMap)
					{
						CspUtils.DebugLog("DISABLING world map button");
					}
					hUDButton2.IsEnabled = false;
				}
			}
		}

		public void EnsureButtonVisible(EnsureButtonVisibleMessage message)
		{
			SHSHudWheels.SHSHudWheelAnimated.HUDButton hUDButton = this.hudButtons.Find((SHSHudWheels.SHSHudWheelAnimated.HUDButton button) => button.buttonType == message.button);
			if (hUDButton != null)
			{
				if (hUDButton.closeType == SHSHudWheels.CloseType.StayOpen && (this.openOrClosed == SHSHudWheels.OpenOrClosed.Closed || this.openOrClosed == SHSHudWheels.OpenOrClosed.Closing))
				{
					this.Open();
					this.SavedMessageForCallback = message;
					if (this.OnOpened == null)
					{
						this.OnOpened = (Action)Delegate.Combine(this.OnOpened, new Action(this.FinishedOpeningCallback));
					}
					return;
				}
				this.EnsureOpen(hUDButton);
			}
		}

		public void ToggleButtonVisibility(ToggleButtonVisibilityMessage message)
		{
			SHSHudWheels.SHSHudWheelAnimated.HUDButton hUDButton = this.hudButtons.Find((SHSHudWheels.SHSHudWheelAnimated.HUDButton button) => button.buttonType == message.button);
			if (hUDButton != null)
			{
				if (this.openOrClosed == SHSHudWheels.OpenOrClosed.Closed || this.openOrClosed == SHSHudWheels.OpenOrClosed.Closing)
				{
					this.EnsureButtonVisible(new EnsureButtonVisibleMessage(message.button));
				}
				else if (this.openOrClosed == SHSHudWheels.OpenOrClosed.Open || this.openOrClosed == SHSHudWheels.OpenOrClosed.Opening)
				{
					if (this.selectedButton != hUDButton)
					{
						this.EnsureOpen(hUDButton);
					}
					else
					{
						this.RecievedButtonClick(this.selectedButton);
					}
				}
			}
		}

		private void FinishedOpeningCallback()
		{
			this.OnOpened = (Action)Delegate.Remove(this.OnOpened, new Action(this.FinishedOpeningCallback));
			this.EnsureButtonVisible(this.SavedMessageForCallback);
			this.SavedMessageForCallback = null;
		}

		public void CloseHud(CloseHudMessage message)
		{
			this.Close();
		}

		public override void OnShow()
		{
			if (this.loc == SHSHudWheels.WheelLocation.Left)
			{
				base.SetPosition(GUIControl.DockingAlignmentEnum.BottomLeft, GUIControl.AnchorAlignmentEnum.Middle, GUIControl.OffsetType.Absolute, new Vector2(48f, -50f));
			}
			else
			{
				base.SetPosition(GUIControl.DockingAlignmentEnum.BottomRight, GUIControl.AnchorAlignmentEnum.Middle, GUIControl.OffsetType.Absolute, new Vector2(-48f, -50f));
			}
			this.LoadSFX();
			this.HubWheelWindow.IsVisible = false;
			this.HubWheelWindow.SetSize(0f, 0f);
			this.idleButton.IsVisible = true;
			this.idleButton.Alpha = 1f;
			this.idleButton.AnimateSize(128f);
			this.ResizeIdleButton(new Vector2(GUIManager.ScreenRect.width, GUIManager.ScreenRect.height));
			this.queuedOpen = false;
			this.queuedClose = false;
			this.queuedClicked = false;
			this.queuedButton = null;
			this.openOrClosed = SHSHudWheels.OpenOrClosed.Closed;
			this.HudWindows.Clear();
			base.AnimationPieceManager.ClearAll();
			this.bkg.HitTestSize = new Vector2(1f, 1f);
			this.blockTestSize = new Vector2(0.35f, 0.35f);
			foreach (SHSHudWheels.SHSHudWheelAnimated.HUDButton current in this.hudButtons)
			{
				current.DisableButtonDueToAnimationInProgress = false;
				current.ToLastFrameHold(false);
			}
			base.OnShow();
		}

		public void ResizeIdleButton(Vector2 screenSize)
		{
			float a = screenSize.x * 0.0002840909f + 0.460227281f;
			float b = screenSize.y * 0.0004496403f + 0.460431665f;
			float num = Mathf.Max(a, b);
			num = Mathf.Clamp(num, 0.75f, 1f);
			this.idleButton.SizeMod = num;
		}

		private void RefreshCloseIfIdle()
		{
			if (!this.CloseIfIdleValid())
			{
				return;
			}
			base.AnimationPieceManager.SwapOut(ref this.CloseIfIdle, SHSHudWheels.SHSHudWheelAnimated.HUDAnimations.CloseIfIdle(this));
		}

		protected virtual bool CloseIfIdleValid()
		{
			if (this.HasHudWindowOpen())
			{
				return false;
			}
			foreach (SHSHudWheels.SHSHudWheelAnimated.HUDButton current in this.hudButtons)
			{
				if (current.IsHovering())
				{
					return false;
				}
			}
			return !this.bkg.Hover;
		}

		private void RemoveCloseIfIdle()
		{
			base.AnimationPieceManager.RemoveIfUnfinished(this.CloseIfIdle);
		}

		public virtual void OpenAndClose()
		{
			if (this.openOrClosed == SHSHudWheels.OpenOrClosed.Closed)
			{
				this.Open();
			}
			else if (this.openOrClosed == SHSHudWheels.OpenOrClosed.Open)
			{
				this.Close();
			}
		}

		public void Open()
		{
			if (this.openOrClosed == SHSHudWheels.OpenOrClosed.Open)
			{
				return;
			}
			if (this.openOrClosed == SHSHudWheels.OpenOrClosed.Closed)
			{
				this.selectedButton = null;
				this.RefreshCloseIfIdle();
				this.openOrClosed = SHSHudWheels.OpenOrClosed.Opening;
				this.ShowButtons(true);
				AnimClip animClip = SHSHudWheels.SHSHudWheelAnimated.HUDAnimations.AnimateOpen(this);
				animClip.OnFinished += delegate
				{
					this.bkg.HitTestSize = new Vector2(0.9f, 0.9f);
					this.blockTestSize = new Vector2(0.7f, 0.7f);
					this.openOrClosed = SHSHudWheels.OpenOrClosed.Open;
					if (this.OnOpened != null)
					{
						this.OnOpened();
					}
					this.TestQueued();
				};
				base.AnimationPieceManager.Add(animClip);
				ShsAudioSource.PlayAutoSound(this.openSFX);
			}
			else
			{
				this.queuedOpen = true;
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			this.CloseIfAppropriate();
		}

		public virtual void CloseIfAppropriate()
		{
			if (this.openOrClosed == SHSHudWheels.OpenOrClosed.Open && !this.HasHudWindowOpen() && (SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Left) || SHSInput.GetMouseButtonDown(SHSInput.MouseButtonType.Right)) && !SHSInput.IsOverUI())
			{
				this.Close();
			}
		}

		public void Close()
		{
			if (this.openOrClosed == SHSHudWheels.OpenOrClosed.Closed)
			{
				return;
			}
			if (this.openOrClosed == SHSHudWheels.OpenOrClosed.Open && this.selectedButton == null)
			{
				this.RemoveCloseIfIdle();
				this.CloseAllHudWindows();
				this.openOrClosed = SHSHudWheels.OpenOrClosed.Closing;
				AnimClip animClip = SHSHudWheels.SHSHudWheelAnimated.HUDAnimations.AnimateClose(this);
				animClip.OnFinished += delegate
				{
					this.RemoveCloseIfIdle();
					this.bkg.HitTestSize = new Vector2(1f, 1f);
					this.blockTestSize = new Vector2(0.35f, 0.35f);
					this.CloseAllHudWindows();
					this.ShowButtons(false);
					this.openOrClosed = SHSHudWheels.OpenOrClosed.Closed;
					this.TestQueued();
				};
				base.AnimationPieceManager.Add(animClip);
				ShsAudioSource.PlayAutoSound(this.closeSFX);
			}
			else if (this.openOrClosed == SHSHudWheels.OpenOrClosed.Open && this.selectedButton != null)
			{
				this.queuedClose = true;
				this.Unselect();
			}
			else
			{
				this.queuedClose = true;
			}
		}

		public void Select(SHSHudWheels.SHSHudWheelAnimated.HUDButton button)
		{
			if (this.openOrClosed == SHSHudWheels.OpenOrClosed.Open && this.selectedButton == null)
			{
				this.selectedButton = button;
				AnimClip animClip = SHSHudWheels.SHSHudWheelAnimated.HUDAnimations.ToSelected(button);
				animClip.OnFinished += delegate
				{
					this.TestQueued();
				};
				base.AnimationPieceManager.SwapOut(ref button.SelectUnselectPiece, animClip);
			}
			else if (this.openOrClosed == SHSHudWheels.OpenOrClosed.Open && this.selectedButton != null)
			{
				this.Unselect();
				this.Select(button);
			}
			else
			{
				this.queuedClicked = true;
				this.queuedButton = button;
			}
		}

		public void Unselect()
		{
			if (this.openOrClosed == SHSHudWheels.OpenOrClosed.Open && this.selectedButton != null)
			{
				AnimClip animClip = SHSHudWheels.SHSHudWheelAnimated.HUDAnimations.ToUnselected(this.selectedButton);
				animClip.OnFinished += delegate
				{
					this.TestQueued();
				};
				base.AnimationPieceManager.SwapOut(ref this.selectedButton.SelectUnselectPiece, animClip);
				this.selectedButton = null;
			}
		}

		public void TestQueued()
		{
			if (this.queuedClose)
			{
				this.queuedClose = false;
				this.Close();
			}
			else if (this.queuedOpen)
			{
				this.queuedOpen = false;
				this.Open();
			}
			else if (this.queuedClicked)
			{
				this.queuedClicked = false;
				SHSHudWheels.SHSHudWheelAnimated.HUDButton button = this.queuedButton;
				this.queuedButton = null;
				this.RecievedButtonClick(button);
			}
		}

		public void ShowButtons(bool show)
		{
			foreach (SHSHudWheels.SHSHudWheelAnimated.HUDButton current in this.hudButtons)
			{
				if (show)
				{
					current.IsVisible = true;
				}
				else
				{
					current.IsVisible = false;
				}
			}
		}

		public virtual void AddButton(SHSHudWheels.ButtonType buttonType, SHSHudWheels.CloseType closeType, Type TypeOfHudWindow, bool removeOnHubClosed)
		{
			SHSHudWheels.SHSHudWheelAnimated.HUDButton hUDButton = this.CreateAndAddHudButton(buttonType, closeType, false, AssetBundleLoader.BundleGroup.Any);
			hUDButton.HudWindowType = TypeOfHudWindow;
			hUDButton.HudWindowRemoveOnHudClosed = removeOnHubClosed;
		}

		public virtual void AddButton(SHSHudWheels.ButtonType buttonType, SHSHudWheels.CloseType closeType, Type TypeOfHudWindow, bool removeOnHubClosed, AssetBundleLoader.BundleGroup bundleGroup)
		{
			SHSHudWheels.SHSHudWheelAnimated.HUDButton hUDButton = this.CreateAndAddHudButton(buttonType, closeType, true, bundleGroup);
			hUDButton.HudWindowType = TypeOfHudWindow;
			hUDButton.HudWindowRemoveOnHudClosed = removeOnHubClosed;
		}

		public void AddButton(SHSHudWheels.ButtonType buttonType, SHSHudWheels.CloseType closeType, Type TypeOfHudWindow, bool removeOnHubClosed, GUIControl.DrawPhaseHintEnum drawPhaseHint)
		{
			SHSHudWheels.SHSHudWheelAnimated.HUDButton hUDButton = this.CreateAndAddHudButton(buttonType, closeType, true, AssetBundleLoader.BundleGroup.Any);
			hUDButton.HudWindowType = TypeOfHudWindow;
			hUDButton.HudWindowRemoveOnHudClosed = removeOnHubClosed;
			hUDButton.HudWindowDrawPhaseHint = drawPhaseHint;
		}

		public virtual void AddButton(SHSHudWheels.ButtonType buttonType, SHSHudWheels.CloseType closeType, Action onButtonClicked, bool removeOnHubClosed)
		{
			SHSHudWheels.SHSHudWheelAnimated.HUDButton hUDButton = this.CreateAndAddHudButton(buttonType, closeType, false, AssetBundleLoader.BundleGroup.Any);
			hUDButton.launchDelegate = onButtonClicked;
		}

		public virtual void AddButton(SHSHudWheels.ButtonType buttonType, SHSHudWheels.CloseType closeType, Action onButtonClicked, bool removeOnHubClosed, AssetBundleLoader.BundleGroup bundleGroup)
		{
			SHSHudWheels.SHSHudWheelAnimated.HUDButton hUDButton = this.CreateAndAddHudButton(buttonType, closeType, true, bundleGroup);
			hUDButton.launchDelegate = onButtonClicked;
		}

		public SHSHudWheels.SHSHudWheelAnimated.HUDButton CreateAndAddHudButton(SHSHudWheels.ButtonType buttonType, SHSHudWheels.CloseType closeType, AssetBundleLoader.BundleGroup bundleGroup)
		{
			return this.CreateAndAddHudButton(buttonType, closeType, true, bundleGroup);
		}

		public SHSHudWheels.SHSHudWheelAnimated.HUDButton CreateAndAddHudButton(SHSHudWheels.ButtonType buttonType, SHSHudWheels.CloseType closeType, bool contentDependent, AssetBundleLoader.BundleGroup bundleGroup)
		{
			SHSHudWheels.SHSHudWheelAnimated.HUDButton hUDButton = new SHSHudWheels.SHSHudWheelAnimated.HUDButton(buttonType, closeType, this);
			hUDButton.MouseOver += delegate(GUIControl sender, GUIMouseEvent EventData)
			{
				this.RemoveCloseIfIdle();
			};
			hUDButton.MouseOut += delegate(GUIControl sender, GUIMouseEvent EventData)
			{
				this.RefreshCloseIfIdle();
			};
			this.hudButtons.Add(hUDButton);
			string tooltipText = SHSHudWheels.GetTooltipText(buttonType);
			if (!string.IsNullOrEmpty(tooltipText))
			{
				hUDButton.button.ToolTip = new GUIControl.NamedToolTipInfo(tooltipText);
			}
			this.HubWheelWindow.AddItem(hUDButton);
			if (contentDependent)
			{
				hUDButton.ConfigureRequiredContent(new ContentReference(ContentTypeEnum.PriorityGroup, bundleGroup));
			}
			return hUDButton;
		}

		public void SetupMaxData()
		{
			float num = (float)((this.loc != SHSHudWheels.WheelLocation.Left) ? (-1) : 1);
			for (int i = 0; i < this.hudButtons.Count; i++)
			{
				float num2 = (float)i * 360f * num / (float)this.hudButtons.Count;
				Vector2 vector = new Vector2(Mathf.Sin(num2 * 0.0174532924f), Mathf.Cos((num2 + 180f) * 0.0174532924f)) * 75f;
				this.HubWheelWindow.MaxData[this.hudButtons[i]].MaxOffset = vector;
				this.hudButtons[i].ReturnOffset = vector;
			}
		}

		private void LoadSFX()
		{
			GUIBundleManager bundleManager = GUIManager.Instance.BundleManager;
			bundleManager.LoadAsset("hud_bundle", "HUD_UI_OptionsIn_audio", null, delegate(UnityEngine.Object obj, AssetBundle bundle, object extraData)
			{
				this.openSFX = (obj as GameObject);
			});
			bundleManager.LoadAsset("hud_bundle", "HUD_UI_OptionsOut_audio", null, delegate(UnityEngine.Object obj, AssetBundle bundle, object extraData)
			{
				this.closeSFX = (obj as GameObject);
			});
		}
	}

	private class SpecialEntitlements : Entitlements
	{
		public static Entitlements.EntitlementFlagEnum[] SpecialFlags = new Entitlements.EntitlementFlagEnum[]
		{
			Entitlements.EntitlementFlagEnum.InventoryCharacterSelect,
			Entitlements.EntitlementFlagEnum.InventoryDragDropMode
		};

		public static void DefaultAllSpecialFlagsBut(params Entitlements.EntitlementFlagEnum[] nonDefault)
		{
			Entitlements.EntitlementFlagEnum[] specialFlags = SHSHudWheels.SpecialEntitlements.SpecialFlags;
			for (int i = 0; i < specialFlags.Length; i++)
			{
				Entitlements.EntitlementFlagEnum flagToSet = specialFlags[i];
				SHSHudWheels.SpecialEntitlements.SetFlagsToDefaultOrNot(flagToSet, nonDefault);
			}
		}

		public static void SetFlagsToDefaultOrNot(Entitlements.EntitlementFlagEnum flagToSet, Entitlements.EntitlementFlagEnum[] nonDefault)
		{
			bool value = false;
			for (int i = 0; i < nonDefault.Length; i++)
			{
				Entitlements.EntitlementFlagEnum entitlementFlagEnum = nonDefault[i];
				if (entitlementFlagEnum == flagToSet)
				{
					value = true;
					break;
				}
			}
			Singleton<Entitlements>.instance.ConfigureEntitlement(flagToSet, value);
		}

		public static void ToggleSpecialFlag(Entitlements.EntitlementFlagEnum toToggle, bool toggleTo)
		{
			bool flag = false;
			Entitlements.EntitlementFlagEnum[] specialFlags = SHSHudWheels.SpecialEntitlements.SpecialFlags;
			for (int i = 0; i < specialFlags.Length; i++)
			{
				Entitlements.EntitlementFlagEnum entitlementFlagEnum = specialFlags[i];
				if (toToggle == entitlementFlagEnum)
				{
					Singleton<Entitlements>.instance.ConfigureEntitlement(toToggle, toggleTo);
					break;
				}
			}
			if (!flag)
			{
				CspUtils.DebugLog("you may not toggle a non-special flag");
			}
		}
	}

	public enum WheelLocation
	{
		Left,
		Right
	}

	public enum ButtonType
	{
		Blank,
		Brawler,
		CardGame,
		MySquad,
		Chat,
		Exit,
		Friends,
		HQ,
		Inventory,
		Shield,
		Settings,
		Shopping,
		SocialSpace,
		WorldMap,
		Arcade
	}

	public enum CloseType
	{
		CloseOnClick,
		StayOpen
	}

	private enum OpenOrClosed
	{
		Open,
		Opening,
		Closing,
		Closed
	}

	protected SHSHudWheels.SHSHudWheelAnimated leftWheel;

	protected SHSHudWheels.SHSHudWheelAnimated rightWheel;

	public SHSHudWheels()
	{
		this.leftWheel = new SHSHudWheels.SHSHudWheelAnimated(SHSHudWheels.WheelLocation.Left);
		this.leftWheel.Id = "LeftWheel";
		this.leftWheel.SetControlFlag(GUIControl.ControlFlagSetting.Persistent, true, true);
		this.Add(this.leftWheel);
		this.rightWheel = new SHSHudWheels.SHSHudWheelAnimated(SHSHudWheels.WheelLocation.Right);
		this.rightWheel.Id = "RightWheel";
		this.rightWheel.SetControlFlag(GUIControl.ControlFlagSetting.Persistent, true, true);
		this.Add(this.rightWheel);
		base.SetSize(new Vector2(1f, 1f), GUIControl.AutoSizeTypeEnum.Percentage, GUIControl.AutoSizeTypeEnum.Percentage);
		base.SetPosition(GUIControl.QuickSizingHint.Centered);
		this.RegisterForSceneTransitionEventComplete();
		AppShell.Instance.EventMgr.AddListener<ChangeHudButtonsAndTraitsMessage>(new ShsEventMgr.GenericDelegate<ChangeHudButtonsAndTraitsMessage>(this.ChangeButtonEnabledStatus));
		AppShell.Instance.EventMgr.AddListener<EnsureButtonVisibleMessage>(new ShsEventMgr.GenericDelegate<EnsureButtonVisibleMessage>(this.EnsureButtonVisible));
		AppShell.Instance.EventMgr.AddListener<CloseHudMessage>(new ShsEventMgr.GenericDelegate<CloseHudMessage>(this.CloseHud));
		AppShell.Instance.EventMgr.AddListener<ToggleButtonVisibilityMessage>(new ShsEventMgr.GenericDelegate<ToggleButtonVisibilityMessage>(this.ToggleButtonVisibility));
	}

	void IDisposable.Dispose()
	{
		this.UnRegisterHudWheels();
	}

	public static GUIAnimatedFrameButton GetAnimatedButton(SHSHudWheels.ButtonType type)
	{
		switch (type)
		{
		case SHSHudWheels.ButtonType.Blank:
			return new GUIAnimatedFrameButton("hud_bundle|blank", 1);
		case SHSHudWheels.ButtonType.Brawler:
			return new GUIAnimatedFrameButton("hud_bundle|brawler", 8);
		case SHSHudWheels.ButtonType.CardGame:
			return new GUIAnimatedFrameButton("hud_bundle|cardgame", 11);
		case SHSHudWheels.ButtonType.MySquad:
			return new GUIAnimatedFrameButton("hud_bundle|cerebro", 22);
		case SHSHudWheels.ButtonType.Chat:
			return new GUIAnimatedFrameButton("hud_bundle|chat", 8);
		case SHSHudWheels.ButtonType.Exit:
			return new GUIAnimatedFrameButton("hud_bundle|exit", 6);
		case SHSHudWheels.ButtonType.Friends:
			return new GUIAnimatedFrameButton("hud_bundle|friends", 11);
		case SHSHudWheels.ButtonType.HQ:
			return new GUIAnimatedFrameButton("hud_bundle|hq", 13);
		case SHSHudWheels.ButtonType.Inventory:
			return new GUIAnimatedFrameButton("hud_bundle|inventory", 7);
		case SHSHudWheels.ButtonType.Shield:
			return new GUIAnimatedFrameButton("hud_bundle|shieildhud", 15);
		case SHSHudWheels.ButtonType.Settings:
			return new GUIAnimatedFrameButton("hud_bundle|settings", 14);
		case SHSHudWheels.ButtonType.Shopping:
			return new GUIAnimatedFrameButton("hud_bundle|shopping", 8);
		case SHSHudWheels.ButtonType.SocialSpace:
			return new GUIAnimatedFrameButton("hud_bundle|socialspaces", 11);
		case SHSHudWheels.ButtonType.WorldMap:
			return new GUIAnimatedFrameButton("hud_bundle|worldmap", 11);
		case SHSHudWheels.ButtonType.Arcade:
			return new GUIAnimatedFrameButton("hud_bundle|hud_icon_minigames_", 12);
		default:
			return null;
		}
	}

	public static string GetTooltipText(SHSHudWheels.ButtonType type)
	{
		switch (type)
		{
		case SHSHudWheels.ButtonType.Brawler:
			return "#TT_HUDWHEEL_1";
		case SHSHudWheels.ButtonType.CardGame:
			return "#TT_HUDWHEEL_2";
		case SHSHudWheels.ButtonType.MySquad:
			return "#TT_HUDWHEEL_8";
		case SHSHudWheels.ButtonType.Chat:
			return "#TT_HUDWHEEL_3";
		case SHSHudWheels.ButtonType.Exit:
			return "#TT_HUDWHEEL_4";
		case SHSHudWheels.ButtonType.Friends:
			return "#TT_HUDWHEEL_5";
		case SHSHudWheels.ButtonType.HQ:
			return "#TT_HUDWHEEL_6";
		case SHSHudWheels.ButtonType.Inventory:
			return "#TT_HUDWHEEL_7";
		case SHSHudWheels.ButtonType.Shield:
			return "#TT_HUDWHEEL_15";
		case SHSHudWheels.ButtonType.Settings:
			return "#TT_HUDWHEEL_9";
		case SHSHudWheels.ButtonType.Shopping:
			return "#TT_HUDWHEEL_10";
		case SHSHudWheels.ButtonType.SocialSpace:
			return "#TT_HUDWHEEL_11";
		case SHSHudWheels.ButtonType.WorldMap:
			return "#TT_HUDWHEEL_12";
		case SHSHudWheels.ButtonType.Arcade:
			return "#TT_HUDWHEEL_17";
		default:
			return null;
		}
	}

	public static Entitlements.EntitlementFlagEnum? GetEntitlementFlag(SHSHudWheels.ButtonType type)
	{
		switch (type)
		{
		case SHSHudWheels.ButtonType.Blank:
			return null;
		case SHSHudWheels.ButtonType.Brawler:
			return new Entitlements.EntitlementFlagEnum?(Entitlements.EntitlementFlagEnum.MissionsPermitSet);
		case SHSHudWheels.ButtonType.CardGame:
			return null;
		case SHSHudWheels.ButtonType.MySquad:
			return new Entitlements.EntitlementFlagEnum?(Entitlements.EntitlementFlagEnum.MySquadPermit);
		case SHSHudWheels.ButtonType.Chat:
			return null;
		case SHSHudWheels.ButtonType.Exit:
			return new Entitlements.EntitlementFlagEnum?(Entitlements.EntitlementFlagEnum.DemoLimitsOn);
		case SHSHudWheels.ButtonType.Friends:
			return new Entitlements.EntitlementFlagEnum?(Entitlements.EntitlementFlagEnum.FriendsListPermit);
		case SHSHudWheels.ButtonType.HQ:
			return new Entitlements.EntitlementFlagEnum?(Entitlements.EntitlementFlagEnum.ParentalHqDeny);
		case SHSHudWheels.ButtonType.Inventory:
			return null;
		case SHSHudWheels.ButtonType.Shield:
			return null;
		case SHSHudWheels.ButtonType.Settings:
			return new Entitlements.EntitlementFlagEnum?(Entitlements.EntitlementFlagEnum.DemoLimitsOn);
		case SHSHudWheels.ButtonType.Shopping:
			return new Entitlements.EntitlementFlagEnum?(Entitlements.EntitlementFlagEnum.ShopWindowPermit);
		case SHSHudWheels.ButtonType.SocialSpace:
			return new Entitlements.EntitlementFlagEnum?(Entitlements.EntitlementFlagEnum.GameWorldPermitSet);
		case SHSHudWheels.ButtonType.Arcade:
			return null;
		}
		return null;
	}

	private void RegisterForSceneTransitionEventComplete()
	{
		AppShell.Instance.OnNewControllerReady += new AppShell.NewControllerReadyDelegate(this.OnNewControllerReady);
	}

	private void UnRegisterForSceneTransitionEventComplete()
	{
		AppShell.Instance.OnNewControllerReady -= new AppShell.NewControllerReadyDelegate(this.OnNewControllerReady);
	}

	private void OnNewControllerReady(AppShell.GameControllerTypeData newGameTypeData, GameController controller)
	{
		switch (controller.controllerType)
		{
		case GameController.ControllerType.Brawler:
			SHSHudWheels.MissionsSetupEnabled();
			return;
		case GameController.ControllerType.CardHub:
			SHSHudWheels.CardGameSetupEnabled();
			return;
		case GameController.ControllerType.CardGame:
			SHSHudWheels.CardGameSetupEnabled();
			return;
		case GameController.ControllerType.HeadQuarters:
			SHSHudWheels.HQSetupEnabled();
			return;
		case GameController.ControllerType.SocialSpace:
			SHSHudWheels.GameWorldSetupEnabled();
			return;
		case GameController.ControllerType.DeckBuilder:
			SHSHudWheels.CardGameSetupEnabled();
			return;
		case GameController.ControllerType.Fallback:
			SHSHudWheels.DefaultSetupEnabled();
			return;
		}
		SHSHudWheels.DefaultSetupEnabled();
	}

	private static void CardGameSetupEnabled()
	{
		SHSHudWheels.ButtonType[] expr_06 = new SHSHudWheels.ButtonType[2];
		expr_06[0] = SHSHudWheels.ButtonType.Chat;
		SHSHudWheels.EnableAllHudButtonsBut(expr_06);
		SHSHudWheels.DefaultAllSpecialFlagsBut(new Entitlements.EntitlementFlagEnum[0]);
		if (AppShell.Instance.SharedHashTable["GUIHudPlaySolo"] == null || (bool)AppShell.Instance.SharedHashTable["GUIHudPlaySolo"])
		{
		}
	}

	private static void GameWorldSetupEnabled()
	{
		SHSHudWheels.EnableAllHudButtonsBut(new SHSHudWheels.ButtonType[1]);
		SHSHudWheels.DefaultAllSpecialFlagsBut(new Entitlements.EntitlementFlagEnum[]
		{
			Entitlements.EntitlementFlagEnum.InventoryCharacterSelect
		});
	}

	private static void MissionsSetupEnabled()
	{
		SHSHudWheels.ButtonType[] expr_06 = new SHSHudWheels.ButtonType[3];
		expr_06[0] = SHSHudWheels.ButtonType.Chat;
		expr_06[1] = SHSHudWheels.ButtonType.Inventory;
		SHSHudWheels.EnableAllHudButtonsBut(expr_06);
		SHSHudWheels.DefaultAllSpecialFlagsBut(new Entitlements.EntitlementFlagEnum[0]);
		if (AppShell.Instance.SharedHashTable["GUIHudPlaySolo"] == null || (bool)AppShell.Instance.SharedHashTable["GUIHudPlaySolo"])
		{
		}
	}

	private static void HQSetupEnabled()
	{
		SHSHudWheels.EnableAllHudButtonsBut(new SHSHudWheels.ButtonType[]
		{
			SHSHudWheels.ButtonType.Chat,
			SHSHudWheels.ButtonType.Blank,
			SHSHudWheels.ButtonType.HQ
		});
		SHSHudWheels.DefaultAllSpecialFlagsBut(new Entitlements.EntitlementFlagEnum[]
		{
			Entitlements.EntitlementFlagEnum.InventoryDragDropMode
		});
	}

	private static void DefaultSetupEnabled()
	{
		SHSHudWheels.EnableAllHudButtonsBut(new SHSHudWheels.ButtonType[1]);
		SHSHudWheels.DefaultAllSpecialFlagsBut(new Entitlements.EntitlementFlagEnum[0]);
	}

	public static void EnableAllHudButtonsBut(params SHSHudWheels.ButtonType[] toDisable)
	{
		ChangeHudButtonsAndTraitsMessage changeHudButtonsAndTraitsMessage = new ChangeHudButtonsAndTraitsMessage();
		changeHudButtonsAndTraitsMessage.EnableAllBut(toDisable);
		AppShell.Instance.EventMgr.Fire(null, changeHudButtonsAndTraitsMessage);
	}

	public static void EnableHudButtons(params SHSHudWheels.ButtonType[] toEnable)
	{
		ChangeHudButtonsAndTraitsMessage changeHudButtonsAndTraitsMessage = new ChangeHudButtonsAndTraitsMessage();
		changeHudButtonsAndTraitsMessage.toEnable.AddRange(toEnable);
		AppShell.Instance.EventMgr.Fire(null, changeHudButtonsAndTraitsMessage);
	}

	public static void DisableHudButtons(params SHSHudWheels.ButtonType[] toDisable)
	{
		ChangeHudButtonsAndTraitsMessage changeHudButtonsAndTraitsMessage = new ChangeHudButtonsAndTraitsMessage();
		changeHudButtonsAndTraitsMessage.toDisable.AddRange(toDisable);
		AppShell.Instance.EventMgr.Fire(null, changeHudButtonsAndTraitsMessage);
	}

	public static void DefaultAllSpecialFlagsBut(params Entitlements.EntitlementFlagEnum[] toEnable)
	{
		SHSHudWheels.SpecialEntitlements.DefaultAllSpecialFlagsBut(toEnable);
	}

	public static void ToggleSpecialFlag(Entitlements.EntitlementFlagEnum toToggle, bool toggleTo)
	{
		SHSHudWheels.SpecialEntitlements.ToggleSpecialFlag(toToggle, toggleTo);
	}

	~SHSHudWheels()
	{
		this.UnRegisterHudWheels();
	}

	public void UnRegisterHudWheels()
	{
		this.UnRegisterForSceneTransitionEventComplete();
		AppShell.Instance.EventMgr.RemoveListener<ChangeHudButtonsAndTraitsMessage>(new ShsEventMgr.GenericDelegate<ChangeHudButtonsAndTraitsMessage>(this.ChangeButtonEnabledStatus));
		AppShell.Instance.EventMgr.RemoveListener<EnsureButtonVisibleMessage>(new ShsEventMgr.GenericDelegate<EnsureButtonVisibleMessage>(this.EnsureButtonVisible));
		AppShell.Instance.EventMgr.RemoveListener<CloseHudMessage>(new ShsEventMgr.GenericDelegate<CloseHudMessage>(this.CloseHud));
		AppShell.Instance.EventMgr.RemoveListener<ToggleButtonVisibilityMessage>(new ShsEventMgr.GenericDelegate<ToggleButtonVisibilityMessage>(this.ToggleButtonVisibility));
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
		this.leftWheel.ResizeIdleButton(message.NewSize);
		this.rightWheel.ResizeIdleButton(message.NewSize);
	}

	public void ChangeButtonEnabledStatus(ChangeHudButtonsAndTraitsMessage message)
	{
		this.leftWheel.ChangeButtonEnabledStatus(message);
		this.rightWheel.ChangeButtonEnabledStatus(message);
	}

	public void EnsureButtonVisible(EnsureButtonVisibleMessage message)
	{
		this.leftWheel.EnsureButtonVisible(message);
		this.rightWheel.EnsureButtonVisible(message);
	}

	public void CloseHud(CloseHudMessage message)
	{
		this.leftWheel.CloseHud(message);
		this.rightWheel.CloseHud(message);
	}

	public void ToggleButtonVisibility(ToggleButtonVisibilityMessage message)
	{
		this.leftWheel.ToggleButtonVisibility(message);
		this.rightWheel.ToggleButtonVisibility(message);
	}
}
