using UnityEngine;

public class SHSHelpTextWindow : GUIVisualCueWindow
{
	private GUIBox bgdBox;

	private GUILabel textLabel;

	protected string text;

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
			GUILabel control = GetControl<GUILabel>("textLabel");
			if (control != null)
			{
				control.Text = text;
			}
		}
	}

	public override bool InitializeResources(bool reload)
	{
		bgdBox = new GUIBox();
		bgdBox.Color = Color.red;
		bgdBox.SetPositionAndSize(QuickSizingHint.ParentSize);
		Add(bgdBox);
		textLabel = new GUILabel();
		textLabel.Id = "textLabel";
		textLabel.SetPositionAndSize(QuickSizingHint.ParentSize);
		textLabel.TextAlignment = TextAnchor.MiddleCenter;
		textLabel.FontSize = 20;
		textLabel.Text = text;
		Add(textLabel);
		return base.InitializeResources(reload);
	}
}
