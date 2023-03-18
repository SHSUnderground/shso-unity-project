public class CharacterReadyMessage : ShsEventMessage
{
	private CharacterSelectionBlock selectionBlock;

	public CharacterSelectionBlock SelectionBlock
	{
		get
		{
			return selectionBlock;
		}
	}

	public CharacterReadyMessage(CharacterSelectionBlock selectionBlock)
	{
		this.selectionBlock = selectionBlock;
	}
}
