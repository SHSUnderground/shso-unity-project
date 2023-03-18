using UnityEngine;

public class NetActionProjectile : NetAction
{
	public GoNetId projectileObjectID;

	public GameObject target;

	public string attackName;

	public int impactIndex;

	public NetActionProjectile()
	{
	}

	public NetActionProjectile(GameObject projectileObject, GameObject newTarget, string newAttackName, int newImpactIndex)
	{
		if (projectileObject != null)
		{
			NetworkComponent networkComponent = projectileObject.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
			if (networkComponent == null)
			{
				CspUtils.DebugLog("No network component found in NetActionProjectile constructor, will not function");
				return;
			}
			projectileObjectID = networkComponent.goNetId;
		}
		else
		{
			projectileObjectID = GoNetId.Invalid;
		}
		target = newTarget;
		attackName = newAttackName;
		impactIndex = newImpactIndex;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(projectileObjectID);
		writer.Write(target);
		writer.Write(attackName);
		writer.Write(impactIndex);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		projectileObjectID = reader.ReadGoNetId();
		target = reader.ReadGameObject();
		attackName = reader.ReadString();
		impactIndex = reader.ReadInt32();
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionProjectile;
	}

	public override string ToString()
	{
		return "NetActionProjectile: " + timestamp;
	}
}
