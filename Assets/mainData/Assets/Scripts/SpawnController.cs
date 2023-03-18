using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : ScenarioEventHandlerEnableBase
{
	public enum SpawnOrder
	{
		Random,
		RoundRobin
	}

	public class SpawnChoice
	{
		public CharacterSpawn characterSpawn;

		public SpawnGroup spawnGroup;

		public SpawnChoice(CharacterSpawn newCharacterSpawn, SpawnGroup newSpawnGroup)
		{
			characterSpawn = newCharacterSpawn;
			spawnGroup = newSpawnGroup;
		}

		public void Spawn(UnityEngine.Object triggerer)
		{
			if (characterSpawn != null)
			{
				characterSpawn.Triggered(triggerer);
			}
			else
			{
				spawnGroup.Spawn(triggerer);
			}
		}

		public void Reset()
		{
			if (characterSpawn != null)
			{
				characterSpawn.goNetId = GoNetId.Invalid;
			}
		}

		public void DestroyAllSpawnedCharacters()
		{
			if (characterSpawn != null)
			{
				characterSpawn.DestroyAllSpawnedCharacters();
			}
			else
			{
				spawnGroup.DestroyAllSpawnedCharacters();
			}
		}

		public bool HasSpawner(CharacterSpawn spawner)
		{
			if (characterSpawn == spawner)
			{
				return true;
			}
			return spawnGroup != null && spawnGroup.HasSpawner(spawner);
		}

		public bool CheckPlayerCount()
		{
			if (characterSpawn != null)
			{
				return characterSpawn.CheckPlayerCount();
			}
			return spawnGroup.CheckPlayerCount();
		}
	}

	public int initialSpawns = 4;

	public int spawnWaveSize = 2;

	public bool waitForFullWave;

	public int minimumActiveSpawns = 3;

	public int maximumActiveSpawns = 5;

	public float spawnRateMultiplier4Player = 3f;

	public int totalSpawns = 10;

	public int totalSpawnMultiplier4Player = 3;

	public float spawnInterval = 5f;

	public float batchInterval = 0.3f;

	public string destructEvent = string.Empty;

	public SpawnOrder spawnOrder;

	public string spawnCompleteEvent = string.Empty;

	public string spawnDieEvent = string.Empty;

	public string spawnWakeEvent = string.Empty;

	protected bool activated;

	protected bool initialized;

	protected int currentSpawned;

	protected int totalSpawned;

	protected int totalKilled;

	protected int totalAwakened;

	protected float nextSpawnTime;

	protected float spawnMultiplier;

	protected int currentTotalSpawns;

	protected int currentSpawnWaveSize;

	protected int currentMinimumSpawns;

	protected int currentMaximumSpawns;

	protected bool isHost;

	protected List<SpawnChoice> invalidChoices;

	protected List<SpawnChoice> availableChoices;

	protected List<SpawnChoice> activeChoices;

	protected List<SpawnChoice> allChoices;

	protected SpawnGroup currentSpawnGroup;

	protected int batchRemaining;

	protected float batchNextSpawnTime;

	protected void GetAvailableSpawners()
	{
		int num = 99;
		CspUtils.DebugLog("GetAvailableSpawners() SpawnController=" + this.gameObject.name);
		for (int i = 0; i < base.transform.GetChildCount(); i++)
		{
			SpawnChoice spawnChoice = null;
			GameObject gameObject = base.transform.GetChild(i).gameObject;		
			CharacterSpawn characterSpawn = gameObject.GetComponent(typeof(CharacterSpawn)) as CharacterSpawn;			
			if (characterSpawn != null)
			{
				CspUtils.DebugLog("GetAvailableSpawners() characterSpawn=" + characterSpawn.gameObject.name);
				spawnChoice = new SpawnChoice(characterSpawn, null);
				if (characterSpawn.IsBoss && totalSpawnMultiplier4Player > 1)
				{
					CspUtils.DebugLog(base.gameObject.name + " with a Boss spawner has totalSpawnMultiplier4Player (" + totalSpawnMultiplier4Player + ") of greater than 1 and should probably be adjusted.  Temporarily setting to 1.");
					totalSpawnMultiplier4Player = 1;
				}
				num = 1;
			}
			SpawnGroup spawnGroup = gameObject.GetComponent(typeof(SpawnGroup)) as SpawnGroup;
			if (spawnGroup != null)
			{
				int availableSpawners = spawnGroup.GetAvailableSpawners();
				spawnGroup.SetSpawnController(this);
				spawnChoice = new SpawnChoice(null, spawnGroup);
				if (availableSpawners > 0 && availableSpawners < num)
				{
					num = availableSpawners;
				}
			}
			if (spawnChoice != null)
			{
				allChoices.Add(spawnChoice);
				if (spawnChoice.CheckPlayerCount())
				{
					availableChoices.Add(spawnChoice);
				}
				else
				{
					invalidChoices.Add(spawnChoice);
				}
			}
			else
			{
				BrawlerSpawnSource x = gameObject.GetComponent(typeof(BrawlerSpawnSource)) as BrawlerSpawnSource;
				if (!(x != null))
				{
					CspUtils.DebugLog("Spawn Controller " + base.gameObject.name + " has child (" + gameObject.name + ") of unknown type");
				}
			}
		}
		if (num > maximumActiveSpawns)
		{
			CspUtils.DebugLog(base.gameObject.name + " has MaximumActiveSpawns (" + maximumActiveSpawns + ") of less than the smallest group size (" + num + ") and should be adjusted.  Temporarily raising MaximumActiveSpawns to match.");
			maximumActiveSpawns = num;
		}
	}

	protected void CheckPlayerCount()
	{
		List<int> list = new List<int>();
		for (int num = availableChoices.Count - 1; num >= 0; num--)
		{
			if (!availableChoices[num].CheckPlayerCount())
			{
				list.Add(num);
			}
		}
		for (int num2 = invalidChoices.Count - 1; num2 >= 0; num2--)
		{
			if (invalidChoices[num2].CheckPlayerCount())
			{
				availableChoices.Add(invalidChoices[num2]);
				invalidChoices.RemoveAt(num2);
			}
		}
		foreach (int item in list)
		{
			invalidChoices.Add(availableChoices[item]);
			availableChoices.RemoveAt(item);
		}
	}

	protected override void Start()
	{
		base.Start();
		CspUtils.DebugLog("SpawnController=" + this.gameObject.name + "  spawnDieEvent=" + spawnDieEvent);
		availableChoices = new List<SpawnChoice>();
		allChoices = new List<SpawnChoice>();
		activeChoices = new List<SpawnChoice>();
		invalidChoices = new List<SpawnChoice>();
		GetAvailableSpawners();
		isHost = AppShell.Instance.ServerConnection.IsGameHost();
	}

	private void LateUpdate()
	{
		if (!activated)
		{
			return;
		}
		spawnMultiplier = calculateSpawnMultiplier();
		currentTotalSpawns = calculateTotalSpawns();
		int num = 0;
		if (totalSpawned == 0 && initialSpawns > 0)
		{
			nextSpawnTime = Time.time + spawnInterval;
			float num2 = (float)initialSpawns * spawnMultiplier;
			num = (int)num2;
		}
		else if (Time.time > nextSpawnTime)
		{
			nextSpawnTime = Time.time + spawnInterval;
			float num3 = (float)spawnWaveSize * spawnMultiplier;
			currentSpawnWaveSize = (int)num3;
			float num4 = (float)minimumActiveSpawns * spawnMultiplier;
			currentMinimumSpawns = (int)num4;
			float num5 = (float)maximumActiveSpawns * spawnMultiplier;
			currentMaximumSpawns = (int)num5;
			num = ((currentSpawnWaveSize + currentSpawned + batchRemaining < currentMinimumSpawns) ? (currentMinimumSpawns - (currentSpawned + batchRemaining)) : ((currentSpawned + currentSpawnWaveSize + batchRemaining <= currentMaximumSpawns) ? currentSpawnWaveSize : (currentMaximumSpawns - (currentSpawned + batchRemaining))));
			if (waitForFullWave && num + batchRemaining < currentSpawnWaveSize)
			{
				num = 0;
			}
		}
		if (!AppShell.Instance.ServerConnection.IsGameHost())
		{
			return;
		}
		if (!isHost)
		{
			OnBecomeHost();
		}
		if (currentTotalSpawns > 0 && num > currentTotalSpawns - (totalSpawned + batchRemaining))
		{
			num = currentTotalSpawns - (totalSpawned + batchRemaining);
		}
		batchRemaining += num;
		if (num > 0)
		{
			batchNextSpawnTime = Time.time;
		}
		if (batchRemaining <= 0 || !(Time.time >= batchNextSpawnTime))
		{
			return;
		}
		CheckPlayerCount();
		SpawnChoice spawnChoice = (batchRemaining < currentTotalSpawns - totalSpawned) ? chooseSpawner(batchRemaining) : chooseSpawner(9999);
		if (spawnChoice != null)
		{
			batchRemaining--;
			availableChoices.Remove(spawnChoice);
			activeChoices.Add(spawnChoice);

			// debug line started causing problems when SpawnChoice was a SpawnGroup (chracterSPawn would be null)
			// would get NPE and code following debug statment could not execute, so had to comment out for now.
			// would be nice to be able to trap these kind of errors...can you do that in UNity?
			try {	
			  CspUtils.DebugLog("SC LateUpdate characterSpawn.name=" + spawnChoice.characterSpawn.gameObject.name);
;			}
			catch (Exception e) {

			}
			spawnChoice.Spawn(base.gameObject);
			if (spawnChoice.characterSpawn != null)
			{
				AppShell.Instance.EventMgr.AddListener<EntityDespawnMessage>(spawnChoice.characterSpawn, OnTrackedCharacterDespawned);
			}
			else if (spawnChoice.spawnGroup.NumAvailableSpawns() == 0)
			{
				currentSpawnGroup = null;
			}
			else
			{
				currentSpawnGroup = spawnChoice.spawnGroup;
			}
			currentSpawned++;
			totalSpawned++;
		}
		if (batchRemaining > 0)
		{
			batchNextSpawnTime = Time.time + batchInterval;
		}
		else
		{
			currentSpawnGroup = null;
		}
	}

	public void RemoteSpawn(CharacterSpawn spawner)
	{
		currentSpawned++;
		totalSpawned++;
		AppShell.Instance.EventMgr.AddListener<EntityDespawnMessage>(spawner, OnTrackedCharacterDespawned);
		foreach (SpawnChoice allChoice in allChoices)
		{
			if (allChoice.HasSpawner(spawner))
			{
				availableChoices.Remove(allChoice);
				activeChoices.Add(allChoice);
				break;
			}
		}
	}

	public void RemoteGroupSpawn(SpawnGroup group)
	{
		currentSpawned++;
		totalSpawned++;
		foreach (SpawnChoice availableChoice in availableChoices)
		{
			if (availableChoice.spawnGroup == group)
			{
				availableChoices.Remove(availableChoice);
				activeChoices.Add(availableChoice);
				break;
			}
		}
	}

	public void OnSpawnGroupAvailable(SpawnGroup spawnGroup)
	{
		foreach (SpawnChoice allChoice in allChoices)
		{
			if (allChoice.spawnGroup == spawnGroup)
			{
				activeChoices.Remove(allChoice);
				if (allChoice.CheckPlayerCount())
				{
					availableChoices.Add(allChoice);
				}
				else
				{
					invalidChoices.Add(allChoice);
				}
			}
		}
	}

	public void OnSpawnGroupDespawn(SpawnGroup spawnGroup)
	{
		decreaseCurrentSpawn();
	}

	public bool IsFinishedSpawning()
	{
		currentTotalSpawns = calculateTotalSpawns();
		return currentTotalSpawns > 0 && totalSpawned >= currentTotalSpawns;
	}

	public bool AreAllSpawnsAwake()
	{
		return totalAwakened >= totalSpawned;
	}

	public bool AreAllSpawnsDead()
	{
		return totalKilled >= totalSpawned;
	}

	protected void OnTrackedCharacterDespawned(EntityDespawnMessage e)
	{
		if (e.cause != EntityDespawnMessage.despawnType.polymorph && e.cause != EntityDespawnMessage.despawnType.reload)
		{
			SpawnData spawnData = e.go.GetComponent(typeof(SpawnData)) as SpawnData;
			if (spawnData == null)
			{
				CspUtils.DebugLog("Could not find SpawnData when trying stop tracking the character <" + e.go.name + "> that just despawned!");
				return;
			}
			decreaseCurrentSpawn();
			foreach (SpawnChoice allChoice in allChoices)
			{
				if (allChoice.characterSpawn == spawnData.spawner)
				{
					allChoice.Reset();
					activeChoices.Remove(allChoice);
					if (allChoice.CheckPlayerCount())
					{
						availableChoices.Add(allChoice);
					}
					else
					{
						invalidChoices.Add(allChoice);
					}
				}
			}
			AppShell.Instance.EventMgr.RemoveListener<EntityDespawnMessage>(spawnData.spawner, OnTrackedCharacterDespawned);
		}
	}

	protected void decreaseCurrentSpawn()
	{
		currentSpawned--;
		currentTotalSpawns = calculateTotalSpawns();
		if (currentSpawned == 0 && currentTotalSpawns > 0 && totalSpawned >= currentTotalSpawns && spawnCompleteEvent != string.Empty)
		{
			activated = false;
			ScenarioEventManager.Instance.FireScenarioEvent(spawnCompleteEvent, false);
		}
	}

	protected void OnTrackedPlayerCharacterKilled(CombatPlayerKilledMessage msg)
	{
		OnTrackedCharacterKilled(msg.Character);
	}

	protected void OnTrackedCombatCharacterKilled(CombatCharacterKilledMessage msg)
	{
		OnTrackedCharacterKilled(msg.Character);
	}

	protected void OnTrackedCharacterKilled(GameObject character)
	{
		if (!(character == null))
		{
			SpawnData component = character.GetComponent<SpawnData>();
			if (!(component == null) && !(component.spawner == null))
			{
				foreach (SpawnChoice allChoice in allChoices)
				{
					if (allChoice.HasSpawner(component.spawner))
					{
						IncreaseTotalKilled();
					}
				}
			}
		}
	}

	protected void OnTrackedCharacterAwakened(CombatCharacterAwakenedMessage msg)
	{
		if (!(msg.Character == null))
		{
			SpawnData component = msg.Character.GetComponent<SpawnData>();
			if (!(component == null) && !(component.spawner == null))
			{
				foreach (SpawnChoice allChoice in allChoices)
				{
					if (allChoice.HasSpawner(component.spawner))
					{
						IncreaseTotalAwakened();
					}
				}
			}
		}
	}

	protected void IncreaseTotalKilled()
	{
		totalKilled++;
		currentTotalSpawns = calculateTotalSpawns();
		CspUtils.DebugLog("ITK SpawnController=" + this.gameObject.name + "  spawnDieEvent=" + spawnDieEvent);
		CspUtils.DebugLog("currentTotalSpawns =" + currentTotalSpawns);
		CspUtils.DebugLog("totalSpawned =" + totalSpawned);
		CspUtils.DebugLog("totalKilled =" + totalKilled);
		if (currentTotalSpawns > 0 && totalSpawned >= currentTotalSpawns && totalKilled >= totalSpawned && spawnDieEvent != string.Empty && AppShell.Instance.ServerConnection.IsGameHost())
		{
			CspUtils.DebugLog("ITK fire SpawnController=" + this.gameObject.name + "  spawnDieEvent=" + spawnDieEvent);
			ScenarioEventManager.Instance.FireScenarioEvent(spawnDieEvent, false);
		}
	}

	protected void IncreaseTotalAwakened()
	{
		totalAwakened++;
		if (IsFinishedSpawning() && AreAllSpawnsAwake() && spawnWakeEvent != string.Empty && AppShell.Instance.ServerConnection.IsGameHost())
		{
			ScenarioEventManager.Instance.FireScenarioEvent(spawnWakeEvent, false);
		}
	}

	protected override void OnEnableEvent(string eventName)
	{
		activated = true;
		if (!initialized)
		{
			initialized = true;
			if (destructEvent != string.Empty)
			{
				ScenarioEventManager.Instance.SubscribeScenarioEvent(destructEvent, OnDestructEvent);
			}
			if (spawnDieEvent != string.Empty || spawnCompleteEvent != string.Empty)
			{
				AppShell.Instance.EventMgr.AddListener<CombatCharacterKilledMessage>(OnTrackedCombatCharacterKilled);
				AppShell.Instance.EventMgr.AddListener<CombatPlayerKilledMessage>(OnTrackedPlayerCharacterKilled);
			}
			if (spawnWakeEvent != string.Empty)
			{
				AppShell.Instance.EventMgr.AddListener<CombatCharacterAwakenedMessage>(OnTrackedCharacterAwakened);
			}
		}
	}

	protected override void OnDisableEvent(string eventName)
	{
		activated = false;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (!initialized)
		{
			return;
		}
		if (AppShell.Instance != null)
		{
			if (spawnDieEvent != string.Empty || spawnCompleteEvent != string.Empty)
			{
				AppShell.Instance.EventMgr.RemoveListener<CombatPlayerKilledMessage>(OnTrackedPlayerCharacterKilled);
				AppShell.Instance.EventMgr.RemoveListener<CombatCharacterKilledMessage>(OnTrackedCombatCharacterKilled);
			}
			if (spawnWakeEvent != string.Empty)
			{
				AppShell.Instance.EventMgr.RemoveListener<CombatCharacterAwakenedMessage>(OnTrackedCharacterAwakened);
			}
		}
		if (destructEvent != string.Empty && ScenarioEventManager.Instance != null)
		{
			ScenarioEventManager.Instance.UnsubscribeScenarioEvent(destructEvent, OnDestructEvent);
		}
	}

	private void OnDestructEvent(string eventName)
	{
		foreach (SpawnChoice allChoice in allChoices)
		{
			allChoice.DestroyAllSpawnedCharacters();
		}
		activated = false;
		batchRemaining = 0;
	}

	public override void ManualReset()
	{
		currentSpawned = 0;
		totalSpawned = 0;
		totalKilled = 0;
		totalAwakened = 0;
		nextSpawnTime = 0f;
		GetAvailableSpawners();
	}

	public void OnBecomeHost()
	{
		if (!isHost)
		{
			isHost = true;
			if (!string.IsNullOrEmpty(spawnWakeEvent) && !ScenarioEventManager.Instance.IsEventRecorded(spawnWakeEvent) && IsFinishedSpawning() && AreAllSpawnsAwake())
			{
				ScenarioEventManager.Instance.FireScenarioEvent(spawnWakeEvent, false);
			}

			CspUtils.DebugLog("OBH SpawnController=" + this.gameObject.name + "  spawnDieEvent=" + spawnDieEvent);
			if (!string.IsNullOrEmpty(spawnDieEvent) && !ScenarioEventManager.Instance.IsEventRecorded(spawnDieEvent) && IsFinishedSpawning() && AreAllSpawnsDead())
			{
				CspUtils.DebugLog("OBH fire SpawnController=" + this.gameObject.name + "  spawnDieEvent=" + spawnDieEvent);
				ScenarioEventManager.Instance.FireScenarioEvent(spawnDieEvent, false);
			}
		}
	}

	protected SpawnChoice chooseSpawner(int batchSize)
	{
		if (currentSpawnGroup != null && currentSpawnGroup.NumAvailableSpawns() > 0)
		{
			return new SpawnChoice(null, currentSpawnGroup);
		}
		if (availableChoices.Count == 0)
		{
			return null;
		}
		switch (spawnOrder)
		{
		case SpawnOrder.Random:
			return chooseRandomSpawner(batchSize);
		case SpawnOrder.RoundRobin:
			return chooseNextSpawner(batchSize);
		default:
			return null;
		}
	}

	protected SpawnChoice chooseRandomSpawner(int batchSize)
	{
		List<SpawnChoice> list = new List<SpawnChoice>();
		foreach (SpawnChoice availableChoice in availableChoices)
		{
			if (availableChoice.spawnGroup != null && availableChoice.spawnGroup.NumAvailableSpawns() <= batchSize)
			{
				list.Add(availableChoice);
			}
		}
		if (list.Count > 0)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			return list[index];
		}
		List<SpawnChoice> list2 = new List<SpawnChoice>();
		foreach (SpawnChoice availableChoice2 in availableChoices)
		{
			if (availableChoice2.spawnGroup == null)
			{
				list2.Add(availableChoice2);
			}
		}
		if (list2.Count > 0)
		{
			int index2 = UnityEngine.Random.Range(0, list2.Count);
			return list2[index2];
		}
		return null;
	}

	protected SpawnChoice chooseNextSpawner(int batchSize)
	{
		foreach (SpawnChoice availableChoice in availableChoices)
		{
			if (availableChoice.spawnGroup != null && availableChoice.spawnGroup.NumAvailableSpawns() <= batchSize)
			{
				return availableChoice;
			}
		}
		foreach (SpawnChoice availableChoice2 in availableChoices)
		{
			if (availableChoice2.spawnGroup == null)
			{
				return availableChoice2;
			}
		}
		return null;
	}

	protected int calculateTotalSpawns()
	{
		if (PlayerCombatController.GetPlayerCount() <= 1)
		{
			return totalSpawns;
		}
		float num = PlayerCombatController.GetPlayerCount() - 1;
		num /= 3f;
		if (totalSpawnMultiplier4Player < 1)
		{
			CspUtils.DebugLog(base.gameObject.name + " has totalSpawnMultiplier4Player of less than one (" + totalSpawnMultiplier4Player + ") and should be changed.  Temporarily adjusting to 1 to avoid problems.");
			totalSpawnMultiplier4Player = 1;
		}
		return (int)((float)totalSpawns * (1f + (float)(totalSpawnMultiplier4Player - 1) * num));
	}

	protected float calculateSpawnMultiplier()
	{
		if (PlayerCombatController.GetPlayerCount() <= 1)
		{
			return 1f;
		}
		float num = PlayerCombatController.GetPlayerCount() - 1;
		if (spawnRateMultiplier4Player < 1f)
		{
			CspUtils.DebugLog(base.gameObject.name + " has spawnRateMultiplier4Player of less than one (" + spawnRateMultiplier4Player + ") and should be changed.  Temporarily adjusting to 1 to avoid problems.");
			spawnRateMultiplier4Player = 1f;
		}
		return 1f + (spawnRateMultiplier4Player - 1f) * (num / 3f);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "SpawnControllerIcon.png");
	}

	public override void DrawTriggerGizmo(string triggerEventName, GameObject triggerObject)
	{
		base.DrawTriggerGizmo(triggerEventName, triggerObject);
		if (triggerEventName == destructEvent)
		{
			Gizmos.color = Color.gray;
			Gizmos.DrawLine(base.gameObject.transform.position, triggerObject.transform.position);
			Gizmos.DrawSphere(base.transform.position, 0.5f);
		}
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		if (!(spawnCompleteEvent != string.Empty) && !(spawnDieEvent != string.Empty))
		{
			return;
		}
		GameObject[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		ScenarioEventHandlerBase scenarioEventHandlerBase = null;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			scenarioEventHandlerBase = (gameObject.GetComponent(typeof(ScenarioEventHandlerBase)) as ScenarioEventHandlerBase);
			if (scenarioEventHandlerBase != null)
			{
				if (spawnCompleteEvent != string.Empty)
				{
					scenarioEventHandlerBase.DrawTriggerGizmo(spawnCompleteEvent, base.gameObject);
				}
				if (spawnDieEvent != string.Empty)
				{
					scenarioEventHandlerBase.DrawTriggerGizmo(spawnDieEvent, base.gameObject);
				}
			}
		}
	}
}
