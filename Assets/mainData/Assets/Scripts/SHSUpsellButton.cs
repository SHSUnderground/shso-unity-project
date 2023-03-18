using UnityEngine;

public class SHSUpsellButton : GUIControlWindow
{
	public Vector2 NormalUV = Vector2.zero;

	public Vector2 HighlightUV = new Vector2(0f, -120f);

	public GUIImage BackgroundImage;

	public GUIImage OverlayPriceImage;

	public override bool IsEnabled
	{
		get
		{
			return base.IsEnabled;
		}
		set
		{
			base.IsEnabled = value;
			BackgroundImage.IsEnabled = value;
			OverlayPriceImage.IsEnabled = value;
		}
	}

	public SHSUpsellButton()
	{
		Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Cached;
		BackgroundImage = new GUIImage();
		BackgroundImage.SetPositionAndSize(NormalUV, new Vector2(178f, 226f));
		Add(BackgroundImage);
		OverlayPriceImage = new GUIImage();
		OverlayPriceImage.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
		OverlayPriceImage.AutoSizeToTexture = true;
		Add(OverlayPriceImage);
		MouseOver += SHSUpsellButton_MouseOver;
		MouseOut += SHSUpsellButton_MouseOut;
	}

	private void SHSUpsellButton_MouseOut(GUIControl sender, GUIMouseEvent EventData)
	{
		BackgroundImage.SetPosition(NormalUV);
	}

	private void SHSUpsellButton_MouseOver(GUIControl sender, GUIMouseEvent EventData)
	{
		BackgroundImage.SetPosition(HighlightUV);
	}
}
