using UnityEngine;

public class GUIVerticalBar : GUISimpleControlWindow
{
	protected const float MIN_TEXT_DISPLAY_HEIGHT = 4f;

	protected GUIImage top;

	protected GUIImage fill;

	protected GUIImage bottom;

	protected GUILabel textLabel;

	protected float maxHeight = 33f;

	protected float percentageOfMax = 1f;

	public float Percent
	{
		set
		{
			percentageOfMax = value;
			if (percentageOfMax <= 0f)
			{
				IsVisible = false;
				return;
			}
			percentageOfMax = Mathf.Min(percentageOfMax, 1f);
			IsVisible = true;
			if (fill != null)
			{
				GUIImage gUIImage = fill;
				Vector2 size = fill.Size;
				gUIImage.SetSize(size.x, maxHeight * percentageOfMax);
				GUIImage gUIImage2 = top;
				Vector2 size2 = fill.Size;
				float y = size2.y;
				Vector2 size3 = bottom.Size;
				gUIImage2.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, 0f - (y + size3.y)), new Vector2(20f, 8f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				GUILabel gUILabel = textLabel;
				Vector2 size4 = fill.Size;
				gUILabel.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(-2f, 0f - (size4.y - 1f)), new Vector2(20f, 10f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			}
			if (textLabel != null)
			{
				if (maxHeight * percentageOfMax < 4f)
				{
					textLabel.IsVisible = false;
				}
				else
				{
					textLabel.IsVisible = true;
				}
			}
		}
	}

	public float MaxHeight
	{
		set
		{
			maxHeight = value;
		}
	}

	public string Text
	{
		set
		{
			if (textLabel != null)
			{
				textLabel.Text = value;
			}
		}
	}

	public GUIVerticalBar()
	{
		Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
	}

	public override bool InitializeResources(bool reload)
	{
		bottom = new GUIImage();
		bottom.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(18f, 4f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		bottom.TextureSource = "deckbuilder_bundle|deckbuilder_level_bar_bottom";
		Add(bottom);
		fill = new GUIImage();
		fill.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -4f), new Vector2(20f, percentageOfMax * maxHeight), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		fill.TextureSource = "deckbuilder_bundle|deckbuilder_level_bar_fill";
		Add(fill);
		top = new GUIImage();
		GUIImage gUIImage = top;
		Vector2 size = fill.Size;
		float y = size.y;
		Vector2 size2 = bottom.Size;
		gUIImage.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, 0f - (y + size2.y)), new Vector2(20f, 8f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		top.TextureSource = "deckbuilder_bundle|deckbuilder_level_bar_top";
		Add(top);
		textLabel = new GUILabel();
		GUILabel gUILabel = textLabel;
		Vector2 size3 = fill.Size;
		gUILabel.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(-2f, 0f - (size3.y + 10f)), new Vector2(20f, 10f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		textLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(29, 87, 202), TextAnchor.MiddleCenter);
		textLabel.Bold = true;
		Add(textLabel);
		return true;
	}
}
