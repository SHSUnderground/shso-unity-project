public class ScenarioEventMessage : NetworkMessage
{
	public string scenarioEventName;

	public ScenarioEventMessage()
	{
	}

	public ScenarioEventMessage(string newScenarioEventName)
	{
		scenarioEventName = newScenarioEventName;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(scenarioEventName);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		scenarioEventName = reader.ReadString();
	}
}
