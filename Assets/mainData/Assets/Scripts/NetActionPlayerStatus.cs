public class NetActionPlayerStatus : NetAction
{
	public PlayerStatusDefinition.Status status;

	public NetActionPlayerStatus()
	{
	}

	public NetActionPlayerStatus(PlayerStatusDefinition.Status status)
	{
		this.status = status;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		status.SerializeToBinary(writer);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		status = PlayerStatusDefinition.Status.DeserializeFromBinary(reader);
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionPlayerStatus;
	}

	public override string ToString()
	{
		return "NetActionPlayerStatus: " + status.ToString() + base.ToString();
	}
}
