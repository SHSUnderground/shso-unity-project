using CardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("CardGame/Private/Player")]
public class CardGamePlayer : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public class DamageAccumulator
	{
		public int attemptedDamage;

		public int inflictedDamage;

		public BattleCard weapon;

		public List<BattleCard> damagedCards;

		public bool useJeopardyMeter;

		public bool finished;

		public DamageAccumulator()
		{
			attemptedDamage = 0;
			inflictedDamage = 0;
			damagedCards = new List<BattleCard>();
			useJeopardyMeter = true;
			finished = false;
		}
	}

	public const int CARDPILE_STOCK = 0;

	public const int CARDPILE_HAND = 1;

	public const int CARDPILE_PLAYED = 2;

	public const int CARDPILE_KEEPERS = 3;

	public const int CARDPILE_SHOW = 4;

	public const int CARDPILE_DISCARD = 5;

	public const int CARDPILE_PRIVATE = 6;

	public const int CARDPILE_JEOPARDY = 7;

	public const int CARDPILE_HIDDEN = 8;

	public const int ANIM_APPROACH = 64;

	public const int ANIM_PLAYSOUND = 65;

	public const int ANIM_HIGHLIGHTATTACK = 66;

	public const int ANIM_KEEPER_NO_DAMAGE = 67;

	public const int ANIM_COIN_FLIP = 68;

	public const int ANIM_NEW_KEEPER = 69;

	public const int INFO_DAMAGE_COMPLETE = 142;

	public const int SOUND_ATTACK_CARD = 0;

	public const int SOUND_KEEPER_ACTIVATE = 2;

	public const int SOUND_KEEPER_DESTROY = 3;

	public const int SOUND_CARD_TRIGGERED = 4;

	public const int SOUND_KEEPER_MISFIRE = 5;

	public const int SOUND_ATTACK_UNBLOCKABLE = 6;

	private PlayerInfo info = new PlayerInfo();

	public CardGroup Deck = new CardGroup();

	public CardGamePlayer opponent;

	public CardPile Stock = new CardPile();

	public CardPile Hand = new CardPile();

	public CardPile Played = new CardPile();

	public CardPile Keepers = new CardPile();

	public CardPile Show = new CardPile();

	public CardPile Discard = new CardPile();

	public CardPile Private = new CardPile();

	public CardPile Jeopardy = new CardPile();

	public CardPile Hidden = new CardPile();

	public RetreatingPanel SidePanel;

	public RetreatingPanel EdgePanel;

	public RetreatingPanel PlayPanel;

	protected List<CardPile> stacks;

	public MeshOpacity KeeperTrayOpacity;

	public GameObject cardPrefab;

	public DeckCounter deckCounterComponent;

	public TransactionMonitor deckLoaderTransaction;

	public JeopardyMeter JeopardyMeterScript;

	private DamageAccumulator damageAccumulator;

	private SHSCardGamePlayerWaitDialog playerWaitDialog;

	private int attacksThisTurn;

	private bool lastCardMovementWasReveal;

	public int stockCountFromLastDamage = -1;

	protected bool myTurn;

	public PlayerInfo Info
	{
		get
		{
			return info;
		}
		set
		{
			info = value;
		}
	}

	public string Hero
	{
		get
		{
			return info.Hero;
		}
	}

	public string HeroPrefab
	{
		get
		{
			return Util.GetPrefabHeroName(Hero);
		}
	}

	public bool MyTurn
	{
		get
		{
			return myTurn;
		}
	}

	public CardGamePlayer()
	{
		stacks = new List<CardPile>(7);
		stacks.Add(Stock);
		stacks.Add(Hand);
		stacks.Add(Played);
		stacks.Add(Keepers);
		stacks.Add(Show);
		stacks.Add(Discard);
		stacks.Add(Private);
		stacks.Add(Jeopardy);
		stacks.Add(Hidden);
	}

	public IEnumerator LoadDeck()
	{
		MultiCoroutine loader = new MultiCoroutine(Deck.LoadRecipe(info.DeckRecipe, deckLoaderTransaction, true));
		yield return StartCoroutine(loader);
		loader.Throw();
	}

	public void Awake()
	{
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.IntroSequenceFinished>(OnIntroSequenceFinished);
	}

	// method added by CSP
	// public void Update() {
	// 	if (this.Info.Type == PlayerType.AI)
	// 		this.deckCounterComponent.gameObject.transform.position = new Vector3(0, -4.2f, -0.86f);
	// 	else 
	// 		this.deckCounterComponent.gameObject.transform.position = new Vector3(4.42f, 11.7f, -1.65f);
	// }

	protected virtual void OnIntroSequenceFinished(CardGameEvent.IntroSequenceFinished msg)
	{
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.IntroSequenceFinished>(OnIntroSequenceFinished);
		if (opponent.Info.Type == PlayerType.Network && CardGameController.Instance.animQ.QueueSize == 0)
		{
			GUIManager.Instance.ShowDialog(typeof(SHSCardGamePlayerWaitDialog), "Waiting for other player...", new GUIDialogNotificationSink(delegate(string Id, IGUIDialogWindow window)
			{
				playerWaitDialog = (window as SHSCardGamePlayerWaitDialog);
			}, delegate
			{
			}, delegate
			{
			}, delegate
			{
				AppShell.Instance.Transition(GameController.ControllerType.SocialSpace);
			}, delegate
			{
			}), GUIControl.ModalLevelEnum.Default);
		}
	}

	public BattleCard FindCardByIndex(int idx)
	{
		foreach (CardPile stack in stacks)
		{
			BattleCard battleCard = stack.Lookup(idx);
			if (battleCard != null)
			{
				return battleCard;
			}
		}
		return null;
	}

	public void FindCardAndPile(int idx, out BattleCard card, out CardPile loc)
	{
		card = null;
		loc = null;
		foreach (CardPile stack in stacks)
		{
			BattleCard battleCard = stack.Lookup(idx);
			if (battleCard != null)
			{
				card = battleCard;
				loc = stack;
				break;
			}
		}
	}

	public void IdentifyCard(BattleCard card, string cardType)
	{
		CspUtils.DebugLog("IdentifyCard() called");

		BattleCard battleCard = Deck.Find(delegate(BattleCard c)
		{
			return c.Type == card.Type;
		});
		if (battleCard == null)
		{
			battleCard = new BattleCard(cardType);
		}

		Renderer renderer = card.CardObj.GetComponentInChildren(typeof(Renderer)) as Renderer;
		CspUtils.DebugLog("setting mainTexture to " + battleCard.FullTexture);  // CSP
		renderer.materials[renderer.materials.Length - 1].mainTexture = battleCard.FullTexture;

		//////////  black added by CSP for testing /////////////////
		if (battleCard.Name != null) {
			CspUtils.DebugLog("ApplyCardTexture battleCard.Name : " + battleCard.Name);
		}
		else {
			CspUtils.DebugLog("ApplyCardTexture battleCard.Name is null! ");
		}
		if (battleCard.FullTexture != null) {
			battleCard.MiniTexture = battleCard.FullTexture;
		}
		else {
			CspUtils.DebugLog("ApplyCardTexture battleCard.FullTexture is null! ");
		}
		///////////////////////////////////////////////////////////////

		card.AttackPattern = battleCard.AttackPattern;
		card.InternalHeroName = battleCard.InternalHeroName;
		card.KeeperPattern = battleCard.KeeperPattern;
		card.Name = battleCard.Name;
		card.NameEng = battleCard.NameEng;
		card.Level = battleCard.Level;
		card.FullTexture = battleCard.FullTexture;
		card.MiniTexture = battleCard.MiniTexture;
		card.AttackFactors = battleCard.AttackFactors;
		card.BlockFactors = battleCard.BlockFactors;
		card.Damage = battleCard.Damage;
		card.Identified = true;
		card.Type = cardType;
		card.PreventKO = battleCard.PreventKO;
	}

	protected void QueueAnimation(string label, CardGameAnimationQueue.CardGameAnimDelegate fn, bool proceed)
	{
		CspUtils.DebugLog("animQ enqueing: " + label);
		CardGameController.Instance.animQ.Enqueue(label, fn, proceed);
	}

	public void AnimPause(float seconds)
	{
		StartCoroutine(CoAnimPause(seconds));
	}

	public void AnimProceed()
	{
		AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.AnimFinished());
	}

	private IEnumerator CoAnimPause(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		AnimProceed();
	}

	private IEnumerator CoAnimPauseNoFinish(float seconds)
	{
		yield return new WaitForSeconds(seconds);
	}

	public bool HasPreventKOKeeper()
	{
		foreach (BattleCard keeper in Keepers)
		{
			if (keeper.PreventKO)
			{
				return true;
			}
		}
		return false;
	}

	public virtual void SendReady(int questID, string questCond, Matchmaker2.Ticket ticket)
	{
		ArrayList arrayList = new ArrayList(10);
		arrayList.Add(info.PlayerID.ToString());
		arrayList.Add(Deck.DeckRecipe);
		arrayList.Add(Info.Hero);
		arrayList.Add(questCond);
		arrayList.Add(questID.ToString());
		DataWarehouse dataWarehouse = new DataWarehouse(ticket.ticket);
		dataWarehouse.Parse();
		string @string = dataWarehouse.GetString("ticket/my_deck_code");
		string value = dataWarehouse.TryGetString("ticket/ai_deck_code", "NULL");
		string value2 = dataWarehouse.TryGetString("ticket/ai_deck", "NULL");
		string string2 = dataWarehouse.GetString("ticket/hero_code");
		string string3 = dataWarehouse.GetString("ticket/player_id");
		arrayList.Add(@string);
		arrayList.Add(string2);
		arrayList.Add(value2);
		arrayList.Add(value);
		arrayList.Add(string3);
		RoomAgent.SendMessage("Ready", arrayList);
	}

	public virtual void SendPickCard(BattleCard card, bool pass, PickCardType pickCardType)
	{
		/////////// block added by CSP   ////////////////
		CspUtils.DebugLog("Base-SendPickCard() called!");
		if (info.Type == PlayerType.AI) {
			CardGameController.Instance.aiPickedCard = card;
		}
		if (info.Type == PlayerType.Human) {
			CardGameController.Instance.playerPickedCard = card;
		}
		////////////////////////////////////////////////

		ArrayList arrayList = new ArrayList(3);
		arrayList.Add(info.PlayerID.ToString());
		arrayList.Add((card != null) ? card.ServerID.ToString() : string.Empty);
		arrayList.Add(pass.ToString());
		RoomAgent.SendMessage("CardPicked", arrayList);
		QueueAnimation("SendPickCard", delegate
		{
			if (pass)
			{
				CardGameController.Instance.AudioManager.Play(CardGameAudioManager.SFX.PassButtonClick);
				AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.DealerChat(CardGameEvent.DealerChat.MessageType.Dealer, 503, CardLocale.Parse(503), info.Name));
				CspUtils.DebugLog("Firing Dealerchat Player Pass Message!");
			}
			else if (pickCardType == PickCardType.Attack)
			{
				AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.DealerChat(CardGameEvent.DealerChat.MessageType.Dealer, 504, CardLocale.Parse(504), info.Name, card.Name));
			}
			else if (pickCardType == PickCardType.Block)
			{
				AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.DealerChat(CardGameEvent.DealerChat.MessageType.Dealer, 505, CardLocale.Parse(505), info.Name, card.Name));
			}
			else if (pickCardType == PickCardType.Discard)
			{
				AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.DealerChat(CardGameEvent.DealerChat.MessageType.Dealer, 510, CardLocale.Parse(510), info.Name, card.Name));
			}
			if (pickCardType == PickCardType.Block)
			{
				foreach (BattleCard item in Played)
				{
					item.CardProps.PlayHighlightFactor(false);
				}
				foreach (BattleCard keeper in Keepers)
				{
					keeper.CardProps.PlayHighlightFactor(false);
				}
				foreach (BattleCard item2 in opponent.Played)
				{
					item2.CardProps.PlayHighlightFactor(false);
				}
				foreach (BattleCard keeper2 in opponent.Keepers)
				{
					keeper2.CardProps.PlayHighlightFactor(false);
				}
			}
			if (!pass && pickCardType == PickCardType.Destroy && (Keepers.Contains(card) || opponent.Keepers.Contains(card)))
			{
				card.CardProps.PlayKeeperDestroy(true);
			}

			/////////// block added by CSP   ////////////////
			if (info.Type == PlayerType.AI) {
				if (CardGameController.Instance.offenseNdx == 1)
					AppShell.Instance.StartCoroutine(CardGameController.Instance.SimulateServer_AfterOffensePick_CSP(pass)); 
				else
					AppShell.Instance.StartCoroutine(CardGameController.Instance.SimulateServer_AfterDefensePick_CSP(pass)); 
			}
			//////////////////////////////////////////////////
		}, true);

		
	}

	public virtual void SendPickFactor(BattleCard.Factor factor)
	{
		char c = BattleCard.FactorToChar(factor);
		ArrayList arrayList = new ArrayList(2);
		arrayList.Add(info.PlayerID.ToString());
		arrayList.Add(c);
		RoomAgent.SendMessage("FactorPicked", arrayList);
		CspUtils.DebugLog("Sending FactorPicked message with factor " + c);
	}

	public virtual void SendPickYesNo(bool yes)
	{
		ArrayList arrayList = new ArrayList(2);
		arrayList.Add(info.PlayerID.ToString());
		arrayList.Add((!yes) ? "0" : "1");
		RoomAgent.SendMessage("YesNoPicked", arrayList);
		CspUtils.DebugLog("Sending YesNoPicked message with yes " + yes);
	}

	public virtual void SendPickNumber(int number)
	{
		ArrayList arrayList = new ArrayList(2);
		arrayList.Add(info.PlayerID.ToString());
		arrayList.Add(number.ToString());
		RoomAgent.SendMessage("NumberPicked", arrayList);
		CspUtils.DebugLog("Sending NumberPicked message with number " + number);
	}

	public virtual void SendDebug(int code, int arg1, int arg2)
	{
		ArrayList arrayList = new ArrayList(4);
		arrayList.Add(info.PlayerID.ToString());
		arrayList.Add(code.ToString());
		arrayList.Add(arg1.ToString());
		arrayList.Add(arg2.ToString());
		RoomAgent.SendMessage("Debug", arrayList);
	}

	public virtual void SendPoke()
	{
		ArrayList arrayList = new ArrayList(1);
		arrayList.Add(info.PlayerID.ToString());
		RoomAgent.SendMessage("Poke", arrayList);
	}

	public virtual void OnBlock(int weaponID, int blockerID, bool meDefending)
	{
		CardGamePlayer player = (!meDefending) ? opponent : this;
		bool flag = false;
		CspUtils.DebugLog("Player " + info.PlayerID + " got OnBlock");
		if (!meDefending && player.info.Type != PlayerType.Network)
		{
			return;
		}
		BattleCard weapon;
		CardPile pile;
		player.opponent.FindCardAndPile(weaponID, out weapon, out pile);
		if (weapon == null)
		{
			player.FindCardAndPile(weaponID, out weapon, out pile);
			if (weapon == null)
			{
				CspUtils.DebugLog("OnDamage weapon could not be found in either player's piles");
				return;
			}
			flag = true;
		}
		BattleCard blocker = player.FindCardByIndex(blockerID);
		QueueAnimation("Playing blocked effect", delegate
		{
			blocker.CardProps.PlayAttackBlocked();
			StartCoroutine(CoAnimPause(0.5f));
		}, false);
		QueueAnimation("Stopping factor highlight", delegate
		{
			weapon.CardProps.PlayHighlightFactor(false);
		}, true);
		QueueAnimation("Playing block clash", delegate
		{
			bool flag2 = player == CardGameController.Instance.players[0];
			CardGameController.Instance.AudioManager.Play(CardGameAudioManager.SFX.BlockClash);
			if (pile != player.Keepers && pile != player.opponent.Keepers)
			{
				if (flag2)
				{
					CardGameController.Instance.AudioManager.Play("Block");
				}
				else
				{
					CardGameController.Instance.AudioManager.Play("Blocked");
				}
			}
			weapon.CardProps.PlayBlockClash(!flag2);
			blocker.CardProps.PlayBlockClash(flag2);
		}, true);
		QueueAnimation("Playing lightning effect", delegate
		{
			CardGameController.Instance.Hud.PlayCardClash(weapon, blocker);
		}, true);
		QueueAnimation("Block Message Blast", delegate
		{
			AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.AttackResult((player.info.Type != 0) ? 1 : 0, CardGameEvent.AttackResultType.HandBlock, false, false, 0));
			StartCoroutine(CoAnimPause(1f));
		}, false);
		if (!flag)
		{
			QueueAnimation("PlayBlockAnimation", delegate
			{
				CardGameController.Instance.PlayBlockAnimation(player, weapon, blocker);
				StartCoroutine(CoAnimPause(1f));
			}, false);
		}
	}

	public virtual void OnBuff(int p, int p_2, bool p_3, int p_4)
	{
		throw new NotImplementedException();
	}

	public virtual void OnGameOver(int reason, bool iWon)
	{
		CspUtils.DebugLog("Player " + info.PlayerID + " got GameOver: " + reason + " " + iWon);
		if (playerWaitDialog != null)
		{
			playerWaitDialog.Hide();
		}
	}

	public virtual void OnDamage(int weaponID, List<int> casualties, int srcDEPRECATED, bool mine, List<string> typeList, int inflicted, int attempted, bool becomeKeeper, bool killKeeper)
	{
		CspUtils.DebugLog(string.Format("Player {0} got Damage: card {1}, inflicted: {2}, attempted: {3}, casualties: ({4})", info.PlayerID, weaponID, inflicted, attempted, string.Join(",", typeList.ToArray())));
		CardGamePlayer player = (!mine) ? opponent : this;
		BattleCard weapon = player.opponent.FindCardByIndex(weaponID);
		int num = 0;
		if (weapon == null)
		{
			weapon = player.FindCardByIndex(weaponID);
			if (weapon == null)
			{
				CspUtils.DebugLog("OnDamage weapon could not be found in either player's piles");
				return;
			}
		}
		if (!mine && player.info.Type != PlayerType.Network)
		{
			return;
		}
		RoomAgent.Suspend();
		QueueAnimation("Stop highlight factor", delegate
		{
			weapon.CardProps.PlayHighlightFactor(false);
		}, true);
		player.damageAccumulator = new DamageAccumulator();
		player.damageAccumulator.attemptedDamage = attempted;
		player.damageAccumulator.inflictedDamage = inflicted;
		player.damageAccumulator.weapon = weapon;
		player.damageAccumulator.useJeopardyMeter = (attempted > 1);
		if (player.damageAccumulator.useJeopardyMeter)
		{
			QueueAnimation("Growing jeopardy meter", delegate
			{
				float num2 = 0.75f;
				player.JeopardyMeterScript.Grow(attempted, 0, weapon.AttackFactors[0]);
				float animTime = num2 * 0.8f;
				if (inflicted == attempted || (player.Stock.Count > 0 && player.Stock.Count - inflicted > 0))
				{
					player.EdgePanel.Retreat(animTime);
				}
				player.opponent.PlayPanel.Retreat(animTime);
				player.opponent.EdgePanel.Retreat(animTime);
				player.opponent.SidePanel.Retreat(animTime);
				SHSCardGameMainWindow sHSCardGameMainWindow = GUIManager.Instance["/SHSMainWindow/SHSCardGameMainWindow"] as SHSCardGameMainWindow;
				sHSCardGameMainWindow.RetreatHud();
				CardGameController.Instance.Hud.ShowCoinPanel(false);
				if (inflicted < attempted && player.Stock.Count - inflicted == 0)
				{
					sHSCardGameMainWindow.ShowDealerChat();
				}
				StartCoroutine(CoAnimPause(num2));
			}, false);
		}
		int additionalHealth = 0;
		if (player.Played.Count == 1 && weapon.AttackFactors.Length == 2 && player.Played[0] != weapon)
		{
			BattleCard battleCard = player.Played[0];
			casualties.Insert(0, battleCard.ServerID);
			typeList.Insert(0, battleCard.Type);
			additionalHealth++;
		}
		if (!player.Keepers.Contains(weapon))
		{
			QueueAnimation("PlayAttackAnim", delegate
			{
				CardGameController.Instance.PlayAttackAnimation(player, weapon, inflicted, player.opponent.Keepers.Contains(weapon), becomeKeeper, killKeeper, player.Stock.Count + player.Hand.Count + additionalHealth);
			}, true);
		}
		else
		{
			QueueAnimation("PlayFlinchAnim", delegate
			{
				CardGameController.Instance.PlayRecoilAnimation(player);
			}, true);
		}
		CardPile loc = null;
		foreach (int casualty in casualties)
		{
			BattleCard card;
			player.FindCardAndPile(casualty, out card, out loc);
			if (card == null)
			{
				CspUtils.DebugLog("Card " + casualty + "(" + typeList[num] + ") was not found in any stack ");
				break;
			}
			if (card.Type.Length < 1 && typeList[num].Length > 0)
			{
				player.IdentifyCard(card, typeList[num]);
			}
			player.damageAccumulator.damagedCards.Add(card);
			CardMoveData moveData = default(CardMoveData);
			moveData.player = player;
			moveData.playerType = player.info.Type;
			moveData.card = card;
			moveData.srcPile = loc;
			moveData.destPile = ((!player.damageAccumulator.useJeopardyMeter) ? player.Discard : player.Jeopardy);
			moveData.startFaceUp = card.Identified;
			moveData.endFaceUp = true;
			moveData.secondsPause = 0.05f;
			moveData.secondsDelay = 0f;
			moveData.proceed = false;
			num++;
			QueueAnimation("CardFlip " + card.ServerID + " to Jeopardy", delegate
			{
				StartCoroutine(AnimCardMoveTime(moveData, true));
				StartCoroutine(CoAnimPause(0.5f));
			}, false);
			if (player.damageAccumulator.useJeopardyMeter)
			{
				int cardCount = num;
				QueueAnimation("Slider animation", delegate
				{
					player.JeopardyMeterScript.slider.MoveTo(cardCount, cardCount);
					StartCoroutine(CoAnimPause(0.4f));
				}, false);
			}
		}
		if (player.damageAccumulator.useJeopardyMeter && player.damageAccumulator.inflictedDamage > 0)
		{
			try {
				int index = player.damageAccumulator.inflictedDamage - 1;
				index = index - 1; // added by CSP for testing
				CspUtils.DebugLog("index=" + index);
				BattleCard blockingCard = player.damageAccumulator.damagedCards[index];
				if (weapon.AttackFactors[0] == blockingCard.BlockFactors[0])
				{
					QueueAnimation("Flashing block icon", delegate
					{
						blockingCard.CardProps.PlayAttackBlocked();
						StartCoroutine(CoAnimPause(1.5f));
					}, false);
				}
			}
			catch (Exception e) {
				CspUtils.DebugLogError(e.ToString());
				CspUtils.DebugLog("player.damageAccumulator.damagedCards.Count=" + player.damageAccumulator.damagedCards.Count);
			}
			
		}

		AppShell.Instance.StartCoroutine(CardGameController.Instance.SimulateServer_AfterDamage_CSP());  // CSP
	}

	public void OnDamageComplete(bool mine)
	{
		CardGamePlayer player = (!mine) ? opponent : this;
		bool inflictedOnOpponent = player == CardGameController.Instance.players[1];
		if (!mine && player.info.Type != PlayerType.Network)
		{
			return;
		}
		if (player.damageAccumulator == null)
		{
			return;
		}
		int num = player.Stock.Count + player.Hand.Count - player.damageAccumulator.inflictedDamage;
		bool luckyBlock = player.damageAccumulator.attemptedDamage > 1 && player.damageAccumulator.inflictedDamage == 1 && !player.opponent.Keepers.Contains(player.damageAccumulator.weapon) && num > 0;
		QueueAnimation("Dispatching attack result", delegate
		{
			AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.AttackResult((player.info.Type != 0) ? 1 : 0, (!luckyBlock) ? CardGameEvent.AttackResultType.Damage : CardGameEvent.AttackResultType.LuckyBlock, true, false, player.damageAccumulator.inflictedDamage));
		}, true);
		QueueAnimation("Announcer: Damage", delegate
		{
			CardGameController.Instance.AudioManager.PlayAnnouncerDamage(player.damageAccumulator.inflictedDamage, inflictedOnOpponent);
		}, true);
		if (luckyBlock)
		{
			if (inflictedOnOpponent)
			{
				QueueAnimation("Announcer: Lucky block (bad)", delegate
				{
					CardGameController.Instance.AudioManager.Play("LuckyBlockBad");
				}, true);
			}
			else
			{
				QueueAnimation("Announcer: Lucky block (good)", delegate
				{
					CardGameController.Instance.AudioManager.Play("LuckyBlockGood");
				}, true);
			}
		}
		if (player.Stock.Count == 0 && player.stockCountFromLastDamage > 0 && player.Hand.Count > 0)
		{
			if (!inflictedOnOpponent)
			{
				CardGameController.Instance.AudioManager.Play("Danger");
			}
			else
			{
				CardGameController.Instance.AudioManager.Play("AlmostThere");
			}
		}
		player.stockCountFromLastDamage = player.Stock.Count;
		if (player.damageAccumulator.useJeopardyMeter)
		{
			QueueAnimation("JeopardyPilePause", delegate
			{
				StartCoroutine(CoAnimPause(1.5f));
			}, false);
			QueueAnimation("Playing jeopardy discard SFX", delegate
			{
				CardGameController.Instance.AudioManager.Play(CardGameAudioManager.SFX.JeopardyToDiscardBG);
			}, true);
			int i = 0;
			foreach (BattleCard damagedCard in player.damageAccumulator.damagedCards)
			{
				i++;
				CardMoveData moveData = default(CardMoveData);
				moveData.player = player;
				moveData.playerType = player.Info.Type;
				moveData.card = damagedCard;
				moveData.srcPile = player.Jeopardy;
				moveData.destPile = player.Discard;
				moveData.startFaceUp = true;
				moveData.endFaceUp = true;
				moveData.secondsPause = 0f;
				moveData.secondsDelay = (float)i * 0.2f;
				moveData.proceed = false;
				moveData.sfx = CardGameAudioManager.SFX.JeopardyToDiscardCard;
				QueueAnimation("CardFlip " + damagedCard.ServerID + " to Discard", delegate
				{
					StartCoroutine(player.AnimCardMoveTime(moveData, false));
				}, true);
			}
			QueueAnimation("Pausing before Jeopardy collapse", delegate
			{
				StartCoroutine(CoAnimPause(0.7f + (float)i * 0.2f));
			}, false);
			float collapseTime = 0.75f;
			float postCollapseDelay = 0.4f;
			QueueAnimation("Collapsing jeopardy meter", delegate
			{
				player.JeopardyMeterScript.Collapse();
				StartCoroutine(CoAnimPause(collapseTime + postCollapseDelay));
			}, false);
			QueueAnimation("Advancing UI", delegate
			{
				player.ShowAllPanels();
				player.opponent.ShowAllPanels();
				SHSCardGameMainWindow sHSCardGameMainWindow = GUIManager.Instance["/SHSMainWindow/SHSCardGameMainWindow"] as SHSCardGameMainWindow;
				sHSCardGameMainWindow.AdvanceHud();
				CardGameController.Instance.Hud.ShowCoinPanel(true);
			}, true);
			QueueAnimation("Removing damage accumulator", delegate
			{
				player.damageAccumulator = null;
			}, true);
			QueueAnimation("Waiting for UI", delegate
			{
				StartCoroutine(CoAnimPause(0.5f));
			}, false);
			player.damageAccumulator.finished = true;
		}
	}

	public virtual void OnHighlight(int animType, int cardID, bool activate)
	{
		BattleCard card = FindCardByIndex(cardID);
		if (card == null)
		{
			card = opponent.FindCardByIndex(cardID);
		}
		if (card != null)
		{
			QueueAnimation("Highlight", delegate
			{
				card.CardProps.PlayKeeperActivate(activate);
			}, true);
		}
	}

	public virtual void OnAnim(int type, int arg1, int arg2)
	{
		CspUtils.DebugLog(string.Format("Player {0} got Anim {1}; arg1: {2}, arg2: {3}", info.PlayerID, type, arg1, arg2));
		CardGamePlayer player;
		BattleCard card;
		switch (type)
		{
		case 65:
		case 68:
			break;
		case 64:
			player = ((arg2 != 1) ? opponent : this);
			card = player.FindCardByIndex(arg1);
			if (card != null && ShouldIAnimate(player, arg2))
			{
				if (stacks[3].Contains(card))
				{
					QueueAnimation("Announcer: KeeperActivate", delegate
					{
						CardGameController.Instance.AudioManager.Play(CardGameAudioManager.Announcer.KeeperActivate);
					}, true);
				}
				QueueAnimation("ApproachTarget", delegate
				{
					CardGameController.Instance.PlayApproachAnimation(player, card);
				}, true);
				QueueAnimation("Pause 2", delegate
				{
					AnimPause(1f);
				}, false);
			}
			break;
		case 66:
			if (arg2 == 1)
			{
				QueueAnimation("Highlight attack", delegate
				{
					CardPile loc;
					FindCardAndPile(arg1, out card, out loc);
					if (card != null)
					{
						if (opponent.Info.Type == PlayerType.Network)
						{
							card.CardProps.PlayHighlightFactor(true);
						}
						for (int i = 0; i < loc.Count - 1; i++)
						{
							BattleCard battleCard = loc[i];
							battleCard.CardProps.PlayHighlightFactor(false);
						}
						BattleCard.Factor[] attackFactors = card.AttackFactors;
						if (attackFactors.Length > 1)
						{
							CardGameController.Instance.AudioManager.Play(CardGameAudioManager.SFX.FactorMulti);
						}
						else
						{
							CardGameController.Instance.AudioManager.PlayFactorAttack(attackFactors[0]);
						}
						if (loc != Keepers)
						{
							attacksThisTurn++;
							if (attacksThisTurn > 1)
							{
								CardGameController.Instance.AudioManager.Play("BonusAttack");
							}
						}
					}
				}, true);
			}
			break;
		case 67:
			player = ((arg2 != 1) ? opponent : this);
			card = player.FindCardByIndex(arg1);
			if (ShouldIAnimate(player, arg2))
			{
				QueueAnimation("Announcer: KeeperActivate", delegate
				{
					CardGameController.Instance.AudioManager.Play(CardGameAudioManager.Announcer.KeeperActivate);
				}, true);
				QueueAnimation("KeeperNoDamage", delegate
				{
					CardGameController.Instance.PlayKeeperNoDamage(player, card);
				}, true);
				QueueAnimation("Pause 2", delegate
				{
					AnimPause(1f);
				}, false);
			}
			break;
		case 69:
			player = ((arg2 != 1) ? opponent : this);
			card = player.FindCardByIndex(arg1);
			if (card != null && ShouldIAnimate(player, arg2))
			{
				SquadBattleCharacterController.Instance.AddKeeper((player.Info.Type != 0) ? SquadBattlePlayerEnum.Right : SquadBattlePlayerEnum.Left, card.InternalHeroName[0]);
			}
			break;
		}
	}

	protected bool ShouldIAnimate(CardGamePlayer player, int effectIsMine)
	{
		return effectIsMine == 1 || player.Info.Type == PlayerType.Network;
	}

	public virtual void OnInfo(int message, int arg1, int arg2)
	{
		CspUtils.DebugLog(string.Format("Player {0} got Info: {1}, with args {2} and {3}", info.PlayerID, message, arg1, arg2));
		if (message == 142)
		{
			bool mine = arg1 == 1;
			OnDamageComplete(mine);
		}
	}

	// method added by CSP
	public void OnInitCards_CSP(bool mine)
	{
		int min = 1;
		int max = this.Deck.Count;

		CspUtils.DebugLog("Player " + info.PlayerID + " got InitCards_CSP: " + min + ", " + max);
		//if (mine || opponent.Info.Type == PlayerType.AI)
		//{
			//CspUtils.DebugLog("Creating dummy cards for player " + ((!mine) ? opponent : this).Info.PlayerID);
			CspUtils.DebugLog("Creating dummy cards for player " +  this.Info.PlayerID);
			int num = 0;
			for (int i = min; i < max; i++)
			{
				BattleCard card = new BattleCard();
				card.Copy(this.Deck[i]);
				card.ServerID = i;
				Stock.CreateDummy(i, card, CardGameController.Instance.fullCardPrefab);
				//((!mine) ? opponent.Stock : Stock).CreateDummy(i, card, CardGameController.Instance.fullCardPrefab);
				//((!mine) ? opponent.Stock : Stock).CreateDummy(i, new BattleCard(i), CardGameController.Instance.fullCardPrefab);
				num++;
			}
			CspUtils.DebugLog("Created " + num + " dummies");
			//if (mine)
			//{
				deckCounterComponent.count += num;
			//}
			//else
			//{
			//	opponent.deckCounterComponent.count += num;
			//}
		//}
	}

	public void OnInitCards(int min, int max, bool mine)
	{
		CspUtils.DebugLog("Player " + info.PlayerID + " got InitCards: " + min + ", " + max);
		if (mine || opponent.Info.Type == PlayerType.Network)
		{
			CspUtils.DebugLog("Creating dummy cards for player " + ((!mine) ? opponent : this).Info.PlayerID);
			int num = 0;
			for (int i = min; i <= max; i++)
			{
				((!mine) ? opponent.Stock : Stock).CreateDummy(i, new BattleCard(i), CardGameController.Instance.fullCardPrefab);
				num++;
			}
			CspUtils.DebugLog("Created " + num + " dummies");
			if (mine)
			{
				deckCounterComponent.count += num;
			}
			else
			{
				opponent.deckCounterComponent.count += num;
			}
		}
	}

	public virtual void OnMoveCard(int cardID, int src, int dest, bool mine, string cardType, int visibility, bool srcOpponent)
	{
		CspUtils.DebugLog(string.Format("Player {0} ({1}) got MoveCard: {2} from {3} to {4}, cardType {5}", info.PlayerID, mine, cardID, src, dest, cardType));
		CardGamePlayer player = (!mine) ? opponent : this;
		if (src == 6)
		{
			src = 4;
		}
		if (dest == 6)
		{
			dest = 4;
		}
		if ((mine || player.info.Type != PlayerType.AI) && (!mine || player.info.Type == PlayerType.AI) && player.info.Type != PlayerType.Network)
		{
			return;
		}
		BattleCard card;
		CardPile loc;
		player.FindCardAndPile(cardID, out card, out loc);
		if (card == null)
		{
			CspUtils.DebugLog("Card " + cardID + " (" + cardType + ") was not found");
			return;
		}
		bool flag = player.damageAccumulator != null && !player.damageAccumulator.finished && player.damageAccumulator.useJeopardyMeter;
		if (src == 1 && dest == 5 && player.damageAccumulator != null)
		{
			if (flag)
			{
				dest = 7;
			}
			player.damageAccumulator.inflictedDamage++;
			player.damageAccumulator.damagedCards.Add(card);
		}
		card.CardProps.PlayKeeperActivate(false);
		bool flag2 = false;
		CspUtils.DebugLog("cardType.Length = " + cardType.Length);
		if (cardType.Length > 0)
		{
			CspUtils.DebugLog("card.Type = " + card.Type);
			if (card.Type.Length < 1)
			{
				player.IdentifyCard(card, cardType);
				flag2 = true;
			}
			if (player.info.Type == PlayerType.AI)
			{
				if ((dest == 1 && src == 1) || dest == 2 || dest == 4 || dest == 5)
				{
					card.Identified = true;
				}
				else if (dest == 1)
				{
					card.Identified = false;
				}
			}
		}
		else if (card.Type.Length > 0)
		{
			CspUtils.DebugLog("Card " + cardID + " (" + card.Type + ") was known, is now unknown.");
			card.Identified = false;
			card.Type = string.Empty;
		}
		else if (src == dest)
		{
			CspUtils.DebugLog("ignore");
			return;
		}
		if (dest == 3)
		{
			CardGameController.Instance.QueueDealerChat(509, card.Name);
		}
		if (dest == 5 && src == 3)
		{
			QueueAnimation("FX: Waiting on Keeper Destroy effect", delegate
			{
				StartCoroutine(CoAnimPause(0.5f));
			}, false);
		}
		if (src == 5 && dest == 0)
		{
			QueueAnimation("SFX: Heal", delegate
			{
				CardGameController.Instance.AudioManager.Play(CardGameAudioManager.SFX.Heal);
			}, true);
		}
		if (src == 0 && dest == 1 && player.Stock.Count == 1)
		{
			QueueAnimation("VO: LastCard", delegate
			{
				if (player == CardGameController.Instance.players[0])
				{
					CardGameController.Instance.AudioManager.Play("Danger");
				}
				else
				{
					CardGameController.Instance.AudioManager.Play("AlmostThere");
				}
			}, true);
		}
		float num = 0.2f;
		float num2 = 0f;
		bool dealCard = loc.Equals(player.Stock) && card.Identified && src != dest && flag2;
		bool fastDeal = !CardGameController.Instance.GameStarted;
		bool flag3 = src == 8 || dest == 8;
		CardMoveData moveData = default(CardMoveData);
		moveData.player = player;
		moveData.playerType = player.info.Type;
		moveData.card = card;
		moveData.srcPile = ((!srcOpponent) ? player.stacks[src] : player.opponent.stacks[src]);
		moveData.destPile = player.stacks[dest];
		moveData.endFaceUp = card.Identified;
		moveData.secondsPause = ((!flag3) ? num : 0f);
		moveData.secondsDelay = ((!flag3) ? num2 : 0f);
		moveData.proceed = !fastDeal;
		QueueAnimation("OnMoveCard " + card.ServerID + " from pile " + src + " to pile " + dest, delegate
		{
			StartCoroutine(AnimCardMoveTime(moveData, dealCard));
			if (fastDeal)
			{
				StartCoroutine(CoAnimPause(0.3f));
			}
		}, false);
		if (dest == 7)
		{
			QueueAnimation("Slider animation", delegate
			{
				player.JeopardyMeterScript.slider.MoveTo(player.damageAccumulator.inflictedDamage, player.damageAccumulator.inflictedDamage);
				StartCoroutine(CoAnimPause(0.4f));
			}, false);
		}
	}

	public IEnumerator AnimCardMoveTime(CardMoveData moveData, bool DealCard)
	{
		BattleCard Card = moveData.card;
		CardPile oldPile = moveData.srcPile;
		CardPile newPile = moveData.destPile;
		float SecondsPause = moveData.secondsPause;
		CardGamePlayer player = moveData.player;
		bool samePile = oldPile == newPile;
		CspUtils.DebugLog("card " + Card.NameEng + " player " + moveData.player + "  oldpile " + oldPile.LayoutProperties + "  newpile " + newPile.LayoutProperties + " CardFaceup = " + Card.Identified + " DealCard = " + DealCard + " SamePile = " + samePile);
		if (!oldPile.Contains(Card))
		{
			CspUtils.DebugLog("Old pile does not contain card");
			DealCard = false;
			CardPile currentPile = oldPile;
			player.FindCardAndPile(Card.ServerID, out Card, out currentPile);
			if (currentPile == oldPile)
			{
				if (moveData.proceed)
				{
					AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.AnimFinished());
				}
				yield break;
			}
			currentPile.Remove(Card);
			oldPile.Add(Card);
			//CspUtils.DebugLog("cardobj mainTexture (mini)= " + Card.CardObj.renderer.materials[Card.CardObj.renderer.materials.Length - 1].mainTexture);
			Card.CardObj.transform.parent = oldPile.GameObj.transform.parent;
			Card.CardObj.transform.localPosition = oldPile.NextPosition();
			Card.CardObj.transform.localRotation = oldPile.NextRotation();
			Card.CardObj.transform.localScale = oldPile.NextScale();
		}
		while (moveData.secondsDelay > 0f)
		{
			yield return 0;
			moveData.secondsDelay -= Time.deltaTime;
		}
		bool faceUp = Card.Faceup = moveData.endFaceUp;
		Card.Moving = true;
		Card.IsVisible = true;
		if (newPile == Discard || newPile == Keepers)
		{
			Card.CardProps.StopAllEffects();
		}
		newPile.PlayEffect();
		if (moveData.sfx != 0)
		{
			CardGameController.Instance.AudioManager.Play(moveData.sfx);
		}
		if (oldPile == player.Stock)
		{
			oldPile.Remove(Card);
			oldPile.Add(Card);
			oldPile.RefreshLayout();
		}
		//CspUtils.DebugLog("cardobj mainTexture (mini)= " + Card.CardObj.renderer.materials[Card.CardObj.renderer.materials.Length - 1].mainTexture);			
		Card.CardObj.transform.parent = newPile.GameObj.transform.parent;
		//CspUtils.DebugLog("cardobj mainTexture (mini)= " + Card.CardObj.renderer.materials[Card.CardObj.renderer.materials.Length - 1].mainTexture);			
		Vector3 StartPosition = Card.CardObj.transform.localPosition;
		Quaternion StartRotation2 = Card.CardObj.transform.localRotation;
		Vector3 StartScale2 = Card.CardObj.transform.localScale;
		Vector3 EndPosition = StartPosition;
		Quaternion EndRotation = StartRotation2;
		Vector3 EndScale = StartScale2;
		bool fadeInKeepers = false;
		bool fadeOutKeepers = false;
		if (player.KeeperTrayOpacity != null)
		{
			if (player.Keepers.Count == 1 && oldPile == player.Keepers)
			{
				fadeOutKeepers = true;
			}
			else if (player.Keepers.Count == 0 && newPile == player.Keepers)
			{
				fadeInKeepers = true;
			}
		}
		bool isEndCard = oldPile[oldPile.Count - 1] == Card;
		if (!samePile)
		{
			oldPile.Remove(Card);
			if (oldPile == player.Stock)
			{
				player.deckCounterComponent.count = player.Stock.Count;
			}
			EndPosition = newPile.NextPosition();
			EndRotation = newPile.NextRotation();
			EndScale = newPile.NextScale();
			newPile.Add(Card);
			Card.SetSelectable();
		}
		float SecondsDuration4 = 1f;
		string animName;
		float ElapsedTime3;
		if (DealCard)
		{
			Card.Animation.Play("flip");
			SecondsDuration4 = Card.Animation["flip"].length;
			ElapsedTime3 = 0f;
			float endTime2 = SecondsDuration4 + SecondsPause;
			while (ElapsedTime3 < endTime2)
			{
				float PercentComplete3 = ElapsedTime3 / (SecondsDuration4 * 2f);
				if (PercentComplete3 > 0.5f)
				{
					PercentComplete3 = 0.5f;
				}
				ElapsedTime3 += Time.deltaTime;
				Card.CardObj.transform.localPosition = Vector3.Lerp(StartPosition, EndPosition, PercentComplete3);
				Card.CardObj.transform.localRotation = Quaternion.Lerp(StartRotation2, EndRotation, PercentComplete3);
				Card.CardObj.transform.localScale = Vector3.Lerp(StartScale2, EndScale, PercentComplete3);
				yield return 0;
			}
			lastCardMovementWasReveal = false;
			animName = "drop";
		}
		else if (samePile && (oldPile == player.Hand || oldPile == player.Stock))
		{
			if (faceUp)
			{
				Card.SetRevealTimestamp();
				string anim = "flip";
				if (!lastCardMovementWasReveal)
				{
					CardGameController.Instance.AudioManager.Play(CardGameAudioManager.SFX.CardReveal);
					CardGameController.Instance.AudioManager.Play("Reveal");
				}
				lastCardMovementWasReveal = true;
				StartPosition = Card.CardObj.transform.localPosition;
				StartRotation2 = Card.CardObj.transform.localRotation;
				StartScale2 = Card.CardObj.transform.localScale;
				Vector3 ShowPosition = StartPosition + new Vector3(0f, 0f, -5f);
				Vector3 ShowScale = StartScale2 * 1.5f;
				Card.Animation[anim].speed = 2f;
				Card.Animation.Play(anim);
				SecondsDuration4 = Card.Animation[anim].length / Card.Animation[anim].speed;
				ElapsedTime3 = 0f;
				float endTime = SecondsDuration4 + SecondsPause;
				while (ElapsedTime3 < endTime)
				{
					float PercentComplete3 = ElapsedTime3 / SecondsDuration4;
					if (PercentComplete3 > 1f)
					{
						PercentComplete3 = 1f;
						Card.Moving = false;
					}
					Card.CardObj.transform.localPosition = Vector3.Lerp(StartPosition, ShowPosition, PercentComplete3);
					Card.CardObj.transform.localScale = Vector3.Lerp(StartScale2, ShowScale, PercentComplete3);
					ElapsedTime3 += Time.deltaTime;
					yield return 0;
				}
				Card.Animation[anim].speed = 1f;
				Card.Moving = true;
				Card.CardObj.transform.parent = newPile.GameObj.transform.parent;
				CspUtils.DebugLog("cardobj mainTexture (mini)= " + Card.CardObj.renderer.materials[Card.CardObj.renderer.materials.Length - 1].mainTexture);
				StartPosition = Card.CardObj.transform.localPosition;
				StartRotation2 = Card.CardObj.transform.localRotation;
				StartScale2 = Card.CardObj.transform.localScale;
				animName = "drop";
			}
			else
			{
				while (!Card.IsRevealComplete())
				{
					yield return 0;
				}
				animName = "down_play";
			}
		}
		else
		{
			animName = newPile.CardAnimation;
			if (animName != string.Empty)
			{
				if (!faceUp && newPile != player.Stock)
				{
					animName = "down_" + animName;
				}
				if (animName.Equals("draw"))
				{
					animName = "play";
				}
				if (moveData.playerType != 0 && moveData.srcPile == player.Discard && moveData.destPile == player.Hand)
				{
					animName = "down_play";
				}
				if (moveData.playerType == PlayerType.Network && moveData.srcPile == player.Show && moveData.destPile == player.Stock && !faceUp)
				{
					animName = "down_draw";
				}
			}
			lastCardMovementWasReveal = false;
		}
		if (Card.Animation.GetClip(animName) != null)
		{
			if (animName == "down_play")
			{
				Card.Animation["down_play"].speed = -1f;
				Card.Animation["down_play"].time = Card.Animation["down_play"].length;
			}
			Card.Animation.Play(animName);
			SecondsDuration4 = Card.Animation[animName].length;
			if (SecondsDuration4 < 0.5f)
			{
				SecondsDuration4 = 0.5f;
			}
			if (animName == "play")
			{
				CspUtils.DebugLog(string.Format("Playing card: SecondsDuration {0}, secondsPause: {1}", SecondsDuration4, SecondsPause));
			}
		}
		else
		{
			CspUtils.DebugLog("This card did not have an animation clip called '" + animName + "' to play!");
			SecondsDuration4 = 0f;
		}
		bool shrinkTray = false;
		ScalingCardTray shrinkingTrayScript = null;
		float shrinkingTrayWidth = 0f;
		Transform scalingTrayXform2 = null;
		if (oldPile == player.Hand)
		{
			scalingTrayXform2 = oldPile.GameObj.transform.parent.Find("HandTray/ScalingTray");
		}
		if (oldPile == player.Keepers)
		{
			scalingTrayXform2 = oldPile.GameObj.transform.parent.Find("KeeperTray/ScalingTray");
		}
		if (scalingTrayXform2 != null && (oldPile.LayoutProperties.MaxCardsBeforeOverlapping == 0 || oldPile.Count <= oldPile.LayoutProperties.MaxCardsBeforeOverlapping))
		{
			shrinkingTrayScript = scalingTrayXform2.gameObject.GetComponent<ScalingCardTray>();
			shrinkTray = true;
			float num = oldPile.GetPileWidth() + ScalingCardTray.kEdgeSpacing;
			Vector3 localScale = scalingTrayXform2.localScale;
			float newWidth3 = num / localScale.x;
			shrinkingTrayWidth = newWidth3;
			if (isEndCard)
			{
				shrinkingTrayScript.Resize(newWidth3, SecondsDuration4);
			}
		}
		scalingTrayXform2 = null;
		if (newPile == player.Hand)
		{
			scalingTrayXform2 = newPile.GameObj.transform.parent.Find("HandTray/ScalingTray");
		}
		if (newPile == player.Keepers)
		{
			scalingTrayXform2 = newPile.GameObj.transform.parent.Find("KeeperTray/ScalingTray");
		}
		if (scalingTrayXform2 != null)
		{
			if (newPile.LayoutProperties.MaxCardsBeforeOverlapping == 0 || newPile.Count <= newPile.LayoutProperties.MaxCardsBeforeOverlapping)
			{
				ScalingCardTray cardTrayScript = scalingTrayXform2.gameObject.GetComponent<ScalingCardTray>();
				float num2 = newPile.GetPileWidth() + ScalingCardTray.kEdgeSpacing;
				Vector3 localScale2 = scalingTrayXform2.localScale;
				float newWidth = num2 / localScale2.x;
				cardTrayScript.Resize(newWidth, (!DealCard) ? SecondsDuration4 : (SecondsDuration4 / 2f));
			}
			else if (newPile.Count >= newPile.LayoutProperties.MaxCardsBeforeOverlapping)
			{
				ScalingCardTray cardTrayScript2 = scalingTrayXform2.gameObject.GetComponent<ScalingCardTray>();
				float num3 = newPile.GetPileWidth(newPile.LayoutProperties.MaxCardsBeforeOverlapping) + ScalingCardTray.kEdgeSpacing;
				Vector3 localScale3 = scalingTrayXform2.localScale;
				float newWidth2 = num3 / localScale3.x;
				cardTrayScript2.Resize(newWidth2, (!DealCard) ? SecondsDuration4 : (SecondsDuration4 / 2f));
			}
		}
		ElapsedTime3 = 0f;
		bool done = false;
		float maxAlpha = 1f;
		float minAlpha = 0f;
		while (!done)
		{
			if (ElapsedTime3 >= SecondsDuration4)
			{
				done = true;
				Card.CardObj.transform.localPosition = EndPosition;
				Card.CardObj.transform.localRotation = EndRotation;
				Card.CardObj.transform.localScale = EndScale;
				if (fadeInKeepers)
				{
					player.KeeperTrayOpacity.alpha = maxAlpha;
				}
				else if (fadeOutKeepers)
				{
					player.KeeperTrayOpacity.alpha = minAlpha;
				}
			}
			else
			{
				float PercentComplete3 = ElapsedTime3 / SecondsDuration4;
				if (fadeInKeepers)
				{
					player.KeeperTrayOpacity.alpha = minAlpha + PercentComplete3 * (maxAlpha - minAlpha);
				}
				else if (fadeOutKeepers)
				{
					player.KeeperTrayOpacity.alpha = minAlpha + (1f - PercentComplete3) * (maxAlpha - minAlpha);
				}
				if (DealCard)
				{
					PercentComplete3 = PercentComplete3 * 0.5f + 0.5f;
				}
				Card.CardObj.transform.localPosition = Vector3.Lerp(StartPosition, EndPosition, PercentComplete3);
				Card.CardObj.transform.localRotation = Quaternion.Lerp(StartRotation2, EndRotation, PercentComplete3);
				Card.CardObj.transform.localScale = Vector3.Lerp(StartScale2, EndScale, PercentComplete3);
			}
			ElapsedTime3 += Time.deltaTime;
			yield return 0;
		}
		Card.Moving = false;
		if (shrinkTray && !isEndCard)
		{
			shrinkingTrayScript.width = shrinkingTrayWidth;
		}
		if (newPile == player.Stock && (oldPile != player.Stock || !faceUp))
		{
			player.deckCounterComponent.count = player.Stock.Count;
			Card.Identified = false;
			Card.Type = string.Empty;
		}
		if (oldPile == player.Stock && newPile == player.Discard)
		{
			QueueAnimation("LoseCardSound", delegate
			{
			}, true);
		}
		if (oldPile == player.Keepers)
		{
			string keeperName = Card.InternalHeroName[0];
			CardGameController.Instance.DestroyKeeper(player, keeperName);
		}
		oldPile.RefreshLayout();
		newPile.RefreshLayout();
		if (moveData.proceed)
		{
			AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.AnimFinished());
		}

		//CspUtils.DebugLog("cardobj mainTexture (mini)= " + Card.CardObj.renderer.materials[Card.CardObj.renderer.materials.Length - 1].mainTexture);
			
	}

	public virtual void OnNewTurn(bool mine)
	{
		CspUtils.DebugLog("Player " + info.PlayerID + " got NewTurn: " + mine);
		myTurn = mine;
		attacksThisTurn = 0;
		if (playerWaitDialog != null)
		{
			playerWaitDialog.Hide();
		}
	}

	public virtual void OnPickCard(List<int> cards, bool canPass, PickCardType pickCardType, int opposingCard, int passButtonId)
	{
		string text = string.Empty;
		foreach (int card in cards)
		{
			text = text + card + " ";
		}
		CspUtils.DebugLog("Player " + info.PlayerID + " got PickCard: (" + text + ") CanPass:" + canPass + " Pick type " + pickCardType + ", Blocker ID: " + opposingCard + ", Pass Button ID: " + passButtonId);
		QueueAnimation("Highlight Attack Factor", delegate
		{
			if (pickCardType == PickCardType.Block)
			{
				BattleCard battleCard = opponent.FindCardByIndex(opposingCard);
				if (battleCard == null)
				{
					battleCard = FindCardByIndex(opposingCard);
				}
				if (battleCard != null)
				{
					battleCard.CardProps.PlayHighlightFactor(true);
				}
			}
		}, true);
	}

	public virtual void OnPickPending()
	{
		QueueAnimation("PickPending", delegate
		{
			ArrayList arrayList = new ArrayList(1);
			arrayList.Add(Info.PlayerID.ToString());
			RoomAgent.SendMessage("PickReady", arrayList);
		}, true);
	}

	public virtual void OnPickFactor()
	{
		CspUtils.DebugLog("Player " + info.PlayerID + " got PickFactor");
	}

	public virtual void OnPickNumber(int min, int max)
	{
		CspUtils.DebugLog("Player " + info.PlayerID + " got PickNumber(" + min + "," + max + ")");
	}

	public virtual void OnPickYesNo(int yesButtonID, int noButtonID)
	{
		CspUtils.DebugLog("Player " + info.PlayerID + " got OnPickYesNo(" + yesButtonID + "," + noButtonID + ")");
	}

	public virtual void OnSetPower(int newPower, bool coinFlip)
	{
		CspUtils.DebugLog("Player " + info.PlayerID + " got SetPower: " + newPower + ", " + coinFlip);
	}

	public void ShowAllPanels()
	{
		float animTime = 0.3f;
		SidePanel.Advance(animTime);
		EdgePanel.Advance(animTime);
		PlayPanel.Advance(animTime);
	}
}
