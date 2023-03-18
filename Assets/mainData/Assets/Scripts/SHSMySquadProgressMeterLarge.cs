using UnityEngine;

public class SHSMySquadProgressMeterLarge : SHSMySquadChallengeProgressMeter
{
	protected override void InitializeMeter()
	{
		meterWidth = 285;
		meterHeight = 26;
		yOffset = 3;
		xOffset = 6;
		SetSize(new Vector2(meterWidth, 32f));
		base.Docking = DockingAlignmentEnum.MiddleLeft;
		base.Anchor = AnchorAlignmentEnum.BottomLeft;
		GUISimpleControlWindow gUISimpleControlWindow = new GUISimpleControlWindow();
		gUISimpleControlWindow.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(meterWidth, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIImage gUIImage = new GUIImage();
		gUIImage.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(meterWidth, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_p1_challenge_meter_base";
		gUISimpleControlWindow.Add(gUIImage);
		leftEnd = new GUIImage();
		leftEnd.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(6f, 3f), new Vector2(11f, 26f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		leftEnd.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_p1_challenge_meter_left";
		gUISimpleControlWindow.Add(leftEnd);
		meterBar = new GUIImage();
		meterBar.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(17f, 3f), new Vector2(263f, 26f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		meterBar.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_p1_challenge_meter_fill";
		gUISimpleControlWindow.Add(meterBar);
		rightEnd = new GUIImage();
		rightEnd.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(280f, 3f), new Vector2(11f, 26f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		rightEnd.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_p1_challenge_meter_right";
		gUISimpleControlWindow.Add(rightEnd);
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(40f, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 19, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(26, 64, 2), GUILabel.GenColor(26, 64, 2), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel.Text = "#LOADING";
		meterLabel = gUIStrokeTextLabel;
		gUISimpleControlWindow.Add(meterLabel);
		GUIStrokeTextLabel gUIStrokeTextLabel2 = new GUIStrokeTextLabel();
		gUIStrokeTextLabel2.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(110f, 0f), new Vector2(100f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel2.SetupText(GUIFontManager.SupportedFontEnum.Komica, 19, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(26, 64, 2), GUILabel.GenColor(26, 64, 2), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel2.Text = "#LOADING";
		loadingLabel = gUIStrokeTextLabel2;
		gUISimpleControlWindow.Add(loadingLabel);
		GUIImage gUIImage2 = new GUIImage();
		gUIImage2.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(meterWidth, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage2.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_p1_challenge_meter_overlay";
		gUISimpleControlWindow.Add(gUIImage2);
		leftEnd.IsVisible = false;
		meterBar.IsVisible = false;
		rightEnd.IsVisible = false;
		meterLabel.IsVisible = false;
		Add(gUISimpleControlWindow);
	}
}
