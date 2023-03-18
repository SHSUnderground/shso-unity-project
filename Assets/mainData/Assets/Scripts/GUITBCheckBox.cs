using UnityEngine;

public class GUITBCheckBox : GUIControlWindow
{
	private GUILabel label;

	private GUIButton box;

	public string Text;

	public bool Selected;

	public GUITBCheckBox()
		: this(string.Empty)
	{
	}

	public GUITBCheckBox(string data)
	{
		Text = data;
		int num = 200;
		int num2 = 42;
		SetSize(num, num2);
		GUIImage gUIImage = new GUIImage();
		gUIImage.SetPositionAndSize(0f, 0f, num, num2);
		gUIImage.TextureSource = "toolbox_bundle|stageBG_453x458";
		Add(gUIImage);
		box = new GUIButton();
		box.SetPosition(5f, 5f);
		box.SetSize(32f, 32f);
		box.StyleInfo = new SHSButtonStyleInfo("toolbox_bundle|checkbox");
		Add(box);
		label = new GUILabel();
		label.SetPositionAndSize(42f, 5f, num - 52, num2 - 10);
		label.Text = data;
		label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, Color.black, TextAnchor.MiddleCenter);
		Add(label);
		GUILabel gUILabel = new GUILabel();
		gUILabel.SetPosition(0f, 0f);
		gUILabel.SetSize(num, num2);
		gUILabel.Click += delegate
		{
			toggleSelected();
		};
		gUILabel.Alpha = 0f;
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, Color.black, TextAnchor.MiddleCenter);
		Add(gUILabel);
	}

	private void toggleSelected()
	{
		Selected = !Selected;
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		box.IsSelected = Selected;
		label.Text = Text;
		base.Draw(drawFlags);
	}
}
