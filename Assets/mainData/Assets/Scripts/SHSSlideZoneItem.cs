using System;
using UnityEngine;

public class SHSSlideZoneItem : GUISubScalingWindow
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

	private readonly SHSZoneSelectorSliderWindow headWindow;

	private Zone zone;

	public GUIImage glow;

	public GUIAnimatedButton sidesButton;

	public GUIAnimatedButton mainButton;

	public GUIAnimatedButton nameButton;

	public GUIHotSpotButton ClickHotSpotButton;

	public Zone Zone
	{
		get
		{
			return zone;
		}
	}

	public SHSSlideZoneItem(SHSZoneSelectorSliderWindow headWindow, Zone zone)
		: base(SHSZoneSelectorSliderWindow.SLIDE_ITEM_SIZE)
	{
		this.headWindow = headWindow;
		this.zone = zone;
		SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f));
		Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		Traits.VisibleAncestryTrait = ControlTraits.VisibilityAncestryTraitEnum.DetachedVisibility;
		glow = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(512f, 512f), new Vector2(0f, 0f));
		glow.Alpha = 0f;
		glow.TextureSource = "zonechooser_bundle|zone_glow";
		AddItem(glow);
		GUIImage gUIImage = GUIControl.CreateControlTopFrame<GUIImage>(new Vector2(512f, 512f), new Vector2(0f, 0f));
		gUIImage.Docking = DockingAlignmentEnum.Middle;
		gUIImage.Anchor = AnchorAlignmentEnum.Middle;
		gUIImage.TextureSource = string.Format("zonechooser_bundle|zone_{0}", zone.zone);
		gUIImage.IsVisible = true;
		gUIImage.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		gUIImage.Click += delegate
		{
		};
		AddItem(gUIImage);
		GUIImage gUIImage2 = new GUIImage();
		gUIImage2.SetSize(zone.nameSize);
		gUIImage2.Offset = new Vector2(0f, 75f);
		gUIImage2.Docking = DockingAlignmentEnum.Middle;
		gUIImage2.Anchor = AnchorAlignmentEnum.Middle;
		gUIImage2.TextureSource = "zonechooser_bundle|L_zone_name_" + zone.zone;
		gUIImage2.Click += delegate
		{
		};
		AddItem(gUIImage2);
		ClickHotSpotButton = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(new Vector2(256f, 512f), new Vector2(0f, 0f));
		ClickHotSpotButton.Click += ClickMeClick;
		AddItem(ClickHotSpotButton);
		if (zone.contentReference != null)
		{
			ClickHotSpotButton.ConfigureRequiredContent(zone.contentReference);
		}
	}

	private void ClickMeClick(GUIControl sender, GUIClickEvent EventData)
	{
		headWindow.SlideItemClick(this);
	}

	public void Move(float value, float fullSizeMod, SHSZoneSelectorSliderWindow headWindow)
	{
		float num = Mathf.Abs(value);
		if (num < 4f)
		{
			IsVisible = true;
			float num2 = (!(num < 1f)) ? (0.7f - num * 0.1f) : (0.8f + Mathf.Cos(num * (float)Math.PI) * 0.2f);
			Offset = MoveOffset.GetOffset(value, num);
			Vector2 offset = Offset;
			Offset = new Vector2(offset.x * 1.6f, 0f);
			num2 *= Mathf.Pow(fullSizeMod, num + 1f);
			SetSize(SHSZoneSelectorSliderWindow.SLIDE_ITEM_SIZE * num2);
			Alpha = Mathf.Clamp01(1.5f - num) * headWindow.Alpha;
			glow.Alpha = (1f - num) * headWindow.Alpha;
		}
		else
		{
			IsVisible = false;
		}
	}
}
