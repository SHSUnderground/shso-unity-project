using CardGame;
using System.Collections.Generic;

public class CardGameLaunchManager
{
	public enum BattleAPlayerOptions
	{
		NoneSelected,
		PlayWithAnyone,
		PlayWithAFriend
	}

	public enum BattlePlayMode
	{
		Quest,
		FriendOrAnyone,
		InvitationRecipient,
		InvitationInitiator,
		DailyQuest
	}

	public enum QuestUserState
	{
		NotOwned,
		Unlocked,
		Locked,
		DailyQuest
	}

	private SHSCardGameGadgetWindow mainWindow;

	public UserProfile Profile;

	public string SelectedHero;

	public DeckProperties SelectedDeck;

	public bool DecksLoaded;

	public BattleAPlayerOptions SelectedFriendsToPlayWithOption;

	public BattlePlayMode CurrentBattlePlayModeOption;

	public string[] SelectedFriendsToPlayWith;

	public int InvitationId;

	public CardQuest SelectedQuest;

	public CardQuestPart.QuestBattle SelectedBattle;

	public CardQuestPart DailyQuestPart;

	public bool ShowOnlyDailyQuest;

	public bool SelectDailyQuest;

	public CardGameLaunchManager(SHSCardGameGadgetWindow mainWindow)
	{
		this.mainWindow = mainWindow;
		if (AppShell.Instance.Profile == null)
		{
			CspUtils.DebugLog("You are trying to run the Card Game Gadget without a User Profile.  This can cause massive instability.  You have been warned.");
			return;
		}
		Profile = AppShell.Instance.Profile;
		SelectedHero = Profile.LastSelectedCostume;
		SelectedDeck = GetLoadingDeck();
		DecksLoaded = false;
		CardCollection.EnumerateDecks();
		CardManager.LoadTextureBundle(true);
		PopulateDailyQuest();
		if (DailyQuestPart != null)
		{
			SelectedQuest = DailyQuestPart.ParentQuest;
			SelectDailyQuest = true;
		}
		else
		{
			if (AppShell.Instance != null && AppShell.Instance.Profile.AvailableQuests.Count > 0)
			{
				SelectedQuest = AppShell.Instance.CardQuestManager.GetQuestByPartId(int.Parse(new List<string>(AppShell.Instance.Profile.AvailableQuests.Keys)[0]));
			}
			SelectDailyQuest = false;
		}
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.DeckListLoaded>(OnDecksLoaded);
	}

	public DeckProperties GetLoadingDeck()
	{
		DeckProperties deckProperties = new DeckProperties();
		deckProperties.DeckName = "Loading...";
		deckProperties.HeroName = Profile.LastSelectedCostume;
		return deckProperties;
	}

	public void OnDecksLoaded(CardGameEvent.DeckListLoaded evt)
	{
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.DeckListLoaded>(OnDecksLoaded);
		DecksLoaded = true;
		DeckProperties selectedDeck = SelectedDeck;
		foreach (KeyValuePair<string, DeckProperties> deck in CardCollection.DeckList)
		{
			if (deck.Value.Legal)
			{
				if (Profile.LastDeckID <= 0 || deck.Value.DeckId == Profile.LastDeckID)
				{
					SelectedDeck = deck.Value;
					return;
				}
				selectedDeck = deck.Value;
			}
		}
		SelectedDeck = selectedDeck;
	}

	public void FriendsToPlayWithSubmitted(string[] friendsToPlayWith)
	{
		if (friendsToPlayWith.Length == 0)
		{
			SelectedFriendsToPlayWithOption = BattleAPlayerOptions.PlayWithAnyone;
			return;
		}
		SelectedFriendsToPlayWithOption = BattleAPlayerOptions.PlayWithAFriend;
		SelectedFriendsToPlayWith = friendsToPlayWith;
	}

	public void PopulateDailyQuest()
	{
		int result;
		if (int.TryParse(BotdDefinition.Instance.BattleOwnableTypeId, out result))
		{
			DailyQuestPart = AppShell.Instance.CardQuestManager.GetQuestPart(result);
			if (DailyQuestPart == null)
			{
				CspUtils.DebugLog("No quest of the day from server, or quest is not listed in the quest list from the server.");
			}
		}
	}

	public QuestUserState GetQuestState(CardQuestPart.QuestBattle questBattle)
	{
		QuestUserState result = QuestUserState.DailyQuest;
		if (questBattle.ParentCardQuestPart == DailyQuestPart && ((questBattle.ParentCardQuestPart.PartType == CardQuestPartsTypeEnum.Easy && questBattle.Stage == 1) || (questBattle.ParentCardQuestPart.PartType == CardQuestPartsTypeEnum.Hard && questBattle.Stage == 4)))
		{
			return result;
		}
		if (!(AppShell.Instance != null) || !AppShell.Instance.Profile.AvailableQuests.ContainsKey(questBattle.ParentCardQuestPart.Id.ToString()))
		{
			result = QuestUserState.NotOwned;
		}
		else
		{
			result = QuestUserState.Unlocked;
			if (!string.IsNullOrEmpty(questBattle.Prereq))
			{
				Dictionary<string, int> dictionary = CardManager.ParseRecipe(questBattle.Prereq);
				{
					foreach (KeyValuePair<string, int> item in dictionary)
					{
						if (!CardCollection.Collection.ContainsKey(item.Key) || dictionary[item.Key] > CardCollection.Collection[item.Key])
						{
							return QuestUserState.Locked;
						}
					}
					return result;
				}
			}
		}
		return result;
	}

	public void GoButtonClicked()
	{
		switch (CurrentBattlePlayModeOption)
		{
		case BattlePlayMode.Quest:
			mainWindow.GoToWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.ChooseAQuest);
			break;
		case BattlePlayMode.DailyQuest:
			mainWindow.GoToWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.ChooseAQuest);
			break;
		case BattlePlayMode.FriendOrAnyone:
			mainWindow.GoToWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.InviteFriends);
			break;
		case BattlePlayMode.InvitationRecipient:
			mainWindow.LaunchManager.StartAcceptInvite();
			break;
		case BattlePlayMode.InvitationInitiator:
			mainWindow.LaunchManager.StartInviteFriends();
			GUIManager.Instance.ShowDynamicWindow(new SHSCardGameAirlockWindow(), GUIControl.ModalLevelEnum.Default);
			break;
		}
	}

	public void StartMission()
	{
		StartInfo startInfo = new StartInfo();
		startInfo.Players[0].DeckRecipe = SelectedDeck.DeckRecipe;
		startInfo.Players[0].Hero = SelectedHero;
		startInfo.Players[1].Hero = SelectedBattle.Enemy;
		startInfo.Players[1].DeckRecipe = SelectedBattle.DeckList;
		startInfo.Players[1].Name = AppShell.Instance.CharacterDescriptionManager[SelectedBattle.Enemy].CharacterName;
		startInfo.QuestKeeper = SelectedQuest.Sponsor;
		startInfo.ArenaName = SelectedBattle.Arena;
		startInfo.ArenaScenario = SelectedBattle.Scenario;
		startInfo.QuestID = SelectedBattle.ParentCardQuestPart.Id;
		startInfo.QuestNodeID = SelectedBattle.Id;
		startInfo.QuestConditions = SelectedBattle.Rules;
		startInfo.TicketsAwarded = SelectedBattle.Tickets;
		startInfo.SilverAwarded = SelectedBattle.Silver;
		startInfo.XPAwarded = SelectedBattle.XP;
		startInfo.RewardCard = SelectedBattle.Reward.Split(':')[0];
		AppShell.Instance.Profile.LastSelectedCostume = SelectedHero;
		AppShell.Instance.Profile.SelectedCostume = SelectedHero;
		AppShell.Instance.Profile.PersistExtendedData();
		AppShell.Instance.SharedHashTable["SocialSpaceCharacterCurrent"] = SelectedHero;
		AppShell.Instance.EventMgr.Fire(this, new CharacterSelectedMessage(SelectedHero));
		AppShell.Instance.SharedHashTable["CardGameLevel"] = startInfo;
		AppShell.Instance.SharedHashTable["CardGameTicket"] = null;
		AppShell.Instance.QueueLocationInfo();
		AppShell.Instance.Matchmaker2.SoloCardGame(SelectedBattle.Scenario, SelectedHero, SelectedDeck.DeckRecipe, SelectedBattle.Id);
		AppShell.Instance.Transition(GameController.ControllerType.CardGame);
	}

	public void StartInviteFriends()
	{
		string randomArena = Util.GetRandomArena();
		AppShell.Instance.Matchmaker2.InviteCardGame(SelectedFriendsToPlayWith, randomArena, SelectedHero, SelectedDeck.DeckRecipe, OnPlayCardGame);
	}

	public void EnterPvPQueue()
	{
		string randomArena = Util.GetRandomArena();
		CspUtils.DebugLog(string.Format("Entering PVP queue with {0}, {1}, {2}", SelectedHero, SelectedDeck.DeckName, randomArena));
		AppShell.Instance.Matchmaker2.EnterPvPQueue(SelectedHero, SelectedDeck.DeckRecipe, randomArena, OnPlayCardGame);
	}

	protected void OnPlayCardGame(Matchmaker2.Ticket ticket)
	{
		if (ticket.status == Matchmaker2.Ticket.Status.SUCCESS)
		{
			AppShell.Instance.Profile.LastSelectedCostume = SelectedHero;
			AppShell.Instance.Profile.SelectedCostume = SelectedHero;
			AppShell.Instance.Profile.PersistExtendedData();
			AppShell.Instance.SharedHashTable["SocialSpaceCharacterCurrent"] = SelectedHero;
			AppShell.Instance.EventMgr.Fire(this, new CharacterSelectedMessage(SelectedHero));
			AppShell.Instance.SharedHashTable["CardGameTicket"] = ticket;
			AppShell.Instance.SharedHashTable["CardGameLevel"] = new StartInfo(ticket);
			AppShell.Instance.QueueLocationInfo();
			AppShell.Instance.Transition(GameController.ControllerType.CardGame);
		}
		else
		{
			CspUtils.DebugLog(ticket.status);
		}
	}

	public void StartAcceptInvite()
	{
		AppShell.Instance.Matchmaker2.AcceptCardGame(InvitationId, SelectedHero, SelectedDeck.DeckRecipe, OnAcceptCardGame);
	}

	protected void OnAcceptCardGame(Matchmaker2.Ticket ticket)
	{
		if (ticket.status == Matchmaker2.Ticket.Status.SUCCESS)
		{
			AppShell.Instance.Profile.LastSelectedCostume = SelectedHero;
			AppShell.Instance.Profile.SelectedCostume = SelectedHero;
			AppShell.Instance.Profile.PersistExtendedData();
			AppShell.Instance.SharedHashTable["SocialSpaceCharacterCurrent"] = SelectedHero;
			AppShell.Instance.EventMgr.Fire(this, new CharacterSelectedMessage(SelectedHero));
			AppShell.Instance.SharedHashTable["CardGameTicket"] = ticket;
			AppShell.Instance.SharedHashTable["CardGameLevel"] = new StartInfo(ticket);
			AppShell.Instance.QueueLocationInfo();
			AppShell.Instance.Transition(GameController.ControllerType.CardGame);
		}
		else
		{
			CspUtils.DebugLog(ticket.status);
		}
	}
}
