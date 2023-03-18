using UnityEngine;

public class NetActionEmote : NetActionPositionFullRotation
{
	public sbyte emoteId;

	public GameObject targetedPlayer;

	public NetActionEmote()
	{
	}

	public NetActionEmote(GameObject initObject, sbyte emoteId, GameObject targetedPlayer)
		: base(initObject)
	{
		this.emoteId = emoteId;
		this.targetedPlayer = targetedPlayer;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(emoteId);
		writer.Write(targetedPlayer);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		emoteId = reader.ReadSByte();
		targetedPlayer = reader.ReadGameObject();
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionEmote;
	}

	public override string ToString()
	{
		return "NetActionEmote: " + timestamp + ", emoteId = " + emoteId;
	}
}
