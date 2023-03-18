using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SHSSocialCharacterDisplayWindow : GUISimpleControlWindow
{
	public class CascadingEmoteChat : GUISimpleControlWindow
	{
		public class MenuChatButton : GUIAnimatedButton
		{
			public MenuChatButton()
			{
				TextureSource = "gameworld_bundle|mshs_gameworld_HUD_chatbutton_normal";
				base.HighlightPath = delegate
				{
					return 1f + AnimClipBuilder.Path.Sin(0f, 2f, 0.5f) * AnimClipBuilder.Path.Linear(0.1f, 0.05f, 0.5f);
				};
				base.HighlightPercentage = new Vector2(1.1f, 1.1f);
				base.PressedPercentage = new Vector2(0.9f, 0.9f);
				MouseOver += OnMouseOver;
			}

			private void OnMouseOver(GUIControl sender, GUIMouseEvent EventData)
			{
				ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("bouncy_hover_over"));
			}
		}

		protected MenuChatButton menuChatButton;

		public CascadingEmoteChat(TargetedHeroButtons thb)
		{
			SetSize(2000f, 2000f);
			SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 11f));
			Add(GUIControl.CreateControlCenter<BlockTestSpot>(new Vector2(64f, 64f), new Vector2(121f, 181f)));
			menuChatButton = GUIControl.CreateControlCenter<MenuChatButton>(new Vector2(64f, 64f), new Vector2(155f, 178f));
			menuChatButton.ToolTip = new NamedToolTipInfo("#TT_CHAT_TOGGLE");
			menuChatButton.HitTestType = HitTestTypeEnum.Circular;
			menuChatButton.HitTestSize = new Vector2(0.9f, 0.9f);
			menuChatButton.Id = "Menu Chat Button";
			Add(menuChatButton);
			menuChatButton.Click += ToggleMenuChat;
		}

		protected void ToggleMenuChat(GUIControl sender, GUIClickEvent EventData)
		{
			AppShell.Instance.EventMgr.Fire(this, new MenuChatActivateMessage(sender.ScreenRect, new Vector2(5f, 20f)));
		}
	}

	public class ChallengeIndicator : GUISimpleControlWindow
	{
		private enum AnimationFlag
		{
			ZoneLoadFlag = 1,
			LocalPlayerSpawnFlag = 2,
			GUIFullScreenFlag = 4,
			IndicatorFocusFlag = 8,
			CanPlayFlag = 3
		}

		private const float _IndicatorClipTime = 1f;

		private const float _IndicatorClipCount = 12f;

		private const float _IndicatorClipWaitTime = 2.5f;

		private const float _IndicatorAlphaFadeTime = 1f;

		private AnimClip _indicatorAlphaAnimation;

		private AnimClip _indicatorClipAnimation;

		private AnimClip _indicatorHoverAnimation;

		private GUIButton _indicatorButton;

		private GUIImage _indicatorAlphaImage;

		private GUIImage _indicatorClipImage;

		private ShsAudioSourceList _indicatorSoundList;

		private Vector2 _IndicatorClipImageSize = new Vector2(73f, 62f);

		private int _indicatorAnimationState;

		private bool _indicatorAnimationPlaying;

		private int _indicatiorClipCurrentFrame;

		private ChallengeManager.ChallengeManagerStateEnum _prevChallengeState;

		public ChallengeIndicator()
		{
			SetSize(128f, 128f);
			SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(-15f, -40f));
			Add(GUIControl.CreateControlFrameCentered<BlockTestSpot>(new Vector2(98f, 48f), Vector2.zero));
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(98f, 48f), Vector2.zero);
			gUIImage.TextureSource = "gameworld_bundle|button_challenge_indicator_container";
			Add(gUIImage);
			_indicatorButton = GUIControl.CreateControlAbsolute<GUIButton>(new Vector2(128f, 128f), new Vector2(0f, -1f));
			_indicatorButton.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|button_challenge_indicator");
			_indicatorButton.HitTestSize = new Vector2(0.61f, 0.23f);
			_indicatorButton.HitTestType = HitTestTypeEnum.Rect;
			_indicatorButton.Click += IndicatorButton_Click;
			_indicatorButton.MouseOver += IndicatorButton_MouseOver;
			_indicatorButton.MouseOut += IndicatorButton_MouseOut;
			_indicatorButton.ToolTip = new NamedToolTipInfo("#SQ_CHALLENGE_BUTTON_TOOLTIP");
			_indicatorButton.ToolTip.OverrideTooltipAlignment = true;
			_indicatorButton.ToolTip.Offset = new Vector2(135f, 35f);
			_indicatorButton.ToolTip.VerticalAlignmentOverride = SHSTooltip.ToolTipVerticalAlignment.Top;
			_indicatorButton.ToolTip.HorizontalAlignmentOverride = SHSTooltip.ToolTipHorizontalAlignment.Right;
			_indicatorButton.Id = "_indicatorButton";
			Add(_indicatorButton);
			_indicatorAlphaImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(128f, 128f), new Vector2(0f, -1f));
			_indicatorAlphaImage.TextureSource = "gameworld_bundle|button_challenge_indicator_highlight";
			_indicatorAlphaImage.Alpha = 0f;
			Add(_indicatorAlphaImage);
			_indicatorClipImage = GUIControl.CreateControlFrameCentered<GUIImage>(_IndicatorClipImageSize, new Vector2(7f, 5f));
			ShowFrame(0f);
			Add(_indicatorClipImage);
			SetFlag(AnimationFlag.ZoneLoadFlag);
			_indicatorSoundList = ShsAudioSourceList.GetList("ChallengeSystem");
			if (_indicatorSoundList == null)
			{
				CspUtils.DebugLog("ChallengeIndicator::ChallengeIndicator() - failed to obtain audio list named 'ChallengeSystem'");
			}
		}

		public void UpdateIndicatorAnimation()
		{
			if (AppShell.Instance.ChallengeManager == null)
			{
				return;
			}
			if (!CanPlayIndicatorAnimation())
			{
				if (IsPlayingIndicatorAnimation())
				{
					StopIndicatorAnimation();
				}
				return;
			}
			if (AppShell.Instance.ChallengeManager.ChallengeManagerStatus == ChallengeManager.ChallengeManagerStateEnum.ChallengeDisplayPending)
			{
				if (_prevChallengeState != ChallengeManager.ChallengeManagerStateEnum.ChallengeDisplayPending)
				{
					OnChallengeDisplayPending();
				}
				if (!IsPlayingIndicatorAnimation())
				{
					StartIndicatorAnimation();
				}
			}
			else if (IsPlayingIndicatorAnimation())
			{
				StopIndicatorAnimation();
			}
			_prevChallengeState = AppShell.Instance.ChallengeManager.ChallengeManagerStatus;
		}

		public void StartIndicatorAnimation()
		{
			if (!IsPlayingIndicatorAnimation())
			{
				DoAlphaAnimation(true);
				DoClipAnimation();
				if (_indicatorSoundList != null)
				{
					ShsAudioSource.PlayAutoSound(_indicatorSoundList.GetSource("challenge_display_pending"));
				}
				_indicatorAnimationPlaying = true;
			}
		}

		public void StopIndicatorAnimation()
		{
			if (IsPlayingIndicatorAnimation())
			{
				base.AnimationPieceManager.Remove(_indicatorAlphaAnimation);
				_indicatorAlphaAnimation = null;
				_indicatorAlphaImage.Alpha = 0f;
				base.AnimationPieceManager.Remove(_indicatorClipAnimation);
				_indicatorClipAnimation = null;
				ShowFrame(0f);
				_indicatorAnimationPlaying = false;
			}
		}

		public bool IsPlayingIndicatorAnimation()
		{
			return _indicatorAnimationPlaying;
		}

		public bool CanPlayIndicatorAnimation()
		{
			return (3 ^ _indicatorAnimationState) == 0;
		}

		public override void OnActive()
		{
			base.OnActive();
			AppShell.Instance.EventMgr.AddListener<GUIFullScreenOpenEvent>(OnGUIFullScreenOpen);
			AppShell.Instance.EventMgr.AddListener<GUIFullScreenClosedEvent>(OnGUIFullScreenClosed);
			AppShell.Instance.EventMgr.AddListener<LocalPlayerChangedMessage>(OnLocalPlayerChanged);
			AppShell.Instance.EventMgr.AddListener<ZoneLoadedMessage>(OnZoneLoaded);
			AppShell.Instance.EventMgr.AddListener<ZoneUnloadedMessage>(OnZoneUnloaded);
		}

		public override void OnInactive()
		{
			base.OnInactive();
			AppShell.Instance.EventMgr.RemoveListener<GUIFullScreenOpenEvent>(OnGUIFullScreenOpen);
			AppShell.Instance.EventMgr.RemoveListener<GUIFullScreenClosedEvent>(OnGUIFullScreenClosed);
			AppShell.Instance.EventMgr.RemoveListener<LocalPlayerChangedMessage>(OnLocalPlayerChanged);
			AppShell.Instance.EventMgr.RemoveListener<ZoneLoadedMessage>(OnZoneLoaded);
			AppShell.Instance.EventMgr.RemoveListener<ZoneUnloadedMessage>(OnZoneUnloaded);
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			UpdateIndicatorAnimation();
		}

		private void DoAlphaAnimation(bool fadeOut)
		{
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Expected O, but got Unknown
			float num = (!fadeOut) ? 0f : 1f;
			float finish = 1f - num;
			_indicatorAlphaImage.Alpha = num;
			_indicatorAlphaAnimation = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(num, finish, 1f), _indicatorAlphaImage);
			_indicatorAlphaAnimation.OnFinished += (Action)(object)(Action)delegate
			{
				DoAlphaAnimation(!fadeOut);
			};
			base.AnimationPieceManager.SwapOut(ref _indicatorAlphaAnimation, _indicatorAlphaAnimation);
		}

		private void DoClipAnimation()
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Expected O, but got Unknown
			_indicatorClipAnimation = (AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(0f, 12f, 1f), ShowFrame) | SHSAnimations.Generic.Wait(2.5f));
			_indicatorClipAnimation.OnFinished += (Action)(object)(Action)delegate
			{
				DoClipAnimation();
			};
			base.AnimationPieceManager.SwapOut(ref _indicatorClipAnimation, _indicatorClipAnimation);
		}

		private void ShowFrame(float frame)
		{
			int num = 1;
			if (frame >= 0f && frame < 12f)
			{
				num += (int)frame;
			}
			if (_indicatiorClipCurrentFrame != num)
			{
				_indicatiorClipCurrentFrame = num;
				if (num < 10)
				{
					_indicatorClipImage.TextureSource = "gameworld_bundle|challenge_icon_anim0" + num;
				}
				else
				{
					_indicatorClipImage.TextureSource = "gameworld_bundle|challenge_icon_anim" + num;
				}
			}
		}

		private void OnChallengeDisplayPending()
		{
		}

		private void IndicatorButton_Click(GUIControl sender, GUIClickEvent EventData)
		{
		}

		private void IndicatorButton_MouseOut(GUIControl sender, GUIMouseEvent EventData)
		{
			ClearFlag(AnimationFlag.IndicatorFocusFlag);
			base.AnimationPieceManager.Remove(_indicatorHoverAnimation);
			_indicatorHoverAnimation = null;
			_indicatorClipImage.SetSize(_IndicatorClipImageSize);
		}

		private void IndicatorButton_MouseOver(GUIControl sender, GUIMouseEvent EventData)
		{
			SetFlag(AnimationFlag.IndicatorFocusFlag);
			AnimPath a = 1f + AnimClipBuilder.Path.Sin(0f, 2f, 0.5f) * AnimClipBuilder.Path.Linear(0.1f, 0.05f, 0.5f);
			base.AnimationPieceManager.SwapOut(ref _indicatorHoverAnimation, AnimClipBuilder.Absolute.SizeX(a * _IndicatorClipImageSize.x, _indicatorClipImage) ^ AnimClipBuilder.Absolute.SizeY(a * _IndicatorClipImageSize.y, _indicatorClipImage));
		}

		private void OnGUIFullScreenOpen(GUIFullScreenOpenEvent msg)
		{
			SetFlag(AnimationFlag.GUIFullScreenFlag);
		}

		private void OnGUIFullScreenClosed(GUIFullScreenClosedEvent msg)
		{
			ClearFlag(AnimationFlag.GUIFullScreenFlag);
		}

		private void OnLocalPlayerChanged(LocalPlayerChangedMessage msg)
		{
			if (msg.localPlayer != null)
			{
				SetFlag(AnimationFlag.LocalPlayerSpawnFlag);
			}
			else
			{
				ClearFlag(AnimationFlag.LocalPlayerSpawnFlag);
			}
		}

		private void OnZoneLoaded(ZoneLoadedMessage msg)
		{
			SetFlag(AnimationFlag.ZoneLoadFlag);
		}

		private void OnZoneUnloaded(ZoneUnloadedMessage msg)
		{
			ClearFlag(AnimationFlag.ZoneLoadFlag);
		}

		private void SetFlag(AnimationFlag flag)
		{
			_indicatorAnimationState |= (int)flag;
			if (!CanPlayIndicatorAnimation() && IsPlayingIndicatorAnimation())
			{
				StopIndicatorAnimation();
			}
		}

		private void ClearFlag(AnimationFlag flag)
		{
			_indicatorAnimationState &= (int)(~flag);
			if (!CanPlayIndicatorAnimation() && IsPlayingIndicatorAnimation())
			{
				StopIndicatorAnimation();
			}
		}
	}

	public class GrowingNameplate : GUISimpleControlWindow
	{
		public class GrowingSubNameplate : GUISimpleControlWindow
		{
			private GUIImage leftPiece;

			private GUIImage midPiece;

			private GUIImage rightPiece;

			private int leftPieceWidth;

			private int rightPieceWidth;

			public GrowingSubNameplate(string PieceLoc, int leftPieceWidth, int rightPieceWidth)
			{
				this.leftPieceWidth = leftPieceWidth;
				this.rightPieceWidth = rightPieceWidth;
				leftPiece = GUIControl.CreateControl<GUIImage>(Vector2.zero, Vector2.zero, DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft);
				midPiece = GUIControl.CreateControl<GUIImage>(Vector2.zero, Vector2.zero, DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
				rightPiece = GUIControl.CreateControl<GUIImage>(Vector2.zero, Vector2.zero, DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight);
				leftPiece.TextureSource = "gameworld_bundle|mshs_gameworld_HUD_" + PieceLoc + "_left";
				midPiece.TextureSource = "gameworld_bundle|mshs_gameworld_HUD_" + PieceLoc + "_center";
				rightPiece.TextureSource = "gameworld_bundle|mshs_gameworld_HUD_" + PieceLoc + "_right";
				Add(midPiece);
				Add(leftPiece);
				Add(rightPiece);
				SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
			}

			public override void HandleResize(GUIResizeMessage message)
			{
				base.HandleResize(message);
				GUIImage gUIImage = leftPiece;
				float width = leftPieceWidth;
				Vector2 size = Size;
				gUIImage.SetSize(width, size.y);
				GUIImage gUIImage2 = rightPiece;
				float width2 = rightPieceWidth;
				Vector2 size2 = Size;
				gUIImage2.SetSize(width2, size2.y);
				GUIImage gUIImage3 = midPiece;
				Vector2 size3 = Size;
				float width3 = size3.x - (float)(leftPieceWidth + rightPieceWidth);
				Vector2 size4 = Size;
				gUIImage3.SetSize(width3, size4.y);
			}
		}

		private GUILabel name;

		private GUILabel securityLevel;

		private GrowingSubNameplate back;

		private GrowingSubNameplate front;

		private float nameplateWidth;

		private GUIHotSpotButton _clickSpot;

		[CompilerGenerated]
		private string _003CSecurityLevelPrefix_003Ek__BackingField;

		public string Text
		{
			get
			{
				return name.Text;
			}
			set
			{
				SetupText(value);
			}
		}

		public string SecurityLevel
		{
			get
			{
				return securityLevel.Text;
			}
			set
			{
				SetupSecurityLevel(value);
			}
		}

		public string SecurityLevelPrefix
		{
			[CompilerGenerated]
			get
			{
				return _003CSecurityLevelPrefix_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CSecurityLevelPrefix_003Ek__BackingField = value;
			}
		}

		public GrowingNameplate(string PieceLoc, TextAnchor securityLevelAnchor, string securityLevelTooltip)
		{
			SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
			HitTestType = HitTestTypeEnum.Rect;
			BlockTestType = BlockTestTypeEnum.Rect;
			_clickSpot = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(Vector2.zero, Vector2.zero);
			_clickSpot.HitTestType = HitTestTypeEnum.Rect;
			_clickSpot.HitTestSize = new Vector2(1f, 1f);
			_clickSpot.Click += HotSpotClick;
			Add(_clickSpot);
			_clickSpot.IsVisible = true;
			name = GUIControl.CreateControlFrameCentered<GUILabel>(Vector2.zero, Vector2.zero);
			name.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(31, 57, 68), TextAnchor.UpperLeft);
			name.WordWrap = false;
			name.Overflow = true;
			securityLevel = GUIControl.CreateControlFrameCentered<GUILabel>(Vector2.zero, Vector2.zero);
			securityLevel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(104, 160, 9), securityLevelAnchor);
			securityLevel.WordWrap = false;
			securityLevel.Overflow = true;
			back = new GrowingSubNameplate(PieceLoc + "_back", 19, 19);
			front = new GrowingSubNameplate(PieceLoc + "_front", 19, 19);
			Add(back);
			Add(front);
			Add(name);
			Add(securityLevel);
			Text = string.Empty;
			SecurityLevel = string.Empty;
		}

		private void HotSpotClick(GUIControl sender, GUIClickEvent EventData)
		{
			MySquadDataManager dataManager = new MySquadDataManager(AppShell.Instance.Profile);
			MySquadWindow dialogWindow = new MySquadWindow(dataManager);
			GUIManager.Instance.ShowDynamicWindow(dialogWindow, ModalLevelEnum.Default);
		}

		private void SetupText(string text)
		{
			name.Text = text;
			SetupNameplateWidth();
		}

		private void SetupSecurityLevel(string text)
		{
			if (AppShell.Instance != null && AppShell.Instance.stringTable != null)
			{
				securityLevel.Text = AppShell.Instance.stringTable[SecurityLevelPrefix] + text;
			}
			else
			{
				securityLevel.Text = text;
			}
			SetupNameplateWidth();
		}

		private void SetupNameplateWidth()
		{
			float num = Mathf.Max(name.GetTextWidth(), securityLevel.GetTextWidth());
			if (!(Mathf.Abs(num - nameplateWidth) <= float.Epsilon))
			{
				nameplateWidth = num;
				back.SetSize(nameplateWidth + 40f, 48f);
				front.SetSize(nameplateWidth + 20f, 36f);
				name.SetSize(nameplateWidth, 30f);
				securityLevel.SetSize(nameplateWidth, 30f);
				SetSize(nameplateWidth + 40f, 48f);
				_clickSpot.SetSize(Size);
			}
		}
	}

	public class MainHeroPortrait : GUISimpleControlWindow
	{
		private GUIImage bkg;

		protected GUIImage heroPortrait;

		protected GUILabel heroNameLabel;

		protected GUILabel heroLevel;

		protected GUIDrawTexture heroLevelBackground;

		protected GrowingNameplate SquadName;

		protected GUIButton ChangeHero;

		protected GUISimpleControlWindow _bg;

		protected GUISimpleControlWindow _xpBar;

		protected GUILabel _xpBarXPTotalLabel;

		protected int _baseXPBarWidth;

		protected float squadMinX;

		public MainHeroPortrait()
		{
			SetSize(500f, 400f);
			SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(-58f, -63f));
			Vector2 offset = Offset;
			squadMinX = 0f - offset.x;
			_bg = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(277f, 163f), new Vector2(-50f, 0f));
			_bg.IsVisible = true;
			Add(_bg);
			bkg = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(277f, 163f), new Vector2(0f, 0f));
			bkg.TextureSource = "gameworld_bundle|mshs_gameworld_HUD_playerframe_sidekick";
			_bg.Add(bkg);
			BlockTestSpot blockTestSpot = GUIControl.CreateControlFrameCentered<BlockTestSpot>(new Vector2(145f, 145f), new Vector2(-92f, 0f));
			blockTestSpot.BlockTestType = BlockTestTypeEnum.Circular;
			blockTestSpot.HitTestType = HitTestTypeEnum.Circular;
			Add(blockTestSpot);
			Add(GUIControl.CreateControlFrameCentered<BlockTestSpot>(new Vector2(140f, 34f), new Vector2(20f, -60f)));
			heroPortrait = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(200f, 218f), new Vector2(-90f, -11f));
			Add(heroPortrait);
			heroNameLabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(150f, 30f), new Vector2(33f, -61f));
			heroNameLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 11, GUILabel.GenColor(104, 160, 9), TextAnchor.MiddleLeft);
			Add(heroNameLabel);
			SquadName = new GrowingNameplate("playername", TextAnchor.LowerLeft, "#TT_PLAYER_TARGETING_MY_SQUAD_LEVEL");
			SquadName.Offset = new Vector2(-122f, -113f);
			SquadName.SecurityLevelPrefix = "#PLAYER_TARGETING_MY_SQUAD_LEVEL";
			Add(SquadName);
			blockTestSpot = GUIControl.CreateControlFrameCentered<BlockTestSpot>(new Vector2(54f, 54f), new Vector2(-159f, -54f));
			blockTestSpot.BlockTestType = BlockTestTypeEnum.Circular;
			blockTestSpot.HitTestType = HitTestTypeEnum.Circular;
			Add(blockTestSpot);
			ChangeHero = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(54f, 54f), new Vector2(-159f, -54f));
			ChangeHero.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|mshs_gameworld_HUD_changehero");
			ChangeHero.ToolTip = new NamedToolTipInfo("#GAMEWORLD_PICKHERO_BUTTON");
			ChangeHero.HitTestSize = new Vector2(1f, 1f);
			ChangeHero.HitTestType = HitTestTypeEnum.Circular;
			ChangeHero.Click += ChangeHero_Click;
			Add(ChangeHero);
			heroLevelBackground = GUIControl.CreateControlFrameCentered<GUIImageWithEvents>(new Vector2(32f, 32f), new Vector2(67f, -61f));
			heroLevelBackground.TextureSource = "gameworld_bundle|mshs_gameworld_HUD_level_background";
			heroLevelBackground.MouseOver += delegate
			{
				_xpBarXPTotalLabel.IsVisible = true;
				heroNameLabel.IsVisible = false;
			};
			heroLevelBackground.MouseOut += delegate
			{
				_xpBarXPTotalLabel.IsVisible = false;
				heroNameLabel.IsVisible = true;
			};
			heroLevelBackground.Alpha = 0f;
			Add(heroLevelBackground);
			heroLevel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(32f, 32f), new Vector2(66f, -61f));
			heroLevel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(255, 255, 255), TextAnchor.MiddleCenter);
			Add(heroLevel);
			_xpBar = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(new Vector2(98f, 21f), new Vector2(144f, 11f));
			GUIImage gUIImage = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(98f, 21f), new Vector2(0f, 0f));
			gUIImage.TextureSource = "gameworld_bundle|xpbarsmall";
			_xpBar.Add(gUIImage);
			_bg.Add(_xpBar);
			Vector2 size = _xpBar.Size;
			_baseXPBarWidth = (int)size.x;
			Vector2 size2 = gUIImage.Size;
			_xpBarXPTotalLabel = GUIControl.CreateControlTopLeftFrame<GUILabel>(new Vector2(100f, size2.y), _xpBar.Position + new Vector2(3f, 0f));
			_xpBarXPTotalLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, GUILabel.GenColor(104, 160, 9), TextAnchor.MiddleCenter);
			_bg.Add(_xpBarXPTotalLabel);
			_xpBarXPTotalLabel.IsVisible = false;
		}

		protected void ChangeHero_Click(GUIControl sender, GUIClickEvent EventData)
		{
			SocialSpaceController.Instance.Controller.ChangeCharacters();
		}

		public override void OnShow()
		{
			RefreshHero();
			base.OnShow();
			AppShell.Instance.EventMgr.AddListener<CharacterSelectedMessage>(OnCharacterSelected);
			AppShell.Instance.EventMgr.AddListener<LeveledUpMessage>(OnLeveledUp);
			AppShell.Instance.EventMgr.AddListener<PlayerInfoUpdateMessage>(OnPlayerInfoUpdated);
			AppShell.Instance.EventMgr.AddListener<HeroXPUpdateMessage>(OnHeroXPUpdateMessage);
			AppShell.Instance.EventMgr.AddListener<BadgeAcquiredMessage>(OnBadgeAcquiredMessage);
		}

		public override void OnHide()
		{
			base.OnHide();
			AppShell.Instance.EventMgr.RemoveListener<CharacterSelectedMessage>(OnCharacterSelected);
			AppShell.Instance.EventMgr.RemoveListener<LeveledUpMessage>(OnLeveledUp);
			AppShell.Instance.EventMgr.RemoveListener<PlayerInfoUpdateMessage>(OnPlayerInfoUpdated);
			AppShell.Instance.EventMgr.RemoveListener<HeroXPUpdateMessage>(OnHeroXPUpdateMessage);
			AppShell.Instance.EventMgr.RemoveListener<BadgeAcquiredMessage>(OnBadgeAcquiredMessage);
		}

		private void OnBadgeAcquiredMessage(BadgeAcquiredMessage msg)
		{
			CspUtils.DebugLog("SHSSocialCharacterDisplayWindow OnBadgeAcquiredMessage");
			RefreshHero();
		}

		private void OnCharacterSelected(CharacterSelectedMessage msg)
		{
			RefreshHero();
		}

		private void OnLeveledUp(LeveledUpMessage msg)
		{
			RefreshHero();
		}

		private void OnPlayerInfoUpdated(PlayerInfoUpdateMessage msg)
		{
			RefreshHero();
		}

		private void OnHeroXPUpdateMessage(HeroXPUpdateMessage msg)
		{
			RefreshHero();
		}

		public virtual void RefreshHero()
		{
			UserProfile profile = AppShell.Instance.Profile;
			SquadName.Text = profile.PlayerName;
			SquadName.SecurityLevel = profile.SquadLevel.ToString();
			ClampSquadNameplatePosition();
			ShsEventMgr eventMgr = AppShell.Instance.EventMgr;
			Vector2 position = SquadName.Position;
			float x = position.x;
			Vector2 size = SquadName.Size;
			eventMgr.Fire(this, new RepositionSocialHUDMessage(x + size.x));
			string selectedCostume = profile.SelectedCostume;
			if (string.IsNullOrEmpty(selectedCostume))
			{
				return;
			}
			Texture2D texture;
			if (GUIManager.Instance.LoadTexture("characters_bundle|" + selectedCostume + "_HUD_default", out texture))
			{
				heroPortrait.Texture = texture;
			}
			else
			{
				heroPortrait.Texture = null;
			}
			heroNameLabel.Text = AppShell.Instance.CharacterDescriptionManager[selectedCostume].CharacterFamily.ToUpperInvariant();
			if (string.IsNullOrEmpty(selectedCostume) || !profile.AvailableCostumes.ContainsKey(selectedCostume))
			{
				heroLevel.Text = "#hero_unknown_level";
				return;
			}
			HeroPersisted heroPersisted = profile.AvailableCostumes[selectedCostume];
			if (heroPersisted.Level >= heroPersisted.MaxLevel)
			{
				heroLevel.Text = "#Max";
				heroLevel.FontSize = 12;
				_xpBarXPTotalLabel.Text = "#Max";
				GUISimpleControlWindow xpBar = _xpBar;
				Vector2 size2 = _xpBar.Size;
				xpBar.SetSize(0f, size2.y);
			}
			else
			{
				heroLevel.Text = string.Empty + heroPersisted.Level;
				heroLevel.FontSize = 13;
				int xpForLevel = XpToLevelDefinition.Instance.GetXpForLevel(heroPersisted.Level);
				int xpForLevel2 = XpToLevelDefinition.Instance.GetXpForLevel(heroPersisted.Level + 1);
				float num = (float)(heroPersisted.Xp - xpForLevel) / (float)(xpForLevel2 - xpForLevel);
				_xpBarXPTotalLabel.Text = string.Empty + (heroPersisted.Xp - xpForLevel) + " / " + (xpForLevel2 - xpForLevel);
				GUISimpleControlWindow xpBar2 = _xpBar;
				float width = (float)_baseXPBarWidth * num;
				Vector2 size3 = _xpBar.Size;
				xpBar2.SetSize(width, size3.y);
			}
		}

		protected void ClampSquadNameplatePosition()
		{
			Vector2 position = SquadName.Position;
			if (position.x < squadMinX)
			{
				Vector2 offset = SquadName.Offset;
				float x = offset.x;
				float num = squadMinX;
				Vector2 position2 = SquadName.Position;
				offset.x = x + (num - position2.x);
				SquadName.Offset = offset;
			}
		}
	}

	public class TargetedHeroButtons : GUISimpleControlWindow
	{
		public class SlidingPanel : GUISimpleControlWindow
		{
			public enum OpenAndCloseEnum
			{
				Nothing,
				Opening,
				Closing,
				Open
			}

			public OpenAndCloseEnum CurrentState;

			public GUIImage heroPortrait;

			public GUILabel heroNameLabel;

			public GUILabel targetHeroLevel;

			public GUIDrawTexture targetHeroLevelBackground;

			public GrowingNameplate SquadName;

			public GUIButton AddFriend;

			public GUIButton BuyHero;

			public GUIButton InviteCardGame;

			public GUIButton ReportPlayer;

			public GUIButton InviteBrawler;

			public GUIButton TheirSquadButton;

			public GUIDrawTexture FriendStatus;

			private TargetedHeroButtons ParentWindow;

			public SlidingPanel(TargetedHeroButtons ParentWindow)
			{
				this.ParentWindow = ParentWindow;
				SetSize(600f, 250f);
				SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(-434f, 0f));
				Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
				IsVisible = false;
				GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(398f, 140f), new Vector2(0f, 0f));
				gUIImage.TextureSource = "gameworld_bundle|mshs_gameworld_HUD_targetmenu_frame";
				Add(gUIImage);
				Add(GUIControl.CreateControlFrameCentered<BlockTestSpot>(new Vector2(400f, 140f), new Vector2(0f, 0f)));
				heroPortrait = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(200f, 218f), new Vector2(130f, 2f));
				Add(heroPortrait);
				heroNameLabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(150f, 30f), new Vector2(39f, -51f));
				heroNameLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 11, GUILabel.GenColor(104, 160, 9), TextAnchor.MiddleLeft);
				Add(heroNameLabel);
				SquadName = new GrowingNameplate("targetname", TextAnchor.LowerRight, "#TT_PLAYER_TARGETING_TARGET_SQUAD_LEVEL");
				SquadName.Offset = new Vector2(147f, -103f);
				SquadName.SecurityLevelPrefix = "#PLAYER_TARGETING_TARGET_SQUAD_LEVEL";
				Add(SquadName);
				AddFriend = GenerateAndAddButton("gameworld_bundle|mshs_gameworld_HUD_addfriend", "#friendlist_addFriend", new Vector2(80f, 80f), new Vector2(-128f, -12f));
				BuyHero = GenerateAndAddButton("gameworld_bundle|mshs_gameworld_HUD_shop", "#TargetingMenu_BuyThisHero", new Vector2(70f, 70f), new Vector2(-95f, 38f));
				BuyHero.ConfigureRequiredContent(new ContentReference(ContentTypeEnum.PriorityGroup, AssetBundleLoader.BundleGroup.Characters));
				InviteCardGame = GenerateAndAddButton("gameworld_bundle|mshs_gameworld_HUD_invitecards", "#TT_FRIENDSLIST_6", new Vector2(66f, 66f), new Vector2(-60f, -10f));
				if (Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.CardGameAllow))
				{
					InviteCardGame.ConfigureRequiredContent(new ContentReference(ContentTypeEnum.PriorityGroup, AssetBundleLoader.BundleGroup.CardGame));
				}
				InviteCardGame.ToolTip.ContextStringLookup[GUIContext.Status.Disabled] = "#TT_FEATURE_OFF";
				ReportPlayer = GenerateAndAddButton("gameworld_bundle|mshs_gameworld_HUD_report", "#TT_FRIENDSLIST_15", new Vector2(80f, 80f), new Vector2(-26f, 36f));
				ReportPlayer.ToolTip.Offset = new Vector2(-40f, 0f);
				InviteBrawler = GenerateAndAddButton("gameworld_bundle|mshs_gameworld_HUD_invitebrawler", "#TT_FRIENDSLIST_3", new Vector2(66f, 66f), new Vector2(13f, -10f));
				InviteBrawler.ConfigureRequiredContent(new ContentReference(ContentTypeEnum.PriorityGroup, AssetBundleLoader.BundleGroup.SpecialMission));
				InviteBrawler.ToolTip.ContextStringLookup[GUIContext.Status.Disabled] = "#TT_FEATURE_OFF";
				TheirSquadButton = GenerateAndAddButton("gameworld_bundle|mshs_gameworld_HUD_squad", "#TT_VIEWTHEIRSQUAD", new Vector2(70f, 70f), new Vector2(38f, 35f));
				TheirSquadButton.ConfigureRequiredContent(new ContentReference(ContentTypeEnum.PriorityGroup, AssetBundleLoader.BundleGroup.All));
				TheirSquadButton.ToolTip.ContextStringLookup[GUIContext.Status.Disabled] = "#TT_FEATURE_OFF";
				TheirSquadButton.ToolTip.Offset = new Vector2(-25f, 0f);
				TheirSquadButton.IsEnabled = true;
				Add(TheirSquadButton);
				Add(GUIControl.CreateControlFrameCentered<BlockTestSpot>(new Vector2(64f, 64f), new Vector2(201f, -51f)));
				GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(64f, 64f), new Vector2(189f, -47f));
				gUIButton.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|mshs_gameworld_HUD_targetmenu_close");
				gUIButton.ToolTip = new NamedToolTipInfo("#TT_COMMON_1");
				gUIButton.HitTestType = HitTestTypeEnum.Circular;
				gUIButton.HitTestSize = new Vector2(0.9f, 0.9f);
				Add(gUIButton);
				gUIButton.Click += delegate
				{
					AppShell.Instance.EventMgr.Fire(this, new SelectedPlayerMessage(-1, string.Empty, string.Empty, null));
				};
				AddTargetButtonsDelegates();
				Add(GUIControl.CreateControlFrameCentered<BlockTestSpot>(new Vector2(32f, 32f), new Vector2(73f, -51f)));
				targetHeroLevelBackground = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(new Vector2(32f, 32f), new Vector2(73f, -51f));
				targetHeroLevelBackground.TextureSource = "gameworld_bundle|mshs_gameworld_HUD_level_background";
				targetHeroLevelBackground.ToolTip = new NamedToolTipInfo("#TT_TARGET_HERO_LEVEL");
				Add(targetHeroLevelBackground);
				targetHeroLevel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(32f, 32f), new Vector2(72f, -51f));
				targetHeroLevel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(255, 255, 255), TextAnchor.MiddleCenter);
				Add(targetHeroLevel);
				FriendStatus = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(new Vector2(64f, 64f), new Vector2(-130f, -9f));
				FriendStatus.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
				FriendStatus.IsVisible = false;
				FriendStatus.HitTestType = HitTestTypeEnum.Circular;
				FriendStatus.BlockTestType = BlockTestTypeEnum.Circular;
				Add(FriendStatus);
			}

			private void AddTargetButtonsDelegates()
			{
				AddFriend.Click += delegate
				{
					if (ParentWindow.SelectedPlayerId != -1)
					{
						if (AppShell.Instance.Profile.AvailableFriends.IsFriendInviteCooldownReady())
						{
							GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, "#FRIEND_REQUEST_CONFIRM", delegate(string Id, GUIDialogWindow.DialogState state)
							{
								if (state == GUIDialogWindow.DialogState.Ok)
								{
									//AppShell.Instance.Profile.AvailableFriends.AddFriend(ParentWindow.SelectedPlayerId);
									AppShell.Instance.Profile.AvailableFriends.AddFriend(ParentWindow.SelectedPlayerName);  // CSP changed from name to ID for testing
								}
							}, ModalLevelEnum.Default);
						}
						else
						{
							AppShell.Instance.Profile.AvailableFriends.FireFriendCooldownIncompleteDialog();
						}
					}
				};
				InviteBrawler.Click += delegate
				{
					if (!Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ParentalMissionsDeny))
					{
						GUIManager.Instance.ShowErrorDialog(SHSErrorCodes.Code.ParentalControlLimit, string.Empty);
					}
					else if (ParentWindow.SelectedPlayerId != -1)
					{
						if (AppShell.Instance.Profile.AvailableFriends.IsPlayerInBlockedList(ParentWindow.SelectedPlayerId))
						{
							SHSFriendsListWindow.UnblockPlayerQuery();
						}
						else
						{
							SHSBrawlerGadget sHSBrawlerGadget = new SHSBrawlerGadget();
							Friend friend = new Friend(ParentWindow.SelectedPlayerName, ParentWindow.SelectedPlayerId, string.Empty, true, false);
							sHSBrawlerGadget.ConfigureOnFriendsInvite(friend);
							GUIManager.Instance.ShowDynamicWindow(sHSBrawlerGadget, ModalLevelEnum.Default);
						}
					}
				};
				BuyHero.Click += delegate
				{
					if (ParentWindow.SelectedPlayerId != -1)
					{
						ShoppingWindow shoppingWindow = new ShoppingWindow(OwnableDefinition.HeroNameToHeroID[ParentWindow.SelectedHeroName]);
						shoppingWindow.launch();
					}
				};
				InviteCardGame.Click += delegate
				{
					if (!Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.CardGameAllow))
					{
						GUIManager.Instance.ShowDialog(typeof(SHSCardGameUnavailableDialogWindow), "#cardgame_not_available_text", new GUIDialogNotificationSink(null), ModalLevelEnum.Default);
					}
					else if (!Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ParentalCardGameDeny))
					{
						GUIManager.Instance.ShowErrorDialog(SHSErrorCodes.Code.ParentalControlLimit, string.Empty);
					}
					else if (ParentWindow.SelectedPlayerId != -1)
					{
						if (ParentWindow.SelectedPlayer == null)
						{
							GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, "The Targeted Player has left the Game World.", (IGUIDialogNotification)null, ModalLevelEnum.Default);
						}
						else if (IsOKToInvite("cardgame"))
						{
							if (AppShell.Instance.Profile.AvailableFriends.IsPlayerInBlockedList(ParentWindow.SelectedPlayerId))
							{
								SHSFriendsListWindow.UnblockPlayerQuery();
							}
							else
							{
								SHSCardGameGadgetWindow sHSCardGameGadgetWindow = new SHSCardGameGadgetWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.Main);
								sHSCardGameGadgetWindow.SetupForCardGameInviter(ParentWindow.SelectedPlayerId, ParentWindow.SelectedPlayerName);
								GUIManager.Instance.ShowDynamicWindow(sHSCardGameGadgetWindow, ModalLevelEnum.Default);
								AddInviteCooldown("cardgame", 30f);
							}
						}
						else
						{
							GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, "Please wait before inviting the same person to a card game", (IGUIDialogNotification)null, ModalLevelEnum.Default);
						}
					}
				};
				ReportPlayer.Click += delegate
				{
					SlidingPanel slidingPanel = this;
					if (ParentWindow.SelectedPlayerId != -1)
					{
						int reportedPlayerId = ParentWindow.SelectedPlayerId;
						string reportedPlayerName = ParentWindow.SelectedPlayerName;
						SHSBlockOrReportPlayerDialogWindow win = null;
						GUIManager.Instance.ShowDialog(typeof(SHSBlockOrReportPlayerDialogWindow), string.Empty, null, new GUIDialogNotificationSink(delegate(string Id, IGUIDialogWindow window)
						{
							win = (SHSBlockOrReportPlayerDialogWindow)window;
						}, delegate
						{
						}, delegate
						{
						}, delegate
						{
						}, delegate(string Id, GUIDialogWindow.DialogState state)
						{
							if (state == GUIDialogWindow.DialogState.Ok)
							{
								if (win.blockButton.IsSelected)
								{
									SHSErrorNotificationWindow.ErrorIconInfo errorIconInfo = SHSErrorNotificationWindow.GetErrorIconInfo(SHSErrorNotificationWindow.ErrorIcons.TooManyFriends);
									GUIDialogNotificationSink notificationSink = new GUIDialogNotificationSink(delegate(string windowId, GUIDialogWindow.DialogState windowState)
									{
										if (windowState == GUIDialogWindow.DialogState.Ok)
										{
											AppShell.Instance.Profile.AvailableFriends.AddBlocked(slidingPanel.ParentWindow.SelectedPlayerId);
										}
									});
									SHSCommonDialogWindow sHSCommonDialogWindow = new SHSCommonDialogWindow(errorIconInfo.IconPath, errorIconInfo.IconSize, "common_bundle|button_close", "common_bundle|L_mshs_button_yes", "common_bundle|L_mshs_button_no", typeof(SHSDialogYesButton), typeof(SHSDialogNoButton), true);
									bool flag = AppShell.Instance.Profile.AvailableFriends.IsPlayerInFriendList(slidingPanel.ParentWindow.SelectedPlayerId);
									sHSCommonDialogWindow.TitleText = ((!flag) ? "#blockconfirm_title" : "#blockconfirm_friend_title");
									sHSCommonDialogWindow.Text = ((!flag) ? "#blockconfirm_text" : "#blockconfirm_friend_text");
									sHSCommonDialogWindow.NotificationSink = notificationSink;
									GUIManager.Instance.ShowDynamicWindow(sHSCommonDialogWindow, ModalLevelEnum.Full);
								}
								else if (win.reportButton.IsSelected)
								{
									slidingPanel.showReportWindow(reportedPlayerId, reportedPlayerName);
								}
							}
						}), ModalLevelEnum.Default);
					}
				};
				TheirSquadButton.Click += delegate
				{
					MySquadDataManager dataManager = new MySquadDataManager(ParentWindow.SelectedPlayerId, ParentWindow.SelectedPlayerName, ParentWindow.SelectedPlayerSquadLevel);
					MySquadWindow dialogWindow = new MySquadWindow(dataManager);
					GUIManager.Instance.ShowDynamicWindow(dialogWindow, ModalLevelEnum.Default);
				};
			}

			private void showReportWindow(int reportedPlayerId, string reportedPlayerName)
			{
				SHSReportPlayerDialogWindow win = null;
				GUIManager.Instance.ShowDialog(typeof(SHSReportPlayerDialogWindow), string.Empty, null, new GUIDialogNotificationSink(delegate(string Id, IGUIDialogWindow window)
				{
					win = (SHSReportPlayerDialogWindow)window;
					win.reportedPlayerName = reportedPlayerName;
				}, delegate
				{
				}, delegate
				{
				}, delegate
				{
				}, delegate
				{
					if (win.reportPlayerConfirmed)
					{
						CommunicationManager.ReportPlayer(reportedPlayerId, reportedPlayerName, win.reportCause, win.reportSubCause, win.reportComments);
						CspUtils.DebugLog("Sending Player Report");
						CspUtils.DebugLog("=====================");
						CspUtils.DebugLog(reportedPlayerId);
						CspUtils.DebugLog(reportedPlayerName);
						CspUtils.DebugLog(win.reportCause);
						CspUtils.DebugLog(win.reportSubCause);
						CspUtils.DebugLog(win.reportComments);
						CspUtils.DebugLog("=====================");
					}
				}), ModalLevelEnum.Default);
			}

			private void AddInviteCooldown(string inviteKey, float cooldownDuration)
			{
				InviteCooldown inviteCooldown = Utils.AddComponent<InviteCooldown>(ParentWindow.SelectedPlayer);
				inviteCooldown.inviteKey = inviteKey;
				inviteCooldown.StartTime = Time.time;
				inviteCooldown.Duration = cooldownDuration;
			}

			private bool IsOKToInvite(string inviteKey)
			{
				InviteCooldown component = Utils.GetComponent<InviteCooldown>(ParentWindow.SelectedPlayer);
				if (component != null)
				{
					if (component.CooldownFinished && (inviteKey == component.inviteKey || inviteKey == null))
					{
						UnityEngine.Object.Destroy(component);
						return true;
					}
					return false;
				}
				return true;
			}

			public GUIButton GenerateAndAddButton(string path, string Tooltip, Vector2 size, Vector2 offset)
			{
				GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(size, offset);
				gUIButton.HitTestType = HitTestTypeEnum.Circular;
				gUIButton.HitTestSize = new Vector2(0.8f, 0.8f);
				gUIButton.StyleInfo = new SHSButtonStyleInfo(path);
				gUIButton.ToolTip = new NamedToolTipInfo(Tooltip);
				Add(gUIButton);
				return gUIButton;
			}
		}

		public class AnimOpenAndClosed : SHSAnimations
		{
			public const float AnimTime = 0.7f;

			public const float BounceTime = 0.3f;

			public static AnimClip Open(SlidingPanel panel)
			{
				return Absolute.OffsetX(GenericPaths.LinearWithBounce(-434f, 0f, 0.7f), panel) ^ Absolute.AnimationAlpha(Path.Linear(panel.AnimationAlpha, 1f, 0.7f), panel) ^ Absolute.SizeXY(Path.Constant(0f, 0.77f) | GenericPaths.LinearWithSingleWiggle(0f, 80f, 0.3f), panel.AddFriend) ^ Absolute.SizeXY(Path.Constant(0f, 0.7f) | GenericPaths.LinearWithSingleWiggle(0f, 70f, 0.3f), panel.BuyHero) ^ Absolute.SizeXY(Path.Constant(0f, 0.63f) | GenericPaths.LinearWithSingleWiggle(0f, 66f, 0.3f), panel.InviteCardGame) ^ Absolute.SizeXY(Path.Constant(0f, 0.56f) | GenericPaths.LinearWithSingleWiggle(0f, 80f, 0.3f), panel.ReportPlayer) ^ Absolute.SizeXY(Path.Constant(0f, 0.48999998f) | GenericPaths.LinearWithSingleWiggle(0f, 66f, 0.3f), panel.InviteBrawler) ^ Absolute.SizeXY(Path.Constant(0f, 0.420000017f) | GenericPaths.LinearWithSingleWiggle(0f, 70f, 0.3f), panel.TheirSquadButton);
			}

			public static AnimClip Close(SlidingPanel panel)
			{
				return Absolute.OffsetX(Path.Linear(0f, -434f, 0.7f), panel) ^ Absolute.AnimationAlpha(Path.Linear(panel.AnimationAlpha, 0f, 0.7f), panel);
			}
		}

		private AnimClip OpenAndClosePiece;

		protected SlidingPanel mainPanel;

		public int SelectedPlayerId;

		public GameObject SelectedPlayer;

		public string SelectedHeroName = string.Empty;

		public string SelectedPlayerName = string.Empty;

		public int SelectedPlayerSquadLevel;

		public TargetedHeroButtons()
		{
			SetSize(600f, 400f);
			SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(35f, 2f));
			mainPanel = new SlidingPanel(this);
			Add(mainPanel);
		}

		public override void OnShow()
		{
			mainPanel.IsVisible = false;
			mainPanel.Offset = new Vector2(-434f, 0f);
			mainPanel.AnimationAlpha = 0f;
			mainPanel.CurrentState = SlidingPanel.OpenAndCloseEnum.Nothing;
			AppShell.Instance.EventMgr.AddListener<SelectedPlayerMessage>(OnHeroTargeted);
			AppShell.Instance.EventMgr.AddListener<FriendListUpdatedMessage>(OnFriendListUpdated);
			AppShell.Instance.EventMgr.AddListener<HeroFetchCompleteMessage>(OnHeroFetchComplete);
			AppShell.Instance.EventMgr.AddListener<PlayerInfoUpdateMessage>(OnHeroTargetedInfoUpdate);
			base.OnShow();
		}

		public override void OnHide()
		{
			AppShell.Instance.EventMgr.RemoveListener<SelectedPlayerMessage>(OnHeroTargeted);
			AppShell.Instance.EventMgr.RemoveListener<FriendListUpdatedMessage>(OnFriendListUpdated);
			AppShell.Instance.EventMgr.RemoveListener<HeroFetchCompleteMessage>(OnHeroFetchComplete);
			base.OnHide();
		}

		private void UpdateBuyHeroButton()
		{
			bool flag = AppShell.Instance.Profile.AvailableCostumes.ContainsKey(SelectedHeroName);
			HeroDefinition heroDef = OwnableDefinition.getHeroDef(SelectedHeroName);
			bool flag2 = true;
			mainPanel.BuyHero.IsEnabled = (!flag && flag2);
			mainPanel.BuyHero.ToolTip = new NamedToolTipInfo(flag ? "#TargetingMenu_AlreadyOwn" : ((!flag2) ? "#TargetingMenu_HeroUnavailable" : "#TargetingMenu_BuyThisHero"));
		}

		private void OnHeroFetchComplete(HeroFetchCompleteMessage message)
		{
			if (message.success)
			{
				UpdateBuyHeroButton();
			}
		}

		public void OnHeroTargetedInfoUpdate(PlayerInfoUpdateMessage message)
		{
			UpdatePanel();
		}

		public void OnHeroTargeted(SelectedPlayerMessage message)
		{
			SelectedPlayerId = message.SelectedPlayerId;
			SelectedPlayer = message.SelectedPlayer;
			SelectedHeroName = message.SelectedHeroName;
			SelectedPlayerName = message.SelectedPlayerName;
			if (mainPanel.IsVisible && message.SelectedPlayerId == -1 && mainPanel.CurrentState != SlidingPanel.OpenAndCloseEnum.Closing)
			{
				ClosePanel();
				return;
			}
			if (message.SelectedPlayerId != -1 && mainPanel.CurrentState != SlidingPanel.OpenAndCloseEnum.Opening && mainPanel.CurrentState != SlidingPanel.OpenAndCloseEnum.Open)
			{
				OpenPanel();
			}
			UpdatePanel();
		}

		public void OnFriendListUpdated(FriendListUpdatedMessage msg)
		{
			UpdatePanel();
		}

		public virtual void UpdatePanel()
		{
			if (SelectedPlayerId != -1 && !(SelectedPlayer == null) && !string.IsNullOrEmpty(SelectedHeroName))
			{
				bool flag = AppShell.Instance.Profile.AvailableFriends.ContainsKey(SelectedPlayerId.ToString());
				Texture2D texture;
				if (GUIManager.Instance.LoadTexture(string.Format("characters_bundle|{0}_targeted_{1}", SelectedHeroName, (!flag) ? "default" : "happy"), out texture))
				{
					mainPanel.heroPortrait.Texture = texture;
				}
				else
				{
					mainPanel.heroPortrait.Texture = null;
				}
				mainPanel.heroNameLabel.Text = AppShell.Instance.CharacterDescriptionManager[SelectedHeroName].CharacterFamily.ToUpperInvariant();
				mainPanel.SquadName.Text = SelectedPlayerName;
				if (flag)
				{
					mainPanel.FriendStatus.IsVisible = true;
					mainPanel.FriendStatus.TextureSource = "gameworld_bundle|mshs_gameworld_HUD_id_friend";
					mainPanel.FriendStatus.ToolTip = new NamedToolTipInfo("#TargetingMenu_AlreadyFriend", new Vector2(-50f, 0f));
					mainPanel.AddFriend.IsVisible = false;
				}
				else if (AppShell.Instance.Profile.AvailableFriends.AvailableBlocked.ContainsKey(SelectedPlayerId.ToString()))
				{
					mainPanel.FriendStatus.IsVisible = true;
					mainPanel.FriendStatus.TextureSource = "gameworld_bundle|mshs_gameworld_HUD_id_blocked";
					mainPanel.FriendStatus.ToolTip = new NamedToolTipInfo("#TargetingMenu_Blocked", new Vector2(-50f, 0f));
					mainPanel.AddFriend.IsVisible = false;
				}
				else
				{
					mainPanel.FriendStatus.IsVisible = false;
					mainPanel.AddFriend.IsVisible = true;
					mainPanel.AddFriend.ToolTip = new NamedToolTipInfo("#friendlist_addFriend");
				}
				int heroLevel;
				SelectedPlayer.GetComponent<SpawnData>().GetHeroLevel(out heroLevel);
				string formattedLevel = XpToLevelDefinition.Instance.GetFormattedLevel(heroLevel);
				mainPanel.targetHeroLevel.Text = formattedLevel;
				SelectedPlayer.GetComponent<SpawnData>().GetSquadLevel(out SelectedPlayerSquadLevel);
				mainPanel.SquadName.SecurityLevel = SelectedPlayerSquadLevel.ToString();
				UpdateBuyHeroButton();
			}
		}

		private void OpenPanel()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Expected O, but got Unknown
			mainPanel.IsVisible = true;
			mainPanel.CurrentState = SlidingPanel.OpenAndCloseEnum.Opening;
			AnimClip animClip = AnimOpenAndClosed.Open(mainPanel);
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				mainPanel.CurrentState = SlidingPanel.OpenAndCloseEnum.Open;
			};
			base.AnimationPieceManager.SwapOut(ref OpenAndClosePiece, animClip);
		}

		private void ClosePanel()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Expected O, but got Unknown
			mainPanel.CurrentState = SlidingPanel.OpenAndCloseEnum.Closing;
			AnimClip animClip = AnimOpenAndClosed.Close(mainPanel);
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				mainPanel.CurrentState = SlidingPanel.OpenAndCloseEnum.Nothing;
				mainPanel.IsVisible = false;
			};
			base.AnimationPieceManager.SwapOut(ref OpenAndClosePiece, animClip);
		}
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

	public static Vector2 PET_WINDOW_OPENED = new Vector2(0f, 190f);

	public static Vector2 PET_WINDOW_CLOSED = new Vector2(-500f, 190f);

	protected GUIButton ChangePet;

	protected ChoosePetHUD petHUD;

	protected ActiveEffectsHUD effectsHUD;

	public SHSSocialCharacterDisplayWindow()
	{
		SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f));
		TargetedHeroButtons targetedHeroButtons = new TargetedHeroButtons();
		Add(targetedHeroButtons);
		MainHeroPortrait control = new MainHeroPortrait();
		Add(control);
		CascadingEmoteChat control2 = new CascadingEmoteChat(targetedHeroButtons);
		Add(control2);
		BlockTestSpot blockTestSpot = GUIControl.CreateControlFrameCentered<BlockTestSpot>(new Vector2(54f, 54f), new Vector2(0f, 140f));
		blockTestSpot.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 140f));
		blockTestSpot.BlockTestType = BlockTestTypeEnum.Circular;
		blockTestSpot.HitTestType = HitTestTypeEnum.Circular;
		Add(blockTestSpot);
		ChangePet = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(54f, 54f), new Vector2(0f, 0f));
		ChangePet.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(25f, 160f));
		ChangePet.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|icon_changesidekick");
		ChangePet.ToolTip = new NamedToolTipInfo("#GAMEWORLD_PICKSIDEKICK_BUTTON");
		ChangePet.HitTestSize = new Vector2(1f, 1f);
		ChangePet.HitTestType = HitTestTypeEnum.Circular;
		ChangePet.BlockTestType = BlockTestTypeEnum.Circular;
		ChangePet.Click += ChangePet_Click;
		Add(ChangePet);
		petHUD = new ChoosePetHUD();
		petHUD.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, PET_WINDOW_CLOSED);
		Add(petHUD);
		petHUD.IsVisible = false;
		effectsHUD = new ActiveEffectsHUD();
		effectsHUD.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, new Vector2(430f, 60f));
		Add(effectsHUD);
		effectsHUD.IsVisible = true;
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
}
