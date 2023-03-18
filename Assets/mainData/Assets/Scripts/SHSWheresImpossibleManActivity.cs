using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class SHSWheresImpossibleManActivity : SHSActivityBase
{
	private const float kResetPeriod = 86400f;

	private GameObject[] interactiveObjects;

	private WheresImpossibleMan activeImpossibleMan;

	private string lastCharacter = string.Empty;

	private int kDailyImpossibleManCount;

	private string kCounterKey;

	private string kStateCounterKey;

	private string kSetCounterKey;

	private string kLastResetCounterKey;

	private ISHSCounterType stateCounter;

	private ISHSCounterType setCounter;

	private ISHSCounterType lastResetCounter;

	public int MaxImpossibleMans
	{
		get
		{
			return kDailyImpossibleManCount;
		}
	}

	public override void Configure(SHSActivityManager manager, DataWarehouse data)
	{
		base.Configure(manager, data);
		kDailyImpossibleManCount = Mathf.Max(0, activityConfigurationData.GetInt("configuration/impossible_man_count"));
		if (kDailyImpossibleManCount == 0)
		{
			CspUtils.DebugLog("Invalid value specified for the impossible man count in the activity configuration: no impossible mans will be spawned");
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
		AppShell.Instance.EventMgr.AddListener<InteractiveObjectUsedMessage>(OnInteractiveObjectUsed);
		interactiveObjects = GameObject.FindGameObjectsWithTag("Interactive");
		base.OnZoneLoaded(zoneName, zoneID);
	}

	private void OnNotificationRefreshRequest(NotificationRefreshRequest e)
	{
		UpdateWheresImpossibleManWindow(false);
	}

	public override void OnZoneUnloaded(string zoneName)
	{
		base.OnZoneUnloaded(zoneName);
		ClearActiveImpossibleMan();
		lastCharacter = string.Empty;
		AppShell.Instance.EventMgr.RemoveListener<LocalPlayerChangedMessage>(OnCharacterSelected);
		AppShell.Instance.EventMgr.RemoveListener<NotificationRefreshRequest>(OnNotificationRefreshRequest);
		AppShell.Instance.EventMgr.RemoveListener<InteractiveObjectUsedMessage>(OnInteractiveObjectUsed);
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
			ResetTimer();
		}
	}

	public void ResetWheresImpossibleManStates()
	{
		stateCounter.Reset();
	}

	public void ResetTimer()
	{
		lastResetCounter.SetCounter((long)ServerTime.time);
		ResetWheresImpossibleManStates();
	}

	public int GetWheresImpossibleManCollectionCount()
	{
		BitArray wheresImpossibleManState = GetWheresImpossibleManState();
		if (wheresImpossibleManState == null)
		{
			return -1;
		}
		int num = 0;
		foreach (bool item in wheresImpossibleManState)
		{
			if (item)
			{
				num++;
			}
		}
		return num;
	}

	public BitArray GetWheresImpossibleManState()
	{
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		return (!localPlayer) ? null : GetZoneCollectionState(localPlayer.name);
	}

	private void OnCharacterSelected(LocalPlayerChangedMessage e)
	{
		if (e.localPlayer != null && e.localPlayer.name != lastCharacter)
		{
			RespawnImpossibleMan();
			UpdateWheresImpossibleManWindow(false);
		}
	}

	private void OnInteractiveObjectUsed(InteractiveObjectUsedMessage e)
	{
		if (activeImpossibleMan.isActivated && activeImpossibleMan.gameObject.active && activeImpossibleMan.interactiveObject == e.interactiveObject.gameObject)
		{
			activeImpossibleMan.ActionTriggered(ActivityObjectActionNameEnum.Click);
			GameObject localPlayer = GameController.GetController().LocalPlayer;
			UpdateZoneCollectionState(localPlayer.name, GetWheresImpossibleManCollectionCount(), true);
			AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_pickup_impossibleman", "wheresimpossibleman", 1, string.Empty);
			UpdateWheresImpossibleManWindow(true);
			SocialSpaceControllerImpl.bumpIdleTimer();
			if (GetWheresImpossibleManCollectionCount() == kDailyImpossibleManCount)
			{
				MissionLauncher componentInChildren = activeImpossibleMan.gameObject.GetComponentInChildren<MissionLauncher>();
				componentInChildren.StartWithPlayer(localPlayer, null);
			}
			RespawnImpossibleMan();
		}
	}

	public void RespawnImpossibleMan()
	{
		//EditorApplication.isPaused = true;
		// CSP - temporarily commented out this block
		//ClearActiveImpossibleMan();
		//SpawnImpossibleMan();
	}

	public override void Reset()
	{
		ResetTimer();
		RespawnImpossibleMan();
		UpdateWheresImpossibleManWindow(false);
	}

	public override void Reactivate()
	{
		UpdateWheresImpossibleManWindow(false);
	}

	protected void ClearActiveImpossibleMan()
	{
		if (activeImpossibleMan != null)
		{
			activeImpossibleMan.Deactivate();
			activeImpossibleMan.gameObject.SetActiveRecursively(false);
		}
	}

	protected void SpawnImpossibleMan()
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
		if (activeImpossibleMan == null)
		{
			GameObject gameObject = GameObject.Find("WheresImpossibleManPrefab");
			activeImpossibleMan = gameObject.GetComponent<WheresImpossibleMan>();
			activeImpossibleMan.activityReference = this;
		}
		else
		{
			activeImpossibleMan.gameObject.SetActiveRecursively(true);
		}
		if (GetWheresImpossibleManCollectionCount() == kDailyImpossibleManCount)
		{
			activeImpossibleMan.gameObject.SetActiveRecursively(false);
			return;
		}
		List<GameObject> list = new List<GameObject>(interactiveObjects);
		if (activeImpossibleMan != null)
		{
			GameObject[] array = interactiveObjects;
			foreach (GameObject gameObject2 in array)
			{
				if (activeImpossibleMan.interactiveObject == gameObject2)
				{
					list.Remove(gameObject2);
				}
			}
		}
		int index = UnityEngine.Random.Range(0, list.Count - 1);
		activeImpossibleMan.SetInteractiveObject(list[index]);
		if (state == ActivityStateEnum.AwaitingAchievementData)
		{
			activeImpossibleMan.gameObject.SetActiveRecursively(false);
			blockedGameObjects.Add(activeImpossibleMan.gameObject);
		}
	}

	private BitArray GetZoneCollectionState(string heroName)
	{
		BitArray bitArray = new BitArray(BitConverter.GetBytes(stateCounter.GetCurrentValue(heroName)));
		BitArray bitArray2 = new BitArray(kDailyImpossibleManCount);
		for (int i = 0; i < kDailyImpossibleManCount; i++)
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
		for (int i = 0; i < kDailyImpossibleManCount; i++)
		{
			if (!bitArray[i])
			{
				return;
			}
		}
		setCounter.AddCounter(heroName);
	}

	public void UpdateWheresImpossibleManWindow(bool onCollect)
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
				sHSSocialMainWindow.OnActivateWheresImpossibleMan(GetWheresImpossibleManCollectionCount(), kDailyImpossibleManCount, true);
			}
			else
			{
				sHSSocialMainWindow.OnActivateWheresImpossibleMan(GetWheresImpossibleManCollectionCount(), kDailyImpossibleManCount);
			}
		}
	}
}
