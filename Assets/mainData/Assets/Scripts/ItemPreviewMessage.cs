internal class ItemPreviewMessage : ShsEventMessage
{
	public enum ItemPreviewMessageStateEnum
	{
		Activated,
		Deactivated
	}

	public readonly string ItemId;

	public readonly ItemPreviewMessageStateEnum ItemPreviewMessageState;

	public ItemPreviewMessage(string ItemId, ItemPreviewMessageStateEnum State)
	{
		this.ItemId = ItemId;
		ItemPreviewMessageState = State;
	}
}
