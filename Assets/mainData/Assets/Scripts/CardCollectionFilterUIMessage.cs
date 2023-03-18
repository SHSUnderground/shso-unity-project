public class CardCollectionFilterUIMessage : ShsEventMessage
{
	public readonly CardFilterType Filter;

	public readonly GUIControl Control;

	public readonly int iData;

	public readonly bool bData;

	public readonly string sData;

	public CardCollectionFilterUIMessage(GUIControl sender, CardFilterType filter)
	{
		Filter = filter;
		Control = sender;
	}

	public CardCollectionFilterUIMessage(GUIControl sender, CardFilterType filter, string s)
	{
		Filter = filter;
		sData = s;
		Control = sender;
	}

	public CardCollectionFilterUIMessage(GUIControl sender, CardFilterType filter, int i)
	{
		Filter = filter;
		iData = i;
		Control = sender;
	}
}
