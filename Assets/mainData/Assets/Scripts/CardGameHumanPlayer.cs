using CardGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("CardGame/Private/Human Player")]
internal class CardGameHumanPlayer : CardGamePlayer
{
	protected Vector3 mouseSleepPosition;

	protected bool isMouseSleeping;

	protected float mouseSleepTime;

	protected PickCardType currentPickCardType;

	protected GUIDynamicWindow pickWindow;

	protected bool _passButtonEnabled;

	private bool soakedThisAttack;

	private bool waitingForCardSelection;

	private List<BattleCard> validCards = new List<BattleCard>();

	private CardGameHintManager hintManager = new CardGameHintManager();

	private int ndx = 0; // added by CSP

	public bool PassButtonEnabled
	{
		get
		{
			return _passButtonEnabled;
		}
	}

	public CardGameHumanPlayer()
	{
		Deck.DeckId = CardGroup.R3DemoHulkIronman;
		Deck.DeckRecipe = CardGroup.R4DemoHulkRecipe;
	}

	public void DisablePassButton()
	{
		_passButtonEnabled = false;
		AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.DisablePassButton());
	}

	public void EnablePassButton(PickCardType pickCardType, int passButtonId)
	{
		_passButtonEnabled = true;
		AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.EnablePassButton((CardGameEvent.EnablePassButton.PassButtonType)passButtonId, pickCardType));
	}

	private void OnEnable()
	{
		DisablePassButton();
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.ClickedPass>(OnPlayerPassed);
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.AttackResult>(OnAttackResult);
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.ClickedPoke>(OnPlayerPoking);
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.PokedTimerCompleted>(OnPokedTimerCompleted);
	}

	private void OnDisable()
	{
		hintManager.Disable();
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.ClickedPass>(OnPlayerPassed);
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.AttackResult>(OnAttackResult);
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.ClickedPoke>(OnPlayerPoking);
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.PokedTimerCompleted>(OnPokedTimerCompleted);
	}

	public void Update()
	{
		HitTestMouse();
		DetectKeypress();
	}

	protected void OnPlayerPassed(CardGameEvent.ClickedPass evt)
	{
		CspUtils.DebugLog("OnPlayerPassed() called!");
		SendPickCard(null, true, evt.pickCardType);
		hintManager.HideHint();
	}

	protected void OnPlayerPoking(CardGameEvent.ClickedPoke evt)
	{
		ArrayList arrayList = new ArrayList(1);
		arrayList.Add(base.Info.PlayerID.ToString());
		RoomAgent.SendMessage("Poke", arrayList);
	}

	protected void OnPokedTimerCompleted(CardGameEvent.PokedTimerCompleted evt)
	{
		if (pickWindow != null)
		{
			pickWindow.Hide();
		}
		ClearValidCards();
		hintManager.HideHint();
	}

	public override void OnAnim(int type, int arg1, int arg2)
	{
		BattleCard card = null;
		switch (type)
		{
		case 65:
		{
			bool localCard = false;
			switch (arg1)
			{
			case 5:
			case 6:
				localCard = (FindCardByIndex(arg2) != null);
				break;
			case 4:
			{
				card = FindCardByIndex(arg2);
				string empty = string.Empty;
				empty = ((card == null) ? "[UNKNOWN CARD]" : card.Name);
				CardGameController.Instance.QueueDealerChat(508, empty);
				break;
			}
			}
			QueueAnimation("PlaySound", delegate
			{
				CardGameController.Instance.AudioManager.PlayFromServer(arg1, localCard);
			}, true);
			break;
		}
		case 68:
			CspUtils.DebugLog("Coin flip result: " + arg1.ToString() + "  arg2: " + arg2.ToString());
			QueueAnimation("CoinFlip", delegate
			{
				if (arg2 > 0)
				{
					card = FindCardByIndex(arg2);
					if (card == null)
					{
						CspUtils.DebugLog("did not find card for me");
						card = opponent.FindCardByIndex(arg2);
					}
					if (card != null)
					{
						CspUtils.DebugLog("keeper activate");
						card.CardProps.PlayKeeperActivate(true);
					}
					else
					{
						CspUtils.DebugLog("did not find card for opponent");
					}
				}
				CardGameController.Instance.Hud.FlipCoin((arg1 == 1) ? true : false, (arg2 == 0) ? true : false);
			}, false);
			QueueAnimation("CoinFlipDone", delegate
			{
				if (arg2 > 0)
				{
					card = FindCardByIndex(arg2);
					if (card == null)
					{
						CspUtils.DebugLog("did not find card for me");
						card = opponent.FindCardByIndex(arg2);
					}
					if (card != null)
					{
						CspUtils.DebugLog("keeper deactivate");
						card.CardProps.PlayKeeperActivate(false);
					}
					else
					{
						CspUtils.DebugLog("did not find card for opponent");
					}
				}
				AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.AnimFinished());
			}, false);
			break;
		}
		base.OnAnim(type, arg1, arg2);
	}

	public override void SendPickCard(BattleCard card, bool pass, PickCardType pickCardType)
	{
		CspUtils.DebugLog("Human-SendPickCard() called!");

		base.SendPickCard(card, pass, pickCardType);
		DisablePassButton();
		ClearValidCards();
		waitingForCardSelection = false;
		if (card != null)
		{
			card.SetSelectable();
		}
		if (!pass)
		{
			sleepMouse();
		}
		hintManager.HideHint();
		if (pass && pickCardType == PickCardType.Attack)
		{
			CardGameController.Instance.AudioManager.Play("Pass");
		}

		CspUtils.DebugLog("About to call SimulateServer_AfterHumanPick_CSP()!");
		///////////// block added by CSP /////////////////////////////
		if (CardGameController.Instance.offenseNdx == 0)
			AppShell.Instance.StartCoroutine(CardGameController.Instance.SimulateServer_AfterOffensePick_CSP(pass)); 
		else
			AppShell.Instance.StartCoroutine(CardGameController.Instance.SimulateServer_AfterDefensePick_CSP(pass));
		//////////////////////////////////////////////////////////////////
	}

	public override void OnSetPower(int newPower, bool coinFlip)
	{
		QueueAnimation("SetPower", delegate
		{
			CardGameController.Instance.SetPower(newPower);
		}, true);
		CardGameController.Instance.QueueDealerChat(501, newPower);
	}

	public override void OnGameOver(int reason, bool iWon)
	{
		base.OnGameOver(reason, iWon);
		int msgId = (!iWon) ? 135 : 134;
		string txt = CardLocale.Parse(msgId);
		QueueAnimation("InfoText", delegate
		{
			AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.DealerChat(CardGameEvent.DealerChat.MessageType.Prompt, msgId, txt, null));
			SquadBattleCharacterController.Instance.Victory((!iWon) ? 1 : 0);
			CardGameController.Instance.HandleGameEnd(iWon, reason == 1);
			CardCollection.Fetch();
		}, true);
	}

	public override void OnInfo(int message, int arg1, int arg2)
	{
		base.OnInfo(message, arg1, arg2);
		switch (message)
		{
		case 127:
			QueueAnimation("HandBlock Audio", delegate
			{
				CardGameController.Instance.AudioManager.Play(CardGameAudioManager.SFX.HandBlock);
			}, true);
			break;
		case 140:
		case 141:
			QueueAnimation("Keeper Misire", delegate
			{
				BattleCard battleCard = FindCardByIndex(arg1);
				if (battleCard == null)
				{
					battleCard = opponent.FindCardByIndex(arg1);
				}
				string empty = string.Empty;
				empty = ((battleCard == null) ? "[UNKNOWN CARD]" : battleCard.Name);
				AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.DealerChat(CardGameEvent.DealerChat.MessageType.Dealer, 512, CardLocale.Parse(512), empty));
				bool flag = message == 140;
				AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.AttackResult((!flag) ? 1 : 0, CardGameEvent.AttackResultType.KeeperMisfire, false, false, 0));
			}, true);
			break;
		default:
			CardGameController.Instance.QueueDealerChat(message);
			break;
		}
	}

	public override void OnMoveCard(int cardID, int src, int dest, bool mine, string cardType, int visibility, bool srcOpponent)
	{
		base.OnMoveCard(cardID, src, dest, mine, cardType, visibility, srcOpponent);
	}

	public override void OnNewTurn(bool mine)
	{
		base.OnNewTurn(mine);
		if (!mine)
		{
			DisablePassButton();
		}
		int num = (!mine) ? 1 : 0;
		string name = CardGameController.Instance.players[num].Info.Name;
		if (!CardGameController.Instance.GameStarted)
		{
			CardGameController.Instance.GameStarted = true;
			int msgId = (!mine) ? 301 : 300;   // 300 player goes first  |    301 opponent goes first
			QueueAnimation("New turn notification", delegate
			{
				AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.DealerChat(CardGameEvent.DealerChat.MessageType.Prompt, msgId, CardLocale.Parse(msgId), null));
			}, true);
			CardGameController.Instance.QueueDealerChat(500, name);
		}
		else
		{
			CardGameController.Instance.QueueDealerChat(502, name);
		}
		QueueAnimation("HighlightAvatar OFF", delegate
		{
			CardGameController.Instance.HighlightAvatar(mine ? 1 : 0);
		}, true);
		QueueAnimation("HighlightAvatar", delegate
		{
			CardGameController.Instance.HighlightAvatar((!mine) ? 1 : 0);
		}, true);
	}

	public override void OnPickCard(List<int> cards, bool canPass, PickCardType pickCardType, int opposingCard, int passButtonId)
	{
		base.OnPickCard(cards, canPass, pickCardType, opposingCard, passButtonId);
		currentPickCardType = pickCardType;
		BattleCard c;
		CardPile pile;
		QueueAnimation("RefreshCardStates", delegate
		{
			ClearValidCards();
			ShowAllPanels();
			int num = 0;
			bool flag = true;
			string text = null;
			switch (pickCardType)
			{
			case PickCardType.Attack:
				num = ((cards.Count <= 0) ? 132 : 130);
				break;
			case PickCardType.Block:
				num = ((cards.Count <= 0) ? 133 : 131);
				break;
			case PickCardType.Deck:
				flag = false;
				num = 150;
				text = "Choose";
				break;
			case PickCardType.Deploy:
				num = 151;
				text = "Choose";
				break;
			case PickCardType.Destroy:
				flag = false;
				num = 152;
				text = "Choose";
				break;
			case PickCardType.Discard:
				num = 136;
				text = "Discard";
				break;
			case PickCardType.Hand:
				flag = false;
				num = 153;
				text = "Choose";
				break;
			case PickCardType.Keeper:
				flag = false;
				num = 154;
				text = "ChooseAKeeper";
				break;
			case PickCardType.Recycle:
				flag = false;
				num = 155;
				text = "Choose";
				break;
			case PickCardType.Reveal:
				num = 156;
				text = "Choose";
				break;
			case PickCardType.Soak:
				num = 129;
				break;
			case PickCardType.Done:
				flag = false;
				num = 157;
				break;
			}
			if (num > 0)
			{
				CspUtils.DebugLog("queuing message " + num);
				CardGameController.Instance.QueueDealerChat(num);
			}
			if (!string.IsNullOrEmpty(text))
			{
				CardGameController.Instance.AudioManager.Play(text);
			}
			else if (pickCardType == PickCardType.Soak && !soakedThisAttack)
			{
				CardGameController.Instance.AudioManager.Play("Choose");
				soakedThisAttack = true;
			}
			foreach (int card in cards)
			{
				FindCardAndPile(card, out c, out pile);
				if (c == null)
				{
					opponent.FindCardAndPile(card, out c, out pile);
				}
				if (c != null)
				{
					////////////// block added by CSP ////////////////////
					if (pickCardType == PickCardType.Attack) {
						if (c.Level <= CardGameController.Instance.powerLevel)  // when attacking, shouldnt only cards within power level should be added to validCards?  'if' added by CSP
							validCards.Add(c); 
					}
					else
					///////////////////////////////////////////////////////
						validCards.Add(c); 
					 
					if (pickCardType == PickCardType.Block)
					{
						c.CardProps.PlayHighlightBlock(true);
					}
				}
			}
			if (canPass)
			{
				EnablePassButton(pickCardType, passButtonId);
			}
			if (flag)
			{
				UpdateSelectableStates(Hand);
			}
			else
			{
				UpdateSelectableStates();
			}
			waitingForCardSelection = true;
			if (pickCardType != PickCardType.Done)
			{
				hintManager.StartTimer(cards, this, opponent);
			}
		}, true);
		if (!canPass && cards.Count == 0)
		{
			CspUtils.DebugLog("User was forced to pass because they had no cards to select but were required to make a selection anyway. Potential broken rule.");
			SendPickCard(null, true, pickCardType);
		}
	}

	public override void OnPickFactor()
	{
		base.OnPickFactor();
		QueueAnimation("PickFactor", delegate
		{
			AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.DealerChat(CardGameEvent.DealerChat.MessageType.Prompt, 601, "#cardgame_pick_factor", null));
			pickWindow = new SHSCardGamePickFactor(delegate(BattleCard.Factor factorPicked)
			{
				SendPickFactor(factorPicked);
				pickWindow = null;
			});
			GUIManager.Instance.ShowDynamicWindow(pickWindow, GUIControl.ModalLevelEnum.None);
			CardGameController.Instance.AudioManager.Play("Choose");
		}, true);
	}

	public override void OnPickNumber(int min, int max)
	{
		base.OnPickNumber(min, max);
		QueueAnimation("PickANumber", delegate
		{
			AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.DealerChat(CardGameEvent.DealerChat.MessageType.Prompt, 600, "#cardgame_pick_number", null));
			pickWindow = new SHSCardGamePickNumber(min, max, delegate(int numberPicked)
			{
				SendPickNumber(numberPicked);
				pickWindow = null;
			});
			GUIManager.Instance.ShowDynamicWindow(pickWindow, GUIControl.ModalLevelEnum.None);
			CardGameController.Instance.AudioManager.Play("Choose");
		}, true);
	}

	public override void OnPickYesNo(int yesButtonID, int noButtonID)
	{
		base.OnPickYesNo(yesButtonID, noButtonID);
		QueueAnimation("PickYesNo", delegate
		{
			AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.DealerChat(CardGameEvent.DealerChat.MessageType.Prompt, 602, "#cardgame_pick_destination", null));
			int pickedYes = yesButtonID;
			pickWindow = new SHSCardGamePickYesNo(yesButtonID, noButtonID, delegate(int numberPicked)
			{
				SendPickYesNo(numberPicked == pickedYes);
				pickWindow = null;
			});
			GUIManager.Instance.ShowDynamicWindow(pickWindow, GUIControl.ModalLevelEnum.None);
			CardGameController.Instance.AudioManager.Play("Choose");
		}, true);
	}

	public override void OnDamage(int weaponID, List<int> casualties, int srcDEPRECATED, bool mine, List<string> typeList, int inflicted, int attempted, bool becomeKeeper, bool killKeeper)
	{
		base.OnDamage(weaponID, casualties, srcDEPRECATED, mine, typeList, inflicted, attempted, becomeKeeper, killKeeper);
		soakedThisAttack = false;
	}

	private void OnAttackResult(CardGameEvent.AttackResult evt)
	{
		switch (evt.type)
		{
		case CardGameEvent.AttackResultType.LuckyBlock:
			AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.DealerChat(CardGameEvent.DealerChat.MessageType.Dealer, 506, CardLocale.Parse(506), CardGameController.Instance.players[evt.playerIndex].Info.Name));
			break;
		case CardGameEvent.AttackResultType.Damage:
			AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.DealerChat(CardGameEvent.DealerChat.MessageType.Dealer, 507, CardLocale.Parse(507), CardGameController.Instance.players[evt.playerIndex].Info.Name, evt.damageDone));
			break;
		}
	}

	private void ClearValidCards()
	{
		ClearValidCards(Hand);
		ClearValidCards(Keepers);
		ClearValidCards(Show);
		ClearValidCards(opponent.Hand);
		ClearValidCards(opponent.Keepers);
		ClearValidCards(opponent.Show);
	}

	private void ClearValidCards(CardPile pile)
	{
		foreach (BattleCard item in pile)
		{
			item.SetSelectable();
			item.CardProps.PlayHighlightFactor(false);
			item.CardProps.PlayHighlightBlock(false);
		}
		validCards.Clear();
	}

	private void UpdateSelectableStates()
	{
		UpdateSelectableStates(Hand);
		UpdateSelectableStates(Keepers);
		UpdateSelectableStates(Show);
		UpdateSelectableStates(opponent.Hand);
		UpdateSelectableStates(opponent.Keepers);
		UpdateSelectableStates(opponent.Show);
	}

	private void UpdateSelectableStates(CardPile pile)
	{
		foreach (BattleCard item in pile)
		{
			if (!validCards.Contains(item))
			{
				item.SetUnselectable();
			}
			else
			{
				item.SetSelectable();
			}
		}
	}

	protected void sleepMouse()
	{
		mouseSleepPosition = SHSInput.mousePosition;
		isMouseSleeping = true;
		mouseSleepTime = 1f;
		CardGameController.Instance.ClearCardDetails();
	}

	private void HitTestMouse()
	{
		if (isMouseSleeping)
		{
			mouseSleepTime -= Time.deltaTime;
			if (mouseSleepTime > 0f)
			{
				return;
			}
			Vector3 vector = SHSInput.mousePosition - mouseSleepPosition;
			if (!(Mathf.Abs(vector.x) > 20f) && !(Mathf.Abs(vector.y) > 20f))
			{
				return;
			}
			isMouseSleeping = false;
		}
		bool flag = false;
		bool flag2 = true;

		if (Input.GetMouseButtonDown(1)) {
			//CardGameController.Instance.MoveCard_CSP(0); // added by CSP for testing.
			//CardGameController.Instance.SimulateServer_CSP(ndx++); // added by CSP for testing.
			
			//AppShell.Instance.StartCoroutine(CardGameController.Instance.SimulateServer_CSP()); // added by CSP for testing.
		}
		
		if (CardGameController.Instance.Hud.Camera != null)
		{
			Ray ray = CardGameController.Instance.Hud.Camera.ScreenPointToRay(SHSInput.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, 512))
			{
				Transform transform = hitInfo.transform;
				GameObject gameObject = transform.gameObject;
				CardProperties cardProperties = Utils.GetComponent<CardProperties>(gameObject);
				while (gameObject != null && transform != null && cardProperties == null && gameObject != transform)
				{
					transform = transform.parent;
					gameObject = ((!(transform == null)) ? transform.gameObject : null);
					cardProperties = ((!(gameObject == null)) ? Utils.GetComponent<CardProperties>(gameObject) : null);
				}
				CspUtils.DebugLog("(bool)cardProperties=" + (bool)cardProperties);
				if ((bool)cardProperties && SHSInput.IsObjectAllowedInput(cardProperties.gameObject))
				{
					CspUtils.DebugLog("SHSInput.GetMouseButtonUp:" + SHSInput.GetMouseButtonUp(SHSInput.MouseButtonType.Left));
					CspUtils.DebugLog("currentPickCardType:" + currentPickCardType);
					CspUtils.DebugLog("validCards.Contains(cardProperties.Card):" + validCards.Contains(cardProperties.Card));  // !!! THIS ONE IS FALSE !!!
					flag = true;
					if (SHSInput.GetMouseButtonUp(SHSInput.MouseButtonType.Left) && currentPickCardType != PickCardType.Done && validCards.Contains(cardProperties.Card))
					{
						CspUtils.DebugLog("send pick card");
						SendPickCard(cardProperties.Card, false, currentPickCardType);
					}
					else
					{
						CspUtils.DebugLog("hover card");
						CardGameController.Instance.HoverCard(cardProperties.Card);
					}
				}
			}
		}
		if (!flag)
		{
			CardGameController.Instance.ClearCardDetails();
		}
	}

	private void DetectKeypress()
	{
		if (waitingForCardSelection && SHSInput.GetButtonDown("Jump", AppShell.Instance) && PassButtonEnabled)
		{
			CspUtils.DebugLog("space bar used to pass");
			SendPickCard(null, true, currentPickCardType);
		}
	}
}
