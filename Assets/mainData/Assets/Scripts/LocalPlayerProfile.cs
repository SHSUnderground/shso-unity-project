using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;

public class LocalPlayerProfile : UserProfile
{
	public override SHSCounterBank CounterBank
	{
		get
		{
			return AppShell.Instance.CounterManager.DefaultCounterBank;
		}
	}

	public override bool IsShieldPlayCapable
	{
		get
		{
			return Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldPlayAllow);
		}
	}

	public override int SidekickID
	{
		get
		{
			return PetDataManager.getCurrentPet();
		}
	}

	public override int SidekickTier
	{
		get
		{
			return sidekickTier;
		}
	}

	public override string SelectedCostume
	{
		get
		{
			return selectedCostume;
		}
		set
		{
			if (!(selectedCostume == value))
			{
				selectedCostume = value;
				if (selectedCostume != null)
				{
					WWWForm wWWForm = new WWWForm();
					wWWForm.AddField("costume", selectedCostume);
					AppShell.Instance.WebService.StartRequest("resources$users/" + AppShell.Instance.Profile.UserId + "/set_costume_info", delegate(ShsWebResponse response)
					{
						if (response.Status != 200)
						{
							CspUtils.DebugLog("SelectedCostume set failure: " + response.Status + ":" + response.Body);
						}
					}, wWWForm.data);
				}
			}
		}
	}

	public LocalPlayerProfile()
	{
	}

	public LocalPlayerProfile(string xmlData, OnProfileLoaded loadCallback)
		: base(xmlData, loadCallback)
	{
		profileType = ProfileTypeEnum.LocalPlayer;
		AppShell.Instance.EventMgr.AddListener<ShoppingItemPurchasedMessage>(OnPurchase);
		AppShell.Instance.EventMgr.AddListener<LeveledUpMessage>(OnLevelUp);
		AppShell.Instance.EventMgr.AddListener<PlayerInfoUpdateMessage>(OnInfoUpdated);
		AppShell.Instance.EventMgr.AddListener<BrawlerStageBegin>(OnBrawlerStageBegin);
		xpMultiplier = AppShell.Instance.cachedXPMultiplier;
	}

	public override void InitializeFromData(RemotePlayerProfileJsonData jsonData, OnProfileLoaded loadCallback)
	{
		throw new NotImplementedException();
	}

	public override void InitializeFromData(RemotePlayerProfileJsonData jsonData)
	{
		throw new NotSupportedException();
	}

	public override void InitializeFromData(string xmlData)
	{
		CspUtils.DebugLog(xmlData);
		if (string.IsNullOrEmpty(xmlData))
		{
			CspUtils.DebugLog("Failed to get valid UserProfile XML - Can't initialize UserProfile from an empty or null string.");
			base.Initialized = false;
			return;
		}
		DataWarehouse dataWarehouse = new DataWarehouse(xmlData);
		dataWarehouse.Parse();
		InitializeCoreProfileFromData(dataWarehouse);
		fetchTransaction = TransactionMonitor.CreateTransactionMonitor("User Profile Fetch", OnFetchComplete, 0f, null);
		availableItems = new ItemCollection();
		StartBulkFetch();
		StartCardCollectionFetch();
		availableFriends = new FriendCollection();
		StartFriendFetch();
		base.Initialized = true;
	}

	protected void InitializeCoreProfileFromData(DataWarehouse data)
	{
		playerName = data.GetString("//player_name");
		userId = data.GetInt("//id");
		creationDate = data.TryGetString("//created", "Unknown");
		squadLevel = data.TryGetInt("//squad_level", 0);
		lastSelectedCostume = data.TryGetString("//extended_data/LastCostume", null);
		lastDeckID = data.TryGetInt("//extended_data/LastDeckID", 0);
		firstCardGame = data.TryGetBool("//extended_data/FirstCardGame", true);
		base.ConsecutiveDaysPlayed = data.TryGetInt("//consecutive_days", 0);
		loginsToday = data.TryGetInt("//logins_today", 0);
		string text = data.TryGetString("//time_til_midnight", null);
		if (!string.IsNullOrEmpty(text))
		{
			base.TimeUntilMidnight = TimeSpan.Parse(text);
		}
		if (ChallengeManagerServerStub.CurrentMode == ChallengeManagerServerStub.ServerMode.ClientSimulation)
		{
			currentChallenge = PlayerPrefs.GetInt("CM_CC", 1);
			lastChallenge = currentChallenge - 1;
		}
		else
		{
			lastChallenge = data.TryGetInt("//last_celebrated", 0);
			currentChallenge = data.TryGetInt("//current_challenge", 0);
			if (currentChallenge == 0)
			{
				CspUtils.DebugLog("Current Challenge passed down by server is invalid. Should never be lower than 1. Value:" + currentChallenge);
			}
			if (lastChallenge >= currentChallenge)
			{
				CspUtils.DebugLog("Last (celebrated) Challenge passed down by server is invalid. Should never be greater than/equal to current challenge. Value:" + lastChallenge);
			}
		}
		demoHack = data.TryGetBool("//extended_data/DemoHack", false);	
		DataWarehouse data2 = data.GetData("//heroes");
		availableCostumes = new HeroCollection(data2);
		//CspUtils.DebugLog("data.xmlString: " + data.xmlString);
		DataWarehouse data3 = data.GetData("//currency");
		//CspUtils.DebugLog("data3.navigator.OuterXml: " + data3.navigator.OuterXml);
		//CspUtils.DebugLog("data3.navigator.InnerXml: " + data3.navigator.InnerXml);
		InitializeCurrencyFromData(data3);
		prizeWheelState = new BitArray(BitConverter.GetBytes(data.GetInt("//prize_wheel/earned_stops")));
		Singleton<Entitlements>.instance.Configure(data);
		if (string.IsNullOrEmpty(lastSelectedCostume) && availableCostumes.Count >= 1)
		{
			using (Dictionary<string, HeroPersisted>.ValueCollection.Enumerator enumerator = availableCostumes.Values.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					HeroPersisted current = enumerator.Current;
					lastSelectedCostume = current.Name;
				}
			}
		}
		medallionID = data.TryGetInt("//medallion_id", -1);
		titleID = data.TryGetInt("//title_id", -1);
		sidekickID = data.TryGetInt("//sidekick_id", -1);
		sidekickTier = data.TryGetInt("//sidekick_tier", -1);
		achievementPoints = data.TryGetInt("//achievement_points", 0);
		trackerData = data.TryGetString("//tracker_data", string.Empty);
		AchievementManager.initDestinyInfoForProfile(this);
		PrefsManager.init(data.GetData("//prefs"));
		offline = false;
		base.Initialized = true;
	}

	~LocalPlayerProfile()
	{
		AppShell.Instance.EventMgr.RemoveListener<LeveledUpMessage>(OnLevelUp);
		AppShell.Instance.EventMgr.RemoveListener<ShoppingItemPurchasedMessage>(OnPurchase);
	}

	private void OnPurchase(ShoppingItemPurchasedMessage message)
	{
		switch (message.ItemType)
		{
		case OwnableDefinition.Category.Hero:
			availableCostumes.Add(message.OwnableName, new HeroPersisted(message.OwnableName));
			break;
		case OwnableDefinition.Category.Mission:
			availableMissions.Add(message.OwnableId, new AvailableMission(message.OwnableId));
			break;
		case OwnableDefinition.Category.Badge:
			AppShell.Instance.Profile.StartBadgesFetch();
			break;
		}
	}

	public override void FetchDataBasedOnCategory(OwnableDefinition.Category category)
	{
		switch (category)
		{
		case OwnableDefinition.Category.TicketPack:
		case OwnableDefinition.Category.MoneyBag:
		case OwnableDefinition.Category.Fractal:
		case OwnableDefinition.Category.Blueprint:
			break;
		case OwnableDefinition.Category.Potion:
			AppShell.Instance.Profile.StartPotionFetch();
			break;
		case OwnableDefinition.Category.Mission:
			AppShell.Instance.Profile.StartMissionsFetch();
			break;
		case OwnableDefinition.Category.HQRoom:
			AppShell.Instance.Profile.StartHQRoomsFetch();
			break;
		case OwnableDefinition.Category.Quest:
			AppShell.Instance.Profile.StartQuestsFetch();
			break;
		case OwnableDefinition.Category.Card:
			AppShell.Instance.Profile.StartCardCollectionFetch();
			break;
		case OwnableDefinition.Category.BoosterPack:
			AppShell.Instance.Profile.StartBoosterPacksFetch();
			break;
		case OwnableDefinition.Category.Badge:
			AppShell.Instance.Profile.StartBadgesFetch();
			break;
		case OwnableDefinition.Category.Sidekick:
			AppShell.Instance.Profile.StartPetsFetch();
			break;
		case OwnableDefinition.Category.Title:
			AppShell.Instance.Profile.StartTitlesFetch();
			break;
		case OwnableDefinition.Category.Medallion:
			AppShell.Instance.Profile.StartMedallionsFetch();
			break;
		case OwnableDefinition.Category.Bundle:
			AppShell.Instance.Profile.StartBundlesFetch();
			break;
		case OwnableDefinition.Category.Craft:
			AppShell.Instance.Profile.StartCraftFetch();
			break;
		case OwnableDefinition.Category.Hero:
			AppShell.Instance.Profile.StartHeroFetch();
			break;
		case OwnableDefinition.Category.MysteryBox:
			AppShell.Instance.Profile.StartMysteryBoxFetch();
			break;
		case OwnableDefinition.Category.SidekickUpgrade:
			AppShell.Instance.Profile.StartSidekickUpgradesFetch();
			break;
		}
	}

	private void OnLevelUp(LeveledUpMessage message)
	{
		StartInventoryFetch();
	}

	private void OnInfoUpdated(PlayerInfoUpdateMessage message)
	{
		if (message.player.PlayerId == base.UserId)
		{
			squadLevel = message.player.SquadLevel;
		}
	}

	public override void refreshSocialAbilities()
	{
		socialAbilities = new List<SpecialAbility>();
		HeroDefinition heroDef = OwnableDefinition.getHeroDef(OwnableDefinition.HeroNameToHeroID[AppShell.Instance.Profile.SelectedCostume]);
		if (heroDef != null)
		{
			foreach (Keyword keyword in heroDef.ownableDef.getKeywords())
			{
				if (keyword.hasContext("social"))
				{
					HeroSpecialAbility item = HeroSpecialAbility.fromKeyword(heroDef.name, keyword);
					socialAbilities.Add(item);
				}
			}
		}
		PetData data = PetDataManager.getData(PetDataManager.getCurrentPet());
		if (data != null)
		{
			foreach (SpecialAbility ability in data.abilities)
			{
				if (ability.displaySpace != "social")
				{
					CspUtils.DebugLog("rejecting special ability " + ability.name + " because its display space is " + ability.displaySpace);
				}
				else if (ability.isUnlocked())
				{
					ability.usesLeft = ability.uses;
					socialAbilities.Add(ability);
				}
			}
		}
		AppShell.Instance.EventMgr.Fire(null, new PlayerSpecialAttacksChanged());
	}

	protected void OnBrawlerStageBegin(BrawlerStageBegin msg)
	{
		if (msg.whatStage == 1)
		{
			brawlerAbilities = new List<SpecialAbility>();
			PetData data = PetDataManager.getData(PetDataManager.getCurrentPet());
			if (data != null)
			{
				foreach (SpecialAbility ability in data.abilities)
				{
					CspUtils.DebugLog("considering " + ability.name);
					if (ability.displaySpace != "brawler")
					{
						CspUtils.DebugLog("rejecting special ability " + ability.name + " because its display space is " + ability.displaySpace);
					}
					else if (!ability.isUnlocked())
					{
						CspUtils.DebugLog("rejecting special ability " + ability.name + " because it is not unlocked");
					}
					else
					{
						bool flag = false;
						foreach (SpecialAbility brawlerAbility in brawlerAbilities)
						{
							if (brawlerAbility.sameAs(ability))
							{
								flag = true;
								brawlerAbility.usesLeft += ability.uses;
								CspUtils.DebugLog("special ability " + ability.name + " is getting added to an already existing ability in the list " + brawlerAbility);
								break;
							}
						}
						if (!flag)
						{
							ability.usesLeft = ability.uses;
							CspUtils.DebugLog("adding new ability " + ability.ToString());
							brawlerAbilities.Add(ability);
						}
					}
				}
			}
			foreach (SpecialAbility brawlerAbility2 in brawlerAbilities)
			{
				CspUtils.DebugLog("  " + brawlerAbility2 + " " + brawlerAbility2.usesLeft);
			}
			AppShell.Instance.EventMgr.Fire(null, new PlayerSpecialAttacksChanged());
		}
	}

	public override void useSpecialAbility(int specialAbilityID)
	{
		CspUtils.DebugLog("useSpecialAbility " + specialAbilityID);
		foreach (SpecialAbility brawlerAbility in brawlerAbilities)
		{
			if (brawlerAbility.specialAbilityID == specialAbilityID)
			{
				brawlerAbility.execute();
				if (--brawlerAbility.usesLeft <= 0)
				{
					brawlerAbilities.Remove(brawlerAbility);
				}
				AppShell.Instance.EventMgr.Fire(null, new PlayerSpecialAttacksChanged());
				return;
			}
		}
		if (BrawlerController.Instance == null)
		{
			foreach (SpecialAbility socialAbility in socialAbilities)
			{
				if (socialAbility.specialAbilityID == specialAbilityID)
				{
					socialAbility.execute();
					if (!(socialAbility is SidekickSpecialAbilityCooldown) && --socialAbility.usesLeft <= 0)
					{
						socialAbilities.Remove(socialAbility);
					}
					AppShell.Instance.EventMgr.Fire(null, new PlayerSpecialAttacksChanged());
					break;
				}
			}
		}
	}

	protected override void InitializeCurrencyFromData(DataWarehouse currencyData)
	{
		silver = currencyData.GetInt("tokens");
		gold = currencyData.GetInt("coins");
		tickets = currencyData.TryGetInt("tickets", 0);
		//shards = 10999; // CSP    
		//CspUtils.DebugLog("currencyData: " + currencyData.navigator.InnerXml);
		shards = currencyData.TryGetInt("shards", 0);
		CspUtils.DebugLog("shards: " + shards);
		AppShell.Instance.EventMgr.Fire(null, new CurrencyUpdateMessage());
	}

	public void SyncExternalGoldBalance(int newBalance)
	{
		gold = newBalance;
		CspUtils.DebugLog("Setting new balance for gold to:" + newBalance);
		AppShell.Instance.EventMgr.Fire(null, new CurrencyUpdateMessage());
	}

	public override void PersistExtendedData()
	{
		ShsWebService webService = AppShell.Instance.WebService;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("<LastCostume>").Append(lastSelectedCostume).Append("</LastCostume>");
		stringBuilder.Append("<LastDeckID>").Append(lastDeckID.ToString()).Append("</LastDeckID>");
		stringBuilder.Append("<FirstCardGame>").Append(firstCardGame.ToString().ToLower()).Append("</FirstCardGame>");
		stringBuilder.Append("<DemoHack>").Append(demoHack.ToString().ToLower()).Append("</DemoHack>");
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("ExtendedData", stringBuilder.ToString());
		webService.StartRequest("resources$users/" + userId, OnExtendedDataPersistedResponse, wWWForm.data);
	}

	protected void OnExtendedDataPersistedResponse(ShsWebResponse response)
	{
		if (response.Status == 200)
		{
			if (string.IsNullOrEmpty(response.Body))
			{
				CspUtils.DebugLog("Failed to get valid UserProfile XML - Can't initialize UserProfile from an empty or null string.");
				return;
			}
			DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
			dataWarehouse.Parse();
			InitializeCoreProfileFromData(dataWarehouse);
		}
		else
		{
			CspUtils.DebugLog("Unable to save extended settings. Response: " + response.Status + " : " + response.Body);
		}
	}

	protected void OnFriendsFetchResponse(ShsWebResponse response)
	{
		if (response.Status != 200)
		{
			if (fetchTransaction != null)
			{
				fetchTransaction.FailStep("friends", "Friend request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			CspUtils.DebugLog("Friends request failed for <" + response.RequestUri + "> with status " + response.Status);
		}
		else
		{
			availableFriends.OnFriendsLoaded(response);
			if (fetchTransaction != null)
			{
				fetchTransaction.CompleteStep("friends");
			}
		}
	}

	protected void OnFetchComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		fetchTransaction = null;
		if (exit != 0)
		{
			CspUtils.DebugLog("UserProfile initialization failed: " + error);
		}
		else if (ProfileLoadedHandler != null)
		{
			ProfileLoadedHandler(this);
		}
	}

	public override void CollectShard()
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("hero_name", AppShell.Instance.Profile.SelectedCostume);
		wWWForm.AddField("zone_name", SocialSpaceController.Instance.ShortZoneName);
		AppShell.Instance.WebService.StartRequest("resources$users/" + AppShell.Instance.Profile.UserId + "/shard_collect/", delegate(ShsWebResponse response)
		{
			if (response.Status == 200)
			{
				CspUtils.DebugLog("shard_collect called successfully");
			}
			else
			{
				CspUtils.DebugLog("shard_collect failed");
			}
		}, wWWForm.data);
	}

	public override void StartCurrencyFetch()
	{
		AppShell.Instance.WebService.StartRequest("resources$users/" + base.UserId + "/currency", OnCurrencyWebResponse);
	}

	public void OnCurrencyWebResponse(ShsWebResponse response)
	{
		if (response.Status == 200)
		{
			try
			{
				DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
				dataWarehouse.Parse();
				dataWarehouse = dataWarehouse.GetData("currency");
				InitializeCurrencyFromData(dataWarehouse);
			}
			catch (Exception arg)
			{
				CspUtils.DebugLog("Exception occurred while processing web response for currency: <" + arg + ">.");
			}
		}
		else
		{
			CspUtils.DebugLog("Failed to get currency data from web service!");
		}
	}

	public override void StartBulkFetch()
	{
		updateCount++;
		if (fetchTransaction != null)
		{
			fetchTransaction.AddStep("bulk");
			fetchTransaction.AddStep("missions");
			fetchTransaction.AddStep("hqrooms");
			fetchTransaction.AddStep("inventory");
			fetchTransaction.AddStep("friends");
			fetchTransaction.AddStep("boosters");
			fetchTransaction.AddStep("quests");
			fetchTransaction.AddStep("potions");
			fetchTransaction.AddStep("badges");
			fetchTransaction.AddStep("pets");
			fetchTransaction.AddStep("titles");
			fetchTransaction.AddStep("medallions");
			fetchTransaction.AddStep("craft");
			fetchTransaction.AddStep("bundles");
			fetchTransaction.AddStep("sidekickUpgrades");
			fetchTransaction.AddStep("mysterybox");
			fetchTransaction.AddStep("gear");
		}
		OwnablesManager.StartOwnablesFetch(OwnableDefinition.Category.All, delegate(ShsWebResponse response, OwnableDefinition.Category category)
		{
			updateCount--;
			OnBulkFetchResponse(response, category, null);
		}, base.UserId);
	}

	protected void OnBulkFetchResponse(ShsWebResponse response, OwnableDefinition.Category category, OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		if (response.Status != 200)
		{
			if (fetchTransaction != null)
			{
				fetchTransaction.FailStep("bulk", "Ownables request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			else
			{
				CspUtils.DebugLog("Bulk request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			if (completionCallback != null)
			{
				completionCallback(false);
			}
			return;
		}
		if (fetchTransaction != null)
		{
			fetchTransaction.CompleteStep("bulk");
		}
		CspUtils.DebugLog(response.Body);
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(response.Body);
		ShsWebResponse shsWebResponse = response.Copy();
		XmlDocument xmlDocument2 = parseBulkInventory(xmlDocument, OwnableDefinition.Category.HQItem);
		shsWebResponse.Body = xmlDocument2.OuterXml;
		OnInventoryFetchResponse(shsWebResponse, OwnableDefinition.Category.HQItem, FireInventoryCompleteMessage);
		xmlDocument2 = parseBulkInventory(xmlDocument, OwnableDefinition.Category.Mission);
		shsWebResponse.Body = xmlDocument2.OuterXml;
		OnMissionsFetchResponse(shsWebResponse, OwnableDefinition.Category.Mission, FireMissionCompleteMessage);
		xmlDocument2 = parseBulkInventory(xmlDocument, OwnableDefinition.Category.HQRoom);
		shsWebResponse.Body = xmlDocument2.OuterXml;
		OnHQRoomsFetchResponse(shsWebResponse, OwnableDefinition.Category.HQRoom, null);
		xmlDocument2 = parseBulkInventory(xmlDocument, OwnableDefinition.Category.BoosterPack);
		shsWebResponse.Body = xmlDocument2.OuterXml;
		OnBoosterPacksFetchResponse(shsWebResponse, OwnableDefinition.Category.BoosterPack, null);
		xmlDocument2 = parseBulkInventory(xmlDocument, OwnableDefinition.Category.Quest);
		shsWebResponse.Body = xmlDocument2.OuterXml;
		OnQuestsFetchResponse(shsWebResponse, OwnableDefinition.Category.Quest, null);
		xmlDocument2 = parseBulkInventory(xmlDocument, OwnableDefinition.Category.Potion);
		shsWebResponse.Body = xmlDocument2.OuterXml;
		OnPotionsFetchResponse(shsWebResponse, OwnableDefinition.Category.Potion, FirePotionCompleteMessage);
		xmlDocument2 = parseBulkInventory(xmlDocument, OwnableDefinition.Category.Badge);
		shsWebResponse.Body = xmlDocument2.OuterXml;
		OnBadgesFetchResponse(shsWebResponse, OwnableDefinition.Category.Badge, null);
		xmlDocument2 = parseBulkInventory(xmlDocument, OwnableDefinition.Category.Sidekick);
		shsWebResponse.Body = xmlDocument2.OuterXml;
		OnPetsFetchResponse(shsWebResponse, OwnableDefinition.Category.Sidekick, null);
		xmlDocument2 = parseBulkInventory(xmlDocument, OwnableDefinition.Category.Title);
		shsWebResponse.Body = xmlDocument2.OuterXml;
		OnTitlesFetchResponse(shsWebResponse, OwnableDefinition.Category.Title, null);
		xmlDocument2 = parseBulkInventory(xmlDocument, OwnableDefinition.Category.Medallion);
		shsWebResponse.Body = xmlDocument2.OuterXml;
		OnMedallionsFetchResponse(shsWebResponse, OwnableDefinition.Category.Medallion, null);
		xmlDocument2 = parseBulkInventory(xmlDocument, OwnableDefinition.Category.Craft);
		shsWebResponse.Body = xmlDocument2.OuterXml;
		OnCraftFetchResponse(shsWebResponse, OwnableDefinition.Category.Craft, null);
		xmlDocument2 = parseBulkInventory(xmlDocument, OwnableDefinition.Category.Bundle);
		shsWebResponse.Body = xmlDocument2.OuterXml;
		OnBundlesFetchResponse(shsWebResponse, OwnableDefinition.Category.Bundle, null);
		xmlDocument2 = parseBulkInventory(xmlDocument, OwnableDefinition.Category.SidekickUpgrade);
		shsWebResponse.Body = xmlDocument2.OuterXml;
		OnSidekickUpgradesFetchResponse(shsWebResponse, OwnableDefinition.Category.SidekickUpgrade, null);
		xmlDocument2 = parseBulkInventory(xmlDocument, OwnableDefinition.Category.MysteryBox);
		shsWebResponse.Body = xmlDocument2.OuterXml;
		OnMysteryBoxFetchResponse(shsWebResponse, OwnableDefinition.Category.MysteryBox, null);
		xmlDocument2 = parseBulkInventory(xmlDocument, OwnableDefinition.Category.Gear);
		shsWebResponse.Body = xmlDocument2.OuterXml;
		OnGearFetchResponse(shsWebResponse, OwnableDefinition.Category.Gear, null);
		CspUtils.DebugLog("OnGearDataFetchResponse");
		OnGearDataFetchResponse(response);
		if (completionCallback != null)
		{
			completionCallback(true);
		}
	}

	private void OnGearDataFetchResponse(ShsWebResponse response)
	{
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		GearDataLoadCollection gearDataLoadCollection = new GearDataLoadCollection(dataWarehouse);
		foreach (GearDataCollectionItem value in gearDataLoadCollection.Values)
		{
			Gear gear = new Gear(value);
			gearCollection.Add(gear.gearID, gear);
		}
	}

	private XmlDocument parseGearInventory(XmlDocument bulkInventoryDocument)
	{
		XmlNodeList xmlNodeList = bulkInventoryDocument.SelectNodes("/masterinventory/gear");
		XmlDocument xmlDocument = new XmlDocument();
		XmlNode xmlNode = xmlDocument.CreateNode(XmlNodeType.Element, "inventory", string.Empty);
		xmlDocument.AppendChild(xmlNode);
		foreach (XmlNode item in xmlNodeList)
		{
			xmlNode.AppendChild(xmlDocument.ImportNode(item, true));
		}
		return xmlDocument;
	}

	private XmlDocument parseBulkInventory(XmlDocument bulkInventoryDocument, OwnableDefinition.Category category)
	{
		XmlNodeList xmlNodeList = bulkInventoryDocument.SelectNodes("/masterinventory/inventory/item[category=\"" + OwnableDefinition.getStringFromCategory(category) + "\"]");
		XmlDocument xmlDocument = new XmlDocument();
		XmlNode xmlNode = xmlDocument.CreateNode(XmlNodeType.Element, "inventory", string.Empty);
		xmlDocument.AppendChild(xmlNode);
		foreach (XmlNode item in xmlNodeList)
		{
			xmlNode.AppendChild(xmlDocument.ImportNode(item, true));
		}
		return xmlDocument;
	}

	public override void StartInventoryFetch()
	{
		StartInventoryFetch(FireInventoryCompleteMessage);
	}

	private void FireInventoryCompleteMessage(bool Success)
	{
		AppShell.Instance.EventMgr.Fire(this, new InventoryFetchCompleteMessage(Success));
	}

	public override void StartInventoryFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		updateCount++;
		OwnablesManager.StartOwnablesFetch(OwnableDefinition.Category.HQItem, delegate(ShsWebResponse response, OwnableDefinition.Category category)
		{
			updateCount--;
			OnInventoryFetchResponse(response, category, completionCallback);
		}, base.UserId);
	}

	protected void OnInventoryFetchResponse(ShsWebResponse response, OwnableDefinition.Category category, OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		if (response.Status != 200)
		{
			if (fetchTransaction != null)
			{
				fetchTransaction.FailStep("inventory", "Ownables request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			else
			{
				CspUtils.DebugLog("Inventory request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			if (completionCallback != null)
			{
				completionCallback(false);
			}
			return;
		}
		availableItems.OnInventoryLoaded(response);
		availableItems.ResetPlacedValues();
		if (fetchTransaction != null)
		{
			fetchTransaction.CompleteStep("inventory");
		}
		if (completionCallback != null)
		{
			completionCallback(true);
		}
	}

	public override void StartGearFetch()
	{
		StartGearFetch(FireMysteryBoxCompleteMessage);
	}

	public override void StartGearFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		OwnablesManager.StartOwnablesFetch(OwnableDefinition.Category.Gear, delegate(ShsWebResponse response, OwnableDefinition.Category category)
		{
			OnGearFetchResponse(response, category, completionCallback);
		}, base.UserId);
	}

	protected void OnGearFetchResponse(ShsWebResponse response, OwnableDefinition.Category category, OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		if (response.Status != 200)
		{
			if (fetchTransaction != null)
			{
				fetchTransaction.FailStep("gear", "Ownables request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			else
			{
				CspUtils.DebugLog("Gear request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			if (completionCallback != null)
			{
				completionCallback(false);
			}
			return;
		}
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		gearOwnableCollection = new GearOwnableCollection(dataWarehouse);
		if (fetchTransaction != null)
		{
			fetchTransaction.CompleteStep("gear");
		}
		if (completionCallback != null)
		{
			completionCallback(true);
		}
	}

	private void FireMysteryBoxCompleteMessage(bool Success)
	{
		AppShell.Instance.EventMgr.Fire(this, new MysteryBoxFetchCompleteMessage(Success));
	}

	public override void StartMysteryBoxFetch()
	{
		StartMysteryBoxFetch(FireMysteryBoxCompleteMessage);
	}

	public override void StartMysteryBoxFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		OwnablesManager.StartOwnablesFetch(OwnableDefinition.Category.MysteryBox, delegate(ShsWebResponse response, OwnableDefinition.Category category)
		{
			OnMysteryBoxFetchResponse(response, category, completionCallback);
		}, base.UserId);
	}

	protected void OnMysteryBoxFetchResponse(ShsWebResponse response, OwnableDefinition.Category category, OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		if (response.Status != 200)
		{
			if (fetchTransaction != null)
			{
				fetchTransaction.FailStep("mysterybox", "Ownables request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			else
			{
				CspUtils.DebugLog("Mysterybox request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			if (completionCallback != null)
			{
				completionCallback(false);
			}
			return;
		}
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		mysteryBoxesCollection = new MysteryBoxCollection(dataWarehouse);
		if (fetchTransaction != null)
		{
			fetchTransaction.CompleteStep("mysterybox");
		}
		if (completionCallback != null)
		{
			completionCallback(true);
		}
	}

	public override void StartPotionFetch()
	{
		StartPotionFetch(FirePotionCompleteMessage);
	}

	public override void StartPotionFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		OwnablesManager.StartOwnablesFetch(OwnableDefinition.Category.Potion, delegate(ShsWebResponse response, OwnableDefinition.Category category)
		{
			OnPotionsFetchResponse(response, category, completionCallback);
		}, base.UserId);
	}

	protected void OnPotionsFetchResponse(ShsWebResponse response, OwnableDefinition.Category category, OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		if (response.Status != 200)
		{
			if (fetchTransaction != null)
			{
				fetchTransaction.FailStep("potions", "Ownables request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			else
			{
				CspUtils.DebugLog("Potion request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			if (completionCallback != null)
			{
				completionCallback(false);
			}
			return;
		}
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		expendablesCollection = new ExpendableCollection(dataWarehouse);
		if (fetchTransaction != null)
		{
			fetchTransaction.CompleteStep("potions");
		}
		if (completionCallback != null)
		{
			completionCallback(true);
		}
	}

	public override void StartMissionsFetch()
	{
		StartMissionsFetch(FireMissionCompleteMessage);
	}

	private void FireMissionCompleteMessage(bool Success)
	{
		AppShell.Instance.EventMgr.Fire(this, new MissionFetchCompleteMessage(Success));
	}

	public void StartMissionsFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		updateCount++;
		OwnablesManager.StartOwnablesFetch(OwnableDefinition.Category.Mission, delegate(ShsWebResponse response, OwnableDefinition.Category category)
		{
			updateCount--;
			OnMissionsFetchResponse(response, category, completionCallback);
		}, base.UserId);
	}

	protected void OnMissionsFetchResponse(ShsWebResponse response, OwnableDefinition.Category category, OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		if (response.Status != 200)
		{
			if (fetchTransaction != null)
			{
				fetchTransaction.FailStep("missions", "Ownables request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			else
			{
				CspUtils.DebugLog("Missions request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			if (completionCallback != null)
			{
				completionCallback(false);
			}
			return;
		}
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		availableMissions = new AvailableMissionCollection(dataWarehouse);
		if (fetchTransaction != null)
		{
			fetchTransaction.CompleteStep("missions");
		}
		if (completionCallback != null)
		{
			completionCallback(true);
		}
	}

	public override void StartHQRoomsFetch()
	{
		StartHQRoomsFetch(null);
	}

	public void StartHQRoomsFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		updateCount++;
		OwnablesManager.StartOwnablesFetch(OwnableDefinition.Category.HQRoom, delegate(ShsWebResponse response, OwnableDefinition.Category category)
		{
			updateCount--;
			OnHQRoomsFetchResponse(response, category, completionCallback);
		}, base.UserId);
	}

	protected void OnHQRoomsFetchResponse(ShsWebResponse response, OwnableDefinition.Category category, OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		if (response.Status != 200)
		{
			if (fetchTransaction != null)
			{
				fetchTransaction.FailStep("hqrooms", "Ownables request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			else
			{
				CspUtils.DebugLog("HQRooms request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			if (completionCallback != null)
			{
				completionCallback(false);
			}
			return;
		}
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		availableHQRooms = new AvailableHQRoomsCollection(dataWarehouse);
		if (fetchTransaction != null)
		{
			fetchTransaction.CompleteStep("hqrooms");
		}
		if (completionCallback != null)
		{
			completionCallback(true);
		}
	}

	public override void StartQuestsFetch()
	{
		StartQuestsFetch(FireCardQuestFetchCompleteMessage);
	}

	private void FireCardQuestFetchCompleteMessage(bool Success)
	{
		AppShell.Instance.EventMgr.Fire(this, new CardQuestFetchCompleteMessage(Success));
	}

	public void StartQuestsFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		updateCount++;
		OwnablesManager.StartOwnablesFetch(OwnableDefinition.Category.Quest, delegate(ShsWebResponse response, OwnableDefinition.Category category)
		{
			updateCount--;
			OnQuestsFetchResponse(response, category, completionCallback);
		}, base.UserId);
	}

	protected void OnQuestsFetchResponse(ShsWebResponse response, OwnableDefinition.Category category, OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		if (response.Status != 200)
		{
			if (fetchTransaction != null)
			{
				fetchTransaction.FailStep("quests", "Ownables request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			else
			{
				CspUtils.DebugLog("Quests request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			if (completionCallback != null)
			{
				completionCallback(false);
			}
			return;
		}
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		availableQuests = new AvailableQuestCollection(dataWarehouse);
		if (fetchTransaction != null)
		{
			fetchTransaction.CompleteStep("quests");
		}
		if (completionCallback != null)
		{
			completionCallback(true);
		}
	}

	private void FirePotionCompleteMessage(bool Success)
	{
		AppShell.Instance.EventMgr.Fire(this, new PotionFetchCompleteMessage(Success));
	}

	public override void StartCardCollectionFetch()
	{
		CardCollection.Fetch(userId);
	}

	public override void StartBoosterPacksFetch()
	{
		StartBoosterPacksFetch(null);
	}

	public override void StartBoosterPacksFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		updateCount++;
		OwnablesManager.StartOwnablesFetch(OwnableDefinition.Category.BoosterPack, delegate(ShsWebResponse response, OwnableDefinition.Category category)
		{
			updateCount--;
			OnBoosterPacksFetchResponse(response, category, completionCallback);
		}, base.UserId);
	}

	protected void OnBoosterPacksFetchResponse(ShsWebResponse response, OwnableDefinition.Category category, OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		if (response.Status != 200)
		{
			if (fetchTransaction != null)
			{
				fetchTransaction.FailStep("BoosterPacks", "Ownables request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			else
			{
				CspUtils.DebugLog("BoosterPacks request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			if (completionCallback != null)
			{
				completionCallback(false);
			}
			return;
		}
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		availableBoosterPacks = new AvailableBoosterPacksCollection(dataWarehouse);
		if (fetchTransaction != null)
		{
			fetchTransaction.CompleteStep("boosters");
		}
		if (completionCallback != null)
		{
			completionCallback(true);
		}
	}

	public override void StartPetsFetch()
	{
		StartPetsFetch(null);
	}

	public override void StartPetsFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		updateCount++;
		OwnablesManager.StartOwnablesFetch(OwnableDefinition.Category.Sidekick, delegate(ShsWebResponse response, OwnableDefinition.Category category)
		{
			updateCount--;
			OnPetsFetchResponse(response, category, completionCallback);
		}, base.UserId);
	}

	protected void OnPetsFetchResponse(ShsWebResponse response, OwnableDefinition.Category category, OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		if (response.Status != 200)
		{
			if (fetchTransaction != null)
			{
				fetchTransaction.FailStep("pets", "Ownables request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			else
			{
				CspUtils.DebugLog("Pets request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			if (completionCallback != null)
			{
				completionCallback(false);
			}
			return;
		}
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		petCollection = new PetCollection(dataWarehouse);
		if (fetchTransaction != null)
		{
			fetchTransaction.CompleteStep("pets");
		}
		if (completionCallback != null)
		{
			completionCallback(true);
		}
		AppShell.Instance.EventMgr.Fire(this, new PetCollectionRefreshed());
	}

	public override void StartTitlesFetch()
	{
		StartTitlesFetch(null);
	}

	public override void StartTitlesFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		updateCount++;
		OwnablesManager.StartOwnablesFetch(OwnableDefinition.Category.Title, delegate(ShsWebResponse response, OwnableDefinition.Category category)
		{
			updateCount--;
			OnTitlesFetchResponse(response, category, completionCallback);
		}, base.UserId);
	}

	protected void OnTitlesFetchResponse(ShsWebResponse response, OwnableDefinition.Category category, OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		if (response.Status != 200)
		{
			if (fetchTransaction != null)
			{
				fetchTransaction.FailStep("titles", "Ownables request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			else
			{
				CspUtils.DebugLog("titles request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			if (completionCallback != null)
			{
				completionCallback(false);
			}
			return;
		}
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		titleCollection = new TitleCollection(dataWarehouse);
		if (fetchTransaction != null)
		{
			fetchTransaction.CompleteStep("titles");
		}
		if (completionCallback != null)
		{
			completionCallback(true);
		}
	}

	public override void StartMedallionsFetch()
	{
		StartMedallionsFetch(null);
	}

	public override void StartMedallionsFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		updateCount++;
		OwnablesManager.StartOwnablesFetch(OwnableDefinition.Category.Medallion, delegate(ShsWebResponse response, OwnableDefinition.Category category)
		{
			updateCount--;
			OnMedallionsFetchResponse(response, category, completionCallback);
		}, base.UserId);
	}

	protected void OnMedallionsFetchResponse(ShsWebResponse response, OwnableDefinition.Category category, OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		if (response.Status != 200)
		{
			if (fetchTransaction != null)
			{
				fetchTransaction.FailStep("medallions", "Ownables request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			else
			{
				CspUtils.DebugLog("Medallions request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			if (completionCallback != null)
			{
				completionCallback(false);
			}
			return;
		}
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		medallionCollection = new MedallionCollection(dataWarehouse);
		if (fetchTransaction != null)
		{
			fetchTransaction.CompleteStep("medallions");
		}
		if (completionCallback != null)
		{
			completionCallback(true);
		}
	}

	public override void StartCraftFetch()
	{
		StartCraftFetch(null);
	}

	public override void StartCraftFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		updateCount++;
		OwnablesManager.StartOwnablesFetch(OwnableDefinition.Category.Craft, delegate(ShsWebResponse response, OwnableDefinition.Category category)
		{
			updateCount--;
			OnCraftFetchResponse(response, category, completionCallback);
		}, base.UserId);
	}

	protected void OnCraftFetchResponse(ShsWebResponse response, OwnableDefinition.Category category, OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		if (response.Status != 200)
		{
			if (fetchTransaction != null)
			{
				fetchTransaction.FailStep("craft", "Ownables request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			else
			{
				CspUtils.DebugLog("Medallions request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			if (completionCallback != null)
			{
				completionCallback(false);
			}
			return;
		}
		int num = -1;
		if (craftCollection != null)
		{
			num = craftCollection.Keys.Count;
		}
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		craftCollection = new CraftCollection(dataWarehouse);
		int count = craftCollection.Keys.Count;
		if (num == 0 && count > 0)
		{
			SHSSocialMainWindow sHSSocialMainWindow = GUIManager.Instance["SHSMainWindow/SHSSocialMainWindow"] as SHSSocialMainWindow;
			if (sHSSocialMainWindow != null)
			{
				sHSSocialMainWindow.OnFirstCraftCollected();
			}
		}
		AppShell.Instance.EventMgr.Fire(this, new CraftCollectionUpdateMessage());
		if (fetchTransaction != null)
		{
			fetchTransaction.CompleteStep("craft");
		}
		if (completionCallback != null)
		{
			completionCallback(true);
		}
	}

	public override void StartBundlesFetch()
	{
		StartBundlesFetch(null);
	}

	public override void StartBundlesFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		updateCount++;
		OwnablesManager.StartOwnablesFetch(OwnableDefinition.Category.Bundle, delegate(ShsWebResponse response, OwnableDefinition.Category category)
		{
			updateCount--;
			OnBundlesFetchResponse(response, category, completionCallback);
		}, base.UserId);
	}

	protected void OnBundlesFetchResponse(ShsWebResponse response, OwnableDefinition.Category category, OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		if (response.Status != 200)
		{
			if (fetchTransaction != null)
			{
				fetchTransaction.FailStep("bundles", "Ownables request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			else
			{
				CspUtils.DebugLog("Bundles request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			if (completionCallback != null)
			{
				completionCallback(false);
			}
			return;
		}
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		bundleCollection = new BundleCollection(dataWarehouse);
		if (fetchTransaction != null)
		{
			fetchTransaction.CompleteStep("bundles");
		}
		if (completionCallback != null)
		{
			completionCallback(true);
		}
	}

	public override void StartSidekickUpgradesFetch()
	{
		StartSidekickUpgradesFetch(null);
	}

	public override void StartSidekickUpgradesFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		updateCount++;
		OwnablesManager.StartOwnablesFetch(OwnableDefinition.Category.SidekickUpgrade, delegate(ShsWebResponse response, OwnableDefinition.Category category)
		{
			updateCount--;
			OnSidekickUpgradesFetchResponse(response, category, completionCallback);
		}, base.UserId);
	}

	protected void OnSidekickUpgradesFetchResponse(ShsWebResponse response, OwnableDefinition.Category category, OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		if (response.Status != 200)
		{
			if (fetchTransaction != null)
			{
				fetchTransaction.FailStep("sidekickUpgrades", "Ownables request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			else
			{
				CspUtils.DebugLog("SidekickUpgrades request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			if (completionCallback != null)
			{
				completionCallback(false);
			}
			return;
		}
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		sidekickUpgradeCollection = new SidekickUpgradeCollection(dataWarehouse);
		if (fetchTransaction != null)
		{
			fetchTransaction.CompleteStep("sidekickUpgrades");
		}
		if (completionCallback != null)
		{
			completionCallback(true);
		}
	}

	public override void StartBadgesFetch()
	{
		StartBadgesFetch(null);
	}

	public override void StartBadgesFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		updateCount++;
		OwnablesManager.StartOwnablesFetch(OwnableDefinition.Category.Badge, delegate(ShsWebResponse response, OwnableDefinition.Category category)
		{
			updateCount--;
			OnBadgesFetchResponse(response, category, completionCallback);
		}, base.UserId);
	}

	protected void OnBadgesFetchResponse(ShsWebResponse response, OwnableDefinition.Category category, OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		if (response.Status != 200)
		{
			if (fetchTransaction != null)
			{
				fetchTransaction.FailStep("badges", "Ownables request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			else
			{
				CspUtils.DebugLog("Badges request failed for <" + response.RequestUri + "> with status " + response.Status);
			}
			if (completionCallback != null)
			{
				completionCallback(false);
			}
			return;
		}
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		BadgeCollection = new BadgeCollection(dataWarehouse);
		if (fetchTransaction != null)
		{
			fetchTransaction.CompleteStep("badges");
		}
		if (completionCallback != null)
		{
			completionCallback(true);
		}
	}

	public override void StartHeroFetch()
	{
		updateCount++;
		AppShell.Instance.WebService.StartRequest("resources$users/" + userId + "/", OnHeroFetchResponse);
	}

	protected void OnHeroFetchResponse(ShsWebResponse response)
	{
		updateCount--;
		if (response.Status == 200)
		{
			DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
			dataWarehouse.Parse();
			DataWarehouse data = dataWarehouse.GetData("//heroes");
			availableCostumes = new HeroCollection(data);
			availableCostumes.ResetPlacedValues();
			AppShell.Instance.EventMgr.Fire(this, new HeroFetchCompleteMessage(true));
			AppShell.Instance.EventMgr.Fire(null, new CharacterSelectedMessage(AppShell.Instance.Profile.LastSelectedCostume));
		}
		else
		{
			CspUtils.DebugLog("Hero Profile Retrieval Error <" + response.Status + ">: " + response.Body);
			AppShell.Instance.EventMgr.Fire(this, new HeroFetchCompleteMessage(false));
		}
	}

	public override void StartFriendFetch()
	{
		AppShell.Instance.WebService.StartRequest("resources$users/" + base.UserId + "/friends.py", OnFriendsFetchResponse);
	}

	public override void AddStars(int count)
	{
		stars = Mathf.Max(stars + count, 0);
		int num = (int)Mathf.Floor((float)stars / 10f);
		if (num > 0)
		{
			stars %= 10;
			AppShell.Instance.EventReporter.ReportAchievementEvent(GameController.GetController().LocalPlayer.name, "convert_star_fractal", "star_fractal", 1, string.Empty);
		}
	}

	public override void UnplaceAllItemsAndHeros()
	{
		foreach (Item value in availableItems.Values)
		{
			value.Placed = 0;
		}
		foreach (HeroPersisted value2 in availableCostumes.Values)
		{
			value2.Placed = false;
		}
	}
}
