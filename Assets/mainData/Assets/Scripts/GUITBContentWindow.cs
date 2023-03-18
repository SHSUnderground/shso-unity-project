using UnityEngine;

public class GUITBContentWindow : GUIControlWindow
{
	public enum Sizes
	{
		Centered_Half,
		Centered_Full,
		Large_Half,
		Large_Full,
		FullScreen_HalfHeight,
		FullScreen_HalfWidth,
		FullScreen_Full,
		FullScreen_SkinnyTall,
		FullScreen_QuarterHalf
	}

	public enum Dimensions
	{
		Size435x217,
		Size435x433,
		Size435x293,
		Size435x585,
		Size965x293,
		Size965x585,
		Size218x585,
		Size248x384
	}

	public enum Location
	{
		Right,
		Left,
		Top,
		Bottom,
		Centered
	}

	private int x;

	private int y;

	public GUITBContentWindow(Sizes size, Location loc)
	{
		SetPositionOffOfLocation(loc);
		GUIImage gUIImage = new GUIImage();
		x = 1;
		y = 1;
		switch (size)
		{
		case Sizes.Centered_Half:
			x = 435;
			y = 217;
			gUIImage.TextureSource = "toolbox_bundle|contentField_435x217";
			break;
		case Sizes.Centered_Full:
			x = 435;
			y = 433;
			gUIImage.TextureSource = "toolbox_bundle|contentField_435x433";
			break;
		case Sizes.Large_Half:
			x = 435;
			y = 293;
			gUIImage.TextureSource = "toolbox_bundle|contentField_435x293";
			break;
		case Sizes.Large_Full:
			x = 435;
			y = 585;
			gUIImage.TextureSource = "toolbox_bundle|contentField_435x585";
			break;
		case Sizes.FullScreen_HalfHeight:
			x = 965;
			y = 293;
			gUIImage.TextureSource = "toolbox_bundle|contentField_965x293";
			break;
		case Sizes.FullScreen_HalfWidth:
			x = 435;
			y = 585;
			gUIImage.TextureSource = "toolbox_bundle|contentField_435x585";
			break;
		case Sizes.FullScreen_Full:
			x = 965;
			y = 585;
			gUIImage.TextureSource = "toolbox_bundle|contentField_965x585";
			break;
		case Sizes.FullScreen_SkinnyTall:
			x = 218;
			y = 585;
			gUIImage.TextureSource = "toolbox_bundle|contentField_218x585";
			break;
		case Sizes.FullScreen_QuarterHalf:
			x = 248;
			y = 384;
			gUIImage.TextureSource = "toolbox_bundle|contentField_248x384";
			break;
		}
		gUIImage.SetSize(x, y);
		gUIImage.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
		Add(gUIImage);
		SetSize(x, y);
	}

	public GUITBContentWindow(Dimensions dim, Location loc)
		: this(ConvertDimToSizes(dim), loc)
	{
	}

	private static Sizes ConvertDimToSizes(Dimensions dim)
	{
		switch (dim)
		{
		case Dimensions.Size435x217:
			return Sizes.Centered_Half;
		case Dimensions.Size435x433:
			return Sizes.Centered_Full;
		case Dimensions.Size435x293:
			return Sizes.Large_Half;
		case Dimensions.Size435x585:
			return Sizes.Large_Full;
		case Dimensions.Size965x293:
			return Sizes.FullScreen_HalfHeight;
		case Dimensions.Size965x585:
			return Sizes.FullScreen_Full;
		case Dimensions.Size218x585:
			return Sizes.FullScreen_SkinnyTall;
		case Dimensions.Size248x384:
			return Sizes.FullScreen_QuarterHalf;
		default:
			return Sizes.FullScreen_Full;
		}
	}

	private void SetPositionOffOfLocation(Location loc)
	{
		switch (loc)
		{
		case Location.Centered:
			SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
			break;
		case Location.Bottom:
			SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -10f));
			break;
		case Location.Left:
			SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(10f, 0f));
			break;
		case Location.Right:
			SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(-10f, 0f));
			break;
		case Location.Top:
			SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(0f, 10f));
			break;
		}
	}

	public override void Add(IGUIControl Control)
	{
		if (Control is GUITBInterface)
		{
			((GUITBInterface)Control).AutoSize(x, y);
		}
		base.Add(Control);
	}
}
