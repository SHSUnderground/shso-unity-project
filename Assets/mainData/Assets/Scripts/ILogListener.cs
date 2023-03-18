public interface ILogListener
{
	void OnLogEntryAdded(SHSLogEntry entry);

	void OnLogEntryRemoved(int logEntryIndex);

	void OnLogEntryRemoved(SHSLogEntry entry);
}
