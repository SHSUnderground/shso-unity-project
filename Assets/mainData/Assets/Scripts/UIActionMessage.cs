internal class UIActionMessage : ShsEventMessage
{
	public enum UIActionTypeEnum
	{
		DialogOpen,
		DialogClose,
		PanelOpen,
		PanelClose
	}

	public readonly UIActionTypeEnum ActionType;

	public readonly string Target;

	public readonly double Flags;

	public UIActionMessage(UIActionTypeEnum ActionType, string Target, double Flags)
	{
		this.ActionType = ActionType;
		this.Target = Target;
		this.Flags = Flags;
	}
}
