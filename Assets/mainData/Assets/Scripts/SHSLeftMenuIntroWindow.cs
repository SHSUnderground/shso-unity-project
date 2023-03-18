using System;
using UnityEngine;

public class SHSLeftMenuIntroWindow : GUIControlWindow
{
	private GUIDrawTexture top;

	private GUIDrawTexture titleBar;

	private GUIDrawTexture bottom;

	private GUIDrawTexture stretch;

	private GUIButton finishLabel;

	private GUIButton finishButton;

	private GUIButton finishShadow;

	private GUILabel titleLabel;

	private GUILabel titleShadowLabel;

	private GUIButton nextButton;

	private GUIButton prevButton;

	private GUILabel descriptionLabel;

	private GUILabel descriptionUpperLabel;

	private GUIButton[] stepButtons;

	private MouseClickDelegate[] stepActions;

	protected string tutorialCounter = "TutorialCounter";

	private TutorialActionMessage.ActionTypeEnum currentAction;

	public TutorialActionMessage.ActionTypeEnum CurrentAction
	{
		get
		{
			return currentAction;
		}
		set
		{
			currentAction = value;
		}
	}

	public string TitleText
	{
		get
		{
			return titleLabel.Text;
		}
		set
		{
			titleLabel.Text = value;
			titleShadowLabel.Text = value;
		}
	}

	public string DescriptionText
	{
		get
		{
			return descriptionLabel.Text;
		}
		set
		{
			descriptionLabel.Text = value;
		}
	}

	public string UpperText
	{
		get
		{
			return descriptionUpperLabel.Text;
		}
		set
		{
			descriptionUpperLabel.Text = value;
		}
	}

	public string MovieSource
	{
		get
		{
			return string.Empty;
		}
		set
		{
		}
	}

	public bool IsNextVisible
	{
		get
		{
			return nextButton.IsVisible;
		}
		set
		{
			nextButton.IsVisible = value;
		}
	}

	public bool IsPrevVisible
	{
		get
		{
			return prevButton.IsVisible;
		}
		set
		{
			prevButton.IsVisible = value;
		}
	}

	public event MouseClickDelegate NextClick;

	public event MouseClickDelegate PrevClick;

	public SHSLeftMenuIntroWindow(string TutorialCounter)
	{
		tutorialCounter = TutorialCounter;
		Traits.ActivationTrait = ControlTraits.ActivationTraitEnum.Auto;
		Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Inherit;
		top = new GUIDrawTexture();
		top.Color = new Color(1f, 1f, 1f, 1f);
		top.IsVisible = true;
		top.TextureSource = "tutorial_bundle|tutorial_window_top";
		top.ScaleMode = ScaleMode.StretchToFill;
		Add(top);
		stretch = new GUIDrawTexture();
		stretch.Color = new Color(1f, 1f, 1f, 1f);
		stretch.IsVisible = true;
		stretch.TextureSource = "tutorial_bundle|tutorial_window_stretchfill";
		stretch.ScaleMode = ScaleMode.StretchToFill;
		Add(stretch);
		bottom = new GUIDrawTexture();
		bottom.Color = new Color(1f, 1f, 1f, 1f);
		bottom.IsVisible = true;
		bottom.TextureSource = "tutorial_bundle|tutorial_window_bottom";
		bottom.ScaleMode = ScaleMode.StretchToFill;
		Add(bottom);
		titleBar = new GUIDrawTexture();
		titleBar.Color = new Color(1f, 1f, 1f, 1f);
		titleBar.IsVisible = true;
		titleBar.TextureSource = "tutorial_bundle|tutorial_title_frame";
		titleBar.ScaleMode = ScaleMode.StretchToFill;
		Add(titleBar);
		titleShadowLabel = new GUILabel();
		titleShadowLabel.Id = "titleShadow";
		titleShadowLabel.Text = "Tutorial (Welcome!)";
		titleShadowLabel.TextAlignment = TextAnchor.MiddleCenter;
		titleShadowLabel.TextColor = Color.black;
		titleShadowLabel.FontSize = 26;
		titleShadowLabel.FontFace = GUIFontManager.SupportedFontEnum.Zooom;
		titleShadowLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		Add(titleShadowLabel);
		titleLabel = new GUILabel();
		titleLabel.Id = "title";
		titleLabel.Text = "Tutorial (Welcome!)";
		titleLabel.TextAlignment = TextAnchor.MiddleCenter;
		titleLabel.TextColor = new Color(1f, 1f, 1f, 1f);
		titleLabel.FontSize = 26;
		titleLabel.FontFace = GUIFontManager.SupportedFontEnum.Zooom;
		titleLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		Add(titleLabel);
		prevButton = new GUIButton();
		prevButton.Id = "prevButton";
		prevButton.StyleInfo = new SHSButtonStyleInfo("tutorial_bundle|L_button_back");
		prevButton.Click += delegate(GUIControl s, GUIClickEvent e)
		{
			this.PrevClick(s, e);
		};
		Add(prevButton);
		nextButton = new GUIButton();
		nextButton.Id = "nextButton";
		nextButton.StyleInfo = new SHSButtonStyleInfo("tutorial_bundle|L_button_skip");
		nextButton.Click += delegate(GUIControl s, GUIClickEvent e)
		{
			this.NextClick(s, e);
		};
		Add(nextButton);
		stepButtons = new GUIButton[8];
		stepActions = new MouseClickDelegate[8];
		for (int i = 0; i < 4; i++)
		{
			int num = i + 1;
			GUIButton gUIButton = new GUIButton();
			gUIButton.Id = "step button" + i.ToString();
			stepActions[i] = delegate
			{
			};
			Add(gUIButton);
			stepButtons[i] = gUIButton;
		}
		for (int i = 0; i < 4; i++)
		{
			int num2 = i + 5;
			GUIButton gUIButton2 = new GUIButton();
			gUIButton2.Id = "step button" + (i + 4).ToString();
			stepActions[i + 4] = delegate
			{
			};
			Add(gUIButton2);
			stepButtons[i + 4] = gUIButton2;
		}
		UpdateStepButtons();
		descriptionUpperLabel = new GUILabel();
		descriptionUpperLabel.Id = "descriptionUpper";
		descriptionUpperLabel.Text = string.Empty;
		descriptionUpperLabel.TextAlignment = TextAnchor.MiddleCenter;
		descriptionUpperLabel.TextColor = Color.white;
		descriptionUpperLabel.FontSize = 14;
		Add(descriptionUpperLabel);
		descriptionLabel = new GUILabel();
		descriptionLabel.Id = "description";
		descriptionLabel.Text = string.Empty;
		descriptionLabel.TextAlignment = TextAnchor.MiddleCenter;
		descriptionLabel.TextColor = Color.white;
		descriptionLabel.FontSize = 14;
		Add(descriptionLabel);
		EnableFinishButton(false);
	}

	protected void UpdateSize(Vector2 WindowSize)
	{
		SetSize(WindowSize, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Vector2 size = new Vector2(WindowSize.x, WindowSize.y);
		if (size.y > 167f)
		{
			size.y = 91f;
		}
		else
		{
			size.y = 91f * (size.y / 167f);
		}
		top.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), size, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Vector2 offset = new Vector2(0f, size.y);
		Vector2 size2 = new Vector2(WindowSize.x, WindowSize.y);
		if (size2.y > 167f)
		{
			size2.y -= 167f;
		}
		else
		{
			size2.y = 0f;
		}
		stretch.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, offset, size2, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Vector2 offset2 = new Vector2(0f, offset.y + size2.y);
		Vector2 size3 = new Vector2(WindowSize.x, WindowSize.y);
		if (size3.y > 167f)
		{
			size3.y = 76f;
		}
		else
		{
			size3.y = 76f * (size3.y / 167f);
		}
		bottom.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, offset2, size3, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		titleBar.SetPositionAndSize(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(286f, 75f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		titleShadowLabel.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(2f, 42f));
		titleShadowLabel.SetSize(230f, 100f);
		titleLabel.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 40f));
		titleLabel.SetSize(230f, 100f);
		prevButton.SetSize(128f, 128f);
		prevButton.SetPosition(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(-32f, 24f));
		nextButton.SetSize(128f, 128f);
		nextButton.SetPosition(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, OffsetType.Absolute, new Vector2(32f, 24f));
		for (int i = 0; i < 4; i++)
		{
			GUIButton gUIButton = stepButtons[i];
			gUIButton.SetPosition(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(56 + i * 32, -18f));
		}
		for (int i = 0; i < 4; i++)
		{
			GUIButton gUIButton2 = stepButtons[i + 4];
			gUIButton2.SetPosition(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(72 + i * 32, 8f));
		}
		float num = Mathf.Max(WindowSize.y - 304f, 0f);
		Vector2 offset3 = new Vector2(0f, offset.y + num / 3f);
		finishShadow.SetSize(256f, 256f);
		finishShadow.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, offset3);
		finishButton.SetSize(256f, 256f);
		finishButton.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, offset3);
		finishLabel.SetSize(256f, 256f);
		finishLabel.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, offset3);
		Vector2 offset4 = new Vector2(0f, offset.y - 7f);
		Vector2 offset5 = new Vector2(0f, offset3.y + Math.Max(num - 40f, 40f));
		float num2 = 0.7915567f;
		Vector2 size4 = new Vector2(WindowSize.x * num2, Mathf.Max(num / 3f + 9f, 82f));
		Vector2 size5 = new Vector2(WindowSize.x * num2, Mathf.Max(num * 2f / 3f, 145f));
		descriptionUpperLabel.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, offset4);
		descriptionUpperLabel.SetSize(size4);
		descriptionLabel.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, offset5);
		descriptionLabel.SetSize(size5);
	}

	public void UpdateStepButtons()
	{
		long num = 0L;
		ISHSCounterType counter = AppShell.Instance.CounterManager.GetCounter(tutorialCounter);
		if (counter != null)
		{
			num = counter.GetCurrentValue();
		}
		if (num > 8)
		{
			num = 8L;
		}
		long num2;
		for (num2 = 0L; num2 < num; num2++)
		{
			int num3 = (int)num2 + 1;
			GUIButton gUIButton = stepButtons[num2];
			gUIButton.StyleInfo = new SHSButtonStyleInfo("tutorial_bundle|tutorial_step" + num3.ToString() + "_green");
			gUIButton.SetSize(64f, 64f);
			gUIButton.HitTestType = HitTestTypeEnum.Circular;
			gUIButton.HitTestSize = new Vector2(0.5f, 0.5f);
			gUIButton.Click += stepActions[num2];
		}
		for (; num2 < 8; num2++)
		{
			int num4 = (int)num2 + 1;
			GUIButton gUIButton2 = stepButtons[num2];
			gUIButton2.StyleInfo = new SHSButtonStyleInfo("tutorial_bundle|tutorial_step" + num4.ToString() + "_blue");
			gUIButton2.SetSize(64f, 64f);
			gUIButton2.HitTestType = HitTestTypeEnum.Circular;
			gUIButton2.HitTestSize = new Vector2(0.5f, 0.5f);
			gUIButton2.Click -= stepActions[num2];
		}
	}

	public void EnableMovie(bool enabled)
	{
	}

	public void EnableFinishButton(bool enabled)
	{
		finishShadow.IsEnabled = enabled;
		finishShadow.IsVisible = enabled;
		finishButton.IsEnabled = enabled;
		finishButton.IsVisible = enabled;
		finishLabel.IsEnabled = enabled;
		finishLabel.IsVisible = enabled;
		if (enabled)
		{
			Vector2 windowSize = new Vector2(286f, 521f);
			UpdateSize(windowSize);
		}
		else
		{
			Vector2 windowSize2 = new Vector2(286f, 344f);
			UpdateSize(windowSize2);
		}
	}
}
