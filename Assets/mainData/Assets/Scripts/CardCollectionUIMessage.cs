public class CardCollectionUIMessage : ShsEventMessage
{
	public enum CCUIEvent
	{
		CollectionGridScroll,
		DeckGridScroll,
		SortTypeSelected,
		FactorSelected,
		BlockSelected,
		MinLevelSelected,
		MaxLevelSelected,
		ResetClicked,
		NewClicked,
		LoadClicked,
		SaveClicked,
		TestClicked,
		UnownedSelected,
		CardMouseEnter
	}

	public readonly CCUIEvent Event;

	public readonly GUIControl Control;

	public CardCollectionUIMessage(CCUIEvent newEvent, GUIControl sender)
	{
		Event = newEvent;
		Control = sender;
	}
}
