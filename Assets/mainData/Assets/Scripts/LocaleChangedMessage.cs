public class LocaleChangedMessage : ShsEventMessage
{
	public readonly string PreviousLocale;

	public readonly string NewLocale;

	public TransactionMonitor Transaction;

	public LocaleChangedMessage(string prevLocale, string newLocale, TransactionMonitor transaction)
	{
		PreviousLocale = prevLocale;
		NewLocale = newLocale;
		Transaction = transaction;
	}
}
