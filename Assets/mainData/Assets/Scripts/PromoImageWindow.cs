using UnityEngine;

public class PromoImageWindow : GUIDynamicWindow
{
	private GUIImage bgImage;

	private GUIImage promoImage;

	private GUIButton okButton;

	private GUIButton playButton;

	private Texture2D _loadedTexture;

	public static PromoImageWindow instance;

	private bool firstFrame = true;

	private bool playRequested = true;

	public static bool alreadyPlaying
	{
		get
		{
			return instance != null;
		}
	}

	public PromoImageWindow(Texture2D tex)
	{
		instance = this;
		_loadedTexture = tex;
	}

	public override bool InitializeResources(bool reload)
	{
		CspUtils.DebugLog("promo 1");
		SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, -30f), new Vector2(648f, 503f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		bgImage = GUIControl.CreateControlTopLeftFrame<GUIImage>(new Vector2(648f, 463f), new Vector2(0f, 0f));
		bgImage.TextureSource = "persistent_bundle|promo_bg";
		Add(bgImage);
		CspUtils.DebugLog("promo 2");
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-10f, -205f), new Vector2(240f, 25f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 30, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 43, 68), GUILabel.GenColor(0, 43, 68), new Vector2(3f, 4f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel.BackColorAlpha = 1f;
		gUIStrokeTextLabel.StrokeColorAlpha = 1f;
		gUIStrokeTextLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUIStrokeTextLabel.Text = "NEWS";
		gUIStrokeTextLabel.IsVisible = true;
		Add(gUIStrokeTextLabel);
		CspUtils.DebugLog("promo 3");
		promoImage = new GUIImage();
		promoImage.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(70f, 102f), new Vector2(_loadedTexture.width, _loadedTexture.height), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		promoImage.Texture = _loadedTexture;
		Add(promoImage);
		promoImage.IsVisible = true;
		CspUtils.DebugLog("promo 4");
		okButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(0f, 216f));
		okButton.Click += delegate
		{
			Okay();
		};
		okButton.HitTestSize = new Vector2(0.5f, 0.5f);
		okButton.HitTestType = HitTestTypeEnum.Circular;
		okButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|L_mshs_button_ok");
		okButton.IsVisible = true;
		Add(okButton);
		CspUtils.DebugLog("promo 5");
		return base.InitializeResources(reload);
	}

	protected void Okay()
	{
		CspUtils.DebugLog("Okay");
		Hide();
		SetInactive();
		promoImage.Dispose();
		Dispose();
		instance = null;
	}
}
