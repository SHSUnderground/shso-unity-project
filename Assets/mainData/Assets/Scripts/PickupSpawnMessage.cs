using UnityEngine;

public class PickupSpawnMessage : NetworkMessage
{
	public string pickupName;

	public Vector3 spawnLocation;

	public GoNetId objectNetId;

	public PickupSpawnMessage()
	{
	}

	public PickupSpawnMessage(GoNetId goNetId, string newPickupName, Vector3 newSpawnLocation, GoNetId newObjectNetId)
		: base(goNetId)
	{
		pickupName = newPickupName;
		spawnLocation = newSpawnLocation;
		objectNetId = newObjectNetId;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(pickupName);
		writer.Write(spawnLocation);
		writer.Write(objectNetId);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		pickupName = reader.ReadString();
		spawnLocation = reader.ReadVector3();
		objectNetId = reader.ReadGoNetId();
	}
}
