using UnityEngine;

public interface IGameController
{
	string ZoneName
	{
		get;
	}

	void SetOwner(GameController owner);

	void Awake();

	void OnEnable();

	void OnDisable();

	void Start();

	void Update();

	void OnOldControllerUnloading(AppShell.GameControllerTypeData currentGameData, AppShell.GameControllerTypeData newGameData);

	bool AddPlayerCharacterSpawner(CharacterSpawn possibleSpawner);

	bool AddNpcSpawner(CharacterSpawn npcSpawner);

	void AddSpawnPoint(SpawnPoint pt);

	Vector3 GetRespawnPoint();

	bool ChangeCharacters();

	void CharacterDespawned(GameObject character);

	void GrantXP(CharacterGlobals player, int xp);
}
