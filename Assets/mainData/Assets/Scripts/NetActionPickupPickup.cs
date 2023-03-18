using UnityEngine;

public class NetActionPickupPickup : NetActionPositionFull
{
	public GoNetId pickupObjectID;

	public GameObject pickupObject;

	public NetActionPickupPickup()
	{
	}

	public NetActionPickupPickup(GameObject initObject, GameObject newPickupObject)
		: base(initObject)
	{
		NetworkComponent networkComponent = newPickupObject.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
		if (networkComponent == null)
		{
			CspUtils.DebugLog("No network component found in NetActionPickupPickup constructor, pickup will not synchronize properly");
		}
		else
		{
			pickupObjectID = networkComponent.goNetId;
		}
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(pickupObjectID);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		pickupObjectID = reader.ReadGoNetId();
		pickupObject = AppShell.Instance.ServerConnection.Game.GetGameObjectFromNetId(pickupObjectID);
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionPickupPickup;
	}

	public override string ToString()
	{
		return "NetActionPickupThrowable: " + timestamp + ", position = " + position;
	}
}
