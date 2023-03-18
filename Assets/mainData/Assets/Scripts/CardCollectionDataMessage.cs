public class CardCollectionDataMessage : ShsEventMessage
{
	public enum CCDataEvent
	{
		DeckCountChanged,
		CollectionCountChanged
	}

	public readonly CCDataEvent Event;

	public readonly int TotalCount;

	public readonly int CardTypeCount;

	public bool ScrollToTop;

	public CardCollectionDataMessage(CCDataEvent newEvent, int totalCount, int cardTypeCount)
	{
		Event = newEvent;
		TotalCount = totalCount;
		CardTypeCount = cardTypeCount;
	}
}
