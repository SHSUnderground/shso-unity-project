public class GUIAutoCloseWindowMessage : ShsEventMessage
{
	public readonly string Target;

	public readonly bool AbortFlow;

	public GUIAutoCloseWindowMessage(string Target)
	{
		this.Target = Target;
		AbortFlow = true;
	}

	public GUIAutoCloseWindowMessage(string Target, bool AbortFlow)
	{
		this.Target = Target;
		this.AbortFlow = AbortFlow;
	}
}
