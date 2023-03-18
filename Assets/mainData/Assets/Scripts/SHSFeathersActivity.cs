using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHSFeathersActivity : SHSActivityBase
{
	private List<FeatherActivitySpawnPoint> spawnPoints;

	private List<Feather> activeFeathers = new List<Feather>();

	private string lastCharacter = string.Empty;

	private bool dbg_allFeathersMode;

	private int kMaxFeatherCount;

	private int[] xpEarnedPerFeather;

	private string kCounterKey;

	private string kStateCounterKey;

	private string kSetCounterKey;

	private string kLastResetCounterKey;

	private ISHSCounterType stateCounter;

	private ISHSCounterType setCounter;

	private ISHSCounterType lastResetCounter;

	//public  SHSFeathersActivity() {
	//	CspUtils.DebugLog("SHSFeathersActivity constructor called!");  // CSP
	//}

	public override void Configure(SHSActivityManager manager, DataWarehouse data)
	{
		base.Configure(manager, data);
		kMaxFeatherCount = Mathf.Max(0, activityConfigurationData.GetInt("configuration/feather_count"));
		if (kMaxFeatherCount == 0)
		{
			CspUtils.DebugLog("Invalid value specified for the feather count in the activity configuration: no feathers will be spawned");
		}
		xpEarnedPerFeather = new int[kMaxFeatherCount];
		for (int i = 0; i < kMaxFeatherCount; i++)
		{
			xpEarnedPerFeather[i] = activityConfigurationData.GetInt("configuration/xp_earned_" + (i + 1).ToString());
		}
		kCounterKey = activityConfigurationData.GetString("configuration/counter_key");
		kStateCounterKey = kCounterKey + "." + activityConfigurationData.GetString("configuration/state_counter_subkey");
		kSetCounterKey = kCounterKey + "." + activityConfigurationData.GetString("configuration/set_collection_counter_subkey");
		kLastResetCounterKey = kCounterKey + "." + activityConfigurationData.GetString("configuration/last_reset_counter_subkey");
	}

	public override void OnZoneLoaded(string zoneName, int zoneID)
	{
		AppShell.Instance.EventMgr.AddListener<LocalPlayerChangedMessage>(OnCharacterSelected);
		AppShell.Instance.EventMgr.AddListener<NotificationRefreshRequest>(OnNotificationRefreshRequest);
		spawnPoints = new List<FeatherActivitySpawnPoint>();
		FeatherActivitySpawnPoint[] array = UnityEngine.Object.FindObjectsOfType(typeof(FeatherActivitySpawnPoint)) as FeatherActivitySpawnPoint[];
		foreach (FeatherActivitySpawnPoint featherActivitySpawnPoint in array)
		{
			featherActivitySpawnPoint.activityReference = this;
			featherActivitySpawnPoint.AssignedFeatherIndex = -1;
			spawnPoints.Add(featherActivitySpawnPoint);
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
		ClearActiveFeathers();
		lastCharacter = string.Empty;
		AppShell.Instance.EventMgr.RemoveListener<LocalPlayerChangedMessage>(OnCharacterSelected);
		AppShell.Instance.EventMgr.RemoveListener<NotificationRefreshRequest>(OnNotificationRefreshRequest);
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
			CspUtils.DebugLog("Feathers CheckCounters ResetTimer!");
			ResetTimer();
		}
	}

	public void ResetFeatherStates()
	{
		stateCounter.Reset();
	}

	public void ResetTimer()
	{
		lastResetCounter.SetCounter((long)ServerTime.time);
		ResetFeatherStates();
	}

	public int GetFeatherCollectionCount()
	{
		BitArray featherState = GetFeatherState();
		if (featherState == null)
		{
			return -1;
		}
		int num = 0;
		foreach (bool item in featherState)
		{
			if (item)
			{
				num++;
			}
		}
		return num;
	}

	public BitArray GetFeatherState()
	{
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		return (!localPlayer) ? null : GetZoneCollectionState(localPlayer.name);
	}

	public void Debug_SpawnAllFeathers()
	{
		dbg_allFeathersMode = true;
		ClearActiveFeathers();
		foreach (FeatherActivitySpawnPoint spawnPoint in spawnPoints)
		{
			spawnPoint.SpawnActivityObject();
		}
	}

	public void Debug_UndoSpawnAllFeathers()
	{
		if (dbg_allFeathersMode)
		{
			dbg_allFeathersMode = false;
			ClearActiveFeathers();
			SpawnFeathers();
		}
	}

	public int Debug_GetCollectionCount()
	{
		if (dbg_allFeathersMode)
		{
			return spawnPoints.Count - activeFeathers.Count;
		}
		return 0;
	}

	private void OnCharacterSelected(LocalPlayerChangedMessage e)
	{
		if (state != ActivityStateEnum.AwaitingAchievementData && e.localPlayer != null && e.localPlayer.name != lastCharacter)
		{
			if (!dbg_allFeathersMode)
			{
				RespawnFeathers();
			}
			else
			{
				Debug_SpawnAllFeathers();
			}
			UpdateFeatherWindow(false);
		}
	}

	private void OnNotificationRefreshRequest(NotificationRefreshRequest e)
	{
		UpdateFeatherWindow(false);
	}

	public void RespawnFeathers()
	{
		ClearActiveFeathers();
		SpawnFeathers();
	}

	public override void Reset()
	{
		ResetTimer();
		RespawnFeathers();
		UpdateFeatherWindow(false);
	}

	public override void Reactivate()
	{
		if (lastCharacter != AppShell.Instance.Profile.SelectedCostume)
		{
			RespawnFeathers();
		}
		UpdateFeatherWindow(false);
	}

	protected void ClearActiveFeathers()
	{
		if (activeFeathers != null)
		{
			foreach (Feather activeFeather in activeFeathers)
			{
				FeatherActivitySpawnPoint featherActivitySpawnPoint = activeFeather.spawner as FeatherActivitySpawnPoint;
				if ((bool)featherActivitySpawnPoint)
				{
					featherActivitySpawnPoint.AssignedFeatherIndex = -1;
				}
				activeFeather.Despawn();
			}
			activeFeathers.Clear();
		}
	}

	protected void SpawnFeathers()
	{
		CspUtils.DebugLog("SpawnFeathers() called!");
		
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
		List<FeatherActivitySpawnPoint> list = new List<FeatherActivitySpawnPoint>(spawnPoints);
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (!list[num].CanCharacterReach(component))
			{
				list.RemoveAt(num);
			}
		}
		int seed = UnityEngine.Random.seed;
		UnityEngine.Random.seed = component.definitionData.CharacterName.GetHashCode() + (int)lastResetCounter.GetCurrentValue();
		int num2 = Mathf.Min(kMaxFeatherCount, list.Count);
		if (kMaxFeatherCount > list.Count)
		{
			CspUtils.DebugLog("There aren't enough feather spawn points for this character.  Add more to the zone or reduce the maximum feather count.");
		}
		for (int i = 0; i < num2; i++)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			if (!zoneCollectionState[i])
			{
				list[index].SpawnActivityObject();
				list[index].AssignedFeatherIndex = i;
			}
			list.RemoveAt(index);
		}
		UnityEngine.Random.seed = seed;
	}

	private BitArray GetZoneCollectionState(string heroName)
	{
		BitArray bitArray = new BitArray(BitConverter.GetBytes(stateCounter.GetCurrentValue(heroName)));
		BitArray bitArray2 = new BitArray(kMaxFeatherCount);
		for (int i = 0; i < kMaxFeatherCount; i++)
		{
			bitArray2[i] = bitArray[i];
		}
		return bitArray2;
	}

	private void UpdateZoneCollectionState(string heroName, int featherIndex, bool collected)
	{
		if (currentZoneID == -1)
		{
			return;
		}
		BitArray bitArray = new BitArray(BitConverter.GetBytes(stateCounter.GetCurrentValue(heroName)));
		bitArray[featherIndex] = collected;
		byte[] array = new byte[8];
		bitArray.CopyTo(array, 0);
		stateCounter.SetCounter(heroName, BitConverter.ToInt64(array, 0));
		for (int i = 0; i < kMaxFeatherCount; i++)
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
		base.RegisterActivityObject(activityObject);
		Feather feather = activityObject as Feather;
		if (feather != null)
		{
			activeFeathers.Add(feather);
			feather.SetHeroIconByPath("characters_bundle|token_" + GameController.GetController().LocalPlayer.name);
		}
	}

	public override void OnActivityAction(ActivityObjectActionNameEnum action, ActivityObject activityObject, object extraData)
	{
		base.OnActivityAction(action, activityObject, extraData);
		if (action == ActivityObjectActionNameEnum.Collision)
		{
			Feather feather = activityObject as Feather;
			FeatherActivitySpawnPoint featherActivitySpawnPoint = activityObject.spawner as FeatherActivitySpawnPoint;
			if (feather != null && featherActivitySpawnPoint != null && featherActivitySpawnPoint.AssignedFeatherIndex != -1)
			{
				GameObject localPlayer = GameController.GetController().LocalPlayer;
				UpdateZoneCollectionState(localPlayer.name, featherActivitySpawnPoint.AssignedFeatherIndex, true);
				activeFeathers.Remove(feather);
				featherActivitySpawnPoint.AssignedFeatherIndex = -1;
				int featherCollectionCount = GetFeatherCollectionCount();
				SocialSpaceControllerImpl.bumpIdleTimer();
				AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_pickup_token", "token", 1, string.Empty);
				UpdateFeatherWindow(true);
			}
			else if (dbg_allFeathersMode)
			{
				activeFeathers.Remove(feather);
				UpdateFeatherWindow(true);
			}
		}
	}

	public void UpdateFeatherWindow(bool onCollect)
	{
		if (state == ActivityStateEnum.AwaitingAchievementData)
		{
			return;
		}
		int featherCount;
		int count;
		if (!dbg_allFeathersMode)
		{
			featherCount = GetFeatherCollectionCount();
			count = kMaxFeatherCount;
		}
		else
		{
			featherCount = Debug_GetCollectionCount();
			count = spawnPoints.Count;
		}
		SHSSocialMainWindow sHSSocialMainWindow = GUIManager.Instance["SHSMainWindow/SHSSocialMainWindow"] as SHSSocialMainWindow;
		if (sHSSocialMainWindow != null)
		{
			if (!onCollect)
			{
				sHSSocialMainWindow.OnCollectFeather(featherCount, count, true);
			}
			else
			{
				sHSSocialMainWindow.OnCollectFeather(featherCount, count);
			}
		}
	}
}
