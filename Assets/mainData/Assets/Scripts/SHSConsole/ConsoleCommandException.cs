using System;

namespace SHSConsole
{
	internal class ConsoleCommandException : Exception
	{
		public ConsoleCommandException(string Message)
			: base(Message)
		{
		}
	}
}
