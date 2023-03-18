using UnityEngine;

public class AssignTargetMessage : NetworkMessage
{
	public GameObject target;

	public AssignTargetMessage()
	{
	}

	public AssignTargetMessage(GoNetId goNetId)
		: base(goNetId)
	{
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(target);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		target = reader.ReadGameObject();
	}
}
