using UnityEngine;

public class NetActionPickupThrowable : NetActionPositionFullRotation
{
	public GameObject pickupObject;

	public NetActionPickupThrowable()
	{
	}

	public NetActionPickupThrowable(GameObject initObject, GameObject newPickupObject)
		: base(initObject)
	{
		pickupObject = newPickupObject;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(pickupObject);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		pickupObject = reader.ReadGameObject();
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionPickupThrowable;
	}

	public override string ToString()
	{
		return "NetActionPickupThrowable: " + timestamp + ", position = " + position;
	}
}
