public class ToggleButtonVisibilityMessage : ShsEventMessage
{
	public SHSHudWheels.ButtonType button;

	public ToggleButtonVisibilityMessage(SHSHudWheels.ButtonType button)
	{
		this.button = button;
	}
}
