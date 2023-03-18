using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialSpaceControllerImpl : IGameController
{
	internal class SocialSpaceControllerLoad : IDisposable, IShsState
	{
		protected const string SOCIAL_BUNDLE_DIRECTORY = "SocialSpace/";

		private TransactionMonitor dataMonitor;

		private bool staticBundlesLoaded;

		private float staticBundleTicker;

		protected SocialSpaceControllerImpl owner;

		protected TransactionMonitor startTransaction;

		protected TransactionMonitor cacheHeroesTransaction;

		protected List<GameObject> bundles;

		protected int frameDelay = 15;

		protected bool viewedWelcomeScreen;

		protected GameObject placeholderSpawner;

		public SocialSpaceControllerLoad(SocialSpaceControllerImpl owner)
		{
			this.owner = owner;
		}

		public void Enter(Type previousState)
		{
			GUINotificationManager.AddDebugKeys();
			Hashtable sharedHashTable = AppShell.Instance.SharedHashTable;
			viewedWelcomeScreen = sharedHashTable.ContainsKey("WelcomeScreenViewed");
			AppShell.Instance.EventMgr.AddListener<WelcomeResponseMessage>(OnWelcomeResponse);
			AppShell.Instance.TransitionHandler.CurrentTransactionContext.CompleteChildTransactionStep("LaunchTransition", "initialize");
			if (owner.owner.isTestScene)
			{
				AppShell.Instance.TransitionHandler.CurrentTransactionContext.CompleteChildTransactionStep("LaunchLogin", "initialize");
				PreloadPlaceholder();
				owner.fsm.GotoState<SocialSpaceControllerSelect>();
				return;
			}
			bundles = new List<GameObject>();
			startTransaction = AppShell.Instance.TransitionHandler.CurrentTransactionContext.Transaction;
			startTransaction.onComplete += OnStartComplete;
			startTransaction.timeout = float.MaxValue;
			startTransaction.AddStep("spawnPointPicked");
			dataMonitor = new TransactionMonitor(OnDataLoaded, 30f, null);
			dataMonitor.AddStep("xml");
			dataMonitor.AddStep("matchmake");
			dataMonitor.AddStep("connect");
			dataMonitor.AddStep("placeholderCache");
			dataMonitor.AddStep("placeholderLoaded");
			dataMonitor.AddStep("cacheHeroesLoaded");
			startTransaction.AddChild(dataMonitor);
			if (startTransaction.HasStep("init"))
			{
				startTransaction.CompleteStep("init");
			}
			cacheHeroesTransaction = new TransactionMonitor(OnCacheHeroesComplete, 30f, null);
			cacheHeroesTransaction.Weight = 10f;
			startTransaction.AddChild(cacheHeroesTransaction);
			ShsAudioSourceList list = ShsAudioSourceList.GetList("ChallengeSystem");
			if (list != null)
			{
				list.PreloadAll(startTransaction);
			}
			if (AppShell.Instance.Profile != null)
			{
				CacheHeroAssets(AppShell.Instance.Profile.LastSelectedCostume);
			}
			Resources.UnloadUnusedAssets();
			if (owner.zoneTicket == null)
			{
				if (owner.zoneName == null)
				{
					CspUtils.DebugLog("owner.zoneName is NULL, defaulting to Daily_Bugle");
					owner.zoneName = "Daily_Bugle";
				}
				AppShell.Instance.Matchmaker2.JoinGameWorld(owner.zoneName, OnMatchmakingTicket);
			}
			else
			{
				CspUtils.DebugLog("Trying to join gameworld with an exiting ticket - valid cases is go to friend, otherwise almost certain to fail! " + owner.zoneTicket.ticket);
				OnMatchmakingTicket(owner.zoneTicket);
			}
			AppShell.Instance.DataManager.LoadGameData("spaces", OnZoneDataLoaded, new Zones(), startTransaction);
			PreloadPlaceholder();
		}

		private void OnDataLoaded(TransactionMonitor.ExitCondition exit, string error, object userData)
		{
			if (exit != 0)
			{
				CspUtils.DebugLog("Game world initialization failed: " + error);
				owner.Abort(true);
				return;
			}
			owner.netSpawnHandler = new PlaceholderSpawnInterrupt(owner.placeholderCharacterName);
			PlaceholderSpawnInterrupt netSpawnHandler = owner.netSpawnHandler;
			netSpawnHandler.OnCharacterSpawned = (PlaceholderSpawnInterrupt.CharacterSpawned)Delegate.Combine(netSpawnHandler.OnCharacterSpawned, new PlaceholderSpawnInterrupt.CharacterSpawned(owner.ConfigureCombatController));
			NetGameManager game = AppShell.Instance.ServerConnection.Game;
			game.onAboutToSpawnEntity = (NetGameManager.AboutToSpawnEntity)Delegate.Combine(game.onAboutToSpawnEntity, new NetGameManager.AboutToSpawnEntity(owner.netSpawnHandler.OnRemotePlayerSpawned));
			CspUtils.DebugLog("bundles.Count=" + bundles.Count);
			if (bundles.Count > 0)
			{
				GameObject gameObject = new GameObject("static_bundles");
				foreach (GameObject bundle in bundles)
				{
					if (bundle != null)
					{
						CspUtils.DebugLog("bundle != null");
						GameObject gameObject2 = UnityEngine.Object.Instantiate(bundle) as GameObject;
						gameObject2.transform.parent = gameObject.transform;
					}
					else {
						CspUtils.DebugLog("bundle == null");
					}
				}
			}
			AppShell.Instance.EventMgr.Fire(this, new ZoneLoadedMessage(owner.zoneName, owner.zoneID));
			staticBundlesLoaded = true;
			staticBundleTicker = 0f;
			frameDelay = 15;
		}

		private void OnWelcomeResponse(WelcomeResponseMessage message)
		{
			if (message.ControlRef != null && message.ControlRef.IsVisible)
			{
				message.ControlRef.Hide();
			}
			owner.fsm.GotoState<SocialSpaceControllerSelect>();
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

		protected void OnCharacterPrespawned(GameObject newCharacter, CharacterSpawnData spawnData)
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
			if (response.Error != null && response.Error != string.Empty)
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
			if (response.Error != null && response.Error != string.Empty)
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
			if (response.Error != null && response.Error != string.Empty)
			{
				CspUtils.DebugLog("Failed to find FX asset bundle for " + characterSpawnData.ModelName + ".");
			}
			cacheHeroesTransaction.CompleteStep(characterSpawnData.ModelName + "_fx");
		}

		public void Update()
		{
			if (startTransaction == null)
			{
				if (frameDelay == 10)
				{
					AppShell.Instance.ServerConnection.QueryAllOwnership();
				}
				else if (frameDelay == 9)
				{
					GameObject gameObject = new GameObject("KillyMcKillington");
					gameObject.transform.position = new Vector3(0f, -200f, 0f);
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
				else if (frameDelay != 8)
				{
					if (frameDelay == 7)
					{
						owner.owner.ControllerReady();
						AppShell.Instance.ServerConnection.Game.ClientReady();
					}
					else if (frameDelay == 0)
					{
						if (viewedWelcomeScreen)
						{
							owner.fsm.GotoState<SocialSpaceControllerSelect>();
						}
						return;
					}
				}
				frameDelay--;
			}
			else if (staticBundlesLoaded)
			{
				staticBundleTicker += Time.deltaTime;
				if (staticBundleTicker >= 0.1f)
				{
					staticBundlesLoaded = false;
					owner.PickSpawnPoint();
					startTransaction.CompleteStep("spawnPointPicked");
				}
			}
		}

		public void Leave(Type nextState)
		{
			startTransaction = null;
			AppShell.Instance.Matchmaker2.Cancel();
			AppShell.Instance.EventMgr.RemoveListener<WelcomeResponseMessage>(OnWelcomeResponse);
			AppShell.Instance.SharedHashTable["WelcomeScreenViewed"] = true;
		}

		public void Dispose()
		{
			startTransaction = null;
			owner = null;
		}

		protected void PreloadPlaceholder()
		{
			placeholderSpawner = (UnityEngine.Object.Instantiate(Resources.Load("Spawners/PlaceholderSpawner")) as GameObject);
			if (placeholderSpawner == null)
				CspUtils.DebugLog("placeholderSpawner is null!!!");
			else
				CspUtils.DebugLog("placeholderSpawner is NOT null!!!");
			//CharacterSpawn component = Utils.GetComponent<CharacterSpawn>(placeholderSpawner);
			CharacterSpawn component = (CharacterSpawn)(placeholderSpawner.GetComponent(typeof(CharacterSpawn)));  // CSP - try this instead of above line.			
			if (component != null)
				CspUtils.DebugLog("component is NOT null!!!");
			else {
				CspUtils.DebugLog("component is null!!!");
				Component [] components = placeholderSpawner.GetComponents(typeof(Component));  // CSP - loop over all components because single get not working.
				for (int i = 0; i < components.Length; ++i) {
					//CspUtils.DebugLog("component name = " + components[i].name);
					//CspUtils.DebugLog("component type = " + components[i].GetType());	
					if (components[i].GetType().ToString() == "CharacterSpawn") {
						//CspUtils.DebugLog("component found!");
						component = (CharacterSpawn)(components[i]);
						break;
					}
				}
			}


			if (component != null)
			{
				owner.placeholderCharacterName = component.CharacterName;
				UnityEngine.Object.Destroy(placeholderSpawner);
				AppShell.Instance.DataManager.LoadGameData("Characters/" + owner.placeholderCharacterName, OnPlaceholderPreloaded, null);
			}
			else if (startTransaction != null)
			{
				dataMonitor.FailStep("placeholderCache", "Could not preload placeholder character");
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
				dataMonitor.FailStep("xml", "Unable to load xml");
				return;
			}
			Zones zones = (Zones)response.DataDefinition;
			Zones.Layout[] layouts = zones.layouts;
			foreach (Zones.Layout layout in layouts)
			{
				if (layout.primaryZone && !SocialSpaceController.Instance.primaryZones.Contains(layout.gameworldID))
				{
					SocialSpaceController.Instance.primaryZones.Add(layout.gameworldID);
				}
			}
			if (!owner.owner.isTestScene)
			{
				string a = owner.zoneName.ToLower();
				Zones.Layout[] layouts2 = zones.layouts;
				foreach (Zones.Layout layout2 in layouts2)
				{
					if (!(a != layout2.name.ToLower()))
					{
						owner.zoneTitle = layout2.title;
						owner.zoneID = layout2.gameworldID;
						string[] array = layout2.bundles;
						foreach (string str in array)
						{
							string text = "SocialSpace/" + str;
							dataMonitor.AddStep(text, TransactionMonitor.DumpTransactionStatus);
							AppShell.Instance.BundleLoader.FetchAssetBundle(text, OnAssetBundleLoaded, startTransaction, true);
						}
						break;
					}
				}
			}
			dataMonitor.CompleteStep("xml");
		}

		protected void OnAssetBundleLoaded(AssetBundleLoadResponse response, object extraData)
		{
			CspUtils.DebugLog("OnAssetBundleLoaded called!");
			if (startTransaction != null)
			{
				CspUtils.DebugLog("startTransaction != null whoohoo");
				if ((response.Error != null && response.Error != string.Empty) || response.Bundle == null)
				{
					dataMonitor.Fail("OnAssetBundleLoaded: " + response.Error);
					return;
				}
				if (response.Bundle.mainAsset != null)
					CspUtils.DebugLog("response.Bundle.mainAsset.name=" + response.Bundle.mainAsset.name);
				else
					CspUtils.DebugLog("response.Bundle.mainAsset=null");
				bundles.Add(response.Bundle.mainAsset as GameObject);
				dataMonitor.CompleteStep(response.Path);
			}
		}

		protected void OnMatchmakingTicket(Matchmaker2.Ticket ticket)
		{
			if (startTransaction != null)
			{
				if (ticket.status != 0)
				{
					dataMonitor.FailStep("matchmake", "OnMatchmakingTicket error: " + ticket.server + " : " + ticket.ticket);
					return;
				}
				dataMonitor.CompleteStep("matchmake");
				AppShell.Instance.ServerConnection.ConnectToGame("shs.all", ticket, OnServerConnect);
			}
		}

		protected void OnServerConnect(bool success, string error)
		{
			if (startTransaction != null)
			{
				if (!success)
				{
					dataMonitor.FailStep("connect", "OnServerConnect: " + error);
				}
				else
				{
					dataMonitor.CompleteStep("connect");
				}
			}
		}

		protected void OnPlaceholderPreloaded(GameDataLoadResponse response, object extraData)
		{
			if (response.Error != null && response.Error != string.Empty)
			{
				CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
				if (startTransaction != null)
				{
					dataMonitor.FailStep("placeholderCache", response.Error);
				}
			}
			else if (startTransaction != null)
			{
				dataMonitor.CompleteStep("placeholderCache");
				dataMonitor.CompleteStep("placeholderLoaded");
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
			else
			{
				dataMonitor.CompleteStep("cacheHeroesLoaded");
			}
		}

		protected void OnStartComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
		{
			startTransaction = null;
		}
	}

	internal class SocialSpaceControllerNews : IDisposable, IShsState
	{
		protected SocialSpaceControllerImpl owner;

		public SocialSpaceControllerNews(SocialSpaceControllerImpl owner)
		{
			this.owner = owner;
		}

		public void Enter(Type previousState)
		{
			if (!NewTutorialManager.allowDailyRewardWindow())
			{
				AppShell.Instance.SharedHashTable["NewsPaperHasBeenShown"] = true;
				owner.fsm.GotoState<SocialSpaceControllerSelect>();
			}
			else
			{
				AppShell.Instance.EventMgr.AddListener<NewsClosedMessage>(OnNewsClosedMessage);
				GUIManager.Instance.ShowDynamicWindow(new SHSWelcomeGadget(), GUIControl.ModalLevelEnum.Full);
			}
		}

		public void Update()
		{
		}

		public void OnNewsClosedMessage(NewsClosedMessage msg)
		{
			AppShell.Instance.SharedHashTable["NewsPaperHasBeenShown"] = true;
			owner.fsm.GotoState<SocialSpaceControllerSelect>();
		}

		public void Leave(Type nextState)
		{
			AppShell.Instance.EventMgr.RemoveListener<NewsClosedMessage>(OnNewsClosedMessage);
		}

		public void Dispose()
		{
			owner = null;
		}
	}

	internal class SocialSpaceControllerPlay : IDisposable, IShsState
	{
		internal StagedSpawner stagedSpawner;

		protected SocialSpaceControllerImpl owner;

		public SocialSpaceControllerPlay(SocialSpaceControllerImpl owner)
		{
			this.owner = owner;
		}

		public void Enter(Type previousState)
		{
			if (owner.owner.isTestScene)
			{
				if (owner.playerSpawners.Count > 0)
				{
					owner.characterSelected = owner.playerSpawners[0].CharacterName;
				}
				else
				{
					owner.characterSelected = "iron_man";
				}
				AppShell.Instance.EventMgr.Fire(this, new ZoneLoadedMessage(owner.zoneName, owner.zoneID));
			}
			else if (NewTutorialManager.allowDailyRewardWindow() && !AppShell.Instance.SharedHashTable.ContainsKey("NewsPaperHasBeenShown"))
			{
				AppShell.Instance.EventMgr.AddListener<NewsClosedMessage>(OnWelcomeGadgetClosed);
				GUIManager.Instance.ShowDynamicWindow(new SHSWelcomeGadget(), GUIControl.ModalLevelEnum.Full);
				return;
			}
			SpawnPlayerAndInitialize();
		}

		public void OnWelcomeGadgetClosed(NewsClosedMessage msg)
		{
			AppShell.Instance.EventMgr.RemoveListener<NewsClosedMessage>(OnWelcomeGadgetClosed);
			SpawnPlayerAndInitialize();
		}

		private void SpawnPlayerAndInitialize()
		{
			AppShell.Instance.AudioManager.RequestCrossfade(null);
			bool flag = false;
			if (!flag && owner.PlayerOverrideSpawningEnabled)
			{
				GameObject reSpawner = new GameObject("__ReSpawner__");
				reSpawner.transform.position = owner.PlayerOverrideEnterLocation;
				reSpawner.transform.rotation = owner.PlayerOverrideEnterRotation;
				SpawnPoint spawnPoint = Utils.AddComponent<SpawnPoint>(reSpawner);
				stagedSpawner = new StagedSpawner(spawnPoint, owner.placeholderCharacterName, owner.characterSelected, owner.standIn);
				stagedSpawner.Spawn(delegate(CharacterGlobals player)
				{
					stagedSpawner = null;
					UnityEngine.Object.Destroy(reSpawner);
					owner.ConfigureCombatController(player);
				});
				owner.PlayerOverrideSpawningEnabled = false;
				flag = true;
			}
			if (!flag && owner.spawnPoint != null)
			{
				stagedSpawner = new StagedSpawner(owner.spawnPoint, owner.placeholderCharacterName, owner.characterSelected, null);
				stagedSpawner.Spawn(delegate(CharacterGlobals player)
				{
					stagedSpawner = null;
					owner.ConfigureCombatController(player);
				});
				flag = true;
			}
			if (!flag && owner.playerSpawners.Count > 0)
			{
				CspUtils.DebugLog("Spawning using the fallback spawner");
				int index = UnityEngine.Random.Range(0, owner.playerSpawners.Count);
				owner.playerSpawners[index].SpawnPlayerCharacter(owner.characterSelected);
			}
			RegisterMessageHandlers();
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
			if (stagedSpawner != null)
			{
				stagedSpawner.Dispose();
				stagedSpawner = null;
			}
			UnregisterMessageHandlers();
		}

		public void Dispose()
		{
			UnregisterMessageHandlers();
			owner = null;
		}

		protected void RegisterMessageHandlers()
		{
		}

		protected void UnregisterMessageHandlers()
		{
		}
	}

	internal class SocialSpaceControllerSelect : IDisposable, IShsState
	{
		protected SocialSpaceControllerImpl owner;

		public SocialSpaceControllerSelect(SocialSpaceControllerImpl owner)
		{
			this.owner = owner;
		}

		public void Enter(Type previousState)
		{
			AppShell.Instance.EventMgr.AddListener<CharacterSelectedMessage>(OnPlayerCharacterSelected);
			GameObject localPlayer = owner.owner.LocalPlayer;
			if (localPlayer == null)
			{
				if (owner.standIn == null && owner.spawnPoint == null)
				{
					owner.PickSpawnPoint();
				}
			}
			else
			{
				owner.PlayerOverrideSpawningEnabled = true;
				owner.PlayerOverrideEnterLocation = localPlayer.transform.position;
				owner.PlayerOverrideEnterRotation = localPlayer.transform.rotation;
				owner.spawnPoint = null;
				CharacterGlobals component = Utils.GetComponent<CharacterGlobals>(localPlayer);
				if (component != null)
				{
					if (owner.standIn == null)
					{
						owner.standIn = new GameObject("__StandIn__");
						owner.standIn.transform.position = localPlayer.transform.position;
						owner.standIn.transform.rotation = localPlayer.transform.rotation;
						PlayerOcclusionDetector.Instance.myPlayer = owner.standIn;
						CameraTargetHelper componentInChildren = localPlayer.GetComponentInChildren<CameraTargetHelper>();
						if (componentInChildren != null)
						{
							componentInChildren.SetTarget(owner.standIn.transform);
						}
					}
					component.spawnData.Despawn(EntityDespawnMessage.despawnType.defeated);
				}
			}
			owner.characterSelected = string.Empty;
			if (!owner.owner.isTestScene)
			{
				PickASelectedCharacter();
			}
		}

		public void Update()
		{
			if ((owner.owner.isTestScene || owner.characterSelected != string.Empty) && (owner.playerSpawners.Count > 0 || owner.spawnPoints.Count > 0))
			{
				owner.fsm.GotoState<SocialSpaceControllerPlay>();
			}
		}

		public void Leave(Type nextState)
		{
			AppShell.Instance.EventMgr.RemoveListener<CharacterSelectedMessage>(OnPlayerCharacterSelected);
		}

		public void Dispose()
		{
			owner = null;
		}

		private void showCharacterSelector()
		{
			MySquadDataManager dataManager = new MySquadDataManager(AppShell.Instance.Profile);
			MyHeroesWindow dialogWindow = new MyHeroesWindow(dataManager);
			GUIManager.Instance.ShowDynamicWindow(dialogWindow, GUIControl.ModalLevelEnum.Default);
		}

		protected void PickASelectedCharacter()
		{
			if (AppShell.Instance.SharedHashTable.ContainsKey("GUIGameWorldForceCharacterSelect") && (bool)AppShell.Instance.SharedHashTable["GUIGameWorldForceCharacterSelect"])
			{
				AppShell.Instance.SharedHashTable["GUIGameWorldForceCharacterSelect"] = false;
				showCharacterSelector();
				return;
			}
			UserProfile profile = AppShell.Instance.Profile;
			if (profile == null)
			{
				CspUtils.DebugLog("Warning: No Profile Found");
				return;
			}
			if (!string.IsNullOrEmpty(owner.zoneCharacter))
			{
				owner.characterSelected = owner.zoneCharacter;
				owner.zoneCharacter = string.Empty;
				profile.SelectedCostume = owner.characterSelected;
			}
			if (!string.IsNullOrEmpty(profile.LastSelectedCostume))
			{
				profile.SelectedCostume = profile.LastSelectedCostume;
			}
			if (string.IsNullOrEmpty(profile.SelectedCostume))
			{
				foreach (KeyValuePair<string, HeroPersisted> availableCostume in profile.AvailableCostumes)
				{
					if (!availableCostume.Value.ShieldAgentOnly || (availableCostume.Value.ShieldAgentOnly && Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldHeroesAllow)))
					{
						profile.SelectedCostume = availableCostume.Key;
						break;
					}
				}
			}
			if (string.IsNullOrEmpty(profile.SelectedCostume))
			{
				showCharacterSelector();
				return;
			}
			CharacterSelectedMessage msg = new CharacterSelectedMessage(profile.SelectedCostume);
			AppShell.Instance.EventMgr.Fire(this, msg);
			OnPlayerCharacterSelected(msg);
		}

		protected void OnPlayerCharacterSelected(CharacterSelectedMessage msg)
		{
			owner.characterSelected = msg.CharacterName;
			AppShell.Instance.SharedHashTable["SocialSpaceCharacterCurrent"] = msg.CharacterName;
			AppShell.Instance.EventReporter.ReportCharacterSelection(msg.CharacterName);
		}
	}

	protected ShsFSM fsm;

	protected static SocialSpaceControllerImpl instance;

	protected string zoneName;

	protected int zoneID = -1;

	protected Matchmaker2.Ticket zoneTicket;

	protected string zoneCharacter;

	protected string zoneSpawnPoint;

	protected Vector3 zoneSpawnPointPositionOverride = Vector3.zero;

	protected bool zoneLoaded;

	protected bool zoneFirstSpawn = true;

	protected Dictionary<string, List<SpawnPoint>> spawnPoints;

	protected List<CharacterSpawn> playerSpawners;

	protected List<CharacterSpawn> npcSpawners;

	protected string characterSelected = string.Empty;

	protected string lastSelectedHero = string.Empty;

	protected int targetUserId = -1;

	public string zoneTitle;

	protected GameObject standIn;

	protected string placeholderCharacterName;

	protected PlaceholderSpawnInterrupt netSpawnHandler;

	protected float originalAttenuationScale;

	public bool PlayerOverrideSpawningEnabled;

	public Vector3 PlayerOverrideEnterLocation = Vector3.zero;

	public Quaternion PlayerOverrideEnterRotation = Quaternion.identity;

	public SpawnPoint spawnPoint;

	protected SocialSpaceController owner;

	private static float lastNonIdleTime;

	public static bool playerIsIdle;

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
		instance = this;
		characterSelected = string.Empty;
		targetUserId = -1;
		spawnPoints = new Dictionary<string, List<SpawnPoint>>();
		playerSpawners = new List<CharacterSpawn>();
		npcSpawners = new List<CharacterSpawn>();
		if ((AppShell.Instance.ServerConnection.State & NetworkManager.ConnectionState.Authenticated) == 0)
		{
			AppShell.Instance.StubServerConnection();
		}
	}

	public static string getZoneName()
	{
		return instance.zoneName;
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
		AppShell.Instance.SharedHashTable["SocialSpaceSpawnPointPositionOverride"] = null;
		fsm = new ShsFSM();
		fsm.AddState(new SocialSpaceControllerLoad(this));
		fsm.AddState(new SocialSpaceControllerNews(this));
		fsm.AddState(new SocialSpaceControllerSelect(this));
		fsm.AddState(new SocialSpaceControllerPlay(this));
		fsm.GotoState<SocialSpaceControllerLoad>();
		originalAttenuationScale = ShsAudioSource.AttenuationScale;
		ShsAudioSource.AttenuationScale = 0.4f;
		AppShell.Instance.EventMgr.AddListener<BattleInviteMessage>(OnBattleInviteMessage);
		AppShell.Instance.EventMgr.AddListener<LocalPlayerChangedMessage>(OnLocalPlayerChanged);
		AppShell.Instance.EventMgr.AddListener<ZoneLoadedMessage>(OnZoneLoaded);
		AppShell.Instance.EventMgr.AddListener<EntityFadeMessage>(OnEntityFade);
		AppShell.Instance.EventMgr.AddListener<EntityEngoldenMessage>(OnEntityEngolden);
		AppShell.Instance.EventMgr.AddListener<EntityTakeoffMessage>(OnEntityTakeoff);
		AppShell.Instance.EventMgr.AddListener<EntityToRCBotMessage>(OnEntityToRCBot);

	}

	public void OnOldControllerUnloading(AppShell.GameControllerTypeData currentGameData, AppShell.GameControllerTypeData newGameData)
	{
		AppShell.Instance.ServerConnection.DisconnectFromGame();
		AppShell.Instance.EventMgr.RemoveListener<LocalPlayerChangedMessage>(OnLocalPlayerChanged);
		AppShell.Instance.EventMgr.RemoveListener<BattleInviteMessage>(OnBattleInviteMessage);
		AppShell.Instance.EventMgr.RemoveListener<ZoneLoadedMessage>(OnZoneLoaded);
		AppShell.Instance.EventMgr.RemoveListener<EntityFadeMessage>(OnEntityFade);
		AppShell.Instance.EventMgr.RemoveListener<EntityEngoldenMessage>(OnEntityEngolden);
		AppShell.Instance.EventMgr.RemoveListener<EntityTakeoffMessage>(OnEntityTakeoff);
		AppShell.Instance.EventMgr.RemoveListener<EntityToRCBotMessage>(OnEntityToRCBot);
		UserProfile profile = AppShell.Instance.Profile;
		if (profile != null)
		{
			profile.SelectedCostume = null;
		}
		if (AppShell.Instance.ServerConnection != null && AppShell.Instance.ServerConnection.Game != null && netSpawnHandler != null)
		{
			NetGameManager game = AppShell.Instance.ServerConnection.Game;
			game.onAboutToSpawnEntity = (NetGameManager.AboutToSpawnEntity)Delegate.Remove(game.onAboutToSpawnEntity, new NetGameManager.AboutToSpawnEntity(netSpawnHandler.OnRemotePlayerSpawned));
			netSpawnHandler.CancelAll();
		}
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		if (localPlayer != null)
		{
			AppShell.Instance.EventMgr.RemoveListener<CharacterStat.StatChangeEvent>(localPlayer, OnCharacterStatChanged);
		}
		ShsAudioSource.AttenuationScale = originalAttenuationScale;
		zoneLoaded = false;
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
		playerSpawners.Add(possibleSpawner);
		return owner.isTestScene;
	}

	public bool AddNpcSpawner(CharacterSpawn npcSpawn)
	{
		npcSpawners.Add(npcSpawn);
		return owner.isTestScene;
	}

	public void AddSpawnPoint(SpawnPoint pt)
	{
		CspUtils.DebugLog("SSCI pt.gameObject.name="  + pt.gameObject.name);
		CspUtils.DebugLog("SSCI pt.group="  + pt.group);
		string text = pt.group ?? string.Empty;
		if (text != string.Empty)
		{
			CspUtils.DebugLog("SSCI AddSpawnPoint text=" + text);
			AddSpawnPointToList(pt, text);
		}
		string text2 = (!(pt.owner != null)) ? null : pt.owner.group;
		text2 = (text2 ?? string.Empty);
		if (text2 != string.Empty && text != text2)
		{
			CspUtils.DebugLog("SSCI AddSpawnPoint text2=" + text2);
			AddSpawnPointToList(pt, text2);
		}
	}

	protected void AddSpawnPointToList(SpawnPoint pt, string name)
	{
		List<SpawnPoint> value = null;
		if (!spawnPoints.TryGetValue(name, out value))
		{
			value = new List<SpawnPoint>();
			spawnPoints.Add(name, value);
		}
		value.Add(pt);
	}

	protected void OnBattleInviteMessage(BattleInviteMessage msg)
	{
		CspUtils.DebugLog("OnBattleInviteMessage");
		if (msg.playerId == -1 && msg.opponentName == string.Empty)
		{
			targetUserId = -1;
			return;
		}
		CspUtils.DebugLog("OnBattleInviteMessage() - asking Matchmaker to invite " + msg.opponentName + " using hero " + characterSelected);
		CspUtils.DebugLog("OnBattleInviteMessage should be deprecated");
	}

	protected void OnCardGameTicket(Matchmaker2.Ticket ticket)
	{
	}

	public void Update()
	{
		if (fsm != null)
		{
			fsm.Update();
		}
		if (owner.isTestScene && playerSpawners.Count >= 1)
		{
			AppShell.Instance.EventMgr.Fire(null, new CharacterSelectedMessage(playerSpawners[0].CharacterName));
		}
		if (AppShell.Instance != null && AppShell.Instance.Profile != null)
		{
			foreach (SpecialAbility socialAbility in AppShell.Instance.Profile.socialAbilities)
			{
				socialAbility.update();
			}
		}
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		if (localPlayer != null)
		{
			if (Time.time - lastNonIdleTime > 600f)
			{
				playerIsIdle = true;
			}
			else
			{
				playerIsIdle = false;
			}
		}
	}

	public static void bumpIdleTimer()
	{
		lastNonIdleTime = Time.time;
	}

	public bool ChangeCharacters()
	{
		SocialSpaceControllerPlay socialSpaceControllerPlay = fsm.GetCurrentStateObject() as SocialSpaceControllerPlay;
		if (socialSpaceControllerPlay == null || socialSpaceControllerPlay.stagedSpawner != null)
		{
			CspUtils.DebugLog("Not currently in play mode. or staged spawn is null.");
			return false;
		}
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		if (localPlayer != null)
		{
			CharacterGlobals component = localPlayer.GetComponent<CharacterGlobals>();
			if (component.motionController != null && !component.motionController.IsOnGround())
			{
				CspUtils.DebugLog("No motion controller or not on the ground");
				return false;
			}
			TroubleBotSticky component2 = Utils.GetComponent<TroubleBotSticky>(localPlayer);
			if (component2 != null)
			{
				CspUtils.DebugLog("Sticky trouble bot component STUCK ON ME... OH THE INHUMANITY. NO CHARACTER CHANGE FOR ME...");
				return false;
			}
			BehaviorManager behaviorManager = component.behaviorManager;
			if (behaviorManager != null)
			{
				CspUtils.DebugLog(behaviorManager.getBehavior().ToString());
				if ((!(behaviorManager.getBehavior() is BehaviorMovement) && !(behaviorManager.getBehavior() is BehaviorEmote)) || !behaviorManager.currentBehaviorInterruptible(typeof(BehaviorMovement)))
				{
					CspUtils.DebugLog("Not in a state where a character select can be done. Aborting...");
					return false;
				}
				behaviorManager.cancelBehavior();
			}
			else
			{
				CspUtils.DebugLog("Behaviour Manager is null. Aborting character change.");
			}
		}
		else
		{
			CspUtils.DebugLog("No active player character.");
		}
		AppShell.Instance.SharedHashTable["GUIGameWorldForceCharacterSelect"] = true;
		fsm.GotoState<SocialSpaceControllerSelect>();
		return true;
	}

	public void CharacterDespawned(GameObject character)
	{
		AppShell.Instance.EventMgr.RemoveListener<CharacterStat.StatChangeEvent>(character, OnCharacterStatChanged);
	}

	private string NetIdToPlayerName(int netId)
	{
		List<NetworkManager.UserInfo> gameAllUsers = AppShell.Instance.ServerConnection.GetGameAllUsers();
		foreach (NetworkManager.UserInfo item in gameAllUsers)
		{
			if (item.userId == netId)
			{
				return item.userName.ToUpper();
			}
		}
		return "this player";
	}

	public Vector3 GetRespawnPoint()
	{
		List<SpawnPoint> value = null;
		if (spawnPoints.TryGetValue("start", out value))
		{
			int index = UnityEngine.Random.Range(0, value.Count);
			return value[index].transform.position;
		}
		CspUtils.DebugLog("Did not find spawn point <start>");
		return Vector3.zero;
	}

	protected void PickSpawnPoint()
	{
		PlayerOverrideSpawningEnabled = false;
		PlayerOverrideEnterLocation = Vector3.zero;
		PlayerOverrideEnterRotation = Quaternion.identity;
		spawnPoint = null;
		if (spawnPoints.Count > 0)
		{
			//string text = zoneSpawnPoint ?? "start"; 
			string text;
			if (zoneSpawnPoint != null)
				text = zoneSpawnPoint;
			else
				text = "start";
			CspUtils.DebugLog("text spawnpoint=" + text);
			foreach(KeyValuePair<string, List<SpawnPoint>> entry in spawnPoints)
			{
				// do something with entry.Value or entry.Key
				CspUtils.DebugLog("spawnpoints:" +  entry.Key);  
				text = entry.Key;
			}

			
			List<SpawnPoint> value = null;
			if (spawnPoints.TryGetValue(text, out value))
			{
				int index = UnityEngine.Random.Range(0, value.Count);
				spawnPoint = value[index];
				spawnPoint.SetCamera();
			}
			else
			{
				CspUtils.DebugLog("Did not find spawn point <" + text + ">");
			}
		}
	}

	protected void ConfigureCombatController(CharacterGlobals spawnedPlayer)
	{
		if (!(spawnedPlayer != null) || !(spawnedPlayer.combatController != null))
		{
			return;
		}
		spawnedPlayer.combatController.EmoteBroadcastRadius = 15f;
		if (spawnedPlayer.networkComponent != null && spawnedPlayer.networkComponent.IsOwner())
		{
			AppShell.Instance.EventMgr.AddListener<CharacterStat.StatChangeEvent>(spawnedPlayer.gameObject, OnCharacterStatChanged);
			PlayerCombatController playerCombatController = spawnedPlayer.combatController as PlayerCombatController;
			if (playerCombatController != null)
			{
				playerCombatController.setPower(999f);
			}
			CombatController combatController = spawnedPlayer.combatController;
			combatController.OnSecondaryAttack = (CombatController.Attacked)Delegate.Combine(combatController.OnSecondaryAttack, new CombatController.Attacked(OnSecondaryAttack));
		}
	}

	protected void OnCharacterStatChanged(CharacterStat.StatChangeEvent e)
	{
		if (e.StatType == CharacterStats.StatType.Power && e.NewValue < e.MaxValue)
		{
			PlayerCombatController component = Utils.GetComponent<PlayerCombatController>(e.Character);
			if (component != null)
			{
				component.setPower(e.MaxValue);
			}
		}
	}

	protected void OnSecondaryAttack(CharacterGlobals attacker)
	{
		if (attacker != null && attacker.combatController != null)
		{
			int num = attacker.combatController.selectedSecondaryAttack + 1;
			num = ((num <= attacker.combatController.maximumSecondaryAttackChain) ? num : 0);
			attacker.combatController.SetSecondaryAttack(num);
		}
	}

	public void GrantXP(CharacterGlobals player, int xp)
	{
		if (!(player == null))
		{
			if (Utils.IsLocalPlayer(player))
			{
				AppShell.Instance.EventReporter.ReportAddXp(player.gameObject.name, xp);
			}
			ShowXPToast(player, xp);
		}
	}

	public static void ShowXPToast(CharacterGlobals player, int xp, int bonusXP = -1)
	{
		Vector3 position = player.transform.position;
		position.y += player.characterController.height;
		GameObject g = UnityEngine.Object.Instantiate(Resources.Load("GUI/3D/XPToast"), position, Quaternion.identity) as GameObject;
		Utils.GetComponent<XPToastAnimator>(g).Text = "+" + xp + " xp";
		double xpMultiplier = AppShell.Instance.Profile.xpMultiplier;
		if (xpMultiplier > 1.0 || bonusXP > 0)
		{
			if (bonusXP != -1)
			{
				xp = bonusXP;
			}
			else
			{
				CspUtils.DebugLog("ShowXPToast xpMod " + xpMultiplier + " xp " + xp + " " + (double)xp * (xpMultiplier - 1.0) + " " + (int)((double)xp * (xpMultiplier - 1.0) + 0.20000000298023224));
				xp = (int)((double)xp * (xpMultiplier - 1.0) + 0.5);
			}
			position = player.transform.position;
			position.y += player.characterController.height - 0.4f;
			g = (UnityEngine.Object.Instantiate(Resources.Load("GUI/3D/XPToast"), position, Quaternion.identity) as GameObject);
			Utils.GetComponent<XPToastAnimator>(g).Text = "+" + xp + " bonus xp";
		}
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("earn_xp"));
	}

	protected void OnLocalPlayerChanged(LocalPlayerChangedMessage e)
	{
		if (e.localPlayer != null)
		{
			Utils.AddComponent<AFKWatcher>(e.localPlayer);
			if (zoneLoaded && zoneFirstSpawn)
			{
				zoneFirstSpawn = false;
			}
			if (e.localPlayer.name != lastSelectedHero)
			{
				lastSelectedHero = e.localPlayer.name;
				owner.StartCoroutine(PlaySpawnVO(e.localPlayer.GetComponent<CharacterGlobals>()));
			}
		}
	}

	protected IEnumerator PlaySpawnVO(CharacterGlobals player)
	{
		yield return new WaitForSeconds(1f);
		if (player != null)
		{
			VOManager.Instance.PlayVO("spawn", player.gameObject);
		}
	}

	protected void OnZoneLoaded(ZoneLoadedMessage e)
	{
		zoneLoaded = true;
	}

	protected void OnEntityFade(EntityFadeMessage e)
	{
		if (!Utils.IsLocalPlayer(e.entity))
		{
		}
	}

	protected void OnEntityEngolden(EntityEngoldenMessage e)
	{
		if (!Utils.IsLocalPlayer(e.entity))
		{
		}
	}

	protected void OnEntityTakeoff(EntityTakeoffMessage e)
	{
		if (!Utils.IsLocalPlayer(e.entity))
		{
		}
	}

	protected void OnEntityToRCBot(EntityToRCBotMessage e)
	{
		if (!Utils.IsLocalPlayer(e.entity))
		{
		}
	}
}
