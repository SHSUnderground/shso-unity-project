public class ZoneUnloadedMessage : ShsEventMessage
{
	public string zoneName;

	public ZoneUnloadedMessage(string zone)
	{
		zoneName = zone;
	}
}
