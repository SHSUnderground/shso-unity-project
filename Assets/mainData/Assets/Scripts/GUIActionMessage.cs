internal class GUIActionMessage : ShsEventMessage
{
	public enum GUIActionTypeEnum
	{
		DialogOpen,
		DialogClose,
		PanelOpen,
		PanelClose
	}

	public readonly GUIActionTypeEnum ActionType;

	public readonly string Target;

	public readonly double Flags;

	public GUIActionMessage(GUIActionTypeEnum ActionType, string Target, double Flags)
	{
		this.ActionType = ActionType;
		this.Target = Target;
		this.Flags = Flags;
	}
}
