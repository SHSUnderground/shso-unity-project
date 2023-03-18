using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHSSeasonalActivity : SHSActivityBase
{
	private List<SeasonalActivitySpawnPoint> spawnPoints;

	private List<SeasonalActivity> activeObjects = new List<SeasonalActivity>();

	private string lastCharacter = string.Empty;

	private bool dbg_allMode;

	private SHSRareSeasonalActivity rareSeasonalActivity;

	private int kMaxCount;

	private string icon;

	private string activityPrefab;

	private string achievementEvent;

	private string kCounterKey;

	private string kStateCounterKey;

	private string kSetCounterKey;

	private string kLastResetCounterKey;

	private ISHSCounterType stateCounter;

	private ISHSCounterType setCounter;

	private ISHSCounterType lastResetCounter;

	public int Max
	{
		get
		{
			return kMaxCount;
		}
	}

	public string Icon
	{
		get
		{
			return icon;
		}
	}

	public override void Configure(SHSActivityManager manager, DataWarehouse data)
	{
		base.Configure(manager, data);
		kMaxCount = Mathf.Max(0, activityConfigurationData.GetInt("configuration/max_count"));
		if (kMaxCount == 0)
		{
			CspUtils.DebugLog("Invalid value specified for the seasonal activity count in the activity configuration: no seasonal activity objects will be spawned");
		}
		kCounterKey = activityConfigurationData.GetString("configuration/counter_key");
		kStateCounterKey = kCounterKey + "." + activityConfigurationData.GetString("configuration/state_counter_subkey");
		kSetCounterKey = kCounterKey + "." + activityConfigurationData.GetString("configuration/set_collection_counter_subkey");
		kLastResetCounterKey = kCounterKey + "." + activityConfigurationData.GetString("configuration/last_reset_counter_subkey");
		icon = activityConfigurationData.GetString("configuration/icon");
		activityPrefab = activityConfigurationData.GetString("configuration/prefab");
		achievementEvent = activityConfigurationData.GetString("configuration/achievement_event");
	}

	public override void OnZoneLoaded(string zoneName, int zoneID)
	{
		AppShell.Instance.EventMgr.AddListener<LocalPlayerChangedMessage>(OnCharacterSelected);
		spawnPoints = new List<SeasonalActivitySpawnPoint>();
		SeasonalActivitySpawnPoint[] array = UnityEngine.Object.FindObjectsOfType(typeof(SeasonalActivitySpawnPoint)) as SeasonalActivitySpawnPoint[];
		foreach (SeasonalActivitySpawnPoint seasonalActivitySpawnPoint in array)
		{
			seasonalActivitySpawnPoint.activityReference = this;
			seasonalActivitySpawnPoint.activityPrefab = activityPrefab;
			seasonalActivitySpawnPoint.AssignedIndex = -1;
			spawnPoints.Add(seasonalActivitySpawnPoint);
		}
		base.OnZoneLoaded(zoneName, zoneID);
	}

	public override void OnZoneUnloaded(string zoneName)
	{
		base.OnZoneUnloaded(zoneName);
		if (spawnPoints != null)
		{
			spawnPoints.Clear();
		}
		ClearActive();
		lastCharacter = string.Empty;
		AppShell.Instance.EventMgr.RemoveListener<LocalPlayerChangedMessage>(OnCharacterSelected);
	}

	public override void Start()
	{
		base.Start();
		SHSCountersManager counterManager = AppShell.Instance.CounterManager;
		stateCounter = counterManager.GetCounter(kStateCounterKey);
		setCounter = counterManager.GetCounter(kSetCounterKey);
		lastResetCounter = counterManager.GetCounter(kLastResetCounterKey);
		CheckCounters();
	}

	protected void CheckCounters()
	{
		double value = lastResetCounter.GetCurrentValue();
		DateTime dateTime = ServerTime.Instance.UnixEpoch.AddSeconds(value);
		DateTime serverTimeInDateTime = ServerTime.Instance.GetServerTimeInDateTime();
		if (serverTimeInDateTime.Year > dateTime.Year || serverTimeInDateTime.Month > dateTime.Month || serverTimeInDateTime.Day > dateTime.Day)
		{
			CspUtils.DebugLog("SeasonalActivty CheckCounters ResetTimer!");
			ResetTimer();
		}
		if (rareSeasonalActivity == null)
		{
			rareSeasonalActivity = (AppShell.Instance.ActivityManager.GetActivity("rareseasonalactivity") as SHSRareSeasonalActivity);
		}
		rareSeasonalActivity.CheckCounters();
	}

	public void ResetStates()
	{
		stateCounter.Reset();
	}

	public void ResetTimer()
	{
		lastResetCounter.SetCounter((long)ServerTime.time);
		ResetStates();
	}

	public int GetCollectionCount()
	{
		BitArray state = GetState();
		if (state == null)
		{
			return -1;
		}
		int num = 0;
		foreach (bool item in state)
		{
			if (item)
			{
				num++;
			}
		}
		return num;
	}

	public BitArray GetState()
	{
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		return (!localPlayer) ? null : GetZoneCollectionState(localPlayer.name);
	}

	public void Debug_SpawnAll()
	{
		dbg_allMode = true;
		ClearActive();
		foreach (SeasonalActivitySpawnPoint spawnPoint in spawnPoints)
		{
			spawnPoint.SpawnActivityObject();
		}
	}

	public void Debug_UndoSpawnAll()
	{
		if (dbg_allMode)
		{
			dbg_allMode = false;
			ClearActive();
			Spawn();
		}
	}

	public int Debug_GetCollectionCount()
	{
		if (dbg_allMode)
		{
			return spawnPoints.Count - activeObjects.Count;
		}
		return 0;
	}

	private void OnCharacterSelected(LocalPlayerChangedMessage e)
	{
		if (state != ActivityStateEnum.AwaitingAchievementData && e.localPlayer != null && e.localPlayer.name != lastCharacter)
		{
			if (!dbg_allMode)
			{
				Respawn();
			}
			else
			{
				Debug_SpawnAll();
			}
			UpdateWindow(false);
		}
	}

	public void Respawn()
	{
		ClearActive();
		Spawn();
	}

	public override void Reset()
	{
		ResetTimer();
		Respawn();
		UpdateWindow(false);
	}

	public override void Reactivate()
	{
		if (lastCharacter != AppShell.Instance.Profile.SelectedCostume)
		{
			Respawn();
		}
		UpdateWindow(false);
	}

	protected void ClearActive()
	{
		if (activeObjects != null)
		{
			foreach (SeasonalActivity activeObject in activeObjects)
			{
				SeasonalActivitySpawnPoint seasonalActivitySpawnPoint = activeObject.spawner as SeasonalActivitySpawnPoint;
				if ((bool)seasonalActivitySpawnPoint)
				{
					seasonalActivitySpawnPoint.AssignedIndex = -1;
				}
				activeObject.Despawn();
			}
			activeObjects.Clear();
		}
	}

	protected void Spawn()
	{
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		if (localPlayer == null)
		{
			return;
		}
		CharacterGlobals component = Utils.GetComponent<CharacterGlobals>(localPlayer);
		if (component == null || component.motionController == null)
		{
			return;
		}
		lastCharacter = localPlayer.name;
		BitArray zoneCollectionState = GetZoneCollectionState(localPlayer.name);
		List<SeasonalActivitySpawnPoint> list = new List<SeasonalActivitySpawnPoint>(spawnPoints);
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (!list[num].CanCharacterReach(component))
			{
				list.RemoveAt(num);
			}
		}
		int seed = UnityEngine.Random.seed;
		UnityEngine.Random.seed = component.definitionData.CharacterName.GetHashCode() + (int)lastResetCounter.GetCurrentValue();
		int num2 = Mathf.Min(kMaxCount, list.Count);
		if (kMaxCount > list.Count)
		{
			CspUtils.DebugLog("There aren't enough seasonal spawn points for this character.  Add more to the zone or reduce the maximum seasonal count.");
		}
		for (int i = 0; i < num2; i++)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			if (!zoneCollectionState[i])
			{
				list[index].AssignedIndex = i;
				list[index].SpawnActivityObject();
			}
			list.RemoveAt(index);
		}
		UnityEngine.Random.seed = seed;
		rareSeasonalActivity.OnCharacterSelected(list);
	}

	private BitArray GetZoneCollectionState(string heroName)
	{
		BitArray bitArray = new BitArray(BitConverter.GetBytes(stateCounter.GetCurrentValue(heroName)));
		BitArray bitArray2 = new BitArray(kMaxCount);
		for (int i = 0; i < kMaxCount; i++)
		{
			bitArray2[i] = bitArray[i];
		}
		return bitArray2;
	}

	private void UpdateZoneCollectionState(string heroName, int index, bool collected)
	{
		if (currentZoneID == -1)
		{
			return;
		}
		BitArray bitArray = new BitArray(BitConverter.GetBytes(stateCounter.GetCurrentValue(heroName)));
		bitArray[index] = collected;
		byte[] array = new byte[8];
		bitArray.CopyTo(array, 0);
		stateCounter.SetCounter(heroName, BitConverter.ToInt64(array, 0));
		for (int i = 0; i < kMaxCount; i++)
		{
			if (!bitArray[i])
			{
				return;
			}
		}
		setCounter.AddCounter(heroName);
	}

	public override void RegisterActivityObject(ActivityObject activityObject)
	{
		if (activityObject.spawner.activityPrefab != activityPrefab)
		{
			rareSeasonalActivity.RegisterActivityObject(activityObject);
		}
		SeasonalActivity seasonalActivity = activityObject as SeasonalActivity;
		if (seasonalActivity != null)
		{
			activeObjects.Add(seasonalActivity);
		}
		base.RegisterActivityObject(activityObject);
	}

	public override void OnActivityAction(ActivityObjectActionNameEnum action, ActivityObject activityObject, object extraData)
	{
		base.OnActivityAction(action, activityObject, extraData);
		if (action != ActivityObjectActionNameEnum.Collision)
		{
			return;
		}
		SeasonalActivity seasonalActivity = activityObject as SeasonalActivity;
		SeasonalActivitySpawnPoint seasonalActivitySpawnPoint = activityObject.spawner as SeasonalActivitySpawnPoint;
		if (seasonalActivity != null && seasonalActivitySpawnPoint != null && seasonalActivitySpawnPoint.AssignedIndex != -1)
		{
			if (activityObject.spawner.activityPrefab != activityPrefab)
			{
				rareSeasonalActivity.OnActivityAction(action, activityObject, extraData);
			}
			else
			{
				GameObject localPlayer = GameController.GetController().LocalPlayer;
				UpdateZoneCollectionState(localPlayer.name, seasonalActivitySpawnPoint.AssignedIndex, true);
				AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, achievementEvent, achievementEvent, 1, string.Empty);
				SocialSpaceControllerImpl.bumpIdleTimer();
				UpdateWindow(true);
			}
			activeObjects.Remove(seasonalActivity);
			seasonalActivitySpawnPoint.AssignedIndex = -1;
		}
		else if (dbg_allMode)
		{
			activeObjects.Remove(seasonalActivity);
			UpdateWindow(true);
		}
	}

	public void UpdateWindow(bool onCollect)
	{
		if (state == ActivityStateEnum.AwaitingAchievementData)
		{
			return;
		}
		int fractalCount = dbg_allMode ? Debug_GetCollectionCount() : GetCollectionCount();
		SHSSocialMainWindow sHSSocialMainWindow = GUIManager.Instance["SHSMainWindow/SHSSocialMainWindow"] as SHSSocialMainWindow;
		if (sHSSocialMainWindow != null)
		{
			if (!onCollect)
			{
				sHSSocialMainWindow.OnCollectSeasonalActivityObject(fractalCount, true);
			}
			else
			{
				sHSSocialMainWindow.OnCollectSeasonalActivityObject(fractalCount);
			}
		}
	}
}
