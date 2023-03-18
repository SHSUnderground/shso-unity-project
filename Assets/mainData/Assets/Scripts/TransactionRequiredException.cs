using System;

public class TransactionRequiredException : Exception
{
	public TransactionRequiredException()
	{
	}

	public TransactionRequiredException(string message)
		: base(message)
	{
	}

	public TransactionRequiredException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
