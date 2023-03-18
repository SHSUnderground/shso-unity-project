using UnityEngine;

public class GUIClickEvent : GUIEvent
{
	public Vector2 ClickPosition;

	public GUIClickEvent()
	{
	}

	public GUIClickEvent(Vector2 ClickPosition)
	{
		this.ClickPosition = ClickPosition;
	}
}
