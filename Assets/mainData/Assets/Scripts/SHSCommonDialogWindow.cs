using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SHSCommonDialogWindow : GUIDialogWindow
{
	private struct BackgroundCache
	{
		public NineSliceTexture backgroundTexture;

		public NineSliceTexture foregroundTexture;

		public NotificationType type;

		public Vector2 size;

		public static BackgroundCache EmptyCache
		{
			get
			{
				BackgroundCache result = default(BackgroundCache);
				result.backgroundTexture = null;
				result.foregroundTexture = null;
				result.type = NotificationType.Common;
				result.size = Vector2.zero;
				return result;
			}
		}
	}

	public enum ButtonGrouping
	{
		Both,
		ConfirmStandalone,
		DenyStandalone
	}

	public enum NotificationType
	{
		Common,
		Error,
		Downloading
	}

	private const int maxCacheHistory = 3;

	private const float maxWidth = 350f;

	private const float buttonSpace = 10f;

	private const float minDialogWidth = 175f;

	private const float closeButtonXOffset = -15f;

	private const float closeButtonYOffset = 10f;

	private const int titleKerning = 19;

	private const int descriptionOffset = 10;

	private const string commonIconPath = "GUI/Notifications/popup_icon_burst";

	private static Queue<BackgroundCache> bgCachePool = new Queue<BackgroundCache>();

	private static bool bundlesAcquired = false;

	private static GUIToolTipManager.ToolTipResources.ToolTipPathData greenNotPathData = default(GUIToolTipManager.ToolTipResources.ToolTipPathData);

	private static GUIToolTipManager.ToolTipResources.ToolTipPathData redNotPathData = default(GUIToolTipManager.ToolTipResources.ToolTipPathData);

	private static GUIToolTipManager.ToolTipResources.ToolTipPathData whiteNotPathData = default(GUIToolTipManager.ToolTipResources.ToolTipPathData);

	private static GUIToolTipManager.ToolTipResources.ResourceBundle greenBundle;

	private static GUIToolTipManager.ToolTipResources.ResourceBundle redBundle;

	private static GUIToolTipManager.ToolTipResources.ResourceBundle whiteBundle;

	protected GUIButton okButton;

	protected GUIButton cancelButton;

	protected GUIDropShadowTextLabel text;

	protected GUILabel descriptionText;

	private GUIImage background;

	private GUIImage innerBackground;

	protected GUIImage dialogIcon;

	protected GUIButton closeButton;

	private NineSliceTexture backgroundNineSlice;

	private NineSliceTexture innerBackgroundNineSlice;

	private GUIToolTipManager.ToolTipResources.ResourceBundle notBackgroundBundle;

	protected int descriptionKerning = 14;

	protected Vector2 iconOffset = default(Vector2);

	protected static Vector2 defaultIconSize = new Vector2(168f, 144f);

	private float buttonDeficitHeight;

	private ButtonGrouping buttonGrouping;

	protected NotificationType type;

	public SHSCommonDialogWindow()
	{
	}

	public SHSCommonDialogWindow(string confirmButtonStyle, Type confirmType)
		: this("GUI/Notifications/popup_icon_burst", defaultIconSize, "common_bundle|button_close", confirmButtonStyle, string.Empty, confirmType, null, true)
	{
	}

	public SHSCommonDialogWindow(string iconPath, string confirmButtonStyle, Type confirmType)
		: this(iconPath, defaultIconSize, "common_bundle|button_close", confirmButtonStyle, string.Empty, confirmType, null, true)
	{
	}

	public SHSCommonDialogWindow(string iconPath, string confirmButtonStyle, Type confirmType, bool closeButtonExist)
		: this(iconPath, defaultIconSize, "common_bundle|button_close", confirmButtonStyle, string.Empty, confirmType, null, closeButtonExist)
	{
	}

	public SHSCommonDialogWindow(string confirmButtonStyle, string denyButtonStyle, Type confirmType, Type denyType)
		: this("GUI/Notifications/popup_icon_burst", defaultIconSize, "common_bundle|button_close", confirmButtonStyle, denyButtonStyle, confirmType, denyType, true)
	{
	}

	public SHSCommonDialogWindow(string iconPath, string confirmButtonStyle, string denyButtonStyle, Type confirmType, Type denyType)
		: this(iconPath, defaultIconSize, "common_bundle|button_close", confirmButtonStyle, denyButtonStyle, confirmType, denyType, true)
	{
	}

	public SHSCommonDialogWindow(string iconPath, string closeButtonStyle, string confirmButtonStyle, string denyButtonStyle, Type confirmType, Type denyType, bool closeButtonExist)
		: this(iconPath, defaultIconSize, closeButtonStyle, confirmButtonStyle, denyButtonStyle, confirmType, denyType, closeButtonExist)
	{
	}

	public SHSCommonDialogWindow(string iconPath, Vector2 iconSize, string closeButtonStyle, string confirmButtonStyle, string denyButtonStyle, Type confirmType, Type denyType, bool closeButtonExist)
	{
		SetPosition(QuickSizingHint.Centered);
		SetSize(431f, 359f);
		float rotation = UnityEngine.Random.Range(-2f, 2f);
		background = GUIControl.CreateControlAbsolute<GUIImage>(Vector2.zero, Vector2.zero);
		background.Rotation = rotation;
		innerBackground = GUIControl.CreateControlAbsolute<GUIImage>(Vector2.zero, Vector2.zero);
		innerBackground.Rotation = rotation;
		Add(background);
		if (closeButtonExist)
		{
			closeButton = GUIControl.CreateControlAbsolute<GUIButton>(new Vector2(45f, 45f), Vector2.zero);
			closeButton.Click += delegate
			{
				OnCancel();
			};
			closeButton.Rotation = rotation;
			closeButton.StyleInfo = new SHSButtonStyleInfo(closeButtonStyle);
			closeButton.ToolTip = new NamedToolTipInfo(SHSTooltip.GetCommonToolTipText(SHSTooltip.CommonToolTipText.Close));
			Add(closeButton);
		}
		descriptionText = new GUILabel();
		descriptionText.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, new Color(26f / 85f, 32f / 85f, 109f / 255f), TextAnchor.UpperLeft);
		descriptionText.Rotation = rotation;
		Add(descriptionText);
		text = new GUIDropShadowTextLabel();
		text.SetPositionAndSize(61f, 57f, 305f, 168f);
		text.FontFace = GUIFontManager.SupportedFontEnum.Zooom;
		text.FontSize = 29;
		text.TextAlignment = TextAnchor.UpperCenter;
		text.FrontColor = new Color(226f / 255f, 92f / 255f, 16f / 255f);
		text.BackColor = new Color(0f, 4f / 51f, 14f / 51f, 0.1f);
		text.TextOffset = new Vector2(2f, 2f);
		text.Rotation = rotation;
		Add(text);
		dialogIcon = GUIControl.CreateControlAbsolute<GUIImage>(Vector2.zero, Vector2.zero);
		dialogIcon.TextureSource = ((!(iconPath == "GUI/Notifications/popup_icon_burst")) ? iconPath : string.Empty);
		dialogIcon.SetSize(iconSize);
		dialogIcon.Rotation = rotation;
		Add(dialogIcon);
		if (confirmType != null && denyType != null)
		{
			buttonGrouping = ButtonGrouping.Both;
		}
		if (confirmType != null)
		{
			ConstructorInfo constructor = confirmType.GetConstructor(new Type[0]);
			okButton = (GUIButton)constructor.Invoke(new object[0]);
			okButton.SetSize(128f, 128f);
			okButton.StyleInfo = new SHSButtonStyleInfo(confirmButtonStyle);
			okButton.Rotation = rotation;
			Add(okButton);
		}
		else
		{
			buttonGrouping = ButtonGrouping.DenyStandalone;
		}
		if (denyType != null)
		{
			ConstructorInfo constructor = denyType.GetConstructor(new Type[0]);
			cancelButton = (GUIButton)constructor.Invoke(new object[0]);
			cancelButton.SetSize(128f, 128f);
			cancelButton.StyleInfo = new SHSButtonStyleInfo(denyButtonStyle);
			cancelButton.Rotation = rotation;
			Add(cancelButton);
		}
		else
		{
			buttonGrouping = ButtonGrouping.ConfirmStandalone;
		}
	}

	public SHSCommonDialogWindow(string iconPath, Vector2 iconSize, Vector2 iconOffset, string closeButtonStyle, string confirmButtonStyle, string denyButtonStyle, Type confirmType, Type denyType, bool closeButtonExist)
		: this(iconPath, iconSize, closeButtonStyle, confirmButtonStyle, denyButtonStyle, confirmType, denyType, closeButtonExist)
	{
		this.iconOffset = iconOffset;
	}

	private static void AcquireBundleResources()
	{
		if (!bundlesAcquired)
		{
			greenNotPathData.bodyPath = "common_bundle|greenNotBody";
			greenNotPathData.attachmentPath = string.Empty;
			greenNotPathData.cornerPaths = new string[4];
			greenNotPathData.borderPaths = new string[4];
			redNotPathData.bodyPath = "common_bundle|redNotBody";
			redNotPathData.attachmentPath = string.Empty;
			redNotPathData.cornerPaths = new string[4];
			redNotPathData.borderPaths = new string[4];
			whiteNotPathData.bodyPath = "common_bundle|whiteNotBody";
			whiteNotPathData.attachmentPath = string.Empty;
			whiteNotPathData.cornerPaths = new string[4];
			whiteNotPathData.borderPaths = new string[4];
			for (int i = 0; i < 4; i++)
			{
				greenNotPathData.cornerPaths[i] = "common_bundle|greenNotCorner" + (i + 1).ToString();
				greenNotPathData.borderPaths[i] = "common_bundle|greenNotBorder" + (i + 1).ToString();
				redNotPathData.cornerPaths[i] = "common_bundle|redNotCorner" + (i + 1).ToString();
				redNotPathData.borderPaths[i] = "common_bundle|redNotBorder" + (i + 1).ToString();
				whiteNotPathData.cornerPaths[i] = "common_bundle|whiteNotCorner" + (i + 1).ToString();
				whiteNotPathData.borderPaths[i] = "common_bundle|whiteNotBorder" + (i + 1).ToString();
			}
			GUIManager.Instance.TooltipManager.ToolTipResource.CreateBundle("GreenNotification", greenNotPathData);
			greenBundle = GUIManager.Instance.TooltipManager.ToolTipResource.GetBundle("GreenNotification");
			GUIManager.Instance.TooltipManager.ToolTipResource.CreateBundle("RedNotification", redNotPathData);
			redBundle = GUIManager.Instance.TooltipManager.ToolTipResource.GetBundle("RedNotification");
			GUIManager.Instance.TooltipManager.ToolTipResource.CreateBundle("WhiteNotification", whiteNotPathData);
			whiteBundle = GUIManager.Instance.TooltipManager.ToolTipResource.GetBundle("WhiteNotification");
			bundlesAcquired = true;
		}
	}

	protected override void dispose(bool disposing)
	{
		base.dispose(disposing);
		if (disposing)
		{
			if (bgCachePool.Count > 3)
			{
				BackgroundCache backgroundCache = bgCachePool.Dequeue();
				backgroundCache.backgroundTexture.ReleaseNineSliceTexture();
				backgroundCache.foregroundTexture.ReleaseNineSliceTexture();
				backgroundCache.backgroundTexture = null;
				backgroundCache.foregroundTexture = null;
			}
			background = null;
			innerBackground = null;
			closeButton = null;
			descriptionText = null;
			text = null;
			dialogIcon = null;
			okButton = null;
			cancelButton = null;
			GC.Collect();
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		SetupBundleDependencies();
		BuildBackground(BuildTextBlock());
		FinalizeAllUIPositioning();
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("popup_alert"));
	}

	public override void OnHide()
	{
		base.OnHide();
		dispose(true);
	}

	protected void SetupBundleDependencies()
	{
		AcquireBundleResources();
		notBackgroundBundle = ((type != 0) ? redBundle : greenBundle);
	}

	protected Vector2 BuildTextBlock()
	{
		text.Text = TitleText;
		descriptionText.Text = Text;
		Vector2 vector = descriptionText.Style.UnityStyle.CalcSize(new GUIContent(descriptionText.Text));
		Vector2 vector2 = text.Style.UnityStyle.CalcSize(new GUIContent(text.Text));
		bool flag = false;
		int num = 0;
		if (text.Text != string.Empty)
		{
			if (vector2.x > 350f - iconOffset.x)
			{
				text.VerticalKerning = 19;
				text.Style.UnityStyle.wordWrap = true;
				text.NoLineLimit = true;
				vector2.x = 350f - iconOffset.x;
				flag = true;
			}
			else
			{
				text.Style.UnityStyle.wordWrap = false;
				text.NoLineLimit = false;
			}
			GUIDropShadowTextLabel gUIDropShadowTextLabel = text;
			Vector2 size = dialogIcon.Size;
			gUIDropShadowTextLabel.SetPosition(size.x, notBackgroundBundle.borderTextures[0].height + whiteBundle.borderTextures[0].height);
			text.SetSize(new Vector2(vector2.x, 100f));
			if (flag)
			{
				text.CalculateTextLayout();
			}
		}
		else
		{
			Remove(text);
		}
		flag = false;
		if (vector.x > 350f)
		{
			descriptionText.VerticalKerning = descriptionKerning;
			descriptionText.Style.UnityStyle.wordWrap = true;
			descriptionText.NoLineLimit = true;
			vector.x = 350f;
			flag = true;
		}
		else
		{
			descriptionText.Style.UnityStyle.wordWrap = false;
			descriptionText.NoLineLimit = false;
		}
		GUILabel gUILabel = descriptionText;
		float x;
		if (type == NotificationType.Error)
		{
			Vector2 size2 = dialogIcon.Size;
			x = size2.x;
		}
		else
		{
			float num2 = notBackgroundBundle.borderTextures[3].width + whiteBundle.borderTextures[3].width;
			Vector2 size3 = dialogIcon.Size;
			x = num2 + size3.x * 0.5f;
		}
		gUILabel.SetPosition(x, notBackgroundBundle.borderTextures[0].height + whiteBundle.borderTextures[0].height + text.TotalTextHeight + 10);
		descriptionText.SetSize(new Vector2(vector.x, 100f));
		if (flag)
		{
			descriptionText.CalculateTextLayout();
		}
		if (ControlList.Contains(text))
		{
			num = text.LongestLine;
		}
		if (num < descriptionText.LongestLine)
		{
			num = descriptionText.LongestLine;
		}
		if ((float)num < 175f && buttonGrouping == ButtonGrouping.Both)
		{
			num = 175;
		}
		return new Vector2(num, text.TotalTextHeight + descriptionText.TotalTextHeight);
	}

	protected void BuildBackground(Vector2 contentSize)
	{
		BackgroundCache cacheToFillOut = BackgroundCache.EmptyCache;
		if (!FindCachedBackground(ref cacheToFillOut, contentSize))
		{
			BackgroundCache item = default(BackgroundCache);
			item.type = type;
			item.size = contentSize;
			item.foregroundTexture = new NineSliceTexture();
			item.foregroundTexture.CreateNineSliceTexture(new Vector2(contentSize.x + iconOffset.x, contentSize.y + 5f), whiteBundle.bodyTexture, whiteBundle.cornerTextures, whiteBundle.borderTextures);
			item.backgroundTexture = new NineSliceTexture(NineSliceTexture.CompressionType.Uncompressed);
			item.backgroundTexture.DelayedPixelApply = true;
			NineSliceTexture backgroundTexture = item.backgroundTexture;
			float x = item.foregroundTexture.Texture.width;
			float num = item.foregroundTexture.Texture.height;
			Vector2 size = okButton.Size;
			backgroundTexture.CreateNineSliceTexture(new Vector2(x, num + size.y * 0.25f), notBackgroundBundle.bodyTexture, notBackgroundBundle.cornerTextures, notBackgroundBundle.borderTextures);
			item.backgroundTexture.CompositeTexture(item.foregroundTexture.Texture, new Color(0f, 0f, 0f, 0f), notBackgroundBundle.cornerTextures[0].width, notBackgroundBundle.borderTextures[0].height);
			item.backgroundTexture.ManualPixelApply();
			UnityEngine.Object.DontDestroyOnLoad(item.foregroundTexture.Texture);
			UnityEngine.Object.DontDestroyOnLoad(item.backgroundTexture.Texture);
			backgroundNineSlice = item.backgroundTexture;
			innerBackgroundNineSlice = item.foregroundTexture;
			bgCachePool.Enqueue(item);
		}
		else
		{
			backgroundNineSlice = cacheToFillOut.backgroundTexture;
			innerBackgroundNineSlice = cacheToFillOut.foregroundTexture;
		}
		background.Texture = backgroundNineSlice.Texture;
		background.Size = new Vector2(backgroundNineSlice.Texture.width, backgroundNineSlice.Texture.height);
	}

	private bool FindCachedBackground(ref BackgroundCache cacheToFillOut, Vector2 contentSizeToCheck)
	{
		bool flag = false;
		foreach (BackgroundCache item in bgCachePool)
		{
			flag = ((int)item.size.x == (int)contentSizeToCheck.x && (int)item.size.y == (int)contentSizeToCheck.y && item.type == type);
			if (flag)
			{
				cacheToFillOut = item;
				return flag;
			}
		}
		return flag;
	}

	protected void FinalizeAllUIPositioning()
	{
		FinalizeAllUIPositioning(innerBackgroundNineSlice.Texture.height);
	}

	private void FinalizeAllUIPositioning(float offset)
	{
		GUIImage gUIImage = background;
		Vector2 size = dialogIcon.Size;
		gUIImage.SetPosition(size.x * 0.5f, 0f);
		dialogIcon.SetPosition(iconOffset.x, iconOffset.y);
		if (closeButton != null)
		{
			GUIButton gUIButton = closeButton;
			Vector2 position = background.Position;
			float num = position.x + (float)notBackgroundBundle.cornerTextures[0].width + (float)innerBackgroundNineSlice.Texture.width;
			Vector2 size2 = closeButton.Size;
			float x = num - size2.x;
			Vector2 position2 = background.Position;
			gUIButton.SetPosition(new Vector2(x, position2.y + (float)notBackgroundBundle.borderTextures[0].height));
		}
		Vector2 size3 = dialogIcon.Size;
		float num2 = size3.x * 0.5f;
		switch (buttonGrouping)
		{
		case ButtonGrouping.Both:
		{
			GUIButton gUIButton4 = okButton;
			Vector2 size10 = background.Size;
			float num5 = size10.x * 0.5f + num2;
			Vector2 size11 = okButton.Size;
			gUIButton4.SetPosition(new Vector2(num5 - size11.x, offset));
			GUIButton gUIButton5 = cancelButton;
			Vector2 position3 = okButton.Position;
			float x2 = position3.x;
			Vector2 size12 = okButton.Size;
			float x3 = x2 + size12.x;
			Vector2 position4 = okButton.Position;
			gUIButton5.SetPosition(new Vector2(x3, position4.y));
			Vector2 size13 = okButton.Size;
			buttonDeficitHeight = size13.y;
			break;
		}
		case ButtonGrouping.ConfirmStandalone:
		{
			GUIButton gUIButton3 = okButton;
			Vector2 size7 = background.Size;
			float num4 = size7.x * 0.5f + num2;
			Vector2 size8 = okButton.Size;
			gUIButton3.SetPosition(new Vector2(num4 - size8.x * 0.5f, offset));
			Vector2 size9 = okButton.Size;
			buttonDeficitHeight = size9.y;
			break;
		}
		case ButtonGrouping.DenyStandalone:
		{
			GUIButton gUIButton2 = cancelButton;
			Vector2 size4 = background.Size;
			float num3 = size4.x * 0.5f + num2;
			Vector2 size5 = cancelButton.Size;
			gUIButton2.SetPosition(new Vector2(num3 - size5.x * 0.5f, offset));
			Vector2 size6 = cancelButton.Size;
			buttonDeficitHeight = size6.y;
			break;
		}
		}
		GUIDropShadowTextLabel gUIDropShadowTextLabel = text;
		Vector2 size14 = background.Size;
		float x4 = size14.x;
		Vector2 size15 = dialogIcon.Size;
		float num6 = (x4 + size15.x) * 0.5f;
		Vector2 size16 = text.Size;
		float x5 = num6 - size16.x * 0.5f;
		Vector2 position5 = text.Position;
		gUIDropShadowTextLabel.Position = new Vector2(x5, position5.y);
		GUILabel gUILabel = descriptionText;
		Vector2 size17 = background.Size;
		float x6 = size17.x;
		Vector2 size18 = dialogIcon.Size;
		float num7 = (x6 + size18.x) * 0.5f;
		Vector2 size19 = descriptionText.Size;
		float x7 = num7 - size19.x * 0.5f;
		Vector2 position6 = descriptionText.Position;
		gUILabel.Position = new Vector2(x7, position6.y);
		Vector2 size20 = background.Size;
		float x8 = size20.x;
		Vector2 size21 = dialogIcon.Size;
		float width = x8 + size21.x * 0.5f;
		Vector2 size22 = background.Size;
		SetSize(width, size22.y + buttonDeficitHeight);
		SetPosition(QuickSizingHint.Centered);
	}
}
