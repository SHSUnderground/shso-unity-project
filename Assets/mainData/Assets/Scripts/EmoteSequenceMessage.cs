using UnityEngine;

public class EmoteSequenceMessage : ShsEventMessage
{
	public GameObject sender;

	public sbyte emote;

	public EmoteSequenceMessage(GameObject sender, sbyte emote)
	{
		this.sender = sender;
		this.emote = emote;
	}
}
