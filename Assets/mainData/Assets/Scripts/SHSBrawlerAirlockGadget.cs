using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SHSBrawlerAirlockGadget : SHSGadget
{
	public class AutoSelectTimer : SHSTimerEx
	{
		private const int makeFillRedTime = 5;

		private const float totalRevolutionTime = 1f;

		private const float maxRotation = 360f;

		private GUIImage timerFill;

		private GUILabel timeText;

		private float currFillRot;

		private float currRotTime;

		private bool fillSwitched;

		public float StartingTime
		{
			get
			{
				return base.Duration;
			}
			set
			{
				base.Duration = value;
			}
		}

		public AutoSelectTimer(GUIImage timerFill, GUILabel timeText)
		{
			this.timerFill = timerFill;
			this.timeText = timeText;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			if (base.TimerState == TimerStateEnum.Running)
			{
				timeText.Text = Convert.ToInt32(base.TimeLeft).ToString();
				currRotTime += Time.deltaTime;
				if (currRotTime <= 1f)
				{
					currFillRot = 360f * currRotTime;
				}
				else
				{
					currFillRot = 0f;
					currRotTime = 0f;
				}
				timerFill.Rotation = currFillRot;
				if (Convert.ToInt32(base.TimeLeft) <= 5 && !fillSwitched)
				{
					fillSwitched = true;
					timerFill.TextureSource = "brawlergadget_bundle|timer_fill_red";
				}
			}
		}
	}

	public enum Player
	{
		p1,
		p2,
		p3,
		p4,
		MaxPlayers
	}

	public class AirlockSlotManager
	{
		public class AirlockSlotData
		{
			public Player player;

			public int userId;

			public AirlockSlotData(int userId, Player player)
			{
				this.userId = userId;
				this.player = player;
			}
		}

		private AirlockWindow airlockWindow;

		private SHSAirlockEventManager airlockEventManager;

		private List<string> missionBlacklistedHeroes = new List<string>();

		private List<AirlockSlotData> airlockSlots;

		private List<Player> availableSlots;

		public AirlockWindow Airlock
		{
			get
			{
				return airlockWindow;
			}
			set
			{
				airlockWindow = value;
			}
		}

		public SHSAirlockEventManager AirlockEventManager
		{
			get
			{
				return airlockEventManager;
			}
		}

		public Hashtable LockedHeroes
		{
			get
			{
				return airlockEventManager.LockedHeroes;
			}
		}

		public AirlockSlotManager(UserProfile profile, Dictionary<string, PowerAttackData> characterR2Info, AirlockWindow airlockWindow, SHSAirlockEventManager.OnRoomUserDrop onDrop)
		{
			this.airlockWindow = airlockWindow;
			airlockSlots = new List<AirlockSlotData>();
			availableSlots = new List<Player>();
			airlockEventManager = new SHSAirlockEventManager(this, characterR2Info, onDrop);
		}

		public void OnShow()
		{
			airlockSlots.Clear();
			availableSlots.Clear();
			availableSlots.Add(Player.p2);
			availableSlots.Add(Player.p3);
			availableSlots.Add(Player.p4);
			airlockEventManager.OnShow();
		}

		public void OnHide()
		{
			airlockEventManager.OnHide();
		}

		public SHSAirlockEventManager.AirlockSlot GetAirlockSlot(int userId)
		{
			return airlockEventManager.GetAirlockSlot(userId);
		}

		public void lockHeroesBecauseOfMission(string missionID)
		{
			AppShell.Instance.DataManager.LoadGameData("Missions/" + missionID, OnMissionDefinitionLoaded, new MissionDefinition());
		}

		protected void OnMissionDefinitionLoaded(GameDataLoadResponse response, object extraData)
		{
			if (response.Error != null && response.Error != string.Empty)
			{
				CspUtils.DebugLog("SHSBRawlerGadget: The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
				return;
			}
			MissionDefinition missionDefinition = response.DataDefinition as MissionDefinition;
			missionBlacklistedHeroes = missionDefinition.blacklistCharacters;
			refreshBlacklistedHeroes();
			airlockWindow.addBannedHeroes(missionBlacklistedHeroes);
		}

		public void refreshBlacklistedHeroes()
		{
			CspUtils.DebugLog("refreshing blacklisted heroes");
			foreach (string missionBlacklistedHero in missionBlacklistedHeroes)
			{
				LockedHeroes.Add(missionBlacklistedHero, true);
				CspUtils.DebugLog("adding " + missionBlacklistedHero + " to the ban list");
				if (missionBlacklistedHero == "non_villains")
				{
					foreach (string character in AppShell.Instance.CharacterDescriptionManager.CharacterList)
					{
						if (!AppShell.Instance.CharacterDescriptionManager.VillainList.Contains(character))
						{
							LockedHeroes.Add(character, true);
						}
					}
				}
			}
		}

		public void DisplayData(SHSAirlockEventManager.AirlockSlot airlockSlotToDisplay)
		{
			if (airlockSlotToDisplay.UserId == AppShell.Instance.ServerConnection.GetGameUserId() || airlockSlotToDisplay.UserId == -1)
			{
				return;
			}
			AddAirlockSlotData(airlockSlotToDisplay.UserId);
			AirlockSlotData slot = getSlot(airlockSlotToDisplay.UserId);
			if (slot != null)
			{
				if (airlockSlotToDisplay.UserName != string.Empty)
				{
					airlockWindow.SetName(slot.player, airlockSlotToDisplay.UserName);
				}
				if (airlockSlotToDisplay.CharacterId != string.Empty)
				{
					airlockWindow.SetPortrait(slot.player, airlockSlotToDisplay.CharacterId);
				}
				if (airlockSlotToDisplay.PowerNumber != -1)
				{
					airlockWindow.SetPower(slot.player, airlockSlotToDisplay.PowerNumber);
				}
				airlockWindow.SetState(slot.player, airlockSlotToDisplay.State);
			}
			else
			{
				CspUtils.DebugLog("The slot wasn't found. Can't change portrait data. This part of the code shouldn't of been reached. Client Id change?");
			}
		}

		public Player FindPlayer(int userId)
		{
			foreach (AirlockSlotData airlockSlot in airlockSlots)
			{
				if (airlockSlot.userId == userId)
				{
					CspUtils.DebugLog("Found the slot");
					return airlockSlot.player;
				}
			}
			return Player.p1;
		}

		public void ClearPlayer(SHSAirlockEventManager.AirlockSlot airlockSlotToRemove)
		{
			AirlockSlotData slot = getSlot(airlockSlotToRemove.UserId);
			if (slot != null)
			{
				airlockWindow.UnselectAll(slot.player);
			}
			RemoveAirlockSlotData(airlockSlotToRemove.UserId);
		}

		private void AddAirlockSlotData(int userId)
		{
			AirlockSlotData slot = getSlot(userId);
			if (slot == null)
			{
				if (availableSlots.Count == 0)
				{
					CspUtils.DebugLog("trying to add a player when the room is full");
					return;
				}
				availableSlots.Sort();
				airlockSlots.Add(new AirlockSlotData(userId, availableSlots[0]));
				availableSlots.RemoveAt(0);
			}
		}

		private void RemoveAirlockSlotData(int userId)
		{
			AirlockSlotData slot = getSlot(userId);
			if (slot != null)
			{
				airlockSlots.Remove(slot);
				availableSlots.Add(slot.player);
			}
		}

		private AirlockSlotData getSlot(int userId)
		{
			foreach (AirlockSlotData airlockSlot in airlockSlots)
			{
				if (airlockSlot.userId == userId)
				{
					return airlockSlot;
				}
			}
			return null;
		}
	}

	private class AirlockTitleWindow : GadgetTopWindow
	{
		private GUIImage missions;

		public AirlockTitleWindow()
		{
			GUIImage gUIImage = new GUIImage();
			gUIImage.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 10f));
			gUIImage.TextureSource = "persistent_bundle|gadget_topmodule";
			gUIImage.SetSize(592f, 141f);
			Add(gUIImage);
			missions = new GUIImage();
			missions.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f));
			missions.SetSize(285f, 54f);
			missions.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
			missions.TextureSource = "brawlergadget_bundle|L_title_missionlaunch";
			Add(missions);
		}

		public void SetTitleTextureSource(string textureSource)
		{
			missions.TextureSource = textureSource;
		}

		public void SetToolTip(ToolTipInfo toolTipInfo)
		{
			missions.ToolTip = toolTipInfo;
		}

		public void SetTitleSize(Vector2 titleSize)
		{
			missions.SetSize(titleSize);
		}
	}

	public class AirlockWindow : GadgetCenterWindow
	{
		public class AirlockChooseAHeroWindow : SHSCharacterSelectScanlineTransition
		{
			public class HeroInfoWindow : GUISimpleControlWindow
			{
				private const float totalFillWidth = 332f;

				private GUIDropShadowTextLabel powerAttackText;

				private GUIButton okButton;

				private GUIButton backButton;

				private AirlockChooseAHeroWindow headWindow;

				private SHSBrawlerGadget.BrawlerPowerSelection brawlerPowers;

				private SHSBrawlerGadget.BrawlerRAttacksNameLoader nameLoader = new SHSBrawlerGadget.BrawlerRAttacksNameLoader();

				public HeroInfoWindow(string heroKey, AirlockChooseAHeroWindow headWindow)
				{
					this.headWindow = headWindow;
					GUIDropShadowTextLabel gUIDropShadowTextLabel = GenDropShadow(new Vector2(300f, 100f), new Vector2(15f, 41f), 32, 255, 248, 141);
					gUIDropShadowTextLabel.FontFace = GUIFontManager.SupportedFontEnum.Zooom;
					gUIDropShadowTextLabel.TextOffset = new Vector2(-3f, 4f);
					gUIDropShadowTextLabel.Text = AppShell.Instance.CharacterDescriptionManager[heroKey].CharacterName;
					gUIDropShadowTextLabel.VerticalKerning = 25;
					StringTable stringTable = AppShell.Instance.stringTable;
					HeroPersisted heroPersisted = AppShell.Instance.Profile.AvailableCostumes[heroKey];
					int xp = heroPersisted.Xp;
					int level = heroPersisted.Level;
					bool flag = level == heroPersisted.MaxLevel;
					int xpForLevel = XpToLevelDefinition.Instance.GetXpForLevel((!flag) ? (level + 1) : level);
					string @string = stringTable.GetString(XpToLevelDefinition.Instance.GetLevelDescriptionForXp(xp, heroPersisted.Tier));
					GUILabel gUILabel = GenDropShadow(new Vector2(281f, 73f), new Vector2(20f, 98f), 18, 200, 255, 32);
					gUILabel.Text = string.Format(stringTable["#airlock_hero_level"], @string);
					GUILabel gUILabel2 = GenDropShadow(new Vector2(281f, 73f), new Vector2(120f, 98f), 18, 255, 174, 102);
					if (flag)
					{
						gUILabel2.Text = "#Max_Exp";
					}
					else
					{
						gUILabel2.Text = string.Format(AppShell.Instance.stringTable["#airlock_hero_experience"], xp.ToString(), xpForLevel.ToString());
					}
					GUILabel gUILabel3 = GenDropShadow(new Vector2(285f, 90f), new Vector2(18f, 120f), 15, 255, 255, 255);
					gUILabel3.Text = AppShell.Instance.CharacterDescriptionManager[heroKey].LongDescription;
					GUIImage gUIImage = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(183f, 183f), new Vector2(299f, 23f));
					gUIImage.TextureSource = "characters_bundle|expandedtooltip_render_" + heroKey;
					backButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(-175f, 215f));
					backButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_brawler_airlock_backbutton");
					backButton.HitTestSize = new Vector2(0.5f, 0.45f);
					backButton.Click += delegate
					{
						headWindow.BackWasClicked();
					};
					okButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(-20f, 202f));
					okButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_okbutton_rectangular");
					okButton.IsEnabled = false;
					okButton.HitTestSize = new Vector2(0.547f, 0.367f);
					okButton.ToolTip = new NamedToolTipInfo("#mission_herolist_ok");
					brawlerPowers = GUIControl.CreateControlCenter<SHSBrawlerGadget.BrawlerPowerSelection>(new Vector2(599f, 531f), new Vector2(230f, 338f));
					GUIImage gUIImage2 = GUIControl.CreateControlCenter<GUIImage>(new Vector2(494f, 413f), new Vector2(240f, 220f));
					gUIImage2.TextureSource = "brawlergadget_bundle|brawler_choosehero_screen_backdrop_new";
					GUIImage gUIImage3 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(63f, 53f), new Vector2(-215f, -45f));
					gUIImage3.TextureSource = "mysquadgadget_bundle|L_mysquad_heroinfo_xp_icon";
					float num = 332f * (float)xp / (float)xpForLevel;
					if (num > 332f)
					{
						num = 332f;
					}
					GUIImage gUIImage4 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(18f, 20f), new Vector2(-180f, -46f));
					gUIImage4.TextureSource = "mysquadgadget_bundle|mysquad_heroinfo_xpbar_left_cap";
					Vector2 size = new Vector2(num, 20f);
					Vector2 offset = gUIImage4.Offset;
					float x = offset.x;
					Vector2 size2 = gUIImage4.Size;
					GUIImage gUIImage5 = GUIControl.CreateControlFrameCentered<GUIImage>(size, new Vector2(x + size2.x * 0.5f + num * 0.5f, -46f));
					gUIImage5.TextureSource = "mysquadgadget_bundle|mysquad_heroinfo_xpbar_fill";
					Vector2 size3 = new Vector2(18f, 20f);
					Vector2 offset2 = gUIImage5.Offset;
					float x2 = offset2.x;
					Vector2 size4 = gUIImage5.Size;
					float num2 = x2 + size4.x * 0.5f;
					Vector2 size5 = gUIImage4.Size;
					GUIImage gUIImage6 = GUIControl.CreateControlFrameCentered<GUIImage>(size3, new Vector2(num2 + size5.x * 0.5f - 1f, -46f));
					gUIImage6.TextureSource = "mysquadgadget_bundle|mysquad_heroinfo_xpbar_right_cap";
					Vector2 size6 = gUIImage4.Size;
					float x3 = size6.x;
					Vector2 size7 = gUIImage5.Size;
					float num3 = x3 + size7.x;
					Vector2 size8 = gUIImage6.Size;
					float num4 = num3 + size8.x;
					Vector2 size9 = new Vector2(150f, 25f);
					Vector2 offset3 = gUIImage4.Offset;
					GUILabel gUILabel4 = GUIControl.CreateControlFrameCentered<GUILabel>(size9, new Vector2(offset3.x + num4 * 0.5f - 6.25f, -46f));
					gUILabel4.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, GUILabel.GenColor(33, 99, 5), TextAnchor.MiddleCenter);
					gUILabel4.Text = (flag ? "#herolist_bar_maxxp" : (xp.ToString() + "/" + xpForLevel.ToString()));
					if (xp == 0)
					{
						gUIImage4.IsVisible = (gUIImage5.IsVisible = (gUIImage6.IsVisible = false));
						gUILabel4.IsVisible = false;
					}
					powerAttackText = GUIControl.CreateControlCenter<GUIDropShadowTextLabel>(new Vector2(300f, 50f), Vector2.zero);
					powerAttackText.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 32, GUILabel.GenColor(255, 248, 141), TextAnchor.UpperLeft);
					powerAttackText.FrontColor = GUILabel.GenColor(255, 248, 141);
					powerAttackText.BackColor = GUILabel.GenColor(0, 0, 0);
					powerAttackText.TextOffset = new Vector2(-2f, 2f);
					powerAttackText.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-80f, 15f));
					powerAttackText.Text = "#brawler_power_attacks";
					GUIImage gUIImage7 = GUIControl.CreateControlCenter<GUIImage>(new Vector2(247f, 67f), new Vector2(237f, 247f));
					gUIImage7.TextureSource = "brawlergadget_bundle|L_title_supermoves";
					brawlerPowers.DisablePowerMovesBasedOnLevel(heroKey, level);
					AssignButtonDelegates();
					nameLoader.SetupTooltips(heroKey, brawlerPowers);
					Add(gUIImage2);
					Add(brawlerPowers);
					Add(gUIImage3);
					Add(powerAttackText);
					Add(gUIDropShadowTextLabel);
					Add(gUILabel);
					Add(gUIImage4);
					Add(gUIImage6);
					Add(gUIImage5);
					Add(gUILabel4);
					Add(gUILabel2);
					Add(gUILabel3);
					Add(gUIImage);
					Add(okButton);
					Add(backButton);
				}

				private void AssignButtonDelegates()
				{
					brawlerPowers.PowerMove1.Click += delegate
					{
						headWindow.PowerWasSelected(1);
						okButton.IsEnabled = true;
					};
					brawlerPowers.PowerMove2.Click += delegate
					{
						headWindow.PowerWasSelected(2);
						okButton.IsEnabled = true;
					};
					brawlerPowers.PowerMove3.Click += delegate
					{
						headWindow.PowerWasSelected(3);
						okButton.IsEnabled = true;
					};
				}

				public override void OnShow()
				{
					base.OnShow();
					okButton.Click += delegate
					{
						headWindow.OkWasClicked();
					};
					if (brawlerPowers.HeroUsed == AppShell.Instance.Profile.LastSelectedCostume)
					{
						switch (AppShell.Instance.Profile.LastSelectedPower)
						{
						case 1:
							brawlerPowers.PowerMove1.FireMouseClick(null);
							break;
						case 2:
							brawlerPowers.PowerMove2.FireMouseClick(null);
							break;
						case 3:
							brawlerPowers.PowerMove3.FireMouseClick(null);
							break;
						}
					}
					else if (brawlerPowers.PowerMove3.IsEnabled)
					{
						brawlerPowers.PowerMove3.FireMouseClick(null);
					}
					else if (brawlerPowers.PowerMove2.IsEnabled)
					{
						brawlerPowers.PowerMove2.FireMouseClick(null);
					}
					else
					{
						brawlerPowers.PowerMove1.FireMouseClick(null);
					}
				}

				public GUIDropShadowTextLabel GenDropShadow(Vector2 size, Vector2 offset, int fontSize, int r, int g, int b)
				{
					GUIDropShadowTextLabel gUIDropShadowTextLabel = GUIControl.CreateControlAbsolute<GUIDropShadowTextLabel>(size, offset);
					gUIDropShadowTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, fontSize, Color.black, TextAnchor.UpperLeft);
					gUIDropShadowTextLabel.TextOffset = new Vector2(-2f, 2f);
					gUIDropShadowTextLabel.FrontColor = GUILabel.GenColor(r, g, b);
					gUIDropShadowTextLabel.BackColor = GUILabel.GenColor(0, 21, 105);
					return gUIDropShadowTextLabel;
				}
			}

			private AirlockWindow airlockWindow;

			private AutoSelectTimer listTimer;

			private string selectedHero = string.Empty;

			private int selectedPower = -1;

			private SHSCharacterSelect characterSelect;

			private GUIImage timerBackground;

			private GUIImage timerCenter;

			private GUIImage timerFill;

			private GUIImage timerHighlight;

			private GUILabel timerText;

			private List<string> lockedHeroKeys = new List<string>();

			public string SelectedHero
			{
				get
				{
					return selectedHero;
				}
			}

			public int SelectedPower
			{
				get
				{
					return selectedPower;
				}
			}

			public SHSCharacterSelect CharacterSelect
			{
				get
				{
					return characterSelect;
				}
			}

			public AirlockChooseAHeroWindow(AirlockWindow airlockWindow)
			{
				this.airlockWindow = airlockWindow;
				characterSelect = new SHSCharacterSelect(AppShell.Instance.Profile, null, "brawlergadget_bundle|brawler_gadget_left_bg_frame", "brawlergadget_bundle|brawler_gadget_left_bg_back");
				SHSCharacterSelect sHSCharacterSelect = characterSelect;
				Vector2 characterSelectionSize = characterSelect.CharacterSelectionSize;
				float x = characterSelectionSize.x;
				Vector2 characterSelectionSize2 = characterSelect.CharacterSelectionSize;
				sHSCharacterSelect.SetCharacterSelectionSize(new Vector2(x, characterSelectionSize2.y - 20f));
				characterSelect.ApplyOffsetToCharacterSelectWindow(new Vector2(0f, -10f));
				CharacterSelectScanlineTransition(characterSelect, GetContentWindow, AppShell.Instance.Profile.LastSelectedCostume, SHSScanlineTransitionWindow<string>.DefaultScanlineTime, new Vector2(520f, 430f));
				base.HeroClicked += HeroWasClicked;
				timerBackground = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(128f, 128f), Vector2.zero);
				timerBackground.SetPosition(Vector2.zero, DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(300f, -25f));
				timerBackground.TextureSource = "brawlergadget_bundle|timer_base";
				timerBackground.IsVisible = false;
				timerCenter = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(128f, 128f), Vector2.zero);
				timerCenter.SetPosition(Vector2.zero, DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(300f, -25f));
				timerCenter.TextureSource = "brawlergadget_bundle|timer_center";
				timerCenter.IsVisible = false;
				timerFill = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(128f, 128f), Vector2.zero);
				timerFill.SetPosition(Vector2.zero, DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(300f, -25f));
				timerFill.TextureSource = "brawlergadget_bundle|timer_fill_green";
				timerFill.IsVisible = false;
				timerText = new GUILabel();
				timerText.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 38, GUILabel.GenColor(58, 71, 94), TextAnchor.MiddleCenter);
				timerText.SetPositionAndSize(Vector2.zero, DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(300f, -25f), new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				timerText.Text = 15f.ToString();
				timerText.IsVisible = false;
				timerHighlight = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(128f, 128f), Vector2.zero);
				timerHighlight.SetPosition(Vector2.zero, DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(300f, -25f));
				timerHighlight.TextureSource = "brawlergadget_bundle|timer_highlight";
				timerHighlight.IsVisible = false;
				Add(timerBackground);
				Add(timerFill);
				Add(timerCenter);
				Add(timerText);
				Add(timerHighlight);
				listTimer = new AutoSelectTimer(timerFill, timerText);
				Add(listTimer);
			}

			public void MakeHeroInfoWindowBlank()
			{
				SelectHero(string.Empty);
			}

			public SHSCharacterSelect GetCharacterSelect()
			{
				return characterSelect;
			}

			public GUISimpleControlWindow GetContentWindow(string heroName)
			{
				return (!(heroName != string.Empty)) ? new GUISimpleControlWindow() : new HeroInfoWindow(heroName, this);
			}

			public void HeroWasClicked(string heroName)
			{
				selectedHero = heroName;
			}

			public void PowerWasSelected(int powerNumber)
			{
				selectedPower = powerNumber;
			}

			public void OkWasClicked()
			{
				if (selectedHero != string.Empty)
				{
					airlockWindow.SetPortrait(Player.p1, selectedHero);
					AppShell.Instance.Profile.LastSelectedCostume = selectedHero;
					AppShell.Instance.Profile.LastSelectedPower = selectedPower;
				}
				if (selectedPower == -1)
				{
					selectedPower = 1;
				}
				airlockWindow.SetPower(Player.p1, selectedPower);
				airlockWindow.baseInfo.TransitionToAirlockWindow();
			}

			public void BackWasClicked()
			{
				airlockWindow.baseInfo.TransitionToAirlockWindow();
			}

			public override void Update()
			{
				base.Update();
				lockedHeroKeys.Clear();
				foreach (string key in airlockWindow.LockedHeroesRef.Keys)
				{
					lockedHeroKeys.Add(key);
				}
				characterSelect.EnableAllHero(true);
				for (int i = 0; i < lockedHeroKeys.Count; i++)
				{
					characterSelect.EnableHero(lockedHeroKeys[i], false);
				}
			}

			public override void OnShow()
			{
				base.OnShow();
				if (airlockWindow.BaseInfo.HostIsWaiting)
				{
					timerBackground.IsVisible = true;
					timerCenter.IsVisible = true;
					timerFill.IsVisible = true;
					timerHighlight.IsVisible = true;
					timerText.IsVisible = true;
					listTimer.Start();
				}
			}

			public override void OnHide()
			{
				base.OnHide();
				listTimer.Stop();
			}

			public void SetTimerInterval(int time)
			{
				listTimer.Duration = time;
			}
		}

		public class CharacterInfo : GUIControlWindow
		{
			private enum HorzLoc
			{
				right,
				left
			}

			private enum VertLoc
			{
				top,
				bottom
			}

			private class PlayerPortrait : GUIControlWindow
			{
				public const float defaultFrameWidth = 215f;

				public const float defaultFrameHeight = 196f;

				public const float centerFrameWidth = 165f;

				public const float defaultEdgeWidth = 25f;

				public const float defaultInnerFrameWidth = 131f;

				public const float defaultInnerFrameHeight = 131f;

				public const float defaultNameFrameWidth = 190f;

				public const float defaultNameFrameHeight = 28f;

				public const float defaultCostumeWidth = 200f;

				public const float defaultCostumeHeight = 218f;

				private const float innerFrameHOffset = 40f;

				private const float innerFrameVOffset = 18f;

				private const float nameFrameVOffset = 45f;

				private const float characterFrameHOffset = 10f;

				private const float heroNameVOffset = 12f;

				private const float powerIconHOffset = 20f;

				private const float powerIconVOffset = 10f;

				private const float costumeVOffset = 15f;

				private const string defaultHeroName = "#airlock_waiting_for_hero";

				private const string choosingHeroName = "#airlock_choosing_hero";

				private const float defaultNameAlpha = 0.7f;

				private const float readyNameAlpha = 1f;

				private const float defaultIconScale = 0.8f;

				private const float totalRevolutionTime = 1f;

				private const float completeRevolution = 360f;

				private const float squadNameDeficit = 5f;

				private const float totalSeekTime = 0.5f;

				private GUIImage leftPiece;

				private GUIImage rightPiece;

				private GUIImage backCenterPiece;

				private GUIImage leftExpandablePiece;

				private GUIImage rightExpandablePiece;

				private GUIImage heroCostume;

				private GUIImage playerFrame;

				private GUIImage playerNameFrame;

				private GUIImage timerBackFill;

				private GUIImage timerFrontFill;

				private GUIButton iconToReadFrom;

				private GUIButton powerIconButton;

				private GUILabel heroName;

				private GUILabel squadName;

				private SHSBrawlerGadget.BrawlerPowerSelection powerSelection;

				private SHSBrawlerGadget.BrawlerRAttacksNameLoader nameLoader;

				private Player playerType;

				private SHSAirlockEventManager.AirlockSlot.PlayerSlotState slotState;

				private string heroKey = string.Empty;

				private string previousHeroKeyLookup = string.Empty;

				private Hashtable maxAttackLookup = new Hashtable();

				private float seekTimer;

				private bool seekToolTipMode;

				private float portraitScale = 1f;

				private float currRotTime;

				private float currRotation;

				private CharacterInfo characterInfo;

				public GUIButton PowerIconButton
				{
					get
					{
						return powerIconButton;
					}
				}

				public PlayerPortrait(Player playerType, SHSBrawlerGadget.BrawlerRAttacksNameLoader nameLoader, CharacterInfo characterInfo)
				{
					this.playerType = playerType;
					this.nameLoader = nameLoader;
					this.characterInfo = characterInfo;
				}

				public override bool InitializeResources(bool reload)
				{
					if (reload)
					{
						return base.InitializeResources(reload);
					}
					portraitScale = ((playerType == Player.p1) ? 1f : 0.8f);
					leftPiece = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(25f, 196f), Vector2.zero);
					leftPiece.SetPosition(Vector2.zero);
					leftPiece.TextureSource = "brawlergadget_bundle|brawler_airlock_playerframe_back_left";
					GUIImage gUIImage = leftPiece;
					Vector2 size = leftPiece.Size;
					float x = size.x * portraitScale;
					Vector2 size2 = leftPiece.Size;
					gUIImage.SetSize(new Vector2(x, size2.y * portraitScale));
					leftExpandablePiece = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(0f * portraitScale, 196f * portraitScale), Vector2.zero);
					leftExpandablePiece.TextureSource = "brawlergadget_bundle|brawler_airlock_playerFrame_expand";
					GUIImage gUIImage2 = leftExpandablePiece;
					Vector2 size3 = leftPiece.Size;
					gUIImage2.SetPosition(new Vector2(size3.x, 0f));
					backCenterPiece = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(165f, 196f), Vector2.zero);
					GUIImage gUIImage3 = backCenterPiece;
					Vector2 size4 = leftPiece.Size;
					gUIImage3.SetPosition(new Vector2(size4.x, 0f));
					backCenterPiece.TextureSource = "brawlergadget_bundle|brawler_airlock_playerframe_back_center";
					GUIImage gUIImage4 = backCenterPiece;
					Vector2 size5 = backCenterPiece.Size;
					float num = size5.x * portraitScale;
					Vector2 size6 = leftExpandablePiece.Size;
					float x2 = num + size6.x;
					Vector2 size7 = backCenterPiece.Size;
					gUIImage4.SetSize(new Vector2(x2, size7.y * portraitScale));
					backCenterPiece.HitTestType = HitTestTypeEnum.Rect;
					backCenterPiece.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
					heroCostume = new GUIImage();
					heroCostume.IsVisible = false;
					powerIconButton = new GUIButton();
					powerIconButton.SetSize(new Vector2(76.8f * portraitScale, 76.8f * portraitScale));
					powerIconButton.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(-20f * portraitScale, 10f * portraitScale));
					powerIconButton.IsVisible = false;
					powerIconButton.Click += PowerIconClickEvent;
					rightExpandablePiece = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(0f * portraitScale, 196f * portraitScale), Vector2.zero);
					GUIImage gUIImage5 = rightExpandablePiece;
					Vector2 size8 = leftPiece.Size;
					float x3 = size8.x;
					Vector2 size9 = backCenterPiece.Size;
					gUIImage5.SetPosition(new Vector2(x3 + size9.x, 0f));
					rightExpandablePiece.TextureSource = "brawlergadget_bundle|brawler_airlock_playerFrame_expand";
					rightPiece = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(25f, 196f), Vector2.zero);
					rightPiece.TextureSource = "brawlergadget_bundle|brawler_airlock_playerframe_back_right";
					GUIImage gUIImage6 = rightPiece;
					Vector2 size10 = rightPiece.Size;
					float x4 = size10.x * portraitScale;
					Vector2 size11 = rightPiece.Size;
					gUIImage6.SetSize(new Vector2(x4, size11.y * portraitScale));
					GUIImage gUIImage7 = rightPiece;
					Vector2 position = rightExpandablePiece.Position;
					float x5 = position.x;
					Vector2 size12 = rightExpandablePiece.Size;
					gUIImage7.SetPosition(new Vector2(x5 + size12.x, 0f));
					playerFrame = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(131f * portraitScale, 131f * portraitScale), new Vector2(10f * portraitScale, 0f));
					playerFrame.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(40f * portraitScale, 18f * portraitScale));
					playerFrame.TextureSource = "brawlergadget_bundle|brawler_airlock_playerframe_front_back";
					timerFrontFill = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(131f * portraitScale, 131f * portraitScale), new Vector2(10f * portraitScale, 0f));
					timerFrontFill.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(40f * portraitScale, 18f * portraitScale));
					timerFrontFill.TextureSource = "brawlergadget_bundle|brawler_airlock_playerframe_front_front";
					timerFrontFill.IsVisible = false;
					timerFrontFill.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
					timerFrontFill.Id = "timerFrontFill";
					timerBackFill = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(131f * portraitScale, 131f * portraitScale), new Vector2(10f * portraitScale, 0f));
					timerBackFill.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(40f * portraitScale, 18f * portraitScale));
					timerBackFill.TextureSource = "brawlergadget_bundle|brawler_airlock_playerframe_front_timer";
					timerBackFill.IsVisible = false;
					timerBackFill.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
					timerBackFill.Id = "timerBackFill";
					playerNameFrame = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(190f * portraitScale, 28f * portraitScale), Vector2.zero);
					playerNameFrame.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 45f * portraitScale));
					playerNameFrame.TextureSource = "brawlergadget_bundle|brawler_airlock_playerframe_namecontainer";
					squadName = new GUILabel();
					GUILabel gUILabel = squadName;
					Vector2 offset = new Vector2(0f, 43f * portraitScale);
					Vector2 size13 = playerNameFrame.Size;
					float x6 = size13.x;
					Vector2 size14 = playerNameFrame.Size;
					gUILabel.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, offset, new Vector2(x6, size14.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
					squadName.SetupText(GUIFontManager.SupportedFontEnum.Komica, (playerType == Player.p1) ? 15 : 13, GUILabel.GenColor(58, 71, 94), TextAnchor.MiddleCenter);
					squadName.Text = string.Empty;
					heroName = new GUILabel();
					GUILabel gUILabel2 = heroName;
					Vector2 offset2 = new Vector2(0f, -12f * portraitScale);
					Vector2 size15 = backCenterPiece.Size;
					float x7 = size15.x;
					Vector2 size16 = squadName.Size;
					gUILabel2.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, offset2, new Vector2(x7, size16.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
					heroName.SetupText(GUIFontManager.SupportedFontEnum.Grobold, (playerType == Player.p1) ? 15 : 13, GUILabel.GenColor(58, 71, 94), TextAnchor.MiddleCenter);
					heroName.Text = string.Empty;
					heroName.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
					heroName.Alpha = 0.7f;
					float num2 = 25f * portraitScale * 2f;
					Vector2 size17 = rightExpandablePiece.Size;
					float num3 = num2 + size17.x;
					Vector2 size18 = leftExpandablePiece.Size;
					float num4 = num3 + size18.x;
					Vector2 size19 = backCenterPiece.Size;
					float width = num4 + size19.x;
					Vector2 size20 = backCenterPiece.Size;
					SetSize(width, size20.y);
					if (playerType != 0)
					{
						SetPortraitState(SHSAirlockEventManager.AirlockSlot.PlayerSlotState.Empty);
					}
					Add(leftPiece);
					Add(leftExpandablePiece);
					Add(backCenterPiece);
					Add(playerFrame);
					Add(heroCostume);
					Add(rightExpandablePiece);
					Add(rightPiece);
					Add(timerBackFill);
					Add(timerFrontFill);
					Add(playerNameFrame);
					Add(powerIconButton);
					Add(squadName);
					Add(heroName);
					return base.InitializeResources(reload);
				}

				protected override void dispose(bool disposing)
				{
					base.dispose(disposing);
					leftPiece = null;
					rightPiece = null;
					backCenterPiece = null;
					leftExpandablePiece = null;
					rightExpandablePiece = null;
					playerFrame = null;
					playerNameFrame = null;
					heroCostume = null;
					heroName = null;
					squadName = null;
				}

				public void SetSquadName(string squadName)
				{
					MakePortraitActive();
					this.squadName.Text = squadName;
					MakeSquadNameFit();
				}

				private void MakeSquadNameFit()
				{
					GUIContent gUIContent = new GUIContent(squadName.Text);
					Vector2 vector = squadName.Style.UnityStyle.CalcSize(gUIContent);
					if (!(vector.x >= 190f * portraitScale - 5f))
					{
						return;
					}
					string text = string.Empty;
					for (int i = 0; i < squadName.Text.Length; i++)
					{
						text += squadName.Text[i];
						gUIContent.text = text + "...";
						vector = squadName.Style.UnityStyle.CalcSize(gUIContent);
						if (vector.x >= 190f * portraitScale - 10f)
						{
							text = text.Remove(text.Length - 3, 3);
							text += "...";
							i = squadName.Text.Length;
						}
					}
					squadName.Text = text;
				}

				private void MakePortraitActive()
				{
					if (playerType != 0 && Alpha <= 1f)
					{
						Alpha = 1f;
					}
				}

				public void SetHeroName(string heroName)
				{
					string characterName = AppShell.Instance.CharacterDescriptionManager[heroName].CharacterName;
					heroKey = heroName;
					MakePortraitActive();
					GeneratePowerIconToolTips(heroName);
					if (this.heroName.Text != characterName.ToUpper() && heroName != string.Empty)
					{
						heroCostume.SetPositionAndSize(Vector2.zero, DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, -15f * portraitScale), new Vector2(200f * portraitScale, 218f * portraitScale), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
						heroCostume.IsVisible = true;
						heroCostume.TextureSource = "characters_bundle|" + heroName + "_HUD_default";
						if (playerType != 0 && slotState == SHSAirlockEventManager.AirlockSlot.PlayerSlotState.Ready)
						{
							VOManager.Instance.PlayVO("hero_name", new VOInputString(heroName));
						}
					}
					this.heroName.Text = characterName.ToUpper();
					this.heroName.Alpha = 1f;
					backCenterPiece.ToolTip = new NamedToolTipInfo(characterName);
				}

				private void GeneratePowerIconToolTips(string heroName)
				{
					if (heroName != string.Empty)
					{
						string characterName = AppShell.Instance.CharacterDescriptionManager[heroName].CharacterName;
						if (powerSelection == null)
						{
							powerSelection = new SHSBrawlerGadget.BrawlerPowerSelection();
						}
						if (this.heroName.Text != characterName.ToUpper() && playerType == Player.p1)
						{
							nameLoader.SetupTooltips(heroName, powerSelection);
						}
					}
				}

				public void SetPowerIconType(int powerType)
				{
					MakePortraitActive();
					if (powerSelection == null)
					{
						return;
					}
					GUIButton gUIButton = null;
					switch (powerType)
					{
					case 1:
						gUIButton = powerSelection.PowerMove1;
						break;
					case 2:
						gUIButton = powerSelection.PowerMove2;
						break;
					case 3:
						gUIButton = powerSelection.PowerMove3;
						break;
					}
					if (playerType == Player.p1)
					{
						if (!nameLoader.ToolTipsLoadedForHero(heroKey))
						{
							iconToReadFrom = gUIButton;
							seekTimer = 0f;
							seekToolTipMode = true;
						}
						else
						{
							powerIconButton.ToolTip = new NamedToolTipInfo(gUIButton.ToolTip.GetToolTipText());
							GUIManager.Instance.TooltipManager.RefreshToolTip();
						}
					}
					powerIconButton.StyleInfo = gUIButton.StyleInfo;
					powerIconButton.IsVisible = true;
				}

				private void RetrieveNewToolTips()
				{
					seekTimer += Time.deltaTime;
					if (seekTimer >= 0.5f)
					{
						if (nameLoader.ToolTipsLoadedForHero(heroKey))
						{
							powerIconButton.ToolTip = new NamedToolTipInfo(iconToReadFrom.ToolTip.GetToolTipText());
							GUIManager.Instance.TooltipManager.RefreshToolTip();
							seekToolTipMode = false;
						}
						else
						{
							seekTimer = 0f;
						}
					}
				}

				public override void OnUpdate()
				{
					base.OnUpdate();
					if (playerType != 0 && (slotState == SHSAirlockEventManager.AirlockSlot.PlayerSlotState.Empty || slotState == SHSAirlockEventManager.AirlockSlot.PlayerSlotState.Choosing) && heroName.Alpha > 0.7f)
					{
						heroName.Alpha = 0.7f;
					}
					if (seekToolTipMode)
					{
						RetrieveNewToolTips();
					}
					if (slotState == SHSAirlockEventManager.AirlockSlot.PlayerSlotState.Choosing)
					{
						currRotTime += Time.deltaTime;
						if (currRotTime <= 1f)
						{
							currRotation = 360f * currRotTime;
						}
						else
						{
							currRotation = 0f;
							currRotTime = 0f;
						}
						timerBackFill.Rotation = currRotation;
					}
				}

				private void PowerIconClickEvent(GUIControl sender, GUIClickEvent clickEvent)
				{
					if (playerType == Player.p1)
					{
						HeroPersisted heroPersisted = AppShell.Instance.Profile.AvailableCostumes[heroKey];
						StatLevelReqsDefinition instance = StatLevelReqsDefinition.Instance;
						UserProfile profile = AppShell.Instance.Profile;
						if (instance != null && heroPersisted != null && profile != null && previousHeroKeyLookup != heroKey)
						{
							maxAttackLookup[heroKey] = instance.GetMaxPowerAttackUnlockedAt(heroPersisted.Level);
							previousHeroKeyLookup = heroKey;
						}
						int num = (int)maxAttackLookup[heroKey];
						int num2 = characterInfo.PowerIndexSelected + 1;
						if (num2 > num)
						{
							num2 = 1;
						}
						AppShell.Instance.Profile.LastSelectedPower = num2;
						characterInfo.ExternalPowerSelected(num2);
					}
				}

				public void SetPortraitState(SHSAirlockEventManager.AirlockSlot.PlayerSlotState state)
				{
					slotState = state;
					if (slotState == SHSAirlockEventManager.AirlockSlot.PlayerSlotState.Choosing)
					{
						heroName.Text = "#airlock_choosing_hero";
						heroName.Alpha = 0.7f;
						backCenterPiece.ToolTip = new NamedToolTipInfo("#airlock_choosing_hero_tt");
					}
					else if (slotState == SHSAirlockEventManager.AirlockSlot.PlayerSlotState.Empty)
					{
						heroName.Text = "#airlock_waiting_for_hero";
						heroName.Alpha = 0.7f;
						squadName.Text = string.Empty;
						heroCostume.IsVisible = false;
						heroCostume.TextureSource = string.Empty;
						powerIconButton.IsVisible = false;
						backCenterPiece.ToolTip = new NamedToolTipInfo("#airlock_waiting_for_hero_tt");
					}
					GUIImage gUIImage = timerFrontFill;
					bool isVisible = slotState == SHSAirlockEventManager.AirlockSlot.PlayerSlotState.Choosing;
					timerBackFill.IsVisible = isVisible;
					gUIImage.IsVisible = isVisible;
				}
			}

			private PlayerPortrait playerPortrait;

			private AirlockWindow mainWindow;

			private string selectedHero = string.Empty;

			private int powerIndexSelected;

			private bool isPowerSelected;

			private bool isHost;

			private Dictionary<string, PowerAttackData> characterR2Info
			{
				get
				{
					return mainWindow.baseInfo.characterR2Info;
				}
			}

			private UserProfile profile
			{
				get
				{
					return mainWindow.profile;
				}
			}

			public string SelectedHero
			{
				get
				{
					return selectedHero;
				}
				set
				{
					selectedHero = value;
				}
			}

			public int PowerIndexSelected
			{
				get
				{
					return powerIndexSelected;
				}
				set
				{
					powerIndexSelected = value;
				}
			}

			public bool IsPowerSelected
			{
				get
				{
					return isPowerSelected;
				}
				set
				{
					isPowerSelected = value;
				}
			}

			public bool IsHost
			{
				get
				{
					return isHost;
				}
				set
				{
					isHost = value;
				}
			}

			public GUIButton PowerIconButton
			{
				get
				{
					return playerPortrait.PowerIconButton;
				}
			}

			public CharacterInfo(Player p, AirlockWindow mainWindow)
			{
				this.mainWindow = mainWindow;
				playerPortrait = new PlayerPortrait(p, this.mainWindow.NameLoader, this);
				playerPortrait.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, Vector2.zero);
				Add(playerPortrait);
				Vector2 size = playerPortrait.Size;
				float x = size.x;
				Vector2 size2 = playerPortrait.Size;
				SetSize(new Vector2(x, size2.y));
			}

			public void ExternalPowerSelected(int i)
			{
				isPowerSelected = true;
				powerIndexSelected = i;
				playerPortrait.SetPowerIconType(i);
			}

			public void ExternalStateChange(SHSAirlockEventManager.AirlockSlot.PlayerSlotState state)
			{
				playerPortrait.SetPortraitState(state);
			}

			public void ExternalHeroSelected(string heroName)
			{
				selectedHero = heroName;
				playerPortrait.SetHeroName(heroName);
			}

			public void ExternalPlayerNameSet(string name)
			{
				playerPortrait.SetSquadName(name);
			}

			public void HeroSelected(string heroName)
			{
				selectedHero = heroName;
				playerPortrait.SetHeroName(heroName);
			}

			public void UnpickHero()
			{
			}

			public void UnselectAll()
			{
			}

			public void FixUpPowerDisplay(string heroName)
			{
			}

			public void RestartAll()
			{
				selectedHero = string.Empty;
				powerIndexSelected = 0;
				isPowerSelected = false;
				UnselectAll();
			}
		}

		private const float playerOneScalar = 1f;

		private const float otherPlayerScalar = 0.8f;

		private const float missionForegroundRot = -5.5f;

		public const float inactivePortraitAlpha = 0.6f;

		private const float missionBgHeight = 428f;

		private const float missionBgWidth = 326f;

		private const float missionForegroundWidth = 251f;

		private const float missionForegroundHeight = 325f;

		private const float readyButtonWidth = 512f;

		private const float readyButtonHeight = 128f;

		private const float goButtonWidth = 512f;

		private const float goButtonHeight = 512f;

		private const float timerWidth = 128f;

		private const float timerHeight = 128f;

		private const float playerPortraitHBeginOffset = 90f;

		private const float playerPortraitVBeginOffset = 20f;

		private const float missionBgHOffset = 56f;

		private const float missionBgVOffset = 10f;

		private const float missionForegroundHOffset = 96f;

		private const float missionForegroundVOffset = 25f;

		private const float readyButtonVOffset = 15f;

		private const float readyButtonHOffset = 2f;

		private const float goButtonVOffset = 15f;

		private const float goButtonHOffset = 2f;

		private const float timerHOffset = 55f;

		private const float timerVOffset = 100f;

		private const float changeHeroButtonSize = 54f;

		private SHSBrawlerAirlockGadget baseInfo;

		private SHSBrawlerGadget.BrawlerRAttacksNameLoader nameLoader = new SHSBrawlerGadget.BrawlerRAttacksNameLoader();

		private bool isLocalPlayerHost;

		private bool initialCharacterLock;

		private GUIImage background;

		private GUIImage foreground;

		private GUIImage missionBackground;

		private GUIImage missionForeground;

		private GUIImage timerBackground;

		private GUIImage timerCenter;

		private GUIImage timerFill;

		private GUIImage timerHighlight;

		private GUIImage hostImage;

		private GUILabel timerText;

		public GUIButton PlayNow;

		private GUIButton changeHeroButton;

		private GUIButton backButton;

		private GUIButton quitButton;

		private GUIButton readyOrGoButton;

		private GUISimpleControlWindow _abilityIconHolder;

		private List<GUIImageWithEvents> _abilityIcons = new List<GUIImageWithEvents>();

		private Hashtable otherPlayerReadyImages = new Hashtable();

		private Hashtable hostImageLocations = new Hashtable();

		private Hashtable lockedHeroesRef;

		public CharacterInfo p1Info;

		private CharacterInfo p2Info;

		private CharacterInfo p3Info;

		private CharacterInfo p4Info;

		private CharacterInfo[] pInfoArray;

		private string airlockMissionKey = string.Empty;

		public SHSBrawlerAirlockGadget BaseInfo
		{
			get
			{
				return baseInfo;
			}
		}

		private UserProfile profile
		{
			get
			{
				return baseInfo.profile;
			}
		}

		public SHSBrawlerGadget.BrawlerRAttacksNameLoader NameLoader
		{
			get
			{
				return nameLoader;
			}
			set
			{
				nameLoader = value;
			}
		}

		public bool IsLocalPlayerHost
		{
			get
			{
				return isLocalPlayerHost;
			}
		}

		public bool InitialCharacterLock
		{
			get
			{
				return initialCharacterLock;
			}
			set
			{
				initialCharacterLock = value;
			}
		}

		public GUIButton ReadyOrGoButton
		{
			get
			{
				return readyOrGoButton;
			}
		}

		public GUIImage TimerFill
		{
			get
			{
				return timerFill;
			}
		}

		public GUILabel TimerText
		{
			get
			{
				return timerText;
			}
		}

		public Hashtable LockedHeroesRef
		{
			get
			{
				return lockedHeroesRef;
			}
			set
			{
				lockedHeroesRef = value;
			}
		}

		public CharacterInfo[] PInfoArray
		{
			get
			{
				return pInfoArray;
			}
		}

		public string AirlockMissionKey
		{
			get
			{
				return airlockMissionKey;
			}
		}

		public AirlockWindow(SHSBrawlerAirlockGadget baseInfo)
		{
			this.baseInfo = baseInfo;
			Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			airlockMissionKey = ((ActiveMission)AppShell.Instance.SharedHashTable["ActiveMission"]).Id;
			CspUtils.DebugLog("AirlockWindow got mission key " + airlockMissionKey);
			background = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(938f, 636f), Vector2.zero);
			background.TextureSource = "persistent_bundle|gadget_mainwindow_frame";
			foreground = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(938f, 636f), Vector2.zero);
			foreground.TextureSource = "brawlergadget_bundle|brawler_airlock_mainwindow_backdrop";
			missionBackground = new GUIImage();
			missionBackground.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(56f, 10f), new Vector2(326f, 428f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			missionBackground.TextureSource = "brawlergadget_bundle|L_brawler_airlock_mission_backdrop";
			missionBackground.HitTestType = HitTestTypeEnum.Rect;
			missionBackground.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
			missionBackground.ToolTip = new NamedToolTipInfo("#airlock_mission_tt");
			missionForeground = new GUIImage();
			missionForeground.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(96f, 25f), new Vector2(251f, 325f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			missionForeground.TextureSource = "missions_bundle|L_mshs_gameworld_" + airlockMissionKey;
			missionForeground.Rotation = -5.5f;
			_abilityIconHolder = new GUISimpleControlWindow();
			_abilityIconHolder.SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, missionForeground.Position + new Vector2(35f, 0f));
			_abilityIconHolder.SetSize(new Vector2(50f, 160f));
			_abilityIconHolder.IsVisible = true;
			_abilityIconHolder.Id = "keywordHolder";
			OwnableDefinition missionDef = OwnableDefinition.getMissionDef(airlockMissionKey);
			_abilityIconHolder.RemoveAllControls();
			foreach (GUIImageWithEvents abilityIcon in _abilityIcons)
			{
				abilityIcon.Dispose();
			}
			_abilityIcons.Clear();
			int num = 0;
			foreach (Keyword keyword in missionDef.getKeywords())
			{
				if (keyword.icon.Length > 0)
				{
					GUIImageWithEvents gUIImageWithEvents = GUIControl.CreateControlTopLeftFrame<GUIImageWithEvents>(new Vector2(48f, 48f), new Vector2(num * 5, num * 50));
					gUIImageWithEvents.TextureSource = keyword.icon;
					_abilityIconHolder.Add(gUIImageWithEvents);
					_abilityIcons.Add(gUIImageWithEvents);
					gUIImageWithEvents.Id = keyword.keyword;
					gUIImageWithEvents.IsVisible = true;
					gUIImageWithEvents.ToolTip = new NamedToolTipInfo(keyword.tooltip, new Vector2(20f, 0f));
					num++;
				}
			}
			backButton = GUIControl.CreateControlBottomFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(-225f, -60f));
			backButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_brawler_airlock_backbutton");
			if (AppShell.Instance.SharedHashTable["BrawlerAirlockReturnsToGadget"] != null && (bool)AppShell.Instance.SharedHashTable["BrawlerAirlockReturnsToGadget"])
			{
				backButton.Click += delegate
				{
					baseInfo.GoBack();
				};
			}
			else
			{
				backButton.Click += delegate
				{
					baseInfo.QuitGame(false);
				};
			}
			backButton.HitTestType = HitTestTypeEnum.Rect;
			backButton.HitTestSize = new Vector2(0.5f, 0.5f);
			backButton.ToolTip = new NamedToolTipInfo("#airlock_back_tt", new Vector2(-75f, 0f));
			quitButton = GUIControl.CreateControlBottomFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(220f, -60f));
			quitButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_brawler_airlock_quitbutton");
			quitButton.ToolTip = new NamedToolTipInfo("#airlock_quit_tt", new Vector2(100f, 0f));
			quitButton.Click += delegate
			{
				baseInfo.QuitGame(false);
			};
			quitButton.HitTestType = HitTestTypeEnum.Rect;
			quitButton.HitTestSize = new Vector2(0.5f, 0.5f);
			p1Info = new CharacterInfo(Player.p1, this);
			CharacterInfo characterInfo = p1Info;
			Vector2 size = p1Info.Size;
			float x = (0f - size.x) * 0.625f - 90f;
			Vector2 size2 = p1Info.Size;
			characterInfo.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(x, (0f - size2.y) * 0.6f + 20f));
			p2Info = new CharacterInfo(Player.p2, this);
			p2Info.SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
			p2Info.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(-90f, 20f));
			p2Info.Alpha = 0.6f;
			p3Info = new CharacterInfo(Player.p3, this);
			p3Info.SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
			CharacterInfo characterInfo2 = p3Info;
			Vector2 size3 = p3Info.Size;
			float x2 = (0f - size3.x) * 0.9f - 90f;
			Vector2 size4 = p3Info.Size;
			characterInfo2.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(x2, size4.y * 0.5f + 20f));
			p3Info.Alpha = 0.6f;
			p4Info = new CharacterInfo(Player.p4, this);
			p4Info.SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
			CharacterInfo characterInfo3 = p4Info;
			Vector2 size5 = p4Info.Size;
			characterInfo3.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2((0f - size5.x) * 0.9f * 2f - 90f, 20f));
			p4Info.Alpha = 0.6f;
			GUIImage gUIImage = null;
			string textureSource = "brawlergadget_bundle|L_brawler_airlock_label_ready";
			otherPlayerReadyImages[Player.p2] = new GUIImage();
			otherPlayerReadyImages[Player.p3] = new GUIImage();
			otherPlayerReadyImages[Player.p4] = new GUIImage();
			hostImageLocations[Player.p1] = new Vector2(137f, 8f);
			hostImageLocations[Player.p2] = new Vector2(290f, 105f);
			hostImageLocations[Player.p3] = new Vector2(135f, 185f);
			hostImageLocations[Player.p4] = new Vector2(-20f, 105f);
			gUIImage = (GUIImage)otherPlayerReadyImages[Player.p2];
			GUIImage gUIImage2 = gUIImage;
			Vector2 offset = p2Info.Offset;
			float x3 = offset.x;
			Vector2 size6 = p2Info.Size;
			float x4 = x3 - size6.x * 0.25f;
			Vector2 offset2 = p2Info.Offset;
			float y = offset2.y;
			Vector2 size7 = p2Info.Size;
			gUIImage2.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(x4, y + size7.y * 0.6f));
			gUIImage.SetSize(new Vector2(82f, 32f));
			gUIImage.TextureSource = textureSource;
			gUIImage.IsVisible = false;
			gUIImage = (GUIImage)otherPlayerReadyImages[Player.p3];
			GUIImage gUIImage3 = gUIImage;
			Vector2 offset3 = p3Info.Offset;
			float x5 = offset3.x;
			Vector2 size8 = p3Info.Size;
			float x6 = x5 - size8.x * 0.25f;
			Vector2 offset4 = p3Info.Offset;
			float y2 = offset4.y;
			Vector2 size9 = p3Info.Size;
			gUIImage3.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(x6, y2 + size9.y * 0.6f));
			gUIImage.SetSize(new Vector2(82f, 32f));
			gUIImage.TextureSource = textureSource;
			gUIImage.IsVisible = false;
			gUIImage = (GUIImage)otherPlayerReadyImages[Player.p4];
			GUIImage gUIImage4 = gUIImage;
			Vector2 offset5 = p4Info.Offset;
			float x7 = offset5.x;
			Vector2 size10 = p4Info.Size;
			float x8 = x7 - size10.x * 0.25f;
			Vector2 offset6 = p4Info.Offset;
			float y3 = offset6.y;
			Vector2 size11 = p4Info.Size;
			gUIImage4.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(x8, y3 + size11.y * 0.6f));
			gUIImage.SetSize(new Vector2(82f, 32f));
			gUIImage.TextureSource = textureSource;
			gUIImage.IsVisible = false;
			hostImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(102f, 30f), Vector2.zero);
			hostImage.TextureSource = "brawlergadget_bundle|L_brawler_airlock_label_host";
			changeHeroButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(54f, 54f), Vector2.zero);
			GUIButton gUIButton = changeHeroButton;
			Vector2 offset7 = p1Info.Offset;
			float x9 = offset7.x;
			Vector2 size12 = p1Info.Size;
			float x10 = x9 - size12.x * 0.62f;
			Vector2 offset8 = p1Info.Offset;
			float y4 = offset8.y;
			Vector2 size13 = p1Info.Size;
			gUIButton.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(x10, y4 - size13.y * 0.34f));
			changeHeroButton.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|mshs_gameworld_HUD_changehero");
			changeHeroButton.ToolTip = new NamedToolTipInfo("#airlock_pick_a_hero_tt");
			changeHeroButton.Click += delegate
			{
				baseInfo.TransitionToChooseCharacter();
			};
			int gameUserId = AppShell.Instance.ServerConnection.GetGameUserId();
			int gameHostId = AppShell.Instance.ServerConnection.GetGameHostId();
			isLocalPlayerHost = (gameUserId == gameHostId);
			readyOrGoButton = GUIControl.CreateControlAbsolute<GUIButton>(Vector2.zero, Vector2.zero);
			readyOrGoButton.SetPosition(Vector2.zero, DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(-2f, 185f));
			readyOrGoButton.SetSize(new Vector2(512f, 512f));
			readyOrGoButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_brawler_airlock_startbutton");
			readyOrGoButton.ToolTip = new NamedToolTipInfo("#airlock_startbutton_tt", new Vector2(-200f, 0f));
			readyOrGoButton.HitTestType = HitTestTypeEnum.Rect;
			readyOrGoButton.HitTestSize = new Vector2(0.8f, 0.2f);
			readyOrGoButton.Click += delegate
			{
				if (!LockedHeroesRef.ContainsKey(p1Info.SelectedHero))
				{
					changeHeroButton.IsEnabled = false;
					readyOrGoButton.IsEnabled = false;
					p1Info.PowerIconButton.IsEnabled = false;
					AppShell.Instance.ServerConnection.SetUserVariable("ready", true);
					BrawlerController.cspReady = true;  // CSP - added for testing
					baseInfo.StartGame();
				}
				else
				{
					GUIDialogNotificationSink notificationSink = new GUIDialogNotificationSink(delegate
					{
						baseInfo.MakeHeroInfoWindowBlank();
						baseInfo.TransitionToChooseCharacter();
					});
					SHSErrorNotificationWindow sHSErrorNotificationWindow = new SHSErrorNotificationWindow(SHSErrorNotificationWindow.ErrorIcons.MissionNoGo);
					sHSErrorNotificationWindow.TitleText = "#error_oops_title";
					sHSErrorNotificationWindow.Text = "#airlock_pick_another_hero_message";
					sHSErrorNotificationWindow.AllowTimeout = false;
					sHSErrorNotificationWindow.NotificationSink = notificationSink;
					GUIManager.Instance.ShowDynamicWindow(sHSErrorNotificationWindow, "Notification Root", ModalLevelEnum.Full);
				}
			};
			timerBackground = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(128f, 128f), Vector2.zero);
			timerBackground.SetPosition(Vector2.zero, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, new Vector2(-55f, 100f));
			timerBackground.TextureSource = "brawlergadget_bundle|timer_base";
			timerCenter = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(128f, 128f), Vector2.zero);
			timerCenter.SetPosition(Vector2.zero, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, new Vector2(-55f, 100f));
			timerCenter.TextureSource = "brawlergadget_bundle|timer_center";
			timerFill = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(128f, 128f), Vector2.zero);
			timerFill.SetPosition(Vector2.zero, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, new Vector2(-55f, 100f));
			timerFill.TextureSource = "brawlergadget_bundle|timer_fill_green";
			timerText = new GUILabel();
			timerText.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 38, GUILabel.GenColor(58, 71, 94), TextAnchor.MiddleCenter);
			timerText.SetPositionAndSize(Vector2.zero, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, new Vector2(-55f, 105f), new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			timerText.Text = 15f.ToString();
			timerHighlight = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(128f, 128f), Vector2.zero);
			timerHighlight.SetPosition(Vector2.zero, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, new Vector2(-55f, 100f));
			timerHighlight.TextureSource = "brawlergadget_bundle|timer_highlight";
			PlayNow = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(100f, 100f), new Vector2(0f, 0f));
			PlayNow.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_brawler_gadget_playnow");
			PlayNow.HitTestType = HitTestTypeEnum.Circular;
			PlayNow.HitTestSize = new Vector2(0.8f, 0.8f);
			PlayNow.IsVisible = false;
			PlayNow.Click += delegate
			{
			};
			Add(background);
			Add(foreground);
			Add(missionBackground);
			Add(missionForeground);
			Add(_abilityIconHolder);
			Add(p4Info);
			Add(p3Info);
			Add(p2Info);
			Add(p1Info);
			Add(changeHeroButton);
			Add(readyOrGoButton);
			Add(backButton);
			Add(quitButton);
			Add(hostImage);
			Add(timerBackground);
			Add(timerFill);
			Add(timerCenter);
			Add(timerText);
			Add(timerHighlight);
			Add((GUIImage)otherPlayerReadyImages[Player.p2]);
			Add((GUIImage)otherPlayerReadyImages[Player.p3]);
			Add((GUIImage)otherPlayerReadyImages[Player.p4]);
			pInfoArray = new CharacterInfo[4];
			pInfoArray[0] = p1Info;
			pInfoArray[1] = p2Info;
			pInfoArray[2] = p3Info;
			pInfoArray[3] = p4Info;
		}

		public void addBannedHeroes(List<string> banList)
		{
			int num = 0;
			int num2 = 280;
			List<string> list = banList;
			if (list.Contains("non_villains"))
			{
				list = new List<string>();
				list.Add("non_villains");
			}
			else if (list.Contains("non_heroes"))
			{
				list = new List<string>();
				list.Add("non_heroes");
			}
			foreach (string item in list)
			{
				CspUtils.DebugLog("adding banned head for " + item);
				Vector2 size = new Vector2(103f, 103f);
				Vector2 position = missionForeground.Position;
				float x = position.x + (float)num;
				Vector2 position2 = missionForeground.Position;
				HeroHead heroHead = GUIControl.CreateControlAbsolute<HeroHead>(size, new Vector2(x, position2.y + (float)num2));
				heroHead.StyleInfo = new SHSButtonStyleInfo("characters_bundle|inventory_character_" + item, SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
				heroHead.Rotation = -5.5f;
				Add(heroHead);
				string name = "#TOOLTIP_HERO_BANNED";
				if (item == "non_villains")
				{
					name = "#TOOLTIP_ALL_HEROES_BANNED";
				}
				else if (item == "non_heroes")
				{
					name = "#TOOLTIP_ALL_VILLAINS_BANNED";
				}
				heroHead.ToolTip = new NamedToolTipInfo(name, new Vector2(-100f, 0f));
				num += 70;
				num2 -= 10;
			}
		}

		public void SetConfirmButtonSizeAndOffset(bool isClientHost)
		{
			readyOrGoButton.SetPosition(Vector2.zero, DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2((!isClientHost) ? (-2f) : (-2f), (!isClientHost) ? (-15f) : 185f));
			readyOrGoButton.SetSize(new Vector2((!isClientHost) ? 512f : 512f, (!isClientHost) ? 128f : 512f));
			readyOrGoButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|" + ((!isClientHost) ? "L_brawler_airlock_readybutton" : "L_brawler_airlock_startbutton"));
		}

		public void SetConfirmButtonToolTip(bool isClientHost)
		{
			readyOrGoButton.ToolTip = new NamedToolTipInfo((!isClientHost) ? "Click here to say, that you're ready for the mission!" : "Click here to start the mission!", new Vector2(-200f, 0f));
		}

		public void SetConfirmButtonHitTestSize(bool isClientHost)
		{
			readyOrGoButton.HitTestSize = new Vector2(isClientHost ? 0.8f : 0.72f, isClientHost ? 0.2f : 0.95f);
		}

		public void SetVisualHostStatus(Player playerToUse)
		{
			Vector2 offset = (Vector2)hostImageLocations[playerToUse];
			hostImage.Offset = offset;
			hostImage.IsVisible = true;
		}

		public void EnabledReadyOrGoButton()
		{
			readyOrGoButton.IsEnabled = true;
		}

		public void CheckIfReady()
		{
			if (p1Info != null && PlayNow != null)
			{
				PlayNow.IsVisible = p1Info.IsPowerSelected;
			}
		}

		public void RestartAll()
		{
			PlayNow.IsVisible = false;
			PlayNow.IsEnabled = true;
			p1Info.RestartAll();
			p2Info.RestartAll();
			p3Info.RestartAll();
			p4Info.RestartAll();
		}

		public void SetPortrait(Player player, string heroName)
		{
			CharacterInfo playerSlot = GetPlayerSlot(player);
			if (playerSlot != null)
			{
				string selectedHero = playerSlot.SelectedHero;
				playerSlot.ExternalHeroSelected(heroName);
				OnHeroSelectedChange(selectedHero, heroName);
			}
		}

		public List<string> GetHeroNameList()
		{
			List<string> list = new List<string>();
			CharacterInfo[] array = pInfoArray;
			foreach (CharacterInfo characterInfo in array)
			{
				if (characterInfo != null && characterInfo.SelectedHero != string.Empty)
				{
					list.Add(characterInfo.SelectedHero);
				}
			}
			return list;
		}

		public void OnHeroSelectedChange(string oldHero, string newHero)
		{
			if (oldHero != newHero)
			{
				baseInfo.OnNewCharacterChoice();
			}
		}

		public void SetName(Player player, string playerName)
		{
			CharacterInfo playerSlot = GetPlayerSlot(player);
			if (playerSlot != null)
			{
				playerSlot.ExternalPlayerNameSet(playerName);
			}
		}

		public void UnselectAll(Player player)
		{
			CharacterInfo playerSlot = GetPlayerSlot(player);
			if (playerSlot != null)
			{
				playerSlot.UnselectAll();
			}
		}

		public void UnchooseP1Hero()
		{
		}

		public void SetPower(Player player, int powerNumber)
		{
			CharacterInfo playerSlot = GetPlayerSlot(player);
			if (playerSlot != null)
			{
				playerSlot.ExternalPowerSelected(powerNumber);
			}
		}

		public void SetIsHost(Player player, bool isHost)
		{
			CharacterInfo playerSlot = GetPlayerSlot(player);
			if (playerSlot != null)
			{
				playerSlot.IsHost = isHost;
			}
		}

		public void SetState(Player player, SHSAirlockEventManager.AirlockSlot.PlayerSlotState slotState)
		{
			CharacterInfo playerSlot = GetPlayerSlot(player);
			if (playerSlot != null)
			{
				playerSlot.ExternalStateChange(slotState);
				if (player != 0 && !playerSlot.IsHost)
				{
					((GUIImage)otherPlayerReadyImages[player]).IsVisible = ((slotState == SHSAirlockEventManager.AirlockSlot.PlayerSlotState.Ready) ? true : false);
				}
				if (playerSlot.IsHost && slotState == SHSAirlockEventManager.AirlockSlot.PlayerSlotState.Empty)
				{
					hostImage.IsVisible = false;
				}
			}
		}

		private CharacterInfo GetPlayerSlot(Player player)
		{
			switch (player)
			{
			case Player.p1:
				return p1Info;
			case Player.p2:
				return p2Info;
			case Player.p3:
				return p3Info;
			case Player.p4:
				return p4Info;
			default:
				return null;
			}
		}

		public void HeroClicked(string heroName)
		{
			string selectedHero = p1Info.SelectedHero;
			p1Info.HeroSelected(heroName);
			if (selectedHero != string.Empty)
			{
				OnHeroSelectedChange(selectedHero, heroName);
			}
		}

		public void FixUpPowerDisplay(string heroName)
		{
			p1Info.FixUpPowerDisplay(heroName);
		}
	}

	public class HeroHead : GUIButton
	{
		private HeroPersisted heroData;

		[CompilerGenerated]
		private bool _003CForcedHover_003Ek__BackingField;

		public bool ForcedHover
		{
			[CompilerGenerated]
			get
			{
				return _003CForcedHover_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CForcedHover_003Ek__BackingField = value;
			}
		}

		public override bool Hover
		{
			get
			{
				return base.Hover || ForcedHover;
			}
		}

		public HeroPersisted HeroData
		{
			get
			{
				return heroData;
			}
			set
			{
				heroData = value;
			}
		}
	}

	protected const float autoChooseTime = 15f;

	private SHSCharacterSelect characterSelect;

	private AirlockWindow airlock;

	private AirlockTitleWindow airlockTitle;

	private AirlockWindow.AirlockChooseAHeroWindow chooseAHeroWindow;

	private SHSCharacterCombinationWindow characterCombination;

	private Dictionary<string, PowerAttackData> characterR2Info;

	public AirlockSlotManager airlockSlotManager;

	private UserProfile profile;

	private bool CharacterSucessfullySelected;

	private bool IgnoreCloseGadget;

	private bool hostIsWaiting;

	private AutoSelectTimer forceTimer;

	private bool attemptingSquadNameFetch;

	public string p1SelectedHero
	{
		get
		{
			return airlock.p1Info.SelectedHero;
		}
		set
		{
			airlock.p1Info.SelectedHero = value;
		}
	}

	private int p1PowerIndexSelected
	{
		get
		{
			return airlock.p1Info.PowerIndexSelected;
		}
	}

	private bool p1IsPowerSelected
	{
		get
		{
			return airlock.p1Info.IsPowerSelected;
		}
	}

	public bool HostIsWaiting
	{
		get
		{
			return hostIsWaiting;
		}
	}

	public AutoSelectTimer ForceTimer
	{
		get
		{
			return forceTimer;
		}
	}

	public bool AttemptingSquadNameFetch
	{
		get
		{
			return attemptingSquadNameFetch;
		}
	}

	public SHSBrawlerAirlockGadget()
	{
		profile = AppShell.Instance.Profile;
		characterSelect = new SHSCharacterSelect(profile, HeroClicked);
		airlock = new AirlockWindow(this);
		forceTimer = new AutoSelectTimer(airlock.TimerFill, airlock.TimerText);
		forceTimer.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, Vector2.zero);
		forceTimer.OnTimerEvent += OnTimerEvent;
		Add(forceTimer);
		chooseAHeroWindow = new AirlockWindow.AirlockChooseAHeroWindow(airlock);
		airlockTitle = new AirlockTitleWindow();
		characterCombination = new SHSCharacterCombinationWindow();
		base.AnimationOpenFinished += OnNewCharacterChoice;
		SetupAndLoadR2InfoDictionary();
		airlockSlotManager = new AirlockSlotManager(profile, characterR2Info, airlock, OnNewCharacterChoice);
		airlockSlotManager.lockHeroesBecauseOfMission(((ActiveMission)AppShell.Instance.SharedHashTable["ActiveMission"]).Id);
		airlock.LockedHeroesRef = airlockSlotManager.LockedHeroes;
		SetupOpeningWindow(BackgroundType.OnePanel, airlock);
		AppShell.Instance.TransitionHandler.CurrentWaitWindow.Alpha = 0f;
		CloseButton.Click -= CloseDelegate;
		CloseButton.Click += delegate
		{
			QuitGame(false);
		};
	}

	public void MakeHeroInfoWindowBlank()
	{
		chooseAHeroWindow.MakeHeroInfoWindowBlank();
	}

	public void GoBack()
	{
		GUIDialogNotificationSink notificationSink = new GUIDialogNotificationSink(delegate(string Id, DialogState state)
		{
			if (state == DialogState.Ok)
			{
				GUIDialogNotificationSink notifySink = new GUIDialogNotificationSink(delegate(string missionWindowId, IGUIDialogWindow window)
				{
					GameController.ControllerType controllerType = AppShell.Instance.PreviousControllerType;
					if (controllerType == GameController.ControllerType.CardGame)
					{
						controllerType = GameController.ControllerType.SocialSpace;
					}
					if (controllerType == GameController.ControllerType.Brawler)
					{
						controllerType = GameController.ControllerType.SocialSpace;
					}
					AppShell.Instance.ServerConnection.DisconnectFromGame();
					((SHSBrawlerGadget)window).SetBrawlerCloseData(new CustomCloseData(controllerType));
					Hide();
				}, null, null, null, null);
				GUIManager.Instance.ShowDialog(typeof(SHSBrawlerGadget), string.Empty, "SHSBrawlerMainWindow", notifySink, ModalLevelEnum.Default);
			}
		});
		SHSErrorNotificationWindow sHSErrorNotificationWindow = new SHSErrorNotificationWindow(SHSErrorNotificationWindow.ErrorIcons.MissionNoGo, SHSErrorNotificationWindow.ErrorChoiceType.OkCancel);
		sHSErrorNotificationWindow.TitleText = "#airlock_quit_message_title";
		sHSErrorNotificationWindow.Text = "#airlock_leave_message";
		sHSErrorNotificationWindow.AllowTimeout = false;
		sHSErrorNotificationWindow.NotificationSink = notificationSink;
		GUIManager.Instance.ShowDynamicWindow(sHSErrorNotificationWindow, "Notification Root", ModalLevelEnum.Full);
	}

	private void transitionOutOfAirlock()
	{
		GameController.ControllerType controllerType = AppShell.Instance.PreviousControllerType;
		if (controllerType == GameController.ControllerType.CardGame)
		{
			controllerType = GameController.ControllerType.SocialSpace;
		}
		if (controllerType == GameController.ControllerType.Brawler)
		{
			controllerType = GameController.ControllerType.SocialSpace;
		}
		AppShell.Instance.TransitionHandler.AbortTransition("brawler_start_transaction");
		AppShell.Instance.Transition(controllerType);
	}

	public void QuitGame(bool forceClose = false)
	{
		if (forceClose)
		{
			transitionOutOfAirlock();
			return;
		}
		GUIDialogNotificationSink notificationSink = new GUIDialogNotificationSink(delegate(string Id, DialogState state)
		{
			if (state == DialogState.Ok)
			{
				transitionOutOfAirlock();
			}
		});
		SHSErrorNotificationWindow sHSErrorNotificationWindow = new SHSErrorNotificationWindow(SHSErrorNotificationWindow.ErrorIcons.MissionNoGo, SHSErrorNotificationWindow.ErrorChoiceType.OkCancel);
		sHSErrorNotificationWindow.TitleText = "#airlock_quit_message_title";
		sHSErrorNotificationWindow.Text = "#airlock_leave_message";
		sHSErrorNotificationWindow.AllowTimeout = false;
		sHSErrorNotificationWindow.NotificationSink = notificationSink;
		GUIManager.Instance.ShowDynamicWindow(sHSErrorNotificationWindow, "Notification Root", ModalLevelEnum.Full);
	}

	public void FetchSquadNames(List<SHSAirlockEventManager.AirlockSlot> listToUpdate)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		attemptingSquadNameFetch = true;
		AnimClip animClip = AnimClipBuilder.Absolute.Nothing(AnimClipBuilder.Path.Linear(0f, 0f, 1f), airlock);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			bool flag = true;
			List<NetworkManager.UserInfo> gameAllUsers = AppShell.Instance.ServerConnection.GetGameAllUsers();
			foreach (NetworkManager.UserInfo info in gameAllUsers)
			{
				SHSAirlockEventManager.AirlockSlot airlockSlot = listToUpdate.Find(delegate(SHSAirlockEventManager.AirlockSlot c)
				{
					return c.UserId == info.userId;
				});
				if (airlockSlot.State != 0)
				{
					airlockSlot.UserName = info.userName;
					if (airlockSlot.UserName == "<unknown>")
					{
						airlockSlot.UserName = string.Empty;
						flag = false;
						FetchSquadNames(listToUpdate);
					}
				}
				attemptingSquadNameFetch = !flag;
			}
		};
		base.AnimationPieceManager.Add(animClip);
	}

	public void ReappointNewHost(int oldHostId, string oldHostName)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		AnimClip animClip = AnimClipBuilder.Absolute.Nothing(AnimClipBuilder.Path.Linear(0f, 0f, 0.1f), airlock);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			int gameHostId = AppShell.Instance.ServerConnection.GetGameHostId();
			int gameUserId = AppShell.Instance.ServerConnection.GetGameUserId();
			if (oldHostId != gameHostId)
			{
				bool flag = gameHostId == gameUserId;
				if (flag)
				{
					airlock.SetConfirmButtonSizeAndOffset(true);
					airlock.SetConfirmButtonToolTip(true);
					airlock.SetConfirmButtonHitTestSize(true);
				}
				string text = "Hey Hero! The original host left the mission. " + airlockSlotManager.GetAirlockSlot(gameHostId).UserName + " is now the host.";
				airlock.PInfoArray[(int)airlockSlotManager.FindPlayer(oldHostId)].IsHost = false;
				airlock.PInfoArray[(int)airlockSlotManager.FindPlayer(gameHostId)].IsHost = true;
				airlock.SetVisualHostStatus(airlockSlotManager.FindPlayer(gameHostId));
				SHSErrorNotificationWindow sHSErrorNotificationWindow = new SHSErrorNotificationWindow(SHSErrorNotificationWindow.ErrorIcons.MissionNoGo);
				sHSErrorNotificationWindow.TitleText = "#error_oops_title";
				sHSErrorNotificationWindow.Text = ((!flag) ? text : "#airlock_host_leaves_client_host");
				sHSErrorNotificationWindow.AllowTimeout = false;
				GUIManager.Instance.ShowDynamicWindow(sHSErrorNotificationWindow, "Notification Root", ModalLevelEnum.Full);
			}
		};
		base.AnimationPieceManager.Add(animClip);
	}

	public override void OnShow()
	{
		base.OnShow();
		RestartAll();
		airlockSlotManager.OnShow();
		AppShell.Instance.EventMgr.AddListener<CharacterSelectedMessage>(OnCharSelected);
		AppShell.Instance.EventMgr.AddListener<OwnershipStringMessage>(OnOwnershipChange);
		AppShell.Instance.EventMgr.AddListener<BrawlerAutoChooseStartMessage>(OnAutoChooseStart);
		AppShell.Instance.EventMgr.AddListener<AirlockTimerMessage>(OnLevelStart);
		AppShell.Instance.EventMgr.AddListener<BrawlerAutoChooseMessage>(OnAutoChoose);
		if (profile != null)
		{
			HeroClicked(profile.LastSelectedCostume, profile.LastSelectedPower);
			airlock.SetName(Player.p1, profile.PlayerName);
		}
		AppShell.Instance.ServerConnection.SetUserVariable("ready", false);
	}

	private void RestartAll()
	{
		IgnoreCloseGadget = false;
		CharacterSucessfullySelected = false;
		characterSelect.EnableAllHero(true);
		airlock.RestartAll();
	}

	public override void OnHide()
	{
		airlockSlotManager.OnHide();
		AppShell.Instance.EventMgr.RemoveListener<CharacterSelectedMessage>(OnCharSelected);
		AppShell.Instance.EventMgr.RemoveListener<OwnershipStringMessage>(OnOwnershipChange);
		AppShell.Instance.EventMgr.RemoveListener<BrawlerAutoChooseStartMessage>(OnAutoChooseStart);
		AppShell.Instance.EventMgr.RemoveListener<AirlockTimerMessage>(OnLevelStart);
		AppShell.Instance.EventMgr.RemoveListener<BrawlerAutoChooseMessage>(OnAutoChoose);
		base.OnHide();
	}

	public void OnNewCharacterChoice()
	{
		AppShell.Instance.EventMgr.Fire(this, new PresetCombinationsRequestMessage(airlock.GetHeroNameList()));
	}

	private void StartGame()
	{
		if (!CharacterSucessfullySelected)
		{
			OnAutoChoose(null);
			return;
		}
		CharacterSelectionBlock characterSelectionBlock = new CharacterSelectionBlock(p1SelectedHero, p1PowerIndexSelected);
		if (!p1IsPowerSelected)
		{
			characterSelectionBlock.r2Attack = 1;
		}
		CharacterRequestedMessage msg = new CharacterRequestedMessage(characterSelectionBlock);
		AppShell.Instance.EventMgr.Fire(null, msg);
	}

	private void TransitionToChooseCharacter()
	{
		SetBackgroundImage("options_bundle|options_gadget_frame");
		airlockTitle.SetTitleTextureSource("persistent_bundle|L_title_pick_a_hero");
		airlockTitle.SetTitleSize(new Vector2(215f, 48f));
		airlockTitle.SetToolTip(new NamedToolTipInfo("#airlock_pick_a_hero_tt"));
		chooseAHeroWindow.SetTimerInterval((int)forceTimer.TimeLeft);
		SetCenterWindow(chooseAHeroWindow);
	}

	private void TransitionToAirlockWindow()
	{
		airlockTitle.SetTitleTextureSource("brawlergadget_bundle|L_title_missionlaunch");
		airlockTitle.SetToolTip(NoToolTipInfo.Instance);
		airlockTitle.SetTitleSize(new Vector2(285f, 54f));
		SetCenterWindow(airlock);
	}

	private void OnCharSelected(CharacterSelectedMessage message)
	{
		CharacterSucessfullySelected = true;
		airlock.PlayNow.IsEnabled = false;
	}

	private void OnOwnershipChange(OwnershipStringMessage message)
	{
		if (message.ownerId != -2 && message.ownerId != -3 && message.ownerId != AppShell.Instance.ServerConnection.GetGameUserId())
		{
			characterSelect.EnableHero(message.strings[0], false);
			if (airlock.p1Info.SelectedHero == message.strings[0])
			{
				airlock.UnchooseP1Hero();
			}
		}
	}

	private void OnLevelStart(AirlockTimerMessage message)
	{
		if (!IgnoreCloseGadget)
		{
			CloseGadget();
		}
	}

	private void OnPresetCombinations(PresetCombinationsSelectedMessage msg)
	{
		characterCombination.ClearCombinations();
		if (msg.comboSelectedList == null || msg.comboSelectedList.Count == 0)
		{
			SetTopWindowImmediate(airlockTitle);
			return;
		}
		SetTopWindowImmediate(characterCombination);
		foreach (BasePresetCombination comboSelected in msg.comboSelectedList)
		{
			characterCombination.AddCombination(comboSelected.DisplayName, comboSelected.DisplayIcon, comboSelected.ActiveCharacters);
		}
	}

	private void OnTimerEvent(SHSTimerEx.TimerEventType Type, int data)
	{
		if (Type == SHSTimerEx.TimerEventType.Completed)
		{
			StartGame();
		}
	}

	private void OnAutoChooseStart(BrawlerAutoChooseStartMessage message)
	{
		hostIsWaiting = true;
		forceTimer.Duration = 15f;
		forceTimer.Start();
	}

	private void OnAutoChoose(BrawlerAutoChooseMessage message)
	{
		CspUtils.DebugLog("OnAutoChoose " + p1SelectedHero + " " + airlock.p1Info.SelectedHero);
		airlockSlotManager.AirlockEventManager.AirlockState = SHSAirlockEventManager.AirlockStateEnum.AutoChoosing;
		CharacterRequestedMessage characterRequestedMessage = null;
		CspUtils.DebugLog("ban list is now ");
		foreach (string key in airlockSlotManager.LockedHeroes.Keys)
		{
			CspUtils.DebugLog("   " + key);
		}
		if (p1SelectedHero != string.Empty && !airlock.LockedHeroesRef.ContainsKey(p1SelectedHero))
		{
			CspUtils.DebugLog("Hero is not banned, proceed");
			characterRequestedMessage = new CharacterRequestedMessage(new CharacterSelectionBlock(p1SelectedHero, (!p1IsPowerSelected) ? 1 : p1PowerIndexSelected));
		}
		else
		{
			CspUtils.DebugLog("Hero is banned, autochoosing");
			if (Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldHeroesAllow))
			{
				foreach (string key2 in profile.AvailableCostumes.Keys)
				{
					if (!airlock.LockedHeroesRef.ContainsKey(key2))
					{
						characterRequestedMessage = new CharacterRequestedMessage(new CharacterSelectionBlock(key2, 1));
						break;
					}
				}
			}
			else
			{
				foreach (HeroPersisted availableRecruitCostume in profile.AvailableRecruitCostumes)
				{
					if (!airlock.LockedHeroesRef.ContainsKey(availableRecruitCostume.Name))
					{
						characterRequestedMessage = new CharacterRequestedMessage(new CharacterSelectionBlock(availableRecruitCostume.Name, 1));
						break;
					}
				}
			}
		}
		if (characterRequestedMessage == null)
		{
			CspUtils.DebugLog("No available hero to choose from, quitting the airlock");
			GUIDialogNotificationSink notificationSink = new GUIDialogNotificationSink(delegate(string Id, DialogState state)
			{
				if (state == DialogState.Ok || state == DialogState.Cancel)
				{
					transitionOutOfAirlock();
				}
			});
			SHSOkDialogWindow sHSOkDialogWindow = new SHSOkDialogWindow();
			sHSOkDialogWindow.TitleText = "#AIRLOCK_NO_POSSIBLE_HERO_TITLE";
			sHSOkDialogWindow.Text = "#AIRLOCK_NO_POSSIBLE_HERO_MESSAGE_NO_VILLAIN_AVAILABLE";
			sHSOkDialogWindow.NotificationSink = notificationSink;
			GUIManager.Instance.ShowDynamicWindow(sHSOkDialogWindow, "Notification Root", ModalLevelEnum.Full);
		}
		else
		{
			airlock.SetPortrait(Player.p1, characterRequestedMessage.CharacterName);
			airlock.SetPower(Player.p1, characterRequestedMessage.R2Attack);
			AppShell.Instance.EventMgr.Fire(null, characterRequestedMessage);
		}
	}

	private void SetupAndLoadR2InfoDictionary()
	{
		characterR2Info = new Dictionary<string, PowerAttackData>();
		if (profile != null)
		{
			foreach (KeyValuePair<string, HeroPersisted> availableCostume in profile.AvailableCostumes)
			{
				HeroPersisted value = null;
				if (!profile.AvailableCostumes.TryGetValue(availableCostume.Key, out value))
				{
					CspUtils.DebugLog("Hero <" + availableCostume.Key + "> being added to hero UI was not found in the hero collection!");
				}
				else
				{
					AppShell.Instance.DataManager.LoadGameData("Characters/" + value.Name, OnCharacterDataLoaded, value);
				}
			}
		}
	}

	protected void OnCharacterDataLoaded(GameDataLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			return;
		}
		HeroPersisted heroPersisted = extraData as HeroPersisted;
		if (heroPersisted == null)
		{
			CspUtils.DebugLog("Hero missing in callback from GameDataManager for character <" + response.Path + ">.");
			return;
		}
		string text = heroPersisted.Name.ToLowerInvariant();
		characterR2Info[text] = new PowerAttackData();
		DataWarehouse data = response.Data;
		int i = 0;
		for (int count = data.GetCount("//player_combat_controller/combat_controller/secondary_attack"); i < count; i++)
		{
			DataWarehouse dataWarehouse = data.GetData("//player_combat_controller/combat_controller/secondary_attack", i);
			int num = 0;
			int count2 = dataWarehouse.GetCount("rank");
			for (int j = 0; j < count2; j++)
			{
				DataWarehouse data2 = dataWarehouse.GetData("rank", j);
				if (data2.GetInt("number") == num)
				{
					dataWarehouse = data2;
					break;
				}
			}
			string name = dataWarehouse.TryGetString("name", "NO ATTACK NAME");
			string displayName = "#" + text + "_r" + (i + 1).ToString();
			characterR2Info[text].StoreAttack(i, name, displayName);
		}
		if (p1SelectedHero == text)
		{
			airlock.FixUpPowerDisplay(text);
		}
	}

	private void HeroClicked(string heroName)
	{
		airlock.HeroClicked(heroName);
		airlock.SetPower(Player.p1, 0);
	}

	private void HeroClicked(string heroName, int lastSelectedPower)
	{
		airlock.HeroClicked(heroName);
		airlock.SetPower(Player.p1, lastSelectedPower);
	}
}
