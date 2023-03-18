using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

public class NetGameManager : IDisposable
{
	public class NetEntity
	{
		public enum SpawnState
		{
			Initialized,
			StatedSpawning,
			FinishedSpawning
		}

		public GoNetId goNetId;

		public NetworkComponent netComp;

		public SpawnData spawnData;

		public List<NetAction> queuedActions;

		public List<OnOwnershipChangeDelegate> ownershipDelegates;

		public float ownershipTimestamp;

		public SpawnState spawnState;

		public bool sentFirstMessage;

		protected int _ownerId;

		public int ownerId
		{
			get
			{
				return _ownerId;
			}
			set
			{
				_ownerId = value;
				CspUtils.DebugLog("NGM ownerId = " + value);
				if (netComp != null)
				{
					netComp.NetOwnerId = _ownerId;
					CspUtils.DebugLog("NGM name= " + netComp.gameObject.name + " NGM netComp.NetOwnerId = " + _ownerId);
				}
			}
		}

		public NetEntity(GoNetId goNetId)
		{
			this.goNetId = goNetId;
			ownerId = -1;
			netComp = null;
			spawnData = null;
			queuedActions = null;
			ownershipDelegates = new List<OnOwnershipChangeDelegate>();
			ownershipTimestamp = 0f;
			spawnState = SpawnState.Initialized;
			sentFirstMessage = false;
		}
	}

	public delegate void OnOwnershipChangeDelegate(GameObject go, bool bAssumedOwnership);

	public delegate bool AboutToSpawnEntity(NewEntityMessage entityInfo);

	public AboutToSpawnEntity onAboutToSpawnEntity;

	public Dictionary<ulong, NetEntity> dictNetObjs;  // CSP changed from protected to public for testing.

	protected Dictionary<ulong, List<NetworkMessage>> pendingMessages;

	protected Dictionary<ulong, float> pendingMessagesTimestamps;

	protected Dictionary<ulong, List<NetEntity>> dictInactiveNetObjs;

	protected float cleanPendingMessagesTime;

	protected bool disposed;

	protected bool clientReady;

	protected float sendRate = 0.25f;

	protected float sendTime = -1f;

	protected int dynamicId = 5;

	protected int sendMaxActionCount = 25;

	public NetGameManager()
	{
		dictNetObjs = new Dictionary<ulong, NetEntity>();
		pendingMessages = new Dictionary<ulong, List<NetworkMessage>>();
		pendingMessagesTimestamps = new Dictionary<ulong, float>();
		dictInactiveNetObjs = new Dictionary<ulong, List<NetEntity>>();
		disposed = false;
		clientReady = false;
		dynamicId = 0;
		AddEventHandlers();
		UnityEngine.Object[] array = UnityEngine.Object.FindSceneObjectsOfType(typeof(GameObject));
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			GameObject gameObject = (GameObject)array2[i];
			NetworkComponent networkComponent = gameObject.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
			if (!(networkComponent == null) && networkComponent.goNetId.IsStaticId())
			{
				OnEntitySpawn(new EntitySpawnMessage(networkComponent.gameObject, CharacterSpawn.Type.Static));
			}
		}
	}

	~NetGameManager()
	{
		Dispose(false);
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				RemoveEventHandlers();
			}
			disposed = true;
		}
	}

	protected NetEntity AddNetEntity(GameObject go)
	{
		CspUtils.DebugLog("AddNetEntity() called for " + go.name);
		NetworkComponent component = go.GetComponent<NetworkComponent>();
		if (component == null)
		{
			CspUtils.DebugLog("Trying to add a network entity that doesn't have a network component");
			return null;
		}
		SpawnData component2 = go.GetComponent<SpawnData>();
		if (component.goNetId.IsPlayerId() && component2 == null)
		{
			CspUtils.DebugLog("Trying to add a player network entity that doesn't have spawn data");
			return null;
		}
		NetEntity value;
		if (dictNetObjs.TryGetValue(component.goNetId, out value))
		{
			CspUtils.DebugLog("AddNetEntity value.ownerId = " + value.ownerId);
			value.netComp = component;

			///// CSP added this block as a kudge fix ////////////////////////////////////////////
			// if this client is not the host, but a host exists, and this spawning entity is not
			//    a player, then 
			if (!component.goNetId.IsPlayerId()) {
		  	  value.netComp.NetOwnerId =  AppShell.Instance.ServerConnection.GetGameHostId();
			}
			else
		//////////////////////////////////////////////////////////////////////////////////////
			  value.netComp.NetOwnerId = value.ownerId;
			CspUtils.DebugLog("ANE name= " + value.netComp.gameObject.name + " ANE netComp.NetOwnerId = " + value.netComp.NetOwnerId);
			value.spawnData = component2;
		}
		else
		{
			CspUtils.DebugLog("AddNetEntity component.NetOwnerId = " + component.NetOwnerId);
			value = new NetEntity(component.goNetId);
			value.ownerId = component.NetOwnerId;
			value.netComp = component;

			////////////////// this block added by CSP  ////////////////
			if (AppShell.Instance.ServerConnection.IsGameHost())
				value.ownerId = AppShell.Instance.ServerConnection.GetGameUserId();
			//value.ownerId = component.goNetId.childId;  // CSP added this line - try assign childId to ownerID after assigning a value to netComp.
														//   not sure if this will have unwanted side effects.	
			/////////////////////////////////////////////////////////////


			value.spawnData = component2;
			dictNetObjs.Add(value.goNetId, value);
		}
		ProcessPendingMessages(value.goNetId);
		return value;
	}

	protected void RemoveNetEntity(GoNetId goNetId)
	{
		NetEntity value;
		if (dictNetObjs.TryGetValue(goNetId, out value))
		{
			CallOwnershipChangeDeletgates(false, value);
			dictNetObjs.Remove(goNetId);
		}
	}

	protected NetEntity AddInactiveNetEntity(GameObject go)
	{
		NetEntity inactiveNetEntity = GetInactiveNetEntity(go);
		if (inactiveNetEntity != null)
		{
			return inactiveNetEntity;
		}
		NetworkComponent component = go.GetComponent<NetworkComponent>();
		if (component == null)
		{
			CspUtils.DebugLog("NetGameManager::AddInactiveEntity() - network component for net entity to add to inactive list is null");
			return null;
		}
		SpawnData component2 = go.GetComponent<SpawnData>();
		if (component.goNetId.IsPlayerId() && component2 == null)
		{
			CspUtils.DebugLog("NetGameManager::AddInactiveEntity() - spawn data for player net entity to add to inactive list is null");
			return null;
		}
		List<NetEntity> value = null;
		if (!dictInactiveNetObjs.TryGetValue(component.goNetId, out value))
		{
			value = new List<NetEntity>();
			dictInactiveNetObjs.Add(component.goNetId, value);
		}
		NetEntity netEntity = new NetEntity(component.goNetId);
		netEntity.ownerId = component.NetOwnerId;
		netEntity.netComp = component;
		netEntity.spawnData = component2;
		value.Add(netEntity);
		return netEntity;
	}

	protected void RemoveInactiveNetEntity(GameObject go)
	{
		NetworkComponent component = go.GetComponent<NetworkComponent>();
		if (component == null)
		{
			CspUtils.DebugLog("NetGameManager::RemoveInactiveNetEntity() - network component for net entity to remove from inactive list is null");
			return;
		}
		NetEntity inactiveNetEntity = GetInactiveNetEntity(go);
		if (inactiveNetEntity == null)
		{
			CspUtils.DebugLog("NetGameManager::RemoveInactiveNetEntity() - no net entity in inactive entity dictionary for " + component.goNetId);
			return;
		}
		List<NetEntity> value = null;
		if (dictInactiveNetObjs.TryGetValue(component.goNetId, out value) && value != null)
		{
			value.Remove(inactiveNetEntity);
			if (value.Count <= 0)
			{
				dictInactiveNetObjs.Remove(component.goNetId);
			}
		}
	}

	protected NetEntity GetInactiveNetEntity(GameObject go)
	{
		NetworkComponent component = go.GetComponent<NetworkComponent>();
		if (component == null)
		{
			CspUtils.DebugLog("NetGameManager::GetInactiveNetEntity() - network component for net entity to retrieve from inactive list is null");
			return null;
		}
		List<NetEntity> value = null;
		if (dictInactiveNetObjs.TryGetValue(component.goNetId, out value) && value != null)
		{
			foreach (NetEntity item in value)
			{
				if (item.netComp == component)
				{
					return item;
				}
			}
		}
		return null;
	}

	public void ClientReady()
	{
		clientReady = true;
		AppShell.Instance.ServerConnection.SendGameMsg(new ClientReadyMessage());
	}

	public void ClientUnready()
	{
		clientReady = false;
	}

	public void Update()
	{
		sendTime -= Time.deltaTime;
		if (sendTime <= 0f)
		{
			sendTime = sendRate;
			AggregateMessage aggregateMessage = new AggregateMessage();
			aggregateMessage.messages = new List<NetworkMessage>();
			int num = 0;
			foreach (NetEntity value in dictNetObjs.Values)
			{
				if (value.queuedActions != null)
				{
					NetActionMessage netActionMessage = new NetActionMessage(value.goNetId);
					//CspUtils.DebugLog("NGM Update netActionMessage.GetType()=" + netActionMessage.GetType().ToString());  //CSP
					netActionMessage.actions = value.queuedActions;
					while (netActionMessage.actions.Count > sendMaxActionCount)
					{
						AggregateMessage aggregateMessage2 = new AggregateMessage();
						aggregateMessage2.messages = new List<NetworkMessage>();
						NetActionMessage netActionMessage2 = new NetActionMessage(value.goNetId);
						netActionMessage2.actions = netActionMessage.actions.GetRange(0, sendMaxActionCount);
						aggregateMessage2.messages.Add(netActionMessage2);
						AppShell.Instance.ServerConnection.SendGameMsg(aggregateMessage2);
						netActionMessage.actions.RemoveRange(0, sendMaxActionCount);
					}
					if (netActionMessage.actions.Count + num > sendMaxActionCount)
					{
						AppShell.Instance.ServerConnection.SendGameMsg(aggregateMessage);
						aggregateMessage = new AggregateMessage();
						aggregateMessage.messages = new List<NetworkMessage>();
						num = 0;
					}
					num += netActionMessage.actions.Count;
					if (netActionMessage.actions.Count > 0)
					{
						aggregateMessage.messages.Add(netActionMessage);
					}
					value.queuedActions = null;
					if (!value.sentFirstMessage)
					{
						value.sentFirstMessage = true;
					}
				}
			}
			if (aggregateMessage.messages.Count > 0)
			{
				AppShell.Instance.ServerConnection.SendGameMsg(aggregateMessage);
			}
		}
		foreach (NetEntity value2 in dictNetObjs.Values)
		{
			if (value2.ownershipDelegates.Count > 0 && Time.time - value2.ownershipTimestamp > 10f)
			{
				CspUtils.DebugLog("Found an old ownership request " + value2.goNetId + " timing it out.  Current owner = " + value2.netComp.NetOwnerId);
				CallOwnershipChangeDeletgates(false, value2);
			}
		}
		if (Time.time > cleanPendingMessagesTime)
		{
			List<ulong> list = new List<ulong>();
			foreach (KeyValuePair<ulong, float> pendingMessagesTimestamp in pendingMessagesTimestamps)
			{
				if (Time.time > pendingMessagesTimestamp.Value + 60f)
				{
					list.Add(pendingMessagesTimestamp.Key);
				}
			}
			foreach (ulong item in list)
			{
				pendingMessages.Remove(item);
				pendingMessagesTimestamps.Remove(item);
			}
			cleanPendingMessagesTime = Time.time + 60f;
		}
	}

	public void ForEachNetEntity(Action<NetEntity> action)
	{
		foreach (NetEntity value in dictNetObjs.Values)
		{
			if (value.spawnData != null)
			{
				action(value);
			}
		}
	}

	public void ForEachNetEntity(CharacterSpawn.Type filter, Action<NetEntity> action)
	{
		foreach (NetEntity value in dictNetObjs.Values)
		{
			if (value.spawnData != null && value.spawnData.spawnType == filter)
			{
				action(value);
			}
		}
	}

	public void ForEachNetEntityMask(CharacterSpawn.Type filter, Action<NetEntity> action)
	{
		foreach (NetEntity value in dictNetObjs.Values)
		{
			if (value.spawnData != null && (value.spawnData.spawnType & filter) != 0)
			{
				action(value);
			}
		}
	}

	public void ForEachInactiveNetEntity(GoNetId goNetId, Action<NetEntity> action)
	{
		List<NetEntity> value = null;
		if (dictInactiveNetObjs.TryGetValue(goNetId, out value))
		{
			List<NetEntity> list = new List<NetEntity>(value);
			foreach (NetEntity item in list)
			{
				action(item);
			}
		}
	}

	public void QueueNetAction(GoNetId goNetId, NetAction action)
	{
		//CspUtils.DebugLog("QueueNetAction NetActionPositionFull");  //CSP
		NetEntity value = null;
		if (dictNetObjs.TryGetValue(goNetId, out value))
		{
			if (value.queuedActions == null)
			{
				if (!value.sentFirstMessage)
				{
				}
				value.queuedActions = new List<NetAction>();
			}
			action.timestamp = Time.time;
			value.queuedActions.Add(action);
		}
		else
		{
			CspUtils.DebugLog("Unknown network object <" + goNetId + ">");
		}
	}

	public GoNetId GetNewDynamicId()
	{
		int gameUserId = AppShell.Instance.ServerConnection.GetGameUserId();
		if (gameUserId < 0)
		{
			throw new Exception("Invalid SmartFox user id");
		}
		return new GoNetId(gameUserId, ++dynamicId);
	}

	public void ProcessMessage(NetworkMessage msg)
	{
		//CspUtils.DebugLog("ProcessMessage msg.GetType()= " + msg.GetType().ToString());  //CSP
		
		//if (msg.GetType() == typeof(NetActionPositionFull))
			//CspUtils.DebugLog("ProcessMessage NetActionPositionFull type");  //CSP .

		if (msg.GetType() == typeof(ClientReadyMessage))
		{
			int gameUserId = AppShell.Instance.ServerConnection.GetGameUserId();
			foreach (NetEntity value7 in dictNetObjs.Values)
			{
				if (value7.netComp != null && value7.netComp.IsOwner())
				{
					if (value7.netComp.goNetId.IsPlayerId())
					{
						if (value7.netComp.goNetId.ChildId == gameUserId)
						{
							value7.netComp.StartCoroutine(SendPlayerToClient(msg.senderRTCId, value7.netComp.gameObject));
						}
						else
						{
							CspUtils.DebugLog("Trying to send player " + value7.netComp.goNetId.ToString() + " but I am player <" + gameUserId + ">");
						}
					}
					else
					{
						NewEntityMessage newEntityMessage = value7.netComp.GetNewEntityMessage();
						AppShell.Instance.ServerConnection.SendGameMsg(newEntityMessage, msg.senderRTCId);
					}
				}
			}
			if (AppShell.Instance.ServerConnection.IsGameHost())
			{
				foreach (NetEntity value8 in dictNetObjs.Values)
				{
					if (value8.netComp != null && value8.netComp.NetOwnerId <= -2)
					{
						if (value8.netComp.goNetId.IsPlayerId())
						{
							CspUtils.DebugLog("Trying to send unowned player " + value8.netComp.goNetId.ToString() + ",<" + value8.netComp.NetOwnerId + ">");
						}
						else
						{
							NewEntityMessage newEntityMessage2 = value8.netComp.GetNewEntityMessage();
							AppShell.Instance.ServerConnection.SendGameMsg(newEntityMessage2, msg.senderRTCId);
						}
					}
				}
			}
		}
		else if (msg.GetType() == typeof(SetPetMessage))
		{
			SetPetMessage setPetMessage = msg as SetPetMessage;
			GameObject gameObject = null;
			foreach (NetEntity value9 in dictNetObjs.Values)
			{
				if (!(value9.netComp != null) || value9.netComp.goNetId.IsPlayerId())
				{
				}
				if (value9.netComp != null && value9.netComp.goNetId.childId == msg.goNetId.childId)
				{
					gameObject = value9.netComp.gameObject;
				}
			}
			if (gameObject != null)
			{
				PetSpawner.requestRemotePet(setPetMessage.petID, gameObject, msg.goNetId.childId);
			}
			else
			{
				PetSpawner.requestRemotePet(setPetMessage.petID, null, msg.goNetId.childId, true);
			}
		}
		else if (msg.GetType() == typeof(SetTitleMessage))
		{
			SetTitleMessage setTitleMessage = msg as SetTitleMessage;
			TitleManager.requestTitleAndMedallion(setTitleMessage.titleID, setTitleMessage.medallionID, msg.goNetId.childId);
		}
		else if (msg.GetType() == typeof(SpawnPrestigeMessage))
		{
			CspUtils.DebugLog("  got SpawnPrestigeMessage from ID " + msg.goNetId.childId);
			SpawnPrestigeMessage spawnPrestigeMessage = msg as SpawnPrestigeMessage;
			TitleManager.setPrestige(msg.goNetId.childId, spawnPrestigeMessage.hero);
			AppShell.Instance.EventMgr.Fire(AppShell.Instance, new SpawnPrestigeRequest(msg.goNetId.childId));
		}
		else if (msg.GetType() == typeof(ForceEmoteMessage))
		{
			ForceEmoteMessage forceEmoteMessage = msg as ForceEmoteMessage;
			CspUtils.DebugLog("  got ForceEmoteMessage " + forceEmoteMessage.emoteID);
			AppShell.Instance.EventMgr.Fire(this, new EmoteMessage(GameController.GetController().LocalPlayer, (sbyte)forceEmoteMessage.emoteID));
		}
		else
		{
			clientReady = true;   // CSP  temporary force to true to test...get rid of this later!
			if (!clientReady)
			{
				CspUtils.DebugLog("ProcessMessage client not ready...returning!");  //CSP
				return;
			}
			if (msg.GetType() == typeof(AggregateMessage))
			{
				AggregateMessage aggregateMessage = msg as AggregateMessage;
				foreach (NetworkMessage message in aggregateMessage.messages)
				{
					message.senderRTCId = msg.senderRTCId;
					//CspUtils.DebugLog("AggregateMessage message.GetType()= " + message.GetType().ToString());  //CSP
					ProcessMessage(message);
				}
			}
			else if (msg.GetType() == typeof(ScenarioEventMessage))
			{
				ScenarioEventMessage scenarioEventMessage = msg as ScenarioEventMessage;
				ScenarioEventManager.Instance.FireScenarioEvent(scenarioEventMessage.scenarioEventName, true);
			}
			else if (msg.GetType() == typeof(ObjectSpawnMessage))
			{
				NetEntity value;
				if (dictNetObjs.TryGetValue(msg.goNetId, out value))
				{
					ObjectSpawnMessage objectSpawnMessage = msg as ObjectSpawnMessage;
					Type type = Type.GetType(objectSpawnMessage.componentName);
					if (type == null)
					{
						CspUtils.DebugLog("Unknown component type " + objectSpawnMessage.componentName + " in object spawn message for object " + objectSpawnMessage.prefabName);
						return;
					}
					Component component = value.netComp.GetComponent(type);
					if (component == null)
					{
						CspUtils.DebugLog("Received ObjectSpawnMessage for object " + value.netComp.gameObject.name + " that has no " + objectSpawnMessage.componentName + " component");
						return;
					}
					MethodInfo method = type.GetMethod("RemoteSpawn");
					GameObject gameObject2 = method.Invoke(component, new object[5]
					{
						objectSpawnMessage.spawnLocation,
						objectSpawnMessage.spawnRotation,
						objectSpawnMessage.objectNetId,
						objectSpawnMessage.prefabName,
						objectSpawnMessage.parent
					}) as GameObject;
					if (objectSpawnMessage.objectNetId.IsValid() && gameObject2 != null)
					{
						AddNetEntity(gameObject2);
					}
				}
				else
				{
					CspUtils.DebugLog("Received ObjectSpawnMessage for unknown object spawner ID " + msg.goNetId);
				}
			}
			else if (msg.GetType() == typeof(PickupSpawnMessage))
			{
				PickupSpawnMessage pickupSpawnMessage = msg as PickupSpawnMessage;
				BrawlerController.Instance.spawnPickup(pickupSpawnMessage.pickupName, pickupSpawnMessage.spawnLocation, pickupSpawnMessage.objectNetId);
			}
			else if (msg.GetType() == typeof(TrapImpactMessage))
			{
				NetEntity value2;
				if (dictNetObjs.TryGetValue(msg.goNetId, out value2))
				{
					if (value2.netComp != null)
					{
						BrawlerTrapBase brawlerTrapBase = value2.netComp.GetComponent(typeof(BrawlerTrapBase)) as BrawlerTrapBase;
						if (brawlerTrapBase != null)
						{
							TrapImpactMessage trapImpactMessage = msg as TrapImpactMessage;
							brawlerTrapBase.RemoteHitTarget(trapImpactMessage.target);
						}
						else
						{
							CspUtils.DebugLog("Received TrapImpactMessage for object " + value2.netComp.gameObject.name + " that has no BrawlerTrapBase component");
						}
					}
				}
				else
				{
					CspUtils.DebugLog("Received TrapImpactMessage for unknown trap with ID " + msg.goNetId);
				}
			}
			else if (msg.GetType() == typeof(TestMessage))
			{
				TestMessage testMessage = msg as TestMessage;
				CspUtils.DebugLog("testmsg = " + testMessage.testString);
			}
			else if (msg.GetType() == typeof(NewEntityMessage))
			{
				CspUtils.DebugLog("msg.GetType() == typeof(NewEntityMessage)");
				CreateEntityFromMessage(msg, !AppShell.Instance.IsHeroAuth);
			}
			else if (msg.GetType() == typeof(DeleteEntityMessage))
			{
				NetEntity value3;
				if (dictNetObjs.TryGetValue(msg.goNetId, out value3))
				{
					if (value3.spawnData != null)
					{
						value3.spawnData.Despawn(EntityDespawnMessage.despawnType.defeated);
					}
					else if (value3.netComp != null && value3.netComp.gameObject != null)
					{
						UnityEngine.Object.Destroy(value3.netComp.gameObject);
					}
				}
			}
			else if (msg.GetType() == typeof(MatchmakingInvitationMessage))
			{
				MatchmakingInvitationMessage value4 = msg as MatchmakingInvitationMessage;
				GameObject gameObject3 = GameObject.FindWithTag("GameController");
				if (gameObject3 != null)
				{
					gameObject3.SendMessage("OnInvitationMessage", value4);
				}
			}
			else if (msg.GetType() == typeof(BrawlerStageCompleteMessage))
			{
				BrawlerStageCompleteMessage brawlerStageCompleteMessage = msg as BrawlerStageCompleteMessage;
				if (BrawlerController.Instance != null)
				{
					BrawlerController.Instance.ProcessStageComplete(brawlerStageCompleteMessage.stageNumber);
				}
			}
			else if (msg.GetType() == typeof(PolymorphSpawnMessage))
			{
				PolymorphSpawnMessage polymorphSpawnMessage = msg as PolymorphSpawnMessage;
				GameObject gameObjectFromNetId = GetGameObjectFromNetId(polymorphSpawnMessage.goNetId);
				if (gameObjectFromNetId == null)
				{
					CacheMessage(msg);
					return;
				}
				NetEntity value5;
				if (!dictNetObjs.TryGetValue(polymorphSpawnMessage.polymorphNetId, out value5))
				{
					value5 = new NetEntity(polymorphSpawnMessage.polymorphNetId);
					dictNetObjs.Add(polymorphSpawnMessage.polymorphNetId, value5);
				}
				else if (polymorphSpawnMessage.goNetId.IsPlayerId())
				{
					value5.spawnState = NetEntity.SpawnState.Initialized;
				}
				if (value5.spawnState != 0)
				{
					return;
				}
				PolymorphController component2 = gameObjectFromNetId.GetComponent<PolymorphController>();
				if (component2 == null)
				{
					CspUtils.DebugLog("On PolymorphSpawnMessage: Polymorph controller for original object does not exist");
					return;
				}
				SpawnData component3 = gameObjectFromNetId.GetComponent<SpawnData>();
				if (component3 == null)
				{
					CspUtils.DebugLog("On PolymorphSpawnMessage: Spawn data for original object does not exist");
					return;
				}
				CharacterSpawn spawner = component3.spawner;
				if (spawner == null && polymorphSpawnMessage.goNetId.IsPlayerId())
				{
					GameObject localPlayer = GameController.GetController().LocalPlayer;
					if (localPlayer != null)
					{
						SpawnData component4 = localPlayer.GetComponent<SpawnData>();
						if (component4 != null)
						{
							spawner = component4.spawner;
						}
					}
				}
				if (spawner == null)
				{
					CspUtils.DebugLog("On PolymorphSpawnMessage: Unable to find spawner for character polymorph");
					return;
				}
				spawner.IsLocal = false;
				spawner.goNetId = polymorphSpawnMessage.polymorphNetId;
				spawner.IsNetworked = true;
				spawner.SpawnOnStart = false;
				component2.RemoteSpawnPolymorphCharacter(polymorphSpawnMessage.polymorphName, spawner);
				value5.spawnState = NetEntity.SpawnState.StatedSpawning;
			}
			else if (msg.goNetId.IsValid())
			{
				//CspUtils.DebugLog("msg.goNetId.IsValid() msg.GetType()= " + msg.GetType().ToString());  //CSP

				NetEntity value6 = null;
				if (dictNetObjs.TryGetValue(msg.goNetId, out value6))
				{
					if (value6.netComp != null)
					{
						CspUtils.DebugLog("ProcessMessage value6");
						value6.netComp.ProcessMessage(msg);
					}
					else
					{
						CspUtils.DebugLog("ProcessMessage  caching message!");
						CacheMessage(msg);
					}
				}
				else
				{
					CacheMessage(msg);
				}
			}
			else
			{
				CspUtils.DebugLog("message was not processed!");
			}
		}
	}

	protected void CacheMessage(NetworkMessage message)
	{
		if (!pendingMessages.ContainsKey(message.goNetId))
		{
			pendingMessages[message.goNetId] = new List<NetworkMessage>();
		}
		pendingMessages[message.goNetId].Add(message);
		pendingMessagesTimestamps[message.goNetId] = Time.time;
	}

	protected void ProcessPendingMessages(GoNetId goNetId)
	{
		if (pendingMessages.ContainsKey(goNetId))
		{
			foreach (NetworkMessage item in pendingMessages[goNetId])
			{
				ProcessMessage(item);
			}
			pendingMessages.Remove(goNetId);
			pendingMessagesTimestamps.Remove(goNetId);
		}
	}

	private IEnumerator SendPlayerToClient(int targetRTCId, GameObject go)
	{
		CharacterGlobals charGlobals = go.GetComponent<CharacterGlobals>();
		if (charGlobals == null || charGlobals.motionController == null)
		{
			yield break;
		}
		while (true)
		{
			if (charGlobals == null || charGlobals.motionController == null)
			{
				yield break;
			}
			if (charGlobals.motionController.IsOnGround() && !charGlobals.motionController.NetUpdatesDisabled)
			{
				break;
			}
			yield return 0;
		}
		if (AppShell.Instance.IsHeroAuth)
		{
			NewEntityMessage newEntity = charGlobals.networkComponent.GetNewEntityMessage();
			string data = NetworkMessage.ToEncodedString64(newEntity);
			HeroCollection available = AppShell.Instance.Profile.AvailableCostumes;
			string code;
			if (available != null && available.ContainsKey(newEntity.modelName))
			{
				HeroPersisted hero = available[newEntity.modelName];
				code = hero.Code;
			}
			else
			{
				code = string.Empty;
			}
			AppShell.Instance.ServerConnection.SendHeroSync(targetRTCId, newEntity.modelName, code, data);
		}
		else
		{
			NewEntityMessage outMsg = charGlobals.networkComponent.GetNewEntityMessage();
			AppShell.Instance.ServerConnection.SendGameMsg(outMsg, targetRTCId);
		}
	}

	private void CreateEntityFromMessage(NetworkMessage msg, bool allowPlayerCreate)
	{
		NewEntityMessage newEntityMessage = msg as NewEntityMessage;
		if (msg.goNetId.IsPlayerId())
		{
			if (!allowPlayerCreate)
			{
				CspUtils.DebugLog("Received player " + msg.goNetId.ToString() + " from <" + msg.senderRTCId + ">, potential hack?");
				return;
			}
			if (msg.goNetId.ChildId != msg.senderRTCId)
			{
				CspUtils.DebugLog("Received player " + msg.goNetId.ToString() + " msg.goNetId.ChildId = " + msg.goNetId.ChildId + " from <" + msg.senderRTCId + ">, shouldn't happen!");
				return;
			}
		}
		NetEntity value;
		if (!dictNetObjs.TryGetValue(msg.goNetId, out value))
		{
			value = new NetEntity(msg.goNetId);
			dictNetObjs.Add(value.goNetId, value);
		}
		if (newEntityMessage.goNetId.IsStaticId())
		{
			if (value.netComp != null)
			{
				value.netComp.SendMessage("ProcessNetworkState", newEntityMessage.extraData, SendMessageOptions.DontRequireReceiver);
			}
		}
		else if (value.spawnState == NetEntity.SpawnState.Initialized)
		{
			if (onAboutToSpawnEntity == null || !onAboutToSpawnEntity(newEntityMessage))
			{
				GameObject gameObject = null;
				CharacterSpawn characterSpawn = null;
				if (newEntityMessage.spawner != null)
				{
					if ((newEntityMessage.spawnType & CharacterSpawn.Type.Player) != 0)
					{
						CharacterSpawn characterSpawn2 = newEntityMessage.spawner.GetComponent(typeof(CharacterSpawn)) as CharacterSpawn;
						gameObject = new GameObject();
						gameObject.transform.position = newEntityMessage.spawner.transform.position;
						gameObject.transform.rotation = newEntityMessage.spawner.transform.rotation;
						characterSpawn = (gameObject.AddComponent(characterSpawn2.GetType()) as CharacterSpawn);
						characterSpawn2.CloneSpawner(characterSpawn);
					}
					else
					{
						gameObject = newEntityMessage.spawner;
						characterSpawn = (newEntityMessage.spawner.GetComponent(typeof(CharacterSpawn)) as CharacterSpawn);
						if (gameObject.transform.parent != null)
						{
							GameObject gameObject2 = gameObject.transform.parent.gameObject;
							SpawnGroup spawnGroup = gameObject2.GetComponent(typeof(SpawnGroup)) as SpawnGroup;
							if (spawnGroup != null)
							{
								spawnGroup.RemoteSpawn(characterSpawn);
							}
							else
							{
								SpawnController spawnController = gameObject2.GetComponent(typeof(SpawnController)) as SpawnController;
								if (spawnController != null)
								{
									spawnController.RemoteSpawn(characterSpawn);
								}
							}
						}
					}
				}
				else if (newEntityMessage.extraData != null && newEntityMessage.extraData.Contains("remote_spawner"))
				{
					gameObject = (UnityEngine.Object.Instantiate(Resources.Load("Spawners/" + newEntityMessage.extraData["remote_spawner"]), newEntityMessage.pos, newEntityMessage.rot) as GameObject);
					//characterSpawn = (gameObject.GetComponent(typeof(CharacterSpawn)) as CharacterSpawn);				
					characterSpawn =  (CharacterSpawn)(CspUtils.findComponentByType(gameObject, "CharacterSpawn"));  // CSP

				}
				else
				{
					gameObject = (UnityEngine.Object.Instantiate(Resources.Load("Spawners/RemoteSpawner"), newEntityMessage.pos, newEntityMessage.rot) as GameObject);
					//characterSpawn = (gameObject.GetComponent(typeof(CharacterSpawn)) as CharacterSpawn);
					characterSpawn =  (CharacterSpawn)(CspUtils.findComponentByType(gameObject, "CharacterSpawn"));    // CSP
				}

				try {
					characterSpawn.SetCharacterName(newEntityMessage.modelName);
				}
				catch(Exception e) {
					CspUtils.DebugLogError(e.StackTrace);
				}
				characterSpawn.IsLocal = false;
				characterSpawn.IsPlayer = ((newEntityMessage.spawnType & CharacterSpawn.Type.Player) != 0);
				characterSpawn.IsAI = ((newEntityMessage.spawnType & CharacterSpawn.Type.AI) != 0);
				characterSpawn.IsBoss = ((newEntityMessage.spawnType & CharacterSpawn.Type.Boss) != 0);
				characterSpawn.DestroyOnSpawn = (characterSpawn.IsPlayer || (newEntityMessage.spawnType & CharacterSpawn.Type.Ally) != 0);
				characterSpawn.goNetId = newEntityMessage.goNetId;
				characterSpawn.IsNetworked = true;
				characterSpawn.netExtraData = newEntityMessage.extraData;
				if (newEntityMessage.extraData.ContainsKey("RecordHistory"))
				{
					characterSpawn.RecordHistory = true;
				}
				else
				{
					characterSpawn.RecordHistory = false;
				}
				if (characterSpawn.IsPlayer)
				{
					characterSpawn.SpawnOnStart = true;
				}
				else if ((newEntityMessage.spawnType & CharacterSpawn.Type.Ally) == 0)
				{
					CspUtils.DebugLog("CreateEntityFromMessage " + newEntityMessage.modelName + " goNetId=" + newEntityMessage.goNetId.ToString());
					characterSpawn.SpawnWithID(newEntityMessage.goNetId);
				}
				else
				{
					characterSpawn.forceFaction(CombatController.Faction.Player);
				}
			}
			value.spawnState = NetEntity.SpawnState.StatedSpawning;
		}
		else
		{
			CspUtils.DebugLog("Ignoring duplicate NewEntityMessage <" + msg.senderRTCId + ">," + msg.goNetId);
		}
	}

	public void OnHeroCreate(string fromRTCId, string hero, string data)
	{
		NetworkMessage networkMessage = NetworkMessage.FromEncodedString64(data);
		networkMessage.senderRTCId = networkMessage.goNetId.ChildId;  // CSP - quick fix until I can get smartfox extension to return correct value.
		//networkMessage.senderRTCId = int.Parse(fromRTCId);
		if (networkMessage.GetType() == typeof(NewEntityMessage))
		{
			NewEntityMessage newEntityMessage = networkMessage as NewEntityMessage;
			if (newEntityMessage.modelName == hero)
			{
				CreateEntityFromMessage(networkMessage, true);
			}
			else
			{
				CspUtils.DebugLog("Mismatched heros <" + newEntityMessage.modelName + "> != <" + hero + "> from user " + fromRTCId);
			}
		}
		else
		{
			CspUtils.DebugLog("Unexpected message type in OnHeroCreate <" + networkMessage.GetType().ToString() + ">");
		}
	}

	public GameObject GetGameObjectFromNetId(GoNetId goNetId)
	{
		NetEntity value = null;
		if (dictNetObjs.TryGetValue(goNetId, out value) && value.netComp != null)
		{
			return value.netComp.gameObject;
		}
		return null;
	}

	public void TakeOwnership(GameObject go, OnOwnershipChangeDelegate onOwnershipChange)
	{
		TakeOwnership(go, onOwnershipChange, true);
	}

	public void TakeOwnership(GameObject go, OnOwnershipChangeDelegate onOwnershipChange, bool autoTransfer)
	{
		NetworkComponent networkComponent = go.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
		NetEntity value;
		if (networkComponent.IsOwner())
		{
			if (onOwnershipChange != null)
			{
				onOwnershipChange(go, true);
			}
		}
		else if (networkComponent.NetOwnerId >= 0)
		{
			if (onOwnershipChange != null)
			{
				onOwnershipChange(go, false);
			}
		}
		else if (!dictNetObjs.TryGetValue(networkComponent.goNetId, out value))
		{
			CspUtils.DebugLog("Unknown object " + networkComponent.goNetId);
			if (onOwnershipChange != null)
			{
				onOwnershipChange(go, false);
			}
		}
		else if (value.ownershipDelegates.Count > 0)
		{
			if (onOwnershipChange != null)
			{
				value.ownershipDelegates.Add(onOwnershipChange);
			}
		}
		else if (networkComponent.NetOwnerId == -1)
		{
			value.ownerId = -3;
			value.ownershipTimestamp = Time.time;
			if (onOwnershipChange != null)
			{
				value.ownershipDelegates.Add(onOwnershipChange);
			}
			AppShell.Instance.ServerConnection.QueryOwnership(networkComponent.goNetId);
		}
		else if (networkComponent.NetOwnerId == -2)
		{
			value.ownerId = -3;
			value.ownershipTimestamp = Time.time;
			if (onOwnershipChange != null)
			{
				value.ownershipDelegates.Add(onOwnershipChange);
			}
			if (networkComponent.goNetId.IsPlayerId())
			{
				AppShell.Instance.ServerConnection.TakeOwnership(networkComponent.goNetId, false);
			}
			else
			{
				AppShell.Instance.ServerConnection.TakeOwnership(networkComponent.goNetId, autoTransfer);
			}
		}
	}

	public void ReleaseOwnership(GameObject go)
	{
		NetworkComponent networkComponent = go.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
		if (networkComponent.IsOwner() || networkComponent.IsPendingOwner())
		{
			NetEntity value;
			if (!dictNetObjs.TryGetValue(networkComponent.goNetId, out value))
			{
				CspUtils.DebugLog("Unknown object <" + networkComponent.goNetId + ">");
				return;
			}
			CallOwnershipChangeDeletgates(false, value);
			value.ownerId = -2;
			AppShell.Instance.ServerConnection.ReleaseOwnership(networkComponent.goNetId);
		}
	}

	public void TransferOwnership(GameObject goNewOwner, GameObject go)
	{
		NetworkComponent networkComponent = go.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
		NetworkComponent networkComponent2 = goNewOwner.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
		NetEntity value;
		if (!(networkComponent == null) && !(networkComponent2 == null) && networkComponent2.NetOwnerId >= 0 && networkComponent.IsOwner() && networkComponent.NetOwnerId != networkComponent2.NetOwnerId && dictNetObjs.TryGetValue(networkComponent.goNetId, out value))
		{
			CallOwnershipChangeDeletgates(false, value);
			value.ownerId = networkComponent2.NetOwnerId;
			AppShell.Instance.ServerConnection.TransferOwnership(networkComponent2.NetOwnerId, networkComponent.goNetId);
		}
	}

	protected void AddEventHandlers()
	{
		AppShell.Instance.EventMgr.AddListener<EntitySpawnMessage>(OnEntitySpawn);
		AppShell.Instance.EventMgr.AddListener<EntityDespawnMessage>(OnEntityDespawn);
		AppShell.Instance.EventMgr.AddListener<OwnershipGoNetMessage>(OnOwnershipChange);
		AppShell.Instance.EventMgr.AddListener<EntityPolymorphMessage>(OnEntityPolymorph);
	}

	protected void RemoveEventHandlers()
	{
		AppShell.Instance.EventMgr.RemoveListener<EntitySpawnMessage>(OnEntitySpawn);
		AppShell.Instance.EventMgr.RemoveListener<EntityDespawnMessage>(OnEntityDespawn);
		AppShell.Instance.EventMgr.RemoveListener<OwnershipGoNetMessage>(OnOwnershipChange);
		AppShell.Instance.EventMgr.RemoveListener<EntityPolymorphMessage>(OnEntityPolymorph);
	}

	protected void OnEntitySpawn(EntitySpawnMessage msg)
	{


		NetworkComponent networkComponent = msg.go.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
		if (networkComponent == null)
		{
			return;
		}

		///// CSP added this block as a kudge fix ////////////////////////////////////////////
		// if this client is not the host, but a host exists, and this spawning entity is not
		//    a player, then 
		if (!networkComponent.goNetId.IsPlayerId()) {
		  networkComponent.NetOwnerId =  AppShell.Instance.ServerConnection.GetGameHostId();
		}
		//////////////////////////////////////////////////////////////////////////////////////

		bool flag = false;
		if (msg.go.name == "mr_placeholder")
		{
			flag = true;
		}
		NetEntity netEntity = null;
		PolymorphController component = msg.go.GetComponent<PolymorphController>();
		if (!networkComponent.goNetId.IsPlayerId() || component == null || !component.IsAPolymorph())
		{
			netEntity = AddNetEntity(msg.go);
		}
		else
		{
			AddInactiveNetEntity(msg.go);
		}
		if (networkComponent.goNetId.IsPlayerId())
		{
			int[] gameAllUserIds = AppShell.Instance.ServerConnection.GetGameAllUserIds();
			bool flag2 = true;
			for (int i = 0; i < gameAllUserIds.Length; i++)
			{
				if (gameAllUserIds[i] == networkComponent.goNetId.childId)
				{
					flag2 = false;
					break;
				}
			}
			if (flag2)
			{
				//GC.Collect();  // CSP - REMOVE THIS AFTER TESTING

				// block temporarily commented out by CSP
				//CspUtils.DebugLog("Destroying player " + networkComponent.goNetId + " since they are no longer in the room.");
				//RemoveNetEntity(networkComponent.goNetId);
				//Utils.DelayedDestroy(msg.go, 0.25f, true);
				//return;
			}
		}
		if (!flag && netEntity != null)
		{
			netEntity.spawnState = NetEntity.SpawnState.FinishedSpawning;
		}
		if ((msg.spawnType & CharacterSpawn.Type.Local) == 0 || flag)
		{
			return;
		}
		if (networkComponent.goNetId.IsPlayerId())
		{
			AppShell.Instance.ServerConnection.TakeOwnership(networkComponent.goNetId, false);
		}
		else
		{
			AppShell.Instance.ServerConnection.TakeOwnership(networkComponent.goNetId, true);
		}
		if (!msg.sendNewEntityMsg)
		{
			return;
		}
		NewEntityMessage newEntityMessage = networkComponent.GetNewEntityMessage();
		if (networkComponent.goNetId.IsPlayerId() && AppShell.Instance.IsHeroAuth && AppShell.Instance.Profile != null)
		{
			string data = NetworkMessage.ToEncodedString64(newEntityMessage);
			HeroCollection availableCostumes = AppShell.Instance.Profile.AvailableCostumes;
			string key;
			if (availableCostumes != null && availableCostumes.ContainsKey(newEntityMessage.modelName))
			{
				HeroPersisted heroPersisted = availableCostumes[newEntityMessage.modelName];
				key = heroPersisted.Code;
			}
			else
			{
				CspUtils.DebugLog("AvailableCostumes was null, or doesn't contain " + newEntityMessage.modelName + " so hero_code will be empty and this will liekly fail.");
				key = string.Empty;
			}
			AppShell.Instance.ServerConnection.SendHeroCreate(newEntityMessage.modelName, key, data);
		}
		else
		{
			CspUtils.DebugLog("OnEntitySpawn " + networkComponent.gameObject.name + " goNetID=" + newEntityMessage.goNetId.ToString() );
			AppShell.Instance.ServerConnection.SendGameMsg(newEntityMessage);
		}
	}

	protected void OnEntityDespawn(EntityDespawnMessage msg)
	{
		NetworkComponent networkComponent = msg.go.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
		if (!(networkComponent == null))
		{
			if (msg.sendDeleteEntityMsg && networkComponent.IsOwner() && networkComponent.goNetId.IsValid())
			{
				DeleteEntityMessage msg2 = new DeleteEntityMessage(networkComponent.goNetId);
				AppShell.Instance.ServerConnection.SendGameMsg(msg2);
			}
			if (networkComponent.IsOwner() && msg.releaseOwnership)
			{
				AppShell.Instance.ServerConnection.Game.ReleaseOwnership(msg.go);
			}
			if (networkComponent.goNetId.IsValid() && msg.sendDeleteEntityMsg)
			{
				RemoveNetEntity(networkComponent.goNetId);
			}
		}
	}

	protected void OnOwnershipChange(OwnershipGoNetMessage msg)
	{
		CspUtils.DebugLog("OnOwnershipChange ownerId=" + msg.ownerId);

		int gameUserId = AppShell.Instance.ServerConnection.GetGameUserId();
		bool flag = false;
		if (msg.ownerId == gameUserId)
		{
			flag = true;
		}
		if (msg.ownerId == -3)
		{
			foreach (NetEntity value2 in dictNetObjs.Values)
			{
				if (value2.ownerId >= -1)
				{
					value2.ownerId = -2;
					if (value2.ownershipDelegates != null && value2.ownershipDelegates.Count > 0)
					{
						CspUtils.DebugLog("Pending ownership delegates on an owned object?");
					}
				}
			}
		}
		else
		{
			foreach (GoNetId goNetId in msg.goNetIds)
			{
				NetEntity value;
				if (flag && goNetId.IsPlayerId() && goNetId.ChildId != gameUserId)
				{
					CspUtils.DebugLog("I got ownership of player " + goNetId.ToString() + " but I am player <" + gameUserId + ">");
				}
				else if (!dictNetObjs.TryGetValue(goNetId, out value))
				{
					if (flag)
					{
						CspUtils.DebugLog("Became owner of unknown object " + goNetId + ", releasing ownership");
						AppShell.Instance.ServerConnection.ReleaseOwnership(goNetId);
					}
					else
					{
						value = new NetEntity(goNetId);
						value.ownerId = msg.ownerId;
						dictNetObjs.Add(value.goNetId, value);
					}
				}
				else
				{
					if (value.netComp == null && flag)
					{
						CspUtils.DebugLog("Became owner of object without network component " + goNetId);
					}
					if (value.netComp != null && value.netComp.NetOwnerId == -3 && msg.ownerId < 0)
					{
						value.ownerId = -3;
						AppShell.Instance.ServerConnection.TakeOwnership(goNetId, true);
					}
					else
					{
						CspUtils.DebugLog("OnOwnershipChange - preparing to call delgates...");
						value.ownerId = msg.ownerId;
						CallOwnershipChangeDeletgates(flag, value);
						if (flag && value.netComp != null && value.netComp.gameObject != null)
						{
							value.netComp.gameObject.SendMessage("OnBecomeOwner", SendMessageOptions.DontRequireReceiver);
						}
					}
				}
			}
		}
	}

	private static void CallOwnershipChangeDeletgates(bool isNewOwner, NetEntity e)
	{
		foreach (OnOwnershipChangeDelegate ownershipDelegate in e.ownershipDelegates)
		{
			try
			{
				CspUtils.DebugLog("CallOwnershipChangeDeletgates method=" + ownershipDelegate.Method.Name);
					
				ownershipDelegate(e.netComp.gameObject, isNewOwner);
			}
			catch (Exception ex)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("Exception calling OnOwnershipChangeDelegate for goNetId ");
				stringBuilder.AppendLine(e.netComp.goNetId.ToString());
				stringBuilder.AppendLine(ex.ToString());
				CspUtils.DebugLog(stringBuilder.ToString());
			}
		}
		e.ownershipDelegates.Clear();
	}

	protected void OnEntityPolymorph(EntityPolymorphMessage msg)
	{
		if (msg.originalType == CharacterSpawn.Type.Unknown || msg.polymorphType == CharacterSpawn.Type.Unknown)
		{
			return;
		}
		NetworkComponent component = msg.original.GetComponent<NetworkComponent>();
		NetworkComponent component2 = msg.polymorph.GetComponent<NetworkComponent>();
		if (component == null || component2 == null)
		{
			return;
		}
		if (component.goNetId.IsPlayerId())
		{
			if (!msg.revert)
			{
				AddInactiveNetEntity(msg.original);
			}
			RemoveInactiveNetEntity(msg.polymorph);
			NetEntity netEntity = AddNetEntity(msg.polymorph);
			netEntity.spawnState = NetEntity.SpawnState.FinishedSpawning;
			component.TransferNetActions(component2, Time.time);
		}
		if ((msg.polymorphType & CharacterSpawn.Type.Local) != 0 && !msg.revert)
		{
			PolymorphSpawnMessage msg2 = new PolymorphSpawnMessage(component.goNetId, msg.polymorph.name, component2.goNetId);
			AppShell.Instance.ServerConnection.SendGameMsg(msg2);
		}
	}

	public void OnUserEnter(int userId)
	{
	}

	public void OnUserLeave(int userId)
	{
		GoNetId goNetId = new GoNetId(GoNetId.PLAYER_ID_FLAG, userId);
		NetEntity value;
		if (!dictNetObjs.TryGetValue(goNetId, out value))
		{
			return;
		}
		if (value.spawnData != null)
		{
			if ((value.spawnData.spawnType & CharacterSpawn.Type.Player) != 0)
			{
				RemoveNetEntity(goNetId);
				UnityEngine.Object.Destroy(value.netComp.gameObject);
			}
			else
			{
				CspUtils.DebugLog("Mismatch between goNetId and spawn type");
			}
		}
		else if (value.spawnState == NetEntity.SpawnState.FinishedSpawning)
		{
			CspUtils.DebugLog("Exiting player <" + userId + "> has a null SpawnData");
		}
	}
}
