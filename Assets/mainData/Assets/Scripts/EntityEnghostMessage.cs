using UnityEngine;

public class EntityEnghostMessage : ShsEventMessage
{
	public GameObject entity;

	public bool ghostingIn;

	public EntityEnghostMessage(GameObject entity, bool ghostingIn)
	{
		this.entity = entity;
		this.ghostingIn = ghostingIn;
	}
}
