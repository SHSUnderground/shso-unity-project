using UnityEngine;

public class CardCollection3DMessage : ShsEventMessage
{
	public enum CC3DEvent
	{
		CardMouseEnter,
		CardMouseLeftClick,
		CardMouseRightClick,
		CounterReady
	}

	public readonly CC3DEvent Event;

	public readonly GameObject Sender;

	public CardCollection3DMessage(CC3DEvent newEvent, GameObject sender)
	{
		Event = newEvent;
		Sender = sender;
	}
}
