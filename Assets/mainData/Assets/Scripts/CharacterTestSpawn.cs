using System.Collections.Generic;
using UnityEngine;

public class CharacterTestSpawn : CharacterSpawn
{
	public bool forceLOD = true;

	public int forcedLOD;

	public bool attachCamera;

	public Vector3 cameraOffest;

	protected override void OnCharacterSpawned(GameObject newCharacter, CharacterSpawnData spawnData)
	{
		base.OnCharacterSpawned(newCharacter, spawnData);
		if (forceLOD)
		{
			LodBase lodBase = newCharacter.GetComponent(typeof(LodBase)) as LodBase;
			if (lodBase != null)
			{
				lodBase.ForceSetLod(forcedLOD);
			}
		}
		if (attachCamera)
		{
			Camera.main.transform.parent = newCharacter.transform;
			Camera.main.transform.localPosition = cameraOffest;
		}
	}

	public void DespawnSpawnedCharacters()
	{
		List<CharacterGlobals> list = new List<CharacterGlobals>(spawnedCharacters);
		foreach (CharacterGlobals item in list)
		{
			item.spawnData.Despawn(EntityDespawnMessage.despawnType.defeated);
		}
	}

	public CharacterGlobals GetSpawnedCharGlobal()
	{
		if (spawnedCharacters.Count > 0)
		{
			return spawnedCharacters[0];
		}
		return null;
	}
}
