internal class HoverHelpMessage : ShsEventMessage
{
	public enum HoverHelpState
	{
		Activate,
		Deactivate
	}

	public readonly string ItemId;

	public readonly HoverHelpState State;

	public HoverHelpMessage(string ItemId, HoverHelpState State)
	{
		this.ItemId = ItemId;
		this.State = State;
	}
}
