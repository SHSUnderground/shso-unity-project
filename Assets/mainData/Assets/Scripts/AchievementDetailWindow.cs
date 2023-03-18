using System;
using UnityEngine;

public class AchievementDetailWindow : GUISimpleControlWindow
{
	private GUIImageWithEvents _openHeader;

	private GUIImage _openFooter;

	private GUIImage _openFill;

	private GUIStrokeTextLabel _headerLabel;

	private GUIImage _pointsUnderlay;

	private GUIImage _pointsCoin;

	private GUIStrokeTextLabel _pointsLabel;

	private GUIImage _trackCheck;

	private GUISimpleControlWindow _headerContainer;

	private GUISimpleControlWindow _contentContainer;

	private GUISimpleControlWindow _footerContainer;

	public NewAchievement targetAchievement;

	public AchievementDetailWindow(PlayerAchievementData playerData, NewAchievement achievement)
	{
		targetAchievement = achievement;
		AchievementData data2 = playerData.getData(achievement.achievementID);
		_openHeader = new GUIImageWithEvents();
		_openHeader.SetSize(new Vector2(550f, 69f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_openHeader.Position = Vector2.zero;
		if (data2 != null && data2.complete == 1)
		{
			_openHeader.TextureSource = "achievement_bundle|detail_top_complete";
		}
		else
		{
			_openHeader.TextureSource = "achievement_bundle|detail_top";
		}
		_openHeader.IsVisible = true;
		Add(_openHeader);
		_openFill = new GUIImage();
		_openFill.SetSize(new Vector2(550f, 54f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIImage openFill = _openFill;
		Vector2 position = _openHeader.Position;
		Vector2 size = _openHeader.Size;
		openFill.Position = position + new Vector2(0f, size.y);
		_openFill.TextureSource = "achievement_bundle|detail_centerfill";
		_openFill.IsVisible = true;
		Add(_openFill);
		_openFooter = new GUIImage();
		_openFooter.SetSize(new Vector2(550f, 54f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_openFooter.Position = Vector2.zero;
		_openFooter.TextureSource = "achievement_bundle|detail_bottom_rewards";
		_openFooter.IsVisible = true;
		Add(_openFooter);
		_headerContainer = new GUISimpleControlWindow();
		_headerContainer.IsVisible = true;
		_headerContainer.SetSize(_openHeader.Size);
		_headerContainer.SetPosition(Vector2.zero);
		Add(_headerContainer);
		_headerLabel = new GUIStrokeTextLabel();
		_headerLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(10f, -5f), new Vector2(416f, 80f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_headerLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 24, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.MiddleLeft);
		_headerLabel.BackColorAlpha = 1f;
		_headerLabel.StrokeColorAlpha = 1f;
		_headerLabel.WordWrap = true;
		_headerLabel.VerticalKerning = 28;
		_headerLabel.Text = AchievementManager.formatAchievementString(achievement.displayName, 0, 0, string.Empty);
		_headerLabel.VerticalKerning = 28;
		_headerLabel.IsVisible = true;
		_headerContainer.Add(_headerLabel);
		_pointsUnderlay = new GUIImage();
		_pointsUnderlay.SetSize(new Vector2(84f, 46f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_pointsUnderlay.Position = new Vector2(436f, 11f);
		_pointsUnderlay.TextureSource = "achievement_bundle|score_underlay";
		_pointsUnderlay.IsVisible = true;
		_headerContainer.Add(_pointsUnderlay);
		_pointsCoin = new GUIImage();
		_pointsCoin.SetSize(new Vector2(50f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_pointsCoin.Position = _pointsUnderlay.Position + new Vector2(50f, -2f);
		_pointsCoin.TextureSource = "achievement_bundle|achievement_coin";
		_pointsCoin.IsVisible = true;
		_headerContainer.Add(_pointsCoin);
		_pointsLabel = new GUIStrokeTextLabel();
		GUIStrokeTextLabel pointsLabel = _pointsLabel;
		Vector2 position2 = _pointsUnderlay.Position;
		Vector2 size2 = _pointsUnderlay.Size;
		float x = size2.x * 0.7f;
		Vector2 size3 = _pointsUnderlay.Size;
		pointsLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, position2, new Vector2(x, size3.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_pointsLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 24, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(0f, 2f), TextAnchor.MiddleCenter);
		_pointsLabel.BackColorAlpha = 1f;
		_pointsLabel.StrokeColorAlpha = 1f;
		_pointsLabel.Text = string.Empty + achievement.achievementPoints;
		_pointsLabel.IsVisible = true;
		_headerContainer.Add(_pointsLabel);
		_contentContainer = new GUISimpleControlWindow();
		_contentContainer.IsVisible = true;
		_contentContainer.SetSize(_openHeader.Size);
		GUISimpleControlWindow contentContainer = _contentContainer;
		Vector2 size4 = _headerContainer.Size;
		contentContainer.SetPosition(new Vector2(0f, size4.y));
		Add(_contentContainer);
		int num = 537;
		Vector2 a = new Vector2(60f, 15f);
		int num2 = 0;
		if (data2 != null && data2.complete == 1)
		{
			GUILabel gUILabel = new GUILabel();
			gUILabel.FontSize = 12;
			gUILabel.FontFace = GUIFontManager.SupportedFontEnum.Komica;
			gUILabel.TextColor = GUILabel.GenColor(0, 0, 0);
			gUILabel.TextAlignment = TextAnchor.UpperRight;
			gUILabel.Text = AppShell.Instance.stringTable.GetString("#ACH_TRACK_COMPLETE") + " " + data2.completeDate.ToString("d MMM yyyy");
			gUILabel.SetPosition(new Vector2(num - 220, 0f));
			gUILabel.SetSize(220f, 20f);
			_contentContainer.Add(gUILabel);
		}
		else
		{
			bool flag = true;
			if (targetAchievement.track == AchievementManager.DestinyTracks.Valor && targetAchievement.achievementID <= 200050)
			{
				flag = false;
			}
			else if (targetAchievement.track != AchievementManager.DestinyTracks.None && !AchievementManager.playerOnAchievement(playerData.playerID, targetAchievement.achievementID))
			{
				flag = false;
			}
			if (flag)
			{
				GUIImageWithEvents gUIImageWithEvents = new GUIImageWithEvents();
				gUIImageWithEvents.SetSize(new Vector2(20f, 18f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				gUIImageWithEvents.Position = new Vector2(num - 150, 0f);
				gUIImageWithEvents.TextureSource = "achievement_bundle|objective_incomplete";
				gUIImageWithEvents.IsVisible = true;
				_contentContainer.Add(gUIImageWithEvents);
				gUIImageWithEvents.Click += delegate
				{
					if (_trackCheck.IsVisible)
					{
						AchievementManager.stopTrackingAchievement(achievement.achievementID);
					}
					else
					{
						AchievementManager.trackAchievement(achievement.achievementID);
					}
					_trackCheck.IsVisible = !_trackCheck.IsVisible;
				};
				_trackCheck = new GUIImage();
				_trackCheck.SetSize(new Vector2(18f, 20f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				_trackCheck.Position = gUIImageWithEvents.Position + new Vector2(0f, 0f);
				_trackCheck.TextureSource = "achievement_bundle|objective_check";
				_trackCheck.IsVisible = false;
				_contentContainer.Add(_trackCheck);
				if (AchievementManager.isTrackingAchievement(achievement.achievementID))
				{
					_trackCheck.IsVisible = true;
				}
				GUILabel gUILabel2 = new GUILabel();
				gUILabel2.FontSize = 12;
				gUILabel2.FontFace = GUIFontManager.SupportedFontEnum.Komica;
				gUILabel2.TextColor = GUILabel.GenColor(0, 0, 0);
				gUILabel2.TextAlignment = TextAnchor.UpperLeft;
				gUILabel2.Text = "#ACH_TRACK_THIS";
				Vector2 position3 = gUIImageWithEvents.Position;
				Vector2 size5 = gUIImageWithEvents.Size;
				gUILabel2.SetPosition(position3 + new Vector2(size5.x, 2f));
				gUILabel2.SetSize(200f, 20f);
				_contentContainer.Add(gUILabel2);
			}
		}
		GUILabel gUILabel3 = new GUILabel();
		gUILabel3.FontSize = 16;
		gUILabel3.FontFace = GUIFontManager.SupportedFontEnum.Komica;
		gUILabel3.TextColor = GUILabel.GenColor(0, 0, 0);
		gUILabel3.TextAlignment = TextAnchor.UpperLeft;
		gUILabel3.WordWrap = true;
		gUILabel3.Text = AchievementManager.formatAchievementString(achievement.displayDesc, 0, 0, string.Empty);
		gUILabel3.SetPosition(a + new Vector2(0f, num2));
		float num3 = num;
		Vector2 position4 = gUILabel3.Position;
		gUILabel3.SetSize(num3 - position4.x, 120f);
		_contentContainer.Add(gUILabel3);
		int num4 = (int)GUINotificationWindow.GetTextBlockSize(new GUILabel[1]
		{
			gUILabel3
		}, GUINotificationWindow.BlockSizeType.Height);
		num2 += num4 + 5;
		foreach (NewAchievementStep step in achievement.steps)
		{
			if (step.hidden != 1 && step.enabled != 0)
			{
				AchievementStepData stepData = playerData.getStepData(step.achievementStepID);
				int num5 = (stepData != null) ? stepData.progress : 0;
				bool flag2 = num5 >= step.threshold;
				GUIImage gUIImage = new GUIImage();
				gUIImage.SetSize(new Vector2(42f, 44f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				gUIImage.Position = a + new Vector2(0f, num2 + 11);
				gUIImage.TextureSource = "achievement_bundle|objective_incomplete";
				gUIImage.IsVisible = true;
				_contentContainer.Add(gUIImage);
				if (flag2)
				{
					GUIImage gUIImage2 = new GUIImage();
					gUIImage2.SetSize(new Vector2(36f, 39f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
					gUIImage2.Position = gUIImage.Position + new Vector2(0f, 0f);
					gUIImage2.TextureSource = "achievement_bundle|objective_check";
					gUIImage2.IsVisible = true;
					_contentContainer.Add(gUIImage2);
				}
				GUILabel gUILabel4 = new GUILabel();
				gUILabel4.FontSize = 16;
				gUILabel4.FontFace = GUIFontManager.SupportedFontEnum.Komica;
				if (flag2)
				{
					gUILabel4.TextColor = GUILabel.GenColor(100, 100, 100);
				}
				else
				{
					gUILabel4.TextColor = GUILabel.GenColor(0, 0, 0);
				}
				gUILabel4.TextAlignment = TextAnchor.MiddleLeft;
				gUILabel4.WordWrap = true;
				gUILabel4.Text = AchievementManager.formatAchievementString(step.displayDesc, num5, step.threshold, step.heroName, step.int1);
				Vector2 position5 = gUIImage.Position;
				Vector2 size6 = gUIImage.Size;
				gUILabel4.SetPosition(position5 + new Vector2(size6.x + 4f, -11f));
				float num6 = num;
				Vector2 position6 = gUILabel4.Position;
				gUILabel4.SetSize(num6 - position6.x, 70f);
				_contentContainer.Add(gUILabel4);
				if (!flag2 && step.type == "meta" && step.int4 > 0)
				{
					NewAchievement metaAch = AchievementManager.getAchievement(step.int4);
					if (metaAch.steps.Count > 0 && metaAch.steps[0].type == "mission_complete_collection")
					{
						AchievementStepDataEx stepDataEx = playerData.getStepDataEx(metaAch.steps[0].achievementStepID);
						string content = string.Empty;
						if (stepDataEx != null)
						{
							content = stepDataEx.str1;
						}
						GUIButton gUIButton = new GUIButton();
						gUIButton.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
						gUIButton.SetSize(new Vector2(48f, 48f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
						gUIButton.SetPosition(new Vector2(num - 185, -15f));
						gUIButton.StyleInfo = new SHSButtonStyleInfo("achievement_bundle|search_field_icon");
						string data = string.Empty;
						if (step.str1 == "h")
						{
							gUIButton.ToolTip = new NamedToolTipInfo("#ACHIEVEMENT_DETAILS_HEROES");
							data = step.subEventType;
						}
						else
						{
							gUIButton.ToolTip = new NamedToolTipInfo("#ACHIEVEMENT_DETAILS_MISSIONS");
							data = achievement.metadata;
						}
						gUIButton.HitTestType = HitTestTypeEnum.Circular;
						gUIButton.HitTestSize = new Vector2(0.6f, 0.6f);
						gUIButton.Click += delegate
						{
							AppShell.Instance.EventMgr.Fire(this, new MissionCompletionDetailRequest(metaAch.steps[0].str1 == "hero", this, content, data));
						};
						_contentContainer.Add(gUIButton);
					}
				}
				num4 = (int)GUINotificationWindow.GetTextBlockSize(new GUILabel[1]
				{
					gUILabel4
				}, GUINotificationWindow.BlockSizeType.Height);
				int num7 = num2;
				Vector2 size7 = gUIImage.Size;
				num2 = num7 + (Math.Max((int)size7.y, num4) + 5);
			}
		}
		num2 += 15;
		if (targetAchievement.jumpAction != string.Empty && (AppShell.Instance.Profile == null || playerData.playerID == (int)AppShell.Instance.Profile.UserId))
		{
			bool flag3 = true;
			if (targetAchievement.track == AchievementManager.DestinyTracks.Conquest && playerData.achievementComplete(targetAchievement.achievementID) && targetAchievement.achievementID <= 210003)
			{
				flag3 = false;
			}
			else if (targetAchievement.track != AchievementManager.DestinyTracks.None && !AchievementManager.playerOnAchievement(playerData.playerID, targetAchievement.achievementID))
			{
				flag3 = false;
			}
			if (flag3)
			{
				num2 += 5;
				AchievementJumpActionContainer achievementJumpActionContainer = new AchievementJumpActionContainer(targetAchievement);
				achievementJumpActionContainer.OnJumpActionClickEvent += requestClose;
				achievementJumpActionContainer.SetPosition(new Vector2(62f, num2));
				int num8 = num2;
				Vector2 size8 = achievementJumpActionContainer.Size;
				num2 = num8 + ((int)size8.y + 5);
				_contentContainer.Add(achievementJumpActionContainer);
			}
		}
		num2 += 15;
		GUISimpleControlWindow contentContainer2 = _contentContainer;
		Vector2 size9 = _headerContainer.Size;
		contentContainer2.SetSize(new Vector2(size9.x, num2));
		_openFill.SetSize(_contentContainer.Size);
		GUIImage openFooter = _openFooter;
		Vector2 position7 = _openFill.Position;
		float x2 = position7.x;
		Vector2 position8 = _openFill.Position;
		float y = position8.y;
		Vector2 size10 = _openFill.Size;
		openFooter.SetPosition(x2, y + size10.y);
		AchievementReward achievementReward = AchievementManager.rewards[achievement.rewardID];
		Vector2 vector = new Vector2(54f, 0f);
		int num9 = 0;
		if (achievementReward.fractals > 0)
		{
			num9++;
		}
		if (achievementReward.xp > 0)
		{
			num9++;
		}
		if (achievementReward.ownableID1 > 0)
		{
			num9++;
		}
		if (achievementReward.ownableID2 > 0)
		{
			num9++;
		}
		if (num9 > 0)
		{
			_footerContainer = new GUISimpleControlWindow();
			_footerContainer.IsVisible = true;
			_footerContainer.SetSize(_openFooter.Size);
			GUISimpleControlWindow footerContainer = _footerContainer;
			Vector2 position9 = _openFooter.Position;
			footerContainer.SetPosition(new Vector2(0f, position9.y));
			Add(_footerContainer);
			Vector2 size11 = _openFooter.Size;
			Vector2 vector2 = new Vector2(size11.x / 2f - (float)(num9 - 1) * vector.x / 2f, 0f);
			GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
			gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, vector2 + new Vector2(-110f, 10f), new Vector2(100f, 30f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, GUILabel.GenColor(0, 204, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(1f, 2f), TextAnchor.MiddleRight);
			gUIStrokeTextLabel.BackColorAlpha = 1f;
			gUIStrokeTextLabel.StrokeColorAlpha = 1f;
			gUIStrokeTextLabel.Text = "Rewards:";
			gUIStrokeTextLabel.IsVisible = true;
			_footerContainer.Add(gUIStrokeTextLabel);
			if (achievementReward.fractals > 0)
			{
				addRewardIcon(303943, achievementReward.fractals, vector2);
				vector2 += vector;
			}
			if (achievementReward.xp > 0)
			{
				addRewardIcon(-4, achievementReward.xp, vector2);
				vector2 += vector;
			}
			if (achievementReward.ownableID1 > 0)
			{
				addRewardIcon(achievementReward.ownableID1, achievementReward.ownableID1Quantity, vector2);
				vector2 += vector;
			}
			if (achievementReward.ownableID2 > 0)
			{
				addRewardIcon(achievementReward.ownableID2, achievementReward.ownableID2Quantity, vector2);
				vector2 += vector;
			}
		}
		Vector2 size12 = _openFooter.Size;
		Vector2 position10 = _openFooter.Position;
		SetSize(size12 + new Vector2(0f, position10.y));
	}

	private void requestClose()
	{
		AppShell.Instance.EventMgr.Fire(this, new CloseAchievementWindowRequest());
	}

	private void addRewardIcon(int ownableTypeID, int quantity, Vector2 position)
	{
		OwnableDefinition def = OwnableDefinition.getDef(ownableTypeID);
		GUISimpleControlWindow icon = def.getIcon(new Vector2(45f, 45f), true);
		icon.SetPosition(position);
		_footerContainer.Add(icon);
		if (quantity > 1)
		{
			GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
			Vector2 position2 = icon.Position;
			Vector2 size = icon.Size;
			Vector2 offset = position2 + new Vector2(0f, size.y / 2f);
			Vector2 size2 = icon.Size;
			gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, offset, new Vector2(size2.x, 24f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 14, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(1f, 2f), TextAnchor.UpperRight);
			gUIStrokeTextLabel.BackColorAlpha = 1f;
			gUIStrokeTextLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
			gUIStrokeTextLabel.Text = "x" + quantity;
			_footerContainer.Add(gUIStrokeTextLabel);
		}
	}
}
