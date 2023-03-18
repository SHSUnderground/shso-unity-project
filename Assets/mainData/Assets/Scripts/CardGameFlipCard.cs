public class CardGameFlipCard : ShsEventMessage
{
	public int numCards;

	public float duration;

	public CardGameFlipCard(int _numCards, float _duration)
	{
		numCards = _numCards;
		duration = _duration;
	}
}
