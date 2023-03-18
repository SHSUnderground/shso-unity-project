public class InventoryFetchCompleteMessage : ShsEventMessage
{
	public bool success;

	public InventoryFetchCompleteMessage(bool success)
	{
		this.success = success;
	}
}
