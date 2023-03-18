using System;

public class TransactionInProgressException : Exception
{
	public TransactionInProgressException()
	{
	}

	public TransactionInProgressException(string message)
		: base(message)
	{
	}

	public TransactionInProgressException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
