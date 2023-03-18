using UnityEngine;

public class NetActionPositionUpdate : NetAction
{
	public sbyte xDelta;

	public sbyte yDelta;

	public sbyte zDelta;

	public bool onGround;

	public NetActionPositionUpdate()
	{
	}

	public NetActionPositionUpdate(Vector3 delta)
	{
		xDelta = (sbyte)(delta.x * 100f);
		yDelta = (sbyte)(delta.y * 10f);
		zDelta = (sbyte)(delta.z * 100f);
	}

	public Vector3 getDelta()
	{
		Vector3 result = default(Vector3);
		result.x = (float)xDelta * 0.01f;
		result.y = (float)yDelta * 0.1f;
		result.z = (float)zDelta * 0.01f;
		return result;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(xDelta);
		writer.Write(yDelta);
		writer.Write(zDelta);
		writer.Write(onGround);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		xDelta = reader.ReadSByte();
		yDelta = reader.ReadSByte();
		zDelta = reader.ReadSByte();
		onGround = reader.ReadBoolean();
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionPositionUpdate;
	}

	public override string ToString()
	{
		return "NetActionPositionUpdate: " + timestamp + ", xDelta = " + xDelta + ", zDelta = " + zDelta;
	}
}
