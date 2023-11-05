using System;
using System.Collections.Generic;
using UnityEngine;

public class SocialSpaceController : GameController
{
	private const string ASSET_DIRECTORY = "SocialSpace";

	private static AssetBundle gameworldPrefabBundle;

	private static PrizeWheelManager prizeWheelData;

	private static string gameworldPrefabBundleName = "SocialSpace/gameworld_common_objects";

	protected static SocialSpaceController instance;

	public GameObject buildingCollisionPrefab;

	private int SelectedPlayerId = -1;

	public GameObject SelectedPlayer;

	protected IGameController controller;

	public List<int> primaryZones = new List<int>();

	public static AssetBundle GameworldPrefabBundle
	{
		get
		{
			return gameworldPrefabBundle;
		}
	}

	public static PrizeWheelManager PrizeWheelData
	{
		get
		{
			return prizeWheelData;
		}
		set
		{
			prizeWheelData = value;
		}
	}

	public static string GameworldPrefabBundleName
	{
		get
		{
			return gameworldPrefabBundleName;
		}
		set
		{
			gameworldPrefabBundleName = value;
		}
	}

	public static SocialSpaceController Instance
	{
		get
		{
			return instance;
		}
	}

	public IGameController Controller
	{
		get
		{
			return controller;
		}
	}

	public string ZoneName
	{
		get
		{
			if (controller != null)
			{
				return controller.ZoneName;
			}
			return string.Empty;
		}
	}

	public string ShortZoneName
	{
		get
		{
			string zoneName = ZoneName;
			if (zoneName.ToLower().Contains("bugle"))
			{
				return "bugle";
			}
			if (zoneName.ToLower().Contains("baxter"))
			{
				return "baxter";
			}
			if (zoneName.ToLower().Contains("ville"))
			{
				return "ville";
			}
			if (zoneName.ToLower().Contains("asgard"))
			{
				return "asgard";
			}
			CspUtils.DebugLog("ERROR: can't find shortname for zone " + ZoneName);
			return zoneName;
		}
	}

	public override void Awake()
	{
		base.Awake();
		if (instance != null)
		{
			CspUtils.DebugLog("A second SocialSpace controller is being created.  This may lead to instabilities!");
		}
		else
		{
			instance = this;
		}
		bCallControllerReadyFromStart = false;
		characterCache = new SimpleCharacterCache();
		SimpleCharacterCache obj = characterCache as SimpleCharacterCache;
		obj.OnCharacterSpawned = (SimpleCharacterCache.CharacterSpawned)Delegate.Combine(obj.OnCharacterSpawned, new SimpleCharacterCache.CharacterSpawned(OnCachedCharacterSpawned));
		if (controllerType == ControllerType.SocialSpace)
		{
			controller = new SocialSpaceControllerImpl();
		}
		else if (controllerType == ControllerType.RailsGameWorld)
		{
			controller = new SocialSpaceRailsTutorialControllerImpl();
		}
		controller.SetOwner(this);
		controller.Awake();
	}

	public override void OnEnable()
	{
		base.OnEnable();
		AppShell.Instance.EventMgr.AddListener<EntityDespawnMessage>(OnCharacterDespawned);
		AppShell.Instance.EventMgr.AddListener<RoomUserLeaveMessage>(OnUserLeave);
		AppShell.Instance.EventMgr.AddListener<SelectedPlayerMessage>(OnPlayerSelected);
		controller.OnEnable();
	}

	public override void OnDisable()
	{
		base.OnDisable();
		controller.OnDisable();
		AppShell.Instance.EventMgr.RemoveListener<RoomUserLeaveMessage>(OnUserLeave);
		AppShell.Instance.EventMgr.RemoveListener<EntityDespawnMessage>(OnCharacterDespawned);
		AppShell.Instance.EventMgr.RemoveListener<SelectedPlayerMessage>(OnPlayerSelected);
		instance = null;
	}

	public override void Start()
	{
		base.Start();
		base.gameObject.AddComponent(typeof(AttackDataManager));
		if (!string.IsNullOrEmpty(gameworldPrefabBundleName))
		{
			AppShell.Instance.BundleLoader.FetchAssetBundle(gameworldPrefabBundleName, delegate(AssetBundleLoadResponse response, object extraData)
			{
				if (response.Error != null)
				{
					CspUtils.DebugLog("Can't load activity bundle: " + response.Error);
				}
				else
				{
					gameworldPrefabBundle = response.Bundle;
				}
			});
		}
		controller.Start();

		CspUtils.DebugLog("SocialSpaceController Start!");
		// force garbage collection every 30 sec.
		//InvokeRepeating("CallGC", 30.0f, 600.0f);  // CSP
	}

	void CallGC() {
		CspUtils.garbageCollect();
	}


	public override void OnOldControllerUnloading(AppShell.GameControllerTypeData currentGameData, AppShell.GameControllerTypeData newGameData)
	{
		base.OnOldControllerUnloading(currentGameData, newGameData);
		controller.OnOldControllerUnloading(currentGameData, newGameData);
	}

	public override bool AddPlayerCharacterSpawner(CharacterSpawn possibleSpawner)
	{
		return controller.AddPlayerCharacterSpawner(possibleSpawner);
	}

	public override bool AllowTargetlessAttacks()
	{
		return false;
	}

	public void AddSpawnPoint(SpawnPoint pt)
	{
		controller.AddSpawnPoint(pt);
	}

	public void Update()
	{
		controller.Update();
	}

	public Vector3 GetRespawnPoint()
	{
		if (controller != null)
		{
			return controller.GetRespawnPoint();
		}
		CspUtils.DebugLog("Controller is NULL in SocialSpaceController");
		return Vector3.zero;
	}

	public void InteractWithPlayer(GameObject player)
	{
		NetworkComponent component = Utils.GetComponent<NetworkComponent>(player);
		if (component == null)
		{
			CspUtils.DebugLog("Target player cannot be interacted with, as they do not have a NetworkComponent attached");
			AppShell.Instance.EventMgr.Fire(this, new SelectedPlayerMessage(-1, string.Empty, string.Empty, null));
			return;
		}
		PlayerDictionary.Player value;
		AppShell.Instance.PlayerDictionary.TryGetValue(component.goNetId.ChildId, out value);
		if (value != null)
		{
			AppShell.Instance.EventMgr.Fire(this, new SelectedPlayerMessage(value.PlayerId, value.Name, player.name, player));
			return;
		}
		CspUtils.DebugLog("Target player cannot be interacted with, as they do not have a valid Gazillion ID.");
		AppShell.Instance.EventMgr.Fire(this, new SelectedPlayerMessage(-1, string.Empty, string.Empty, null));
	}

	public void DoEmote(GameObject emoter, sbyte emote)
	{
		if (emoter == null)
		{
			return;
		}
		BehaviorManager component = emoter.GetComponent<BehaviorManager>();
		if (SelectedPlayer != null)
		{
			BehaviorTurnTo behaviorTurnTo = component.requestChangeBehavior<BehaviorTurnTo>(false);
			if (behaviorTurnTo != null)
			{
				behaviorTurnTo.Initialize(SelectedPlayer.transform.position, delegate
				{
					OnEmoteTurn(emoter, emote);
				});
			}
			EmotesDefinition.EmoteDefinition emoteById = EmotesDefinition.Instance.GetEmoteById(emote);
			if (emoteById != null)
			{
				AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_event", "emote_to_player", 1, SelectedPlayerId, -10000, emoteById.command, string.Empty);
			}
		}
		else
		{
			OnEmoteTurn(emoter, emote);
		}
	}

	private void OnEmoteTurn(GameObject emoter, sbyte emote)
	{
		if (!(emoter == null))
		{
			BehaviorManager component = emoter.GetComponent<BehaviorManager>();
			BehaviorEmote behaviorEmote = component.requestChangeBehavior<BehaviorEmote>(false);
			if (behaviorEmote != null && !behaviorEmote.Initialize(emote))
			{
				component.endBehavior();
			}
		}
	}

	private void OnPlayerSelected(SelectedPlayerMessage msg)
	{
		SelectedPlayerId = msg.SelectedPlayerId;
		SelectedPlayer = msg.SelectedPlayer;
		if (SelectedPlayerId != -1 && SelectedPlayer != null)
		{
			GUIManager.Instance.AttachTargetedPlayerIndicator(SelectedPlayer);
		}
		if (SelectedPlayerId != -2 || SelectedPlayer != null)
		{
		}
		if (SelectedPlayerId == -1)
		{
			GUIManager.Instance.DetachTargetedPlayerIndicator();
		}
	}

	private void OnCharacterDespawned(EntityDespawnMessage e)
	{
		controller.CharacterDespawned(e.go);
		DespawnCharacter(e.go);
	}

	private void OnUserLeave(RoomUserLeaveMessage e)
	{
		GameObject gameObjectFromNetId = AppShell.Instance.ServerConnection.Game.GetGameObjectFromNetId(new GoNetId(GoNetId.PLAYER_ID_FLAG, e.userId));
		if (gameObjectFromNetId != null)
		{
			DespawnCharacter(gameObjectFromNetId);
		}
	}

	private void DespawnCharacter(GameObject character)
	{
		NetworkComponent component = Utils.GetComponent<NetworkComponent>(character);
		if (component != null)
		{
			PlayerDictionary.Player value;
			AppShell.Instance.PlayerDictionary.TryGetValue(component.goNetId.ChildId, out value);
			if (value != null && value.PlayerId == SelectedPlayerId)
			{
				AppShell.Instance.EventMgr.Fire(this, new SelectedPlayerMessage(-1, string.Empty, string.Empty, null));
			}
		}
		EffectSequenceList effectSequenceList = character.GetComponent(typeof(EffectSequenceList)) as EffectSequenceList;
		if (effectSequenceList != null)
		{
			effectSequenceList.RequestLoadedCallback(PlayTransportEffect, character);
		}
	}

	private void PlayTransportEffect(EffectSequenceList fxList, object extraData)
	{
		EffectSequence logicalEffectSequence = fxList.GetLogicalEffectSequence("Transport");
		if (logicalEffectSequence != null)
		{
			GameObject gameObject = new GameObject("TransportEffect");
			gameObject.transform.position = (extraData as GameObject).transform.position;
			logicalEffectSequence.Initialize(null, OnTransportEffectDone, null);
			logicalEffectSequence.SetParent(gameObject, true);
			logicalEffectSequence.StartSequence();
		}
	}

	private void OnTransportEffectDone(EffectSequence sequence)
	{
		UnityEngine.Object.Destroy(sequence.transform.parent.gameObject);
	}

	private void OnCachedCharacterSpawned(GameObject cachedCharacter, CharacterSpawnData initData, CharacterSpawn targetSpawnPoint)
	{
		PlayTransportEffect(Utils.GetComponent<EffectSequenceList>(cachedCharacter), cachedCharacter);
	}
}
