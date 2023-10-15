using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif

//using UnityEditor;
using System.Collections;

public class SHSBrawlerGadget : SHSGadget
{
	public class BrawlerChooseAHeroWindow : SHSCharacterSelectScanlineTransition
	{
		public class HeroInfoWindow : GUISimpleControlWindow
		{
			private const float totalFillWidth = 332f;

			private GUIDropShadowTextLabel powerAttackText;

			private GUIButton okButton;

			private GUIButton backButton;

			private BrawlerChooseAHeroWindow headWindow;

			private BrawlerPowerSelection brawlerPowers;

			public HeroInfoWindow(string heroKey, BrawlerChooseAHeroWindow headWindow)
			{
				this.headWindow = headWindow;
				GUIDropShadowTextLabel gUIDropShadowTextLabel = GenDropShadow(new Vector2(300f, 100f), new Vector2(15f, 41f), 32, 255, 248, 141);
				gUIDropShadowTextLabel.FontFace = GUIFontManager.SupportedFontEnum.Zooom;
				gUIDropShadowTextLabel.TextOffset = new Vector2(-3f, 4f);
				gUIDropShadowTextLabel.Text = AppShell.Instance.CharacterDescriptionManager[heroKey].CharacterName;
				gUIDropShadowTextLabel.VerticalKerning = 25;
				int level = AppShell.Instance.Profile.AvailableCostumes[heroKey].Level;
				GUILabel gUILabel = GenDropShadow(new Vector2(281f, 73f), new Vector2(20f, 98f), 18, 200, 255, 32);
				gUILabel.Text = XpToLevelDefinition.GetLevelText(heroKey);
				GUILabel gUILabel2 = GenDropShadow(new Vector2(281f, 73f), new Vector2(120f, 98f), 18, 255, 174, 102);
				gUILabel2.Text = XpToLevelDefinition.GetExpText(heroKey);
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
				brawlerPowers = GUIControl.CreateControlCenter<BrawlerPowerSelection>(new Vector2(599f, 531f), new Vector2(230f, 338f));
				GUIImage gUIImage2 = GUIControl.CreateControlCenter<GUIImage>(new Vector2(494f, 413f), new Vector2(240f, 220f));
				gUIImage2.TextureSource = "brawlergadget_bundle|brawler_choosehero_screen_backdrop_new";
				GUIImage gUIImage3 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(63f, 53f), new Vector2(-215f, -45f));
				gUIImage3.TextureSource = "mysquadgadget_bundle|L_mysquad_heroinfo_xp_icon";
				float num = 332f * GetPercToNextLevel(heroKey);
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
				Vector2 size6 = new Vector2(150f, 25f);
				Vector2 offset3 = gUIImage4.Offset;
				float x3 = offset3.x;
				Vector2 size7 = gUIImage4.Size;
				float a = x3 + size7.x * 0.5f + num * 0.5f;
				Vector2 offset4 = gUIImage4.Offset;
				GUILabel gUILabel4 = GUIControl.CreateControlFrameCentered<GUILabel>(size6, new Vector2(Mathf.Max(a, offset4.x + 55f), -47.5f));
				gUILabel4.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, GUILabel.GenColor(24, 80, 0), TextAnchor.MiddleCenter);
				gUILabel4.Text = XpToLevelDefinition.GetExpText(heroKey);
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
				headWindow.brawlerGadget.nameLoader.SetupTooltips(heroKey, brawlerPowers);
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
				okButton.Click += delegate
				{
					headWindow.OkWasClicked();
				};
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
				StatLevelReqsDefinition instance = StatLevelReqsDefinition.Instance;
				int num = 1;
				if (instance != null)
				{
					string lastSelectedCostume = AppShell.Instance.Profile.LastSelectedCostume;
					num = instance.GetMaxPowerAttackUnlockedAt(AppShell.Instance.Profile.AvailableCostumes[lastSelectedCostume].Level);
				}
				if (AppShell.Instance.Profile.LastSelectedPower > num)
				{
					AppShell.Instance.Profile.LastSelectedPower = num;
				}
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
				if (!IsEnabled)
				{
					brawlerPowers.PowerMove1.IsEnabled = false;
					brawlerPowers.PowerMove2.IsEnabled = false;
					brawlerPowers.PowerMove3.IsEnabled = false;
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

			public static float GetPercToNextLevel(string heroName)
			{
				int expCur;
				int expNext;
				bool max;
				XpToLevelDefinition.GetExp(out expCur, out expNext, out max, heroName);
				float num = (float)expCur * 1f / ((float)expNext * 1f);
				if (expCur == expNext || max || num > 1f || num < 0f || float.IsInfinity(num) || float.IsNaN(num))
				{
					num = 1f;
				}
				return num;
			}
		}

		private SHSBrawlerGadget brawlerGadget;

		private SHSCharacterSelect characterSelect;

		public BrawlerChooseAHeroWindow(SHSBrawlerGadget brawlerGadget)
		{
			this.brawlerGadget = brawlerGadget;
			characterSelect = GetCharacterSelect();
			CharacterSelectScanlineTransition(characterSelect, GetContentWindow, brawlerGadget.DataManager.SelectedHero, SHSScanlineTransitionWindow<string>.DefaultScanlineTime, new Vector2(520f, 430f));
			base.HeroClicked += HeroWasClicked;
		}

		public SHSCharacterSelect GetCharacterSelect()
		{
			SHSCharacterSelect sHSCharacterSelect = new SHSCharacterSelect(AppShell.Instance.Profile, null, "brawlergadget_bundle|brawler_gadget_left_bg_frame", "brawlergadget_bundle|brawler_gadget_left_bg_back");
			Vector2 characterSelectionSize = sHSCharacterSelect.CharacterSelectionSize;
			float x = characterSelectionSize.x;
			Vector2 characterSelectionSize2 = sHSCharacterSelect.CharacterSelectionSize;
			sHSCharacterSelect.SetCharacterSelectionSize(new Vector2(x, characterSelectionSize2.y - 20f));
			sHSCharacterSelect.ApplyOffsetToCharacterSelectWindow(new Vector2(0f, -10f));
			return sHSCharacterSelect;
		}

		public void UpdateCharacterSelect()
		{
			foreach (KeyValuePair<string, HeroPersisted> availableCostume in AppShell.Instance.Profile.AvailableCostumes)
			{
				if (!characterSelect.HasHero(availableCostume.Key))
				{
					characterSelect.AddHero(availableCostume.Value, null);
				}
			}
		}

		public GUISimpleControlWindow GetContentWindow(string heroName)
		{
			return new HeroInfoWindow(heroName, this);
		}

		public void HeroWasClicked(string heroName)
		{
			brawlerGadget.DataManager.SelectedHero = heroName;
		}

		public void PowerWasSelected(int powerNumber)
		{
			brawlerGadget.DataManager.SelectedPower = powerNumber;
		}

		public void OkWasClicked()
		{
			brawlerGadget.ChooseAHeroOkButtonClicked();
		}

		public void BackWasClicked()
		{
			brawlerGadget.BackWasClicked();
		}
	}

	public class BrawlerChooseAMissionWindow : GadgetCenterWindow
	{
		public class SliderAnimation : SHSAnimations
		{
			public static AnimClip CenterSlider(GUISlider slider, float CenterLocation, bool muted, Action<float> del)
			{
				float mod = (!muted) ? 1f : 0.25f;
				float time = (!muted) ? 0.3f : 0.5f;
				return Custom.Function(GenericPaths.LinearWithMutedSingleWiggle(slider.Value, CenterLocation, mod, time), del);
			}
		}

		public class FavoriteMissionCard : MissionCard
		{
			public int orderID;

			public FavoriteMissionCard(string missionId, string missionKey, SHSBrawlerGadget brawlerGadget, int orderID)
				: base(missionId, missionKey, brawlerGadget, CardType.FavoriteMission)
			{
				this.orderID = orderID;
				GUIDrawTexture gUIDrawTexture = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(MissionCard.ModSize(0.85f), Vector2.zero);
				gUIDrawTexture.TextureSource = "missions_bundle|L_mshs_gameworld_" + missionKey;
				AddItem(gUIDrawTexture);
				GUIImageWithEvents gUIImageWithEvents = GUIControl.CreateControlBottomFrameCentered<GUIImageWithEvents>(new Vector2(50f, 50f), new Vector2(0f, -50f));
				gUIImageWithEvents.TextureSource = "brawlergadget_bundle|favorite_mission_star_small";
				gUIImageWithEvents.ToolTip = new NamedToolTipInfo("#BRAWLER_FAVORITE_MISSION");
				AddItem(gUIImageWithEvents);
				gUIDrawTexture.Click += delegate
				{
					ClickedOnAMission();
				};
				InitializeRequiredContent();
			}

			public void setDaily()
			{
				GUIImage gUIImage = GUIControl.CreateControlBottomRightFrameCentered<GUIImage>(new Vector2(75f, 75f), new Vector2(-40f, -40f));
				gUIImage.TextureSource = "brawlergadget_bundle|L_daily_mission_logo";
				AddItem(gUIImage);
			}

			public override int SelfCompare(MissionCard other)
			{
				FavoriteMissionCard favoriteMissionCard = other as FavoriteMissionCard;
				if (favoriteMissionCard != null)
				{
					return orderID.CompareTo(favoriteMissionCard.orderID);
				}
				return 1;
			}
		}

		public class MissionOfTheDay : MissionCard
		{
			public MissionOfTheDay(string missionId, string missionKey, SHSBrawlerGadget brawlerGadget)
				: base(missionId, missionKey, brawlerGadget, CardType.MissionOfTheDay)
			{
				GUIDrawTexture gUIDrawTexture = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(MissionCard.ModSize(0.85f), Vector2.zero);
				gUIDrawTexture.TextureSource = "missions_bundle|L_mshs_gameworld_" + missionKey;
				AddItem(gUIDrawTexture);
				GUIImage gUIImage = GUIControl.CreateControlBottomRightFrameCentered<GUIImage>(new Vector2(75f, 75f), new Vector2(-40f, -40f));
				gUIImage.TextureSource = "brawlergadget_bundle|L_daily_mission_logo";
				AddItem(gUIImage);
				gUIDrawTexture.Click += delegate
				{
					ClickedOnAMission();
				};
				InitializeRequiredContent();
			}

			public override int SelfCompare(MissionCard other)
			{
				if (other is FavoriteMissionCard)
				{
					return -1;
				}
				MissionOfTheDay missionOfTheDay = other as MissionOfTheDay;
				if (missionOfTheDay != null)
				{
					return string.Compare(missionKey, missionOfTheDay.missionKey, true);
				}
				return string.Compare(missionKey, other.missionKey, true);
			}
		}

		public class OwnedMission : MissionCard
		{
			public OwnedMission(string missionId, string missionKey, SHSBrawlerGadget brawlerGadget)
				: base(missionId, missionKey, brawlerGadget, CardType.OwnedMission)
			{
				GUIDrawTexture gUIDrawTexture = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(MissionCard.ModSize(0.85f), Vector2.zero);
				gUIDrawTexture.TextureSource = "brawlergadget_bundle|brawler_gadget_buymission_panel";
				AddItem(gUIDrawTexture);
				GUIDrawTexture gUIDrawTexture2 = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(MissionCard.ModSize(0.85f), Vector2.zero);
				gUIDrawTexture2.TextureSource = "missions_bundle|L_mshs_gameworld_" + missionKey;
				AddItem(gUIDrawTexture2);
				gUIDrawTexture2.Click += delegate
				{
					ClickedOnAMission();
				};
				InitializeRequiredContent();
			}

			public override int SelfCompare(MissionCard other)
			{
				if (other is FavoriteMissionCard)
				{
					return -1;
				}
				if (other is MissionOfTheDay)
				{
					return -1;
				}
				OwnedMission ownedMission = other as OwnedMission;
				if (ownedMission != null)
				{
					return string.Compare(missionKey, ownedMission.missionKey, true);
				}
				return string.Compare(missionKey, other.missionKey, true);
			}
		}

		public class UnownedMission : MissionCard
		{
			public int SortingID;

			public UnownedMission(string missionId, string missionKey, SHSBrawlerGadget brawlerGadget, string ShoppingLookupName, int SortingID)
				: base(missionId, missionKey, brawlerGadget, CardType.UnownedMission)
			{
				this.SortingID = SortingID;
				GUIDrawTexture gUIDrawTexture = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(MissionCard.ModSize(1f), Vector2.zero);
				gUIDrawTexture.TextureSource = "brawlergadget_bundle|brawler_gadget_buymission_panel";
				AddItem(gUIDrawTexture);
				GUIDrawTexture gUIDrawTexture2 = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(MissionCard.ModSize(0.45f), new Vector2(77f, -74f));
				gUIDrawTexture2.Rotation = 6f;
				gUIDrawTexture2.TextureSource = "missions_bundle|L_mshs_gameworld_" + missionKey;
				AddItem(gUIDrawTexture2);
				GUIDropShadowTextLabel gUIDropShadowTextLabel = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(142f, 185f), new Vector2(-50f, -80f));
				gUIDropShadowTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 28, Color.black, TextAnchor.MiddleCenter);
				gUIDropShadowTextLabel.TextOffset = new Vector2(-2f, 2f);
				gUIDropShadowTextLabel.FrontColor = Color.white;
				gUIDropShadowTextLabel.BackColor = GUILabel.GenColor(0, 21, 105);
				gUIDropShadowTextLabel.Text = "#you_dont_own_this_mission";
				Add(gUIDropShadowTextLabel);
				GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(210f, 145f), new Vector2(6f, 87f));
				gUIButton.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|L_buyit_button");
				gUIButton.HitTestSize = new Vector2(0.87f, 0.79f);
				gUIButton.ToolTip = new NamedToolTipInfo("#buy_this_mission");
				Add(gUIButton);
				gUIButton.Click += delegate
				{
					ShoppingWindow shoppingWindow = new ShoppingWindow(int.Parse(missionId));
					shoppingWindow.launch();
				};
			}

			public override int SelfCompare(MissionCard other)
			{
				UnownedMission unownedMission = other as UnownedMission;
				if (unownedMission != null)
				{
					return SortingID - unownedMission.SortingID;
				}
				return string.Compare(missionKey, other.missionKey, true);
			}
		}

		public abstract class MissionCard : GUISubScalingWindow, IComparable<MissionCard>
		{
			public enum CardType
			{
				FavoriteMission,
				MissionOfTheDay,
				OwnedMission,
				UnownedMission
			}

			protected static Rect contentLoadingRect = new Rect(50f, 70f, 180f, 180f);

			protected CardType cardType;

			public string missionId;

			public string missionKey;

			private SHSBrawlerGadget brawlerGadget;

			private OwnableDefinition _def;

			private BrawlerChooseAMissionWindow missionWindow;

			public BrawlerChooseAMissionWindow MissionWindow
			{
				set
				{
					missionWindow = value;
				}
			}

			public MissionCard(string missionId, string missionKey, SHSBrawlerGadget brawlerGadget, CardType cardType)
				: base(ModSize(1f))
			{
				this.cardType = cardType;
				this.missionId = missionId;
				this.missionKey = missionKey;
				CspUtils.DebugLog("MC missionId=" + missionId);  // CSP
				CspUtils.DebugLog("MC missionKey=" + missionKey);  //CSP
				this.brawlerGadget = brawlerGadget;
				_def = OwnableDefinition.getMissionDef(missionKey);
				SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
				Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
				Traits.VisibleAncestryTrait = ControlTraits.VisibilityAncestryTraitEnum.DetachedVisibility;
			}

			protected void InitializeRequiredContent()
			{
				if (missionKey == LauncherSequences.FixedMissionKey)
				{
					ConfigureRequiredContent(new ContentReference(ContentTypeEnum.PriorityGroup, AssetBundleLoader.BundleGroup.SpecialMission));
				}
				else
				{
					ConfigureRequiredContent(new ContentReference(ContentTypeEnum.PriorityGroup, AssetBundleLoader.BundleGroup.MissionsAndEnemies));
				}
				ContentLoadingCustomDrawRect = contentLoadingRect;
			}

			public override void HandleResize(GUIResizeMessage message)
			{
				base.HandleResize(message);
				ContentLoadingCustomDrawRect = contentLoadingRect;
			}

			public void ClickedOnAMission()
			{
				if (missionKey == brawlerGadget.DataManager.DailyMissionKey)
				{
					brawlerGadget.DataManager.SelectedPlayMode = PlayMode.PlayDailyMission;
				}
				brawlerGadget.MissionClicked(missionKey);
			}

			public void Move(float value)
			{
				float num = Mathf.Abs(value);
				Alpha = Mathf.Clamp01(1.5f / num - 0.75f);
				Offset = new Vector2(Mathf.Sin((float)Math.PI / 180f * (value * 135f)) * 80f, -10f);
				Rotation = Mathf.Sin((float)Math.PI / 180f * (value * 100f)) * 4f;
				IsVisible = (num < 1.1666f);
				if (num < 0.5f)
				{
					ToTheFront();
					if (!(this is UnownedMission))
					{
						missionWindow.FrontMissionKey = missionKey;
						missionWindow.FrontMissionID = Convert.ToInt32(missionId);
						missionWindow.toggleStar(Convert.ToInt32(missionId));
					}
					ISHSCounterType counter = AppShell.Instance.CounterManager.GetCounter("MissionsComplete");
					if (counter != null)
					{
						missionWindow.TimesPlayedText = counter.GetCurrentValue(missionKey).ToString();
					}
					ISHSCounterType counter2 = AppShell.Instance.CounterManager.GetCounter("MissionsHighestMedal");
					if (counter2 != null)
					{
						long currentValue = counter2.GetCurrentValue(missionKey);
						string medalName = missionWindow.GetMedalName(currentValue);
						missionWindow.MissionMedalTextureSource = "brawlergadget_bundle|brawler_" + medalName + "_small";
					}
					missionWindow.setKeywords(_def.getKeywords());
				}
			}

			public void ToTheFront()
			{
				Parent.ControlList.Remove(this);
				Parent.ControlList.Add(this);
			}

			public static Vector2 ModSize(float perc)
			{
				return new Vector2(330f, 428f) * 0.9f * perc;
			}

			public abstract int SelfCompare(MissionCard other);

			public int CompareTo(MissionCard other)
			{
				if (cardType == other.cardType)
				{
					return SelfCompare(other);
				}
				return cardType - other.cardType;
			}
		}

		private SHSBrawlerGadget brawlerGadget;

		private GUISlider slider;

		private GUIButton okButton;

		private GUIButton backButton;

		private GUISimpleControlWindow contentWindow;

		private GUIImage background;

		private GUIImage missionInfo;

		private GUIImage missionInfoSign;

		private GUIImage bestRank;

		private GUIImage timesPlayed;

		private GUIImage missionMedal;

		private GUIImageWithEvents favoriteButtonOn;

		private GUIImageWithEvents favoriteButtonOff;

		private GUIDropShadowTextLabel timesPlayedText;

		private GUISimpleControlWindow _keywordIconHolder;

		private List<GUIImageWithEvents> _abilityIcons = new List<GUIImageWithEvents>();

		private List<MissionCard> missions = new List<MissionCard>();

		private Hashtable medalNameLookups = new Hashtable();

		public int FrontMissionID;

		private string missionToSelect;

		private string frontMissionKey = string.Empty;

		private bool disableAnimation;

		public AnimClip centerAnimation;

		private float targetAnimationLocation;

		public string MissionMedalTextureSource
		{
			set
			{
				missionMedal.TextureSource = value;
			}
		}

		public string TimesPlayedText
		{
			set
			{
				timesPlayedText.Text = value;
			}
		}

		public string FrontMissionKey
		{
			get
			{
				return frontMissionKey;
			}
			set
			{
				frontMissionKey = value;
			}
		}

		public BrawlerChooseAMissionWindow(SHSBrawlerGadget brawlerGadget)
		{
			this.brawlerGadget = brawlerGadget;
			medalNameLookups[0L] = "bronze";
			medalNameLookups[1L] = "silver";
			medalNameLookups[2L] = "gold";
			medalNameLookups[3L] = "adamantium";
			contentWindow = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(SHSGadget.CENTER_WINDOW_SIZE, new Vector2(-100f, 0f));
			background = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(791f, 401f), new Vector2(0f, 0f));
			background.TextureSource = "brawlergadget_bundle|brawler_gadget_choosemission_backdrop_new";
			background.Id = "background";
			missionInfo = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(168f, 327f), new Vector2(300f, -20f));
			missionInfo.TextureSource = "brawlergadget_bundle|brawler_gadget_mission_info_panel";
			missionInfo.Id = "missionInfo";
			missionInfoSign = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(107f, 60f), new Vector2(300f, -130f));
			missionInfoSign.TextureSource = "brawlergadget_bundle|L_choosemission_missioninfo";
			missionInfoSign.Id = "missionInfoSign";
			bestRank = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(120f, 31f), new Vector2(295f, -70f));
			bestRank.TextureSource = "brawlergadget_bundle|L_choosemission_bestRank";
			bestRank.Id = "bestRank";
			timesPlayed = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(83f, 48f), new Vector2(277f, 38f));
			timesPlayed.TextureSource = "brawlergadget_bundle|L_choosemission_timesplayed_new";
			timesPlayed.Id = "timesPlayed";
			timesPlayedText = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(50f, 100f), new Vector2(285f, 80f));
			timesPlayedText.SetupText(GUIFontManager.SupportedFontEnum.Komica, 32, GUILabel.GenColor(255, 241, 148), TextAnchor.MiddleCenter);
			timesPlayedText.FrontColor = GUILabel.GenColor(255, 241, 148);
			timesPlayedText.BackColor = GUILabel.GenColor(0, 21, 105);
			timesPlayedText.TextOffset = new Vector2(-2f, 3f);
			timesPlayedText.Text = "0";
			timesPlayedText.Id = "timesPlayedText";
			missionMedal = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(80f, 80f), new Vector2(290f, -32f));
			missionMedal.TextureSource = "brawlergadget_bundle|brawler_bronze_small";
			Vector2 offset = new Vector2(290f, 130f);
			favoriteButtonOn = GUIControl.CreateControlFrameCentered<GUIImageWithEvents>(new Vector2(50f, 50f), offset);
			favoriteButtonOn.TextureSource = "brawlergadget_bundle|favorite_mission_star_empty_small";
			favoriteButtonOn.Id = "missionInfoSign";
			favoriteButtonOn.Click += delegate
			{
				PrefsManager.addFavoriteMission(FrontMissionID);
				favoriteButtonOff.IsVisible = true;
				favoriteButtonOn.IsVisible = false;
			};
			favoriteButtonOn.ToolTip = new NamedToolTipInfo("#BRAWLER_ADD_FAVORITE_MISSION");
			favoriteButtonOff = GUIControl.CreateControlFrameCentered<GUIImageWithEvents>(new Vector2(50f, 50f), offset);
			favoriteButtonOff.TextureSource = "brawlergadget_bundle|favorite_mission_star_small";
			favoriteButtonOff.Id = "missionInfoSign";
			favoriteButtonOff.Click += delegate
			{
				PrefsManager.removeFavoriteMission(FrontMissionID);
				favoriteButtonOff.IsVisible = false;
				favoriteButtonOn.IsVisible = true;
			};
			favoriteButtonOff.ToolTip = new NamedToolTipInfo("#BRAWLER_REMOVE_FAVORITE_MISSION");
			_keywordIconHolder = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(50f, 160f), new Vector2(205f, -90f));
			_keywordIconHolder.IsVisible = true;
			_keywordIconHolder.Id = "keywordHolder";
			okButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(512f, 512f), new Vector2(-3f, 244f));
			okButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_okbutton_big");
			okButton.HitTestSize = new Vector2(0.8f, 0.25f);
			okButton.Click += delegate
			{

				bool flag = false;
				foreach (MissionCard mission in missions)
				{
					if (mission.missionKey == frontMissionKey)
					{
						flag = true;
						mission.ClickedOnAMission();
						break;
					}
				}
				if (!flag)
				{
					this.brawlerGadget.MissionClicked(frontMissionKey);
				}
	
			};
			backButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(-224f, 255f));
			backButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_brawler_airlock_backbutton");
			backButton.HitTestSize = new Vector2(0.5f, 0.45f);
			backButton.Click += delegate
			{
				this.brawlerGadget.BackWasClicked();
			};
			RefreshMissionList();
			slider = GUIControl.CreateControlFrameCentered<GUISlider>(new Vector2(732f, 56f), new Vector2(0f, 170f));
			slider.Orientation = GUISlider.SliderOrientationEnum.Horizontal;
			slider.ArrowsEnabled = true;
			slider.Min = 0f;
			slider.Max = missions.Count - 1;
			slider.UseMouseWheelScroll = true;
			slider.ConsrainToMaxAndMin = true;
			slider.MouseScrollWheelAmount = 1f;
			slider.Changed += slider_Changed;
			slider.StartArrow.ToolTip = new NamedToolTipInfo("#shopping_arrows_missions");
			slider.EndArrow.ToolTip = new NamedToolTipInfo("#shopping_arrows_missions");
			slider.ThumbButton.ToolTip = new NamedToolTipInfo("#shopping_button_missions");
			Add(background);
			Add(contentWindow);
			Add(slider);
			Add(missionInfo);
			Add(missionInfoSign);
			Add(bestRank);
			Add(timesPlayed);
			Add(timesPlayedText);
			Add(missionMedal);
			Add(favoriteButtonOff);
			Add(favoriteButtonOn);
			Add(okButton);
			Add(backButton);
			Add(_keywordIconHolder);
		}

		public void setKeywords(List<Keyword> keywords)
		{
			_keywordIconHolder.RemoveAllControls();
			foreach (GUIImageWithEvents abilityIcon in _abilityIcons)
			{
				abilityIcon.Dispose();
			}
			_abilityIcons.Clear();
			int num = 0;
			foreach (Keyword keyword in keywords)
			{
				if (keyword.icon.Length > 0)
				{
					GUIImageWithEvents gUIImageWithEvents = GUIControl.CreateControlTopLeftFrame<GUIImageWithEvents>(new Vector2(48f, 48f), new Vector2(0f, num * 50));
					gUIImageWithEvents.TextureSource = keyword.icon;
					_keywordIconHolder.Add(gUIImageWithEvents);
					_abilityIcons.Add(gUIImageWithEvents);
					gUIImageWithEvents.Id = keyword.keyword;
					gUIImageWithEvents.IsVisible = true;
					gUIImageWithEvents.ToolTip = new NamedToolTipInfo(keyword.tooltip, new Vector2(20f, 0f));
					num++;
				}
			}
		}

		public string GetMedalName(long medalValue)
		{
			if (medalNameLookups.ContainsKey(medalValue))
			{
				return (string)medalNameLookups[medalValue];
			}
			return string.Empty;
		}

		public void toggleStar(int missionID)
		{
			if (PrefsManager.getFavoriteMissions().Contains(missionID))
			{
				favoriteButtonOff.IsVisible = true;
				favoriteButtonOn.IsVisible = false;
			}
			else
			{
				favoriteButtonOff.IsVisible = false;
				favoriteButtonOn.IsVisible = true;
			}
		}

		private void GoLeft(GUIControl sender, GUIClickEvent EventData)
		{
			AnimateCenter(Mathf.Clamp(Mathf.RoundToInt(targetAnimationLocation - 1f), slider.Min, slider.Max), true);
		}

		private void GoRight(GUIControl sender, GUIClickEvent EventData)
		{
			AnimateCenter(Mathf.Clamp(Mathf.RoundToInt(targetAnimationLocation + 1f), slider.Min, slider.Max), true);
		}

		private void OnShoppingItemPurchased(ShoppingItemPurchasedMessage message)
		{
			if (message.ItemType == OwnableDefinition.Category.Mission)
			{
				missionToSelect = message.OwnableName;
			}
		}

		public override void OnShow()
		{
			base.OnShow();
			slider.FireChanged();
			AppShell.Instance.EventMgr.AddListener<MissionFetchCompleteMessage>(OnMissionFetchComplete);
			AppShell.Instance.EventMgr.AddListener<ShoppingItemPurchasedMessage>(OnShoppingItemPurchased);
		}

		public override void OnHide()
		{
			base.OnHide();
			AppShell.Instance.EventMgr.RemoveListener<MissionFetchCompleteMessage>(OnMissionFetchComplete);
			AppShell.Instance.EventMgr.RemoveListener<ShoppingItemPurchasedMessage>(OnShoppingItemPurchased);
		}

		private void OnMissionFetchComplete(MissionFetchCompleteMessage msg)
		{
			RefreshMissionList();
			SelectDefaultMission();
		}

		private void SelectDefaultMission()
		{
			if (missionToSelect != null)
			{
				int i;
				for (i = 0; i < missions.Count && !(missions[i].missionKey == missionToSelect); i++)
				{
				}
				if (i >= missions.Count)
				{
					i = 0;
				}
				missionToSelect = null;
				float value = slider.Value;
				slider.Value = i;
				if ((int)value == i)
				{
					slider.FireChanged();
				}
			}
			else
			{
				slider.Value = 0f;
			}
		}

		private void slider_Changed(GUIControl sender, GUIChangedEvent eventData)
		{
			for (int i = 0; i < missions.Count; i++)
			{
				missions[i].Move((float)i - slider.Value);
			}
			if (slider.IsDragging && !base.AnimationInProgress)
			{
				base.AnimationPieceManager.ClearAll();
				centerAnimation = null;
			}
			if (!slider.IsDragging && !disableAnimation)
			{
				AnimateCenter(Mathf.RoundToInt(slider.Value), false);
			}
		}

		public void AnimateCenter(float CenterLocation, bool muted)
		{
			if (!base.AnimationInProgress)
			{
				targetAnimationLocation = CenterLocation;
				base.AnimationPieceManager.ClearAll();
				centerAnimation = SliderAnimation.CenterSlider(slider, CenterLocation, muted, AnimateSlider);
				base.AnimationPieceManager.Add(centerAnimation);
			}
		}

		public void AnimateSlider(float x)
		{
			disableAnimation = true;
			slider.Value = x;
			disableAnimation = false;
		}

		public void RefreshMissionList()
		{
			foreach (MissionCard mission in missions)
			{
				contentWindow.Remove(mission);
			}
			missions.Clear();
			PopulateMissionList();
			foreach (MissionCard mission2 in missions)
			{
				contentWindow.Add(mission2);
			}
		}

		public void PopulateMissionList()
		{
			if (PlayerPrefs.GetInt("WipMissions", 0) != 0)
			{
				PopulateMissionListWithAllMissions(brawlerGadget);
				missions.Sort();
			}
			else
			{
				PopulateMissionListWithOwnedAndPurchaseableMissions(brawlerGadget);
				missions.Sort();
			}
		}

		private void PopulateMissionListWithOwnedAndPurchaseableMissions(SHSBrawlerGadget brawlerGadget)
		{
			MissionManifest missionManifest = AppShell.Instance.MissionManifest;
			BrawlerPlayerDataManager DataManager = brawlerGadget.DataManager;
			AvailableMissionCollection availableMissions = AppShell.Instance.Profile.AvailableMissions;
			if (!LauncherSequences.DependencyCheck(AssetBundleLoader.BundleGroup.MissionsAndEnemies, false))
			{
				string missionIdFromKey = missionManifest.GetMissionIdFromKey(LauncherSequences.FixedMissionKey);
				if (missionIdFromKey != null)
				{
					MissionCard missionCard = new OwnedMission(missionIdFromKey, missionManifest[missionIdFromKey].MissionKey, brawlerGadget);
					missionCard.MissionWindow = this;
					missions.Add(missionCard);
				}
				else
				{
					CspUtils.DebugLog("Fixed mission <" + LauncherSequences.FixedMissionKey + "> is not in the mission manifest");
				}
			}
			int num = 0;
			foreach (int favoriteMission in PrefsManager.getFavoriteMissions())
			{
				if (missionManifest.ContainsKey(string.Empty + favoriteMission))
				{
					FavoriteMissionCard favoriteMissionCard = new FavoriteMissionCard(string.Empty + favoriteMission, missionManifest[string.Empty + favoriteMission].MissionKey, brawlerGadget, num++);
					favoriteMissionCard.MissionWindow = this;
					missions.Add(favoriteMissionCard);
					if (DataManager.DailyMissionId == string.Empty + favoriteMission)
					{
						favoriteMissionCard.setDaily();
					}
				}
				else
				{
					CspUtils.DebugLog("MissionManifest does not contain favorite mission ID " + favoriteMission);
				}
			}
			if (DataManager.DailyMissionId != null && missionManifest.ContainsKey(DataManager.DailyMissionId))
			{
				MissionCard missionCard2 = missions.Find(delegate(MissionCard card)
				{
					return card.missionId == DataManager.DailyMissionId;
				});
				if (missionCard2 == null)
				{
					MissionCard missionCard3 = new MissionOfTheDay(DataManager.DailyMissionId, missionManifest[DataManager.DailyMissionId].MissionKey, brawlerGadget);
					missionCard3.MissionWindow = this;
					missions.Add(missionCard3);
				}
			}
			else
			{
				CspUtils.DebugLog("mission Id " + DataManager.DailyMissionId + " is the mission of the day but does not exist in the mission manifest");
			}
			using (Dictionary<string, AvailableMission>.ValueCollection.Enumerator enumerator2 = availableMissions.Values.GetEnumerator())
			{
				AvailableMission am;
				while (enumerator2.MoveNext())
				{
					am = enumerator2.Current;
					MissionCard missionCard4 = missions.Find(delegate(MissionCard card)
					{
						return card.missionId == am.MissionId;
					});
					if (missionCard4 == null)
					{
						if (missionManifest.ContainsKey(am.MissionId))
						{
							MissionCard missionCard5 = new OwnedMission(am.MissionId, missionManifest[am.MissionId].MissionKey, brawlerGadget);
							missionCard5.MissionWindow = this;
							missions.Add(missionCard5);
						}
						else
						{
							CspUtils.DebugLog("mission Id " + am.MissionId + " is in your owned missions but does not exist in the mission manifest");
						}
					}
				}
			}
			int num2 = 0;
			using (List<CatalogItem>.Enumerator enumerator3 = NewShoppingManager.categoryContents[NewShoppingManager.ShoppingCategory.Mission].GetEnumerator())
			{
				CatalogItem item;
				while (enumerator3.MoveNext())
				{
					item = enumerator3.Current;
					MissionCard missionCard6 = missions.Find(delegate(MissionCard card)
					{
						return card.missionId == string.Empty + item.ownableTypeID;
					});
					if (missionCard6 == null)
					{
						if (missionManifest.ContainsKey(string.Empty + item.ownableTypeID))
						{
							MissionCard missionCard7 = new UnownedMission(string.Empty + item.ownableTypeID, missionManifest[string.Empty + item.ownableTypeID].MissionKey, brawlerGadget, item.name, num2);
							missionCard7.MissionWindow = this;
							missions.Add(missionCard7);
							num2++;
						}
						else
						{
							CspUtils.DebugLog("mission Id " + item.ownableTypeID + " is in the shopping catelog but does not exist in the mission manifest");
						}
					}
				}
			}
		}

		private void PopulateMissionListWithAllMissions(SHSBrawlerGadget brawlerGadget)
		{
			using (Dictionary<string, MissionManifestEntry>.KeyCollection.Enumerator enumerator = AppShell.Instance.MissionManifest.Keys.GetEnumerator())
			{
				string missionId;
				while (enumerator.MoveNext())
				{
					missionId = enumerator.Current;
					MissionCard missionCard = missions.Find(delegate(MissionCard card)
					{
						return card.missionId == missionId;
					});
					if (missionCard == null)
					{
						MissionCard missionCard2 = new OwnedMission(missionId, AppShell.Instance.MissionManifest[missionId].MissionKey, brawlerGadget);
						missionCard2.MissionWindow = this;
						missions.Add(missionCard2);
					}
				}
			}
		}
	}

	public class BrawlerInviteFriendsWindow : GadgetCenterWindow
	{
		public class FriendSelection : SHSSelectionWindow<FriendItem, GUIControlWindow>
		{
			public FriendSelection(GUISlider slider)
				: base(slider, SelectionWindowType.OneAcross)
			{
				SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
				slider.FireChanged();
			}
		}

		public class FriendItem : SHSSelectionItem<GUIControlWindow>, IComparable<FriendItem>
		{
			public GUILabel nameLabel;

			public GUILabel locationLabel;

			private GUIButton thumbsUp;

			private GUIHotSpotButton hotSpot;

			private GUIImage friendStatus;

			public Friend friend;

			private BrawlerInviteFriendsWindow headWindow;

			public bool selected;

			private bool trueFriend;

			public FriendItem(Friend friend, bool trueFriend, BrawlerInviteFriendsWindow headWindow)
			{
				this.friend = friend;
				this.headWindow = headWindow;
				this.trueFriend = trueFriend;
				item = new GUIControlWindow();
				item.HitTestType = HitTestTypeEnum.Transparent;
				itemSize = new Vector2(203f, 46f);
				currentState = SelectionState.Active;
				nameLabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(160f, 46f), new Vector2(14f, 7f));
				nameLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(81, 83, 78), TextAnchor.UpperLeft);
				nameLabel.HitTestType = HitTestTypeEnum.Rect;
				nameLabel.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
				nameLabel.ToolTip = new NamedToolTipInfo("#mg_tt_5");
				nameLabel.WordWrap = false;
				locationLabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(160f, 46f), new Vector2(14f, 22f));
				locationLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, GUILabel.GenColor(139, 149, 96), TextAnchor.UpperLeft);
				locationLabel.WordWrap = false;
				thumbsUp = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(56f, 56f), new Vector2(79f, 0f));
				thumbsUp.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|thumbsup_button");
				friendStatus = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(35f, 35f), new Vector2(-83f, 0f));
				friendStatus.TextureSource = ((!trueFriend) ? "communication_bundle|mshs_player_indent_icon_met" : "communication_bundle|mshs_player_indent_icon_friend");
				hotSpot = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(new Vector2(203f, 46f), new Vector2(0f, 5f));
				item.Add(nameLabel);
				item.Add(locationLabel);
				item.Add(thumbsUp);
				item.Add(friendStatus);
				item.Add(hotSpot);
				hotSpot.Click += hotSpot_Click;
				thumbsUp.Click += hotSpot_Click;
				UpdateInfo();
			}

			public void hotSpot_Click(GUIControl sender, GUIClickEvent EventData)
			{
				if (selected)
				{
					headWindow.RemoveFriendClickedOn(friend);
					return;
				}
				selected = true;
				UpdateInfo();
				headWindow.FriendClickedOn(friend);
			}

			public void UpdateInfo()
			{
				hotSpot.IsVisible = (friend.Online && friend.AvailableForActivity);
				nameLabel.Text = friend.Name;
				locationLabel.Text = AppShell.Instance.stringTable["#friend_location"] + friend.Location;
				thumbsUp.IsVisible = selected;
				friendStatus.Alpha = ((!friend.Online || !friend.AvailableForActivity) ? 0.4f : 1f);
				if (friend.Online && friend.AvailableForActivity)
				{
					currentState = SelectionState.Active;
					return;
				}
				thumbsUp.IsVisible = false;
				currentState = SelectionState.Passive;
			}

			public int CompareTo(FriendItem other)
			{
				if (trueFriend == other.trueFriend && friend.Online == other.friend.Online && friend.AvailableForActivity == other.friend.AvailableForActivity)
				{
					return friend.Name.CompareTo(other.friend.Name);
				}
				if (!trueFriend && other.trueFriend)
				{
					return -1;
				}
				if (friend.AvailableForActivity && !other.friend.AvailableForActivity)
				{
					return -1;
				}
				if (!friend.AvailableForActivity && other.friend.AvailableForActivity)
				{
					return 1;
				}
				return friend.Name.CompareTo(other.friend.Name);
			}
		}

		public class SelectedFriendsList : GUISimpleControlWindow
		{
			public class SelectedFriendItem : GUISimpleControlWindow
			{
				public Friend StoredFriend;

				private GUILabel label;

				public SelectedFriendItem(BrawlerInviteFriendsWindow headWindow)
				{
					label = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(250f, 28f), Vector2.zero);
					label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 21, Color.white, TextAnchor.MiddleLeft);
					label.HitTestType = HitTestTypeEnum.Rect;
					label.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
					Add(label);
					label.Click += delegate
					{
						if (StoredFriend != null)
						{
							headWindow.RemoveFriendClickedOn(StoredFriend);
						}
					};
				}

				public void StoreFriend(Friend StoredFriend)
				{
					this.StoredFriend = StoredFriend;
					if (StoredFriend != null)
					{
						label.Text = StoredFriend.Name;
					}
					else
					{
						label.Text = string.Empty;
					}
				}
			}

			private List<Friend> SelectedFriends = new List<Friend>(10);

			private List<SelectedFriendItem> playerNames = new List<SelectedFriendItem>(10);

			public int FriendCount
			{
				get
				{
					return SelectedFriends.Count;
				}
			}

			public SelectedFriendsList(BrawlerInviteFriendsWindow headWindow)
			{
				GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(599f, 531f), Vector2.zero);
				gUIImage.TextureSource = "brawlergadget_bundle|brawler_gadget_invite_backdrop";
				Add(gUIImage);
				GUIDrawTexture gUIDrawTexture = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(new Vector2(310f, 104f), new Vector2(50f, -175f));
				gUIDrawTexture.ToolTip = new NamedToolTipInfo("#mg_tt_4");
				Add(gUIDrawTexture);
				GUIDropShadowTextLabel gUIDropShadowTextLabel = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(312f, 100f), new Vector2(44f, -173f));
				gUIDropShadowTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 24, Color.white, TextAnchor.MiddleCenter);
				gUIDropShadowTextLabel.FrontColor = Color.white;
				gUIDropShadowTextLabel.BackColor = GUILabel.GenColor(0, 21, 105);
				gUIDropShadowTextLabel.TextOffset = new Vector2(-2f, 3f);
				gUIDropShadowTextLabel.Text = RIGHT_TEXT;
				Add(gUIDropShadowTextLabel);
				for (int i = 0; i < 10; i++)
				{
					SelectedFriendItem selectedFriendItem = new SelectedFriendItem(headWindow);
					selectedFriendItem.SetSize(new Vector2(250f, 28f));
					selectedFriendItem.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(46f, 145 + 28 * i));
					playerNames.Add(selectedFriendItem);
					Add(selectedFriendItem);
				}
			}

			public void RefreshFriendsList()
			{
				for (int i = 0; i < playerNames.Count; i++)
				{
					if (i < SelectedFriends.Count)
					{
						playerNames[i].StoreFriend(SelectedFriends[i]);
					}
					else
					{
						playerNames[i].StoreFriend(null);
					}
				}
			}

			public void AddFriend(Friend friend)
			{
				SelectedFriends.Add(friend);
				RefreshFriendsList();
			}

			public void RemoveFriend(Friend friend)
			{
				SelectedFriends.Remove(friend);
				RefreshFriendsList();
			}

			public string[] GetSelectedFriendsArray()
			{
				List<string> list = new List<string>(SelectedFriends.Count);
				foreach (Friend selectedFriend in SelectedFriends)
				{
					list.Add(selectedFriend.Id.ToString());
				}
				return list.ToArray();
			}
		}

		public FriendSelection FriendsWindow;

		private SelectedFriendsList SelectedFriendsWindow;

		public static string LEFT_TEXT = "#gadget_friends_list_left_info_text";

		public static string RIGHT_TEXT = "#gadget_friends_list_right_info_confirm_text";

		public BrawlerInviteFriendsWindow(SHSBrawlerGadget brawlerGadget)
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(314f, 492f), new Vector2(-243f, 40f));
			gUIImage.TextureSource = "brawlergadget_bundle|brawler_gadget_left_bg";
			GUISlider gUISlider = GUIControl.CreateControlFrameCentered<GUISlider>(new Vector2(50f, 340f), new Vector2(-117f, 68f));
			gUISlider.StartArrow.ToolTip = new NamedToolTipInfo("#mg_tt_1");
			gUISlider.EndArrow.ToolTip = new NamedToolTipInfo("#mg_tt_1");
			FriendsWindow = new FriendSelection(gUISlider);
			FriendsWindow.SetSize(217f, 364f);
			FriendsWindow.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-231f, 70f));
			FriendsWindow.AddList(getFriendsList());
			GUIDrawTexture gUIDrawTexture = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(new Vector2(285f, 104f), new Vector2(-245f, -140f));
			gUIDrawTexture.TextureSource = "brawlergadget_bundle|brawler_gadget_left_fakefade_top";
			gUIDrawTexture.ToolTip = new NamedToolTipInfo("#mg_tt_2");
			GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(261f, 67f), new Vector2(-230f, 237f));
			gUIImage2.TextureSource = "brawlergadget_bundle|brawler_gadget_left_fakefade_bottom";
			SelectedFriendsWindow = new SelectedFriendsList(this);
			SelectedFriendsWindow.SetSize(599f, 531f);
			SelectedFriendsWindow.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(116f, 30f));
			GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(163f, 225f));
			gUIButton.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|L_okbutton");
			gUIButton.HitTestSize = new Vector2(0.547f, 0.367f);
			gUIButton.Click += delegate
			{
				int num = 0;
				foreach (FriendItem item in FriendsWindow.items)
				{
					if (item.friend.Online)
					{
						num++;
					}
				}
				if (num != 0 && SelectedFriendsWindow.FriendCount == 0)
				{
					GUIDialogNotificationSink notificationSink = new GUIDialogNotificationSink(delegate(string Id, DialogState state)
					{
						if (state == DialogState.Ok && !DetermineIfAnyPlayerIsBlocked(SelectedFriendsWindow.GetSelectedFriendsArray()))
						{
							brawlerGadget.FriendsToPlayWithSubmitted(SelectedFriendsWindow.GetSelectedFriendsArray());
						}
					});
					SHSErrorNotificationWindow sHSErrorNotificationWindow = new SHSErrorNotificationWindow(SHSErrorNotificationWindow.ErrorIcons.MissionNoGo, SHSErrorNotificationWindow.ErrorChoiceType.YesNo);
					sHSErrorNotificationWindow.TitleText = "#error_oops";
					sHSErrorNotificationWindow.Text = "#notification_mission_allheroinvite";
					sHSErrorNotificationWindow.AllowTimeout = false;
					sHSErrorNotificationWindow.NotificationSink = notificationSink;
					GUIManager.Instance.ShowDynamicWindow(sHSErrorNotificationWindow, "Notification Root", ModalLevelEnum.Full);
				}
				else if (SelectedFriendsWindow.FriendCount == 0)
				{
					brawlerGadget.NoFriendsToPlayWithSubmitted();
				}
				else if (!DetermineIfAnyPlayerIsBlocked(SelectedFriendsWindow.GetSelectedFriendsArray()))
				{
					brawlerGadget.FriendsToPlayWithSubmitted(SelectedFriendsWindow.GetSelectedFriendsArray());
				}
			};
			gUIButton.ToolTip = new NamedToolTipInfo("#mg_tt_3");
			GUIDropShadowTextLabel gUIDropShadowTextLabel = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(286f, 100f), new Vector2(-244f, -145f));
			gUIDropShadowTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 24, Color.white, TextAnchor.MiddleCenter);
			gUIDropShadowTextLabel.FrontColor = Color.white;
			gUIDropShadowTextLabel.BackColor = GUILabel.GenColor(0, 21, 105);
			gUIDropShadowTextLabel.TextOffset = new Vector2(-2f, 3f);
			gUIDropShadowTextLabel.Text = LEFT_TEXT;
			Add(gUIImage);
			Add(FriendsWindow);
			Add(gUIDrawTexture);
			Add(gUIImage2);
			Add(SelectedFriendsWindow);
			Add(gUIButton);
			Add(gUISlider);
			Add(gUIDropShadowTextLabel);
		}

		private bool DetermineIfAnyPlayerIsBlocked(string[] playerIdList)
		{
			foreach (string s in playerIdList)
			{
				if (AppShell.Instance.Profile.AvailableFriends.IsPlayerInBlockedList(int.Parse(s)))
				{
					SHSFriendsListWindow.UnblockPlayerQuery();
					return true;
				}
			}
			return false;
		}

		public void RemoveFriendClickedOn(Friend toRemove)
		{
			SelectedFriendsWindow.RemoveFriend(toRemove);
			FriendItem friendItem = FriendsWindow.items.Find(delegate(FriendItem friend)
			{
				return friend.friend == toRemove;
			});
			if (friendItem != null)
			{
				friendItem.selected = false;
				friendItem.UpdateInfo();
			}
		}

		public void FriendClickedOn(Friend friend)
		{
			SelectedFriendsWindow.AddFriend(friend);
		}

		public void PreSelectFriendsAndTempFriends(params Friend[] tempFriends)
		{
			Friend f;
			for (int i = 0; i < tempFriends.Length; i++)
			{
				f = tempFriends[i];
				FriendItem friendItem = FriendsWindow.items.Find(delegate(FriendItem friend)
				{
					return friend.friend.Id == f.Id;
				});
				if (friendItem == null)
				{
					FriendItem friendItem2 = new FriendItem(f, false, this);
					FriendsWindow.AddItem(friendItem2);
					friendItem2.hotSpot_Click(this, null);
				}
				else
				{
					friendItem.hotSpot_Click(this, null);
				}
			}
			FriendsWindow.SortItemList();
		}

		private List<FriendItem> getFriendsList()
		{
			List<FriendItem> list = new List<FriendItem>();
			UserProfile profile = AppShell.Instance.Profile;
			if (profile == null)
			{
				return list;
			}
			foreach (KeyValuePair<string, Friend> availableFriend in profile.AvailableFriends)
			{
				list.Add(new FriendItem(availableFriend.Value, true, this));
			}
			return list;
		}

		public override void OnShow()
		{
			base.OnShow();
			AppShell.Instance.EventMgr.AddListener<FriendUpdateMessage>(OnFriendUpdate);
			foreach (FriendItem item in FriendsWindow.items)
			{
				item.UpdateInfo();
			}
			FriendsWindow.SortItemList();
		}

		public override void OnHide()
		{
			base.OnHide();
			AppShell.Instance.EventMgr.RemoveListener<FriendUpdateMessage>(OnFriendUpdate);
		}

		private void OnFriendUpdate(FriendUpdateMessage message)
		{
			FriendItem friendItem = FriendsWindow.items.Find(delegate(FriendItem friend)
			{
				return friend.friend.Id == message.FriendID;
			});
			friendItem.UpdateInfo();
			FriendsWindow.SortItemList();
		}

		public void OnFriendListUpdated(FriendListUpdatedMessage message)
		{
			if (message.UpdateType == FriendListUpdatedMessage.Type.Reload)
			{
				foreach (KeyValuePair<string, Friend> availableFriend in AppShell.Instance.Profile.AvailableFriends)
				{
					Friend value = availableFriend.Value;
					bool flag = false;
					foreach (FriendItem item in FriendsWindow.items)
					{
						if (item.friend.Id == value.Id)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						FriendsWindow.AddItem(new FriendItem(new Friend(value.Name, value.Id, value.Location, value.Online, value.AvailableForActivity), true, this));
					}
				}
				FriendsWindow.SortItemList();
			}
		}
	}

	public class BrawlerMissionLaunchWindow : GadgetCenterWindow
	{
		public abstract class MainCard : SHSGlowOutlineWindow
		{
			public enum Card
			{
				Left,
				Middle,
				Right
			}

			public override bool IsEnabled
			{
				set
				{
					base.IsEnabled = value;
					foreach (IGUIControl control in ControlList)
					{
						control.IsEnabled = value;
					}
				}
			}

			public MainCard(Card card)
				: base(GetSelectedGlowPath(card))
			{
				SetSize(2000f, 2000f);
				SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
			}

			private static List<Vector2> GetSelectedGlowPath(Card card)
			{
				switch (card)
				{
				case Card.Left:
					return SHSGlowOutlineWindow.GetGlowPath(new Vector2(-117f, -140f), new Vector2(0f, -160f), new Vector2(117f, -156f), new Vector2(135f, 156f), new Vector2(-79f, 174f), new Vector2(-105f, 0f));
				case Card.Middle:
					return SHSGlowOutlineWindow.GetGlowPath(new Vector2(-146f, -127f), new Vector2(-20f, -139f), new Vector2(104f, -136f), new Vector2(100f, 18f), new Vector2(91f, 176f), new Vector2(-20f, 176f), new Vector2(-132f, 174f));
				case Card.Right:
					return SHSGlowOutlineWindow.GetGlowPath(new Vector2(-121f, -182f), new Vector2(140f, -162f), new Vector2(94f, 158f), new Vector2(-142f, 121f));
				default:
					return SHSGlowOutlineWindow.GetGlowPath();
				}
			}
		}

		public class GoButtonWindow : SHSGlowOutlineWindow
		{
			private GUIButton goButton;

			public override bool IsEnabled
			{
				set
				{
					base.IsEnabled = value;
					foreach (IGUIControl control in ControlList)
					{
						control.IsEnabled = value;
					}
				}
			}

			public GoButtonWindow(SHSBrawlerGadget brawlerGadget)
				: base(GetGoButtonGlowPath())
			{
				SetSize(360f, 122f);
				SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
				Offset = new Vector2(-2f, 241f);
				goButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(512f, 512f), Vector2.zero);
				goButton.HitTestSize = new Vector2(0.9f, 0.9f);
				goButton.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|L_gadget_gobutton");
				Add(goButton);
				Highlight(true);
				goButton.Click += delegate
				{
					brawlerGadget.GoButtonClicked();
				};
			}

			private static List<Vector2> GetGoButtonGlowPath()
			{
				return SHSGlowOutlineWindow.GetGlowPath(new Vector2(-161f, -28f), new Vector2(-137f, -39f), new Vector2(-89f, -45f), new Vector2(0f, -48f), new Vector2(89f, -45f), new Vector2(137f, -39f), new Vector2(161f, -28f), new Vector2(160f, 0f), new Vector2(145f, 36f), new Vector2(127f, 42f), new Vector2(0f, 46f), new Vector2(-127f, 42f), new Vector2(-145f, 36f), new Vector2(-160f, 0f));
			}
		}

		public class LeftCard : MainCard
		{
			private GUIButton playWithFriends;

			private GUIButton playDailyMission;

			private GUIButton selectedButton;

			private GUIButton playSolo;

			private SHSBrawlerGadget brawlerGadget;

			public override float Alpha
			{
				set
				{
					base.Alpha = value;
					if (selectedButton == null)
					{
						playSolo.Alpha = value;
						playWithFriends.Alpha = value;
						playDailyMission.Alpha = value;
					}
					else
					{
						playSolo.Alpha = ((playSolo == selectedButton) ? 1f : 0.65f) * value;
						playWithFriends.Alpha = ((playWithFriends == selectedButton) ? 1f : 0.65f) * value;
						playDailyMission.Alpha = ((playDailyMission == selectedButton) ? 1f : 0.65f) * value;
					}
				}
			}

			public LeftCard(SHSBrawlerGadget brawlerGadget)
				: base(Card.Left)
			{
				this.brawlerGadget = brawlerGadget;
				Offset = new Vector2(-268f, 2f);
				playSolo = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(231f, 118f), new Vector2(3f, -110f));
				playSolo.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_playsolo");
				playSolo.OnLoadingActivate = OnPlaySoloClickedLoading;  // added by CSP as a test.
				Add(playSolo);
				playWithFriends = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(227f, 123f), new Vector2(10f, 12.5f));
				playWithFriends.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_playwithfriends");
				Add(playWithFriends);
				playDailyMission = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(215f, 108f), new Vector2(18f, 130f));
				playDailyMission.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_playdailymission");
				playDailyMission.ConfigureRequiredContent(new ContentReference(ContentTypeEnum.PriorityGroup, AssetBundleLoader.BundleGroup.MissionsAndEnemies));
				playDailyMission.OnLoadingActivate = OnPlayDailyMissionClickedLoading;
				Add(playDailyMission);
				playSolo.Click += OnPlaySoloClicked;
				playWithFriends.Click += OnPlayWithFriendsClicked;
				playDailyMission.Click += OnPlayDailyMissionClicked;
				playDailyMission.MouseOver += delegate
				{
					if (playDailyMission.IsContentLoaded)
					{
						brawlerGadget.MissionLaunchWindow.rightCard.FadeInDailyMission();
					}
				};
				playDailyMission.MouseOut += delegate
				{
					brawlerGadget.MissionLaunchWindow.rightCard.FadeOutDailyMission();
				};
			}

			protected void OnPlaySoloClicked(GUIControl sender, GUIClickEvent EventData)
			{
				brawlerGadget.PlaySoloClicked();
				DisableAllBut(playSolo);
			}

			protected void OnPlayWithFriendsClicked(GUIControl sender, GUIClickEvent EventData)
			{
				brawlerGadget.PlayWithFriendsClicked();
				DisableAllBut(playWithFriends);
			}

			protected void OnPlayDailyMissionClicked(GUIControl sender, GUIClickEvent EventData)
			{
				brawlerGadget.PlayDailyMissionClicked();
				DisableAllBut(playDailyMission);
			}

			protected void OnPlaySoloClickedLoading(object sender)  // this method added by CSP for testing.
			{
				//SHSControlCustomDrawMethods.OnLoadingActivateDefault(this);
				CspUtils.DebugLog("OnPlaySoloClickedLoading");
;				brawlerGadget.CloseDelegate(null, null);
			}


			protected void OnPlayDailyMissionClickedLoading(object sender)
			{
				SHSControlCustomDrawMethods.OnLoadingActivateDefault(this);
				brawlerGadget.CloseDelegate(null, null);
			}

			public void DisableAllBut(GUIButton button)
			{
				selectedButton = button;
				Alpha = Alpha;
			}

			public void PlayModeUpdated(PlayMode playMode)
			{
				switch (playMode)
				{
				case PlayMode.PlaySolo:
					DisableAllBut(playSolo);
					brawlerGadget.DataManager.SelectedPlayMode = PlayMode.PlaySolo;
					break;
				case PlayMode.PlayWithFriends:
					DisableAllBut(playWithFriends);
					brawlerGadget.DataManager.SelectedPlayMode = PlayMode.PlayWithFriends;
					break;
				case PlayMode.PlayDailyMission:
					DisableAllBut(playDailyMission);
					brawlerGadget.DataManager.SelectedPlayMode = PlayMode.PlayDailyMission;
					break;
				}
			}
		}

		public class MiddleCard : MainCard
		{
			private GUIButton chooseHero;

			private GUIButton chooseHeroButton;

			private GUIDrawTexture HeroCircle;

			private GUIImage HeroFrame;

			private GUIDrawTexture HeroPortrait;

			private GUIImage chooseHeroTitle;

			private GUIImage attacksTitle;

			private BrawlerPowerSelection powerSelect;

			private SHSBrawlerGadget brawlerGadget;

			public bool heroSelected;

			private string heroName = string.Empty;

			public override bool IsEnabled
			{
				set
				{
					base.IsEnabled = value;
					if (heroSelected && IsEnabled)
					{
						powerSelect.DisablePowerMovesBasedOnLevel(heroName, AppShell.Instance.Profile.AvailableCostumes[heroName].Level);
					}
				}
			}

			public MiddleCard(SHSBrawlerGadget brawlerGadget)
				: base(Card.Middle)
			{
				this.brawlerGadget = brawlerGadget;
				heroSelected = false;
				Offset = new Vector2(15f, -27f);
				HeroPortrait = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(new Vector2(183f, 183f), new Vector2(-16f, -21f));
				HeroPortrait.Click += delegate(GUIControl sender, GUIClickEvent eventArgs)
				{
					chooseHeroButton.FireMouseClick(eventArgs);
				};
				Add(HeroPortrait);
				chooseHeroButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(64f, 64f), new Vector2(-115f, -130f));
				chooseHeroButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|changehero_icon");
				chooseHeroButton.ToolTip = new NamedToolTipInfo("#airlock_pick_a_hero_tt");
				chooseHeroButton.Id = "chooseHeroButton";
				Add(chooseHeroButton);
				chooseHeroTitle = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(160f, 31f), new Vector2(0f, -130f));
				chooseHeroTitle.TextureSource = "persistent_bundle|L_title_pick_a_hero";
				Add(chooseHeroTitle);
				powerSelect = new BrawlerPowerSelection(BrawlerPowerSelection.PowerSelectionSize.Small);
				powerSelect.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, new Vector2(512f, 512f));
				powerSelect.Offset = new Vector2(-20f, 140f);
				Add(powerSelect);
				powerSelect.PowerMove1.Click += delegate
				{
					brawlerGadget.DataManager.SelectedPower = 1;
				};
				powerSelect.PowerMove2.Click += delegate
				{
					brawlerGadget.DataManager.SelectedPower = 2;
				};
				powerSelect.PowerMove3.Click += delegate
				{
					brawlerGadget.DataManager.SelectedPower = 3;
				};
				attacksTitle = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(203f, 51f), new Vector2(-20f, 85f));
				attacksTitle.TextureSource = "brawlergadget_bundle|L_subhead_power_attacks";
				Add(attacksTitle);
				SetTraitsOfCtrls();
				AddDelegates(brawlerGadget);
			}

			public override void OnShow()
			{
				base.OnShow();
				UpdateSelectedHero(brawlerGadget.DataManager.SelectedHero, brawlerGadget.DataManager.SelectedPower);
				HeroPortrait.Alpha = 1f;
				HeroPortrait.IsVisible = heroSelected;
				powerSelect.IsVisible = heroSelected;
				if (heroSelected)
				{
					StatLevelReqsDefinition instance = StatLevelReqsDefinition.Instance;
					int num = 1;
					if (instance != null)
					{
						num = instance.GetMaxPowerAttackUnlockedAt(AppShell.Instance.Profile.AvailableCostumes[heroName].Level);
					}
					if (AppShell.Instance.Profile.LastSelectedPower > num)
					{
						AppShell.Instance.Profile.LastSelectedPower = num;
					}
					switch (AppShell.Instance.Profile.LastSelectedPower)
					{
					case 1:
						powerSelect.PowerMove1.FireMouseClick(null);
						break;
					case 2:
						powerSelect.PowerMove2.FireMouseClick(null);
						break;
					case 3:
						powerSelect.PowerMove3.FireMouseClick(null);
						break;
					}
					if (!IsEnabled)
					{
						powerSelect.PowerMove1.IsEnabled = false;
						powerSelect.PowerMove2.IsEnabled = false;
						powerSelect.PowerMove3.IsEnabled = false;
					}
				}
			}

			public void SetTraitsOfCtrls()
			{
			}

			private void UpdateSelectedHero(string heroName, int powerSelected)
			{
				heroSelected = true;
				this.heroName = heroName;
				if (IsEnabled)
				{
					powerSelect.DisablePowerMovesBasedOnLevel(heroName, AppShell.Instance.Profile.AvailableCostumes[heroName].Level);
				}
				brawlerGadget.nameLoader.SetupTooltips(heroName, powerSelect);
				HeroPortrait.TextureSource = "characters_bundle|expandedtooltip_render_" + heroName;
			}

			public void AddDelegates(SHSBrawlerGadget brawlerGadget)
			{
				chooseHeroButton.Click += delegate
				{
					brawlerGadget.ChooseAHeroClicked();
				};
				powerSelect.PowerMove1.Click += delegate
				{
					brawlerGadget.DataManager.SelectedPower = 1;
				};
				powerSelect.PowerMove2.Click += delegate
				{
					brawlerGadget.DataManager.SelectedPower = 2;
				};
				powerSelect.PowerMove3.Click += delegate
				{
					brawlerGadget.DataManager.SelectedPower = 3;
				};
			}
		}

		public class RightCard : MainCard
		{
			public GUIDrawTexture MissionImage;

			private GUIDrawTexture ChangeMission;

			private SHSBrawlerGadget brawlerGadget;

			private GUIImage DailyMissionImage;

			private GUIImage dailyMissionBadge;

			private GUIImage missionHeadline;

			private GUIButton dailyMissionButton;

			private GUISimpleControlWindow _abilityIconHolder;

			private List<GUIImageWithEvents> _abilityIcons = new List<GUIImageWithEvents>();

			private AnimClip currentFadeMissionAnimation;

			private AnimClip currentFadeDailyMissionAnimation;

			public RightCard(SHSBrawlerGadget brawlerGadget)
				: base(Card.Right)
			{
				this.brawlerGadget = brawlerGadget;
				Offset = new Vector2(260f, 25f);
				MissionImage = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(new Vector2(210f, 275f), new Vector2(-10f, 3f));
				MissionImage.Rotation = 6f;
				Add(MissionImage);
				missionHeadline = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(166f, 34f), new Vector2(37f, -170f));
				missionHeadline.TextureSource = "brawlergadget_bundle|L_title_pick_mission";
				missionHeadline.Rotation = 7f;
				Add(missionHeadline);
				dailyMissionButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(62f, 62f), new Vector2(-80f, -180f));
				dailyMissionButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|changemission_icon");
				dailyMissionButton.Id = "dailyMissionButton";
				Add(dailyMissionButton);
				DailyMissionImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(210f, 275f), new Vector2(-10f, 3f));
				DailyMissionImage.Rotation = 6f;
				DailyMissionImage.TextureSource = "missions_bundle|L_mshs_gameworld_" + brawlerGadget.DataManager.DailyMissionKey;
				Add(DailyMissionImage);
				dailyMissionBadge = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(151f, 151f), new Vector2(50f, 140f));
				dailyMissionBadge.TextureSource = "brawlergadget_bundle|L_daily_mission_logo";
				dailyMissionBadge.Id = "dailyMissionBadge";
				Add(dailyMissionBadge);
				_abilityIconHolder = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(200f, 65f), new Vector2(-30f, 162f));
				_abilityIconHolder.IsVisible = true;
				_abilityIconHolder.Id = "keywordHolder";
				Add(_abilityIconHolder);
				AddDelegates(brawlerGadget);
			}

			public override void OnShow()
			{
				base.OnShow();
				UpdateSelectedMission();
			}

			public void UpdateSelectedMission()
			{
				MissionImage.Alpha = 1f;
				DailyMissionImage.Alpha = 0f;
				MissionImage.TextureSource = "missions_bundle|L_mshs_gameworld_" + brawlerGadget.DataManager.SelectedMissionKey;
				//MissionImage.TextureSource = "missions_bundle|L_mshs_gameworld_m_102S_1_Skydome003";  // CSP test
				bool flag = brawlerGadget.DataManager.SelectedMissionKey == brawlerGadget.DataManager.DailyMissionKey;
				dailyMissionBadge.IsVisible = flag;
				dailyMissionBadge.Alpha = ((!flag) ? 0f : 1f);
				OwnableDefinition missionDef = OwnableDefinition.getMissionDef(brawlerGadget.DataManager.SelectedMissionKey);
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
						GUIImageWithEvents gUIImageWithEvents = GUIControl.CreateControlTopLeftFrame<GUIImageWithEvents>(new Vector2(48f, 48f), new Vector2(num * 49, num * 8));
						gUIImageWithEvents.TextureSource = keyword.icon;
						_abilityIconHolder.Add(gUIImageWithEvents);
						_abilityIcons.Add(gUIImageWithEvents);
						gUIImageWithEvents.Id = keyword.keyword;
						gUIImageWithEvents.IsVisible = true;
						gUIImageWithEvents.ToolTip = new NamedToolTipInfo(keyword.tooltip, new Vector2(20f, 0f));
						num++;
					}
				}
			}

			public void AddDelegates(SHSBrawlerGadget brawlerGadget)
			{
				MissionImage.Click += delegate
				{
					brawlerGadget.ChooseAMissionClicked();
				};
				dailyMissionButton.Click += delegate
				{
					brawlerGadget.ChooseAMissionClicked();
				};
			}

			public void FadeInChangeMission()
			{
				base.AnimationPieceManager.SwapOut(ref currentFadeMissionAnimation, SHSAnimations.Generic.FadeOut(MissionImage, 0.3f));
			}

			public void FadeOutChangeMission()
			{
				base.AnimationPieceManager.SwapOut(ref currentFadeMissionAnimation, SHSAnimations.Generic.FadeIn(MissionImage, 0.3f));
			}

			public void FadeInDailyMission()
			{
				if (!(brawlerGadget.DataManager.SelectedMissionKey == brawlerGadget.DataManager.DailyMissionKey))
				{
					dailyMissionBadge.IsVisible = true;
					dailyMissionBadge.Alpha = 0f;
					base.AnimationPieceManager.Add(SHSAnimations.Generic.FadeIn(dailyMissionBadge, 0.3f));
					base.AnimationPieceManager.SwapOut(ref currentFadeDailyMissionAnimation, SHSAnimations.Generic.FadeIn(DailyMissionImage, 0.3f));
				}
			}

			public void FadeOutDailyMission()
			{
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_004f: Expected O, but got Unknown
				if (!(brawlerGadget.DataManager.SelectedMissionKey == brawlerGadget.DataManager.DailyMissionKey))
				{
					AnimClip animClip = SHSAnimations.Generic.FadeIn(dailyMissionBadge, 0.3f);
					animClip.OnFinished += (Action)(object)(Action)delegate
					{
						dailyMissionBadge.IsVisible = false;
					};
					base.AnimationPieceManager.Add(animClip);
					base.AnimationPieceManager.SwapOut(ref currentFadeDailyMissionAnimation, SHSAnimations.Generic.FadeOut(DailyMissionImage, 0.3f));
				}
			}
		}

		private GoButtonWindow goButton;

		private LeftCard leftCard;

		private MiddleCard middleCard;

		public RightCard rightCard;

		private SHSBrawlerGadget brawlerGadget;

		private GUIImage bkg;

		public BrawlerMissionLaunchWindow(SHSBrawlerGadget brawlerGadget)
		{
			this.brawlerGadget = brawlerGadget;
			bkg = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(816f, 428f), new Vector2(0f, 0f));
			bkg.TextureSource = "brawlergadget_bundle|brawler_gadget_mainscreen_backdrop";
			Add(bkg);
			leftCard = new LeftCard(brawlerGadget);
			middleCard = new MiddleCard(brawlerGadget);
			rightCard = new RightCard(brawlerGadget);
			Add(leftCard);
			Add(middleCard);
			Add(rightCard);
			goButton = new GoButtonWindow(brawlerGadget);
			Add(goButton);
		}

		public void UpdateSelectedMission()
		{
			rightCard.UpdateSelectedMission();
		}

		public void UpdateSelectedButton(PlayMode playMode)
		{
			leftCard.PlayModeUpdated(playMode);
		}

		public override void OnShow()
		{
			base.OnShow();
			UpdateSelectedButton(brawlerGadget.DataManager.SelectedPlayMode);
		}
	}

	public class BrawlerPlayerDataManager
	{
		public UserProfile Profile;

		public PlayMode SelectedPlayMode;

		public bool PlayModeClicked;

		public string[] SelectedInvitees;

		public string SelectedMissionKey;

		public string DailyMissionKey;

		public string DailyMissionId;

		public string SelectedHero;

		public int SelectedPower;

		public BrawlerPlayerDataManager(SHSBrawlerGadget mainWindow)
		{
			if (AppShell.Instance.Profile == null)
			{
				CspUtils.DebugLog("You are trying to run the Mission Gadget without a User Profile.  This can cause massive instability.  You have been warned.");
				return;
			}
			Profile = AppShell.Instance.Profile;
			PlayModeClicked = false;
			SelectedInvitees = new string[0];
			SelectedHero = Profile.LastSelectedCostume;
			SelectedPower = Profile.LastSelectedPower;
			PopulateDailyMissionKey();
			if (!LauncherSequences.DependencyCheck(AssetBundleLoader.BundleGroup.MissionsAndEnemies, false))
			{
				SelectedMissionKey = LauncherSequences.FixedMissionKey;
				SelectedPlayMode = PlayMode.PlaySolo;
			}
			else
			{
				SelectedMissionKey = DailyMissionKey;
				SelectedPlayMode = PlayMode.PlayDailyMission;
			}
		}

		private void PopulateDailyMissionKey()
		{
			if (MotdDefinition.Instance != null)
			{
				foreach (string key in AppShell.Instance.MissionManifest.Keys)
				{
					MissionManifestEntry missionManifestEntry = AppShell.Instance.MissionManifest[key];
					if (missionManifestEntry.TypeId == MotdDefinition.Instance.MissionOwnableTypeId)
					{
						DailyMissionId = key;
						DailyMissionKey = missionManifestEntry.MissionKey;
						break;
					}
				}
			}
			else
			{
				foreach (string key2 in AppShell.Instance.MissionManifest.Keys)
				{
					MissionManifestEntry missionManifestEntry2 = AppShell.Instance.MissionManifest[key2];
					if (missionManifestEntry2.MissionType != MissionManifestEntry.MissionManifestEntryTypeEnum.WorkInProgress)
					{
						DailyMissionId = key2;
						DailyMissionKey = missionManifestEntry2.MissionKey;
						break;
					}
				}
			}
		}
	}

	public class BrawlerPowerSelection : GUISimpleControlWindow
	{
		public enum PowerSelectionSize
		{
			Small,
			Large
		}

		private const int maxPowerAmount = 3;

		private const int checkMarkXOffset = 4;

		private const int checkMarkYOffset = -3;

		private const int smallBackgroundYOffset = 5;

		private const int smallCircleXOffset = 25;

		private const int smallCircleYOffset = 8;

		private const int smallAttackNameOffset = 43;

		private const int largeBackgroundXOffset = 11;

		private const int largeBackgroundYOffset = 10;

		private PowerSelectionSize selectionSize;

		public GUIButton PowerMove1;

		public GUIButton PowerMove2;

		public GUIButton PowerMove3;

		private GUILabel attackName;

		private GUIImage buttonSelected1;

		private GUIImage buttonSelected2;

		private GUIImage checkMark;

		private GUIImage[] buttonSelectBackgrounds;

		private GUIImage[] buttonCheckCircles;

		private Vector2[] checkMarkLocations;

		public string HeroUsed;

		public BrawlerPowerSelection()
			: this(PowerSelectionSize.Large)
		{
		}

		public BrawlerPowerSelection(PowerSelectionSize selectionSize)
		{
			this.selectionSize = selectionSize;
			buttonSelectBackgrounds = new GUIImage[3];
			buttonCheckCircles = new GUIImage[3];
			checkMarkLocations = new Vector2[3];
			bool flag = this.selectionSize == PowerSelectionSize.Small;
			string text = (!flag) ? "large" : "small";
			Vector2 size = new Vector2((!flag) ? 352f : 211f, (!flag) ? 118f : 89f);
			for (int i = 0; i < 3; i++)
			{
				buttonSelectBackgrounds[i] = GUIControl.CreateControlFrameCentered<GUIImage>(size, Vector2.zero);
				buttonSelectBackgrounds[i].TextureSource = "brawlergadget_bundle|brawler_gadget_" + text + "_power" + (i + 1).ToString() + "_selection";
				buttonCheckCircles[i] = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(80f, 80f), Vector2.zero);
				buttonCheckCircles[i].TextureSource = "brawlergadget_bundle|power_selection_button_normal";
			}
			PowerMove1 = genPowerMoveButton("brawlergadget_bundle|super_power_1");
			PowerMove2 = genPowerMoveButton("brawlergadget_bundle|super_power_2");
			PowerMove3 = genPowerMoveButton("brawlergadget_bundle|super_power_3");
			checkMark = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(35f, 34f), new Vector2(0f, 0f));
			checkMark.TextureSource = "brawlergadget_bundle|large_checkmark";
			attackName = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(400f, 50f), Vector2.zero);
			attackName.SetupText(GUIFontManager.SupportedFontEnum.Komica, (!flag) ? 18 : 14, GUILabel.GenColor(255, 248, 141), TextAnchor.UpperCenter);
			Add(buttonSelectBackgrounds[0]);
			Add(buttonSelectBackgrounds[1]);
			Add(buttonSelectBackgrounds[2]);
			Add(PowerMove1);
			Add(PowerMove2);
			Add(PowerMove3);
			Add(buttonCheckCircles[0]);
			Add(buttonCheckCircles[1]);
			Add(buttonCheckCircles[2]);
			Add(checkMark);
			Add(attackName);
			SetupInitialPositioning();
			AssignButtonDelegates();
		}

		private void SetupInitialPositioning()
		{
			bool flag = selectionSize == PowerSelectionSize.Small;
			PowerMove1.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2((!flag) ? (-80f) : (-37f), (!flag) ? (-12f) : (-6f)));
			PowerMove2.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2((!flag) ? (-1f) : (-2f), (!flag) ? (-12f) : (-6f)));
			PowerMove3.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2((!flag) ? 78f : 33.5f, (!flag) ? (-12f) : (-6f)));
			buttonCheckCircles[0].SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2((!flag) ? (-50f) : (-10f), (!flag) ? 3f : 2f));
			buttonCheckCircles[1].SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2((!flag) ? 35f : 20f, (!flag) ? 3f : 2f));
			buttonCheckCircles[2].SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2((!flag) ? 100f : 47f, (!flag) ? 3f : 2f));
			checkMarkLocations[0] = new Vector2((!flag) ? (-90f) : (-40f), (!flag) ? 0f : (-3f));
			checkMarkLocations[1] = new Vector2((!flag) ? 40f : 25f, (!flag) ? 0f : (-3f));
			checkMarkLocations[2] = new Vector2((!flag) ? 153f : 90f, (!flag) ? 0f : (-3f));
			for (int i = 0; i < 3; i++)
			{
				buttonSelectBackgrounds[i].SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
			}
			attackName.Offset = new Vector2(0f, (!flag) ? 50f : 42f);
			Vector2 size = new Vector2((!flag) ? 352f : 211f, (!flag) ? 118f : 89f);
			SetSize(size);
		}

		private void SelectPower(int power)
		{
			bool flag = selectionSize == PowerSelectionSize.Small;
			for (int i = 0; i < 3; i++)
			{
				buttonSelectBackgrounds[i].IsVisible = false;
				buttonCheckCircles[i].Size = new Vector2((!flag) ? 100f : 80f, (!flag) ? 100f : 80f);
			}
			buttonCheckCircles[0].SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2((!flag) ? (-50f) : (-10f), (!flag) ? 3f : 2f));
			buttonCheckCircles[1].SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2((!flag) ? 35f : 20f, (!flag) ? 3f : 2f));
			buttonCheckCircles[2].SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2((!flag) ? 100f : 47f, (!flag) ? 3f : 2f));
			float num = 0f;
			buttonSelectBackgrounds[power].IsVisible = true;
			buttonCheckCircles[power].Size = new Vector2((!flag) ? 128f : 100f, (!flag) ? 128f : 100f);
			Vector2 size = buttonCheckCircles[power].Size;
			float num2 = size.x * 0.13f;
			if (power == 0)
			{
				num += num2;
			}
			if (power == 2)
			{
				num -= num2;
			}
			GUIImage obj = buttonCheckCircles[power];
			Vector2 offset = buttonCheckCircles[power].Offset;
			float x = offset.x + num;
			Vector2 offset2 = buttonCheckCircles[power].Offset;
			obj.Offset = new Vector2(x, offset2.y);
			checkMark.Offset = checkMarkLocations[power];
			SetAttackName(power);
		}

		private void SetAttackName(int power)
		{
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Expected O, but got Unknown
			GUIButton buttonToCopyFrom = null;
			switch (power)
			{
			case 0:
				buttonToCopyFrom = PowerMove1;
				break;
			case 1:
				buttonToCopyFrom = PowerMove2;
				break;
			case 2:
				buttonToCopyFrom = PowerMove3;
				break;
			}
			if (buttonToCopyFrom.ToolTip != NoToolTipInfo.Instance)
			{
				attackName.Text = buttonToCopyFrom.ToolTip.GetToolTipText();
				return;
			}
			AnimClip animClip = AnimClipBuilder.Absolute.Nothing(AnimClipBuilder.Path.Linear(0f, 0f, 0.1f), this);
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				if (buttonToCopyFrom.ToolTip != NoToolTipInfo.Instance)
				{
					attackName.Text = buttonToCopyFrom.ToolTip.GetToolTipText();
				}
				else
				{
					SetAttackName(power);
				}
			};
			base.AnimationPieceManager.Add(animClip);
		}

		private GUIButton genPowerMoveButton(string texturePath)
		{
			float d = (selectionSize != 0) ? 1f : 0.7f;
			GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(96f, 96f) * d, Vector2.zero);
			gUIButton.StyleInfo = new SHSButtonStyleInfo(texturePath);
			gUIButton.HitTestType = HitTestTypeEnum.Circular;
			gUIButton.HitTestSize = new Vector2(0.6875f, 0.6875f);
			return gUIButton;
		}

		private GUIImage genButtonSelectedImage(string texturePath)
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(256f, 256f), new Vector2(144f, 51f));
			gUIImage.TextureSource = texturePath;
			gUIImage.IsVisible = false;
			return gUIImage;
		}

		public void DisablePowerMovesBasedOnLevel(string heroName, int currentLevel)
		{
			HeroUsed = heroName;
			int num = 1;
			StatLevelReqsDefinition instance = StatLevelReqsDefinition.Instance;
			if (instance == null)
			{
				CspUtils.DebugLog("Can't update available r2 buttons without the leveling up rules!");
			}
			else
			{
				num = instance.GetMaxPowerAttackUnlockedAt(currentLevel);
			}
			PowerMove1.IsEnabled = (num >= 1);
			buttonSelectBackgrounds[0].IsVisible = (num >= 1);
			buttonCheckCircles[0].IsVisible = (num >= 1);
			PowerMove2.IsEnabled = (num >= 2);
			buttonSelectBackgrounds[1].IsVisible = (num >= 2);
			buttonCheckCircles[1].IsVisible = (num >= 2);
			PowerMove3.IsEnabled = (num >= 3);
			buttonSelectBackgrounds[2].IsVisible = (num >= 3);
			buttonCheckCircles[2].IsVisible = (num >= 3);
		}

		private void AssignButtonDelegates()
		{
			PowerMove1.Click += delegate
			{
				AppShell.Instance.Profile.LastSelectedPower = 1;
				SelectPower(0);
			};
			PowerMove2.Click += delegate
			{
				AppShell.Instance.Profile.LastSelectedPower = 2;
				SelectPower(1);
			};
			PowerMove3.Click += delegate
			{
				AppShell.Instance.Profile.LastSelectedPower = 3;
				SelectPower(2);
			};
		}
	}

	public class BrawlerRAttacksNameLoader
	{
		public class ButtonHolder
		{
			private GUIButton button1;

			private GUIButton button2;

			private GUIButton button3;

			public ButtonHolder(BrawlerPowerSelection powerSelection)
			{
				button1 = powerSelection.PowerMove1;
				button2 = powerSelection.PowerMove2;
				button3 = powerSelection.PowerMove3;
			}

			public void SetupTooltips(PowerAttackData pad)
			{
				button1.ToolTip = new NamedToolTipInfo(pad.displayName[0]);
				button2.ToolTip = new NamedToolTipInfo(pad.displayName[1]);
				button3.ToolTip = new NamedToolTipInfo(pad.displayName[2]);
			}
		}

		private Dictionary<string, PowerAttackData> characterR2Info;

		private Dictionary<string, ButtonHolder> callback;

		public BrawlerRAttacksNameLoader()
		{
			callback = new Dictionary<string, ButtonHolder>();
			characterR2Info = new Dictionary<string, PowerAttackData>();
		}

		public void SetupTooltips(string heroName, BrawlerPowerSelection powerSelection)
		{
			ButtonHolder buttonHolder = new ButtonHolder(powerSelection);
			if (characterR2Info.ContainsKey(heroName))
			{
				buttonHolder.SetupTooltips(characterR2Info[heroName]);
				return;
			}
			if (callback.ContainsKey(heroName))
			{
				callback.Remove(heroName);
			}
			callback.Add(heroName, buttonHolder);
			UserProfile profile = AppShell.Instance.Profile;
			if (profile.AvailableCostumes.ContainsKey(heroName))
			{
				AppShell.Instance.DataManager.LoadGameData("Characters/" + heroName, OnCharacterDataLoaded, profile.AvailableCostumes[heroName]);
			}
			else
			{
				CspUtils.DebugLog("you have tried to select a character you do not own.  bad on you!");
			}
		}

		public bool ToolTipsLoadedForHero(string heroName)
		{
			return characterR2Info.ContainsKey(heroName);
		}

		private void PreformCallback(string heroName)
		{
			if (callback.ContainsKey(heroName))
			{
				callback[heroName].SetupTooltips(characterR2Info[heroName]);
				callback.Remove(heroName);
			}
		}

		private void OnCharacterDataLoaded(GameDataLoadResponse response, object extraData)
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
				string displayName = string.Format("#RIGHTCLICK{0}_{1}", (i + 1).ToString(), text.ToUpper());
				characterR2Info[text].StoreAttack(i, name, displayName);
			}
			PreformCallback(text);
		}
	}

	private class BrawlerTopWindow : GadgetTopWindow
	{
		public BrawlerTopWindow(float x, float y, string simplePath, ToolTipInfo toolTipInfo)
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(592f, 141f), new Vector2(0f, 10f));
			gUIImage.TextureSource = "persistent_bundle|gadget_topmodule";
			Add(gUIImage);
			GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(x, y), Vector2.zero);
			gUIImage2.TextureSource = simplePath;
			gUIImage2.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
			gUIImage2.ToolTip = toolTipInfo;
			Add(gUIImage2);
		}

		public BrawlerTopWindow(float x, float y, string simplePath)
			: this(x, y, simplePath, NoToolTipInfo.Instance)
		{
		}
	}

	public enum BrawlerWindows
	{
		MissionLaunch,
		InviteFriends,
		ChooseAHero,
		ChooseAMission
	}

	public enum PlayMode
	{
		PlaySolo,
		PlayWithFriends,
		PlayDailyMission
	}

	private bool beginMissionFired = false;  // CSP added initializization of this variable;

	private BrawlerMissionLaunchWindow MissionLaunchWindow;

	private BrawlerInviteFriendsWindow InviteFriendsWindow;

	private BrawlerChooseAHeroWindow ChooseAHeroWindow;

	private BrawlerChooseAMissionWindow ChooseAMissionWindow;

	private BrawlerTopWindow MissionLaunchTopWindow;

	private BrawlerTopWindow InviteFriendsTopWindow;

	private BrawlerTopWindow ChooseAHeroTopWindow;

	private BrawlerTopWindow ChooseAMissionTopWindow;

	private BrawlerRAttacksNameLoader nameLoader;

	private BrawlerPlayerDataManager DataManager;

	public SHSBrawlerGadget()
	{
		DataManager = new BrawlerPlayerDataManager(this);
		nameLoader = new BrawlerRAttacksNameLoader();
		MissionLaunchWindow = new BrawlerMissionLaunchWindow(this);
		InviteFriendsWindow = new BrawlerInviteFriendsWindow(this);
		ChooseAHeroWindow = new BrawlerChooseAHeroWindow(this);
		ChooseAMissionWindow = new BrawlerChooseAMissionWindow(this);
		MissionLaunchTopWindow = new BrawlerTopWindow(261f, 47f, "brawlergadget_bundle|L_title_pick_mission");
		InviteFriendsTopWindow = new BrawlerTopWindow(241f, 44f, "persistent_bundle|L_title_invitefriends");
		ChooseAHeroTopWindow = new BrawlerTopWindow(215f, 48f, "persistent_bundle|L_title_pick_a_hero", new NamedToolTipInfo("#airlock_pick_a_hero_tt"));
		ChooseAMissionTopWindow = new BrawlerTopWindow(243f, 45f, "brawlergadget_bundle|L_title_pick_mission");
		SetupOpeningWindow(BackgroundType.OnePanel, MissionLaunchWindow);
		SetBackgroundImage("persistent_bundle|gadget_mainwindow_frame");
		SetupOpeningTopWindow(MissionLaunchTopWindow);
	}

	public void PlaySolo()
	{
		AppShell.Instance.QueueLocationInfo();
		ActiveMission activeMission = new ActiveMission(DataManager.SelectedMissionKey);
		activeMission.BecomeActiveMission();
		AppShell.Instance.Matchmaker2.SoloBrawler(DataManager.SelectedMissionKey, PlayBeginCallback);
	}

	public void PlayWithFriends()
	{
		AppShell.Instance.QueueLocationInfo();
		AppShell.Instance.Matchmaker2.InviteBrawler(DataManager.SelectedMissionKey, DataManager.SelectedInvitees, PlayBeginCallback);
	}

	private void PlayBeginCallback(Matchmaker2.Ticket ticket)
	{
		//CspUtils.DebugLog(ticket.ticket);   // commented out by CSP for testing.
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(ticket.ticket);
		XmlNode xmlNode = xmlDocument.SelectSingleNode("//mission");
		if (xmlNode == null)
		{
			CspUtils.DebugLog("Brawler ticket does not contain the mission: " + ticket.ticket);
		}
		else if (ticket.status == Matchmaker2.Ticket.Status.SUCCESS)
		{
			CspUtils.DebugLog("ticket local set to true " + AppShell.Instance.ServerConnection.GetGameUserId() + " and host " + AppShell.Instance.ServerConnection.GetGameHostId());
			ticket.local = true;
			AppShell.Instance.SharedHashTable["BrawlerTicket"] = ticket;
			ActiveMission activeMission = new ActiveMission(xmlNode.InnerText);
			activeMission.BecomeActiveMission();
			AppShell.Instance.Transition(GameController.ControllerType.Brawler);

			CloseGadget();  // added by CSP for testing.
		}
		else
		{
			GUIDialogNotificationSink notificationSink = new GUIDialogNotificationSink(delegate
			{
				MissionLaunchTopWindow.Hide();
				MissionLaunchWindow.Hide();
				Hide();
			});
			SHSErrorNotificationWindow sHSErrorNotificationWindow = new SHSErrorNotificationWindow(SHSErrorNotificationWindow.ErrorIcons.MissionNoGo);
			sHSErrorNotificationWindow.TitleText = "#error_oops";
			sHSErrorNotificationWindow.Text = "#missioninvite_error";
			sHSErrorNotificationWindow.NotificationSink = notificationSink;
			GUIManager.Instance.ShowDynamicWindow(sHSErrorNotificationWindow, ModalLevelEnum.Full);
		}
	}

	public void PlayWithAnyone()
	{
		AppShell.Instance.QueueLocationInfo();
		ActiveMission activeMission = new ActiveMission(DataManager.SelectedMissionKey);
		activeMission.BecomeActiveMission();
		AppShell.Instance.Matchmaker2.AnyoneBrawler(PlayBeginCallback, DataManager.SelectedMissionKey);
	}

	public void GoToWindow(BrawlerWindows win)
	{
		switch (win)
		{
		case BrawlerWindows.ChooseAHero:
			SetCenterWindow(ChooseAHeroWindow);
			SetTopWindow(ChooseAHeroTopWindow);
			break;
		case BrawlerWindows.ChooseAMission:
			SetCenterWindow(ChooseAMissionWindow);
			SetTopWindow(ChooseAMissionTopWindow);
			break;
		case BrawlerWindows.InviteFriends:
			SetCenterWindow(InviteFriendsWindow);
			SetTopWindow(InviteFriendsTopWindow);
			break;
		case BrawlerWindows.MissionLaunch:
			SetCenterWindow(MissionLaunchWindow);
			SetTopWindow(MissionLaunchTopWindow);
			break;
		}
	}

	public void ConfigureOnFriendsInvite(params Friend[] invitees)
	{
		DataManager.SelectedPlayMode = PlayMode.PlayWithFriends;
		DataManager.PlayModeClicked = true;
		InviteFriendsWindow.PreSelectFriendsAndTempFriends(invitees);
		MissionLaunchWindow.UpdateSelectedButton(PlayMode.PlayWithFriends);
		DataManager.SelectedInvitees = ExtractStrings(invitees);
	}

	private string[] ExtractStrings(Friend[] friends)
	{
		List<string> list = new List<string>();
		foreach (Friend friend in friends)
		{
			list.Add(friend.Id.ToString());
		}
		return list.ToArray();
	}

	public void PlaySoloClicked()
	{
		DataManager.SelectedPlayMode = PlayMode.PlaySolo;
		DataManager.PlayModeClicked = true;
		VOManager.Instance.PlayVO("mission_solo", VOInputString.FromStrings(DataManager.SelectedHero));
	}

	public void PlayWithFriendsClicked()
	{
		AppShell.Instance.Profile.AvailableFriends.ReloadFriendList();  // added by CSP
		DataManager.SelectedPlayMode = PlayMode.PlayWithFriends;
		DataManager.PlayModeClicked = true;
		SetBackgroundImage("persistent_bundle|gadget_base_one_panel");
		GoToWindow(BrawlerWindows.InviteFriends);
		VOManager.Instance.PlayVO("mission_multi", VOInputString.FromStrings(DataManager.SelectedHero));
	}

	public void PlayDailyMissionClicked()
	{
		DataManager.SelectedPlayMode = PlayMode.PlayDailyMission;
		DataManager.SelectedMissionKey = DataManager.DailyMissionKey;
		DataManager.PlayModeClicked = true;
		MissionLaunchWindow.UpdateSelectedMission();
		VOManager.Instance.PlayVO("mission_multi", VOInputString.FromStrings(DataManager.SelectedHero));
	}

	public void ChooseAHeroClicked()
	{
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("large_click_down"));
		SetBackgroundImage("options_bundle|options_gadget_frame");
		GoToWindow(BrawlerWindows.ChooseAHero);
	}

	public void ChooseAMissionClicked()
	{
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("large_click_down"));
		GoToWindow(BrawlerWindows.ChooseAMission);
	}

	public void ChooseAHeroOkButtonClicked()
	{
		SetBackgroundImage("persistent_bundle|gadget_mainwindow_frame");
		GoToWindow(BrawlerWindows.MissionLaunch);
	}

	public void BackWasClicked()
	{
		SetBackgroundImage("persistent_bundle|gadget_mainwindow_frame");
		GoToWindow(BrawlerWindows.MissionLaunch);
	}

	public void MissionClicked(string missionKey)
	{
		DataManager.SelectedMissionKey = missionKey;

		DataManager.DailyMissionKey = DataManager.SelectedMissionKey; // CSP - temporarily always make seelcted misison daily mission

		MissionLaunchWindow.UpdateSelectedMission();
		if (!DataManager.PlayModeClicked)
		{
			if (DataManager.SelectedMissionKey == DataManager.DailyMissionKey)
			{
				DataManager.SelectedPlayMode = PlayMode.PlayDailyMission;
			}
			else
			{
				DataManager.SelectedPlayMode = PlayMode.PlaySolo;
			}
		}
		else if (DataManager.SelectedMissionKey != DataManager.DailyMissionKey && DataManager.SelectedPlayMode == PlayMode.PlayDailyMission)
		{
			DataManager.SelectedPlayMode = PlayMode.PlaySolo;
			DataManager.PlayModeClicked = true;
		}
		VOManager.Instance.PlayVO("mission_name", new VOInputString(missionKey));
		SetBackgroundImage("persistent_bundle|gadget_mainwindow_frame");
		GoToWindow(BrawlerWindows.MissionLaunch);
	}

	public void FriendsToPlayWithSubmitted(string[] invitees)
	{
		DataManager.SelectedInvitees = invitees;
		SetBackgroundImage("persistent_bundle|gadget_mainwindow_frame");
		GoToWindow(BrawlerWindows.MissionLaunch);
	}

	public void NoFriendsToPlayWithSubmitted()
	{
		DataManager.SelectedPlayMode = PlayMode.PlaySolo;
		SetBackgroundImage("persistent_bundle|gadget_mainwindow_frame");
		GoToWindow(BrawlerWindows.MissionLaunch);
	}

	public void GoButtonClicked()
	{

		CspUtils.DebugLog("GoButtonClicked!");
		if (!beginMissionFired)
		{
			CspUtils.DebugLog("GoButtonClicked - checking for banned hero selection: " + DataManager.SelectedHero);
			beginMissionFired = true;
			loadMissionData(DataManager.SelectedMissionKey);
		}		
	}

	public void loadMissionData(string missionID)
	{
		//CspUtils.DebugLog("lmd missionID=" + missionID); // CSP
		AppShell.Instance.DataManager.LoadGameData("Missions/" + missionID, OnMissionDefinitionLoaded, new MissionDefinition());
	}

	protected void OnMissionDefinitionLoaded(GameDataLoadResponse response, object extraData)
	{
		CspUtils.DebugLog("GoButtonClicked - mission data loaded");
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("BrawlerTransitionAndInfoHandler: The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			return;
		}
		MissionDefinition missionDefinition = response.DataDefinition as MissionDefinition;
		bool flag = false;
		CspUtils.DebugLog("  checking for banned hero data");
		foreach (string blacklistCharacter in missionDefinition.blacklistCharacters)
		{
			CspUtils.DebugLog("  checking " + blacklistCharacter);
			if (blacklistCharacter == DataManager.SelectedHero)
			{
				flag = true;
			}
			if (blacklistCharacter == "non_villains" && !AppShell.Instance.CharacterDescriptionManager.VillainList.Contains(DataManager.SelectedHero))
			{
				CspUtils.DebugLogWarning("This mission only allows villains, and " + DataManager.SelectedHero + " is not a villain, check their XML.");
				flag = true;
			}
		}
		if (flag)
		{
			CspUtils.DebugLog("GoButtonClicked - Banned Hero Detected!");
			SHSErrorNotificationWindow sHSErrorNotificationWindow = new SHSErrorNotificationWindow(SHSErrorNotificationWindow.ErrorIcons.MissionNoGo);
			sHSErrorNotificationWindow.TitleText = "#error_oops";
			sHSErrorNotificationWindow.Text = "#BRAWLER_BANNED_HERO_NOTIF";
			GUIManager.Instance.ShowDynamicWindow(sHSErrorNotificationWindow, ModalLevelEnum.Full);
			beginMissionFired = false;
			return;
		}
		CspUtils.DebugLog("GoButtonClicked - Hero is ok!");
		AppShell.Instance.Profile.LastSelectedCostume = DataManager.SelectedHero;
		AppShell.Instance.Profile.LastSelectedPower = DataManager.SelectedPower;
		AppShell.Instance.SharedHashTable["BrawlerAirlockPlaySolo"] = (DataManager.SelectedPlayMode == PlayMode.PlaySolo);
		AppShell.Instance.SharedHashTable["BrawlerAirlockReturnsToGadget"] = true;
		switch (DataManager.SelectedPlayMode)
		{
		case PlayMode.PlaySolo:
			PlaySolo();
			break;
		case PlayMode.PlayWithFriends:
			PlayWithFriends();
			break;
		case PlayMode.PlayDailyMission:
			PlayWithAnyone();
			break;
		}
	}

	public void SetBrawlerCloseData(CustomCloseData closeData)
	{
		MissionLaunchWindow.CloseData = closeData;
		InviteFriendsWindow.CloseData = closeData;
		ChooseAHeroWindow.CloseData = closeData;
		ChooseAMissionWindow.CloseData = closeData;
	}

	public override void OnShow()
	{
		base.OnShow();
		AppShell.Instance.EventMgr.AddListener<FriendListUpdatedMessage>(InviteFriendsWindow.OnFriendListUpdated);
		AppShell.Instance.EventMgr.AddListener<CollectionAddedMessage>(AppShell.Instance.Profile.AvailableCostumes, OnCollectionAddedMessage);
	}

	public override void OnHide()
	{
		base.OnHide();
		AppShell.Instance.EventMgr.RemoveListener<FriendListUpdatedMessage>(InviteFriendsWindow.OnFriendListUpdated);
		AppShell.Instance.EventMgr.RemoveListener<CollectionAddedMessage>(AppShell.Instance.Profile.AvailableCostumes, OnCollectionAddedMessage);
	}

	public void OnCollectionAddedMessage(CollectionAddedMessage msg)
	{
		ChooseAHeroWindow.UpdateCharacterSelect();
	}
}
