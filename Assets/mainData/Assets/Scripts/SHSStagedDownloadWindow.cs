using System.Collections.Generic;
using UnityEngine;

public class SHSStagedDownloadWindow : GUIDynamicWindow
{
	public class StageState
	{
		public AssetBundleLoader.BundleGroup assetBundleGroup;

		public bool completed;

		public float percent;

		public string stage;

		public string imagePath;

		public StageCompleteDelegate completeDelegate;

		public bool showState;
	}

	public delegate void StageCompleteDelegate(SHSStagedDownloadListWindow sender);

	private static bool downloadStatusCurrentlyShowing;

	private GUIImage backgroundImage;

	private GUIDropShadowTextLabel titleLabel;

	private GUIImage iconImage;

	private GUIButton closeButton;

	private GUILabel messageLabel;

	private GUIButton okButton;

	private static List<StageState> stageStatusList;

	private GUILayoutWindow layoutWin;

	public static bool DownloadStatusCurrentlyShowing
	{
		get
		{
			return downloadStatusCurrentlyShowing;
		}
	}

	public static List<StageState> StageStatusList
	{
		get
		{
			return stageStatusList;
		}
	}

	public SHSStagedDownloadWindow()
	{
		backgroundImage = new GUIImage();
		backgroundImage.SetPosition(QuickSizingHint.Centered);
		backgroundImage.SetSize(new Vector2(630f, 500f));
		backgroundImage.TextureSource = "gameworld_bundle|report_notification_background_large";
		backgroundImage.Offset = new Vector2(0f, 32f);
		Add(backgroundImage);
		titleLabel = new GUIDropShadowTextLabel();
		titleLabel.SetPositionAndSize(170f, 108f, 305f, 168f);
		titleLabel.FontFace = GUIFontManager.SupportedFontEnum.Zooom;
		titleLabel.FontSize = 29;
		titleLabel.TextAlignment = TextAnchor.UpperCenter;
		titleLabel.FrontColor = new Color(226f / 255f, 92f / 255f, 16f / 255f);
		titleLabel.BackColor = new Color(0f, 4f / 51f, 14f / 51f, 0.1f);
		titleLabel.TextOffset = new Vector2(2f, 2f);
		titleLabel.Text = "#download_state_title";
		Add(titleLabel);
		iconImage = new GUIImage();
		iconImage.SetPositionAndSize(-2f, 26f, 173f, 171f);
		iconImage.TextureSource = "common_bundle|notification_icon_downloading";
		Add(iconImage);
		closeButton = GUIControl.CreateControlAbsolute<GUIButton>(new Vector2(45f, 45f), new Vector2(569f, 82f));
		closeButton.Click += okButton_Click;
		closeButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|button_close");
		closeButton.ToolTip = new NamedToolTipInfo(SHSTooltip.GetCommonToolTipText(SHSTooltip.CommonToolTipText.Close));
		Add(closeButton);
		messageLabel = new GUILabel();
		messageLabel.SetPositionAndSize(170f, 137f, 350f, 160f);
		messageLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, new Color(26f / 85f, 32f / 85f, 109f / 255f), TextAnchor.UpperLeft);
		messageLabel.Text = string.Empty;
		Add(messageLabel);
		layoutWin = new GUILayoutWindow();
		layoutWin.SetPositionAndSize(80f, 197f, 460f, 390f);
		Add(new GUILayoutBeginSection(GUILayoutBeginSection.OrientationEnum.Vertical));
		int num = 0;
		foreach (StageState stageStatus in StageStatusList)
		{
			if (stageStatus.showState)
			{
				num++;
				Add(new GUILayoutBeginSection(GUILayoutBeginSection.OrientationEnum.Horizontal));
				SHSStagedDownloadListWindow control = new SHSStagedDownloadListWindow(stageStatus);
				layoutWin.Add(control);
				Add(new GUILayoutEndSection(GUILayoutEndSection.OrientationEnum.Horizontal));
			}
		}
		Add(new GUILayoutEndSection(GUILayoutEndSection.OrientationEnum.Vertical));
		layoutWin.SetPositionAndSize(80f, 197f, 460f, 390f);
		layoutWin.IsVisible = true;
		Add(layoutWin);
		okButton = new GUIButton();
		okButton.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(112f, 26f), new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		okButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|L_mshs_button_ok");
		okButton.Text = string.Empty;
		okButton.Click += okButton_Click;
		Add(okButton);
		SetPosition(QuickSizingHint.Centered);
		SetSize(new Vector2(650f, 310 + num * 39));
		backgroundImage.SetSize(new Vector2(630f, 260 + num * 39));
	}

	static SHSStagedDownloadWindow()
	{
		List<StageState> list = new List<StageState>();
		StageState stageState = new StageState();
		stageState.completed = true;
		stageState.percent = 1f;
		stageState.stage = "#stage_TutorialUIBugle";
		stageState.imagePath = "smarttip_bundle|worldmapTip";
		stageState.completeDelegate = delegate(SHSStagedDownloadListWindow sender)
		{
			CspUtils.DebugLog("Launching Daily Bugle");
			LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.WorldMap);
			if (sender != null)
			{
				sender.Parent.Parent.Hide();
			}
		};
		stageState.showState = true;
		list.Add(stageState);
		stageState = new StageState();
		stageState.assetBundleGroup = AssetBundleLoader.BundleGroup.PrizeWheel;
		stageState.stage = "#STAGE_ACTIVITIES";
		stageState.imagePath = "smarttip_bundle|worldmapTip";
		stageState.completeDelegate = delegate(SHSStagedDownloadListWindow sender)
		{
			CspUtils.DebugLog("Launching Prize Wheel");
			LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.WorldMap);
			if (sender != null)
			{
				sender.Parent.Parent.Hide();
			}
		};
		stageState.showState = true;
		list.Add(stageState);
		stageState = new StageState();
		stageState.assetBundleGroup = AssetBundleLoader.BundleGroup.NonBugleGameWorlds;
		stageState.stage = "#stage_NonBugleGameWorlds";
		stageState.imagePath = "smarttip_bundle|worldmapTip";
		stageState.completeDelegate = delegate(SHSStagedDownloadListWindow sender)
		{
			CspUtils.DebugLog("Launching NonBugleGameWorlds");
			LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.WorldMap);
			if (sender != null)
			{
				sender.Parent.Parent.Hide();
			}
		};
		stageState.showState = true;
		list.Add(stageState);
		stageState = new StageState();
		stageState.assetBundleGroup = AssetBundleLoader.BundleGroup.Characters;
		stageState.stage = "#stage_Shopping";
		stageState.imagePath = "smarttip_bundle|shoppingTip";
		stageState.completeDelegate = delegate(SHSStagedDownloadListWindow sender)
		{
			CspUtils.DebugLog("Launching Shopping");
			LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.Shopping);
			if (sender != null)
			{
				sender.Parent.Parent.Hide();
			}
		};
		stageState.showState = true;
		list.Add(stageState);
		stageState = new StageState();
		stageState.assetBundleGroup = AssetBundleLoader.BundleGroup.SpecialMission;
		stageState.stage = "#stage_SpecialMission";
		stageState.imagePath = "smarttip_bundle|brawlerTip";
		stageState.completeDelegate = delegate(SHSStagedDownloadListWindow sender)
		{
			CspUtils.DebugLog("Launching Special Mission");
			LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.Missions);
			if (sender != null)
			{
				sender.Parent.Parent.Hide();
			}
		};
		stageState.showState = true;
		list.Add(stageState);
		stageState = new StageState();
		stageState.assetBundleGroup = AssetBundleLoader.BundleGroup.MissionsAndEnemies;
		stageState.stage = "#stage_Missions";
		stageState.imagePath = "smarttip_bundle|brawlerTip";
		stageState.completeDelegate = delegate(SHSStagedDownloadListWindow sender)
		{
			CspUtils.DebugLog("Launching Missions");
			LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.Missions);
			if (sender != null)
			{
				sender.Parent.Parent.Hide();
			}
		};
		stageState.showState = true;
		list.Add(stageState);
		stageState = new StageState();
		stageState.assetBundleGroup = AssetBundleLoader.BundleGroup.HQ;
		stageState.stage = "#stage_HQ";
		stageState.imagePath = "smarttip_bundle|hqTip";
		stageState.completeDelegate = delegate(SHSStagedDownloadListWindow sender)
		{
			CspUtils.DebugLog("Launching HQ");
			LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.Hq);
			if (sender != null)
			{
				sender.Parent.Parent.Hide();
			}
		};
		stageState.showState = true;
		list.Add(stageState);
		stageState = new StageState();
		stageState.assetBundleGroup = AssetBundleLoader.BundleGroup.CardGame;
		stageState.stage = "#stage_CardGame";
		stageState.imagePath = "smarttip_bundle|cardgameTip";
		stageState.completeDelegate = delegate(SHSStagedDownloadListWindow sender)
		{
			CspUtils.DebugLog("Launching Card Game");
			LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.CardGame);
			if (sender != null)
			{
				sender.Parent.Parent.Hide();
			}
		};
		stageState.showState = true;
		list.Add(stageState);
		stageStatusList = list;
	}

	private void okButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		Hide();
	}

	public override void OnShow()
	{
		messageLabel.Text = "#download_state_message";
		base.OnShow();
		downloadStatusCurrentlyShowing = true;
		AppShell.Instance.BundleLoader.AggressiveBackgroundDownloading(true);
	}

	public override void OnHide()
	{
		downloadStatusCurrentlyShowing = false;
		AppShell.Instance.BundleLoader.AggressiveBackgroundDownloading(false);
		base.OnHide();
	}
}
