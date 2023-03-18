public class SetTitleMessage : NetworkMessage
{
	public int titleID = -1;

	public int medallionID = -1;

	public SetTitleMessage()
	{
	}

	public SetTitleMessage(GoNetId goNetId)
		: base(goNetId)
	{
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(titleID);
		writer.Write(medallionID);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		titleID = reader.ReadInt32();
		medallionID = reader.ReadInt32();
	}
}
