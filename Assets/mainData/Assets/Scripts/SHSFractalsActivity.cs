using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHSFractalsActivity : SHSActivityBase
{
	private List<FractalActivitySpawnPoint> spawnPoints;

	private List<Fractal> activeFractals = new List<Fractal>();

	private string lastCharacter = string.Empty;

	private bool dbg_allFractalsMode;

	private SHSGoldenFractalActivity goldenFractalActivity;

	private int kMaxFractalCount;

	private FractalActivitySpawnPoint.FractalType FractalType = FractalActivitySpawnPoint.FractalType.Fractal;

	private string kCounterKey;

	private string kStateCounterKey;

	private string kSetCounterKey;

	private string kLastResetCounterKey;

	private ISHSCounterType stateCounter;

	private ISHSCounterType setCounter;

	private ISHSCounterType lastResetCounter;

	public int MaxFractals
	{
		get
		{
			return kMaxFractalCount;
		}
	}

	public override void Configure(SHSActivityManager manager, DataWarehouse data)
	{
		base.Configure(manager, data);
		kMaxFractalCount = Mathf.Max(0, activityConfigurationData.GetInt("configuration/fractal_count"));
		if (kMaxFractalCount == 0)
		{
			CspUtils.DebugLog("Invalid value specified for the fractal count in the activity configuration: no fractals will be spawned");
		}
		kCounterKey = activityConfigurationData.GetString("configuration/counter_key");
		kStateCounterKey = kCounterKey + "." + activityConfigurationData.GetString("configuration/state_counter_subkey");
		kSetCounterKey = kCounterKey + "." + activityConfigurationData.GetString("configuration/set_collection_counter_subkey");
		kLastResetCounterKey = kCounterKey + "." + activityConfigurationData.GetString("configuration/last_reset_counter_subkey");
	}

	public override void OnZoneLoaded(string zoneName, int zoneID)
	{
		AppShell.Instance.EventMgr.AddListener<LocalPlayerChangedMessage>(OnCharacterSelected);
		spawnPoints = new List<FractalActivitySpawnPoint>();
		FractalActivitySpawnPoint[] array = UnityEngine.Object.FindObjectsOfType(typeof(FractalActivitySpawnPoint)) as FractalActivitySpawnPoint[];
		foreach (FractalActivitySpawnPoint fractalActivitySpawnPoint in array)
		{
			fractalActivitySpawnPoint.activityReference = this;
			fractalActivitySpawnPoint.AssignedFractalIndex = -1;
			spawnPoints.Add(fractalActivitySpawnPoint);
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
		ClearActiveFractals();
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
			CspUtils.DebugLog("Fractals CheckCounters ResetTimer!");
			ResetTimer();
		}
		if (goldenFractalActivity == null)
		{
			goldenFractalActivity = (AppShell.Instance.ActivityManager.GetActivity("goldenfractalactivity") as SHSGoldenFractalActivity);
		}
		goldenFractalActivity.CheckCounters();
	}

	public void ResetFractalStates()
	{
		stateCounter.Reset();
	}

	public void ResetTimer()
	{
		lastResetCounter.SetCounter((long)ServerTime.time);
		ResetFractalStates();
	}

	public int GetFractalCollectionCount()
	{
		BitArray fractalState = GetFractalState();
		if (fractalState == null)
		{
			return -1;
		}
		int num = 0;
		foreach (bool item in fractalState)
		{
			if (item)
			{
				num++;
			}
		}
		return num;
	}

	public BitArray GetFractalState()
	{
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		return (!localPlayer) ? null : GetZoneCollectionState(localPlayer.name);
	}

	public void Debug_SpawnAllFractals()
	{
		dbg_allFractalsMode = true;
		ClearActiveFractals();
		foreach (FractalActivitySpawnPoint spawnPoint in spawnPoints)
		{
			spawnPoint.SpawnActivityObject();
		}
	}

	public void Debug_UndoSpawnAllFractals()
	{
		if (dbg_allFractalsMode)
		{
			dbg_allFractalsMode = false;
			ClearActiveFractals();
			SpawnFractals();
		}
	}

	public int Debug_GetCollectionCount()
	{
		if (dbg_allFractalsMode)
		{
			return spawnPoints.Count - activeFractals.Count;
		}
		return 0;
	}

	private void OnCharacterSelected(LocalPlayerChangedMessage e)
	{
		if (state != ActivityStateEnum.AwaitingAchievementData && e.localPlayer != null && e.localPlayer.name != lastCharacter)
		{
			if (!dbg_allFractalsMode)
			{
				RespawnFractals();
			}
			else
			{
				Debug_SpawnAllFractals();
			}
			UpdateFractalWindow(false);
		}
	}

	public void RespawnFractals()
	{
		ClearActiveFractals();
		SpawnFractals();
	}

	public override void Reset()
	{
		ResetTimer();
		RespawnFractals();
		UpdateFractalWindow(false);
	}

	public override void Reactivate()
	{
		if (lastCharacter != AppShell.Instance.Profile.SelectedCostume)
		{
			RespawnFractals();
		}
		UpdateFractalWindow(false);
	}

	protected void ClearActiveFractals()
	{
		if (activeFractals != null)
		{
			foreach (Fractal activeFractal in activeFractals)
			{
				FractalActivitySpawnPoint fractalActivitySpawnPoint = activeFractal.spawner as FractalActivitySpawnPoint;
				if ((bool)fractalActivitySpawnPoint)
				{
					fractalActivitySpawnPoint.AssignedFractalIndex = -1;
				}
				activeFractal.Despawn();
			}
			activeFractals.Clear();
		}
	}

	protected void SpawnFractals()
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
		List<FractalActivitySpawnPoint> list = new List<FractalActivitySpawnPoint>(spawnPoints);
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (!list[num].CanCharacterReach(component))
			{
				list.RemoveAt(num);
			}
		}
		int seed = UnityEngine.Random.seed;
		UnityEngine.Random.seed = component.definitionData.CharacterName.GetHashCode() + (int)lastResetCounter.GetCurrentValue();
		int num2 = Mathf.Min(kMaxFractalCount, list.Count);
		if (kMaxFractalCount > list.Count)
		{
			CspUtils.DebugLog("There aren't enough fractal spawn points for this character.  Add more to the zone or reduce the maximum fractal count.");
		}
		for (int i = 0; i < num2; i++)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			if (!zoneCollectionState[i])
			{
				list[index].AssignedFractalIndex = i;
				list[index].SpawnActivityObject();
			}
			list.RemoveAt(index);
		}
		UnityEngine.Random.seed = seed;
		goldenFractalActivity.OnCharacterSelected(list);
	}

	private BitArray GetZoneCollectionState(string heroName)
	{
		BitArray bitArray = new BitArray(BitConverter.GetBytes(stateCounter.GetCurrentValue(heroName)));
		BitArray bitArray2 = new BitArray(kMaxFractalCount);
		for (int i = 0; i < kMaxFractalCount; i++)
		{
			bitArray2[i] = bitArray[i];
		}
		return bitArray2;
	}

	private void UpdateZoneCollectionState(string heroName, int FractalIndex, bool collected)
	{
		if (currentZoneID == -1)
		{
			return;
		}
		BitArray bitArray = new BitArray(BitConverter.GetBytes(stateCounter.GetCurrentValue(heroName)));
		bitArray[FractalIndex] = collected;
		byte[] array = new byte[8];
		bitArray.CopyTo(array, 0);
		stateCounter.SetCounter(heroName, BitConverter.ToInt64(array, 0));
		for (int i = 0; i < kMaxFractalCount; i++)
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
		if (activityObject.spawner.activityPrefab == "GoldenFractalPickupPrefab")
		{
			goldenFractalActivity.RegisterActivityObject(activityObject);
		}
		Fractal fractal = activityObject as Fractal;
		if (fractal != null)
		{
			activeFractals.Add(fractal);
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
		Fractal fractal = activityObject as Fractal;
		FractalActivitySpawnPoint fractalActivitySpawnPoint = activityObject.spawner as FractalActivitySpawnPoint;
		if (fractal != null && fractalActivitySpawnPoint != null && fractalActivitySpawnPoint.AssignedFractalIndex != -1)
		{
			if (activityObject.spawner.activityPrefab == "GoldenFractalPickupPrefab")
			{
				goldenFractalActivity.OnActivityAction(action, activityObject, extraData);
			}
			else
			{
				GameObject localPlayer = GameController.GetController().LocalPlayer;
				UpdateZoneCollectionState(localPlayer.name, fractalActivitySpawnPoint.AssignedFractalIndex, true);
				AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_pickup_fractal", "fractal", 1, string.Empty);
				GUIManager.Instance.ShowDynamicWindow(new SHSFractalholioWindow(new Vector2(GUIManager.ScreenRect.width - 100f, 50f), 1, false), GUIControl.ModalLevelEnum.None);
				SocialSpaceControllerImpl.bumpIdleTimer();
				UpdateFractalWindow(true);
			}
			activeFractals.Remove(fractal);
			fractalActivitySpawnPoint.AssignedFractalIndex = -1;
		}
		else if (dbg_allFractalsMode)
		{
			activeFractals.Remove(fractal);
			UpdateFractalWindow(true);
		}
	}

	public void UpdateFractalWindow(bool onCollect)
	{
		if (state == ActivityStateEnum.AwaitingAchievementData)
		{
			return;
		}
		int fractalCount = dbg_allFractalsMode ? Debug_GetCollectionCount() : GetFractalCollectionCount();
		SHSSocialMainWindow sHSSocialMainWindow = GUIManager.Instance["SHSMainWindow/SHSSocialMainWindow"] as SHSSocialMainWindow;
		if (sHSSocialMainWindow != null)
		{
			if (!onCollect)
			{
				sHSSocialMainWindow.OnCollectFractal(FractalType, fractalCount, true);
			}
			else
			{
				sHSSocialMainWindow.OnCollectFractal(FractalType, fractalCount);
			}
		}
	}
}
