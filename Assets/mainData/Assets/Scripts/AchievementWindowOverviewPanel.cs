using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementWindowOverviewPanel : GUISimpleControlWindow
{
	public class OverviewProgressBar : GUISimpleControlWindow
	{
		private PlayerAchievementData _playerData;

		public OverviewProgressBar(PlayerAchievementData playerData, string label, int groupID = 0, bool small = false)
		{
			_playerData = playerData;
			int num = 28;
			GUIImage gUIImage = new GUIImage();
			if (small)
			{
				gUIImage.SetSize(new Vector2(295f, 41f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				gUIImage.TextureSource = "achievement_bundle|progress_empty";
				num = 20;
			}
			else
			{
				gUIImage.SetSize(new Vector2(590f, 62f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				gUIImage.TextureSource = "achievement_bundle|progress_empty";
			}
			gUIImage.Position = Vector2.zero;
			Add(gUIImage);
			SetSize(gUIImage.Size);
			if (groupID > 0)
			{
				GUIHotSpotButton gUIHotSpotButton = new GUIHotSpotButton();
				gUIHotSpotButton.SetSize(Size);
				gUIHotSpotButton.SetPosition(Vector2.zero);
				gUIHotSpotButton.Click += delegate
				{
					AppShell.Instance.EventMgr.Fire(this, new AchievementGroupSelectedMessage(groupID));
				};
				Add(gUIHotSpotButton);
			}
			GUISimpleControlWindow gUISimpleControlWindow = new GUISimpleControlWindow();
			gUISimpleControlWindow.SetSize(gUIImage.Size);
			gUISimpleControlWindow.SetPosition(gUIImage.Position);
			gUISimpleControlWindow.IsVisible = true;
			Add(gUISimpleControlWindow);
			GUIImage gUIImage2 = new GUIImage();
			gUIImage2.SetSize(gUIImage.Size, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIImage2.Position = Vector2.zero;
			gUIImage2.TextureSource = "achievement_bundle|progress_full";
			gUISimpleControlWindow.Add(gUIImage2);
			GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
			gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, gUIImage.Position + new Vector2(20f, 0f), gUIImage.Size + new Vector2(-100f, 0f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, num, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 43, 68), GUILabel.GenColor(0, 43, 68), new Vector2(2f, 2f), TextAnchor.MiddleLeft);
			gUIStrokeTextLabel.BackColorAlpha = 1f;
			gUIStrokeTextLabel.StrokeColorAlpha = 1f;
			gUIStrokeTextLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
			gUIStrokeTextLabel.Text = label;
			gUIStrokeTextLabel.IsVisible = true;
			Add(gUIStrokeTextLabel);
			GUIStrokeTextLabel gUIStrokeTextLabel2 = new GUIStrokeTextLabel();
			gUIStrokeTextLabel2.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, gUIImage.Position + new Vector2(0f, 0f), gUIImage.Size - new Vector2(20f, 0f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIStrokeTextLabel2.SetupText(GUIFontManager.SupportedFontEnum.Grobold, num - 2, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 43, 68), GUILabel.GenColor(0, 43, 68), new Vector2(2f, 2f), TextAnchor.MiddleRight);
			gUIStrokeTextLabel2.BackColorAlpha = 1f;
			gUIStrokeTextLabel2.StrokeColorAlpha = 1f;
			gUIStrokeTextLabel2.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
			gUIStrokeTextLabel2.IsVisible = true;
			Add(gUIStrokeTextLabel2);
			int num2 = 0;
			int num3 = 0;
			if (groupID == 0)
			{
				num2 = playerData.totalCompletedAchievements;
				num3 = AchievementManager.totalAchievements;
			}
			else
			{
				num2 = playerData.totalAchievementsCompletedByGroup[groupID];
				if (!AchievementManager.totalAchievementsByGroup.ContainsKey(groupID))
				{
					CspUtils.DebugLog("AchievementManager.totalAchievementsByRootGroup does not contain group ID " + groupID);
					num3 = 0;
				}
				else
				{
					num3 = AchievementManager.totalAchievementsByGroup[groupID];
				}
			}
			num2 = Math.Min(num2, num3);
			gUIStrokeTextLabel2.Text = string.Format("{0:#,0}", num2) + " / " + string.Format("{0:#,0}", num3);
			float num4 = (float)num2 / (float)num3;
			Vector2 size = gUIImage.Size;
			float x = size.x * num4;
			Vector2 size2 = gUIImage.Size;
			gUISimpleControlWindow.SetSize(new Vector2(x, size2.y));
		}
	}

	private PlayerAchievementData _playerData;

	private List<GUIControl> _controls = new List<GUIControl>();

	private GUISimpleControlWindow _scrollPaneMask;

	private GUISimpleControlWindow _scrollPane;

	private GUISlider _slider;

	private int maxY;

	private int sliderX;

	private int displayGroupID = -1;

	public AchievementWindowOverviewPanel(PlayerAchievementData playerData)
	{
		_playerData = playerData;
		Vector2 vector = new Vector2(615f, 481f);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		SetSize(vector + new Vector2(30f, 0f));
		_scrollPaneMask = new GUISimpleControlWindow();
		_scrollPaneMask.SetSize(vector);
		_scrollPaneMask.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(5f, 0f));
		Add(_scrollPaneMask);
		_scrollPane = new GUISimpleControlWindow();
		_scrollPane.SetSize(vector.x, 4000f);
		_scrollPane.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, _scrollPaneMask.Position + new Vector2(8f, 0f));
		_scrollPane.IsVisible = true;
		_scrollPaneMask.Add(_scrollPane);
		GUIImage gUIImage = new GUIImage();
		gUIImage.SetSize(new Vector2(613f, 25f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage.Position = new Vector2(1f, 0f);
		gUIImage.TextureSource = "achievement_bundle|contentpanel_topmask";
		gUIImage.IsVisible = true;
		Add(gUIImage);
		GUIImage gUIImage2 = new GUIImage();
		gUIImage2.SetSize(new Vector2(613f, 22f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Vector2 position = gUIImage.Position;
		float x = position.x;
		Vector2 size = Size;
		float y = size.y;
		Vector2 size2 = gUIImage2.Size;
		gUIImage2.Position = new Vector2(x, y - size2.y);
		gUIImage2.TextureSource = "achievement_bundle|contentpanel_bottommask";
		gUIImage2.IsVisible = true;
		Add(gUIImage2);
		_slider = new GUISlider();
		_slider.Changed += slider_Changed;
		_slider.UseMouseWheelScroll = true;
		_slider.MouseScrollWheelAmount = 1f;
		_slider.TickValue = 40f;
		_slider.ArrowsEnabled = true;
		_slider.SetSize(40f, vector.y - 20f);
		Vector2 size3 = _scrollPaneMask.Size;
		float x2 = size3.x;
		Vector2 size4 = _slider.Size;
		sliderX = (int)(x2 - size4.x + 20f);
		_slider.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(1000f, 10f));
		Add(_slider);
		_slider.IsVisible = false;
	}

	public void Close()
	{
		foreach (GUIControl control in _controls)
		{
			_scrollPane.Remove(control);
			control.Dispose();
		}
		_controls.Clear();
	}

	private void slider_Changed(GUIControl sender, GUIChangedEvent eventData)
	{
		GUISimpleControlWindow scrollPane = _scrollPane;
		float num = 0f - _slider.Percentage;
		float num2 = maxY;
		Vector2 size = _scrollPaneMask.Size;
		scrollPane.Offset = new Vector2(0f, num * (num2 - size.y));
	}

	public void displayGroup(int displayGroupID)
	{
		this.displayGroupID = displayGroupID;
		bool flag = false;
		if (Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ClientConsoleAllow))
		{
			flag = true;
		}
		foreach (GUIControl control in _controls)
		{
			_scrollPane.Remove(control);
			control.Dispose();
		}
		_controls.Clear();
		if (displayGroupID == -10)
		{
			displayGroupID = 0;
		}
		OverviewProgressBar overviewProgressBar = new OverviewProgressBar(_playerData, "Overall Progress", displayGroupID);
		overviewProgressBar.SetPosition(0f, 30f);
		_scrollPane.Add(overviewProgressBar);
		_controls.Add(overviewProgressBar);
		List<AchievementDisplayGroup> list = AchievementManager.rootGroups;
		if (AchievementManager.allGroups.ContainsKey(displayGroupID))
		{
			AchievementDisplayGroup achievementDisplayGroup = AchievementManager.allGroups[displayGroupID];
			list = achievementDisplayGroup.childGroups;
		}
		if (list.Count > 8)
		{
			_slider.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(sliderX, 10f));
			_slider.IsVisible = true;
		}
		else
		{
			_slider.IsVisible = false;
		}
		int num = 0;
		int num2 = 100;
		foreach (AchievementDisplayGroup item in list)
		{
			if (item.groupID > 0 && (!item.restricted || flag))
			{
				OverviewProgressBar overviewProgressBar2 = new OverviewProgressBar(_playerData, item.name, item.groupID, true);
				if (num % 2 == 1)
				{
					overviewProgressBar2.SetPosition(300f, num2);
				}
				else
				{
					overviewProgressBar2.SetPosition(0f, num2);
				}
				_scrollPane.Add(overviewProgressBar2);
				_controls.Add(overviewProgressBar2);
				if (num % 2 == 1)
				{
					num2 += 55;
				}
				num++;
			}
		}
		num2 += 50;
		GUISimpleControlWindow scrollPane = _scrollPane;
		Vector2 size = _scrollPane.Size;
		scrollPane.SetSize(size.x, num2);
		maxY = num2;
		_slider.Value = 0f;
		slider_Changed(null, null);
	}
}
