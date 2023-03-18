using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHSGoldenFractalActivity : SHSActivityBase
{
	private Fractal goldenFractal;

	private string kCounterKey;

	private string kStateCounterKey;

	private string kSetCounterKey;

	private string kLastResetCounterKey;

	private ISHSCounterType stateCounter;

	private ISHSCounterType zoneCounter;

	private ISHSCounterType lastResetCounter;

	public override void Configure(SHSActivityManager manager, DataWarehouse data)
	{
		base.Configure(manager, data);
		kCounterKey = activityConfigurationData.GetString("configuration/counter_key");
		kStateCounterKey = kCounterKey + "." + activityConfigurationData.GetString("configuration/state_counter_subkey");
		kSetCounterKey = kCounterKey + "." + activityConfigurationData.GetString("configuration/set_collection_counter_subkey");
		kLastResetCounterKey = kCounterKey + "." + activityConfigurationData.GetString("configuration/last_reset_counter_subkey");
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

	public void OnCharacterSelected(List<FractalActivitySpawnPoint> spawnPoints)
	{
		ClearActiveFractals();
		SpawnFractals(spawnPoints);
		UpdateFractalWindow(false);
	}

	protected void ClearActiveFractals()
	{
		if (goldenFractal != null)
		{
			FractalActivitySpawnPoint fractalActivitySpawnPoint = goldenFractal.spawner as FractalActivitySpawnPoint;
			if ((bool)fractalActivitySpawnPoint)
			{
				fractalActivitySpawnPoint.AssignedFractalIndex = -1;
				fractalActivitySpawnPoint.activityPrefab = "FractalPickupPrefab";
			}
			goldenFractal.Despawn();
		}
		goldenFractal = null;
	}

	protected void SpawnFractals(List<FractalActivitySpawnPoint> spawnPoints)
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
				int num2 = UnityEngine.Random.Range(0, spawnPoints.Count);
				CspUtils.DebugLog("  using index " + num2 + " with location of " + spawnPoints[num2].transform.position + " " + spawnPoints[num2].transform.localPosition);
				spawnPoints[num2].AssignedFractalIndex = 0;
				spawnPoints[num2].activityPrefab = "GoldenFractalPickupPrefab";
				spawnPoints[num2].SpawnActivityObject();
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
		AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_pickup_goldenfractal", "golden_fractal", 1, string.Empty);
		UpdateFractalWindow(true);
		SocialSpaceControllerImpl.bumpIdleTimer();
		activityObject.spawner.activityPrefab = "FractalPickupPrefab";
	}

	public override void RegisterActivityObject(ActivityObject activityObject)
	{
		goldenFractal = (activityObject as Fractal);
	}

	public void UpdateFractalWindow(bool onCollect)
	{
		if (state == ActivityStateEnum.AwaitingAchievementData)
		{
			return;
		}
		int fractalCollectionCount = GetFractalCollectionCount();
		SHSSocialMainWindow sHSSocialMainWindow = GUIManager.Instance["SHSMainWindow/SHSSocialMainWindow"] as SHSSocialMainWindow;
		if (sHSSocialMainWindow != null)
		{
			if (!onCollect)
			{
				sHSSocialMainWindow.OnCollectGoldenFractal(fractalCollectionCount, true);
			}
			else
			{
				sHSSocialMainWindow.OnCollectGoldenFractal(fractalCollectionCount);
			}
		}
	}
}
