public class PolymorphSpawnMessage : NetworkMessage
{
	public string polymorphName;

	public GoNetId polymorphNetId;

	public PolymorphSpawnMessage()
	{
	}

	public PolymorphSpawnMessage(GoNetId goNetId, string polymorphName, GoNetId polymorphNetId)
		: base(goNetId)
	{
		this.polymorphName = polymorphName;
		this.polymorphNetId = polymorphNetId;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(polymorphName);
		writer.Write(polymorphNetId);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		polymorphName = reader.ReadString();
		polymorphNetId = reader.ReadGoNetId();
	}
}
