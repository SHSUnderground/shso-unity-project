using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSTroubleBotActivity : SHSActivityBase
{
	private class QueuedSpawnerInfo
	{
		public Vector3 position;

		public Quaternion rotation;

		public QueuedSpawnerInfo(Vector3 position, Quaternion rotation)
		{
			this.position = position;
			this.rotation = rotation;
		}
	}

	private class actionInfoClass
	{
		public ActivityObject activityObject;

		public GameObject currentPlayer;

		public BotSmashActionState state;
	}

	private enum BotSmashActionState
	{
		Splatting,
		WaitingForSplat,
		Splatted
	}

	public enum ActivityCycleEnum
	{
		Second,
		Minute,
		Daily,
		Hourly,
		Infinite
	}

	private List<GameObject> activitySpawnersList;

	private List<GameObject> activityObjectsList;

	private Dictionary<string, Queue<QueuedSpawnerInfo>> queuedSpawnersByZone;

	private int initialSpawn;

	private int maximumSpawn;

	private float spawnRate;

	private ActivityCycleEnum activityCycle;

	private float spawnInterval;

	private string counterKey = string.Empty;

	private ISHSCounterType counterType;

	private ISHSCounterType counterTypeTime;

	private int initialBots;

	private int newBots;

	private int deployedBots;

	private int zoneMaximum;

	private int xpEarnedPerTroublebot;

	private List<actionInfoClass> currentActivityActions;

	private List<actionInfoClass> deleteActivityActionsQueue;

	private EffectSequence seq;

	private DateTime LastSpawnTime
	{
		get
		{
			if (counterTypeTime != null)
			{
				long currentValue = counterTypeTime.GetCurrentValue();
				if (currentValue == 1)
				{
					return DateTime.MaxValue;
				}
				return DateTime.UtcNow.AddSeconds((double)currentValue - ServerTime.time);
			}
			return DateTime.UtcNow;
		}
		set
		{
			if (counterTypeTime != null)
			{
				if (value > DateTime.UtcNow)
				{
					counterTypeTime.SetCounter(1L);
				}
				else
				{
					counterTypeTime.SetCounter((long)(ServerTime.time - (DateTime.UtcNow - value).TotalSeconds));
				}
			}
		}
	}

	public override void Configure(SHSActivityManager manager, DataWarehouse data)
	{
		base.Configure(manager, data);
		initialSpawn = activityConfigurationData.GetInt("configuration/startcount");
		spawnRate = activityConfigurationData.GetFloat("configuration/spawn_rate");
		activityCycle = (ActivityCycleEnum)(int)Enum.Parse(typeof(ActivityCycleEnum), activityConfigurationData.GetString("configuration/spawn_interval"));
		if (spawnRate > 0f)
		{
			spawnInterval = convertTimeRangeToSeconds(1f, activityCycle) / spawnRate;
		}
		else
		{
			spawnInterval = 2.14748365E+09f;
		}
		maximumSpawn = activityConfigurationData.GetInt("configuration/max_spawn");
		xpEarnedPerTroublebot = activityConfigurationData.GetInt("configuration/xp_earned");
		currentActivityActions = new List<actionInfoClass>();
		deleteActivityActionsQueue = new List<actionInfoClass>();
		queuedSpawnersByZone = new Dictionary<string, Queue<QueuedSpawnerInfo>>();
	}

	public override void Start()
	{
		base.Start();
		activitySpawnersList = new List<GameObject>();
		activityObjectsList = new List<GameObject>();
		configureActivitySpawners();
		counterType = AppShell.Instance.CounterManager.GetCounter(counterKey);
		counterTypeTime = AppShell.Instance.CounterManager.GetCounter(counterKey + "_ts");
		computeTroubleBotsToDeploy(counterType, counterTypeTime);
		deployBots(initialBots, true, false);
		deployBots(newBots, false, false);
		if (counterTypeTime != null && counterTypeTime.GetCurrentValue() != 0L && DateTime.UtcNow > LastSpawnTime)
		{
			double num = convertTimeRangeToSeconds(1f, activityCycle);
			LastSpawnTime = LastSpawnTime.AddSeconds(num / (double)spawnRate * (double)newBots);
		}
	}

	private void computeTroubleBotsToDeploy(ISHSCounterType counter, ISHSCounterType timestamp)
	{
		if (counterType == null || timestamp == null)
		{
			return;
		}
		if (timestamp.GetCurrentValue() == 0L)
		{
			initialBots = initialSpawn;
		}
		zoneMaximum = Math.Min(maximumSpawn, activitySpawnersList.Count);
		initialBots = Math.Min((int)counter.GetCurrentValue(), zoneMaximum);
		if (initialBots < counter.GetCurrentValue())
		{
			counter.SetCounter(initialBots);
			LastSpawnTime = DateTime.MaxValue;
		}
		if (initialBots <= 0)
		{
			initialBots = 1;
			counter.SetCounter(initialBots);
			LastSpawnTime = DateTime.UtcNow;
		}
		DateTime serverTimeInDateTime = ServerTime.Instance.GetServerTimeInDateTime();
		DateTime lastSpawnTime = LastSpawnTime;
		if (lastSpawnTime < serverTimeInDateTime)
		{
			if ((serverTimeInDateTime - lastSpawnTime).TotalHours > 1.0)
			{
				newBots = zoneMaximum - initialBots;
			}
		}
		else
		{
			newBots = 0;
		}
	}

	private int getInactiveCycleCount(DateTime serverDate, DateTime cachedDate)
	{
		int result = 0;
		switch (activityCycle)
		{
		case ActivityCycleEnum.Daily:
		{
			DateTime d = new DateTime(serverDate.Year, serverDate.Month, serverDate.Day);
			DateTime d2 = new DateTime(cachedDate.Year, cachedDate.Month, cachedDate.Day);
			result = (int)Math.Floor((d - d2).TotalDays);
			break;
		}
		case ActivityCycleEnum.Second:
			result = (int)(serverDate - cachedDate).TotalSeconds;
			break;
		case ActivityCycleEnum.Minute:
			result = (int)(serverDate - cachedDate).TotalMinutes;
			break;
		case ActivityCycleEnum.Hourly:
			result = (int)(serverDate - cachedDate).TotalHours;
			break;
		default:
			CspUtils.DebugLog("Activity doesn't support activity cycle: " + activityCycle.ToString());
			break;
		}
		return result;
	}

	private GameObject findSpawnerByLocation(QueuedSpawnerInfo queuedInfo)
	{
		GameObject[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			ActivitySpawnPoint component = Utils.GetComponent<ActivitySpawnPoint>(gameObject);
			if (component != null && component.transform.position == queuedInfo.position && component.transform.rotation == queuedInfo.rotation)
			{
				return gameObject;
			}
		}
		return null;
	}

	private void deployBots(int count, bool ignoreCounterIncrement, bool recordSpawnTime)
	{
		for (int i = 0; i < count; i++)
		{
			ActivitySpawnPoint nextSpawner = getNextSpawner();
			if (nextSpawner != null)
			{
				ActivityObject component = Utils.GetComponent<ActivityObject>(nextSpawner, Utils.SearchChildren);
				if (component != null)
				{
					CspUtils.DebugLog("spawner: " + nextSpawner + " is set to be not used, but has an activity spawner on it!");
				}
				nextSpawner.SpawnActivityObject();
				deployedBots++;
				if (deployedBots == zoneMaximum)
				{
					LastSpawnTime = DateTime.MaxValue;
				}
				else if (recordSpawnTime)
				{
					LastSpawnTime = DateTime.UtcNow;
				}
			}
			else
			{
				LastSpawnTime = DateTime.MaxValue;
			}
		}
		if (!ignoreCounterIncrement && counterType != null)
		{
			counterType.AddCounter(count);
		}
	}

	public void deployAll()
	{
		foreach (GameObject activitySpawners in activitySpawnersList)
		{
			ActivitySpawnPoint component = Utils.GetComponent<ActivitySpawnPoint>(activitySpawners);
			ActivityObject component2 = Utils.GetComponent<ActivityObject>(activitySpawners, Utils.SearchChildren);
			if (component != null && component2 == null)
			{
				component.SpawnActivityObject();
			}
		}
	}

	private ActivitySpawnPoint getNextSpawner()
	{
		GameObject gameObject = null;
		ActivitySpawnPoint activitySpawnPoint = null;
		bool flag = false;
		if (queuedSpawnersByZone.ContainsKey(currentZone) && queuedSpawnersByZone[currentZone].Count > 0)
		{
			QueuedSpawnerInfo queuedInfo = queuedSpawnersByZone[currentZone].Dequeue();
			gameObject = findSpawnerByLocation(queuedInfo);
			if (gameObject != null)
			{
				activitySpawnPoint = Utils.GetComponent<ActivitySpawnPoint>(gameObject);
				flag = true;
			}
		}
		if (!flag)
		{
			flag = true;
			int count = activitySpawnersList.Count;
			if (count == 0)
			{
				return null;
			}
			int num = UnityEngine.Random.Range(0, count);
			int num2 = num;
			bool flag2 = false;
			gameObject = activitySpawnersList[num2];
			activitySpawnPoint = Utils.GetComponent<ActivitySpawnPoint>(gameObject);
			while (activitySpawnPoint.used && (num != num2 || !flag2))
			{
				flag = false;
				num2++;
				if (num2 >= count)
				{
					num2 = 0;
					flag2 = true;
				}
				gameObject = activitySpawnersList[num2];
				activitySpawnPoint = Utils.GetComponent<ActivitySpawnPoint>(gameObject);
				if (!activitySpawnPoint.used)
				{
					flag = true;
					break;
				}
			}
		}
		return (!flag) ? null : activitySpawnPoint;
	}

	private void configureActivitySpawners()
	{
		GameObject[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			ActivitySpawnPoint component = Utils.GetComponent<ActivitySpawnPoint>(gameObject);
			if (component != null && component.activity.ToUpper() == Name)
			{
				activitySpawnersList.Add(gameObject);
				component.activityReference = this;
			}
		}
	}

	public override void OnZoneLoaded(string zoneName, int zoneID)
	{
		SocialSpaceController socialSpaceController = GameController.GetController() as SocialSpaceController;
		if (socialSpaceController != null && socialSpaceController.isTestScene)
		{
			zoneName = "Daily_Bugle";
		}
		counterKey = Name + "." + zoneName.ToUpper();
		base.OnZoneLoaded(zoneName, zoneID);
	}

	public override void OnZoneUnloaded(string zoneName)
	{
		base.OnZoneUnloaded(zoneName);
		if (State == ActivityStateEnum.Started)
		{
			if (queuedSpawnersByZone.ContainsKey(currentZone))
			{
				queuedSpawnersByZone[currentZone].Clear();
			}
			queuedSpawnersByZone[currentZone] = new Queue<QueuedSpawnerInfo>();
			foreach (GameObject activitySpawners in activitySpawnersList)
			{
				if (!(activitySpawners == null))
				{
					ActivitySpawnPoint component = Utils.GetComponent<ActivitySpawnPoint>(activitySpawners);
					if (component != null && component.used)
					{
						queuedSpawnersByZone[currentZone].Enqueue(new QueuedSpawnerInfo(activitySpawners.transform.position, activitySpawners.transform.rotation));
					}
				}
			}
		}
	}

	public override void Update()
	{
		base.Update();
		deleteActivityActionsQueue.Clear();
		if (counterType != null && counterTypeTime != null && State == ActivityStateEnum.Started)
		{
			foreach (actionInfoClass currentActivityAction in currentActivityActions)
			{
				if (UpdateBotSmashingAction(currentActivityAction))
				{
					deleteActivityActionsQueue.Add(currentActivityAction);
				}
			}
			foreach (actionInfoClass item in deleteActivityActionsQueue)
			{
				currentActivityActions.Remove(item);
			}
			if (zoneMaximum > counterType.GetCurrentValue() && DateTime.UtcNow - LastSpawnTime > TimeSpan.FromSeconds(spawnInterval))
			{
				deployBots(1, false, true);
			}
		}
	}

	public override void Complete()
	{
		base.Complete();
	}

	public override void Reset()
	{
		result = ActivityResultEnum.Dormant;
		base.Reset();
	}

	public override void RegisterActivityObject(ActivityObject activityObject)
	{
		activityObjectsList.Add(activityObject.gameObject);
		activityObject.Activate();
		AppShell.Instance.EventMgr.AddListener<ActivityObjectDespawnMessage>(activityObject.gameObject, OnActivityObjectDespawned);
	}

	public override void UnRegisterActivityObject(ActivityObject activityObject)
	{
		activityObjectsList.Remove(activityObject.gameObject);
		AppShell.Instance.EventMgr.RemoveListener<ActivityObjectDespawnMessage>(activityObject.gameObject, OnActivityObjectDespawned);
	}

	private void OnActivityObjectDespawned(ActivityObjectDespawnMessage msg)
	{
		ActivityObject component = Utils.GetComponent<ActivityObject>(msg.go);
		UnRegisterActivityObject(component);
	}

	public override void OnActivityAction(ActivityObjectActionNameEnum action, ActivityObject activityObject, object extraData)
	{
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		if (!(localPlayer == null))
		{
			StartBotSmashingAction(activityObject, localPlayer);
		}
	}

	private void StartBotSmashingAction(ActivityObject activityObject, GameObject currentPlayer)
	{
		actionInfoClass actionInfoClass = new actionInfoClass();
		actionInfoClass.activityObject = activityObject;
		actionInfoClass.currentPlayer = currentPlayer;
		actionInfoClass.state = BotSmashActionState.Splatting;
		currentActivityActions.Add(actionInfoClass);
		result = ActivityResultEnum.InProgress;
	}

	private bool UpdateBotSmashingAction(actionInfoClass action)
	{
		bool result = false;
		switch (action.state)
		{
		case BotSmashActionState.Splatting:
			if (manager.EffectPrefabBundle != null)
			{
				AIControllerNPC[] array = Utils.FindObjectsOfType<AIControllerNPC>();
				AIControllerNPC[] array2 = array;
				foreach (AIControllerNPC aIControllerNPC in array2)
				{
					if ((aIControllerNPC.gameObject.transform.position - action.activityObject.transform.position).magnitude <= 10f)
					{
						sbyte id = EmotesDefinition.Instance.GetEmoteByCommand("scared").id;
						aIControllerNPC.SendMessage("OnStartled", new KeyValuePair<sbyte, GameObject>(id, action.activityObject.gameObject), SendMessageOptions.RequireReceiver);
					}
				}
				UnityEngine.Object @object = null;
				TroubleBotActivityObject troubleBotActivityObject = action.activityObject as TroubleBotActivityObject;
				@object = ((!(troubleBotActivityObject != null) || !(troubleBotActivityObject.smashedPrefab != null)) ? manager.EffectPrefabBundle.Load("TroubleBotSmashedPrefab") : troubleBotActivityObject.smashedPrefab.gameObject);
				if (@object != null)
				{
					GameObject g = UnityEngine.Object.Instantiate(@object) as GameObject;
					seq = Utils.GetComponent<EffectSequence>(g);
					if (seq != null)
					{
						seq.SetParent(action.activityObject.gameObject);
						seq.Initialize(action.activityObject.gameObject, delegate
						{
							action.state = BotSmashActionState.Splatted;
						}, delegate
						{
						});
						seq.StartSequence();
						SkinnedMeshRenderer[] components = Utils.GetComponents<SkinnedMeshRenderer>(action.activityObject, Utils.SearchChildren);
						SkinnedMeshRenderer[] array3 = components;
						foreach (SkinnedMeshRenderer skinnedMeshRenderer in array3)
						{
							skinnedMeshRenderer.enabled = false;
						}
					}
					else
					{
						CspUtils.DebugLog("Cant find effect sequence in game object.");
					}
				}
				else
				{
					CspUtils.DebugLog("Cant get prefab for trouble bot effect.");
				}
				counterType.AddCounter(-1L);
				string localCharacter = GameController.GetController().LocalCharacter;
				if (localCharacter != null)
				{
					AppShell.Instance.CounterManager.AddCounter(Name, localCharacter);
				}
				deployedBots--;
				if (LastSpawnTime > DateTime.UtcNow)
				{
					LastSpawnTime = DateTime.UtcNow;
				}
				AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_pickup_troublebot", "troublebot", 1, string.Empty);
				GUIManager.Instance.ShowDynamicWindow(new SHSFractalholioWindow(new Vector2(GUIManager.ScreenRect.width - 100f, 50f), 1, false), GUIControl.ModalLevelEnum.None);
				action.state = BotSmashActionState.WaitingForSplat;
			}
			else
			{
				CspUtils.DebugLog("manager's effect bundle is not loaded.");
				action.state = BotSmashActionState.Splatted;
			}
			break;
		case BotSmashActionState.Splatted:
			action.activityObject.Despawn();
			base.result = ActivityResultEnum.Success;
			result = true;
			break;
		}
		return result;
	}

	private float convertTimeRangeToSeconds(float number, ActivityCycleEnum range)
	{
		float result = 0f;
		switch (range)
		{
		case ActivityCycleEnum.Second:
			result = number;
			break;
		case ActivityCycleEnum.Minute:
			result = number * 60f;
			break;
		case ActivityCycleEnum.Daily:
			result = 86400f * number;
			break;
		case ActivityCycleEnum.Hourly:
			result = 3600f * number;
			break;
		case ActivityCycleEnum.Infinite:
			result = float.MaxValue;
			break;
		}
		return result;
	}
}
