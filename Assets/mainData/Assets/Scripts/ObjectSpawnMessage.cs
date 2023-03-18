using UnityEngine;

public class ObjectSpawnMessage : NetworkMessage
{
	public string componentName;

	public Vector3 spawnLocation;

	public Quaternion spawnRotation;

	public GoNetId objectNetId;

	public string prefabName;

	public GameObject parent;

	public ObjectSpawnMessage()
	{
	}

	public ObjectSpawnMessage(GoNetId goNetId, string newComponentName, Vector3 newSpawnLocation, Quaternion newSpawnRotation, GoNetId newObjectNetId, string newPrefabName, GameObject newParent)
		: base(goNetId)
	{
		componentName = newComponentName;
		spawnLocation = newSpawnLocation;
		spawnRotation = newSpawnRotation;
		objectNetId = newObjectNetId;
		prefabName = newPrefabName;
		parent = newParent;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(componentName);
		writer.Write(spawnLocation);
		writer.Write(spawnRotation);
		writer.Write(objectNetId);
		writer.Write(prefabName);
		writer.Write(parent);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		componentName = reader.ReadString();
		spawnLocation = reader.ReadVector3();
		spawnRotation = reader.ReadQuaternion();
		objectNetId = reader.ReadGoNetId();
		prefabName = reader.ReadString();
		parent = reader.ReadGameObject();
	}
}
