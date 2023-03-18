using System.Collections;
using UnityEngine;

public class NewEntityMessage : NetworkMessage
{
	public string modelName;

	public CharacterSpawn.Type spawnType;

	public Vector3 pos = Vector3.zero;

	public Quaternion rot = Quaternion.identity;

	public GameObject spawner;

	public Hashtable extraData;

	public NewEntityMessage()
	{
	}

	public NewEntityMessage(GoNetId goNetId)
		: base(goNetId)
	{
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(modelName);
		writer.Write((int)spawnType);
		writer.Write(pos);
		writer.Write(rot);
		writer.Write(spawner);
		writer.Write(extraData);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		modelName = reader.ReadString();
		spawnType = (CharacterSpawn.Type)reader.ReadInt32();
		pos = reader.ReadVector3();
		rot = reader.ReadQuaternion();
		spawner = reader.ReadGameObject();
		extraData = reader.ReadHashtable();
	}
}
