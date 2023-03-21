using LitJson;
using System;

public class RemotePlayerProfile : UserProfile, IDisposable
{
	private bool isShieldPlayCapable;

	public override bool IsShieldPlayCapable
	{
		get
		{
			return isShieldPlayCapable;
		}
	}

	public RemotePlayerProfile(long playerId)
	{
		userId = playerId;
		profileType = ProfileTypeEnum.RemotePlayer;
	}

	public override void InitializeFromData(RemotePlayerProfileJsonData jsonData)
	{
		InitializeFromData(jsonData, null);
	}

	public override void InitializeFromData(RemotePlayerProfileJsonData jsonData, OnProfileLoaded loadedCallback)
	{
		playerName = jsonData.player_name;
		isShieldPlayCapable = (jsonData.shield_play_allow == 1);
		availableCostumes = new HeroCollection(jsonData.heroes);
		currentChallenge = jsonData.current_challenge;
		lastChallenge = jsonData.last_celebrated;
		medallionID = jsonData.medallion_id;
		titleID = jsonData.title_id;
		sidekickID = jsonData.sidekick_id;
		sidekickTier = jsonData.sidekick_tier;
		selectedCostume = jsonData.current_costume;
		achievementPoints = jsonData.achievement_points;
		CspUtils.DebugLog("RemotePlayerProfile loaded " + medallionID + " " + titleID + " " + sidekickID + " " + selectedCostume + " " + achievementPoints);
		if (loadedCallback != null)
		{
			loadedCallback(this);
		}
	}

	public override void InitializeFromData(string xmlData)
	{
		throw new NotSupportedException();
	}

	public override void PersistExtendedData()
	{
		throw new NotSupportedException();
	}

	public override void CollectShard()
	{
		throw new NotSupportedException();
	}

	public override void StartCurrencyFetch()
	{
		throw new NotSupportedException();
	}

	protected override void InitializeCurrencyFromData(DataWarehouse currencyData)
	{
		throw new NotSupportedException();
	}

	public override void StartBulkFetch()
	{
		throw new NotSupportedException();
	}

	public override void StartInventoryFetch()
	{
		throw new NotSupportedException();
	}

	public override void StartInventoryFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		throw new NotSupportedException();
	}

	public override void StartPotionFetch()
	{
		throw new NotSupportedException();
	}

	public override void StartPotionFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		throw new NotSupportedException();
	}

	public override void StartMysteryBoxFetch()
	{
		throw new NotSupportedException();
	}

	public override void StartMysteryBoxFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		throw new NotSupportedException();
	}

	public override void StartGearFetch()
	{
		throw new NotSupportedException();
	}

	public override void StartGearFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		throw new NotSupportedException();
	}

	public override void StartMissionsFetch()
	{
		throw new NotSupportedException();
	}

	public override void StartHQRoomsFetch()
	{
		throw new NotSupportedException();
	}

	public override void StartQuestsFetch()
	{
		throw new NotSupportedException();
	}

	public override void StartCardCollectionFetch()
	{
		throw new NotSupportedException();
	}

	public override void StartBoosterPacksFetch()
	{
		throw new NotSupportedException();
	}

	public override void StartBoosterPacksFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		throw new NotSupportedException();
	}

	public override void StartTitlesFetch()
	{
		throw new NotSupportedException();
	}

	public override void StartTitlesFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		throw new NotSupportedException();
	}

	public override void StartMedallionsFetch()
	{
		throw new NotSupportedException();
	}

	public override void StartMedallionsFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		throw new NotSupportedException();
	}

	public override void StartCraftFetch()
	{
		throw new NotSupportedException();
	}

	public override void StartCraftFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		throw new NotSupportedException();
	}

	public override void StartBundlesFetch()
	{
		throw new NotSupportedException();
	}

	public override void StartBundlesFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		throw new NotSupportedException();
	}

	public override void StartSidekickUpgradesFetch()
	{
		throw new NotSupportedException();
	}

	public override void StartSidekickUpgradesFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		throw new NotSupportedException();
	}

	public override void StartBadgesFetch()
	{
		throw new NotSupportedException();
	}

	public override void StartBadgesFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		throw new NotSupportedException();
	}

	public override void StartPetsFetch()
	{
		throw new NotSupportedException();
	}

	public override void StartPetsFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback)
	{
		throw new NotSupportedException();
	}

	public override void StartHeroFetch()
	{
		throw new NotSupportedException();
	}

	public override void StartFriendFetch()
	{
		throw new NotSupportedException();
	}

	public override void FetchDataBasedOnCategory(OwnableDefinition.Category category)
	{
		throw new NotSupportedException();
	}

	public override void AddStars(int count)
	{
		throw new NotSupportedException();
	}

	public override void UnplaceAllItemsAndHeros()
	{
		throw new NotSupportedException();
	}

	public override string ToString()
	{
		return string.Format("{0}'s squad with {1} heroes and X counters.", base.PlayerName, base.AvailableCostumes.Count);
	}

	public static void FetchProfile(long playerId, OnProfileLoaded onLoaded)
	{
		AppShell.Instance.WebService.StartRequest("resources$users/squad.py", delegate(ShsWebResponse response)
		{
			OnFetchProfileResponse(response, playerId, onLoaded);
		});
	}

	private static void OnFetchProfileResponse(ShsWebResponse response, long playerId, OnProfileLoaded onLoaded)
	{
		if (response.Status == 200)
		{
			RemotePlayerProfileJsonData jsonData = JsonMapper.ToObject<RemotePlayerProfileJsonData>(response.Body);
			RemotePlayerProfile remotePlayerProfile = new RemotePlayerProfile(playerId);
			remotePlayerProfile.InitializeFromData(jsonData, onLoaded);
		}
		else
		{
			CspUtils.DebugLog("SERVER: Failed to get squad data for player: <" + response.Status + ">: " + response.Body);
		}
	}

	public void Dispose()
	{
		if (AppShell.Instance.CounterManager != null)
		{
			AppShell.Instance.CounterManager.RemoveCounterBank(userId);
		}
	}
}
