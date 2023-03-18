public class MenuChatSelectedMessage : ShsEventMessage
{
	public MenuChatGroup Group;

	public MenuChatSelectedMessage(MenuChatGroup group)
	{
		Group = group;
	}
}
