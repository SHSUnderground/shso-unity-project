public class CardCollectionSavedMessage : ShsEventMessage
{
	public readonly bool Succeeded;

	public CardCollectionSavedMessage(bool succeeded)
	{
		Succeeded = succeeded;
	}
}
