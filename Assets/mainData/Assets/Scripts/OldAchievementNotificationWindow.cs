using System;
using System.Collections;
using UnityEngine;

public class OldAchievementNotificationWindow : NotificationWindow
{
	private const int textBlockOffset = 25;

	private const int medalOffset = 15;

	private const int textLineOffset = 4;

	private const float buttonShrinkPercentage = 0.67f;

	private const float okButtonOffset = 55f;

	private const float fadeStartTime = 3.75f;

	private const float fadeDurationTime = 2f;

	private GUIImage medalIcon;

	private GUILabel levelLabel;

	private GUILabel playerNameLabel;

	private GUILabel medalNameLabel;

	private GUIButton okButton;

	private GUIImage backgroundLeft;

	private GUIImage backgroundRight;

	private GUIImage backgroundMiddle;

	private static Hashtable achievementInfoLookup;

	public OldAchievementNotificationWindow()
		: base(NotificationData.NotificationType.OldAchievement)
	{
		Traits.HitTestType = HitTestTypeEnum.Rect;
		Traits.BlockTestType = BlockTestTypeEnum.Rect;
		SetControlFlag(ControlFlagSetting.HitTestIgnore, false, true);
		Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		Traits.UpdateTrait = ControlTraits.UpdateTraitEnum.AlwaysUpdate;
	}

	static OldAchievementNotificationWindow()
	{
		achievementInfoLookup = new Hashtable();
		achievementInfoLookup[Achievement.AchievementLevelEnum.Adamantium] = new Color(186f, 223f, 255f);
		achievementInfoLookup[Achievement.AchievementLevelEnum.Bronze] = new Color(244f, 168f, 117f);
		achievementInfoLookup[Achievement.AchievementLevelEnum.Gold] = new Color(255f, 219f, 20f);
		achievementInfoLookup[Achievement.AchievementLevelEnum.Silver] = new Color(205f, 205f, 205f);
	}

	public OldAchievementNotificationData getActualData()
	{
		return (OldAchievementNotificationData)_data;
	}

	public override void init(NotificationHUD parent, NotificationData data)
	{
		base.init(parent, data);
		Color color = (Color)achievementInfoLookup[getActualData().newLevel];
		GUIContent gUIContent = new GUIContent();
		float num = 0f;
		levelLabel = new GUILabel();
		levelLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, color, TextAnchor.UpperLeft);
		levelLabel.Text = getActualData().achievement.GetAchievementName(getActualData().newLevel);
		levelLabel.IsVisible = true;
		playerNameLabel = new GUILabel();
		playerNameLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
		UserProfile profile = AppShell.Instance.Profile;
		playerNameLabel.Text = ((profile == null) ? "Not logged in" : AppShell.Instance.CharacterDescriptionManager[getActualData().heroKey].CharacterName);
		playerNameLabel.IsVisible = true;
		medalNameLabel = new GUILabel();
		medalNameLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 17, GUILabel.GenColor(255, 249, 157), TextAnchor.UpperLeft);
		medalNameLabel.Text = getActualData().achievement.GetNameForLevel(getActualData().newLevel);
		medalNameLabel.IsVisible = true;
		backgroundLeft = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(64f, 128f), Vector2.zero);
		backgroundLeft.SetPosition(new Vector2(0f, 0f));
		backgroundLeft.TextureSource = "notification_bundle|achievement_frame_left";
		backgroundMiddle = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(GUINotificationWindow.GetTextBlockSize(new GUILabel[3]
		{
			levelLabel,
			playerNameLabel,
			medalNameLabel
		}, GUINotificationWindow.BlockSizeType.Width), 128f), Vector2.zero);
		GUIImage gUIImage = backgroundMiddle;
		Vector2 position = backgroundLeft.Position;
		float x = position.x;
		Vector2 size = backgroundLeft.Size;
		float x2 = x + size.x;
		Vector2 position2 = backgroundLeft.Position;
		gUIImage.SetPosition(new Vector2(x2, position2.y));
		backgroundMiddle.TextureSource = "notification_bundle|achievement_frame_center";
		backgroundRight = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(32f, 128f), Vector2.zero);
		GUIImage gUIImage2 = backgroundRight;
		Vector2 position3 = backgroundMiddle.Position;
		float x3 = position3.x;
		Vector2 size2 = backgroundMiddle.Size;
		float x4 = x3 + size2.x;
		Vector2 position4 = backgroundMiddle.Position;
		gUIImage2.SetPosition(new Vector2(x4, position4.y));
		backgroundRight.TextureSource = "notification_bundle|achievement_frame_right";
		medalIcon = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(64f, 64f), Vector2.zero);
		GUIImage gUIImage3 = medalIcon;
		Vector2 position5 = backgroundLeft.Position;
		float x5 = position5.x;
		Vector2 position6 = backgroundLeft.Position;
		gUIImage3.SetPosition(new Vector2(x5, position6.y + 15f));
		medalIcon.SetSize(new Vector2(64f, 64f));
		medalIcon.TextureSource = "notification_bundle|" + getActualData().achievement.Id + "_" + getActualData().newLevel.ToString();
		okButton = new GUIButton();
		okButton.SetSize(new Vector2(85.76f, 85.76f));
		GUIButton gUIButton = okButton;
		Vector2 position7 = backgroundMiddle.Position;
		float x6 = position7.x;
		Vector2 size3 = backgroundMiddle.Size;
		float num2 = x6 + size3.x * 0.5f;
		Vector2 size4 = okButton.Size;
		gUIButton.SetPosition(new Vector2(num2 - size4.x * 0.5f, 55f));
		okButton.StyleInfo = new SHSButtonStyleInfo("notification_bundle|simple_ok_button");
		okButton.Click += delegate
		{
			_parent.notificationComplete(this);
		};
		Vector2 position8 = backgroundLeft.Position;
		float x7 = position8.x;
		Vector2 size5 = backgroundLeft.Size;
		float x8 = x7 + size5.x;
		Vector2 position9 = backgroundLeft.Position;
		Vector2 vector = new Vector2(x8, position9.y + 25f);
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
		Vector2 position10 = okButton.Position;
		float y = position10.y;
		Vector2 size6 = okButton.Size;
		float num3 = y + size6.y;
		Vector2 size7 = backgroundLeft.Size;
		float num4 = num3 - size7.y;
		Vector2 size8 = backgroundLeft.Size;
		float x9 = size8.x;
		Vector2 size9 = backgroundMiddle.Size;
		float num5 = x9 + size9.x;
		Vector2 size10 = backgroundRight.Size;
		float x10 = num5 + size10.x;
		Vector2 size11 = backgroundLeft.Size;
		SetSize(new Vector2(x10, size11.y + num4), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
	}

	public override void activate()
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		Alpha = 0f;
		AnimClip animClip = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, 1f), this);
		AnimClip animClip2 = SHSAnimations.Generic.Wait(55f);
		AnimClip animClip3 = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(1f, 0f, 1f), this);
		base.animClip = animClip;
		base.animClip |= animClip2;
		base.animClip |= animClip3;
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			_fadeInComplete = true;
		};
		animClip3.OnFinished += (Action)(object)(Action)delegate
		{
			_parent.notificationComplete(this);
		};
		_parent.animManager.Add(base.animClip);
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("popup_alert"));
	}
}
