using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetSpawner
{
	private int _petOwnableID;

	private PetData _petData;

	private GameObject _parent;

	private int _parentGoNetID = -1;

	private static Dictionary<int, SpawnedPetData> _petDict = new Dictionary<int, SpawnedPetData>();

	private static ArrayList _spawnerList = new ArrayList();

	private static ArrayList _queuedSpawns = new ArrayList();

	public int parentGoNetID
	{
		get
		{
			return _parentGoNetID;
		}
	}

	public PetSpawner(int petOwnableID, GameObject parent, int parentGoNetID, bool queued = false)
	{
		_petOwnableID = petOwnableID;
		_parent = parent;
		_petData = PetDataManager.getData(_petOwnableID);
		_parentGoNetID = parentGoNetID;
		bool flag = _parentGoNetID == -1;
		if (_petDict.ContainsKey(_parentGoNetID))
		{
			SpawnedPetData spawnedPetData = _petDict[_parentGoNetID];
			if (spawnedPetData.parentPlayerObject != null)
			{
				EffectSequenceList effectSequenceList = spawnedPetData.parentPlayerObject.GetComponent(typeof(EffectSequenceList)) as EffectSequenceList;
				EffectSequence logicalEffectSequence = effectSequenceList.GetLogicalEffectSequence("Transport");
				if (logicalEffectSequence != null && spawnedPetData.petObject != null)
				{
					GameObject gameObject = new GameObject("TransportEffect");
					gameObject.transform.position = spawnedPetData.petObject.transform.position;
					logicalEffectSequence.Initialize(null, OnTransportEffectDone, null);
					logicalEffectSequence.SetParent(gameObject, true);
					logicalEffectSequence.StartSequence();
				}
				else
				{
					CspUtils.DebugLog("no prefab :|");
				}
				CharacterGlobals component = spawnedPetData.parentPlayerObject.GetComponent<CharacterGlobals>();
				if (component != null)
				{
					component.activeSidekick = null;
				}
			}
			Object.Destroy(spawnedPetData.petObject);
			_petDict.Remove(_parentGoNetID);
		}
		if (petOwnableID == -1)
		{
			if (flag)
			{
				SetPetMessage setPetMessage = new SetPetMessage(new GoNetId(GoNetId.PLAYER_ID_FLAG, AppShell.Instance.ServerConnection.GetGameUserId()));
				setPetMessage.petID = petOwnableID;
				AppShell.Instance.ServerConnection.SendGameMsg(setPetMessage);
			}
		}
		else if (queued)
		{
			_queuedSpawns.Add(this);
		}
		else
		{
			beginLoad();
		}
	}

	public static void requestPet(int petOwnableID)
	{
		_spawnerList.Add(new PetSpawner(petOwnableID, null, -1));
	}

	public static void requestRemotePet(int petOwnableID, GameObject parent, int parentGoNetID, bool queued = false)
	{
		_spawnerList.Add(new PetSpawner(petOwnableID, parent, parentGoNetID, queued));
	}

	public static void heroCreated(int parentGoNetID, GameObject parentObject)
	{
		foreach (PetSpawner queuedSpawn in _queuedSpawns)
		{
			if (queuedSpawn.parentGoNetID == parentGoNetID)
			{
				queuedSpawn.setParent(parentObject);
				queuedSpawn.beginLoad();
				_queuedSpawns.Remove(queuedSpawn);
				break;
			}
		}
	}

	public void beginLoad()
	{
		AppShell.Instance.BundleLoader.LoadAsset(_petData.bundlePath, string.Empty, null, delegate
		{
			OnPetAssetLoaded();
		});
	}

	public void setParent(GameObject newParent)
	{
		_parent = newParent;
	}

	private void OnPetAssetLoaded()
	{
		if (_petData.characterDataPath != string.Empty)
		{
			AppShell.Instance.DataManager.LoadGameData(_petData.characterDataPath, OnPetCharacterDataLoaded, null);
		}
		else
		{
			FinishPet(null);
		}
	}

	protected void OnPetCharacterDataLoaded(GameDataLoadResponse response, object extraData)
	{
		FinishPet(response.Data);
	}

	public void EffectsLoadedCallback(EffectSequenceList fxList, object extraData)
	{
		string text = extraData as string;
		if (text == null || !(fxList.gameObject != null))
		{
			return;
		}
		Object effectSequencePrefabByName = fxList.GetEffectSequencePrefabByName(text);
		if (!(effectSequencePrefabByName == null))
		{
			GameObject gameObject = Object.Instantiate(effectSequencePrefabByName) as GameObject;
			EffectSequence component = Utils.GetComponent<EffectSequence>(gameObject);
			if (component != null && component.AttachToParent)
			{
				component.Initialize(fxList.gameObject, null, null);
			}
			if (fxList.gameObject != null && !fxList.gameObject.active)
			{
				gameObject.active = false;
			}
			Utils.AttachGameObject(fxList.gameObject, gameObject);
		}
	}

	protected void FinishPet(DataWarehouse characterData)
	{
		bool flag = _parentGoNetID == -1;
		CachedAssetBundle cachedAssetBundle = AppShell.Instance.BundleLoader.CachedBundles[_petData.bundlePath];
		if (cachedAssetBundle == null)
		{
			CspUtils.DebugLog("FinishPet - bundle is null, aborting");
			_spawnerList.Remove(this);
			return;
		}
		if (_parent == null)
		{
			if (flag)
			{
				_parent = GameController.GetController().LocalPlayer;
			}
			if (_parent == null)
			{
				CspUtils.DebugLog("FinishPet - _parent is null, aborting");
				_spawnerList.Remove(this);
				return;
			}
		}
		GameObject gameObject = (GameObject)Object.Instantiate(cachedAssetBundle.Bundle.mainAsset);
		if (gameObject != null)
		{
			gameObject.transform.position = _parent.transform.position + new Vector3(0f, 1f, 0f);
			gameObject.AddComponent(typeof(ShsCharacterController));
			SpawnData spawnData = gameObject.AddComponent(typeof(SpawnData)) as SpawnData;
			spawnData.modelName = _petData.bundlePath;
			Animation component = Utils.GetComponent<Animation>(gameObject, Utils.SearchChildren);
			if (component != null)
			{
				FacialAnimation facialAnimation = Utils.AddComponent<FacialAnimation>(component.gameObject);
				facialAnimation.facialExpression = FacialAnimation.Expression.Normal;
			}
			EffectSequenceList effectSequenceList = gameObject.AddComponent(typeof(EffectSequenceList)) as EffectSequenceList;
			if (characterData != null)
			{
				DataWarehouse data = characterData.GetData("//effect_sequence_list");
				effectSequenceList.InitializeFromData(data, cachedAssetBundle.Bundle, _petData.bundlePath);
				effectSequenceList.RequestLoadedCallback(EffectsLoadedCallback, characterData.TryGetString("//character_model/effect_name", string.Empty));
			}
			BehaviorManager behaviorManager = gameObject.AddComponent(typeof(BehaviorManager)) as BehaviorManager;
			if (_petData.customRunAnim != string.Empty)
			{
				behaviorManager.OverrideAnimation("movement_run", _petData.customRunAnim);
				behaviorManager.OverrideAnimation("movement_walk", _petData.customRunAnim);
			}
			if (_petData.customIdleAnim != string.Empty)
			{
				behaviorManager.OverrideAnimation("movement_idle", _petData.customIdleAnim);
			}
			CharacterGlobals characterGlobals = gameObject.AddComponent(typeof(CharacterGlobals)) as CharacterGlobals;
			CharacterDefinition characterDefinition = characterGlobals.definitionData = new CharacterDefinition();
			CharacterController characterController = gameObject.GetComponent(typeof(CharacterController)) as CharacterController;
			characterController.radius = 0.5f;
			characterController.height = 1f;
			characterController.center = new Vector3(0f, 0.5f, 0f);
			CharacterMotionController characterMotionController = gameObject.AddComponent(typeof(CharacterMotionController)) as CharacterMotionController;
			characterMotionController.setBaseSpeed(6.3f);
			characterMotionController.hotSpotType = PetDataManager.getData(_petData.id).hotSpotType;
			characterGlobals.Awake();
			PetCommandManager petCommandManager = gameObject.AddComponent(typeof(PetCommandManager)) as PetCommandManager;
			petCommandManager.petData = _petData;
			petCommandManager.target = _parent;
			AIControllerPet aIControllerPet = gameObject.AddComponent(typeof(AIControllerPet)) as AIControllerPet;
			aIControllerPet.target = _parent;
			aIControllerPet.petData = _petData;
			gameObject.AddComponent(typeof(LodCharacter));
			SkinnedMeshRenderer[] componentsInChildren = aIControllerPet.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			SkinnedMeshRenderer[] array = componentsInChildren;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
			{
				skinnedMeshRenderer.gameObject.AddComponent<DynamicShadowTarget>();
			}
			gameObject.layer = 12;
			gameObject.transform.localScale = _petData.scale;
			Utils.SetLayerTree(gameObject, gameObject.layer);
			if (flag)
			{
			}
			if (_petDict.ContainsKey(_parentGoNetID))
			{
				Object.Destroy(_petDict[_parentGoNetID].petObject);
				_petDict.Remove(_parentGoNetID);
			}
			_petDict.Add(_parentGoNetID, new SpawnedPetData(_petData.id, gameObject, _parent));
			EffectSequenceList effectSequenceList2 = _parent.GetComponent(typeof(EffectSequenceList)) as EffectSequenceList;
			EffectSequence logicalEffectSequence = effectSequenceList2.GetLogicalEffectSequence("Transport");
			if (logicalEffectSequence != null)
			{
				GameObject gameObject2 = new GameObject("TransportEffect");
				gameObject2.transform.position = gameObject.transform.position;
				logicalEffectSequence.Initialize(null, OnTransportEffectDone, null);
				logicalEffectSequence.SetParent(gameObject2, true);
				logicalEffectSequence.StartSequence();
			}
			if (flag)
			{
				SetPetMessage setPetMessage = new SetPetMessage(new GoNetId(GoNetId.PLAYER_ID_FLAG, AppShell.Instance.ServerConnection.GetGameUserId()));
				setPetMessage.petID = _petData.id;
				AppShell.Instance.ServerConnection.SendGameMsg(setPetMessage);
			}
			CharacterGlobals component2 = _parent.GetComponent<CharacterGlobals>();
			if (component2 != null)
			{
				component2.activeSidekick = characterGlobals;
			}
			if (flag)
			{
				if (AppShell.Instance.ExpendablesManager.hasActiveEffect("BuffPotionSpeedIncrease"))
				{
					characterMotionController.addSpeedMultiplier(1.5f);
				}
				foreach (SidekickSpecialAbility ability in _petData.abilities)
				{
					if (ability.isUnlocked())
					{
						ability.attachToPetObject(gameObject);
					}
				}
				petCommandManager.spawned();
			}
		}
		_spawnerList.Remove(this);
	}

	private void OnTransportEffectDone(EffectSequence sequence)
	{
		Object.Destroy(sequence.transform.parent.gameObject);
	}
}
