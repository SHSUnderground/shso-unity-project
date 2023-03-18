using UnityEngine;

public class EntityToRCBotMessage : ShsEventMessage
{
	public GameObject entity;

	public bool isRCBot;

	public EntityToRCBotMessage(GameObject entity, bool isRCBot)
	{
		this.entity = entity;
		this.isRCBot = isRCBot;
	}
}
