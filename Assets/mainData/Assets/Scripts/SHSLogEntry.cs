using System;

public class SHSLogEntry
{
	public enum LogEntryType
	{
		Debug,
		Info,
		Warning,
		Error,
		Critical,
		Highlight
	}

	public string Message;

	public LogEntryType EntryType;

	public DateTime TimeStamp;

	public bool IsHighlighted;

	public SHSLogEntry(string message, LogEntryType type)
	{
		Message = message;
		EntryType = type;
		TimeStamp = DateTime.Now;
		IsHighlighted = false;
	}
}
