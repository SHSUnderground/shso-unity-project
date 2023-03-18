using System;
using UnityEngine;

public class SHSBrawlerScoreWindow : GUIControlWindow
{
	public static int globalScore = 0;

	protected const int TOTAL_MEDALS = 4;

	public int[] scoreTargets;

	protected int currentMedalLevel;

	protected int toNextMedal = 1;

	protected int scoreOffset;

	protected int currentScore;

	private GUIImage bgd;

	private GUIImage[] medalGoals;

	private GUIImage[] medals;

	private GUIStrokeTextLabel survivalTimeLabel;

	private GUIStrokeTextLabel currentScoreLabel;

	private GUIAlphaCutoffDrawTexture scoreBar;

	private GUIImage littleSparkles;

	private GUIImage bigSparkles;

	private GUIImage medalGlow;

	private GUIImage medalRays;

	private string[] medalNames = new string[4]
	{
		"bronze",
		"silver",
		"gold",
		"adamantium"
	};

	private Vector2 dotLocation = new Vector2(134f, 123f);

	private Vector2[] medalSizes = new Vector2[4]
	{
		new Vector2(68f, 74f),
		new Vector2(73f, 72f),
		new Vector2(76f, 74f),
		new Vector2(87f, 82f)
	};

	private float startTime;

	private static bool shouldUpdate = true;

	public SHSBrawlerScoreWindow()
	{
		scoreTargets = new int[4];
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		SetSize(176f, 176f);
		bgd = new GUIImage();
		bgd.SetPositionAndSize(new Vector2(0f, 0f), new Vector2(175f, 175f));
		bgd.TextureSource = "brawler_bundle|score_bar_bg";
		bgd.Color = new Color(1f, 1f, 1f, 1f);
		bgd.IsVisible = true;
		Add(bgd);
		scoreBar = new GUIAlphaCutoffDrawTexture();
		scoreBar.SetPositionAndSize(new Vector2(0f, 0f), new Vector2(175f, 175f));
		scoreBar.TextureSource = "brawler_bundle|score_bar_" + medalNames[0];
		scoreBar.AlphaCutoffTextureSource = "brawler_bundle|score_bar_mask";
		scoreBar.Color = new Color(1f, 1f, 1f, 1f);
		scoreBar.IsVisible = true;
		scoreBar.AlphaCutoff = 1f;
		Add(scoreBar);
		medalGoals = new GUIImage[4];
		for (int i = 0; i < 4; i++)
		{
			GUIImage gUIImage = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(18f, 18f), dotLocation);
			gUIImage.Anchor = AnchorAlignmentEnum.Middle;
			gUIImage.TextureSource = "brawler_bundle|score_dot_" + medalNames[i];
			gUIImage.IsVisible = false;
			Add(gUIImage);
			medalGoals[i] = gUIImage;
		}
		littleSparkles = new GUIImage();
		littleSparkles.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, new Vector2(175f, 175f));
		littleSparkles.TextureSource = "brawler_bundle|score_medal_sparkles";
		littleSparkles.IsVisible = false;
		Add(littleSparkles);
		bigSparkles = new GUIImage();
		bigSparkles.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, new Vector2(175f, 175f));
		bigSparkles.TextureSource = "brawler_bundle|score_medal_sparkles";
		bigSparkles.IsVisible = false;
		Add(bigSparkles);
		medalGlow = new GUIImage();
		medalGlow.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, new Vector2(175f, 175f));
		medalGlow.TextureSource = "brawler_bundle|score_medal_glow";
		medalGlow.IsVisible = false;
		Add(medalGlow);
		medalRays = new GUIImage();
		medalRays.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, new Vector2(175f, 175f));
		medalRays.TextureSource = "brawler_bundle|score_medal_rays";
		medalRays.IsVisible = false;
		Add(medalRays);
		medals = new GUIImage[4];
		for (int i = 0; i < 4; i++)
		{
			GUIImage gUIImage2 = GUIControl.CreateControlAbsolute<GUIImage>(medalSizes[i], new Vector2(0f, 0f));
			gUIImage2.Anchor = AnchorAlignmentEnum.Middle;
			gUIImage2.Docking = DockingAlignmentEnum.Middle;
			gUIImage2.TextureSource = "brawler_bundle|score_medal_" + medalNames[i];
			gUIImage2.Color = Color.white;
			gUIImage2.Alpha = 1f;
			gUIImage2.IsVisible = false;
			Add(gUIImage2);
			medals[i] = gUIImage2;
		}
		ActiveMission activeMission = (ActiveMission)AppShell.Instance.SharedHashTable["ActiveMission"];
		if (activeMission != null && activeMission.IsSurvivalMode)
		{
			startTime = Time.time;
			survivalTimeLabel = new GUIStrokeTextLabel();
			survivalTimeLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 249, 157), GUILabel.GenColor(0, 23, 67), GUILabel.GenColor(0, 23, 67), new Vector2(3f, 5f), TextAnchor.MiddleCenter);
			survivalTimeLabel.SetPositionAndSize(new Vector2(0f, 0f), new Vector2(175f, 155f));
			survivalTimeLabel.Text = "00:00:00";
			Add(survivalTimeLabel);
		}
		currentScoreLabel = new GUIStrokeTextLabel();
		currentScoreLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 249, 157), GUILabel.GenColor(0, 23, 67), GUILabel.GenColor(0, 23, 67), new Vector2(3f, 5f), TextAnchor.MiddleCenter);
		currentScoreLabel.SetPositionAndSize(new Vector2(0f, 0f), new Vector2(175f, 285f));
		currentScoreLabel.Text = "0";
		Add(currentScoreLabel);
	}

	public override void Update()
	{
		base.Update();
		if (shouldUpdate)
		{
			ActiveMission activeMission = (ActiveMission)AppShell.Instance.SharedHashTable["ActiveMission"];
			if (activeMission != null && activeMission.IsSurvivalMode)
			{
				float num = Time.time - startTime;
				TimeSpan timeSpan = TimeSpan.FromSeconds(num);
				survivalTimeLabel.Text = string.Format("{0}:{1}:{2}", timeSpan.Hours.ToString("D2"), timeSpan.Minutes.ToString("D2"), Math.Max(0, timeSpan.Seconds).ToString("D2"));
			}
		}
		shouldUpdate = !shouldUpdate;
	}

	public void GetScoreTargets(int playerCount)
	{
		ActiveMission activeMission = (ActiveMission)AppShell.Instance.SharedHashTable["ActiveMission"];
		if (activeMission == null || activeMission.MissionDefinition == null)
		{
			for (int i = 0; i < 4; i++)
			{
				scoreTargets[i] = i * 500;
			}
		}
		else
		{
			for (int j = 1; j <= 3; j++)
			{
				scoreTargets[j] = activeMission.MissionDefinition.ScoreNeededForRating((MissionDefinition.Ratings)j, playerCount);
			}
		}
		UpdateScore(currentScore, true);
	}

	public void UpdateScore(int newScore, bool initialize)
	{
		currentScore = newScore;
		globalScore = currentScore;  // added by CSP
		currentScoreLabel.Text = string.Format("{0:n0}", currentScore);
		int num = Mathf.CeilToInt((float)newScore * 0.97f);
		int i;
		for (i = 0; i < 4 && num >= scoreTargets[i]; i++)
		{
		}
		i--;
		scoreOffset = scoreTargets[i];
		if (i + 1 < 4)
		{
			toNextMedal = scoreTargets[i + 1] - scoreOffset;
		}
		if (initialize)
		{
			medals[currentMedalLevel].IsVisible = false;
			currentMedalLevel = i;
			scoreBar.TextureSource = "brawler_bundle|score_bar_" + medalNames[currentMedalLevel];
			medals[currentMedalLevel].IsVisible = true;
			if (i + 1 < medalGoals.Length)
			{
				medalGoals[i + 1].IsVisible = true;
			}
		}
		else if (currentMedalLevel != i)
		{
			base.AnimationPieceManager.Add(SHSAnimations.Generic.FadeOut(medals[currentMedalLevel], 0.33f) | SHSAnimations.Generic.ChangeVisibility(false, medals[currentMedalLevel]));
			medals[i].Alpha = 1f;
			base.AnimationPieceManager.Add(SHSAnimations.Generic.Wait(0.25f) | (SHSAnimations.Generic.ChangeVisibility(true, medals[i]) ^ AnimClipBuilder.Absolute.Rotation(AnimClipBuilder.Path.Linear(-15f, 0f, 0.5f), medals[i]) ^ SHSAnimations.Generic.AnimationBounceTransitionIn(medalSizes[i], 1f, medals[i])));
			medalGoals[i].IsVisible = false;
			if (i + 1 < medalGoals.Length)
			{
				medalGoals[i + 1].IsVisible = true;
			}
			if (i > currentMedalLevel)
			{
				littleSparkles.SetSize(new Vector2(43f, 43f));
				bigSparkles.SetSize(new Vector2(43f, 43f));
				base.AnimationPieceManager.Add(SHSAnimations.Generic.Wait(0.25f) | (SHSAnimations.Generic.ChangeVisibility(true, littleSparkles, bigSparkles) ^ (SHSAnimations.Generic.FadeIn(littleSparkles, 0.25f) ^ SHSAnimations.Generic.FadeIn(bigSparkles, 0.25f)) ^ AnimClipBuilder.Absolute.Rotation(AnimClipBuilder.Path.Linear(-15f, 15f, 1.25f), littleSparkles) ^ AnimClipBuilder.Absolute.Rotation(AnimClipBuilder.Path.Linear(-15f, 35f, 1.25f), bigSparkles) ^ ((SHSAnimations.Generic.ChangeSizeDirect(littleSparkles, new Vector2(175f, 175f), new Vector2(43f, 43f), 1f, 0f) ^ SHSAnimations.Generic.ChangeSizeDirect(bigSparkles, new Vector2(131f, 131f), new Vector2(43f, 43f), 1f, 0f)) | (SHSAnimations.Generic.FadeOut(littleSparkles, 0.25f) ^ SHSAnimations.Generic.FadeOut(bigSparkles, 0.25f)))) | SHSAnimations.Generic.ChangeVisibility(false, littleSparkles, bigSparkles));
				medalRays.SetSize(new Vector2(88f, 88f));
				base.AnimationPieceManager.Add(SHSAnimations.Generic.Wait(0.33f) | (SHSAnimations.Generic.ChangeVisibility(true, medalRays) ^ SHSAnimations.Generic.FadeIn(medalRays, 0.33f) ^ AnimClipBuilder.Absolute.Rotation(AnimClipBuilder.Path.Linear(-60f, 0f, 1.4f), medalRays) ^ (SHSAnimations.Generic.ChangeSizeDirect(medalRays, new Vector2(175f, 175f), new Vector2(88f, 88f), 1.2f, 0f) | SHSAnimations.Generic.FadeOut(medalRays, 0.2f))) | SHSAnimations.Generic.ChangeVisibility(false, medalRays));
				base.AnimationPieceManager.Add(SHSAnimations.Generic.Wait(0.4f) | SHSAnimations.Generic.ChangeVisibility(true, medalGlow) | SHSAnimations.Generic.FadeIn(medalGlow, 0.5f) | SHSAnimations.Generic.FadeOut(medalGlow, 0.5f) | SHSAnimations.Generic.ChangeVisibility(false, medalGlow));
			}
			if (i + 1 < medalGoals.Length)
			{
				base.AnimationPieceManager.Add(AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0.5f, 1f, 1f), medalGoals[i + 1]) ^ SHSAnimations.Generic.SizeChangeWithEndBounce(10f, 18f, 0.33f, 0.33f, medalGoals[i + 1]));
			}
			currentMedalLevel = i;
			scoreBar.TextureSource = "brawler_bundle|score_bar_" + medalNames[currentMedalLevel];
		}
		float num2 = (float)(num - scoreOffset) / (float)toNextMedal;
		scoreBar.AlphaCutoff = 1f - num2;
	}

	public string GetCurrentMedalName()
	{
		if (currentMedalLevel >= 0 && currentMedalLevel < 4)
		{
			return medalNames[currentMedalLevel];
		}
		CspUtils.DebugLog("unable to retrieve current medal name due to invalid current medal level");
		return string.Empty;
	}
}
