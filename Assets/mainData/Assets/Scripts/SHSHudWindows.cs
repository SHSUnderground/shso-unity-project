using System;
//using System.Runtime.CompilerServices;

public class SHSHudWindows : GUIDynamicWindow
{
	public SHSHudWheels.ButtonType buttonType;

	public bool RemoveOnHudClose;

	private bool closeToggled;



	public event Action OnToggleClose;

	public void ToggleClosed()
	{
		if (this.closeToggled)
		{
			return;
		}
		this.closeToggled = true;
		this.OnToggleClose();
	}
}