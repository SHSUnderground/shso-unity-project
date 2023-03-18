using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CharacterSpawn : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	[Flags]
	public enum Type
	{
		Unknown = 0x0,
		Local = 0x1,
		Remote = 0x2,
		Player = 0x4,
		AI = 0x8,
		Boss = 0x10,
		Static = 0x20,
		NPC = 0x40,
		Moderator = 0x80,
		Pet = 0x100,
		Ally = 0x200,
		Polymorph = 0x400,
		LocalPlayer = 0x5,
		LocalAI = 0x9,
		LocalPet = 0x101
	}

	protected class SpawnPositionBlockData
	{
		public static readonly float SPAWN_POSITION_BLOCK_TIME_OUT = 60f;

		public static readonly float SPAWN_POSITION_BLOCK_RANGE_DECREMENT = 0.01f;

		public float spawnPositionBlockRange = 1f;

		public float spawnPositionBlockTime = Time.time;

		public CombatController spawnPositionBlocker;
	}

	public delegate void OnSpawnCallback(GameObject obj);

	public delegate void OnTransactionInitialized(CharacterSpawn spawner, TransactionMonitor spawnTransaction);

	public delegate void OnRealCharacterDataLoaded(DataWarehouse data);

	public delegate void CharacterSpawned(GameObject characterObject, CharacterSpawnData spawnData);

	public string CharacterName = "wolverine";

	protected string SpawnName = "wolverine";

	public int R2Attack = 1;

	public bool IsLocal = true;

	public bool IsPlayer = true;

	public bool IsAI;

	public bool IsNpc;

	public bool IsBoss;

	public bool IsNetworked = true;

	public bool DestroyOnSpawn;

	public bool IgnorePrecache;

	public bool SpawnOnStart = true;

	public bool RecordHistory;

	public Hashtable netExtraData;

	public bool ForceLocalPlayerSpawnOnStart;

	public bool despawnOnDeath = true;

	public bool blinkBeforeDespawn = true;

	public bool overrideAggroDistance;

	public float aggroDistance = 10f;

	public bool canPlayVOWithoutCutscene;

	public bool wakeUpOnAggro = true;

	public bool wakeUpOnEvent = true;

	public bool wakeUpOnHit = true;

	public string overrideDeathAnim = string.Empty;

	public string overrideDeathSequenceName = string.Empty;

	public bool overrideBossDeathSequence;

	public OnRealCharacterDataLoaded onRealCharacterDataLoaded;

	public GameObject prefabTextBillboard;

	public GameObject prefabPlayerBillboard;

	public GameObject prefabOcclusionDetector;

	public GameObject prefabBlobShadow;

	public GameObject prefabPushaway;

	public GameObject spawnFXPrefab;

	public GameObject spawnInSource;

	public string startingEffect = string.Empty;

	public bool Players1 = true;

	public bool Players2 = true;

	public bool Players3 = true;

	public bool Players4 = true;

	public bool spawnCachedOnGround = true;

	public float groundRayLength = 10f;

	public bool rewardsPoints = true;

	public bool attacksObjects;

	[HideInInspector]
	public NetworkComponent spawnerNetwork;

	[HideInInspector]
	public GoNetId goNetId = GoNetId.Invalid;

	protected GameObject model;

	protected GameObject obj;

	protected EffectSequenceList fxSeqList;

	protected List<CharacterGlobals> spawnedCharacters;

	protected bool forceAICombat;

	protected CombatController.Faction forcedFaction = CombatController.Faction.None;

	protected bool isLocalBackup;

	public bool doTransportEffect = true;

	protected bool isAIPlayer;

	protected bool spawnRayPointFound;

	protected Vector3 spawnRayPoint;

	protected float spawnRayPointOffset = ShsCharacterController.GroundOffset;

	private bool _haltSpawn;

	public static readonly int MAX_OWNERSHIP_ATTEMPTS = 5;

	protected int ownershipAttempts;

	public bool HaltSpawn
	{
		get
		{
			return _haltSpawn;
		}
		set
		{
			_haltSpawn = value;
		}
	}

	public event OnSpawnCallback onSpawnCallback;

	public event OnTransactionInitialized onTransactionInitialized;

	public static void d(string deb)
	{
		CspUtils.DebugLog(deb);
	}

	protected IEnumerator Spawn(string characterName, int r2Attack, Transform location, Type type, Hashtable initialNetData, CharacterSpawned callback, bool isNetworked, object extra)
	{
		d(characterName + "IEnumerator Spawn ");
		if (isNetworked)
		{
			while ((AppShell.Instance.ServerConnection.State & NetworkManager.ConnectionState.ConnectedToGame) == 0)
			{
				yield return 0;
			}
		}
		while (BrawlerController.Instance != null && !BrawlerController.Instance.prespawnStarted)
		{
			yield return 0;
		}
		if (type == Type.LocalPlayer && isNetworked)
		{
			goNetId = new GoNetId(GoNetId.PLAYER_ID_FLAG, AppShell.Instance.ServerConnection.GetGameUserId());
		}
		CharacterSpawnData spawnData = new CharacterSpawnData(characterName, r2Attack, location, type, initialNetData, callback, isNetworked, extra);
		spawnData.SetSpawner(this);
		if (spawnerNetwork != null && IsLocal && !AppShell.Instance.ServerConnection.IsGameHost())
		{
			yield break;  // CSP don't spawn if not game host?
		}
		////// else block added by CSP for testing //////////
		else {
			CspUtils.DebugLog("IsLocal " + IsLocal);  // why is this false for aim agent??
			CspUtils.DebugLog("!AppShell.Instance.ServerConnection.IsGameHost()  " + (!AppShell.Instance.ServerConnection.IsGameHost()));
			CspUtils.DebugLog("spawnerNetwork!=null " + (spawnerNetwork != null ));
		}
		////////////////////////////////////////////////////
		if (extra is PolymorphSpawnData)
		{
			spawnData.Type |= Type.Polymorph;
		}
		if (BrawlerController.Instance != null && type == Type.LocalAI && (spawnerNetwork == null || AppShell.Instance.ServerConnection.IsGameHost()) && !(extra is PolymorphSpawnData))
		{
			Vector3 spawnPosition = GetSpawnPosition(spawnData);
			SpawnPositionBlockData blockedData = new SpawnPositionBlockData();
			while (!CheckSpawnPosition(spawnPosition, blockedData))
			{
				yield return new WaitForSeconds(0.5f);
			}
		}
		bool precached = false;
		IgnorePrecache = true; // CSP - added this as test to see if this helps kingpin thugs..
		if (GameController.GetController() != null && !IgnorePrecache)
		{
			ICharacterCache prespawnHandler = GameController.GetController().CharacterCache;
			if (prespawnHandler != null && prespawnHandler.IsCharacterCached(SpawnName))
			{
				precached = true;
				spawnData.spawnTransaction = TransactionMonitor.CreateTransactionMonitor(characterName + "_spawnTransaction", OnReadyForCacheRequest, 5f, spawnData);
				spawnData.AddOwnershipStep();
			}
		}

		CspUtils.DebugLog("CS Spawn() characterName=" + characterName + " precached=" + precached);

		if (!precached)
		{
			spawnData.spawnTransaction = TransactionMonitor.CreateTransactionMonitor(characterName + "_spawnTransaction", OnReadyForSpawn, 5f, spawnData);
			spawnData.spawnTransaction.AddStep("assetLoad");
			spawnData.AddOwnershipStep();
			spawnData.spawnTransaction.CompleteStep("ownership");  // CSP -  temporary to get kingpin thugs working.
			if (this.onTransactionInitialized != null)
			{
				this.onTransactionInitialized(this, spawnData.spawnTransaction);
			}
			spawnData.assetLoadTransaction = TransactionMonitor.CreateTransactionMonitor("CharacterSpawn_assetLoadTransaction(" + characterName + ")", OnAssetsLoaded, 5f, spawnData);
			spawnData.assetLoadTransaction.AddStep("gamedata", delegate(string step, bool success, TransactionMonitor transaction)
			{
				LogStep(characterName, step, success, transaction);
			});
			spawnData.assetLoadTransaction.AddStep("assets", delegate(string step, bool success, TransactionMonitor transaction)
			{
				LogStep(characterName, step, success, transaction);
			});
			AppShell.Instance.DataManager.LoadGameData("Characters/" + characterName, OnCharacterDataLoaded, spawnData);
		}
	}

	protected void LogStep(string characterName, string step, bool success, TransactionMonitor transaction)
	{
	}

	public virtual void Prespawn()
	{
		d(CharacterName + " Prespawn");
		ICharacterCache characterCache = GameController.GetController().CharacterCache;
		if (characterCache != null)
		{
			if (characterCache.IsCharacterCached(SpawnName))
			{
				ToggleAIPlayerPrespawn(false);
				return;
			}
			characterCache.StartCharacterCache(SpawnName);
			CharacterSpawnData characterSpawnData = new CharacterSpawnData(CharacterName, 1, base.transform, GetSpawnType(), netExtraData, OnCharacterPrespawned, false, null);
			characterSpawnData.SetSpawner(this);
			characterSpawnData.assetLoadTransaction = TransactionMonitor.CreateTransactionMonitor("Prespawn_assetLoadTransaction", OnReadyForPrefab, 5f, characterSpawnData);
			characterSpawnData.assetLoadTransaction.AddStep("gamedata");
			characterSpawnData.assetLoadTransaction.AddStep("assets");
			characterSpawnData.prespawnTransaction = TransactionMonitor.CreateTransactionMonitor("Prespawn_prespawnTransaction", PrespawnComplete, 5f, characterSpawnData);
			characterSpawnData.prespawnTransaction.AddStep("assetsLoaded");
			characterSpawnData.prespawnTransaction.AddStep("effectsLoaded");
			characterSpawnData.prespawnTransaction.CompleteStep("effectsLoaded");   /// CSP temporary
			AppShell.Instance.DataManager.LoadGameData("Characters/" + CharacterName, OnCharacterDataLoaded, characterSpawnData);
		}
	}

	protected void OnCharacterDataLoaded(GameDataLoadResponse response, object extraData)
	{
		d(CharacterName + " OnCharacterDataLoaded ");
		if (HaltSpawn)
		{
			return;
		}
		CharacterSpawnData characterSpawnData = extraData as CharacterSpawnData;
		if (characterSpawnData == null)
		{
			CspUtils.DebugLog("Spawn data missing in callback from GameDataManager for character <" + response.Path + ">.");
		}
		else
		{
			if (characterSpawnData.assetLoadTransaction == null)
			{
				return;
			}
			if (response.Error != null && response.Error != string.Empty)
			{
				ICharacterCache characterCache = GameController.GetController().CharacterCache;
				if (characterCache != null && characterCache.IsCharacterBeingCached(SpawnName))
				{
					characterCache.FailCacheCharacter(SpawnName, response.Error);
				}
				else
				{
					CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
				}
				characterSpawnData.assetLoadTransaction.FailStep("gamedata", response.Error);
				return;
			}
			characterSpawnData.characterData = response.Data;
			if (onRealCharacterDataLoaded != null)
			{
				DataWarehouse data = characterSpawnData.characterData.GetData("//character_motion_controller");
				onRealCharacterDataLoaded(data);
				onRealCharacterDataLoaded = null;
			}
			if (HqController2.Instance != null)
			{
				string requestUri = "Characters/HQ/hq_" + CharacterName;
				AppShell.Instance.DataManager.LoadGameData(requestUri, OnCharacterHQDataLoaded, characterSpawnData);
			}
			else if ((characterSpawnData.Type & Type.NPC) != 0)
			{
				string requestUri2 = "Characters/NPC/data_" + CharacterName;
				AppShell.Instance.DataManager.LoadGameData(requestUri2, OnCharacterNPCDataLoaded, characterSpawnData);
			}
			else
			{
				LoadCharacterAssetBundle(characterSpawnData);
			}
		}
	}

	public void forceFaction(CombatController.Faction theFaction)
	{
		forcedFaction = theFaction;
	}

	protected void LoadCharacterAssetBundle(CharacterSpawnData spawnData)
	{
		d(CharacterName + " LoadCharacterAssetBundle ");
		if (spawnData.characterData == null)
		{
			CspUtils.DebugLog("No character data found for <" + spawnData.ModelName + ">");
			spawnData.assetLoadTransaction.FailStep("gamedata", "No character data found");
			return;
		}
		string @string = spawnData.characterData.GetString("//asset_bundle");
		if (@string != null)
		{
			spawnData.characterBundlePath = @string;
			spawnData.assetLoadTransaction.CompleteStep("gamedata");
			AppShell.Instance.BundleLoader.FetchAssetBundle(@string, OnCharacterAssetBundleLoaded, spawnData);
		}
		else
		{
			CspUtils.DebugLog("No asset bundle name found in character data <" + spawnData.ModelName + ">.  Cannot spawn character.");
			spawnData.assetLoadTransaction.FailStep("gamedata", "GameData did not specify an asset bundle");
		}
	}

	protected void OnCharacterHQDataLoaded(GameDataLoadResponse response, object extraData)
	{
		CharacterSpawnData characterSpawnData = extraData as CharacterSpawnData;
		if (characterSpawnData == null)
		{
			CspUtils.DebugLog("Spawn data missing in callback from GameDataManager for character <" + response.Path + ">.");
		}
		else
		{
			if (characterSpawnData.assetLoadTransaction == null)
			{
				return;
			}
			if (response.Error != null && response.Error != string.Empty)
			{
				CspUtils.DebugLog("The following error occurred while fetching ai hq data for <" + response.Path + ">: " + response.Error);
				if (characterSpawnData.spawnTransaction != null)
				{
					characterSpawnData.spawnTransaction.Fail("Failing spawn transaction because asset loading failed.");
				}
				characterSpawnData.assetLoadTransaction.Fail(response.Error);
			}
			else
			{
				characterSpawnData.hqData = response.Data;
				LoadCharacterAssetBundle(characterSpawnData);
			}
		}
	}

	protected void OnCharacterNPCDataLoaded(GameDataLoadResponse response, object extraData)
	{
		d(CharacterName + " OnCharacterNPCDataLoaded ");
		CharacterSpawnData characterSpawnData = extraData as CharacterSpawnData;
		if (characterSpawnData == null)
		{
			CspUtils.DebugLog("Spawn data missing in callback from GameDataManager for character <" + response.Path + ">.");
		}
		else if (characterSpawnData.assetLoadTransaction != null)
		{
			if (response.Error != null && response.Error != string.Empty)
			{
				CspUtils.DebugLog("The following error occurred while fetching ai hq data for <" + response.Path + ">: " + response.Error);
			}
			characterSpawnData.npcData = response.Data;
			LoadCharacterAssetBundle(characterSpawnData);
		}
	}

	protected void OnCharacterAssetBundleLoaded(AssetBundleLoadResponse response, object extraData)
	{
		d(CharacterName + " OnCharacterAssetBundleLoaded ");
		if (HaltSpawn)
		{
			CspUtils.DebugLog("HaltSpawn is true - returning.");
			return;
		}
		CharacterSpawnData characterSpawnData = extraData as CharacterSpawnData;
		if (characterSpawnData == null)
		{
			CspUtils.DebugLog("Spawn data missing in callback from AssetBundleLoader for character <" + response.Path + ">.");
		}
		else
		{
			if (characterSpawnData.assetLoadTransaction == null)
			{
				CspUtils.DebugLog("characterSpawnData.assetLoadTransaction == null - returning...");
				return;
			}
			if (response.Error != null && response.Error != string.Empty)
			{
				CspUtils.DebugLog("The following error occurred while loading character assets for <" + response.Path + ">: " + response.Error);
				characterSpawnData.assetLoadTransaction.FailStep("assets", response.Error);
				return;
			}
			if (response.Bundle == null)
			{
				CspUtils.DebugLog("Asset bundle is missing for <" + response.Path + ">: " + response.Error);
				characterSpawnData.assetLoadTransaction.FailStep("assets", "no bundle returned");
				return;
			}
			characterSpawnData.characterAssets = response.Bundle;
			if (characterSpawnData.characterAssets == null)
			{
				CspUtils.DebugLog("Character asset bundle missing in callback from AssetBundleLoader for character <" + response.Path + ">.");
				characterSpawnData.assetLoadTransaction.FailStep("assets", "Asset bundle is missing");
				return;
			}
			string @string = characterSpawnData.characterData.GetString("//character_model/model_name");
			if (@string == null)
			{
				CspUtils.DebugLog("No model name in character data for <" + characterSpawnData.ModelName + ">.  Cannot spawn character.");
				characterSpawnData.assetLoadTransaction.FailStep("assets", "No model name in character data, cannot load prefab.");
			}
			else
			{
				CspUtils.DebugLog("About to LoadAsset() " + @string);
				characterSpawnData.assetLoadTransaction.AddStepBundle("assets", response.Path);
				AppShell.Instance.BundleLoader.LoadAsset(response.Path, @string, characterSpawnData, OnCharacterPrefabLoaded);
			}
		}
	}

	protected void OnCharacterPrefabLoaded(UnityEngine.Object asset, AssetBundle bundle, object extraData)
	{
		d(CharacterName + " OnCharacterPrefabLoaded ");
		if (HaltSpawn)
		{
			return;
		}
		CharacterSpawnData characterSpawnData = extraData as CharacterSpawnData;
		if (characterSpawnData == null)
		{
			CspUtils.DebugLog("Spawn data missing in callback from AssetBundleLoader for unknown character.");
			return;
		}
		characterSpawnData.CharacterPrefab = asset;
		if (characterSpawnData.CharacterPrefab == null)
		{
			CspUtils.DebugLog("Failed to load character prefab from asset bundle for character <" + characterSpawnData.ModelName + ">.");
			characterSpawnData.assetLoadTransaction.FailStep("assets", "Character Prefab failed to load");
		}
		else
		{
			characterSpawnData.assetLoadTransaction.CompleteStep("assets");
		}
	}

	protected void OnReadyForCacheRequest(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		d(CharacterName + " OnReadyForCacheRequest ");
		CharacterSpawnData characterSpawnData = userData as CharacterSpawnData;
		if (characterSpawnData != null)
		{
			characterSpawnData.spawnTransaction = null;
		}
		GameController controller = GameController.GetController();
		ICharacterCache characterCache = controller.CharacterCache;
		if (characterCache == null)
		{
			return;
		}
		switch (exit)
		{
		case TransactionMonitor.ExitCondition.Success:
			characterCache.SpawnCachedCharacter(SpawnName, userData as CharacterSpawnData, this);
			break;
		case TransactionMonitor.ExitCondition.Fail:
		case TransactionMonitor.ExitCondition.TimedOut:
			if (controller is BrawlerController)
			{
				AppShell.Instance.CriticalError(SHSErrorCodes.Code.BrawlerSpawnerOwnershipFail, error);
			}
			else if (controller is SocialSpaceController)
			{
				AppShell.Instance.CriticalError(SHSErrorCodes.Code.GameWorldSpawnerOwnershipFail, error);
			}
			else
			{
				AppShell.Instance.CriticalError(SHSErrorCodes.Code.UnableToConnect, error);
			}
			break;
		}
	}

	protected void OnAssetsLoaded(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		d(CharacterName + " OnAssetsLoaded ");
		CharacterSpawnData characterSpawnData = userData as CharacterSpawnData;
		if (characterSpawnData == null)
		{
			return;
		}
		characterSpawnData.assetLoadTransaction = null;
		if (characterSpawnData.spawnTransaction != null)
		{
			if (exit != 0)
			{
				characterSpawnData.spawnTransaction.FailStep("assetLoad", error);
			}
			else
			{
				characterSpawnData.spawnTransaction.CompleteStep("assetLoad");
			}
		}
	}

	protected void OnReadyForSpawn(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		d(CharacterName + " OnReadyForSpawn ");
		CharacterSpawnData characterSpawnData = userData as CharacterSpawnData;
		if (characterSpawnData == null)
		{
			CspUtils.DebugLog("Spawn data is missing - aborting spawn");
			return;
		}
		characterSpawnData.spawnTransaction = null;
		if (exit != 0)
		{
			CspUtils.DebugLog("exit != 0  - aborting spawn");
			return;
		}
		if (characterSpawnData.characterAssets == null)
		{
			CspUtils.DebugLog("Spawn data is missing the asset bundle for character <" + characterSpawnData.ModelName + ">. - aborting spawn");
			return;
		}
		DataWarehouse characterData = characterSpawnData.characterData;
		AssetBundle characterAssets = characterSpawnData.characterAssets;
		Transform transform = characterSpawnData.Location;
		if (transform == null)
		{
			CspUtils.DebugLog("transform == null  - aborting spawn");
			return;
		}
		Type type = characterSpawnData.Type;
		PolymorphSpawnData polymorphSpawnData = characterSpawnData.extra as PolymorphSpawnData;
		bool flag = polymorphSpawnData != null;
		if (spawnInSource != null && !flag)
		{
			transform = spawnInSource.transform;
		}
		CharacterDefinition characterDefinition = new CharacterDefinition();
		characterDefinition.InitializeFromData(characterData);
		string @string = characterData.GetString("//name");
		if ((GetSpawnType() & Type.NPC) == 0)
		{
			HeroOffsetData.AddHeroOffsetData(@string, characterData);
		}
		obj = (UnityEngine.Object.Instantiate(characterSpawnData.CharacterPrefab) as GameObject);
		obj.name = @string;
		CspUtils.DebugLog("Prefab Instantiated: " + obj.name);
		obj.transform.position = transform.position;
		obj.transform.rotation = transform.rotation;
		if (BrawlerController.Instance != null && !BrawlerController.Instance.pvpEnabled() && (type & Type.Player) != 0)
		{
			obj.layer = 21;
		}
		else
		{
			obj.layer = 12;
		}
		SpawnData spawnData = obj.AddComponent(typeof(SpawnData)) as SpawnData;
		spawnData.spawnType = type;
		spawnData.modelName = characterSpawnData.ModelName;
		spawnData.sizeRank = characterData.TryGetFloat("//character_model/size_rank", 5f);
		spawnData.sendNewEntityMsg = !flag;
		spawnData.rewardsPoints = rewardsPoints;
		spawnData.spawner = this;
		CspUtils.DebugLog("CS ORFS spawner.name=" + spawnData.spawner.gameObject.name + " IsNetworked=" + spawnData.spawner.IsNetworked);
		Vector3 vector = characterData.TryGetVector("//character_model/scale", Vector3.one);
		if (vector != Vector3.one)
		{
			obj.transform.localScale = vector;
		}
		UnityEngine.Object[] componentsInChildren = obj.GetComponentsInChildren(typeof(SkinnedMeshRenderer));
		UnityEngine.Object[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)array[i];
			skinnedMeshRenderer.updateWhenOffscreen = characterData.TryGetBool("//lod/animate_offscreen", false);
		}
		AddLodComponent(obj, characterData);
		if (prefabOcclusionDetector != null && type == Type.LocalPlayer && !flag)
		{
			if (PlayerOcclusionDetector.Instance == null)
			{
				GameObject child = UnityEngine.Object.Instantiate(prefabOcclusionDetector) as GameObject;
				Utils.AttachGameObject(obj, child);
			}
			else
			{
				PlayerOcclusionDetector instance = PlayerOcclusionDetector.Instance;
				instance.myPlayer = obj;
				instance.myCameraMgr = CameraLiteManager.Instance;
			}
		}
		Utils.SetLayerTree(obj, obj.layer);
		if (characterData.TryGetBool("//character_model/disable_shadow", false))
		{
			DisableShadows(obj);
		}
		else
		{
			GraphicsOptions.AddPlayerShadow(obj);
		}
		float num = characterData.TryGetFloat("//click_box/height", 0f);
		if (num > 0f && (type & Type.Player) == 0)
		{
			GameObject original = Resources.Load("Character/ClickBoxCapsule") as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
			gameObject.name = "ClickBox";
			Utils.AttachGameObject(obj, gameObject);
			CapsuleCollider capsuleCollider = gameObject.GetComponent(typeof(CapsuleCollider)) as CapsuleCollider;
			capsuleCollider.height = num;
			capsuleCollider.radius = characterData.TryGetFloat("//click_box/radius", num / 2f);
			if (capsuleCollider.radius == capsuleCollider.height / 2f)
			{
				capsuleCollider.height += 0.01f;
			}
			capsuleCollider.center = characterData.TryGetVector("//click_box/center", new Vector3(0f, num / 3f, 0f));
			if (HqController2.Instance != null)
			{
				Rigidbody rigidbody = Utils.AddComponent<Rigidbody>(gameObject);
				rigidbody.useGravity = false;
				rigidbody.isKinematic = true;
			}
		}
		CharacterController characterController = obj.AddComponent(typeof(CharacterController)) as CharacterController;
		characterController.height = characterData.TryGetFloat("//character_controller/height", 2f);
		characterController.radius = characterData.TryGetFloat("//character_controller/radius", 0.5f);
		characterController.slopeLimit = characterData.TryGetFloat("//character_controller/slope_limit", 90f);
		characterController.stepOffset = characterData.TryGetFloat("//character_controller/step_offset", 0.3f);
		characterController.center = characterData.TryGetVector("//character_controller/center", new Vector3(0f, 1f, 0f));
		ShsCharacterController shsCharacterController = obj.AddComponent(typeof(ShsCharacterController)) as ShsCharacterController;
		shsCharacterController.pickupBone = characterData.TryGetString("//character_controller/pickup_bone", "fx_Rpalm");
		BehaviorManager behaviorManager = obj.AddComponent(typeof(BehaviorManager)) as BehaviorManager;
		string text = characterData.TryGetString("//behavior_manager/default_behavior_type", "BehaviorMovement");
		if (text != "BehaviorBase")
		{
			behaviorManager.defaultBehaviorType = text;
		}
		CharacterMotionController characterMotionController = obj.AddComponent(typeof(CharacterMotionController)) as CharacterMotionController;
		CspUtils.DebugLog(obj.name + " characterMotionController=" + characterMotionController);
		characterMotionController.SetRestoreLocation(characterSpawnData.Location.position);
		DataWarehouse data = characterData.GetData("//character_motion_controller");
		CspUtils.DebugLog(obj.name + " data=" + data);
		characterMotionController.InitializeFromData(data);
		CspUtils.DebugLog("spawnInSource=" + spawnInSource);
		CspUtils.DebugLog("flag=" + flag);
		if (spawnInSource != null && !flag)
		{
			characterMotionController.setSpawnAnimation(new SpawnAnimateData(spawnInSource, base.transform.position));
		}
		Animation component = Utils.GetComponent<Animation>(obj, Utils.SearchChildren);
		if (component != null)
		{
			FacialAnimation facialAnimation = Utils.AddComponent<FacialAnimation>(component.gameObject);
			facialAnimation.facialExpression = FacialAnimation.Expression.Normal;
			facialAnimation.defaultFacialExpression = (FacialAnimation.Expression)(int)Enum.Parse(typeof(FacialAnimation.Expression), characterData.TryGetString("//default_facial_expression", "Normal"));
		}
		CharacterStats characterStats = Utils.AddComponent<CharacterStats>(obj);
		DataWarehouse data2 = characterData.GetData("//character_stats");
		characterStats.InitializeFromData(data2);
		AddExtraComponents(obj, characterData, characterAssets);
		bool flag2 = false;
		type &= ~Type.Polymorph;
		switch (type)
		{
		case Type.LocalPlayer:
		{
			
			PlayerCombatController playerCombatController = LoadPlayerCombatController(obj, characterData);
			CspUtils.DebugLog(obj.name + " playerCombatController=" + playerCombatController);
			UserProfile profile = AppShell.Instance.Profile;
			if (profile != null && profile.AvailableCostumes[@string] != null)
			{
				CspUtils.DebugLog("Character Spawn for " + @string + " " + profile.AvailableCostumes[@string].Level);
				playerCombatController.changeLevel(profile.AvailableCostumes[@string].Level);
			}
			playerCombatController.SetSecondaryAttack(characterSpawnData.R2Attack);
			PlayerInputControllerDebug playerInputControllerDebug = obj.AddComponent(typeof(PlayerInputControllerDebug)) as PlayerInputControllerDebug;
			CspUtils.DebugLog(obj.name + " playerInputControllerDebug=" + playerInputControllerDebug);
			playerInputControllerDebug.effectSeq = null;
			flag2 = true;
			if (!flag)
			{
				AppShell.Instance.EventMgr.Fire(null, new BrawlerHideSelectMessage());
			}
			break;
		}
		case Type.Remote | Type.Player:
			LoadPlayerCombatController(obj, characterData);
			flag2 = true;
			break;
		default:
			if ((type & Type.Boss) != 0)
			{
				LoadAICombatController(obj, characterData);
				BossAIControllerBrawler bossAIControllerBrawler = obj.AddComponent(typeof(BossAIControllerBrawler)) as BossAIControllerBrawler;
				DataWarehouse bossAiMeleeData = characterData.TryGetData("//boss_ai_melee_controller", new EmptyDataWarehouse());
				bossAIControllerBrawler.InitializeFromData(bossAiMeleeData);
				bossAIControllerBrawler.InitializeFromSpawner(this);
			}
			else if ((type & Type.AI) != 0)
			{
				if (BrawlerController.Instance != null)
				{
					CombatController combatController = LoadAICombatController(obj, characterData);
					if ((type & Type.Ally) != 0)
					{
						AllyControllerBrawler allyControllerBrawler = obj.AddComponent(typeof(AllyControllerBrawler)) as AllyControllerBrawler;
						DataWarehouse aiMeleeData = characterData.TryGetData("//ai_melee_controller", new EmptyDataWarehouse());
						allyControllerBrawler.InitializeFromData(aiMeleeData);
						allyControllerBrawler.InitializeFromSpawner(this);
						allyControllerBrawler.aggroDistance = 90f;
						combatController.SetSecondaryAttack(characterSpawnData.R2Attack);
						CspUtils.DebugLog(CharacterName + " is Ally " + characterSpawnData.extra);
						AllySpawnData allySpawnData = characterSpawnData.extra as AllySpawnData;
						if (allySpawnData.duration > 0)
						{
							allyControllerBrawler.spawnTime = Time.time;
							allyControllerBrawler.lifeDuration = allySpawnData.duration;
						}
						allyControllerBrawler.oneShot = allySpawnData.oneShot;
						allyControllerBrawler.forcedAttackName = allySpawnData.forcedAttackName;
						allyControllerBrawler.deathAnimOverride = allySpawnData.deathAnimOverride;
					}
					else
					{
						AIControllerBrawler aIControllerBrawler = obj.AddComponent(typeof(AIControllerBrawler)) as AIControllerBrawler;
						DataWarehouse aiMeleeData2 = characterData.TryGetData("//ai_melee_controller", new EmptyDataWarehouse());
						aIControllerBrawler.InitializeFromData(aiMeleeData2);
						aIControllerBrawler.InitializeFromSpawner(this);
					}
				}
				else if (HqController2.Instance != null)
				{
					AIControllerHQ aIControllerHQ = (!(HqController2.Instance is HqControllerTutorial)) ? obj.AddComponent<AIControllerHQ>() : obj.AddComponent<AIControllerHQTutorial>();
					if (characterSpawnData.hqData != null)
					{
						aIControllerHQ.InitializeFromData(characterSpawnData.hqData);
					}
				}
				else
				{
					LoadAICombatController(obj, characterData);
				}
			}
			else if ((type & Type.NPC) != 0)
			{
				GameObject gameObject2 = GameObject.Find("/NPC_RT");
				if (gameObject2 == null)
				{
					gameObject2 = new GameObject("NPC_RT");
				}
				Utils.AttachGameObject(gameObject2, obj);
				if (this is NPCSpawn)
				{
					string text2 = ((NPCSpawn)this).npcName;
					if (string.IsNullOrEmpty(text2))
					{
						text2 = ((NPCSpawn)this).GetNextDefaultName();
					}
					obj.name = text2;
				}
				NPCCommandManager x = obj.AddComponent(typeof(NPCCommandManager)) as NPCCommandManager;
				if (x == null)
				{
					CspUtils.DebugLog("Can't add NPC Command Manager to spawned NPC.");
					return;
				}
				AIControllerNPC aIControllerNPC = obj.AddComponent(typeof(AIControllerNPC)) as AIControllerNPC;
				if (characterSpawnData.npcData != null)
				{
					aIControllerNPC.InitializeFromData(characterSpawnData.npcData);
				}
			}
			else if ((type & Type.Pet) != 0)
			{
				GameObject gameObject3 = GameObject.Find("/NPC_RT");
				if (gameObject3 == null)
				{
					gameObject3 = new GameObject("NPC_RT");
				}
				Utils.AttachGameObject(gameObject3, obj);
				if (this is PetSpawn)
				{
					obj.name = "Fluffy";
				}
				NPCCommandManager x2 = obj.AddComponent(typeof(NPCCommandManager)) as NPCCommandManager;
				if (x2 == null)
				{
					CspUtils.DebugLog("Can't add NPC Command Manager to spawned Pet.");
					return;
				}
				AIControllerPet aIControllerPet = obj.AddComponent(typeof(AIControllerPet)) as AIControllerPet;
				if (characterSpawnData.npcData != null)
				{
					aIControllerPet.InitializeFromData(characterSpawnData.npcData);
				}
			}
			else if (type == Type.Local || type == Type.Remote)
			{
				LoadDummyCombatController(obj, characterData);
			}
			break;
		}
		if (flag)
		{
			type |= Type.Polymorph;
		}
		fxSeqList = (obj.AddComponent(typeof(EffectSequenceList)) as EffectSequenceList);
		DataWarehouse data3 = characterData.GetData("//effect_sequence_list");
		fxSeqList.InitializeFromData(data3, characterAssets, characterSpawnData.characterBundlePath);
		fxSeqList.RequestLoadedCallback(EffectsLoadedCallback, characterSpawnData);
		HairTrafficController hairTrafficController = obj.AddComponent(typeof(HairTrafficController)) as HairTrafficController;
		hairTrafficController.player = obj;
		NetworkComponent networkComponent = null;
		if (characterSpawnData.IsNetworked)
		{
			networkComponent = (obj.AddComponent(typeof(NetworkComponent)) as NetworkComponent);
			if (!characterSpawnData.goNetId.IsValid())
			{
				networkComponent.goNetId = AppShell.Instance.ServerConnection.Game.GetNewDynamicId();
			}
			else
			{
				networkComponent.goNetId = characterSpawnData.goNetId;
			}
			PlayerDictionary.Player value;
			AppShell.Instance.PlayerDictionary.TryGetValue(networkComponent.goNetId.ChildId, out value);
			CspUtils.DebugLog("networkComponent.goNetId.ChildId=" + networkComponent.goNetId.ChildId);
			//int gameUserId = AppShell.Instance.ServerConnection.GetGameUserId();  // added by CSP
			//hairTrafficController.playerId = gameUserId;  // added by CSP

			// the following block commented out by CSP, trying a different method above.
			if (value != null)
			{
				CspUtils.DebugLog("PlayerDictionary value found! value.PlayerId=" + value.PlayerId + " name=" + value.Name + " uid=" + value.UserId);
				//hairTrafficController.playerId = value.PlayerId;  // commented out by CSP
				hairTrafficController.playerId = value.UserId;
			}
			else 
			{
				CspUtils.DebugLog("PlayerDictionary value is null!");
			}
		}
		PolymorphController polymorphController = obj.AddComponent(typeof(PolymorphController)) as PolymorphController;
		polymorphController.Initialize(polymorphSpawnData);
		if (prefabTextBillboard != null)
		{
			GameObject child2 = UnityEngine.Object.Instantiate(prefabTextBillboard) as GameObject;
			Utils.AttachGameObject(obj, child2);
		}
		if (prefabPlayerBillboard != null)
		{
			GameObject gameObject4 = UnityEngine.Object.Instantiate(prefabPlayerBillboard) as GameObject;
			Utils.AttachGameObject(obj, gameObject4);
			Vector3 localPosition = gameObject4.transform.localPosition;
			gameObject4.transform.localPosition = new Vector3(localPosition.x, characterData.TryGetFloat("//character_controller/height", 0f) + characterData.TryGetFloat("//character_controller/playerstatusbillboardoffset", 0.3f), localPosition.z);
			PlayerBillboard component2 = Utils.GetComponent<PlayerBillboard>(gameObject4);
			if (component2 != null)
			{
				component2.Configure();
				hairTrafficController.billboard = component2;
				component2.htc = hairTrafficController;
			}
		}
		if (RecordHistory)
		{
			obj.AddComponent(typeof(DebugHistory));
		}
		CharacterGlobals characterGlobals = obj.AddComponent(typeof(CharacterGlobals)) as CharacterGlobals;
		characterGlobals.definitionData = characterDefinition;
		if (characterSpawnData.IsNetworked)
		{
			if (spawnData.spawnType == (Type.Remote | Type.Player))
			{
				AppShell.Instance.ServerConnection.Game.ForEachNetEntity(delegate(NetGameManager.NetEntity e)
				{
					Physics.IgnoreCollision(obj.collider, e.netComp.gameObject.collider);
				});
			}
			else
			{
				AppShell.Instance.ServerConnection.Game.ForEachNetEntity(Type.Remote | Type.Player, delegate(NetGameManager.NetEntity e)
				{
					Physics.IgnoreCollision(obj.collider, e.netComp.gameObject.collider);
				});
			}
		}
		if ((spawnData.spawnType & Type.NPC) != 0)
		{
			UnityEngine.Object[] array2 = UnityEngine.Object.FindObjectsOfType(typeof(CharacterGlobals));
			UnityEngine.Object[] array3 = array2;
			for (int j = 0; j < array3.Length; j++)
			{
				CharacterGlobals characterGlobals2 = (CharacterGlobals)array3[j];
				if ((characterGlobals2.spawnData.spawnType & Type.Player) != 0)
				{
					Physics.IgnoreCollision(obj.collider, characterGlobals2.gameObject.collider);
				}
			}
		}
		UnityEngine.Object[] array4 = UnityEngine.Object.FindObjectsOfType(typeof(AIControllerNPC));
		UnityEngine.Object[] array5 = array4;
		for (int k = 0; k < array5.Length; k++)
		{
			AIControllerNPC aIControllerNPC2 = (AIControllerNPC)array5[k];
			Collider componentInChildren = aIControllerNPC2.GetComponentInChildren<Collider>();
			if (componentInChildren != null && componentInChildren != obj.collider)
			{
				Physics.IgnoreCollision(obj.collider, componentInChildren);
			}
			else if (componentInChildren == null)
			{
				CspUtils.DebugLog("Cant shut off collision between: " + aIControllerNPC2.name + " and " + obj + ". No collider.");
			}
		}
		if (prefabPushaway != null && (type & Type.Player) == 0 && HqController2.Instance == null)
		{
			GameObject gameObject5 = UnityEngine.Object.Instantiate(prefabPushaway) as GameObject;
			CapsuleCollider capsuleCollider2 = gameObject5.GetComponent(typeof(CapsuleCollider)) as CapsuleCollider;
			if (capsuleCollider2 != null)
			{
				capsuleCollider2.radius = characterController.radius;
				capsuleCollider2.height = characterController.height;
				Vector3 center = characterController.center;
				center.y += characterController.height * 0.15f;
				gameObject5.transform.localPosition = center;
			}
			Utils.AttachGameObject(obj, gameObject5);
		}
		if (characterSpawnData.netExtraData != null && characterSpawnData.netExtraData.Count > 0)
		{
			obj.SendMessage("ProcessNetworkState", characterSpawnData.netExtraData, SendMessageOptions.DontRequireReceiver);
		}
		if ((type & Type.Boss) != 0 || type == Type.LocalPlayer)
		{
			Singleton<VOBundleLoader>.instance.LoadCharacter(@string);
		}
		characterSpawnData.OnSpawnCallback(obj, characterSpawnData);
		if (flag2 && spawnData.modelName != "mr_placeholder" && !(GameController.GetController() is BrawlerController))
		{
			if (type == (Type.Remote | Type.Player))
			{
				PetSpawner.heroCreated(networkComponent.goNetId.childId, obj);
			}
			else
			{
				PetDataManager.changeCurrentPet(PetDataManager.getCurrentPet(), true);
				AppShell.callAnalytics("player", "social_spawn", "character", @string);
			}
			if (networkComponent.goNetId.childId == AppShell.Instance.ServerConnection.GetGameUserId())
			{
				TitleManager.init(networkComponent.goNetId.childId);
			}
			TitleManager.heroCreated(networkComponent.goNetId.childId);
		}
		if (type != Type.LocalPlayer)
		{
			return;
		}
		CspUtils.DebugLog("Local player has spawned, checking for buffs");
		if (GameController.GetController() is SocialSpaceController)
		{
			AppShell.Instance.StartCoroutine(AchievementManager.spawnTrackedAchievements());
			AppShell.Instance.EventMgr.Fire(this, new GlobalNavStateMessage(false));
			if (AppShell.PromoSpawnAllowed)
			{
				AppShell.PromoSpawnAllowed = false;
				CspUtils.DebugLog("char spawn promo");
				SHSSocialMainWindow sHSSocialMainWindow = GUIManager.Instance["SHSMainWindow/SHSSocialMainWindow"] as SHSSocialMainWindow;
				if (sHSSocialMainWindow != null)
				{
					sHSSocialMainWindow.ShowPromoImage();
				}
			}
		}
		PlayerCombatController component3 = obj.GetComponent<PlayerCombatController>();
		if (AppShell.Instance.ExpendablesManager.activeExpendQueue.Count > 0)
		{
			int[] array6 = new int[AppShell.Instance.ExpendablesManager.activeExpendQueue.Count];
			AppShell.Instance.ExpendablesManager.activeExpendQueue.Keys.CopyTo(array6, 0);
			int[] array7 = array6;
			foreach (int key in array7)
			{
				IExpendHandler expendHandler = AppShell.Instance.ExpendablesManager.activeExpendQueue[key];
				if (expendHandler.ExpendableDefinition != null && expendHandler.ExpendableDefinition.CombatEffects.Count > 0)
				{
					foreach (ExpendCombatEffect combatEffect in expendHandler.ExpendableDefinition.CombatEffects)
					{
						CspUtils.DebugLog("Player has a buff potion - activating effect " + combatEffect.name);
						component3.createCombatEffect(combatEffect.name, component3, true);
					}
				}
			}
		}
		if (AppShell.Instance.Profile != null)
		{
			foreach (SpecialAbility brawlerAbility in AppShell.Instance.Profile.brawlerAbilities)
			{
				CspUtils.DebugLog(brawlerAbility);
				if (brawlerAbility is SidekickSpecialAbilityBrawler)
				{
					(brawlerAbility as SidekickSpecialAbilityBrawler).beginBrawler(component3);
				}
			}
		}
		NewTutorialManager.playerSpawnBegun();
	}

	public static void AddExtraComponents(GameObject obj, DataWarehouse characterData, AssetBundle characterAssets)
	{
		d(obj.name + " AddExtraComponents ");
		if (characterData.GetCount("//components") > 0)
		{
			foreach (DataWarehouse item in characterData.GetIterator("//components/component"))
			{
				string @string = item.GetString("class");
				string text = item.TryGetString("target", null);
				string text2 = item.TryGetString("prefab", null);
				try
				{
					GameObject gameObject = obj;
					if (text != null)
					{
						Transform transform = Utils.FindNodeInChildren(obj.transform, text);
						if (transform == null)
						{
							throw new Exception(string.Format("Component target {0} not found", text));
						}
						gameObject = transform.gameObject;
					}
					Component component = gameObject.AddComponent(@string);
					if (component == null)
					{
						throw new Exception(string.Format("Unable to add {0} component.  (component was null)", @string));
					}
					System.Type type = component.GetType();
					if (text2 != null)
					{
						UnityEngine.Object @object = characterAssets.Load(text2);
						GameObject gameObject2 = @object as GameObject;
						if (gameObject2 == null)
						{
							if (@object != null)
							{
								throw new Exception(string.Format("Prefab {0} is a {1} instead of a GameObject", text2, @object.GetType().Name));
							}
							throw new Exception(string.Format("Unable to find prefab {0}", text2));
						}
						Component component2 = gameObject2.GetComponent(@string);
						if (component2 == null)
						{
							throw new Exception(string.Format("Prefab {0} does not contain a component of type {1}", text2, @string));
						}
						FieldInfo[] fields = type.GetFields();
						foreach (FieldInfo fieldInfo in fields)
						{
							fieldInfo.SetValue(component, fieldInfo.GetValue(component2));
						}
					}
					if (item.GetCount("properties") > 0)
					{
						foreach (DataWarehouse item2 in item.GetIterator("properties/property"))
						{
							string string2 = item2.GetString("name");
							string string3 = item2.GetString("value");
							MethodInfo method = type.GetMethod("set_" + string2);
							FieldInfo fieldInfo2 = null;
							System.Type type2 = null;
							if (method != null)
							{
								type2 = method.GetParameters()[0].ParameterType;
							}
							else
							{
								fieldInfo2 = type.GetField(string2);
								if (fieldInfo2 == null)
								{
									CspUtils.DebugLog(string.Format("Property {0} not found while adding component {1}", string2, @string));
									continue;
								}
								type2 = fieldInfo2.FieldType;
							}
							MethodInfo method2 = type2.GetMethod("Parse", new System.Type[1]
							{
								typeof(string)
							});
							if (method2 == null)
							{
								CspUtils.DebugLog(string.Format("Unable to set property {0} due to missing Parse() method while adding component {1}.  (Unsupported type)", string2, @string));
							}
							else
							{
								object obj2 = method2.Invoke(component, new object[1]
								{
									string3
								});
								if (method != null)
								{
									method.Invoke(component, new object[1]
									{
										obj2
									});
								}
								else
								{
									fieldInfo2.SetValue(component, obj2);
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					CspUtils.DebugLog(string.Format("Error adding {0} component to character {1}: {2}", @string, obj.name, ex.Message));
				}
			}
		}
	}

	private void AddLodComponent(GameObject obj, DataWarehouse characterData)
	{
		d(CharacterName + " AddLodComponent ");
		if (obj.GetComponentInChildren<SkinnedMeshRenderer>() != null && obj.GetComponentInChildren<LodCharacter>() == null && characterData.TryGetBool("//lod/enabled", true))
		{
			obj.AddComponent<LodCharacter>();
		}
	}

	protected void OnReadyForPrefab(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		d(CharacterName + " OnReadyForPrefab ");
		CharacterSpawnData characterSpawnData = userData as CharacterSpawnData;
		if (characterSpawnData == null)
		{
			CspUtils.DebugLog("Spawn data is missing");
			return;
		}
		characterSpawnData.assetLoadTransaction = null;
		if (exit != 0)
		{
			return;
		}
		if (characterSpawnData.characterAssets == null)
		{
			CspUtils.DebugLog("Spawn data is missing the asset bundle for character <" + characterSpawnData.ModelName + ">.");
			return;
		}
		if (characterSpawnData.CharacterPrefab == null)
		{
			CspUtils.DebugLog("CharacterPrefab is missing the asset bundle for character <" + characterSpawnData.ModelName + ">.");
			return;
		}
		DataWarehouse characterData = characterSpawnData.characterData;
		AssetBundle characterAssets = characterSpawnData.characterAssets;
		Type type = characterSpawnData.Type;
		CharacterDefinition characterDefinition = new CharacterDefinition();
		characterDefinition.InitializeFromData(characterData);
		string @string = characterData.GetString("//name");
		this.obj = (UnityEngine.Object.Instantiate(characterSpawnData.CharacterPrefab) as GameObject);
		this.obj.name = @string;
		CspUtils.DebugLog("Prefab Instantiated: " + this.obj.name);
		if (BrawlerController.Instance != null && (type & Type.Player) != 0)
		{
			this.obj.layer = 21;
		}
		else
		{
			this.obj.layer = 12;
		}
		this.obj.transform.position = new Vector3(0f, 1000f, 0f);
		SpawnData spawnData = this.obj.AddComponent(typeof(SpawnData)) as SpawnData;
		spawnData.spawnType = type;
		spawnData.modelName = characterSpawnData.ModelName;
		spawnData.sizeRank = characterData.TryGetFloat("//character_model/size_rank", 5f);
		spawnData.sendNewEntityMsg = true;
		//spawnData.spawner = this;  // CSP - added....not sure why this was not already here, or what it aill affect. Seems like kingpin thugs need spawner to be initialized.
		Vector3 vector = characterData.TryGetVector("//character_model/scale", Vector3.one);
		if (vector != Vector3.one)
		{
			this.obj.transform.localScale = vector;
		}
		CspUtils.DebugLog("    OnReadyForPrefab got scale of " + vector + " for " + base.gameObject);
		UnityEngine.Object[] componentsInChildren = this.obj.GetComponentsInChildren(typeof(SkinnedMeshRenderer));
		UnityEngine.Object[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)array[i];
			skinnedMeshRenderer.updateWhenOffscreen = characterData.TryGetBool("//lod/animate_offscreen", false);
		}
		AddLodComponent(this.obj, characterData);
		if (prefabOcclusionDetector != null && type == Type.LocalPlayer)
		{
			GameObject child = UnityEngine.Object.Instantiate(prefabOcclusionDetector) as GameObject;
			Utils.AttachGameObject(this.obj, child);
		}
		Utils.SetLayerTree(this.obj, this.obj.layer);
		if (characterData.TryGetBool("//character_model/disable_shadow", false))
		{
			DisableShadows(this.obj);
		}
		else
		{
			GraphicsOptions.AddPlayerShadow(this.obj);
		}
		float num = characterData.TryGetFloat("//click_box/height", 0f);
		if (num > 0f)
		{
			GameObject original = Resources.Load("Character/ClickBoxCapsule") as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
			gameObject.name = "ClickBox";
			Utils.AttachGameObject(this.obj, gameObject);
			CapsuleCollider capsuleCollider = gameObject.GetComponent(typeof(CapsuleCollider)) as CapsuleCollider;
			capsuleCollider.height = num;
			capsuleCollider.radius = characterData.TryGetFloat("//click_box/radius", num / 2f);
			if (capsuleCollider.radius == capsuleCollider.height / 2f)
			{
				capsuleCollider.height += 0.01f;
			}
			capsuleCollider.center = characterData.TryGetVector("//click_box/center", new Vector3(0f, num / 3f, 0f));
			if (HqController2.Instance != null)
			{
				Rigidbody rigidbody = Utils.AddComponent<Rigidbody>(gameObject);
				rigidbody.useGravity = false;
				rigidbody.isKinematic = true;
			}
		}
		CharacterController characterController = this.obj.AddComponent(typeof(CharacterController)) as CharacterController;
		characterController.height = characterData.TryGetFloat("//character_controller/height", 2f);
		characterController.radius = characterData.TryGetFloat("//character_controller/radius", 0.5f);
		characterController.slopeLimit = characterData.TryGetFloat("//character_controller/slope_limit", 90f);
		characterController.stepOffset = characterData.TryGetFloat("//character_controller/step_offset", 0.3f);
		characterController.center = characterData.TryGetVector("//character_controller/center", new Vector3(0f, 1f, 0f));
		ShsCharacterController shsCharacterController = this.obj.AddComponent(typeof(ShsCharacterController)) as ShsCharacterController;
		shsCharacterController.pickupBone = characterData.TryGetString("//character_controller/pickup_bone", "fx_Rpalm");
		BehaviorManager behaviorManager = this.obj.AddComponent(typeof(BehaviorManager)) as BehaviorManager;
		string text = characterData.TryGetString("//behavior_manager/default_behavior_type", "BehaviorMovement");
		if (text != "BehaviorBase")
		{
			behaviorManager.defaultBehaviorType = text;
		}
		CharacterMotionController characterMotionController = this.obj.AddComponent(typeof(CharacterMotionController)) as CharacterMotionController;
		characterMotionController.SetRestoreLocation(characterSpawnData.Location.position);
		DataWarehouse data = characterData.GetData("//character_motion_controller");
		characterMotionController.InitializeFromData(data);
		if (spawnInSource != null)
		{
			characterMotionController.setSpawnAnimation(new SpawnAnimateData(spawnInSource, base.transform.position));
		}
		Animation component = Utils.GetComponent<Animation>(this.obj, Utils.SearchChildren);
		if (component != null)
		{
			FacialAnimation facialAnimation = Utils.AddComponent<FacialAnimation>(component.gameObject);
			facialAnimation.facialExpression = FacialAnimation.Expression.Normal;
			facialAnimation.defaultFacialExpression = (FacialAnimation.Expression)(int)Enum.Parse(typeof(FacialAnimation.Expression), characterData.TryGetString("//default_facial_expression", "Normal"));
		}
		CharacterStats characterStats = Utils.AddComponent<CharacterStats>(this.obj);
		DataWarehouse data2 = characterData.GetData("//character_stats");
		characterStats.InitializeFromData(data2);
		if (characterData.GetCount("//components") > 0)
		{
			foreach (DataWarehouse item in characterData.GetIterator("//components/component"))
			{
				string string2 = item.GetString("class");
				string text2 = item.TryGetString("target", null);
				string text3 = item.TryGetString("prefab", null);
				try
				{
					GameObject gameObject2 = this.obj;
					if (text2 != null)
					{
						Transform transform = Utils.FindNodeInChildren(this.obj.transform, text2);
						if (transform == null)
						{
							throw new Exception(string.Format("Component target {0} not found", text2));
						}
						gameObject2 = transform.gameObject;
					}
					Component component2 = gameObject2.AddComponent(string2);
					if (component2 == null)
					{
						throw new Exception(string.Format("Unable to add {0} component.  (component was null)", string2));
					}
					System.Type type2 = component2.GetType();
					if (text3 != null)
					{
						UnityEngine.Object @object = characterAssets.Load(text3);
						GameObject gameObject3 = @object as GameObject;
						if (gameObject3 == null)
						{
							if (@object == null)
							{
								throw new Exception(string.Format("Prefab {0} is a {1} instead of a GameObject", text3, @object.GetType().Name));
							}
							throw new Exception(string.Format("Unable to find prefab {0}", text3));
						}
						Component component3 = gameObject3.GetComponent(string2);
						if (component3 == null)
						{
							throw new Exception(string.Format("Prefab {0} does not contain a component of type {1}", text3, string2));
						}
						FieldInfo[] fields = type2.GetFields();
						foreach (FieldInfo fieldInfo in fields)
						{
							fieldInfo.SetValue(component2, fieldInfo.GetValue(component3));
						}
					}
					if (item.GetCount("properties") > 0)
					{
						foreach (DataWarehouse item2 in item.GetIterator("properties/property"))
						{
							string string3 = item2.GetString("name");
							string string4 = item2.GetString("value");
							MethodInfo method = type2.GetMethod("set_" + string3);
							FieldInfo fieldInfo2 = null;
							System.Type type3 = null;
							if (method != null)
							{
								type3 = method.GetParameters()[0].ParameterType;
							}
							else
							{
								fieldInfo2 = type2.GetField(string3);
								if (fieldInfo2 == null)
								{
									CspUtils.DebugLog(string.Format("Property {0} not found while adding component {1}", string3, string2));
									continue;
								}
								type3 = fieldInfo2.FieldType;
							}
							MethodInfo method2 = type3.GetMethod("Parse", new System.Type[1]
							{
								typeof(string)
							});
							if (method2 == null)
							{
								CspUtils.DebugLog(string.Format("Unable to set property {0} due to missing Parse() method while adding component {1}.  (Unsupported type)", string3, string2));
							}
							else
							{
								object obj = method2.Invoke(component2, new object[1]
								{
									string4
								});
								if (method != null)
								{
									method.Invoke(component2, new object[1]
									{
										obj
									});
								}
								else
								{
									fieldInfo2.SetValue(component2, obj);
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					CspUtils.DebugLog(string.Format("Error adding {0} component to character {1}: {2}", string2, this.obj.name, ex.Message));
				}
			}
		}
		if ((type & Type.Boss) != 0)
		{
			LoadAICombatController(this.obj, characterData);
			BossAIControllerBrawler bossAIControllerBrawler = this.obj.AddComponent(typeof(BossAIControllerBrawler)) as BossAIControllerBrawler;
			DataWarehouse data3 = characterData.GetData("//boss_ai_melee_controller");
			bossAIControllerBrawler.InitializeFromData(data3);
		}
		else if ((type & Type.AI) != 0)
		{
			if (BrawlerController.Instance != null)
			{
				LoadAICombatController(this.obj, characterData);
				AIControllerBrawler aIControllerBrawler = this.obj.AddComponent(typeof(AIControllerBrawler)) as AIControllerBrawler;
				DataWarehouse aiMeleeData = characterData.TryGetData("//ai_melee_controller", null);
				aIControllerBrawler.InitializeFromData(aiMeleeData);
			}
			else if (HqController2.Instance != null)
			{
				AIControllerHQ aIControllerHQ = (!(HqController2.Instance is HqControllerTutorial)) ? this.obj.AddComponent<AIControllerHQ>() : this.obj.AddComponent<AIControllerHQTutorial>();
				if (characterSpawnData.hqData != null)
				{
					aIControllerHQ.InitializeFromData(characterSpawnData.hqData);
				}
			}
		}
		else if ((type & Type.NPC) != 0)
		{
			GameObject gameObject4 = GameObject.Find("/NPC_RT");
			if (gameObject4 == null)
			{
				gameObject4 = new GameObject("NPC_RT");
			}
			Utils.AttachGameObject(gameObject4, this.obj);
			if (this is NPCSpawn)
			{
				string text4 = ((NPCSpawn)this).npcName;
				if (string.IsNullOrEmpty(text4))
				{
					text4 = ((NPCSpawn)this).GetNextDefaultName();
				}
				this.obj.name = text4;
			}
			NPCCommandManager x = this.obj.AddComponent(typeof(NPCCommandManager)) as NPCCommandManager;
			if (x == null)
			{
				CspUtils.DebugLog("Can't add NPC Command Manager to spawned NPC.");
				return;
			}
			AIControllerNPC aIControllerNPC = this.obj.AddComponent(typeof(AIControllerNPC)) as AIControllerNPC;
			if (characterSpawnData.npcData != null)
			{
				aIControllerNPC.InitializeFromData(characterSpawnData.npcData);
			}
		}
		else if (type == Type.Local || type == Type.Remote)
		{
			LoadDummyCombatController(this.obj, characterData);
		}
		fxSeqList = (this.obj.AddComponent(typeof(EffectSequenceList)) as EffectSequenceList);
		DataWarehouse data4 = characterData.GetData("//effect_sequence_list");
		fxSeqList.InitializeFromData(data4, characterAssets, characterSpawnData.characterBundlePath);
		fxSeqList.RequestLoadedCallback(EffectsLoadedCallback, characterSpawnData);
		this.obj.AddComponent(typeof(PolymorphController));
		if (prefabTextBillboard != null)
		{
			GameObject child2 = UnityEngine.Object.Instantiate(prefabTextBillboard) as GameObject;
			Utils.AttachGameObject(this.obj, child2);
		}
		if (prefabPlayerBillboard != null)
		{
			GameObject gameObject5 = UnityEngine.Object.Instantiate(prefabPlayerBillboard) as GameObject;
			Utils.AttachGameObject(this.obj, gameObject5);
			Vector3 localPosition = gameObject5.transform.localPosition;
			gameObject5.transform.localPosition = new Vector3(localPosition.x, characterData.TryGetFloat("//character_controller/height", 0f) + 0.3f, localPosition.z);
			PlayerBillboard component4 = Utils.GetComponent<PlayerBillboard>(gameObject5);
			if (component4 != null)
			{
				component4.Configure();
			}
		}
		if (RecordHistory)
		{
			this.obj.AddComponent(typeof(DebugHistory));
		}
		CharacterGlobals characterGlobals = this.obj.AddComponent(typeof(CharacterGlobals)) as CharacterGlobals;
		characterGlobals.definitionData = characterDefinition;
		if (prefabPushaway != null && (type & Type.Player) == 0 && HqController2.Instance == null)
		{
			GameObject gameObject6 = UnityEngine.Object.Instantiate(prefabPushaway) as GameObject;
			CapsuleCollider capsuleCollider2 = gameObject6.GetComponent(typeof(CapsuleCollider)) as CapsuleCollider;
			if (capsuleCollider2 != null)
			{
				capsuleCollider2.radius = characterController.radius;
				capsuleCollider2.height = characterController.height;
				Vector3 center = characterController.center;
				center.y += characterController.height * 0.15f;
				gameObject6.transform.localPosition = center;
			}
			Utils.AttachGameObject(this.obj, gameObject6);
		}
		this.obj.active = false;
		characterSpawnData.OnSpawnCallback(this.obj, characterSpawnData);
	}

	public void SpawnFromPrefab(GameObject prefab, CharacterSpawnData spawnData)
	{
		d(CharacterName + " SpawnFromPrefab ");
		if (spawnData == null)
		{
			CspUtils.DebugLog("Spawn data is missing");
			return;
		}
		Transform transform = spawnData.Location;
		Type type = spawnData.Type;
		PolymorphSpawnData polymorphSpawnData = spawnData.extra as PolymorphSpawnData;
		bool flag = polymorphSpawnData != null;
		if (spawnInSource != null && !flag)
		{
			transform = spawnInSource.transform;
		}
		obj = (UnityEngine.Object.Instantiate(prefab, transform.position, transform.rotation) as GameObject);
		obj.name = prefab.name;
		CspUtils.DebugLog("Prefab Instantiated: " + obj.name);
		Utils.ActivateTree(obj, true);
		obj.hideFlags = (HideFlags)0;
		if (spawnInSource == null && spawnCachedOnGround)
		{
			transform.position = GetSpawnGroundPosition(obj.GetComponent<CharacterController>());
		}
		obj.transform.position = transform.position;
		obj.transform.rotation = transform.rotation;
		CharacterGlobals characterGlobals = obj.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
		CharacterGlobals characterGlobals2 = prefab.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
		if (spawnData.IsNetworked)
		{
			NetworkComponent networkComponent = characterGlobals.networkComponent = (obj.AddComponent(typeof(NetworkComponent)) as NetworkComponent);
			networkComponent.enabled = true;
			if (!spawnData.goNetId.IsValid())
			{
				networkComponent.goNetId = AppShell.Instance.ServerConnection.Game.GetNewDynamicId();
			}
			else
			{
				networkComponent.goNetId = spawnData.goNetId;
			}
		}
		characterGlobals.definitionData = characterGlobals2.definitionData;
		if (characterGlobals.effectsList != null)
		{
			characterGlobals.effectsList.InitializeFromCopy(characterGlobals2.effectsList);
		}
		if (characterGlobals.combatController != null)
		{
			characterGlobals.combatController.InitializeFromCopy(characterGlobals2.combatController);
		}
		if (characterGlobals.motionController != null)
		{
			characterGlobals.motionController.InitializeFromCopy(characterGlobals2.motionController);
			characterGlobals.motionController.SetRestoreLocation(spawnData.Location.position);
		}
		if (characterGlobals.brawlerCharacterAI != null)
		{
			characterGlobals.brawlerCharacterAI.InitializeFromCopy(characterGlobals2.brawlerCharacterAI);
			characterGlobals.brawlerCharacterAI.InitializeFromSpawner(this);
		}
		if (characterGlobals.polymorphController != null)
		{
			characterGlobals.polymorphController.Initialize(polymorphSpawnData);
		}
		if (spawnInSource != null && !flag)
		{
			characterGlobals.motionController.setSpawnAnimation(new SpawnAnimateData(spawnInSource, base.transform.position));
		}
		SpawnData spawnData2 = characterGlobals.spawnData;
		spawnData2.spawnType = type;
		spawnData2.sendNewEntityMsg = !flag;
		spawnData2.rewardsPoints = rewardsPoints;
		spawnData2.spawner = this;
		if (RecordHistory)
		{
			obj.AddComponent(typeof(DebugHistory));
		}
		if (spawnData.IsNetworked && (spawnData2.spawnType & Type.Player) != 0)
		{
			AppShell.Instance.ServerConnection.Game.ForEachNetEntityMask(Type.Player, delegate(NetGameManager.NetEntity e)
			{
				Physics.IgnoreCollision(obj.collider, e.netComp.gameObject.collider);
			});
		}
		if ((spawnData2.spawnType & Type.NPC) == 0)
		{
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(AIControllerNPC));
			UnityEngine.Object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				AIControllerNPC aIControllerNPC = (AIControllerNPC)array2[i];
				Collider componentInChildren = aIControllerNPC.GetComponentInChildren<Collider>();
				if (componentInChildren != null)
				{
					Physics.IgnoreCollision(obj.collider, componentInChildren);
				}
				else
				{
					CspUtils.DebugLog("Cant shut off collision between: " + aIControllerNPC.name + " and " + obj + ". No collider.");
				}
			}
		}
		else
		{
			UnityEngine.Object[] array3 = UnityEngine.Object.FindObjectsOfType(typeof(CharacterGlobals));
			UnityEngine.Object[] array4 = array3;
			for (int j = 0; j < array4.Length; j++)
			{
				CharacterGlobals characterGlobals3 = (CharacterGlobals)array4[j];
				if ((characterGlobals3.spawnData.spawnType & Type.Player) != 0)
				{
					Physics.IgnoreCollision(obj.collider, characterGlobals3.gameObject.collider);
				}
			}
		}
		if (spawnData.netExtraData != null && spawnData.netExtraData.Count > 0)
		{
			obj.SendMessage("ProcessNetworkState", spawnData.netExtraData, SendMessageOptions.DontRequireReceiver);
		}
		if ((spawnData2.spawnType & Type.Boss) != 0 || spawnData2.spawnType == Type.LocalPlayer)
		{
			Singleton<VOBundleLoader>.instance.LoadCharacter(obj.name);
		}
		spawnData.OnSpawnCallback(obj, spawnData);
	}

	public void EffectsLoadedCallback(EffectSequenceList fxList, object extraData)
	{
		d(CharacterName + " EffectsLoadedCallback ");
		if (HaltSpawn)
		{
			return;
		}
		CharacterSpawnData characterSpawnData = extraData as CharacterSpawnData;
		string text = characterSpawnData.characterData.TryGetString("//character_model/effect_name", null);
		if (text != null && obj != null)
		{
			UnityEngine.Object effectSequencePrefabByName = fxList.GetEffectSequencePrefabByName(text);
			if (effectSequencePrefabByName == null)
			{
				CspUtils.DebugLog("No effect prefab found in character assets for <" + text + ">.  Cannot spawn effect.");
			}
			else
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(effectSequencePrefabByName) as GameObject;
				EffectSequence component = Utils.GetComponent<EffectSequence>(gameObject);
				if (component != null && component.AttachToParent)
				{
					component.Initialize(obj, null, null);
				}
				if (obj != null && !obj.active)
				{
					gameObject.active = false;
				}
				Utils.AttachGameObject(obj, gameObject);
			}
		}
		if ((GetSpawnType() & Type.NPC) == 0)
		{
			CreateColliders(characterSpawnData.characterData);
		}
		if (prefabTextBillboard != null)
		{
			UnityEngine.Object effectSequencePrefabByName2 = fxList.GetEffectSequencePrefabByName("_TextBillboard");
			if (effectSequencePrefabByName2 != null)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(effectSequencePrefabByName2) as GameObject;
				Utils.AttachGameObject(fxList.gameObject, gameObject2);
				fxList.gameObject.SendMessage("OnTextBillboardChanged", gameObject2, SendMessageOptions.DontRequireReceiver);
			}
		}
		if (characterSpawnData.prespawnTransaction != null)
		{
			characterSpawnData.prespawnTransaction.CompleteStep("effectsLoaded");
		}
	}

	protected PlayerCombatController LoadPlayerCombatController(GameObject target, DataWarehouse characterData)
	{
		d(CharacterName + " LoadPlayerCombatController ");
		DataWarehouse dataWarehouse = null;
		dataWarehouse = ((characterData.GetCount("//player_combat_controller") <= 0) ? characterData.GetData("//combat_controller") : characterData.GetData("//player_combat_controller"));
		DataWarehouse attackData = null;
		if (characterData.GetCount("//attack_data") > 0)
		{
			attackData = characterData.GetData("//attack_data");
		}
		PlayerCombatController playerCombatController = target.AddComponent(typeof(PlayerCombatController)) as PlayerCombatController;
		playerCombatController.prestigeEffectID = characterData.TryGetString("//prestige_effect_id", "622040");
		playerCombatController.InitializeFromData(dataWarehouse, attackData);
		return playerCombatController;
	}

	protected CombatController LoadDummyCombatController(GameObject target, DataWarehouse characterData)
	{
		d(CharacterName + " LoadDummyCombatController ");
		CombatController combatController = target.AddComponent(typeof(CombatController)) as CombatController;
		DataWarehouse data = characterData.GetData("//combat_controller");
		DataWarehouse attackData = null;
		if (characterData.GetCount("//attack_data") > 0)
		{
			attackData = characterData.GetData("//attack_data");
		}
		combatController.InitializeFromData(data, attackData);
		return combatController;
	}

	protected CombatController LoadAICombatController(GameObject target, DataWarehouse characterData)
	{
		d(CharacterName + " LoadAICombatController ");
		if (characterData.GetCount("//ai_combat_controller") > 0)
		{
			DataWarehouse data = characterData.GetData("//ai_combat_controller");
			DataWarehouse attackData = null;
			if (characterData.GetCount("//attack_data") > 0)
			{
				attackData = characterData.GetData("//attack_data");
			}
			AICombatController aICombatController = target.AddComponent(typeof(AICombatController)) as AICombatController;
			aICombatController.InitializeFromData(data, attackData);
			if (forcedFaction != CombatController.Faction.None)
			{
				aICombatController.faction = forcedFaction;
			}
			return aICombatController;
		}
		DataWarehouse data2 = characterData.GetData("//combat_controller");
		DataWarehouse attackData2 = null;
		if (characterData.GetCount("//attack_data") > 0)
		{
			attackData2 = characterData.GetData("//attack_data");
		}
		if (forceAICombat)
		{
			AICombatController aICombatController2 = target.AddComponent(typeof(AICombatController)) as AICombatController;
			aICombatController2.InitializeFromSpareData(data2, attackData2);
			if (forcedFaction != CombatController.Faction.None)
			{
				aICombatController2.faction = forcedFaction;
			}
			return aICombatController2;
		}
		CombatController combatController = target.AddComponent(typeof(CombatController)) as CombatController;
		combatController.InitializeFromData(data2, attackData2);
		if (forcedFaction != CombatController.Faction.None)
		{
			combatController.faction = forcedFaction;
		}
		return combatController;
	}

	public void CreateColliders(DataWarehouse characterData)
	{
		d(CharacterName + " CreateColliders ");
		if ((!(HqController2.Instance != null) || (GetSpawnType() & Type.AI) == 0) && !(obj == null))
		{
			CombatController combatController = obj.GetComponent(typeof(CombatController)) as CombatController;
			if (combatController == null)
			{
				CspUtils.DebugLog("Cannot find Combat Controller in CreateColliders on <" + obj.name + ">.");
				return;
			}
			combatController.PrecachedInitialize();
			foreach (DataWarehouse item in characterData.GetIterator("//character_model/collider"))
			{
				string @string = item.GetString("name");
				string colliderPrefabName = item.TryGetString("collider_prefab", "_AttackCollider");
				string string2 = item.GetString("node");
				Vector3 offset = item.TryGetVector("offset", new Vector3(0f, 0f, 0f));
				float scale = item.TryGetFloat("scale", 1f);
				if (@string == string2)
				{
					CspUtils.DebugLog(obj.name + " has collider " + @string + " set to same name as node.  Please rename the collider.");
				}
				combatController.addCollider(colliderPrefabName, string2, @string, offset, scale);
			}
		}
	}

	public static void ConnectCameras(GameObject player)
	{
		d(player.name + " ConnectCameras ");
		CameraTargetHelper[] array = Utils.FindObjectsOfType<CameraTargetHelper>();
		GameObject gameObject = null;
		if (array != null && array.Length > 0)
		{
			gameObject = array[0].gameObject;
		}
		if (gameObject == null)
		{
			GameObject original = Resources.Load("Character/CameraTargetHelper") as GameObject;
			gameObject = (UnityEngine.Object.Instantiate(original) as GameObject);
		}
		CameraTargetHelper component = gameObject.GetComponent<CameraTargetHelper>();

		if (component != null)
			CspUtils.DebugLog("component is NOT null!!!");
		else {
			CspUtils.DebugLog("component is null!!!");
			Component [] components = gameObject.GetComponents(typeof(Component));  // CSP - loop over all components because single get not working.
			for (int i = 0; i < components.Length; ++i) {
				//CspUtils.DebugLog("component name = " + components[i].name);
				//CspUtils.DebugLog("component type = " + components[i].GetType());	
				if (components[i].GetType().ToString() == "CameraTargetHelper") {
					CspUtils.DebugLog("component found!");
					component = (CameraTargetHelper)(components[i]);
					break;
				}
			}
		}


		component.SetTarget(player.transform);
		UnityEngine.Object[] array2 = UnityEngine.Object.FindSceneObjectsOfType(typeof(CameraTarget));
		UnityEngine.Object[] array3 = array2;
		for (int i = 0; i < array3.Length; i++)
		{
			CameraTarget cameraTarget = (CameraTarget)array3[i];
			Transform target = cameraTarget.Target;
			if (cameraTarget.Target == null)
			{
				cameraTarget.Target = gameObject.transform;
			}
			else
			{
				SpawnPoint component2 = Utils.GetComponent<SpawnPoint>(cameraTarget.Target);
				if (component2 != null)
				{
					cameraTarget.Target = gameObject.transform;
				}
				else
				{
					CharacterSpawn component3 = Utils.GetComponent<CharacterSpawn>(cameraTarget.Target);
					if (component3 != null)
					{
						cameraTarget.Target = gameObject.transform;
					}
				}
			}
			if (!(cameraTarget.gameObject == null))
			{
				CameraLite component4 = Utils.GetComponent<CameraLite>(cameraTarget.gameObject);
				if (component4 != null && target != cameraTarget.Target)
				{
					component4.Reset();
				}
			}
		}
	}

	protected void FireTransportEffect(EffectSequenceList fxList, object extraData)
	{
		d(CharacterName + " FireTransportEffect ");
		if (!(this == null) && !(base.gameObject == null) && (GetSpawnType() & Type.NPC) == 0)
		{
			EffectSequence effectSequence = null;
			if (spawnFXPrefab != null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(spawnFXPrefab, base.transform.position, base.transform.rotation) as GameObject;
				effectSequence = (gameObject.GetComponent(typeof(EffectSequence)) as EffectSequence);
			}
			if (effectSequence == null)
			{
				effectSequence = fxList.GetLogicalEffectSequence("Transport");
			}
			if (effectSequence != null)
			{
				effectSequence.Initialize(null, null, null);
				effectSequence.StartSequence();
			}
		}
	}

	protected virtual void OnCharacterSpawned(GameObject newCharacter, CharacterSpawnData spawnData)
	{
		d(CharacterName + " OnCharacterSpawned ");
		FinalSpawnSetup(newCharacter, spawnData);
		if (HqController2.Instance == null && doTransportEffect)
		{
			EffectSequenceList effectSequenceList = newCharacter.GetComponent(typeof(EffectSequenceList)) as EffectSequenceList;
			effectSequenceList.RequestLoadedCallback(FireTransportEffect, newCharacter);
		}
		if (DestroyOnSpawn)
		{
			CspUtils.DebugLog("Destorying... " + base.gameObject.name);
			UnityEngine.Object.Destroy(base.gameObject);   
		}
		else
		{
			CharacterGlobals characterGlobals = newCharacter.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
			if (characterGlobals == null)
			{
				CspUtils.DebugLog("Could not find CharacterGlobals when trying to track the character <" + newCharacter.name + "> we just spawned!");
			}
			else
			{
				if (spawnedCharacters.Count == 0)
				{
					AppShell.Instance.EventMgr.AddListener<EntityDespawnMessage>(this, OnTrackedCharacterDespawned);
				}
				spawnedCharacters.Add(characterGlobals);
			}
		}
		if (this.onSpawnCallback != null)
		{
			this.onSpawnCallback(newCharacter);
			this.onSpawnCallback = null;
		}
		IsLocal = isLocalBackup;
	}

	protected virtual void FinalSpawnSetup(GameObject newCharacter, CharacterSpawnData spawnData)
	{
		d(CharacterName + " FinalSpawnSetup ");
		if (spawnData.Type == Type.LocalPlayer)
		{
			newCharacter.tag = "Player";
			if (!(spawnData.extra is PolymorphSpawnData))
			{
				ConnectCameras(newCharacter);
			}
			if (BrawlerController.Instance != null)
			{
				newCharacter.AddComponent<CharacterCollisionTest>();
			}
		}
		CombatController component = Utils.GetComponent<CombatController>(newCharacter);
		if (!(component != null))
		{
			return;
		}
		if (startingEffect != string.Empty && startingEffect != null && newCharacter.active)
		{
			if (component.effectSequenceSource == null)
			{
				component.effectSequenceSource = Utils.GetComponent<EffectSequenceList>(newCharacter);
			}
			component.createCombatEffectRemote(startingEffect, component.gameObject, true);
		}
		if (!wakeUpOnHit)
		{
			component.faction = CombatController.Faction.Neutral;
		}
	}

	protected void OnCharacterPrespawned(GameObject newCharacter, CharacterSpawnData spawnData)
	{
		d(CharacterName + " OnCharacterPrespawned ");
		spawnData.prespawnedCharacter = newCharacter;
		if (spawnData.prespawnTransaction != null)
		{
			spawnData.prespawnTransaction.CompleteStep("assetsLoaded");
		}
	}

	protected virtual void PrespawnComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		d(CharacterName + " PrespawnComplete ");
		CharacterSpawnData characterSpawnData = userData as CharacterSpawnData;
		characterSpawnData.prespawnTransaction = null;
		if (exit == TransactionMonitor.ExitCondition.Success)
		{
			ICharacterCache characterCache = GameController.GetController().CharacterCache;
			if (characterCache != null)
			{
				string spawnName = SpawnName;
				ToggleAIPlayerPrespawn(false);
				characterCache.CacheCharacter(spawnName, characterSpawnData.prespawnedCharacter);
				characterSpawnData.prespawnedCharacter.active = false;
				Utils.ActivateTree(characterSpawnData.prespawnedCharacter, false);
			}
			characterSpawnData.prespawnedCharacter = null;
		}
	}

	protected void OnTrackedCharacterDespawned(EntityDespawnMessage e)
	{
		d(CharacterName + " OnTrackedCharacterDespawned ");
		CharacterGlobals characterGlobals = e.go.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
		if (characterGlobals == null)
		{
			CspUtils.DebugLog("Could not find CharacterGlobals when trying stop tracking the character <" + e.go.name + "> that just despawned!");
			return;
		}
		spawnedCharacters.Remove(characterGlobals);
		if (spawnedCharacters.Count == 0)
		{
			AppShell.Instance.EventMgr.RemoveListener<EntityDespawnMessage>(this, OnTrackedCharacterDespawned);
		}
	}

	public Type GetSpawnType()
	{
		Type type = Type.Unknown;
		if (IsPlayer)
		{
			type |= Type.Player;
		}
		if (IsAI)
		{
			type |= Type.AI;
		}
		if (IsBoss)
		{
			type |= Type.Boss;
		}
		if (IsNpc)
		{
			type = Type.NPC;
		}
		if (IsLocal)
		{
			return type | Type.Local;
		}
		return type | Type.Remote;
	}

	public virtual void Awake()
	{
		d(CharacterName + " Awake isLocal=" + IsLocal);
		foreach (Transform item in base.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		SpawnName = CharacterName;
		spawnedCharacters = new List<CharacterGlobals>();
	}

	public void OnDisable()
	{
		d(CharacterName + " OnDisable ");
		AppShell.Instance.EventMgr.RemoveListener<EntityDespawnMessage>(this, OnTrackedCharacterDespawned);
		spawnedCharacters.Clear();
	}

	public void Start()
	{
		d(CharacterName + " Start " + SpawnOnStart);
		isLocalBackup = IsLocal;
		MeshRenderer meshRenderer = GetComponent(typeof(MeshRenderer)) as MeshRenderer;
		if (meshRenderer != null)
		{
			meshRenderer.enabled = false;
		}
		if (CharacterName == null || CharacterName == string.Empty)
		{
			CspUtils.DebugLog("Spawner <" + base.gameObject.name + "? has an empty character name.");
			return;
		}
		Type spawnType = GetSpawnType();
		if (spawnType == Type.LocalPlayer)
		{
			d("Start() - type is player and local");
			GameObject gameObject = GameObject.FindGameObjectWithTag("GameController");
			if (gameObject != null)
			{
				GameController gameController = gameObject.GetComponent(typeof(GameController)) as GameController;
				if (gameController != null)
				{
					gameController.AddPlayerCharacterSpawner(this);
				}
			}
			else
			{
				CspUtils.DebugLog("No GameController found in this scene.  Unable to spawn player character.");
			}
			if (ForceLocalPlayerSpawnOnStart)
			{
				SpawnOnStart = true;
			}
			else
			{
				SpawnOnStart = false;
			}
		}
		if (SpawnOnStart)
		{
			StartCoroutine(SpawnOnNextFrame());
		}
		spawnerNetwork = (GetComponent(typeof(NetworkComponent)) as NetworkComponent);
	}

	public void SpawnOnTime(float wait)
	{
		StartCoroutine(spawnOnTime(wait));
	}

	private IEnumerator spawnOnTime(float wait)
	{
		yield return new WaitForSeconds(wait);
		StartCoroutine(SpawnOnNextFrame());
	}

	protected IEnumerator SpawnOnNextFrame()
	{
		d(CharacterName + " SpawnOnNextFrame");
		yield return 0;
		StartCoroutine(Spawn(CharacterName, 1, base.transform, GetSpawnType(), netExtraData, OnCharacterSpawned, IsNetworked, null));
	}

	public void SpawnWithID(GoNetId goNetId)
	{
		d(CharacterName + " SpawnWithID " + goNetId);
		StartCoroutine(spawnWithID(goNetId));
	}

	protected IEnumerator spawnWithID(GoNetId goNetId)
	{
		yield return 0;
		this.goNetId = goNetId;
		d(CharacterName + " spawnWithID " + goNetId);
		yield return StartCoroutine(Spawn(CharacterName, 1, base.transform, GetSpawnType(), netExtraData, OnCharacterSpawned, IsNetworked, null));
	}

	public void SpawnCharacter(string characterName, int r2Attack, Transform destination, object extra)
	{
		d(characterName + " SpawnCharacter ");
		if (characterName == null || characterName == string.Empty)
		{
			CspUtils.DebugLog("Invalid character name");
		}
		else
		{
			StartCoroutine(Spawn(characterName, r2Attack, destination, GetSpawnType(), netExtraData, OnCharacterSpawned, IsNetworked, extra));
		}
	}

	public void SpawnWithData(object extra)
	{
		StartCoroutine(Spawn(CharacterName, R2Attack, base.transform, GetSpawnType(), netExtraData, OnCharacterSpawned, IsNetworked, extra));
	}

	public void SpawnPlayerCharacter(string characterName)
	{
		d(characterName + " SpawnPlayerCharacter ");
		if (!(this == null) && !(base.gameObject == null))
		{
			if (characterName == null || characterName == string.Empty)
			{
				CspUtils.DebugLog("Invalid character name");
			}
			else
			{
				StartCoroutine(Spawn(characterName, 1, base.transform, Type.LocalPlayer, netExtraData, OnCharacterSpawned, IsNetworked, null));
			}
		}
	}

	public static void SpawnAlly(string characterName, Vector3 spawnPosition, CombatController.Faction faction, int rAttack, int duration = -1, bool oneShot = false, string forcedAttackName = "", string deathAnimOverride = "")
	{
		GameObject gameObject = new GameObject();
		gameObject.transform.localPosition = spawnPosition + new Vector3(0f, 1f, 0f);
		CharacterSpawn characterSpawn = gameObject.AddComponent<CharacterSpawn>();
		characterSpawn.forceFaction(faction);
		AllySpawnData allySpawnData = new AllySpawnData(duration, oneShot, forcedAttackName, deathAnimOverride);
		characterSpawn.SpawnAllyCharacter(new CharacterSelectionBlock(characterName, rAttack), allySpawnData);
	}

	public void SpawnAllyCharacter(CharacterSelectionBlock characterData, AllySpawnData allySpawnData)
	{
		d(characterData.name + " SpawnAllyCharacter ");
		if (characterData.name == null || characterData.name == string.Empty)
		{
			CspUtils.DebugLog("Invalid character name");
		}
		else
		{
			StartCoroutine(Spawn(characterData.name, characterData.r2Attack, base.transform, Type.Local | Type.AI | Type.Ally, netExtraData, OnCharacterSpawned, IsNetworked, allySpawnData));
		}
	}

	public void SpawnPlayerCharacter(CharacterSelectionBlock characterData)
	{
		d(characterData.name + " SpawnPlayerCharacter ");
		if (characterData.name == null || characterData.name == string.Empty)
		{
			CspUtils.DebugLog("Invalid character name");
		}
		else
		{
			StartCoroutine(Spawn(characterData.name, characterData.r2Attack, base.transform, Type.LocalPlayer, netExtraData, OnCharacterSpawned, IsNetworked, null));
		}
	}

	public void Triggered(UnityEngine.Object trigger)
	{
		d(CharacterName + " Triggered ");
		Type spawnType = GetSpawnType();
		StartCoroutine(Spawn(CharacterName, 1, base.transform, spawnType, netExtraData, OnCharacterSpawned, IsNetworked, null));
	}

	public void TriggerGroupAggro(UnityEngine.Object triggeror)
	{
		d(CharacterName + " TriggerGroupAggro ");
		PlayerCombatController playerCombatController = null;
		GameObject gameObject = triggeror as GameObject;
		if (gameObject != null)
		{
			playerCombatController = (gameObject.GetComponent(typeof(PlayerCombatController)) as PlayerCombatController);
		}
		if (playerCombatController != null)
		{
			foreach (CharacterGlobals spawnedCharacter in spawnedCharacters)
			{
				if (spawnedCharacter != null)
				{
					spawnedCharacter.gameObject.SendMessage("HitByEnemy", playerCombatController, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	public void DestroyAllSpawnedCharacters()
	{
		foreach (CharacterGlobals spawnedCharacter in spawnedCharacters)
		{
			CspUtils.DebugLog("Destorying... " + base.gameObject.name);
			spawnedCharacter.combatController.killed(base.gameObject, 0.4f);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (base.gameObject.transform.parent != null)
		{
			SpawnController spawnController = base.gameObject.transform.parent.GetComponent(typeof(SpawnController)) as SpawnController;
			if (spawnController != null)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawLine(base.gameObject.transform.position, spawnController.transform.position);
				for (int i = 0; i < spawnController.transform.GetChildCount(); i++)
				{
					GameObject gameObject = spawnController.transform.GetChild(i).gameObject;
					Gizmos.color = Color.yellow;
					Gizmos.DrawLine(gameObject.transform.position, base.gameObject.transform.position);
				}
			}
			SpawnGroup spawnGroup = base.gameObject.transform.parent.GetComponent(typeof(SpawnGroup)) as SpawnGroup;
			if (spawnGroup != null)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(base.gameObject.transform.position, spawnGroup.transform.position);
				for (int j = 0; j < spawnGroup.transform.GetChildCount(); j++)
				{
					GameObject gameObject2 = spawnGroup.transform.GetChild(j).gameObject;
					Gizmos.color = Color.green;
					Gizmos.DrawLine(gameObject2.transform.position, base.gameObject.transform.position);
				}
			}
		}
		if (spawnInSource != null)
		{
			Vector3 upVector = default(Vector3);
			Vector3 atVector = default(Vector3);
			float vectorLength = 0f;
			BrawlerSpawnSource.InitMotionVectors(spawnInSource, base.transform.position, ref upVector, ref atVector, ref vectorLength);
			int num = (int)Mathf.Min(Mathf.Max(Mathf.Ceil(vectorLength / 0.5f), 1f), 40f);
			float num2 = 1f / (float)num;
			Gizmos.color = Color.blue;
			for (int k = 0; k < num; k++)
			{
				float curveTime = (float)k * num2;
				float curveTime2 = (float)(k + 1) * num2;
				Vector3 from = BrawlerSpawnSource.EvalPosition(curveTime, spawnInSource.transform.position, upVector, atVector);
				Vector3 to = BrawlerSpawnSource.EvalPosition(curveTime2, spawnInSource.transform.position, upVector, atVector);
				Gizmos.DrawLine(from, to);
			}
		}
		if (overrideAggroDistance)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(base.transform.position, aggroDistance);
		}
	}

	public virtual void CloneSpawner(CharacterSpawn target)
	{
		target.prefabTextBillboard = prefabTextBillboard;
		target.prefabPlayerBillboard = prefabPlayerBillboard;
		target.prefabOcclusionDetector = prefabOcclusionDetector;
		target.prefabBlobShadow = prefabBlobShadow;
		target.prefabPushaway = prefabPushaway;
		target.spawnFXPrefab = spawnFXPrefab;
		target.spawnInSource = spawnInSource;
		target.SpawnName = SpawnName;
		target.startingEffect = startingEffect;
	}

	public virtual void SetCharacterName(string newName)
	{
		d(newName + " SetCharacterName ");
		CharacterName = newName;
		SpawnName = newName;
	}

	public void ToggleAIPlayerPrespawn(bool on)
	{
		d(CharacterName + " ToggleAIPlayerPrespawn isAIPlayer=" + isAIPlayer + " IsPlayer=" + IsPlayer + "on=" + on);
		if (on && !isAIPlayer)
		{
			if (IsPlayer)
			{
				IsAI = (isAIPlayer = on);
				IsPlayer = false;
				SpawnName = "evil_" + CharacterName;
			}
		}
		else if (isAIPlayer)
		{
			IsAI = (isAIPlayer = on);
			IsPlayer = true;
			SpawnName = CharacterName;
		}
	}

	public bool CheckPlayerCount()
	{
		if (PlayerCombatController.PlayerList == null)
		{
			return true;
		}
		bool result = true;
		switch (PlayerCombatController.GetPlayerCount())
		{
		case 1:
			result = Players1;
			break;
		case 2:
			result = Players2;
			break;
		case 3:
			result = Players3;
			break;
		case 4:
			result = Players4;
			break;
		}
		return result;
	}

	protected bool CheckSpawnPosition(Vector3 spawnPosition, SpawnPositionBlockData blockData)
	{
		d(CharacterName + " CheckSpawnPosition ");
		if (blockData.spawnPositionBlocker != null)
		{
			blockData.spawnPositionBlockRange -= SpawnPositionBlockData.SPAWN_POSITION_BLOCK_RANGE_DECREMENT;
			if (blockData.spawnPositionBlockRange < 0f)
			{
				blockData.spawnPositionBlockRange = 0f;
			}
			if (IsSpawnPositionBlockedByEntity(spawnPosition, blockData.spawnPositionBlocker, blockData.spawnPositionBlockRange))
			{
				return Time.time - blockData.spawnPositionBlockTime >= SpawnPositionBlockData.SPAWN_POSITION_BLOCK_TIME_OUT;
			}
			blockData.spawnPositionBlocker = null;
		}
		if (IsSpawnPositionBlockedByEntity(spawnPosition, CombatController.GetFactionList(CombatController.Faction.Enemy), blockData))
		{
			return false;
		}
		if (IsSpawnPositionBlockedByEntity(spawnPosition, CombatController.GetFactionList(CombatController.Faction.Player), blockData))
		{
			return false;
		}
		return true;
	}

	public Vector3 GetSpawnGroundPosition(CharacterController charControl)
	{
		d(CharacterName + " GetSpawnGroundPosition ");
		if (!FindSpawnRayPoint(charControl.gameObject.transform.position))
		{
			CspUtils.DebugLog("Failed to hit ground when trying to determine spawn ground position for spawner <" + base.gameObject.name + "> set to spawn <" + charControl.gameObject.name + ">");
			return charControl.gameObject.transform.position;
		}
		return ShsCharacterController.FindPositionOnGround(charControl, spawnRayPoint, spawnRayPointOffset);
	}

	public Vector3 GetSpawnPosition(CharacterSpawnData spawnData)
	{
		d(CharacterName + " GetSpawnPosition ");
		Vector3 result = default(Vector3);
		if (spawnData == null || spawnData.Location == null)
		{
			return result;
		}
		result = spawnData.Location.position;
		PolymorphSpawnData polymorphSpawnData = spawnData.extra as PolymorphSpawnData;
		bool flag = polymorphSpawnData != null;
		if (spawnInSource != null && !flag)
		{
			result = spawnInSource.transform.position;
		}
		if (BrawlerController.Instance != null && (spawnData.Type & Type.AI) != 0 && spawnInSource == null && spawnCachedOnGround && FindSpawnRayPoint(result))
		{
			result = spawnRayPoint;
		}
		return result;
	}

	protected bool FindSpawnRayPoint(Vector3 origin)
	{
		if (!spawnRayPointFound)
		{
			Vector3 position = origin + Vector3.up * (groundRayLength / 2f);
			RaycastHit hitInfo;
			if (ShsCharacterController.FindGround(position, groundRayLength, out hitInfo))
			{
				spawnRayPoint = hitInfo.point;
				spawnRayPointFound = true;
			}
		}
		return spawnRayPointFound;
	}

	protected bool IsSpawnPositionBlockedByEntity(Vector3 spawnPosition, CombatController combatEntity, float blockRange)
	{
		if (combatEntity == null || combatEntity.isKilled)
		{
			return false;
		}
		SpawnData component = combatEntity.GetComponent<SpawnData>();
		if (component == null || (component.spawnType & Type.AI) == 0)
		{
			return false;
		}
		float num = (combatEntity.transform.position - spawnPosition).sqrMagnitude;
		CharacterController component2 = combatEntity.GetComponent<CharacterController>();
		if (component2 != null)
		{
			num -= component2.radius;
		}
		return num <= blockRange * blockRange;
	}

	protected bool IsSpawnPositionBlockedByEntity(Vector3 spawnPosition, List<CombatController> combatEntities, SpawnPositionBlockData blockData)
	{
		if (combatEntities != null)
		{
			foreach (CombatController combatEntity in combatEntities)
			{
				if (IsSpawnPositionBlockedByEntity(spawnPosition, combatEntity, blockData.spawnPositionBlockRange))
				{
					blockData.spawnPositionBlocker = combatEntity;
					blockData.spawnPositionBlockRange = 1f;
					return true;
				}
			}
		}
		return false;
	}

	protected virtual void OnBecomeOwner()
	{
		if (!(BrawlerController.Instance == null) && IsAI && !AppShell.Instance.ServerConnection.IsGameHost())
		{
			AppShell.Instance.ServerConnection.Game.ReleaseOwnership(base.gameObject);
		}
	}

	protected void DisableShadows(GameObject character)
	{
		if (character != null)
		{
			SkinnedMeshRenderer[] componentsInChildren = character.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				skinnedMeshRenderer.castShadows = false;
			}
		}
	}
}
