using MySquadChallenge;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSSocialSurvivalModeLeaderboard : GUIDynamicWindow
{
	protected GUIImage bgImage;

	protected GUIImage heroSelectionOverlay;

	protected GUIStrokeTextLabel congratulationsTextLabel;

	protected GUIButton okButton;

	protected GUIButton heroSelectButton;

	protected GUITBCloseButton heroSelectCancelButton;

	protected SHSMyHeroesWindow heroSelectionWindow;

	protected MySquadDataManager squadDataManager;

	protected string missionId;

	protected int selectedHeroId;

	protected List<GUIButton> heroButtons = new List<GUIButton>();

	protected List<GUIStrokeTextLabel> textLabels = new List<GUIStrokeTextLabel>();

	public SHSSocialSurvivalModeLeaderboard(string missionId)
	{
		this.missionId = missionId;
	}

	public override bool InitializeResources(bool reload)
	{
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Expected O, but got Unknown
		SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero, new Vector2(1064f, 526f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		bgImage = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(0f, 0f), new Vector2(0f, 0f));
		bgImage.TextureSource = "gameworld_bundle|subscription_notification_background_panel";
		Add(bgImage);
		okButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(0f, 166f));
		okButton.Click += delegate
		{
			onClick();
		};
		okButton.HitTestSize = new Vector2(0.5f, 0.5f);
		okButton.HitTestType = HitTestTypeEnum.Circular;
		okButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|L_mshs_button_ok");
		okButton.IsVisible = false;
		Add(okButton);
		congratulationsTextLabel = new GUIStrokeTextLabel();
		congratulationsTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 55, GUILabel.GenColor(212, 124, 9), GUILabel.GenColor(0, 23, 67), GUILabel.GenColor(0, 23, 67), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
		congratulationsTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(532f, 27f), new Vector2(450f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		congratulationsTextLabel.Text = "#SURVIVAL_HIGHSCORES";
		congratulationsTextLabel.IsVisible = false;
		Add(congratulationsTextLabel);
		heroSelectButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(54f, 54f), new Vector2(100f, 166f));
		heroSelectButton.Click += delegate
		{
			if (heroSelectionWindow == null)
			{
				heroSelectionOverlay = new GUIImage();
				heroSelectionOverlay.SetSize(341f, 502f);
				heroSelectionOverlay.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(300f, 0f));
				heroSelectionOverlay.TextureSource = "mysquadgadget_bundle|mysquad_2panel_left_overlay_opaque";
				Add(heroSelectionOverlay);
				heroSelectCancelButton = new GUITBCloseButton();
				heroSelectCancelButton.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(600f, 0f));
				heroSelectCancelButton.Click += delegate
				{
					selectedHeroId = 0;
					HideHeroSelectWindow();
					FetchScoreData();
				};
				Add(heroSelectCancelButton);
				MySquadDataManager mySquadDataManager = new MySquadDataManager(AppShell.Instance.Profile.UserId, string.Empty, 0);
				List<SHSHeroSelectionItem> heroListFromProfile = SHSMyHeroesWindow.GetHeroListFromProfile(mySquadDataManager.Profile, OnHeroClick);
				SHSMyHeroesWindow.AddAllHeroesToHeroList(OnHeroClick, heroListFromProfile);
				heroSelectionWindow = new SHSMyHeroesWindow(heroListFromProfile);
				heroSelectionWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(300f, 0f));
				Add(heroSelectionWindow);
			}
			else
			{
				heroSelectionWindow.Show();
				heroSelectionOverlay.Show();
				heroSelectCancelButton.Show();
			}
		};
		heroSelectButton.HitTestSize = new Vector2(1f, 1f);
		heroSelectButton.HitTestType = HitTestTypeEnum.Circular;
		heroSelectButton.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|mshs_gameworld_HUD_changehero");
		heroSelectButton.ToolTip = new NamedToolTipInfo("#GAMEWORLD_PICKHERO_BUTTON");
		heroSelectButton.IsVisible = false;
		Add(heroSelectButton);
		AnimClip animClip = AnimClipBuilder.Absolute.SizeX(AnimClipBuilder.Path.Linear(0f, 1064f, 0.25f), bgImage) ^ AnimClipBuilder.Absolute.SizeY(AnimClipBuilder.Path.Linear(0f, 426f, 0.25f), bgImage) ^ AnimClipBuilder.Absolute.OffsetX(AnimClipBuilder.Path.Linear(190f, 0f, 0.25f), this);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			congratulationsTextLabel.IsVisible = true;
			okButton.IsVisible = true;
			heroSelectButton.IsVisible = true;
			FetchScoreData();
		};
		base.AnimationPieceManager.Add(animClip);
		return base.InitializeResources(reload);
	}

	protected void HideHeroSelectWindow()
	{
		heroSelectionWindow.Hide();
		heroSelectionOverlay.Hide();
		heroSelectCancelButton.Hide();
	}

	protected void OnHeroClick(string hero)
	{
		selectedHeroId = OwnableDefinition.HeroNameToHeroID[hero];
		HideHeroSelectWindow();
		FetchScoreData();
	}

	protected void onClick()
	{
		Hide();
	}

	protected void FetchScoreData()
	{
		foreach (GUIStrokeTextLabel textLabel in textLabels)
		{
			Remove(textLabel);
		}
		textLabels.Clear();
		foreach (GUIButton heroButton in heroButtons)
		{
			Remove(heroButton);
		}
		heroButtons.Clear();
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("mission_id", missionId);
		wWWForm.AddField("hero_id", selectedHeroId);
		wWWForm.AddField("is_multiplayer_score", 0);
		int heroId2 = default(int);
		AppShell.Instance.WebService.StartRequest("resources$users/get_high_scores.py", delegate(ShsWebResponse response)
		{
			if (response.Status == 200)
			{
				DataWarehouse dataWarehouse2 = new DataWarehouse(response.Body);
				CspUtils.DebugLogWarning(response.Body);
				dataWarehouse2.Parse();
				GUIStrokeTextLabel gUIStrokeTextLabel2 = new GUIStrokeTextLabel();
				gUIStrokeTextLabel2.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 15, GUILabel.GenColor(172, 214, 9), GUILabel.GenColor(0, 23, 67), GUILabel.GenColor(0, 23, 67), new Vector2(2f, 2f), TextAnchor.MiddleLeft);
				gUIStrokeTextLabel2.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(60f, 75f), new Vector2(450f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				gUIStrokeTextLabel2.Text = "#SURVIVAL_SOLO_HEADER";
				Add(gUIStrokeTextLabel2);
				textLabels.Add(gUIStrokeTextLabel2);
				int num3 = 1;
				int num4 = 100;
				foreach (DataWarehouse item in dataWarehouse2.GetIterator("//scores/leaders/leader"))
				{
					string text2 = string.Format("{2}. {0} - {1}", item.GetString("player"), string.Format("{0:n0}", item.GetInt("score")), item.TryGetInt("rank", num3));
					CspUtils.DebugLogWarning(text2);
					num3++;
					gUIStrokeTextLabel2 = new GUIStrokeTextLabel();
					gUIStrokeTextLabel2.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 15, GUILabel.GenColor(172, 214, 9), GUILabel.GenColor(0, 23, 67), GUILabel.GenColor(0, 23, 67), new Vector2(2f, 2f), TextAnchor.MiddleLeft);
					gUIStrokeTextLabel2.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(90f, num4), new Vector2(520f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
					gUIStrokeTextLabel2.Text = text2;
					Add(gUIStrokeTextLabel2);
					textLabels.Add(gUIStrokeTextLabel2);
					heroId2 = item.GetInt("hero");
					GUIButton gUIButton2 = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(40f, 40f), new Vector2(55f, num4 + 8));
					string value2 = string.Empty;
					if (OwnableDefinition.HeroIDToHeroName.TryGetValue(item.GetInt("hero"), out value2))
					{
						gUIButton2.StyleInfo = new SHSButtonStyleInfo("characters_bundle|token_" + value2.ToLowerInvariant().Replace(" ", "_"), true);
					}
					gUIButton2.Click += delegate
					{
						ShoppingWindow shoppingWindow2 = new ShoppingWindow(heroId2);
						shoppingWindow2.launch();
					};
					gUIButton2.ToolTip = new NamedToolTipInfo("#CIN_" + OwnableDefinition.HeroIDToHeroName[item.GetInt("hero")].ToUpper() + "_EXNM", Vector2.zero);
					Add(gUIButton2);
					heroButtons.Add(gUIButton2);
					if (item.GetString("player").ToLowerInvariant() == AppShell.Instance.Profile.PlayerName.ToLowerInvariant())
					{
						gUIStrokeTextLabel2.FrontColor = GUILabel.GenColor(212, 124, 9);
					}
					num4 += 20;
				}
				gUIStrokeTextLabel2 = new GUIStrokeTextLabel();
				gUIStrokeTextLabel2.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 15, GUILabel.GenColor(172, 214, 9), GUILabel.GenColor(0, 23, 67), GUILabel.GenColor(0, 23, 67), new Vector2(2f, 2f), TextAnchor.MiddleLeft);
				gUIStrokeTextLabel2.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(60f, 300f), new Vector2(450f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				gUIStrokeTextLabel2.Text = string.Format(AppShell.Instance.stringTable["#SURVIVAL_YOU"], string.Format("{0:n0}", dataWarehouse2.GetInt("//scores/player_score")));
				Add(gUIStrokeTextLabel2);
				textLabels.Add(gUIStrokeTextLabel2);
			}
		}, wWWForm.data);
		wWWForm.AddField("is_multiplayer_score", 1);
		int heroId = default(int);
		AppShell.Instance.WebService.StartRequest("resources$users/get_high_scores.py", delegate(ShsWebResponse response)
		{
			if (response.Status == 200)
			{
				DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
				CspUtils.DebugLogWarning(response.Body);
				dataWarehouse.Parse();
				int num = 1;
				int num2 = 100;
				GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
				gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 15, GUILabel.GenColor(172, 214, 9), GUILabel.GenColor(0, 23, 67), GUILabel.GenColor(0, 23, 67), new Vector2(2f, 2f), TextAnchor.MiddleRight);
				gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, new Vector2(1000f, 75f), new Vector2(450f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				gUIStrokeTextLabel.Text = "#SURVIVAL_GROUP_HEADER";
				Add(gUIStrokeTextLabel);
				textLabels.Add(gUIStrokeTextLabel);
				foreach (DataWarehouse item2 in dataWarehouse.GetIterator("//scores/leaders/leader"))
				{
					string text = string.Format("{2}. {0} - {1}", item2.GetString("player"), string.Format("{0:n0}", item2.GetInt("score")), item2.TryGetInt("rank", num));
					CspUtils.DebugLogWarning(text);
					num++;
					gUIStrokeTextLabel = new GUIStrokeTextLabel();
					gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 15, GUILabel.GenColor(172, 214, 9), GUILabel.GenColor(0, 23, 67), GUILabel.GenColor(0, 23, 67), new Vector2(2f, 2f), TextAnchor.MiddleRight);
					gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, new Vector2(970f, num2), new Vector2(480f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
					gUIStrokeTextLabel.Text = text;
					Add(gUIStrokeTextLabel);
					textLabels.Add(gUIStrokeTextLabel);
					heroId = item2.GetInt("hero");
					GUIButton gUIButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(40f, 40f), new Vector2(970f, num2 + 8));
					string value = string.Empty;
					if (OwnableDefinition.HeroIDToHeroName.TryGetValue(item2.GetInt("hero"), out value))
					{
						gUIButton.StyleInfo = new SHSButtonStyleInfo("characters_bundle|token_" + value.ToLowerInvariant().Replace(" ", "_"), true);
					}
					gUIButton.Click += delegate
					{
						ShoppingWindow shoppingWindow = new ShoppingWindow(heroId);
						shoppingWindow.launch();
					};
					gUIButton.ToolTip = new NamedToolTipInfo("#CIN_" + OwnableDefinition.HeroIDToHeroName[item2.GetInt("hero")].ToUpper() + "_EXNM", Vector2.zero);
					Add(gUIButton);
					heroButtons.Add(gUIButton);
					if (item2.GetString("player").ToLowerInvariant() == AppShell.Instance.Profile.PlayerName.ToLowerInvariant())
					{
						gUIStrokeTextLabel.FrontColor = GUILabel.GenColor(212, 124, 9);
					}
					num2 += 20;
				}
				gUIStrokeTextLabel = new GUIStrokeTextLabel();
				gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 15, GUILabel.GenColor(172, 214, 9), GUILabel.GenColor(0, 23, 67), GUILabel.GenColor(0, 23, 67), new Vector2(2f, 2f), TextAnchor.MiddleRight);
				gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, new Vector2(1000f, 300f), new Vector2(450f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				gUIStrokeTextLabel.Text = string.Format(AppShell.Instance.stringTable["#SURVIVAL_YOU"], string.Format("{0:n0}", dataWarehouse.GetInt("//scores/player_score")));
				Add(gUIStrokeTextLabel);
				textLabels.Add(gUIStrokeTextLabel);
			}
		}, wWWForm.data);
	}

	protected override void dispose(bool disposing)
	{
		base.dispose(disposing);
		bgImage = null;
		okButton = null;
		heroSelectButton = null;
		heroSelectionOverlay = null;
		heroSelectionWindow = null;
		if (squadDataManager != null)
		{
			squadDataManager.ClearProfile();
		}
		heroButtons.Clear();
		textLabels.Clear();
	}
}
