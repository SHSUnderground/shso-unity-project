public class NetActionVO : NetAction
{
	public ResolvedVOAction action;

	public NetActionVO()
	{
	}

	public NetActionVO(ResolvedVOAction action)
	{
		this.action = action;
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionVO;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		action.SerializeToBinary(writer);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		action = new ResolvedVOAction();
		action.DeserializeFromBinary(reader);
	}
}
