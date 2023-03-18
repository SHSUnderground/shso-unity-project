using UnityEngine;

public interface ICharacterCache
{
	bool IsCharacterCached(string characterName);

	void StartCharacterCache(string characterName);

	void CacheCharacter(string characterName, GameObject prefab);

	bool SpawnCachedCharacter(string characterName, CharacterSpawnData initData, CharacterSpawn targetSpawnPoint);

	void ClearCharacterCache();

	bool IsCharacterBeingCached(string characterName);

	void FailCacheCharacter(string characterName, string error);
}
