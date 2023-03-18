public class SHSClipInputWindow : GUIControlWindow
{
	protected GUIButton clipButton;

	public SHSClipInputWindow()
	{
		clipButton = new GUIButton();
		clipButton.SetPositionAndSize(-30f, 20f, 60f, 60f);
		clipButton.Text = "BLA";
		clipButton.MouseOver += clipButton_MouseOver;
		clipButton.MouseOut += clipButton_MouseOut;
		Add(clipButton);
	}

	private void clipButton_MouseOut(GUIControl sender, GUIMouseEvent EventData)
	{
		CspUtils.DebugLog("clip button mouse out.");
	}

	private void clipButton_MouseOver(GUIControl sender, GUIMouseEvent EventData)
	{
		CspUtils.DebugLog("clip button mouse over");
	}
}
