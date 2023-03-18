using UnityEngine;

public class EntitySpawnAnimationCompleteMessage : ShsEventMessage
{
	public GameObject go;

	public EntitySpawnAnimationCompleteMessage(GameObject spawn)
	{
		go = spawn;
	}
}
