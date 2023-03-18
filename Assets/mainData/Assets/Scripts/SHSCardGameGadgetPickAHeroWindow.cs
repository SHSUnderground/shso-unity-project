using System.Collections.Generic;
using UnityEngine;

public class SHSCardGameGadgetPickAHeroWindow : SHSCharacterSelectScanlineTransition
{
	public class CharacterSelectInfoWindow : GUISimpleControlWindow
	{
		private GUIDropShadowTextLabel HeroName;

		private GUIDropShadowTextLabel HeroLevel;

		private GUIDropShadowTextLabel HeroExperience;

		private GUIDropShadowTextLabel HeroDesc;

		private GUIButton OkButton;

		private GUIImage HeroPortrait;

		private SHSCardGameGadgetWindow mainGadget;

		public CharacterSelectInfoWindow(string heroKey, SHSCardGameGadgetWindow mainGadget)
		{
			this.mainGadget = mainGadget;
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(509f, 492f), new Vector2(-20f, 0f));
			gUIImage.TextureSource = "persistent_bundle|char_select_background";
			Add(gUIImage);
			HeroName = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(435f, 40f), new Vector2(-241f, -187f));
			HeroName.Anchor = AnchorAlignmentEnum.MiddleLeft;
			HeroName.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 36, Color.white, TextAnchor.MiddleLeft);
			HeroName.FrontColor = GUILabel.GenColor(255, 252, 202);
			HeroName.BackColor = GUILabel.GenColor(0, 15, 75);
			HeroName.TextOffset = new Vector2(-4f, 3f);
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
			HeroDesc = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(422f, 80f), new Vector2(-231f, -120f));
			HeroDesc.Anchor = AnchorAlignmentEnum.TopLeft;
			HeroDesc.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, Color.white, TextAnchor.MiddleLeft);
			HeroDesc.BackColor = GUILabel.GenColor(0, 15, 75);
			HeroDesc.TextOffset = new Vector2(-2f, 2f);
			Add(HeroDesc);
			HeroPortrait = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(200f, 200f), new Vector2(-35f, 55f));
			HeroPortrait.IsVisible = false;
			Add(HeroPortrait);
			OkButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(-30f, 201f));
			OkButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_okbutton_rectangular");
			OkButton.IsEnabled = true;
			OkButton.HitTestType = HitTestTypeEnum.Alpha;
			OkButton.Click += OkWasClicked;
			Add(OkButton);
			GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(-188f, 213f));
			gUIButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_brawler_airlock_backbutton");
			gUIButton.HitTestType = HitTestTypeEnum.Alpha;
			gUIButton.Click += BackWasClicked;
			Add(gUIButton);
			GUIButton gUIButton2 = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(124f, 208f));
			gUIButton2.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_brawler_airlock_quitbutton");
			gUIButton2.HitTestType = HitTestTypeEnum.Alpha;
			gUIButton2.Click += delegate
			{
				mainGadget.CloseGadget();
			};
			Add(gUIButton2);
			InitializeHeroInfo(heroKey);
		}

		public void OkWasClicked(GUIControl sender, GUIClickEvent args)
		{
			string heroSelected = ((SHSCardGameGadgetPickAHeroWindow)parent.Parent.Parent).heroSelected;
			mainGadget.LaunchManager.SelectedHero = heroSelected;
			mainGadget.GoToWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.Main);
		}

		public void BackWasClicked(GUIControl sender, GUIClickEvent args)
		{
			mainGadget.GoToWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.Main);
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

	private SHSCardGameGadgetWindow mainGadget;

	private SHSCharacterSelect characterSelect;

	private string heroSelected;

	public SHSCardGameGadgetPickAHeroWindow(SHSCardGameGadgetWindow mainGadget)
	{
		this.mainGadget = mainGadget;
		characterSelect = GetCharacterSelect();
		CharacterSelectScanlineTransition(characterSelect, GetContentWindow, mainGadget.LaunchManager.SelectedHero, SHSScanlineTransitionWindow<string>.DefaultScanlineTime, new Vector2(520f, 430f));
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
		return new CharacterSelectInfoWindow(heroName, mainGadget);
	}

	public void HeroWasClicked(string heroName)
	{
		heroSelected = heroName;
	}
}
