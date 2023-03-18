public class SpawnPrestigeMessage : NetworkMessage
{
	public string hero = string.Empty;

	public SpawnPrestigeMessage()
	{
	}

	public SpawnPrestigeMessage(GoNetId goNetId)
		: base(goNetId)
	{
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(hero);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		hero = reader.ReadString();
	}
}
