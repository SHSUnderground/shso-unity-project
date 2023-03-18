using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSAchievementsWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private class AchievementDisplay : GUIControlWindow
	{
		public class AchievemeentScrollWindow : SHSSelectionWindow<AchievementViewItem, GUIControlWindow>
		{
			public AchievemeentScrollWindow(GUISlider slider)
				: base(slider, 800f, new Vector2(800f, 40f), 10)
			{
			}
		}

		public class AchievementViewItem : SHSSelectionItem<GUIControlWindow>, IComparable<AchievementViewItem>
		{
			private GUILabel name;

			private GUILabel bronze;

			private GUILabel silver;

			private GUILabel gold;

			private GUILabel adamantium;

			private GUITextField NewValue;

			private GUIButton SetField;

			private IAchievement ach;

			private string currentHero;

			private string nameTxt;

			public AchievementViewItem(IAchievement ach, string currentHero, Dictionary<string, long> heroCounterAccumDict)
			{
				this.ach = ach;
				this.currentHero = currentHero;
				nameTxt = ach.Id;
				item = new GUIControlWindow();
				itemSize = new Vector2(800f, 40f);
				name = GUIControl.CreateControlAbsolute<GUILabel>(new Vector2(150f, 40f), new Vector2(0f, 0f));
				name.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
				item.Add(name);
				bronze = GUIControl.CreateControlAbsolute<GUILabel>(new Vector2(120f, 40f), new Vector2(150f, 0f));
				bronze.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
				item.Add(bronze);
				silver = GUIControl.CreateControlAbsolute<GUILabel>(new Vector2(120f, 40f), new Vector2(270f, 0f));
				silver.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
				item.Add(silver);
				gold = GUIControl.CreateControlAbsolute<GUILabel>(new Vector2(120f, 40f), new Vector2(390f, 0f));
				gold.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
				item.Add(gold);
				adamantium = GUIControl.CreateControlAbsolute<GUILabel>(new Vector2(150f, 40f), new Vector2(510f, 0f));
				adamantium.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
				item.Add(adamantium);
				NewValue = new GUITextField();
				NewValue.SetPositionAndSize(660f, 5f, 60f, 25f);
				item.Add(NewValue);
				SetField = new GUIButton();
				SetField.SetPositionAndSize(720f, 5f, 60f, 25f);
				SetField.Text = "Set";
				item.Add(SetField);
				if (heroCounterAccumDict.ContainsKey(ach.Id))
				{
					SetField.Click += delegate
					{
						long result;
						long.TryParse(NewValue.Text, out result);
						heroCounterAccumDict[ach.Id] = result;
						ach.DebugSetSourceValue(currentHero, heroCounterAccumDict[ach.Id]);
						SetupInfo();
					};
				}
			}

			public void SetupInfo()
			{
				name.Text = nameTxt;
				SetTextFromLevel(bronze, ach, Achievement.AchievementLevelEnum.Bronze, currentHero);
				SetTextFromLevel(silver, ach, Achievement.AchievementLevelEnum.Silver, currentHero);
				SetTextFromLevel(gold, ach, Achievement.AchievementLevelEnum.Gold, currentHero);
				SetTextFromLevel(adamantium, ach, Achievement.AchievementLevelEnum.Adamantium, currentHero);
				NewValue.Text = string.Empty + Math.Max(ach.GetCharacterLevelValue(currentHero), 0L);
			}

			public void SetTextFromLevel(GUILabel label, IAchievement ach, Achievement.AchievementLevelEnum level, string currentHero)
			{
				long characterLevelValue = ach.GetCharacterLevelValue(currentHero);
				long levelThreshold = ach.GetLevelThreshold(level);
				label.Text = level.ToString() + ": " + Math.Max(characterLevelValue, 0L) + " / " + levelThreshold;
				label.TextColor = ((levelThreshold > characterLevelValue) ? Color.white : Color.green);
			}

			public int CompareTo(AchievementViewItem other)
			{
				return nameTxt.CompareTo(other.nameTxt);
			}
		}

		public AchievemeentScrollWindow achievements;

		public GUILabel selectedHero;

		public AchievementDisplay()
		{
			GUISlider gUISlider = new GUISlider();
			gUISlider.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight);
			gUISlider.SetSize(50f, 360f);
			selectedHero = GUIControl.CreateControlAbsolute<GUILabel>(new Vector2(120f, 40f), new Vector2(150f, 50f));
			selectedHero.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), TextAnchor.UpperLeft);
			Add(selectedHero);
			achievements = new AchievemeentScrollWindow(gUISlider);
			achievements.SetSize(800f, 500f);
			achievements.SetPosition(0f, 0f);
			Add(achievements);
		}

		public void SetupInfo(string currentHero, Dictionary<string, long> heroCounterAccumDict)
		{
			achievements.ClearItems();
			selectedHero.Text = currentHero;
			foreach (IAchievement value in AppShell.Instance.AchievementsManager.Achievements.Values)
			{
				AchievementViewItem achievementViewItem = new AchievementViewItem(value, currentHero, heroCounterAccumDict);
				achievementViewItem.SetupInfo();
				achievements.AddItem(achievementViewItem);
			}
			achievements.SortItemList();
		}
	}

	private string currentHero = "shps";

	private GUIDropDownListBox heroDDList = new GUIDropDownListBox();

	private Dictionary<string, long> heroCounterAccumDict;

	private AchievementDisplay achievementDisplay;

	public SHSAchievementsWindow(string Name)
		: this(Name, null)
	{
	}

	public SHSAchievementsWindow(string Name, GUISlider slider)
		: base(Name, slider)
	{
		heroDDList = new GUIDropDownListBox();
		heroDDList.SetPositionAndSize(450f, 5f, 250f, 30f);
		heroDDList.AddItem("wolverine");
		heroDDList.AddItem("hulk");
		heroDDList.AddItem("ironman");
		AppShell.Instance.EventMgr.AddListener<DropDownItemSelectedMessage>(heroDDList, onDDList);
		Add(heroDDList);
		heroCounterAccumDict = new Dictionary<string, long>();
		achievementDisplay = new AchievementDisplay();
		achievementDisplay.SetPositionAndSize(50f, 50f, 850f, 500f);
		Add(achievementDisplay);
	}

	~SHSAchievementsWindow()
	{
		AppShell.Instance.EventMgr.RemoveListener<DropDownItemSelectedMessage>(heroDDList, onDDList);
	}

	private void onDDList(DropDownItemSelectedMessage message)
	{
		currentHero = message.ItemName;
		heroCounterAccumDict.Clear();
		foreach (IAchievement value in AppShell.Instance.AchievementsManager.Achievements.Values)
		{
			heroCounterAccumDict[value.Id] = value.GetCharacterLevelValue(currentHero);
		}
		SetupWindow(AppShell.Instance.Profile);
	}

	private void SetupWindow(UserProfile profile)
	{
		if (profile != null)
		{
			achievementDisplay.SetupInfo(currentHero, heroCounterAccumDict);
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		heroDDList.ClearAll();
		UserProfile profile = AppShell.Instance.Profile;
		if (profile != null)
		{
			List<string> list = new List<string>();
			foreach (HeroPersisted value in profile.AvailableCostumes.Values)
			{
				list.Add(value.Name);
			}
			list.Sort();
			foreach (string item in list)
			{
				heroDDList.AddItem(item);
			}
			heroDDList.SelectFirstItem();
		}
	}
}
