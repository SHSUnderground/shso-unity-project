public class CardQuestFetchCompleteMessage : ShsEventMessage
{
	public bool success;

	public CardQuestFetchCompleteMessage(bool success)
	{
		this.success = success;
	}
}
