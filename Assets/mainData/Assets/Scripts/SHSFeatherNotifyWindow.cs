using UnityEngine;

public class SHSFeatherNotifyWindow : GUIChildWindow
{
	private GUILabel _numLabel;

	private GUIImage _faceIcon;

	private int _collectedFeathers;

	private int _maxFeathers;

	private float _fadeStartTime;

	public float FadeDelayTime;

	private static readonly float FadeDelayTimeDefault = 4f;

	private static readonly float FadeDurationTime = 0.5f;

	private static readonly int LogoOffset = 20;

	private static readonly int TextBlockOffset = 20;

	private static readonly int TextLineOffset = 10;

	public SHSFeatherNotifyWindow()
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
		SetFeatherCount(_collectedFeathers, _maxFeathers);
		GUILabel numLabel = _numLabel;
		Vector2 size = gUIImage.Size;
		float num = size.x * 0.5f;
		Vector2 size2 = _numLabel.Size;
		numLabel.Position = new Vector2(num - size2.x * 0.5f + (float)LogoOffset, TextBlockOffset);
		GUILabel gUILabel = new GUILabel();
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 249, 157), TextAnchor.UpperLeft);
		gUILabel.Text = "#TOKEN_COUNT_TOAST";
		UpdateLabelSize(gUILabel);
		Vector2 size3 = gUIImage.Size;
		float num2 = size3.x * 0.5f;
		Vector2 size4 = gUILabel.Size;
		float x = num2 - size4.x * 0.5f + (float)LogoOffset;
		Vector2 position = _numLabel.Position;
		float y = position.y;
		Vector2 size5 = _numLabel.Size;
		gUILabel.Position = new Vector2(x, y + size5.y - (float)TextLineOffset);
		_faceIcon = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(64f, 64f), Vector2.zero);
		_faceIcon.Position = new Vector2(13f, 13f);
		Add(gUIImage);
		Add(gUILabel);
		Add(_numLabel);
		Add(_faceIcon);
		Vector2 offset = new Vector2(0f, -94f);
		Vector2 size6 = gUIImage.Size;
		float x2 = size6.x;
		Vector2 size7 = gUIImage.Size;
		SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, offset, new Vector2(x2, size7.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		return base.InitializeResources(reload);
	}

	public override void OnShow()
	{
		base.OnShow();
		Alpha = 1f;
		_fadeStartTime = Time.time;
		UpdateFaceIcon();
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

	public void CollectFeather(int collectedFeathers, int maxFeathers)
	{
		GUIManager.Instance.ShowDynamicWindow(new SHSFeatherholioWindow(new Vector2(0f, GUIManager.ScreenRect.height - 94f), collectedFeathers, maxFeathers, GetFaceIconTextureSource()), ModalLevelEnum.None);
		SetFeatherCount(collectedFeathers, maxFeathers);
	}

	public void SetFeatherCount(int collectedFeathers, int maxFeathers)
	{
		_collectedFeathers = collectedFeathers;
		_maxFeathers = maxFeathers;
		if (_numLabel != null)
		{
			_numLabel.Text = _collectedFeathers.ToString() + "/" + _maxFeathers.ToString();
			UpdateLabelSize(_numLabel);
		}
		if (_collectedFeathers >= _maxFeathers)
		{
			_fadeStartTime = Time.time + FadeDelayTime;
		}
	}

	public void UpdateFaceIcon()
	{
		if (_faceIcon != null)
		{
			_faceIcon.TextureSource = GetFaceIconTextureSource();
		}
	}

	private void UpdateLabelSize(GUILabel label)
	{
		GUIContent gUIContent = new GUIContent();
		gUIContent.text = label.Text;
		label.Size = label.Style.UnityStyle.CalcSize(gUIContent);
	}

	private string GetFaceIconTextureSource()
	{
		if (GameController.GetController() != null && GameController.GetController().LocalPlayer != null)
		{
			return "characters_bundle|token_" + GameController.GetController().LocalPlayer.name;
		}
		CspUtils.DebugLog("Could not get hero face for hero token window");
		return string.Empty;
	}
}
