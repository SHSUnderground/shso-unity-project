using System.Collections.Generic;
using UnityEngine;

public class SpawnGroup : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public SpawnController.SpawnOrder spawnOrder;

	protected SpawnController spawnController;

	protected List<CharacterSpawn> allSpawners;

	protected List<CharacterSpawn> availableSpawners;

	protected List<CharacterSpawn> invalidSpawners;

	protected List<CharacterSpawn> activeSpawners;

	private void Start()
	{
	}

	public int GetAvailableSpawners()
	{
		availableSpawners = new List<CharacterSpawn>();
		allSpawners = new List<CharacterSpawn>();
		invalidSpawners = new List<CharacterSpawn>();
		activeSpawners = new List<CharacterSpawn>();
		int num = 0;
		for (int i = 0; i < base.transform.GetChildCount(); i++)
		{
			GameObject gameObject = base.transform.GetChild(i).gameObject;
			CharacterSpawn characterSpawn = gameObject.GetComponent(typeof(CharacterSpawn)) as CharacterSpawn;
			if (characterSpawn != null)
			{
				allSpawners.Add(characterSpawn);
				if (characterSpawn.CheckPlayerCount())
				{
					num++;
					availableSpawners.Add(characterSpawn);
				}
				else
				{
					invalidSpawners.Add(characterSpawn);
				}
			}
		}
		return num;
	}

	public void SetSpawnController(SpawnController owningController)
	{
		spawnController = owningController;
	}

	public void Spawn(Object trigger)
	{
		if (availableSpawners.Count == 0)
		{
			CspUtils.DebugLog("SpawnGroup " + base.name + " asked to spawn but no spawners available");
			return;
		}
		int index = 0;
		if (spawnOrder == SpawnController.SpawnOrder.Random)
		{
			index = Random.Range(0, availableSpawners.Count);
		}
		CharacterSpawn characterSpawn = availableSpawners[index];
		availableSpawners.RemoveAt(index);
		activeSpawners.Add(characterSpawn);
		characterSpawn.Triggered(trigger);
		AppShell.Instance.EventMgr.AddListener<EntityDespawnMessage>(characterSpawn, OnTrackedCharacterDespawned);
	}

	public void RemoteSpawn(CharacterSpawn spawner)
	{
		availableSpawners.Remove(spawner);
		activeSpawners.Add(spawner);
		AppShell.Instance.EventMgr.AddListener<EntityDespawnMessage>(spawner, OnTrackedCharacterDespawned);
		spawnController.RemoteGroupSpawn(this);
	}

	protected void OnTrackedCharacterDespawned(EntityDespawnMessage e)
	{
		if (e.cause == EntityDespawnMessage.despawnType.polymorph)
		{
			return;
		}
		SpawnData spawnData = e.go.GetComponent(typeof(SpawnData)) as SpawnData;
		if (spawnData == null)
		{
			CspUtils.DebugLog("Could not find SpawnData when trying stop tracking the character <" + e.go.name + "> that just despawned!");
			return;
		}
		if (spawnData.spawner != null)
		{
			spawnData.spawner.goNetId = GoNetId.Invalid;
		}
		activeSpawners.Remove(spawnData.spawner);
		if (spawnData.spawner.CheckPlayerCount())
		{
			availableSpawners.Add(spawnData.spawner);
		}
		else
		{
			invalidSpawners.Add(spawnData.spawner);
		}
		AppShell.Instance.EventMgr.RemoveListener<EntityDespawnMessage>(spawnData.spawner, OnTrackedCharacterDespawned);
		spawnController.OnSpawnGroupDespawn(this);
		if (activeSpawners.Count == 0)
		{
			spawnController.OnSpawnGroupAvailable(this);
		}
	}

	public int NumAvailableSpawns()
	{
		return availableSpawners.Count;
	}

	public void DestroyAllSpawnedCharacters()
	{
		foreach (CharacterSpawn allSpawner in allSpawners)
		{
			allSpawner.DestroyAllSpawnedCharacters();
		}
	}

	public bool HasSpawner(CharacterSpawn spawner)
	{
		return allSpawners.Contains(spawner);
	}

	public bool CheckPlayerCount()
	{
		List<int> list = new List<int>();
		for (int num = availableSpawners.Count - 1; num >= 0; num--)
		{
			if (!availableSpawners[num].CheckPlayerCount())
			{
				list.Add(num);
			}
		}
		for (int num2 = invalidSpawners.Count - 1; num2 >= 0; num2--)
		{
			if (invalidSpawners[num2].CheckPlayerCount())
			{
				availableSpawners.Add(invalidSpawners[num2]);
				invalidSpawners.RemoveAt(num2);
			}
		}
		foreach (int item in list)
		{
			availableSpawners.RemoveAt(item);
		}
		int num3 = 0;
		foreach (CharacterSpawn activeSpawner in activeSpawners)
		{
			if (activeSpawner.CheckPlayerCount())
			{
				num3++;
			}
		}
		return num3 + availableSpawners.Count > 0;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "SpawnGroupIcon.png");
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(base.transform.position, base.transform.parent.position);
	}
}
