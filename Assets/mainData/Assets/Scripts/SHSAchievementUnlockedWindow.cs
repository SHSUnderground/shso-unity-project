using System.Collections;
using UnityEngine;

public class SHSAchievementUnlockedWindow : GUINotificationWindow
{
	private struct AchievementUIInfo
	{
		public int[] colors;

		public string toolTipText;

		public AchievementUIInfo(int[] colors, string toolTipText)
		{
			this.colors = colors;
			this.toolTipText = toolTipText;
		}
	}

	private const int textBlockOffset = 25;

	private const int medalOffset = 15;

	private const int textLineOffset = 4;

	private const float buttonShrinkPercentage = 0.67f;

	private const float okButtonOffset = 55f;

	private const float fadeStartTime = 3.75f;

	private const float fadeDurationTime = 2f;

	private Achievement achievement;

	private Achievement.AchievementLevelEnum achievementLevel;

	private GUIImage medalIcon;

	private GUILabel levelLabel;

	private GUILabel playerNameLabel;

	private GUILabel medalNameLabel;

	private GUIButton okButton;

	private NotificationBackground notificationBackground;

	private GUIImage backgroundLeft;

	private GUIImage backgroundRight;

	private GUIImage backgroundMiddle;

	private Hashtable achievementInfoLookup;

	public SHSAchievementUnlockedWindow(Achievement achievement, Achievement.AchievementLevelEnum prevLevel, Achievement.AchievementLevelEnum newLevel, string heroKey)
	{
		CspUtils.DebugLog("SHSAchievementUnlockedWindow wha?");
		this.achievement = achievement;
		achievementLevel = newLevel;
		achievementInfoLookup = new Hashtable();
		achievementInfoLookup[Achievement.AchievementLevelEnum.Adamantium] = new AchievementUIInfo(new int[3]
		{
			186,
			223,
			255
		}, string.Empty);
		achievementInfoLookup[Achievement.AchievementLevelEnum.Bronze] = new AchievementUIInfo(new int[3]
		{
			244,
			168,
			117
		}, string.Empty);
		achievementInfoLookup[Achievement.AchievementLevelEnum.Gold] = new AchievementUIInfo(new int[3]
		{
			255,
			219,
			20
		}, string.Empty);
		achievementInfoLookup[Achievement.AchievementLevelEnum.Silver] = new AchievementUIInfo(new int[3]
		{
			205,
			205,
			205
		}, string.Empty);
	}

	public override bool InitializeResources(bool reload)
	{
		if (reload)
		{
			return base.InitializeResources(reload);
		}
		AchievementUIInfo achievementUIInfo = (AchievementUIInfo)achievementInfoLookup[achievementLevel];
		GUIContent gUIContent = new GUIContent();
		float num = 0f;
		levelLabel = new GUILabel();
		levelLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(achievementUIInfo.colors[0], achievementUIInfo.colors[1], achievementUIInfo.colors[2]), TextAnchor.UpperLeft);
		levelLabel.Text = achievement.GetAchievementName(achievementLevel);
		levelLabel.IsVisible = true;
		playerNameLabel = new GUILabel();
		playerNameLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
		UserProfile profile = AppShell.Instance.Profile;
		playerNameLabel.Text = ((profile == null) ? "Not logged in" : AppShell.Instance.CharacterDescriptionManager[profile.LastSelectedCostume].CharacterName);
		playerNameLabel.IsVisible = true;
		medalNameLabel = new GUILabel();
		medalNameLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 17, GUILabel.GenColor(255, 249, 157), TextAnchor.UpperLeft);
		medalNameLabel.Text = achievement.GetNameForLevel(achievementLevel);
		medalNameLabel.IsVisible = true;
		notificationBackground = new NotificationBackground();
		notificationBackground.Build(GUINotificationWindow.GetTextBlockSize(new GUILabel[3]
		{
			levelLabel,
			playerNameLabel,
			medalNameLabel
		}, BlockSizeType.Width));
		backgroundLeft = notificationBackground.GetBackgroundPiece(NotificationBackground.PieceType.LeftPiece);
		backgroundMiddle = notificationBackground.GetBackgroundPiece(NotificationBackground.PieceType.MiddlePiece);
		backgroundRight = notificationBackground.GetBackgroundPiece(NotificationBackground.PieceType.RightPiece);
		medalIcon = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(64f, 64f), Vector2.zero);
		GUIImage gUIImage = medalIcon;
		Vector2 position = backgroundLeft.Position;
		float x = position.x;
		Vector2 position2 = backgroundLeft.Position;
		gUIImage.SetPosition(new Vector2(x, position2.y + 15f));
		medalIcon.SetSize(new Vector2(64f, 64f));
		medalIcon.TextureSource = "notification_bundle|" + achievement.Id + "_" + achievementLevel.ToString();
		okButton = new GUIButton();
		okButton.SetSize(new Vector2(85.76f, 85.76f));
		GUIButton gUIButton = okButton;
		Vector2 position3 = backgroundMiddle.Position;
		float x2 = position3.x;
		Vector2 size = backgroundMiddle.Size;
		float num2 = x2 + size.x * 0.5f;
		Vector2 size2 = okButton.Size;
		gUIButton.SetPosition(new Vector2(num2 - size2.x * 0.5f, 55f));
		okButton.StyleInfo = new SHSButtonStyleInfo("notification_bundle|simple_ok_button");
		okButton.Click += delegate
		{
			Hide();
		};
		Vector2 position4 = backgroundLeft.Position;
		float x3 = position4.x;
		Vector2 size3 = backgroundLeft.Size;
		float x4 = x3 + size3.x;
		Vector2 position5 = backgroundLeft.Position;
		Vector2 vector = new Vector2(x4, position5.y + 25f);
		gUIContent.text = playerNameLabel.Text;
		Vector2 vector2 = playerNameLabel.Style.UnityStyle.CalcSize(gUIContent);
		playerNameLabel.Position = new Vector2(vector.x, vector.y);
		playerNameLabel.SetSize(new Vector2(vector2.x, vector2.y));
		num += vector2.y;
		medalNameLabel.Position = new Vector2(vector.x, vector.y + num - 4f);
		gUIContent.text = medalNameLabel.Text;
		vector2 = medalNameLabel.Style.UnityStyle.CalcSize(gUIContent);
		medalNameLabel.SetSize(new Vector2(vector2.x, vector2.y));
		num += vector2.y;
		levelLabel.Position = new Vector2(vector.x, vector.y + num - 8f);
		gUIContent.text = levelLabel.Text;
		vector2 = levelLabel.Style.UnityStyle.CalcSize(gUIContent);
		levelLabel.SetSize(new Vector2(vector2.x, vector2.y));
		Add(backgroundLeft);
		Add(backgroundMiddle);
		Add(backgroundRight);
		Add(medalIcon);
		Add(playerNameLabel);
		Add(medalNameLabel);
		Add(levelLabel);
		Add(okButton);
		Vector2 position6 = okButton.Position;
		float y = position6.y;
		Vector2 size4 = okButton.Size;
		float num3 = y + size4.y;
		Vector2 size5 = backgroundLeft.Size;
		float num4 = num3 - size5.y;
		base.OccupiedSlot = GUINotificationWindow.SlotsManager.AssignSlot(GUINotificationManager.GUINotificationStyleEnum.AchievementNotify, this);
		SlotManager slotsManager = GUINotificationWindow.SlotsManager;
		int occupiedSlot = base.OccupiedSlot;
		Vector2 size6 = backgroundLeft.Size;
		slotsManager.AddOffset(occupiedSlot, size6.y + num4);
		Vector2 offset = new Vector2(0f, 0f - GUINotificationWindow.SlotsManager.GetCurrentOffset(base.OccupiedSlot));
		Vector2 size7 = backgroundLeft.Size;
		float x5 = size7.x;
		Vector2 size8 = backgroundMiddle.Size;
		float num5 = x5 + size8.x;
		Vector2 size9 = backgroundRight.Size;
		float x6 = num5 + size9.x;
		Vector2 size10 = backgroundLeft.Size;
		SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, offset, new Vector2(x6, size10.y + num4), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		return base.InitializeResources(reload);
	}

	protected override void dispose(bool disposing)
	{
		base.dispose(disposing);
		levelLabel = null;
		playerNameLabel = null;
		medalNameLabel = null;
		medalIcon = null;
		okButton = null;
		backgroundLeft = null;
		backgroundMiddle = null;
		backgroundRight = null;
		achievementInfoLookup = null;
		notificationBackground.Dispose();
		notificationBackground = null;
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		float num = Time.time - timeStarted;
		if (num > 3.75f)
		{
			Alpha = 1f - (num - 3.75f) / 2f;
		}
		if (Alpha <= 0f)
		{
			Hide();
		}
	}

	public override void OnShow()
	{
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("earn_achievement"));
	}

	public override void Hide()
	{
		if (!base.WindowOverwrite)
		{
			GUINotificationWindow.WindowHandler.ClearWindowOfType(GUINotificationManager.GUINotificationStyleEnum.AchievementNotify);
			GUINotificationWindow.SlotsManager.UnassignSlot(base.OccupiedSlot);
		}
		base.Hide();
	}
}
