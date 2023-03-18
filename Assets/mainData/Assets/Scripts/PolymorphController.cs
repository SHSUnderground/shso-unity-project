using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolymorphController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum ActionLevel
	{
		Normal,
		Warning,
		Error
	}

	private delegate void DelayStartPolymorph(string[] msgArgs);

	public string polymorphForm;

	public float polymorphDelay;

	public string polymorphEffect;

	public float revertDelay;

	public string revertEffect = string.Empty;

	public string revertDeathEffect = string.Empty;

	public GameObject polymorphObject;

	public GameObject originalObject;

	public string polymorphCombatEffect = string.Empty;

	protected CharacterGlobals charGlobals;

	protected CharacterGlobals polymorphGlobals;

	protected CharacterGlobals originalGlobals;

	private ShsFSM _factionFSM;

	private ShsFSM _polymorphFSM;

	private CharacterSpawn _polymorphSpawner;

	private string _originalCharacterName = string.Empty;

	private bool _endOriginalPolymorph;

	public void Start()
	{
		charGlobals = GetComponent<CharacterGlobals>();
		PolymorphStateProxy proxy = new PolymorphStateProxy(this);
		_factionFSM = new ShsFSM();
		_factionFSM.AddState(new PolymorphNullState(proxy));
		_factionFSM.AddState(new PolymorphStartState(proxy));
		_factionFSM.AddState(new PolymorphFactionState(proxy));
		_factionFSM.AddState(new PolymorphEndState(proxy));
		_factionFSM.GotoState<PolymorphNullState>();
		proxy = new PolymorphStateProxy(this);
		_polymorphFSM = new ShsFSM();
		_polymorphFSM.AddState(new PolymorphNullState(proxy));
		_polymorphFSM.AddState(new PolymorphStartState(proxy));
		_polymorphFSM.AddState(new PolymorphSpawnState(proxy));
		_polymorphFSM.AddState(new PolymorphCharacterState(proxy));
		_polymorphFSM.AddState(new PolymorphEndState(proxy));
		_polymorphFSM.AddState(new RevertCharacterState(proxy));
		_polymorphFSM.AddState(new PolymorphRemoteState(proxy));
		_polymorphFSM.AddState(new RevertCharacterOnDeathState(proxy));
		_polymorphFSM.AddState(new RevertRemoteState(proxy));
		_polymorphFSM.GotoState<PolymorphNullState>();
		if (originalObject != null)
		{
			InitializeFromOriginal();
		}
	}

	public void Update()
	{
		if (IsPolymorphed())
		{
			UpdateFSM(_polymorphFSM);
		}
		if (_factionFSM.GetCurrentState() != typeof(PolymorphNullState))
		{
			UpdateFSM(_factionFSM);
		}
	}

	private void OnDisable()
	{
		if (polymorphObject != null && polymorphObject.active)
		{
			GameObject gameObject = polymorphObject;
			CharacterGlobals characterGlobals = polymorphGlobals;
			RemovePolymorphLink();
			if (characterGlobals != null && characterGlobals.spawnData != null && Utils.IsCharacterSpawned(gameObject))
			{
				LogPolymorphAction(base.gameObject, charGlobals, gameObject, characterGlobals, "DESPAWNING POLYMORPH");
				characterGlobals.spawnData.Despawn(EntityDespawnMessage.despawnType.polymorph, false);
			}
			else
			{
				LogPolymorphAction(base.gameObject, charGlobals, gameObject, characterGlobals, "DESTROYING POLYMORPH");
				Utils.DelayedDestroy(gameObject);
			}
		}
		if (!(originalObject != null) || !originalObject.active)
		{
			return;
		}
		GameObject gameObject2 = originalObject;
		CharacterGlobals characterGlobals2 = originalGlobals;
		if (characterGlobals2 != null && characterGlobals2.polymorphController != null)
		{
			characterGlobals2.polymorphController.RemovePolymorphLink();
		}
		if (characterGlobals2 != null && characterGlobals2.spawnData != null && Utils.IsCharacterSpawned(gameObject2))
		{
			if (characterGlobals2.polymorphController != null && characterGlobals2.polymorphController.IsAPolymorph())
			{
				LogPolymorphAction(base.gameObject, charGlobals, gameObject2, characterGlobals2, "DESPAWNING ORIGINAL as POLYMORPH");
				characterGlobals2.spawnData.Despawn(EntityDespawnMessage.despawnType.polymorph, false);
			}
			else
			{
				LogPolymorphAction(base.gameObject, charGlobals, gameObject2, characterGlobals2, "DESPAWNING ORIGINAL as DEFEATED");
				characterGlobals2.spawnData.Despawn(EntityDespawnMessage.despawnType.defeated);
			}
		}
		else
		{
			LogPolymorphAction(base.gameObject, charGlobals, gameObject2, characterGlobals2, "DESTROYING ORIGINAL");
			Utils.DelayedDestroy(gameObject2);
		}
	}

	public void Initialize(PolymorphSpawnData polymorphData)
	{
		if (polymorphData != null)
		{
			originalObject = polymorphData.Original;
			originalGlobals = polymorphData.Original.GetComponent<CharacterGlobals>();
			CombatController component = base.gameObject.GetComponent<CombatController>();
			component.HideCombatant(false);
		}
	}

	public void StartPolymorphToObject(string[] msgArgs)
	{
		if (IsPolymorphed() || charGlobals.combatController.isKilled)
		{
			return;
		}
		if (msgArgs == null || msgArgs.Length <= 0)
		{
			CspUtils.DebugLog("Failed to start polymorph - no arguments supplied");
			return;
		}
		polymorphDelay = 0f;
		revertDelay = 0f;
		polymorphEffect = string.Empty;
		revertEffect = string.Empty;
		revertDeathEffect = string.Empty;
		polymorphForm = msgArgs[0];
		if (msgArgs.Length > 1)
		{
			polymorphDelay = float.Parse(msgArgs[1]);
		}
		if (msgArgs.Length > 2)
		{
			revertDelay = float.Parse(msgArgs[2]);
		}
		if (msgArgs.Length > 3)
		{
			polymorphEffect = msgArgs[3];
		}
		if (msgArgs.Length > 4)
		{
			revertEffect = msgArgs[4];
		}
		if (msgArgs.Length > 5)
		{
			revertDeathEffect = msgArgs[5];
		}
		if (revertDeathEffect == string.Empty)
		{
			revertDeathEffect = revertEffect;
		}
		CombatController combatController = null;
		CombatEffectMessage componentInChildren = GetComponentInChildren<CombatEffectMessage>();
		if (componentInChildren != null)
		{
			combatController = componentInChildren.getSourceCombatController();
		}
		PolymorphCharacterStateData polymorphCharacterStateData = new PolymorphCharacterStateData();
		polymorphCharacterStateData.Initialize(charGlobals, polymorphEffect, revertEffect, polymorphForm, false, string.Empty, combatController.gameObject);
		PolymorphBaseState polymorphBaseState = _polymorphFSM.GetCurrentStateObject() as PolymorphBaseState;
		PolymorphStartState state = _polymorphFSM.GetState<PolymorphStartState>();
		PolymorphEndState state2 = _polymorphFSM.GetState<PolymorphEndState>();
		RevertCharacterOnDeathState state3 = _polymorphFSM.GetState<RevertCharacterOnDeathState>();
		polymorphBaseState.stateProxy.Data = polymorphCharacterStateData;
		state.Initialize(polymorphDelay);
		state2.Initialize(revertDelay);
		state3.Initialize(revertDeathEffect);
		_polymorphFSM.GotoState<PolymorphStartState>();
	}

	public void StartPolymorphToCharacter(string[] msgArgs)
	{
		StartPolymorphToCharacterInternal(msgArgs, false);
	}

	public void StartPolymorphToPlayerCharacter(string[] msgArgs)
	{
		StartPolymorphToCharacterInternal(msgArgs, true);
	}

	public void StartPolymorphToFaction(string[] msgArgs)
	{
		if (charGlobals == null)
		{
			StartCoroutine(StartPolymorphOnDelay(0.01f, StartPolymorphToFaction, msgArgs));
			return;
		}
		if (msgArgs == null || msgArgs.Length < 3)
		{
			int num = 0;
			if (msgArgs != null)
			{
				num = msgArgs.Length;
			}
			CspUtils.DebugLog("In StartPolymorphToFaction: Need 3 arguments to change faction. <" + num + "> arguments supplied");
			return;
		}
		if (msgArgs.Length >= 4)
		{
			float num2 = 1f;
			float result = 0f;
			if (float.TryParse(msgArgs[3], out result))
			{
				num2 = result;
			}
			if (num2 < 1f)
			{
				CspUtils.DebugLog("polymorph chance to apply is " + num2);
				if (Random.Range(0f, 1f) > num2)
				{
					CspUtils.DebugLog("Polymorph failed chance to apply");
					return;
				}
			}
		}
		CombatController.Faction faction = CombatController.Faction.None;
		string a = msgArgs[0].ToLower();
		if (a == "player")
		{
			faction = CombatController.Faction.Player;
		}
		else if (a == "enemy")
		{
			faction = CombatController.Faction.Enemy;
		}
		else if (a == "neutral")
		{
			faction = CombatController.Faction.Neutral;
		}
		else if (a == "environment")
		{
			faction = CombatController.Faction.Environment;
		}
		if (faction == CombatController.Faction.None)
		{
			CspUtils.DebugLog("In StartPolymorphToFaction: Faction type <" + msgArgs[0] + "> unknown");
			return;
		}
		CombatController charmer = null;
		CombatEffectMessage componentInChildren = GetComponentInChildren<CombatEffectMessage>();
		if (componentInChildren != null)
		{
			charmer = componentInChildren.getSourceCombatController();
		}
		PolymorphFactionStateData polymorphFactionStateData = new PolymorphFactionStateData();
		polymorphFactionStateData.Initialize(charGlobals, faction, bool.Parse(msgArgs[1]), bool.Parse(msgArgs[2]), charmer);
		PolymorphBaseState polymorphBaseState = _factionFSM.GetCurrentStateObject() as PolymorphBaseState;
		polymorphBaseState.stateProxy.Data = polymorphFactionStateData;
		_factionFSM.GotoState<PolymorphStartState>();
	}

	public void EndPolymorph()
	{
		if (!IsPolymorphed() || polymorphObject == null)
		{
			if (charGlobals != null && charGlobals.combatController != null && !charGlobals.combatController.isKilled)
			{
				LogPolymorphError(base.gameObject, charGlobals, polymorphObject, polymorphGlobals, "START REVERT FAIL - not polymorphed");
			}
			return;
		}
		if (_polymorphFSM.GetCurrentState() != typeof(PolymorphCharacterState))
		{
			LogPolymorphError(base.gameObject, charGlobals, polymorphObject, polymorphGlobals, "START REVERT FAIL - not in polymorph character state");
			return;
		}
		if (polymorphGlobals != null && polymorphGlobals.polymorphController != null && polymorphGlobals.polymorphController.IsPolymorphed())
		{
			polymorphGlobals.polymorphController._endOriginalPolymorph = true;
			return;
		}
		if (polymorphGlobals != null && polymorphGlobals.brawlerCharacterAI != null)
		{
			polymorphGlobals.brawlerCharacterAI.LockOwnership(true);
		}
		LogPolymorphAction(base.gameObject, charGlobals, polymorphObject, polymorphGlobals, "START REVERT");
		_polymorphFSM.GotoState<PolymorphEndState>();
	}

	public void EndPolymorphOnDeath()
	{
		if (!IsPolymorphed())
		{
			LogPolymorphAction(base.gameObject, charGlobals, polymorphObject, polymorphGlobals, "START DEATH REVERT - not polymorphed");
			return;
		}
		if (polymorphGlobals != null)
		{
			polymorphGlobals.polymorphController.EndPolymorphOnDeath();
		}
		if (polymorphGlobals != null && polymorphGlobals.brawlerCharacterAI != null)
		{
			polymorphGlobals.brawlerCharacterAI.LockOwnership(true);
		}
		LogPolymorphAction(base.gameObject, charGlobals, polymorphObject, polymorphGlobals, "START DEATH REVERT");
		_polymorphFSM.GotoState<RevertCharacterOnDeathState>();
	}

	public void EndPolymorphToFaction()
	{
		if (_factionFSM.GetCurrentState() != typeof(PolymorphNullState))
		{
			_factionFSM.GotoState<PolymorphEndState>();
		}
	}

	public GameObject SpawnPolymorphObject(string formPrefab)
	{
		Vector3 position = base.gameObject.transform.position;
		position.y += 100f;
		polymorphObject = SpawnPolymorphObject(formPrefab, position, base.gameObject.transform.rotation);
		if (polymorphObject == null)
		{
			CspUtils.DebugLog("Polymorph object was unable to be created from prefab: " + formPrefab);
			return null;
		}
		charGlobals.networkComponent.AnnounceObjectSpawn(polymorphObject, "PolymorphController", formPrefab);
		return polymorphObject;
	}

	public void SpawnPolymorphCharacter(string form, CharacterSpawn spawner, string combatEffect)
	{
		PolymorphSpawn polymorphSpawn = spawner as PolymorphSpawn;
		if (polymorphSpawn != null)
		{
			polymorphSpawn.AddForm(form, combatEffect, charGlobals.combatController.faction);
		}
		_polymorphSpawner = spawner;
		_originalCharacterName = spawner.CharacterName;
		PolymorphSpawnData extra = new PolymorphSpawnData(base.gameObject);
		spawner.doTransportEffect = false;
		spawner.SetCharacterName(form);
		spawner.onSpawnCallback += OnSpawnPolymorphCharacter;
		spawner.SpawnWithData(extra);
	}

	public GameObject RemoteSpawn(Vector3 formPosition, Quaternion formRotation, GoNetId formId, string formPrefab, GameObject parent)
	{
		polymorphObject = SpawnPolymorphObject(formPrefab, formPosition, base.gameObject.transform.rotation);
		if (polymorphObject == null)
		{
			CspUtils.DebugLog("Polymorph object was unable to be created from prefab: " + formPrefab);
			return null;
		}
		if (formId.IsValid())
		{
			NetworkComponent component = polymorphObject.GetComponent<NetworkComponent>();
			if (component != null)
			{
				component.goNetId = formId;
			}
		}
		return polymorphObject;
	}

	public void RemoteSpawnPolymorphCharacter(string formName, CharacterSpawn spawner)
	{
		LogPolymorphAction(base.gameObject, charGlobals, "REMOTE POLYMORPH SPAWN <" + formName + ">");
		SpawnPolymorphCharacter(formName, spawner, polymorphCombatEffect);
	}

	public void RemotePolymorphCharacter()
	{
		if (IsPolymorphed())
		{
			LogPolymorphAction(base.gameObject, charGlobals, polymorphObject, polymorphGlobals, "REMOTE POLYMORPH");
			PolymorphBaseState polymorphBaseState = _polymorphFSM.GetCurrentStateObject() as PolymorphBaseState;
			polymorphBaseState.SetPolymorph(polymorphObject);
			polymorphBaseState.RemotePolymorphEnabled = true;
		}
		else
		{
			LogPolymorphAction(base.gameObject, charGlobals, polymorphObject, polymorphGlobals, "REMOTE POLYMORPH - received but not polymorphed");
		}
	}

	public void RemoteRevert()
	{
		if (IsPolymorphed())
		{
			LogPolymorphAction(base.gameObject, charGlobals, polymorphObject, polymorphGlobals, "REMOTE REVERT");
			_polymorphFSM.GotoState<RevertCharacterState>();
		}
		else
		{
			LogPolymorphAction(base.gameObject, charGlobals, polymorphObject, polymorphGlobals, "REMOTE REVERT - received but not polymorphed");
		}
	}

	public void OnDestroyPolymorph(GameObject obj)
	{
		if (obj != null && polymorphObject == obj)
		{
			if (polymorphGlobals == null)
			{
				AppShell.Instance.EventMgr.RemoveListener<PickupMessage>(OnPickup);
			}
			else
			{
				AppShell.Instance.EventMgr.RemoveListener<EntityDespawnMessage>(OnEntityDespawn);
			}
			RemovePolymorphLink();
		}
	}

	public void OnEndPolymorph()
	{
		if (_endOriginalPolymorph)
		{
			if (IsAPolymorph() && originalGlobals != null && originalGlobals.polymorphController != null)
			{
				originalGlobals.polymorphController.EndPolymorph();
			}
			_endOriginalPolymorph = false;
		}
	}

	public bool IsPolymorphed()
	{
		return _polymorphFSM != null && _polymorphFSM.GetCurrentState() != typeof(PolymorphNullState);
	}

	public bool IsAPolymorph()
	{
		return originalObject != null;
	}

	public GameObject GetOriginalObject()
	{
		return (!(originalGlobals != null)) ? base.gameObject : originalGlobals.polymorphController.GetOriginalObject();
	}

	public CharacterGlobals GetOriginalCharacter()
	{
		return (!(originalGlobals != null)) ? charGlobals : originalGlobals.polymorphController.GetOriginalCharacter();
	}

	public CharacterGlobals GetPolymorphCharacter()
	{
		return polymorphGlobals;
	}

	public static void LogPolymorphAction(GameObject actor, CharacterGlobals actorGlobals, string action)
	{
	}

	public static void LogPolymorphAction(GameObject actor, CharacterGlobals actorGlobals, GameObject receiver, CharacterGlobals receiverGlobals, string action)
	{
	}

	public static void LogPolymorphWarning(GameObject actor, CharacterGlobals actorGlobals, string action)
	{
		LogPolymorphAction(actor, actorGlobals, null, null, action, ActionLevel.Warning);
	}

	public static void LogPolymorphError(GameObject actor, CharacterGlobals actorGlobals, string action)
	{
		LogPolymorphAction(actor, actorGlobals, null, null, action, ActionLevel.Error);
	}

	public static void LogPolymorphWarning(GameObject actor, CharacterGlobals actorGlobals, GameObject receiver, CharacterGlobals receiverGlobals, string action)
	{
		LogPolymorphAction(actor, actorGlobals, receiver, receiverGlobals, action, ActionLevel.Warning);
	}

	public static void LogPolymorphError(GameObject actor, CharacterGlobals actorGlobals, GameObject receiver, CharacterGlobals receiverGlobals, string action)
	{
		LogPolymorphAction(actor, actorGlobals, receiver, receiverGlobals, action, ActionLevel.Error);
	}

	public static void LogPolymorphAction(GameObject actor, CharacterGlobals actorGlobals, GameObject receiver, CharacterGlobals receiverGlobals, string action, ActionLevel actionLevel)
	{
		string text = string.Empty;
		if (actor != null)
		{
			text = actor.name;
		}
		string text2 = string.Empty;
		if (actorGlobals != null && actorGlobals.networkComponent != null)
		{
			text2 = actorGlobals.networkComponent.goNetId.ToString();
		}
		string text3 = string.Empty;
		if (actorGlobals != null && actorGlobals.polymorphController != null && actorGlobals.polymorphController._polymorphFSM != null)
		{
			text3 = actorGlobals.polymorphController._polymorphFSM.GetCurrentState().ToString();
		}
		if (receiver == null && receiverGlobals == null)
		{
			LogPolymorphAction(string.Format("{0} FROM <{1}> {2} WHILE IN STATE {3}", action, text, text2, text3), actionLevel);
			return;
		}
		string text4 = string.Empty;
		if (receiver != null)
		{
			text4 = receiver.name;
		}
		string text5 = string.Empty;
		if (receiverGlobals != null && receiverGlobals.networkComponent != null)
		{
			text5 = receiverGlobals.networkComponent.goNetId.ToString();
		}
		LogPolymorphAction(string.Format("{0} FROM <{1}> {2} ON <{3}> {4} WHILE IN STATE {5}", action, text, text2, text4, text5, text3), actionLevel);
	}

	public static void LogPolymorphAction(string action, ActionLevel actionLevel)
	{
		switch (actionLevel)
		{
		case ActionLevel.Warning:
			CspUtils.DebugLog(action);
			break;
		case ActionLevel.Error:
			CspUtils.DebugLog(action);
			break;
		default:
			CspUtils.DebugLog(action);
			break;
		}
	}

	private void UpdateFSM(ShsFSM machine)
	{
		machine.Update();
		PolymorphBaseState polymorphBaseState = machine.GetCurrentStateObject() as PolymorphBaseState;
		if (polymorphBaseState.Done())
		{
			machine.GotoState(polymorphBaseState.GetNextState());
		}
	}

	private void StartPolymorphToCharacterInternal(string[] msgArgs, bool choosePlayer)
	{
		if (IsPolymorphed() && _polymorphFSM.GetCurrentState() != typeof(RevertCharacterState))
		{
			LogPolymorphError(base.gameObject, charGlobals, "START POLYMORPH FAIL - already polymorphed");
		}
		else
		{
			if (charGlobals.combatController.isKilled)
			{
				return;
			}
			polymorphDelay = 0f;
			revertDelay = 0f;
			polymorphEffect = string.Empty;
			revertEffect = string.Empty;
			polymorphCombatEffect = string.Empty;
			polymorphForm = string.Empty;
			revertDeathEffect = string.Empty;
			if (msgArgs != null)
			{
				if (msgArgs.Length > 0)
				{
					polymorphDelay = float.Parse(msgArgs[0]);
				}
				if (msgArgs.Length > 1)
				{
					revertDelay = float.Parse(msgArgs[1]);
				}
				if (msgArgs.Length > 2)
				{
					polymorphEffect = msgArgs[2];
				}
				if (msgArgs.Length > 3)
				{
					revertEffect = msgArgs[3];
				}
				if (msgArgs.Length > 4)
				{
					polymorphCombatEffect = msgArgs[4];
				}
				if (msgArgs.Length > 5)
				{
					polymorphForm = msgArgs[5];
				}
				if (msgArgs.Length > 6)
				{
					revertDeathEffect = msgArgs[6];
				}
			}
			if (polymorphForm == string.Empty)
			{
				polymorphForm = GetCharacterForm(choosePlayer);
			}
			if (polymorphForm == string.Empty)
			{
				LogPolymorphWarning(base.gameObject, charGlobals, "START POLYMORPH ABORTED - no polymorph form");
				return;
			}
			LogPolymorphAction(base.gameObject, charGlobals, "START POLYMORPH");
			if (revertDeathEffect == string.Empty)
			{
				revertDeathEffect = revertEffect;
			}
			GameObject combatTarget = null;
			if (charGlobals.combatController.InCombat())
			{
				combatTarget = charGlobals.combatController.GetCombatTarget();
			}
			if (charGlobals != null && charGlobals.brawlerCharacterAI != null)
			{
				charGlobals.brawlerCharacterAI.LockOwnership(true);
			}
			PolymorphCharacterStateData polymorphCharacterStateData = new PolymorphCharacterStateData();
			polymorphCharacterStateData.Initialize(charGlobals, polymorphEffect, revertEffect, polymorphForm, true, polymorphCombatEffect, combatTarget);
			PolymorphBaseState polymorphBaseState = _polymorphFSM.GetCurrentStateObject() as PolymorphBaseState;
			PolymorphStartState state = _polymorphFSM.GetState<PolymorphStartState>();
			PolymorphEndState state2 = _polymorphFSM.GetState<PolymorphEndState>();
			RevertCharacterOnDeathState state3 = _polymorphFSM.GetState<RevertCharacterOnDeathState>();
			polymorphBaseState.stateProxy.Data = polymorphCharacterStateData;
			state.Initialize(polymorphDelay);
			state2.Initialize(revertDelay);
			state3.Initialize(revertDeathEffect);
			_polymorphFSM.GotoState<PolymorphStartState>();
		}
	}

	private GameObject SpawnPolymorphObject(string polmorphPrefabName, Vector3 polymorphLocation, Quaternion polymorphRotation)
	{
		if (BrawlerController.Instance == null || BrawlerController.Instance.brawlerBundle == null)
		{
			return null;
		}
		Object @object = BrawlerController.Instance.brawlerBundle.Load(polmorphPrefabName);
		if (@object == null)
		{
			CspUtils.DebugLog("Prefab not found for polymorph form: " + polmorphPrefabName);
			return null;
		}
		GameObject gameObject = Object.Instantiate(@object, polymorphLocation, polymorphRotation) as GameObject;
		if (gameObject == null)
		{
			return null;
		}
		AppShell.Instance.EventMgr.AddListener<PickupMessage>(OnPickup);
		return gameObject;
	}

	private void OnSpawnPolymorphCharacter(GameObject obj)
	{
		if (_polymorphSpawner != null)
		{
			_polymorphSpawner.doTransportEffect = true;
			_polymorphSpawner.goNetId = GoNetId.Invalid;
			_polymorphSpawner.SetCharacterName(_originalCharacterName);
			_polymorphSpawner.onSpawnCallback -= OnSpawnPolymorphCharacter;
		}
		_polymorphSpawner = null;
		if (obj == null)
		{
			LogPolymorphError(base.gameObject, charGlobals, "SPAWNED NULL OBJECT");
			return;
		}
		if (polymorphObject != null)
		{
			LogPolymorphError(base.gameObject, charGlobals, polymorphObject, polymorphGlobals, "SPAWNED ADDITIONAL POLYMORPH " + obj.name);
		}
		polymorphObject = obj;
		polymorphGlobals = obj.GetComponent<CharacterGlobals>();
		AppShell.Instance.EventMgr.AddListener<EntitySpawnMessage>(OnEntitySpawn);
		AppShell.Instance.EventMgr.AddListener<EntityDespawnMessage>(OnEntityDespawn);
	}

	private void InitializeFromOriginal()
	{
		if (!(originalObject == null) && !(originalGlobals == null))
		{
			if (charGlobals.brawlerCharacterAI != null)
			{
				charGlobals.brawlerCharacterAI.RunAI(false);
			}
			charGlobals.networkComponent.NetOwnerId = originalGlobals.networkComponent.NetOwnerId;
			charGlobals.behaviorManager.setMotionEnabled(false);
		}
	}

	private string GetCharacterForm(bool choosePlayer)
	{
		if (choosePlayer)
		{
			List<PlayerCombatController> list = new List<PlayerCombatController>(PlayerCombatController.PlayerList);
			if (BrawlerController.Instance != null && BrawlerController.Instance.GetValidPolymorphs != null)
			{
				HashSet<string> getValidPolymorphs = BrawlerController.Instance.GetValidPolymorphs;
				foreach (PlayerCombatController player in PlayerCombatController.PlayerList)
				{
					if (!getValidPolymorphs.Contains(player.gameObject.name))
					{
						list.Remove(player);
					}
				}
			}
			if (list.Count > 0)
			{
				int num = 0;
				int num2 = Random.Range(0, list.Count - 1);
				foreach (PlayerCombatController item in list)
				{
					if (num++ == num2)
					{
						return item.gameObject.name;
					}
				}
			}
		}
		PolymorphSpawn polymorphSpawn = charGlobals.spawnData.spawner as PolymorphSpawn;
		if (polymorphSpawn != null)
		{
			PolymorphForm randomForm = polymorphSpawn.GetRandomForm();
			if (randomForm != null)
			{
				return randomForm.characterName;
			}
		}
		return string.Empty;
	}

	private void OnEntitySpawn(EntitySpawnMessage msg)
	{
		if (msg == null || msg.go != polymorphObject)
		{
			return;
		}
		LogPolymorphAction(base.gameObject, charGlobals, polymorphObject, polymorphGlobals, "POLYMORPH FINISHED SPAWNING");
		AppShell.Instance.EventMgr.RemoveListener<EntitySpawnMessage>(OnEntitySpawn);
		if (_polymorphFSM.GetCurrentState() == typeof(PolymorphSpawnState))
		{
			PolymorphSpawnState state = _polymorphFSM.GetState<PolymorphSpawnState>();
			state.SetPolymorph(polymorphObject);
			_polymorphFSM.GotoState(state.GetNextState());
			return;
		}
		PolymorphBaseState polymorphBaseState = _polymorphFSM.GetCurrentStateObject() as PolymorphBaseState;
		if (polymorphBaseState != null && polymorphBaseState.RemotePolymorphEnabled)
		{
			polymorphBaseState.SetPolymorph(polymorphObject);
		}
	}

	private void OnEntityDespawn(EntityDespawnMessage msg)
	{
		if (msg != null && !(msg.go != polymorphObject))
		{
			LogPolymorphAction(base.gameObject, charGlobals, polymorphObject, polymorphGlobals, "POLYMORPH DESPAWNED");
			RemovePolymorphLink();
			AppShell.Instance.EventMgr.Fire(base.gameObject, new CombatCharacterKilledMessage(base.gameObject, charGlobals.combatController, null));
			charGlobals.spawnData.Despawn(EntityDespawnMessage.despawnType.defeated);
		}
	}

	private void OnPickup(PickupMessage msg)
	{
		if (msg != null && !(msg.Pickup != polymorphObject))
		{
			if (!(charGlobals.behaviorManager.getBehavior() is BehaviorPolymorph) && polymorphObject != null)
			{
				polymorphObject.transform.position = base.transform.position;
			}
			GameObject gameObject = GetOriginalObject();
			if (gameObject == null)
			{
				gameObject = base.gameObject;
			}
			CharacterGlobals originalCharacter = GetOriginalCharacter();
			if (originalCharacter == null)
			{
				originalCharacter = charGlobals;
			}
			CombatController component = msg.Character.GetComponent<CombatController>();
			CombatController characterCombat = (!(originalCharacter != null)) ? null : originalCharacter.combatController;
			AppShell.Instance.EventMgr.Fire(gameObject, new CombatCharacterKilledMessage(gameObject, characterCombat, component));
			RemovePolymorphLink();
			if (originalCharacter != null && originalCharacter.spawnData != null)
			{
				originalCharacter.spawnData.Despawn(EntityDespawnMessage.despawnType.defeated, false);
			}
		}
	}

	private IEnumerator StartPolymorphOnDelay(float delay, DelayStartPolymorph startMethod, string[] msgArgs)
	{
		yield return new WaitForSeconds(delay);
		startMethod(msgArgs);
	}

	private void RemovePolymorphLink()
	{
		if (polymorphGlobals != null && polymorphGlobals.polymorphController != null && polymorphGlobals.polymorphController.originalObject == base.gameObject)
		{
			polymorphGlobals.polymorphController.originalObject = null;
			polymorphGlobals.polymorphController.originalGlobals = null;
		}
		if (charGlobals != null && charGlobals.brawlerCharacterAI != null)
		{
			charGlobals.brawlerCharacterAI.LockOwnership(false);
		}
		polymorphObject = null;
		polymorphGlobals = null;
	}
}
