using UnityEngine;

public class IconBar : GUISimpleControlWindow
{
	protected static readonly Vector2 LockIconSize = new Vector2(32f, 35f);

	public IconBar()
	{
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		GUIImage gUIImage = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(279f, 36f), new Vector2(10f, 11f));
		gUIImage.TextureSource = "mysquadgadget_bundle|generic_bar_short_bg";
		Add(gUIImage);
	}

	public GUIDrawTexture CreateIcon(Vector2 size, Vector2 offset)
	{
		GUIDrawTexture gUIDrawTexture = GUIControl.CreateControlAbsolute<GUIDrawTexture>(size, offset);
		gUIDrawTexture.Traits.ActivationTrait = ControlTraits.ActivationTraitEnum.Manual;
		gUIDrawTexture.HitTestType = HitTestTypeEnum.Circular;
		Add(gUIDrawTexture);
		return gUIDrawTexture;
	}

	public GUIDrawTexture CreateIcon(Vector2 size, Vector2 offset, string source)
	{
		GUIDrawTexture gUIDrawTexture = CreateIcon(size, offset);
		SetIconTexture(gUIDrawTexture, "mysquadgadget_bundle", source);
		return gUIDrawTexture;
	}

	public GUIDrawTexture CreateLockIcon(Vector2 size, Vector2 offset, string tooltip)
	{
		GUIDrawTexture gUIDrawTexture = CreateIcon(size, offset, "mysquad_gadget_charactericon_lock");
		gUIDrawTexture.HitTestSize = new Vector2(0.875f, 0.875f);
		gUIDrawTexture.IsVisible = true;
		CreateLockToolTip(gUIDrawTexture, tooltip);
		return gUIDrawTexture;
	}

	public void CreateLockToolTip(GUIDrawTexture lockIcon, string tooltip)
	{
		lockIcon.ToolTip = new NamedToolTipInfo(tooltip, new Vector2(20f, 0f));
	}

	public void SetIconTexture(GUIDrawTexture icon, string bundle, string source)
	{
		icon.TextureSource = bundle + "|" + source;
	}
}
