using CardGame;

public struct CardMoveData
{
	public CardGamePlayer player;

	public PlayerType playerType;

	public BattleCard card;

	public CardPile srcPile;

	public CardPile destPile;

	public float secondsPause;

	public float secondsDelay;

	public float secondsAnimDuration;

	public bool proceed;

	public bool startFaceUp;

	public bool endFaceUp;

	public string animName;

	public float animSpeed;

	public CardGameAudioManager.SFX sfx;

	public CardMoveData(PlayerType _type, BattleCard _card, CardPile _src, CardPile _dest)
	{
		playerType = _type;
		player = null;
		card = _card;
		srcPile = _src;
		destPile = _dest;
		startFaceUp = (endFaceUp = false);
		animName = string.Empty;
		animSpeed = 1f;
		secondsPause = 0f;
		secondsDelay = 0f;
		secondsAnimDuration = 1f;
		proceed = false;
		sfx = CardGameAudioManager.SFX.None;
	}

	public void pickAnimation()
	{
	}
}
