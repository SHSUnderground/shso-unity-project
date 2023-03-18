using UnityEngine;

public class GameWorldMenuChatMessage : ShsEventMessage
{
	public MenuChatGroup Group;

	public GameObject player;

	public GameWorldMenuChatMessage(GameObject player, MenuChatGroup group)
	{
		Group = group;
		this.player = player;
	}
}
