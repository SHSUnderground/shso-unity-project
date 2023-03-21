using CardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuilderController : GameController
{
	public delegate void OnCardGroupPopulated(CardGroup cardGroup);

	public const float PerCardInstanceLimit = 4f;

	private const int FullSizeTextureCacheSize = 25;

	protected static DeckBuilderController instance;

	protected TransactionMonitor deckLoaderTransaction;

	protected ShsEventMgr eventMgr;

	protected UserProfile userProfile;

	protected int countersLoaded;

	protected CardCollection collection;

	protected SortedCardList myCards;

	protected SortedCardList deckCards;

	protected List<CardFilter> filterStates;

	protected ShsAudioSourceList uiSfx;

	public string fullCardPrefabName;

	public string collectionGridPrefabName;

	public string deckGridPrefabName;

	public string boosterPackGridPrefabName;

	public string cardCounterPrefabName;

	public GameObject fullCardPrefab;

	public GameObject collectionGridPrefab;

	public GameObject deckGridPrefab;

	public GameObject boosterPackGridPrefab;

	public GameObject cardCounterPrefab;

	public Camera collectionCamera;

	public Camera deckCamera;

	public Camera boosterPackCamera;

	public Camera mainCamera;

	protected GameObject collectionGrid;

	protected GameObject deckGrid;

	protected GameObject boosterPackGrid;

	private Dictionary<string, Texture2D> FullSizeTextures;

	private Dictionary<string, float> FullSizeTextureTime;

	private Dictionary<char, string> factorLookup;

	private Dictionary<char, string> teamLookup;

	private bool doneLoadingBundle;

	public static DeckBuilderController Instance
	{
		get
		{
			return instance;
		}
	}

	public SortedCardList DeckCards
	{
		get
		{
			return deckCards;
		}
	}

	public SortedCardList MyCards
	{
		get
		{
			return myCards;
		}
	}

	public Dictionary<char, string> FactorLookup
	{
		get
		{
			return factorLookup;
		}
	}

	public Dictionary<char, string> TeamLookup
	{
		get
		{
			return teamLookup;
		}
	}

	public GameObject BoosterPackGrid
	{
		get
		{
			return boosterPackGrid;
		}
	}

	private void OnCardGameCommonLoaded(AssetBundleLoadResponse response, object extraData)
	{
		if (response.Bundle != null)
		{
			fullCardPrefab = (GameObject)response.Bundle.Load(fullCardPrefabName);
			if (fullCardPrefab == null)
			{
				CspUtils.DebugLog("Failed to load FullCard_prefab");
			}
			cardCounterPrefab = (GameObject)response.Bundle.Load(cardCounterPrefabName);
			if (cardCounterPrefab == null)
			{
				CspUtils.DebugLog("Failed to load card counter Prefab");
			}
		}
		else
		{
			CspUtils.DebugLog("Failed to load cardgame_common: " + response.Error);
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
		StartTransaction.CompleteStep("toolboxBundleLoaded");
	}

	private void OnDeckBuilderBundleLoaded(AssetBundleLoadResponse response, object extraData)
	{
		if (response.Bundle != null)
		{
			collectionGridPrefab = (GameObject)response.Bundle.Load(collectionGridPrefabName);
			if (collectionGridPrefab == null)
			{
				CspUtils.DebugLog("Failed to load collection grid Prefab");
			}
			deckGridPrefab = (GameObject)response.Bundle.Load(deckGridPrefabName);
			if (deckGridPrefab == null)
			{
				CspUtils.DebugLog("Failed to load deck grid Prefab");
			}
			boosterPackGridPrefab = (GameObject)response.Bundle.Load(boosterPackGridPrefabName);
			if (boosterPackGridPrefab == null)
			{
				CspUtils.DebugLog("Failed to load booster pack grid Prefab");
			}
		}
		else
		{
			CspUtils.DebugLog("Failed to load deckbuilder_bundle: " + response.Error);
		}
		doneLoadingBundle = true;
		StartTransaction.CompleteStep("deckBuilderBundleLoaded");
	}

	private IEnumerator Initialize()
	{
		StartTransaction = AppShell.Instance.TransitionHandler.CurrentTransactionContext.Transaction;
		StartTransaction.onComplete += OnStartComplete;
		deckLoaderTransaction = TransactionMonitor.CreateTransactionMonitor("Deck Loader Transaction", OnRecipeLoadComplete, 30f, null);
		StartTransaction.AddChild(deckLoaderTransaction);
		factorLookup = new Dictionary<char, string>();
		factorLookup['A'] = "Animal";
		factorLookup['S'] = "Strength";
		factorLookup['E'] = "Elemental";
		factorLookup['N'] = "Energy";
		factorLookup['T'] = "Tech";
		factorLookup['P'] = "Speed";
		teamLookup = new Dictionary<char, string>();
		teamLookup['F'] = "F4";
		teamLookup['A'] = "Avengers";
		teamLookup['P'] = "SpideyFriends";
		teamLookup['X'] = "Xmen";
		teamLookup['S'] = "SS";
		teamLookup['B'] = "Brotherhood";
		FullSizeTextures = new Dictionary<string, Texture2D>();
		FullSizeTextureTime = new Dictionary<string, float>();
		if (instance != null)
		{
			CspUtils.DebugLog("A second DeckBuilderController is being created.  This may lead to instabilities!");
		}
		else
		{
			instance = this;
		}
		countersLoaded = 0;
		eventMgr = AppShell.Instance.EventMgr;
		userProfile = AppShell.Instance.Profile;
		if (userProfile == null)
		{
			userProfile = UserProfile.OfflineUserProfile;
		}
		doneLoadingBundle = false;
		AppShell.Instance.BundleLoader.FetchAssetBundle("CardGame/cardgame_common", OnCardGameCommonLoaded, null, true);
		while (!doneLoadingBundle)
		{
			yield return 0;
		}
		doneLoadingBundle = false;
		AppShell.Instance.BundleLoader.FetchAssetBundle("GUI/non_locale/deckbuilder_bundle", OnDeckBuilderBundleLoaded, null, true);
		while (!doneLoadingBundle)
		{
			yield return 0;
		}
		doneLoadingBundle = false;
		GUIManager.Instance.BundleManager.LoadBundle("toolbox_bundle", OnToolboxBundleLoaded, null);
		while (!doneLoadingBundle)
		{
			yield return 0;
		}
		filterStates = new List<CardFilter>(Enum.GetValues(typeof(CardFilterType)).Length);
		filterStates.Add(new CardFilter(CardFilterType.BlockStrength, BattleCard.Factor.Strength));
		filterStates.Add(new CardFilter(CardFilterType.BlockSpeed, BattleCard.Factor.Speed));
		filterStates.Add(new CardFilter(CardFilterType.BlockElemental, BattleCard.Factor.Elemental));
		filterStates.Add(new CardFilter(CardFilterType.BlockTech, BattleCard.Factor.Tech));
		filterStates.Add(new CardFilter(CardFilterType.BlockAnimal, BattleCard.Factor.Animal));
		filterStates.Add(new CardFilter(CardFilterType.BlockEnergy, BattleCard.Factor.Energy));
		filterStates.Add(new CardFilter(CardFilterType.FactorStrength, BattleCard.Factor.Strength));
		filterStates.Add(new CardFilter(CardFilterType.FactorSpeed, BattleCard.Factor.Speed));
		filterStates.Add(new CardFilter(CardFilterType.FactorElemental, BattleCard.Factor.Elemental));
		filterStates.Add(new CardFilter(CardFilterType.FactorTech, BattleCard.Factor.Tech));
		filterStates.Add(new CardFilter(CardFilterType.FactorAnimal, BattleCard.Factor.Animal));
		filterStates.Add(new CardFilter(CardFilterType.FactorEnergy, BattleCard.Factor.Energy));
		filterStates.Add(new CardFilter(CardFilterType.TeamAvengers, "A"));
		filterStates.Add(new CardFilter(CardFilterType.TeamXmen, "X"));
		filterStates.Add(new CardFilter(CardFilterType.TeamBrotherhood, "B"));
		filterStates.Add(new CardFilter(CardFilterType.TeamSS, "S"));
		filterStates.Add(new CardFilter(CardFilterType.TeamF4, "F"));
		filterStates.Add(new CardFilter(CardFilterType.TeamSpideyFriends, "P"));
		filterStates.Add(new CardFilter(CardFilterType.MinLevel, 1));
		filterStates.Add(new CardFilter(CardFilterType.MaxLevel, 20));
		filterStates.Add(new CardFilter(CardFilterType.TextSearch, string.Empty));
		PlayerData data = new PlayerData();
		data.Kind = PlayerData.PlayerKind.Local;
		data.UserId = userProfile.UserId;
		data.Name = userProfile.PlayerName;
		data.NetId = 0;
		GameObject playerGO = new GameObject("Player");
		Utils.AttachGameObject(base.gameObject, playerGO);
		GameObject frame3 = UnityEngine.Object.Instantiate(collectionGridPrefab) as GameObject;
		Utils.AttachGameObject(collectionCamera.gameObject, frame3);
		PlayerPanelProperties panelProperties3 = Utils.GetComponent<PlayerPanelProperties>(frame3);
		panelProperties3.Initialize(collectionCamera);
		Transform layoutTransform3 = frame3.transform.Find("CardLayout_CollectionGrid");
		if (layoutTransform3 != null)
		{
			collectionGrid = layoutTransform3.gameObject;
		}
		else
		{
			CspUtils.DebugLog("Failed to find collection grid layout transform");
		}
		frame3 = (UnityEngine.Object.Instantiate(deckGridPrefab) as GameObject);
		Utils.AttachGameObject(deckCamera.gameObject, frame3);
		panelProperties3 = Utils.GetComponent<PlayerPanelProperties>(frame3);
		panelProperties3.Initialize(deckCamera);
		layoutTransform3 = frame3.transform.Find("CardLayout_DeckGrid");
		if (layoutTransform3 != null)
		{
			deckGrid = layoutTransform3.gameObject;
		}
		else
		{
			CspUtils.DebugLog("Failed to find deck grid layout transform");
		}
		frame3 = (UnityEngine.Object.Instantiate(boosterPackGridPrefab) as GameObject);
		Utils.AttachGameObject(boosterPackCamera.gameObject, frame3);
		panelProperties3 = Utils.GetComponent<PlayerPanelProperties>(frame3);
		panelProperties3.Initialize(boosterPackCamera);
		layoutTransform3 = frame3.transform.Find("BoosterPack_CollectionGrid");
		if (layoutTransform3 != null)
		{
			boosterPackGrid = layoutTransform3.gameObject;
		}
		else
		{
			CspUtils.DebugLog("Failed to find booster pack layout transform");
		}
		uiSfx = ShsAudioSourceList.GetList("DeckBuilder");
		uiSfx.PreloadAll(StartTransaction);
		StartTransaction.CompleteStep("instantiation");
		StartCoroutine(LoadCollectionCards());
	}

	protected void OnRecipeLoadComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		StartTransaction.CompleteStep("recipeloaded");
	}

	protected void OnStartComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		CspUtils.DebugLog("DeckBuilderController.OnStartComplete()");
		StartTransaction = null;
		if (exit != 0)
		{
			AppShell.Instance.CriticalError(SHSErrorCodes.Code.DeckControllerFail);
		}
	}

	public void FetchCardCollection()
	{
		StartCoroutine(FetchCardCollection_DoWork());
	}

	private IEnumerator FetchCardCollection_DoWork()
	{
		CardCollection.Fetch();
		while (!CardCollection.IsDoneFetching)
		{
			yield return new WaitForEndOfFrame();
		}
		if (CardCollection.IsError)
		{
			if (StartTransaction != null)
			{
				StartTransaction.FailStep("fetchCardCollection", "Unable to load card collection");
			}
			yield break;
		}
		if (StartTransaction != null)
		{
			StartTransaction.CompleteStep("fetchCardCollection");
		}
		CardGroup collectionCards = new CardGroup();
		yield return StartCoroutine(collectionCards.LoadRecipe(CardCollection.Collection, deckLoaderTransaction, false));
		SortedCardList.SortMethod sortMethod = SortedCardList.SortMethod.CardName;
		if (myCards != null)
		{
			sortMethod = myCards.PrimarySort;
			myCards.Reset(true);
		}
		myCards = new SortedCardList(collectionCards, collectionGrid, fullCardPrefab);
		foreach (CardListCard sourceCard2 in myCards)
		{
			AddCounterToCard(sourceCard2);
		}
		if (StartTransaction != null)
		{
			StartTransaction.CompleteStep("addCounters");
		}
		if (deckCards != null)
		{
			foreach (CardListCard sourceCard in deckCards)
			{
				myCards.SubtractCard(sourceCard, sourceCard.Available, false);
			}
		}
		AppShell.Instance.EventMgr.Fire(this, new CardCollectionDataMessage(CardCollectionDataMessage.CCDataEvent.CollectionCountChanged, myCards.TotalCards, myCards.CountVisible()));
		myCards.PrimarySort = sortMethod;
		myCards.Sort(myCards.Compare);
		myCards.FreshLayout();
	}

	public override void Awake()
	{
		CspUtils.DebugLog("DeckBuilderController::Awake()");
		bCallControllerReadyFromStart = false;
		base.Awake();
		StartCoroutine(Initialize());
	}

	public override void Start()
	{
		CspUtils.DebugLog("DeckBuilderController::Start()");
		AppShell.Instance.QueueLocationInfo();
		base.Start();
	}

	public void PopulateCardGroup(CardGroup cardGroup, Dictionary<string, int> cardCollection, OnCardGroupPopulated onPopulated, bool fullSize)
	{
		StartCoroutine(PopulateCardGroup_DoWork(cardGroup, cardCollection, onPopulated, fullSize));
	}

	private IEnumerator PopulateCardGroup_DoWork(CardGroup cardGroup, Dictionary<string, int> cardCollection, OnCardGroupPopulated onPopulated, bool fullSize)
	{
		yield return StartCoroutine(cardGroup.LoadRecipe(cardCollection, null, fullSize));
		if (onPopulated != null)
		{
			onPopulated(cardGroup);
		}
	}

	protected IEnumerator LoadCollectionCards()
	{
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
		StartTransaction.CompleteStep("cardDataLoaded");
		yield return StartCoroutine(FetchCardCollection_DoWork());
		deckCards = new SortedCardList(deckGrid, fullCardPrefab);
		ControllerReady();
	}

	private void AddCounterToCard(CardListCard card)
	{
		if (!(card.counterComponent != null))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(cardCounterPrefab) as GameObject;
			Utils.AttachGameObject(card.CardObj, gameObject);
			gameObject.gameObject.transform.localPosition = new Vector3(1.75f, -2.6f, -0.1f);
			card.counterComponent = Utils.GetComponent<CardInventoryCounter>(gameObject);
			card.counterComponent.SetCount(card.Available);
		}
	}

	private bool CanAddToDeck(CardListCard card, bool isInDeck)
	{
		if (card.Available < 1)
		{
			return false;
		}
		int totalCards = deckCards.TotalCards;
		if (totalCards >= 99)
		{
			GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, "#deckbuilder_max_cards", delegate
			{
			}, GUIControl.ModalLevelEnum.Default);
			return false;
		}
		bool flag = false;
		if (isInDeck && (float)card.counterComponent.count >= 4f)
		{
			flag = true;
		}
		else
		{
			foreach (CardListCard deckCard in deckCards)
			{
				if (CardListCard.EquivalentCards(deckCard, card) && (float)deckCard.counterComponent.count >= 4f)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			CspUtils.DebugLog(string.Format("Can't add more than {0} instances of a single card to a deck", 4f));
			GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, "#deckbuilder_instance_limit", delegate
			{
			}, GUIControl.ModalLevelEnum.Default);
			return false;
		}
		return true;
	}

	private void MoveCardToDeck(CardListCard card)
	{
		if (myCards.SubtractCard(card, 1, false) > 0)
		{
			AddCounterToCard(deckCards.AddCard(card));
			AppShell.Instance.EventMgr.Fire(this, new CardCollectionDataMessage(CardCollectionDataMessage.CCDataEvent.DeckCountChanged, deckCards.TotalCards, deckCards.CountVisible()));
		}
	}

	private void MoveCardToCollection(CardListCard card)
	{
		if (deckCards.SubtractCard(card, 1, true) > 0)
		{
			myCards.AddCard(card);
			AppShell.Instance.EventMgr.Fire(this, new CardCollectionDataMessage(CardCollectionDataMessage.CCDataEvent.DeckCountChanged, deckCards.TotalCards, deckCards.CountVisible()));
			CspUtils.DebugLog("Pulled card " + card.Type + " from the user's deck");
		}
	}

	private void DoTransferCard(GameObject go, CardCollection3DMessage.CC3DEvent clickEvent)
	{
		CardListCard cardFromObject = GetCardFromObject(go, myCards);
		if (cardFromObject != null)
		{
			switch (clickEvent)
			{
			case CardCollection3DMessage.CC3DEvent.CardMouseLeftClick:
				if (CanAddToDeck(cardFromObject, false))
				{
					MoveCardToDeck(cardFromObject);
				}
				break;
			case CardCollection3DMessage.CC3DEvent.CardMouseRightClick:
				foreach (CardListCard deckCard in deckCards)
				{
					if (CardListCard.EquivalentCards(deckCard, cardFromObject))
					{
						MoveCardToCollection(deckCard);
						return;
					}
				}
				break;
			}
		}
		else
		{
			cardFromObject = GetCardFromObject(go, deckCards);
			if (cardFromObject == null)
			{
				CspUtils.DebugLog("Null card found in deck list");
				return;
			}
			switch (clickEvent)
			{
			case CardCollection3DMessage.CC3DEvent.CardMouseLeftClick:
				MoveCardToCollection(cardFromObject);
				break;
			case CardCollection3DMessage.CC3DEvent.CardMouseRightClick:
				if (CanAddToDeck(cardFromObject, true))
				{
					CardListCard cardListCard = null;
					foreach (CardListCard myCard in myCards)
					{
						if (CardListCard.EquivalentCards(myCard, cardFromObject))
						{
							cardListCard = myCard;
							break;
						}
					}
					if (cardListCard != null)
					{
						MoveCardToDeck(cardListCard);
					}
				}
				break;
			}
		}
		deckCards.FreshLayout();
	}

	public override void OnEnable()
	{
		base.OnEnable();
		if (AppShell.Instance != null)
		{
			eventMgr.AddListener<CardCollectionUIMessage>(OnUIEvent);
			eventMgr.AddListener<CardCollection3DMessage>(On3DEvent);
			eventMgr.AddListener<CardCollectionUIMessage>(OnSortTypeSelected);
			eventMgr.AddListener<CardCollectionFilterUIMessage>(OnFilterUIEvent);
			eventMgr.AddListener<CardGameEvent.DeckListLoaded>(OnDecksLoaded);
		}
		else
		{
			CspUtils.DebugLog("Couldn't subscribe to events: No AppShell instance found!");
		}
	}

	public override void OnDisable()
	{
		if (AppShell.Instance != null)
		{
			eventMgr.RemoveListener<CardCollectionUIMessage>(OnUIEvent);
			eventMgr.RemoveListener<CardCollection3DMessage>(On3DEvent);
			eventMgr.RemoveListener<CardCollectionUIMessage>(OnSortTypeSelected);
			eventMgr.RemoveListener<CardCollectionFilterUIMessage>(OnFilterUIEvent);
			eventMgr.RemoveListener<CardGameEvent.DeckListLoaded>(OnDecksLoaded);
		}
		base.OnDisable();
	}

	private void OnDecksLoaded(CardGameEvent.DeckListLoaded evt)
	{
		int deckId = deckCards.sourceGroup.DeckId;
		if (deckId != -1)
		{
			foreach (KeyValuePair<string, DeckProperties> deck in CardCollection.DeckList)
			{
				if (deck.Value.DeckId == deckId)
				{
					return;
				}
			}
			CspUtils.DebugLog("Resetting deckID of recently deleted deck - was " + deckId);
			deckCards.sourceGroup.DeckId = -1;
		}
	}

	public override void DisableSceneRendering()
	{
	}

	public override void EnableSceneRendering()
	{
	}

	protected void OnUIEvent(CardCollectionUIMessage msg)
	{
		Vector3 localPosition;
		switch (msg.Event)
		{
		case CardCollectionUIMessage.CCUIEvent.CollectionGridScroll:
		{
			GUISlider gUISlider = msg.Control as GUISlider;
			if (myCards != null)
			{
				localPosition = myCards.GameObj.transform.parent.transform.localPosition;
				localPosition.y = gUISlider.Value / 100f * myCards.ScrollableHeight;
				myCards.GameObj.transform.parent.transform.localPosition = localPosition;
			}
			break;
		}
		case CardCollectionUIMessage.CCUIEvent.DeckGridScroll:
		{
			GUISlider gUISlider = msg.Control as GUISlider;
			if (deckCards != null)
			{
				localPosition = deckCards.GameObj.transform.parent.transform.localPosition;
				localPosition.y = gUISlider.Value / 100f * deckCards.ScrollableHeight;
				deckCards.GameObj.transform.parent.transform.localPosition = localPosition;
			}
			break;
		}
		case CardCollectionUIMessage.CCUIEvent.LoadClicked:
			ShowLoadDialog(deckCards.saved);
			break;
		case CardCollectionUIMessage.CCUIEvent.NewClicked:
			DoNewDeck();
			break;
		case CardCollectionUIMessage.CCUIEvent.SaveClicked:
		{
			if (deckCards.Count == 0)
			{
				GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, "#deckbuilder_save_empty_deck", delegate
				{
				}, GUIControl.ModalLevelEnum.Default);
				break;
			}
			int totalCards = deckCards.TotalCards;
			if (totalCards != 40)
			{
				GUIDialogNotificationSink notificationSink = new GUIDialogNotificationSink(delegate(string Id, GUIDialogWindow.DialogState state)
				{
					if (state == GUIDialogWindow.DialogState.Ok)
					{
						ShowSaveDialog();
					}
				});
				SHSCommonDialogWindow sHSCommonDialogWindow = new SHSCommonDialogWindow("common_bundle|notification_icon_cardgamenogo", new Vector2(159f, 198f), new Vector2(0f, 0f), string.Empty, "common_bundle|L_mshs_button_ok", "common_bundle|L_mshs_button_cancel", typeof(SHSDialogOkButton), typeof(SHSDialogCancelButton), false);
				if (totalCards > 40)
				{
					sHSCommonDialogWindow.TitleText = "#deckbuilder_save_over_40_title";
					sHSCommonDialogWindow.Text = "#deckbuilder_save_over_40";
					sHSCommonDialogWindow.NotificationSink = notificationSink;
				}
				else if (totalCards < 40)
				{
					sHSCommonDialogWindow.TitleText = "#deckbuilder_save_under_40_title";
					sHSCommonDialogWindow.Text = "#deckbuilder_save_under_40";
					sHSCommonDialogWindow.NotificationSink = notificationSink;
				}
				GUIManager.Instance.ShowDynamicWindow(sHSCommonDialogWindow, GUIControl.ModalLevelEnum.Full);
			}
			else
			{
				ShowSaveDialog();
			}
			break;
		}
		}
	}

	private void ShowSaveDialog()
	{
		SHSCardGameLoadSaveDialog windowRef = null;
		GUIManager.Instance.ShowDialog(typeof(SHSCardGameLoadSaveDialog), string.Empty, new GUIDialogNotificationSink(delegate(string Id, IGUIDialogWindow window)
		{
			windowRef = (window as SHSCardGameLoadSaveDialog);
			windowRef.CurrentMode = SHSCardGameLoadSaveDialog.DialogMode.Save;
		}, delegate
		{
		}, delegate
		{
		}, delegate
		{
		}, delegate(string Id, GUIDialogWindow.DialogState state)
		{
			if (state != GUIDialogWindow.DialogState.Cancel)
			{
				string deckName = windowRef.DeckName.Trim();
				if (string.IsNullOrEmpty(deckName))
				{
					GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, "#DECKBUILDER_SAVE_NAME_EMPTY", (IGUIDialogNotification)null, GUIControl.ModalLevelEnum.Default);
				}
				else if (CardCollection.DeckList.ContainsKey(deckName))
				{
					GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, "#deckbuilder_overwrite", delegate(string sId, GUIDialogWindow.DialogState innerstate)
					{
						if (innerstate == GUIDialogWindow.DialogState.Ok)
						{
							DeckProperties deckProperties = CardCollection.DeckList[deckName];
							OnSaveNameSelected(deckName, deckProperties.DeckId);
						}
					}, GUIControl.ModalLevelEnum.Default);
				}
				else if (deckName != deckCards.sourceGroup.DeckName)
				{
					CspUtils.DebugLog("Name change from:" + deckName + " to " + deckCards.sourceGroup.DeckName);
					deckCards.sourceGroup.DeckId = -1;
					OnSaveNameSelected(deckName, deckCards.sourceGroup.DeckId);
				}
				else
				{
					CspUtils.DebugLog("SAVING");
					OnSaveNameSelected(deckName, deckCards.sourceGroup.DeckId);
				}
			}
			else
			{
				AppShell.Instance.EventMgr.Fire(this, new CardCollectionSavedMessage(false));
			}
		}), GUIControl.ModalLevelEnum.Default);
	}

	private void OnSaveNameSelected(string name, int id)
	{
		if (deckCards.sourceGroup == null)
		{
			deckCards.sourceGroup = new CardGroup();
		}
		deckCards.sourceGroup.DeckName = name;
		deckCards.sourceGroup.DeckRecipe = deckCards.ToString();
		WWWForm wWWForm = new WWWForm();
		if (id > -1)
		{
			wWWForm.AddField("deck_id", id);
		}
		wWWForm.AddField("cards", deckCards.sourceGroup.DeckRecipe);
		wWWForm.AddField("deck_name", deckCards.sourceGroup.DeckName);
		int num = (deckCards.TotalCards >= 40) ? 1 : 0;
		wWWForm.AddField("legal", num);
		if (num == 1)
		{
			if (userProfile.LastDeckID != id)
			{
				userProfile.LastDeckID = id;
				userProfile.PersistExtendedData();
			}
		}
		else if (userProfile.LastDeckID == id)
		{
			userProfile.LastDeckID = 0;
			userProfile.PersistExtendedData();
		}
		AppShell.Instance.WebService.StartRequest("resources$users/deck_save.py", OnDeckSaved, wWWForm.data);
	}

	protected void OnDeckSaved(ShsWebResponse response)
	{
		CspUtils.DebugLog("OnDeckSaved");
		if (response.Status == 200)
		{
			DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
			dataWarehouse.Parse();
			int @int = dataWarehouse.GetInt("Deck/ID");
			deckCards.sourceGroup.DeckId = @int;
			deckCards.saved = true;
			CspUtils.DebugLog("set deck id to " + @int);
			userProfile.LastDeckID = @int;
			userProfile.PersistExtendedData();
			AppShell.Instance.EventMgr.Fire(this, new CardCollectionSavedMessage(true));
		}
		else
		{
			CspUtils.DebugLog("Unable to save deck: " + response.Body);
			userProfile.LastDeckID = 0;
			userProfile.PersistExtendedData();
			AppShell.Instance.EventMgr.Fire(this, new CardCollectionSavedMessage(false));
		}
	}

	public void DoNewDeck()
	{
		myCards.Reset(false);
		deckCards.Reset(true);
		AppShell.Instance.EventMgr.Fire(this, new CardCollectionDataMessage(CardCollectionDataMessage.CCDataEvent.DeckCountChanged, 0, 0));
	}

	private void ShowLoadDialog(bool confirmed)
	{
		if (confirmed)
		{
			SHSCardGameLoadSaveDialog windowRef = null;
			GUIManager.Instance.ShowDialog(typeof(SHSCardGameLoadSaveDialog), string.Empty, new GUIDialogNotificationSink(delegate(string Id, IGUIDialogWindow window)
			{
				windowRef = (window as SHSCardGameLoadSaveDialog);
				windowRef.CurrentMode = SHSCardGameLoadSaveDialog.DialogMode.Load;
			}, delegate
			{
			}, delegate
			{
			}, delegate
			{
			}, delegate(string Id, GUIDialogWindow.DialogState state)
			{
				if (state != GUIDialogWindow.DialogState.Cancel)
				{
					DeckProperties selectedDeck = windowRef.SelectedDeck;
					CspUtils.DebugLog("Initializing CardGameLevelInfo with ID " + selectedDeck.DeckId + ": " + selectedDeck.DeckRecipe);
					deckCards.Reset(true);
					myCards.Reset(false);
					deckCards.sourceGroup.DeckId = selectedDeck.DeckId;
					deckCards.sourceGroup.DeckName = selectedDeck.DeckName;
					if (selectedDeck.Legal)
					{
						AppShell.Instance.Profile.LastDeckID = selectedDeck.DeckId;
						AppShell.Instance.Profile.PersistExtendedData();
					}
					StartCoroutine(LoadDeck(selectedDeck.DeckRecipe));
				}
			}), GUIControl.ModalLevelEnum.Default);
		}
		else
		{
			GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkCancelDialog, "#deckbuilder_load_deck_warning", delegate(string Id, GUIDialogWindow.DialogState state)
			{
				if (state == GUIDialogWindow.DialogState.Ok)
				{
					ShowLoadDialog(true);
				}
			}, GUIControl.ModalLevelEnum.Default);
		}
	}

	private IEnumerator LoadDeck(string deckRecipe)
	{
		yield return StartCoroutine(deckCards.sourceGroup.LoadRecipe(deckRecipe, null, false));
		deckCards.Import();
		foreach (CardListCard c in deckCards)
		{
			AddCounterToCard(c);
			myCards.SubtractCard(c, c.Available, false);
		}
		deckCards.Sort(deckCards.Compare);
		deckCards.FreshLayout();
		CardCollectionDataMessage msg = new CardCollectionDataMessage(CardCollectionDataMessage.CCDataEvent.DeckCountChanged, deckCards.TotalCards, deckCards.CountVisible());
		msg.ScrollToTop = true;
		AppShell.Instance.EventMgr.Fire(this, msg);
	}

	public GameObject PickCardPanel(Camera camera, GUIImage renderTexture)
	{
		Vector2 mouseScreenPosition = SHSInput.mouseScreenPosition;
		Rect screenRect = renderTexture.ScreenRect;
		mouseScreenPosition -= new Vector2(screenRect.x, screenRect.y);
		if (!(mouseScreenPosition.x < 0f))
		{
			float x = mouseScreenPosition.x;
			Vector2 size = renderTexture.Size;
			if (!(x > size.x) && !(mouseScreenPosition.y < 0f))
			{
				float y = mouseScreenPosition.y;
				Vector2 size2 = renderTexture.Size;
				if (!(y > size2.y))
				{
					float pixelWidth = camera.pixelWidth;
					float pixelHeight = camera.pixelHeight;
					float x2 = mouseScreenPosition.x;
					Vector2 size3 = renderTexture.Size;
					float x3 = x2 / size3.x * pixelWidth;
					float num = 0f - mouseScreenPosition.y + screenRect.height;
					Vector2 size4 = renderTexture.Size;
					Ray ray = camera.ScreenPointToRay(new Vector3(x3, num / size4.y * pixelHeight));
					RaycastHit hitInfo;
					if (Physics.Raycast(ray, out hitInfo, 10f, 512))
					{
						return hitInfo.collider.gameObject;
					}
					return null;
				}
			}
		}
		return null;
	}

	protected void On3DEvent(CardCollection3DMessage msg)
	{
		switch (msg.Event)
		{
		case CardCollection3DMessage.CC3DEvent.CardMouseLeftClick:
		case CardCollection3DMessage.CC3DEvent.CardMouseRightClick:
			DoTransferCard(msg.Sender, msg.Event);
			break;
		case CardCollection3DMessage.CC3DEvent.CounterReady:
			countersLoaded++;
			break;
		}
	}

	private void OnSortTypeSelected(CardCollectionUIMessage msg)
	{
		if (msg.Event == CardCollectionUIMessage.CCUIEvent.SortTypeSelected)
		{
			GUIToggleButton gUIToggleButton = (GUIToggleButton)msg.Control;
			if (myCards != null && deckCards != null)
			{
				SortedCardList.SortMethod primarySort = (SortedCardList.SortMethod)(int)gUIToggleButton.Tag;
				myCards.PrimarySort = primarySort;
				deckCards.PrimarySort = primarySort;
				myCards.Sort(myCards.Compare);
				myCards.FreshLayout();
				deckCards.Sort(deckCards.Compare);
				deckCards.FreshLayout();
				AppShell.Instance.EventMgr.Fire(this, new CardCollectionDataMessage(CardCollectionDataMessage.CCDataEvent.DeckCountChanged, deckCards.TotalCards, deckCards.CountVisible()));
				AppShell.Instance.EventMgr.Fire(this, new CardCollectionDataMessage(CardCollectionDataMessage.CCDataEvent.CollectionCountChanged, myCards.TotalCards, myCards.CountVisible()));
			}
		}
	}

	private void OnFilterUIEvent(CardCollectionFilterUIMessage msg)
	{
		switch (msg.Filter)
		{
		case CardFilterType.BlockStrength:
		case CardFilterType.BlockSpeed:
		case CardFilterType.BlockElemental:
		case CardFilterType.BlockTech:
		case CardFilterType.BlockAnimal:
		case CardFilterType.BlockEnergy:
		case CardFilterType.FactorStrength:
		case CardFilterType.FactorSpeed:
		case CardFilterType.FactorElemental:
		case CardFilterType.FactorTech:
		case CardFilterType.FactorAnimal:
		case CardFilterType.FactorEnergy:
		{
			GUIButton gUIButton2 = msg.Control as GUIButton;
			filterStates[(int)msg.Filter].active = gUIButton2.IsSelected;
			break;
		}
		case CardFilterType.TeamAvengers:
		case CardFilterType.TeamXmen:
		case CardFilterType.TeamBrotherhood:
		case CardFilterType.TeamSS:
		case CardFilterType.TeamF4:
		case CardFilterType.TeamSpideyFriends:
		{
			GUIButton gUIButton = msg.Control as GUIButton;
			filterStates[(int)msg.Filter].active = gUIButton.IsSelected;
			break;
		}
		case CardFilterType.MinLevel:
		case CardFilterType.MaxLevel:
		{
			GUISlider gUISlider = msg.Control as GUISlider;
			filterStates[(int)msg.Filter].iData = (int)gUISlider.Value;
			break;
		}
		case CardFilterType.TextSearch:
		{
			CardFilter cardFilter = filterStates[(int)msg.Filter];
			if (msg.sData == "Card Search" || msg.sData == string.Empty)
			{
				cardFilter.active = false;
				cardFilter.sData = string.Empty;
			}
			else
			{
				cardFilter.active = true;
				cardFilter.sData = msg.sData;
			}
			break;
		}
		}
		myCards.ApplyFilters(filterStates);
		AppShell.Instance.EventMgr.Fire(this, new CardCollectionDataMessage(CardCollectionDataMessage.CCDataEvent.DeckCountChanged, deckCards.TotalCards, deckCards.CountVisible()));
		AppShell.Instance.EventMgr.Fire(this, new CardCollectionDataMessage(CardCollectionDataMessage.CCDataEvent.CollectionCountChanged, myCards.TotalCards, myCards.CountVisible()));
	}

	public bool IsFilterActive(CardFilterType type)
	{
		return filterStates[(int)type].active;
	}

	public void OnTransactionStepComplete(string step, bool success, TransactionMonitor transaction)
	{
		switch (step)
		{
		case "counters":
			if (success)
			{
				foreach (CardListCard myCard in myCards)
				{
					myCard.UpdateCounter();
				}
			}
			break;
		}
	}

	private CardListCard GetCardFromObject(GameObject ob, SortedCardList list)
	{
		foreach (CardListCard item in list)
		{
			if (ob.Equals(item.CardObj))
			{
				return item;
			}
		}
		return null;
	}

	public Texture GetFullSizeTexture(BattleCard card)
	{
		if (FullSizeTextures.ContainsKey(card.Type))
		{
			FullSizeTextureTime[card.Type] = Time.time;
			return FullSizeTextures[card.Type];
		}
		Texture2D texture2D = CardManager.LoadCardTexture(card.Type, card.BundleName);
		if (FullSizeTextures.Count > 25)
		{
			float num = Time.time;
			string key = string.Empty;
			foreach (KeyValuePair<string, float> item in FullSizeTextureTime)
			{
				if (item.Value < num)
				{
					num = item.Value;
					key = item.Key;
				}
			}
			FullSizeTextures.Remove(key);
			FullSizeTextureTime.Remove(key);
		}
		FullSizeTextures.Add(card.Type, texture2D);
		FullSizeTextureTime.Add(card.Type, Time.time);
		return texture2D;
	}

	public void BuildAutoDeck(string themeName)
	{
		DeckBuilderConfigDefinition deckBuilderConfigDefinition = DeckBuilderConfigDefinition.Instance;
		DeckTheme deckTheme = deckBuilderConfigDefinition.FindTheme(themeName);
		if (deckTheme == null)
		{
			deckTheme = new DeckTheme();
		}
		StartCoroutine(BuildAutoDeck(deckTheme));
	}

	protected void AddWorkingAffinity(CardListCard c, CardAffinity abilityAffinity, CardAffinity rawCardAffinity)
	{
		foreach (KeyValuePair<string, int> item in c.Affinity)
		{
			if (item.Key.StartsWith("ability"))
			{
				KeyValuePair<string, int> kvp = new KeyValuePair<string, int>(item.Key.ToLower(), item.Value);
				abilityAffinity.Add(kvp);
			}
			else
			{
				rawCardAffinity.Add(item);
			}
		}
	}

	public IEnumerator BuildAutoDeck(DeckTheme theme)
	{
		DeckBuilderConfigDefinition config = DeckBuilderConfigDefinition.Instance;
		CardAffinity workingAffinity = new CardAffinity();
		CardAffinity rawCardAffinity = new CardAffinity();
		CardAffinity abilityAffinity = new CardAffinity();
		List<CardListCard> pickList = new List<CardListCard>();
		if (theme.decklist != string.Empty)
		{
			if (!CardCollection.Contains(theme.decklist))
			{
				CspUtils.DebugLog("Recipe not contained within user's card collection.");
				yield break;
			}
			Dictionary<string, int> cardDict = CardManager.ParseRecipe(theme.decklist);
			foreach (KeyValuePair<string, int> kvp4 in cardDict)
			{
				foreach (CardListCard c2 in myCards)
				{
					if (c2.Type == kvp4.Key)
					{
						for (int k = 0; k < kvp4.Value; k++)
						{
							MoveCardToDeck(c2);
							yield return new WaitForEndOfFrame();
						}
					}
				}
			}
		}
		if (myCards.TotalCards + deckCards.TotalCards < 40)
		{
			CspUtils.DebugLog("Insufficient card count for an auto-deck");
			yield break;
		}
		rawCardAffinity.Add(theme.affinity);
		foreach (CardListCard card4 in myCards)
		{
			card4.GenerateAffinityTags();
		}
		foreach (CardListCard c in deckCards)
		{
			AddWorkingAffinity(c, abilityAffinity, rawCardAffinity);
		}
		do
		{
			workingAffinity.Clear();
			pickList.Clear();
			int deckCount = deckCards.TotalCards;
			workingAffinity.Add("blockA", (int)(config.BlockMagnitude * (float)config.BlockWeightMultiplier * (float)(deckCount / 40) * (float)(config.ByBlockExpected - deckCards.CountByBlock(BattleCard.Factor.Animal))));
			workingAffinity.Add("blockE", (int)(config.BlockMagnitude * (float)config.BlockWeightMultiplier * (float)(deckCount / 40) * (float)(config.ByBlockExpected - deckCards.CountByBlock(BattleCard.Factor.Elemental))));
			workingAffinity.Add("blockN", (int)(config.BlockMagnitude * (float)config.BlockWeightMultiplier * (float)(deckCount / 40) * (float)(config.ByBlockExpected - deckCards.CountByBlock(BattleCard.Factor.Energy))));
			workingAffinity.Add("blockP", (int)(config.BlockMagnitude * (float)config.BlockWeightMultiplier * (float)(deckCount / 40) * (float)(config.ByBlockExpected - deckCards.CountByBlock(BattleCard.Factor.Speed))));
			workingAffinity.Add("blockS", (int)(config.BlockMagnitude * (float)config.BlockWeightMultiplier * (float)(deckCount / 40) * (float)(config.ByBlockExpected - deckCards.CountByBlock(BattleCard.Factor.Strength))));
			workingAffinity.Add("blockT", (int)(config.BlockMagnitude * (float)config.BlockWeightMultiplier * (float)(deckCount / 40) * (float)(config.ByBlockExpected - deckCards.CountByBlock(BattleCard.Factor.Tech))));
			if (deckCount > 0)
			{
				workingAffinity.Add("factorA", (int)(config.FactorMagnitude * (float)config.FactorWeightMultiplier * (float)deckCards.CountByFactor(BattleCard.Factor.Animal) / (float)deckCount));
				workingAffinity.Add("factorE", (int)(config.FactorMagnitude * (float)config.FactorWeightMultiplier * (float)deckCards.CountByFactor(BattleCard.Factor.Elemental) / (float)deckCount));
				workingAffinity.Add("factorN", (int)(config.FactorMagnitude * (float)config.FactorWeightMultiplier * (float)deckCards.CountByFactor(BattleCard.Factor.Energy) / (float)deckCount));
				workingAffinity.Add("factorP", (int)(config.FactorMagnitude * (float)config.FactorWeightMultiplier * (float)deckCards.CountByFactor(BattleCard.Factor.Speed) / (float)deckCount));
				workingAffinity.Add("factorS", (int)(config.FactorMagnitude * (float)config.FactorWeightMultiplier * (float)deckCards.CountByFactor(BattleCard.Factor.Strength) / (float)deckCount));
				workingAffinity.Add("factorT", (int)(config.FactorMagnitude * (float)config.FactorWeightMultiplier * (float)deckCards.CountByFactor(BattleCard.Factor.Tech) / (float)deckCount));
			}
			for (int j = 1; j <= config.Max_Level; j++)
			{
				workingAffinity.Add("level" + j, (int)(config.LevelMagnitude * (float)config.LevelWeightMultiplier * (float)(deckCount / 40) * (float)(config.ByLevelExpected - deckCards.CountByLevel(j))));
			}
			foreach (KeyValuePair<string, int> kvp3 in rawCardAffinity)
			{
				workingAffinity.Add(kvp3);
			}
			int weight2 = 0;
			foreach (CardListCard card3 in myCards)
			{
				if (!deckCards.ContainsCard(card3))
				{
					card3.score = 0;
					foreach (string tag in card3.Tags)
					{
						if (workingAffinity.ContainsKey(tag))
						{
							card3.score += workingAffinity[tag];
						}
					}
					foreach (KeyValuePair<string, int> kvp2 in abilityAffinity)
					{
						if (card3.AbilityText.Contains(kvp2.Key.Substring(7)))
						{
							card3.score += kvp2.Value;
						}
					}
					foreach (KeyValuePair<string, int> kvp in card3.Affinity)
					{
						weight2 = (int)((float)kvp.Value * ComputeDesirabilityWeight(kvp.Key, deckCards));
						card3.score += weight2;
					}
					pickList.Add(card3);
				}
			}
			if (pickList.Count < 1)
			{
				CspUtils.DebugLog("Ran out of cards to pick from!");
				break;
			}
			pickList.Sort(delegate(CardListCard x, CardListCard y)
			{
				return y.score.CompareTo(x.score);
			});
			int index = 0;
			float chance = config.BaseTopPickChance;
			while (index < pickList.Count - 1 && !(UnityEngine.Random.Range(0f, 1f) < chance))
			{
				index++;
				chance *= config.NextPickChanceMult;
			}
			CardListCard selection = pickList[index];
			int max2 = Math.Min(selection.Available, 4);
			max2 = Math.Min(max2, UnityEngine.Random.Range(1, 20));
			for (int i = 0; i < max2; i++)
			{
				if (deckCards.TotalCards >= 40)
				{
					break;
				}
				MoveCardToDeck(selection);
				AddWorkingAffinity(selection, abilityAffinity, rawCardAffinity);
			}
			yield return new WaitForEndOfFrame();
		}
		while (deckCards.TotalCards < 40);
		deckCards.Sort(delegate(CardListCard card1, CardListCard card2)
		{
			if (card1.Level != card2.Level)
			{
				return card1.Level.CompareTo(card2.Level);
			}
			if (card1.Damage != card2.Damage)
			{
				return card1.Damage.CompareTo(card2.Damage);
			}
			BattleCard.Factor[] attackFactors = card1.AttackFactors;
			BattleCard.Factor[] attackFactors2 = card2.AttackFactors;
			int num = attackFactors[0].CompareTo(attackFactors2[0]);
			if (num != 0)
			{
				return num;
			}
			num = card1.HeroName.CompareTo(card2.HeroName);
			return (num != 0) ? num : card1.Name.CompareTo(card2.Name);
		});
		deckCards.FreshLayout();
		ShsAudioSource.PlayAutoSound(uiSfx.GetSource("build_deck"));
		CardCollectionDataMessage msg = new CardCollectionDataMessage(CardCollectionDataMessage.CCDataEvent.DeckCountChanged, deckCards.TotalCards, deckCards.CountVisible());
		msg.ScrollToTop = true;
		AppShell.Instance.EventMgr.Fire(this, msg);
		AppShell.Instance.EventMgr.Fire(this, new CardCollectionDataMessage(CardCollectionDataMessage.CCDataEvent.CollectionCountChanged, myCards.TotalCards, myCards.CountVisible()));
	}

	public static float ComputeDesirabilityWeight(string key, SortedCardList cardList)
	{
		int num = 0;
		if (key.StartsWith("ability"))
		{
			foreach (CardListCard card in cardList)
			{
				if (card.AbilityText.Contains(key.Substring(7).ToLower()))
				{
					num += card.Available;
				}
			}
		}
		else
		{
			foreach (CardListCard card2 in cardList)
			{
				if (card2.Tags.Contains(key))
				{
					num += card2.Available;
				}
			}
		}
		return (float)num / (float)cardList.TotalCards;
	}
}
