public class EnsureButtonVisibleMessage : ShsEventMessage
{
	public SHSHudWheels.ButtonType button;

	public EnsureButtonVisibleMessage(SHSHudWheels.ButtonType button)
	{
		this.button = button;
	}
}
