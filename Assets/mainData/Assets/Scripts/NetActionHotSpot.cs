using UnityEngine;

public class NetActionHotSpot : NetActionPositionFull
{
	public HotSpot hotSpot;

	public NetActionHotSpot()
	{
	}

	public NetActionHotSpot(GameObject initObject, HotSpot newHotSpot)
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
		GameObject gameObject = reader.ReadGameObject();
		hotSpot = (gameObject.GetComponent(typeof(HotSpot)) as HotSpot);
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionHotSpot;
	}

	public override string ToString()
	{
		return "NetActionHotSpot: " + timestamp + ", hotSpot = " + hotSpot.name + ", position = " + position;
	}
}
