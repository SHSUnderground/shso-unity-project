using System.Collections.Generic;
using UnityEngine;

public class SHSWaitWindow : GUITopLevelWindow
{
	public delegate void OnWaitWindowVisible();

	private const string loadingBarFrameTexSrc = "GUI/loading/loading_bar_frame";

	private const string loadingBarBehindFrameTexSrc = "GUI/loading/loading_bar_behind_frame";

	private const string loadingTextTexSrc = "loading_bundle|L_loading_text";

	private const string leftTipBoxTexSrc = "GUI/loading/loading_tipbox_leftside";

	private const string middleTipBoxTexSrc = "GUI/loading/loading_tipbox_center";

	private const string rightTipBoxTexSrc = "GUI/loading/loading_tipbox_rightside";

	private const string loadingBarFillTexSrc = "GUI/loading/loading_bar_fill";

	private const string loadingBarFillTipTexSrc = "GUI/loading/loading_bar_rightside";

	private const float maxWidthResolution = 1920f;

	private const float maxHeightResolution = 1213f;

	private const float loadingFrameWidth = 772f;

	private const float loadingFrameHeight = 112f;

	private const float loadingBarBehindFrameWidth = 772f;

	private const float loadingBarBehindFrameHeight = 112f;

	private const float loadingTextWidth = 147f;

	private const float loadingTextHeight = 41f;

	private const float leftTipBoxWidth = 158f;

	private const float leftTipBoxHeight = 147f;

	private const float centerTipBoxWidth = 1f;

	private const float centerTipBoxHeight = 145f;

	private const float rightTipBoxWidth = 124f;

	private const float rightTipBoxHeight = 145f;

	private const float loadingBarTipWidth = 61f;

	private const float loadingBarTipHeight = 82f;

	private const float maxDelta = 0.02f;

	private const float maxPercentAllowed = 100f;

	private const float maxIncrementAllowed = 30f;

	private const int progressThreshold = 5;

	private const float syncWaitTime = 0.2f;

	public GUIDrawTexture BackgroundLoading;

	protected GUILabel helperText;

	protected SHSHintLabel tipText;

	protected GUIDropShadowTextLabel titleText;

	protected GUIDropShadowTextLabel whoseTipText;

	protected GUIProgressBar progressBar;

	protected GUIImage backdrop;

	protected GUIImage loadingBarFrame;

	protected GUIImage loadingBarBehindFrame;

	protected GUIImage loadingTextImg;

	protected GUIImage leftTipBoxImg;

	protected GUIImage centerTipBoxImg;

	protected GUIImage rightTipBoxImg;

	protected GUIImage tipBoxIcon;

	protected GUIImage loadingBarFillTip;

	protected GUIImage additionalImage;

	private Queue<float> progressValQueue = new Queue<float>();

	protected GUILoadingScreenContext windowContext;

	private OnWaitWindowVisible onWindowBeginsWaiting;

	private bool progressInit;

	private bool maxReached;

	private float lerpSpeed = 3f;

	private float progressTimer;

	private float progressSource;

	private float progressDestination;

	private float currentProgress;

	private float lastProgressVal;

	private float syncTicker;

	private bool percentInSync = true;

	private int cachedWidth;

	private float resolutionWidthPercentage;

	private int cachedHeight;

	private float resolutionHeightPercentage;

	public GUILoadingScreenContext WindowContext
	{
		get
		{
			return windowContext;
		}
		set
		{
			windowContext = value;
		}
	}

	public OnWaitWindowVisible OnWindowBeginsWaiting
	{
		get
		{
			return onWindowBeginsWaiting;
		}
		set
		{
			onWindowBeginsWaiting = value;
		}
	}

	public float ProgressValue
	{
		get
		{
			return currentProgress;
		}
		set
		{
			bool flag = value >= 100f && !maxReached;
			if ((int)(value - lastProgressVal) < 5 && !flag)
			{
				return;
			}
			if (value >= 100f)
			{
				maxReached = true;
			}
			if (value - lastProgressVal >= 30f)
			{
				float num = value - lastProgressVal;
				float num2 = num * 0.25f;
				for (int i = 0; i < 4; i++)
				{
					float item = lastProgressVal + num2 * (float)(i + 1);
					progressValQueue.Enqueue(item);
				}
			}
			else
			{
				progressValQueue.Enqueue(value);
			}
			percentInSync = false;
			lastProgressVal = value;
			syncTicker = 0f;
		}
	}

	public bool PercentInSync
	{
		get
		{
			return percentInSync;
		}
	}

	protected float ResolutionWidthPercentage
	{
		get
		{
			if (cachedWidth != (int)GUIManager.ScreenRect.width)
			{
				cachedWidth = (int)GUIManager.ScreenRect.width;
				resolutionWidthPercentage = GUIManager.ScreenRect.width / 1920f;
			}
			return resolutionWidthPercentage;
		}
	}

	protected float ResolutionHeightPercentage
	{
		get
		{
			if (cachedHeight != (int)GUIManager.ScreenRect.height)
			{
				cachedHeight = (int)GUIManager.ScreenRect.height;
				resolutionHeightPercentage = GUIManager.ScreenRect.height / 1213f;
			}
			return resolutionHeightPercentage;
		}
	}

	public SHSWaitWindow()
		: base("SHSWaitWindow")
	{
	}

	public override bool InitializeResources(bool reload)
	{
		backdrop = new GUIImage();
		backdrop.SetPosition(QuickSizingHint.Centered);
		backdrop.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		backdrop.TextureSource = "gui/white2x2";
		backdrop.Color = GUILabel.GenColor(56, 86, 172);
		backdrop.Traits.RespectDisabledAlphaTrait = ControlTraits.RespectDisabledAlphaTraitEnum.DisrespectDisabledAlpha;
		Add(backdrop);
		BackgroundLoading = new GUIDrawTexture();
		BackgroundLoading.SetPosition(QuickSizingHint.Centered);
		BackgroundLoading.SetSize(GUIManager.ScreenRect.width, GUIManager.ScreenRect.height);
		BackgroundLoading.Text = "BackgroundTexture";
		BackgroundLoading.Id = "loadingBackground";
		BackgroundLoading.Rotation = 0f;
		BackgroundLoading.Color = new Color(1f, 1f, 1f, 1f);
		BackgroundLoading.TextureSource = "gui/loading/loading";
		BackgroundLoading.IsVisible = true;
		Add(BackgroundLoading);
		additionalImage = GUIControl.CreateControlFrameCentered<GUIImage>(Vector2.zero, Vector2.zero);
		additionalImage.Id = "additionalLoadingImage";
		Add(additionalImage);
		loadingBarBehindFrame = new GUIImage();
		loadingBarBehindFrame.TextureSource = "GUI/loading/loading_bar_behind_frame";
		loadingBarBehindFrame.Id = "loadingBarBehindFrame";
		loadingBarBehindFrame.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, Vector2.zero, new Vector2(772f * ResolutionWidthPercentage, 112f * ResolutionHeightPercentage), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Add(loadingBarBehindFrame);
		progressBar = new GUIProgressBar();
		progressBar.Id = "percentBar";
		progressBar.UpdateSpeed = 50f;
		progressBar.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -14f * ResolutionHeightPercentage));
		progressBar.SetSize(500f * ResolutionWidthPercentage, 47f * ResolutionHeightPercentage);
		progressBar.ForegroundTexture = "GUI/loading/loading_bar_fill";
		Add(progressBar);
		loadingBarFillTip = new GUIImage();
		loadingBarFillTip.Id = "loadingBarFillTip";
		loadingBarFillTip.Position = Vector2.zero;
		loadingBarFillTip.Size = new Vector2(61f * ResolutionWidthPercentage, 82f * ResolutionHeightPercentage);
		loadingBarFillTip.TextureSource = "GUI/loading/loading_bar_rightside";
		loadingBarFillTip.IsVisible = false;
		Add(loadingBarFillTip);
		loadingBarFrame = new GUIImage();
		loadingBarFrame.TextureSource = "GUI/loading/loading_bar_frame";
		loadingBarFrame.Id = "loadingBarFrame";
		loadingBarFrame.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, Vector2.zero, new Vector2(772f * ResolutionWidthPercentage, 112f * ResolutionHeightPercentage), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Add(loadingBarFrame);
		loadingTextImg = new GUIImage();
		loadingTextImg.TextureSource = ((!GUILoadingScreenContext.LoadingScreenAssetsAvailable) ? string.Empty : "loading_bundle|L_loading_text");
		loadingTextImg.Id = "loadingTextImg";
		loadingTextImg.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -21f * ResolutionHeightPercentage), new Vector2(147f * ResolutionWidthPercentage, 41f * ResolutionHeightPercentage), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Add(loadingTextImg);
		leftTipBoxImg = new GUIImage();
		leftTipBoxImg.Id = "leftTipBoxImg";
		leftTipBoxImg.TextureSource = "GUI/loading/loading_tipbox_leftside";
		leftTipBoxImg.IsVisible = false;
		Add(leftTipBoxImg);
		centerTipBoxImg = new GUIImage();
		centerTipBoxImg.Id = "centerTipBoxImg";
		centerTipBoxImg.TextureSource = "GUI/loading/loading_tipbox_center";
		centerTipBoxImg.IsVisible = false;
		Add(centerTipBoxImg);
		rightTipBoxImg = new GUIImage();
		rightTipBoxImg.Id = "rightTipBoxImg";
		rightTipBoxImg.TextureSource = "GUI/loading/loading_tipbox_rightside";
		rightTipBoxImg.IsVisible = false;
		Add(rightTipBoxImg);
		titleText = new GUIDropShadowTextLabel();
		titleText.FontFace = GUIFontManager.SupportedFontEnum.Zooom;
		titleText.FontSize = 37;
		titleText.Id = "titleText";
		titleText.Text = string.Empty;
		titleText.TextAlignment = TextAnchor.UpperCenter;
		titleText.FrontColor = GUILabel.GenColor(255, 247, 199);
		titleText.BackColor = GUILabel.GenColor(0, 21, 105);
		titleText.BackColorAlpha = 0.2f;
		titleText.TextOffset = new Vector2(-2f, 2f);
		titleText.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(0f, 20f));
		titleText.SetSize(500f, 100f);
		Add(titleText);
		whoseTipText = new GUIDropShadowTextLabel();
		whoseTipText.FontFace = GUIFontManager.SupportedFontEnum.Grobold;
		whoseTipText.FontSize = (int)(30f * ResolutionHeightPercentage);
		whoseTipText.Id = "whoseTipText";
		whoseTipText.Text = string.Empty;
		whoseTipText.TextAlignment = TextAnchor.UpperCenter;
		whoseTipText.FrontColor = GUILabel.GenColor(47, 91, 4);
		whoseTipText.BackColor = GUILabel.GenColor(0, 21, 105);
		whoseTipText.BackColorAlpha = 0.2f;
		whoseTipText.TextOffset = new Vector2(-2f, 2f);
		Add(whoseTipText);
		tipText = new SHSHintLabel();
		tipText.SetupText(GUIFontManager.SupportedFontEnum.Komica, (int)(30f * ((ResolutionHeightPercentage + ResolutionWidthPercentage) / 2f)), GUILabel.GenColor(47, 91, 4), TextAnchor.UpperCenter);
		tipText.Id = "tipText";
		tipText.Text = string.Empty;
		tipText.IsVisible = false;
		Add(tipText);
		helperText = new GUILabel();
		helperText.SetupText(GUIFontManager.SupportedFontEnum.Zooom, (int)(36f * ((ResolutionHeightPercentage + ResolutionWidthPercentage) / 2f)), Color.white, TextAnchor.MiddleCenter);
		helperText.Id = "helperText";
		helperText.Text = "#loading";
		helperText.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -80f));
		helperText.SetSize(500f, 100f);
		helperText.IsVisible = false;
		Add(helperText);
		tipBoxIcon = new GUIImage();
		tipBoxIcon.Id = "tipBoxIcon";
		tipBoxIcon.IsVisible = false;
		Traits.LifeSpan = ControlTraits.LifeSpanTraitEnum.DestroyOnHide;
		Traits.RespectDisabledAlphaTrait = ControlTraits.RespectDisabledAlphaTraitEnum.DisrespectDisabledAlpha;
		if (windowContext != null)
		{
			SetWindowAppearance();
		}
		return base.InitializeResources(reload);
	}

	private void SetWindowAppearance()
	{
		bool flag = GUIManager.Instance.StyleManager != null;
		bool flag2 = flag && windowContext.WhoseTipText != string.Empty;
		titleText.Text = windowContext.LocationName;
		tipText.Text = windowContext.TipText;
		whoseTipText.Text = ((!flag2) ? string.Empty : windowContext.WhoseTipText);
		GUIContent gUIContent = null;
		Vector2 textSize = Vector2.zero;
		if (flag)
		{
			gUIContent = new GUIContent(tipText.Text);
			textSize = tipText.Style.UnityStyle.CalcSize(gUIContent);
		}
		if (!string.IsNullOrEmpty(windowContext.TipText))
		{
			BuildTipBox(textSize, windowContext.TipTextureSource);
		}
		else
		{
			centerTipBoxImg.IsVisible = false;
			leftTipBoxImg.IsVisible = false;
			rightTipBoxImg.IsVisible = false;
			tipText.IsVisible = false;
		}
		BackgroundLoading.TextureSource = windowContext.BackgroundTextureSource;
		additionalImage.TextureSource = windowContext.AdditionalTextureSource;
		additionalImage.Offset = windowContext.AdditionalTextureOffset;
		additionalImage.Size = windowContext.AdditionalTextureSize;
		if (windowContext.CustomSetup != null)
		{
			windowContext.CustomSetup(this);
		}
	}

	public void BuildTipBox(Vector2 textSize, string iconSource)
	{
		centerTipBoxImg.IsVisible = true;
		leftTipBoxImg.IsVisible = true;
		rightTipBoxImg.IsVisible = true;
		float num = textSize.x + 15f;
		centerTipBoxImg.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -125f * ResolutionHeightPercentage), new Vector2(num, 145f * ResolutionHeightPercentage), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		float num2 = 158f * ResolutionWidthPercentage;
		float num3 = 124f * ResolutionWidthPercentage;
		GUIImage gUIImage = leftTipBoxImg;
		float x = (0f - (num + num2)) * 0.5f + 2f;
		Vector2 offset = centerTipBoxImg.Offset;
		gUIImage.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(x, offset.y), new Vector2(num2, 147f * ResolutionHeightPercentage), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIImage gUIImage2 = rightTipBoxImg;
		float x2 = (num + num3) * 0.5f - 2f;
		Vector2 offset2 = centerTipBoxImg.Offset;
		gUIImage2.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(x2, offset2.y), new Vector2(num3, 145f * ResolutionHeightPercentage), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SHSHintLabel sHSHintLabel = tipText;
		Vector2 offset3 = centerTipBoxImg.Offset;
		sHSHintLabel.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, offset3.y - 42f * ResolutionWidthPercentage), textSize, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIDropShadowTextLabel gUIDropShadowTextLabel = whoseTipText;
		Vector2 offset4 = centerTipBoxImg.Offset;
		gUIDropShadowTextLabel.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, offset4.y - 73f * ResolutionHeightPercentage), textSize, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		tipText.IsVisible = true;
		tipBoxIcon.TextureSource = iconSource;
		tipBoxIcon.IsVisible = true;
		tipBoxIcon.SetPositionAndSize(AnchorAlignmentEnum.Middle, leftTipBoxImg.RotationPoint, new Vector2(256f, 256f));
		Add(tipBoxIcon);
	}

	protected override void InitializeBundleList()
	{
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("brawler_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		base.InitializeBundleList();
	}

	protected void ProgressUpdate()
	{
		if (!progressInit && progressValQueue.Count > 0)
		{
			progressInit = true;
			progressTimer = 0f;
			progressSource = 0f;
			progressDestination = progressValQueue.Dequeue();
		}
		else if (progressTimer >= 1f && progressValQueue.Count > 0)
		{
			progressTimer = 0f;
			progressSource = progressDestination;
			progressDestination = progressValQueue.Dequeue();
		}
		float num = Time.deltaTime;
		if (num > 0.02f)
		{
			num = 0.02f;
		}
		progressTimer += num * lerpSpeed;
		currentProgress = Mathf.Lerp(progressSource, progressDestination, progressTimer);
		if (currentProgress >= 100f)
		{
			syncTicker += num;
		}
		if (syncTicker >= 0.2f)
		{
			percentInSync = true;
		}
	}

	protected void UpdateBaseProgressUI()
	{
		progressBar.Value = currentProgress;
		loadingBarFillTip.IsVisible = IsVisible;
		GUIImage gUIImage = loadingBarFillTip;
		Vector2 position = progressBar.Position;
		float x = position.x;
		Vector2 size = progressBar.Size;
		float x2 = x + size.x * progressBar.Value / 100f - 15f;
		Vector2 position2 = progressBar.Position;
		float y = position2.y;
		Vector2 size2 = loadingBarFillTip.Size;
		gUIImage.Position = new Vector2(x2, y - size2.y * 0.25f);
	}

	protected void BaseUpdate()
	{
		base.Update();
	}

	public override void Update()
	{
		BaseUpdate();
		ProgressUpdate();
		UpdateBaseProgressUI();
	}
}
