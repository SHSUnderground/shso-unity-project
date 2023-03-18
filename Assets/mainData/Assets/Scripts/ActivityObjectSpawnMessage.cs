using UnityEngine;

public class ActivityObjectSpawnMessage : ShsEventMessage
{
	public GameObject go;

	public ActivityObjectSpawnMessage(GameObject spawn)
	{
		go = spawn;
	}
}
