using UnityEngine;

public class EntityDespawnMessage : ShsEventMessage
{
	public enum despawnType
	{
		defeated,
		polymorph,
		effect,
		reload
	}

	public GameObject go;

	public CharacterSpawn.Type type;

	public despawnType cause;

	public bool sendDeleteEntityMsg;

	public bool releaseOwnership;

	public EntityDespawnMessage(GameObject go, CharacterSpawn.Type type, despawnType cause, bool sendDeleteEntityMsg, bool releaseOwnership)
	{
		this.go = go;
		this.type = type;
		this.cause = cause;
		this.sendDeleteEntityMsg = sendDeleteEntityMsg;
		this.releaseOwnership = releaseOwnership;
	}
}
