using UnityEngine;

public class EntityEngoldenMessage : ShsEventMessage
{
	public GameObject entity;

	public bool goldingIn;

	public EntityEngoldenMessage(GameObject entity, bool goldingIn)
	{
		this.entity = entity;
		this.goldingIn = goldingIn;
	}
}
