public class AckTossCoinMessage : ShsEventMessage
{
	public readonly int CoinTossResult;

	public AckTossCoinMessage(int Result)
	{
		CoinTossResult = Result;
	}
}
