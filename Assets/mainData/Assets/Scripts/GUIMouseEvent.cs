using UnityEngine;

public class GUIMouseEvent : GUIEvent
{
	public Vector2 MousePosition;

	public GUIMouseEvent()
	{
	}

	public GUIMouseEvent(Vector2 MousePosition)
	{
		this.MousePosition = MousePosition;
	}
}
