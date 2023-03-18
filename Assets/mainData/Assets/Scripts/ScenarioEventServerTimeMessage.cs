public class ScenarioEventServerTimeMessage : NetworkMessage
{
	public double serverTime;

	public ScenarioEventServerTimeMessage()
	{
	}

	public ScenarioEventServerTimeMessage(GoNetId goNetId)
		: base(goNetId)
	{
	}

	public ScenarioEventServerTimeMessage(GoNetId goNetId, double newServerTime)
		: base(goNetId)
	{
		serverTime = newServerTime;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(serverTime);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		serverTime = reader.ReadDouble();
	}
}
