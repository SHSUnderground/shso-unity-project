using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class UserProfile
{
	public enum ProfileTypeEnum
	{
		LocalPlayer,
		RemotePlayer
	}

	public delegate void OnProfileLoaded(UserProfile profile);

	protected string playerName = "Unknown";

	protected long userId = -1L;

	protected string creationDate = string.Empty;

	protected int squadLevel;

	protected HeroCollection availableCostumes;

	protected string selectedCostume = string.Empty;

	protected string lastSelectedCostume = string.Empty;

	protected int lastSelectedPower = 1;

	protected int lastDeckID;

	protected bool firstCardGame = true;

	protected bool demoHack;

	protected AvailableMissionCollection availableMissions;

	protected AvailableQuestCollection availableQuests;

	protected int gold;

	protected int silver;

	protected int tickets;

	protected int stars;

	protected int shards;

	protected ItemCollection availableItems;

	protected FriendCollection availableFriends;

	protected AvailableHQRoomsCollection availableHQRooms;

	protected bool offline;

	protected BitArray prizeWheelState;

	protected TransactionMonitor fetchTransaction;

	protected int updateCount;

	protected SHSCounterBank counterBank;

	protected int loginsToday;

	protected int lastChallenge;

	protected int currentChallenge;

	protected Dictionary<string, bool> collectedSessionCollectibles = new Dictionary<string, bool>();

	protected Dictionary<string, object> sessionVars = new Dictionary<string, object>();

	public bool dailyRewardDisplayed;

	public int titleID = -1;

	public int medallionID = -1;

	public int sidekickID = -1;

	public int sidekickTier = -1;

	public int achievementPoints;

	public string trackerData = string.Empty;

	public double xpMultiplier = 1.0;

	public Dictionary<AchievementManager.DestinyTracks, int> currentDestinyIDs = new Dictionary<AchievementManager.DestinyTracks, int>();

	public OnProfileLoaded ProfileLoadedHandler;

	public List<SpecialAbility> brawlerAbilities = new List<SpecialAbility>();

	public List<SpecialAbility> socialAbilities = new List<SpecialAbility>();

	public float nextShardTime;

	protected AvailableBoosterPacksCollection availableBoosterPacks;

	protected ExpendableCollection expendablesCollection;

	protected MysteryBoxCollection mysteryBoxesCollection;

	protected GearOwnableCollection gearOwnableCollection;

	protected BadgeCollection BadgeCollection;

	protected PetCollection petCollection;

	protected MedallionCollection medallionCollection;

	protected TitleCollection titleCollection;

	protected CraftCollection craftCollection;

	protected BundleCollection bundleCollection;

	protected SidekickUpgradeCollection sidekickUpgradeCollection;

	protected Dictionary<int, Gear> gearCollection = new Dictionary<int, Gear>();

	private static string LAST_RESORT_CHARACTER = "iron_man";

	protected ProfileTypeEnum profileType;

	[CompilerGenerated]
	private bool _003CInitialized_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CConsecutiveDaysPlayed_003Ek__BackingField;

	[CompilerGenerated]
	private TimeSpan _003CTimeUntilMidnight_003Ek__BackingField;

	public bool UpdateInProgress
	{
		get
		{
			return updateCount > 0;
		}
	}

	public virtual SHSCounterBank CounterBank
	{
		get
		{
			return counterBank;
		}
		set
		{
			counterBank = value;
		}
	}

	public string PlayerName
	{
		get
		{
			return playerName;
		}
	}

	public int SquadLevel
	{
		get
		{
			return squadLevel;
		}
	}

	public long UserId
	{
		get
		{
			return userId;
		}
	}

	public string CreationDate
	{
		get
		{
			return creationDate;
		}
	}

	public HeroCollection AvailableCostumes
	{
		get
		{
			return availableCostumes;
		}
	}

	public List<HeroPersisted> AvailableShieldCostumes
	{
		get
		{
			List<HeroPersisted> list = new List<HeroPersisted>();
			foreach (KeyValuePair<string, HeroPersisted> availableCostume in AvailableCostumes)
			{
				if (availableCostume.Value.ShieldAgentOnly)
				{
					list.Add(availableCostume.Value);
				}
			}
			return list;
		}
	}

	public List<HeroPersisted> AvailableRecruitCostumes
	{
		get
		{
			List<HeroPersisted> list = new List<HeroPersisted>();
			foreach (KeyValuePair<string, HeroPersisted> availableCostume in AvailableCostumes)
			{
				if (!availableCostume.Value.ShieldAgentOnly)
				{
					list.Add(availableCostume.Value);
				}
			}
			return list;
		}
	}

	public ItemCollection AvailableItems
	{
		get
		{
			return availableItems;
		}
	}

	public AvailableMissionCollection AvailableMissions
	{
		get
		{
			return availableMissions;
		}
	}

	public AvailableQuestCollection AvailableQuests
	{
		get
		{
			return availableQuests;
		}
	}

	public FriendCollection AvailableFriends
	{
		get
		{
			return availableFriends;
		}
	}

	public AvailableHQRoomsCollection AvailableHQRooms
	{
		get
		{
			return availableHQRooms;
		}
	}

	public AvailableBoosterPacksCollection AvailableBoosterPacks
	{
		get
		{
			return availableBoosterPacks;
		}
	}

	public ExpendableCollection ExpendablesCollection
	{
		get
		{
			return expendablesCollection;
		}
	}

	public MysteryBoxCollection MysteryBoxesCollection
	{
		get
		{
			return mysteryBoxesCollection;
		}
	}

	public GearOwnableCollection GearOwnableCollection
	{
		get
		{
			return gearOwnableCollection;
		}
	}

	public BadgeCollection Badges
	{
		get
		{
			return BadgeCollection;
		}
	}

	public PetCollection Pets
	{
		get
		{
			return petCollection;
		}
	}

	public MedallionCollection Medallions
	{
		get
		{
			return medallionCollection;
		}
	}

	public TitleCollection Titles
	{
		get
		{
			return titleCollection;
		}
	}

	public CraftCollection Crafts
	{
		get
		{
			return craftCollection;
		}
	}

	public BundleCollection Bundles
	{
		get
		{
			return bundleCollection;
		}
	}

	public SidekickUpgradeCollection SidekickUpgrades
	{
		get
		{
			return sidekickUpgradeCollection;
		}
	}

	public virtual string SelectedCostume
	{
		get
		{
			return selectedCostume;
		}
		set
		{
			selectedCostume = value;
		}
	}

	public string LastSelectedCostume
	{
		get
		{
			HeroPersisted value;
			if (availableCostumes.TryGetValue(lastSelectedCostume, out value))
			{
				if (!value.ShieldAgentOnly || Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldHeroesAllow))
				{
					return lastSelectedCostume;
				}
				List<HeroPersisted> availableRecruitCostumes = AvailableRecruitCostumes;
				if (availableRecruitCostumes.Count < 0)
				{
					CspUtils.DebugLog("No valid recruit heroes to select.");
					lastSelectedCostume = LAST_RESORT_CHARACTER;
				}
				else
				{
					int index = UnityEngine.Random.Range(0, availableRecruitCostumes.Count);
					CspUtils.DebugLog("No valid last costume. choosing " + availableRecruitCostumes[index].Name);
					lastSelectedCostume = availableRecruitCostumes[index].Name;
				}
			}
			else
			{
				lastSelectedCostume = LAST_RESORT_CHARACTER;
			}
			return lastSelectedCostume;
		}
		set
		{
			lastSelectedCostume = value;
		}
	}

	public int LastSelectedPower
	{
		get
		{
			return lastSelectedPower;
		}
		set
		{
			lastSelectedPower = value;
		}
	}

	public int LastDeckID
	{
		get
		{
			return lastDeckID;
		}
		set
		{
			lastDeckID = value;
		}
	}

	public bool FirstCardGame
	{
		get
		{
			return firstCardGame;
		}
		set
		{
			firstCardGame = value;
		}
	}

	public virtual bool IsShieldPlayCapable
	{
		get
		{
			return false;
		}
	}

	public bool DemoHack
	{
		get
		{
			return demoHack;
		}
		set
		{
			demoHack = value;
		}
	}

	public int Gold
	{
		get
		{
			return gold;
		}
		set
		{
			gold = value;
		}
	}

	public int Silver
	{
		get
		{
			return silver;
		}
		set
		{
			silver = value;
		}
	}

	public int Tickets
	{
		get
		{
			return tickets;
		}
		set
		{
			tickets = value;
		}
	}

	public int Shards
	{
		get
		{
			return shards;
		}
		set
		{
			shards = value;
		}
	}

	public BitArray PrizeWheelState
	{
		get
		{
			return prizeWheelState;
		}
		set
		{
			prizeWheelState = value;
		}
	}

	public bool Offline
	{
		get
		{
			return offline;
		}
		set
		{
			offline = value;
		}
	}

	public int Stars
	{
		get
		{
			return stars;
		}
	}

	public ProfileTypeEnum ProfileType
	{
		get
		{
			return profileType;
		}
	}

	public bool Initialized
	{
		[CompilerGenerated]
		get
		{
			return _003CInitialized_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CInitialized_003Ek__BackingField = value;
		}
	}

	public bool ReceivedDailyReward
	{
		get
		{
			return loginsToday == 1;
		}
	}

	public int ConsecutiveDaysPlayed
	{
		[CompilerGenerated]
		get
		{
			return _003CConsecutiveDaysPlayed_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CConsecutiveDaysPlayed_003Ek__BackingField = value;
		}
	}

	public TimeSpan TimeUntilMidnight
	{
		[CompilerGenerated]
		get
		{
			return _003CTimeUntilMidnight_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CTimeUntilMidnight_003Ek__BackingField = value;
		}
	}

	public int LastChallenge
	{
		get
		{
			return lastChallenge;
		}
	}

	public int CurrentChallenge
	{
		get
		{
			return currentChallenge;
		}
	}

	public string Title
	{
		get
		{
			if (titleID == -1)
			{
				return "#TITLE_CHOOSE_TITLE";
			}
			OwnableDefinition def = OwnableDefinition.getDef(titleID);
			if (def == null)
			{
				return "#TITLE_CHOOSE_TITLE";
			}
			return def.name;
		}
	}

	public string Medallion
	{
		get
		{
			if (medallionID == -1)
			{
				return string.Empty;
			}
			OwnableDefinition def = OwnableDefinition.getDef(medallionID);
			if (def == null)
			{
				return string.Empty;
			}
			return def.metadata;
		}
	}

	public virtual int SidekickID
	{
		get
		{
			return sidekickID;
		}
	}

	public virtual int SidekickTier
	{
		get
		{
			return sidekickTier;
		}
	}

	public Dictionary<string, bool> CollectedSessionCollectibles
	{
		get
		{
			return collectedSessionCollectibles;
		}
		set
		{
			collectedSessionCollectibles = value;
		}
	}

	public Dictionary<string, object> SessionVars
	{
		get
		{
			return sessionVars;
		}
		set
		{
			sessionVars = value;
		}
	}

	public static UserProfile OfflineUserProfile
	{
		get
		{
			UserProfile userProfile = new LocalPlayerProfile();
			userProfile.userId = -1L;
			userProfile.playerName = "Offline User";
			userProfile.lastSelectedCostume = null;
			userProfile.lastDeckID = 0;
			userProfile.firstCardGame = true;
			DataWarehouse dataWarehouse = new DataWarehouse("<heroes>\r\n                    <hero>\r\n                        <name>wolverine</name>\r\n                        <xp>20</xp>\r\n                        <level>1</level>\r\n                        <xp_to_next_level>100</xp_to_next_level>\r\n                    </hero>\r\n                    <hero>\r\n                        <name>iron_man</name>\r\n                        <xp>700</xp>\r\n                        <level>2</level>\r\n                        <xp_to_next_level>100</xp_to_next_level>\r\n                    </hero>\r\n                    <hero>\r\n                        <name>storm</name>\r\n                        <xp>0</xp>\r\n                        <level>1500</level>\r\n                        <xp_to_next_level>100</xp_to_next_level>\r\n                    </hero>\r\n                    <hero>\r\n                        <name>hulk</name>\r\n                        <xp>0</xp>\r\n                        <level>4</level>\r\n                        <xp_to_next_level>3200</xp_to_next_level>\r\n                    </hero>\r\n                </heroes>");
			dataWarehouse.Parse();
			userProfile.availableCostumes = new HeroCollection(dataWarehouse);
			DataWarehouse dataWarehouse2 = new DataWarehouse(string.Empty);
			dataWarehouse2.Parse();
			userProfile.availableMissions = new AvailableMissionCollection(dataWarehouse2);
			userProfile.offline = true;
			return userProfile;
		}
	}

	protected UserProfile()
	{
	}

	protected UserProfile(string xmlData, OnProfileLoaded loadCallback)
	{
		ProfileLoadedHandler = loadCallback;
		InitializeFromData(xmlData);
	}

	public virtual void useSpecialAbility(int specialAbilityID)
	{
	}

	public virtual void refreshSocialAbilities()
	{
	}

	public abstract void InitializeFromData(string xmlData);

	public abstract void InitializeFromData(RemotePlayerProfileJsonData jsonData);

	public abstract void InitializeFromData(RemotePlayerProfileJsonData jsonData, OnProfileLoaded loadCallback);

	public abstract void PersistExtendedData();

	public abstract void CollectShard();

	public abstract void StartCurrencyFetch();

	protected abstract void InitializeCurrencyFromData(DataWarehouse currencyData);

	public abstract void StartBulkFetch();

	public abstract void StartInventoryFetch();

	public abstract void StartInventoryFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback);

	public abstract void StartPotionFetch();

	public abstract void StartPotionFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback);

	public abstract void StartMysteryBoxFetch();

	public abstract void StartMysteryBoxFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback);

	public abstract void StartGearFetch();

	public abstract void StartGearFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback);

	public abstract void StartMissionsFetch();

	public abstract void StartHQRoomsFetch();

	public abstract void StartQuestsFetch();

	public abstract void StartCardCollectionFetch();

	public abstract void StartBoosterPacksFetch();

	public abstract void StartBoosterPacksFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback);

	public abstract void StartBadgesFetch();

	public abstract void StartBadgesFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback);

	public abstract void StartPetsFetch();

	public abstract void StartPetsFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback);

	public abstract void StartTitlesFetch();

	public abstract void StartTitlesFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback);

	public abstract void StartMedallionsFetch();

	public abstract void StartMedallionsFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback);

	public abstract void StartCraftFetch();

	public abstract void StartCraftFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback);

	public abstract void StartBundlesFetch();

	public abstract void StartBundlesFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback);

	public abstract void StartSidekickUpgradesFetch();

	public abstract void StartSidekickUpgradesFetch(OwnablesManager.OwnablesFetchCompleteDelegate completionCallback);

	public abstract void StartHeroFetch();

	public abstract void FetchDataBasedOnCategory(OwnableDefinition.Category category);

	public abstract void StartFriendFetch();

	public abstract void AddStars(int count);

	public abstract void UnplaceAllItemsAndHeros();

	public static UserProfile CreateOfflineUserProfile(TextAsset userProfile, TextAsset inventory)
	{
		UserProfile userProfile2 = new LocalPlayerProfile();
		DataWarehouse dataWarehouse = new DataWarehouse(userProfile.text);
		dataWarehouse.Parse();
		userProfile2.creationDate = "Unknown";
		userProfile2.userId = -1L;
		userProfile2.playerName = "Offline User";
		userProfile2.lastSelectedCostume = dataWarehouse.TryGetString("//extended_data/LastCostume", null);
		userProfile2.lastDeckID = dataWarehouse.TryGetInt("//extended_data/LastDeckID", 0);
		userProfile2.firstCardGame = dataWarehouse.TryGetBool("//extended_data/FirstCardGame", true);
		userProfile2.demoHack = dataWarehouse.TryGetBool("//extended_data/DemoHack", false);
		DataWarehouse data = dataWarehouse.GetData("//heroes");
		userProfile2.availableCostumes = new HeroCollection(data);
		userProfile2.InitializeCurrencyFromData(dataWarehouse.GetData("//currency"));
		userProfile2.prizeWheelState = new BitArray(BitConverter.GetBytes(dataWarehouse.GetInt("//prize_wheel/earned_stops")));
		userProfile2.availableItems = new ItemCollection();
		userProfile2.availableFriends = new FriendCollection();
		Singleton<Entitlements>.instance.Configure(dataWarehouse);
		userProfile2.offline = true;
		DataWarehouse dataWarehouse2 = new DataWarehouse(inventory.text);
		dataWarehouse2.Parse();
		DataWarehouse data2 = dataWarehouse2.GetData("//inventory");
		userProfile2.availableItems = new ItemCollection(data2);
		return userProfile2;
	}
}
