using UnityEngine;

public class NetActionAttack : NetActionPositionFullRotation
{
	public GameObject targetObject;

	public string attackName;

	public NetActionAttack()
	{
		targetObject = null;
		attackName = string.Empty;
	}

	public NetActionAttack(GameObject initObject, GameObject newTargetObject, string newAttackName)
		: base(initObject)
	{
		targetObject = newTargetObject;
		attackName = newAttackName;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(targetObject);
		writer.Write(attackName);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		targetObject = reader.ReadGameObject();
		attackName = reader.ReadString();
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionAttack;
	}

	public override string ToString()
	{
		return "NetActionAttack: " + timestamp + ", attackName = " + attackName + ", position = " + position;
	}
}
