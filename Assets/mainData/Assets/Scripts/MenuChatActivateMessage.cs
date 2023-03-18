using UnityEngine;

public class MenuChatActivateMessage : ShsEventMessage
{
	public MenuChatGroup Group;

	public MenuLevelInfo MenuLevel;

	public Rect OriginRect;

	public MenuChatActivateMessage()
		: this(null, null, new Rect(0f, 0f, 0f, 0f))
	{
	}

	public MenuChatActivateMessage(Rect rect, Vector2 offset)
		: this(null, null, new Rect(rect.x + offset.x, rect.y + offset.y, rect.width, rect.height))
	{
	}

	public MenuChatActivateMessage(MenuChatGroup group, MenuLevelInfo level, Rect origin)
	{
		Group = group;
		OriginRect = origin;
		MenuLevel = level;
	}
}
