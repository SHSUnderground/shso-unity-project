public class NetActionMultiHotspotSync : NetAction
{
	public double serverLaunchTime = -1.0;

	public NetActionMultiHotspotSync()
	{
	}

	public NetActionMultiHotspotSync(double serverLaunchTime)
	{
		this.serverLaunchTime = serverLaunchTime;
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionMultiHotspotSync;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(serverLaunchTime);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		serverLaunchTime = reader.ReadDouble();
	}

	public override string ToString()
	{
		return "NetActionMultiHotspotSync: " + timestamp + ", launch time = " + serverLaunchTime;
	}
}
