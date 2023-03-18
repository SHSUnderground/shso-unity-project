using System.Collections.Generic;
using UnityEngine;

public class AchievementWindowContentPanel : GUISimpleControlWindow
{
	private GUISimpleControlWindow _scrollPaneMask;

	private GUISimpleControlWindow _scrollPane;

	private GUISlider _slider;

	private int maxY;

	private PlayerAchievementData _playerData;

	private List<AchievementDetailWindow> _windows = new List<AchievementDetailWindow>();

	private int sliderX;

	private HeroCompletionsWindow _heroCompletionWindow;

	private MissionCompletionsWindow _missionCompletionWindow;

	public AchievementWindowContentPanel(PlayerAchievementData playerData, NewAchievement initialAchievement)
	{
		IsVisible = false;
		_playerData = playerData;
		Vector2 size = new Vector2(615f, 481f);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		SetSize(size);
		_scrollPaneMask = new GUISimpleControlWindow();
		_scrollPaneMask.SetSize(size);
		_scrollPaneMask.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(5f, 0f));
		Add(_scrollPaneMask);
		_scrollPane = new GUISimpleControlWindow();
		_scrollPane.SetSize(size.x, 4000f);
		_scrollPane.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(14f, 0f));
		_scrollPane.IsVisible = true;
		_scrollPaneMask.Add(_scrollPane);
		_heroCompletionWindow = new HeroCompletionsWindow();
		_heroCompletionWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 40f));
		_heroCompletionWindow.IsVisible = false;
		_missionCompletionWindow = new MissionCompletionsWindow();
		_missionCompletionWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 40f));
		_missionCompletionWindow.IsVisible = false;
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
		Vector2 size2 = Size;
		float y = size2.y;
		Vector2 size3 = gUIImage2.Size;
		gUIImage2.Position = new Vector2(x, y - size3.y);
		gUIImage2.TextureSource = "achievement_bundle|contentpanel_bottommask";
		gUIImage2.IsVisible = true;
		Add(gUIImage2);
		_slider = new GUISlider();
		_slider.Changed += slider_Changed;
		_slider.UseMouseWheelScroll = true;
		_slider.MouseScrollWheelAmount = 1f;
		_slider.TickValue = 40f;
		_slider.ArrowsEnabled = true;
		_slider.SetSize(40f, size.y - 20f);
		Vector2 size4 = _scrollPaneMask.Size;
		float x2 = size4.x;
		Vector2 size5 = _slider.Size;
		sliderX = (int)(x2 - size5.x);
		_slider.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(1000f, 10f));
		Add(_slider);
		_slider.IsVisible = false;
		AppShell.Instance.EventMgr.AddListener<AchievementGroupSelectedMessage>(OnAchievementGroupSelectedMessage);
		AppShell.Instance.EventMgr.AddListener<MissionCompletionDetailRequest>(OnMissionCompletionDetailRequest);
		if (initialAchievement != null)
		{
			showGroup(initialAchievement.displayGroupID);
			foreach (AchievementDetailWindow window in _windows)
			{
				if (window.targetAchievement.achievementID == initialAchievement.achievementID)
				{
					GUISlider slider = _slider;
					Vector2 position2 = window.Position;
					slider.Value = position2.y / (float)maxY * 100f;
					slider_Changed(null, null);
					break;
				}
			}
		}
	}

	public void Close()
	{
		AppShell.Instance.EventMgr.RemoveListener<AchievementGroupSelectedMessage>(OnAchievementGroupSelectedMessage);
		if (_heroCompletionWindow != null)
		{
			_heroCompletionWindow.Dispose();
			_heroCompletionWindow = null;
		}
		if (_missionCompletionWindow != null)
		{
			_missionCompletionWindow.Dispose();
			_missionCompletionWindow = null;
		}
	}

	private void OnMissionCompletionDetailRequest(MissionCompletionDetailRequest msg)
	{
		if (msg.hero)
		{
			_scrollPane.Remove(_missionCompletionWindow);
			_scrollPane.Add(_missionCompletionWindow);
			_missionCompletionWindow.SetPosition(msg.requester.Position + new Vector2(0f, 66f));
			_missionCompletionWindow.IsVisible = true;
			_missionCompletionWindow.updateList(msg.content, msg.data, AppShell.Instance.Profile.UserId == _playerData.playerID);
		}
		else
		{
			_scrollPane.Remove(_heroCompletionWindow);
			_scrollPane.Add(_heroCompletionWindow);
			_heroCompletionWindow.SetPosition(msg.requester.Position + new Vector2(0f, 66f));
			_heroCompletionWindow.IsVisible = true;
			_heroCompletionWindow.updateList(msg.content, msg.data, AppShell.Instance.Profile.UserId == _playerData.playerID);
		}
	}

	private void OnAchievementGroupSelectedMessage(AchievementGroupSelectedMessage msg)
	{
		showGroup(msg.groupID);
	}

	private void showGroup(int groupID)
	{
		foreach (AchievementDetailWindow window in _windows)
		{
			_scrollPane.Remove(window);
			window.Dispose();
		}
		if (AchievementManager.allGroups.ContainsKey(groupID))
		{
			maxY = 15;
			_slider.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(sliderX, 10f));
			AchievementDisplayGroup achievementDisplayGroup = AchievementManager.allGroups[groupID];
			achievementDisplayGroup.achievements.Sort(delegate(NewAchievement left, NewAchievement right)
			{
				return (left.achievementID >= right.achievementID) ? 1 : (-1);
			});
			foreach (NewAchievement achievement in achievementDisplayGroup.achievements)
			{
				if (achievement.hidden != 1 && achievement.enabled != 0)
				{
					AchievementDetailWindow achievementDetailWindow = new AchievementDetailWindow(_playerData, achievement);
					achievementDetailWindow.SetPosition(0f, maxY);
					achievementDetailWindow.IsVisible = true;
					_scrollPane.Add(achievementDetailWindow);
					int num = maxY;
					Vector2 size = achievementDetailWindow.Size;
					maxY = num + ((int)size.y + 5);
					_windows.Add(achievementDetailWindow);
				}
			}
			maxY += 200;
			GUISimpleControlWindow scrollPane = _scrollPane;
			Vector2 size2 = _scrollPane.Size;
			scrollPane.SetSize(size2.x, maxY);
			if (achievementDisplayGroup.achievements.Count > 0 && achievementDisplayGroup.achievements[0].track != AchievementManager.DestinyTracks.None)
			{
				foreach (AchievementDetailWindow window2 in _windows)
				{
					if (!AchievementManager.hasPlayerCompletedAchievement(_playerData.playerID, window2.targetAchievement.achievementID))
					{
						GUISlider slider = _slider;
						Vector2 position = window2.Position;
						slider.Value = position.y / (float)maxY * 100f;
						slider_Changed(null, null);
						break;
					}
				}
				return;
			}
			_slider.Value = 0f;
			slider_Changed(null, null);
		}
	}

	private void redraw()
	{
		maxY = 0;
		foreach (AchievementDetailWindow window in _windows)
		{
			window.SetPosition(0f, maxY);
			int num = maxY;
			Vector2 size = window.Size;
			maxY = num + ((int)size.y + 5);
		}
		slider_Changed(null, null);
	}

	private void slider_Changed(GUIControl sender, GUIChangedEvent eventData)
	{
		_scrollPane.Offset = new Vector2(0f, (0f - _slider.Percentage) * (float)maxY);
	}
}
