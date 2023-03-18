public class NetActionPolymorph : NetAction
{
	public bool revert;

	public NetActionPolymorph()
	{
	}

	public NetActionPolymorph(bool revert)
	{
		this.revert = revert;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(revert);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		revert = reader.ReadBoolean();
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionPolymorph;
	}

	public override string ToString()
	{
		return "NetActionPolymorph: " + timestamp;
	}
}
