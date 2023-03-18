using System;

public class InvalidVOOutputException : ApplicationException
{
	public InvalidVOOutputException(string message)
		: base(message)
	{
	}
}
