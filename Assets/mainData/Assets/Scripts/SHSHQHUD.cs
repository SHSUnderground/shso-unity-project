using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSHQHUD : GUISimpleControlWindow
{
	public enum Direction
	{
		Right,
		Left
	}

	private enum HQState
	{
		Play,
		Pause
	}

	public class BlockTestSpot : GUISimpleControlWindow
	{
		public BlockTestSpot()
		{
			HitTestType = HitTestTypeEnum.Rect;
			BlockTestType = BlockTestTypeEnum.Rect;
			IsVisible = false;
		}
	}

	public class SHSHQHeroSelect : GUISimpleControlWindow
	{
		public class HeroHead : GUIButton
		{
			public HeroPersisted hp;

			private SHSHQHeroSelect heroSelectWindow;

			public HeroHead(HeroPersisted hp, SHSHQHeroSelect heroSelectWindow)
			{
				this.hp = hp;
				this.heroSelectWindow = heroSelectWindow;
				SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
				Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
				StyleInfo = new SHSButtonStyleInfo("characters_bundle|inventory_character_" + hp.Name, SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
				HitTestType = HitTestTypeEnum.Circular;
				base.HitTestSize = new Vector2(0.52f, 0.52f);
				base.ToolTip = new NamedToolTipInfo(string.Format(AppShell.Instance.stringTable["#TT_HQ_FIND_HERO"], AppShell.Instance.CharacterDescriptionManager[hp.Name].CharacterName), new Vector2(0f, -10f));
				Click += delegate
				{
					if (HqController2.Instance != null)
					{
						HqController2.Instance.GoTo(hp.Name);
					}
				};
			}

			public void UpdatePosition(float x)
			{
				float num = Mathf.Abs(x);
				Offset = new Vector2((35f + (4f - Mathf.Pow(x, 2f))) * x, num * 6f + 20f);
				SetSize(80.47f - num * 14.08f, 80.47f - num * 14.08f);
				Rotation = x * 5f;
				Alpha = Mathf.Clamp01(1f - 0.00137174211f * Mathf.Pow(num, 6f)) * (-0.05f * num + 1f) * heroSelectWindow.alphaPercentageMouseOut + Mathf.Clamp01(1f - 0.00137174211f * Mathf.Pow(num, 6f)) * (-0.05f * num + 0.4f) * (1f - heroSelectWindow.alphaPercentageMouseOut);
			}
		}

		private SHSHQHUD ParentWindow;

		private float alphaPercentageMouseOut;

		private GUIImage scanline;

		private AnimClip AlphaPercentageMouseOutAnimation;

		private CircularList<HeroHead> heroHeads = new CircularList<HeroHead>();

		private AnimClip currentMovementAnimation;

		public int CurrentTargetPosition;

		public float CurrentPosition;

		private AnimClip scanlineMovement;

		public SHSHQHeroSelect(SHSHQHUD ParentWindow)
		{
			this.ParentWindow = ParentWindow;
			SetSize(new Vector2(662f, 129f));
			SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle);
			GUIImage gUIImage = GUIControl.CreateControlBottomFrameCentered<GUIImage>(new Vector2(662f, 129f), new Vector2(0f, -64f));
			gUIImage.TextureSource = "hq_bundle|mshs_hq_hud_character_window";
			Add(gUIImage);
			scanline = GUIControl.CreateControlBottomFrameCentered<GUIImage>(new Vector2(232f, 16f), Vector2.zero);
			scanline.TextureSource = "hq_bundle|mshs_hq_hud_character_window_scanline";
			Add(scanline);
		}

		public void HeroBoxMouseOver()
		{
			base.AnimationPieceManager.SwapOut(ref AlphaPercentageMouseOutAnimation, AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(alphaPercentageMouseOut, 1f, 0.3f), AdjustAlphaPercentageMouseOut));
		}

		public void HeroBoxMouseOut()
		{
			base.AnimationPieceManager.SwapOut(ref AlphaPercentageMouseOutAnimation, AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(alphaPercentageMouseOut, 0f, 0.3f), AdjustAlphaPercentageMouseOut));
		}

		public void AdjustAlphaPercentageMouseOut(float x)
		{
			alphaPercentageMouseOut = x;
			UpdatePosition(CurrentPosition);
		}

		public void FullRefreshHeroes()
		{
			foreach (HeroHead heroHead2 in heroHeads)
			{
				Remove(heroHead2);
			}
			heroHeads.Clear();
			using (Dictionary<string, HeroPersisted>.ValueCollection.Enumerator enumerator2 = AppShell.Instance.Profile.AvailableCostumes.Values.GetEnumerator())
			{
				HeroPersisted hp;
				while (enumerator2.MoveNext())
				{
					hp = enumerator2.Current;
					HeroHead heroHead = heroHeads.Find(delegate(HeroHead test)
					{
						return test.hp == hp;
					});
					if (hp.Placed)
					{
						if (heroHead == null)
						{
							heroHead = new HeroHead(hp, this);
							heroHeads.Add(heroHead);
							Add(heroHead);
						}
					}
					else if (heroHead != null)
					{
						heroHeads.Remove(heroHead);
						Remove(heroHead);
					}
				}
			}
			UpdatePosition(CurrentPosition);
		}

		public void GoLeft()
		{
			CurrentTargetPosition--;
			base.AnimationPieceManager.SwapOut(ref currentMovementAnimation, AnimClipBuilder.Custom.Function(SHSAnimations.GenericPaths.LinearWithBounce(CurrentPosition, CurrentTargetPosition, 0.3f), UpdatePosition));
		}

		public void GoRight()
		{
			CurrentTargetPosition++;
			base.AnimationPieceManager.SwapOut(ref currentMovementAnimation, AnimClipBuilder.Custom.Function(SHSAnimations.GenericPaths.LinearWithBounce(CurrentPosition, CurrentTargetPosition, 0.3f), UpdatePosition));
		}

		public void UpdatePosition(float x)
		{
			if (heroHeads.Count <= 5)
			{
				switch (heroHeads.Count)
				{
				case 0:
					return;
				case 1:
					x = 0f;
					break;
				case 2:
					x = 0.5f;
					break;
				case 3:
					x = 0f;
					break;
				case 4:
					x = 0.5f;
					break;
				case 5:
					x = 0f;
					break;
				}
				CurrentTargetPosition = 0;
				ParentWindow.CharScrollLeft.IsVisible = false;
				ParentWindow.CharScrollRight.IsVisible = false;
			}
			else
			{
				ParentWindow.CharScrollLeft.IsVisible = true;
				ParentWindow.CharScrollRight.IsVisible = true;
			}
			CurrentPosition = x;
			heroHeads.BasePosition = x;
			List<double> map = heroHeads.GetMap();
			for (int i = 0; i < heroHeads.Count; i++)
			{
				heroHeads[i].UpdatePosition((float)map[i]);
			}
		}

		public override void OnShow()
		{
			base.OnShow();
			alphaPercentageMouseOut = 0f;
			CurrentPosition = 0f;
			CurrentTargetPosition = 0;
			FullRefreshHeroes();
			BeginScanline();
			AppShell.Instance.EventMgr.AddListener<HeroFetchCompleteMessage>(OnHeroFetchComplete);
			AppShell.Instance.EventMgr.AddListener<HeroCollectionUpdateMessage>(OnHQHeroPlaced);
		}

		public override void OnHide()
		{
			base.OnHide();
			AppShell.Instance.EventMgr.RemoveListener<HeroFetchCompleteMessage>(OnHeroFetchComplete);
			AppShell.Instance.EventMgr.RemoveListener<HeroCollectionUpdateMessage>(OnHQHeroPlaced);
		}

		public void BeginScanline()
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			AnimClip animClip = AnimClipBuilder.Absolute.OffsetY(AnimClipBuilder.Path.Linear(-100f, 0f, 2.6f), scanline);
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				BeginScanline();
			};
			base.AnimationPieceManager.SwapOut(ref scanlineMovement, animClip);
		}

		public void OnHQHeroPlaced(HeroCollectionUpdateMessage msg)
		{
			FullRefreshHeroes();
		}

		public void OnHeroFetchComplete(HeroFetchCompleteMessage msg)
		{
			if (msg.success)
			{
				FullRefreshHeroes();
			}
		}
	}

	public class OpeningAnimation : SHSAnimations
	{
		public static AnimClip OpenAnim(SHSHQHUD hud)
		{
			AnimClip animClip = Generic.Blank();
			foreach (GUIControl key in hud.OffsetLookup.Keys)
			{
				key.Offset = Vector2.zero;
				Vector2 vector = hud.OffsetLookup[key];
				animClip = (animClip ^ Absolute.OffsetX(GenericPaths.AddBounce(Path.Linear(0f, vector.x, 0.8f), 0f, vector.x), key) ^ Absolute.OffsetY(GenericPaths.AddBounce(Path.Sin(0f, 0.25f, 0.8f) * vector.y, 0f, vector.y), key));
			}
			return Absolute.Nothing(Path.Constant(0f, 0.25f), AnimClipBuilder.IgnoreWarningControl) | animClip;
		}
	}

	public enum PlayPauseEnum
	{
		Play,
		Pause
	}

	public class PlayPauseCombinedButton : GUISimpleControlWindow
	{
		public GUIHotSpotButton hotSpot;

		private GUIHotSpotButton playTooltip;

		private GUIHotSpotButton pauseTooltip;

		private GUIAnimatedButton bkg;

		private GUIAnimatedButton pulse;

		private GUIAnimatedButton symbol;

		private GUIAnimatedButton shine;

		public PlayPauseEnum CurrentState;

		private AnimClip pulseAnim;

		public PlayPauseCombinedButton()
		{
			SetSize(new Vector2(256f, 256f));
			SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
			Offset = new Vector2(0f, -124f);
			bkg = CreateAndAddButton("layer1");
			pulse = CreateAndAddButton("layer2");
			symbol = CreateAndAddButton("play");
			shine = CreateAndAddButton("layer4");
			CurrentState = PlayPauseEnum.Play;
			hotSpot = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(new Vector2(256f, 256f), Vector2.zero);
			hotSpot.HitTestSize = new Vector2(0.57f, 0.35f);
			Add(hotSpot);
			playTooltip = CreateAndAddTooltipHotspot("#TT_HQMINIMAP_6");
			pauseTooltip = CreateAndAddTooltipHotspot("#TT_HQMINIMAP_7");
			bkg.LinkToSourceButton(hotSpot);
			pulse.LinkToSourceButton(hotSpot);
			symbol.LinkToSourceButton(hotSpot);
			shine.LinkToSourceButton(hotSpot);
			hotSpot.MouseOver += delegate
			{
				StartPulse();
			};
			hotSpot.MouseOut += delegate
			{
				EndPulse();
			};
		}

		public override bool InitializeResources(bool reload)
		{
			if (reload)
			{
				return base.InitializeResources(reload);
			}
			Texture2D texture2D = GUIManager.Instance.LoadTexture("hq_bundle|mshs_hq_hud_button_playpause_layer1");
			if (texture2D != null)
			{
				hotSpot.HitTestType = HitTestTypeEnum.Alpha;
				hotSpot.HitTestSize = new Vector2(1f, 1f);
				hotSpot.SetMask(texture2D);
				playTooltip.HitTestType = HitTestTypeEnum.Alpha;
				playTooltip.HitTestSize = new Vector2(1f, 1f);
				playTooltip.Mask = hotSpot.Mask;
				pauseTooltip.HitTestType = HitTestTypeEnum.Alpha;
				pauseTooltip.HitTestSize = new Vector2(1f, 1f);
				pauseTooltip.Mask = hotSpot.Mask;
			}
			return base.InitializeResources(reload);
		}

		public void ShowPlay()
		{
			CurrentState = PlayPauseEnum.Play;
			playTooltip.IsVisible = true;
			pauseTooltip.IsVisible = false;
			symbol.TextureSource = "hq_bundle|mshs_hq_hud_button_playpause_play";
		}

		public void ShowPause()
		{
			CurrentState = PlayPauseEnum.Pause;
			playTooltip.IsVisible = false;
			pauseTooltip.IsVisible = true;
			symbol.TextureSource = "hq_bundle|mshs_hq_hud_button_playpause_pause";
		}

		public void StartPulse()
		{
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Expected O, but got Unknown
			float time = 0.75f * (1f - pulse.Alpha);
			float time2 = 0.75f;
			AnimClip animClip = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(pulse.Alpha, 1f, time) | AnimClipBuilder.Path.Linear(1f, 0f, time2), pulse);
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				StartPulse();
			};
			base.AnimationPieceManager.SwapOut(ref pulseAnim, animClip);
		}

		public void EndPulse()
		{
			float time = pulse.Alpha * 0.25f;
			AnimClip newPiece = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(pulse.Alpha, 0f, time), pulse);
			base.AnimationPieceManager.SwapOut(ref pulseAnim, newPiece);
		}

		private GUIAnimatedButton CreateAndAddButton(string path)
		{
			GUIAnimatedButton gUIAnimatedButton = GUIControl.CreateControlFrameCentered<GUIAnimatedButton>(new Vector2(256f, 256f), Vector2.zero);
			gUIAnimatedButton.SetupButton(0.95f, 1f, 0.9f);
			gUIAnimatedButton.TextureSource = "hq_bundle|mshs_hq_hud_button_playpause_" + path;
			gUIAnimatedButton.HitTestType = HitTestTypeEnum.Transparent;
			Add(gUIAnimatedButton);
			return gUIAnimatedButton;
		}

		private GUIHotSpotButton CreateAndAddTooltipHotspot(string tooltip)
		{
			GUIHotSpotButton gUIHotSpotButton = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(new Vector2(256f, 256f), Vector2.zero);
			gUIHotSpotButton.HitTestSize = new Vector2(0.57f, 0.35f);
			gUIHotSpotButton.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
			gUIHotSpotButton.ToolTip = new NamedToolTipInfo(tooltip, new Vector2(10f, 20f));
			gUIHotSpotButton.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			Add(gUIHotSpotButton);
			return gUIHotSpotButton;
		}
	}

	public class PlayPauseButton : GUISimpleControlWindow
	{
		private class PlayPauseAnim : SHSAnimations
		{
			public static AnimClip Spin(GUIControl ctrl, float spinTime)
			{
				float time = GenericFunctions.FrationalTime(0f, 360f, ctrl.Rotation, spinTime);
				return Absolute.Rotation(Path.Linear(ctrl.Rotation, 360f, time), ctrl);
			}

			public static AnimClip FadeIn(GUIControl ctrl)
			{
				ctrl.IsVisible = true;
				return Generic.FadeIn(ctrl, 0.3f);
			}

			public static AnimClip FadeOut(GUIControl ctrl)
			{
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Expected O, but got Unknown
				AnimClip animClip = Generic.FadeOut(ctrl, 0.3f);
				animClip.OnFinished += (Action)(object)(Action)delegate
				{
					ctrl.IsVisible = false;
				};
				return animClip;
			}
		}

		public GUIHotSpotButton hotSpot;

		private GUIAnimatedButton shine;

		private GUIAnimatedButton symbol;

		private GUIAnimatedButton spin;

		private GUIAnimatedButton bkg;

		public AnimClip FadeInAndOut;

		private float spinTime = 1f;

		private AnimClip spinAnim;

		public PlayPauseButton(string path)
		{
			SetSize(new Vector2(128f, 128f));
			SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
			bkg = CreateAndAddButton(path + "_layer4");
			spin = CreateAndAddButton(path + "_layer3");
			symbol = CreateAndAddButton(path + "_layer2");
			shine = CreateAndAddButton(path + "_layer1");
			spin.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			hotSpot = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(new Vector2(128f, 128f), Vector2.zero);
			hotSpot.HitTestSize = new Vector2(0.53f, 0.53f);
			hotSpot.HitTestType = HitTestTypeEnum.Circular;
			hotSpot.MouseOver += hotSpot_MouseOver;
			hotSpot.MouseOut += hotSpot_MouseOut;
			Add(hotSpot);
			bkg.LinkToSourceButton(hotSpot);
			spin.LinkToSourceButton(hotSpot);
			symbol.LinkToSourceButton(hotSpot);
			shine.LinkToSourceButton(hotSpot);
		}

		private void hotSpot_MouseOver(GUIControl sender, GUIMouseEvent EventData)
		{
			spinTime = 0.5f;
			ApplySpin();
		}

		private void hotSpot_MouseOut(GUIControl sender, GUIMouseEvent EventData)
		{
			spinTime = 1f;
			ApplySpin();
		}

		public void Activate()
		{
			base.AnimationPieceManager.SwapOut(ref FadeInAndOut, PlayPauseAnim.FadeIn(spin));
			ChangeHighlightOnButtons(1f, 1.1f, false);
		}

		public void Deactivate()
		{
			base.AnimationPieceManager.SwapOut(ref FadeInAndOut, PlayPauseAnim.FadeOut(spin));
			ChangeHighlightOnButtons(0.8f, 0.9f, false);
		}

		public override void OnShow()
		{
			base.OnShow();
			spinTime = 1f;
			ApplySpin();
		}

		private void ApplySpin()
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected O, but got Unknown
			if (spin.Rotation == 360f)
			{
				spin.Rotation = 0f;
			}
			AnimClip animClip = PlayPauseAnim.Spin(spin, spinTime);
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				ApplySpin();
			};
			base.AnimationPieceManager.SwapOut(ref spinAnim, animClip);
		}

		private void ChangeHighlightOnButtons(float normal, float highlight, bool instantRefresh)
		{
			GUIAnimatedButton[] array = new GUIAnimatedButton[4]
			{
				bkg,
				spin,
				symbol,
				shine
			};
			GUIAnimatedButton[] array2 = array;
			foreach (GUIAnimatedButton gUIAnimatedButton in array2)
			{
				if (instantRefresh)
				{
					Vector2 pressedPercentage = gUIAnimatedButton.PressedPercentage;
					gUIAnimatedButton.SetupButton(normal, highlight, pressedPercentage.x);
				}
				else
				{
					gUIAnimatedButton.NormalPercentage = new Vector2(normal, normal);
					gUIAnimatedButton.HighlightPercentage = new Vector2(highlight, highlight);
					gUIAnimatedButton.GoToCurrentState();
				}
			}
		}

		private GUIAnimatedButton CreateAndAddButton(string path)
		{
			GUIAnimatedButton gUIAnimatedButton = GUIControl.CreateControlFrameCentered<GUIAnimatedButton>(new Vector2(128f, 128f), Vector2.zero);
			gUIAnimatedButton.SetupButton(0.8f, 0.9f, 0.7f);
			gUIAnimatedButton.TextureSource = "hq_bundle|mshs_button_" + path;
			gUIAnimatedButton.HitTestType = HitTestTypeEnum.Transparent;
			Add(gUIAnimatedButton);
			return gUIAnimatedButton;
		}
	}

	public class SHSHQRoomTitle : GUISimpleControlWindow
	{
		public class AnimTitle : SHSAnimations
		{
			public static AnimClip Move(SHSHQRoomTitle main, Direction dir, bool fadeOut)
			{
				//IL_0165: Unknown result type (might be due to invalid IL or missing references)
				//IL_016f: Expected O, but got Unknown
				int num = (dir != Direction.Left) ? 1 : (-1);
				main.TitleVis(true);
				AnimClip animClip = Generic.Blank();
				for (int i = 0; i < main.titles.Count; i++)
				{
					GUIImage gUIImage = main.titles[i];
					float delay = (float)i * 0.03f;
					float num2 = 1f - 0.2f * (float)i;
					float time = (i == 0) ? 5 : 0;
					float time2 = (i == 0) ? 2 : 0;
					gUIImage.Alpha = num2;
					animClip = (animClip ^ Absolute.OffsetX(AddDelay(GenericPaths.LinearWithBounce(main.Rect.width / 2f * (float)num + (float)(290 * num), 0f, 0.5f), delay), gUIImage) ^ Absolute.Alpha(AddDelay(Path.Linear(num2, num2, 1f) | Path.Constant(num2, time) | Path.Linear(num2, (!fadeOut) ? 1 : 0, time2), delay), gUIImage));
				}
				animClip.OnFinished += (Action)(object)(Action)delegate
				{
					if (fadeOut)
					{
						main.TitleVis(false);
					}
				};
				return animClip;
			}

			public static AnimPath AddDelay(AnimPath path, float delay)
			{
				float value = path.GetValue(0f);
				return Path.Constant(value, delay) | path;
			}
		}

		private List<GUIImage> titles;

		private AnimClip titleMoveAnim;

		public SHSHQRoomTitle()
		{
			SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
			SetPosition(QuickSizingHint.Centered);
			titles = new List<GUIImage>(5);
			for (int i = 0; i < 5; i++)
			{
				GUIImage gUIImage = GUIControl.CreateControl<GUIImage>(new Vector2(580f, 90f), Vector2.zero, DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle);
				titles.Add(gUIImage);
				Add(gUIImage);
				ControlToBack(gUIImage);
			}
		}

		public void ShowRoom(string roomName, Direction dir, bool fadeOut)
		{
			string textureSource = RoomNameToTextureSourceLookup(roomName);
			foreach (GUIImage title in titles)
			{
				title.TextureSource = textureSource;
			}
			base.AnimationPieceManager.SwapOut(ref titleMoveAnim, AnimTitle.Move(this, dir, fadeOut));
		}

		public void TitleVis(bool vis)
		{
			foreach (GUIImage title in titles)
			{
				title.IsVisible = vis;
			}
		}

		private static string RoomNameToTextureSourceLookup(string roomName)
		{
			switch (roomName)
			{
			case "bridge":
				return "hq_bundle|L_mshs_hq_room_title_bridge";
			case "cafeteria":
				return "hq_bundle|L_mshs_hq_room_title_cafeteria";
			case "cells":
				return "hq_bundle|L_mshs_hq_room_title_detentioncell";
			case "dorm1":
				return "hq_bundle|L_mshs_hq_room_title_dormroom1";
			case "dorm2":
				return "hq_bundle|L_mshs_hq_room_title_dormroom2";
			case "dorm3":
				return "hq_bundle|L_mshs_hq_room_title_dormroom3";
			case "dorm4":
				return "hq_bundle|L_mshs_hq_room_title_dormroom4";
			case "dorm5":
				return "hq_bundle|L_mshs_hq_room_title_dormroom5";
			case "dorm6":
				return "hq_bundle|L_mshs_hq_room_title_dormroom6";
			case "gym":
				return "hq_bundle|L_mshs_hq_room_title_gym";
			case "lab":
				return "hq_bundle|L_mshs_hq_room_title_laboratory";
			case "rec_room":
				return "hq_bundle|L_mshs_hq_room_title_recreationroom";
			case "training":
				return "hq_bundle|L_mshs_hq_room_title_trainingroom";
			default:
				return string.Empty;
			}
		}
	}

	public delegate void OnClickOk();

	private SHSHQHeroSelect HeroSelect;

	private SHSHQRoomTitle RoomTitle;

	private GUIButton CharScrollLeft;

	private GUIButton CharScrollRight;

	private GUIButton CharShortcut;

	private GUIButton FoodShortcut;

	private GUIButton Load;

	private GUIButton Save;

	private GUIButton Reset;

	private GUIButton RoomLocked;

	private GUIButton RoomUnlocked;

	private GUIButton RoomScrollLeft;

	private GUIButton RoomScrollRight;

	private GUIButton ZoomIn;

	private GUIButton ZoomOut;

	private PlayPauseCombinedButton PlayAndPauseButton;

	private GUILabel InfoLabel;

	private GUIButton PurchaseButton;

	private GUIButton SubscribeButton;

	private SHSPauseOverlay PauseOverlay;

	private GUISimpleControlWindow _globalNavAnchor;

	private GlobalNav _globalNav;

	private Dictionary<GUIControl, Vector2> OffsetLookup = new Dictionary<GUIControl, Vector2>();

	private Direction LastKnownDirection;

	private HQState CurrentHQState;

	private HqRoom2 SelectedRoom
	{
		get
		{
			return HqController2.Instance.ActiveRoom;
		}
	}

	public SHSHQHUD()
	{
		Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		IsVisible = false;
		SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
		SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		Add(GUIControl.CreateControlBottomFrameCentered<BlockTestSpot>(new Vector2(162f, 129f), new Vector2(0f, -107f)));
		Add(GUIControl.CreateControlBottomFrameCentered<BlockTestSpot>(new Vector2(355f, 50f), new Vector2(0f, -94f)));
		Add(GUIControl.CreateControlBottomFrameCentered<BlockTestSpot>(new Vector2(662f, 81f), new Vector2(0f, -35f)));
		PauseOverlay = new SHSPauseOverlay();
		Add(PauseOverlay);
		RoomTitle = new SHSHQRoomTitle();
		Add(RoomTitle);
		GUIHotSpotButton gUIHotSpotButton = GUIControl.CreateControlBottomFrameCentered<GUIHotSpotButton>(new Vector2(260f, 70f), new Vector2(0f, -50f));
		gUIHotSpotButton.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		Add(gUIHotSpotButton);
		gUIHotSpotButton.MouseOver += delegate
		{
			HeroSelect.HeroBoxMouseOver();
		};
		gUIHotSpotButton.MouseOut += delegate
		{
			HeroSelect.HeroBoxMouseOut();
		};
		HeroSelect = new SHSHQHeroSelect(this);
		Add(HeroSelect);
		GUIImage gUIImage = GUIControl.CreateControlBottomFrameCentered<GUIImage>(new Vector2(662f, 129f), new Vector2(0f, -64f));
		gUIImage.TextureSource = "hq_bundle|mshs_hq_hud_main_bg";
		Add(gUIImage);
		CharScrollLeft = CreateAndAddHQButton("character_scroll_left", new Vector2(-105f, -24f), string.Empty);
		CharScrollRight = CreateAndAddHQButton("character_scroll_right", new Vector2(104f, -24f), string.Empty);
		CharScrollLeft.ToolTip = new NamedToolTipInfo("#TT_HQMINIMAP_25", new Vector2(0f, -50f));
		CharScrollRight.ToolTip = new NamedToolTipInfo("#TT_HQMINIMAP_25", new Vector2(0f, -50f));
		CharScrollLeft.HitTestType = HitTestTypeEnum.Alpha;
		CharScrollRight.HitTestType = HitTestTypeEnum.Alpha;
		RoomScrollLeft = CreateAndAddHQButton("room_scroll_left", new Vector2(-261f, -35f), "#TT_HQMINIMAP_4", new Vector2(128f, 128f));
		RoomScrollRight = CreateAndAddHQButton("room_scroll_right", new Vector2(262f, -35f), "#TT_HQMINIMAP_5", new Vector2(128f, 128f));
		RoomScrollLeft.HitTestType = HitTestTypeEnum.Alpha;
		RoomScrollRight.HitTestType = HitTestTypeEnum.Alpha;
		CharShortcut = CreateAndAddHQButton("character_shortcut", new Vector2(-106f, -96f), "#TT_HQMINIMAP_26");
		FoodShortcut = CreateAndAddHQButton("food_shortcut", new Vector2(-145f, -82f), "#TT_HQMINIMAP_27");
		Load = CreateAndAddHQButton("load", new Vector2(145f, -82f), "#TT_HQMINIMAP_9");
		Save = CreateAndAddHQButton("save", new Vector2(106f, -97f), "#TT_HQMINIMAP_8");
		Reset = CreateAndAddHQButton("reset_locked", new Vector2(162f, -25f), "#TT_HQMINIMAP_3");
		RoomLocked = CreateAndAddHQButton("room_locked", new Vector2(194f, -51f), "#TT_HQMINIMAP_28");
		RoomUnlocked = CreateAndAddHQButton("room_unlocked", new Vector2(194f, -51f), "#TT_HQMINIMAP_29");
		ZoomIn = CreateAndAddHQButton("zoom_in", new Vector2(-194f, -53f), "#TT_HQMINIMAP_1");
		ZoomOut = CreateAndAddHQButton("zoom_out", new Vector2(-165f, -25f), "#TT_HQMINIMAP_2");
		PlayAndPauseButton = new PlayPauseCombinedButton();
		OffsetLookup.Add(PlayAndPauseButton, PlayAndPauseButton.Offset);
		Add(PlayAndPauseButton);
		PurchaseButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(168f, 116f), new Vector2(-305f, 158f));
		PurchaseButton.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|L_buyit_button");
		PurchaseButton.HitTestSize = new Vector2(0.87f, 0.79f);
		PurchaseButton.IsVisible = false;
		PurchaseButton.IsEnabled = true;
		PurchaseButton.Id = "PurchaseButton";
		Add(PurchaseButton);
		SubscribeButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(-302f, 177f));
		SubscribeButton.StyleInfo = new SHSButtonStyleInfo("hq_bundle|L_hq_loading_agentbutton");
		SubscribeButton.HitTestType = HitTestTypeEnum.Alpha;
		SubscribeButton.Click += delegate
		{
			LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.Subscribe);
		};
		SubscribeButton.IsVisible = false;
		SubscribeButton.IsEnabled = true;
		SubscribeButton.Id = "SubscribeButton";
		Add(SubscribeButton);
		InfoLabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(150f, 123f), new Vector2(-326f, 23f));
		InfoLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 18, ColorUtil.FromRGB255(6, 71, 165), TextAnchor.MiddleCenter);
		InfoLabel.VerticalKerning = 18;
		InfoLabel.Text = "#hq_room_expired_nag";
		InfoLabel.WordWrap = true;
		InfoLabel.IsVisible = false;
		InfoLabel.Rotation = -9f;
		InfoLabel.Id = "SubscribeLabel";
		Add(InfoLabel);
		_globalNav = new GlobalNav(true);
		_globalNav.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, Vector2.zero);
		_globalNavAnchor = GUIControl.CreateControlTopRightFrame<GUISimpleControlWindow>(_globalNav.Size, new Vector2(0f, 0f));
		Add(_globalNavAnchor);
		_globalNavAnchor.IsVisible = true;
		_globalNavAnchor.Add(_globalNav);
		AddTheButtonDelegates();
	}

	public void AddTheButtonDelegates()
	{
		PurchaseButton.Click += delegate
		{
			if (SelectedRoom != null)
			{
				ShoppingWindow shoppingWindow = new ShoppingWindow(int.Parse(SelectedRoom.TypeId));
				shoppingWindow.launch();
			}
		};
		PlayAndPauseButton.hotSpot.Click += delegate
		{
			if (PlayAndPauseButton.CurrentState == PlayPauseEnum.Play)
			{
				if (HqController2.Instance != null && HqController2.Instance.State != typeof(HqController2.HqControllerFlinga))
				{
					HqController2.Instance.GotoFlingaMode();
				}
			}
			else if (HqController2.Instance != null && HqController2.Instance.State != typeof(HqController2.HqControllerView))
			{
				HqController2.Instance.GotoViewMode();
			}
		};
		CharShortcut.Click += delegate
		{
			if (!SHSInventoryAnimatedWindow.isCurrentTab(0))
			{
				SHSInventoryAnimatedWindow.SetHeroTabAsDefault();
				AppShell.Instance.EventMgr.Fire(this, new GlobalNavButtonVisibleMessage(GlobalNav.GlobalNavType.Inventory));
				AppShell.Instance.EventMgr.Fire(this, new InventoryRequestCenterButtonOnNewCurrentLoc());
			}
			else
			{
				AppShell.Instance.EventMgr.Fire(this, new GlobalNavButtonVisibleMessage(GlobalNav.GlobalNavType.Inventory));
			}
		};
		FoodShortcut.Click += delegate
		{
			if (!SHSInventoryAnimatedWindow.isCurrentTab(5))
			{
				SHSInventoryAnimatedWindow.SetFoodTabAsDefault();
				AppShell.Instance.EventMgr.Fire(this, new GlobalNavButtonVisibleMessage(GlobalNav.GlobalNavType.Inventory));
				AppShell.Instance.EventMgr.Fire(this, new InventoryRequestCenterButtonOnNewCurrentLoc());
			}
			else
			{
				AppShell.Instance.EventMgr.Fire(this, new GlobalNavButtonVisibleMessage(GlobalNav.GlobalNavType.Inventory));
			}
		};
		RoomLocked.Click += delegate
		{
			if (SelectedRoom != null)
			{
				AppShell.Instance.EventMgr.Fire(this, new HQRoomStateChangeMessage(SelectedRoom.Id, HqRoom2.AccessState.Unlocked));
			}
		};
		RoomUnlocked.Click += delegate
		{
			if (SelectedRoom != null)
			{
				AppShell.Instance.EventMgr.Fire(this, new HQRoomStateChangeMessage(SelectedRoom.Id, HqRoom2.AccessState.Locked));
			}
		};
		CharScrollLeft.Click += delegate
		{
			HeroSelect.GoLeft();
		};
		CharScrollRight.Click += delegate
		{
			HeroSelect.GoRight();
		};
		Load.Click += delegate
		{
			ShowDialog("#HQ_Dialolg_Load", delegate
			{
				if (HqController2.Instance != null)
				{
					HqController2.Instance.Load();
				}
			});
		};
		Save.Click += delegate
		{
			ShowDialog("#HQ_Dialolg_Save", delegate
			{
				if (HqController2.Instance != null)
				{
					HqController2.Instance.Save();
				}
			});
		};
		Reset.Click += delegate
		{
			ShowDialog("#HQ_Dialolg_Reset", delegate
			{
				if (HqController2.Instance != null)
				{
					HqController2.Instance.Reset();
				}
			});
		};
		ZoomIn.Click += delegate
		{
			AppShell.Instance.EventMgr.Fire(this, new HQRoomZoomRequestMessage(true));
		};
		ZoomOut.Click += delegate
		{
			AppShell.Instance.EventMgr.Fire(this, new HQRoomZoomRequestMessage(false));
		};
		RoomScrollRight.Click += delegate
		{
			LastKnownDirection = Direction.Right;
			AppShell.Instance.EventMgr.Fire(this, new HQRoomChangeRequestMessage(HQRoomChangeRequestMessage.RoomCycleDirection.Next));
		};
		RoomScrollLeft.Click += delegate
		{
			LastKnownDirection = Direction.Left;
			AppShell.Instance.EventMgr.Fire(this, new HQRoomChangeRequestMessage(HQRoomChangeRequestMessage.RoomCycleDirection.Previous));
		};
	}

	public void ShowDialog(string text, OnClickOk del)
	{
		GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, text, delegate(string identifier, GUIDialogWindow.DialogState state)
		{
			if (state == GUIDialogWindow.DialogState.Ok && del != null)
			{
				del();
			}
		}, ModalLevelEnum.Default);
	}

	public override void OnShow()
	{
		base.OnShow();
		AppShell.Instance.EventMgr.AddListener<HQModeChanged>(OnHqModeChanged);
		AppShell.Instance.EventMgr.AddListener<HQRoomChangedMessage>(OnHqRoomChanged);
		AppShell.Instance.EventMgr.AddListener<HQRoomStateChangeMessage>(OnHqRoomStateChanged);
		AppShell.Instance.EventMgr.AddListener<GUIFullScreenClosedEvent>(OnFullScreenUIClosed);
		LastKnownDirection = Direction.Right;
		CurrentHQState = HQState.Play;
		OnHqModeChanged(new HQModeChanged(typeof(HqController2.HqControllerFlinga)));
		if (SelectedRoom != null)
		{
			OnHqRoomChanged(new HQRoomChangedMessage(SelectedRoom.Id));
		}
		base.AnimationPieceManager.Add(OpeningAnimation.OpenAnim(this));
	}

	public override void OnHide()
	{
		base.OnHide();
		AppShell.Instance.EventMgr.RemoveListener<HQModeChanged>(OnHqModeChanged);
		AppShell.Instance.EventMgr.RemoveListener<HQRoomChangedMessage>(OnHqRoomChanged);
		AppShell.Instance.EventMgr.RemoveListener<HQRoomStateChangeMessage>(OnHqRoomStateChanged);
		AppShell.Instance.EventMgr.RemoveListener<GUIFullScreenClosedEvent>(OnFullScreenUIClosed);
	}

	private void OnHqRoomStateChanged(HQRoomStateChangeMessage msg)
	{
		if (SelectedRoom != null && msg.roomName == SelectedRoom.Id)
		{
			switch (msg.state)
			{
			case HqRoom2.AccessState.Locked:
				RoomLocked.IsVisible = true;
				RoomUnlocked.IsVisible = false;
				PurchaseButton.IsVisible = false;
				SubscribeButton.IsVisible = false;
				InfoLabel.IsVisible = false;
				break;
			case HqRoom2.AccessState.Unlocked:
				RoomLocked.IsVisible = false;
				RoomUnlocked.IsVisible = true;
				PurchaseButton.IsVisible = false;
				SubscribeButton.IsVisible = false;
				InfoLabel.IsVisible = false;
				break;
			case HqRoom2.AccessState.Unpurchased:
				RoomLocked.IsVisible = false;
				RoomUnlocked.IsVisible = false;
				InfoLabel.IsVisible = true;
				PurchaseButton.IsVisible = true;
				SubscribeButton.IsVisible = false;
				InfoLabel.Text = "#hq_room_purchase_nag_sub";
				break;
			case HqRoom2.AccessState.Unauthorized:
				RoomLocked.IsVisible = false;
				RoomUnlocked.IsVisible = false;
				PurchaseButton.IsVisible = false;
				InfoLabel.Text = "#hq_room_expired_nag";
				InfoLabel.IsVisible = true;
				SubscribeButton.IsVisible = true;
				break;
			}
		}
	}

	private void OnHqRoomChanged(HQRoomChangedMessage msg)
	{
		SendMessageAndShowRoomTitle();
	}

	private void OnFullScreenUIClosed(GUIFullScreenClosedEvent msg)
	{
		SendMessageAndShowRoomTitle();
	}

	public void SendMessageAndShowRoomTitle()
	{
		if (!(SelectedRoom != null))
		{
			return;
		}
		OnHqRoomStateChanged(new HQRoomStateChangeMessage(SelectedRoom.Id, SelectedRoom.RoomState));
		bool fadeOut = SelectedRoom.RoomState == HqRoom2.AccessState.Locked || SelectedRoom.RoomState == HqRoom2.AccessState.Unlocked;
		RoomTitle.ShowRoom(SelectedRoom.Id, LastKnownDirection, fadeOut);
		if (CurrentHQState == HQState.Pause && ShouldShowPause())
		{
			if (!PauseOverlay.IsVisible)
			{
				PauseOverlay.IsVisible = true;
			}
		}
		else
		{
			PauseOverlay.IsVisible = false;
		}
	}

	private bool ShouldShowPause()
	{
		if (SelectedRoom == null)
		{
			return false;
		}
		return SelectedRoom.RoomState != HqRoom2.AccessState.Unauthorized && SelectedRoom.RoomState != HqRoom2.AccessState.Unpurchased;
	}

	private void OnHqModeChanged(HQModeChanged msg)
	{
		if (msg.modeType == typeof(HqController2.HqControllerFlinga))
		{
			CurrentHQState = HQState.Play;
			PlayAndPauseButton.ShowPause();
			Save.IsEnabled = false;
			Load.IsEnabled = false;
			Reset.IsEnabled = false;
			PauseOverlay.IsVisible = false;
			return;
		}
		CurrentHQState = HQState.Pause;
		PlayAndPauseButton.ShowPlay();
		Save.IsEnabled = true;
		Load.IsEnabled = true;
		Reset.IsEnabled = true;
		if (ShouldShowPause() && !PauseOverlay.IsVisible)
		{
			PauseOverlay.IsVisible = true;
		}
	}

	private GUIButton CreateAndAddHQButton(string path, Vector2 offset, string tooltip, Vector2 size)
	{
		GUIButton gUIButton = GUIControl.CreateControlBottomFrameCentered<GUIButton>(size, offset);
		OffsetLookup.Add(gUIButton, offset);
		gUIButton.StyleInfo = new SHSButtonStyleInfo("hq_bundle|mshs_button_" + path);
		gUIButton.HitTestType = HitTestTypeEnum.Circular;
		gUIButton.HitTestSize = new Vector2(0.625f, 0.625f);
		if (!string.IsNullOrEmpty(tooltip))
		{
			gUIButton.ToolTip = new NamedToolTipInfo(tooltip);
		}
		Add(gUIButton);
		return gUIButton;
	}

	private GUIButton CreateAndAddHQButton(string path, Vector2 offset, string tooltip)
	{
		return CreateAndAddHQButton(path, offset, tooltip, new Vector2(64f, 64f));
	}

	public override void Update()
	{
		base.Update();
		float mouseWheelDelta = SHSInput.GetMouseWheelDelta();
		if (mouseWheelDelta != 0f)
		{
			if (0f > mouseWheelDelta)
			{
				AppShell.Instance.EventMgr.Fire(this, new HQRoomZoomRequestMessage(false));
			}
			else if (0f < mouseWheelDelta)
			{
				AppShell.Instance.EventMgr.Fire(this, new HQRoomZoomRequestMessage(true));
			}
		}
	}
}
