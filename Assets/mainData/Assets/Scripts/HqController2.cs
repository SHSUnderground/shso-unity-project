using ShsAudio;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HqController2 : GameController
{
	internal class HqControllerView : IShsState
	{
		public void Enter(Type previousState)
		{
			Instance.ActiveRoom.GotoViewMode();
			Instance.hqInput.GotoViewMode();
			foreach (AIControllerHQ aIController in Instance.AIControllers)
			{
				aIController.DeactivateFlinga();
			}
			AppShell.Instance.EventMgr.Fire(Instance, new HQModeChanged(GetType()));
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	internal class HqControllerFlinga : IShsState
	{
		public void Enter(Type previousState)
		{
			Instance.ActiveRoom.GotoFlingaMode();
			Instance.hqInput.GotoFlingaMode();
			Instance.SetAIPauseState(false);
			AppShell.Instance.EventMgr.Fire(Instance, new HQModeChanged(GetType()));
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
			Instance.SetAIPauseState(true);
		}
	}

	internal class HqControllerIntro : IShsState
	{
		private bool trackingHero;

		private List<string> initialHeroes;

		private string initialHero;

		public void Enter(Type previousState)
		{
			initialHeroes = new List<string>();
			trackingHero = false;
			AppShell.Instance.AudioManager.RequestCrossfade(null);
			if (Instance.minimumAI == 0)
			{
				GoToFlingaModeAndDisplayUI();
			}
		}

		public void Leave(Type nextState)
		{
			foreach (AIControllerHQ aIController in Instance.AIControllers)
			{
				if (aIController.CurrentRoom != Instance.ActiveRoom)
				{
					aIController.DeActivate();
				}
			}
		}

		public void GoToFlingaModeAndDisplayUI()
		{
			if (initialHeroes != null)
			{
				initialHeroes.Clear();
			}
			Instance.ControllerReady();
			Instance.StartTransaction.CompleteStep("controllerReady");
			Instance.fsm.GotoState<HqControllerFlinga>();
			GameObject aI = Instance.GetAI(initialHero);
			if (aI != null)
			{
				AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(aI);
				if (component != null)
				{
					component.DoGrandEntrance();
				}
			}
			List<string> list = new List<string>();
			foreach (Item value in Instance.Profile.AvailableItems.Values)
			{
				string placedObjectAssetBundle = value.Definition.PlacedObjectAssetBundle;
				if (placedObjectAssetBundle != null && !Instance.IsAssetBundleLoaded(value.Definition) && !list.Contains(placedObjectAssetBundle))
				{
					list.Add(placedObjectAssetBundle);
				}
			}
			foreach (string item in list)
			{
				AppShell.Instance.BundleLoader.FetchAssetBundle(item, Instance.onAssetBundleLoaded, null);
			}
			AppShell.Instance.EventMgr.Fire(this, new SHSHQWindow.DisplayHQUIMessage());
		}

		public void Update()
		{
			if (Instance.HasProxiesLoading)
			{
				return;
			}
			if (!trackingHero)
			{
				if (Instance.Profile.AvailableCostumes.Count == 0)
				{
					CspUtils.DebugLog("Player has no available costumes! Could not spawn default heroes.");
					GoToFlingaModeAndDisplayUI();
					return;
				}
				initialHero = Instance.Profile.LastSelectedCostume;
				foreach (AIControllerHQ item in Instance.ActiveRoom.AIInRoom)
				{
					if (item.CharacterName != initialHero)
					{
						initialHeroes.Add(item.CharacterName);
					}
				}
				initialHeroes.Add(initialHero);
				if (initialHeroes.Count < Instance.minimumAI)
				{
					List<string> list = new List<string>();
					foreach (HeroPersisted value in Instance.Profile.AvailableCostumes.Values)
					{
						if (value.ShieldAgentOnly && !Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldHeroesAllow))
						{
							if (value.Name == initialHero)
							{
								initialHeroes.Remove(initialHero);
								initialHero = null;
							}
						}
						else
						{
							GameObject aI = Instance.GetAI(value.Name);
							if (!(aI != null) && !(value.Name == initialHero) && !initialHeroes.Contains(value.Name))
							{
								list.Add(value.Name);
							}
						}
					}
					if (initialHero == null && list.Count > 0)
					{
						initialHero = list[UnityEngine.Random.Range(0, list.Count)];
					}
					int num = Instance.minimumAI - initialHeroes.Count;
					if (list.Count <= num)
					{
						initialHeroes.AddRange(list);
					}
					else
					{
						for (int i = 0; i < num; i++)
						{
							int index = UnityEngine.Random.Range(0, list.Count);
							initialHeroes.Add(list[index]);
							list.RemoveAt(index);
						}
					}
				}
				foreach (string initialHero2 in initialHeroes)
				{
					Vector3 vector = Instance.ActiveRoom.RandomLocation;
					if (initialHero2 == initialHero || vector == Vector3.zero)
					{
						vector = Instance.ActiveRoom.RandomDoor;
					}
					if (!Instance.IsAISpawned(initialHero2))
					{
						Instance.ActiveRoom.PlaceAICharacter(initialHero2, vector);
					}
					else
					{
						GameObject aI2 = Instance.GetAI(initialHero2);
						if (aI2 != null)
						{
							AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(aI2);
							if (component != null && component.CurrentRoom != Instance.ActiveRoom)
							{
								component.SpawnRoom.DespawnAI(component);
								Instance.ActiveRoom.SpawnAI(component);
								component.CurrentRoom = Instance.ActiveRoom;
							}
							aI2.transform.position = vector;
						}
					}
				}
				if (initialHero == null)
				{
					CspUtils.DebugLog("Switching to flinga mode");
					GoToFlingaModeAndDisplayUI();
				}
				else
				{
					trackingHero = true;
				}
			}
			else if (initialHeroes.Count > 0)
			{
				foreach (string initialHero3 in initialHeroes)
				{
					if (Instance.IsSpawning(initialHero3))
					{
						return;
					}
				}
				CspUtils.DebugLog("Switching to flinga mode");
				GoToFlingaModeAndDisplayUI();
			}
		}
	}

	internal class HqControllerLoad : IDisposable, IShsState
	{
		protected class CharacterLoadData
		{
			public TransactionMonitor transactionMonitor;

			public string characterName;

			public string modelName;

			public CharacterLoadData(TransactionMonitor monitor, string characterName)
			{
				transactionMonitor = monitor;
				this.characterName = characterName;
			}
		}

		protected HqController2 owner;

		protected TransactionMonitor initTransaction;

		protected TransactionMonitor startTransaction;

		protected TransactionMonitor roomLoadTransaction;

		protected TransactionMonitor itemLoadTransaction;

		public HqControllerLoad(HqController2 owner)
		{
			this.owner = owner;
		}

		public void Enter(Type previousState)
		{
			UnityEngine.Object[] topLevelObjects = DebugUtil.GetTopLevelObjects();
			UnityEngine.Object[] array = topLevelObjects;
			foreach (UnityEngine.Object @object in array)
			{
				if (@object is GameObject)
				{
					GameObject gameObject = @object as GameObject;
					if (Utils.GetComponent<Animation>(gameObject) != null)
					{
						CspUtils.DebugLog("Found unwanted game object before loading HQ: " + gameObject.name);
					}
				}
			}
			Instance.staticData = new HqStaticData();
			owner.StartTransaction = AppShell.Instance.TransitionHandler.CurrentTransactionContext.Transaction;
			startTransaction = owner.StartTransaction;
			initTransaction = TransactionMonitor.CreateTransactionMonitor("HQ Init Transaction", OnStartComplete, float.MaxValue, null);
			initTransaction.AddStep("start", "Started Load");
			initTransaction.AddStep("bundles");
			startTransaction.AddChild(initTransaction);
			if (Instance.Sounds != null)
			{
				Instance.Sounds.PreloadAll(startTransaction);
			}
			roomLoadTransaction = TransactionMonitor.CreateTransactionMonitor("HQ Room Load Transaction", OnRoomLoadComplete, float.MaxValue, null);
			roomLoadTransaction.AddStep("room_xml", "Room XML Loaded");
			roomLoadTransaction.AddStep("theme_xml", "Theme XML Loaded");
			initTransaction.AddChild(roomLoadTransaction);
			if (Instance.Profile != null && AppShell.Instance.SharedHashTable["HQ_Placement"] == null)
			{
				string uri = "resources$users/hq_room_all.py";
				roomLoadTransaction.AddStep("room_placement");
				AppShell.Instance.WebService.StartRequest(uri, delegate(ShsWebResponse r)
				{
					RoomPlacementLoaded(r, roomLoadTransaction);
				}, null, ShsWebService.ShsWebServiceType.RASP);
			}
			AppShell.Instance.DataManager.LoadGameData("HQ/Rooms", RoomDataLoaded, new HqStaticData(), roomLoadTransaction);
			AppShell.Instance.DataManager.LoadGameData("HQ/Themes", ThemeDataLoaded, new HqStaticData(), roomLoadTransaction);
			foreach (string key in Instance.Profile.AvailableCostumes.Keys)
			{
				StartCharacterDataLoad(key, roomLoadTransaction);
			}
			StartBundleLoad("HQ/hq_shared", roomLoadTransaction);
			initTransaction.CompleteStep("start");
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}

		public void Dispose()
		{
			startTransaction = null;
			roomLoadTransaction = null;
		}

		protected void RoomDataLoaded(GameDataLoadResponse response, object extraData)
		{
			if (roomLoadTransaction == extraData)
			{
				if (response.DataDefinition == null)
				{
					roomLoadTransaction.FailStep("room_xml", "File <" + response.Path + "> failed to load");
					return;
				}
				Instance.staticData.rooms = ((HqStaticData)response.DataDefinition).rooms;
				foreach (RoomStaticData room in Instance.staticData.rooms)
				{
					if (!(room.bundleName == string.Empty) && Instance.Profile.AvailableHQRooms.ContainsKey(room.typeid))
					{
						StartBundleLoad(room.bundleName, roomLoadTransaction);
						if (room.fixedBundleNames != null)
						{
							foreach (string fixedBundleName in room.fixedBundleNames)
							{
								StartBundleLoad(fixedBundleName, roomLoadTransaction);
							}
						}
					}
				}
				StartBundleLoad(Helpers.GetAudioBundleName(SABundle.HQ_Physics), roomLoadTransaction);
				StartBundleLoad(Helpers.GetAudioBundleName(SABundle.HQ_Base), roomLoadTransaction);
				StartBundleLoad(Helpers.GetAudioBundleName(SABundle.HQ_Hero_Common), roomLoadTransaction);
				roomLoadTransaction.CompleteStep("room_xml");
			}
		}

		protected void ThemeDataLoaded(GameDataLoadResponse response, object extraData)
		{
			if (roomLoadTransaction == extraData)
			{
				if (response.DataDefinition == null)
				{
					roomLoadTransaction.FailStep("theme_xml", "File <" + response.Path + "> failed to load");
					return;
				}
				Instance.staticData.defaultTheme = ((HqStaticData)response.DataDefinition).defaultTheme;
				Instance.staticData.colors = ((HqStaticData)response.DataDefinition).colors;
				Instance.staticData.room_themes = ((HqStaticData)response.DataDefinition).room_themes;
				Instance.staticData.item_themes = ((HqStaticData)response.DataDefinition).item_themes;
				roomLoadTransaction.CompleteStep("theme_xml");
			}
		}

		protected void RoomPlacementLoaded(ShsWebResponse response, object extraData)
		{
			if (roomLoadTransaction == extraData)
			{
				if (response.Status != 200)
				{
					roomLoadTransaction.FailStep("room_placement", "Response: " + response.Status);
				}
				else
				{
					try
					{
						string[] array = response.Body.Split('\n');
						for (int i = 0; i < array.Length; i++)
						{
							array[i] = array[i].Trim();
						}
						int j = 0;
						while (j < array.Length)
						{
							int num = j;
							string text = array[j++];
							if (j >= array.Length)
							{
								break;
							}
							j++;
							if (j >= array.Length)
							{
								break;
							}
							for (; j < array.Length && array[j].Length > 0; j++)
							{
							}
							string[] array2 = new string[j - num];
							for (int k = 0; k < array2.Length; k++)
							{
								array2[k] = array[num + k];
							}
							AppShell.Instance.SharedHashTable["HQ_" + text.ToLower()] = array2;
							for (; j < array.Length && array[j].Length == 0; j++)
							{
							}
						}
						AppShell.Instance.SharedHashTable["HQ_Placement"] = true;
						roomLoadTransaction.CompleteStep("room_placement");
					}
					catch (Exception message)
					{
						CspUtils.DebugLog(message);
						roomLoadTransaction.FailStep("room_placement", "Exception");
					}
				}
			}
		}

		protected void StartBundleLoad(string name, TransactionMonitor transactionMonitor)
		{
			if (transactionMonitor != null)
			{
				AssetBundle value = null;
				if (!Instance.bundles.TryGetValue(name, out value))
				{
					Instance.bundles.Add(name, null);
					transactionMonitor.AddStep(name);
					transactionMonitor.AddStepBundle(name, name);
					AppShell.Instance.BundleLoader.FetchAssetBundle(name, OnAssetBundleLoaded, transactionMonitor);
				}
			}
		}

		protected void StartCharacterDataLoad(string characterName, TransactionMonitor transactionMonitor)
		{
			if (transactionMonitor != null && !Instance.characterPrefabs.ContainsKey(characterName))
			{
				Instance.characterPrefabs.Add(characterName, null);
				transactionMonitor.AddStep(characterName);
				AppShell.Instance.DataManager.LoadGameData("Characters/" + characterName, OnCharacterDataLoaded, new CharacterLoadData(transactionMonitor, characterName));
			}
		}

		protected void OnCharacterDataLoaded(GameDataLoadResponse response, object extraData)
		{
			if (extraData == null || !(extraData is CharacterLoadData))
			{
				return;
			}
			CharacterLoadData characterLoadData = extraData as CharacterLoadData;
			TransactionMonitor transactionMonitor = characterLoadData.transactionMonitor;
			if (response.Error != null && response.Error != string.Empty)
			{
				CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
				transactionMonitor.Fail(response.Error);
				return;
			}
			string text = response.Data.TryGetString("//character_model/model_name", null);
			if (string.IsNullOrEmpty(text))
			{
				CspUtils.DebugLog("Could not get model name from game data.");
				transactionMonitor.FailStep(characterLoadData.characterName, "Could not get model name from game data.");
				return;
			}
			characterLoadData.modelName = text;
			string text2 = response.Data.TryGetString("//asset_bundle", null);
			if (!string.IsNullOrEmpty(text2))
			{
				StartCharacterBundleLoad(text2, characterLoadData);
			}
			transactionMonitor.CompleteStep(characterLoadData.characterName);
		}

		protected void StartCharacterBundleLoad(string name, CharacterLoadData loadData)
		{
			if (loadData.transactionMonitor != null)
			{
				loadData.transactionMonitor.AddStep(name);
				loadData.transactionMonitor.AddStepBundle(name, name);
				AppShell.Instance.BundleLoader.FetchAssetBundle(name, OnCharacterAssetBundleLoaded, loadData);
			}
		}

		protected void OnAssetBundleLoaded(AssetBundleLoadResponse response, object extraData)
		{
			if (extraData != null && extraData is TransactionMonitor)
			{
				TransactionMonitor transactionMonitor = extraData as TransactionMonitor;
				if ((response.Error != null && response.Error != string.Empty) || response.Bundle == null)
				{
					CspUtils.DebugLog("Failed to load: " + response.Path);
					transactionMonitor.Fail(response.Error);
				}
				else
				{
					Instance.bundles[response.Path] = response.Bundle;
					transactionMonitor.CompleteStep(response.Path);
				}
			}
		}

		protected void OnCharacterAssetBundleLoaded(AssetBundleLoadResponse response, object extraData)
		{
			if (extraData != null && extraData is CharacterLoadData)
			{
				CharacterLoadData characterLoadData = extraData as CharacterLoadData;
				TransactionMonitor transactionMonitor = characterLoadData.transactionMonitor;
				if (response.Error != null && response.Error != string.Empty)
				{
					CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
					transactionMonitor.Fail(response.Error);
				}
				else if (string.IsNullOrEmpty(characterLoadData.modelName))
				{
					CspUtils.DebugLog("No model name in character data.");
					transactionMonitor.FailStep(response.Path, "No model name in character data, cannot load prefab.");
				}
				else
				{
					transactionMonitor.AddStep(characterLoadData.modelName);
					AppShell.Instance.BundleLoader.LoadAsset(response.Path, characterLoadData.modelName, characterLoadData, OnCharacterPrefabLoaded);
					transactionMonitor.CompleteStep(response.Path);
				}
			}
		}

		protected void OnCharacterPrefabLoaded(UnityEngine.Object asset, AssetBundle bundle, object extraData)
		{
			if (extraData != null && extraData is CharacterLoadData)
			{
				CharacterLoadData characterLoadData = extraData as CharacterLoadData;
				TransactionMonitor transactionMonitor = characterLoadData.transactionMonitor;
				if (string.IsNullOrEmpty(characterLoadData.modelName))
				{
					CspUtils.DebugLog("No model name in character data.");
					transactionMonitor.Fail("No model name in character data, cannot load prefab.");
				}
				else if (string.IsNullOrEmpty(characterLoadData.characterName))
				{
					CspUtils.DebugLog("Could not get character name from data");
					transactionMonitor.FailStep(characterLoadData.modelName, "Could not get character name from data.");
				}
				else
				{
					Instance.characterPrefabs[characterLoadData.characterName] = (asset as GameObject);
					transactionMonitor.CompleteStep(characterLoadData.modelName);
				}
			}
		}

		protected void OnRoomLoadComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
		{
			roomLoadTransaction = null;
			if (startTransaction == null)
			{
				return;
			}
			if (exit != 0)
			{
				startTransaction.FailStep("bundles", error);
				return;
			}
			List<string> list = new List<string>();
			List<RoomStaticData> list2 = new List<RoomStaticData>();
			foreach (RoomStaticData room in Instance.staticData.rooms)
			{
				if (!(room.bundleName == string.Empty))
				{
					AvailableHQRoomsCollection availableHQRooms = Instance.Profile.AvailableHQRooms;
					if (availableHQRooms.ContainsKey(room.typeid))
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(Instance.bundles[room.bundleName].mainAsset, Vector3.zero, Quaternion.identity) as GameObject;
						HqRoom2 component = Utils.GetComponent<HqRoom2>(gameObject, Utils.SearchChildren);
						component.transform.parent = null;
						string[] array = AppShell.Instance.SharedHashTable["HQ_" + room.id] as string[];
						bool flag = false;
						if (array != null)
						{
							component.LoadSaveData(array);
							if (component.SavedItemIds != null)
							{
								foreach (string savedItemId in component.SavedItemIds)
								{
									Item value;
									Instance.Profile.AvailableItems.TryGetValue(savedItemId, out value);
									if (value != null)
									{
										if (!Instance.IsAssetBundleLoaded(value.Definition))
										{
											flag = true;
											if (!list.Contains(value.Definition.PlacedObjectAssetBundle))
											{
												list.Add(value.Definition.PlacedObjectAssetBundle);
											}
										}
									}
									else
									{
										CspUtils.DebugLog("Placed Item " + savedItemId + " was not found in the inventory.");
									}
								}
							}
						}
						component.Initialize(room);
						if (flag)
						{
							list2.Add(room);
						}
						else
						{
							component.LoadAI();
							component.LoadItems();
							component.Deactivate();
						}
						UnityEngine.Object.Destroy(gameObject);
						Instance.rooms.Add(room.id, component);
						Instance.roomIds.Add(room.id);
						Utils.ActivateTree(component.gameObject, false);
					}
					else
					{
						GameObject gameObject2 = new GameObject(room.id);
						HqUnpurchasedRoom hqUnpurchasedRoom = gameObject2.AddComponent<HqUnpurchasedRoom>();
						hqUnpurchasedRoom.roomCamera = gameObject2.AddComponent<CameraLite>();
						hqUnpurchasedRoom.transform.parent = null;
						hqUnpurchasedRoom.Initialize(room);
						Instance.rooms.Add(room.id, hqUnpurchasedRoom);
						Instance.roomIds.Add(room.id);
					}
				}
			}
			if (list.Count == 0)
			{
				initTransaction.CompleteStep("bundles");
				return;
			}
			itemLoadTransaction = TransactionMonitor.CreateTransactionMonitor("HQ Item Load Transaction", OnItemLoadComplete, float.MaxValue, list2);
			foreach (string item in list)
			{
				StartBundleLoad(item, itemLoadTransaction);
			}
			initTransaction.AddChild(itemLoadTransaction);
		}

		protected void OnItemLoadComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
		{
			itemLoadTransaction = null;
			if (userData is List<RoomStaticData>)
			{
				List<RoomStaticData> list = userData as List<RoomStaticData>;
				CspUtils.DebugLog("OnItemLoadComplete.");
				foreach (RoomStaticData item in list)
				{
					CspUtils.DebugLog("Initializing room " + item.id);
					HqRoom2 hqRoom = Instance.Rooms[item.id];
					if (hqRoom != null)
					{
						hqRoom.LoadAI();
						hqRoom.LoadItems();
						hqRoom.Deactivate();
					}
				}
			}
			initTransaction.CompleteStep("bundles");
		}

		protected void OnStartComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
		{
			CspUtils.DebugLog("HqControllerLoad.OnStartComplete() called when HQController's StartTransaction fulfilled");
			startTransaction = null;
			if (exit != 0)
			{
				CspUtils.DebugLog("HQ initialization failed: " + error);
				Instance.Abort();
				AppShell.Instance.CriticalError(SHSErrorCodes.Code.CantEnterHQ, error);
				return;
			}
			foreach (RoomStaticData room in Instance.staticData.rooms)
			{
				if (Instance.Profile.AvailableHQRooms.ContainsKey(room.typeid))
				{
					Instance.SetActiveRoom(Instance.Rooms[room.id]);
					Instance.fsm.GotoState<HqControllerIntro>();
					return;
				}
			}
			Instance.ResetActiveRoom();
			Instance.ControllerReady();
			Instance.StartTransaction.CompleteStep("controllerReady");
			AppShell.Instance.EventMgr.Fire(this, new SHSHQWindow.DisplayHQUIMessage());
			Instance.fsm.GotoState<HqControllerFlinga>();
		}
	}

	internal class HqControllerPlacement : IShsState
	{
		public void Enter(Type previousState)
		{
			Instance.ActiveRoom.GotoPlacementMode();
			Instance.hqInput.GotoPlacementMode();
			Instance.SetAIPauseState(true);
			AppShell.Instance.EventMgr.Fire(Instance, new HQModeChanged(GetType()));
		}

		public void Update()
		{
			if (SHSInput.GetKeyDown(KeyCode.R))
			{
				Instance.NextActiveRoom();
			}
		}

		public void Leave(Type nextState)
		{
		}
	}

	internal class HqTutorialLoad : IDisposable, IShsState
	{
		protected class CharacterLoadData
		{
			public TransactionMonitor transactionMonitor;

			public string characterName;

			public string modelName;

			public CharacterLoadData(TransactionMonitor monitor, string characterName)
			{
				transactionMonitor = monitor;
				this.characterName = characterName;
			}
		}

		protected HqControllerTutorial owner;

		protected TransactionMonitor sharedBundleLoadTransaction;

		protected HqRoom2 currentRoom;

		protected bool spawnedAI;

		public HqTutorialLoad(HqControllerTutorial owner)
		{
			this.owner = owner;
		}

		public void Enter(Type previousState)
		{
			Instance.staticData = new HqStaticData();
			sharedBundleLoadTransaction = TransactionMonitor.CreateTransactionMonitor("HQ Room Load Transaction", OnStartComplete, float.MaxValue, null);
			StartBundleLoad("HQ/hq_shared", sharedBundleLoadTransaction);
			StartBundleLoad("HQ/bridge", sharedBundleLoadTransaction);
			StartBundleLoad("HQ/dorm1", sharedBundleLoadTransaction);
			StartBundleLoad("HQ/Items/Bridge_Props", sharedBundleLoadTransaction);
			if (AppShell.Instance != null)
			{
				sharedBundleLoadTransaction.AddStep("theme_xml", "Theme XML Loaded");
				AppShell.Instance.DataManager.LoadGameData("HQ/Themes", ThemeDataLoaded, new HqStaticData(), sharedBundleLoadTransaction);
			}
			StartCharacterDataLoad("falcon", sharedBundleLoadTransaction);
			StartCharacterDataLoad("cyclops", sharedBundleLoadTransaction);
			StartBundleLoad("HQ/hq_shared", sharedBundleLoadTransaction);
		}

		public void Dispose()
		{
			sharedBundleLoadTransaction = null;
		}

		protected void ThemeDataLoaded(GameDataLoadResponse response, object extraData)
		{
			if (response.DataDefinition == null)
			{
				sharedBundleLoadTransaction.FailStep("theme_xml", "File <" + response.Path + "> failed to load");
				return;
			}
			HqStaticData hqStaticData = response.DataDefinition as HqStaticData;
			if (hqStaticData != null)
			{
				Instance.staticData.defaultTheme = hqStaticData.defaultTheme;
				Instance.staticData.colors = hqStaticData.colors;
				Instance.staticData.room_themes = hqStaticData.room_themes;
				Instance.staticData.item_themes = hqStaticData.item_themes;
			}
			sharedBundleLoadTransaction.CompleteStep("theme_xml");
		}

		protected void StartBundleLoad(string name, TransactionMonitor transactionMonitor)
		{
			if (transactionMonitor != null)
			{
				AssetBundle value = null;
				if (!Instance.bundles.TryGetValue(name, out value))
				{
					Instance.bundles.Add(name, null);
					transactionMonitor.AddStep(name);
					transactionMonitor.AddStepBundle(name, name);
					AppShell.Instance.BundleLoader.FetchAssetBundle(name, OnAssetBundleLoaded, transactionMonitor);
				}
			}
		}

		protected void StartCharacterDataLoad(string characterName, TransactionMonitor transactionMonitor)
		{
			if (transactionMonitor != null && !Instance.characterPrefabs.ContainsKey(characterName))
			{
				Instance.characterPrefabs.Add(characterName, null);
				transactionMonitor.AddStep(characterName);
				AppShell.Instance.DataManager.LoadGameData("Characters/" + characterName, OnCharacterDataLoaded, new CharacterLoadData(transactionMonitor, characterName));
			}
		}

		protected void OnCharacterDataLoaded(GameDataLoadResponse response, object extraData)
		{
			if (extraData == null || !(extraData is CharacterLoadData))
			{
				return;
			}
			CharacterLoadData characterLoadData = extraData as CharacterLoadData;
			TransactionMonitor transactionMonitor = characterLoadData.transactionMonitor;
			if (response.Error != null && response.Error != string.Empty)
			{
				CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
				transactionMonitor.Fail(response.Error);
				return;
			}
			string text = response.Data.TryGetString("//character_model/model_name", null);
			if (string.IsNullOrEmpty(text))
			{
				CspUtils.DebugLog("Could not get model name from game data.");
				transactionMonitor.FailStep(characterLoadData.characterName, "Could not get model name from game data.");
				return;
			}
			characterLoadData.modelName = text;
			string text2 = response.Data.TryGetString("//asset_bundle", null);
			if (!string.IsNullOrEmpty(text2))
			{
				StartCharacterBundleLoad(text2, characterLoadData);
			}
			transactionMonitor.CompleteStep(characterLoadData.characterName);
		}

		protected void StartCharacterBundleLoad(string name, CharacterLoadData loadData)
		{
			if (loadData.transactionMonitor != null)
			{
				loadData.transactionMonitor.AddStep(name);
				loadData.transactionMonitor.AddStepBundle(name, name);
				AppShell.Instance.BundleLoader.FetchAssetBundle(name, OnCharacterAssetBundleLoaded, loadData);
			}
		}

		protected void OnAssetBundleLoaded(AssetBundleLoadResponse response, object extraData)
		{
			if (extraData != null && extraData is TransactionMonitor)
			{
				TransactionMonitor transactionMonitor = extraData as TransactionMonitor;
				if ((response.Error != null && response.Error != string.Empty) || response.Bundle == null)
				{
					CspUtils.DebugLog("Failed to load: " + response.Path);
					transactionMonitor.Fail(response.Error);
				}
				else
				{
					CspUtils.DebugLog("Loaded asset bundle " + response.Path);
					Instance.bundles[response.Path] = response.Bundle;
					transactionMonitor.CompleteStep(response.Path);
				}
			}
		}

		protected void OnCharacterAssetBundleLoaded(AssetBundleLoadResponse response, object extraData)
		{
			if (extraData != null && extraData is CharacterLoadData)
			{
				CharacterLoadData characterLoadData = extraData as CharacterLoadData;
				TransactionMonitor transactionMonitor = characterLoadData.transactionMonitor;
				if (response.Error != null && response.Error != string.Empty)
				{
					CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
					transactionMonitor.Fail(response.Error);
				}
				else if (string.IsNullOrEmpty(characterLoadData.modelName))
				{
					CspUtils.DebugLog("No model name in character data.");
					transactionMonitor.FailStep(response.Path, "No model name in character data, cannot load prefab.");
				}
				else
				{
					transactionMonitor.AddStep(characterLoadData.modelName);
					AppShell.Instance.BundleLoader.LoadAsset(response.Path, characterLoadData.modelName, characterLoadData, OnCharacterPrefabLoaded);
					transactionMonitor.CompleteStep(response.Path);
				}
			}
		}

		protected void OnCharacterPrefabLoaded(UnityEngine.Object asset, AssetBundle bundle, object extraData)
		{
			if (extraData != null && extraData is CharacterLoadData)
			{
				CharacterLoadData characterLoadData = extraData as CharacterLoadData;
				TransactionMonitor transactionMonitor = characterLoadData.transactionMonitor;
				if (string.IsNullOrEmpty(characterLoadData.modelName))
				{
					CspUtils.DebugLog("No model name in character data.");
					transactionMonitor.Fail("No model name in character data, cannot load prefab.");
				}
				else if (string.IsNullOrEmpty(characterLoadData.characterName))
				{
					CspUtils.DebugLog("Could not get character name from data");
					transactionMonitor.FailStep(characterLoadData.modelName, "Could not get character name from data.");
				}
				else
				{
					Instance.characterPrefabs[characterLoadData.characterName] = (asset as GameObject);
					transactionMonitor.CompleteStep(characterLoadData.modelName);
				}
			}
		}

		protected void OnStartComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
		{
			CspUtils.DebugLog("OnStartComplete enter");
			sharedBundleLoadTransaction = null;
			if (exit == TransactionMonitor.ExitCondition.Success)
			{
				if (Instance != null)
				{
					RoomStaticData roomStaticData = new RoomStaticData();
					roomStaticData.id = "bridge";
					roomStaticData.item_cap = 10;
					currentRoom = InitializeRoom("HQ/bridge", roomStaticData);
					owner.Bridge = currentRoom;
					roomStaticData.id = "dorm1";
					roomStaticData.item_cap = 40;
					owner.Dorm1 = InitializeRoom("HQ/dorm1", roomStaticData);
					owner.Dorm1.RoomState = HqRoom2.AccessState.Locked;
				}
				Instance.SetActiveRoom(currentRoom);
				currentRoom.GotoFlingaMode();
			}
			Vector3 randomDoor = Instance.ActiveRoom.RandomDoor;
			GameObject gameObject = HqAIProxy.CreateProxy("falcon");
			gameObject.transform.parent = Instance.ActiveRoom.transform;
			gameObject.transform.position = randomDoor;
			HqAIProxy component = Utils.GetComponent<HqAIProxy>(gameObject);
			if (component != null)
			{
				component.Spawn("falcon", randomDoor, Instance.ActiveRoom);
				Instance.AddProxy(component);
			}
			spawnedAI = true;
			owner.StartTransaction.CompleteStep("secondaryInitializeStep");
			Instance.ActiveRoom.RoomState = HqRoom2.AccessState.Locked;
			CspUtils.DebugLog("OnStartComplete exit");
		}

		protected HqRoom2 InitializeRoom(string prefabName, RoomStaticData dummyData)
		{
			HqRoom2 hqRoom = null;
			GameObject gameObject = UnityEngine.Object.Instantiate(Instance.bundles[prefabName].mainAsset, Vector3.zero, Quaternion.identity) as GameObject;
			if (gameObject != null)
			{
				hqRoom = Utils.GetComponent<HqRoom2>(gameObject, Utils.SearchChildren);
				if (hqRoom != null)
				{
					hqRoom.Initialize(dummyData);
					Instance.rooms.Add(dummyData.id, hqRoom);
					Instance.roomIds.Add(dummyData.id);
				}
			}
			return hqRoom;
		}

		public void Update()
		{
			if (!spawnedAI || owner.HasProxiesLoading)
			{
				return;
			}
			if (Instance.isTestScene)
			{
				if (AppShell.Instance.LaunchTransitionTransaction != null)
				{
					AppShell.Instance.LaunchTransitionTransaction.CompleteStep("initialize");
				}
				if (AppShell.Instance.LaunchLoginTransaction != null)
				{
					AppShell.Instance.LaunchLoginTransaction.CompleteStep("initialize");
				}
			}
			Instance.ControllerReady();
			owner.GoToPlayMode();
		}

		public void Leave(Type nextState)
		{
		}
	}

	internal class HqTutorialPause : IShsState
	{
		public void Enter(Type previousState)
		{
			Instance.ActiveRoom.GotoViewMode();
			Instance.SetAIPauseState(true);
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	internal class HqTutorialPlay : IShsState
	{
		public void Enter(Type previousState)
		{
			Instance.ActiveRoom.GotoFlingaMode();
			Instance.SetAIPauseState(false);
			AppShell.Instance.EventMgr.Fire(Instance, new HQModeChanged(GetType()));
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	protected const string STR_ASSIGNROOM_PROMPT = "#hq_assign_room_prompt";

	protected const string STR_ITEM_CAP = "#hq_item_cap_reached";

	public Vector3 gravity = new Vector3(0f, -9.8f, 0f);

	public GameObject[] TempPrefabList;

	public static string HQ_USER_ID_KEY = "HQ_HqUserId";

	public float initial_tracking_time = 0.25f;

	public float initial_pullback_time = 1.5f;

	public int minimumAI = 5;

	public GameObject TransporterFX;

	protected static HqController2 instance;

	public ShsFSM fsm;

	protected HqStaticData staticData;

	protected Dictionary<string, AssetBundle> bundles;

	protected Dictionary<string, GameObject> characterPrefabs;

	protected HqRoom2 activeRoom;

	protected Dictionary<string, HqRoom2> rooms;

	protected List<string> roomIds;

	protected int roomCycle;

	protected int themeCycle = -1;

	protected GameObject spawnFX;

	protected List<AIControllerHQ> AIControllers;

	protected bool resetHitWhileSpawning;

	protected List<HqAIProxy> activeProxies = new List<HqAIProxy>();

	protected HqInput hqInput;

	protected Vector3 oldGravity;

	protected UserProfile profile;

	protected GUIDrawTexture loadScreen;

	protected bool returnToFlingaOnSceneEnable;

	[CompilerGenerated]
	private ShsAudioSourceList _003CSounds_003Ek__BackingField;

	public HqRoom2 ActiveRoom
	{
		get
		{
			return activeRoom;
		}
	}

	public Dictionary<string, HqRoom2> Rooms
	{
		get
		{
			return rooms;
		}
	}

	public static HqController2 Instance
	{
		get
		{
			return instance;
		}
	}

	public List<Color> ThemeBaseColors
	{
		get
		{
			return staticData.colors;
		}
	}

	public string DefaultThemeName
	{
		get
		{
			return staticData.defaultTheme;
		}
	}

	public Type State
	{
		get
		{
			return fsm.GetCurrentState();
		}
	}

	public HqInput Input
	{
		get
		{
			return hqInput;
		}
	}

	public UserProfile Profile
	{
		get
		{
			return profile;
		}
	}

	public bool Visiting
	{
		get
		{
			return Profile != AppShell.Instance.Profile;
		}
	}

	public bool HasProxiesLoading
	{
		get
		{
			return activeProxies != null && activeProxies.Count > 0;
		}
	}

	public GUIDrawTexture LoadScreen
	{
		get
		{
			return loadScreen;
		}
	}

	public ShsAudioSourceList Sounds
	{
		[CompilerGenerated]
		get
		{
			return _003CSounds_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CSounds_003Ek__BackingField = value;
		}
	}

	protected virtual GUITopLevelWindow MainWindow
	{
		get
		{
			return (GUITopLevelWindow)GUIManager.Instance["SHSMainWindow/SHSHQWindow"];
		}
	}

	public override void Awake()
	{
		base.Awake();
		if (instance != null)
		{
			CspUtils.DebugLog("A second HQController is being created.  This may lead to instabilities!");
		}
		else
		{
			instance = this;
		}
		bCallControllerReadyFromStart = false;
		AppShell.Instance.EventMgr.AddListener<NewControllerReadyMessage>(OnNewControllerReady);
	}

	public override void OnEnable()
	{
		base.OnEnable();
		oldGravity = Physics.gravity;
		Physics.gravity = gravity;
		AppShell.Instance.EventMgr.AddListener<HQRoomChangeRequestMessage>(onRoomChangeRequest);
		AppShell.Instance.EventMgr.AddListener<HQRoomRespawnMessage>(onObjectRespawned);
		AppShell.Instance.EventMgr.AddListener<HQRoomStateChangeMessage>(onRoomStateChangedMessage);
		AppShell.Instance.EventMgr.AddListener<ShoppingItemPurchasedMessage>(onItemPurchasedMessage);
	}

	public override void OnDisable()
	{
		base.OnDisable();
		Physics.gravity = oldGravity;
		AppShell.Instance.EventMgr.RemoveListener<NewControllerReadyMessage>(OnNewControllerReady);
		AppShell.Instance.EventMgr.RemoveListener<HQRoomChangeRequestMessage>(onRoomChangeRequest);
		AppShell.Instance.EventMgr.RemoveListener<HQRoomRespawnMessage>(onObjectRespawned);
		AppShell.Instance.EventMgr.RemoveListener<HQRoomStateChangeMessage>(onRoomStateChangedMessage);
		AppShell.Instance.EventMgr.RemoveListener<ShoppingItemPurchasedMessage>(onItemPurchasedMessage);
		Abort();
	}

	public override void Start()
	{
		base.Start();
		AppShell.Instance.Matchmaker2.SoloHQ();
		rooms = new Dictionary<string, HqRoom2>();
		roomIds = new List<string>();
		bundles = new Dictionary<string, AssetBundle>();
		characterPrefabs = new Dictionary<string, GameObject>();
		AIControllers = new List<AIControllerHQ>();
		Sounds = ShsAudioSourceList.GetList("HQ");
		fsm = new ShsFSM();
		fsm.AddState(new HqControllerLoad(this));
		fsm.AddState(new HqControllerIntro());
		fsm.AddState(new HqControllerView());
		fsm.AddState(new HqControllerPlacement());
		fsm.AddState(new HqControllerFlinga());
		profile = null;
		if (AppShell.Instance.SharedHashTable[HQ_USER_ID_KEY] != null)
		{
			int num = (int)AppShell.Instance.SharedHashTable[HQ_USER_ID_KEY];
			AppShell.Instance.SharedHashTable.Remove(HQ_USER_ID_KEY);
			AppShell.Instance.WebService.StartRequest("resources$users/hq.py", OnProfileWebResponse);
		}
		else
		{
			profile = AppShell.Instance.Profile;
			fsm.GotoState<HqControllerLoad>();
		}
		hqInput = Utils.GetComponent<HqInput>(base.gameObject);
		if (hqInput == null)
		{
			CspUtils.DebugLog("HqController2 can not find HqInput");
		}
	}

	public virtual void Update()
	{
		foreach (AIControllerHQ aIController in AIControllers)
		{
			if (aIController.Mode == AIControllerHQ.AiMode.Sim)
			{
				aIController.Sim();
			}
		}
		for (int num = activeProxies.Count - 1; num >= 0; num--)
		{
			if (activeProxies[num].SpawnCompleted)
			{
				if (activeProxies[num].SpawnSucceeded)
				{
					if (activeProxies[num].CanSwap)
					{
						activeProxies[num].Swap();
						activeProxies.RemoveAt(num);
					}
				}
				else
				{
					HeroPersisted value = null;
					if (profile.AvailableCostumes.TryGetValue(activeProxies[num].characterName, out value))
					{
						value.Placed = false;
					}
					activeProxies[num].DestroyProxy();
					activeProxies.RemoveAt(num);
				}
			}
		}
		if (fsm != null)
		{
			fsm.Update();
		}
	}

	public override void OnOldControllerUnloading(AppShell.GameControllerTypeData currentGameData, AppShell.GameControllerTypeData newGameData)
	{
		if (hqInput != null)
		{
			hqInput.OnLeavingHQ();
		}
		AppShell.Instance.SharedHashTable.Remove("HQ_Placement");
		foreach (HqRoom2 value in rooms.Values)
		{
			if (AppShell.Instance.SharedHashTable.ContainsKey("HQ_" + value.Id))
			{
				AppShell.Instance.SharedHashTable.Remove("HQ_" + value.Id);
			}
		}
		foreach (AIControllerHQ aIController in AIControllers)
		{
			aIController.SaveHungerValue();
		}
		AppShell.Instance.Profile.UnplaceAllItemsAndHeros();
		base.OnOldControllerUnloading(currentGameData, newGameData);
	}

	public GameObject GetTempObjectPrefab(int idx)
	{
		return TempPrefabList[idx];
	}

	public void GotoViewMode()
	{
		if (fsm.GetCurrentState() != typeof(HqControllerLoad))
		{
			fsm.GotoState<HqControllerView>();
		}
	}

	public void GotoPlacementMode()
	{
		if (fsm.GetCurrentState() != typeof(HqControllerLoad) && fsm.GetCurrentState() != typeof(HqControllerPlacement))
		{
			fsm.GotoState<HqControllerPlacement>();
		}
	}

	public void GotoFlingaMode()
	{
		if (fsm.GetCurrentState() != typeof(HqControllerLoad) && fsm.GetCurrentState() != typeof(HqControllerFlinga))
		{
			fsm.GotoState<HqControllerFlinga>();
		}
	}

	public void GoToNextTheme()
	{
		ActiveRoom.ApplyTheme(NextThemeName(), true);
	}

	public bool ThemeExists(string name)
	{
		return staticData.room_themes.Exists(delegate(ThemeStaticData t)
		{
			return t.name == name;
		});
	}

	public bool GetRoomThemeFromName(string name, out List<Color> replacements)
	{
		foreach (ThemeStaticData room_theme in staticData.room_themes)
		{
			if (room_theme.name == name)
			{
				replacements = room_theme.colors;
				return true;
			}
		}
		replacements = null;
		return false;
	}

	public bool GetItemThemeFromHeroName(string heroName, out List<Color> replacements)
	{
		foreach (ThemeStaticData item_theme in staticData.item_themes)
		{
			if (item_theme.hero == heroName)
			{
				replacements = item_theme.colors;
				return true;
			}
		}
		replacements = null;
		return false;
	}

	public ThemeStaticData GetHeroTheme(string hero_name)
	{
		foreach (ThemeStaticData room_theme in staticData.room_themes)
		{
			if (room_theme.hero == hero_name)
			{
				return room_theme;
			}
		}
		return null;
	}

	public string GetHeroThemeName(string hero_name)
	{
		foreach (ThemeStaticData room_theme in staticData.room_themes)
		{
			if (room_theme.hero == hero_name)
			{
				return room_theme.name;
			}
		}
		return null;
	}

	public AssetBundle GetAssetBundle(string name)
	{
		AssetBundle value = null;
		if (bundles.TryGetValue(name, out value))
		{
			return value;
		}
		CspUtils.DebugLog("Could not find asset bundle " + name);
		return null;
	}

	public bool IsAssetBundleLoaded(ItemDefinition itemDef)
	{
		return bundles.ContainsKey(itemDef.PlacedObjectAssetBundle);
	}

	public GameObject GetCharacterPrefab(string character)
	{
		if (characterPrefabs.ContainsKey(character))
		{
			return characterPrefabs[character];
		}
		return null;
	}

	public RoomStaticData GetRoomData(string id)
	{
		foreach (RoomStaticData room in staticData.rooms)
		{
			if (room.id == id)
			{
				return room;
			}
		}
		return null;
	}

	public void AddAssetBundle(string name, AssetBundle bundle)
	{
		if (!bundles.ContainsKey(name))
		{
			bundles[name] = bundle;
		}
	}

	public void SwitchRooms(HqRoom2 outRoom, HqRoom2 inRoom)
	{
		if (rooms.ContainsKey(outRoom.Id))
		{
			rooms[outRoom.Id] = inRoom;
		}
	}

	public void ReDropAI(string characterName, Vector3 position, HqRoom2 room)
	{
		CheckRoomAssignment(characterName, room);
		foreach (AIControllerHQ aIController in AIControllers)
		{
			if (aIController.gameObject.name == characterName)
			{
				if (aIController.SpawnRoom != room)
				{
					aIController.SpawnRoom.DespawnAI(aIController);
					room.SpawnAI(aIController);
				}
				aIController.ChangeBehavior<BehaviorPaused>(true);
				aIController.CurrentRoom = room;
				aIController.gameObject.transform.position = position;
				break;
			}
		}
	}

	public void DespawnAI(AIControllerHQ ai)
	{
		if (ai.SpawnRoom != null)
		{
			ai.SpawnRoom.DespawnAI(ai);
		}
		if (AIControllers.Contains(ai))
		{
			AIControllers.Remove(ai);
		}
	}

	public void Reset()
	{
		List<string> list = new List<string>();
		for (int num = AIControllers.Count - 1; num >= 0; num--)
		{
			if (AIControllers[num].CurrentRoom == ActiveRoom)
			{
				list.Add(AIControllers[num].gameObject.name);
				AIControllers[num].Despawn();
			}
		}
		if (list.Count > 0)
		{
			AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(list.ToArray(), CollectionResetMessage.ActionType.Add, "Heroes"));
		}
		if (ActiveRoom != null)
		{
			ActiveRoom.Reset();
		}
	}

	public void SetAIPauseState(bool paused)
	{
		foreach (AIControllerHQ aIController in AIControllers)
		{
			aIController.Paused = paused;
		}
	}

	public void AIIsTakingOverItem(HqObject2 obj)
	{
		if (obj.State == typeof(HqObject2.HqObjectFlingaSelected))
		{
			obj.GotoFlingaMode();
			hqInput.GotoFlingaMode();
		}
	}

	public bool IsAISpawned(string characterName)
	{
		foreach (AIControllerHQ aIController in AIControllers)
		{
			if (aIController.gameObject.name == characterName)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsSpawning(string characterName)
	{
		foreach (HqAIProxy activeProxy in activeProxies)
		{
			if (activeProxy.characterName == characterName)
			{
				return true;
			}
		}
		return false;
	}

	public GameObject GetAI(string characterName)
	{
		foreach (AIControllerHQ aIController in AIControllers)
		{
			if (aIController == null || aIController.gameObject == null)
			{
				CspUtils.DebugLog("AIControllers has a null ai somehow.");
			}
			else if (aIController.gameObject.name == characterName)
			{
				return aIController.gameObject;
			}
		}
		return null;
	}

	public AIControllerHQ GetAIController(string characterName)
	{
		foreach (AIControllerHQ aIController in AIControllers)
		{
			if (aIController.CharacterName == characterName)
			{
				return aIController;
			}
		}
		return null;
	}

	public PlacedItem FindPlacedItem(Item inventoryItem)
	{
		foreach (HqRoom2 value in rooms.Values)
		{
			foreach (HqItem placedItem2 in value.PlacedItems)
			{
				PlacedItem placedItem = placedItem2 as PlacedItem;
				if (placedItem != null && placedItem.InventoryItem == inventoryItem)
				{
					return placedItem;
				}
			}
		}
		return null;
	}

	public int PlacedCount(Item inventoryItem)
	{
		int num = 0;
		foreach (HqRoom2 value in rooms.Values)
		{
			foreach (HqItem placedItem2 in value.PlacedItems)
			{
				PlacedItem placedItem = placedItem2 as PlacedItem;
				if (placedItem != null && placedItem.InventoryItem.Id == inventoryItem.Id)
				{
					num++;
				}
			}
		}
		return num;
	}

	public bool IsHeroPlaced(HeroPersisted hero)
	{
		return GetAIController(hero.Name) != null;
	}

	public void SetCollisionOnAIController(AIControllerHQ aiController)
	{
		foreach (AIControllerHQ aIController in AIControllers)
		{
			if (aIController.gameObject.active && aIController != aiController)
			{
				Physics.IgnoreCollision(aiController.gameObject.collider, aIController.gameObject.collider);
			}
		}
	}

	public void SetActiveRoom(HqRoom2 newRoom)
	{
		if (activeRoom != null)
		{
			hqInput.DeactiveRoom();
			activeRoom.Deactivate();
			activeRoom = null;
		}
		activeRoom = newRoom;
		activeRoom.Activate();
		hqInput.ActivateRoom();
		if (spawnFX != null)
		{
			Utils.ActivateTree(spawnFX, false);
		}
		AppShell.Instance.EventMgr.Fire(this, new HQRoomChangedMessage(activeRoom.Id));
		roomCycle = roomIds.IndexOf(newRoom.Id);
		Type currentState = fsm.GetCurrentState();
		if (currentState == typeof(HqControllerFlinga))
		{
			activeRoom.GotoFlingaMode();
			Instance.hqInput.GotoFlingaMode();
			Instance.SetAIPauseState(false);
		}
		else if (currentState == typeof(HqControllerPlacement))
		{
			activeRoom.GotoPlacementMode();
			Instance.hqInput.GotoPlacementMode();
			Instance.SetAIPauseState(true);
		}
		else if (currentState == typeof(HqControllerView))
		{
			activeRoom.GotoViewMode();
			Instance.hqInput.GotoViewMode();
			Instance.SetAIPauseState(true);
		}
	}

	public void CheckRoomAssignment(string characterName, HqRoom2 room)
	{
		HqAssignableRoom assignRoom = room as HqAssignableRoom;
		if (assignRoom != null && assignRoom.IsInAssignmentMode && assignRoom.Owner != characterName)
		{
			GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkCancelDialog, "#hq_assign_room_prompt", delegate(string Id, GUIDialogWindow.DialogState state)
			{
				if (state == GUIDialogWindow.DialogState.Ok)
				{
					assignRoom.AssignRoomTo(characterName);
				}
			}, GUIControl.ModalLevelEnum.Default);
		}
	}

	public void AddProxy(HqAIProxy proxy)
	{
		activeProxies.Add(proxy);
	}

	public void Save()
	{
		if (!Visiting)
		{
			ActiveRoom.Save();
		}
	}

	public void Load()
	{
		if (!Visiting)
		{
			ActiveRoom.RevertToLastSave();
		}
	}

	public void ResetShadows()
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindSceneObjectsOfType(typeof(Light));
		if (GraphicsOptions.Shadows == GraphicsOptions.ShadowLevel.None)
		{
			UnityEngine.Object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Light light = (Light)array2[i];
				if (light.type == LightType.Directional)
				{
					light.shadows = LightShadows.None;
				}
			}
			return;
		}
		UnityEngine.Object[] array3 = array;
		for (int j = 0; j < array3.Length; j++)
		{
			Light light2 = (Light)array3[j];
			if (light2.type == LightType.Directional)
			{
				light2.shadows = LightShadows.Soft;
			}
		}
	}

	public bool CanDrag(DragDropInfo dragDropInfo)
	{
		if (dragDropInfo.CollectionId == DragDropInfo.CollectionType.Items && Instance.ActiveRoom.ItemCapReached)
		{
			GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, "#hq_item_cap_reached", delegate
			{
			}, GUIControl.ModalLevelEnum.Default);
			return false;
		}
		return true;
	}

	public Rigidbody GetRigidbody(GameObject go)
	{
		while (go != null)
		{
			if (go.rigidbody != null)
			{
				return go.rigidbody;
			}
			if (go.transform.parent == null)
			{
				break;
			}
			go = go.transform.parent.gameObject;
		}
		return null;
	}

	public override void EnableSceneRendering()
	{
		base.EnableSceneRendering();
		if (returnToFlingaOnSceneEnable)
		{
			GotoFlingaMode();
		}
	}

	public override void DisableSceneRendering()
	{
		base.DisableSceneRendering();
		returnToFlingaOnSceneEnable = (State == typeof(HqControllerFlinga));
		if (returnToFlingaOnSceneEnable)
		{
			GotoViewMode();
		}
	}

	public virtual bool IsInPlayMode()
	{
		return State == typeof(HqControllerFlinga);
	}

	public bool IsAllowableCharacter(string CharacterName)
	{
		if (profile == null)
		{
			CspUtils.DebugLog("Could not get UserProfile");
			return false;
		}
		HeroPersisted value = null;
		if (!profile.AvailableCostumes.TryGetValue(CharacterName, out value))
		{
			CspUtils.DebugLog("Could not get HeroPersisted for " + CharacterName);
			return false;
		}
		if (value.ShieldAgentOnly && !Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldHeroesAllow))
		{
			CspUtils.DebugLog("Could not place shield only hero " + CharacterName + ". You do not have shield heroes entitlement.");
			return false;
		}
		return true;
	}

	public void Despawn(string characterName)
	{
		foreach (AIControllerHQ aIController in AIControllers)
		{
			if (aIController.gameObject.name.ToLower() == characterName.ToLower())
			{
				AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(new string[1]
				{
					aIController.name
				}, CollectionResetMessage.ActionType.Add, "Heroes"));
				aIController.Despawn();
				break;
			}
		}
	}

	public void DespawnAllAI()
	{
		List<string> list = new List<string>();
		for (int num = AIControllers.Count - 1; num >= 0; num--)
		{
			list.Add(AIControllers[num].name);
			AIControllers[num].Despawn();
		}
		AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(list.ToArray(), CollectionResetMessage.ActionType.Add, "Heroes"));
		AIControllers.Clear();
	}

	public void DespawnAIInCurrentRoom()
	{
		List<string> list = new List<string>();
		for (int num = AIControllers.Count - 1; num >= 0; num--)
		{
			if (AIControllers[num].CurrentRoom == ActiveRoom)
			{
				list.Add(AIControllers[num].name);
				AIControllers[num].Despawn();
			}
		}
		AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(list.ToArray(), CollectionResetMessage.ActionType.Add, "Heroes"));
	}

	public string GoTo(string characterName)
	{
		foreach (AIControllerHQ aIController in AIControllers)
		{
			if (!(aIController == null) && !(aIController.gameObject == null) && aIController.name.ToLower() == characterName.ToLower())
			{
				if (aIController.CurrentRoom != ActiveRoom)
				{
					hqInput.SetHeroCamera(aIController.gameObject.name);
					SetActiveRoom(aIController.CurrentRoom);
					for (int i = 0; i < roomIds.Count; i++)
					{
						if (rooms[roomIds[i]] == aIController.CurrentRoom)
						{
							return roomIds[i];
						}
					}
				}
				else
				{
					hqInput.SetHeroCamera(aIController, 1f);
				}
			}
		}
		return string.Empty;
	}

	protected void Abort()
	{
		AIControllers.Clear();
		if (fsm != null)
		{
			fsm.ClearState();
			fsm.Dispose();
			fsm = null;
		}
	}

	protected void ResetActiveRoom()
	{
		roomCycle = 0;
		SetActiveRoom(rooms[roomIds[roomCycle]]);
	}

	protected void NextActiveRoom()
	{
		roomCycle++;
		if (roomCycle >= roomIds.Count)
		{
			roomCycle = 0;
		}
		SetActiveRoom(rooms[roomIds[roomCycle]]);
	}

	protected void PrevActiveRoom()
	{
		roomCycle--;
		if (roomCycle < 0)
		{
			roomCycle = rooms.Count - 1;
		}
		SetActiveRoom(rooms[roomIds[roomCycle]]);
	}

	protected string NextThemeName()
	{
		themeCycle++;
		if (themeCycle >= staticData.room_themes.Count)
		{
			themeCycle = 0;
		}
		return staticData.room_themes[themeCycle].name;
	}

	public void onEntitySpawned(GameObject obj)
	{
		AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(obj);
		if (component != null)
		{
			if (resetHitWhileSpawning)
			{
				AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(new string[1]
				{
					component.gameObject.name
				}, CollectionResetMessage.ActionType.Add, "Heroes"));
				component.Despawn();
				resetHitWhileSpawning = false;
				return;
			}
			foreach (AIControllerHQ aIController in AIControllers)
			{
				if (aIController.CharacterName == component.CharacterName)
				{
					CspUtils.DebugLog("Trying to spawn multiple instances of one character: " + aIController.CharacterName);
					component.Despawn();
					return;
				}
			}
			if (obj.active)
			{
				SetCollisionOnAIController(component);
			}
			AIControllers.Add(component);
		}
		else
		{
			CspUtils.DebugLog("Could not find AIControllerHQ");
		}
	}

	protected void onRoomStateChangedMessage(HQRoomStateChangeMessage message)
	{
		HqRoom2 value = null;
		if (rooms.TryGetValue(message.roomName, out value))
		{
			value.RoomState = message.state;
		}
	}

	protected virtual void OnNewControllerReady(NewControllerReadyMessage readyMessage)
	{
		AddUIElements();
	}

	private void AddUIElements()
	{
		AddLoadScreen();
	}

	private void AddLoadScreen()
	{
		if (MainWindow != null && loadScreen == null)
		{
			loadScreen = new GUIDrawTexture();
			loadScreen.SetPositionAndSize(GUIControl.DockingAlignmentEnum.Middle, GUIControl.AnchorAlignmentEnum.Middle, GUIControl.OffsetType.Absolute, Vector2.zero, new Vector2(1022f, 638f), GUIControl.AutoSizeTypeEnum.Absolute, GUIControl.AutoSizeTypeEnum.Absolute);
			loadScreen.Rotation = 0f;
			MainWindow.Add(loadScreen, GUIControl.DrawOrder.DrawLast);
			loadScreen.IsVisible = false;
		}
	}

	protected void OnProfileWebResponse(ShsWebResponse response)
	{
		if (response.Status == 200)
		{
			CspUtils.DebugLog("User Profile Found <" + response.Status + ">: " + response.Body);
			profile = new LocalPlayerProfile(response.Body, OnProfileLoaded);
		}
		else
		{
			CspUtils.DebugLog("User Profile Error <" + response.Status + ">: " + response.Body);
		}
	}

	protected void OnProfileLoaded(UserProfile profile)
	{
		fsm.GotoState<HqControllerLoad>();
	}

	private void onObjectRespawned(HQRoomRespawnMessage message)
	{
		if (message.respawnObj != null)
		{
			HqObject2 component = Utils.GetComponent<HqObject2>(message.respawnObj);
			if (component != null && component.State == typeof(HqObject2.HqObjectFlingaSelected))
			{
				component.GotoFlingaMode();
			}
		}
	}

	private void onRoomChangeRequest(HQRoomChangeRequestMessage message)
	{
		if (message.roomId == null)
		{
			ShsAudioSource.PlayAutoSound(Sounds.GetSource("room_change"));
			if (message.requestedDirection == HQRoomChangeRequestMessage.RoomCycleDirection.Next)
			{
				NextActiveRoom();
			}
			else
			{
				PrevActiveRoom();
			}
			return;
		}
		int num = 0;
		while (true)
		{
			if (num < roomIds.Count)
			{
				if (roomIds[num] == message.roomId)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		SetActiveRoom(rooms[roomIds[num]]);
	}

	private void onItemPurchasedMessage(ShoppingItemPurchasedMessage msg)
	{
		if (msg.ItemType == OwnableDefinition.Category.HQItem)
		{
			ItemDefinition value = null;
			AppShell.Instance.ItemDictionary.TryGetValue(msg.OwnableId, out value);
			if (value != null)
			{
				if (!IsAssetBundleLoaded(value))
				{
					CspUtils.DebugLog("Loading asset bundle " + value.PlacedObjectAssetBundle);
					AppShell.Instance.BundleLoader.FetchAssetBundle(value.PlacedObjectAssetBundle, onAssetBundleLoaded);
				}
			}
			else
			{
				CspUtils.DebugLog("Could not find item definition for item player purchased. Id = " + msg.OwnableId);
			}
		}
		else if (msg.ItemType == OwnableDefinition.Category.Hero && !Instance.characterPrefabs.ContainsKey(msg.OwnableName))
		{
			CspUtils.DebugLog("Received a purchase message for " + msg.OwnableName + ". Loading assets.");
			Instance.characterPrefabs.Add(msg.OwnableName, null);
			AppShell.Instance.DataManager.LoadGameData("Characters/" + msg.OwnableName, OnCharacterDataLoaded, msg.OwnableName);
		}
	}

	protected void OnCharacterDataLoaded(GameDataLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			return;
		}
		string text = response.Data.TryGetString("//character_model/model_name", null);
		if (string.IsNullOrEmpty(text))
		{
			CspUtils.DebugLog("Could not get model name from game data.");
			return;
		}
		string text2 = extraData as string;
		if (string.IsNullOrEmpty(text2))
		{
			CspUtils.DebugLog("Could not get character name from extra data.");
			return;
		}
		CspUtils.DebugLog("Character data for " + text2 + " loaded.");
		string text3 = response.Data.TryGetString("//asset_bundle", null);
		if (!string.IsNullOrEmpty(text3))
		{
			CspUtils.DebugLog("Loading asset bundle for " + text2);
			KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(text, text2);
			AppShell.Instance.BundleLoader.FetchAssetBundle(text3, OnCharacterAssetBundleLoaded, keyValuePair);
		}
	}

	protected void OnCharacterAssetBundleLoaded(AssetBundleLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			return;
		}
		KeyValuePair<string, string> keyValuePair = (KeyValuePair<string, string>)extraData;
		string key = keyValuePair.Key;
		if (string.IsNullOrEmpty(key))
		{
			CspUtils.DebugLog("No model name in character data.");
			return;
		}
		CspUtils.DebugLog("Asset bundle loaded for " + keyValuePair.Value + ". Loading prefab.");
		AppShell.Instance.BundleLoader.LoadAsset(response.Path, key, keyValuePair, OnCharacterPrefabLoaded);
	}

	protected void OnCharacterPrefabLoaded(UnityEngine.Object asset, AssetBundle bundle, object extraData)
	{
		KeyValuePair<string, string> keyValuePair = (KeyValuePair<string, string>)extraData;
		CspUtils.DebugLog("Character prefab loaded for " + keyValuePair.Value);
		Instance.characterPrefabs[keyValuePair.Value] = (asset as GameObject);
		AppShell.Instance.EventMgr.Fire(this, new UpdateInventoryOnBundleLoadedMessage());
	}

	protected void onAssetBundleLoaded(AssetBundleLoadResponse response, object extraData)
	{
		if ((response.Error != null && response.Error != string.Empty) || response.Bundle == null)
		{
			CspUtils.DebugLog("Failed to load: " + response.Path);
			return;
		}
		Instance.bundles[response.Path] = response.Bundle;
		AppShell.Instance.EventMgr.Fire(this, new UpdateInventoryOnBundleLoadedMessage());
	}
}
