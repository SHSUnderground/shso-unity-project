using System.Runtime.CompilerServices;
using UnityEngine;

public class GUIStrokeTextButton : GUIControlWindow
{
	[CompilerGenerated]
	private GUIButton _003CButton_003Ek__BackingField;

	[CompilerGenerated]
	private GUIStrokeTextLabel _003CButtonLabel_003Ek__BackingField;

	public override bool IsEnabled
	{
		get
		{
			return base.IsEnabled;
		}
		set
		{
			base.IsEnabled = value;
			foreach (IGUIControl control in ControlList)
			{
				control.IsEnabled = value;
			}
		}
	}

	public GUIButton Button
	{
		[CompilerGenerated]
		get
		{
			return _003CButton_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CButton_003Ek__BackingField = value;
		}
	}

	public GUIStrokeTextLabel ButtonLabel
	{
		[CompilerGenerated]
		get
		{
			return _003CButtonLabel_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CButtonLabel_003Ek__BackingField = value;
		}
	}

	public GUIStrokeTextButton(string buttonTextureSource, Vector2 buttonSize, Vector2 buttonOffset, string label, Vector2 labelSize, Vector2 labelOffset)
	{
		Button = GUIControl.CreateControlFrameCentered<GUIButton>(buttonSize, buttonOffset);
		Button.StyleInfo = new SHSButtonStyleInfo(buttonTextureSource);
		Button.HitTestType = HitTestTypeEnum.Alpha;
		Button.Click += Button_Click;
		Add(Button);
		ButtonLabel = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(labelSize, labelOffset);
		ButtonLabel.Text = label;
		Add(ButtonLabel);
		HitTestType = HitTestTypeEnum.Transparent;
	}

	private void Button_Click(GUIControl sender, GUIClickEvent EventData)
	{
		FireMouseClick(EventData);
	}
}
