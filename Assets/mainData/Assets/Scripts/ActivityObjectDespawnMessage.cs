using UnityEngine;

public class ActivityObjectDespawnMessage : ShsEventMessage
{
	public GameObject go;

	public bool dueToDestroy;

	public ActivityObjectDespawnMessage(GameObject go, bool dueToDestroy)
	{
		this.go = go;
		this.dueToDestroy = dueToDestroy;
	}
}
