using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSDiagnosticsWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private float loadingDialogStartTime;

	private GUILabel TitleLabel;

	private int curHeight;

	private GUILabel NavLabel;

	private GUILabel HotControlLabel;

	private GUILabel CaptureStateLabel;

	private GUITextField notifyField;

	private GUILabel modalMode;

	private List<GUILabel> modalNameList;

	private GUILabel focusTestLabel;

	private bool UIDebugDrawCheckboxSelected;

	private bool UIHitTestCheckboxSelected;

	private bool UIDrawRegionCheckboxSelected;

	private bool UIHitTestRegionCheckboxSelected;

	private GUILayoutControlWindow leftWindow;

	private GUILayoutControlWindow midWindow;

	private GUILayoutControlWindow rightWindow;

	public static bool tooltipDebug;

	private GUILabel gameZoneLabel;

	private GUILabel gameCharacterLabel;

	private GUILabel gameTicketLabel;

	private GUILabel gameSpawnGroupLabel;

	private GUILabel gameServerTimeLabel;

	private GUILabel isShieldAgentLabel;

	public SHSDiagnosticsWindow(string PanelName)
		: base(PanelName, null)
	{
		SetBackground(new Color(0.3f, 0.5f, 0.8f, 0.2f));
		leftWindow = new GUILayoutControlWindow();
		leftWindow.SetBackground(new Color(1f, 0.5f, 0.5f, 1f));
		leftWindow.Margin = new Rect(20f, 20f, 20f, 20f);
		midWindow = new GUILayoutControlWindow();
		midWindow.SetBackground(new Color(0.5f, 0.5f, 0.5f, 1f));
		midWindow.Margin = new Rect(20f, 20f, 20f, 20f);
		rightWindow = new GUILayoutControlWindow();
		rightWindow.SetBackground(new Color(0.5f, 1.5f, 0.5f, 1f));
		rightWindow.Margin = new Rect(20f, 20f, 20f, 20f);
		Add(leftWindow);
		Add(midWindow);
		Add(rightWindow);
		leftWindow.Add(new GUILayoutBeginSection(GUILayoutBeginSection.OrientationEnum.Vertical, 250));
		TitleLabel = new GUILabel();
		TitleLabel.Size = new Vector2(200f, 20f);
		TitleLabel.Text = "UI Options";
		TitleLabel.Rotation = 0f;
		TitleLabel.IsVisible = true;
		TitleLabel.TooltipKey = string.Empty;
		TitleLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
		TitleLabel.Margin = new Rect(5f, 5f, 5f, 5f);
		leftWindow.Add(TitleLabel);
		GUIToggleButton gUIToggleButton = addCustomToggleButton("Master Debug Drawing Mode Toggle", leftWindow);
		gUIToggleButton.Changed += delegate
		{
			UIDebugDrawCheckboxSelected = !UIDebugDrawCheckboxSelected;
			bool settingAsBool4 = AppShell.Instance.DebugOptions.GetSettingAsBool("UI_DEBUG_DRAW");
			AppShell.Instance.DebugOptions.SetSetting("UI_DEBUG_DRAW", !settingAsBool4);
		};
		GUIToggleButton gUIToggleButton2 = addCustomToggleButton("Toggle UI Block Test Regions", leftWindow);
		gUIToggleButton2.Changed += delegate
		{
			UIHitTestCheckboxSelected = !UIHitTestCheckboxSelected;
			bool settingAsBool3 = AppShell.Instance.DebugOptions.GetSettingAsBool("UI_SHOW_HITTEST_REGIONS");
			AppShell.Instance.DebugOptions.SetSetting("UI_SHOW_HITTEST_REGIONS", !settingAsBool3);
		};
		GUIToggleButton gUIToggleButton3 = addCustomToggleButton("Toggle UI Draw Regions", leftWindow);
		gUIToggleButton3.Changed += delegate
		{
			UIDrawRegionCheckboxSelected = !UIDrawRegionCheckboxSelected;
			bool settingAsBool2 = AppShell.Instance.DebugOptions.GetSettingAsBool("UI_SHOW_DRAW_REGIONS");
			AppShell.Instance.DebugOptions.SetSetting("UI_SHOW_DRAW_REGIONS", !settingAsBool2);
			GUIManager.Instance.DebugDrawFlags.Set(Convert.ToInt32(DebugDrawSetting.ShowDrawRegions), !settingAsBool2);
		};
		GUIToggleButton gUIToggleButton4 = addCustomToggleButton("Toggle HitTest Draw Regions", leftWindow);
		gUIToggleButton4.Changed += delegate
		{
			UIHitTestRegionCheckboxSelected = !UIHitTestRegionCheckboxSelected;
			bool settingAsBool = AppShell.Instance.DebugOptions.GetSettingAsBool("UI_SHOW_HIT_TEST_DRAW_REGIONS");
			AppShell.Instance.DebugOptions.SetSetting("UI_SHOW_HIT_TEST_DRAW_REGIONS", !settingAsBool);
			GUIManager.Instance.DebugDrawFlags.Set(Convert.ToInt32(DebugDrawSetting.ShowHitTestRegions), !settingAsBool);
		};
		GUILabel gUILabel = new GUILabel();
		gUILabel.Size = new Vector2(200f, 20f);
		gUILabel.Text = "Layer View Options";
		gUILabel.Rotation = 0f;
		gUILabel.Color = new Color(1f, 1f, 1f, 1f);
		gUILabel.IsVisible = true;
		gUILabel.TooltipKey = string.Empty;
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
		gUILabel.Margin = new Rect(5f, 5f, 5f, 5f);
		leftWindow.Add(gUILabel);
		GUIToggleButton LayerSelectAll = addCustomToggleButton("All", leftWindow);
		GUIToggleButton LayerSelectStacked = addCustomToggleButton("Stacked", leftWindow);
		GUIToggleButton LayerSelectTop = addCustomToggleButton("Top", leftWindow);
		LayerSelectAll.Changed += delegate
		{
			GUIManager.drawTypeLayer = GUIManager.VisulizerLayering.All;
			LayerSelectStacked.Value = false;
			LayerSelectTop.Value = false;
		};
		LayerSelectStacked.Changed += delegate
		{
			GUIManager.drawTypeLayer = GUIManager.VisulizerLayering.Stacked;
			LayerSelectAll.Value = false;
			LayerSelectTop.Value = false;
		};
		LayerSelectTop.Changed += delegate
		{
			GUIManager.drawTypeLayer = GUIManager.VisulizerLayering.Top;
			LayerSelectAll.Value = false;
			LayerSelectStacked.Value = false;
		};
		GUIManager.drawTypeLayer = GUIManager.VisulizerLayering.All;
		LayerSelectAll.Value = true;
		GUIToggleButton simulateTutorialMode = addCustomToggleButton("Menu Chat 2.0", leftWindow);
		simulateTutorialMode.Value = (PlayerPrefs.GetInt("MenuChat20Enabled", 0) == 1);
		simulateTutorialMode.Changed += delegate
		{
			bool value7 = simulateTutorialMode.Value;
			PlayerPrefs.SetInt("MenuChat20Enabled", value7 ? 1 : 0);
			CspUtils.DebugLog("Menu Chat 2.0 set to " + value7);
		};
		leftWindow.Add(simulateTutorialMode);
		GUIButton gUIButton = new GUIButton();
		gUIButton.Size = new Vector2(90f, 25f);
		gUIButton.Text = "Trigger Bundle Group Load ";
		gUIButton.Rotation = 0f;
		gUIButton.Color = new Color(1f, 1f, 1f, 1f);
		gUIButton.IsVisible = true;
		gUIButton.StyleName = "Unstyled";
		gUIButton.Margin = new Rect(5f, 5f, 5f, 5f);
		gUIButton.Click += delegate
		{
			BundleGroupLoadedMessage msg = new BundleGroupLoadedMessage(AssetBundleLoader.BundleGroup.HQ, true);
			AppShell.Instance.EventMgr.Fire(this, msg);
		};
		leftWindow.Add(gUIButton);
		SHSCardGameLoadSaveDialog windowRef = null;
		GUIButton gUIButton2 = new GUIButton();
		gUIButton2.Size = new Vector2(90f, 25f);
		gUIButton2.Text = "LOAD DECK";
		gUIButton2.Rotation = 0f;
		gUIButton2.Color = new Color(1f, 1f, 1f, 1f);
		gUIButton2.IsVisible = true;
		gUIButton2.StyleName = "Unstyled";
		gUIButton2.Margin = new Rect(5f, 5f, 5f, 5f);
		gUIButton2.Click += delegate
		{
			GUIManager.Instance.ShowDialog(typeof(SHSCardGameLoadSaveDialog), string.Empty, new GUIDialogNotificationSink(delegate(string Id, IGUIDialogWindow window)
			{
				windowRef = (window as SHSCardGameLoadSaveDialog);
				((SHSCardGameLoadSaveDialog)window).CurrentMode = SHSCardGameLoadSaveDialog.DialogMode.Load;
			}, delegate
			{
			}, delegate
			{
			}, delegate
			{
			}, delegate(string Id, GUIDialogWindow.DialogState state)
			{
				if (state != GUIDialogWindow.DialogState.Cancel && windowRef != null)
				{
					CspUtils.DebugLog("Saving..." + windowRef.SelectedDeck.DeckName);
				}
			}), ModalLevelEnum.Default);
		};
		leftWindow.Add(gUIButton2);
		leftWindow.Add(new GUILayoutEndSection(GUILayoutEndSection.OrientationEnum.Vertical));
		midWindow.Add(new GUILayoutBeginSection(GUILayoutBeginSection.OrientationEnum.Vertical, 250));
		NavLabel = new GUILabel();
		NavLabel.Size = new Vector2(110f, 15f);
		NavLabel.Text = "Navigation Options";
		NavLabel.Rotation = 0f;
		NavLabel.IsVisible = true;
		NavLabel.Margin = new Rect(5f, 5f, 5f, 5f);
		NavLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
		midWindow.Add(NavLabel);
		midWindow.Add(new GUILayoutBeginSection(GUILayoutBeginSection.OrientationEnum.Horizontal));
		GUIButton gUIButton3 = new GUIButton();
		gUIButton3.Size = new Vector2(90f, 25f);
		gUIButton3.Text = "Options";
		gUIButton3.Rotation = 0f;
		gUIButton3.Color = new Color(1f, 1f, 1f, 1f);
		gUIButton3.IsVisible = true;
		gUIButton3.TooltipKey = string.Empty;
		gUIButton3.StyleName = "Unstyled";
		gUIButton3.Margin = new Rect(5f, 5f, 5f, 5f);
		gUIButton3.Click += delegate
		{
			GUIManager.Instance.Root["SHSMainWindow/SHSSystemMainWindow/SHSSysOptionsWindow"].Show(ModalLevelEnum.Default);
		};
		midWindow.Add(gUIButton3);
		midWindow.Add(new GUILayoutEndSection(GUILayoutEndSection.OrientationEnum.Horizontal));
		notifyField = new GUITextField();
		notifyField.Size = new Vector2(130f, 25f);
		notifyField.Text = "Daily Bugle Zone";
		midWindow.Add(notifyField);
		GUIButton gUIButton4 = new GUIButton();
		gUIButton4.Size = new Vector2(90f, 25f);
		gUIButton4.Text = "Arcade Window";
		gUIButton4.Rotation = 0f;
		gUIButton4.Color = new Color(1f, 1f, 1f, 1f);
		gUIButton4.IsVisible = true;
		gUIButton4.TooltipKey = string.Empty;
		gUIButton4.StyleName = "Unstyled";
		gUIButton4.Margin = new Rect(5f, 5f, 5f, 5f);
		gUIButton4.Click += delegate
		{
			LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.CardGame);
		};
		midWindow.Add(gUIButton4);
		GUIButton gUIButton5 = new GUIButton();
		gUIButton5.Size = new Vector2(90f, 25f);
		gUIButton5.Text = "ToggleUI";
		gUIButton5.Rotation = 0f;
		gUIButton5.Color = new Color(1f, 1f, 1f, 1f);
		gUIButton5.IsVisible = true;
		gUIButton5.StyleName = "Unstyled";
		gUIButton5.Margin = new Rect(5f, 5f, 5f, 5f);
		gUIButton5.Click += delegate
		{
			GUIManager.Instance.DrawingEnabled = !GUIManager.Instance.DrawingEnabled;
		};
		midWindow.Add(gUIButton5);
		midWindow.Add(new GUILayoutBeginSection(GUILayoutBeginSection.OrientationEnum.Horizontal, 300));
		GUIButton gUIButton6 = new GUIButton();
		gUIButton6.Size = new Vector2(70f, 25f);
		gUIButton6.Text = "Ok Cancel";
		gUIButton6.Rotation = 0f;
		gUIButton6.Color = new Color(1f, 1f, 1f, 1f);
		gUIButton6.IsVisible = true;
		gUIButton6.StyleName = "Unstyled";
		gUIButton6.Margin = new Rect(5f, 5f, 5f, 5f);
		gUIButton6.Click += delegate
		{
			GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkCancelDialog, "OkCancel Dialog", new GUIDialogNotificationSink(delegate
			{
			}, null, null, null, null), ModalLevelEnum.None);
		};
		midWindow.Add(gUIButton6);
		GUIButton gUIButton7 = new GUIButton();
		gUIButton7.Size = new Vector2(70f, 25f);
		gUIButton7.Text = "Ok";
		gUIButton7.Rotation = 0f;
		gUIButton7.Color = new Color(1f, 1f, 1f, 1f);
		gUIButton7.IsVisible = true;
		gUIButton7.StyleName = "Unstyled";
		gUIButton7.Margin = new Rect(5f, 5f, 5f, 5f);
		gUIButton7.Click += delegate
		{
			GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, "Ok Dialog", new GUIDialogNotificationSink(delegate
			{
			}, delegate
			{
				CspUtils.DebugLog("Shown");
			}, delegate
			{
				CspUtils.DebugLog("Closing");
			}, delegate
			{
				CspUtils.DebugLog("Cancelled");
			}, delegate
			{
				CspUtils.DebugLog("Closed");
			}), ModalLevelEnum.Default);
		};
		midWindow.Add(gUIButton7);
		GUIButton gUIButton8 = new GUIButton();
		gUIButton8.Size = new Vector2(70f, 25f);
		gUIButton8.Text = "Yes/No";
		gUIButton8.Rotation = 0f;
		gUIButton8.Color = new Color(1f, 1f, 1f, 1f);
		gUIButton8.IsVisible = true;
		gUIButton8.StyleName = "Unstyled";
		gUIButton8.Margin = new Rect(5f, 5f, 5f, 5f);
		gUIButton8.Click += delegate
		{
			GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, "Yes/No Dialog", new GUIDialogNotificationSink(delegate
			{
			}, delegate
			{
				CspUtils.DebugLog("Shown");
			}, delegate
			{
				CspUtils.DebugLog("Closing");
			}, delegate
			{
				CspUtils.DebugLog("Cancelled");
			}, delegate
			{
				CspUtils.DebugLog("Closed");
			}), ModalLevelEnum.None);
		};
		midWindow.Add(gUIButton8);
		GUIButton gUIButton9 = new GUIButton();
		gUIButton9.Size = new Vector2(70f, 25f);
		gUIButton9.Text = "Error";
		gUIButton9.Rotation = 0f;
		gUIButton9.Color = new Color(1f, 1f, 1f, 1f);
		gUIButton9.IsVisible = true;
		gUIButton9.StyleName = "Unstyled";
		gUIButton9.Margin = new Rect(5f, 5f, 5f, 5f);
		gUIButton9.Click += delegate
		{
			AppShell.Instance.CriticalError(SHSErrorCodes.Code.OutOfMemory);
		};
		midWindow.Add(gUIButton9);
		GUIButton gUIButton10 = new GUIButton();
		gUIButton10.Size = new Vector2(70f, 25f);
		gUIButton10.Text = "Loading";
		gUIButton10.Rotation = 0f;
		gUIButton10.Color = new Color(1f, 1f, 1f, 1f);
		gUIButton10.IsVisible = true;
		gUIButton10.StyleName = "Unstyled";
		gUIButton10.Margin = new Rect(5f, 5f, 5f, 5f);
		gUIButton10.Click += delegate
		{
			GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.ProgressDialog, "Retrieving results of your epic conquest from the server. Please wait...", new GUIDialogNotificationSink(delegate
			{
				loadingDialogStartTime = Time.time;
			}, delegate
			{
				CspUtils.DebugLog("Shown");
			}, delegate
			{
				CspUtils.DebugLog("Closing");
			}, delegate
			{
				CspUtils.DebugLog("Cancelled");
			}, delegate
			{
				CspUtils.DebugLog("Closed");
			}, delegate
			{
				return (!(Time.time - loadingDialogStartTime > 2f)) ? GUIDialogWindow.DialogUpdateState.InProgress : GUIDialogWindow.DialogUpdateState.Complete;
			}), ModalLevelEnum.None);
		};
		midWindow.Add(gUIButton10);
		midWindow.Add(new GUILayoutEndSection(GUILayoutEndSection.OrientationEnum.Horizontal));
		GUIToggleButton serverDisplayCheckbox = addCustomToggleButton("Toggle Server Name", midWindow);
		serverDisplayCheckbox.Value = (PlayerPrefs.GetInt("serverNameDisplay", 0) == 1);
		serverDisplayCheckbox.Changed += delegate
		{
			bool value6 = serverDisplayCheckbox.Value;
			PlayerPrefs.SetInt("serverNameDisplay", value6 ? 1 : 0);
			GUIManager.Instance.GetRoot(GUIManager.UILayer.Debug)["SHSDebugWindow/ServerName"].IsVisible = value6;
		};
		GUIToggleButton emoteInterruptCheckbox = addCustomToggleButton("Allow Emote Interrupts", midWindow);
		emoteInterruptCheckbox.Value = (PlayerPrefs.GetInt("emoteInterrupt", 0) == 1);
		emoteInterruptCheckbox.Changed += delegate
		{
			bool value5 = emoteInterruptCheckbox.Value;
			PlayerPrefs.SetInt("emoteInterrupt", value5 ? 1 : 0);
		};
		GUIToggleButton fpsDisplayCheckbox = addCustomToggleButton("Toggle FPS Display", midWindow);
		fpsDisplayCheckbox.Value = (PlayerPrefs.GetInt("fpsNameDisplay", 0) == 1);
		fpsDisplayCheckbox.Changed += delegate
		{
			bool value4 = fpsDisplayCheckbox.Value;
			PlayerPrefs.SetInt("fpsNameDisplay", value4 ? 1 : 0);
			GUIManager.Instance.GetRoot(GUIManager.UILayer.Debug)["SHSDebugWindow/FrameRateLabel"].IsVisible = value4;
			GUIManager.Instance.GetRoot(GUIManager.UILayer.Debug)["SHSDebugWindow/FrameRateLabel2"].IsVisible = value4;
		};
		GUIToggleButton fpsLoggingCheckbox = addCustomToggleButton("Toggle FPS Logging", midWindow);
		fpsLoggingCheckbox.Changed += delegate
		{
			bool value3 = fpsLoggingCheckbox.Value;
			SHSDebugWindow sHSDebugWindow3 = GUIManager.Instance.GetRoot(GUIManager.UILayer.Debug)["SHSDebugWindow"] as SHSDebugWindow;
			if (sHSDebugWindow3 != null)
			{
				sHSDebugWindow3.IsFrameRateLogging = value3;
			}
		};
		GUIToggleButton gUIToggleButton5 = addCustomToggleButton("Reset Global Min FPS", midWindow);
		gUIToggleButton5.Changed += delegate
		{
			SHSDebugWindow sHSDebugWindow2 = GUIManager.Instance.GetRoot(GUIManager.UILayer.Debug)["SHSDebugWindow"] as SHSDebugWindow;
			if (sHSDebugWindow2 != null)
			{
				sHSDebugWindow2.OnGlobalMinFpsReset();
			}
			else
			{
				CspUtils.DebugLog("Unable to find the SHSDebugWindow to reset the global min FPS counter.");
			}
		};
		GUIToggleButton playerPosAndRotCheckbox = addCustomToggleButton("Toggle Player Position & Rotation", midWindow);
		playerPosAndRotCheckbox.Value = (PlayerPrefs.GetInt("playerPosAndRotLabelDisplay", 0) == 1);
		playerPosAndRotCheckbox.Changed += delegate
		{
			bool value2 = playerPosAndRotCheckbox.Value;
			PlayerPrefs.SetInt("playerPosAndRotLabelDisplay", value2 ? 1 : 0);
			SHSDebugWindow sHSDebugWindow = GUIManager.Instance.GetRoot(GUIManager.UILayer.Debug)["SHSDebugWindow"] as SHSDebugWindow;
			if (sHSDebugWindow != null)
			{
				GUIManager.Instance.GetRoot(GUIManager.UILayer.Debug)["SHSDebugWindow/PlayerPositionLabel"].IsVisible = value2;
				GUIManager.Instance.GetRoot(GUIManager.UILayer.Debug)["SHSDebugWindow/PlayerRotationLabel"].IsVisible = value2;
			}
		};
		GUIToggleButton labelBoundsCheck = addCustomToggleButton("Label Bounds Checking", midWindow);
		labelBoundsCheck.Value = (PlayerPrefs.GetInt("lbChecking", 0) == 1);
		labelBoundsCheck.Changed += delegate
		{
			bool value = labelBoundsCheck.Value;
			PlayerPrefs.SetInt("lbChecking", value ? 1 : 0);
			GUIManager.Instance.Diagnostics.EnableLabelBoundsChecking = value;
		};
		GUIButton gUIButton11 = new GUIButton();
		gUIButton11.Size = new Vector2(90f, 25f);
		gUIButton11.Text = "Texture Report";
		gUIButton11.Rotation = 0f;
		gUIButton11.Color = new Color(1f, 1f, 1f, 1f);
		gUIButton11.IsVisible = true;
		gUIButton11.StyleName = "Unstyled";
		gUIButton11.Margin = new Rect(5f, 5f, 5f, 5f);
		gUIButton11.Click += delegate
		{
			GUIManager.Instance.Diagnostics.LogTextureReport();
		};
		midWindow.Add(gUIButton11);
		GUIButton gUIButton12 = new GUIButton();
		gUIButton12.Size = new Vector2(90f, 25f);
		gUIButton12.Text = "GUI Update Report";
		gUIButton12.Rotation = 0f;
		gUIButton12.Color = new Color(1f, 1f, 1f, 1f);
		gUIButton12.IsVisible = true;
		gUIButton12.StyleName = "Unstyled";
		gUIButton12.Margin = new Rect(5f, 5f, 5f, 5f);
		gUIButton12.Click += delegate
		{
			GUIManager.Instance.Diagnostics.LogUpdateReport();
		};
		midWindow.Add(gUIButton12);
		GUIButton gUIButton13 = new GUIButton();
		gUIButton13.Size = new Vector2(90f, 25f);
		gUIButton13.Text = "GUI Asset Xref Report";
		gUIButton13.Rotation = 0f;
		gUIButton13.Color = new Color(1f, 1f, 1f, 1f);
		gUIButton13.IsVisible = true;
		gUIButton13.StyleName = "Unstyled";
		gUIButton13.Margin = new Rect(5f, 5f, 5f, 5f);
		gUIButton13.Click += delegate
		{
			GUIManager.Instance.Diagnostics.GUIAssetComparison();
		};
		midWindow.Add(gUIButton13);
		GUIButton gUIButton14 = new GUIButton();
		gUIButton14.Size = new Vector2(90f, 25f);
		gUIButton14.Text = "GUI Asset Xref Report Cumulative";
		gUIButton14.Rotation = 0f;
		gUIButton14.Color = new Color(1f, 1f, 1f, 1f);
		gUIButton14.IsVisible = true;
		gUIButton14.StyleName = "Unstyled";
		gUIButton14.Margin = new Rect(5f, 5f, 5f, 5f);
		gUIButton14.Click += delegate
		{
			GUIManager.Instance.Diagnostics.GUIAssetComparisonCumulative();
		};
		midWindow.Add(gUIButton14);
		midWindow.Add(new GUILayoutEndSection(GUILayoutEndSection.OrientationEnum.Vertical));
		rightWindow.Add(new GUILayoutBeginSection(GUILayoutBeginSection.OrientationEnum.Vertical, 250));
		HotControlLabel = new GUILabel();
		HotControlLabel.Size = new Vector2(110f, 30f);
		HotControlLabel.Text = string.Empty;
		HotControlLabel.Rotation = 0f;
		HotControlLabel.Color = new Color(1f, 1f, 1f, 1f);
		HotControlLabel.IsVisible = true;
		HotControlLabel.Margin = new Rect(5f, 5f, 5f, 5f);
		HotControlLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
		rightWindow.Add(HotControlLabel);
		CaptureStateLabel = new GUILabel();
		CaptureStateLabel.Size = new Vector2(110f, 30f);
		CaptureStateLabel.Text = string.Empty;
		CaptureStateLabel.Rotation = 0f;
		CaptureStateLabel.Color = new Color(1f, 1f, 1f, 1f);
		CaptureStateLabel.IsVisible = true;
		CaptureStateLabel.Margin = new Rect(5f, 5f, 5f, 5f);
		CaptureStateLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
		rightWindow.Add(CaptureStateLabel);
		GUILabel gUILabel2 = new GUILabel();
		gUILabel2.Size = new Vector2(120f, 20f);
		gUILabel2.Text = "Game World Information";
		gUILabel2.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
		rightWindow.Add(gUILabel2);
		rightWindow.Add(new GUILayoutSpace(20));
		gameZoneLabel = new GUILabel();
		gameZoneLabel.Size = new Vector2(180f, 20f);
		gameZoneLabel.Text = "{No Zone}";
		gameZoneLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
		rightWindow.Add(gameZoneLabel);
		gameCharacterLabel = new GUILabel();
		gameCharacterLabel.Size = new Vector2(180f, 20f);
		gameCharacterLabel.Text = "{No Character}";
		gameCharacterLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
		rightWindow.Add(gameCharacterLabel);
		gameTicketLabel = new GUILabel();
		gameTicketLabel.Size = new Vector2(180f, 20f);
		gameTicketLabel.Text = "{No Ticket}";
		gameTicketLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
		rightWindow.Add(gameTicketLabel);
		gameSpawnGroupLabel = new GUILabel();
		gameSpawnGroupLabel.Size = new Vector2(180f, 20f);
		gameSpawnGroupLabel.Text = "{No SpawnGroup}";
		gameSpawnGroupLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
		rightWindow.Add(gameSpawnGroupLabel);
		gameServerTimeLabel = new GUILabel();
		gameServerTimeLabel.Size = new Vector2(180f, 20f);
		gameServerTimeLabel.Text = "{No ServerTime}";
		gameServerTimeLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
		rightWindow.Add(gameServerTimeLabel);
		isShieldAgentLabel = new GUILabel();
		isShieldAgentLabel.Size = new Vector2(180f, 20f);
		isShieldAgentLabel.Text = "{No Profile}";
		isShieldAgentLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
		rightWindow.Add(isShieldAgentLabel);
		GUILabel gUILabel3 = new GUILabel();
		gUILabel3.Size = new Vector2(180f, 20f);
		gUILabel3.Text = "MODAL MODE:";
		gUILabel3.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
		rightWindow.Add(gUILabel3);
		modalMode = new GUILabel();
		modalMode.Size = new Vector2(180f, 20f);
		modalMode.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
		rightWindow.Add(modalMode);
		rightWindow.Add(new GUILayoutSpace(10));
		focusTestLabel = new GUILabel();
		focusTestLabel.Size = new Vector2(120f, 20f);
		focusTestLabel.Text = "Has Focus?";
		focusTestLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
		rightWindow.Add(focusTestLabel);
		rightWindow.Add(new GUILayoutSpace(20));
		GUILabel gUILabel4 = new GUILabel();
		gUILabel4.Size = new Vector2(180f, 20f);
		gUILabel4.Text = "MODAL LIST:";
		gUILabel4.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
		rightWindow.Add(gUILabel4);
		modalNameList = new List<GUILabel>(10);
		for (int i = 0; i < 10; i++)
		{
			GUILabel gUILabel5 = new GUILabel();
			gUILabel5.Size = new Vector2(180f, 20f);
			gUILabel5.Text = string.Empty;
			gUILabel5.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
			rightWindow.Add(gUILabel5);
			modalNameList.Add(gUILabel5);
		}
		rightWindow.Add(new GUILayoutEndSection(GUILayoutEndSection.OrientationEnum.Vertical));
	}

	private GUIToggleButton addCustomToggleButton(string name, GUIWindow theWindowToAddItToo)
	{
		GUIToggleButton gUIToggleButton = new GUIToggleButton();
		gUIToggleButton.Text = name;
		gUIToggleButton.Spacing = 35f;
		gUIToggleButton.SetButtonSize(new Vector2(25f, 25f));
		gUIToggleButton.SetSize(240f, 25f);
		gUIToggleButton.Value = false;
		gUIToggleButton.Margin = new Rect(5f, 5f, 5f, 5f);
		theWindowToAddItToo.Add(gUIToggleButton);
		return gUIToggleButton;
	}

	public override void OnActive()
	{
		base.OnActive();
	}

	private int adjustCurHeight(int num)
	{
		int result = curHeight;
		curHeight += num;
		return result;
	}

	public override void DrawPreprocess()
	{
		base.DrawPreprocess();
		List<string> list = GUIManager.Instance.GetModalNameList();
		for (int i = 0; i < 10; i++)
		{
			modalNameList[i].Text = ((i < list.Count) ? list[i] : string.Empty);
		}
		list = null;
	}

	public override void OnUpdate()
	{
		gameZoneLabel.Text = string.Empty;
		gameTicketLabel.Text = string.Empty;
		gameCharacterLabel.Text = string.Empty;
		gameSpawnGroupLabel.Text = string.Empty;
		gameZoneLabel.Text = AppShell.Instance.SharedHashTable["SocialSpaceLevelCurrent"] + " (current) : " + AppShell.Instance.SharedHashTable["SocialSpaceLevel"] + " (queued)";
		gameTicketLabel.Text = AppShell.Instance.SharedHashTable["SocialSpaceTicketCurrent"] + " (current) : " + AppShell.Instance.SharedHashTable["SocialSpaceTicket"] + " (queued)";
		gameCharacterLabel.Text = AppShell.Instance.SharedHashTable["SocialSpaceCharacterCurrent"] + " (current) : " + AppShell.Instance.SharedHashTable["SocialSpaceCharacter"] + " (queued)";
		gameSpawnGroupLabel.Text = AppShell.Instance.SharedHashTable["SocialSpaceSpawnPoint"] + " (current)";
		gameServerTimeLabel.Text = "Server time: " + ServerTime.time;
		isShieldAgentLabel.Text = "Shield Agent? " + ((AppShell.Instance.Profile == null) ? "Not logged in" : Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldPlayAllow).ToString());
		HotControlLabel.Text = "HOT CTRL: " + GUIUtility.hotControl + Environment.NewLine + "HOT KEY: " + GUIUtility.keyboardControl;
		CaptureStateLabel.Text = "BLOCK FLAGS: " + SHSInput.BlockType;
		GUIManager gUIManager = null;
		if ((gUIManager = GUIManager.Instance) != null)
		{
			modalMode.Text = gUIManager.CurrentState.ToString();
		}
		focusTestLabel.Text = "Has Focus? : " + AppShell.Instance.HasFocus;
		base.OnUpdate();
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
		if (leftWindow != null && midWindow != null && rightWindow != null)
		{
			leftWindow.SetPositionAndSize(new Vector2(10f, 0f), DockingAlignmentEnum.None, AnchorAlignmentEnum.None, OffsetType.Absolute, Vector2.zero, new Vector2(0.3f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
			midWindow.SetPositionAndSize(new Vector2(Rect.width / 3f, 0f), DockingAlignmentEnum.None, AnchorAlignmentEnum.None, OffsetType.Absolute, Vector2.zero, new Vector2(0.3f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
			rightWindow.SetPositionAndSize(new Vector2(Rect.width / 3f * 2f, 0f), DockingAlignmentEnum.None, AnchorAlignmentEnum.None, OffsetType.Absolute, Vector2.zero, new Vector2(0.3f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		}
	}

	public override void DrawFinalize()
	{
		base.DrawFinalize();
	}
}
