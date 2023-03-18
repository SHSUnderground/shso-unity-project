using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSMySquadGadget : SHSGadget
{
	public class HeroInfoTabbedData : GUISimpleControlWindow
	{
		public class ItemsTab : GenericTab
		{
			public class ItemUnlocked : GUISimpleControlWindow
			{
				public ItemUnlocked(ItemDefinition def, Vector2 offset, int ReqLevel, int curLevel)
				{
					SetSize(160f, 160f);
					SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, offset);
					GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(113f, 113f), Vector2.zero);
					gUIImage.TextureSource = "mysquadgadget_bundle|mysquad_heroinfo_items_backdrop";
					Add(gUIImage);
					GUIDrawTexture gUIDrawTexture = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(new Vector2(80f, 80f), Vector2.zero);
					gUIDrawTexture.TextureSource = "items_bundle|" + def.Icon;
					Add(gUIDrawTexture);
					if (AppShell.Instance.Profile.AvailableItems.ContainsKey(def.Id))
					{
						gUIDrawTexture.ToolTip = new InventoryHoverHelpInfo(AppShell.Instance.Profile.AvailableItems[def.Id]);
					}
					else
					{
						gUIDrawTexture.ToolTip = new GenericHoverHelpInfo(def.Name, def.Description, "items_bundle|" + def.Icon, new Vector2(160f, 160f));
					}
					if (curLevel < ReqLevel)
					{
						gUIDrawTexture.IsEnabled = false;
						GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(52f, 52f), new Vector2(21f, 31f));
						gUIImage2.TextureSource = "mysquadgadget_bundle|L_mysquad_gadget_lv" + ReqLevel + "lock";
						gUIImage2.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
						gUIImage2.ToolTip = new NamedToolTipInfo(MySquadDataManager.GetUnlockAtLevelText(ReqLevel));
						Add(gUIImage2);
					}
				}
			}

			private int level;

			public ItemsTab(SHSMySquadGadget BaseGadget, string heroName)
			{
				List<ItemDefinition> theFiveItemUnlockables = MySquadDataManager.GetTheFiveItemUnlockables(heroName);
				if (BaseGadget.DataManager.Profile.AvailableCostumes.ContainsKey(heroName))
				{
					level = BaseGadget.DataManager.Profile.AvailableCostumes[heroName].Level;
				}
				if (theFiveItemUnlockables.Count >= 5)
				{
					Add(new ItemUnlocked(theFiveItemUnlockables[0], new Vector2(-145f, -50f), 2, level));
					Add(new ItemUnlocked(theFiveItemUnlockables[1], new Vector2(0f, -50f), 4, level));
					Add(new ItemUnlocked(theFiveItemUnlockables[2], new Vector2(145f, -50f), 6, level));
					Add(new ItemUnlocked(theFiveItemUnlockables[3], new Vector2(-72f, 50f), 8, level));
					Add(new ItemUnlocked(theFiveItemUnlockables[4], new Vector2(72f, 50f), 10, level));
				}
			}
		}

		public class MedalsTab : GenericTab
		{
			private SHSMySquadGadget BaseGadget;

			public MedalsTab(SHSMySquadGadget BaseGadget, string heroName)
			{
				this.BaseGadget = BaseGadget;
				GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(386f, 238f), new Vector2(0f, 0f));
				gUIImage.TextureSource = "mysquadgadget_bundle|mysquad_heroinfo_medals_backdrop";
				Add(gUIImage);
				SetupData(heroName);
			}

			public void SetupData(string heroName)
			{
				List<SHSCounterAchievement> list = new List<SHSCounterAchievement>();
				foreach (Achievement value in AppShell.Instance.AchievementsManager.Achievements.Values)
				{
					if (value is SHSCounterAchievement)
					{
						list.Add(value as SHSCounterAchievement);
					}
				}
				list.Sort(delegate(SHSCounterAchievement a, SHSCounterAchievement b)
				{
					return a.ShortDescription.CompareTo(b.ShortDescription);
				});
				float num = 0f;
				float num2 = 0f;
				foreach (SHSCounterAchievement item in list)
				{
					float x = -151f + num * 99f + ((!(num >= 2f)) ? 0f : (2f * num));
					float y = -74f + num2 * 75f;
					DrawAchievement(item, heroName, new Vector2(x, y));
					num += 1f;
					if (num >= 4f)
					{
						num = 0f;
						num2 += 1f;
					}
				}
			}

			private void DrawAchievement(SHSCounterAchievement achievement, string heroName, Vector2 offset)
			{
				GUIDrawTexture gUIDrawTexture = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(new Vector2(117f, 107f) * 0.8f, offset);
				Achievement.AchievementLevelEnum level = achievement.GetLevel(heroName, BaseGadget.ActiveProfile.CounterBank);
				string textureSource = string.Empty;
				if (level != Achievement.AchievementLevelEnum.Unknown)
				{
					textureSource = "notification_bundle|" + achievement.Id + "_" + level.ToString();
				}
				if (level == Achievement.AchievementLevelEnum.NotAchieved)
				{
					textureSource = "notification_bundle|" + achievement.Id + "_" + Achievement.AchievementLevelEnum.Bronze.ToString();
					gUIDrawTexture.IsEnabled = false;
				}
				gUIDrawTexture.TextureSource = textureSource;
				gUIDrawTexture.ToolTip = new AchievementHoverHelpInfo(achievement, BaseGadget.ActiveProfile.CounterBank, heroName, Achievement.GetNextHighestLevel(level));
				gUIDrawTexture.ToolTipOffset = new Vector2(0f, 20f);
				Add(gUIDrawTexture);
			}
		}

		public abstract class GenericTab : GUISimpleControlWindow
		{
			public GenericTab()
			{
				SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-11f, 111f));
				SetSize(new Vector2(472f, 262f));
				SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
			}
		}

		public class UpgradesTab : GenericTab
		{
			private string heroName;

			private StatLevelReqsDefinition lvlData;

			private int level;

			private SHSMySquadGadget BaseGadget;

			public UpgradesTab(SHSMySquadGadget BaseGadget, string heroName)
			{
				this.BaseGadget = BaseGadget;
				this.heroName = heroName;
				lvlData = StatLevelReqsDefinition.Instance;
				GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(407f, 213f), new Vector2(0f, 0f));
				gUIImage.TextureSource = "mysquadgadget_bundle|L_mysquad_heroinfo_upgrades_backdrop";
				Add(gUIImage);
				GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(63f, 53f), new Vector2(-174f, -89f));
				gUIImage2.TextureSource = "mysquadgadget_bundle|L_mysquad_heroinfo_xp_icon";
				Add(gUIImage2);
				level = 0;
				if (BaseGadget.DataManager.Profile.AvailableCostumes.ContainsKey(heroName))
				{
					level = BaseGadget.DataManager.Profile.AvailableCostumes[heroName].Level;
				}
				AddExpBar();
				AddPowerEmotes();
				AddHeroUp();
				AddPowerAttacks();
				AddHealthBar();
			}

			public void AddExpBar()
			{
				float percToNextLevel = MySquadDataManager.GetPercToNextLevel(BaseGadget.ActiveProfile, heroName);
				float num = percToNextLevel * 291f;
				AddImage(new Vector2(18f, 20f), new Vector2(-132f, -88f), "mysquad_heroinfo_xpbar_left_cap");
				AddImage(new Vector2(num, 20f), new Vector2(-132f + num * 0.5f + 9f, -88f), "mysquad_heroinfo_xpbar_fill");
				AddImage(new Vector2(18f, 20f), new Vector2(-132f + num + 18f, -88f), "mysquad_heroinfo_xpbar_right_cap");
				GUILabel gUILabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(200f, 200f), new Vector2(Mathf.Max(-132f + num * 0.5f + 9f, -69f) + 43f, -89f));
				gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, GUILabel.GenColor(33, 99, 105), TextAnchor.MiddleLeft);
				gUILabel.Text = XpToLevelDefinition.GetExpText(BaseGadget.ActiveProfile, heroName);
				Add(gUILabel);
			}

			public void AddHealthBar()
			{
				GUILabel gUILabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(200f, 200f), Vector2.zero);
				gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, GUILabel.GenColor(33, 99, 105), TextAnchor.MiddleCenter);
				CspUtils.DebugLog("SHSMySqyadHeroInfoUpdates " + lvlData.GetNumberOfHealthRanksForLevel(level));
				switch (lvlData.GetNumberOfHealthRanksForLevel(level))
				{
				case 0:
					AddImage(new Vector2(127f, 20f), new Vector2(2f, -45f), "mysquad_heroinfo_health_lv1");
					gUILabel.Offset = new Vector2(2f, -45f);
					gUILabel.Text = MySquadDataManager.GetHealthLevelText(1);
					AddLock(new Vector2(89f, -45f), lvlData.GetLevelHealthRankIsUnlockedAt(1));
					AddLock(new Vector2(129f, -45f), lvlData.GetLevelHealthRankIsUnlockedAt(2));
					AddLock(new Vector2(168f, -45f), lvlData.GetLevelHealthRankIsUnlockedAt(3));
					break;
				case 1:
					AddImage(new Vector2(162f, 20f), new Vector2(20f, -45f), "mysquad_heroinfo_health_lv2");
					gUILabel.Offset = new Vector2(20f, -45f);
					gUILabel.Text = MySquadDataManager.GetHealthLevelText(2);
					AddLock(new Vector2(129f, -45f), lvlData.GetLevelHealthRankIsUnlockedAt(2));
					AddLock(new Vector2(168f, -45f), lvlData.GetLevelHealthRankIsUnlockedAt(3));
					break;
				case 2:
					AddImage(new Vector2(205f, 20f), new Vector2(42f, -45f), "mysquad_heroinfo_health_lv3");
					gUILabel.Offset = new Vector2(42f, -45f);
					gUILabel.Text = MySquadDataManager.GetHealthLevelText(3);
					AddLock(new Vector2(168f, -45f), lvlData.GetLevelHealthRankIsUnlockedAt(3));
					break;
				case 3:
					AddImage(new Vector2(248f, 20f), new Vector2(64f, -45f), "mysquad_heroinfo_health_lv4");
					gUILabel.Offset = new Vector2(64f, -45f);
					gUILabel.Text = MySquadDataManager.GetHealthLevelText(4);
					break;
				case 4:
					AddImage(new Vector2(248f, 20f), new Vector2(64f, -45f), "mysquad_heroinfo_health_lv5");
					gUILabel.Offset = new Vector2(64f, -45f);
					gUILabel.Text = MySquadDataManager.GetHealthLevelText(5);
					break;
				case 5:
					AddImage(new Vector2(248f, 20f), new Vector2(64f, -45f), "mysquad_heroinfo_health_lv6");
					gUILabel.Offset = new Vector2(64f, -45f);
					gUILabel.Text = MySquadDataManager.GetHealthLevelText(6);
					break;
				}
				Add(gUILabel);
			}

			public void AddPowerAttacks()
			{
				AddPowerAttack(new Vector2(-45f, -2f), "mysquad_heroinfo_upgrades_power_attack_1", 0);
				AddPowerAttack(new Vector2(-2f, -2f), "mysquad_heroinfo_upgrades_power_attack_2", 1);
				AddImprovedPowerAttack(new Vector2(41f, -2f), "mysquad_heroinfo_upgrades_power_attack_1plus", 0);
				AddPowerAttack(new Vector2(84f, -2f), "mysquad_heroinfo_upgrades_power_attack_3", 2);
				AddImprovedPowerAttack(new Vector2(127f, -2f), "mysquad_heroinfo_upgrades_power_attack_2plus", 1);
				AddImprovedPowerAttack(new Vector2(170f, -2f), "mysquad_heroinfo_upgrades_power_attack_3plus", 2);
			}

			public void AddPowerAttack(Vector2 offset, string source, int rank)
			{
				throw new Exception("SHSMySquadHeroInfoUpgrades not implemented");
			}

			public void AddImprovedPowerAttack(Vector2 offset, string source, int attackIndex)
			{
				throw new Exception("SHSMySquadHeroInfoUpgrades not implemented");
			}

			public void AddHeroUp()
			{
				GUIImage gUIImage = AddImage(new Vector2(58f, 54f), new Vector2(-38f, 41f), "mysquad_heroinfo_upgrades_hero_up_1");
				GUIImage gUIImage2 = AddLockableImage(new Vector2(58f, 54f), new Vector2(16f, 41f), new Vector2(15f, 15f), "mysquad_heroinfo_upgrades_hero_up_2", 1 > lvlData.GetNumberOfHeroupRanksForLevel(level), lvlData.GetLevelHeroupRankIsUnlockedAt(1));
				gUIImage.ToolTip = new NamedToolTipInfo("#HeroUp", new Vector2(25f, -5f));
				gUIImage2.ToolTip = new NamedToolTipInfo("#UpgradedHeroUp", new Vector2(25f, -5f));
			}

			public void AddPowerEmotes()
			{
				List<EmotesDefinition.EmoteDefinition> emotesByCategory = EmotesDefinition.Instance.GetEmotesByCategory(EmotesDefinition.EmoteCategoriesEnum.PowerEmote);
				if (emotesByCategory.Count >= 3)
				{
					AddPowerEmote(emotesByCategory[0], new Vector2(60f, 52f), new Vector2(-36f, 85f), "mysquad_heroinfo_upgrades_power_emote_1", "#emotechat_pemote1");
					AddPowerEmote(emotesByCategory[1], new Vector2(61f, 54f), new Vector2(19f, 85f), "mysquad_heroinfo_upgrades_power_emote_2", "#emotechat_pemote2");
					AddPowerEmote(emotesByCategory[2], new Vector2(67f, 57f), new Vector2(74f, 85f), "mysquad_heroinfo_upgrades_power_emote_3", "#emotechat_pemote3");
				}
			}

			public void AddPowerEmote(EmotesDefinition.EmoteDefinition def, Vector2 size, Vector2 offset, string source, string tooltip)
			{
				string failReason;
				bool flag = EmotesDefinition.Instance.RequirementsCheck(def.id, heroName, out failReason);
				GUIImage gUIImage = AddImage(size, offset, source);
				gUIImage.ToolTip = new NamedToolTipInfo(tooltip, new Vector2(41f, -5f));
				gUIImage.IsEnabled = flag;
				gUIImage.HitTestSize = new Vector2(0.925f, 0.925f);
				if (!flag)
				{
					AddLock(offset + new Vector2(15f, 15f), failReason);
				}
			}

			public GUIImage AddLockableImage(Vector2 size, Vector2 offset, Vector2 lockOffset, string source, bool disable, int unlockLevel)
			{
				GUIImage gUIImage = AddImage(size, offset, source);
				gUIImage.IsEnabled = !disable;
				if (disable)
				{
					AddLock(offset + lockOffset, unlockLevel);
				}
				return gUIImage;
			}

			public GUIImage AddImage(Vector2 size, Vector2 offset, string source)
			{
				GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(size, offset);
				gUIImage.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
				gUIImage.HitTestType = HitTestTypeEnum.Circular;
				gUIImage.TextureSource = "mysquadgadget_bundle|" + source;
				Add(gUIImage);
				return gUIImage;
			}

			public GUIImage AddLock(Vector2 offset, int unlockLevel)
			{
				return AddLock(offset, MySquadDataManager.GetUnlockAtLevelText(unlockLevel));
			}

			public GUIImage AddLock(Vector2 offset, string tooltip)
			{
				GUIImage gUIImage = AddImage(new Vector2(32f, 35f), offset, "mysquad_gadget_charactericon_lock");
				gUIImage.ToolTip = new NamedToolTipInfo(tooltip, new Vector2(15f, -5f));
				gUIImage.HitTestSize = new Vector2(0.875f, 0.875f);
				return gUIImage;
			}
		}

		private SHSMySquadGadget BaseGadget;

		private FadeInOut FadeUpgrades;

		private FadeInOut FadeMedals;

		private FadeInOut FadeItems;

		public HeroInfoTabbedData(SHSMySquadGadget BaseGadget, string heroName)
		{
			this.BaseGadget = BaseGadget;
			FadeUpgrades = new FadeInOut(this);
			FadeMedals = new FadeInOut(this);
			FadeItems = new FadeInOut(this);
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(491f, 175f), new Vector2(-14f, -140f));
			gUIImage.TextureSource = "mysquadgadget_bundle|mysquad_heroinfo_panel";
			Add(gUIImage);
			GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(472f, 262f), new Vector2(-14f, 118f));
			gUIImage2.TextureSource = "mysquadgadget_bundle|mysquad_heroinfo_tabs_content_panel";
			Add(gUIImage2);
			CreateAndAddButtonTab(FadeUpgrades, "upgrades", new Vector2(-173f, -37f));
			CreateAndAddButtonTab(FadeMedals, "medals", new Vector2(-35f, -38f));
			CreateAndAddButtonTab(FadeItems, "items", new Vector2(105f, -39f));
			CreateAndAddImageTab(FadeUpgrades, "upgrades");
			CreateAndAddImageTab(FadeMedals, "medals");
			CreateAndAddImageTab(FadeItems, "items");
			SetupTab(FadeUpgrades, new UpgradesTab(BaseGadget, heroName));
			SetupTab(FadeMedals, new MedalsTab(BaseGadget, heroName));
			SetupTab(FadeItems, new ItemsTab(BaseGadget, heroName));
			Add(new HeroInfoPortrait(BaseGadget, heroName, true));
		}

		public override void OnShow()
		{
			base.OnShow();
			FadeUpgrades.SetState(BaseGadget.DataManager.CurrentlySelectedTab == MySquadDataManager.Tabs.Upgrades);
			FadeMedals.SetState(BaseGadget.DataManager.CurrentlySelectedTab == MySquadDataManager.Tabs.Medals);
			FadeItems.SetState(BaseGadget.DataManager.CurrentlySelectedTab == MySquadDataManager.Tabs.Items);
		}

		public void SetupTab(FadeInOut toAdd, GenericTab tab)
		{
			tab.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			tab.IsVisible = false;
			toAdd.RegisterFade(tab);
			Add(tab);
		}

		public GUIButton CreateAndAddButtonTab(FadeInOut toAdd, string path, Vector2 offset)
		{
			GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), offset);
			gUIButton.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			gUIButton.IsVisible = false;
			gUIButton.HitTestSize = new Vector2(0.539f, 0.148f);
			gUIButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|L_mysquad_heroinfo_" + path + "_tab");
			gUIButton.Click += delegate
			{
				toAdd.FadeIn();
				if (toAdd != FadeUpgrades)
				{
					FadeUpgrades.FadeOut();
				}
				else
				{
					BaseGadget.DataManager.CurrentlySelectedTab = MySquadDataManager.Tabs.Upgrades;
				}
				if (toAdd != FadeMedals)
				{
					FadeMedals.FadeOut();
				}
				else
				{
					BaseGadget.DataManager.CurrentlySelectedTab = MySquadDataManager.Tabs.Medals;
				}
				if (toAdd != FadeItems)
				{
					FadeItems.FadeOut();
				}
				else
				{
					BaseGadget.DataManager.CurrentlySelectedTab = MySquadDataManager.Tabs.Items;
				}
			};
			toAdd.RegisterAntiFade(gUIButton);
			Add(gUIButton);
			return gUIButton;
		}

		public GUIImage CreateAndAddImageTab(FadeInOut toAdd, string path)
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(490f, 54f), new Vector2(-14f, -40f));
			gUIImage.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			gUIImage.IsVisible = false;
			gUIImage.TextureSource = "mysquadgadget_bundle|L_mysquad_heroinfo_" + path + "_tab_active";
			toAdd.RegisterFade(gUIImage);
			Add(gUIImage);
			return gUIImage;
		}
	}

	public class HeroInfoPortrait : GUISimpleControlWindow
	{
		public HeroInfoPortrait(SHSMySquadGadget BaseGadget, string heroName, bool owned)
		{
			SetSize(2000f, 2000f);
			SetPosition(QuickSizingHint.Centered);
			Offset = new Vector2(5f, -149f);
			GUIDropShadowTextLabel gUIDropShadowTextLabel = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(400f, 50f), new Vector2(-33f, -38f));
			gUIDropShadowTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 32, GUILabel.GenColor(255, 248, 141), GUILabel.GenColor(0, 0, 0), new Vector2(-2f, 2f), TextAnchor.MiddleLeft);
			gUIDropShadowTextLabel.BackColorAlpha = 0.2f;
			gUIDropShadowTextLabel.Text = AppShell.Instance.CharacterDescriptionManager[heroName].CharacterName;
			Add(gUIDropShadowTextLabel);
			if (gUIDropShadowTextLabel.GetTextWidth() > 276f)
			{
				gUIDropShadowTextLabel.FontSize = 28;
			}
			if (owned)
			{
				GUILabel gUILabel = CreateAndAddLabel(new Vector2(-33f, -18f), GUILabel.GenColor(159, 246, 36), 18);
				GUILabel gUILabel2 = CreateAndAddLabel(new Vector2(57f, -18f), GUILabel.GenColor(255, 124, 52), 18);
				gUILabel.Text = XpToLevelDefinition.GetLevelText(BaseGadget.ActiveProfile, heroName);
				gUILabel2.Text = XpToLevelDefinition.GetExpText(BaseGadget.ActiveProfile, heroName);
			}
			else
			{
				GUILabel gUILabel3 = CreateAndAddLabel(new Vector2(-33f, -18f), GUILabel.GenColor(133, 133, 133), 18);
				gUILabel3.Text = "#you_do_not_own_hero";
			}
			GUILabel gUILabel4 = CreateAndAddLabel(new Vector2(-93f, 37f), GUILabel.GenColor(255, 255, 255), 14);
			gUILabel4.SetSize(281f, 84f);
			gUILabel4.VerticalKerning = 14;
			gUILabel4.Text = AppShell.Instance.CharacterDescriptionManager[heroName].LongDescription;
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(200f, 200f), new Vector2(136f, -5f));
			gUIImage.TextureSource = "characters_bundle|expandedtooltip_render_" + heroName;
			Add(gUIImage);
		}

		public GUILabel CreateAndAddLabel(Vector2 offset, Color color, int fontSize)
		{
			GUILabel gUILabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(400f, 50f), offset);
			gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, fontSize, color, TextAnchor.MiddleLeft);
			Add(gUILabel);
			return gUILabel;
		}
	}

	public class HeroInfoLockedTab : GUISimpleControlWindow
	{
		public HeroInfoLockedTab(SHSMySquadGadget BaseGadget, string heroName)
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(491f, 175f), new Vector2(-14f, -140f));
			gUIImage.TextureSource = "mysquadgadget_bundle|mysquad_heroinfo_panel";
			Add(gUIImage);
			GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(485f, 335f), new Vector2(-21f, 93f));
			gUIImage2.TextureSource = "mysquadgadget_bundle|L_mysquad_heroinfo_locked_tabs";
			Add(gUIImage2);
			Add(new HeroInfoPortrait(BaseGadget, heroName, false));
			GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(223f, 155f), new Vector2(79f, 141f));
			gUIButton.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|L_buyit_button");
			gUIButton.HitTestSize = new Vector2(0.87f, 0.79f);
			Add(gUIButton);
			gUIButton.Click += delegate
			{
				BaseGadget.InteractionManager.PurchaseHero(heroName);
			};
		}
	}

	public class FadeInOut
	{
		private GUIWindow window;

		private List<GUIControl> toFade = new List<GUIControl>();

		private List<GUIControl> toAntiFade = new List<GUIControl>();

		private AnimClip fadeAnim;

		public FadeInOut(GUIWindow window)
		{
			this.window = window;
		}

		public void RegisterFade(GUIControl ctrl)
		{
			toFade.Add(ctrl);
		}

		public void RegisterAntiFade(GUIControl ctrl)
		{
			toAntiFade.Add(ctrl);
		}

		public void SetState(bool on)
		{
			toFade.ForEach(delegate(GUIControl ctrl)
			{
				ctrl.IsVisible = on;
				ctrl.Alpha = (on ? 1 : 0);
			});
			toAntiFade.ForEach(delegate(GUIControl ctrl)
			{
				ctrl.IsVisible = !on;
				ctrl.Alpha = ((!on) ? 1 : 0);
			});
		}

		public void FadeIn()
		{
			window.AnimationPieceManager.SwapOut(ref fadeAnim, SHSAnimations.Generic.FadeInVis(toFade, 0.2f) ^ SHSAnimations.Generic.FadeOutVis(toAntiFade, 0.2f));
		}

		public void FadeOut()
		{
			window.AnimationPieceManager.SwapOut(ref fadeAnim, SHSAnimations.Generic.FadeOutVis(toFade, 0.2f) ^ SHSAnimations.Generic.FadeInVis(toAntiFade, 0.2f));
		}
	}

	public class SHSMySquadCharacterSelect : GadgetLeftWindow
	{
		public class CharacterSelection : SHSSelectionWindow<CharacterItem, SHSItemLoadingWindow>
		{
			public CharacterSelection(GUISlider slider)
				: base(slider, SelectionWindowType.ThreeAcross)
			{
				SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
			}

			public void Sort(string sort)
			{
				string text = GetSearchName(sort);
				bool flag = false;
				while (!flag)
				{
					foreach (CharacterItem item in items)
					{
						item.active = item.NameMatch(text);
						if (item.active)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						if (text == string.Empty)
						{
							return;
						}
						text = text.Substring(0, text.Length - 1);
					}
				}
				RequestARefresh();
			}
		}

		public class CharacterItem : SHSSelectionItem<SHSItemLoadingWindow>, IComparable<CharacterItem>
		{
			public GUIButton heroHead;

			private string name;

			private string searchName;

			public string Name
			{
				get
				{
					return name;
				}
			}

			public event HeroClickedDelegate HeroClicked;

			public CharacterItem(HeroPersisted hero, HeroClickedDelegate heroClickDelegate, SelectionState defaultState)
			{
				name = hero.Name;
				searchName = GetSearchName(AppShell.Instance.CharacterDescriptionManager[hero.Name].CharacterFamily);
				item = new SHSItemLoadingWindow();
				item.HitTestType = HitTestTypeEnum.Transparent;
				itemSize = new Vector2(103f, 103f);
				currentState = defaultState;
				heroHead = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(103f, 103f), new Vector2(0f, 0f));
				heroHead.StyleInfo = new SHSButtonStyleInfo("characters_bundle|inventory_character_" + name, SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
				item.Add(heroHead);
				if (defaultState.ToString() == "Special")
				{
					GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(32f, 35f), new Vector2(27f, 27f));
					gUIImage.TextureSource = "mysquadgadget_bundle|mysquad_gadget_charactericon_lock";
					item.Add(gUIImage);
				}
				else
				{
					GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(39f, 32f), new Vector2(27f, 27f));
					gUIImage2.TextureSource = "mysquadgadget_bundle|L_character_icon_level_container";
					item.Add(gUIImage2);
					GUILabel gUILabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(80f, 80f), new Vector2(26f, 28f));
					gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, GUILabel.GenColor(58, 71, 94), TextAnchor.MiddleCenter);
					gUILabel.Text = hero.Level.ToString();
					if (hero.Level == hero.MaxLevel)
					{
						gUILabel.Text = MySquadDataManager.GetMax();
						gUILabel.FontSize = 9;
					}
					item.Add(gUILabel);
				}
				if (heroClickDelegate != null)
				{
					this.HeroClicked = (HeroClickedDelegate)Delegate.Combine(this.HeroClicked, heroClickDelegate);
				}
				heroHead.HitTestType = HitTestTypeEnum.Circular;
				heroHead.HitTestSize = new Vector2(0.6f, 0.6f);
				heroHead.Click += delegate
				{
					if (this.HeroClicked != null)
					{
						this.HeroClicked(name);
					}
				};
				heroHead.ToolTip = new NamedToolTipInfo(AppShell.Instance.CharacterDescriptionManager[hero.Name].CharacterName);
				if (hero.ShieldAgentOnly)
				{
					currentState = SelectionState.Subscription;
				}
			}

			public int CompareTo(CharacterItem other)
			{
				if (currentState == other.currentState)
				{
					return name.CompareTo(other.name);
				}
				if (currentState.ToString() == "Special")
				{
					return 1;
				}
				if (other.currentState.ToString() == "Special")
				{
					return -1;
				}
				return name.CompareTo(other.name);
			}

			public bool NameMatch(string partName)
			{
				return searchName.Contains(partName);
			}
		}

		public delegate void HeroClickedDelegate(string heroName);

		public CharacterSelection characterSelection;

		public event HeroClickedDelegate HeroClicked;

		public SHSMySquadCharacterSelect(UserProfile profile, HeroClickedDelegate heroClickDelegate)
			: this(GetHeroListFromProfile(profile, heroClickDelegate))
		{
		}

		public SHSMySquadCharacterSelect(List<CharacterItem> heroesToDisplay)
		{
			SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(314f, 492f), Vector2.zero);
			gUIImage.TextureSource = "persistent_bundle|leftselectwindow_backframe";
			GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(314f, 492f), Vector2.zero);
			gUIImage2.TextureSource = "persistent_bundle|leftselectwindow_frontframe";
			GUISlider gUISlider = GUIControl.CreateControlFrameCentered<GUISlider>(new Vector2(50f, 360f), new Vector2(131f, 30f));
			gUISlider.ArrowsEnabled = true;
			characterSelection = new CharacterSelection(gUISlider);
			characterSelection.AddList(heroesToDisplay);
			characterSelection.SortItemList();
			characterSelection.SetSize(227f, 380f);
			characterSelection.SetPosition(54f, 89f);
			foreach (CharacterItem item in heroesToDisplay)
			{
				item.HeroClicked += OnHeroClicked;
			}
			GUIImage gUIImage3 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(253f, 55f), new Vector2(0f, -191f));
			gUIImage3.TextureSource = "persistent_bundle|curved_searchbar";
			GUIImage gUIImage4 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(81f, 81f), new Vector2(102f, -190f));
			gUIImage4.TextureSource = "persistent_bundle|gadget_searchbutton_normal";
			GUITextField search = GUIControl.CreateControlFrameCentered<GUITextField>(new Vector2(178f, 55f), new Vector2(-20f, -191f));
			search.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, GUILabel.GenColor(81, 82, 81), TextAnchor.MiddleLeft);
			search.Rotation = -3f;
			search.WordWrap = false;
			search.Changed += delegate
			{
				characterSelection.Sort(search.Text);
			};
			Add(gUIImage);
			Add(characterSelection);
			Add(gUIImage2);
			Add(gUIImage3);
			Add(gUIImage4);
			Add(search);
			Add(gUISlider);
			base.AnimationOnOpen = SHSAnimations.WindowOpenCloseDelegates.FadeIn(0.3f, this);
			base.AnimationOnClose = SHSAnimations.WindowOpenCloseDelegates.FadeOut(0.3f, this);
		}

		private void OnHeroClicked(string heroName)
		{
			if (this.HeroClicked != null)
			{
				this.HeroClicked(heroName);
			}
		}

		public void EnableHero(string heroName, bool enable)
		{
			CharacterItem characterItem = characterSelection.Find(delegate(CharacterItem item)
			{
				return item.Name == heroName;
			});
			if (characterItem != null)
			{
				characterItem.heroHead.IsEnabled = enable;
			}
		}

		public void EnableAllHero(bool enable)
		{
			foreach (CharacterItem item in characterSelection.items)
			{
				item.heroHead.IsEnabled = enable;
			}
		}

		public bool IsHeroEnabled(string heroName)
		{
			CharacterItem characterItem = characterSelection.Find(delegate(CharacterItem item)
			{
				return item.Name == heroName;
			});
			if (characterItem == null)
			{
				return false;
			}
			return characterItem.heroHead.IsEnabled;
		}

		public static List<CharacterItem> GetHeroListFromProfile(UserProfile profile, HeroClickedDelegate heroClickDelegate)
		{
			List<CharacterItem> list = new List<CharacterItem>();
			if (profile == null)
			{
				return list;
			}
			foreach (KeyValuePair<string, HeroPersisted> availableCostume in profile.AvailableCostumes)
			{
				HeroPersisted value = null;
				if (!profile.AvailableCostumes.TryGetValue(availableCostume.Key, out value))
				{
					CspUtils.DebugLog("Hero <" + availableCostume.Key + "> being added to hero UI was not found in the hero collection!");
				}
				else
				{
					list.Add(new CharacterItem(value, heroClickDelegate, SHSSelectionItem<SHSItemLoadingWindow>.SelectionState.Active));
				}
			}
			return list;
		}

		public static string GetSearchName(string name)
		{
			name = name.ToLowerInvariant();
			name = name.Replace("_", string.Empty);
			name = name.Replace("'", string.Empty);
			name = name.Replace("-", string.Empty);
			name = name.Replace(" ", string.Empty);
			name = name.Replace(".", string.Empty);
			name = name.Replace("!", string.Empty);
			return name;
		}
	}

	public enum MySquadWindowUp
	{
		HeroInfo,
		SquadInfo,
		LoadingPlayerProfile
	}

	private class MySquadTopWindow : GadgetTopWindow
	{
		public MySquadTopWindow()
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(164f, 43f), new Vector2(2f, 7f));
			gUIImage.TextureSource = "mysquadgadget_bundle|L_my_squad_gadget_title";
			Add(gUIImage);
		}
	}

	public class MySquadInfoButton : GUISimpleControlWindow
	{
		public GUIAnimatedButton Main;

		private GUIAnimatedButton name;

		private GUIAnimatedButton Anim;

		private AnimClip FrameAnim;

		private string PartialAnimPath;

		private string NormalPath;

		private int AnimLength;

		private bool IsSelected;

		private bool IsMouseOver;

		private bool AnimPlaying;

		public MySquadInfoButton(string path, int AnimLength)
		{
			SetSize(256f, 256f);
			SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle);
			this.AnimLength = AnimLength;
			PartialAnimPath = path + "_info_button_icon_highlight";
			NormalPath = path + "_info_button_icon_normal";
			Main = GUIControl.CreateControlFrameCentered<GUIAnimatedButton>(new Vector2(256f, 256f), new Vector2(0f, 0f));
			Main.HitTestType = HitTestTypeEnum.Circular;
			Main.HitTestSize = new Vector2(0.398f, 0.398f);
			Main.TextureSource = "mysquadgadget_bundle|info_button_background_normal";
			Add(Main);
			Anim = GUIControl.CreateControlFrameCentered<GUIAnimatedButton>(new Vector2(256f, 256f), new Vector2(0f, 0f));
			Anim.TextureSource = "mysquadgadget_bundle|" + NormalPath;
			Add(Anim);
			name = GUIControl.CreateControlFrameCentered<GUIAnimatedButton>(new Vector2(256f, 256f), new Vector2(0f, 0f));
			name.TextureSource = "mysquadgadget_bundle|L_" + path + "_info_button_text_normal";
			Add(name);
			Anim.HitTestType = HitTestTypeEnum.Transparent;
			name.HitTestType = HitTestTypeEnum.Transparent;
			Anim.LinkToSourceButton(Main);
			name.LinkToSourceButton(Main);
			Main.MouseOver += delegate
			{
				IsMouseOver = true;
				UpdateAnimBasedOnState();
			};
			Main.MouseOut += delegate
			{
				IsMouseOver = false;
				UpdateAnimBasedOnState();
			};
		}

		public void SetSelected(bool IsSelected)
		{
			this.IsSelected = IsSelected;
			if (IsSelected)
			{
				SetupButton(1f, 1.05f, 1f);
			}
			else
			{
				SetupButton(0.85f, 0.95f, 0.82f);
			}
			UpdateAnimBasedOnState();
		}

		private void SetupButton(float NormalPercentage, float HighlightPercentage, float PressedPercentage)
		{
			Main.AnimSetupButton(NormalPercentage, HighlightPercentage, PressedPercentage);
			Anim.AnimSetupButton(NormalPercentage, HighlightPercentage, PressedPercentage);
			name.AnimSetupButton(NormalPercentage + 0.05f, HighlightPercentage + 0.05f, PressedPercentage + 0.05f);
		}

		public void UpdateAnimBasedOnState()
		{
			if (IsSelected)
			{
				if (!AnimPlaying)
				{
					BeginAnim();
				}
			}
			else if (IsMouseOver)
			{
				if (AnimPlaying)
				{
				}
			}
			else
			{
				StopAnim();
			}
		}

		private void BeginAnim()
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Expected O, but got Unknown
			AnimPlaying = true;
			AnimClip animClip = AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(0f, AnimLength, 1f), UpdateAnim) ^ AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Sin(0f, 1f, 1f), PulseButton);
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				BeginAnim();
			};
			base.AnimationPieceManager.SwapOut(ref FrameAnim, animClip);
		}

		private void StopAnim()
		{
			AnimPlaying = false;
			base.AnimationPieceManager.RemoveIfUnfinished(FrameAnim);
			Anim.TextureSource = "mysquadgadget_bundle|" + NormalPath;
			name.SetSize(256f, 256f);
		}

		private void PulseButton(float x)
		{
			Vector2 size = new Vector2(256f, 256f) * (1f + x * 0.0035f);
			name.SetSize(size);
		}

		private void UpdateAnim(float x)
		{
			int num = Mathf.FloorToInt(x) % AnimLength + 1;
			Anim.TextureSource = "mysquadgadget_bundle|" + PartialAnimPath + num;
		}
	}

	public class MySquadInteractionManager
	{
		private MySquadInfoButton heroInfo;

		private MySquadInfoButton squadInfo;

		private SHSMySquadGadget BaseGadget;

		public MySquadWindowUp CurrentWindowUp;

		public MySquadInteractionManager(SHSMySquadGadget BaseGadget)
		{
			this.BaseGadget = BaseGadget;
		}

		public void SetupGadgetOpening(MySquadWindowUp target)
		{
			BaseGadget.SetBackgroundSize(new Vector2(1020f, 644f));
			heroInfo = new MySquadInfoButton("hero", 4);
			heroInfo.Offset = new Vector2(-417f, -216f);
			heroInfo.Main.Click += delegate
			{
				GoTo(MySquadWindowUp.HeroInfo);
			};
			heroInfo.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			heroInfo.IsVisible = (target != MySquadWindowUp.LoadingPlayerProfile);
			BaseGadget.Add(heroInfo);
			squadInfo = new MySquadInfoButton("squad", 6);
			squadInfo.Offset = new Vector2(-432f, -374f);
			squadInfo.Main.Click += delegate
			{
				GoTo(MySquadWindowUp.SquadInfo);
			};
			squadInfo.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			squadInfo.IsVisible = (target != MySquadWindowUp.LoadingPlayerProfile);
			BaseGadget.Add(squadInfo);
			AnimClip OpenAnim = BaseGadget.AnimationOnOpen();
			OpenAnim ^= SHSAnimations.Generic.AnimationFadeTransitionIn(heroInfo, squadInfo);
			BaseGadget.AnimationOnOpen = delegate
			{
				return OpenAnim;
			};
			AnimClip CloseAnim = BaseGadget.AnimationOnClose();
			CloseAnim ^= SHSAnimations.Generic.AnimationFadeTransitionOut(heroInfo, squadInfo);
			BaseGadget.AnimationOnClose = delegate
			{
				return CloseAnim;
			};
			BaseGadget.SetupOpeningWindow(BackgroundType.OnePanel, GetCenterWindow(target));
			CurrentWindowUp = target;
			SetupButtonsSelected();
			BaseGadget.SetupOpeningTopWindow(new MySquadTopWindow());
			BaseGadget.SetBackgroundImage("mysquadgadget_bundle|my_squad_gadget_background_main");
			BaseGadget.CloseButton.Offset = new Vector2(457f, -556f);
			BaseGadget.CloseButton.SetSize(new Vector2(55f, 55f));
		}

		public void GoTo(MySquadWindowUp target)
		{
			if (BaseGadget.SetCenterWindow(GetCenterWindow(target)))
			{
				CurrentWindowUp = target;
				SetupButtonsSelected();
			}
		}

		public void SetupButtonsSelected()
		{
			heroInfo.SetSelected(CurrentWindowUp == MySquadWindowUp.HeroInfo);
			squadInfo.SetSelected(CurrentWindowUp == MySquadWindowUp.SquadInfo);
			heroInfo.IsVisible = (CurrentWindowUp != MySquadWindowUp.LoadingPlayerProfile);
			squadInfo.IsVisible = (CurrentWindowUp != MySquadWindowUp.LoadingPlayerProfile);
		}

		private GadgetCenterWindow GetCenterWindow(MySquadWindowUp target)
		{
			switch (target)
			{
			case MySquadWindowUp.SquadInfo:
				return BaseGadget.SquadInfo;
			case MySquadWindowUp.LoadingPlayerProfile:
				return BaseGadget.LoadInfo;
			default:
				return null;
			}
		}

		public void PurchaseHero(string heroName)
		{
			ShoppingWindow shoppingWindow = new ShoppingWindow(OwnableDefinition.HeroNameToHeroID[heroName]);
			shoppingWindow.launch();
			BaseGadget.CloseGadget();
		}

		private bool IsHeroNonSubscriber(string heroName)
		{
			HeroDefinition heroDef = OwnableDefinition.getHeroDef(heroName);
			if (heroDef == null)
			{
				return false;
			}
			return heroDef.ownableDef.subscriberOnly == 1;
		}
	}

	public class MySquadLoadingWindow : GadgetCenterWindow
	{
		private GUIStrokeTextLabel loadingLabel;

		private GUIImage loadingImage;

		public MySquadLoadingWindow(SHSMySquadGadget BaseGadget)
		{
			loadingLabel = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(400f, 100f), new Vector2(110f, -35f));
			loadingLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 44, GUILabel.GenColor(208, 233, 110), GUILabel.GenColor(14, 38, 116), GUILabel.GenColor(55, 53, 15), new Vector2(1f, 1f), TextAnchor.MiddleLeft);
			loadingLabel.Text = "#MYSQUAD_LOADING_TEXT";
			Add(loadingLabel);
			loadingImage = new GUIImage();
			loadingImage.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-3f, 32f), new Vector2(100f, 100f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			loadingImage.TextureSource = "GUI/blank";
			loadingImage.ConfigureRequiredContent(new ContentReference(ContentTypeEnum.Other, null));
			Add(loadingImage);
		}
	}

	public class MySquadSquadInfo : GadgetCenterWindow
	{
		public class ShieldBadge : GUISimpleControlWindow
		{
			public const float FPS = 12f;

			public const float REFRESH_TIME = 10f;

			private GUIImage bkg;

			private GUIImage badge;

			private GUIHotSpotButton button;

			private AnimClip shimmerCountdown;

			private AnimClip shimmerOnShow;

			private string path;

			private int pathNumber = -1;

			private int MaxLength;

			public ShieldBadge(string PartialPath, int MaxLength)
			{
				this.MaxLength = MaxLength;
				path = "mysquadgadget_bundle|L_shield_" + PartialPath + "_logo_";
				SetPosition(QuickSizingHint.Centered);
				SetSize(191f, 191f);
				bkg = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(191f, 191f), Vector2.zero);
				bkg.TextureSource = "mysquadgadget_bundle|mysquad_shield_logo_dropshadow";
				Add(bkg);
				badge = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(178f, 178f), Vector2.zero);
				badge.Rotation = -15f;
				Add(badge);
				SetPath(0);
				button = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(new Vector2(178f, 178f), Vector2.zero);
				button.HitTestType = HitTestTypeEnum.Circular;
				button.MouseOver += delegate
				{
					if (pathNumber == 0)
					{
						BeginShimmer();
						base.AnimationPieceManager.RemoveIfUnfinished(shimmerOnShow);
					}
				};
				Add(button);
			}

			public override void OnShow()
			{
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Expected O, but got Unknown
				base.OnShow();
				AnimClip animClip = SHSAnimations.Generic.Wait(1.5f);
				animClip.OnFinished += (Action)(object)(Action)delegate
				{
					BeginShimmer();
				};
				base.AnimationPieceManager.SwapOut(ref shimmerOnShow, animClip);
			}

			public void BeginShimmer()
			{
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0038: Expected O, but got Unknown
				AnimClip animClip = AnimClipBuilder.Custom.Function(AnimClipBuilder.Path.Linear(0f, 120f, 10f), Shimmer);
				animClip.OnFinished += (Action)(object)(Action)delegate
				{
					BeginShimmer();
				};
				base.AnimationPieceManager.SwapOut(ref shimmerCountdown, animClip);
			}

			public void Shimmer(float i)
			{
				int num = Mathf.RoundToInt(i);
				if (num > MaxLength)
				{
					num = 0;
				}
				SetPath(num);
			}

			private void SetPath(int i)
			{
				if (i != pathNumber)
				{
					pathNumber = i;
					if (i < 10)
					{
						badge.TextureSource = path + "0" + i;
					}
					else
					{
						badge.TextureSource = path + i;
					}
				}
			}
		}

		private SHSMySquadGadget BaseGadget;

		private GUIDropShadowTextLabel SquadName;

		private GUIDropShadowTextLabel SecurityLevel;

		private GUIDropShadowTextLabel BronzeMedals;

		private GUIDropShadowTextLabel SilverMedals;

		private GUIDropShadowTextLabel GoldMedals;

		private GUIDropShadowTextLabel AdamantiumMedals;

		private GUILabel CurrencyGold;

		private GUILabel CurrencySilver;

		private GUILabel CurrencyTickets;

		public MySquadSquadInfo(SHSMySquadGadget BaseGadget)
		{
			this.BaseGadget = BaseGadget;
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(784f, 483f), new Vector2(31f, 36f));
			gUIImage.TextureSource = "mysquadgadget_bundle|my_squad_gadget_content_background_" + ((!BaseGadget.ActiveProfile.IsShieldPlayCapable) ? "recruit" : "agent");
			Add(gUIImage);
			GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(549f, 121f), new Vector2(27f, 108f));
			gUIImage2.TextureSource = "mysquadgadget_bundle|L_my_squad_gadget_squadmedal_container";
			Add(gUIImage2);
			GUIImage gUIImage3 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(617f, 91f), new Vector2(31f, 218f));
			gUIImage3.TextureSource = "mysquadgadget_bundle|L_my_squad_gadget_currency_container";
			Add(gUIImage3);
			GUIDropShadowTextLabel gUIDropShadowTextLabel = CreateAndAddTitleLabel(new Vector2(118f, -116f));
			gUIDropShadowTextLabel.Text = "#MySquad_SquadName";
			GUIDropShadowTextLabel gUIDropShadowTextLabel2 = CreateAndAddTitleLabel(new Vector2(118f, -60f));
			gUIDropShadowTextLabel2.Text = "#MySquad_SecurityLevel";
			SquadName = CreateAndAddGenericLabel(new Vector2(118f, -93f), GUILabel.GenColor(254, 207, 95), 21);
			SecurityLevel = CreateAndAddGenericLabel(new Vector2(118f, -29f), GUILabel.GenColor(172, 214, 9), 38);
			SecurityLevel.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
			SecurityLevel.ToolTip = new NamedToolTipInfo("Higher levels can unlock new things.", new Vector2(-300f, 0f));
			BronzeMedals = CreateAndAddMedalLabel(new Vector2(-149f, 136f), GUILabel.GenColor(134, 53, 25));
			SilverMedals = CreateAndAddMedalLabel(new Vector2(-18f, 136f), GUILabel.GenColor(83, 88, 101));
			GoldMedals = CreateAndAddMedalLabel(new Vector2(113f, 136f), GUILabel.GenColor(113, 76, 10));
			AdamantiumMedals = CreateAndAddMedalLabel(new Vector2(266f, 136f), GUILabel.GenColor(70, 83, 117));
			CurrencyGold = CreateAndAddCurrencyLabel(new Vector2(26f, 233f), GUILabel.GenColor(115, 83, 0));
			CurrencySilver = CreateAndAddCurrencyLabel(new Vector2(208f, 233f), GUILabel.GenColor(75, 93, 113));
			CurrencyTickets = CreateAndAddCurrencyLabel(new Vector2(394f, 233f), GUILabel.GenColor(30, 99, 209));
			GUIImage gUIImage4 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(179f, 32f), new Vector2(-228f, -154f));
			gUIImage4.TextureSource = "mysquadgadget_bundle|L_my_squad_gadget_status_title";
			Add(gUIImage4);
			GUIImage gUIImage5 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(214f, 37f), new Vector2(-190f, 41f));
			gUIImage5.TextureSource = "mysquadgadget_bundle|L_my_squad_gadget_missionratings_title";
			Add(gUIImage5);
			string partialPath = (!Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldPlayAllow)) ? "recruit" : "agent";
			int maxLength = (!Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldPlayAllow)) ? 10 : 12;
			ShieldBadge shieldBadge = new ShieldBadge(partialPath, maxLength);
			shieldBadge.Offset = new Vector2(-186f, -66f);
			Add(shieldBadge);
		}

		public override void OnShow()
		{
			base.OnShow();
			SetupText();
		}

		public GUIDropShadowTextLabel CreateAndAddTitleLabel(Vector2 offset)
		{
			GUIDropShadowTextLabel gUIDropShadowTextLabel = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(400f, 50f), offset);
			gUIDropShadowTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(250, 242, 197), GUILabel.GenColor(0, 21, 105), new Vector2(-2f, 2f), TextAnchor.MiddleLeft);
			Add(gUIDropShadowTextLabel);
			return gUIDropShadowTextLabel;
		}

		public GUIDropShadowTextLabel CreateAndAddGenericLabel(Vector2 offset, Color color, int fontSize)
		{
			GUIDropShadowTextLabel gUIDropShadowTextLabel = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(400f, 50f), offset);
			gUIDropShadowTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, fontSize, color, GUILabel.GenColor(0, 21, 105), new Vector2(-2f, 2f), TextAnchor.MiddleLeft);
			Add(gUIDropShadowTextLabel);
			return gUIDropShadowTextLabel;
		}

		public GUIDropShadowTextLabel CreateAndAddMedalLabel(Vector2 offset, Color color)
		{
			GUIDropShadowTextLabel gUIDropShadowTextLabel = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(400f, 50f), offset);
			gUIDropShadowTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 33, color, GUILabel.GenColor(255, 255, 255), new Vector2(-2f, 2f), TextAnchor.MiddleCenter);
			gUIDropShadowTextLabel.BackColorAlpha = 0.7f;
			Add(gUIDropShadowTextLabel);
			return gUIDropShadowTextLabel;
		}

		public GUILabel CreateAndAddCurrencyLabel(Vector2 offset, Color color)
		{
			GUILabel gUILabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(400f, 50f), offset);
			gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 19, color, TextAnchor.MiddleLeft);
			Add(gUILabel);
			return gUILabel;
		}

		public void SetupText()
		{
			string partialPath = (!BaseGadget.DataManager.Profile.IsShieldPlayCapable) ? "recruit" : "agent";
			int maxLength = (!BaseGadget.DataManager.Profile.IsShieldPlayCapable) ? 10 : 12;
			ShieldBadge shieldBadge = new ShieldBadge(partialPath, maxLength);
			shieldBadge.Offset = new Vector2(-186f, -66f);
			Add(shieldBadge);
			SquadName.Text = BaseGadget.DataManager.Profile.PlayerName;
			SecurityLevel.Text = BaseGadget.DataManager.Profile.SquadLevel.ToString();
			long[] missionMedals = getMissionMedals();
			BronzeMedals.Text = GetPrettyNumber((int)missionMedals[3]);
			SilverMedals.Text = GetPrettyNumber((int)missionMedals[2]);
			GoldMedals.Text = GetPrettyNumber((int)missionMedals[1]);
			AdamantiumMedals.Text = GetPrettyNumber((int)missionMedals[0]);
			if (BaseGadget.DataManager.Profile.ProfileType == UserProfile.ProfileTypeEnum.LocalPlayer)
			{
				CurrencyGold.Text = GetPrettyNumber(BaseGadget.DataManager.Profile.Gold);
				CurrencySilver.Text = GetPrettyNumber(BaseGadget.DataManager.Profile.Silver);
				CurrencyTickets.Text = GetPrettyNumber(BaseGadget.DataManager.Profile.Tickets);
				return;
			}
			CurrencyGold.Offset = new Vector2(40f, 233f);
			CurrencyGold.Text = "- - -";
			CurrencySilver.Text = "- - -";
			CurrencySilver.Offset = new Vector2(223f, 233f);
			CurrencyTickets.Text = "- - -";
			CurrencyTickets.Offset = new Vector2(414f, 233f);
		}

		public static string GetPrettyNumber(int amount)
		{
			string text = amount.ToString();
			if (text.Length > 3)
			{
				List<string> list = new List<string>(3);
				string text2 = string.Empty;
				for (int num = text.Length - 1; num >= 0; num--)
				{
					text2 = text[num] + text2;
					if (text2.Length == 3)
					{
						list.Add(text2);
						text2 = string.Empty;
					}
				}
				if (text2.Length != 0)
				{
					list.Add(text2);
				}
				if (list.Count == 2)
				{
					return list[1] + "," + list[0];
				}
				if (list.Count == 3)
				{
					return list[2] + "," + list[1] + "," + list[0];
				}
			}
			return text;
		}

		private long[] getMissionMedals()
		{
			long[] array = new long[4];
			Dictionary<string, ISHSCounterType> counters = AppShell.Instance.CounterManager.Counters;
			for (int i = 0; i < array.Length; i++)
			{
				string text = string.Empty;
				switch (i)
				{
				case 0:
					text = "FLAWLESSHEROCOUNTER";
					break;
				case 1:
					text = "HIGHMARKSCOUNTER";
					break;
				case 2:
					text = "SILVERMEDALS";
					break;
				case 3:
					text = "BRONZEMEDALS";
					break;
				}
				array[i] = 0L;
				ISHSCounterType value;
				if (counters.TryGetValue(text, out value))
				{
					foreach (KeyValuePair<string, HeroPersisted> availableCostume in BaseGadget.DataManager.Profile.AvailableCostumes)
					{
						long value2;
						if (value.QualifierValues[BaseGadget.DataManager.Profile.CounterBank].TryGetValue(availableCostume.Value.Name, out value2))
						{
							array[i] += value2;
						}
					}
				}
				else
				{
					CspUtils.DebugLog("Counter Name: " + text + " does not exist.  You will be getting a 0 for medal: " + i + " <0:Adamantium, 1:Gold, 2:Silver, 3:Bronze>");
				}
			}
			return array;
		}
	}

	private const float MIN_WAITTIME_FOR_PROFILE = 2f;

	private MySquadSquadInfo SquadInfo;

	private MySquadDataManager DataManager;

	private MySquadInteractionManager InteractionManager;

	private MySquadLoadingWindow LoadInfo;

	private UserProfile ActiveProfile;

	private bool profileShown;

	private bool profilePreloaded;

	private bool profileLoaded;

	private float startTime;

	private bool cancelled;

	public SHSMySquadGadget()
		: this(AppShell.Instance.Profile)
	{
	}

	public SHSMySquadGadget(long playerId)
	{
		InteractionManager = new MySquadInteractionManager(this);
		LoadInfo = new MySquadLoadingWindow(this);
		RemotePlayerProfile.FetchProfile(playerId, delegate(UserProfile profile)
		{
			if (cancelled)
			{
				((RemotePlayerProfile)profile).Dispose();
			}
			else
			{
				InitializeGadgetWithProfile(profile, MySquadWindowUp.SquadInfo);
				profileLoaded = true;
			}
		});
		InteractionManager.SetupGadgetOpening(MySquadWindowUp.LoadingPlayerProfile);
	}

	public SHSMySquadGadget(UserProfile profile)
		: this(profile, MySquadWindowUp.SquadInfo)
	{
	}

	public SHSMySquadGadget(UserProfile profile, MySquadWindowUp targetWindow)
	{
		InteractionManager = new MySquadInteractionManager(this);
		InitializeGadgetWithProfile(profile, targetWindow);
		profilePreloaded = true;
		InteractionManager.SetupGadgetOpening(targetWindow);
	}

	public override void OnUpdate()
	{
		if (!profilePreloaded && !profileShown)
		{
			if (profileLoaded && Time.time - startTime > 2f)
			{
				InteractionManager.GoTo(MySquadWindowUp.SquadInfo);
				profileShown = true;
			}
			base.OnUpdate();
		}
	}

	private void InitializeGadgetWithProfile(UserProfile profile, MySquadWindowUp targetWindow)
	{
		ActiveProfile = profile;
		DataManager = new MySquadDataManager(profile);
		SquadInfo = new MySquadSquadInfo(this);
	}

	public override void OnShow()
	{
		base.OnShow();
		startTime = Time.time;
	}

	public override void OnHide()
	{
		if (ActiveProfile != null && ActiveProfile.ProfileType == UserProfile.ProfileTypeEnum.RemotePlayer)
		{
			((RemotePlayerProfile)ActiveProfile).Dispose();
		}
		if (!profilePreloaded && !profileLoaded)
		{
			cancelled = true;
		}
		base.OnHide();
	}
}
