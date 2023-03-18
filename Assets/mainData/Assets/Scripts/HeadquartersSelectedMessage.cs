public class HeadquartersSelectedMessage : ShsEventMessage
{
	public readonly string headquartersName;

	public HeadquartersSelectedMessage(string HeadquartersName)
	{
		headquartersName = HeadquartersName;
	}
}
