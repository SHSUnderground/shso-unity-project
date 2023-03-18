public class NetActionDropThrowable : NetAction
{
	public bool silent;

	public NetActionDropThrowable()
	{
	}

	public NetActionDropThrowable(bool Silent)
	{
		silent = Silent;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(silent);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		silent = reader.ReadBoolean();
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionDropThrowable;
	}

	public override string ToString()
	{
		return "NetActionDropThrowable: " + timestamp;
	}
}
