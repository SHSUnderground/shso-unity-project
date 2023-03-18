using UnityEngine;

public class NetActionInteractiveObjectController : NetActionPositionFull
{
	public InteractiveObjectController hotSpot;

	public NetActionInteractiveObjectController()
	{
	}

	public NetActionInteractiveObjectController(GameObject initObject, InteractiveObjectController newHotSpot)
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
		if (gameObject != null)
		{
			hotSpot = Utils.GetComponent<InteractiveObjectController>(gameObject);
		}
		else
		{
			hotSpot = null;
		}
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionInteractiveObjectController;
	}

	public override string ToString()
	{
		return "NetActionInteractiveObjectController: " + timestamp + ", hotSpot = " + hotSpot.name + ", position = " + position;
	}
}
