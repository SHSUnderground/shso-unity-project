public class GUIDisplayChangeMessage : ShsEventMessage
{
	public enum DisplayTypeEnum
	{
		Visible,
		Invisible,
		Active,
		Inactive
	}

	private DisplayTypeEnum displayChangeType;

	private GUIControl controlChanged;

	public DisplayTypeEnum DisplayChangeType
	{
		get
		{
			return displayChangeType;
		}
		set
		{
			displayChangeType = value;
		}
	}

	public GUIControl ControlChanged
	{
		get
		{
			return controlChanged;
		}
		set
		{
			controlChanged = value;
		}
	}

	public GUIDisplayChangeMessage(GUIControl controlChanged, DisplayTypeEnum displayChangeType)
	{
		this.displayChangeType = displayChangeType;
		this.controlChanged = controlChanged;
	}
}
