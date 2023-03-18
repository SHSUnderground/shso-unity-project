public class ForceEmoteMessage : NetworkMessage
{
	public int emoteID;

	public ForceEmoteMessage()
	{
	}

	public ForceEmoteMessage(GoNetId goNetId)
		: base(goNetId)
	{
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(emoteID);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		emoteID = reader.ReadInt32();
	}
}
