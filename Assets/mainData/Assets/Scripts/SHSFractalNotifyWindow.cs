using UnityEngine;

public class SHSFractalNotifyWindow : GUIChildWindow
{
	private GUILabel _numLabel;

	private GUIImage _fractalIcon;

	private int _fractalCount;

	private float _fadeStartTime;

	private int _fractalMax;

	private FractalActivitySpawnPoint.FractalType _fractalType = FractalActivitySpawnPoint.FractalType.Fractal;

	public float FadeDelayTime;

	private static readonly float FadeDelayTimeDefault = 4f;

	private static readonly float FadeDurationTime = 0.5f;

	private static readonly int LogoOffset = 20;

	private static readonly int TextBlockOffset = 20;

	private static readonly int TextLineOffset = 10;

	public SHSFractalNotifyWindow()
	{
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		Traits.HitTestType = HitTestTypeEnum.Transparent;
		SetControlFlag(ControlFlagSetting.HitTestIgnore, true, false);
		Traits.UpdateTrait = ControlTraits.UpdateTraitEnum.AlwaysUpdate;
		Traits.LifeSpan = ControlTraits.LifeSpanTraitEnum.DestroyOnHide;
		FadeDelayTime = FadeDelayTimeDefault;
	}

	public override bool InitializeResources(bool reload)
	{
		GUIImage gUIImage = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(225f, 94f), Vector2.zero);
		gUIImage.Position = Vector2.zero;
		gUIImage.TextureSource = "GUI/Notifications/gameworld_pickup_toast_herotokens";
		_numLabel = new GUILabel();
		_numLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 26, GUILabel.GenColor(255, 249, 157), TextAnchor.UpperLeft);
		SetFractalCount(_fractalType, _fractalCount);
		GUILabel numLabel = _numLabel;
		Vector2 size = gUIImage.Size;
		float num = size.x * 0.5f;
		Vector2 size2 = _numLabel.Size;
		numLabel.Position = new Vector2(num - size2.x * 0.5f + (float)LogoOffset, TextBlockOffset);
		GUILabel gUILabel = new GUILabel();
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 249, 157), TextAnchor.UpperLeft);
		gUILabel.Text = "#FRACTAL_COUNT_TOAST";
		UpdateLabelSize(gUILabel);
		Vector2 size3 = gUIImage.Size;
		float num2 = size3.x * 0.5f;
		Vector2 size4 = gUILabel.Size;
		float x = num2 - size4.x * 0.5f + (float)LogoOffset;
		Vector2 position = _numLabel.Position;
		float y = position.y;
		Vector2 size5 = _numLabel.Size;
		gUILabel.Position = new Vector2(x, y + size5.y - (float)TextLineOffset);
		_fractalIcon = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(64f, 64f), Vector2.zero);
		_fractalIcon.Position = new Vector2(13f, 13f);
		Add(gUIImage);
		Add(gUILabel);
		Add(_numLabel);
		Add(_fractalIcon);
		Vector2 offset = new Vector2(0f, -174f);
		Vector2 size6 = gUIImage.Size;
		float x2 = size6.x;
		Vector2 size7 = gUIImage.Size;
		SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, offset, new Vector2(x2, size7.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		SHSFractalsActivity sHSFractalsActivity = AppShell.Instance.ActivityManager.GetActivity("fractalsactivity") as SHSFractalsActivity;
		_fractalMax = sHSFractalsActivity.MaxFractals;
		return base.InitializeResources(reload);
	}

	public override void OnShow()
	{
		base.OnShow();
		Alpha = 1f;
		_fadeStartTime = Time.time;
		if (_fractalIcon != null)
		{
			_fractalIcon.TextureSource = GetFractalTextureSource();
		}
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (_fadeStartTime > 0f && Time.time >= _fadeStartTime + FadeDelayTime)
		{
			Alpha = 1f - (Time.time - (_fadeStartTime + FadeDelayTime)) / FadeDurationTime;
			if (Alpha <= 0f)
			{
				Hide();
			}
		}
	}

	public void CollectFractal(FractalActivitySpawnPoint.FractalType fractalType, int fractalCount)
	{
		GUIManager.Instance.ShowDynamicWindow(new SHSFractalholioWindow(new Vector2(0f, GUIManager.ScreenRect.height - 174f), fractalType, fractalCount, GetFractalTextureSource()), ModalLevelEnum.None);
	}

	public void SetFractalCount(FractalActivitySpawnPoint.FractalType fractalType, int fractalCount)
	{
		if (_numLabel != null)
		{
			SHSFractalsActivity sHSFractalsActivity = AppShell.Instance.ActivityManager.GetActivity("fractalsactivity") as SHSFractalsActivity;
			_numLabel.Text = sHSFractalsActivity.GetFractalCollectionCount().ToString() + "/" + _fractalMax.ToString();
			UpdateLabelSize(_numLabel);
		}
		_fractalType = fractalType;
		_fractalCount = fractalCount;
	}

	private void UpdateLabelSize(GUILabel label)
	{
		GUIContent gUIContent = new GUIContent();
		gUIContent.text = label.Text;
		label.Size = label.Style.UnityStyle.CalcSize(gUIContent);
	}

	private string GetFractalTextureSource()
	{
		if (GameController.GetController() != null && GameController.GetController().LocalPlayer != null)
		{
			switch (_fractalType)
			{
			case FractalActivitySpawnPoint.FractalType.Fractal:
				return "common_bundle|fractal";
			case FractalActivitySpawnPoint.FractalType.DinosaurBone:
				return "common_bundle|wip_attractive";
			default:
				return "common_bundle|wip_attractive";
			}
		}
		CspUtils.DebugLog("Could not get fractal token for collection window");
		return string.Empty;
	}
}
