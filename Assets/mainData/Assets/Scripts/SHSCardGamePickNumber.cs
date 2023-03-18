using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSCardGamePickNumber : GUIDynamicWindow
{
	private Action<int> onNumberSelected_;

	public SHSCardGamePickNumber(int minNumber, int maxNumber, Action<int> onNumberSelected)
	{
		onNumberSelected_ = onNumberSelected;
		SetSize(new Vector2(530f, 169f));
		SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
		GUIImage gUIImage = new GUIImage();
		gUIImage.TextureSource = "cardgame_bundle|mshs_cg_hand_container";
		gUIImage.SetSize(new Vector2(530f, 169f));
		gUIImage.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
		Add(gUIImage);
		List<Vector2> selectedGlowPath = SHSGlowOutlineWindow.GenerateCircularPath(22f, 16);
		SHSGlowOutlineWindow glowWindow = new SHSGlowOutlineWindow(selectedGlowPath, "cardgame_bundle|mshs_cg_dot_p", 2, 0.4f, 0.75f);
		glowWindow.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(60f, 60f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		glowWindow.HighlightSpeed = 85f;
		List<GUIAnimatedButton> list = new List<GUIAnimatedButton>();
		int num = 0;
		float num2 = 47f;
		float num3 = 55f;
		for (int i = minNumber; i <= maxNumber; i++)
		{
			int num4 = num % 10;
			int num5 = num / 10;
			GUIAnimatedButton numberButton = new GUIAnimatedButton();
			numberButton.Id = "numberButton_" + i.ToString();
			numberButton.TextureSource = "cardgame_bundle|mshs_cg_number_" + i.ToString();
			numberButton.SetSize(new Vector2(48f, 48f));
			numberButton.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(num2 + (float)(num4 * 48), num3 + (float)(num5 * 54)));
			numberButton.SetupButton(1f, 1.05f, 0.95f);
			numberButton.HitTestType = HitTestTypeEnum.Circular;
			numberButton.HitTestSize = new Vector2(0.85f, 0.85f);
			int numberSelected = i;
			numberButton.Click += delegate
			{
				if (onNumberSelected_ != null)
				{
					onNumberSelected_(numberSelected);
				}
				Hide();
			};
			numberButton.MouseOver += delegate
			{
				glowWindow.IsVisible = true;
				glowWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, numberButton.Offset);
			};
			numberButton.MouseOut += delegate
			{
				glowWindow.IsVisible = false;
			};
			list.Add(numberButton);
			Add(numberButton);
			num++;
		}
		Add(glowWindow);
		glowWindow.Highlight(true);
		glowWindow.IsVisible = false;
	}
}
