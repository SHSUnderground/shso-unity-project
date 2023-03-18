using UnityEngine;

public class NetActionDirectedMenuChat : NetActionPositionFull
{
	public GameObject sourcePlayer;

	public GameObject targetPlayer;

	public MenuChatGroup group;

	public NetActionDirectedMenuChat()
	{
	}

	public NetActionDirectedMenuChat(GameObject initObject, GameObject targetPlayer, MenuChatGroup group)
		: base(initObject)
	{
		sourcePlayer = initObject;
		this.targetPlayer = targetPlayer;
		this.group = group;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(sourcePlayer);
		writer.Write(targetPlayer);
		writer.Write(group.PhraseKey);
		writer.Write(group.EmoteId ?? string.Empty);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		sourcePlayer = reader.ReadGameObject();
		targetPlayer = reader.ReadGameObject();
		MenuChatGroup menuChatGroup = new MenuChatGroup();
		menuChatGroup.PhraseKey = reader.ReadString();
		menuChatGroup.EmoteId = reader.ReadString();
		group = menuChatGroup;
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionDirectedMenuChat;
	}

	public override string ToString()
	{
		return "NetActionDirectedMenuChat: " + timestamp + ", targetPlayer = " + targetPlayer + ", emoteId = " + group.EmoteId;
	}
}
