using UnityEngine;

public class SHSArcadePlayInstructions : GUIChildWindow
{
	private GUIStrokeTextLabel instructionsTitle;

	private GUIImage instructionsImage;

	private GUILabel instructionsLabel;

	private AnimClip fadeAnimImage;

	private AnimClip fadeAnimText;

	public SHSArcadePlayInstructions()
	{
		instructionsTitle = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(216f, 35f), new Vector2(0f, 19f));
		instructionsTitle.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 33, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 114, 255), GUILabel.GenColor(22, 77, 195), new Vector2(4f, 3f), TextAnchor.MiddleCenter);
		instructionsTitle.Text = "#ARCADE_HOWTOPLAY_TITLE";
		instructionsTitle.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		Add(instructionsTitle);
		instructionsImage = GUIControl.CreateControlTopFrameCentered<GUIImage>(new Vector2(207f, 99f), new Vector2(0f, 80f));
		instructionsImage.TextureSource = string.Empty;
		Add(instructionsImage);
		instructionsLabel = GUIControl.CreateControlTopFrame<GUILabel>(new Vector2(200f, 225f), new Vector2(0f, 125f));
		instructionsLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, ColorUtil.FromRGB255(55, 53, 15), TextAnchor.UpperLeft);
		instructionsLabel.Text = string.Empty;
		Add(instructionsLabel);
	}

	public void UpdateInstructions(string instructionImage, string instructionText)
	{
		instructionsImage.TextureSource = string.Format("arcade_bundle|{0}", instructionImage);
		instructionsLabel.Text = instructionText;
	}

	public void FadeIn()
	{
		base.AnimationPieceManager.SwapOut(ref fadeAnimImage, SHSAnimations.Generic.FadeInVis(instructionsImage, 0.2f));
		base.AnimationPieceManager.SwapOut(ref fadeAnimText, SHSAnimations.Generic.FadeInVis(instructionsLabel, 0.2f));
	}

	public void FadeOut()
	{
		base.AnimationPieceManager.SwapOut(ref fadeAnimImage, SHSAnimations.Generic.FadeOutVis(instructionsImage, 0.2f));
		base.AnimationPieceManager.SwapOut(ref fadeAnimText, SHSAnimations.Generic.FadeOutVis(instructionsLabel, 0.2f));
	}
}
