using UnityEngine;

public class SHSMySquadRewardTextbox : GUISimpleControlWindow
{
	protected GUIImage background;

	protected GUIDropShadowTextLabel label;

	public string Text
	{
		set
		{
			label.Text = value;
		}
	}

	public SHSMySquadRewardTextbox()
	{
		background = new GUIImage();
		background.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(112f, 42f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		background.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_p1_reward_amount_frame";
		Add(background);
		label = new GUIDropShadowTextLabel();
		label.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(112f, 42f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 20, 54), new Vector2(0f, 1f), TextAnchor.MiddleCenter);
		Add(label);
	}
}
