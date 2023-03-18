using UnityEngine;

public class SHSMySquadChallengeCheckbox : GUIControlWindow
{
	protected GUIDropShadowTextLabel labelTextBox;

	protected GUIImage box;

	protected GUIImage checkMark;

	public string Label
	{
		set
		{
			labelTextBox.Text = value;
		}
	}

	public bool Checked
	{
		get
		{
			return checkMark.IsVisible;
		}
		set
		{
			checkMark.IsVisible = value;
		}
	}

	public SHSMySquadChallengeCheckbox(string label, string tooltip)
	{
		if (label == null)
		{
			SetSize(46f, 46f);
		}
		else
		{
			SetSize(new Vector2(200f, 46f));
		}
		box = new GUIImage();
		box.Id = "box";
		box.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(46f, 46f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		box.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_p1_checkbox";
		Add(box);
		checkMark = new GUIImage();
		checkMark.Id = "checkMark";
		checkMark.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(22f, 0f), new Vector2(46f, 46f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		checkMark.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_p1_checkmark";
		Add(checkMark);
		labelTextBox = new GUIDropShadowTextLabel();
		labelTextBox.Id = "label";
		labelTextBox.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		labelTextBox.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(5, 33, 83), new Vector2(1f, 2f), TextAnchor.MiddleLeft);
		labelTextBox.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(40f, 0f), new Vector2(100f, 30f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		if (!string.IsNullOrEmpty(label))
		{
			labelTextBox.Text = label;
			Add(labelTextBox);
		}
		if (!string.IsNullOrEmpty(tooltip))
		{
			toolTip = new NamedToolTipInfo(tooltip);
			HitTestType = HitTestTypeEnum.Rect;
		}
	}

	public void ConfigureForSingleDisplay()
	{
		SetSize(300f, 130f);
		SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
		Offset = new Vector2(15f, 0f);
		labelTextBox.SetSize(200f, 130f);
		labelTextBox.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
		labelTextBox.Offset = new Vector2(43f, 4f);
		labelTextBox.TextAlignment = TextAnchor.UpperLeft;
		box.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
		box.Offset = new Vector2(0f, -9f);
		checkMark.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
		checkMark.Offset = new Vector2(0f, -9f);
	}
}
