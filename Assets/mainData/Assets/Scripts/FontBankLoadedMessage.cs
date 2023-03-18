public class FontBankLoadedMessage : ShsEventMessage
{
	public readonly string NewLocale;

	public TransactionMonitor Transaction;

	public FontBankLoadedMessage(string newLocale, TransactionMonitor transaction)
	{
		NewLocale = newLocale;
		Transaction = transaction;
	}
}
