using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHSRareSeasonalActivity : SHSActivityBase
{
	private SeasonalActivity rareObject;

	private string kCounterKey;

	private string kStateCounterKey;

	private string kSetCounterKey;

	private string kLastResetCounterKey;

	private ISHSCounterType stateCounter;

	private ISHSCounterType zoneCounter;

	private ISHSCounterType lastResetCounter;

	private string icon;

	private string previousActivityPrefab;

	private string activityPrefab;

	private string achievementEvent;

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
		kCounterKey = activityConfigurationData.GetString("configuration/counter_key");
		kStateCounterKey = kCounterKey + "." + activityConfigurationData.GetString("configuration/state_counter_subkey");
		kSetCounterKey = kCounterKey + "." + activityConfigurationData.GetString("configuration/set_collection_counter_subkey");
		kLastResetCounterKey = kCounterKey + "." + activityConfigurationData.GetString("configuration/last_reset_counter_subkey");
		icon = activityConfigurationData.GetString("configuration/icon");
		activityPrefab = activityConfigurationData.GetString("configuration/prefab");
		achievementEvent = activityConfigurationData.GetString("configuration/achievement_event");
	}

	public void CheckCounters()
	{
		SHSCountersManager counterManager = AppShell.Instance.CounterManager;
		stateCounter = counterManager.GetCounter(kStateCounterKey);
		zoneCounter = counterManager.GetCounter(kSetCounterKey);
		lastResetCounter = counterManager.GetCounter(kLastResetCounterKey);
		double value = lastResetCounter.GetCurrentValue();
		DateTime dateTime = ServerTime.Instance.UnixEpoch.AddSeconds(value);
		DateTime serverTimeInDateTime = ServerTime.Instance.GetServerTimeInDateTime();
		if (serverTimeInDateTime.Year > dateTime.Year || serverTimeInDateTime.Month > dateTime.Month || serverTimeInDateTime.Day > dateTime.Day)
		{
			lastResetCounter.SetCounter((long)ServerTime.time);
			stateCounter.Reset();
			zoneCounter.Reset();
		}
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

	public void OnCharacterSelected(List<SeasonalActivitySpawnPoint> spawnPoints)
	{
		ClearActive();
		Spawn(spawnPoints);
		UpdateWindow(false);
	}

	protected void ClearActive()
	{
		if (rareObject != null)
		{
			SeasonalActivitySpawnPoint seasonalActivitySpawnPoint = rareObject.spawner as SeasonalActivitySpawnPoint;
			if ((bool)seasonalActivitySpawnPoint)
			{
				if (string.IsNullOrEmpty(previousActivityPrefab))
				{
					previousActivityPrefab = seasonalActivitySpawnPoint.activityPrefab;
				}
				seasonalActivitySpawnPoint.AssignedIndex = -1;
				seasonalActivitySpawnPoint.activityPrefab = activityPrefab;
			}
			rareObject.Despawn();
		}
		rareObject = null;
	}

	protected void Spawn(List<SeasonalActivitySpawnPoint> spawnPoints)
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
		BitArray zoneCollectionState = GetZoneCollectionState(localPlayer.name);
		if (!zoneCollectionState[0])
		{
			int num = (int)zoneCounter.GetCurrentValue(localPlayer.name);
			if (num == 0)
			{
				num = SocialSpaceController.Instance.primaryZones[UnityEngine.Random.Range(0, SocialSpaceController.Instance.primaryZones.Count)];
				zoneCounter.SetCounter(localPlayer.name, num);
			}
			if (num == currentZoneID)
			{
				int seed = UnityEngine.Random.seed;
				UnityEngine.Random.seed = component.definitionData.CharacterName.GetHashCode() + (int)lastResetCounter.GetCurrentValue();
				int index = UnityEngine.Random.Range(0, spawnPoints.Count);
				spawnPoints[index].AssignedIndex = 0;
				spawnPoints[index].activityPrefab = activityPrefab;
				spawnPoints[index].SpawnActivityObject();
				UnityEngine.Random.seed = seed;
			}
		}
	}

	private BitArray GetZoneCollectionState(string heroName)
	{
		BitArray bitArray = new BitArray(BitConverter.GetBytes(stateCounter.GetCurrentValue(heroName)));
		BitArray bitArray2 = new BitArray(1);
		bitArray2[0] = bitArray[0];
		return bitArray2;
	}

	private void UpdateZoneCollectionState()
	{
		if (currentZoneID != -1)
		{
			string name = GameController.GetController().LocalPlayer.name;
			BitArray bitArray = new BitArray(BitConverter.GetBytes(stateCounter.GetCurrentValue(name)));
			bitArray[0] = true;
			byte[] array = new byte[8];
			bitArray.CopyTo(array, 0);
			stateCounter.SetCounter(name, BitConverter.ToInt64(array, 0));
			if (bitArray[0])
			{
			}
		}
	}

	public override void OnActivityAction(ActivityObjectActionNameEnum action, ActivityObject activityObject, object extraData)
	{
		base.OnActivityAction(action, activityObject, extraData);
		UpdateZoneCollectionState();
		AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, achievementEvent, achievementEvent, 1, string.Empty);
		UpdateWindow(true);
		SocialSpaceControllerImpl.bumpIdleTimer();
		activityObject.spawner.activityPrefab = previousActivityPrefab;
	}

	public override void RegisterActivityObject(ActivityObject activityObject)
	{
		rareObject = (activityObject as SeasonalActivity);
	}

	public void UpdateWindow(bool onCollect)
	{
		if (state == ActivityStateEnum.AwaitingAchievementData)
		{
			return;
		}
		int collectionCount = GetCollectionCount();
		SHSSocialMainWindow sHSSocialMainWindow = GUIManager.Instance["SHSMainWindow/SHSSocialMainWindow"] as SHSSocialMainWindow;
		if (sHSSocialMainWindow != null)
		{
			if (!onCollect)
			{
				sHSSocialMainWindow.OnCollectRareSeasonal(collectionCount, true);
			}
			else
			{
				sHSSocialMainWindow.OnCollectRareSeasonal(collectionCount);
			}
		}
	}
}
