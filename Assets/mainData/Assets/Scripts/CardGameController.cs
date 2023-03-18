using CardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEngine;

[AddComponentMenu("CardGame/Controller")]
public class CardGameController : GameController
{
	protected static CardGameController instance = null;

	public int powerLevel = -1;

	public int winner = -1;

	public CardGameAudioManager AudioManager;

	public CardGameAnimationQueue animQ;

	public CardGameHud Hud;

	public CardGamePlayer[] players = new CardGamePlayer[2];

	protected AssetBundle cardgameBundle;

	public string fullCardPrefabName;

	public string centerPanelPrefabName;

	public string coinPanelPrefabName;

	public string powerPanelPrefabName;

	public string playerHudPrefabName;

	public string opponentHudPrefabName;

	public GameObject fullCardPrefab;

	public GameObject centerPanelPrefab;

	public GameObject coinPanelPrefab;

	public GameObject powerPanelPrefab;

	public GameObject playerHudPrefab;

	public GameObject opponentHudPrefab;

	public GameObject prefabPlayerBillboard;

	public bool InitializationComplete;

	private TransactionMonitor heroLoaderTransaction;

	protected bool doneLoadingBundle;

	protected Matchmaker2.Ticket ticket;

	private BattleCard fullCardDetailsSource;

	private Vector3 fullCardDetailsSourceScale;

	private Vector3 fullCardDetailsSourcePos;

	private HashSet<string> preloadedHeroes = new HashSet<string>();

	public GameObject playerTurnIndicatorPrefab;

	public GameObject spawnEffectPrefab;

	private bool goGoGadgetJeopardyMeter_;

	public bool goGoGadgetJeopardyMeter;

	private HairTrafficController[] HairTrafficControllers = new HairTrafficController[2];

	private bool[] HairTrafficControllerVisibility = new bool[2];

	private bool gameStarted;

	private bool gameCompleted;

	private SquadBattleCharacterController characterController;

	private AnimClipManager AnimationPieceManager = new AnimClipManager();

	public static Dictionary<string, bool> EffectBlacklist;
	public BattleCard playerPickedCard = null;  // added by CSP
	public BattleCard aiPickedCard = null;  // added by CSP
	BattleCard offensePickedCard = null;  // added by CSP
	BattleCard defensePickedCard = null;  // added by CSP
	public int offenseNdx = 1; // added by CSP
	public int defenseNdx = 0; // added by CSP
	public int removeFromHand = 0; // added by CSP

	public static CardGameController Instance
	{
		get
		{
			return instance;
		}
	}

	public AssetBundle CardGameBundle
	{
		get
		{
			return cardgameBundle;
		}
	}

	public bool GameStarted
	{
		get
		{
			return gameStarted;
		}
		set
		{
			gameStarted = value;
		}
	}

	public bool GameCompleted
	{
		get
		{
			return gameCompleted;
		}
		set
		{
			gameCompleted = value;
		}
	}

	static CardGameController()
	{
		Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
		dictionary.Add("R_buff_defense_sequence", true);
		dictionary.Add("R_buff_regen_sequence", true);
		dictionary.Add("R_buff_speed_sequence", true);
		dictionary.Add("R_buff_strength_sequence", true);
		dictionary.Add("R_buff_super_sequence", true);
		dictionary.Add("R_buff_defense_particles", true);
		dictionary.Add("R_buff_regen_particles", true);
		dictionary.Add("R_buff_speed_particles", true);
		dictionary.Add("R_buff_strength_particles", true);
		dictionary.Add("R_buff_super_particles", true);
		EffectBlacklist = dictionary;
	}

	public override void Awake()
	{
		base.Awake();
		instance = this;
		bCallControllerReadyFromStart = false;
		Hud = base.transform.GetComponentInChildren<CardGameHud>();
		CspUtils.DebugLog("Initializing");
		StartCoroutine(Initialize());
		RegisterDebugEvents();
	}

	public void Update()
	{
		if (goGoGadgetJeopardyMeter != goGoGadgetJeopardyMeter_)
		{
			goGoGadgetJeopardyMeter_ = goGoGadgetJeopardyMeter;
			if (goGoGadgetJeopardyMeter_)
			{
				players[0].JeopardyMeterScript.Grow(12, 1, BattleCard.Factor.Animal);
				players[1].JeopardyMeterScript.Grow(12, 1, BattleCard.Factor.Animal);
			}
			else
			{
				players[0].JeopardyMeterScript.Collapse();
				players[1].JeopardyMeterScript.Collapse();
			}
		}
		AnimationPieceManager.Update(Time.deltaTime);
	}

	public override void OnEnable()
	{
		base.OnEnable();
		instance = this;
		AppShell.Instance.EventMgr.AddListener<CardGameFlipCard>(CardFlip);
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.SlowMotion>(OnSloMo);
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.NoMoSloMo>(OnCancelSloMo);
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.CombatFinished>(OnAttackFinished);
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.AvatarSpawned>(OnAvatarSpawned);
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.ShowVersusIcon>(OnShowVersus);
	}

	public override void OnDisable()
	{
		base.OnDisable();
		instance = null;
		AppShell.Instance.EventMgr.RemoveListener<CardGameFlipCard>(CardFlip);
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.SlowMotion>(OnSloMo);
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.NoMoSloMo>(OnCancelSloMo);
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.CombatFinished>(OnAttackFinished);
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.AvatarSpawned>(OnAvatarSpawned);
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.ShowVersusIcon>(OnShowVersus);
		UnregisterDebugEvents();
	}

	public override void OnOldControllerUnloading(AppShell.GameControllerTypeData currentGameData, AppShell.GameControllerTypeData newGameData)
	{
		base.OnOldControllerUnloading(currentGameData, newGameData);
		RoomAgent.Disconnect();
		Time.timeScale = 1f;
	}

	private IEnumerator Initialize()
	{
		StartTransaction = AppShell.Instance.TransitionHandler.CurrentTransactionContext.Transaction;
		StartTransaction.onComplete += delegate
		{
			InitializationComplete = true;
		};
		TransactionMonitor[] deckLoaderTransaction = new TransactionMonitor[2]
		{
			TransactionMonitor.CreateTransactionMonitor(OnRecipeLoadComplete, 30f, 0),
			TransactionMonitor.CreateTransactionMonitor(OnRecipeLoadComplete, 30f, 0)
		};
		deckLoaderTransaction[0].Id = "Load Player 1 Deck";
		deckLoaderTransaction[1].Id = "Load Player 2 Deck";
		StartTransaction.AddChild(deckLoaderTransaction[0]);
		StartTransaction.AddChild(deckLoaderTransaction[1]);
		StartTransaction.AddStep("loadedDeck", "Card Assets Loaded");
		heroLoaderTransaction = TransactionMonitor.CreateTransactionMonitor("Hero Load Transaction", OnHeroLoadComplete, 60f, null);
		heroLoaderTransaction.Id = "heroLoaderTransaction";
		StartTransaction.AddChild(heroLoaderTransaction);
		StartTransaction.AddStep("loadedHeroAssets", "Character Assets Loaded");
		StartTransaction.AddStep("spawnedAvatars", "Characters Created");
		StartTransaction.AddStep("camera", "Camera On");
		StartTransaction.AddStep("loadtoolboxUIBundle", "Extending UI conflagration");
		StartTransaction.AddStep("onReady", "Ready");
		//ShsAudioSourceList.GetList("CardGameResultsScreen").PreloadAll(StartTransaction);  // commented out by CSP
		StartInfo start = AppShell.Instance.SharedHashTable["CardGameLevel"] as StartInfo;
		if (start == null)
		{
			Shutdown("Card game did not receive start info");
			yield break;
		}
		while (AppShell.Instance.SharedHashTable["CardGameTicket"] == null)
		{
			yield return new WaitForEndOfFrame();
		}
		ticket = (Matchmaker2.Ticket)AppShell.Instance.SharedHashTable["CardGameTicket"];
		AppShell.Instance.SharedHashTable["CardGameTicket"] = null;
		StartTransaction.CompleteStep("ticket");
		if (AppShell.Instance.SharedHashTable.ContainsKey("OverrideDeck0"))
		{
			start.Players[0].DeckRecipe = AppShell.Instance.SharedHashTable["OverrideDeck0"].ToString();
		}
		if (AppShell.Instance.SharedHashTable.ContainsKey("OverrideDeck1"))
		{
			start.Players[1].DeckRecipe = AppShell.Instance.SharedHashTable["OverrideDeck1"].ToString();
		}
		if (AppShell.Instance.SharedHashTable.ContainsKey("OverrideCGArena"))
		{
			start.ArenaName = AppShell.Instance.SharedHashTable["OverrideCGArena"].ToString();
		}
		if (AppShell.Instance.SharedHashTable.ContainsKey("OverrideAvatar0"))
		{
			start.Players[0].Hero = AppShell.Instance.SharedHashTable["OverrideAvatar0"].ToString();
		}
		if (AppShell.Instance.SharedHashTable.ContainsKey("OverrideAvatar1"))
		{
			start.Players[1].Hero = AppShell.Instance.SharedHashTable["OverrideAvatar1"].ToString();
		}
		StartTransaction.CompleteStep("playerdata");
		GameObject go = new GameObject("AnimationQueue");
		animQ = Utils.AddComponent<CardGameAnimationQueue>(go);
		Utils.AttachGameObject(base.gameObject, go);
		gameStarted = false;
		doneLoadingBundle = false;
		fullCardPrefab = null;
		centerPanelPrefab = null;
		coinPanelPrefab = null;
		powerPanelPrefab = null;
		playerHudPrefab = null;
		opponentHudPrefab = null;
		AudioManager = new CardGameAudioManager();
		doneLoadingBundle = false;
		GUIManager.Instance.BundleManager.LoadBundle("gameworld_bundle", OnGameWorldBundleLoaded, null);
		while (!doneLoadingBundle)
		{
			yield return 0;
		}
		AppShell.Instance.BundleLoader.FetchAssetBundle("CardGame/cardgame_common", OnCardGameCommonLoaded, null, true);
		while (!doneLoadingBundle)
		{
			yield return 0;
		}
		doneLoadingBundle = false;
		AppShell.Instance.BundleLoader.FetchAssetBundle("Audio/cardgame_audio", delegate
		{
			doneLoadingBundle = true;
		}, null, true);
		while (!doneLoadingBundle)
		{
			yield return 0;
		}
		StartTransaction.CompleteStep("bundle");
		CardManager.LoadTextureBundle(false);
		while (!CardManager.TextureBundleLoaded)
		{
			yield return new WaitForEndOfFrame();
		}
		StartTransaction.CompleteStep("textureBundle");
		CardManager.LoadCardData();
		while (!CardManager.CardDataLoaded)
		{
			yield return new WaitForEndOfFrame();
		}
		StartTransaction.CompleteStep("cardData");
		players = CreatePlayers(start.Players);
		AppShell.Instance.SharedHashTable["GUIHudPlaySolo"] = (start.Players[1].Type == PlayerType.AI);
		StartTransaction.CompleteStep("createPlayers");
		CspUtils.DebugLog("Players created: " + players[0].Hero + " vs " + players[1].Hero);
		MultiCoroutine startupHandler = new MultiCoroutine(LoadArena(start), ConnectToRoom(start));
		yield return StartCoroutine(startupHandler);
		try
		{
			startupHandler.Throw();
		}
		catch (Exception ex2)
		{
			Shutdown(ex2.Message);
			yield break;
		}
		MultiCoroutine deckLoader = new MultiCoroutine();
		players[0].deckLoaderTransaction = deckLoaderTransaction[0];
		players[1].deckLoaderTransaction = deckLoaderTransaction[1];
		CardGamePlayer[] array = players;
		foreach (CardGamePlayer p in array)
		{
			CspUtils.DebugLog("p.Info.Type=" + p.Info.Type);
			if (p.Info.Type != PlayerType.Network)
			{
				deckLoader.Add(p.LoadDeck());
			}
			else
			{
				StartTransaction.CompleteChildTransaction(p.deckLoaderTransaction);
			}
		}
		yield return StartCoroutine(deckLoader);
		try
		{
			deckLoader.Throw();
		}
		catch (Exception ex)
		{
			Shutdown(ex.Message);
			yield break;
		}
		StartTransaction.CompleteStep("loadedDeck");
		for (int i = 0; i < 5; i++)
		{
			characterController = SquadBattleCharacterController.Instance;
			if (characterController != null)
			{
				break;
			}
			yield return new WaitForEndOfFrame();
		}
		if (characterController == null)
		{
			Shutdown("Card game could not find the character controller.");
			yield break;
		}
		HashSet<string> heroSet = new HashSet<string>();
		CardGamePlayer[] array2 = players;
		foreach (CardGamePlayer player in array2)
		{
			heroSet.Add(player.HeroPrefab);
			foreach (BattleCard card in player.Deck)
			{
				string[] internalHeroName = card.InternalHeroName;
				foreach (string hero in internalHeroName)
				{
					heroSet.Add(Util.GetPrefabHeroName(hero));
				}
			}
		}
		yield return StartCoroutine(PreloadHeroes(heroSet));
		StartTransaction.CompleteStep("loadedHeroAssets");
		CardGamePlayer[] array3 = players;
		foreach (CardGamePlayer p2 in array3)
		{
			if (p2.Info.Type != PlayerType.Network)
			{
				//p2.SendReady(start.QuestNodeID, start.QuestConditions, ticket);  // CSP temporarily commented out for testing
			}
		}
		CspUtils.DebugLog("--- CharacterController.Initialize");
		characterController.Initialize(players[0].HeroPrefab, players[1].HeroPrefab);
		CspUtils.DebugLog("--- Turning on camera");
		yield return StartCoroutine(TurnOnCamera());
		StartTransaction.CompleteStep("camera");
		doneLoadingBundle = false;
		GUIManager.Instance.BundleManager.LoadBundle("toolbox_bundle", OnToolboxBundleLoaded, null);
		while (!doneLoadingBundle)
		{
			yield return 0;
		}
		players[0].KeeperTrayOpacity.SetOpacity(0f);
		players[1].KeeperTrayOpacity.SetOpacity(0f);
		StartTransaction.CompleteStep("onReady");
		ControllerReady();

		///////////////  CSP added block to test without server message ///////////////////
		
		RoomAgent.SetPlayers(players);
		RoomAgent.Connect_CSP();  // needed for faking cardserver ... adds server msg listeners. CSP
		
		// initialize Stock card pile for both players 
		//players[0].OnInitCards(1,5,true); 
		players[0].OnInitCards_CSP(true);
		players[1].OnInitCards_CSP(false); 

		DumpStock_CSP(0);
		DumpStock_CSP(1);

		AppShell.Instance.StartCoroutine(CardGameController.Instance.SimulateServer_CSP()); // added by CSP for testing.
		//////////////////////////////////////////////////////////////////////////////
	}


	///////////// cardserver methods added by CSP ////////////////////////////////////////////////

	public void CardServer_MoveCard(int pid, int cardID, int src, int dest, bool mine, string cardType, int visibility, bool srcOpponent) {
		//string mine;
		int playerID;

		if (pid == 0)
		{	
			playerID = pid;
			//mine = "true";
		}
		else
		{    
			playerID = 0;
			//mine = "false";
		}
		////////////////////////////
		string[] array = new string[9];
		array[0] = "MoveCard";
		array[1] = playerID.ToString();
		array[2] = cardID.ToString();
		array[3] = src.ToString();
		array[4] = dest.ToString();
		array[5] = mine.ToString();
		array[6] = cardType;
		array[7] = visibility.ToString();
		array[8] = srcOpponent.ToString();	
		AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.ServerMessage(array));
		//CspUtils.DebugLog("Calling Fire() for cardgameevent.servermessage!");
	}

	public void CardServer_OnNewTurn(int pid) {
		string[] array = new string[3];
		array[0] = "NewTurn";
		array[1] = pid.ToString();
		array[2] = "true";
		AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.ServerMessage(array));
	}

	public void CardServer_OnPickCard(int pid, List<int> cards, bool canPass, PickCardType pickCardType, int opposingCard, int passButtonId) {
		string cardList2 = "";
		foreach (int card in cards) {
			cardList2 += card.ToString() + ";";
		}
		if (cardList2.Length > 0)
			cardList2 = cardList2.Remove(cardList2.Length - 1, 1); // remove trailing ';'
		
		string[] array = new string[7];
		array[0] = "PickCard";
		array[1] = pid.ToString();
		array[2] = cardList2;
		array[3] = canPass.ToString();
		int pct = (int) pickCardType;
		array[4] = pct.ToString();
		array[5] = opposingCard.ToString();
		array[6] = passButtonId.ToString();
		AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.ServerMessage(array));
	}

	public void CardServer_OnDamage(int pid, int weaponID, List<int> casualties, bool mine, List<string> typeList, int inflicted, int attempted, bool becomeKeeper, bool killKeeper) {
		string cardList2 = "";
		foreach (int card in casualties) {
			cardList2 += card.ToString() + ";";
		}
		if (cardList2.Length > 0)
			cardList2 = cardList2.Remove(cardList2.Length - 1, 1); // remove trailing ';'

		string typeList2 = "";
		foreach (string type in typeList) {
			typeList2 += type + ";";
		}
		if (typeList2.Length > 0)
			typeList2 = typeList2.Remove(typeList2.Length - 1, 1); // remove trailing ';'
		
		string[] array = new string[11];
		array[0] = "Damage";
		array[1] = pid.ToString();
		array[2] = weaponID.ToString();
		array[3] = cardList2;
		array[4] = "0";  // deprecated
		array[5] = mine.ToString();
		array[6] = typeList2;
		array[7] = inflicted.ToString();
		array[8] = attempted.ToString();
		array[9] = becomeKeeper.ToString();
		array[10] = killKeeper.ToString();

		AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.ServerMessage(array));
	}

	public void CardServer_OnBlock(int pid, int weaponID, int blockerID, bool meDefending) {
		string[] array = new string[5];
		array[0] = "Block";
		array[1] = pid.ToString();
		array[2] = weaponID.ToString();
		array[3] = blockerID.ToString();
		array[4] = meDefending.ToString();

		AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.ServerMessage(array));
	}

	public void CardServer_OnInfo(int pid, int message, int arg1, int arg2) {
		string[] array = new string[5];
		array[0] = "Info";
		array[1] = pid.ToString();
		array[2] = message.ToString();
		array[3] = arg1.ToString();
		array[4] = arg2.ToString();

		AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.ServerMessage(array));
	}

	public IEnumerator SimulateServer_CSP() 
	{
		// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		// !!!!!! I THINK I NEED TO DO ONLY ONE TASK PER MOUSE CLICK...USE A COUNTER AND A CASE STATEMENT TO DECIDE WHAT TASK WE ARE DOING THIS TIME. 
		// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

		yield return new WaitForSeconds(3);

		Utils.ListShuffle(players[0].Stock);
		Utils.ListShuffle(players[1].Stock);

		for (int i=0; i<4; i++) {  // NEED TO USE MoveCard_CSP() TO DEAL INTIAL 4 CARDS TO EACH PLAYER HERE.....				
			//(int cardID, int src, int dest, bool mine, string cardType, int visibility, bool srcOpponent)
			// type = CardData.GetString("Cards.Card.ID");   // from cards xml
			BattleCard bc = players[0].Stock[0];  // get next card off the stock pile to put in players hand.
			bc.Identified = true;
			//CspUtils.DebugLog("card dump:");
			//bc.Dump(Console.Out);
			//CspUtils.DebugLog("bc.MiniTexture :" + bc.MiniTexture + " bc.FullTexture:" + bc.FullTexture);
			CardServer_MoveCard(0, bc.ServerID, 0, 1, true, bc.Type, 1, false);
			yield return new WaitForSeconds(1);
			bc = players[1].Stock[0];  // get next card off the stock pile to put in players hand.
			bc.Identified = true;
			CardServer_MoveCard(1, bc.ServerID, 0, 1, false, bc.Type, 1, false);
			yield return new WaitForSeconds(1);
		}	

		///////// removes all cards from stock piles.....for testing game end only!! //////////////
		//players[0].Stock.Clear();
		//players[1].Stock.Clear();

		//for (int i=0; i<15; i++) {  // remove 15 cards from stock for testing quicker game
		//	players[0].Stock.RemoveAt(0);
		//	players[1].Stock.RemoveAt(0);
		//}
		//////////////////////////////////////////////////////////////////////

		AppShell.Instance.StartCoroutine(CardGameController.Instance.SimulateServer_NewTurn_CSP());

		yield return 0; 
	}

	public IEnumerator SimulateServer_NewTurn_CSP()
	{
		// switch offense and defense indexes.
		offenseNdx = 1 - offenseNdx;
		defenseNdx = 1 - defenseNdx;

		// reset picked cards to null
		playerPickedCard = null;
		aiPickedCard = null;

		// flip power level coin
		bool heads = false;
		if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
			heads = true;
		CardGameController.Instance.Hud.FlipCoin(heads, heads);  // true/true will cause power level +1
		yield return new WaitForSeconds(5);

		// first player won coin toss, so they go first	
		//players[0].OnNewTurn(true);
		CardServer_OnNewTurn(offenseNdx);
		yield return new WaitForSeconds(1);

		// draw one card from deck to hand
		if (players[offenseNdx].Stock.Count > 0) {
			BattleCard bc = players[offenseNdx].Stock[0];  // get next card off the stock pile to put in players hand.
			bc.Identified = true;
			CardServer_MoveCard(offenseNdx, bc.ServerID, 0, 1, (offenseNdx == 0), bc.Type, 1, false);
			yield return new WaitForSeconds(2);
		}
		
		// onPickCard() called by server first, to enable player to pick a card, which then causes SendPickCard() to be called???
		//OnPickCard(List<int> cards, bool canPass, PickCardType pickCardType, int opposingCard, int passButtonId)
		// first paramater must be list of card ServerIDs
		List<int> sids = new List<int>(); 
		foreach (BattleCard card in players[offenseNdx].Hand) {
			if (card.Level <= powerLevel)
				sids.Add(card.ServerID);
		}
		//players[0].OnPickCard(sids , true, PickCardType.Attack, -1, 0);
		CardServer_OnPickCard(offenseNdx, sids , true, PickCardType.Attack, -1, 0);
		// at this point, should be waiting on player to pick an attack card...
		yield return new WaitForSeconds(1);  // NEED TO CHANGE THIS WAIT TO BLOCK UNTIL SendPickCard() IS CALLED, INSTEAD.
		

		yield return 0; 
	}

	public IEnumerator SimulateServer_AfterOffensePick_CSP(bool pass) 
	{
		yield return new WaitForSeconds(1);

		if (offenseNdx == 0) 
			offensePickedCard = playerPickedCard;
		else 
			offensePickedCard = aiPickedCard;

		if ((offensePickedCard == null) && (pass = true))  // then player passed
		{
			// start next turn since player passed....
			AppShell.Instance.StartCoroutine(CardGameController.Instance.SimulateServer_NewTurn_CSP());
		}
		else  // else player attacked
		{
			// // move attack card from hand to played	
			//playerPickedCard = players[0].Hand[0];   // for now, just play the first card in hand no matter what player chose.
			//players[0].OnMoveCard(playerPickedCard.ServerID, 1, 2, true, playerPickedCard.Type, 1, false);
			bool mine;
			if (offenseNdx == 0)
				mine = true;
			else
				mine = false;
			CardServer_MoveCard(offenseNdx, offensePickedCard.ServerID, 1, 2, mine, offensePickedCard.Type, 1, false);
			yield return new WaitForSeconds(1);

			// // now AI players turn to pick card	
			//OnPickCard(List<int> cards, bool canPass, PickCardType pickCardType, int opposingCard, int passButtonId)
			// first paramater must be list of card ServerIDs
			List<int> sids = new List<int>();
			CspUtils.DebugLog("players[defenseNdx].Hand.Count= " + players[defenseNdx].Hand.Count); 
			foreach (BattleCard card in players[defenseNdx].Hand) {
				// only add cards with matching block factor.
				foreach (BattleCard.Factor af in offensePickedCard.GetAttackFactors()) {
					if (Array.Exists(card.GetBlockFactors(), element => element == af)) {
						sids.Add(card.ServerID);  // found a block to the attack, so stop here.
						CspUtils.DebugLog("found block card to add to pick list!");
						break;
					}
				}
			}
			//players[1].OnPickCard(sids , true, PickCardType.Block, playerPickedCard.ServerID, 0);
			CardServer_OnPickCard(defenseNdx, sids , true, PickCardType.Block, offensePickedCard.ServerID, 0);
			// at this point, should be waiting on AI player to both pick a block card, and call SendPickCard(BattleCard card, bool pass, PickCardType pickCardType)
			yield return new WaitForSeconds(1);  // NEED TO CHANGE THIS WAIT TO BLOCK UNTIL SendPickCard() IS CALLED, INSTEAD.
		}


		yield return 0; 
	}

	// note: return value should be number of crads that will need to be removed from defender's hand.
	public int computeCasualties(CardGamePlayer player, BattleCard weapon, out List<int> casualties, out List<string> typeList) {

		BattleCard.Factor[] attackFactors = weapon.GetAttackFactors();
		int maxDamage = weapon.Damage;
		CspUtils.DebugLog("computeCasualties maxDamage=" + maxDamage);
		int currDamage = 0;
		casualties = new List<int>();
		typeList = new List<string>();

		// look for first card with a block factor that matches the attack factor...that's where damage will stop.
		foreach (BattleCard card in player.Stock) {
			casualties.Add(card.ServerID);
			typeList.Add(card.Type);
			currDamage++;
			if (currDamage >= maxDamage)
				return 0;	// we've inflicted the maximum amount of damage, so stop here.
			foreach (BattleCard.Factor af in attackFactors) {
				if (Array.Exists(card.GetBlockFactors(), element => element == af)) {
					return 0;  // found a block to the attack, so stop here.
				}
			}
		} 

		// !!! NOTE: if damage is greater than # cards in Stock, then cards from Hand must be taken  !!!!
		if (maxDamage > currDamage)
			return maxDamage - currDamage;
		else
			return 0;
	}
	public IEnumerator SimulateServer_AfterDefensePick_CSP(bool pass) 
	{
		yield return new WaitForSeconds(1);

		if (offenseNdx == 0) 
			defensePickedCard = aiPickedCard;
		else 
			defensePickedCard = playerPickedCard;

		CspUtils.DebugLog("pass=" + pass);

		removeFromHand = 0;

		//if ((!pass) && (aiPickedCard != null)) { // check if block card played, otherwise AI takes damage
		if ((pass) && (defensePickedCard == null)) { // check if passing, then AI takes damage
		
			List<int> casualties; // = new List<int>();
			//casualties.Add(players[1].Stock[0].ServerID);
			//casualties.Add(players[1].Stock[1].ServerID);
			List<string> typeList; // = new List<string>();
			//typeList.Add(players[1].Stock[0].Type);
			//typeList.Add(players[1].Stock[1].Type);
			removeFromHand = computeCasualties(players[defenseNdx], offensePickedCard, out casualties, out typeList);
			CardServer_OnDamage(defenseNdx, offensePickedCard.ServerID, casualties, true, typeList, 2, 4, false, false);
			//yield return new WaitForSeconds(5);
			yield return 0;  // rely on end-of-damage logic to start next step
		}
		else {  // not passing, so must be blocked
		
			// move block card from AI hand to played	
			//aiPickedCard = players[1].Hand[0];   // for now, just play the first card in hand no matter what player chose.
			//players[0].OnMoveCard(aiPickedCard.ServerID, 1, 2, false, aiPickedCard.Type, 1, false);
			CspUtils.DebugLog("defensePickedCard.ServerID=" + defensePickedCard.ServerID);
			bool mine;
			if (offenseNdx == 0)
				mine = false;
			else
				mine = true;
			CardServer_MoveCard(offenseNdx, defensePickedCard.ServerID, 1, 2, mine, defensePickedCard.Type, 1, false);
			yield return new WaitForSeconds(1);
					
			// perform AI block of human attack
			//players[1].OnBlock(playerPickedCard.ServerID, aiPickedCard.ServerID, true);
			CardServer_OnBlock(defenseNdx, offensePickedCard.ServerID, defensePickedCard.ServerID, true);
			yield return new WaitForSeconds(1);

			AppShell.Instance.StartCoroutine(CardGameController.Instance.SimulateServer_EndTurn_CSP(false));

		}



		yield return 0; 
	}

	public IEnumerator SimulateServer_AfterDamage_CSP() 
	{
		yield return new WaitForSeconds(1);
		CspUtils.DebugLog("about to call CardServer_OnInfo()!");
		CardServer_OnInfo(defenseNdx, 142, 1, 0);  // cause OnDamageComplete() to be called, to close jeopardy meter.
		yield return new WaitForSeconds(1);

		if (removeFromHand > 0) {  // there wasnt enough cards in Stock to absorb all damage, so some cards need to be removed from hand, too.
			while ((players[defenseNdx].Hand.Count > 0) && (removeFromHand > 0)) {
				// move first card in hand to discard
				BattleCard discard = players[defenseNdx].Hand[0];
				CardServer_MoveCard(offenseNdx, discard.ServerID, 1, 5, (offenseNdx == 1), discard.Type, 1, false);
				yield return new WaitForSeconds(2);
				removeFromHand--;
			}
		}

		AppShell.Instance.StartCoroutine(CardGameController.Instance.SimulateServer_EndTurn_CSP(true));

		yield return 0; 
	}

	public IEnumerator SimulateServer_EndTurn_CSP(bool pass) 
	{
		// move played card from human from played pile to discard	pile
		CardServer_MoveCard(offenseNdx, offensePickedCard.ServerID, 2, 5, (offenseNdx == 0), offensePickedCard.Type, 1, false);
		yield return new WaitForSeconds(1);

		if (!pass) {  // only return to discard if attack was blocked
			// move played card from AI from played pile to discard	pile
			CardServer_MoveCard(offenseNdx, defensePickedCard.ServerID, 2, 5, (offenseNdx == 1), defensePickedCard.Type, 1, false);
			yield return new WaitForSeconds(1);
		}

		// check for end-of-game
		if ((players[0].Stock.Count == 0) && (players[0].Hand.Count == 0))  // AI wins
			players[0].OnGameOver(0, false);
		else {
			if ((players[1].Stock.Count == 0) && (players[1].Hand.Count == 0))  // Human wins
				players[0].OnGameOver(0, true);
			else
				// start new turn
				AppShell.Instance.StartCoroutine(CardGameController.Instance.SimulateServer_NewTurn_CSP());
		}


		yield return 0; 
	}

	public void DumpStock_CSP(int pid) 
	{
		CspUtils.DebugLog("DumpStock player " + pid + ":");
		for (int i=0; i<players[pid].Stock.Count; i++) {
			BattleCard b = players[pid].Stock[i];
			if (b != null) {
				CspUtils.DebugLog("i=" + i + "  srvId=" + b.ServerID + " type=" + b.Type);
			}
		}
	}
	///////////////////////////////// end of cardserver methods ///////////////////////////////////////////////////////////////

	private void OnGameWorldBundleLoaded(AssetBundleLoadResponse response, object extraData)
	{
		if (response.Bundle == null)
		{
			CspUtils.DebugLog("Failed to load gameworld_bundle: " + response.Error);
		}
		doneLoadingBundle = true;
	}

	private void OnToolboxBundleLoaded(AssetBundleLoadResponse response, object extraData)
	{
		if (response.Bundle == null)
		{
			CspUtils.DebugLog("Failed to load toolbox_bundle: " + response.Error);
		}
		doneLoadingBundle = true;
		StartTransaction.CompleteStep("loadtoolboxUIBundle");
	}

	protected void OnRecipeLoadComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
	}

	protected void OnHeroLoadComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		CspUtils.DebugLog("Hero assets loaded");
		heroLoaderTransaction = null;
	}

	protected void OnAvatarSpawned(CardGameEvent.AvatarSpawned evt)
	{
		SquadBattleCharacterSpawnData spawnData = evt.SpawnData;
		GameObject obj = spawnData.obj;
		DataWarehouse spawnData2 = spawnData.spawnData;
		SquadBattlePlayerEnum playerEnum = evt.PlayerEnum;
		int num = (int)playerEnum;
		HairTrafficController hairTrafficController = obj.AddComponent(typeof(HairTrafficController)) as HairTrafficController;
		hairTrafficController.player = obj;
		HairTrafficControllers[num] = hairTrafficController;
		if (prefabPlayerBillboard != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefabPlayerBillboard) as GameObject;
			Utils.AttachGameObject(obj, gameObject);
			Vector3 localPosition = gameObject.transform.localPosition;
			gameObject.transform.localPosition = new Vector3(localPosition.x, spawnData2.TryGetFloat("//character_controller/height", 0f) + spawnData2.TryGetFloat("//character_controller/playerstatusbillboardoffset", 0.3f), localPosition.z);
			PlayerBillboard component = Utils.GetComponent<PlayerBillboard>(gameObject);
			if (component != null)
			{
				hairTrafficController.billboard = component;
				component.htc = hairTrafficController;
				CardGamePlayer cardGamePlayer = players[num];
				if (!AppShell.Instance.CharacterDescriptionManager[cardGamePlayer.Hero].IsVillain)
				{
					component.HeroName = AppShell.Instance.CharacterDescriptionManager[cardGamePlayer.Hero].CharacterFamily;
				}
				else
				{
					component.HeroName = AppShell.Instance.CharacterDescriptionManager[cardGamePlayer.Hero].CharacterName;
				}
				SpawnData.PlayerType playerType = SpawnData.PlayerType.Stranger;
				if (cardGamePlayer.Info.PlayerID != -1)
				{
					if (cardGamePlayer.Info.IsFriend)
					{
						playerType |= SpawnData.PlayerType.Friend;
					}
					if (cardGamePlayer.Info.ShieldAgent)
					{
						playerType |= SpawnData.PlayerType.ShieldAgent;
					}
					component.SquadName = cardGamePlayer.Info.Name;
					component.Configure(playerType);
				}
				else
				{
					component.Configure(playerType);
				}
				HeroOffsetData.AddHeroOffsetData(cardGamePlayer.Hero, evt.SpawnData.spawnData);
			}
		}
		hairTrafficController.ToggleBillboard(false, 0f);
		HairTrafficControllerVisibility[num] = false;
	}

	protected override void OnStartTransactionComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		switch (exit)
		{
		case TransactionMonitor.ExitCondition.Success:
			CspUtils.DebugLog("Card Game Initialization SUCCESS.");
			break;
		case TransactionMonitor.ExitCondition.Fail:
			CspUtils.DebugLog("Card Game Initialization FAILED.");
			break;
		case TransactionMonitor.ExitCondition.TimedOut:
			CspUtils.DebugLog("Card Game Initialization TIMED OUT.");
			break;
		}
	}

	private void OnCardGameCommonLoaded(AssetBundleLoadResponse response, object extraData)
	{
		if (response.Bundle != null)
		{
			cardgameBundle = response.Bundle;
			fullCardPrefab = (response.Bundle.Load(fullCardPrefabName) as GameObject);
			if (fullCardPrefab == null)
			{
				CspUtils.DebugLog("Failed to load FullCard_prefab");
			}
			centerPanelPrefab = (response.Bundle.Load(centerPanelPrefabName) as GameObject);
			if (centerPanelPrefab == null)
			{
				CspUtils.DebugLog("Failed to load Center Panel Prefab");
			}
			coinPanelPrefab = (response.Bundle.Load(coinPanelPrefabName) as GameObject);
			if (coinPanelPrefab == null)
			{
				CspUtils.DebugLog("Failed to load Coin Panel Prefab");
			}
			powerPanelPrefab = (response.Bundle.Load(powerPanelPrefabName) as GameObject);
			if (powerPanelPrefab == null)
			{
				CspUtils.DebugLog("Failed to load Power Panel Prefab");
			}
			opponentHudPrefab = (response.Bundle.Load(opponentHudPrefabName) as GameObject);
			if (opponentHudPrefab == null)
			{
				CspUtils.DebugLog("Failed to load Opponent HUD Prefab");
			}
			playerHudPrefab = (response.Bundle.Load(playerHudPrefabName) as GameObject);
			if (playerHudPrefab == null)
			{
				CspUtils.DebugLog("Failed to load Player HUD Prefab");
			}
			AudioManager.OnAudioLoaded(response, extraData);
		}
		else
		{
			CspUtils.DebugLog("Failed to load cardgame_common: " + response.Error);
		}
		doneLoadingBundle = true;
	}

	private void Shutdown(string message)
	{
		CspUtils.DebugLog("Shutting down for: " + message);
		ControllerReady();
		RoomAgent.Disconnect();
		StartTransaction.Fail(message);
		StartCoroutine(SwitchToFallback(message));
	}

	private IEnumerator SwitchToFallback(string message)
	{
		yield return new WaitForEndOfFrame();
		SHSErrorCodes.Response response = SHSErrorCodes.GetResponse(SHSErrorCodes.Code.CantEnterCardGame);
		GUIManager.Instance.ShowErrorDialog(response, message);
		AppShell.Instance.TransitionHandler.AbortTransition("card_start_transaction");
	}

	public UnityEngine.Object GetEffectByName(string name)
	{
		UnityEngine.Object result = null;
		if (cardgameBundle != null && !string.IsNullOrEmpty(name))
		{
			result = cardgameBundle.Load(name);
		}
		return result;
	}

	private IEnumerator LoadArena(StartInfo start)
	{
		string bundlePath = "CardGame/" + start.ArenaName;
		string[] assetBundleList = new string[1]
		{
			bundlePath + "_scenario" + start.ArenaScenario
		};
		GameObject staticRoot = new GameObject("static_bundles");
		List<AssetBundleLoadResponse> responses = new List<AssetBundleLoadResponse>();
		string[] array = assetBundleList;
		foreach (string bundle in array)
		{
			AppShell.Instance.BundleLoader.FetchAssetBundle(bundle, delegate(AssetBundleLoadResponse response, object extraData)
			{
				CspUtils.DebugLog("Added response");
				responses.Add(response);
			}, null);
		}
		while (responses.Count < assetBundleList.Length)
		{
			yield return new WaitForEndOfFrame();
		}
		foreach (AssetBundleLoadResponse response2 in responses)
		{
			if (response2.Bundle != null)
			{
				GameObject obj = UnityEngine.Object.Instantiate(response2.Bundle.mainAsset) as GameObject;
				obj.transform.parent = staticRoot.transform;
			}
			if (response2.Error != null)
			{
				throw new FileLoadException("Failed loading asset bundle (" + response2.Path + "): " + response2.Error);
			}
			CspUtils.DebugLog("Loaded.." + response2.Path + " " + response2.Error);
		}
		Hud.Initialize(players);
		StartTransaction.CompleteStep("loadArena");
	}

	private IEnumerator ConnectToRoom(StartInfo start)
	{
		////// this block to temporarily bypass game room connect ////////////
		//RoomAgent.SetPlayers(players);
		//CspUtils.DebugLog("Players selected");
		StartTransaction.CompleteStep("connectRoom");
		yield break;
		////////////////////////////////////

		if (ticket.status == Matchmaker2.Ticket.Status.SUCCESS)
		{
			if (AppShell.Instance.ServerConfig.TryGetBool("//custom_smartfox_server", false))
			{
				string whichEnv = AppShell.Instance.ServerConfig.TryGetString("//environment", "DEV");
				string server = AppShell.Instance.ServerConfig.TryGetString("//" + whichEnv + "/smartfox/customserver", null);
				ticket.server = server;
			}
			CspUtils.DebugLog("Card game server " + ticket.server + " | ticket: " + ticket.ticket);
			start.RoomTicket = ticket;
			MultiCoroutine connector = new MultiCoroutine(RoomAgent.Connect(start));
			yield return StartCoroutine(connector);
			try
			{
				connector.Throw();
				CspUtils.DebugLog("Connected to room");
			}
			catch (Exception ex)
			{
				CspUtils.DebugLog("Card game failed connecting: " + ex.Message);
				SHSErrorCodes.Response response = new SHSErrorCodes.Response(0, SHSErrorCodes.Code.CantEnterCardGame, delegate
				{
					Shutdown("Unable to connect to the card game server");
				}, SHSErrorCodes.Message.CantEnterCardGame);
				GUIManager.Instance.ShowErrorDialog(response, ex.Message);
				yield break;
			}
			RoomAgent.SetPlayers(players);
			CspUtils.DebugLog("Players selected");
			StartTransaction.CompleteStep("connectRoom");
			yield break;
		}
		throw new Exception("Failed to acquire a ticket: " + ticket.ticket);
	}

	private IEnumerator PreloadHeroes(HashSet<string> heroSet)
	{
		foreach (string heroName in heroSet)
		{
			heroLoaderTransaction.AddStep(heroName);
		}
		GameDataManager dataManager = AppShell.Instance.DataManager;
		Dictionary<string, GameDataLoadResponse> dataLoadResponses = new Dictionary<string, GameDataLoadResponse>();
		foreach (string hero in heroSet)
		{
			//CspUtils.DebugLog("Loading gamedata for " + hero);
			dataManager.LoadGameData("Characters/" + hero, delegate(GameDataLoadResponse response, object extraData)
			{
				string text = (string)extraData;
				dataLoadResponses[text] = response;
				//CspUtils.DebugLog("Loaded hero " + text);
				heroLoaderTransaction.CompleteStep(text);
			}, hero);
		}
		while (dataLoadResponses.Count < heroSet.Count)
		{
			yield return new WaitForEndOfFrame();
		}
		AssetBundleLoader loader = AppShell.Instance.BundleLoader;
		int remainingHeroes = heroSet.Count;
		foreach (KeyValuePair<string, GameDataLoadResponse> response2 in dataLoadResponses)
		{
			if (!string.IsNullOrEmpty(response2.Value.Error))
			{
				throw new FileLoadException(string.Format("Error loading hero prefab {0}: {1}", response2.Key, response2.Value.Error));
			}
			DataWarehouse data = response2.Value.Data;
			string bundleName = data.GetString("//asset_bundle");
			string modelName = data.GetString("//character_model/model_name");
			if (!string.IsNullOrEmpty(bundleName) && !string.IsNullOrEmpty(modelName))
			{
				loader.LoadAsset(bundleName, modelName, null, delegate
				{
					remainingHeroes--;
				});
				loader.PreLoadAsset(bundleName, "movement_run");
				DataWarehouse fxData = data.GetData("//effect_sequence_list");
				string characterFxBundleName = fxData.TryGetString("character_fx", string.Empty);
				foreach (DataWarehouse dw in fxData.GetIterator("logical_effect"))
				{
					string fxPrefabName = dw.TryGetString("prefab_name", string.Empty);
					if (!string.IsNullOrEmpty(fxPrefabName) && !string.IsNullOrEmpty(characterFxBundleName))
					{
						loader.PreLoadAsset(characterFxBundleName, fxPrefabName);
					}
				}
				Singleton<VOBundleLoader>.instance.LoadCharacter(response2.Key);
			}
			else
			{
				CspUtils.DebugLog("Bundle name <" + bundleName + "> or model name <" + modelName + "> was null or empty!  Didn't request a load...");
			}
		}
		while (remainingHeroes != 0)
		{
			yield return new WaitForEndOfFrame();
		}
		foreach (string s in heroSet)
		{
			preloadedHeroes.Add(s);
		}
	}

	private CardGamePlayer[] CreatePlayers(PlayerInfo[] players)
	{
		CspUtils.DebugLog("Creating players");
		GameObject gameObject = new GameObject("Players");
		Utils.AttachGameObject(base.gameObject, gameObject);
		CardGamePlayer[] array = new CardGamePlayer[players.Length];
		for (int i = 0; i < array.Length; i++)
		{
			CardGamePlayer cardGamePlayer = null;
			switch (players[i].Type)
			{
			case PlayerType.Human:
				cardGamePlayer = CreatePlayer<CardGameHumanPlayer>();
				cardGamePlayer.gameObject.name = "HumanPlayer";
				break;
			case PlayerType.AI:
				cardGamePlayer = CreatePlayer<CardGameAIPlayer>();
				cardGamePlayer.gameObject.name = "AIPlayer";
				break;
			case PlayerType.Network:
				cardGamePlayer = CreatePlayer<CardGameNetworkPlayer>();
				cardGamePlayer.gameObject.name = "NetworkPlayer";
				break;
			default:
				throw new Exception("Unknown player type encountered");
			}
			cardGamePlayer.Info = players[i];
			array[i] = cardGamePlayer;
			Utils.AttachGameObject(gameObject, cardGamePlayer.gameObject);
		}
		array[0].opponent = array[1];
		array[1].opponent = array[0];
		CspUtils.DebugLog("Players " + array[0].Info.PlayerID + " and " + array[1].Info.PlayerID + " created");
		CspUtils.DebugLog("Player1 type: " + array[0].gameObject.name + "   Player2 type: " + array[1].gameObject.name);
		return array;
	}

	private CardGamePlayer CreatePlayer<T>() where T : CardGamePlayer
	{
		GameObject g = new GameObject();
		return Utils.AddComponent<T>(g);
	}

	private IEnumerator TurnOnCamera()
	{
		CameraLiteManagerSquadBattle cameraMan;
		do
		{
			cameraMan = (CameraLiteManager.Instance as CameraLiteManagerSquadBattle);
			if (cameraMan == null)
			{
				yield return new WaitForEndOfFrame();
			}
		}
		while (cameraMan == null);
		cameraMan.suspended = false;
	}

	public void SetPower(int power)
	{
		if (power != powerLevel)
		{
			AppShell.Instance.EventMgr.Fire(this, new CardGameEvent.SetPowerLevel(powerLevel, power));
			powerLevel = power;
		}
	}

	public void QueueDealerChat(int msgId, params object[] args)
	{
		CspUtils.DebugLog("animQ enqueing: DealerChat");
		animQ.Enqueue("DealerChat", delegate
		{
			AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.DealerChat(CardGameEvent.DealerChat.MessageType.Prompt, msgId, CardLocale.Parse(msgId), args));
		}, true);
	}

	public void DisableHud()
	{
		Hud.Camera.enabled = false;
		SHSCardGameMainWindow sHSCardGameMainWindow = GUIManager.Instance["/SHSMainWindow/SHSCardGameMainWindow"] as SHSCardGameMainWindow;
		sHSCardGameMainWindow.HideHudElements();
	}

	public void ShowPlayerBillboard(SquadBattlePlayerEnum playerEnum, float animTime)
	{
		if ((int)playerEnum < HairTrafficControllers.Length && HairTrafficControllers[(int)playerEnum] != null && !HairTrafficControllerVisibility[(int)playerEnum])
		{
			HairTrafficControllers[(int)playerEnum].ToggleBillboard(true, animTime);
			HairTrafficControllerVisibility[(int)playerEnum] = true;
		}
	}

	public void HidePlayerBillboard(SquadBattlePlayerEnum playerEnum, float animTime)
	{
		if ((int)playerEnum < HairTrafficControllers.Length && HairTrafficControllers[(int)playerEnum] != null && HairTrafficControllerVisibility[(int)playerEnum])
		{
			HairTrafficControllers[(int)playerEnum].ToggleBillboard(false, animTime);
			HairTrafficControllerVisibility[(int)playerEnum] = false;
		}
	}

	public void HighlightAvatar(int playerIndex)
	{
		characterController.HighlightAvatar(playerIndex);
	}

	public void DestroyKeeper(CardGamePlayer owner, string keeperName)
	{
		SquadBattlePlayerEnum player = (!(owner == players[0])) ? SquadBattlePlayerEnum.Right : SquadBattlePlayerEnum.Left;
		characterController.RemoveKeeper(player, keeperName);
	}

	public void PlayRecoilAnimation(CardGamePlayer defender)
	{
		SquadBattleRecoil squadBattleRecoil = new SquadBattleRecoil();
		squadBattleRecoil.player = ((!(defender == players[0])) ? SquadBattlePlayerEnum.Right : SquadBattlePlayerEnum.Left);
		squadBattleRecoil.characterName = Util.GetPrefabHeroName(defender.HeroPrefab);
		squadBattleRecoil.recoilType = CombatController.AttackData.RecoilType.Small;
		characterController.PlayRecoil(squadBattleRecoil);
		AppShell.Instance.EventMgr.Fire(this, new CardGameEvent.ResumeServerQueue());
	}

	public void PlayAttackAnimation(CardGamePlayer defender, BattleCard weapon, int damage, bool isKeeper, bool becomeKeeper, bool killKeeper, int startingHealth)
	{
		SquadBattleAction squadBattleAction = new SquadBattleAction();
		string[] internalHeroName = weapon.InternalHeroName;
		squadBattleAction.attackingCharacterName = Util.GetPrefabHeroName(internalHeroName[0]);
		if (internalHeroName.Length > 1)
		{
			squadBattleAction.secondaryAttackingCharacterName = Util.GetPrefabHeroName(internalHeroName[1]);
		}
		squadBattleAction.damage = damage;
		squadBattleAction.player = ((!(defender == players[1])) ? SquadBattlePlayerEnum.Right : SquadBattlePlayerEnum.Left);
		squadBattleAction.attackerBecomesKeeper = becomeKeeper;
		squadBattleAction.attackerKeeperDestroyed = killKeeper;
		squadBattleAction.startingHealth = startingHealth;
		squadBattleAction.preventKO = defender.HasPreventKOKeeper();
		SquadBattleAttackPattern squadBattleAttackPattern = (!isKeeper) ? weapon.AttackPattern : weapon.KeeperPattern;
		SquadBattleAttackPattern squadBattleAttackPattern2;
		if (squadBattleAttackPattern == null)
		{
			squadBattleAttackPattern2 = new SquadBattleAttackPattern();
			squadBattleAttackPattern2.AttackSequenceString = "L1,L2,L3,L4,L5";
			squadBattleAttackPattern2.RepeatSequence = false;
		}
		else
		{
			squadBattleAttackPattern2 = new SquadBattleAttackPattern(squadBattleAttackPattern);
		}
		squadBattleAction.attackPattern = squadBattleAttackPattern2;
		CspUtils.DebugLog("Requesting attack sequence: " + squadBattleAction.attackingCharacterName + " " + squadBattleAction.attackPattern.AttackSequenceString + " " + squadBattleAction.damage + " " + squadBattleAction.attackerBecomesKeeper.ToString() + " " + squadBattleAction.attackerKeeperDestroyed.ToString() + "  keeper:" + isKeeper + "   preventKO:" + squadBattleAction.preventKO + "   second character: " + squadBattleAction.secondaryAttackingCharacterName + ((!squadBattleAction.attackPattern.DelaySecondCharacterSpawn) ? string.Empty : "(delayed)"));
		characterController.QueueAction(squadBattleAction);
		if (!players[0].Played.Contains(fullCardDetailsSource) && !players[1].Played.Contains(fullCardDetailsSource))
		{
			ClearCardDetails();
		}
	}

	public void PlayBlockAnimation(CardGamePlayer defender, BattleCard weapon, BattleCard blockCard)
	{
		SquadBattleAction squadBattleAction = new SquadBattleAction();
		string[] internalHeroName = weapon.InternalHeroName;
		squadBattleAction.attackingCharacterName = Util.GetPrefabHeroName(internalHeroName[0]);
		if (internalHeroName.Length > 1)
		{
			squadBattleAction.secondaryAttackingCharacterName = Util.GetPrefabHeroName(internalHeroName[1]);
		}
		squadBattleAction.damage = 0;
		squadBattleAction.player = ((!(defender == players[1])) ? SquadBattlePlayerEnum.Right : SquadBattlePlayerEnum.Left);
		squadBattleAction.attackerBecomesKeeper = false;
		if (defender.opponent.Keepers.Contains(weapon) || defender.Keepers.Contains(weapon))
		{
			squadBattleAction.attackerKeeperDestroyed = true;
		}
		else
		{
			squadBattleAction.attackerKeeperDestroyed = false;
		}
		squadBattleAction.blockingCharacterName = defender.HeroPrefab;
		Instance.AudioManager.Play(CardGameAudioManager.SFX.HandBlock);
		SquadBattleAttackPattern squadBattleAttackPattern = new SquadBattleAttackPattern();
		if (weapon.AttackPattern == null)
		{
			squadBattleAttackPattern.AttackSequenceString = "L1,L2,L3,L4,L5";
			squadBattleAttackPattern.RepeatSequence = true;
		}
		else
		{
			squadBattleAttackPattern.AttackSequenceString = weapon.AttackPattern.AttackSequenceString;
			squadBattleAttackPattern.RepeatSequence = weapon.AttackPattern.RepeatSequence;
		}
		squadBattleAction.attackPattern = squadBattleAttackPattern;
		characterController.QueueAction(squadBattleAction);
		if (!players[0].Played.Contains(fullCardDetailsSource) && !players[1].Played.Contains(fullCardDetailsSource))
		{
			ClearCardDetails();
		}
	}

	public void PlayApproachAnimation(CardGamePlayer attacker, BattleCard card)
	{
		SquadBattlePlayerEnum attackingTeam = (!(attacker == players[0])) ? SquadBattlePlayerEnum.Right : SquadBattlePlayerEnum.Left;
		string[] internalHeroName = card.InternalHeroName;
		bool flag = (internalHeroName.Length > 1) ? true : false;
		bool flag2 = false;
		string[] array = internalHeroName;
		foreach (string text in array)
		{
			if (!string.IsNullOrEmpty(text) && (!flag2 || !card.AttackPattern.DelaySecondCharacterSpawn))
			{
				characterController.ApproachTarget(attackingTeam, Util.GetPrefabHeroName(text), flag, flag2);
				flag2 = true;
			}
		}
		if (!flag)
		{
		}
	}

	public void PlayKeeperNoDamage(CardGamePlayer attacker, BattleCard card)
	{
		SquadBattleAction squadBattleAction = new SquadBattleAction();
		squadBattleAction.attackingCharacterName = Util.GetPrefabHeroName(card.InternalHeroName[0]);
		squadBattleAction.player = ((!(attacker == players[0])) ? SquadBattlePlayerEnum.Right : SquadBattlePlayerEnum.Left);
		if (card.KeeperPattern.ZeroDamageEmote.Length == 0)
		{
			squadBattleAction.emoteString = "emote_cheer";
		}
		else
		{
			squadBattleAction.emoteString = card.KeeperPattern.ZeroDamageEmote;
		}
		characterController.KeeperNoDamage(squadBattleAction);
	}

	public void CardFlip(CardGameFlipCard evt)
	{
		CspUtils.DebugLog("Bleh1");
		int numCards = evt.numCards;
		float duration = evt.duration;
		if (numCards <= 0)
		{
			CspUtils.DebugLog("non-positive integer passed to CardFlip()");
			return;
		}
		float secondsDuration = duration / (float)numCards;
		StartCoroutine(CoFlipCards(numCards, secondsDuration));
	}

	protected IEnumerator CoFlipCards(int NumCards, float SecondsDuration)
	{
		CspUtils.DebugLog("Co-flip cards");
		float nextFlip = 0f;
		for (int cardsRemaining = NumCards; cardsRemaining > 0; cardsRemaining--)
		{
			while (Time.time < nextFlip)
			{
				yield return 0;
			}
			AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.AnimFinished());
			nextFlip = Time.time + SecondsDuration;
		}
	}

	public void OnSloMo(CardGameEvent.SlowMotion evt)
	{
		Time.timeScale = 0.1f;
		StartCoroutine(CoSloMo());
	}

	protected IEnumerator CoSloMo()
	{
		float killSloMoTime = Time.time + 0.15f;
		while (Time.time < killSloMoTime)
		{
			yield return 0;
		}
		Time.timeScale = 1f;
	}

	public void OnCancelSloMo(CardGameEvent.NoMoSloMo evt)
	{
		Time.timeScale = 1f;
	}

	public void OnAttackFinished(CardGameEvent.CombatFinished evt)
	{
	}

	public void HandleGameEnd(bool playerWon, bool playerForfeited)
	{
		gameCompleted = true;
		winner = ((!playerWon) ? 1 : 0);
		SHSCardGameCompleteWindow sHSCardGameCompleteWindow = (SHSCardGameCompleteWindow)GUIManager.Instance["/SHSMainWindow/SHSCardGameMainWindow/SHSCardGameCompleteWindow"];
		sHSCardGameCompleteWindow.SetPlayerWon(playerWon, playerForfeited);
		sHSCardGameCompleteWindow.Show();
		if (!IsPvPBattle())
		{
			StartInfo startInfo = AppShell.Instance.SharedHashTable["CardGameLevel"] as StartInfo;
			if (startInfo != null)
			{
			}
			if (playerWon)
			{
				CheckForQuestCompletion();
			}
		}
	}

	private void CheckForQuestCompletion()
	{
		StartInfo startInfo = AppShell.Instance.SharedHashTable["CardGameLevel"] as StartInfo;
		if (startInfo != null && startInfo.QuestID != -1)
		{
			CardQuestPart.QuestBattle questBattle = null;
			CardQuestPart questPart = AppShell.Instance.CardQuestManager.GetQuestPart(startInfo.QuestID);
			if (questPart != null)
			{
				foreach (CardQuestPart.QuestBattle node in questPart.Nodes)
				{
					if (node.Id == startInfo.QuestNodeID)
					{
						questBattle = node;
						break;
					}
				}
			}
			if (questBattle != null)
			{
				CardQuestPart parentCardQuestPart = questBattle.ParentCardQuestPart;
				int num = questBattle.NumberOfCardsWon + 1;
				foreach (CardQuestPart.QuestBattle node2 in parentCardQuestPart.Nodes)
				{
					if (node2 != questBattle)
					{
						string[] array = node2.Reward.Split(';');
						if (array.Length == 1 || (array.Length > 1 && string.IsNullOrEmpty(array[1])))
						{
							string text = array[0];
							string[] array2 = text.Split(':');
							if (array2.Length == 2 && (!CardCollection.Collection.ContainsKey(array2[0]) || CardCollection.Collection[array2[0]] < num))
							{
								break;
							}
						}
					}
				}
			}
		}
	}

	public void HoverCard(BattleCard TargetCard)
	{
		if (!Hud.Camera.enabled)
		{
			return;
		}
		bool flag = !TargetCard.Moving && TargetCard.Faceup && TargetCard != fullCardDetailsSource;
		if (flag)
		{
			if (players[0].Discard.Contains(TargetCard) && players[0].Discard[players[0].Discard.Count - 1] != TargetCard)
			{
				flag = false;
			}
			else if (players[1].Discard.Contains(TargetCard) && players[1].Discard[players[1].Discard.Count - 1] != TargetCard)
			{
				flag = false;
			}
		}
		if (fullCardDetailsSource != null && fullCardDetailsSource.ServerID != TargetCard.ServerID)
		{
			ClearCardDetails();
		}
		if (!flag)
		{
			return;
		}
		ShowCardDetails(TargetCard);
		if (fullCardDetailsSource != TargetCard)
		{
			if (fullCardDetailsSource != null)
			{
				fullCardDetailsSource.CardObj.transform.localScale = fullCardDetailsSourceScale;
				fullCardDetailsSource.CardObj.transform.localPosition = fullCardDetailsSourcePos;
			}
			fullCardDetailsSource = TargetCard;
			fullCardDetailsSourceScale = TargetCard.CardObj.transform.localScale;
			fullCardDetailsSourcePos = TargetCard.CardObj.transform.localPosition;
			TargetCard.CardObj.transform.localScale = fullCardDetailsSourceScale * 1.2f;
			if (players[0].Hand.Contains(TargetCard))
			{
				TargetCard.CardObj.transform.localPosition = fullCardDetailsSourcePos + new Vector3(0f, 1.11f, 0f);
			}
			else if (players[0].Keepers.Contains(TargetCard))
			{
				TargetCard.CardObj.transform.localPosition = fullCardDetailsSourcePos + new Vector3(0f, 0.5f, 0f);
			}
			else if (players[1].Keepers.Contains(TargetCard))
			{
				TargetCard.CardObj.transform.localPosition = fullCardDetailsSourcePos + new Vector3(0f, -0.5f, 0f);
			}
		}
	}

	public void ClearCardDetails()
	{
		if (fullCardDetailsSource != null)
		{
			fullCardDetailsSource.CardObj.transform.localScale = fullCardDetailsSourceScale;
			fullCardDetailsSource.CardObj.transform.localPosition = fullCardDetailsSourcePos;
			fullCardDetailsSource = null;
		}
		AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.FullCardDetails(null, true));
	}

	protected void ShowCardDetails(BattleCard Card)
	{
		bool leftSide = true;
		BattleCard battleCard = players[0].FindCardByIndex(Card.ServerID);
		if (battleCard != null)
		{
			leftSide = false;
		}
		AppShell.Instance.EventMgr.Fire(this, new CardGameEvent.FullCardDetails(Card.FullTexture, leftSide));
		AudioManager.Play(CardGameAudioManager.SFX.CardHover);
	}

	public override void OnNotificationResult(Hashtable msg)
	{
		string a = (string)msg["message_type"];
		if (a == "quest_game_rewards" || a == "card_pvp_game_rewards")
		{
			OnRewardsNotificationResult(msg);
		}
		else
		{
			base.OnNotificationResult(msg);
		}
	}

	public void OnRewardsNotificationResult(Hashtable msg)
	{
		UserProfile profile = AppShell.Instance.Profile;
		if (profile == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		string card = string.Empty;
		string ownable = string.Empty;
		if (msg.ContainsKey("tickets_awarded"))
		{
			num = int.Parse((string)msg["tickets_awarded"]);
			profile.Tickets += num;
		}
		if (msg.ContainsKey("silver_awarded"))
		{
			num3 = int.Parse((string)msg["silver_awarded"]);
			profile.Silver += num3;
		}
		if (msg.ContainsKey("card_awarded"))
		{
			string recipe = (string)msg["card_awarded"];
			Dictionary<string, int> dictionary = CardManager.ParseRecipe(recipe);
			using (Dictionary<string, int>.Enumerator enumerator = dictionary.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					card = enumerator.Current.Key;
				}
			}
			if ((string)msg["message_type"] == "quest_game_rewards")
			{
				AppShell.Instance.CounterManager.AddCounter("CardGameQuestCounter", players[0].Info.Hero, false);
			}
			else
			{
				AppShell.Instance.CounterManager.AddCounter("CardGamePvPCounter", players[0].Info.Hero, false);
			}
		}
		if (msg.ContainsKey("hero_xp_awarded") && players != null && players[0] != null)
		{
			num2 = int.Parse((string)msg["hero_xp_awarded"]);
			HeroPersisted value;
			if (profile.AvailableCostumes.TryGetValue(players[0].Hero, out value))
			{
				value.UpdateXp(num2, true);
			}
		}
		if (msg.ContainsKey("ownables_awarded"))
		{
			ownable = (string)msg["ownables_awarded"];
		}
		AppShell.Instance.EventMgr.Fire(this, new CardGameEvent.CardGameResults(num, num2, num3, card, ownable));
	}

	public void OnShowVersus(CardGameEvent.ShowVersusIcon msg)
	{
		GameObject gameObject = CardGameBundle.Load("VS_Prefab") as GameObject;
		if (gameObject != null)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity) as GameObject;
			EffectSequence component = Utils.GetComponent<EffectSequence>(gameObject2, Utils.SearchChildren);
			if (component != null)
			{
				component.Initialize(Hud.Camera.gameObject, null, null);
				component.transform.localPosition = new Vector3(0f, -0.55f, 1.15f);
				component.transform.localRotation = Quaternion.AngleAxis(180f, Vector3.up);
				component.StartSequence();
			}
			UnityEngine.Object.Destroy(gameObject2, 5f);
		}
	}

	public bool IsPvPBattle()
	{
		if (players == null || players.Length != 2)
		{
			return false;
		}
		CardGamePlayer[] array = players;
		foreach (CardGamePlayer cardGamePlayer in array)
		{
			if (cardGamePlayer.Info.Type == PlayerType.Network)
			{
				return true;
			}
		}
		return false;
	}

	protected override void RegisterDebugKeys()
	{
		KeyCodeEntry keyCodeEntry = new KeyCodeEntry(KeyCode.Equals, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, DebugIncreasePower);
		keyCodeEntry = new KeyCodeEntry(KeyCode.Minus, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, DebugDecreasePower);
		keyCodeEntry = new KeyCodeEntry(KeyCode.W, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, DebugWin);
		keyCodeEntry = new KeyCodeEntry(KeyCode.L, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, DebugLose);
		base.RegisterDebugKeys();
	}

	private void RegisterDebugEvents()
	{
	}

	private void UnregisterDebugEvents()
	{
	}

	[Description("Increase power")]
	private void DebugIncreasePower(SHSKeyCode code)
	{
		players[0].SendDebug(3, 0, 0);
	}

	[Description("Decrease power")]
	private void DebugDecreasePower(SHSKeyCode code)
	{
		players[0].SendDebug(4, 0, 0);
	}

	[Description("Win the game")]
	private void DebugWin(SHSKeyCode code)
	{
		players[0].SendDebug(5, 0, 0);
	}

	[Description("Lose the game")]
	private void DebugLose(SHSKeyCode code)
	{
		players[0].SendDebug(6, 0, 0);
	}
}
