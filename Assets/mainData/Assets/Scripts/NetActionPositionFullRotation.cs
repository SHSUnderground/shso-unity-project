using UnityEngine;

public class NetActionPositionFullRotation : NetActionPositionFull
{
	public Vector3 lookVector;

	public NetActionPositionFullRotation()
	{
	}

	public NetActionPositionFullRotation(GameObject initObject)
		: base(initObject)
	{
		lookVector = initObject.transform.forward;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(lookVector);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		lookVector = reader.ReadVector3();
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionPositionFullRotation;
	}

	public override string ToString()
	{
		return "NetActionPositionFullRotation: " + timestamp + ", lookVector = " + lookVector;
	}
}
