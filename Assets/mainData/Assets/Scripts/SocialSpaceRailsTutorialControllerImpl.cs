using System;
using System.Collections.Generic;
using UnityEngine;

public class SocialSpaceRailsTutorialControllerImpl : IGameController
{
	internal class SocialSpaceRailsTutorialControllerLoad : IDisposable, IShsState
	{
		protected const string SOCIAL_BUNDLE_DIRECTORY = "SocialSpace/";

		protected SocialSpaceRailsTutorialControllerImpl owner;

		protected TransactionMonitor startTransaction;

		protected TransactionMonitor cacheHeroesTransaction;

		protected List<GameObject> bundles;

		protected int frameDelay = 15;

		protected GameObject placeholderSpawner;

		public SocialSpaceRailsTutorialControllerLoad(SocialSpaceRailsTutorialControllerImpl owner)
		{
			this.owner = owner;
		}

		public void Enter(Type previousState)
		{
			if (owner.owner.isTestScene)
			{
				AppShell.Instance.TransitionHandler.CurrentTransactionContext.CompleteChildTransactionStep("LaunchTransition", "initialize");
				AppShell.Instance.TransitionHandler.CurrentTransactionContext.CompleteChildTransactionStep("LaunchLogin", "initialize");
				PreloadPlaceholder();
				owner.fsm.GotoState<SocialSpaceRailsTutorialControllerPlay>();
				return;
			}
			bundles = new List<GameObject>();
			AppShell.Instance.TransitionHandler.CurrentTransactionContext.CompleteChildTransactionStep("LaunchTransition", "initialize");
			startTransaction = AppShell.Instance.TransitionHandler.CurrentTransactionContext.Transaction;
			startTransaction.onComplete += OnStartComplete;
			startTransaction.timeout = 0f;
			startTransaction.AddStep("xml", TransactionMonitor.DumpTransactionStatus);
			startTransaction.AddStep("placeholderCache", TransactionMonitor.DumpTransactionStatus);
			if (startTransaction.HasStep("rails_init"))
			{
				startTransaction.CompleteStep("rails_init");
			}
			cacheHeroesTransaction = new TransactionMonitor(OnCacheHeroesComplete, 30f, null);
			cacheHeroesTransaction.Weight = 10f;
			startTransaction.AddChild(cacheHeroesTransaction);
			bool flag = false;
			if (AppShell.Instance.Profile != null)
			{
				foreach (KeyValuePair<string, HeroPersisted> availableCostume in AppShell.Instance.Profile.AvailableCostumes)
				{
					CacheHeroAssets(availableCostume.Key);
					if (availableCostume.Key.CompareTo("spider_man") == 0)
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				CacheHeroAssets("spider_man");
			}
			AppShell.Instance.DataManager.LoadGameData("spaces", OnZoneDataLoaded, new Zones(), startTransaction);
			startTransaction.AddStep("Audio/en_us_tutorial_audio", TransactionMonitor.DumpTransactionStatus);
			AppShell.Instance.BundleLoader.FetchAssetBundle("Audio/en_us_tutorial_audio", OnVoiceOverAudioLoaded);
			PreloadPlaceholder();
		}

		public void Update()
		{
			if (startTransaction != null)
			{
				return;
			}
			if (frameDelay == 9)
			{
				GameObject gameObject = new GameObject("KillyMcKillington");
				gameObject.transform.position = new Vector3(0f, -100f, 0f);
				BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
				boxCollider.size = new Vector3(1000f, 100f, 1000f);
				boxCollider.isTrigger = true;
				gameObject.AddComponent<SocialSpaceKillZone>();
				if (owner.owner.buildingCollisionPrefab != null)
				{
					EffectSequence component = Utils.GetComponent<EffectSequence>(owner.owner.buildingCollisionPrefab);
					if (component != null)
					{
						UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject));
						for (int i = 0; i < array.Length; i++)
						{
							GameObject gameObject2 = (GameObject)array[i];
							if ((gameObject2.layer & 0x1E) != 0)
							{
								EffectOnBuildingCollision effectOnBuildingCollision = Utils.AddComponent<EffectOnBuildingCollision>(gameObject2);
								effectOnBuildingCollision.effect = component;
							}
						}
					}
				}
			}
			else if (frameDelay == 7)
			{
				owner.owner.ControllerReady();
			}
			else if (frameDelay == 0)
			{
				owner.fsm.GotoState<SocialSpaceRailsTutorialControllerPlay>();
				return;
			}
			frameDelay--;
		}

		public void Leave(Type nextState)
		{
			startTransaction = null;
		}

		public void Dispose()
		{
			startTransaction = null;
			owner = null;
		}

		protected void PreloadPlaceholder()
		{
			placeholderSpawner = (UnityEngine.Object.Instantiate(Resources.Load("Spawners/PlaceholderSpawner")) as GameObject);
			CharacterSpawn component = Utils.GetComponent<CharacterSpawn>(placeholderSpawner);
			if (component != null)
			{
				UnityEngine.Object.Destroy(placeholderSpawner);
				AppShell.Instance.DataManager.LoadGameData("Characters/" + component.CharacterName, OnPlaceholderPreloaded, null);
			}
			else if (startTransaction != null)
			{
				startTransaction.FailStep("placeholderCache", "Could not preload placeholder character");
			}
		}

		protected void OnZoneDataLoaded(GameDataLoadResponse response, object extraData)
		{
			if (startTransaction == null)
			{
				return;
			}
			if (response.DataDefinition == null)
			{
				startTransaction.FailStep("xml", "Unable to load xml");
				return;
			}
			if (!owner.owner.isTestScene)
			{
				string a = owner.zoneName.ToLower();
				Zones zones = (Zones)response.DataDefinition;
				Zones.Layout[] layouts = zones.layouts;
				foreach (Zones.Layout layout in layouts)
				{
					if (!(a != layout.name.ToLower()))
					{
						owner.zoneTitle = layout.title;
						owner.zoneID = layout.gameworldID;
						string[] array = layout.bundles;
						foreach (string str in array)
						{
							string text = "SocialSpace/" + str;
							startTransaction.AddStep(text, TransactionMonitor.DumpTransactionStatus);
							AppShell.Instance.BundleLoader.FetchAssetBundle(text, OnAssetBundleLoaded, startTransaction, true);
						}
						break;
					}
				}
			}
			startTransaction.CompleteStep("xml");
		}

		protected void OnAssetBundleLoaded(AssetBundleLoadResponse response, object extraData)
		{
			CspUtils.DebugLog("Loaded.." + response.Path + " " + response.Error);
			if (startTransaction != null)
			{
				if (!string.IsNullOrEmpty(response.Error) || response.Bundle == null)
				{
					startTransaction.Fail("OnAssetBundleLoaded: " + response.Error);
					return;
				}
				bundles.Add(response.Bundle.mainAsset as GameObject);
				startTransaction.CompleteStep(response.Path);
			}
		}

		protected void OnVoiceOverAudioLoaded(AssetBundleLoadResponse response, object extraData)
		{
			CspUtils.DebugLog("Loaded.." + response.Path + " " + response.Error);
			if (startTransaction != null)
			{
				if (!string.IsNullOrEmpty(response.Error) || response.Bundle == null)
				{
					startTransaction.Fail("OnAssetBundleLoaded: " + response.Error);
				}
				else
				{
					startTransaction.CompleteStep(response.Path);
				}
			}
		}

		protected void CacheHeroAssets(string heroName)
		{
			cacheHeroesTransaction.AddStep(heroName + "_gamedata");
			cacheHeroesTransaction.AddStep(heroName + "_assets");
			cacheHeroesTransaction.AddStep(heroName + "_fx");
			Transform location = null;
			object extraData = new CharacterSpawnData(heroName, 1, location, CharacterSpawn.Type.LocalPlayer, null, OnCharacterPrespawned, false, null);
			AppShell.Instance.DataManager.LoadGameData("Characters/" + heroName, OnCharacterDataLoaded, extraData);
		}

		protected static void OnCharacterPrespawned(GameObject newCharacter, CharacterSpawnData spawnData)
		{
		}

		protected void OnCharacterDataLoaded(GameDataLoadResponse response, object extraData)
		{
			CharacterSpawnData characterSpawnData = extraData as CharacterSpawnData;
			if (characterSpawnData == null)
			{
				CspUtils.DebugLog("FATAL ERROR: Spawn data missing in callback from GameDataManager for unknown character <" + response.Path + ">.");
				return;
			}
			if (!string.IsNullOrEmpty(response.Error))
			{
				CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
				cacheHeroesTransaction.CompleteStep(characterSpawnData.ModelName + "_assets");
				cacheHeroesTransaction.CompleteStep(characterSpawnData.ModelName + "_fx");
				return;
			}
			cacheHeroesTransaction.CompleteStep(characterSpawnData.ModelName + "_gamedata");
			characterSpawnData.characterData = response.Data;
			if (characterSpawnData.characterData == null)
			{
				CspUtils.DebugLog("Character data missing in callback from GameDataManager for character <" + response.Path + ">.");
				cacheHeroesTransaction.CompleteStep(characterSpawnData.ModelName + "_assets");
				cacheHeroesTransaction.CompleteStep(characterSpawnData.ModelName + "_fx");
				return;
			}
			string @string = characterSpawnData.characterData.GetString("//asset_bundle");
			if (@string != null)
			{
				if (AppShell.Instance.BundleLoader.IsAssetBundleCachedOnDisk(@string))
				{
					OnCharacterAssetBundleCached(@string, characterSpawnData);
				}
				else
				{
					AppShell.Instance.BundleLoader.FetchAssetBundle(@string, OnCharacterAssetBundleLoaded, characterSpawnData);
				}
			}
			else
			{
				CspUtils.DebugLog("No asset bundle name found in character data <" + characterSpawnData.ModelName + ">.  Cannot spawn character.");
				cacheHeroesTransaction.CompleteStep(characterSpawnData.ModelName + "_assets");
				cacheHeroesTransaction.CompleteStep(characterSpawnData.ModelName + "_fx");
			}
		}

		protected void OnCharacterAssetBundleCached(string bundleName, CharacterSpawnData spawnData)
		{
			cacheHeroesTransaction.CompleteStep(spawnData.ModelName + "_assets");
			string @string = spawnData.characterData.GetString("//character_model/model_name");
			if (@string == null)
			{
				CspUtils.DebugLog("No model name in character data for <" + spawnData.ModelName + ">.  Cannot spawn character.");
				cacheHeroesTransaction.CompleteStep(spawnData.ModelName + "_fx");
			}
			else
			{
				AppShell.Instance.BundleLoader.LoadAsset(bundleName, @string, spawnData, OnCharacterPrefabLoaded);
			}
		}

		protected void OnCharacterAssetBundleLoaded(AssetBundleLoadResponse response, object extraData)
		{
			CharacterSpawnData characterSpawnData = extraData as CharacterSpawnData;
			if (characterSpawnData == null)
			{
				CspUtils.DebugLog("Spawn data missing in callback from AssetBundleLoader for character <" + response.Path + ">.");
				return;
			}
			cacheHeroesTransaction.CompleteStep(characterSpawnData.ModelName + "_assets");
			if (!string.IsNullOrEmpty(response.Error))
			{
				CspUtils.DebugLog("The following error occurred while loading character assets for <" + response.Path + ">: " + response.Error);
				cacheHeroesTransaction.CompleteStep(characterSpawnData.ModelName + "_fx");
				return;
			}
			if (response.Bundle == null)
			{
				CspUtils.DebugLog("Asset bundle is missing for <" + response.Path + ">: " + response.Error);
				cacheHeroesTransaction.CompleteStep(characterSpawnData.ModelName + "_fx");
				return;
			}
			string @string = characterSpawnData.characterData.GetString("//character_model/model_name");
			if (@string == null)
			{
				CspUtils.DebugLog("No model name in character data for <" + characterSpawnData.ModelName + ">.  Cannot spawn character.");
				cacheHeroesTransaction.CompleteStep(characterSpawnData.ModelName + "_fx");
			}
			else
			{
				AppShell.Instance.BundleLoader.LoadAsset(response.Path, @string, characterSpawnData, OnCharacterPrefabLoaded);
			}
		}

		protected void OnCharacterPrefabLoaded(UnityEngine.Object asset, AssetBundle bundle, object extraData)
		{
			CharacterSpawnData characterSpawnData = extraData as CharacterSpawnData;
			if (characterSpawnData == null)
			{
				CspUtils.DebugLog("Spawn data missing in callback from AssetBundleLoader for unknown character.");
				return;
			}
			if (asset == null)
			{
				CspUtils.DebugLog("Failed to load character prefab from asset bundle for character <" + characterSpawnData.ModelName + ">.");
				cacheHeroesTransaction.CompleteStep(characterSpawnData.ModelName + "_fx");
				return;
			}
			cacheHeroesTransaction.CompleteStep(characterSpawnData.ModelName + "_assets");
			DataWarehouse data = characterSpawnData.characterData.GetData("//effect_sequence_list");
			string text = data.TryGetString("character_fx", string.Empty);
			if (!string.IsNullOrEmpty(text) && !AppShell.Instance.BundleLoader.IsAssetBundleCachedOnDisk(text))
			{
				AppShell.Instance.BundleLoader.FetchAssetBundle(text, OnCharacterFxLoaded, characterSpawnData);
			}
			else
			{
				cacheHeroesTransaction.CompleteStep(characterSpawnData.ModelName + "_fx");
			}
		}

		protected void OnCharacterFxLoaded(AssetBundleLoadResponse response, object extraData)
		{
			CharacterSpawnData characterSpawnData = extraData as CharacterSpawnData;
			if (characterSpawnData == null)
			{
				CspUtils.DebugLog("Spawn data missing in callback from AssetBundleLoader for unknown character.");
				return;
			}
			if (!string.IsNullOrEmpty(response.Error))
			{
				CspUtils.DebugLog("Failed to find FX asset bundle for " + characterSpawnData.ModelName + ".");
			}
			cacheHeroesTransaction.CompleteStep(characterSpawnData.ModelName + "_fx");
		}

		protected void OnPlaceholderPreloaded(GameDataLoadResponse response, object extraData)
		{
			if (!string.IsNullOrEmpty(response.Error))
			{
				CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			}
			if (startTransaction != null)
			{
				startTransaction.CompleteStep("placeholderCache");
			}
		}

		protected void OnCacheHeroesComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
		{
			cacheHeroesTransaction = null;
			if (exit != 0)
			{
				CspUtils.DebugLog("Game world initialization failed: " + error);
				owner.Abort(true);
			}
		}

		protected void OnStartComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
		{
			startTransaction = null;
			if (exit != 0)
			{
				CspUtils.DebugLog("Game world initialization failed: " + error);
				owner.Abort(true);
				return;
			}
			if (bundles.Count > 0)
			{
				GameObject gameObject = new GameObject("static_bundles");
				foreach (GameObject bundle in bundles)
				{
					if (bundle != null)
					{
						GameObject gameObject2 = UnityEngine.Object.Instantiate(bundle) as GameObject;
						gameObject2.transform.parent = gameObject.transform;
					}
				}
			}
			frameDelay = 15;
		}
	}

	internal class SocialSpaceRailsTutorialControllerPlay : IDisposable, IShsState
	{
		protected SocialSpaceRailsTutorialControllerImpl owner;

		public SocialSpaceRailsTutorialControllerPlay(SocialSpaceRailsTutorialControllerImpl owner)
		{
			this.owner = owner;
		}

		public void Enter(Type previousState)
		{
			AppShell.Instance.AudioManager.RequestCrossfade(null);
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}

		public void Dispose()
		{
			owner = null;
		}
	}

	protected ShsFSM fsm;

	private string zoneName;

	protected int zoneID = -1;

	protected Matchmaker2.Ticket zoneTicket;

	protected string zoneCharacter;

	protected string zoneSpawnPoint;

	public string zoneTitle;

	protected SocialSpaceController owner;

	public string ZoneName
	{
		get
		{
			return zoneName;
		}
	}

	public void SetOwner(GameController owner)
	{
		this.owner = (owner as SocialSpaceController);
	}

	public void Awake()
	{
	}

	public void OnEnable()
	{
	}

	public void OnDisable()
	{
		if (fsm != null)
		{
			fsm.ClearState();
			fsm.Dispose();
			fsm = null;
		}
	}

	public void Start()
	{
		zoneName = (AppShell.Instance.SharedHashTable["SocialSpaceLevel"] as string);
		zoneTicket = (AppShell.Instance.SharedHashTable["SocialSpaceTicket"] as Matchmaker2.Ticket);
		zoneCharacter = (AppShell.Instance.SharedHashTable["SocialSpaceCharacter"] as string);
		zoneSpawnPoint = (AppShell.Instance.SharedHashTable["SocialSpaceSpawnPoint"] as string);
		AppShell.Instance.StoreLocationInfo();
		AppShell.Instance.SharedHashTable["SocialSpaceLevel"] = null;
		AppShell.Instance.SharedHashTable["SocialSpaceTicket"] = null;
		AppShell.Instance.SharedHashTable["SocialSpaceCharacter"] = null;
		AppShell.Instance.SharedHashTable["SocialSpaceSpawnPoint"] = null;
		fsm = new ShsFSM();
		fsm.AddState(new SocialSpaceRailsTutorialControllerLoad(this));
		fsm.AddState(new SocialSpaceRailsTutorialControllerPlay(this));
		fsm.GotoState<SocialSpaceRailsTutorialControllerLoad>();
	}

	public void OnOldControllerUnloading(AppShell.GameControllerTypeData currentGameData, AppShell.GameControllerTypeData newGameData)
	{
		AppShell.Instance.ServerConnection.DisconnectFromGame();
		UserProfile profile = AppShell.Instance.Profile;
		if (profile != null)
		{
			profile.SelectedCostume = null;
		}
		AppShell.Instance.EventMgr.Fire(this, new ZoneUnloadedMessage(zoneName));
	}

	protected void Abort(bool returnToMenu)
	{
		if (fsm != null)
		{
			fsm.ClearState();
			fsm.Dispose();
			fsm = null;
		}
		AppShell.Instance.CriticalError(SHSErrorCodes.Code.CantEnterGameWorld);
	}

	public bool AddPlayerCharacterSpawner(CharacterSpawn possibleSpawner)
	{
		return true;
	}

	public bool AddNpcSpawner(CharacterSpawn npcSpawn)
	{
		return owner.isTestScene;
	}

	public void AddSpawnPoint(SpawnPoint pt)
	{
	}

	public void Update()
	{
		if (fsm != null)
		{
			fsm.Update();
		}
	}

	public bool ChangeCharacters()
	{
		return false;
	}

	public void CharacterDespawned(GameObject character)
	{
	}

	public Vector3 GetRespawnPoint()
	{
		return Vector3.zero;
	}

	public void GrantXP(CharacterGlobals player, int xp)
	{
	}
}
