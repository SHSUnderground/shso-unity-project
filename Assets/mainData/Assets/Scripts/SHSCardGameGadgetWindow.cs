using System.Collections.Generic;

public class SHSCardGameGadgetWindow : SHSGadget
{
	public enum CardGameWindowTypeEnum
	{
		ScreenWithBigPlayButtonThatQuoteEverybodyQuoteLoves,
		Main,
		ChooseAQuest,
		ChooseAHero,
		ChooseADeck,
		InviteFriends
	}

	private GadgetTopWindow topWindow;

	private GadgetCenterWindow mainWindow;

	private readonly SHSCardGameTopWindows.CardGameTopWindow IntroTopWindow;

	private readonly SHSCardGameTopWindows.CardGameTopWindow ChooseADeckTopWindow;

	private readonly SHSCardGameTopWindows.CardGameTopWindow ChooseAHeroTopWindow;

	private readonly SHSCardGameTopWindows.CardGameTopWindow ChooseAQuestTopWindow;

	private readonly SHSCardGameTopWindows.CardGameTopWindow MainTopWindow;

	private readonly SHSCardGameTopWindows.CardGameTopWindow FriendsTopWindow;

	private readonly SHSCardGameTopWindows.CardGameTopWindow LaunchTopWindow;

	private readonly SHSCardGameInitialSelectionWindow IntroWindow;

	private readonly SHSCardGameGadgetMainWindow MainWindow;

	private readonly SHSCardGameGadgetPickADeckWindow ChooseADeckWindow;

	private readonly SHSCardGameGadgetPickAHeroWindow ChooseAHeroWindow;

	private readonly SHSCardGameGadgetPickAQuestWindow ChooseAQuestWindow;

	private readonly SHSCardGameGadgetFriendsWindow FriendsWindow;

	public CardGameLaunchManager LaunchManager;

	public bool WindowsVisible
	{
		set
		{
			mainWindow.IsVisible = value;
			topWindow.IsVisible = value;
			Background.IsVisible = value;
			CloseButton.IsVisible = value;
		}
	}

	public SHSCardGameGadgetWindow()
		: this(CardGameWindowTypeEnum.ScreenWithBigPlayButtonThatQuoteEverybodyQuoteLoves)
	{
	}

	public SHSCardGameGadgetWindow(CardGameWindowTypeEnum initialWindow)
	{
		foreach (IGUIContainer root in GUIManager.Instance.Roots)
		{
			List<SHSCardGameGadgetWindow> controlsOfType = root.GetControlsOfType<SHSCardGameGadgetWindow>(true);
			foreach (SHSCardGameGadgetWindow item in controlsOfType)
			{
				CspUtils.DebugLog("Closing existing gadget " + item.Id);
				item.CloseGadget();
			}
		}
		PlayerStatus.SetLocalStatus(PlayerStatusDefinition.Instance.GetStatus("CardGame"));
		LaunchManager = new CardGameLaunchManager(this);
		IntroTopWindow = new SHSCardGameTopWindows.CardGameTopWindow(298f, 48f, "cardgamegadget_bundle|L_cardlauncher_title_cardgamelaunch");
		MainTopWindow = new SHSCardGameTopWindows.CardGameTopWindow(280f, 46f, "cardgamegadget_bundle|L_cardlauncher_title_playacardgame");
		ChooseADeckTopWindow = new SHSCardGameTopWindows.CardGameTopWindow(195f, 43f, "cardgamegadget_bundle|L_cardlauncher_title_pickadeck");
		ChooseAQuestTopWindow = new SHSCardGameTopWindows.CardGameTopWindow(229f, 44f, "cardgamegadget_bundle|L_cardlauncher_title_choosequest");
		ChooseAHeroTopWindow = new SHSCardGameTopWindows.CardGameTopWindow(246f, 45f, "cardgamegadget_bundle|L_cardlauncher_subtitle_pickahero");
		FriendsTopWindow = new SHSCardGameTopWindows.CardGameTopWindow(241f, 44f, "persistent_bundle|L_title_invitefriends");
		IntroWindow = new SHSCardGameInitialSelectionWindow(this);
		MainWindow = new SHSCardGameGadgetMainWindow(this);
		ChooseADeckWindow = new SHSCardGameGadgetPickADeckWindow(this);
		ChooseAHeroWindow = new SHSCardGameGadgetPickAHeroWindow(this);
		ChooseAQuestWindow = new SHSCardGameGadgetPickAQuestWindow(this);
		FriendsWindow = new SHSCardGameGadgetFriendsWindow(this);
		GoToWindow(initialWindow, true);
	}

	public override void OnHide()
	{
		base.OnHide();
		if (PlayerStatus.GetLocalStatus() == PlayerStatusDefinition.Instance.GetStatus("CardGame"))
		{
			PlayerStatus.ClearLocalStatus();
		}
	}

	public void GoToWindow(CardGameWindowTypeEnum windowType)
	{
		GoToWindow(windowType, false);
	}

	public void GoToWindow(CardGameWindowTypeEnum windowType, bool initialSetup)
	{
		string backgroundImage;
		switch (windowType)
		{
		case CardGameWindowTypeEnum.Main:
			topWindow = MainTopWindow;
			mainWindow = MainWindow;
			backgroundImage = "persistent_bundle|gadget_mainwindow_frame";
			break;
		case CardGameWindowTypeEnum.ChooseADeck:
			mainWindow = ChooseADeckWindow;
			topWindow = ChooseADeckTopWindow;
			backgroundImage = "persistent_bundle|gadget_mainwindow_frame";
			break;
		case CardGameWindowTypeEnum.ChooseAQuest:
			mainWindow = ChooseAQuestWindow;
			topWindow = ChooseAQuestTopWindow;
			backgroundImage = "cardgamegadget_bundle|cardlauncher_quests_frame";
			break;
		case CardGameWindowTypeEnum.ChooseAHero:
			mainWindow = ChooseAHeroWindow;
			topWindow = ChooseAHeroTopWindow;
			backgroundImage = "cardgamegadget_bundle|cardlauncher_quests_frame";
			break;
		case CardGameWindowTypeEnum.InviteFriends:
			mainWindow = FriendsWindow;
			topWindow = FriendsTopWindow;
			backgroundImage = "persistent_bundle|gadget_base_one_panel";
			break;
		case CardGameWindowTypeEnum.ScreenWithBigPlayButtonThatQuoteEverybodyQuoteLoves:
			mainWindow = IntroWindow;
			topWindow = IntroTopWindow;
			backgroundImage = "persistent_bundle|gadget_base_one_panel";
			break;
		default:
			mainWindow = IntroWindow;
			topWindow = IntroTopWindow;
			backgroundImage = string.Empty;
			break;
		}
		SetCenterWindow(mainWindow);
		SetTopWindow(topWindow);
		SetBackgroundImage(backgroundImage);
		if (initialSetup)
		{
			SetupOpeningTopWindow(topWindow);
			SetupOpeningWindow(BackgroundType.OnePanel, mainWindow);
		}
	}

	public void SetupForCardGameInviter(int friendId, string friendName)
	{
		SHSCardGameGadgetFriendsWindow.FriendItem friendItem = FriendsWindow.FriendsWindow.items.Find(delegate(SHSCardGameGadgetFriendsWindow.FriendItem fi)
		{
			return fi.friend.Id == friendId;
		});
		if (friendItem == null)
		{
			Friend friend = new Friend(friendName, friendId, string.Empty, true, true);
			friendItem = new SHSCardGameGadgetFriendsWindow.FriendItem(friend, false, FriendsWindow);
			FriendsWindow.FriendsWindow.AddItem(friendItem);
		}
		friendItem.selected = true;
		FriendsWindow.FriendClickedOn(friendItem.friend);
		LaunchManager.FriendsToPlayWithSubmitted(new string[1]
		{
			friendId.ToString()
		});
		LaunchManager.CurrentBattlePlayModeOption = CardGameLaunchManager.BattlePlayMode.InvitationInitiator;
	}

	public void SetupForCardGameInvitee(int InvitationId)
	{
		LaunchManager.InvitationId = InvitationId;
		LaunchManager.CurrentBattlePlayModeOption = CardGameLaunchManager.BattlePlayMode.InvitationRecipient;
	}
}
