public class InventoryCollectionUpdateMessage : ShsEventMessage
{
	public string[] keys;

	public InventoryCollectionUpdateMessage(params string[] keys)
	{
		this.keys = keys;
	}
}
