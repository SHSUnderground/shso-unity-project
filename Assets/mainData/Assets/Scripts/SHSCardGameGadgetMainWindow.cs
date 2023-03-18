using System.Collections.Generic;
using UnityEngine;

public class SHSCardGameGadgetMainWindow : SHSCardGameGadgetCenterWindowBase
{
	public class GoButtonWindow : SHSGlowOutlineWindow
	{
		private readonly SHSCardGameGadgetWindow mainWindow;

		public GoButtonWindow(SHSCardGameGadgetWindow mainWindow)
			: base(GetGoButtonGlowPath())
		{
			this.mainWindow = mainWindow;
			SetSize(360f, 122f);
			SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
			Offset = new Vector2(-2f, 241f);
			GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(512f, 512f), Vector2.zero);
			gUIButton.StyleInfo = new SHSButtonStyleInfo("cardgamegadget_bundle|L_gadget_button_large_play");
			Add(gUIButton);
			gUIButton.Click += goButton_Click;
		}

		private void goButton_Click(GUIControl sender, GUIClickEvent EventData)
		{
			mainWindow.LaunchManager.GoButtonClicked();
		}

		private static List<Vector2> GetGoButtonGlowPath()
		{
			return SHSGlowOutlineWindow.GetGlowPath(new Vector2(-161f, -28f), new Vector2(-137f, -39f), new Vector2(-89f, -45f), new Vector2(0f, -48f), new Vector2(89f, -45f), new Vector2(137f, -39f), new Vector2(161f, -28f), new Vector2(160f, 0f), new Vector2(145f, 36f), new Vector2(127f, 42f), new Vector2(0f, 46f), new Vector2(-127f, 42f), new Vector2(-145f, 36f), new Vector2(-160f, 0f));
		}

		public override void OnShow()
		{
			base.OnShow();
			Highlight(true);
		}
	}

	public class ChooseOptionsWindow : ChooseWindowBase
	{
		private readonly GUIStrokeTextButton cardquest;

		private readonly GUIStrokeTextButton playsomeone;

		private readonly GUIStrokeTextButton playdaily;

		public ChooseOptionsWindow(SHSCardGameGadgetWindow mainWindow)
			: base(mainWindow)
		{
			Offset = new Vector2(-255f, 0f);
			cardquest = new GUIStrokeTextButton("cardgamegadget_bundle|cardlauncher_button_pickquest", new Vector2(256f, 256f), new Vector2(0f, 0f), "#CGG_PLAYAQUEST", new Vector2(100f, 100f), new Vector2(54f, -3f));
			cardquest.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-13f, -127f), new Vector2(240f, 126f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			cardquest.ButtonLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 30, ColorUtil.FromRGB255(251, 255, 213), ColorUtil.FromRGB255(97, 124, 16), ColorUtil.FromRGB255(71, 118, 5), new Vector2(-3f, 5f), TextAnchor.MiddleLeft);
			cardquest.ButtonLabel.Rotation = -4f;
			cardquest.Click += CardquestClick;
			cardquest.Id = "CardQuestButton";
			Add(cardquest);
			playsomeone = new GUIStrokeTextButton("cardgamegadget_bundle|cardlauncher_button_playfriends", new Vector2(256f, 256f), new Vector2(0f, 0f), "#CGG_PLAYSOMEONE", new Vector2(132f, 100f), new Vector2(49f, -2f));
			playsomeone.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-7f, -26f), new Vector2(240f, 126f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			playsomeone.ButtonLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 27, ColorUtil.FromRGB255(251, 255, 213), ColorUtil.FromRGB255(97, 124, 16), ColorUtil.FromRGB255(71, 118, 5), new Vector2(-3f, 5f), TextAnchor.MiddleLeft);
			playsomeone.ButtonLabel.Rotation = -4f;
			playsomeone.Click += PlaysomeoneClick;
			playsomeone.Id = "PlaySomeoneButton";
			Add(playsomeone);
			playdaily = new GUIStrokeTextButton("cardgamegadget_bundle|cardlauncher_button_playdaily", new Vector2(256f, 256f), new Vector2(0f, 0f), "#CGG_PLAYDAILY", new Vector2(130f, 100f), new Vector2(31f, 4f));
			playdaily.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(3f, 65f), new Vector2(240f, 126f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			playdaily.ButtonLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 27, ColorUtil.FromRGB255(240, 254, 255), ColorUtil.FromRGB255(0, 93, 120), ColorUtil.FromRGB255(21, 106, 121), new Vector2(-3f, 5f), TextAnchor.MiddleLeft);
			playdaily.ButtonLabel.Rotation = -4f;
			playdaily.Click += PlaydailyClick;
			playdaily.Id = "PlayDailyButton";
			Add(playdaily);
		}

		public override void OnShow()
		{
			playdaily.IsEnabled = (mainWindow.LaunchManager.DailyQuestPart != null);
			SetupSelected();
			base.OnShow();
		}

		private void SetupSelected()
		{
			switch (mainWindow.LaunchManager.CurrentBattlePlayModeOption)
			{
			case CardGameLaunchManager.BattlePlayMode.Quest:
				CardquestClick(cardquest, null);
				break;
			case CardGameLaunchManager.BattlePlayMode.FriendOrAnyone:
				PlaysomeoneClick(playsomeone, null);
				break;
			case CardGameLaunchManager.BattlePlayMode.InvitationInitiator:
				PlaytargetedClick(null, null);
				break;
			case CardGameLaunchManager.BattlePlayMode.InvitationRecipient:
				SetupInvitee();
				break;
			case CardGameLaunchManager.BattlePlayMode.DailyQuest:
				PlaydailyClick(playdaily, null);
				break;
			}
		}

		private void SetupInvitee()
		{
			cardquest.IsEnabled = false;
			playsomeone.IsEnabled = false;
			playdaily.IsEnabled = false;
		}

		private void CardquestClick(GUIControl sender, GUIClickEvent EventData)
		{
			mainWindow.LaunchManager.CurrentBattlePlayModeOption = CardGameLaunchManager.BattlePlayMode.Quest;
			mainWindow.LaunchManager.ShowOnlyDailyQuest = false;
			mainWindow.LaunchManager.SelectedFriendsToPlayWithOption = CardGameLaunchManager.BattleAPlayerOptions.NoneSelected;
			DisableAllBut(sender);
		}

		private void DisableAllBut(GUIControl button)
		{
			if (button == null)
			{
				cardquest.Alpha = Alpha;
				cardquest.Button.IsSelected = false;
				playsomeone.Alpha = Alpha;
				playsomeone.Button.IsSelected = false;
				playdaily.Alpha = Alpha;
				playdaily.Button.IsSelected = false;
			}
			else
			{
				cardquest.Alpha = ((cardquest == button) ? 1f : 0.65f) * Alpha;
				cardquest.Button.IsSelected = (cardquest == button);
				playsomeone.Alpha = ((playsomeone == button) ? 1f : 0.65f) * Alpha;
				playsomeone.Button.IsSelected = (playsomeone == button);
				playdaily.Alpha = ((playdaily == button) ? 1f : 0.65f) * Alpha;
				playdaily.Button.IsSelected = (playdaily == button);
			}
		}

		private void DisableAll()
		{
			cardquest.Alpha = 0.65f * Alpha;
			cardquest.Button.IsSelected = true;
			cardquest.Button.IsEnabled = false;
			playsomeone.Alpha = 0.65f;
			playsomeone.Button.IsSelected = true;
			playsomeone.Button.IsEnabled = false;
			playdaily.Alpha = 0.65f;
			playdaily.Button.IsSelected = true;
			playdaily.Button.IsEnabled = false;
		}

		private void PlaysomeoneClick(GUIControl sender, GUIClickEvent EventData)
		{
			mainWindow.LaunchManager.CurrentBattlePlayModeOption = CardGameLaunchManager.BattlePlayMode.FriendOrAnyone;
			DisableAllBut(sender);
		}

		private void PlaytargetedClick(GUIControl sender, GUIClickEvent EventData)
		{
			mainWindow.LaunchManager.CurrentBattlePlayModeOption = CardGameLaunchManager.BattlePlayMode.InvitationInitiator;
			DisableAll();
		}

		private void PlaydailyClick(GUIControl sender, GUIClickEvent EventData)
		{
			mainWindow.LaunchManager.CurrentBattlePlayModeOption = CardGameLaunchManager.BattlePlayMode.DailyQuest;
			mainWindow.LaunchManager.ShowOnlyDailyQuest = true;
			mainWindow.LaunchManager.SelectedFriendsToPlayWithOption = CardGameLaunchManager.BattleAPlayerOptions.NoneSelected;
			mainWindow.LaunchManager.SelectedQuest = mainWindow.LaunchManager.DailyQuestPart.ParentQuest;
			DisableAllBut(sender);
		}
	}

	public class ChooseWindowBase : GUISimpleControlWindow
	{
		protected SHSCardGameGadgetWindow mainWindow;

		public ChooseWindowBase(SHSCardGameGadgetWindow mainWindow)
		{
			this.mainWindow = mainWindow;
			SetSize(new Vector2(400f, 400f));
			SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
		}
	}

	public class ChooseHeroWindow : ChooseWindowBase
	{
		private readonly GUIButton chooseHeroButton;

		private readonly GUIDrawTexture HeroPortrait;

		private readonly GUIImage chooseHeroTitle;

		private GUIStrokeTextLabel HeroName;

		public ChooseHeroWindow(SHSCardGameGadgetWindow cardGadget)
			: base(cardGadget)
		{
			mainWindow = cardGadget;
			Offset = new Vector2(15f, -27f);
			HeroPortrait = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(new Vector2(183f, 183f), new Vector2(-16f, -21f));
			Add(HeroPortrait);
			chooseHeroButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(64f, 64f), new Vector2(-115f, -130f));
			chooseHeroButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|changehero_icon");
			chooseHeroButton.Id = "chooseHeroButton";
			Add(chooseHeroButton);
			chooseHeroTitle = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(144f, 30f), new Vector2(0f, -130f));
			chooseHeroTitle.TextureSource = "cardgamegadget_bundle|L_cardlauncher_subtitle_pickahero";
			Add(chooseHeroTitle);
			HeroName = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(160f, 90f), new Vector2(-21f, 89f));
			HeroName.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 24, ColorUtil.FromRGB255(255, 255, 255), ColorUtil.FromRGB255(0, 102, 254), ColorUtil.FromRGB255(0, 8, 45), new Vector2(-2f, -2f), TextAnchor.MiddleCenter);
			HeroName.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Ignore;
			HeroName.Rotation = 1f;
			Add(HeroName);
			AddDelegates(cardGadget);
		}

		public override void OnShow()
		{
			base.OnShow();
			UpdateSelectedHero(mainWindow.LaunchManager.SelectedHero);
		}

		private void UpdateSelectedHero(string heroName)
		{
			HeroPortrait.IsVisible = !string.IsNullOrEmpty(heroName);
			if (HeroPortrait.IsVisible)
			{
				HeroPortrait.Alpha = 1f;
				HeroPortrait.TextureSource = "characters_bundle|expandedtooltip_render_" + heroName;
				HeroName.Text = AppShell.Instance.CharacterDescriptionManager[heroName].CharacterName;
			}
		}

		public void AddDelegates(SHSCardGameGadgetWindow brawlerGadget)
		{
			HeroPortrait.Click += delegate(GUIControl sender, GUIClickEvent eventData)
			{
				chooseHeroButton.FireMouseClick(eventData);
			};
			chooseHeroButton.Click += delegate
			{
				ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("large_click_down"));
				mainWindow.GoToWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.ChooseAHero);
			};
		}
	}

	public class ChooseDeckWindow : ChooseWindowBase
	{
		private readonly GUIButton chooseDeckButton;

		private readonly GUIImage chooseDeckTitle;

		private readonly GUIDrawTexture DeckImage;

		private readonly GUIStrokeTextLabel DeckName;

		public ChooseDeckWindow(SHSCardGameGadgetWindow cardGadget)
			: base(cardGadget)
		{
			mainWindow = cardGadget;
			Offset = new Vector2(263f, 0f);
			Rotation = 3f;
			chooseDeckTitle = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(141f, 43f), new Vector2(34f, -143f));
			chooseDeckTitle.TextureSource = "cardgamegadget_bundle|L_cardlauncher_subtitle_pickaDeck";
			Add(chooseDeckTitle);
			chooseDeckButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(64f, 64f), new Vector2(-87f, -148f));
			chooseDeckButton.StyleInfo = new SHSButtonStyleInfo("cardgamegadget_bundle|changeDeck_icon");
			chooseDeckButton.Id = "chooseDeckButton";
			Add(chooseDeckButton);
			DeckImage = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(new Vector2(210f, 169f), new Vector2(0f, -33f));
			DeckImage.TextureSource = "cardgamegadget_bundle|cardlauncher_yourdeck_graphic";
			Add(DeckImage);
			DeckName = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(150f, 60f), new Vector2(-14f, 80f));
			DeckName.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 24, ColorUtil.FromRGB255(255, 255, 255), ColorUtil.FromRGB255(0, 102, 254), ColorUtil.FromRGB255(0, 8, 45), new Vector2(-2f, -2f), TextAnchor.MiddleCenter);
			DeckName.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Ignore;
			DeckName.Rotation = 1f;
			Add(DeckName);
			AddDelegates(cardGadget);
		}

		public override void OnShow()
		{
			base.OnShow();
			DeckImage.Alpha = 1f;
			if (mainWindow.LaunchManager.SelectedDeck != null)
			{
				DeckName.Text = mainWindow.LaunchManager.SelectedDeck.DeckName;
			}
			else
			{
				DeckName.Text = string.Empty;
			}
		}

		public void AddDelegates(SHSCardGameGadgetWindow cardgameGadget)
		{
			DeckImage.Click += delegate(GUIControl sender, GUIClickEvent eventData)
			{
				chooseDeckButton.FireMouseClick(eventData);
			};
			chooseDeckButton.Click += delegate
			{
				ChooseDeckWindow chooseDeckWindow = this;
				ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("large_click_down"));
				SHSCardGameLoadSaveDialog windowRef = null;
				GUIManager.Instance.ShowDialog(typeof(SHSCardGameLoadSaveDialog), string.Empty, new GUIDialogNotificationSink(delegate(string id, IGUIDialogWindow window)
				{
					windowRef = (window as SHSCardGameLoadSaveDialog);
					windowRef.CurrentMode = SHSCardGameLoadSaveDialog.DialogMode.Load;
					windowRef.LegalDecksOnly = true;
				}, delegate
				{
				}, delegate
				{
				}, delegate
				{
				}, delegate(string id, GUIDialogWindow.DialogState state)
				{
					if (state != GUIDialogWindow.DialogState.Cancel)
					{
						chooseDeckWindow.mainWindow.LaunchManager.SelectedDeck = windowRef.SelectedDeck;
						AppShell.Instance.Profile.LastDeckID = windowRef.SelectedDeck.DeckId;
						AppShell.Instance.Profile.PersistExtendedData();
						chooseDeckWindow.DeckName.Text = chooseDeckWindow.mainWindow.LaunchManager.SelectedDeck.DeckName;
					}
				}), ModalLevelEnum.Default);
			};
		}
	}

	public ChooseOptionsWindow chooseOptionsWindow;

	public SHSCardGameGadgetMainWindow(SHSCardGameGadgetWindow mainWindow)
		: base(mainWindow)
	{
		GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(826f, 454f), new Vector2(0f, 10f));
		gUIImage.TextureSource = "cardgamegadget_bundle|cardlauncher_mainscreen_backdrop";
		Add(gUIImage);
		chooseOptionsWindow = new ChooseOptionsWindow(mainWindow);
		Add(chooseOptionsWindow);
		ChooseHeroWindow control = new ChooseHeroWindow(mainWindow);
		Add(control);
		ChooseDeckWindow control2 = new ChooseDeckWindow(mainWindow);
		Add(control2);
		GoButtonWindow control3 = new GoButtonWindow(mainWindow);
		Add(control3);
		GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(-227f, 253f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_brawler_airlock_backbutton");
		gUIButton.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton.Click += delegate
		{
			mainWindow.GoToWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.ScreenWithBigPlayButtonThatQuoteEverybodyQuoteLoves);
		};
		Add(gUIButton);
		GUIButton gUIButton2 = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(224f, 248f));
		gUIButton2.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_brawler_airlock_quitbutton");
		gUIButton2.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton2.Click += delegate
		{
			mainWindow.CloseButton.FireMouseClick(null);
		};
		Add(gUIButton2);
	}
}
