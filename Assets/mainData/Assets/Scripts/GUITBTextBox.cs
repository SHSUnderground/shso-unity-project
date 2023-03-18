using UnityEngine;

public class GUITBTextBox : GUIControlWindow, GUITBInterface
{
	private GUILabel label;

	public string Text = string.Empty;

	public GUITBTextBox()
	{
		label = new GUILabel();
		label.SetPosition(20f, 20f);
		label.Text = Text;
		label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(0, 0, 0), TextAnchor.MiddleCenter);
		Add(label);
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		label.Text = Text;
		base.Draw(drawFlags);
	}

	public new void AutoSize(float x, float y)
	{
		SetSize(x, y);
		SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
		label.SetSize(x - 40f, y - 40f);
	}
}
