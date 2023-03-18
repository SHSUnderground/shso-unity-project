using System.Collections.Generic;
using UnityEngine;

public class AchievementWindowNavPanel : GUISimpleControlWindow
{
	private int targetPlayerID = -1;

	private int currentGroupID = -1;

	private GUISimpleControlWindow _scrollPaneMask;

	private GUISimpleControlWindow _scrollPane;

	private GUISlider _slider;

	private int deltY = 30;

	private int maxY;

	private Dictionary<int, List<AchievementWindowNavButton>> _groupButtonsByParentGroup = new Dictionary<int, List<AchievementWindowNavButton>>();

	private List<AchievementWindowNavButton> _allButtons = new List<AchievementWindowNavButton>();

	public AchievementWindowNavPanel(int targetPlayerID, Vector2 desiredSize, NewAchievement initialAchievement)
	{
		this.targetPlayerID = targetPlayerID;
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		SetSize(desiredSize);
		_scrollPaneMask = new GUISimpleControlWindow();
		_scrollPaneMask.SetSize(desiredSize);
		_scrollPaneMask.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f));
		Add(_scrollPaneMask);
		_scrollPane = new GUISimpleControlWindow();
		_scrollPane.SetSize(desiredSize.x, 4000f);
		_scrollPane.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, _scrollPaneMask.Position);
		_scrollPaneMask.Add(_scrollPane);
		_slider = new GUISlider();
		_slider.Changed += slider_Changed;
		_slider.UseMouseWheelScroll = false;
		_slider.MouseScrollWheelAmount = 1f;
		_slider.TickValue = 40f;
		_slider.ArrowsEnabled = true;
		_slider.SetSize(40f, desiredSize.y);
		GUISlider slider = _slider;
		Vector2 size = _scrollPaneMask.Size;
		float x = size.x;
		Vector2 size2 = _slider.Size;
		slider.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(x - size2.x, 0f));
		Add(_slider);
		slider_Changed(null, null);
		foreach (AchievementDisplayGroup value in AchievementManager.allGroups.Values)
		{
			int offset = (value.parentGroupID != 0) ? 1 : 0;
			makeGroupButton(value.name, offset, value);
		}
		if (initialAchievement == null)
		{
			selectGroup(-10);
		}
		else
		{
			AchievementDisplayGroup achievementDisplayGroup = AchievementManager.allGroups[initialAchievement.displayGroupID];
			selectGroup(achievementDisplayGroup.parentGroupID);
			selectGroup(initialAchievement.displayGroupID);
		}
		AppShell.Instance.EventMgr.AddListener<AchievementGroupSelectedMessage>(OnAchievementGroupSelectedMessage);
	}

	private void OnAchievementGroupSelectedMessage(AchievementGroupSelectedMessage msg)
	{
		if (msg.groupID <= 0)
		{
			selectGroup(-10);
		}
		else
		{
			selectGroup(msg.groupID);
		}
	}

	private void selectGroup(int groupID)
	{
		bool flag = false;
		if (Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ClientConsoleAllow))
		{
			flag = true;
		}
		AchievementDisplayGroup achievementDisplayGroup = AchievementManager.allGroups[groupID];
		int num = -1;
		if (achievementDisplayGroup.parentGroupID != 0)
		{
			currentGroupID = groupID;
			foreach (AchievementWindowNavButton item in _groupButtonsByParentGroup[achievementDisplayGroup.parentGroupID])
			{
				if (item.group.groupID != currentGroupID)
				{
					item.select(false);
				}
				else
				{
					Vector2 position = item.Position;
					num = (int)position.y;
					item.select(true);
				}
			}
		}
		else
		{
			if (currentGroupID != -1 && currentGroupID != 0)
			{
				foreach (AchievementWindowNavButton allButton in _allButtons)
				{
					allButton.select(false);
					if (allButton.group.parentGroupID != 0)
					{
						allButton.IsVisible = false;
						allButton.SetPosition(0f, -100f);
					}
				}
			}
			currentGroupID = groupID;
			maxY = 0;
			foreach (AchievementWindowNavButton item2 in _groupButtonsByParentGroup[0])
			{
				item2.SetPosition(0f, maxY);
				int num2 = maxY;
				Vector2 size = item2.Size;
				maxY = num2 + (int)size.y;
				if (item2.group.groupID == currentGroupID)
				{
					item2.select(true);
					if (_groupButtonsByParentGroup.ContainsKey(currentGroupID))
					{
						bool flag2 = true;
						foreach (AchievementWindowNavButton item3 in _groupButtonsByParentGroup[currentGroupID])
						{
							if (!item3.group.restricted || flag)
							{
								item3.IsVisible = true;
								item3.SetPosition(0f, maxY);
								int num3 = maxY;
								Vector2 size2 = item3.Size;
								maxY = num3 + (int)size2.y;
								if (!flag2)
								{
									flag2 = true;
									item3.select(true);
									Vector2 position2 = item3.Position;
									num = (int)position2.y;
								}
							}
						}
					}
				}
				else
				{
					item2.select(false);
				}
			}
			GUISimpleControlWindow scrollPane = _scrollPane;
			Vector2 size3 = Size;
			scrollPane.SetSize(size3.x, maxY + deltY);
		}
		if (num != -1)
		{
			_slider.Value = (float)num / (float)maxY * 100f;
			_scrollPane.Offset = new Vector2(0f, -num);
		}
		else
		{
			_slider.Value = 0f;
			slider_Changed(null, null);
		}
	}

	private void makeGroupButton(string label, int offset, AchievementDisplayGroup group)
	{
		AchievementWindowNavButton achievementWindowNavButton = new AchievementWindowNavButton(label, offset, group);
		achievementWindowNavButton.bg.Click += delegate
		{
			int groupID = group.groupID;
			if (currentGroupID == group.groupID)
			{
				groupID = group.parentGroupID;
			}
			AppShell.Instance.EventMgr.Fire(this, new AchievementGroupSelectedMessage(groupID));
		};
		_scrollPane.Add(achievementWindowNavButton);
		if (group.parentGroupID == 0)
		{
			achievementWindowNavButton.IsVisible = true;
		}
		else
		{
			achievementWindowNavButton.IsVisible = false;
		}
		achievementWindowNavButton.SetPosition(0f, -100f);
		if (!_groupButtonsByParentGroup.ContainsKey(group.parentGroupID))
		{
			_groupButtonsByParentGroup.Add(group.parentGroupID, new List<AchievementWindowNavButton>());
		}
		_groupButtonsByParentGroup[group.parentGroupID].Add(achievementWindowNavButton);
		_allButtons.Add(achievementWindowNavButton);
	}

	private void slider_Changed(GUIControl sender, GUIChangedEvent eventData)
	{
		_scrollPane.Offset = new Vector2(0f, (0f - _slider.Percentage) * (float)maxY);
	}
}
