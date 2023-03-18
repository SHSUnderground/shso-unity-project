using UnityEngine;

public class SHSZoneChooserSlider : GUISlider
{
	public SHSZoneChooserSlider()
	{
		base.ArrowsEnabled = true;
		base.Orientation = SliderOrientationEnum.Horizontal;
		base.ScrollButtonStyleInfo = new SHSButtonStyleInfo("shopping_bundle|shopping_pagetracker_bobble", SHSButtonStyleInfo.SizeCategoryEnum.Small);
		base.ScrollButtonSize = new Vector2(128f, 128f);
		base.ScrollButtonHitTestSize = new Vector2(0.76f, 0.28f);
		base.ScrollButtonHitTestType = HitTestTypeEnum.Rect;
		base.ConsrainToMaxAndMin = false;
	}

	public void SetTooltipForArrows()
	{
		StartArrow.ToolTip = new NamedToolTipInfo("#zoneselector_arrows");
		EndArrow.ToolTip = new NamedToolTipInfo("#zoneselector_arrows");
		ThumbButton.ToolTip = new NamedToolTipInfo("#zoneselector_thumb");
	}
}
