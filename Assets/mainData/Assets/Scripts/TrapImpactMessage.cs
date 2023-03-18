using UnityEngine;

public class TrapImpactMessage : NetworkMessage
{
	public GameObject target;

	public TrapImpactMessage()
	{
	}

	public TrapImpactMessage(GoNetId goNetId, GameObject newTarget)
		: base(goNetId)
	{
		target = newTarget;
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
