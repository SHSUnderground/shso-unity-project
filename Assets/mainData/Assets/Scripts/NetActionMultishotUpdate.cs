using UnityEngine;

public class NetActionMultishotUpdate : NetAction
{
	public float nextAttackTime;

	public GameObject firstTarget;

	public GameObject secondTarget;

	public NetActionMultishotUpdate()
	{
	}

	public NetActionMultishotUpdate(float nextTime, GameObject target1, GameObject target2)
	{
		nextAttackTime = nextTime;
		firstTarget = target1;
		secondTarget = target2;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(nextAttackTime);
		writer.Write(firstTarget);
		writer.Write(secondTarget);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		nextAttackTime = reader.ReadFloat();
		firstTarget = reader.ReadGameObject();
		secondTarget = reader.ReadGameObject();
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionMultishotUpdate;
	}

	public override string ToString()
	{
		return "NetActionMultishotUpdate: " + nextAttackTime.ToString() + ", " + firstTarget.name + ", " + secondTarget.name + ", " + base.ToString();
	}
}
