using System.Collections.Generic;

public class NetActionMessage : NetworkMessage
{
	public List<NetAction> actions;

	public NetActionMessage()
	{
	}

	public NetActionMessage(GoNetId goNetId)
		: base(goNetId)
	{
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(actions.Count);
		foreach (NetAction action in actions)
		{
			writer.Write(action);
		}
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		int num = reader.ReadInt32();
		actions = new List<NetAction>(num);
		for (int i = 0; i < num; i++)
		{
			actions.Add(reader.ReadNetAction());
		}
	}
}
