using UnityEngine;

public class SHSMySquadProgressMeterSmall : SHSMySquadChallengeProgressMeter
{
	protected override void InitializeMeter()
	{
		meterWidth = 220;
		meterHeight = 27;
		yOffset = 3;
		xOffset = 6;
		SetSize(new Vector2(250f, 32f));
		base.Docking = DockingAlignmentEnum.MiddleLeft;
		base.Anchor = AnchorAlignmentEnum.MiddleLeft;
		GUISimpleControlWindow gUISimpleControlWindow = new GUISimpleControlWindow();
		gUISimpleControlWindow.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(13f, 0f), new Vector2(meterWidth, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIImage gUIImage = new GUIImage();
		gUIImage.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(meterWidth, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage.TextureSource = "mysquadgadget_bundle|current_challenge_progress_bar_empty";
		gUISimpleControlWindow.Add(gUIImage);
		leftEnd = new GUIImage();
		leftEnd.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(6f, 3f), new Vector2(11f, 27f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		leftEnd.TextureSource = "mysquadgadget_bundle|current_challenge_progress_bar_leftside";
		gUISimpleControlWindow.Add(leftEnd);
		meterBar = new GUIImage();
		meterBar.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(17f, 3f), new Vector2(263f, 27f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		meterBar.TextureSource = "mysquadgadget_bundle|current_challenge_progress_bar_middle";
		gUISimpleControlWindow.Add(meterBar);
		rightEnd = new GUIImage();
		rightEnd.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(280f, 3f), new Vector2(11f, 27f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		rightEnd.TextureSource = "mysquadgadget_bundle|current_challenge_progress_bar_rightside";
		gUISimpleControlWindow.Add(rightEnd);
		GUIDropShadowTextLabel gUIDropShadowTextLabel = new GUIDropShadowTextLabel();
		gUIDropShadowTextLabel.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(40f, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIDropShadowTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(26, 64, 2), new Vector2(1f, 1f), TextAnchor.MiddleCenter);
		gUIDropShadowTextLabel.Text = "100%";
		meterLabel = gUIDropShadowTextLabel;
		gUISimpleControlWindow.Add(meterLabel);
		GUIDropShadowTextLabel gUIDropShadowTextLabel2 = new GUIDropShadowTextLabel();
		gUIDropShadowTextLabel2.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(140f, 0f), new Vector2(55f, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIDropShadowTextLabel2.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(127, 125, 30), new Vector2(1f, 1f), TextAnchor.MiddleCenter);
		gUIDropShadowTextLabel2.Text = "#LOADING";
		loadingLabel = gUIDropShadowTextLabel2;
		gUISimpleControlWindow.Add(loadingLabel);
		leftEnd.IsVisible = false;
		meterBar.IsVisible = false;
		rightEnd.IsVisible = false;
		meterLabel.IsVisible = false;
		Add(gUISimpleControlWindow);
	}
}
