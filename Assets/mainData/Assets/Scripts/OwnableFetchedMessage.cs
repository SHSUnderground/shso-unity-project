public class OwnableFetchedMessage : ShsEventMessage
{
	public OwnableDefinition.Category Category;

	public OwnableFetchedMessage(OwnableDefinition.Category category)
	{
		Category = category;
	}
}
