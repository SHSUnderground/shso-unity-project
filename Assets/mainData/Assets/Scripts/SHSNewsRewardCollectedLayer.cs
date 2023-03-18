using UnityEngine;

public class SHSNewsRewardCollectedLayer : SHSNewsRewardDayLayer
{
	public SHSNewsRewardCollectedLayer(string rewardCollectedTextureSource, string rewardCollectedLabel)
	{
		GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(84f, 94f), Vector2.zero);
		gUIImage.TextureSource = rewardCollectedTextureSource;
		layerControls.Add(gUIImage);
		GUILabel gUILabel = GUIControl.CreateControlBottomFrame<GUILabel>(new Vector2(84f, 92f), new Vector2(0f, -10f));
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 14, GUILabel.GenColor(179, 231, 100), TextAnchor.MiddleCenter);
		gUILabel.Text = rewardCollectedLabel;
		gUILabel.Rotation = -27f;
		layerControls.Add(gUILabel);
	}

	public override void OnMouseOver(AnimClipManager clipManager)
	{
		base.OnMouseOver(clipManager);
		clipManager.SwapOut(ref layerAnimation, SHSAnimations.Generic.FadeOut(layerControls, 0.2f));
	}

	public override void OnMouseOut(AnimClipManager clipManager)
	{
		base.OnMouseOut(clipManager);
		clipManager.SwapOut(ref layerAnimation, SHSAnimations.Generic.FadeIn(layerControls, 0.2f));
	}
}
