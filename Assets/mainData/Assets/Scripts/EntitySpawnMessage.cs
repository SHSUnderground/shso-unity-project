using UnityEngine;

public class EntitySpawnMessage : ShsEventMessage
{
	public GameObject go;

	public CharacterSpawn.Type spawnType;

	public bool sendNewEntityMsg;

	public EntitySpawnMessage(GameObject spawn, CharacterSpawn.Type spawnType)
	{
		go = spawn;
		this.spawnType = spawnType;
		sendNewEntityMsg = true;
	}

	public EntitySpawnMessage(GameObject spawn, CharacterSpawn.Type spawnType, bool sendNewEntityMsg)
	{
		go = spawn;
		this.spawnType = spawnType;
		this.sendNewEntityMsg = sendNewEntityMsg;
	}
}
