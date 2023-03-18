public class GUITBBlankWindow : GUIControlWindow
{
	public enum Sizes
	{
		Centered,
		Large,
		FullScreen
	}

	public enum Dimensions
	{
		Size453x458,
		Size453x609,
		Size983x609
	}

	public GUITBBlankWindow(Sizes size)
	{
		SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
		GUIImage gUIImage = new GUIImage();
		int num = 1;
		int num2 = 1;
		switch (size)
		{
		case Sizes.Centered:
			num = 453;
			num2 = 458;
			gUIImage.TextureSource = "toolbox_bundle|stageBG_453x458";
			break;
		case Sizes.Large:
			num = 453;
			num2 = 609;
			gUIImage.TextureSource = "toolbox_bundle|stageBG_453x609";
			break;
		case Sizes.FullScreen:
			num = 983;
			num2 = 609;
			gUIImage.TextureSource = "toolbox_bundle|stageBG_983x609";
			break;
		}
		gUIImage.SetSize(num, num2);
		gUIImage.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
		Add(gUIImage);
		SetSize(num, num2);
	}

	public GUITBBlankWindow(Dimensions dim)
		: this(ConvertDimToSizes(dim))
	{
	}

	private static Sizes ConvertDimToSizes(Dimensions dim)
	{
		switch (dim)
		{
		case Dimensions.Size453x458:
			return Sizes.Centered;
		case Dimensions.Size453x609:
			return Sizes.Large;
		case Dimensions.Size983x609:
			return Sizes.FullScreen;
		default:
			return Sizes.FullScreen;
		}
	}
}
