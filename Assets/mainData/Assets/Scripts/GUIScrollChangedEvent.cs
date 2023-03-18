using UnityEngine;

public class GUIScrollChangedEvent : GUIEvent
{
	public Vector2 OldValue;

	public Vector2 NewValue;

	public GUIScrollChangedEvent()
	{
	}

	public GUIScrollChangedEvent(Vector2 oldValue, Vector2 newValue)
	{
		OldValue = oldValue;
		NewValue = newValue;
	}
}
