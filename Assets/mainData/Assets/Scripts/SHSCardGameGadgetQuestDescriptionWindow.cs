using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSCardGameGadgetQuestDescriptionWindow : GUISimpleControlWindow
{
	public class GrowAndShrinkCard : SHSAnimations
	{
		public static AnimClip GrowCard(GUIControl ctrl)
		{
			Vector2 tINY_SIZE_CARD_SIZE = TINY_SIZE_CARD_SIZE;
			float x = tINY_SIZE_CARD_SIZE.x;
			Vector2 fULL_SIZE_CARD_SIZE = FULL_SIZE_CARD_SIZE;
			float x2 = fULL_SIZE_CARD_SIZE.x;
			Vector2 size = ctrl.Size;
			float time = GenericFunctions.FrationalTime(x, x2, size.x, 0.3f);
			Vector2 size2 = ctrl.Size;
			float x3 = size2.x;
			Vector2 fULL_SIZE_CARD_SIZE2 = FULL_SIZE_CARD_SIZE;
			AnimClip pieceOne = Absolute.SizeX(GenericPaths.LinearWithWiggle(x3, fULL_SIZE_CARD_SIZE2.x, time), ctrl);
			Vector2 size3 = ctrl.Size;
			float y = size3.y;
			Vector2 fULL_SIZE_CARD_SIZE3 = FULL_SIZE_CARD_SIZE;
			AnimClip pieceOne2 = pieceOne ^ Absolute.SizeY(GenericPaths.LinearWithWiggle(y, fULL_SIZE_CARD_SIZE3.y, time), ctrl);
			Vector2 offset = ctrl.Offset;
			float x4 = offset.x;
			Vector2 fULL_SIZE_CARD_OFFSET = FULL_SIZE_CARD_OFFSET;
			AnimClip pieceOne3 = pieceOne2 ^ Absolute.OffsetX(GenericPaths.LinearWithWiggle(x4, fULL_SIZE_CARD_OFFSET.x, time), ctrl);
			Vector2 offset2 = ctrl.Offset;
			float y2 = offset2.y;
			Vector2 fULL_SIZE_CARD_OFFSET2 = FULL_SIZE_CARD_OFFSET;
			return pieceOne3 ^ Absolute.OffsetY(GenericPaths.LinearWithWiggle(y2, fULL_SIZE_CARD_OFFSET2.y, time), ctrl) ^ Absolute.Rotation(GenericPaths.LinearWithWiggle(ctrl.Rotation, 0f, time), ctrl);
		}

		public static AnimClip ShrinkCard(GUIControl ctrl)
		{
			Vector2 fULL_SIZE_CARD_SIZE = FULL_SIZE_CARD_SIZE;
			float x = fULL_SIZE_CARD_SIZE.x;
			Vector2 tINY_SIZE_CARD_SIZE = TINY_SIZE_CARD_SIZE;
			float x2 = tINY_SIZE_CARD_SIZE.x;
			Vector2 size = ctrl.Size;
			float time = GenericFunctions.FrationalTime(x, x2, size.x, 0.3f);
			Vector2 size2 = ctrl.Size;
			float x3 = size2.x;
			Vector2 tINY_SIZE_CARD_SIZE2 = TINY_SIZE_CARD_SIZE;
			AnimClip pieceOne = Absolute.SizeX(Path.Linear(x3, tINY_SIZE_CARD_SIZE2.x, time), ctrl);
			Vector2 size3 = ctrl.Size;
			float y = size3.y;
			Vector2 tINY_SIZE_CARD_SIZE3 = TINY_SIZE_CARD_SIZE;
			AnimClip pieceOne2 = pieceOne ^ Absolute.SizeY(Path.Linear(y, tINY_SIZE_CARD_SIZE3.y, time), ctrl);
			Vector2 offset = ctrl.Offset;
			float x4 = offset.x;
			Vector2 tINY_SIZE_CARD_OFFSET = TINY_SIZE_CARD_OFFSET;
			AnimClip pieceOne3 = pieceOne2 ^ Absolute.OffsetX(Path.Linear(x4, tINY_SIZE_CARD_OFFSET.x, time), ctrl);
			Vector2 offset2 = ctrl.Offset;
			float y2 = offset2.y;
			Vector2 tINY_SIZE_CARD_OFFSET2 = TINY_SIZE_CARD_OFFSET;
			return pieceOne3 ^ Absolute.OffsetY(Path.Linear(y2, tINY_SIZE_CARD_OFFSET2.y, time), ctrl) ^ Absolute.Rotation(Path.Linear(ctrl.Rotation, 12f, time), ctrl);
		}
	}

	public class FadeInOut : SHSAnimations
	{
		public static AnimClip FadeIn(GUIControl ctrl)
		{
			return Absolute.Alpha(Path.Linear(ctrl.Alpha, 1f, 0.25f), ctrl);
		}

		public static AnimClip FadeOut(GUIControl ctrl)
		{
			return Absolute.Alpha(Path.Linear(ctrl.Alpha, 0f, 0.25f), ctrl);
		}
	}

	public class QuestNodeWindow : GUISimpleControlWindow
	{
		private readonly SHSCardGameGadgetQuestDescriptionWindow parentWindow;

		public CardQuestPart.QuestBattle questBattle;

		public QuestNodeWindow(CardQuestPart questPart, CardQuestPart.QuestBattle questBattle, SHSCardGameGadgetQuestDescriptionWindow parentWindow)
		{
			this.parentWindow = parentWindow;
			this.questBattle = questBattle;
			SetSize(500f, 500f);
			SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f));
			GUIDropShadowTextLabel gUIDropShadowTextLabel = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(500f, 60f), new Vector2(20f, -98f));
			gUIDropShadowTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 37, GUILabel.GenColor(247, 201, 94), GUILabel.GenColor(0, 0, 0), new Vector2(-2f, 2f), TextAnchor.MiddleLeft);
			gUIDropShadowTextLabel.Text = questPart.Subname1;
			gUIDropShadowTextLabel.WordWrap = false;
			Add(gUIDropShadowTextLabel);
			GUILabel gUILabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(500f, 60f), new Vector2(19f, -79f));
			gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(193, 255, 63), TextAnchor.MiddleLeft);
			gUILabel.Text = questBattle.Name;
			gUILabel.WordWrap = false;
			Add(gUILabel);
			string text = string.Empty;
			string text2 = string.Empty;
			CardGameLaunchManager.QuestUserState questState = parentWindow.parentWindow.mainWindow.LaunchManager.GetQuestState(questBattle);
			switch (questState)
			{
			case CardGameLaunchManager.QuestUserState.Unlocked:
			case CardGameLaunchManager.QuestUserState.DailyQuest:
				text = questBattle.Description;
				text2 = questBattle.RulesText;
				break;
			case CardGameLaunchManager.QuestUserState.Locked:
				text = "#CGG_BATTLELOCKED";
				text2 = "#CGG_RULESLOCKED";
				break;
			}
			if (questState != 0)
			{
				GUISimpleControlWindow gUISimpleControlWindow = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(300f, 300f), new Vector2(95f, 112f));
				gUISimpleControlWindow.Id = "battleDescription";
				GUILabel gUILabel2 = GUIControl.CreateControlTopLeftFrame<GUILabel>(new Vector2(121f, 60f), new Vector2(0f, 0f));
				gUILabel2.Id = "StoryWord";
				gUILabel2.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 24, GUILabel.GenColor(22, 71, 174), TextAnchor.UpperLeft);
				gUILabel2.Text = "#CGG_STORY_TITLE";
				gUISimpleControlWindow.Add(gUILabel2);
				GUILabel gUILabel3 = GUIControl.CreateControlTopLeftFrame<GUILabel>(new Vector2(248f, 150f), new Vector2(0f, 15f));
				gUILabel3.Id = "StoryText";
				gUILabel3.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(58, 71, 94), TextAnchor.UpperLeft);
				gUILabel3.VerticalKerning = 12;
				gUILabel3.Text = text;
				gUISimpleControlWindow.Add(gUILabel3);
				Vector2 size = new Vector2(121f, 20f);
				float num = gUILabel3.TotalTextHeight;
				Vector2 offset = gUILabel3.Offset;
				GUILabel gUILabel4 = GUIControl.CreateControlTopLeftFrame<GUILabel>(size, new Vector2(0f, num + offset.y));
				gUILabel4.Id = "RulesWord";
				gUILabel4.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 24, GUILabel.GenColor(22, 71, 174), TextAnchor.UpperLeft);
				gUILabel4.Text = "#CGG_RULES_TITLE";
				gUISimpleControlWindow.Add(gUILabel4);
				Vector2 size2 = new Vector2(180f, 97f);
				Vector2 offset2 = gUILabel4.Offset;
				float y = offset2.y;
				Vector2 size3 = gUILabel4.Size;
				GUILabel gUILabel5 = GUIControl.CreateControlTopLeftFrame<GUILabel>(size2, new Vector2(0f, y + size3.y));
				gUILabel5.Id = "RulesText";
				gUILabel5.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(58, 71, 94), TextAnchor.UpperLeft);
				gUILabel5.VerticalKerning = 12;
				gUILabel5.Text = text2;
				gUISimpleControlWindow.Add(gUILabel5);
				Add(gUISimpleControlWindow);
			}
			else
			{
				GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(210f, 145f), new Vector2(72f, 39f));
				gUIButton.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|L_buyit_button");
				gUIButton.HitTestSize = new Vector2(0.87f, 0.79f);
				Add(gUIButton);
				gUIButton.Click += delegate
				{
					ShoppingWindow shoppingWindow = new ShoppingWindow(questBattle.ParentCardQuestPart.Id);
					shoppingWindow.launch();
				};
			}
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(183f, 183f), new Vector2(-153f, 27f));
			gUIImage.TextureSource = "characters_bundle|expandedtooltip_render_" + questPart.Sponsor;
			Add(gUIImage);
			GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(134f, 140f), new Vector2(189f, 136f));
			gUIImage2.TextureSource = "cardgamegadget_bundle|L_cardlauncher_prizecontainer";
			Add(gUIImage2);
		}

		public override void OnShow()
		{
			CardGameLaunchManager launchManager = parentWindow.parentWindow.mainWindow.LaunchManager;
			parentWindow.SetSelectedQuestNode(questBattle, launchManager.GetQuestState(questBattle) == CardGameLaunchManager.QuestUserState.Unlocked || launchManager.GetQuestState(questBattle) == CardGameLaunchManager.QuestUserState.DailyQuest);
			base.OnShow();
		}
	}

	public class TabWindowManager
	{
		public class AnimateTab : SHSAnimations
		{
			public const float ANIM_TIME = 0.35f;

			public static AnimClip AnimTab(int tabToBeSetOn, int curSelectedTab, TabWindow frontTabs, TabWindow backTabs, SHSCardGameGadgetQuestDescriptionWindow headWindow, List<GUIImage> bkgGlow)
			{
				AnimClip animClip = Generic.Blank();
				for (int i = 0; i < headWindow.nodeWindowList.Count; i++)
				{
					bool flag = tabToBeSetOn == i;
					if (flag)
					{
						frontTabs.tabs[i].Alpha = 1f;
						bkgGlow[i].Alpha = 1f;
						animClip = (animClip ^ Flip(frontTabs.tabs[i], true, true) ^ Flip(bkgGlow[i], true, true) ^ Flip(backTabs.tabs[i], false, false));
					}
					else
					{
						backTabs.tabs[i].SetSize(93f, 117f);
						TabWindow.Tab tab = backTabs.tabs[i];
						Vector2 offset = backTabs.tabs[i].Offset;
						tab.Offset = new Vector2(offset.x, 0f);
						backTabs.tabs[i].IsVisible = true;
						animClip = (animClip ^ Fade(frontTabs.tabs[i], false) ^ Fade(bkgGlow[i], false) ^ Fade(backTabs.tabs[i], true));
					}
					animClip ^= FadeWin(headWindow.nodeWindowList[i], flag);
				}
				return animClip;
			}

			public static AnimClip Flip(GUIControl tab, bool flipOpen, bool delay)
			{
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f8: Expected O, but got Unknown
				float num = (!flipOpen) ? 117 : 0;
				float num2 = flipOpen ? (-20) : 0;
				AnimClip animClip = Absolute.SizeY(Path.Constant(num, (!delay) ? 0f : 0.175f) | Path.Linear(num, flipOpen ? 117 : 0, 0.175f), tab) ^ Absolute.OffsetY(Path.Constant(num2, (!delay) ? 0f : 0.175f) | Path.Linear(num2, (!flipOpen) ? (-20) : 0, 0.175f), tab);
				if (flipOpen)
				{
					tab.IsVisible = true;
				}
				else
				{
					animClip.OnFinished += (Action)(object)(Action)delegate
					{
						tab.IsVisible = false;
					};
				}
				return animClip;
			}

			public static AnimClip Fade(GUIControl tab, bool fadeIn)
			{
				return (!fadeIn) ? Generic.FadeOut(tab, 0.35f) : Generic.FadeIn(tab, 0.35f);
			}

			public static AnimClip FadeWin(GUIControl win, bool fadeIn)
			{
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Expected O, but got Unknown
				if (fadeIn)
				{
					AnimClip result = Absolute.Alpha(Path.Sin(0f, 0.25f, 0.35f), win);
					win.IsVisible = true;
					return result;
				}
				AnimClip animClip = Absolute.Alpha(Path.Cos(0f, 0.25f, 0.35f), win);
				animClip.OnFinished += (Action)(object)(Action)delegate
				{
					win.IsVisible = false;
				};
				return animClip;
			}
		}

		public class TabWindow : GUISimpleControlWindow
		{
			public enum Ground
			{
				Foreground,
				Background
			}

			public class Tab : GUISubScalingWindow
			{
				private GUIImage bkg2;

				private int stage;

				private CardQuestPart.QuestBattle questBattle;

				private GUIImage QuestIcon;

				private SHSCardGameGadgetQuestDescriptionWindow parentWindow;

				public event TabClicked OnTabClicked;

				public Tab(CardQuestPart.QuestBattle questBattle, int stage, bool OnOrOff, SHSCardGameGadgetQuestDescriptionWindow parentWindow)
					: base(93f, 117f)
				{
					this.questBattle = questBattle;
					this.parentWindow = parentWindow;
					this.stage = stage;
					SetSize(93f, 117f);
					SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
					Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
					GUIHotSpotButton gUIHotSpotButton = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(new Vector2(75f, 99f), new Vector2(0f, 0f));
					gUIHotSpotButton.Click += HotSpotClick;
					gUIHotSpotButton.MouseOut += HotSpotMouseOut;
					gUIHotSpotButton.MouseOver += HotSpotMouseOver;
					GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(75f, 99f), new Vector2(0f, 0f));
					gUIImage.TextureSource = "cardgamegadget_bundle|cardlauncher_quest_tab" + ((!OnOrOff) ? "OFF" : "ON");
					bkg2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(75f, 99f), new Vector2(0f, 0f));
					QuestIcon = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(93f, 93f), new Vector2(0f, 0f));
					QuestIcon.TextureSource = "characters_bundle|inventory_character_" + questBattle.Enemy + "_normal";
					CardGameLaunchManager launchManager = parentWindow.parentWindow.mainWindow.LaunchManager;
					bool flag = launchManager.GetQuestState(questBattle) == CardGameLaunchManager.QuestUserState.Unlocked || launchManager.GetQuestState(questBattle) == CardGameLaunchManager.QuestUserState.DailyQuest;
					bkg2.TextureSource = ((!flag) ? "cardgamegadget_bundle|cardlauncher_quest_tabLOCK" : ("cardgamegadget_bundle|cardlauncher_quest_lv" + questBattle.Rating + "bg"));
					QuestIcon.Alpha = ((!flag) ? 0.5f : 1f);
					AddItem(gUIHotSpotButton);
					AddItem(gUIImage);
					AddItem(bkg2);
					AddItem(QuestIcon);
					if (flag)
					{
						GUIImage gUIImage2 = GUIControl.CreateControlTopFrameCentered<GUIImage>(new Vector2(76f, 25f), new Vector2(0f, 22f));
						gUIImage2.TextureSource = "cardgamegadget_bundle|cardlauncher_quest_checkmarks_container";
						AddItem(gUIImage2);
						string[] array = questBattle.Reward.Split(';');
						if (array.Length == 1 || (array.Length > 1 && string.IsNullOrEmpty(array[1])))
						{
							string text = array[0];
							string[] array2 = text.Split(':');
							if (array2.Length == 2 && CardCollection.Collection.ContainsKey(array2[0]))
							{
								for (int i = 0; i < Math.Min(CardCollection.Collection[array2[0]], 4); i++)
								{
									GUIImage gUIImage3 = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(17f, 15f), new Vector2(17 * i + 13, 15f));
									gUIImage3.TextureSource = "cardgamegadget_bundle|cardlauncher_quest_checkmark";
									AddItem(gUIImage3);
								}
							}
						}
						else
						{
							CspUtils.DebugLog(string.Format("Quest Battle {0} has a non 1 quantity of card rewards. This should not be the case", questBattle.Id));
						}
					}
					if (launchManager.GetQuestState(questBattle) == CardGameLaunchManager.QuestUserState.DailyQuest)
					{
						GUIImage gUIImage4 = GUIControl.CreateControlBottomFrameCentered<GUIImage>(new Vector2(58f, 58f), new Vector2(25f, -32f));
						gUIImage4.TextureSource = "cardgamegadget_bundle|cardlauncher_quest_dailyquest_icon";
						AddItem(gUIImage4);
					}
				}

				public void OnCardQuestFetchComplete(CardQuestFetchCompleteMessage message)
				{
					CardGameLaunchManager launchManager = parentWindow.parentWindow.mainWindow.LaunchManager;
					bool flag = launchManager.GetQuestState(questBattle) == CardGameLaunchManager.QuestUserState.Unlocked || launchManager.GetQuestState(questBattle) == CardGameLaunchManager.QuestUserState.DailyQuest;
					bkg2.TextureSource = ((!flag) ? "cardgamegadget_bundle|cardlauncher_quest_tabLOCK" : ("cardgamegadget_bundle|cardlauncher_quest_lv" + questBattle.Rating + "bg"));
					QuestIcon.Alpha = ((!flag) ? 0.5f : 1f);
					if (!flag)
					{
						return;
					}
					GUIImage gUIImage = GUIControl.CreateControlTopFrameCentered<GUIImage>(new Vector2(76f, 25f), new Vector2(0f, 22f));
					gUIImage.TextureSource = "cardgamegadget_bundle|cardlauncher_quest_checkmarks_container";
					AddItem(gUIImage);
					string[] array = questBattle.Reward.Split(';');
					if (array.Length == 1 || (array.Length > 1 && string.IsNullOrEmpty(array[1])))
					{
						string text = array[0];
						string[] array2 = text.Split(':');
						if (array2.Length == 2 && CardCollection.Collection.ContainsKey(array2[0]))
						{
							for (int i = 0; i < Math.Min(CardCollection.Collection[array2[0]], 4); i++)
							{
								GUIImage gUIImage2 = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(17f, 15f), new Vector2(17 * i + 13, 15f));
								gUIImage2.TextureSource = "cardgamegadget_bundle|cardlauncher_quest_checkmark";
								AddItem(gUIImage2);
							}
						}
					}
					else
					{
						CspUtils.DebugLog(string.Format("Quest Battle {0} has a non 1 quantity of card rewards. This should not be the case", questBattle.Id));
					}
				}

				private void HotSpotMouseOver(GUIControl sender, GUIMouseEvent EventData)
				{
					ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("hover_over"));
					QuestIcon.TextureSource = "characters_bundle|inventory_character_" + questBattle.Enemy + "_highlight";
				}

				private void HotSpotMouseOut(GUIControl sender, GUIMouseEvent EventData)
				{
					QuestIcon.TextureSource = "characters_bundle|inventory_character_" + questBattle.Enemy + "_normal";
				}

				private void HotSpotClick(GUIControl sender, GUIClickEvent EventData)
				{
					if (this.OnTabClicked != null)
					{
						this.OnTabClicked(stage - 1);
					}
					ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("click_down"));
				}
			}

			public delegate void TabClicked(int i);

			public List<Tab> tabs = new List<Tab>();

			public TabWindow(CardQuest quest, Ground ground, TabClicked tabClicked, SHSCardGameGadgetQuestDescriptionWindow parentWindow)
			{
				SetSize(650f, 200f);
				SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-105f, -169f));
				foreach (CardQuestPart part in quest.Parts)
				{
					for (int i = 0; i < part.Nodes.Count; i++)
					{
						CardQuestPart.QuestBattle questBattle = part.Nodes[i];
						Tab tab = (ground != Ground.Background) ? new Tab(questBattle, questBattle.Stage, true, parentWindow) : new Tab(questBattle, questBattle.Stage, false, parentWindow);
						if (tabClicked != null)
						{
							tab.OnTabClicked += tabClicked;
						}
						tab.Offset = new Vector2(-99 + 75 * (questBattle.Stage - 1), 0f);
						Add(tab);
						tabs.Add(tab);
					}
				}
			}
		}

		public TabWindow frontTabs;

		public TabWindow backTabs;

		private SHSCardGameGadgetQuestDescriptionWindow parentWindow;

		private List<GUIImage> bkgGlow = new List<GUIImage>();

		private AnimClip curFlipAnimation;

		private int selectedTab;

		public int SelectedTab
		{
			get
			{
				return selectedTab;
			}
		}

		public TabWindowManager(CardQuest quest, SHSCardGameGadgetQuestDescriptionWindow parentWindow)
		{
			this.parentWindow = parentWindow;
			frontTabs = new TabWindow(quest, TabWindow.Ground.Foreground, OnTabClicked, parentWindow);
			backTabs = new TabWindow(quest, TabWindow.Ground.Background, OnTabClicked, parentWindow);
			foreach (TabWindow.Tab tab in backTabs.tabs)
			{
				GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(93f, 117f), new Vector2(0f, 0f));
				gUIImage.TextureSource = "cardgamegadget_bundle|cardlauncher_quest_tabLIGHT";
				gUIImage.Offset = tab.Offset;
				gUIImage.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
				backTabs.Add(gUIImage);
				bkgGlow.Add(gUIImage);
			}
		}

		public void OnTabClicked(int i)
		{
			SelectTab(i);
		}

		public void SetInitialTab(int tabToBeSetOn)
		{
			selectedTab = tabToBeSetOn;
			for (int i = 0; i < parentWindow.nodeWindowList.Count; i++)
			{
				bool flag = tabToBeSetOn == i;
				frontTabs.tabs[i].IsVisible = flag;
				bkgGlow[i].IsVisible = flag;
				backTabs.tabs[i].IsVisible = !flag;
				parentWindow.nodeWindowList[i].IsVisible = flag;
			}
		}

		public void SelectTab(int i)
		{
			SelectTab(i, false);
		}

		public void SelectTab(int i, bool force)
		{
			if (selectedTab != i || force)
			{
				parentWindow.AnimationPieceManager.SwapOut(ref curFlipAnimation, AnimateTab.AnimTab(i, selectedTab, frontTabs, backTabs, parentWindow, bkgGlow));
				selectedTab = i;
			}
		}
	}

	public const string CARD_GAME_TEXT_STORY = "#CGG_STORY_TITLE";

	public const string CARD_GAME_TEXT_RULES = "#CGG_RULES_TITLE";

	public const float FULL_SIZE_CARD_ROTATION = 0f;

	public const float TINY_SIZE_CARD_ROTATION = 12f;

	public const int MAX_WINS_PER_BATTLE = 4;

	public static readonly Vector2 FULL_SIZE_CARD_SIZE = new Vector2(366f, 512f) * 0.9f;

	public static readonly Vector2 TINY_SIZE_CARD_SIZE = new Vector2(48f, 67f);

	public static readonly Vector2 FULL_SIZE_CARD_OFFSET = new Vector2(0f, 0f);

	public static readonly Vector2 TINY_SIZE_CARD_OFFSET = new Vector2(191f, 150f);

	public List<QuestNodeWindow> nodeWindowList;

	public TabWindowManager tabWindowManager;

	private GUISimpleControlWindow PrizeCardWindow;

	private GUIImage PrizeCard;

	private GUIImage CardBG;

	private GUIImage CardLock;

	private GUIButton BuyButton;

	private GUIHotSpotButton expandHotspot;

	private GUIHotSpotButton shrinkHotspot;

	private GUIButton OkButton;

	private CardQuestPart.QuestBattle SelectedQuestBattle;

	private SHSCardGameGadgetPickAQuestWindow parentWindow;

	private CardQuest quest;

	private bool prizeCardUpdatePending;

	private string prizeIdPending;

	private AnimClip fadeBuyButton;

	private AnimClip growAndShrink;

	private AnimClip fadeLockImage;

	private bool IsDailyBattleAndNotOwned
	{
		get
		{
			if (AppShell.Instance != null && SelectedQuestBattle != null && parentWindow.mainWindow.LaunchManager.GetQuestState(SelectedQuestBattle) == CardGameLaunchManager.QuestUserState.DailyQuest && !AppShell.Instance.Profile.AvailableQuests.ContainsKey(SelectedQuestBattle.ParentCardQuestPart.Id.ToString()))
			{
				return true;
			}
			return false;
		}
	}

	public SHSCardGameGadgetQuestDescriptionWindow(CardQuest quest, SHSCardGameGadgetPickAQuestWindow parentWindow)
	{
		this.parentWindow = parentWindow;
		this.quest = quest;
		tabWindowManager = new TabWindowManager(quest, this);
		Add(tabWindowManager.backTabs);
		GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(485f, 328f), new Vector2(-16f, 24f));
		gUIImage.TextureSource = "cardgamegadget_bundle|cardlauncher_quest_right_container";
		Add(gUIImage);
		PopulateAndAddQuestNodes(quest);
		Add(tabWindowManager.frontTabs);
		AddButtons();
		PopulateAndAddPrizeCard();
	}

	public override void OnShow()
	{
		base.OnShow();
		CardGameLaunchManager launchManager = parentWindow.mainWindow.LaunchManager;
		bool flag = false;
		int initialTab = 0;
		for (int i = 0; i < nodeWindowList.Count; i++)
		{
			CardQuestPart.QuestBattle questBattle = nodeWindowList[i].questBattle;
			if (launchManager.SelectedBattle != null && questBattle.Id == launchManager.SelectedBattle.Id)
			{
				initialTab = i;
				break;
			}
			if (launchManager.GetQuestState(questBattle) == CardGameLaunchManager.QuestUserState.DailyQuest)
			{
				initialTab = i;
				flag = true;
			}
			bool flag2 = launchManager.GetQuestState(questBattle) == CardGameLaunchManager.QuestUserState.Unlocked;
			if (flag || !flag2)
			{
				continue;
			}
			string[] array = questBattle.Reward.Split(';');
			if (array.Length != 1 && (array.Length <= 1 || !string.IsNullOrEmpty(array[1])))
			{
				continue;
			}
			string text = array[0];
			string[] array2 = text.Split(':');
			if (array2.Length != 2)
			{
				continue;
			}
			if (CardCollection.Collection.ContainsKey(array2[0]))
			{
				int num = CardCollection.Collection[array2[0]];
				if (num < 4)
				{
					initialTab = i;
					flag = true;
				}
			}
			else
			{
				initialTab = i;
				flag = true;
			}
		}
		tabWindowManager.SetInitialTab(initialTab);
		AppShell.Instance.EventMgr.AddListener<CardQuestFetchCompleteMessage>(OnCardQuestFetchComplete);
	}

	public override void OnHide()
	{
		AppShell.Instance.EventMgr.RemoveListener<CardQuestFetchCompleteMessage>(OnCardQuestFetchComplete);
		base.OnHide();
	}

	private void OnCardQuestFetchComplete(CardQuestFetchCompleteMessage message)
	{
		PopulateAndAddQuestNodes(quest);
		foreach (TabWindowManager.TabWindow.Tab tab in tabWindowManager.frontTabs.tabs)
		{
			tab.OnCardQuestFetchComplete(message);
		}
		foreach (TabWindowManager.TabWindow.Tab tab2 in tabWindowManager.backTabs.tabs)
		{
			tab2.OnCardQuestFetchComplete(message);
		}
		if (PrizeCardWindow != null)
		{
			ControlToFront(PrizeCardWindow);
			ControlToFront(expandHotspot);
		}
		else
		{
			PopulateAndAddPrizeCard();
		}
		tabWindowManager.SelectTab(tabWindowManager.SelectedTab, true);
	}

	private void OkButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		parentWindow.QuestBattleSelected(SelectedQuestBattle);
	}

	private void BackButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		parentWindow.QuestBattleCancel();
	}

	private void QuitButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		parentWindow.QuestBattleQuit();
	}

	public void PopulateAndAddQuestNodes(CardQuest quest)
	{
		if (nodeWindowList != null)
		{
			nodeWindowList.ForEach(Remove);
			nodeWindowList.Clear();
		}
		nodeWindowList = new List<QuestNodeWindow>(quest.Parts[0].Nodes.Count);
		foreach (CardQuestPart part in quest.Parts)
		{
			foreach (CardQuestPart.QuestBattle node in part.Nodes)
			{
				QuestNodeWindow questNodeWindow = new QuestNodeWindow(part, node, this);
				questNodeWindow.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
				nodeWindowList.Add(questNodeWindow);
				Add(questNodeWindow);
			}
		}
	}

	public void AddButtons()
	{
		OkButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(-30f, 210f));
		OkButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_okbutton_rectangular");
		OkButton.IsEnabled = true;
		OkButton.HitTestType = HitTestTypeEnum.Alpha;
		OkButton.Click += OkButton_Click;
		Add(OkButton);
		GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(-188f, 222f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_brawler_airlock_backbutton");
		gUIButton.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton.Click += BackButton_Click;
		Add(gUIButton);
		GUIButton gUIButton2 = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(124f, 217f));
		gUIButton2.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_brawler_airlock_quitbutton");
		gUIButton2.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton2.Click += QuitButton_Click;
		Add(gUIButton2);
	}

	public void PopulateAndAddPrizeCard()
	{
		CreatePrizeCardWindow();
		expandHotspot = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(TINY_SIZE_CARD_SIZE, TINY_SIZE_CARD_OFFSET);
		expandHotspot.Rotation = 12f;
		expandHotspot.Id = "expandHotspot";
		expandHotspot.MouseOver += expandPrizeCard_MouseOver;
		Add(expandHotspot);
	}

	private void CreatePrizeCardWindow()
	{
		PrizeCardWindow = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(TINY_SIZE_CARD_SIZE, TINY_SIZE_CARD_OFFSET);
		PrizeCardWindow.Id = "PrizeCardWindow";
		PrizeCardWindow.Rotation = 12f;
		PrizeCard = new GUIImage();
		PrizeCard.Id = "PrizeCard";
		PrizeCard.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero, new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		PrizeCardWindow.Add(PrizeCard);
		shrinkHotspot = new GUIHotSpotButton();
		shrinkHotspot.Id = "shrinkHotspot";
		shrinkHotspot.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, FULL_SIZE_CARD_OFFSET, FULL_SIZE_CARD_SIZE * 1.08f, AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		shrinkHotspot.MouseOut += expandPrizeCard_MouseOut;
		Add(shrinkHotspot);
		shrinkHotspot.IsEnabled = false;
		CardBG = new GUIImage();
		CardBG.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero, new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		CardBG.TextureSource = "cardgamegadget_bundle|darken_overlay_to_stretch_vertically";
		CardBG.Id = "CardBG";
		CardBG.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		CardBG.IsVisible = false;
		PrizeCardWindow.Add(CardBG);
		CardLock = new GUIImage();
		CardLock.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero, new Vector2(1f, 0.79f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		CardLock.TextureSource = "cardgamegadget_bundle|card_quest_locked_overlay";
		CardLock.Id = "CardLock";
		CardLock.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		CardLock.IsVisible = false;
		PrizeCardWindow.Add(CardLock);
		BuyButton = new GUIButton();
		BuyButton.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 148f), new Vector2(512f, 512f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		BuyButton.StyleInfo = new SHSButtonStyleInfo("cardgamegadget_bundle|L_buy_quest_button");
		BuyButton.Id = "BuyButton";
		BuyButton.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		BuyButton.HitTestType = HitTestTypeEnum.Alpha;
		BuyButton.ToolTip = new NamedToolTipInfo("#TT_CARDGAME_DAILY_BATTLE");
		BuyButton.ToolTip.Offset = new Vector2(150f, 156f);
		BuyButton.IsVisible = false;
		BuyButton.IsEnabled = false;
		BuyButton.Click += delegate
		{
			ShoppingWindow shoppingWindow = new ShoppingWindow(SelectedQuestBattle.ParentCardQuestPart.Id);
			shoppingWindow.launch();
		};
		BuyButton.MouseOver += delegate
		{
			AnimClip newPiece2 = FadeInOut.FadeIn(BuyButton);
			base.AnimationPieceManager.SwapOut(ref fadeBuyButton, newPiece2);
		};
		BuyButton.MouseOut += delegate
		{
			AnimClip newPiece = FadeInOut.FadeOut(BuyButton);
			base.AnimationPieceManager.SwapOut(ref fadeBuyButton, newPiece);
		};
		PrizeCardWindow.Add(BuyButton);
		Add(PrizeCardWindow);
	}

	public void SetSelectedQuestNode(CardQuestPart.QuestBattle questBattle, bool OkToSelect)
	{
		SelectedQuestBattle = questBattle;
		SetPrizeCard(questBattle.Reward);
		OkButton.IsEnabled = OkToSelect;
		CardLock.IsVisible = IsDailyBattleAndNotOwned;
	}

	public void SetPrizeCard(string prizeId)
	{
		if (CardManager.TextureBundleLoaded)
		{
			string[] array = prizeId.Split(':');
			if (array.Length > 0)
			{
				Texture2D texture2D = CardManager.LoadCardTexture(array[0]);
				if (texture2D != null)
				{
					PrizeCard.Texture = texture2D;
				}
			}
		}
		else
		{
			prizeCardUpdatePending = true;
			prizeIdPending = prizeId;
		}
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (prizeCardUpdatePending)
		{
			prizeCardUpdatePending = false;
			SetPrizeCard(prizeIdPending);
		}
	}

	private void expandPrizeCard_MouseOut(GUIControl sender, GUIMouseEvent EventData)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		AnimClip animClip = GrowAndShrinkCard.ShrinkCard(PrizeCardWindow);
		shrinkHotspot.IsEnabled = false;
		base.AnimationPieceManager.Remove(fadeLockImage);
		CardLock.AnimationAlpha = 1f;
		if (BuyButton != null)
		{
			BuyButton.IsEnabled = false;
			BuyButton.IsVisible = false;
		}
		if (CardBG != null)
		{
			CardBG.IsVisible = false;
		}
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			expandHotspot.IsEnabled = true;
		};
		base.AnimationPieceManager.SwapOut(ref growAndShrink, animClip);
	}

	private void expandPrizeCard_MouseOver(GUIControl sender, GUIMouseEvent EventData)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		AnimClip animClip = GrowAndShrinkCard.GrowCard(PrizeCardWindow);
		expandHotspot.IsEnabled = false;
		shrinkHotspot.IsEnabled = true;
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			if (IsDailyBattleAndNotOwned)
			{
				if (CardBG != null)
				{
					CardBG.IsVisible = true;
				}
				if (BuyButton != null)
				{
					BuyButton.Alpha = 0f;
					BuyButton.IsEnabled = true;
					BuyButton.IsVisible = true;
					StartLockFade();
				}
			}
		};
		base.AnimationPieceManager.SwapOut(ref growAndShrink, animClip);
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("panel_up"));
	}

	protected void StartLockFade()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		AnimClip lockFadeAnim = GetLockFadeAnim(CardLock);
		lockFadeAnim.OnFinished += (Action)(object)(Action)delegate
		{
			StartLockFade();
		};
		base.AnimationPieceManager.SwapOut(ref fadeLockImage, lockFadeAnim);
	}

	protected static AnimClip GetLockFadeAnim(GUIImage img)
	{
		return AnimClipBuilder.Absolute.AnimationAlpha(AnimClipBuilder.Path.Linear(1f, 0f, 1.5f) | AnimClipBuilder.Path.Linear(0f, 1f, 1.5f), img);
	}
}
