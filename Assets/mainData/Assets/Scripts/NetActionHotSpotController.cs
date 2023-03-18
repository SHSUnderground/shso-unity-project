using UnityEngine;

public class NetActionHotSpotController : NetActionPositionFull
{
	public HotSpotController hotSpot;

	public NetActionHotSpotController()
	{
	}

	public NetActionHotSpotController(GameObject initObject, HotSpotController newHotSpot)
		: base(initObject)
	{
		hotSpot = newHotSpot;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(hotSpot.gameObject);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		GameObject g = reader.ReadGameObject();
		hotSpot = Utils.GetComponent<HotSpotController>(g);
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionHotSpotController;
	}

	public override string ToString()
	{
		return "NetActionHotSpotController: " + timestamp + ", hotSpot = " + hotSpot.name + ", position = " + position;
	}
}
