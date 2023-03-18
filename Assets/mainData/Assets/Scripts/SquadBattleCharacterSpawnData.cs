using UnityEngine;

public class SquadBattleCharacterSpawnData
{
	public delegate void TemporaryCharacterSpawned(GameObject spawnedCharacter);

	public string characterName = "unknown";

	public SquadBattleCharacterLocator spawnLocator;

	public SquadBattleCharacterLocator homeLocator;

	public SquadBattleAction action;

	public GameObject approachTarget;

	public DataWarehouse spawnData;

	public GameObject obj;

	public string assetBundleName;

	public TemporaryCharacterSpawned onSpawned;
}
