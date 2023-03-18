using UnityEngine;

public class NetActionHitObject : NetActionPositionFull
{
	public GoNetId hitObjectID;

	public GameObject hitObject;

	public float damage;

	public NetActionHitObject()
	{
	}

	public NetActionHitObject(GameObject initObject, GoNetId newHitObjectID, float newDamage)
		: base(initObject)
	{
		hitObjectID = newHitObjectID;
		damage = newDamage;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(hitObjectID);
		writer.Write(damage);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		hitObjectID = reader.ReadGoNetId();
		damage = reader.ReadFloat();
		hitObject = AppShell.Instance.ServerConnection.Game.GetGameObjectFromNetId(hitObjectID);
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionHitObject;
	}

	public override string ToString()
	{
		return "NetActionHitObject: " + timestamp;
	}
}
