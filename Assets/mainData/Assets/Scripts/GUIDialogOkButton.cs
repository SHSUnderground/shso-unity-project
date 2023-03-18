public class GUIDialogOkButton : GUIButton
{
	public GUIDialogOkButton()
	{
		Click += GUIDialogOkButton_Click;
		Text = "#gui_ok";
	}

	private void GUIDialogOkButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		GUIDialogWindow gUIDialogWindow = FindParentDialog();
		if (gUIDialogWindow != null)
		{
			gUIDialogWindow.OnOk();
		}
		else
		{
			CspUtils.DebugLog("Button: " + sender.Id + " does not have a dialog window as a parent to report to on its click event.");
		}
	}

	private GUIDialogWindow FindParentDialog()
	{
		bool flag = false;
		IGUIContainer parent = Parent;
		while (parent != null && !parent.IsRoot)
		{
			if (parent is GUIDialogWindow)
			{
				flag = true;
				break;
			}
			parent = parent.Parent;
		}
		return (!flag) ? null : ((GUIDialogWindow)parent);
	}
}
