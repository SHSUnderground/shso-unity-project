public class CardGameUIMessage : ShsEventMessage
{
	public enum UIEvent
	{
		InitializePlayer,
		StartHand,
		EnablePassButton,
		DisablePassButton,
		ClickPassButton
	}

	public readonly UIEvent Event;

	public readonly int PlayerNum;

	public CardGameUIMessage(UIEvent NewEvent, int AffectedPlayer)
	{
		Event = NewEvent;
		PlayerNum = AffectedPlayer;
	}
}
