using UnityEngine;

public class SHSScavengerItemNotifyWindow : GUIChildWindow
{
	private GUIImage _background;

	private GUILabel _numLabel;

	private GUILabel _itemName;

	private GUIImage _icon;

	private int _count;

	private float _fadeStartTime;

	public float FadeDelayTime;

	private static readonly float FadeDelayTimeDefault = 5f;

	private static readonly float FadeDurationTime = 0.5f;

	private int _specificIconID = -1;

	public SHSScavengerItemNotifyWindow()
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
		_background = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(225f, 94f), Vector2.zero);
		_background.Position = Vector2.zero;
		_background.TextureSource = "GUI/Notifications/gameworld_pickup_toast_herotokens";
		_numLabel = new GUILabel();
		_numLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, GUILabel.GenColor(255, 249, 157), TextAnchor.UpperLeft);
		SetCount(_count);
		_numLabel.Position = new Vector2(140f, 55f);
		_itemName = new GUILabel();
		_itemName.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, GUILabel.GenColor(255, 249, 157), TextAnchor.MiddleCenter);
		_itemName.VerticalKerning = -1;
		_itemName.WordWrap = true;
		_itemName.Size = new Vector2(110f, 80f);
		_itemName.Position = new Vector2(83f, -3f);
		_icon = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(64f, 64f), Vector2.zero);
		_icon.Position = new Vector2(13f, 13f);
		Add(_background);
		Add(_itemName);
		Add(_numLabel);
		Add(_icon);
		Vector2 offset = new Vector2(0f, -334f);
		Vector2 size = _background.Size;
		float x = size.x;
		Vector2 size2 = _background.Size;
		SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, offset, new Vector2(x, size2.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
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

	public void showSpecificIcon(int ownableID)
	{
		_specificIconID = ownableID;
		if (_icon != null)
		{
			_icon.TextureSource = GetTextureSource();
		}
		OwnableDefinition def = OwnableDefinition.getDef(_specificIconID);
		if (def != null)
		{
			_itemName.Text = def.name;
		}
	}

	public void SetCount(int count)
	{
		if (_numLabel != null)
		{
			_numLabel.Text = "x" + (count + 1) + string.Empty;
			UpdateLabelSize(_numLabel);
		}
		_specificIconID = -1;
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
			if (_specificIconID != -1)
			{
				OwnableDefinition def = OwnableDefinition.getDef(_specificIconID);
				if (def != null)
				{
					return def.iconFullPath;
				}
			}
			return "shopping_bundle|craft_generic";
		}
		CspUtils.DebugLog("Could not get scavenger object token for collection window");
		return string.Empty;
	}
}
