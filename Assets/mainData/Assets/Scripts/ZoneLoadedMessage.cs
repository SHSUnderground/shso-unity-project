public class ZoneLoadedMessage : ShsEventMessage
{
	public string zoneName;

	public int zoneID;

	public ZoneLoadedMessage(string zone)
	{
		zoneName = zone;
		zoneID = -1;
	}

	public ZoneLoadedMessage(string zone, int zoneID)
	{
		zoneName = zone;
		this.zoneID = zoneID;
	}
}
