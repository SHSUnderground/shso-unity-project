using UnityEngine;

public class SHSVOBrawlerObjectiveWindow : GUIDynamicWindow, IVOSpeakerWindow
{
	private const float MAX_EXPAND = 306f;

	private const int LABEL_MAX_LINES = 4;

	private static readonly Vector2 WINDOW_SIZE = new Vector2(574f, 240f);

	private static readonly Vector2 BG_LEFT_SIZE = new Vector2(243f, 160f);

	private static readonly Vector2 BG_MIDDLE_SIZE = new Vector2(1f, 160f);

	private static readonly Vector2 BG_RIGHT_SIZE = new Vector2(250f, 160f);

	private static readonly Vector2 SPEAKER_ICON_SIZE = new Vector2(200f, 218f);

	private static readonly Vector2 SPEAKER_LABEL_SIZE = new Vector2(325f, 80f);

	private static readonly Vector2 CLOSE_BUTTON_SIZE = new Vector2(48f, 48f);

	private static readonly Vector2 CLOSE_BUTTON_OFFSET = new Vector2(-44f, 54f);

	private ResolvedVOAction _vo;

	private string _speaker;

	private string _speakerText;

	private GUIImage _speakerIcon;

	private GUILabel _speakerLabel;

	private GUIImage _bgMiddle;

	private GUITBCloseButton _closeButton;

	private GUISimpleControlWindow _closeBlocker;

	public SHSVOBrawlerObjectiveWindow()
	{
		Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Ignore;
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		Traits.HitTestType = HitTestTypeEnum.Transparent;
	}

	public static IVOSpeakerWindow CreateWindow()
	{
		SHSVOBrawlerObjectiveWindow sHSVOBrawlerObjectiveWindow = new SHSVOBrawlerObjectiveWindow();
		GUIManager.Instance.ShowDynamicWindow(sHSVOBrawlerObjectiveWindow, ModalLevelEnum.None);
		return sHSVOBrawlerObjectiveWindow;
	}

	public override bool InitializeResources(bool reload)
	{
		SetSize(WINDOW_SIZE);
		SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, Vector2.zero);
		GUIImage gUIImage = GUIControl.CreateControl<GUIImage>(BG_LEFT_SIZE, new Vector2(43f, 0f), DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft);
		gUIImage.TextureSource = "common_bundle|mission_narrated_intro_module_left";
		gUIImage.Id = "VOBgLeft";
		Add(gUIImage);
		_bgMiddle = GUIControl.CreateControl<GUIImage>(BG_MIDDLE_SIZE, Vector2.zero, DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
		_bgMiddle.TextureSource = "common_bundle|mission_narrated_intro_module_middle";
		_bgMiddle.Id = "VOBgMiddle";
		Add(_bgMiddle);
		GUIImage gUIImage2 = GUIControl.CreateControl<GUIImage>(BG_RIGHT_SIZE, new Vector2(-37f, 0f), DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight);
		gUIImage2.TextureSource = "common_bundle|mission_narrated_intro_module_right";
		gUIImage2.Id = "VOBgRight";
		Add(gUIImage2);
		_speakerIcon = GUIControl.CreateControl<GUIImage>(SPEAKER_ICON_SIZE, new Vector2(15f, 1f), DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
		_speakerIcon.Id = "VOSpeakerIcon";
		Add(_speakerIcon);
		SetCharacter(_speaker);
		_speakerLabel = GUIControl.CreateControl<GUILabel>(SPEAKER_LABEL_SIZE, new Vector2(50f, 10f), DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
		_speakerLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 17, GUILabel.GenColor(16, 31, 58), TextAnchor.UpperLeft);
		_speakerLabel.Id = "VOSpeakerLabel";
		_speakerLabel.WordWrap = true;
		_speakerLabel.Overflow = false;
		_speakerLabel.NoLineLimit = true;
		Add(_speakerLabel);
		SetText(_speakerText);
		AddCloseButton();
		return base.InitializeResources(reload);
	}

	protected void AddCloseButton()
	{
		Vector2 vector = new Vector2(0.8f, 0.8f);
		_closeBlocker = new GUISimpleControlWindow();
		_closeBlocker.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, CLOSE_BUTTON_OFFSET);
		_closeBlocker.SetSize(CLOSE_BUTTON_SIZE);
		_closeBlocker.HitTestType = HitTestTypeEnum.Circular;
		_closeBlocker.HitTestSize = vector;
		_closeBlocker.BlockTestType = BlockTestTypeEnum.Circular;
		_closeBlocker.BlockTestSize = vector;
		_closeBlocker.IsVisible = false;
		Add(_closeBlocker);
		_closeButton = new GUITBCloseButton();
		_closeButton.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, CLOSE_BUTTON_OFFSET);
		_closeButton.SetSize(CLOSE_BUTTON_SIZE);
		_closeButton.BlockTestType = BlockTestTypeEnum.Circular;
		_closeButton.BlockTestSize = vector;
		_closeButton.Click += OnCloseButtonClicked;
		Add(_closeButton);
	}

	protected void OnCloseButtonClicked(GUIControl sender, GUIClickEvent EventData)
	{
		if (_vo != null)
		{
			VOManager.Instance.Stop(_vo);
		}
		else
		{
			AnimateOut();
		}
	}

	public void SetVO(ResolvedVOAction vo)
	{
		_vo = vo;
	}

	public void SetCharacter(string characterID)
	{
		_speaker = characterID;
		if (_speakerIcon != null && !string.IsNullOrEmpty(_speaker))
		{
			_speakerIcon.TextureSource = "characters_bundle|" + _speaker + "_HUD_default";
		}
	}

	public void SetText(string textID)
	{
		ClearText();
		_speakerText = textID;
		if (string.IsNullOrEmpty(_speakerText))
		{
			return;
		}
		_speakerLabel.Text = _speakerText;
		_speakerLabel.CalculateTextLayout();
		if (_speakerLabel.LineCount > 4)
		{
			if (_speakerLabel.LineCount > 5)
			{
				ExpandWindow(306f);
			}
			else
			{
				ExpandWindow(_speakerLabel.GetLineWidth(4));
			}
		}
	}

	public void AnimateIn()
	{
		IsVisible = true;
	}

	public void AnimateOut()
	{
		IsVisible = false;
		AppShell.Instance.EventMgr.Fire(this, new BrawlerMissionBriefCompleteMessage());
	}

	private void ClearText()
	{
		if (_speakerLabel != null)
		{
			_speakerLabel.Text = string.Empty;
			_speakerLabel.InvalidateKerning();
			_speakerLabel.VerticalKerning = 18;
			SetSize(WINDOW_SIZE);
			if (_bgMiddle != null)
			{
				_bgMiddle.SetSize(BG_MIDDLE_SIZE);
			}
			if (_speakerLabel != null)
			{
				_speakerLabel.SetSize(SPEAKER_LABEL_SIZE);
			}
		}
	}

	private void ExpandWindow(float expandWidth)
	{
		if (!(expandWidth <= 0f))
		{
			expandWidth = Mathf.Min(expandWidth, 306f);
			Vector2 wINDOW_SIZE = WINDOW_SIZE;
			float x = wINDOW_SIZE.x + expandWidth;
			Vector2 wINDOW_SIZE2 = WINDOW_SIZE;
			SetSize(new Vector2(x, wINDOW_SIZE2.y));
			if (_speakerLabel != null)
			{
				GUILabel speakerLabel = _speakerLabel;
				Vector2 sPEAKER_LABEL_SIZE = SPEAKER_LABEL_SIZE;
				float width = sPEAKER_LABEL_SIZE.x + expandWidth;
				Vector2 sPEAKER_LABEL_SIZE2 = SPEAKER_LABEL_SIZE;
				speakerLabel.SetSize(width, sPEAKER_LABEL_SIZE2.y);
			}
			if (_bgMiddle != null)
			{
				GUIImage bgMiddle = _bgMiddle;
				Vector2 bG_MIDDLE_SIZE = BG_MIDDLE_SIZE;
				float width2 = bG_MIDDLE_SIZE.x + expandWidth;
				Vector2 bG_MIDDLE_SIZE2 = BG_MIDDLE_SIZE;
				bgMiddle.SetSize(width2, bG_MIDDLE_SIZE2.y);
			}
			_speakerLabel.ClearKerning();
			_speakerLabel.CalculateTextLayout();
		}
	}
}
