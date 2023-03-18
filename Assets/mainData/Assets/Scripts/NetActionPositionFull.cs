using UnityEngine;

public class NetActionPositionFull : NetAction
{
	public Vector3 position;

	public bool onGround;

	public float health;

	public float power;

	public NetActionPositionFull()
	{
	}

	public NetActionPositionFull(GameObject initObject)
	{
		position = initObject.transform.position;
		health = 1f;
		power = 0f;
		CombatController combatController = initObject.GetComponent(typeof(CombatController)) as CombatController;
		if (combatController != null)
		{
			health = combatController.getHealth();
			PlayerCombatController playerCombatController = combatController as PlayerCombatController;
			if (playerCombatController != null)
			{
				power = playerCombatController.getPower();
			}
		}
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(position);
		writer.Write(onGround);
		writer.Write(health);
		writer.Write(power);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		position = reader.ReadVector3();
		onGround = reader.ReadBoolean();
		health = reader.ReadFloat();
		power = reader.ReadFloat();
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionPositionFull;
	}

	public override string ToString()
	{
		return "NetActionPositionFull: " + timestamp + ", position = " + position;
	}
}
