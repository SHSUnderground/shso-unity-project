using System.Collections;

public class PrizeWheelResultsMessage : ShsEventMessage
{
	public bool success;

	public Hashtable payload;

	public PrizeWheelResultsMessage(bool success, Hashtable payload)
	{
		this.success = success;
		this.payload = payload;
	}
}
