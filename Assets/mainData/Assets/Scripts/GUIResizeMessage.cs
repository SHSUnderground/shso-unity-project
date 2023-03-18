using UnityEngine;

public class GUIResizeMessage : ShsEventMessage
{
	public readonly GUIControl.HandleResizeSource Source;

	public readonly Vector2 LastSize;

	public readonly Vector2 NewSize;

	public readonly Rect LastRect;

	public readonly Rect NewRect;

	public readonly bool StartupFlag;

	public GUIResizeMessage(GUIControl.HandleResizeSource Source, Rect LastRect, Rect NewRect)
		: this(Source, LastRect, NewRect, false)
	{
	}

	public GUIResizeMessage(GUIControl.HandleResizeSource Source, Rect LastRect, Rect NewRect, bool StartupFlag)
	{
		this.Source = Source;
		this.LastRect = LastRect;
		this.NewRect = NewRect;
		LastSize = new Vector2(LastRect.width, LastRect.height);
		NewSize = new Vector2(NewRect.width, NewRect.height);
		this.StartupFlag = StartupFlag;
	}
}
