using System;
using UnityEngine;

public class SlideArcadeItem : GUISubScalingWindow
{
	private class MoveOffset : SHSAnimations
	{
		public static Vector2 GetOffset(float value, float abs)
		{
			return new Vector2(GetXOffset(value), GetYOffset(abs));
		}

		private static float GetXOffset(float value)
		{
			float num = Mathf.Abs(value);
			float num2 = Mathf.Sign(value);
			if (num < 1f)
			{
				return Extrapolate(0f, 182f, num) * num2;
			}
			if (num < 2f)
			{
				return Extrapolate(182f, 295f, num - 1f) * num2;
			}
			if (num < 3f)
			{
				return Extrapolate(295f, 400f, num - 2f) * num2;
			}
			return Extrapolate(400f, 500f, num - 3f) * num2;
		}

		private static float GetYOffset(float value)
		{
			if (value < 1f)
			{
				return Extrapolate(-11f, -28f, value);
			}
			if (value < 2f)
			{
				return Extrapolate(-28f, -32f, value - 1f);
			}
			if (value < 3f)
			{
				return Extrapolate(-32f, -34f, value - 2f);
			}
			return Extrapolate(-34f, -35f, value - 3f);
		}

		private static float Extrapolate(float start, float end, float perc)
		{
			return start * (1f - perc) + end * perc;
		}
	}

	private readonly SHSArcadeSliderWindow headWindow;

	private ArcadeGame game;

	public ArcadeGame Game
	{
		get
		{
			return game;
		}
	}

	public SlideArcadeItem(SHSArcadeSliderWindow headWindow, ArcadeGame game)
		: base(SHSArcadeSliderWindow.SLIDE_ITEM_SIZE)
	{
		this.headWindow = headWindow;
		this.game = game;
		SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -12f));
		Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		Traits.VisibleAncestryTrait = ControlTraits.VisibilityAncestryTraitEnum.DetachedVisibility;
		GUIHotSpotButton gUIHotSpotButton = GUIControl.CreateControlBottomFrame<GUIHotSpotButton>(SHSArcadeSliderWindow.SLIDE_ITEM_SIZE, new Vector2(0f, 0f));
		gUIHotSpotButton.Click += ClickMeClick;
		AddItem(gUIHotSpotButton);
		GUIImage gUIImage = GUIControl.CreateControlTopFrame<GUIImage>(new Vector2(256f, 256f), new Vector2(0f, 0f));
		gUIImage.Docking = DockingAlignmentEnum.Middle;
		gUIImage.Anchor = AnchorAlignmentEnum.Middle;
		gUIImage.TextureSource = string.Format("arcade_bundle|{0}", game.DisplayImage);
		gUIImage.IsVisible = true;
		gUIImage.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		gUIImage.Click += delegate
		{
		};
		AddItem(gUIImage);
	}

	private void ClickMeClick(GUIControl sender, GUIClickEvent EventData)
	{
		headWindow.SlideItemClick(this);
	}

	public void Move(float value, float fullSizeMod, SHSArcadeSliderWindow headWindow)
	{
		float num = Mathf.Abs(value);
		if (num < 4f)
		{
			IsVisible = true;
			float num2 = (!(num < 1f)) ? (0.7f - num * 0.1f) : (0.8f + Mathf.Cos(num * (float)Math.PI) * 0.2f);
			Offset = MoveOffset.GetOffset(value, num);
			num2 *= Mathf.Pow(fullSizeMod, num + 1f);
			SetSize(SHSArcadeSliderWindow.SLIDE_ITEM_SIZE * num2);
			Alpha = Mathf.Clamp01(4f - num) * headWindow.Alpha;
		}
		else
		{
			IsVisible = false;
		}
	}
}
