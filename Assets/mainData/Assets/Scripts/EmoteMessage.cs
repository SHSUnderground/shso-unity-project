using UnityEngine;

public class EmoteMessage : ShsEventMessage
{
	public GameObject sender;

	public sbyte emote;

	public EmoteMessage(GameObject sender, sbyte emote)
	{
		this.sender = sender;
		this.emote = emote;
	}
}
