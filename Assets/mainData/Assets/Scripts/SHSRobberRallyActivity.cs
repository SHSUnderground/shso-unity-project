using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHSRobberRallyActivity : SHSActivityBase
{
	private float activityEndTime;

	private List<ActivityObject> activityObjects;

	private List<RobberSpawn> spawnPoints;

	private List<AIControllerRobber> robbers = new List<AIControllerRobber>();

	private AIControllerRobber activeRobber;

	private int coinsToCollect;

	private float nextRobberSpawnTime;

	private bool rallyInProgress;

	private ISHSCounterType activationTimeCounter;

	private SHSIndicatorArrow indicatorArrow;

	private float kActiveRespawnDelaySeconds;

	private float kInactiveRespawnDelaySeconds;

	private float kDefaultActivityTime;

	public override void Configure(SHSActivityManager manager, DataWarehouse data)
	{
		base.Configure(manager, data);
		kActiveRespawnDelaySeconds = activityConfigurationData.GetFloat("configuration/active_respawn_delay_seconds");
		kInactiveRespawnDelaySeconds = activityConfigurationData.GetFloat("configuration/inactive_respawn_delay_seconds");
		kDefaultActivityTime = activityConfigurationData.GetFloat("configuration/default_rally_duration");
		AIControllerRobber.kActivityName = Name;
	}

	public override void OnZoneLoaded(string zoneName, int zoneID)
	{
		spawnPoints = new List<RobberSpawn>();
		spawnPoints.AddRange(Object.FindObjectsOfType(typeof(RobberSpawn)) as RobberSpawn[]);
		activationTimeCounter = AppShell.Instance.CounterManager.GetCounter(Name + "." + zoneName);
		double num = 0.0;
		if (activationTimeCounter != null)
		{
			num = activationTimeCounter.GetCurrentValue();
		}
		float num2 = (float)(ServerTime.time - num);
		if (num2 >= kInactiveRespawnDelaySeconds)
		{
			nextRobberSpawnTime = -1f;
			SpawnRobber();
		}
		else
		{
			float a = kInactiveRespawnDelaySeconds - num2;
			nextRobberSpawnTime = Time.time + Mathf.Min(a, kActiveRespawnDelaySeconds);
		}
		base.OnZoneLoaded(zoneName, zoneID);
	}

	public override void OnZoneUnloaded(string zoneName)
	{
		if (activityObjects != null)
		{
			activityObjects.Clear();
		}
		if (spawnPoints != null)
		{
			spawnPoints.Clear();
		}
		if (robbers != null)
		{
			robbers.Clear();
		}
		activeRobber = null;
		base.OnZoneUnloaded(zoneName);
	}

	public void BeginRally(AIControllerRobber robber)
	{
		if (rallyInProgress)
		{
			return;
		}
		if (robber == null)
		{
			CspUtils.DebugLog("Attempted to start Robber Rally without an associated Robber object");
			return;
		}
		if (robber.AssignedPath == null)
		{
			CspUtils.DebugLog("Attempted to start Robber Rally without an assigned path");
			return;
		}
		foreach (AIControllerRobber robber2 in robbers)
		{
			if (robber2 != robber)
			{
				robber2.Despawn();
			}
		}
		robbers.Clear();
		DisableCollision(robber.gameObject);
		activityObjects = new List<ActivityObject>();
		NPCPathNode startNode = robber.AssignedPath.GetStartNode();
		InitializeNode(startNode, startNode);
		coinsToCollect = 0;
		if (robber.AssignedPath is RobberPath)
		{
			RobberPath robberPath = robber.AssignedPath as RobberPath;
			activityEndTime = Time.time + robberPath.timeLimit;
		}
		else
		{
			activityEndTime = Time.time + kDefaultActivityTime;
		}
		if (activationTimeCounter != null)
		{
			activationTimeCounter.SetCounter((long)ServerTime.time);
		}
		nextRobberSpawnTime = Time.time + kActiveRespawnDelaySeconds;
		if (indicatorArrow == null)
		{
			indicatorArrow = (GUIManager.Instance["/SHSMainWindow/SHSSocialMainWindow/SHSIndicatorArrow"] as SHSIndicatorArrow);
		}
		indicatorArrow.arrowTarget = robber.gameObject;
		indicatorArrow.Show();
		activeRobber = robber;
		result = ActivityResultEnum.InProgress;
		rallyInProgress = true;
		robber.BeginActivity(this);
	}

	public override void Update()
	{
		base.Update();
		if (rallyInProgress && Time.time > activityEndTime)
		{
			result = ActivityResultEnum.Failure;
			Complete();
		}
		if (nextRobberSpawnTime >= 0f && Time.time >= nextRobberSpawnTime)
		{
			nextRobberSpawnTime = -1f;
			SpawnRobber();
		}
	}

	public override void Complete()
	{
		base.Complete();
		if (indicatorArrow != null)
		{
			indicatorArrow.Hide();
			indicatorArrow = null;
		}
		rallyInProgress = false;
		if (activeRobber == null)
		{
			CspUtils.DebugLog("Robber rally is complete, but there is no active robber.  This is a bug!");
			Reset();
		}
		else if (result == ActivityResultEnum.Success)
		{
			AppShell.Instance.EventMgr.Fire(this, new ChallengeEventMessage("ui_message", "sc31_robber_caught"));
			activeRobber.StartCoroutine(DelayedFinish(activeRobber));
		}
		else if (result == ActivityResultEnum.Failure)
		{
			activeRobber.StartCoroutine(DelayedLose(activeRobber));
		}
		else
		{
			activeRobber.Despawn();
			Reset();
		}
	}

	public void Win()
	{
		result = ActivityResultEnum.Success;
		Complete();
	}

	public override void Reset()
	{
		base.Reset();
		activeRobber = null;
		rallyInProgress = false;
	}

	private void SpawnRobber()
	{
		if (spawnPoints.Count > 0)
		{
			RobberSpawn robberSpawn = spawnPoints[Random.Range(0, spawnPoints.Count)];
			robberSpawn.onSpawnCallback += OnRobberSpawned;
			robberSpawn.SpawnOnTime(0f);
		}
		if (activationTimeCounter != null)
		{
			activationTimeCounter.SetCounter(0L);
		}
	}

	public void SpawnAllRobbers()
	{
		foreach (AIControllerRobber robber in robbers)
		{
			robber.Despawn();
		}
		robbers.Clear();
		foreach (RobberSpawn spawnPoint in spawnPoints)
		{
			spawnPoint.onSpawnCallback += OnRobberSpawned;
			spawnPoint.SpawnOnTime(0f);
		}
		nextRobberSpawnTime = -1f;
	}

	public void SpawnRobber(string robberName)
	{
		foreach (AIControllerRobber robber in robbers)
		{
			if (robber.gameObject.name.ToLower() == robberName.ToLower())
			{
				robber.Despawn();
				break;
			}
		}
		foreach (RobberSpawn spawnPoint in spawnPoints)
		{
			if (spawnPoint.gameObject.transform.parent != null && spawnPoint.gameObject.transform.parent.gameObject.name.ToLower() == robberName.ToLower())
			{
				spawnPoint.onSpawnCallback += OnRobberSpawned;
				spawnPoint.SpawnOnTime(0f);
				break;
			}
		}
		nextRobberSpawnTime = -1f;
	}

	private void OnRobberSpawned(GameObject obj)
	{
		SpawnData componentInChildren = obj.GetComponentInChildren<SpawnData>();
		if (componentInChildren == null)
		{
			CspUtils.DebugLog("Robber spawned without spawn data");
			Object.Destroy(obj);
			return;
		}
		RobberSpawn robberSpawn = componentInChildren.spawner as RobberSpawn;
		if (robberSpawn == null)
		{
			CspUtils.DebugLog("Robber spawned without a valid RobberSpawn");
			Object.Destroy(obj);
			return;
		}
		GameObject gameObject = componentInChildren.gameObject;
		AIControllerRobber aIControllerRobber = gameObject.AddComponent(typeof(AIControllerRobber)) as AIControllerRobber;
		aIControllerRobber.path = robberSpawn.path;
		aIControllerRobber.startSFX = robberSpawn.startSFX;
		aIControllerRobber.caughtSFX = robberSpawn.caughtSFX;
		aIControllerRobber.consideredHuman = robberSpawn.consideredHuman;
		if (!string.IsNullOrEmpty(robberSpawn.caughtAnimation))
		{
			aIControllerRobber.caughtAnimation = robberSpawn.caughtAnimation;
		}
		if (!string.IsNullOrEmpty(robberSpawn.gotAwayAnimation))
		{
			aIControllerRobber.gotAwayAnimation = robberSpawn.gotAwayAnimation;
		}
		if (!string.IsNullOrEmpty(robberSpawn.caughtEffectName))
		{
			aIControllerRobber.caughtEffectName = robberSpawn.caughtEffectName;
		}
		if (!string.IsNullOrEmpty(robberSpawn.gotAwayEffectName))
		{
			aIControllerRobber.gotAwayEffectName = robberSpawn.gotAwayEffectName;
		}
		aIControllerRobber.despawnEffect = robberSpawn.despawnEffect;
		aIControllerRobber.stareAtPlayer = robberSpawn.StareAtPlayer;
		aIControllerRobber.stareAtCameraWhileInactive = robberSpawn.StareAtCameraWhileInactive;
		aIControllerRobber.robberMoveSpeed = robberSpawn.robberMoveSpeed;
		robbers.Add(aIControllerRobber);
		if (state == ActivityStateEnum.AwaitingAchievementData)
		{
			aIControllerRobber.gameObject.SetActiveRecursively(false);
			blockedGameObjects.Add(aIControllerRobber.gameObject);
		}
	}

	private void InitializeNode(NPCPathNode node, NPCPathNode startNode)
	{
		ActivitySpawnPoint component = Utils.GetComponent<ActivitySpawnPoint>(node.gameObject);
		if (component != null)
		{
			component.activityReference = this;
			component.SpawnActivityObject();
		}
		NPCPathNode[] nextNodes = node.nextNodes;
		foreach (NPCPathNode nPCPathNode in nextNodes)
		{
			if (nPCPathNode != startNode)
			{
				InitializeNode(nPCPathNode, startNode);
			}
		}
	}

	public override void RegisterActivityObject(ActivityObject activityObject)
	{
		base.RegisterActivityObject(activityObject);
		activityObject.Deactivate();
		activityObjects.Add(activityObject);
		ParticleRenderer[] components = Utils.GetComponents<ParticleRenderer>(activityObject.gameObject, Utils.SearchChildren);
		ParticleRenderer[] array = components;
		foreach (ParticleRenderer particleRenderer in array)
		{
			if (particleRenderer.name.Contains("inactive"))
			{
				particleRenderer.enabled = false;
			}
		}
		NPCPathNode component = Utils.GetComponent<NPCPathNode>(activityObject.spawner);
		if (component != null && component.nextNodes.Length > 0 && component.nextNodes[0] != null)
		{
			Vector3 position = component.nextNodes[0].transform.position;
			Vector3 position2 = activityObject.gameObject.transform.position;
			position.y = position2.y;
			activityObject.gameObject.transform.LookAt(position);
		}
	}

	public override void OnActivityAction(ActivityObjectActionNameEnum action, ActivityObject activityObject, object extraData)
	{
		base.OnActivityAction(action, activityObject, extraData);
		activityObjects.Remove(activityObject);
		activityObject.Despawn();
		coinsToCollect--;
	}

	public void OnRobberLeaveNode(NPCPathNode node)
	{
		ActivitySpawnPoint component = Utils.GetComponent<ActivitySpawnPoint>(node.gameObject);
		if (component != null)
		{
			foreach (ActivityObject activityObject in activityObjects)
			{
				if (activityObject.spawner == component)
				{
					coinsToCollect++;
					activityObject.Activate();
					break;
				}
			}
		}
	}

	private IEnumerator DelayedFinish(AIControllerRobber robber)
	{
		yield return new WaitForSeconds(robber.OnCaught());
		robber.Despawn();
		string localCharacter = GameController.GetController().LocalCharacter;
		if (localCharacter != null)
		{
			AppShell.Instance.CounterManager.AddCounter(Name, localCharacter);
		}
		CspUtils.DebugLog(string.Empty + robber.gameObject.name);
		CspUtils.DebugLog(string.Empty + robber.gameObject);
		string zoneName = OwnableDefinition.simpleZoneName(SocialSpaceControllerImpl.getZoneName());
		if (robber.gameObject.name == "npc_robber_02")
		{
			if (zoneName == "asgard")
			{
				AppShell.Instance.EventReporter.ReportAchievementEvent(GameController.GetController().LocalPlayer.name, "social_zone", "asgard_gold_loki", 1, string.Empty);
			}
			else
			{
				AppShell.Instance.EventReporter.ReportAchievementEvent(GameController.GetController().LocalPlayer.name, "social_zone", "vv_junkyard_dog", 1, string.Empty);
			}
		}
		else if (robber.gameObject.name == "npc_robber_03")
		{
			AppShell.Instance.EventReporter.ReportAchievementEvent(GameController.GetController().LocalPlayer.name, "social_zone", "vv_junkyard_dog", 1, string.Empty);
		}
		else if (robber.gameObject.name.Contains("seasonal"))
		{
			AppShell.Instance.EventReporter.ReportAchievementEvent(GameController.GetController().LocalPlayer.name, "social_zone", robber.gameObject.name, 1, string.Empty);
		}
		AppShell.Instance.EventReporter.ReportAchievementEvent(GameController.GetController().LocalPlayer.name, "social_pickup_robber", "robber", 1, string.Empty);
		SocialSpaceControllerImpl.bumpIdleTimer();
		Reset();
	}

	private IEnumerator DelayedLose(AIControllerRobber robber)
	{
		yield return new WaitForSeconds(robber.OnGotAway());
		robber.Despawn();
		Reset();
	}

	private void DisableCollision(GameObject robber)
	{
		Collider[] components = Utils.GetComponents<Collider>(robber, Utils.SearchChildren);
		Object[] array = Object.FindObjectsOfType(typeof(CharacterController));
		for (int i = 0; i < array.Length; i++)
		{
			CharacterController characterController = (CharacterController)array[i];
			Collider[] array2 = components;
			foreach (Collider collider in array2)
			{
				if (characterController != collider)
				{
					Physics.IgnoreCollision(characterController, collider);
				}
			}
		}
	}
}
