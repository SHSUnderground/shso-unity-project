using UnityEngine;

public class NetActionDie : NetActionPositionFull
{
	public float duration;

	public GameObject attacker;

	public NetActionDie()
	{
	}

	public NetActionDie(GameObject initObject, GameObject newAttacker, float newDuration)
		: base(initObject)
	{
		duration = newDuration;
		attacker = newAttacker;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(duration);
		writer.Write(attacker);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		duration = reader.ReadFloat();
		attacker = reader.ReadGameObject();
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionDie;
	}

	public override string ToString()
	{
		return "NetActionDie: " + timestamp + ", attacker = " + attacker.name;
	}
}
