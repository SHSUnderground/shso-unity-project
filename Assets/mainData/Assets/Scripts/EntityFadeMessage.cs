using UnityEngine;

public class EntityFadeMessage : ShsEventMessage
{
	public GameObject entity;

	public bool fadeOut;

	public EntityFadeMessage(GameObject entity, bool fadeOut)
	{
		this.entity = entity;
		this.fadeOut = fadeOut;
	}
}
