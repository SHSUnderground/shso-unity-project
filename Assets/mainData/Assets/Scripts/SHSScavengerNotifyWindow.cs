using UnityEngine;

public class SHSScavengerNotifyWindow : GUIChildWindow
{
	private GUILabel _numLabel;

	private GUIImage _icon;

	private int _count;

	private float _fadeStartTime;

	private int _objectMax;

	public float FadeDelayTime;

	private static readonly float FadeDelayTimeDefault = 4f;

	private static readonly float FadeDurationTime = 0.5f;

	private static readonly int LogoOffset = 20;

	private static readonly int TextBlockOffset = 18;

	private static readonly int TextLineOffset = 16;

	public SHSScavengerNotifyWindow()
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
		_numLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 22, GUILabel.GenColor(255, 249, 157), TextAnchor.UpperLeft);
		SetCount(_count);
		GUILabel numLabel = _numLabel;
		Vector2 size = gUIImage.Size;
		float num = size.x * 0.5f;
		Vector2 size2 = _numLabel.Size;
		numLabel.Position = new Vector2(num - size2.x * 0.5f + (float)LogoOffset, TextBlockOffset);
		GUILabel gUILabel = new GUILabel();
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, GUILabel.GenColor(255, 249, 157), TextAnchor.MiddleCenter);
		gUILabel.Text = "#CRAFTING_COUNT_TOAST";
		gUILabel.WordWrap = true;
		gUILabel.VerticalKerning = -1;
		gUILabel.Size = new Vector2(100f, 50f);
		Vector2 size3 = gUIImage.Size;
		float num2 = size3.x * 0.5f;
		Vector2 size4 = gUILabel.Size;
		float x = num2 - size4.x * 0.5f + (float)LogoOffset;
		Vector2 position = _numLabel.Position;
		float y = position.y;
		Vector2 size5 = _numLabel.Size;
		gUILabel.Position = new Vector2(x, y + size5.y - (float)TextLineOffset);
		_icon = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(64f, 64f), Vector2.zero);
		_icon.Position = new Vector2(13f, 13f);
		Add(gUIImage);
		Add(gUILabel);
		Add(_numLabel);
		Add(_icon);
		Vector2 offset = new Vector2(0f, -254f);
		Vector2 size6 = gUIImage.Size;
		float x2 = size6.x;
		Vector2 size7 = gUIImage.Size;
		SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, offset, new Vector2(x2, size7.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		HeroPersisted value;
		if (AppShell.Instance.Profile.AvailableCostumes.TryGetValue(AppShell.Instance.Profile.SelectedCostume, out value))
		{
			_objectMax = value.maxScavengeObjects;
		}
		else
		{
			_objectMax = 5;
		}
		return base.InitializeResources(reload);
	}

	public override void OnShow()
	{
		base.OnShow();
		Alpha = 1f;
		_fadeStartTime = Time.time;
		if (_icon != null)
		{
			_icon.TextureSource = GetTextureSource();
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

	public void CollectObject(int count)
	{
		GUIManager.Instance.ShowDynamicWindow(new SHSScavengerholioWindow(new Vector2(0f, GUIManager.ScreenRect.height - 254f), count, GetTextureSource()), ModalLevelEnum.None);
	}

	public void SetCount(int count)
	{
		if (_numLabel != null)
		{
			int num = 0;
			HeroPersisted value;
			num = (AppShell.Instance.Profile.AvailableCostumes.TryGetValue(AppShell.Instance.Profile.SelectedCostume, out value) ? value.objectsCollected : 0);
			_numLabel.Text = string.Empty + num + "/" + _objectMax.ToString();
			UpdateLabelSize(_numLabel);
		}
		_count = count;
	}

	private void UpdateLabelSize(GUILabel label)
	{
		GUIContent gUIContent = new GUIContent();
		gUIContent.text = label.Text;
		label.Size = label.Style.UnityStyle.CalcSize(gUIContent);
	}

	private string GetTextureSource()
	{
		if (GameController.GetController() != null && GameController.GetController().LocalPlayer != null)
		{
			return "shopping_bundle|craft_generic";
		}
		CspUtils.DebugLog("Could not get scavenger object token for collection window");
		return string.Empty;
	}
}
