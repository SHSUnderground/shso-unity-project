public class LoadingProgressMessage : ShsEventMessage
{
	public enum LoadingState
	{
		Started,
		InProgress,
		Complete
	}

	public readonly LoadingState State;

	public readonly string Message;

	public readonly float Pct;

	public LoadingProgressMessage(LoadingState state, float pct, string message)
	{
		State = state;
		Pct = pct;
		Message = message;
	}
}
