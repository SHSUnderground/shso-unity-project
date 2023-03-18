using UnityEngine;

public class NetActionExitDoor : NetActionPositionFull
{
	public DoorManager doorManager;

	public NetActionExitDoor()
	{
	}

	public NetActionExitDoor(GameObject initObject, DoorManager newDoorManager)
		: base(initObject)
	{
		doorManager = newDoorManager;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(doorManager.gameObject);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		GameObject g = reader.ReadGameObject();
		doorManager = Utils.GetComponent<DoorManager>(g);
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionExitDoor;
	}

	public override string ToString()
	{
		return "NetActionExitDoor: " + timestamp + ", hotSpot = " + doorManager.name + ", position = " + position;
	}
}
