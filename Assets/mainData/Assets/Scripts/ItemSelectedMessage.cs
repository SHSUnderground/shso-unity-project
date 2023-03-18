internal class ItemSelectedMessage : ShsEventMessage
{
	public readonly string ItemId;

	public ItemSelectedMessage(string ItemId)
	{
		this.ItemId = ItemId;
	}
}
