using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSScavengerActivity : SHSActivityBase
{
	private List<ScavengerActivitySpawnPoint> spawnPoints;

	private List<ScavengerObject> activeObjects = new List<ScavengerObject>();

	private string lastCharacter = string.Empty;

	private bool dbg_allObjectsMode;

	private string kCounterKey;

	private string kStateCounterKey;

	private string kLastResetCounterKey;

	private ISHSCounterType stateCounter;

	private ISHSCounterType lastResetCounter;

	public override void Configure(SHSActivityManager manager, DataWarehouse data)
	{
		base.Configure(manager, data);
		kCounterKey = activityConfigurationData.GetString("configuration/counter_key");
		kStateCounterKey = kCounterKey + "." + activityConfigurationData.GetString("configuration/state_counter_subkey");
		kLastResetCounterKey = kCounterKey + "." + activityConfigurationData.GetString("configuration/last_reset_counter_subkey");
	}

	public override void OnZoneLoaded(string zoneName, int zoneID)
	{
		AppShell.Instance.EventMgr.AddListener<CharacterResponseReceivedMessage>(OnCharacterSelected);
		spawnPoints = new List<ScavengerActivitySpawnPoint>();
		ScavengerActivitySpawnPoint[] array = UnityEngine.Object.FindObjectsOfType(typeof(ScavengerActivitySpawnPoint)) as ScavengerActivitySpawnPoint[];
		foreach (ScavengerActivitySpawnPoint scavengerActivitySpawnPoint in array)
		{
			scavengerActivitySpawnPoint.activityReference = this;
			scavengerActivitySpawnPoint.assignedIndex = -1;
			spawnPoints.Add(scavengerActivitySpawnPoint);
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
		ClearActiveObjects();
		lastCharacter = string.Empty;
		AppShell.Instance.EventMgr.RemoveListener<CharacterResponseReceivedMessage>(OnCharacterSelected);
	}

	public override void Start()
	{
		base.Start();
		SHSCountersManager counterManager = AppShell.Instance.CounterManager;
		stateCounter = counterManager.GetCounter(kStateCounterKey);
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
			CspUtils.DebugLog("Scavenger CheckCounters ResetTimer!");
			ResetTimer();
		}
	}

	public void ResetScavengerStates()
	{
		stateCounter.Reset();
	}

	public void ResetTimer()
	{
		lastResetCounter.SetCounter((long)ServerTime.time);
		ResetScavengerStates();
	}

	public void Debug_SpawnAllObjects()
	{
		dbg_allObjectsMode = true;
		ClearActiveObjects();
		foreach (ScavengerActivitySpawnPoint spawnPoint in spawnPoints)
		{
			spawnPoint.SpawnActivityObject();
		}
	}

	public void Debug_UndoSpawnAllObjects()
	{
		if (dbg_allObjectsMode)
		{
			dbg_allObjectsMode = false;
			ClearActiveObjects();
			SpawnObjects();
		}
	}

	private void OnCharacterSelected(CharacterResponseReceivedMessage e)
	{
		if (state != ActivityStateEnum.AwaitingAchievementData && AppShell.Instance.Profile.SelectedCostume != lastCharacter)
		{
			if (!dbg_allObjectsMode)
			{
				RespawnObjects();
			}
			else
			{
				Debug_SpawnAllObjects();
			}
			UpdateScavengerWindow(false, 0);
		}
	}

	public void RespawnObjects()
	{
		ClearActiveObjects();
		SpawnObjects();
	}

	public override void Reset()
	{
		WWWForm wWWForm = new WWWForm();
		AppShell.Instance.WebService.StartRequest("resources$users/" + AppShell.Instance.Profile.UserId + "/scavenger_reset/", delegate(ShsWebResponse response)
		{
			if (response.Status == 200)
			{
				ResetTimer();
				RespawnObjects();
				UpdateScavengerWindow(false, 0);
			}
		}, wWWForm.data);
	}

	public override void Reactivate()
	{
		if (lastCharacter != AppShell.Instance.Profile.SelectedCostume)
		{
			RespawnObjects();
		}
		UpdateScavengerWindow(false, 0);
	}

	protected void ClearActiveObjects()
	{
		if (activeObjects != null)
		{
			foreach (ScavengerObject activeObject in activeObjects)
			{
				ScavengerActivitySpawnPoint scavengerActivitySpawnPoint = activeObject.spawner as ScavengerActivitySpawnPoint;
				if ((bool)scavengerActivitySpawnPoint)
				{
					scavengerActivitySpawnPoint.assignedIndex = -1;
				}
				activeObject.Despawn();
			}
			activeObjects.Clear();
		}
	}

	protected void SpawnObjects()
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
		List<ScavengerActivitySpawnPoint> list = new List<ScavengerActivitySpawnPoint>(spawnPoints);
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (!list[num].CanCharacterReach(component))
			{
				list.RemoveAt(num);
			}
		}
		HeroPersisted value = null;
		if (!AppShell.Instance.Profile.AvailableCostumes.TryGetValue(AppShell.Instance.Profile.SelectedCostume, out value))
		{
			CspUtils.DebugLog("ERROR:  could not locate hero data in local profile for " + AppShell.Instance.Profile.SelectedCostume);
			return;
		}
		if (!value.scavengerInfo.Contains(","))
		{
			CspUtils.DebugLog("ERROR:  current heroData does not have scavenger info in it: " + value.scavengerInfo);
			return;
		}
		long num2 = (long)(ServerTime.time - (double)(int)(DateTime.Now - DateTime.Today).TotalSeconds);
		string[] array = value.scavengerInfo.Split(',');
		int num3 = array.Length;
		int seed = UnityEngine.Random.seed;
		UnityEngine.Random.seed = component.definitionData.CharacterName.GetHashCode() + (int)num2;
		int num4 = Mathf.Min(num3, list.Count);
		if (num3 > list.Count)
		{
			CspUtils.DebugLog("There aren't enough Scavenger Hunt spawn points for this character.  Add more to the zone or reduce the maximum count.");
		}
		for (int i = 0; i < num4; i++)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			if (array[i] != "0")
			{
				list[index].ownableTypeID = Convert.ToInt32(array[i]);
				list[index].SpawnActivityObject();
				list[index].assignedIndex = i;
			}
			list.RemoveAt(index);
		}
		UnityEngine.Random.seed = seed;
	}

	public override void RegisterActivityObject(ActivityObject activityObject)
	{
		base.RegisterActivityObject(activityObject);
		ScavengerObject scavengerObject = activityObject as ScavengerObject;
		if (scavengerObject != null)
		{
			activeObjects.Add(scavengerObject);
			OwnableDefinition ownableForScavengerZone = OwnableDefinition.getOwnableForScavengerZone(SocialSpaceController.Instance.ShortZoneName, (activityObject.spawner as ScavengerActivitySpawnPoint).ownableTypeID);
			if (ownableForScavengerZone != null)
			{
				scavengerObject.SetHeroIconByPath(ownableForScavengerZone.iconBase);
			}
		}
	}

	public override void OnActivityAction(ActivityObjectActionNameEnum action, ActivityObject activityObject, object extraData)
	{
		base.OnActivityAction(action, activityObject, extraData);
		if (action != ActivityObjectActionNameEnum.Collision)
		{
			return;
		}
		ScavengerObject scavengerObject = activityObject as ScavengerObject;
		ScavengerActivitySpawnPoint scavengerActivitySpawnPoint = activityObject.spawner as ScavengerActivitySpawnPoint;
		if (scavengerObject != null && scavengerActivitySpawnPoint != null && scavengerActivitySpawnPoint.assignedIndex != -1)
		{
			UpdateScavengerWindow(true, scavengerActivitySpawnPoint.assignedIndex);
			activeObjects.Remove(scavengerObject);
			scavengerActivitySpawnPoint.assignedIndex = -1;
			HeroPersisted value = null;
			if (AppShell.Instance.Profile.AvailableCostumes.TryGetValue(AppShell.Instance.Profile.SelectedCostume, out value))
			{
			}
			AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_pickup_craft", "scavenger", 1, string.Empty);
			SocialSpaceControllerImpl.bumpIdleTimer();
		}
		else if (dbg_allObjectsMode)
		{
			activeObjects.Remove(scavengerObject);
			UpdateScavengerWindow(true, 0);
		}
	}

	public void UpdateScavengerWindow(bool onCollect, int objectIndex)
	{
		if (state == ActivityStateEnum.AwaitingAchievementData)
		{
			return;
		}
		SHSSocialMainWindow sHSSocialMainWindow = GUIManager.Instance["SHSMainWindow/SHSSocialMainWindow"] as SHSSocialMainWindow;
		if (sHSSocialMainWindow != null)
		{
			if (!onCollect)
			{
				sHSSocialMainWindow.OnCollectScavengerObject(objectIndex, true);
			}
			else
			{
				sHSSocialMainWindow.OnCollectScavengerObject(objectIndex);
			}
		}
	}
}
