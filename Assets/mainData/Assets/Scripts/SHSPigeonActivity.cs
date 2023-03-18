using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHSPigeonActivity : SHSActivityBase
{
	private class PigeonDistance
	{
		public float sqrDistance;

		public AIControllerPigeon pigeon;

		public PigeonDistance(AIControllerPigeon pigeon, Vector3 relativeLocation)
		{
			this.pigeon = pigeon;
			sqrDistance = (relativeLocation - pigeon.transform.position).sqrMagnitude;
		}
	}

	private List<PigeonActivitySpawnPoint> spawnPoints;

	private List<AIControllerPigeon> pigeonLocations;

	private List<AIControllerPigeon> inactivePigeons;

	private List<AIControllerPigeon> activePigeons;

	private float nextPigeonActivateTime = -1f;

	private int spawnedCharacters;

	private int maxPigeons;

	private DRange kInitialPigeonCount;

	private FRange kPigeonCooldown;

	private List<LimitingThreshold> limitingThresholds;

	public override void Configure(SHSActivityManager manager, DataWarehouse data)
	{
		base.Configure(manager, data);
		int @int = activityConfigurationData.GetInt("configuration/min_start_pigeons");
		int int2 = activityConfigurationData.GetInt("configuration/max_start_pigeons");
		kInitialPigeonCount = new DRange(@int, int2);
		float @float = activityConfigurationData.GetFloat("configuration/min_pigeon_respawn_seconds");
		float float2 = activityConfigurationData.GetFloat("configuration/max_pigeon_respawn_seconds");
		kPigeonCooldown = new FRange(@float, float2);
		limitingThresholds = LimitingThreshold.LoadAndSort(activityConfigurationData, "configuration/limiting_thresholds");
	}

	public override void OnZoneLoaded(string zoneName, int zoneID)
	{
		spawnPoints = new List<PigeonActivitySpawnPoint>(UnityEngine.Object.FindObjectsOfType(typeof(PigeonActivitySpawnPoint)) as PigeonActivitySpawnPoint[]);
		spawnedCharacters = AppShell.Instance.ServerConnection.GetGameUserCount();
		CheckThresholds();
		AppShell.Instance.EventMgr.AddListener<RoomUserEnterMessage>(OnUserEnterRoom);
		AppShell.Instance.EventMgr.AddListener<RoomUserLeaveMessage>(OnUserLeaveRoom);
		AppShell.Instance.EventMgr.AddListener<GraphicsOptionsChange>(OnGraphicsOptionsChange);
		pigeonLocations = new List<AIControllerPigeon>();
		foreach (PigeonActivitySpawnPoint spawnPoint in spawnPoints)
		{
			spawnPoint.activityReference = this;
			spawnPoint.SpawnActivityObject();
		}
		base.OnZoneLoaded(zoneName, zoneID);
	}

	public override void OnZoneUnloaded(string zoneName)
	{
		base.OnZoneUnloaded(zoneName);
		AppShell.Instance.EventMgr.RemoveListener<RoomUserLeaveMessage>(OnUserLeaveRoom);
		AppShell.Instance.EventMgr.RemoveListener<RoomUserEnterMessage>(OnUserEnterRoom);
		AppShell.Instance.EventMgr.RemoveListener<GraphicsOptionsChange>(OnGraphicsOptionsChange);
		if (spawnPoints != null)
		{
			spawnPoints.Clear();
		}
		if (pigeonLocations != null)
		{
			pigeonLocations.Clear();
		}
		if (inactivePigeons != null)
		{
			inactivePigeons.Clear();
		}
		if (activePigeons != null)
		{
			activePigeons.Clear();
		}
		nextPigeonActivateTime = -1f;
	}

	public override void RegisterActivityObject(ActivityObject activityObject)
	{
		base.RegisterActivityObject(activityObject);
		pigeonLocations.Add(Utils.GetComponent<AIControllerPigeon>(activityObject));
		if (pigeonLocations.Count == spawnPoints.Count)
		{
			InitializePigeons();
		}
	}

	protected void InitializePigeons()
	{
		inactivePigeons = new List<AIControllerPigeon>(pigeonLocations);
		activePigeons = new List<AIControllerPigeon>();
		int num = Mathf.Min(Mathf.Min(inactivePigeons.Count, kInitialPigeonCount.RandomValue), maxPigeons);
		for (int i = 0; i < num; i++)
		{
			int index = UnityEngine.Random.Range(0, inactivePigeons.Count);
			AIControllerPigeon aIControllerPigeon = inactivePigeons[index];
			aIControllerPigeon.StartCoroutine(ApplyRandomAnimOffset(aIControllerPigeon));
			activePigeons.Add(aIControllerPigeon);
			inactivePigeons.RemoveAt(index);
		}
		foreach (AIControllerPigeon inactivePigeon in inactivePigeons)
		{
			Utils.ActivateTree(inactivePigeon.gameObject, false);
		}
		nextPigeonActivateTime = Time.time + kPigeonCooldown.RandomValue;
		foreach (AIControllerPigeon pigeonLocation in pigeonLocations)
		{
			pigeonLocation.OnTakeoffFinished = (AIControllerPigeon.TakeoffFinishedDelegate)Delegate.Combine(pigeonLocation.OnTakeoffFinished, new AIControllerPigeon.TakeoffFinishedDelegate(OnPigeonFinishedTakeoff));
			pigeonLocation.OnTakeoffStarted = (AIControllerPigeon.TakeoffStartedDelegate)Delegate.Combine(pigeonLocation.OnTakeoffStarted, new AIControllerPigeon.TakeoffStartedDelegate(OnPigeonStartedTakeoff));
		}
	}

	public override void Update()
	{
		base.Update();
		if (nextPigeonActivateTime > 0f && Time.time > nextPigeonActivateTime && activePigeons.Count < maxPigeons)
		{
			ActivateRandomPigeon();
			nextPigeonActivateTime = Time.time + kPigeonCooldown.RandomValue;
		}
	}

	private IEnumerator ApplyRandomAnimOffset(AIControllerPigeon pigeon)
	{
		yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 1f));
		pigeon.RewindAnimation();
	}

	private void ActivateRandomPigeon()
	{
		List<AIControllerPigeon> list = new List<AIControllerPigeon>(inactivePigeons);
		AIControllerPigeon aIControllerPigeon;
		while (true)
		{
			if (list.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, list.Count);
				aIControllerPigeon = list[index];
				Utils.ActivateTree(aIControllerPigeon.gameObject, true);
				if (aIControllerPigeon.IsSafeToLand())
				{
					break;
				}
				Utils.ActivateTree(aIControllerPigeon.gameObject, false);
				list.RemoveAt(index);
				continue;
			}
			return;
		}
		activePigeons.Add(aIControllerPigeon);
		inactivePigeons.Remove(aIControllerPigeon);
		aIControllerPigeon.StartCoroutine(aIControllerPigeon.DoLanding());
	}

	private void OnPigeonStartedTakeoff(AIControllerPigeon pigeon)
	{
		activePigeons.Remove(pigeon);
	}

	private void OnPigeonFinishedTakeoff(AIControllerPigeon pigeon)
	{
		inactivePigeons.Add(pigeon);
		Utils.ActivateTree(pigeon.gameObject, false);
	}

	private void OnUserEnterRoom(RoomUserEnterMessage e)
	{
		spawnedCharacters++;
		CheckThresholds();
	}

	private void OnUserLeaveRoom(RoomUserLeaveMessage e)
	{
		spawnedCharacters = Mathf.Max(spawnedCharacters - 1, 0);
		CheckThresholds();
	}

	private void OnGraphicsOptionsChange(GraphicsOptionsChange e)
	{
		CheckThresholds();
	}

	private void CheckThresholds()
	{
		float num = 100f;
		foreach (LimitingThreshold limitingThreshold in limitingThresholds)
		{
			if (limitingThreshold.CharacterCount <= spawnedCharacters && limitingThreshold.MaxPercent < num)
			{
				num = limitingThreshold.MaxPercent;
			}
		}
		maxPigeons = (int)(num / 100f * (float)spawnPoints.Count);
		PrunePigeons();
	}

	private void PrunePigeons()
	{
		if (activePigeons == null)
		{
			return;
		}
		int num = activePigeons.Count - maxPigeons;
		if (num > 0)
		{
			List<PigeonDistance> list = new List<PigeonDistance>();
			GameObject localPlayer = GameController.GetController().LocalPlayer;
			Vector3 relativeLocation = (!(localPlayer != null)) ? Vector3.zero : localPlayer.transform.position;
			foreach (AIControllerPigeon activePigeon in activePigeons)
			{
				list.Add(new PigeonDistance(activePigeon, relativeLocation));
			}
			list.Sort(delegate(PigeonDistance x, PigeonDistance y)
			{
				return (y.sqrDistance > x.sqrDistance) ? 1 : (-1);
			});
			for (int i = 0; i < num; i++)
			{
				AIControllerPigeon pigeon = list[i].pigeon;
				pigeon.StartCoroutine(pigeon.Takeoff());
			}
		}
	}
}
