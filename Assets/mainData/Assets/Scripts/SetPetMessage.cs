public class SetPetMessage : NetworkMessage
{
	public int petID;

	public SetPetMessage()
	{
	}

	public SetPetMessage(GoNetId goNetId)
		: base(goNetId)
	{
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(petID);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		petID = reader.ReadInt32();
	}
}
