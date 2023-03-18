using System.Collections.Generic;
using UnityEngine;

public class SimpleCharacterCache : ICharacterCache
{
	public delegate void CharacterSpawned(GameObject cachedPrefab, CharacterSpawnData initData, CharacterSpawn spawnPoint);

	public delegate void CharacterCached(string characterName, GameObject cachedCharacter);

	private CharacterSpawned _onCharacterSpawned;

	private CharacterCached _onCharacterCached;

	private Dictionary<string, GameObject> precachedCharacters = new Dictionary<string, GameObject>();

	public CharacterSpawned OnCharacterSpawned
	{
		get
		{
			return _onCharacterSpawned;
		}
		set
		{
			_onCharacterSpawned = value;
		}
	}

	public CharacterCached OnCharacterCached
	{
		get
		{
			return _onCharacterCached;
		}
		set
		{
			_onCharacterCached = value;
		}
	}

	public bool IsCharacterCached(string characterName)
	{
		if (characterName == null || characterName == string.Empty)
		{
			CspUtils.DebugLog("Null character name");
			characterName = string.Empty;
		}
		return precachedCharacters.ContainsKey(characterName);
	}

	public void StartCharacterCache(string characterName)
	{
	}

	public void CacheCharacter(string characterName, GameObject prefab)
	{
		precachedCharacters[characterName] = prefab;
		if (OnCharacterCached != null)
		{
			OnCharacterCached(characterName, prefab);
		}
	}

	public bool SpawnCachedCharacter(string characterName, CharacterSpawnData initData, CharacterSpawn targetSpawnPoint)
	{
		GameObject value;
		if (precachedCharacters.TryGetValue(characterName, out value))
		{
			targetSpawnPoint.SpawnFromPrefab(value, initData);
			if (OnCharacterSpawned != null)
			{
				OnCharacterSpawned(value, initData, targetSpawnPoint);
			}
			return true;
		}
		return false;
	}

	public void ClearCharacterCache()
	{
		precachedCharacters.Clear();
	}

	public bool IsCharacterBeingCached(string characterName)
	{
		return true;
	}

	public void FailCacheCharacter(string characterName, string error)
	{
		CspUtils.DebugLog("Cache failed for <" + characterName + " > with error: " + error);
	}
}
