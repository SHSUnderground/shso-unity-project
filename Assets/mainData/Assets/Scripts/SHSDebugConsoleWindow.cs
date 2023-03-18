using System;
using UnityEngine;

public class SHSDebugConsoleWindow : GUIControlWindow
{
	private GUITabbedDialogWindow debugDialog;

	private GUICursorManager.CursorType priorCursorType;

	private SHSLogWindow logwin;

	private GUIButton resizeButton;

	private bool dragMode;

	public SHSLogWindow LogWindow
	{
		get
		{
			return logwin;
		}
		set
		{
			logwin = value;
		}
	}

	public SHSDebugConsoleWindow()
	{
		id = "SHSDebugConsoleWindow";
		debugDialog = new GUITabbedDialogWindow(GUITabbedDialogWindow.TabbedDialogOrientationEnum.Horizontal);
		debugDialog.Id = "DebugDialog";
		debugDialog.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
		debugDialog.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		if (ShsPlayerPrefs.HasKey(ShsPlayerPrefs.Keys.DebugWindowSizeX))
		{
			SetSize(new Vector2(ShsPlayerPrefs.GetFloat(ShsPlayerPrefs.Keys.DebugWindowSizeX), ShsPlayerPrefs.GetFloat(ShsPlayerPrefs.Keys.DebugWindowSizeY)));
		}
		debugDialog.HitTestType = HitTestTypeEnum.Rect;
		debugDialog.BackgroundTextureSource = "debug_bundle|DebugWindowBackground";
		debugDialog.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Inherit;
		debugDialog.TabButtonNormalStyle = GUIManager.Instance.StyleManager.GetStyle("DebugTabStyleNormal");
		debugDialog.TabButtonSelectedStyle = GUIManager.Instance.StyleManager.GetStyle("DebugTabStyleSelected");
		Add(debugDialog);
		resizeButton = new GUIButton();
		resizeButton.Id = "resizeButton";
		resizeButton.SetPositionAndSize(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, OffsetType.Absolute, Vector2.zero, new Vector2(20f, 20f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		resizeButton.Text = string.Empty;
		resizeButton.Rotation = 0f;
		resizeButton.Color = new Color(1f, 1f, 1f, 1f);
		resizeButton.IsVisible = true;
		resizeButton.Style = GUIManager.Instance.StyleManager.GetStyle("ResizeControlIcon");
		resizeButton.MouseDown += delegate
		{
			dragMode = true;
		};
		resizeButton.MouseUp += delegate
		{
			ShsPlayerPrefs.SetFloat(ShsPlayerPrefs.Keys.DebugWindowSizeX, base.rect.width);
			ShsPlayerPrefs.SetFloat(ShsPlayerPrefs.Keys.DebugWindowSizeY, base.rect.height);
			dragMode = false;
		};
		Add(resizeButton);
		logwin = new SHSLogWindow("LogPanel");
		debugDialog.AddTab("Network", string.Empty, new SHSServersViewerWindow("Network"));
		debugDialog.AddTab("Command", string.Empty, new SHSCommandWindow("Command"));
		debugDialog.AddTab("Input", string.Empty, new SHSDebugInputViewerWindow("Input"));
		debugDialog.AddTab("Diag", string.Empty, new SHSDiagnosticsWindow("DiagWin"));
		debugDialog.AddTab("Keys", string.Empty, new SHSKeyboardViewerWindow("KeyWin"));
		debugDialog.AddTab("UI", string.Empty, new SHSUIViewerWindow("UIWin"));
		debugDialog.AddTab("Achievements", string.Empty, new SHSAchievementsWindow("Achievements"));
		debugDialog.AddTab("Counters", string.Empty, new SHSCountersWindow("Counters"));
		debugDialog.AddTab("Bundles", string.Empty, new SHSBundleViewerWindow("Bundles"));
		debugDialog.AddTab("Transactions", string.Empty, new SHSTransactionViewerWindow("Transactions"));
		debugDialog.AddTab("Events", string.Empty, new SHSEventViewerWindow("Events"));
		debugDialog.AddTab("Log", string.Empty, logwin);
		debugDialog.AddTab("CardGame", string.Empty, new SHSCardSADebugWindow("CardGame"));
		debugDialog.AddTab("SmartTips", string.Empty, new SHSSmartTipsViewerWindow("Smart Tips"));
		debugDialog.AddTab("Levels", string.Empty, new SHSLevelUpViewerWindow("Levels"));
		debugDialog.AddTab("Entitlements", string.Empty, new EntitlementsViewerWindow("Entitlements"));
		debugDialog.AddTab("Options", string.Empty, new SHSDebugOptionsWindow("Options", this));
		debugDialog.AddTab("Errors", string.Empty, new SHSErrorsWindow("Errors"));
		debugDialog.AddTab("Strings", string.Empty, new SHSStringTableViewerWindow("Strings"));
		debugDialog.AddTab("Challenges", string.Empty, new SHSChallengeViewerWindow("Challenges"));
		debugDialog.AddTab("Expendables", string.Empty, new SHSExpendablesWindow("Expendables"));
		debugDialog.AddTab("Combinations", string.Empty, new SHSCharacterCombinationViewerWindow("Combinations"));
	}

	private void preformDragResize()
	{
		if (dragMode)
		{
			Vector2 mouseScreenPosition = SHSInput.mouseScreenPosition;
			float x = Rect.x;
			float y = Rect.y;
			float num = mouseScreenPosition.x - Rect.x;
			Vector2 rectSize = resizeButton.RectSize;
			float width = Math.Max(Math.Min(num + rectSize.x / 2f, 1022f), 50f);
			float num2 = mouseScreenPosition.y - Rect.y;
			Vector2 rectSize2 = resizeButton.RectSize;
			SetPositionAndSize(x, y, width, Math.Max(Math.Min(num2 + rectSize2.y / 2f, 644f), 50f));
		}
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		preformDragResize();
		base.Draw(drawFlags);
	}

	public void SHSDebugConsoleWindowWindowFunction(int WindowId)
	{
		debugDialog.DrawPreprocess();
		debugDialog.Draw(DrawModeSetting.NormalMode);
		debugDialog.DrawFinalize();
		resizeButton.Draw(DrawModeSetting.NormalMode);
	}

	public override void OnActive()
	{
		base.OnActive();
	}

	public override void OnShow()
	{
		priorCursorType = GUIManager.Instance.CursorManager.CursorCurrentType;
		GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Native);
		if (debugDialog.SelectedTab == null)
		{
			debugDialog.SelectTab(0);
		}
		base.OnShow();
	}

	public override void OnHide()
	{
		GUIManager.Instance.CursorManager.SetCursorType(priorCursorType);
		GUIManager.Instance.ClearKeyboardFocus();
		base.OnHide();
	}
}
