using System;
using UnityEngine;

public class SHSMySquadChallengeMySquadWindow : SHSGadget.GadgetCenterWindow
{
	protected class StaticMeterOverlay : GUIImage
	{
		public override float AnimationAlpha
		{
			get
			{
				return base.AnimationAlpha;
			}
			set
			{
			}
		}
	}

	protected MySquadDataManager dataManager;

	protected SpinningSquadBackgroundControl spinner;

	protected RotatingHeroSilhouetteControl silhouettes;

	protected GUIDropShadowTextLabel currentChallengeLabel;

	protected GUIStrokeTextLabel challengeTitle;

	protected GUIDropShadowTextLabel challengeDescription;

	protected GUIDrawTexture challengeCompleteBanner;

	protected StaticMeterOverlay rightmeterFillSolid;

	protected GUISimpleControlWindow progressWindow;

	protected GUISimpleControlWindow nextChallengeControl;

	protected GUIButton nextChallengeButton;

	protected SHSMySquadRewardIcon rewardIcon;

	protected GUISimpleControlWindow collectRewardWindow;

	protected GUIImage[] squadTierIcons;

	protected GUIImage squadTierIconLarge;

	protected GUIStrokeTextLabel squadLevelValue;

	protected bool profileInitialized;

	protected GUISimpleControlWindow squadLevelContainer;

	protected GUISimpleControlWindow loadingTextWindow;

	protected GUIDropShadowTextLabel goldValue;

	protected GUIDropShadowTextLabel silverValue;

	protected GUIDropShadowTextLabel ticketsValue;

	protected GUISimpleControlWindow progressContainer;

	protected GUISimpleControlWindow rewardCollectedContainer;

	protected GUIStrokeTextLabel rewardCollectedLabel;

	protected GUIStrokeTextLabel rewardCollectedValueLabel;

	protected SHSMySquadRewardTextbox rewardTextWindow;

	protected LargeSerumAnimation serumAnimation;

	protected AnimClip animClipHackRef;

	private GUISimpleControlWindow BuyGoldControl;

	protected GUIImage[] hero_icons;

	protected static readonly string[] RewardedHeroes = new string[10]
	{
		"daredevil",
		"elektra",
		"storm_mohawk",
		"firestar",
		"sentry",
		"wolverine_jeans",
		"iron_man_stealth",
		"spider_man_future",
		"colossus",
		"modok_playable"
	};

	protected Vector2[] IconPositions;

	protected static readonly int[] LevelsPerTier = new int[10]
	{
		5,
		5,
		5,
		5,
		5,
		5,
		5,
		5,
		5,
		20
	};

	private GUISimpleControlWindow skipChallengeControl;

	private GUIButton skipChallengeButton;

	private GUIStrokeTextLabel skipChallengeButtonLabel;

	private GUIStrokeTextLabel squadTitleLabel;

	private ChooseTitleHUD chooseTitleHUD;

	public static Vector2 TITLE_WINDOW_OPENED = new Vector2(400f, 120f);

	public static Vector2 TITLE_WINDOW_CLOSED = new Vector2(-800f, 120f);

	private ChooseMedallionHUD chooseMedallionHUD;

	public static Vector2 MEDALLION_WINDOW_OPENED = new Vector2(0f, 190f);

	public static Vector2 MEDALLION_WINDOW_CLOSED = new Vector2(-500f, 190f);

	private GUIButton changeMedallionButton;

	public SHSMySquadChallengeMySquadWindow(MySquadDataManager dataManager)
	{
		this.dataManager = dataManager;
		SetSize(new Vector2(1020f, 644f));
		Offset = new Vector2(0f, -318f);
		profileInitialized = false;
		GUISimpleControlWindow gUISimpleControlWindow = new GUISimpleControlWindow();
		gUISimpleControlWindow.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(-223f, -51f), new Vector2(521f, 521f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIImage gUIImage = new GUIImage();
		gUIImage.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(521f, 521f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_p1_circle_rightmeter_fill";
		gUIImage.Id = "rightmeterFill";
		gUISimpleControlWindow.Add(gUIImage);
		rightmeterFillSolid = new StaticMeterOverlay();
		rightmeterFillSolid.SetPositionAndSize(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(127f, 35f), new Vector2(226f, 439f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		rightmeterFillSolid.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_p1_circle_rightmeter_fill_solid";
		rightmeterFillSolid.Id = "rightmeterFillSolid";
		gUISimpleControlWindow.Add(rightmeterFillSolid);
		Add(gUISimpleControlWindow);
		GUIImage gUIImage2 = new GUIImage();
		gUIImage2.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0.2f, -3.67f), new Vector2(1020f, 644f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage2.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_p1_containers";
		gUIImage2.Id = "background";
		Add(gUIImage2);
		GUISimpleControlWindow gUISimpleControlWindow2 = new GUISimpleControlWindow();
		gUISimpleControlWindow2.Id = "spinningPartsContainer";
		gUISimpleControlWindow2.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(-223f, -50f), new Vector2(521f, 521f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		spinner = new SpinningSquadBackgroundControl();
		spinner.Id = "spinner";
		spinner.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(521f, 521f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUISimpleControlWindow2.Add(spinner);
		silhouettes = new RotatingHeroSilhouetteControl();
		silhouettes.Id = "silhouettes";
		silhouettes.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(521f, 521f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUISimpleControlWindow2.Add(silhouettes);
		squadLevelContainer = new GUISimpleControlWindow();
		squadLevelContainer.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -178f), new Vector2(200f, 172f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		squadLevelContainer.Id = "squadLevelContainer";
		GUIImage gUIImage3 = new GUIImage();
		gUIImage3.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(165f, 165f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage3.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_p1_squadlevel_base";
		gUIImage3.Id = "squadLevelContainer";
		squadLevelContainer.Add(gUIImage3);
		squadTierIconLarge = new GUIImage();
		squadTierIconLarge.Id = "squadTierIconLarge";
		squadTierIconLarge.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -20f), new Vector2(198f, 172f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		squadTierIconLarge.TextureSource = "mysquadgadget_bundle|tier_icon_1";
		squadTierIconLarge.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		squadTierIconLarge.IsVisible = false;
		squadLevelContainer.Add(squadTierIconLarge);
		GUIImage gUIImage4 = new GUIImage();
		gUIImage4.Id = "squadLevelFront";
		gUIImage4.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(165f, 165f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage4.TextureSource = "mysquadgadget_bundle|L_mshs_mysquad_challenge_p1_squadlevel";
		squadLevelContainer.Add(gUIImage4);
		squadLevelValue = new GUIStrokeTextLabel();
		squadLevelValue.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -15f), new Vector2(200f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		squadLevelValue.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 31, GUILabel.GenColor(181, 213, 29), GUILabel.GenColor(3, 30, 83), GUILabel.GenColor(3, 30, 83), new Vector2(3f, 3f), TextAnchor.MiddleCenter);
		squadLevelValue.Id = "squadLevelValue";
		squadLevelContainer.Add(squadLevelValue);
		squadLevelContainer.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		squadLevelContainer.IsVisible = false;
		gUISimpleControlWindow2.Add(squadLevelContainer);
		Add(gUISimpleControlWindow2);
		GUIImage gUIImage5 = new GUIImage();
		gUIImage5.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(-223f, -51f), new Vector2(521f, 521f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage5.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_p1_circle_dots";
		gUIImage5.Id = "dots";
		Add(gUIImage5);
		if (this.dataManager.Profile != null)
		{
			InitializeWindow();
			return;
		}
		loadingTextWindow = new GUISimpleControlWindow();
		loadingTextWindow.Id = "loadingTextWindow";
		loadingTextWindow.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(-220f, -265f), new Vector2(300f, 300f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -38f), new Vector2(300f, 100f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, GUILabel.GenColor(177, 216, 23), GUILabel.GenColor(0, 26, 73), GUILabel.GenColor(0, 26, 73), new Vector2(3f, 3f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel.Id = "loadingLabel1";
		gUIStrokeTextLabel.Text = "Accessing info for";
		loadingTextWindow.Add(gUIStrokeTextLabel);
		GUIStrokeTextLabel gUIStrokeTextLabel2 = new GUIStrokeTextLabel();
		gUIStrokeTextLabel2.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(300f, 100f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel2.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 24, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 26, 73), GUILabel.GenColor(0, 26, 73), new Vector2(3f, 3f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel2.Text = dataManager.SquadName;
		gUIStrokeTextLabel2.Id = "loadingLabel2";
		loadingTextWindow.Add(gUIStrokeTextLabel2);
		GUIStrokeTextLabel gUIStrokeTextLabel3 = new GUIStrokeTextLabel();
		gUIStrokeTextLabel3.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, 38f), new Vector2(300f, 100f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel3.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, GUILabel.GenColor(177, 216, 23), GUILabel.GenColor(0, 26, 73), GUILabel.GenColor(0, 26, 73), new Vector2(3f, 3f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel3.Text = "please stand by...";
		gUIStrokeTextLabel3.Id = "loadingLabel3";
		loadingTextWindow.Add(gUIStrokeTextLabel3);
		Add(loadingTextWindow);
	}

	public override void OnShow()
	{
		base.OnShow();
		if (profileInitialized)
		{
			OnProfileInitialization();
		}
		AppShell.Instance.EventMgr.AddListener<CurrencyUpdateMessage>(OnCurrencyUpdated);
		AppShell.Instance.EventMgr.AddListener<PlayerChangedSquadInfoMessage>(OnSquadInfoChanged);
	}

	public override void OnHide()
	{
		AppShell.Instance.EventMgr.RemoveListener<CurrencyUpdateMessage>(OnCurrencyUpdated);
		AppShell.Instance.EventMgr.AddListener<PlayerChangedSquadInfoMessage>(OnSquadInfoChanged);
		base.OnHide();
	}

	private void OnSquadInfoChanged(PlayerChangedSquadInfoMessage message)
	{
		squadTitleLabel.Text = AppShell.Instance.Profile.Title;
		string text = AppShell.Instance.Profile.Medallion;
		if (text != string.Empty)
		{
			if (text.IndexOf("_normal") != -1)
			{
				text = text.Substring(0, text.IndexOf("_normal"));
			}
			changeMedallionButton.StyleInfo = new SHSButtonStyleInfo(text, SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		}
	}

	private void OnCurrencyUpdated(CurrencyUpdateMessage message)
	{
		if (dataManager.Profile is LocalPlayerProfile)
		{
			goldValue.Text = dataManager.Profile.Gold.ToString();
			silverValue.Text = dataManager.Profile.Silver.ToString();
			ticketsValue.Text = dataManager.Profile.Shards.ToString();
		}
	}

	private void OnProfileInitialization()
	{
		silhouettes.StartAnimations();
		spinner.StartSpinningAnimations();
		squadLevelContainer.IsVisible = true;
		squadLevelValue.Text = dataManager.SquadLevel.ToString();
		UpdateChallengeDisplay();
		if (loadingTextWindow != null)
		{
			loadingTextWindow.IsVisible = false;
		}
	}

	public override void OnActive()
	{
		base.OnActive();
		AppShell.Instance.EventMgr.AddListener<ChallengeManagerStateChangedMessage>(OnChallengeManagerStateChanged);
	}

	public override void OnInactive()
	{
		base.OnInactive();
		AppShell.Instance.EventMgr.RemoveListener<ChallengeManagerStateChangedMessage>(OnChallengeManagerStateChanged);
	}

	protected void InitializeWindow()
	{
		IconPositions = new Vector2[10]
		{
			new Vector2(-333f, -92f),
			new Vector2(-372f, -124f),
			new Vector2(-406f, -168f),
			new Vector2(-424f, -215f),
			new Vector2(-431f, -272f),
			new Vector2(-424f, -324f),
			new Vector2(-406f, -372f),
			new Vector2(-372f, -414f),
			new Vector2(-332f, -451f),
			new Vector2(-282f, -477f)
		};
		hero_icons = new GUIImage[IconPositions.Length];
		for (int i = 0; i < IconPositions.Length; i++)
		{
			GUIImage gUIImage = new GUIImage();
			gUIImage.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, IconPositions[i], new Vector2(82f, 82f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIImage.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			gUIImage.IsVisible = false;
			gUIImage.Id = "heroIcon_" + i;
			hero_icons[i] = gUIImage;
			Add(gUIImage);
		}
		GUIImage gUIImage2 = new GUIImage();
		gUIImage2.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(-223f, -51f), new Vector2(521f, 521f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage2.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_p1_circle_rightmeter_overlay";
		gUIImage2.Id = "rightmeterOverlay";
		Add(gUIImage2);
		GUIImage gUIImage3 = new GUIImage();
		gUIImage3.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(-222f, -48f), new Vector2(521f, 521f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage3.TextureSource = "mysquadgadget_bundle|L_mshs_mysquad_challenge_p1_circle_labels";
		gUIImage3.Id = "circleLabels";
		Add(gUIImage3);
		Vector2[] array = new Vector2[10]
		{
			new Vector2(-110f, -105f),
			new Vector2(-70f, -139f),
			new Vector2(-37f, -181f),
			new Vector2(-18f, -230f),
			new Vector2(-11f, -282f),
			new Vector2(-17f, -338f),
			new Vector2(-37f, -388f),
			new Vector2(-71f, -432f),
			new Vector2(-111f, -466f),
			new Vector2(-160f, -490f)
		};
		squadTierIcons = new GUIImage[10];
		for (int j = 0; j < squadTierIcons.Length; j++)
		{
			squadTierIcons[j] = new GUIImage();
			squadTierIcons[j].Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			squadTierIcons[j].TextureSource = "mysquadgadget_bundle|tier_icon_" + (j + 1) + "_small";
			squadTierIcons[j].SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, array[j], new Vector2(60f, 52f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			squadTierIcons[j].IsVisible = false;
			squadTierIcons[j].Id = "squad_tier_icon_" + j;
			Add(squadTierIcons[j]);
		}
		currentChallengeLabel = new GUIDropShadowTextLabel();
		currentChallengeLabel.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(137f, -420f), new Vector2(200f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		currentChallengeLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 20, GUILabel.GenColor(136, 242, 255), GUILabel.GenColor(1, 28, 66), new Vector2(1f, 2f), TextAnchor.MiddleLeft);
		currentChallengeLabel.Bold = true;
		currentChallengeLabel.Text = "#SQ_MYSQUAD_CURRENTCHALLENGE";
		currentChallengeLabel.Id = "currentChallengeLabel";
		Add(currentChallengeLabel);
		skipChallengeControl = new GUISimpleControlWindow();
		skipChallengeControl.Id = "skipChallengeControl";
		skipChallengeControl.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		skipChallengeControl.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(356f, -314f), new Vector2(256f, 256f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		skipChallengeButton = new GUIButton();
		skipChallengeButton.Id = "skipChallengeButton";
		skipChallengeButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|mshs_mysquad_challenge_smaller_next_button");
		skipChallengeButton.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(256f, 256f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		skipChallengeButton.HitTestType = HitTestTypeEnum.Alpha;
		skipChallengeButton.MouseDown += delegate
		{
			skipCurrentChallenge();
		};
		skipChallengeControl.Add(skipChallengeButton);
		skipChallengeButtonLabel = new GUIStrokeTextLabel();
		skipChallengeButtonLabel.Id = "nextChallengeLabel";
		skipChallengeButtonLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(63, 86, 0), GUILabel.GenColor(120, 170, 4), new Vector2(2f, 3f), TextAnchor.MiddleCenter);
		skipChallengeButtonLabel.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, -3f), new Vector2(152f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		skipChallengeButtonLabel.Text = "#SQ_NEXT_CHALLENGE";
		skipChallengeControl.Add(skipChallengeButtonLabel);
		Add(skipChallengeControl);
		challengeTitle = new GUIStrokeTextLabel();
		challengeTitle.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(255f, -362f), new Vector2(400f, 100f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		challengeTitle.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 20, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 70, 129), GUILabel.GenColor(7, 68, 157), new Vector2(1f, 2f), TextAnchor.MiddleLeft);
		challengeTitle.BackColorAlpha = 0.75f;
		challengeTitle.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		challengeTitle.Id = "challengeTitle";
		Add(challengeTitle);
		challengeDescription = new GUIDropShadowTextLabel();
		challengeDescription.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(255f, -1f), new Vector2(400f, 400f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		challengeDescription.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(6, 73, 123), new Vector2(1f, 2f), TextAnchor.UpperLeft);
		challengeDescription.Bold = true;
		challengeDescription.Id = "challengeDescription";
		Add(challengeDescription);
		progressContainer = new GUISimpleControlWindow();
		progressContainer.Id = "progressContainer";
		progressContainer.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		progressContainer.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(238f, -189f), new Vector2(454f, 161f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIImage gUIImage4 = new GUIImage();
		gUIImage4.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(454f, 161f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage4.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_p1_reward_checkboxes";
		progressContainer.Add(gUIImage4);
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(104f, 7f), new Vector2(200f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, GUILabel.GenColor(255, 217, 85), GUILabel.GenColor(0, 82, 112), GUILabel.GenColor(0, 82, 112), new Vector2(0f, 0f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel.BackColorAlpha = 0f;
		gUIStrokeTextLabel.Bold = true;
		gUIStrokeTextLabel.Text = "#SQ_CHALLENGE_PROGRESS";
		progressContainer.Add(gUIStrokeTextLabel);
		progressContainer.IsVisible = false;
		Add(progressContainer);
		rewardCollectedContainer = new GUISimpleControlWindow();
		rewardCollectedContainer.Id = "rewardCollectedWindow";
		rewardCollectedContainer.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(240f, -188f), new Vector2(454f, 161f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		rewardCollectedContainer.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		GUIImage gUIImage5 = new GUIImage();
		gUIImage5.Id = "rewardCollectedWindowBackground";
		gUIImage5.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(454f, 161f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage5.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_p1_reward_complete";
		rewardCollectedContainer.Add(gUIImage5);
		rewardCollectedLabel = new GUIStrokeTextLabel();
		rewardCollectedLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(85f, 50f), new Vector2(200f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		rewardCollectedLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, GUILabel.GenColor(255, 217, 85), GUILabel.GenColor(0, 82, 112), GUILabel.GenColor(0, 82, 112), new Vector2(0f, 0f), TextAnchor.MiddleCenter);
		rewardCollectedLabel.BackColorAlpha = 0f;
		rewardCollectedLabel.Bold = true;
		rewardCollectedLabel.Text = "#SQ_CHALLENGE_PROGRESS";
		rewardCollectedContainer.Add(rewardCollectedLabel);
		rewardCollectedValueLabel = new GUIStrokeTextLabel();
		rewardCollectedValueLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(80f, 77f), new Vector2(200f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		rewardCollectedValueLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 20, GUILabel.GenColor(136, 242, 255), GUILabel.GenColor(1, 28, 66), GUILabel.GenColor(1, 28, 66), new Vector2(0f, 0f), TextAnchor.MiddleCenter);
		rewardCollectedValueLabel.BackColorAlpha = 0f;
		rewardCollectedValueLabel.Bold = true;
		rewardCollectedValueLabel.Text = "#SQ_CHALLENGE_PROGRESS";
		rewardCollectedContainer.Add(rewardCollectedValueLabel);
		rewardCollectedContainer.IsVisible = false;
		Add(rewardCollectedContainer);
		rewardIcon = new SHSMySquadRewardIcon();
		rewardIcon.Id = "rewardIcon";
		rewardIcon.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(385f, -175f), new Vector2(200f, 200f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		rewardIcon.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		rewardIcon.IsVisible = false;
		Add(rewardIcon);
		rewardTextWindow = new SHSMySquadRewardTextbox();
		rewardTextWindow.Id = "rewardTextWindow";
		rewardTextWindow.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(385f, -200f), new Vector2(112f, 42f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		rewardTextWindow.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		rewardTextWindow.IsVisible = false;
		Add(rewardTextWindow);
		progressWindow = new GUISimpleControlWindow();
		progressWindow.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(190f, -146f), new Vector2(285f, 161f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Add(progressWindow);
		challengeCompleteBanner = new GUIDrawTexture();
		challengeCompleteBanner.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		challengeCompleteBanner.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(240f, -250f), new Vector2(462f, 245f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		challengeCompleteBanner.TextureSource = "mysquadgadget_bundle|L_mshs_mysquad_challenge_p1_challenge_complete";
		challengeCompleteBanner.IsVisible = false;
		challengeCompleteBanner.Id = "challengeCompleteBanner";
		challengeCompleteBanner.MouseOver += delegate
		{
			AnimClip newPiece2 = SHSAnimations.Generic.FadeOut(challengeCompleteBanner, 0.5f);
			base.AnimationPieceManager.SwapOut(ref animClipHackRef, newPiece2);
		};
		challengeCompleteBanner.MouseOut += delegate
		{
			AnimClip newPiece = SHSAnimations.Generic.FadeIn(challengeCompleteBanner, 0.5f);
			base.AnimationPieceManager.SwapOut(ref animClipHackRef, newPiece);
		};
		nextChallengeControl = new GUISimpleControlWindow();
		nextChallengeControl.Id = "nextChallengeControl";
		nextChallengeControl.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		nextChallengeControl.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(356f, -314f), new Vector2(256f, 256f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		nextChallengeButton = new GUIButton();
		nextChallengeButton.Id = "nextChallengeButton";
		nextChallengeButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|mshs_mysquad_challenge_smaller_next_button");
		nextChallengeButton.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(256f, 256f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		nextChallengeButton.HitTestType = HitTestTypeEnum.Alpha;
		nextChallengeButton.MouseDown += delegate
		{
			AppShell.Instance.ChallengeManager.SetViewedChallenge((int)nextChallengeButton.Tag, null);
		};
		nextChallengeControl.Add(nextChallengeButton);
		GUIStrokeTextLabel gUIStrokeTextLabel2 = new GUIStrokeTextLabel();
		gUIStrokeTextLabel2.Id = "nextChallengeLabel";
		gUIStrokeTextLabel2.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(63, 86, 0), GUILabel.GenColor(120, 170, 4), new Vector2(2f, 3f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel2.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, -3f), new Vector2(152f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel2.Text = "#SQ_NEXT_CHALLENGE";
		nextChallengeControl.Add(gUIStrokeTextLabel2);
		Add(challengeCompleteBanner);
		Add(nextChallengeControl);
		collectRewardWindow = new GUISimpleControlWindow();
		collectRewardWindow.Id = "collectRewardWindow";
		collectRewardWindow.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(385f, -136f), new Vector2(256f, 256f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIButton gUIButton = new GUIButton();
		gUIButton.Id = "collectRewardButton";
		gUIButton.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(256f, 256f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|L_mshs_mysquad_challenge_p1_collectreward");
		gUIButton.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton.MouseDown += delegate
		{
			AppShell.Instance.EventMgr.Fire(this, new SHSMySquadChallengeTabStrip.TabChangedEvent(SHSMySquadChallengeTabStrip.TabType.Challenges));
		};
		collectRewardWindow.Add(gUIButton);
		serumAnimation = new LargeSerumAnimation();
		serumAnimation.Id = "serumImage";
		serumAnimation.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(23f, 23f), new Vector2(200f, 200f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		collectRewardWindow.Add(serumAnimation);
		collectRewardWindow.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		collectRewardWindow.IsVisible = false;
		Add(collectRewardWindow);
		GUIDropShadowTextLabel gUIDropShadowTextLabel = new GUIDropShadowTextLabel();
		gUIDropShadowTextLabel.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(50f, -151f), new Vector2(70f, 20f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIDropShadowTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(223, 203, 68), GUILabel.GenColor(0, 23, 66), new Vector2(1f, 1f), TextAnchor.MiddleLeft);
		gUIDropShadowTextLabel.Bold = true;
		gUIDropShadowTextLabel.Text = "#SQ_MYSQUAD_GOLD";
		gUIDropShadowTextLabel.Id = "goldLabel";
		Add(gUIDropShadowTextLabel);
		goldValue = new GUIDropShadowTextLabel();
		goldValue.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(55f, -151f), new Vector2(200f, 20f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		goldValue.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(223, 203, 68), GUILabel.GenColor(0, 23, 66), new Vector2(1f, 1f), TextAnchor.MiddleRight);
		goldValue.Bold = true;
		goldValue.Overflow = true;
		goldValue.Id = "goldValue";
		Add(goldValue);
		GUIDropShadowTextLabel gUIDropShadowTextLabel2 = new GUIDropShadowTextLabel();
		gUIDropShadowTextLabel2.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(41f, -125f), new Vector2(70f, 20f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIDropShadowTextLabel2.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(205, 227, 231), GUILabel.GenColor(0, 23, 66), new Vector2(1f, 1f), TextAnchor.MiddleLeft);
		gUIDropShadowTextLabel2.Bold = true;
		gUIDropShadowTextLabel2.Text = "#SQ_MYSQUAD_SILVER";
		gUIDropShadowTextLabel2.Id = "silverLabel";
		Add(gUIDropShadowTextLabel2);
		silverValue = new GUIDropShadowTextLabel();
		silverValue.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(55f, -125f), new Vector2(200f, 20f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		silverValue.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(205, 227, 231), GUILabel.GenColor(0, 23, 66), new Vector2(1f, 1f), TextAnchor.MiddleRight);
		silverValue.Bold = true;
		silverValue.Overflow = true;
		silverValue.Id = "silverValue";
		Add(silverValue);
		GUIDropShadowTextLabel gUIDropShadowTextLabel3 = new GUIDropShadowTextLabel();
		gUIDropShadowTextLabel3.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(32f, -99f), new Vector2(70f, 20f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIDropShadowTextLabel3.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(138, 195, 252), GUILabel.GenColor(0, 23, 66), new Vector2(1f, 1f), TextAnchor.MiddleLeft);
		gUIDropShadowTextLabel3.Bold = true;
		gUIDropShadowTextLabel3.Text = "#SQ_TAB_FRACTALS";
		gUIDropShadowTextLabel3.Size = new Vector2(90f, 20f);
		gUIDropShadowTextLabel3.Id = "ticketsLabel";
		Add(gUIDropShadowTextLabel3);
		ticketsValue = new GUIDropShadowTextLabel();
		ticketsValue.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(55f, -99f), new Vector2(200f, 20f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		ticketsValue.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(138, 195, 252), GUILabel.GenColor(0, 23, 66), new Vector2(1f, 1f), TextAnchor.MiddleRight);
		ticketsValue.Bold = true;
		ticketsValue.Overflow = true;
		ticketsValue.Id = "ticketsValue";
		Add(ticketsValue);
		if (!Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.IsPayingSubscriber))
		{
			GUIButton gUIButton2 = new GUIButton();
			gUIButton2.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(312f, 118f), new Vector2(512f, 512f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIButton2.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|L_mshs_mysquad_challenge_p1_subscribe");
			gUIButton2.HitTestType = HitTestTypeEnum.Alpha;
			gUIButton2.Click += delegate
			{
				LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.Subscribe);
			};
			gUIButton2.Id = "subscribeButton";
			Add(gUIButton2);
			GUIStrokeTextLabel gUIStrokeTextLabel3 = new GUIStrokeTextLabel();
			gUIStrokeTextLabel3.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(341f, -62f), new Vector2(124f, 100f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIStrokeTextLabel3.SetupText(GUIFontManager.SupportedFontEnum.Komica, 17, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(63, 86, 0), GUILabel.GenColor(63, 86, 0), new Vector2(2f, 2f), TextAnchor.UpperLeft);
			gUIStrokeTextLabel3.Text = "#SQ_MYSQUAD_SUBSCRIBE";
			gUIStrokeTextLabel3.VerticalKerning = 20;
			gUIStrokeTextLabel3.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Ignore;
			gUIStrokeTextLabel3.Id = "subscribeLabel";
			Add(gUIStrokeTextLabel3);
		}
		else
		{
			GUIButton gUIButton3 = new GUIButton();
			gUIButton3.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(313f, -6f), new Vector2(256f, 256f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIButton3.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|L_button_buygold_shoppingpopup");
			gUIButton3.HitTestType = HitTestTypeEnum.Alpha;
			gUIButton3.Id = "buyGoldButton";
			gUIButton3.Click += delegate
			{
				GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, "#MTX_BUMPER_MESSAGE", delegate(string Id, GUIDialogWindow.DialogState state)
				{
					if (state == GUIDialogWindow.DialogState.Ok)
					{
						ShsWebService.SafeJavaScriptCall("HEROUPNS.RedirectToStore();");
					}
				}, ModalLevelEnum.Default);
			};
			Add(gUIButton3);
		}
		BuyGoldControl = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(988f, 372f), new Vector2(260f, -214f));
		BuyGoldControl.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		GUIImage gUIImage6 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(230f, 97f), new Vector2(80f, 64f));
		gUIImage6.TextureSource = "shopping_bundle|buygold_popup_background";
		BuyGoldControl.Add(gUIImage6);
		GUIButton gUIButton4 = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(59f, 68f));
		gUIButton4.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|L_button_buygold_shoppingpopup");
		gUIButton4.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton4.Click += delegate
		{
			GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, "#MTX_BUMPER_MESSAGE", delegate(string Id, GUIDialogWindow.DialogState state)
			{
				if (state == GUIDialogWindow.DialogState.Ok)
				{
					ShsWebService.SafeJavaScriptCall("HEROUPNS.RedirectToStore();");
				}
			}, ModalLevelEnum.Default);
		};
		gUIButton4.ToolTip = new NamedToolTipInfo("#mtx_purchase", new Vector2(100f, 150f));
		BuyGoldControl.Add(gUIButton4);
		GUILabel gUILabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(145f, 150f), new Vector2(85f, 35f));
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, GUILabel.GenColor(255, 255, 255), TextAnchor.MiddleCenter);
		gUILabel.Text = "#SHOPPING_NEED_MORE_GOLD";
		gUILabel.VerticalKerning = 15;
		gUILabel.Bold = true;
		GUILabel gUILabel2 = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(145f, 150f), new Vector2(86f, 36f));
		gUILabel2.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, GUILabel.GenColor(138, 49, 15), TextAnchor.MiddleCenter);
		gUILabel2.Text = "#SHOPPING_NEED_MORE_GOLD";
		gUILabel2.VerticalKerning = 15;
		gUILabel2.Bold = true;
		BuyGoldControl.Add(gUILabel2);
		BuyGoldControl.Add(gUILabel);
		GUIButton gUIButton5 = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(48f, 48f), new Vector2(180f, 30f));
		gUIButton5.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|button_mysquad_close");
		gUIButton5.Click += delegate
		{
			BuyGoldControl.IsVisible = false;
		};
		gUIButton5.ToolTip = new NamedToolTipInfo("#TT_COMMON_1");
		BuyGoldControl.Add(gUIButton5);
		BuyGoldControl.IsVisible = false;
		Add(BuyGoldControl);
		GUIStrokeTextLabel gUIStrokeTextLabel4 = new GUIStrokeTextLabel();
		gUIStrokeTextLabel4.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(214f, -474f), new Vector2(400f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel4.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 27, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 65, 158), GUILabel.GenColor(0, 52, 128), new Vector2(2f, 3f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel4.BackColorAlpha = 0.75f;
		gUIStrokeTextLabel4.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUIStrokeTextLabel4.Text = dataManager.Profile.PlayerName;
		gUIStrokeTextLabel4.Id = "squadNameLabel";
		Add(gUIStrokeTextLabel4);
		squadTitleLabel = new GUIStrokeTextLabel();
		squadTitleLabel.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(174f, -505f), new Vector2(400f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		squadTitleLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 65, 158), GUILabel.GenColor(0, 52, 128), new Vector2(2f, 3f), TextAnchor.MiddleLeft);
		squadTitleLabel.BackColorAlpha = 0.75f;
		squadTitleLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		squadTitleLabel.Text = dataManager.Profile.Title;
		squadTitleLabel.Id = "squadTitleLabel";
		Add(squadTitleLabel);
		GUIButton gUIButton6 = new GUIButton();
		gUIButton6.Id = "changeTitleButton";
		gUIButton6.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|changetitle_icon");
		gUIButton6.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(-60f, -490f), new Vector2(64f, 64f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton6.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton6.MouseDown += delegate
		{
			chooseTitleHUD.IsVisible = true;
			chooseTitleHUD.SetPosition(TITLE_WINDOW_OPENED);
		};
		Add(gUIButton6);
		string text = AppShell.Instance.Profile.Medallion;
		if (text != string.Empty)
		{
			if (text.Contains("_normal"))
			{
				text = text.Substring(0, text.IndexOf("_normal"));
			}
		}
		else
		{
			text = "brawlergadget_bundle|brawler_gadget_powerON_placeholder";
		}
		changeMedallionButton = new GUIButton();
		changeMedallionButton.Id = "changeMedallionButton";
		changeMedallionButton.StyleInfo = new SHSButtonStyleInfo(text, SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		changeMedallionButton.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -480f), new Vector2(32f, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		changeMedallionButton.MouseDown += delegate
		{
			chooseMedallionHUD.IsVisible = true;
			chooseMedallionHUD.SetPosition(MEDALLION_WINDOW_OPENED);
		};
		Add(changeMedallionButton);
		chooseTitleHUD = new ChooseTitleHUD();
		chooseTitleHUD.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, TITLE_WINDOW_CLOSED);
		Add(chooseTitleHUD);
		chooseTitleHUD.IsVisible = false;
		chooseMedallionHUD = new ChooseMedallionHUD();
		chooseMedallionHUD.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, TITLE_WINDOW_CLOSED);
		Add(chooseMedallionHUD);
		chooseMedallionHUD.IsVisible = false;
		profileInitialized = true;
	}

	protected void OnChallengeManagerStateChanged(ChallengeManagerStateChangedMessage msg)
	{
		UpdateChallengeDisplay();
	}

	protected void UpdateChallengeDisplay()
	{
		nextChallengeControl.IsVisible = false;
		rewardIcon.IsVisible = false;
		rewardTextWindow.IsVisible = false;
		collectRewardWindow.IsVisible = false;
		challengeCompleteBanner.IsVisible = false;
		rewardCollectedContainer.IsVisible = false;
		progressContainer.IsVisible = false;
		for (int num = progressWindow.ControlList.Count - 1; num >= 0; num--)
		{
			progressWindow.Remove(progressWindow.ControlList[num]);
		}
		int num2 = 0;
		if (dataManager.Profile is LocalPlayerProfile)
		{
			num2 = UpdateChallengeDisplay(dataManager.Profile as LocalPlayerProfile);
		}
		else if (dataManager.Profile is RemotePlayerProfile)
		{
			num2 = UpdateChallengeDisplay(dataManager.Profile as RemotePlayerProfile);
		}
		int num3 = 0;
		int num4 = 0;
		float num5 = 0f;
		int[] levelsPerTier = LevelsPerTier;
		foreach (int num6 in levelsPerTier)
		{
			num4 += num6;
			num5 = 1f - (float)(num4 - num2) / (float)num6;
			if (num2 < num4)
			{
				break;
			}
			squadTierIcons[num3].IsVisible = true;
			num3++;
		}
		if (num3 > 0)
		{
			squadTierIconLarge.TextureSource = "mysquadgadget_bundle|tier_icon_" + num3.ToString();
			squadTierIconLarge.IsVisible = true;
		}
		squadLevelValue.Text = dataManager.SquadLevel.ToString();
		if (AppShell.Instance.ChallengeManager.MaxKnownChallenges > 0)
		{
			float num7 = (float)num3 / (float)LevelsPerTier.Length;
			num7 += num5 * 1f / (float)LevelsPerTier.Length;
			float num8 = Math.Max(0f, 1f - num7);
			StaticMeterOverlay staticMeterOverlay = rightmeterFillSolid;
			Vector2 size = rightmeterFillSolid.Size;
			staticMeterOverlay.SetSize(new Vector2(size.x, 439f * num8));
		}
	}

	private void skipCurrentChallenge()
	{
		ChallengeInfo challengeInfo = AppShell.Instance.ChallengeManager.ChallengeDictionary[AppShell.Instance.ChallengeManager.CurrentChallenge.Id];
		CspUtils.DebugLog("skipCurrentChallenge " + challengeInfo.ChallengeId + " " + AppShell.Instance.ChallengeManager.CurrentChallenge.Id);
		if (NewShoppingManager.CatalogOwnableMap.ContainsKey(challengeInfo.BypassItem))
		{
			NewShoppingManager.CatalogOwnableJson catalogOwnableJson = NewShoppingManager.CatalogOwnableMap[challengeInfo.BypassItem];
			if (catalogOwnableJson.price > AppShell.Instance.Profile.Gold)
			{
				BuyGoldControl.IsVisible = true;
				return;
			}
			ShoppingService.PurchaseItem(challengeInfo.BypassItem, catalogOwnableJson.catalog_ownable_id, 1, 0);
			ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("purchase_click_down"));
		}
		else
		{
			CspUtils.DebugLog("skipCurrentChallenge - could not find ownable " + challengeInfo.BypassItem + " for purchase");
		}
	}

	private int UpdateChallengeDisplay(RemotePlayerProfile profile)
	{
		if (Singleton<Entitlements>.instance.MaxCheck(Entitlements.EntitlementFlagEnum.MaxChallengeLevel, profile.CurrentChallenge))
		{
			if (AppShell.Instance.ChallengeManager.ChallengeDictionary.ContainsKey(profile.CurrentChallenge))
			{
				ChallengeInfo challengeInfo = AppShell.Instance.ChallengeManager.ChallengeDictionary[profile.CurrentChallenge];
				challengeTitle.Text = challengeInfo.Name;
				challengeDescription.Text = challengeInfo.Description;
				if (challengeInfo != null)
				{
					rewardIcon.Challenge = challengeInfo;
					rewardIcon.IsVisible = true;
					rewardIcon.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, new Vector2(385f, -156f));
				}
				string value = challengeInfo.Reward.value;
				string id = string.Empty;
				if (challengeInfo.Reward.rewardType == ChallengeRewardType.Gold)
				{
					id = "#DAILY_REWARDS_GOLD";
				}
				else if (challengeInfo.Reward.rewardType == ChallengeRewardType.Silver)
				{
					id = "#DAILY_REWARD_SILVER";
				}
				else if (challengeInfo.Reward.rewardType == ChallengeRewardType.Tickets)
				{
					id = "#DAILY_REWARD_TICKETS";
				}
				rewardCollectedValueLabel.Text = string.Format(AppShell.Instance.stringTable[id], value);
				rewardCollectedLabel.Text = "#SQ_CHALLENGE_REWARD";
			}
		}
		else
		{
			challengeTitle.Text = "#SQ_CHALLENGES_COMPLETED";
			challengeCompleteBanner.IsVisible = false;
			rewardCollectedLabel.Text = " ";
			rewardCollectedValueLabel.Text = "#SQ_ALL_CHALLENGES_COMPLETED";
		}
		rewardCollectedContainer.IsVisible = true;
		goldValue.Text = "---";
		silverValue.Text = "---";
		ticketsValue.Text = "---";
		int num = 0;
		for (int i = 0; i < RewardedHeroes.Length; i++)
		{
			if (dataManager.Profile.AvailableCostumes.ContainsKey(RewardedHeroes[i]) && num < hero_icons.Length)
			{
				hero_icons[num].TextureSource = "characters_bundle|token_" + RewardedHeroes[i];
				hero_icons[num].IsVisible = true;
				num++;
			}
		}
		return profile.LastChallenge;
	}

	private int UpdateChallengeDisplay(LocalPlayerProfile profile)
	{
		int result = 0;
		skipChallengeControl.IsVisible = false;
		ChallengeManager.ChallengeManagerStateEnum currentState;
		IChallenge challenge;
		ChallengeInfo challengeInfo;
		AppShell.Instance.ChallengeManager.GetChallengeManagerDisplayInfo(out currentState, out challenge, out challengeInfo);
		switch (currentState)
		{
		case ChallengeManager.ChallengeManagerStateEnum.Uninitialized:
		case ChallengeManager.ChallengeManagerStateEnum.Inactive:
			challengeCompleteBanner.IsVisible = false;
			break;
		case ChallengeManager.ChallengeManagerStateEnum.ChallengeInProgress:
			if (challenge == null)
			{
				break;
			}
			currentChallengeLabel.Text = string.Format(AppShell.Instance.stringTable["#SQ_MYSQUAD_CHALLENGEITEM_TITLE"], challenge.Id);
			challengeTitle.Text = challenge.Name;
			challengeDescription.Text = challenge.Description;
			challengeDescription.IsVisible = true;
			challengeCompleteBanner.IsVisible = false;
			result = challenge.Id - 1;
			challengeInfo = AppShell.Instance.ChallengeManager.ChallengeDictionary[challenge.Id];
			if (challengeInfo != null)
			{
				rewardIcon.Challenge = challengeInfo;
				rewardIcon.IsVisible = true;
				rewardIcon.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, new Vector2(385f, -175f));
				if (challengeInfo.Reward.rewardType != ChallengeRewardType.Hero)
				{
					rewardTextWindow.Text = GetRewardText(challengeInfo);
					rewardTextWindow.IsVisible = true;
				}
				progressContainer.IsVisible = true;
				AppShell.Instance.ChallengeManager.GetChallengeProgressWindow(progressWindow, challenge.Id, true);
				NewShoppingManager.CatalogOwnableJson catalogOwnableJson = null;
				if (NewShoppingManager.CatalogOwnableMap.ContainsKey(challengeInfo.BypassItem))
				{
					catalogOwnableJson = NewShoppingManager.CatalogOwnableMap[challengeInfo.BypassItem];
				}
				skipChallengeControl.IsVisible = true;
				skipChallengeButtonLabel.Text = string.Format(AppShell.Instance.stringTable["#SKIP_CHALLENGE_LABEL"], catalogOwnableJson.price);
				if (catalogOwnableJson != null)
				{
					skipChallengeControl.ToolTip = new NamedToolTipInfo("Skip this challenge for " + catalogOwnableJson.price + " gold", new Vector2(100f, 60f));
				}
				else
				{
					skipChallengeControl.ToolTip = new NamedToolTipInfo("Skip this challenge", new Vector2(100f, 60f));
				}
			}
			break;
		case ChallengeManager.ChallengeManagerStateEnum.ChallengeDisplayPending:
			currentChallengeLabel.Text = string.Format(AppShell.Instance.stringTable["#SQ_MYSQUAD_CHALLENGEITEM_TITLE"], challengeInfo.ChallengeId);
			challengeTitle.Text = challengeInfo.Name;
			challengeDescription.Text = challengeInfo.Description;
			challengeDescription.IsVisible = true;
			challengeCompleteBanner.IsVisible = true;
			result = challengeInfo.ChallengeId;
			nextChallengeButton.Tag = challengeInfo.ChallengeId;
			rewardCollectedContainer.IsVisible = true;
			rewardIcon.Challenge = challengeInfo;
			rewardIcon.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, new Vector2(385f, -156f));
			if (challengeInfo.Reward.grantMode == ChallengeGrantMode.Auto)
			{
				nextChallengeControl.IsVisible = true;
				rewardIcon.IsVisible = true;
				if (challengeInfo.Reward.rewardType == ChallengeRewardType.Hero)
				{
					rewardCollectedLabel.Text = "#SQ_HERO_COLLECTED";
					rewardCollectedValueLabel.Text = AppShell.Instance.CharacterDescriptionManager[challengeInfo.Reward.qualifier].CharacterName;
				}
				else
				{
					rewardCollectedLabel.Text = "#SQ_REWARD_COLLECTED";
					rewardCollectedValueLabel.Text = GetRewardText(challengeInfo);
				}
			}
			else
			{
				rewardCollectedLabel.Text = "#SQ_REWARD_COLLECTED";
				rewardCollectedValueLabel.Text = "#SQ_USE_SUPER_HERO_SERUM";
				collectRewardWindow.IsVisible = true;
				serumAnimation.BeginSerumDrip();
			}
			break;
		case ChallengeManager.ChallengeManagerStateEnum.ChallengeVerificationPending:
			currentChallengeLabel.Text = string.Empty;
			challengeTitle.Text = "#SQ_VERIFYING_CHALLENGE";
			challengeDescription.IsVisible = false;
			challengeCompleteBanner.IsVisible = false;
			rewardCollectedContainer.IsVisible = false;
			break;
		case ChallengeManager.ChallengeManagerStateEnum.AllChallengesCompleted:
			currentChallengeLabel.Text = string.Empty;
			challengeTitle.Text = "#SQ_CHALLENGES_COMPLETED";
			challengeDescription.IsVisible = false;
			result = AppShell.Instance.ChallengeManager.MaxKnownChallenges;
			challengeCompleteBanner.IsVisible = false;
			rewardCollectedLabel.Text = " ";
			rewardCollectedValueLabel.Text = "#SQ_ALL_CHALLENGES_COMPLETED";
			rewardCollectedContainer.IsVisible = true;
			break;
		default:
			challengeCompleteBanner.IsVisible = false;
			break;
		}
		goldValue.Text = dataManager.Profile.Gold.ToString();
		silverValue.Text = dataManager.Profile.Silver.ToString();
		ticketsValue.Text = dataManager.Profile.Shards.ToString();
		int num = 0;
		for (int i = 0; i < RewardedHeroes.Length; i++)
		{
			if (dataManager.Profile.AvailableCostumes.ContainsKey(RewardedHeroes[i]) && num < hero_icons.Length)
			{
				ChallengeInfo heroChallenge = AppShell.Instance.ChallengeManager.GetHeroChallenge(RewardedHeroes[i]);
				if (heroChallenge == null || AppShell.Instance.ChallengeManager.LastViewedChallengeId >= heroChallenge.ChallengeId - 1)
				{
					hero_icons[num].TextureSource = "characters_bundle|token_" + RewardedHeroes[i];
					hero_icons[num].IsVisible = true;
					num++;
				}
			}
		}
		return result;
	}

	protected string GetRewardText(ChallengeInfo info)
	{
		string value = info.Reward.value;
		string id = string.Empty;
		if (info.Reward.rewardType == ChallengeRewardType.Gold)
		{
			id = "#DAILY_REWARDS_GOLD";
		}
		else if (info.Reward.rewardType == ChallengeRewardType.Silver)
		{
			id = "#DAILY_REWARD_SILVER";
		}
		else if (info.Reward.rewardType == ChallengeRewardType.Tickets)
		{
			id = "#DAILY_REWARD_TICKETS";
		}
		return string.Format(AppShell.Instance.stringTable[id], value);
	}

	public override void Update()
	{
		base.Update();
		if (!profileInitialized && dataManager.Profile != null)
		{
			InitializeWindow();
			OnProfileInitialization();
		}
	}
}
