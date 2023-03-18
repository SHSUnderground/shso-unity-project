public class CounterUpdatedMessage : ShsEventMessage
{
	public string Key;

	public string Path;

	public long PreviousValue;

	public long NewValue;

	public string QualifierKey;

	public ISHSCounterType Counter;

	public CounterUpdatedMessage(string key, string path, long previousValue, long newValue, string qualifierKey, ISHSCounterType counter)
	{
		Key = counter.Name;
		Path = counter.Path;
		PreviousValue = previousValue;
		NewValue = newValue;
		QualifierKey = qualifierKey;
		Counter = counter;
	}
}
