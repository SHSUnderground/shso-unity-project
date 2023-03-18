public interface ILog
{
	void AddListener(ILogListener listener);

	void RemoveListener(ILogListener listener);
}
