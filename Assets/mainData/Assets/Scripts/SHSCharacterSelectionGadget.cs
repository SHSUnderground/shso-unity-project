using MySquadChallenge;
using UnityEngine;

public class SHSCharacterSelectionGadget : SHSGadget
{
	public class CharacterSelectMainWindow : SHSCharacterSelectScanlineTransition
	{
		public class CharacterSelectInfoWindow : GUISimpleControlWindow
		{
			private GUIDropShadowTextLabel HeroName;

			private GUIDropShadowTextLabel HeroLevel;

			private GUIDropShadowTextLabel HeroExperience;

			private GUIDropShadowTextLabel HeroDesc;

			private GUIButton OkButton;

			private GUIImage HeroPortrait;

			public CharacterSelectInfoWindow(string heroKey, SHSCharacterSelectionGadget mainGadget)
			{
				GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(509f, 492f), new Vector2(-20f, 0f));
				gUIImage.TextureSource = "persistent_bundle|char_select_background";
				Add(gUIImage);
				HeroName = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(435f, 40f), new Vector2(-241f, -187f));
				HeroName.Anchor = AnchorAlignmentEnum.MiddleLeft;
				HeroName.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 36, Color.white, TextAnchor.MiddleLeft);
				HeroName.FrontColor = GUILabel.GenColor(255, 252, 202);
				HeroName.BackColor = GUILabel.GenColor(0, 15, 75);
				HeroName.TextOffset = new Vector2(-4f, 3f);
				HeroName.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
				Add(HeroName);
				HeroLevel = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(90f, 24f), new Vector2(-231f, -161f));
				HeroLevel.Anchor = AnchorAlignmentEnum.MiddleLeft;
				HeroLevel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 20, Color.white, TextAnchor.MiddleLeft);
				HeroLevel.FrontColor = GUILabel.GenColor(200, 255, 32);
				HeroLevel.BackColor = GUILabel.GenColor(0, 15, 75);
				HeroLevel.TextOffset = new Vector2(-2f, 2f);
				Add(HeroLevel);
				HeroExperience = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(220f, 24f), new Vector2(-231f, -135f));
				HeroExperience.Anchor = AnchorAlignmentEnum.MiddleLeft;
				HeroExperience.SetupText(GUIFontManager.SupportedFontEnum.Komica, 20, Color.white, TextAnchor.MiddleLeft);
				HeroExperience.FrontColor = GUILabel.GenColor(255, 174, 102);
				HeroExperience.BackColor = GUILabel.GenColor(0, 15, 75);
				HeroExperience.TextOffset = new Vector2(-2f, 2f);
				Add(HeroExperience);
				HeroDesc = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(422f, 90f), new Vector2(-231f, -126f));
				HeroDesc.Anchor = AnchorAlignmentEnum.TopLeft;
				HeroDesc.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, Color.white, TextAnchor.MiddleLeft);
				HeroDesc.BackColor = GUILabel.GenColor(0, 15, 75);
				HeroDesc.TextOffset = new Vector2(-2f, 2f);
				Add(HeroDesc);
				HeroPortrait = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(200f, 200f), new Vector2(-35f, 55f));
				HeroPortrait.IsVisible = false;
				Add(HeroPortrait);
				OkButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(-30f, 201f));
				OkButton.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|L_okbutton");
				OkButton.IsEnabled = true;
				OkButton.HitTestSize = new Vector2(0.547f, 0.367f);
				OkButton.Click += delegate
				{
					mainGadget.Close();
				};
				Add(OkButton);
				InitializeHeroInfo(heroKey);
			}

			public void InitializeHeroInfo(string heroKey)
			{
				HeroName.Text = AppShell.Instance.CharacterDescriptionManager[heroKey].CharacterName;
				if (SHSCharacterSelectionSimulation.SimulationActive)
				{
					SetupCharacterSelectSimText(heroKey);
				}
				else
				{
					HeroLevel.Text = XpToLevelDefinition.GetLevelText(heroKey);
					HeroExperience.Text = XpToLevelDefinition.GetExpText(heroKey);
				}
				HeroDesc.Text = AppShell.Instance.CharacterDescriptionManager[heroKey].LongDescription;
				HeroPortrait.TextureSource = "characters_bundle|expandedtooltip_render_" + heroKey;
				HeroPortrait.IsVisible = true;
			}

			public void SetupCharacterSelectSimText(string heroKey)
			{
				int characterExperience = SHSCharacterSelectionSimulation.GetCharacterExperience(heroKey);
				StringTable stringTable = AppShell.Instance.stringTable;
				int levelForXp = XpToLevelDefinition.Instance.GetLevelForXp(characterExperience);
				string @string = stringTable.GetString(XpToLevelDefinition.Instance.GetLevelDescriptionForXp(characterExperience));
				HeroLevel.Text = string.Format(AppShell.Instance.stringTable["#airlock_hero_level"], @string);
				if (levelForXp == XpToLevelDefinition.Instance.MaxLevel)
				{
					HeroExperience.Text = "#Max_Exp";
				}
				else
				{
					HeroExperience.Text = string.Format(AppShell.Instance.stringTable["#airlock_hero_experience"], characterExperience.ToString(), AppShell.Instance.stringTable[XpToLevelDefinition.Instance.GetXpDescriptionForLevel(levelForXp + 1)]);
				}
			}
		}

		public class CharacterSelectDetailWindow : GUISimpleControlWindow
		{
			private GUIDropShadowTextLabel HeroName;

			private SHSHeroStatsWindow _statsWindow;

			public CharacterSelectDetailWindow(string heroKey, SHSCharacterSelectionGadget mainGadget)
			{
				GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(586f, 419f), new Vector2(-172f, -8f));
				gUIImage.TextureSource = "mysquadgadget_bundle|hero_profile_bars";
				Add(gUIImage);
				HeroName = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(435f, 40f), new Vector2(-241f, -187f));
				HeroName.Anchor = AnchorAlignmentEnum.MiddleLeft;
				HeroName.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 36, Color.white, TextAnchor.MiddleLeft);
				HeroName.FrontColor = GUILabel.GenColor(255, 252, 202);
				HeroName.BackColor = GUILabel.GenColor(0, 15, 75);
				HeroName.TextOffset = new Vector2(-4f, 3f);
				HeroName.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
				Add(HeroName);
				_statsWindow = new SHSHeroStatsWindow(new MySquadDataManager(AppShell.Instance.Profile.UserId, AppShell.Instance.Profile.PlayerName, AppShell.Instance.Profile.SquadLevel));
				_statsWindow.SetPositionAndSize(new Vector2(0f, 40f), new Vector2(586f, 419f));
				Add(_statsWindow);
				InitializeHeroInfo(heroKey);
			}

			public void InitializeHeroInfo(string heroKey)
			{
				HeroName.Text = AppShell.Instance.CharacterDescriptionManager[heroKey].CharacterName;
				_statsWindow.SelectHero(heroKey);
			}
		}

		private SHSCharacterSelectionGadget mainGadget;

		public CharacterSelectMainWindow(UserProfile profile, SHSCharacterSelectionGadget mainGadget)
		{
			this.mainGadget = mainGadget;
			SHSCharacterSelect sHSCharacterSelect = null;
			CharacterSelectScanlineTransition((!SHSCharacterSelectionSimulation.SimulationActive) ? new SHSCharacterSelect(profile, mainGadget.HeroClicked) : new SHSCharacterSelect(SHSCharacterSelectionSimulation.GenerateCharacterItemList(mainGadget.HeroClicked)), GetContentWindow, mainGadget.heroSelected);
			base.HeroClicked += OnHeroSelected;
		}

		public GUISimpleControlWindow GetContentWindow(string heroKey)
		{
			return new CharacterSelectInfoWindow(heroKey, mainGadget);
		}

		public void OnHeroSelected(string heroKey)
		{
			mainGadget.heroSelected = heroKey;
		}
	}

	public class SHSCharacterTopWindowText : GadgetTopWindow
	{
		public SHSCharacterTopWindowText()
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(592f, 141f), new Vector2(0f, 10f));
			gUIImage.TextureSource = "persistent_bundle|gadget_topmodule";
			Add(gUIImage);
			GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(215f, 48f), new Vector2(0f, 0f));
			gUIImage2.TextureSource = "persistent_bundle|L_title_pick_a_hero";
			Add(gUIImage2);
		}
	}

	public static bool FirstLoginCharacterSelectDone;

	private CharacterSelectMainWindow characterSelectWindow;

	private SHSCharacterTopWindowText characterTopWindowText;

	public string heroSelected = "ms_marvel";

	private string lastHeroSelected;

	private UserProfile profile;

	public SHSCharacterSelectionGadget()
	{
		profile = AppShell.Instance.Profile;
		if (SHSCharacterSelectionSimulation.SimulationActive)
		{
			heroSelected = SHSCharacterSelectionSimulation.SimulatedCharacter;
			CloseButton.IsEnabled = SHSCharacterSelectionSimulation.AllowCharacterSelectClose;
			CloseButton.IsVisible = SHSCharacterSelectionSimulation.AllowCharacterSelectClose;
		}
		else
		{
			heroSelected = profile.LastSelectedCostume;
		}
		lastHeroSelected = heroSelected;
		characterSelectWindow = new CharacterSelectMainWindow(profile, this);
		characterTopWindowText = new SHSCharacterTopWindowText();
		SetupOpeningWindow(BackgroundType.OnePanel, characterSelectWindow);
		SetupOpeningTopWindow(characterTopWindowText);
		CloseButton.Click += delegate
		{
			heroSelected = lastHeroSelected;
			Close();
		};
		AppShell.Instance.EventMgr.AddListener<CharacterSelectedMessage>(OnCharacterSelectedMessage);
	}

	public void Close()
	{
		if (profile != null)
		{
			if (profile.SelectedCostume != heroSelected && AchievementManager.shouldReportAchievementEvent("generic_event", "swap_hero", string.Empty))
			{
				AppShell.Instance.delayedAchievementEvent(heroSelected, "generic_event", "swap_hero", string.Empty, 3f);
			}
			profile.LastSelectedCostume = heroSelected;
			profile.SelectedCostume = heroSelected;
			profile.PersistExtendedData();
		}
		VOManager.Instance.StopAll();
		AppShell.Instance.EventMgr.Fire(this, new CharacterSelectedMessage(heroSelected));
	}

	private void OnCharacterSelectedMessage(CharacterSelectedMessage msg)
	{
		CloseGadget();
	}

	public override void CloseGadget()
	{
		AppShell.Instance.EventMgr.RemoveListener<CharacterSelectedMessage>(OnCharacterSelectedMessage);
		base.CloseGadget();
	}

	private void HeroClicked(string heroName)
	{
		heroSelected = heroName;
		characterSelectWindow.OnHeroSelected(heroSelected);
	}
}
