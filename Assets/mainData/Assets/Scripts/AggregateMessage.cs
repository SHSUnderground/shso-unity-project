using System.Collections.Generic;

public class AggregateMessage : NetworkMessage
{
	public List<NetworkMessage> messages;

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(messages);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		messages = reader.ReadNetworkMessageList();
	}
}
