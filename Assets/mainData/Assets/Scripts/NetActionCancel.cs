public class NetActionCancel : NetAction
{
	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionCancel;
	}

	public override string ToString()
	{
		return "NetActionCancel: " + timestamp;
	}
}
