using System.Runtime.CompilerServices;
using UnityEngine;

public class SHSUpsellWindow : GUIDialogWindow
{
	private readonly GUIImage backgroundImage;

	private readonly GUIButton backupButton;

	private readonly GUIButton closeButton;

	private readonly GUIImage iconBoy;

	private readonly GUIButton okButton;

	private readonly SHSUpsellButton option1Button;

	private readonly SHSUpsellButton option2Button;

	private readonly SHSUpsellButton option3Button;

	private readonly GUILabel recurwarningLabel;

	private readonly GUIDropShadowTextLabel sellBullet1;

	private readonly GUIDropShadowTextLabel sellBullet2;

	private readonly GUIDropShadowTextLabel sellBullet3;

	private readonly GUIDropShadowTextLabel titleLabel;

	[CompilerGenerated]
	private string _003CSubscriptionPlan_003Ek__BackingField;

	public string SubscriptionPlan
	{
		[CompilerGenerated]
		get
		{
			return _003CSubscriptionPlan_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CSubscriptionPlan_003Ek__BackingField = value;
		}
	}

	public SHSUpsellWindow()
	{
		backgroundImage = new GUIImage();
		backgroundImage.SetPosition(QuickSizingHint.Centered);
		backgroundImage.SetSize(new Vector2(594f, 412f));
		backgroundImage.TextureSource = "gameworld_bundle|subscription_notification_background_panel";
		backgroundImage.Offset = new Vector2(0f, 0f);
		Add(backgroundImage);
		titleLabel = new GUIDropShadowTextLabel();
		titleLabel.SetPositionAndSize(267f, 187f, 366f, 125f);
		titleLabel.FontFace = GUIFontManager.SupportedFontEnum.Zooom;
		titleLabel.FontSize = 37;
		titleLabel.TextAlignment = TextAnchor.UpperCenter;
		titleLabel.FrontColor = ColorUtil.FromRGB255(62, 40, 161);
		titleLabel.BackColor = new Color(0f, 4f / 51f, 14f / 51f, 0.1f);
		titleLabel.TextOffset = new Vector2(2f, 2f);
		titleLabel.Text = "#UPSELL_TITLE";
		titleLabel.VerticalKerning = 27;
		GUIStyle unityStyle = titleLabel.Style.UnityStyle;
		GUIContent content = new GUIContent(titleLabel.Text);
		Vector2 size = titleLabel.Size;
		float num = unityStyle.CalcHeight(content, size.x);
		Vector2 size2 = titleLabel.Size;
		if (num >= size2.y)
		{
			titleLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		}
		Add(titleLabel);
		sellBullet1 = new GUIDropShadowTextLabel();
		sellBullet1.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(250f, -77f), new Vector2(350f, 35f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		sellBullet1.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 32, ColorUtil.FromRGB255(122, 80, 109), ColorUtil.FromRGB255(122, 80, 109), new Vector2(1f, 1f), TextAnchor.MiddleLeft);
		sellBullet1.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		sellBullet1.Text = "#UPSELL_BULLET_1";
		Add(sellBullet1);
		sellBullet2 = new GUIDropShadowTextLabel();
		sellBullet2.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(250f, -50f), new Vector2(350f, 35f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		sellBullet2.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 32, ColorUtil.FromRGB255(122, 80, 109), ColorUtil.FromRGB255(122, 80, 109), new Vector2(1f, 1f), TextAnchor.MiddleLeft);
		sellBullet2.Text = "#UPSELL_BULLET_2";
		sellBullet2.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		Add(sellBullet2);
		sellBullet3 = new GUIDropShadowTextLabel();
		sellBullet3.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(250f, -22f), new Vector2(350f, 35f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		sellBullet3.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 32, ColorUtil.FromRGB255(122, 80, 109), ColorUtil.FromRGB255(122, 80, 109), new Vector2(1f, 1f), TextAnchor.MiddleLeft);
		sellBullet3.Text = "#UPSELL_BULLET_3";
		sellBullet3.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		Add(sellBullet3);
		option1Button = GUIControl.CreateControlFrameCentered<SHSUpsellButton>(new Vector2(175f, 100f), new Vector2(-176f, 45f));
		option1Button.BackgroundImage.TextureSource = string.Format("WEBCACHE${0}subscription_bg.png", "/");
		option1Button.OverlayPriceImage.TextureSource = string.Format("WEBCACHE${0}subscription1.png", "/");
		option1Button.Click += delegate
		{
			SubscriptionPlan = "one_month1";
			OnSubscribe();
		};
		Add(option1Button);
		option2Button = GUIControl.CreateControlFrameCentered<SHSUpsellButton>(new Vector2(175f, 100f), new Vector2(0f, 45f));
		option2Button.BackgroundImage.TextureSource = string.Format("WEBCACHE${0}subscription_bg.png", "/");
		option2Button.OverlayPriceImage.TextureSource = string.Format("WEBCACHE${0}subscription2.png", "/");
		option2Button.Click += delegate
		{
			SubscriptionPlan = "six_month1";
			OnSubscribe();
		};
		Add(option2Button);
		option3Button = GUIControl.CreateControlFrameCentered<SHSUpsellButton>(new Vector3(175f, 100f), new Vector3(173f, 45f));
		option3Button.BackgroundImage.TextureSource = string.Format("WEBCACHE${0}subscription_bg.png", "/");
		option3Button.OverlayPriceImage.TextureSource = string.Format("WEBCACHE${0}subscription3.png", "/");
		option3Button.Click += delegate
		{
			SubscriptionPlan = "twelve_month1";
			OnSubscribe();
		};
		Add(option3Button);
		backupButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(512f, 512f), new Vector2(0f, 41f));
		backupButton.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|L_button_joinnow");
		backupButton.Click += delegate
		{
			ShsWebService.SafeJavaScriptCall("HEROUPNS.RedirectToSubscribe();");
		};
		backupButton.HitTestType = HitTestTypeEnum.Alpha;
		backupButton.IsVisible = false;
		Add(backupButton);
		closeButton = GUIControl.CreateControlAbsolute<GUIButton>(new Vector2(45f, 45f), new Vector2(634f, 160f));
		closeButton.Click += okButton_Click;
		closeButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|button_close");
		closeButton.ToolTip = new NamedToolTipInfo(SHSTooltip.GetCommonToolTipText(SHSTooltip.CommonToolTipText.Close));
		Add(closeButton);
		recurwarningLabel = new GUILabel();
		recurwarningLabel.SetPositionAndSize(221f, 376f, 350f, 160f);
		recurwarningLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, ColorUtil.FromRGB255(78, 96, 109), TextAnchor.MiddleCenter);
		recurwarningLabel.Text = "#UPSELL_WARN";
		Add(recurwarningLabel);
		okButton = new GUIButton();
		okButton.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -43f), new Vector2(256f, 256f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		okButton.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|L_button_backtogame");
		okButton.Text = string.Empty;
		okButton.HitTestType = HitTestTypeEnum.Alpha;
		okButton.Click += okButton_Click;
		Add(okButton);
		iconBoy = new GUIImage();
		iconBoy.SetPositionAndSize(77f, 115f, 168f, 230f);
		iconBoy.TextureSource = "gameworld_bundle|subscription_notification_hero";
		Add(iconBoy);
		SetPosition(QuickSizingHint.Centered);
		SetSize(new Vector2(800f, 700f));
	}

	private void okButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		Hide();
	}

	private void OnSubscribe()
	{
		Time.timeScale = 0f;
		ShsWebService.SafeJavaScriptCall("HEROUPNS.RedirectToSubscription('" + SubscriptionPlan + "');");
		foreach (IGUIControl item in controlList.FindAll(delegate(IGUIControl ctrl)
		{
			return ctrl is GUIWindow || ctrl is GUIButton;
		}))
		{
			item.IsEnabled = false;
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		ShsWebAssetCache webAssetCache = AppShell.Instance.WebAssetCache;
		if (!webAssetCache.CachedWebAssets.ContainsKey("/subscription1.png") || !webAssetCache.CachedWebAssets.ContainsKey("/subscription2.png") || !webAssetCache.CachedWebAssets.ContainsKey("/subscription3.png"))
		{
			option1Button.IsVisible = false;
			option2Button.IsVisible = false;
			option3Button.IsVisible = false;
			backupButton.IsVisible = true;
		}
		else
		{
			option1Button.IsVisible = true;
			option2Button.IsVisible = true;
			option3Button.IsVisible = true;
			backupButton.IsVisible = false;
		}
		AppShell.Instance.BundleLoader.AggressiveBackgroundDownloading(true);
	}

	public override void OnHide()
	{
		AppShell.Instance.BundleLoader.AggressiveBackgroundDownloading(false);
		base.OnHide();
	}
}
