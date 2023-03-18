using UnityEngine;

public class SHSPlayerHighScoreWindow : GUIChildWindow
{
	private GUIImage bgd;

	private GUIStrokeTextLabel highScoreTitle;

	private GUIDropShadowTextLabel scoreLabel;

	private AnimClip fadeAnim;

	public SHSPlayerHighScoreWindow()
	{
		bgd = new GUIImage();
		bgd.SetPositionAndSize(QuickSizingHint.ParentSize);
		bgd.TextureSource = "persistent_bundle|arcade_launcher_best_container";
		Add(bgd);
		highScoreTitle = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(325f, 31f), new Vector2(0f, 42f));
		highScoreTitle.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 31, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 114, 255), GUILabel.GenColor(0, 0, 0), new Vector2(4f, 3f), TextAnchor.MiddleCenter);
		highScoreTitle.Text = "#ARCADE_PERSONAL_BEST_TITLE";
		highScoreTitle.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		Add(highScoreTitle);
		scoreLabel = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(150f, 30f), new Vector2(0f, 7f));
		scoreLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 27, ColorUtil.FromRGB255(162, 255, 0), Color.black, new Vector2(1f, 1f), TextAnchor.MiddleCenter);
		scoreLabel.Text = string.Format("{0:#,0}", 0);
		Add(scoreLabel);
	}

	public void UpdateBestScore(int score)
	{
		scoreLabel.Text = string.Format("{0:#,0}", score);
	}

	public void FadeIn()
	{
		base.AnimationPieceManager.SwapOut(ref fadeAnim, SHSAnimations.Generic.FadeInVis(scoreLabel, 0.2f));
	}

	public void FadeOut()
	{
		base.AnimationPieceManager.SwapOut(ref fadeAnim, SHSAnimations.Generic.FadeOutVis(scoreLabel, 0.2f));
	}
}
