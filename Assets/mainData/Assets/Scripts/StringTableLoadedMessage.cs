public class StringTableLoadedMessage : ShsEventMessage
{
	public readonly string NewLocale;

	public TransactionMonitor Transaction;

	public StringTableLoadedMessage(string newLocale, TransactionMonitor transaction)
	{
		NewLocale = newLocale;
		Transaction = transaction;
	}
}
