internal class ItemDeselectedMessage : ShsEventMessage
{
	public readonly string ItemId;

	public ItemDeselectedMessage(string ItemId)
	{
		this.ItemId = ItemId;
	}
}
