using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Net/Network Component")]
public class NetworkComponent : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected const float NO_MESSAGE_PROBLEM_TIME = 10f;

	public GoNetId goNetId = GoNetId.Invalid;

	public float netActionBufferTime = 0.25f;

	protected List<NetAction> netActionPositionQueue;

	protected List<NetAction> netActionQueue;

	protected CharacterGlobals charGlobals;

	protected CharacterMotionController characterMotionController;

	protected BehaviorManager behaviorManager;

	protected CombatController combatController;

	protected DebugHistory debugHistory;

	protected float lastFullPosition;

	protected Vector3 lastDestinationSet;

	protected bool preProcessed;

	protected bool remotePositionOnGround;

	protected float lastMessageTime;

	protected bool disconnectReported;

	protected int netOwnerId = -1;  // CSP  -1=no owner(?)   -2=pending release(?)   -3=pending ownership

	protected bool isOwner;

	public int NetOwnerId
	{
		get
		{
			return netOwnerId;
		}
		set
		{
			netOwnerId = value;
			isOwner = false;
			//isOwner = true;  // CSP - temporarily set to true for testing.

			// this block added by CSP ...set owner to true if char uses AI //////////////
			//AIControllerBrawler temp = GetComponent(typeof(AIControllerBrawler)) as AIControllerBrawler;
			//if (temp != null)
			//	isOwner = true;
			//////////////////////////////////////////


			CspUtils.DebugLog("bgo name=" + base.gameObject.name + " netOwnerId= " + netOwnerId + 
				"GetGameUserId()= " + AppShell.Instance.ServerConnection.GetGameUserId());
			if (netOwnerId >= 0 && netOwnerId == AppShell.Instance.ServerConnection.GetGameUserId())
			{
				CspUtils.DebugLog("bgo true name=" + base.gameObject.name + " netOwnerId= " + netOwnerId);
				isOwner = true;
			}
			SpawnData spawnData = GetComponent(typeof(SpawnData)) as SpawnData;
			if (spawnData != null)
			{
				if (isOwner)
				{
					spawnData.spawnType &= ~CharacterSpawn.Type.Remote;
					spawnData.spawnType |= CharacterSpawn.Type.Local;
				}
				else
				{
					spawnData.spawnType &= ~CharacterSpawn.Type.Local;
					spawnData.spawnType |= CharacterSpawn.Type.Remote;
				}
			}
		}
	}

	public bool IsOwner()
	{
		return isOwner;
	}

	public bool IsOwnedBySomeoneElse()
	{
		return !isOwner && netOwnerId >= 0;
	}

	public bool IsPendingOwner()
	{
		return netOwnerId == -3;
	}

	private void Start()
	{
		charGlobals = (base.gameObject.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals);
		characterMotionController = (base.gameObject.GetComponent(typeof(CharacterMotionController)) as CharacterMotionController);
		combatController = (base.gameObject.GetComponent(typeof(CombatController)) as CombatController);
		behaviorManager = (base.gameObject.GetComponent(typeof(BehaviorManager)) as BehaviorManager);
		debugHistory = (base.gameObject.GetComponent(typeof(DebugHistory)) as DebugHistory);
		netActionPositionQueue = new List<NetAction>();
		netActionQueue = new List<NetAction>();
		lastFullPosition = -1f;
		preProcessed = false;
		remotePositionOnGround = false;
		lastMessageTime = 0f;
		disconnectReported = false;
		if (goNetId.IsStaticId())
		{
			AppShell.Instance.EventMgr.Fire(null, new EntitySpawnMessage(base.gameObject, CharacterSpawn.Type.Static));
		}
	}

	private void Update()
	{
		if (IsOwnedBySomeoneElse() && !disconnectReported && lastMessageTime + 10f < Time.time && base.gameObject.GetComponent<PlayerCombatController>() != null && !base.gameObject.GetComponent<PlayerCombatController>().isKilled)
		{
			disconnectReported = true;
			AppShell.Instance.EventMgr.Fire(base.gameObject, new NetworkConnectionProblem(base.gameObject, false));
		}
		while (netActionPositionQueue.Count > 0 && netActionPositionQueue[0].timestamp <= Time.time)
		{
			ProcessActionPosition();
		}
		while (netActionQueue.Count > 0 && netActionQueue[0].timestamp <= Time.time)
		{
			NetAction netAction = netActionQueue[0];
			netActionQueue.RemoveAt(0);
			ProcessAction(netAction);
			ProcessActionOnInactiveEntities(netAction);
		}
		if (characterMotionController != null && behaviorManager.MotionEnabled)
		{
			characterMotionController.MotionUpdate();
		}
	}

	protected void checkYDiscrepancy(Vector3 newDestination)
	{
		if (remotePositionOnGround)
		{
			Vector3 position = base.transform.position;
			float y = position.y;
			Vector3 destination = charGlobals.motionController.getDestination();
			float num = y - destination.y;
			if (num < -0.1f && charGlobals.motionController.getVerticalVelocity() <= 0f)
			{
				charGlobals.motionController.teleportToDestination();
			}
		}
	}

	public void ProcessActionPosition()
	{
		//CspUtils.DebugLog("ProcessActionPosition() called!");  // CSP
		NetAction netAction = netActionPositionQueue[0];
		netActionPositionQueue.RemoveAt(0);
		if (debugHistory != null)
		{
			debugHistory.AddAction(netAction);
		}
		NetActionPositionFull netActionPositionFull = netAction as NetActionPositionFull;
		if (netActionPositionFull != null)
		{
			if (!characterMotionController.NetUpdatesDisabled)
			{
				//return;  // CSP testing, remove soon!!!

				lastFullPosition = Time.time;
				NetActionPositionFullRotation netActionPositionFullRotation = netAction as NetActionPositionFullRotation;
				checkYDiscrepancy(netActionPositionFull.position);
				remotePositionOnGround = netActionPositionFull.onGround;
				if (netActionPositionFullRotation != null)
				{
					//CspUtils.DebugLog("NetActionPositionFullRotation recvd!");  //CSP
					characterMotionController.setDestination(netActionPositionFull.position, netActionPositionFullRotation.lookVector);
				}
				else
				{
					//CspUtils.DebugLog("NetActionPositionFull recvd!");   //CSP
					characterMotionController.setDestination(netActionPositionFull.position);
				}
				behaviorManager.getBehavior().destinationChanged();
				lastDestinationSet = netActionPositionFull.position;
			}
		}
		else if (netAction.getType() == NetAction.NetActionType.NetActionPositionUpdate)
		{
			if (!characterMotionController.NetUpdatesDisabled && lastFullPosition >= 0f)
			{
				//return;  // CSP testing, remove soon!!!

				//CspUtils.DebugLog("NetActionPositionUpdate recvd!");  //CSP
				NetActionPositionUpdate netActionPositionUpdate = netAction as NetActionPositionUpdate;
				lastDestinationSet += netActionPositionUpdate.getDelta();
				checkYDiscrepancy(lastDestinationSet);
				remotePositionOnGround = netActionPositionUpdate.onGround;
				characterMotionController.setDestination(lastDestinationSet);
				behaviorManager.getBehavior().destinationChanged();
			}
			return;
		}
		if (characterMotionController != null)
		{
			netAction.timestamp += characterMotionController.positionSampleRate;
		}
		netActionQueue.Add(netAction);
	}

	protected void ProcessAction(NetAction netAction)
	{
		if (debugHistory != null)
		{
			debugHistory.AddAction(netAction);
		}
		NetActionPositionFull netActionPositionFull = netAction as NetActionPositionFull;
		if (netActionPositionFull != null)
		{
			this.combatController.setHealth(netActionPositionFull.health);
			PlayerCombatController playerCombatController = this.combatController as PlayerCombatController;
			if (playerCombatController != null)
			{
				playerCombatController.setPower(netActionPositionFull.power);
			}
		}
		switch (netAction.getType())
		{
		case NetAction.NetActionType.NetActionPositionUpdate:
		case NetAction.NetActionType.NetActionPositionFull:
		case NetAction.NetActionType.NetActionPositionFullRotation:
			break;
		case NetAction.NetActionType.NetAction:
			CspUtils.DebugLog("Received base NetAction");
			break;
		case NetAction.NetActionType.NetActionJump:
			characterMotionController.jumpPressed();
			break;
		case NetAction.NetActionType.NetActionJumpCancel:
			characterMotionController.netJumpReleased(netAction as NetActionJumpCancel);
			break;
		case NetAction.NetActionType.NetActionEmote:
		{
			NetActionEmote netActionEmote = netAction as NetActionEmote;
			BehaviorEmote behaviorEmote = behaviorManager.getBehavior() as BehaviorEmote;
			if ((behaviorEmote == null || !behaviorEmote.Looping || behaviorEmote.EmoteID != netActionEmote.emoteId) && behaviorManager.allowUserInput())
			{
				if (netActionEmote.targetedPlayer != null && netActionEmote.targetedPlayer == GameController.GetController().LocalPlayer)
				{
					AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_event", "emote_to_player", 1, -10000, -10000, netActionEmote.emoteId.ToString(), string.Empty);
				}
				BehaviorEmote behaviorEmote2 = behaviorManager.requestChangeBehavior(typeof(BehaviorEmote), false) as BehaviorEmote;
				if (behaviorEmote2 != null && !behaviorEmote2.Initialize(netActionEmote.emoteId))
				{
					behaviorManager.endBehavior();
				}
			}
			break;
		}
		case NetAction.NetActionType.NetActionDirectedMenuChat:
		{
			NetActionDirectedMenuChat netActionDirectedMenuChat = netAction as NetActionDirectedMenuChat;
			MenuChatGroup menuChatGroup = new MenuChatGroup();
			menuChatGroup.PhraseKey = netActionDirectedMenuChat.group.PhraseKey;
			string emoteId = netActionDirectedMenuChat.group.EmoteId;
			menuChatGroup.EmoteId = ((!string.IsNullOrEmpty(emoteId)) ? emoteId : null);
			DirectedMenuChat.DirectMenuChatEmote(base.gameObject, menuChatGroup, netActionDirectedMenuChat.targetPlayer);
			break;
		}
		case NetAction.NetActionType.NetActionSit:
		{
			NetActionSit netActionSit = netAction as NetActionSit;
			characterMotionController.teleportTo(netActionSit.position);
			characterMotionController.transform.rotation = Quaternion.LookRotation(netActionSit.lookVector);
			behaviorManager.requestChangeBehavior(typeof(BehaviorSit), false);
			break;
		}
		case NetAction.NetActionType.NetActionStand:
		{
			BehaviorSit behaviorSit = behaviorManager.getBehavior() as BehaviorSit;
			if (behaviorSit != null)
			{
				behaviorSit.stand();
			}
			break;
		}
		case NetAction.NetActionType.NetActionHotSpot:
		{
			NetActionHotSpot netActionHotSpot = netAction as NetActionHotSpot;
			netActionHotSpot.hotSpot.UseHotSpot(base.gameObject);
			break;
		}
		case NetAction.NetActionType.NetActionHotSpotController:
		{
			NetActionHotSpotController netActionHotSpotController = netAction as NetActionHotSpotController;
			netActionHotSpotController.hotSpot.StartWithPlayer(base.gameObject);
			break;
		}
		case NetAction.NetActionType.NetActionMultiHotspotSync:
		{
			NetActionMultiHotspotSync netActionMultiHotspotSync = netAction as NetActionMultiHotspotSync;
			MultiHotspotCoordinator component = Utils.GetComponent<MultiHotspotCoordinator>(base.gameObject);
			if (component != null)
			{
				component.ReceiveLaunchTime(netActionMultiHotspotSync.serverLaunchTime);
			}
			break;
		}
		case NetAction.NetActionType.NetActionInteractiveObjectController:
		{
			NetActionInteractiveObjectController netActionInteractiveObjectController = netAction as NetActionInteractiveObjectController;
			if (netActionInteractiveObjectController.hotSpot != null && netActionInteractiveObjectController.hotSpot.CanPlayerUse(base.gameObject))
			{
				netActionInteractiveObjectController.hotSpot.StartWithPlayer(base.gameObject, null);
			}
			break;
		}
		case NetAction.NetActionType.NetActionAttack:
		{
			NetActionAttack netActionAttack = netAction as NetActionAttack;
			if (!this.combatController.isKilled && !this.combatController.isHidden)
			{
				CombatController.AttackData attackData3 = AttackDataManager.Instance.getAttackData(netActionAttack.attackName);
				if (attackData3 != null)
				{
					characterMotionController.teleportTo(netActionAttack.position, netActionAttack.lookVector);
					this.combatController.createAttackBehavior(netActionAttack.targetObject, attackData3, false, true);
				}
			}
			break;
		}
		case NetAction.NetActionType.NetActionImpact:
		{
			NetActionImpact netActionImpact = netAction as NetActionImpact;
			if (netActionImpact.sourceObject != null)
			{
				CombatController.AttackData attackData2 = AttackDataManager.Instance.getAttackData(netActionImpact.attackName);
				CombatController.ImpactData impactData = attackData2.impacts[netActionImpact.impactIndex];
				this.combatController.hitByAttackRemote(netActionImpact.impactPosition, netActionImpact.sourceObject, netActionImpact.damage, impactData);
			}
			break;
		}
		case NetAction.NetActionType.NetActionHitObject:
		{
			NetActionHitObject netActionHitObject = netAction as NetActionHitObject;
			if (netActionHitObject.hitObject != null)
			{
				ObjectCombatController objectCombatController = netActionHitObject.hitObject.GetComponent(typeof(ObjectCombatController)) as ObjectCombatController;
				if (objectCombatController != null)
				{
					objectCombatController.takeDamageRemote(netActionHitObject.damage, base.gameObject);
				}
			}
			break;
		}
		case NetAction.NetActionType.NetActionDie:
			if (!behaviorManager.getBehavior().GetType().IsInstanceOfType(typeof(BehaviorDie)))
			{
				NetActionDie netActionDie = netAction as NetActionDie;
				this.combatController.killed(netActionDie.attacker, netActionDie.duration);
			}
			break;
		case NetAction.NetActionType.NetActionPickupThrowable:
		{
			NetActionPickupThrowable netActionPickupThrowable = netAction as NetActionPickupThrowable;
			if (!(netActionPickupThrowable.pickupObject != null))
			{
				break;
			}
			BehaviorPickupThrowable behaviorPickupThrowable = behaviorManager.requestChangeBehavior(typeof(BehaviorPickupThrowable), true) as BehaviorPickupThrowable;
			if (behaviorPickupThrowable != null)
			{
				ThrowableGround throwableGroundComponent = netActionPickupThrowable.pickupObject.GetComponent(typeof(ThrowableGround)) as ThrowableGround;
				if (!behaviorPickupThrowable.Initialize(throwableGroundComponent))
				{
					behaviorManager.endBehavior();
				}
			}
			break;
		}
		case NetAction.NetActionType.NetActionDropThrowable:
		{
			NetActionDropThrowable netActionDropThrowable = netAction as NetActionDropThrowable;
			characterMotionController.dropThrowableRemote(netActionDropThrowable.silent);
			break;
		}
		case NetAction.NetActionType.NetActionPickupPickup:
		{
			NetActionPickupPickup netActionPickupPickup = netAction as NetActionPickupPickup;
			if (netActionPickupPickup.pickupObject != null)
			{
				Pickup pickup = netActionPickupPickup.pickupObject.GetComponent(typeof(Pickup)) as Pickup;
				if (pickup != null)
				{
					pickup.OnPickup(this.combatController);
				}
			}
			break;
		}
		case NetAction.NetActionType.NetActionExitDoor:
		{
			NetActionExitDoor netActionExitDoor = netAction as NetActionExitDoor;
			netActionExitDoor.doorManager.ExitWithPlayer(base.gameObject, null, false);
			break;
		}
		case NetAction.NetActionType.NetActionCancel:
			behaviorManager.endBehavior();
			break;
		case NetAction.NetActionType.NetActionProjectile:
		{
			NetActionProjectile netActionProjectile = netAction as NetActionProjectile;
			if (netActionProjectile.target == null)
			{
				break;
			}
			CombatController combatController = netActionProjectile.target.GetComponent(typeof(CombatController)) as CombatController;
			CombatController.AttackData attackData = AttackDataManager.Instance.getAttackData(netActionProjectile.attackName);
			if (!netActionProjectile.projectileObjectID.IsValid())
			{
				ImpactProjectile impactProjectile = CombatController.ImpactData.CreateImpact(attackData, netActionProjectile.impactIndex, charGlobals, combatController) as ImpactProjectile;
				impactProjectile.CreateProjectile(charGlobals, null, null);
				impactProjectile.LaunchProjectile(combatController);
				break;
			}
			GameObject gameObjectFromNetId = AppShell.Instance.ServerConnection.Game.GetGameObjectFromNetId(netActionProjectile.projectileObjectID);
			if (gameObjectFromNetId != null)
			{
				ProjectileConverter projectileConverter = gameObjectFromNetId.GetComponent(typeof(ProjectileConverter)) as ProjectileConverter;
				if (projectileConverter != null)
				{
					ImpactObject impactObject = CombatController.ImpactData.CreateImpact(attackData, netActionProjectile.impactIndex, charGlobals, combatController) as ImpactObject;
					impactObject.launchObjectAt(projectileConverter, combatController);
				}
			}
			break;
		}
		case NetAction.NetActionType.NetActionCombatEffect:
		{
			NetActionCombatEffect netActionCombatEffect = netAction as NetActionCombatEffect;
			if (netActionCombatEffect.remove)
			{
				this.combatController.removeCombatEffectRemote(netActionCombatEffect.combatEffectName);
			}
			else
			{
				this.combatController.createCombatEffectRemote(netActionCombatEffect.combatEffectName, netActionCombatEffect.source, netActionCombatEffect.usePrefabSource);
			}
			break;
		}
		case NetAction.NetActionType.NetActionPlayerStatus:
		{
			NetActionPlayerStatus netActionPlayerStatus = netAction as NetActionPlayerStatus;
			PlayerStatus.SetStatus(charGlobals, netActionPlayerStatus.status, true);
			break;
		}
		case NetAction.NetActionType.NetActionMultishotUpdate:
		{
			NetActionMultishotUpdate netActionMultishotUpdate = netAction as NetActionMultishotUpdate;
			BehaviorAttackMultiShot behaviorAttackMultiShot = behaviorManager.getBehavior() as BehaviorAttackMultiShot;
			if (behaviorAttackMultiShot != null)
			{
				behaviorAttackMultiShot.GetNewTargets(netActionMultishotUpdate.nextAttackTime, netActionMultishotUpdate.firstTarget, netActionMultishotUpdate.secondTarget);
			}
			break;
		}
		case NetAction.NetActionType.NetActionPolymorph:
		{
			NetActionPolymorph netActionPolymorph = netAction as NetActionPolymorph;
			if (!(charGlobals.polymorphController == null))
			{
				if (netActionPolymorph.revert)
				{
					charGlobals.polymorphController.RemoteRevert();
				}
				else
				{
					charGlobals.polymorphController.RemotePolymorphCharacter();
				}
			}
			break;
		}
		case NetAction.NetActionType.NetActionChargeCollision:
		{
			BehaviorAttackCharge behaviorAttackCharge = behaviorManager.getBehavior() as BehaviorAttackCharge;
			if (behaviorAttackCharge != null)
			{
				behaviorAttackCharge.OnNetMotionCollided();
			}
			break;
		}
		case NetAction.NetActionType.NetActionLeveledUpStart:
		case NetAction.NetActionType.NetActionLeveledUpEnd:
		case NetAction.NetActionType.NetActionChallengeUpStart:
		case NetAction.NetActionType.NetActionChallengeUpEnd:
			(netAction as NetActionCelebrate).Process(charGlobals);
			break;
		case NetAction.NetActionType.NetActionVO:
			VOManager.Instance.PlayNetVO(netAction as NetActionVO);
			break;
		}
	}

	public void QueueNetAction(NetAction action)
	{
		//CspUtils.DebugLog("QueueNetAction IsOwner()=" + IsOwner());   //CSP
		//CspUtils.DebugLog("QueueNetAction goNetId child/parent=" + goNetId.childId + " " + goNetId.parentId);
		if (goNetId.IsValid() && IsOwner())
		{
			QueueNetActionRelay(action);
		}
		else
		{
			CspUtils.DebugLog(string.Format("Invalid NetAction <{0}> test {1},{2}", action.GetType().ToString(), goNetId.IsValid().ToString(), IsOwner().ToString()));
		}
	}

	public void QueueNetActionIgnoringOwnership(NetAction action)
	{
		if (goNetId.IsValid())
		{
			QueueNetActionRelay(action);
		}
		else
		{
			CspUtils.DebugLog(string.Format("Invalid NetAction <{0}> test {1}", action.GetType().ToString(), goNetId.IsValid().ToString()));
		}
	}

	protected void QueueNetActionRelay(NetAction action)
	{
		NetActionPositionFull netActionPositionFull = action as NetActionPositionFull;
		if (netActionPositionFull != null)
		{
			netActionPositionFull.onGround = charGlobals.motionController.IsOnGround();
			charGlobals.motionController.positionSent(true);
		}
		else
		{
			NetActionPositionUpdate netActionPositionUpdate = action as NetActionPositionUpdate;
			if (netActionPositionUpdate != null)
			{
				netActionPositionUpdate.onGround = charGlobals.motionController.IsOnGround();
				charGlobals.motionController.positionSent(false);
			}
		}
		//CspUtils.DebugLog("QueueNetActionRelay NetActionPositionFull");    //CSP
		AppShell.Instance.ServerConnection.Game.QueueNetAction(goNetId, action);
	}

	public void ProcessMessage(NetworkMessage msg)
	{
		if (disconnectReported)
		{
			disconnectReported = false;
			AppShell.Instance.EventMgr.Fire(base.gameObject, new NetworkConnectionProblem(base.gameObject, true));
		}
		lastMessageTime = Time.time;
		if (msg.GetType() == typeof(NetActionMessage))
		{
			NetActionMessage netActionMessage = msg as NetActionMessage;
			float num = 0f;
			float num2 = Time.time;
			if (charGlobals != null)
			{
				num2 += charGlobals.motionController.positionSampleRate;
			}
			foreach (NetAction action in netActionMessage.actions)
			{
				if (num == 0f)
				{
					num = action.timestamp;
					action.timestamp = num2;
				}
				else
				{
					action.timestamp = num2 + (action.timestamp - num);
				}
				netActionPositionQueue.Add(action);
			}
		}
		else if (msg.GetType() == typeof(AssignTargetMessage))
		{
			AssignTargetMessage assignTargetMessage = msg as AssignTargetMessage;
			AIControllerBrawler aIControllerBrawler = GetComponent(typeof(AIControllerBrawler)) as AIControllerBrawler;
			if (aIControllerBrawler != null)
			{
				aIControllerBrawler.netAssignTarget(assignTargetMessage.target);
			}
			else
			{
				CspUtils.DebugLog("Unable to get AIController for " + base.gameObject.name);
			}
		}
		else if (msg.GetType() == typeof(RemoteImpactMessage))
		{
			RemoteImpactMessage remoteImpactMessage = msg as RemoteImpactMessage;
			if (remoteImpactMessage.sourceObject != null)
			{
				CombatController.AttackData attackData = AttackDataManager.Instance.getAttackData(remoteImpactMessage.attackName);
				CombatController.ImpactData impactData = attackData.impacts[remoteImpactMessage.impactIndex];
				CombatController sourceCombatController = remoteImpactMessage.sourceObject.GetComponent(typeof(CombatController)) as CombatController;
				combatController.hitByAttackLocal(remoteImpactMessage.impactPosition + base.transform.position, sourceCombatController, attackData.attackName, remoteImpactMessage.damage, impactData);
			}
		}
		else if (msg.GetType() == typeof(ScenarioEventServerTimeMessage))
		{
			ScenarioEventServerTimeMessage scenarioEventServerTimeMessage = msg as ScenarioEventServerTimeMessage;
			ScenarioEventSynchronousTransferBox component = GetComponent<ScenarioEventSynchronousTransferBox>();
			if (component != null)
			{
				component.SynchronizeToHost(scenarioEventServerTimeMessage.serverTime);
			}
			else
			{
				CspUtils.DebugLog("NetworkComponent::ProcessMessage() - failed to get synchronous transfer box for server time message");
			}
		}
	}

	public NewEntityMessage GetNewEntityMessage()
	{
		Hashtable hashtable = new Hashtable();
		SendMessage("CollectNetworkState", hashtable, SendMessageOptions.DontRequireReceiver);
		string modelName = string.Empty;
		CharacterSpawn.Type type = CharacterSpawn.Type.Static;
		SpawnData spawnData = GetComponent(typeof(SpawnData)) as SpawnData;
		GameObject spawner = null;
		if (spawnData != null)
		{
			type = spawnData.spawnType;
			modelName = spawnData.modelName;
			if (spawnData.spawner != null)
			{
				if (spawnData.spawner.spawnerNetwork != null && spawnData.spawner.IsLocal)
				{
					spawner = spawnData.spawner.gameObject;
				}
				if (spawnData.spawner.RecordHistory)
				{
					hashtable.Add("RecordHistory", 1);
				}
			}
		}
		type &= ~CharacterSpawn.Type.Local;
		type |= CharacterSpawn.Type.Remote;
		NewEntityMessage newEntityMessage = new NewEntityMessage(goNetId);
		newEntityMessage.modelName = modelName;
		newEntityMessage.spawnType = type;
		newEntityMessage.pos = base.gameObject.transform.position;
		newEntityMessage.rot = base.gameObject.transform.rotation;
		newEntityMessage.spawner = spawner;
		newEntityMessage.extraData = hashtable;
		return newEntityMessage;
	}

	public bool IsNextPositionAvailable()
	{
		if (netActionPositionQueue.Count > 0 && netActionPositionQueue[0].timestamp <= Time.time + characterMotionController.positionSampleRate)
		{
			return true;
		}
		return false;
	}

	public void AnnounceObjectSpawn(GameObject newObject, string componentName, string prefabName)
	{
		AnnounceObjectSpawn(newObject, componentName, prefabName, null);
	}

	public void AnnounceObjectSpawn(GameObject newObject, string componentName, string prefabName, GameObject parentObject)
	{
		NetworkComponent networkComponent = newObject.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
		GoNetId invalid = GoNetId.Invalid;
		if (networkComponent != null)
		{
			networkComponent.goNetId = AppShell.Instance.ServerConnection.Game.GetNewDynamicId();
			invalid = networkComponent.goNetId;
		}
		ObjectSpawnMessage msg = new ObjectSpawnMessage(goNetId, componentName, (!(parentObject == null)) ? (newObject.transform.position - parentObject.transform.position) : newObject.transform.position, newObject.transform.rotation, invalid, prefabName, parentObject);
		AppShell.Instance.ServerConnection.SendGameMsg(msg);
		AppShell.Instance.EventMgr.Fire(base.gameObject, new EntitySpawnMessage(newObject, CharacterSpawn.Type.Unknown));
	}

	public void TransferNetActions(NetworkComponent toNetwork, float afterTime)
	{
		TransferNetActions(netActionPositionQueue, toNetwork.netActionPositionQueue, afterTime);
		TransferNetActions(netActionQueue, toNetwork.netActionQueue, afterTime);
	}

	private void TransferNetActions(List<NetAction> fromlist, List<NetAction> toList, float afterTime)
	{
		List<NetAction> list = new List<NetAction>();
		foreach (NetAction item in fromlist)
		{
			if (item.timestamp > afterTime)
			{
				int num = 0;
				foreach (NetAction to in toList)
				{
					if (to.timestamp > item.timestamp)
					{
						break;
					}
					num++;
				}
				if (num < toList.Count)
				{
					toList.Insert(num, item);
				}
				else
				{
					toList.Add(item);
				}
				list.Add(item);
			}
		}
		foreach (NetAction item2 in list)
		{
			fromlist.Remove(item2);
		}
		list.Clear();
	}

	public void ProcessActionOnInactiveEntities(NetAction netAction)
	{
		if (netAction.getType() != NetAction.NetActionType.NetActionCombatEffect && netAction.getType() != NetAction.NetActionType.NetActionPolymorph)
		{
			return;
		}
		if (netAction.getType() == NetAction.NetActionType.NetActionCombatEffect)
		{
			NetActionCombatEffect netActionCombatEffect = netAction as NetActionCombatEffect;
			if (!netActionCombatEffect.remove)
			{
				return;
			}
		}
		AppShell.Instance.ServerConnection.Game.ForEachInactiveNetEntity(goNetId, delegate(NetGameManager.NetEntity e)
		{
			if (e != null && e.netComp != null)
			{
				e.netComp.ProcessAction(netAction);
			}
		});
	}
}
