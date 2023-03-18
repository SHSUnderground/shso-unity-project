using UnityEngine;

public class RemoteImpactMessage : NetworkMessage
{
	public GameObject sourceObject;

	public Vector3 impactPosition;

	public string attackName;

	public int impactIndex;

	public float damage;

	public RemoteImpactMessage()
	{
	}

	public RemoteImpactMessage(GoNetId goNetId)
		: base(goNetId)
	{
	}

	public RemoteImpactMessage(GoNetId goNetId, Vector3 newImpactPosition, GameObject newSourceObject, string newAttackName, int newImpactIndex, float newDamage)
		: base(goNetId)
	{
		impactPosition = newImpactPosition;
		sourceObject = newSourceObject;
		attackName = newAttackName;
		impactIndex = newImpactIndex;
		damage = newDamage;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(impactPosition);
		writer.Write(sourceObject);
		writer.Write(attackName);
		writer.Write(impactIndex);
		writer.Write(damage);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		impactPosition = reader.ReadVector3();
		sourceObject = reader.ReadGameObject();
		attackName = reader.ReadString();
		impactIndex = reader.ReadInt32();
		damage = reader.ReadFloat();
	}
}
