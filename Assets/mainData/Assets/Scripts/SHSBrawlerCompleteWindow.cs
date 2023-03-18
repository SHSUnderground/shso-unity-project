using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SHSBrawlerCompleteWindow : GUIControlWindow
{
	private enum MissionResultStep
	{
		ShowWindow,
		ReadData,
		SpecialBonus,
		KoBonus,
		ComboBonus,
		SurvivalBonus,
		Rewards,
		RewardsEffect,
		LevelUp,
		Finish
	}

	protected class playerNameScoreWindow : GUIControlWindow
	{
		private const int SCORE_FONT_SIZE = 31;

		private GUIImage namePlate;

		public GUILabel nameText;

		private GUILabel scoreShadow;

		private GUILabel scoreText;

		public long netOwnerID;

		public MissionResults playerScore;

		public playerNameScoreWindow()
		{
			Vector2 vector = new Vector2(-48f, -28f);
			namePlate = GUIControl.CreateControlBottomFrame<GUIImage>(new Vector2(192f, 33f), Vector2.zero);
			namePlate.TextureSource = "brawler_bundle|mc_name_bubble";
			ChangeAlignment(namePlate, AnchorAlignmentEnum.Middle);
			Add(namePlate);
			nameText = GUIControl.CreateControlBottomFrame<GUILabel>(new Vector2(192f, 33f), Vector2.zero);
			nameText.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, Utils.ColorFromBytes(58, 71, 94, byte.MaxValue), TextAnchor.UpperCenter);
			nameText.Text = string.Empty;
			Add(nameText);
			scoreShadow = GUIControl.CreateControlBottomFrame<GUILabel>(new Vector2(192f, 48f), vector - new Vector2(2f, -2f));
			scoreShadow.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 31, Utils.ColorFromBytes(0, 0, 0, 128), TextAnchor.MiddleRight);
			scoreShadow.Text = string.Empty;
			Add(scoreShadow);
			scoreText = GUIControl.CreateControlBottomFrame<GUILabel>(new Vector2(192f, 48f), vector);
			scoreText.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 31, Utils.ColorFromBytes(byte.MaxValue, 249, 157, byte.MaxValue), TextAnchor.MiddleRight);
			scoreText.Text = string.Empty;
			Add(scoreText);
		}

		public void SetScore(int scoreValue)
		{
			scoreShadow.Text = scoreValue.ToString();
			scoreText.Text = scoreValue.ToString();
		}

		public void SetPlayerName(string playerName)
		{
			nameText.Text = playerName;
		}

		public void AcquireResults(MissionResults ScoreForPlayer)
		{
			playerScore = ScoreForPlayer;
		}

		public AnimClip AnimateIn()
		{
			namePlate.SetSize(0f, 0f);
			nameText.Alpha = 0f;
			return SHSAnimations.Generic.AnimationBounceTransitionIn(new Vector2(192f, 33f), 1f, namePlate) ^ SHSAnimations.Generic.FadeIn(nameText, 0.25f);
		}

		public AnimClip CountUp(int startScore, int finalScore, float duration)
		{
			int scoreToAdd = finalScore - startScore;
			if ((float)scoreToAdd / 800f < duration)
			{
				duration = (float)scoreToAdd / 800f;
			}
			if (duration <= 0f)
			{
				duration = 0.01f;
			}
			Vector2 vector = new Vector2(-48f, -28f);
			float num = 1f;
			scoreShadow.Scale = num;
			scoreText.Scale = num;
			scoreShadow.FontSize = Mathf.CeilToInt(num * 31f);
			scoreText.FontSize = Mathf.CeilToInt(num * 31f);
			scoreShadow.Offset = vector - new Vector2(2f, -2f);
			scoreText.Offset = vector;
			SetScore(0);
			scoreShadow.Alpha = 1f;
			scoreText.Alpha = 1f;
			
			SetScore(finalScore);  // added by CSP for testing.
			
			return SHSAnimations.Generic.ChangeVisibility(true, scoreText, scoreShadow) | new AnimClipFunction(duration, delegate(float totalTime)
			{
				int num2 = Mathf.CeilToInt((float)scoreToAdd * (totalTime / duration));
				if (num2 > finalScore)
				{
					num2 = finalScore;
				}
				SetScore(num2 + startScore);
			});
		}

		public AnimClip ShowOff(float scaleDuration)
		{
			return new AnimClipFunction(scaleDuration, delegate(float totalTime)
			{
				float num = Mathf.Min(totalTime / scaleDuration, 1f);
				Vector2 vector = new Vector2(-48f, -28f) * (1f - num) + new Vector2(-48f, -64f) * num;
				float num2 = 1f + num * 0.5f;
				scoreShadow.Scale = num2;
				scoreText.Scale = num2;
				scoreShadow.FontSize = Mathf.CeilToInt(num2 * 31f);
				scoreText.FontSize = Mathf.CeilToInt(num2 * 31f);
				scoreShadow.Offset = vector - new Vector2(2f, -2f);
				scoreText.Offset = vector;
			});
		}

		public AnimClip FadeEverything(float duration)
		{
			return FadeScore(duration) ^ SHSAnimations.Generic.FadeOut(namePlate, duration) ^ SHSAnimations.Generic.FadeOut(nameText, duration);
		}

		public AnimClip FadeScore(float duration)
		{
			return SHSAnimations.Generic.FadeOut(scoreText, duration) ^ SHSAnimations.Generic.FadeOut(scoreShadow, duration);
		}
	}

	protected class OwnableWindow : GUIControlWindow
	{
		private GUIImage backgroundLeft;

		private GUIImage backgroundMiddle;

		private GUIImage backgroundRight;

		private GUIImage rewardPlate;

		private GUIImage rewardImage;

		private GUILabel rewardShadow;

		private GUILabel rewardAmount;

		private GUILabel labelShadow;

		private GUILabel rewardLabel;

		private int rewardTotal;

		private ShsAudioSourceList sounds;

		private bool useLargeCounter;

		private static Vector2 rewardLabelSize = new Vector2(130f, 100f);

		private Vector2 REWARD_IMAGE_SIZE = new Vector2(100f, 100f);

		public OwnableWindow()
		{
			rewardPlate = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(300f, 100f), Vector2.zero);
			rewardPlate.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
			Add(rewardPlate);
			rewardImage = GUIControl.CreateControlFrameCentered<GUIImage>(REWARD_IMAGE_SIZE, new Vector2(-80f, 0f));
			rewardImage.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
			Add(rewardImage);
			Vector2 vector = new Vector2(108f, -2f);
			rewardShadow = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(300f, 100f), vector - new Vector2(2f, -2f));
			rewardShadow.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 38, Utils.ColorFromBytes(0, 21, 0, 128), TextAnchor.MiddleLeft);
			rewardShadow.Text = string.Empty;
			Add(rewardShadow);
			rewardAmount = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(300f, 100f), vector);
			rewardAmount.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 38, Utils.ColorFromBytes(byte.MaxValue, 249, 157, byte.MaxValue), TextAnchor.MiddleLeft);
			rewardAmount.Text = string.Empty;
			Add(rewardAmount);
			Vector2 vector2 = new Vector2(160f, 1f);
			labelShadow = GUIControl.CreateControlFrameCentered<GUILabel>(rewardLabelSize, vector2 - new Vector2(2f, -2f));
			labelShadow.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 20, Utils.ColorFromBytes(0, 21, 0, 128), TextAnchor.MiddleLeft);
			labelShadow.WordWrap = true;
			labelShadow.Text = string.Empty;
			Add(labelShadow);
			rewardLabel = GUIControl.CreateControlFrameCentered<GUILabel>(rewardLabelSize, vector2);
			rewardLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 20, Utils.ColorFromBytes(byte.MaxValue, 249, 157, byte.MaxValue), TextAnchor.MiddleLeft);
			rewardLabel.WordWrap = true;
			rewardLabel.Text = string.Empty;
			Add(rewardLabel);
		}

		public void SetBonusString(string str)
		{
			Vector2 offset = new Vector2(190f, 36f);
			GUILabel gUILabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(320f, 130f), offset);
			gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 14, Utils.ColorFromBytes(byte.MaxValue, 249, 157, byte.MaxValue), TextAnchor.MiddleLeft);
			gUILabel.Text = str;
			Add(gUILabel);
		}

		public void SetReward(int reward, string label, string rewardType, string toolTipText)
		{
			rewardPlate.TextureSource = "brawler_bundle|mc_ownable";
			rewardImage.TextureSource = rewardType;
			rewardPlate.ToolTip = new NamedToolTipInfo(toolTipText);
			rewardShadow.Text = reward.ToString();
			rewardAmount.Text = rewardShadow.Text;
			rewardTotal = reward;
			labelShadow.Text = label;
			rewardLabel.Text = label;
			GUIContent content = new GUIContent(rewardAmount.Text);
			Rect position = new Rect(0f, 0f, rewardLabelSize.x, rewardLabelSize.y);
			Vector2 cursorPixelPosition = rewardAmount.Style.UnityStyle.GetCursorPixelPosition(position, content, rewardAmount.Text.Length);
			rewardLabel.Offset = new Vector2(54f + cursorPixelPosition.x, 6f);
			labelShadow.Offset = new Vector2(52f + cursorPixelPosition.x, 8f);
		}

		public void SetAudioElements(ShsAudioSourceList sounds, bool useLargeCounter)
		{
			this.sounds = sounds;
			this.useLargeCounter = useLargeCounter;
		}

		public AnimClip AnimateIn(float duration)
		{
			rewardAmount.Text = string.Empty;
			rewardShadow.Text = string.Empty;
			AnimClip pieceOne = SHSAnimations.Generic.ChangeVisibility(true, this);
			pieceOne |= (AnimClip)new AnimClipFunction(0f, delegate
			{
				ShsAudioSource.PlayAutoSound(sounds.GetSource("pop_up_small"));
			});
			pieceOne |= (SHSAnimations.Generic.AnimationBounceTransitionIn(new Vector2(300f, 100f), 1f, rewardPlate) ^ SHSAnimations.Generic.AnimationBounceTransitionIn(REWARD_IMAGE_SIZE, 1f, rewardImage) ^ SHSAnimations.Generic.AnimationBounceTransitionIn(rewardLabelSize, 1f, labelShadow) ^ SHSAnimations.Generic.AnimationBounceTransitionIn(rewardLabelSize, 1f, rewardLabel));
			pieceOne |= (AnimClip)new AnimClipFunction(0f, delegate
			{
			});
			pieceOne |= (AnimClip)new AnimClipFunction(duration, delegate(float totalTime)
			{
				float num = Mathf.Min(totalTime / duration, 1f);
				string text = Mathf.FloorToInt(num * (float)rewardTotal).ToString();
				if (useLargeCounter && rewardAmount.Text != text)
				{
					ShsAudioSource.PlayAutoSound(sounds.GetSource("counter_large"));
				}
				rewardAmount.Text = text;
				rewardShadow.Text = rewardAmount.Text;
			});
			pieceOne |= (AnimClip)new AnimClipFunction(0f, delegate
			{
			});
			return pieceOne | SHSAnimations.Generic.Wait(1f);
		}
	}

	protected class RewardWindow : GUIControlWindow
	{
		private GUIImage rewardPlate;

		private GUIImage rewardImage;

		private GUILabel rewardShadow;

		private GUILabel rewardAmount;

		private GUILabel labelShadow;

		private GUILabel rewardLabel;

		public int rewardTotal;

		private ShsAudioSourceList sounds;

		private bool useLargeCounter;

		public RewardWindow()
		{
			rewardPlate = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(300f, 100f), Vector2.zero);
			rewardPlate.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
			Add(rewardPlate);
			rewardImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(100f, 100f), new Vector2(-100f, 0f));
			rewardImage.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
			Add(rewardImage);
			Vector2 vector = new Vector2(108f, -2f);
			rewardShadow = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(300f, 100f), vector - new Vector2(2f, -2f));
			rewardShadow.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 38, Utils.ColorFromBytes(0, 21, 0, 128), TextAnchor.MiddleLeft);
			rewardShadow.Text = string.Empty;
			Add(rewardShadow);
			rewardAmount = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(300f, 100f), vector);
			rewardAmount.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 38, Utils.ColorFromBytes(byte.MaxValue, 249, 157, byte.MaxValue), TextAnchor.MiddleLeft);
			rewardAmount.Text = string.Empty;
			Add(rewardAmount);
			Vector2 vector2 = new Vector2(160f, 6f);
			labelShadow = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(300f, 100f), vector2 - new Vector2(2f, -2f));
			labelShadow.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 22, Utils.ColorFromBytes(0, 21, 0, 128), TextAnchor.MiddleLeft);
			labelShadow.Text = string.Empty;
			Add(labelShadow);
			rewardLabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(300f, 100f), vector2);
			rewardLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 22, Utils.ColorFromBytes(byte.MaxValue, 249, 157, byte.MaxValue), TextAnchor.MiddleLeft);
			rewardLabel.Text = string.Empty;
			Add(rewardLabel);
		}

		public void SetBonusString(string str)
		{
			Vector2 offset = new Vector2(160f, 36f);
			GUILabel gUILabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(300f, 100f), offset);
			gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 19, Utils.ColorFromBytes(byte.MaxValue, 249, 157, byte.MaxValue), TextAnchor.MiddleLeft);
			gUILabel.Text = str;
			Add(gUILabel);
		}

		public void SetReward(int reward, string label, string rewardType, string toolTipText)
		{
			if (rewardType.Contains("|"))
			{
				rewardPlate.TextureSource = "brawler_bundle|mc_ownable";
				rewardImage.TextureSource = rewardType;
			}
			else
			{
				rewardPlate.TextureSource = "brawler_bundle|mc_" + rewardType;
			}
			rewardPlate.ToolTip = new NamedToolTipInfo(toolTipText);
			rewardShadow.Text = reward.ToString();
			rewardAmount.Text = rewardShadow.Text;
			rewardTotal = reward;
			labelShadow.Text = label;
			rewardLabel.Text = label;
			GUIContent content = new GUIContent(rewardAmount.Text);
			Rect position = new Rect(0f, 0f, 300f, 100f);
			Vector2 cursorPixelPosition = rewardAmount.Style.UnityStyle.GetCursorPixelPosition(position, content, rewardAmount.Text.Length);
			rewardLabel.Offset = new Vector2(114f + cursorPixelPosition.x, 6f);
			labelShadow.Offset = new Vector2(112f + cursorPixelPosition.x, 8f);
		}

		public void SetAudioElements(ShsAudioSourceList sounds, bool useLargeCounter)
		{
			this.sounds = sounds;
			this.useLargeCounter = useLargeCounter;
		}

		public AnimClip AnimateIn(float duration)
		{
			rewardAmount.Text = string.Empty;
			rewardShadow.Text = string.Empty;
			AnimClip pieceOne = SHSAnimations.Generic.ChangeVisibility(true, this);
			pieceOne |= (AnimClip)new AnimClipFunction(0f, delegate
			{
				ShsAudioSource.PlayAutoSound(sounds.GetSource("pop_up_small"));
			});
			pieceOne |= (SHSAnimations.Generic.AnimationBounceTransitionIn(new Vector2(300f, 100f), 1f, rewardPlate) ^ SHSAnimations.Generic.AnimationBounceTransitionIn(new Vector2(300f, 100f), 1f, labelShadow) ^ SHSAnimations.Generic.AnimationBounceTransitionIn(new Vector2(300f, 100f), 1f, rewardLabel));
			pieceOne |= (AnimClip)new AnimClipFunction(0f, delegate
			{
				if (useLargeCounter)
				{
				}
			});
			pieceOne |= (AnimClip)new AnimClipFunction(duration, delegate(float totalTime)
			{
				float num = Mathf.Min(totalTime / duration, 1f);
				string text = Mathf.FloorToInt(num * (float)rewardTotal).ToString();
				if (useLargeCounter && rewardAmount.Text != text)
				{
					try { // CSP added try catch
					  ShsAudioSource.PlayAutoSound(sounds.GetSource("counter_large"));  
					} 
					catch (Exception e) {
					   CspUtils.DebugLog(e);
					} 
				}
				rewardAmount.Text = text;
				rewardShadow.Text = rewardAmount.Text;
			});
			pieceOne |= (AnimClip)new AnimClipFunction(0f, delegate
			{
			});
			return pieceOne | SHSAnimations.Generic.Wait(1f);
		}
	}

	public class LevelUpWindow : GUIControlWindow
	{
		protected List<LeveledUpMessage> leveledUpMessages;

		protected SHSBrawlerCompleteWindow brawlerCompleteWindow;

		private bool ShowingLevelUp;

		public LevelUpWindow(SHSBrawlerCompleteWindow brawlerCompleteWindow)
		{
			this.brawlerCompleteWindow = brawlerCompleteWindow;
		}

		public override bool InitializeResources(bool reload)
		{
			SetPositionAndSize(QuickSizingHint.ParentSize);
			Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			Traits.HitTestType = HitTestTypeEnum.Transparent;
			IsEnabled = true;
			IsVisible = false;
			return base.InitializeResources(reload);
		}

		public override void OnActive()
		{
			base.OnActive();
			leveledUpMessages = new List<LeveledUpMessage>();
			if (AppShell.Instance != null && AppShell.Instance.EventMgr != null)
			{
				AppShell.Instance.EventMgr.AddListener<LeveledUpMessage>(OnLeveledUp);
				AppShell.Instance.EventMgr.AddListener<LeveledUpAwardHiddenMessage>(OnLeveledUpAwardHidden);
			}
		}

		public override void OnInactive()
		{
			base.OnInactive();
			leveledUpMessages = null;
			if (AppShell.Instance != null && AppShell.Instance.EventMgr != null)
			{
				AppShell.Instance.EventMgr.RemoveListener<LeveledUpMessage>(OnLeveledUp);
				AppShell.Instance.EventMgr.RemoveListener<LeveledUpAwardHiddenMessage>(OnLeveledUpAwardHidden);
			}
		}

		public override void OnShow()
		{
			base.OnShow();
			AnimateLevelUpWindow();
		}

		public bool LeveledUp()
		{
			return leveledUpMessages.Count > 0;
		}

		protected void OnLeveledUp(LeveledUpMessage msg)
		{
			if (msg != null)
			{
				leveledUpMessages.Add(msg);
				leveledUpMessages.Sort(delegate(LeveledUpMessage m1, LeveledUpMessage m2)
				{
					if (m1.NewLevel < m2.NewLevel)
					{
						return -1;
					}
					return (m1.NewLevel > m2.NewLevel) ? 1 : 0;
				});
			}
		}

		protected void OnLeveledUpAwardHidden(LeveledUpAwardHiddenMessage msg)
		{
			if (!ShowingLevelUp)
			{
				return;
			}
			AnimateLevelUpWindow();
			GameObject gameObject = GameObject.Find(msg.message.Hero);
			if (gameObject != null)
			{
				BehaviorManager component = gameObject.GetComponent<BehaviorManager>();
				if (component != null && component.getBehavior() is BehaviorLeveledUp)
				{
					component.endBehavior();
				}
			}
		}

		protected void PlayLevelUpSequence(LeveledUpMessage msg)
		{
			ShowingLevelUp = true;
			GUIManager.Instance.ShowDynamicWindow(new SHSLevelUpWindow(msg), ModalLevelEnum.Full);
			GameObject gameObject = GameObject.Find(msg.Hero);
			if (gameObject != null)
			{
				BehaviorManager component = gameObject.GetComponent<BehaviorManager>();
				if (component != null && !(component.getBehavior() is BehaviorLeveledUp))
				{
					component.requestChangeBehavior<BehaviorLeveledUp>(true);
				}
			}
		}

		protected void AnimateLevelUpWindow()
		{
			AnimClip toAdd = SHSAnimations.Generic.Wait(0.5f);
			if (leveledUpMessages.Count > 0)
			{
				toAdd |= AnimateLevelUp();
			}
			else
			{
				toAdd |= brawlerCompleteWindow.AnimateNextStep();
			}
			ShowingLevelUp = false;
			base.AnimationPieceManager.Add(toAdd);
		}

		protected AnimClip AnimateLevelUp()
		{
			return new AnimClipFunction(0f, delegate
			{
				if (leveledUpMessages.Count > 0)
				{
					PlayLevelUpSequence(leveledUpMessages[0]);
					leveledUpMessages.RemoveAt(0);
				}
			});
		}
	}

	private const int MAX_REWARD_PLATES = 3;

	private const int MAX_OWNABLE_PLATES = 3;

	public const float COUNT_UP_TIME = 4f;

	public const float MIN_COUNTUP_RATE = 800f;

	public const float WIN_SCALE_TIME = 0.5f;

	public const float HOLD_TIME = 0.75f;

	public const float FADE_DURATION = 1f;

	public const float REWARD_TIME = 1f;

	private GUIImage background;

	private SHSGlowOutlineWindow backgroundAnts;

	private GUIImage scoreBarBg;

	private GUIAlphaCutoffDrawTexture scoreBar;

	private GUIImage earnedMessage;

	private GUIImage medalGoal;

	private string[] medalNames = new string[4]
	{
		"bronze",
		"silver",
		"gold",
		"adamantium"
	};

	private GUIImage medal;

	private GUIStrokeTextLabel totalScore;

	private string[] criteriaNames = new string[4]
	{
		"special",
		"knockouts",
		"combos",
		"survival"
	};

	private GUIImage criteria;

	private GUILabel mainScore;

	private GUILabel mainScoreShadow;

	private string[] rewardNames = new string[3]
	{
		"tickets",
		"coins",
		"xp"
	};

	private string[] rewardLabels = new string[3]
	{
		"#missionrewards_ticket_label",
		"#missionrewards_silver_label",
		"#missionrewards_experience_label"
	};

	private string[] rewardTooltips = new string[3]
	{
		"#missionrewards_tt_1",
		"#missionrewards_tt_2",
		"#missionrewards_tt_3"
	};

	private RewardWindow[] rewardPlates;

	private OwnableWindow[] ownablePlates;

	private playerNameScoreWindow[] playerNamePlates;

	private GUIHotSpotButton clicker;

	private GUITBCloseButton closeButton;

	private bool SkipAll;

	private bool upsellClicked;

	private GUIButton buyMissionButton;

	private GUIDrawTexture buyMissionButtonBackground;

	private GUIDrawTexture buyMissionButtonTexture;

	private GUIDropShadowTextLabel buyMissionButtonShadow;

	private GUIButton survivalLeaderboardButton;

	private GUIStrokeTextLabel survivalLeaderboardLabel;

	private MissionResultStep completeStep;

	private ShsAudioSourceList sounds = ShsAudioSourceList.GetList("MissionResultsScreen");

	public LevelUpWindow levelUpPlate;

	protected int[] scoreTargets;

	protected ActiveMission currentMissionReference;

	protected EventResultMissionEvent currentMissionResults;

	protected int coinReward;

	protected int ticketReward;

	protected int xpReward;

	protected int bonusXPReward;

	protected int rewardTier = 1;

	protected string ownableRewards = string.Empty;

	protected int specialBonus;

	protected int visiblePlayers;

	protected string playerHeroName = string.Empty;

	protected int talliedScore;

	protected int currentCategoryScore;

	private List<BrawlerStatManager.CharacterScoreData> dispStatBlocks;

	private int currentMedalValue;

	public SHSBrawlerCompleteWindow()
	{
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		background = (ChangeAlignment(GUIControl.CreateControlTopFrame<GUIImage>(new Vector2(946f, 392f), new Vector2(0f, 16f)), AnchorAlignmentEnum.Middle) as GUIImage);
		background.TextureSource = "brawler_bundle|mc_frame";
		Add(background);
		backgroundAnts = new SHSGlowOutlineWindow(GetAntPath(), 4);
		backgroundAnts.SetPositionAndSize(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, new Vector2(688f, 392f));
		backgroundAnts.Offset = new Vector2(0f, 16f);
		Add(backgroundAnts);
		Vector2 vector = new Vector2(-280f, -18f);
		Vector2 offset = vector + new Vector2(0f, 308f);
		Vector2 offset2 = vector + new Vector2(0f, 186f);
		Vector2 offset3 = vector + new Vector2(77f, 315f);
		Vector2 offset4 = vector + new Vector2(260f, 160f);
		Vector2 vector2 = vector + new Vector2(244f, 240f);
		Vector2 offset5 = vector2 - new Vector2(3f, -3f);
		scoreBarBg = (ChangeAlignment(GUIControl.CreateControlTopFrame<GUIImage>(new Vector2(512f, 512f), vector), AnchorAlignmentEnum.Middle) as GUIImage);
		scoreBarBg.TextureSource = "brawler_bundle|mc_bar_bg";
		scoreBarBg.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		Add(scoreBarBg);
		scoreBar = (ChangeAlignment(GUIControl.CreateControlTopFrame<GUIAlphaCutoffDrawTexture>(new Vector2(512f, 512f), vector), AnchorAlignmentEnum.Middle) as GUIAlphaCutoffDrawTexture);
		scoreBar.TextureSource = "brawler_bundle|mc_bar_" + medalNames[0];
		scoreBar.AlphaCutoffTextureSource = "brawler_bundle|mc_bar_mask";
		scoreBar.AlphaCutoff = 1f;
		scoreBar.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		scoreBar.Color = Color.white;
		Add(scoreBar);
		earnedMessage = (ChangeAlignment(GUIControl.CreateControlTopFrame<GUIImage>(new Vector2(140f, 80f), offset), AnchorAlignmentEnum.Middle) as GUIImage);
		earnedMessage.TextureSource = "brawler_bundle|mc_medal_earned";
		earnedMessage.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		Add(earnedMessage);
		medalGoal = GUIControl.CreateControl<GUIImage>(new Vector2(64f, 64f), offset3, DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.Middle);
		medalGoal.TextureSource = "brawler_bundle|mc_dot_" + medalNames[0];
		medalGoal.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		Add(medalGoal);
		criteria = GUIControl.CreateControlTopFrame<GUIImage>(new Vector2(436f, 92f), offset4);
		criteria.TextureSource = "brawler_bundle|mc_criteria_" + criteriaNames[0];
		criteria.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		Add(criteria);
		mainScoreShadow = GUIControl.CreateControlTopFrame<GUILabel>(new Vector2(296f, 92f), offset5);
		mainScoreShadow.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 67, Utils.ColorFromBytes(0, 21, 105, byte.MaxValue), TextAnchor.MiddleRight);
		mainScoreShadow.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		mainScoreShadow.Text = "987,654";
		Add(mainScoreShadow);
		mainScore = GUIControl.CreateControlTopFrame<GUILabel>(new Vector2(296f, 92f), vector2);
		mainScore.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 67, Utils.ColorFromBytes(byte.MaxValue, 249, 157, byte.MaxValue), TextAnchor.MiddleRight);
		mainScore.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		mainScore.Text = "987,654";
		Add(mainScore);
		playerNamePlates = new playerNameScoreWindow[4];
		for (int i = 0; i < playerNamePlates.Length; i++)
		{
			playerNameScoreWindow playerNameScoreWindow = GUIControl.CreateControlAbsolute<playerNameScoreWindow>(new Vector2(192f, 192f), new Vector2(i * 100, 0f));
			playerNameScoreWindow.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			playerNamePlates[i] = playerNameScoreWindow;
			Add(playerNameScoreWindow);
		}
		Vector2 a = new Vector2(-48f, 108f);
		clicker = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(Vector2.zero, Vector2.zero);
		clicker.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		clicker.SetPositionAndSize(QuickSizingHint.ParentSize);
		Add(clicker);
		medal = (ChangeAlignment(GUIControl.CreateControlTopFrame<GUIImage>(new Vector2(147f, 141f), offset2), AnchorAlignmentEnum.Middle) as GUIImage);
		medal.TextureSource = "brawler_bundle|mc_medal_" + medalNames[0];
		medal.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		Add(medal);
		totalScore = (ChangeAlignment(GUIControl.CreateControlTopFrame<GUIStrokeTextLabel>(new Vector2(147f, 141f), offset2), AnchorAlignmentEnum.Middle) as GUIStrokeTextLabel);
		totalScore.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 28, GUILabel.GenColor(255, 249, 157), GUILabel.GenColor(138, 69, 0), GUILabel.GenColor(138, 69, 0), new Vector2(3f, 5f), TextAnchor.MiddleCenter);
		totalScore.SetPositionAndSize(new Vector2(142f, 10f), new Vector2(175f, 285f));
		totalScore.Text = "0";
		Add(totalScore);
		rewardPlates = new RewardWindow[3];
		for (int j = 0; j < 3; j++)
		{
			RewardWindow rewardWindow = GUIControl.CreateControlTopFrame<RewardWindow>(new Vector2(300f, 100f), a + new Vector2(0f, 86 * j));
			rewardWindow.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			rewardWindow.SetReward(0, rewardLabels[j], rewardNames[j], rewardTooltips[j]);
			rewardWindow.SetAudioElements(sounds, false);
			rewardPlates[j] = rewardWindow;
			Add(rewardWindow);
		}
		Vector2 a2 = a + new Vector2(270f, 0f);
		ownablePlates = new OwnableWindow[3];
		for (int k = 0; k < 3; k++)
		{
			OwnableWindow ownableWindow = GUIControl.CreateControlTopFrame<OwnableWindow>(new Vector2(400f, 100f), a2 + new Vector2(0f, 86 * k));
			ownableWindow.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			ownableWindow.SetReward(0, string.Empty, string.Empty, string.Empty);
			ownableWindow.SetAudioElements(sounds, false);
			ownableWindow.IsVisible = false;
			ownablePlates[k] = ownableWindow;
			Add(ownableWindow);
		}
		closeButton = GUIControl.CreateControlTopFrame<GUITBCloseButton>(new Vector2(44f, 44f), new Vector2(430f, 65f));
		Add(closeButton);
		buyMissionButton = new GUIButton();
		buyMissionButtonBackground = new GUIDrawTexture();
		buyMissionButtonShadow = new GUIDropShadowTextLabel();
		buyMissionButtonTexture = new GUIDrawTexture();
		survivalLeaderboardButton = new GUIButton();
		survivalLeaderboardLabel = new GUIStrokeTextLabel();
		ActiveMission activeMission = AppShell.Instance.SharedHashTable["ActiveMission"] as ActiveMission;
		if (activeMission != null)
		{
			MissionDefinition missionDefinition = activeMission.MissionDefinition;
			string text = missionDefinition.Id;
			AppShell.callAnalytics("player", "mission", "complete", string.Empty + text);
			if (missionDefinition.MissionPurchaseOverride != null)
			{
				text = missionDefinition.MissionPurchaseOverride.ToLowerInvariant();
			}
			string text2 = "#you_dont_own_this_mission";
			string missionIdFromKey = AppShell.Instance.MissionManifest.GetMissionIdFromKey(activeMission.Id);
			int similarMissionID = -1;
			if (AppShell.Instance.Profile.AvailableMissions.ContainsKey(missionIdFromKey) || OwnableDefinition.getDef(int.Parse(missionIdFromKey)).released == 0)
			{
				string itemName = new string(Enumerable.ToArray(Enumerable.Where((IEnumerable<char>)Enumerable.Last(text.ToLowerInvariant().Split('_')).TrimEnd('a'), (Func<char, bool>)delegate(char c)
				{
					return c < '0' || c > '9';
				})));
				similarMissionID = NewShoppingManager.findSimilarUnownedItem(itemName, NewShoppingManager.ShoppingCategory.Mission);
				text2 = "#upsell_try_this_mission";
			}
			if (similarMissionID != -1)
			{
				text = OwnableDefinition.getDef(similarMissionID).name;
				Vector2 b = new Vector2(-400f, 180f);
				float num = 0.75f;
				buyMissionButtonBackground = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(new Vector2(297f, 385.2f) * num, Vector2.zero + b);
				buyMissionButtonBackground.TextureSource = "brawlergadget_bundle|brawler_gadget_buymission_panel";
				Add(buyMissionButtonBackground);
				buyMissionButtonTexture = GUIControl.CreateControlFrameCentered<GUIDrawTexture>(new Vector2(133.65f, 173.34f) * num, new Vector2(77f, -74f) * num + b);
				buyMissionButtonTexture.Rotation = 6f;
				buyMissionButtonTexture.TextureSource = "missions_bundle|L_mshs_gameworld_" + text;
				Add(buyMissionButtonTexture);
				buyMissionButtonShadow = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(142f, 185f) * num, new Vector2(-50f, -80f) * num + b);
				buyMissionButtonShadow.SetupText(GUIFontManager.SupportedFontEnum.Zooom, (int)(28f * num), Color.black, TextAnchor.MiddleCenter);
				buyMissionButtonShadow.TextOffset = new Vector2(-2f, 2f);
				buyMissionButtonShadow.FrontColor = Color.white;
				buyMissionButtonShadow.BackColor = GUILabel.GenColor(0, 21, 105);
				buyMissionButtonShadow.Text = text2;
				Add(buyMissionButtonShadow);
				buyMissionButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(210f, 145f) * num, new Vector2(6f, 87f) * num + b);
				buyMissionButton.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|L_buyit_button");
				buyMissionButton.HitTestType = HitTestTypeEnum.Alpha;
				buyMissionButton.HitTestSize = new Vector2(0.87f, 0.79f) * num;
				buyMissionButton.ToolTip = new NamedToolTipInfo("#buy_this_mission");
				buyMissionButton.Click += delegate
				{
					ShoppingWindow shoppingWindow = new ShoppingWindow(similarMissionID);
					shoppingWindow.launch();
					upsellClicked = true;
					buyMissionButtonBackground.SetVisible(false);
					buyMissionButtonTexture.SetVisible(false);
					buyMissionButtonShadow.SetVisible(false);
					buyMissionButton.SetVisible(false);
				};
				Add(buyMissionButton);
			}
			if (activeMission.IsSurvivalMode)
			{
				float d = 0.35f;
				survivalLeaderboardButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(512f, 175f) * d, new Vector2(6f, 87f) * d + new Vector2(-20f, -256f));
				survivalLeaderboardButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|stdbutton");
				survivalLeaderboardButton.HitTestType = HitTestTypeEnum.Alpha;
				survivalLeaderboardButton.ToolTip = new NamedToolTipInfo("#tt_survival_leaderboard");
				survivalLeaderboardButton.Click += delegate
				{
					SHSSocialSurvivalModeLeaderboard dialogWindow = new SHSSocialSurvivalModeLeaderboard(AppShell.Instance.MissionManifest.GetMissionIdFromKey(currentMissionReference.Id));
					GUIManager.Instance.ShowDynamicWindow(dialogWindow, ModalLevelEnum.Full);
					upsellClicked = true;
				};
				Add(survivalLeaderboardButton);
				survivalLeaderboardLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 20, GUILabel.GenColor(212, 124, 9), GUILabel.GenColor(138, 69, 0), GUILabel.GenColor(138, 69, 0), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
				survivalLeaderboardLabel.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(6f, 87f) * d + new Vector2(-20f, -256f), new Vector2(512f, 175f) * d, AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				survivalLeaderboardLabel.Text = "#survival_highscores";
				Add(survivalLeaderboardLabel);
			}
		}
		ForceVisibility(false);
		clicker.Click += delegate
		{
			base.AnimationPieceManager.SkipAnimations = true;
		};
		closeButton.Click += delegate
		{
			SkipAll = true;
			base.AnimationPieceManager.SkipAnimations = true;
			if (upsellClicked)
			{
				AppShell.Instance.EventMgr.Fire(this, new BrawlerSummaryCompleteMessage());
				Hide();
			}
		};
		SocialSpaceControllerImpl.bumpIdleTimer();
		AppShell.Instance.EventMgr.AddListener<OpenAchievementsWindowMessage>(OnOpenAchievementsWindowMessage);
		levelUpPlate = new LevelUpWindow(this);
	}

	public void OnOpenAchievementsWindowMessage(OpenAchievementsWindowMessage msg)
	{
		CspUtils.DebugLog("OnOpenAchievementsWindowMessage ");
		upsellClicked = true;
	}

	private static List<Vector2> GetAntPath()
	{
		List<Vector2> glowPath = SHSGlowOutlineWindow.GetGlowPath(new Vector2(15f, 52f), new Vector2(25f, 47f), new Vector2(67f, 40f), new Vector2(67f, 34f), new Vector2(78f, 24f), new Vector2(200f, 7f), new Vector2(299f, 1f), new Vector2(344f, 1f), new Vector2(389f, 1f), new Vector2(488f, 7f), new Vector2(610f, 24f), new Vector2(618f, 31f), new Vector2(618f, 36f), new Vector2(668f, 45f), new Vector2(677f, 52f), new Vector2(682f, 65f), new Vector2(681f, 82f), new Vector2(672f, 194f), new Vector2(657f, 279f), new Vector2(638f, 356f), new Vector2(633f, 366f), new Vector2(619f, 371f), new Vector2(440f, 383f), new Vector2(318f, 386f), new Vector2(221f, 383f), new Vector2(61f, 372f), new Vector2(43f, 367f), new Vector2(37f, 357f), new Vector2(21f, 273f), new Vector2(11f, 201f), new Vector2(7f, 127f), new Vector2(7f, 67f));
		for (int i = 0; i < glowPath.Count; i++)
		{
			List<Vector2> list;
			List<Vector2> list2 = list = glowPath;
			int index;
			int index2 = index = i;
			Vector2 a = list[index];
			list2[index2] = a - new Vector2(344f, 196f);
		}
		return glowPath;
	}

	private void NextStep()
	{
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
		int num = 1;
		if (!SkipAll)
		{
			base.AnimationPieceManager.SkipAnimations = false;
		}
		switch (completeStep)
		{
		case MissionResultStep.ShowWindow:
		{
			background.IsVisible = true;
			List<BrawlerStatManager.CharacterScoreData> allStatBlocks = BrawlerStatManager.instance.GetAllStatBlocks();
			allStatBlocks.Sort(BrawlerController.Instance.SmallerPlayer);  // CSP added
			while (allStatBlocks.Count > 4)
			{
				allStatBlocks.RemoveAt(4);
			}
			dispStatBlocks = allStatBlocks;
			visiblePlayers = allStatBlocks.Count;
			int num4 = 0;
			
			// CSP added block
			for (int i=0; i<allStatBlocks.Count; i++) 
			{
				playerNameScoreWindow playerNameScoreWindow3 = playerNamePlates[num4];
				playerNameScoreWindow3.IsVisible = true;
				playerNameScoreWindow3.netOwnerID = allStatBlocks[i].netOwnerID;
				playerNameScoreWindow3.SetPlayerName(allStatBlocks[i].squadName);
				playerNameScoreWindow3.Docking = DockingAlignmentEnum.BottomMiddle;
				playerNameScoreWindow3.Anchor = AnchorAlignmentEnum.Middle;
				num4++;
			}

			// CSP commented out block 
			// foreach (BrawlerStatManager.CharacterScoreData item in allStatBlocks)
			// {
			// 	playerNameScoreWindow playerNameScoreWindow3 = playerNamePlates[num4];
			// 	playerNameScoreWindow3.IsVisible = true;
			// 	playerNameScoreWindow3.netOwnerID = item.netOwnerID;
			// 	playerNameScoreWindow3.SetPlayerName(item.squadName);
			// 	playerNameScoreWindow3.Docking = DockingAlignmentEnum.BottomMiddle;
			// 	playerNameScoreWindow3.Anchor = AnchorAlignmentEnum.Middle;
			// 	num4++;
			// }

			SetupPlayerNameplates();
			AnimClip animClip = SHSAnimations.Generic.Wait(1f);
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				SetupPlayerNameplates();
			};
			base.AnimationPieceManager.Add(animClip);
			AnimateInWindow();

			Hashtable hashtable = fakeMissionResults(allStatBlocks); 	// CSP added for testing
			BrawlerController.Instance.OnNotificationResult(hashtable);  // CSP added for testing

			break;
		}
		case MissionResultStep.ReadData:
		{
			if (currentMissionReference == null)
			{
				return;
			}
			scoreTargets = new int[4];
			scoreTargets[0] = 0;
			for (int num12 = 1; num12 <= 3; num12++)
			{
				scoreTargets[num12] = currentMissionReference.MissionDefinition.ScoreNeededForRating((MissionDefinition.Ratings)num12, visiblePlayers);
			}
			long key = AppShell.Instance.PlayerDictionary.GetPlayerId(AppShell.Instance.ServerConnection.GetGameUserId());
			if (currentMissionResults.PlayerResults.ContainsKey(key))
			{
				MissionResults localResult = currentMissionResults.PlayerResults[key];
				playerHeroName = localResult.heroName;
				ticketReward = localResult.tickets;
				coinReward = localResult.coins;
				xpReward = localResult.earnedXp;
				bonusXPReward = localResult.bonusXp;
				//int xpTotal = xpReward + bonusXPReward; // CSP added
				//Debug.Log("Earned XP=" + xpTotal);
				ownableRewards = localResult.ownable;
				rewardTier = localResult.rewardTier;
				ActiveMission activeMission = AppShell.Instance.SharedHashTable["ActiveMission"] as ActiveMission;

				// CSP - temp comment out block for testing.
				// if (activeMission != null && activeMission.IsSurvivalMode)
				// {
				// 	WWWForm wWWForm = new WWWForm();
				// 	wWWForm.AddField("mission_id", AppShell.Instance.MissionManifest.GetMissionIdFromKey(activeMission.Id));
				// 	wWWForm.AddField("hero_id", OwnableDefinition.HeroNameToHeroID[localResult.heroName]);
				// 	wWWForm.AddField("is_multiplayer_score", Convert.ToInt32(PlayerCombatController.GetPlayerCount() > 1 || currentMissionResults.PlayerResults.Count > 1));
				// 	wWWForm.AddField("score", localResult.comboScore + localResult.enemyKoScore + localResult.survivalScore);
				// 	AppShell.Instance.WebService.StartRequest("resources$users/" + AppShell.Instance.Profile.UserId + "/turn_in_score/", delegate(ShsWebResponse response)
				// 	{
				// 		if (response.Status != 200)
				// 		{
				// 			CspUtils.DebugLogError("Error recording score");
				// 		}
				// 		CspUtils.DebugLogWarning("Recorded score " + (localResult.comboScore + localResult.enemyKoScore + localResult.survivalScore));
				// 	}, wWWForm.data);
				// }
			}
			else
			{
				CspUtils.DebugLog("Did not receive results for the local player!");
			}
			foreach (long key2 in currentMissionResults.PlayerResults.Keys)
			{
				playerNameScoreWindow[] array9 = playerNamePlates;
				foreach (playerNameScoreWindow playerNameScoreWindow5 in array9)
				{
					if (playerNameScoreWindow5.IsVisible && playerNameScoreWindow5.netOwnerID == key2)
					{
						playerNameScoreWindow5.AcquireResults(currentMissionResults.PlayerResults[key2]);
						break;
					}
				}
			}
			playerNameScoreWindow[] array10 = playerNamePlates;
			foreach (playerNameScoreWindow playerNameScoreWindow6 in array10)
			{
				if (playerNameScoreWindow6.IsVisible && playerNameScoreWindow6.playerScore == null)
				{
					playerNameScoreWindow6.IsVisible = false;
				}
			}
			//ShsAudioSource.PlayAutoSound(sounds.GetSource("window_open"));   // CSP temporary commented out
			medal.IsVisible = true;
			totalScore.IsVisible = true;
			scoreBarBg.IsVisible = true;
			scoreBar.IsVisible = true;
			totalScore.IsVisible = true;
			AnimateInMedal();
			clicker.IsVisible = true;
			closeButton.IsVisible = true;
			buyMissionButton.IsVisible = true;
			buyMissionButtonBackground.IsVisible = true;
			buyMissionButtonTexture.IsVisible = true;
			buyMissionButtonShadow.IsVisible = true;
			survivalLeaderboardButton.IsVisible = true;
			survivalLeaderboardLabel.IsVisible = true;
			AnimateInButtons();
			//talliedScore = 0;   // commented out by CSP until scoring is completely fixed.
			currentMedalValue = -1;
			UpdateScoreBar();
			num = GetStepIncrement(MissionResultStep.SpecialBonus);
			break;
		}
		case MissionResultStep.SpecialBonus:
		{
			currentCategoryScore = 0;
			playerNameScoreWindow[] array11 = playerNamePlates;
			foreach (playerNameScoreWindow playerNameScoreWindow7 in array11)
			{
				if (playerNameScoreWindow7.IsVisible)
				{
					currentCategoryScore += playerNameScoreWindow7.playerScore.gimmickScore;
				}
			}
			AnimateCriteriaInitial(0, currentCategoryScore);
			num = GetStepIncrement(MissionResultStep.KoBonus);
			break;
		}
		case MissionResultStep.KoBonus:
		{
			AnimClip toAdd3 = OpenNewCriteria(1);
			currentCategoryScore = 0;
			playerNameScoreWindow topScorer2 = null;
			int num3 = 0;
			playerNameScoreWindow[] array2 = playerNamePlates;
			foreach (playerNameScoreWindow playerNameScoreWindow2 in array2)
			{
				if (playerNameScoreWindow2.IsVisible)
				{
					int startScore2 = currentCategoryScore;
					currentCategoryScore += playerNameScoreWindow2.playerScore.enemyKoScore;
					toAdd3 |= (CountScore(startScore2, currentCategoryScore, 4f) ^ playerNameScoreWindow2.CountUp(0, playerNameScoreWindow2.playerScore.enemyKoScore, 4f));
					if (playerNameScoreWindow2.playerScore.enemyKoScore == num3)
					{
						topScorer2 = null;
					}
					else if (playerNameScoreWindow2.playerScore.enemyKoScore > num3)
					{
						topScorer2 = playerNameScoreWindow2;
						num3 = playerNameScoreWindow2.playerScore.enemyKoScore;
					}
				}
			}
			toAdd3 |= CloseWithCurrentWinner(topScorer2);
			base.AnimationPieceManager.Add(toAdd3);
			num = GetStepIncrement(MissionResultStep.ComboBonus);
			break;
		}
		case MissionResultStep.ComboBonus:
		{
			AnimClip toAdd5 = OpenNewCriteria(2);
			currentCategoryScore = 0;
			playerNameScoreWindow topScorer3 = null;
			int num10 = 0;
			playerNameScoreWindow[] array8 = playerNamePlates;
			foreach (playerNameScoreWindow playerNameScoreWindow4 in array8)
			{
				if (playerNameScoreWindow4.IsVisible)
				{
					int startScore3 = currentCategoryScore;
					currentCategoryScore += playerNameScoreWindow4.playerScore.comboScore;
					toAdd5 |= (CountScore(startScore3, currentCategoryScore, 4f) ^ playerNameScoreWindow4.CountUp(0, playerNameScoreWindow4.playerScore.comboScore, 4f));
					if (playerNameScoreWindow4.playerScore.comboScore == num10)
					{
						topScorer3 = null;
					}
					else if (playerNameScoreWindow4.playerScore.comboScore > num10)
					{
						topScorer3 = playerNameScoreWindow4;
						num10 = playerNameScoreWindow4.playerScore.comboScore;
					}
				}
			}
			toAdd5 |= CloseWithCurrentWinner(topScorer3);
			base.AnimationPieceManager.Add(toAdd5);
			num = GetStepIncrement(MissionResultStep.SurvivalBonus);
			break;
		}
		case MissionResultStep.SurvivalBonus:
		{
			AnimClip toAdd2 = OpenNewCriteria(3);
			currentCategoryScore = 0;
			playerNameScoreWindow topScorer = null;
			int num2 = 0;
			playerNameScoreWindow[] array = playerNamePlates;
			foreach (playerNameScoreWindow playerNameScoreWindow in array)
			{
				if (playerNameScoreWindow.IsVisible)
				{
					int startScore = currentCategoryScore;
					currentCategoryScore += playerNameScoreWindow.playerScore.survivalScore;
					toAdd2 |= (CountScore(startScore, currentCategoryScore, 4f) ^ playerNameScoreWindow.CountUp(0, playerNameScoreWindow.playerScore.survivalScore, 4f));
					if (playerNameScoreWindow.playerScore.survivalScore == num2)
					{
						topScorer = null;
					}
					else if (playerNameScoreWindow.playerScore.survivalScore > num2)
					{
						topScorer = playerNameScoreWindow;
						num2 = playerNameScoreWindow.playerScore.survivalScore;
					}
				}
			}
			toAdd2 |= CloseWithCurrentWinner(topScorer);
			base.AnimationPieceManager.Add(toAdd2);
			break;
		}
		case MissionResultStep.Rewards:
		{
			AnimClip toAdd4 = AnimateOutCriteriaSection(1f);
			//medal.TextureSource = "brawler_bundle|mc_medal_" + medalNames[rewardTier - 1];   // CSP temp comment out
			rewardPlates[0].SetReward(ticketReward, rewardLabels[0], rewardNames[0], rewardTooltips[0]);
			rewardPlates[1].SetReward(coinReward, rewardLabels[1], rewardNames[1], rewardTooltips[1]);
			rewardPlates[2].SetReward(xpReward, rewardLabels[2], rewardNames[2], rewardTooltips[2]);
			if (bonusXPReward > 0)
			{
				rewardPlates[2].SetBonusString("+" + bonusXPReward + " bonus XP!");
			}
			int num5 = 0;
			for (int k = 0; k < 3; k++)
			{
				ownablePlates[k].IsVisible = false;
			}
			if (ownableRewards == string.Empty || ownableRewards == null)
			{
				CspUtils.DebugLog("skipping ownable rewards");
			}
			else
			{
				string[] array3 = ownableRewards.Split(',');
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				string[] array4 = array3;
				foreach (string text in array4)
				{
					if (dictionary.ContainsKey(text))
					{
						dictionary[text]++;
					}
					else
					{
						dictionary.Add(text, 1);
					}
					if (OwnableDefinition.isUniqueItem(int.Parse(text)))
					{
						dictionary[text] = 1;
					}
				}
				int num6 = 0;
				foreach (string key3 in dictionary.Keys)
				{
					OwnableDefinition def = OwnableDefinition.getDef(int.Parse(key3));
					if (def != null && num6 < ownablePlates.Length)
					{
						num5++;
						ownablePlates[num6].IsVisible = true;
						string text2 = def.iconBase;
						if (def.category != OwnableDefinition.Category.Craft && !text2.Contains("_shopping"))
						{
							text2 += "_normal";
						}
						ownablePlates[num6].SetReward(dictionary[key3], def.name, text2, def.name);
						num6++;
						AppShell.Instance.Profile.FetchDataBasedOnCategory(def.category);
					}
				}
			}
			rewardPlates[0].SetAudioElements(sounds, true);
			Vector2[] array5 = new Vector2[6]
			{
				new Vector2(270f, 108f),
				new Vector2(270f, 194f),
				new Vector2(270f, 280f),
				new Vector2(540f, 108f),
				new Vector2(540f, 194f),
				new Vector2(540f, 280f)
			};
			int num7 = 0;
			RewardWindow[] array6 = rewardPlates;
			foreach (RewardWindow rewardWindow in array6)
			{
				if (rewardWindow.rewardTotal != 0)
				{
					rewardWindow.SetPosition(array5[num7] + new Vector2(60f, 0f));
					num7++;
				}
			}
			OwnableWindow[] array7 = ownablePlates;
			foreach (OwnableWindow ownableWindow in array7)
			{
				ownableWindow.SetPosition(array5[num7]);
				num7++;
			}
			for (int num8 = 0; num8 < 3; num8++)
			{
				if (rewardPlates[num8].rewardTotal != 0)
				{
					toAdd4 |= rewardPlates[num8].AnimateIn(1f);
				}
			}
			for (int num9 = 0; num9 < num5; num9++)
			{
				toAdd4 |= ownablePlates[num9].AnimateIn(1f);
			}
			BrawlerController.Instance.ShowRewardEffect();
			toAdd4 |= AnimateNextStep();
			base.AnimationPieceManager.Add(toAdd4);

			//////////// block added by CSP for testing ////////////////
			CspUtils.DebugLog("isvisble kludge!");
			List<BrawlerStatManager.CharacterScoreData> allStatBlocks = BrawlerStatManager.instance.GetAllStatBlocks();
			allStatBlocks.Sort(BrawlerController.Instance.SmallerPlayer); 
			playerNameScoreWindow[] array = playerNamePlates;
			foreach (playerNameScoreWindow playerNameScoreWindow in array)
			{		
				if (playerNameScoreWindow.nameText.Text.Length > 0) {
				  playerNameScoreWindow.IsVisible = true;	
				  for (int i=0; i<allStatBlocks.Count; i++) {	
					if (allStatBlocks[i].squadName == playerNameScoreWindow.nameText.Text) {
						playerNameScoreWindow.CountUp(allStatBlocks[i].individualScoreContribution, allStatBlocks[i].individualScoreContribution, 4f);
					} 	
				  }
				}	
			}
			///////////////////////////////////////////////

			break;
		}
		case MissionResultStep.RewardsEffect:
		{
			backgroundAnts.Highlight(true);
			float waitTime = 12f;
			if (levelUpPlate.LeveledUp())
			{
				waitTime = 4f;
			}
			else
			{
				num++;
			}
			AnimClip toAdd = SHSAnimations.Generic.Wait(waitTime);
			toAdd |= AnimateNextStep();
			base.AnimationPieceManager.Add(toAdd);
			break;
		}
		case MissionResultStep.LevelUp:
			Hide();
			levelUpPlate.Show();
			break;
		case MissionResultStep.Finish:
			DoSummaryComplete();
			break;
		}
		completeStep += num;
	}

	private Vector2[] GetPlateLocations(int length)
	{
		Vector2[] array = new Vector2[length];
		float num = -100 * (length - 1);
		for (int i = 0; i < length; i++)
		{
			array[i] = new Vector2(num + (float)(200 * i), -100f);
		}
		return array;
	}

	private void SetupPlayerNameplates()
	{
		int num = 0;
		List<playerNameScoreWindow> list = new List<playerNameScoreWindow>(4);
	
		// CSP comment out block for testing
		// foreach (BrawlerStatManager.CharacterScoreData dispStatBlock in dispStatBlocks)
		// {
		// 	playerNameScoreWindow playerNameScoreWindow = playerNamePlates[num];
		// 	Vector2 vector2 = playerNameScoreWindow.Offset = Camera.main.WorldToScreenPoint(dispStatBlock.dioramaPosition);
		// 	list.Add(playerNameScoreWindow);
		// 	num++;
		// }

		//////////////// CSP add block for testing  //////////////////////////////
		List<BrawlerStatManager.CharacterScoreData> allStatBlocks = BrawlerStatManager.instance.GetAllStatBlocks();
		allStatBlocks.Sort(BrawlerController.Instance.SmallerPlayer); 
		Vector2[] a2 = GetPlateLocations(allStatBlocks.Count);
		for (int i=0; i<allStatBlocks.Count; i++) {
			playerNameScoreWindow playerNameScoreWindow = playerNamePlates[num];
			Vector2 vector2 = playerNameScoreWindow.Offset = Camera.main.WorldToScreenPoint(allStatBlocks[i].dioramaPosition);
					
			playerNameScoreWindow.Offset = a2[i];

			list.Add(playerNameScoreWindow);
			num++;
		}
		////////////////////////////////////////////////////


		// commented out by CSP for testing
		// Vector2[] plateLocations = GetPlateLocations(list.Count);
		// Vector2[] array = plateLocations;
		// foreach (Vector2 offset in array)
		// {
		// 	playerNameScoreWindow playerNameScoreWindow2 = list[0];
		// 	foreach (playerNameScoreWindow item in list)
		// 	{
		// 		Vector2 offset2 = item.Offset;
		// 		float x = offset2.x;
		// 		Vector2 offset3 = playerNameScoreWindow2.Offset;
		// 		if (x < offset3.x)
		// 		{
		// 			playerNameScoreWindow2 = item;
		// 		}
		// 	}
		// 	playerNameScoreWindow2.Offset = offset;
		// 	list.Remove(playerNameScoreWindow2);
		// }

	}

	private int GetStepIncrement(MissionResultStep nextExpectedStep)
	{
		int num = 1;
		bool flag = false;
		if (nextExpectedStep == MissionResultStep.SpecialBonus && !flag)
		{
			playerNameScoreWindow[] array = playerNamePlates;
			foreach (playerNameScoreWindow playerNameScoreWindow in array)
			{
			
			 	if (playerNameScoreWindow.IsVisible && playerNameScoreWindow.playerScore.gimmickScore > 0)
			 	{
			 		flag = true;
			 	}
			}
			if (!flag)
			{
				num++;
				nextExpectedStep++;
			}
		}
		if (nextExpectedStep == MissionResultStep.KoBonus && !flag)
		{
			playerNameScoreWindow[] array2 = playerNamePlates;
			foreach (playerNameScoreWindow playerNameScoreWindow2 in array2)
			{
				
				 if (playerNameScoreWindow2.IsVisible && playerNameScoreWindow2.playerScore.enemyKoScore > 0)
				 {
				 	flag = true;
				 }
			}
			if (!flag)
			{
				num++;
				nextExpectedStep++;
			}
		}
		if (nextExpectedStep == MissionResultStep.ComboBonus && !flag)
		{
			playerNameScoreWindow[] array3 = playerNamePlates;
			foreach (playerNameScoreWindow playerNameScoreWindow3 in array3)
			{
				
				 if (playerNameScoreWindow3.IsVisible && playerNameScoreWindow3.playerScore.comboScore > 0)
				 {
				 	flag = true;
				 }
			}
			if (!flag)
			{
				num++;
				nextExpectedStep++;
			}
		}
		if (nextExpectedStep == MissionResultStep.SurvivalBonus && !flag)
		{
			playerNameScoreWindow[] array4 = playerNamePlates;
			foreach (playerNameScoreWindow playerNameScoreWindow4 in array4)
			{
	
			 	if (playerNameScoreWindow4.IsVisible && playerNameScoreWindow4.playerScore.survivalScore > 0)
			 	{
			 		flag = true;
			 	}
			}
			if (!flag)
			{
				num++;
				nextExpectedStep++;
			}
		}
		return num;
	}

	private void DoSummaryComplete()
	{
		if (!upsellClicked)
		{
			AppShell.Instance.EventMgr.Fire(this, new BrawlerSummaryCompleteMessage());
			Hide();
		}
	}

	protected void UpdateScoreBar()
	{
		int medalValue = GetMedalForScore(talliedScore);
		if (currentMedalValue != medalValue)
		{
			base.AnimationPieceManager.Add(SHSAnimations.Generic.AnimationBounceTransitionOut(new Vector2(147f, 141f), 1f, medal) | new AnimClipFunction(0f, delegate
			{
				//ShsAudioSource.PlayAutoSound(sounds.GetSource("medal_meter"));  // CSP comment out temporarily
				medal.TextureSource = "brawler_bundle|mc_medal_" + medalNames[medalValue];
				StringTable stringTable = AppShell.Instance.stringTable;
				medal.ToolTip = new NamedToolTipInfo(stringTable["#missionrewards_tt_medal1"] + " " + stringTable["#missionrewards_tt_" + medalNames[medalValue] + "medal"] + " " + stringTable["#missionrewards_tt_medal2"]);
				medal.ToolTip.Offset = new Vector2(-100f, 0f);
			}) | SHSAnimations.Generic.AnimationBounceTransitionIn(new Vector2(147f, 141f), 1f, medal));
			currentMedalValue = medalValue;
			if (medalValue + 1 < medalNames.Length)
			{
				scoreBar.TextureSource = "brawler_bundle|mc_bar_" + medalNames[medalValue];
				medalGoal.TextureSource = "brawler_bundle|mc_dot_" + medalNames[medalValue + 1];
			}
			else
			{
				scoreBar.TextureSource = "brawler_bundle|mc_bar_" + medalNames[medalValue];
				medalGoal.TextureSource = "brawler_bundle|mc_dot_" + medalNames[medalValue];
			}
		}
		float num = scoreTargets[medalValue];
		float num2 = talliedScore;
		float num3 = 1f;
		num3 = ((medalValue + 1 >= medalNames.Length) ? ((float)(scoreTargets[medalValue] * 2) - num) : ((float)scoreTargets[medalValue + 1] - num));
		float num4 = (num2 - num) / num3;
		scoreBar.AlphaCutoff = 1f - num4;
	}

	protected int GetMedalForScore(int scoreCheck)
	{
		int i;
		for (i = 0; i < 3; i++)
		{
			if (scoreTargets == null)
			{
				break;
			}
			if (scoreCheck < scoreTargets[i + 1])
			{
				break;
			}
		}
		return i;
	}

	protected void UpdateMissionCounters()
	{
		int medalForScore = GetMedalForScore(currentMissionResults.TotalScore);
		AppShell.Instance.CounterManager.AddCounter("MissionsComplete", currentMissionReference.Id);
		ISHSCounterType counter = AppShell.Instance.CounterManager.GetCounter("MissionsHighestMedal");
		if (counter.GetCurrentValue(currentMissionReference.Id) < medalForScore)
		{
			counter.SetCounter(currentMissionReference.Id, medalForScore);
		}
		if (playerHeroName != null && playerHeroName != string.Empty)
		{
			AppShell.Instance.CounterManager.AddCounter("TirelessHeroCounter", playerHeroName);
			if (medalForScore == 0)
			{
				AppShell.Instance.CounterManager.AddCounter("BronzeMedals", playerHeroName);
			}
			if (medalForScore == 1)
			{
				AppShell.Instance.CounterManager.AddCounter("BronzeMedals", playerHeroName);
				AppShell.Instance.CounterManager.AddCounter("SilverMedals", playerHeroName);
			}
			if (medalForScore == 2)
			{
				AppShell.Instance.CounterManager.AddCounter("BronzeMedals", playerHeroName);
				AppShell.Instance.CounterManager.AddCounter("SilverMedals", playerHeroName);
				AppShell.Instance.CounterManager.AddCounter("HighMarksCounter", playerHeroName);
			}
			if (medalForScore == 3)
			{
				AppShell.Instance.CounterManager.AddCounter("BronzeMedals", playerHeroName);
				AppShell.Instance.CounterManager.AddCounter("SilverMedals", playerHeroName);
				AppShell.Instance.CounterManager.AddCounter("HighMarksCounter", playerHeroName);
				AppShell.Instance.CounterManager.AddCounter("FlawlessHeroCounter", playerHeroName);
			}
			if (visiblePlayers == 4)
			{
				AppShell.Instance.CounterManager.AddCounter("SuperSquadFourCounter", playerHeroName);
			}
		}
	}

	public void ProcessMissionResultList(EventResultMissionEvent results)
	{
		currentMissionReference = (ActiveMission)AppShell.Instance.SharedHashTable["ActiveMission"];
		currentMissionResults = results;
		NextStep();
		UpdateMissionCounters();
	}

	protected void ForceVisibility(bool newState)
	{
		background.IsVisible = newState;
		scoreBarBg.IsVisible = newState;
		scoreBar.IsVisible = newState;
		earnedMessage.IsVisible = newState;
		medal.IsVisible = newState;
		totalScore.IsVisible = newState;
		medalGoal.IsVisible = newState;
		criteria.IsVisible = newState;
		mainScore.IsVisible = newState;
		mainScoreShadow.IsVisible = newState;
		RewardWindow[] array = rewardPlates;
		foreach (RewardWindow rewardWindow in array)
		{
			rewardWindow.IsVisible = newState;
		}
		playerNameScoreWindow[] array2 = playerNamePlates;
		foreach (playerNameScoreWindow playerNameScoreWindow in array2)
		{
			playerNameScoreWindow.IsVisible = newState;
		}
		clicker.IsVisible = newState;
		closeButton.IsVisible = newState;
		buyMissionButton.IsVisible = newState;
		buyMissionButtonBackground.IsVisible = newState;
		buyMissionButtonTexture.IsVisible = newState;
		buyMissionButtonShadow.IsVisible = newState;
		survivalLeaderboardButton.IsVisible = newState;
		survivalLeaderboardLabel.IsVisible = newState;
	}

	public override void OnHide()
	{
		base.OnHide();
		SkipAll = false;
		upsellClicked = false;
		ForceVisibility(false);
		AppShell.Instance.EventMgr.RemoveListener<OpenAchievementsWindowMessage>(OnOpenAchievementsWindowMessage);
		base.AnimationPieceManager.ForceCompleteAllAndClear();
	}

	// CSP added  this method for for testing
	Hashtable fakeMissionResults(List<BrawlerStatManager.CharacterScoreData> allStatBlocks) {
		Hashtable hash = new Hashtable();

		hash.Add("message_type", "brawler_scoring");

		hash.Add("total_score", allStatBlocks[0].individualScoreContribution.ToString());
		hash.Add("kos", allStatBlocks[0].enemiesKOd.ToString());
		hash.Add("pickups", "4");

		hash.Add("leveled_up", "0");
		hash.Add("hero_name", allStatBlocks[0].squadName);

		PlayerDictionary.Player value;
		AppShell.Instance.PlayerDictionary.TryGetValue(AppShell.Instance.ServerConnection.GetGameUserId(), out value);
		if (value != null)
		{
			hash.Add("player_ids", value.PlayerId.ToString());
		}
		else 
		{
			hash.Add("player_ids", "-1");  //3870526
		}

		ActiveMission activeMission = AppShell.Instance.SharedHashTable["ActiveMission"] as ActiveMission;
		String xp = "350";  // default to regular mission XP reward
		String fractals = "50";  // default to regular mission fractal reward
		// !!! TBD !!! for now, survivial missions should get less to keep people from just dying to collect rewards fast
		if (activeMission.IsMayhem) {
			xp = "35";
			fractals = "5";
		}
		if (activeMission.IsCrisis) {
			xp = "500";
			fractals = "150";
		}
		if (activeMission.IsSurvivalMode) {
			xp = "70";
			fractals = "10";
		}
			
		hash.Add("hero_current_xp", "0");
		hash.Add("bonus_xp", "0");
		hash.Add("silver", fractals);
		hash.Add("tickets", "75");
		hash.Add("xp", xp);
		hash.Add("ownable_type_id", null);
		hash.Add("reward_tier", "1");
		hash.Add("display_data", null);

		string myUserName = AppShell.Instance.ServerConnection.GetGameServer().GetUserName(AppShell.Instance.ServerConnection.GetGameUserId());

		for (int i=0; i<allStatBlocks.Count; i++) {
			CspUtils.DebugLog(i + ": individualScoreContribution=" + allStatBlocks[i].individualScoreContribution);
			CspUtils.DebugLog("enemiesKOd=" + allStatBlocks[i].enemiesKOd);
			CspUtils.DebugLog("squadName=" + allStatBlocks[i].squadName);

			if (myUserName == allStatBlocks[i].squadName) {
				talliedScore = allStatBlocks[i].individualScoreContribution;   //SHSBrawlerScoreWindow.globalScore;     
				totalScore.Text = string.Format("{0:n0}", talliedScore);
			}
			
		}

		
		return hash;
	}
	public override void OnShow()
	{
		base.OnShow();
		SkipAll = false;
		upsellClicked = false;
		NextStep();
		//NextStep();  // CSP added for testing
		
	}

	protected void AnimateInWindow()
	{
		AnimClip toAdd = SHSAnimations.Generic.AnimationBounceTransitionIn(new Vector2(946f, 392f), 1f, background);
		playerNameScoreWindow[] array = playerNamePlates;
		foreach (playerNameScoreWindow playerNameScoreWindow in array)
		{
			if (playerNameScoreWindow.IsVisible)
			{
				toAdd |= playerNameScoreWindow.AnimateIn();
			}
		}
		base.AnimationPieceManager.Add(toAdd);
	}

	protected void AnimateInMedal()
	{
		AnimClip toAdd = SHSAnimations.Generic.AnimationBounceTransitionIn(new Vector2(147f, 141f), 1f, medal) ^ SHSAnimations.Generic.AnimationBounceTransitionIn(new Vector2(512f, 512f), 1f, scoreBarBg, scoreBar);
		AnimClip animClip = SHSAnimations.Generic.AnimationBounceTransitionIn(new Vector2(140f, 80f), 1f, earnedMessage) ^ SHSAnimations.Generic.ChangeVisibility(true, earnedMessage);
		animClip ^= (AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, 0.5f), medalGoal) ^ SHSAnimations.Generic.ChangeVisibility(true, medalGoal));
		toAdd |= animClip;
		toAdd |= AnimateNextStep();
		base.AnimationPieceManager.Add(toAdd);
	}

	protected void AnimateInButtons()
	{
		closeButton.Alpha = 0f;
		AnimClip toAdd = SHSAnimations.Generic.Wait(1f);
		toAdd |= SHSAnimations.Generic.FadeIn(closeButton, 1f);
		base.AnimationPieceManager.Add(toAdd);
		buyMissionButton.Alpha = 0f;
		AnimClip toAdd2 = SHSAnimations.Generic.Wait(1f);
		toAdd2 |= SHSAnimations.Generic.FadeIn(buyMissionButton, 1f);
		base.AnimationPieceManager.Add(toAdd2);
		buyMissionButtonBackground.Alpha = 0f;
		AnimClip toAdd3 = SHSAnimations.Generic.Wait(1f);
		toAdd3 |= SHSAnimations.Generic.FadeIn(buyMissionButtonBackground, 1f);
		base.AnimationPieceManager.Add(toAdd3);
		buyMissionButtonTexture.Alpha = 0f;
		AnimClip toAdd4 = SHSAnimations.Generic.Wait(1f);
		toAdd4 |= SHSAnimations.Generic.FadeIn(buyMissionButtonTexture, 1f);
		base.AnimationPieceManager.Add(toAdd4);
		buyMissionButtonShadow.Alpha = 0f;
		AnimClip toAdd5 = SHSAnimations.Generic.Wait(1f);
		toAdd5 |= SHSAnimations.Generic.FadeIn(buyMissionButtonShadow, 1f);
		base.AnimationPieceManager.Add(toAdd5);
		survivalLeaderboardButton.Alpha = 0f;
		AnimClip toAdd6 = SHSAnimations.Generic.Wait(1f);
		toAdd6 |= SHSAnimations.Generic.FadeIn(survivalLeaderboardButton, 1f);
		base.AnimationPieceManager.Add(toAdd6);
		survivalLeaderboardLabel.Alpha = 0f;
		AnimClip toAdd7 = SHSAnimations.Generic.Wait(1f);
		toAdd7 |= SHSAnimations.Generic.FadeIn(survivalLeaderboardLabel, 1f);
		base.AnimationPieceManager.Add(toAdd7);
	}

	protected void AnimateCriteriaInitial(int category, int categoryScore)
	{
		AnimClip toAdd = OpenNewCriteria(category);
		toAdd |= SHSAnimations.Generic.Wait(0.75f);
		toAdd |= CountScore(0, categoryScore, 4f);
		toAdd |= AnimateHoldScore(false);
		base.AnimationPieceManager.Add(toAdd);
	}

	protected AnimClip OpenNewCriteria(int category)
	{
		return new AnimClipFunction(0f, delegate
		{
			mainScore.Text = string.Empty;
			mainScoreShadow.Text = string.Empty;
			criteria.IsVisible = true;
			criteria.SetSize(436f, 92f);
			criteria.TextureSource = "brawler_bundle|mc_criteria_" + criteriaNames[category];
			mainScore.Alpha = 1f;
			mainScoreShadow.Alpha = 1f;
			mainScore.IsVisible = true;
			mainScoreShadow.IsVisible = true;
			ShsAudioSource.PlayAutoSound(sounds.GetSource("pop_up"));
			ChangeAlignment(criteria, AnchorAlignmentEnum.MiddleLeft);
		}) ^ SHSAnimations.Generic.AnimationBounceTransitionIn(new Vector2(436f, 92f), 1f, criteria);
	}

	protected AnimClip CloseCriteria()
	{
		return new AnimClipFunction(0f, delegate
		{
			ShsAudioSource.PlayAutoSound(sounds.GetSource("pop_out"));
			ChangeAlignment(criteria, AnchorAlignmentEnum.MiddleRight);
		}) ^ SHSAnimations.Generic.AnimationBounceTransitionOut(new Vector2(436f, 92f), 1f, criteria) ^ AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(1f, 0f, 1f), mainScore) ^ AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(1f, 0f, 1f), mainScoreShadow);
	}

	protected AnimClip ClosePlayerPlates()
	{
		AnimClip result = SHSAnimations.Generic.Blank();
		playerNameScoreWindow[] array = playerNamePlates;

		// CSP comment block out for testing 
		// foreach (playerNameScoreWindow playerNameScoreWindow in array)
		// {
		// 	if (playerNameScoreWindow.IsVisible)
		// 	{
		// 		result ^= playerNameScoreWindow.FadeScore(1f);
		// 	}
		// }
		return result;
	}

	protected AnimClip CountScore(int startScore, int finalScore, float duration)
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		int scoreToAdd = finalScore - startScore;
		int lastAddedScore = 0;
		if ((float)scoreToAdd / 800f < duration)
		{
			duration = (float)scoreToAdd / 800f;
		}
		if (duration <= 0f)
		{
			duration = 0.01f;
		}
		AnimClip animClip = new AnimClipFunction(0f, delegate
		{
		});
		animClip |= (AnimClip)new AnimClipFunction(duration, delegate(float totalTime)
		{
			int num = Mathf.CeilToInt((float)scoreToAdd * (totalTime / duration));
			if (num > scoreToAdd)
			{
				num = scoreToAdd;
			}
			//talliedScore += num - lastAddedScore;   // CSP temp comment out
			lastAddedScore = num;
			mainScore.Text = (lastAddedScore + startScore).ToString();
			mainScoreShadow.Text = mainScore.Text;
			totalScore.Text = string.Format("{0:n0}", talliedScore);
			//UpdateScoreBar();    // CSP temp comment out
		});
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
		};
		return animClip;
	}

	protected AnimClip CloseWithCurrentWinner(playerNameScoreWindow topScorer)
	{
		AnimClip pieceOne = SHSAnimations.Generic.Blank();
		if (topScorer != null)
		{
			pieceOne |= SHSAnimations.Generic.Wait(0.75f);
			pieceOne |= topScorer.ShowOff(0.5f);
		}
		return pieceOne | AnimateHoldScore(true);
	}

	protected AnimClip AnimateOutCriteriaSection(float duration)
	{
		AnimClip result = SHSAnimations.Generic.AnimationBounceTransitionOut(new Vector2(512f, 512f), 1f, scoreBar) ^ SHSAnimations.Generic.AnimationBounceTransitionOut(new Vector2(512f, 512f), 1f, scoreBarBg);
		playerNameScoreWindow[] array = playerNamePlates;
		// CSP - comment out block for testing
		// foreach (playerNameScoreWindow playerNameScoreWindow in array)
		// {
		// 	result ^= playerNameScoreWindow.FadeEverything(duration);
		// }
		result ^= SHSAnimations.Generic.FadeOutVis(medalGoal, duration);
		earnedMessage.IsVisible = false;
		mainScore.IsVisible = false;
		mainScoreShadow.IsVisible = false;
		criteria.IsVisible = false;
		medal.SetSize(247f, 232f);
		//ShsAudioSource.PlayAutoSound(sounds.GetSource("medal_awarded"));  // CSP comment out temporary
		return result;
	}

	protected AnimClip AnimateNextStep()
	{
		return new AnimClipFunction(0f, delegate
		{
			NextStep();
		});
	}

	protected AnimClip AnimateHoldScore(bool closePlates)
	{
		return new AnimClipFunction(0f, delegate
		{
			if (!SkipAll)
			{
				base.AnimationPieceManager.SkipAnimations = false;
			}
			AnimClip toAdd = SHSAnimations.Generic.Wait(0.75f);
			if (closePlates)
			{
				toAdd |= (CloseCriteria() ^ ClosePlayerPlates());
			}
			else
			{
				toAdd |= CloseCriteria();
			}
			toAdd |= AnimateNextStep();
			base.AnimationPieceManager.Add(toAdd);
		});
	}

	public static GUIControl ChangeAlignment(GUIControl adjustMe, AnchorAlignmentEnum newAlignment)
	{
		Vector2 vector = new Vector2(0f, 0f);
		switch (adjustMe.Anchor)
		{
		case AnchorAlignmentEnum.TopLeft:
		case AnchorAlignmentEnum.MiddleLeft:
		case AnchorAlignmentEnum.BottomLeft:
			switch (newAlignment)
			{
			case AnchorAlignmentEnum.TopMiddle:
			case AnchorAlignmentEnum.Middle:
			case AnchorAlignmentEnum.BottomMiddle:
			{
				Vector2 size6 = adjustMe.Size;
				vector.x = size6.x * 0.5f;
				break;
			}
			case AnchorAlignmentEnum.TopRight:
			case AnchorAlignmentEnum.MiddleRight:
			case AnchorAlignmentEnum.BottomRight:
			{
				Vector2 size5 = adjustMe.Size;
				vector.x = size5.x;
				break;
			}
			}
			break;
		case AnchorAlignmentEnum.TopMiddle:
		case AnchorAlignmentEnum.Middle:
		case AnchorAlignmentEnum.BottomMiddle:
			switch (newAlignment)
			{
			case AnchorAlignmentEnum.TopLeft:
			case AnchorAlignmentEnum.MiddleLeft:
			case AnchorAlignmentEnum.BottomLeft:
			{
				Vector2 size4 = adjustMe.Size;
				vector.x = (0f - size4.x) * 0.5f;
				break;
			}
			case AnchorAlignmentEnum.TopRight:
			case AnchorAlignmentEnum.MiddleRight:
			case AnchorAlignmentEnum.BottomRight:
			{
				Vector2 size3 = adjustMe.Size;
				vector.x = size3.x * 0.5f;
				break;
			}
			}
			break;
		case AnchorAlignmentEnum.TopRight:
		case AnchorAlignmentEnum.MiddleRight:
		case AnchorAlignmentEnum.BottomRight:
			switch (newAlignment)
			{
			case AnchorAlignmentEnum.TopLeft:
			case AnchorAlignmentEnum.MiddleLeft:
			case AnchorAlignmentEnum.BottomLeft:
			{
				Vector2 size2 = adjustMe.Size;
				vector.x = 0f - size2.x;
				break;
			}
			case AnchorAlignmentEnum.TopRight:
			case AnchorAlignmentEnum.MiddleRight:
			case AnchorAlignmentEnum.BottomRight:
			{
				Vector2 size = adjustMe.Size;
				vector.x = (0f - size.x) * 0.5f;
				break;
			}
			}
			break;
		}
		switch (adjustMe.Anchor)
		{
		case AnchorAlignmentEnum.TopLeft:
		case AnchorAlignmentEnum.TopMiddle:
		case AnchorAlignmentEnum.TopRight:
			switch (newAlignment)
			{
			case AnchorAlignmentEnum.MiddleLeft:
			case AnchorAlignmentEnum.Middle:
			case AnchorAlignmentEnum.MiddleRight:
			{
				Vector2 size12 = adjustMe.Size;
				vector.y = size12.y * 0.5f;
				break;
			}
			case AnchorAlignmentEnum.BottomLeft:
			case AnchorAlignmentEnum.BottomMiddle:
			case AnchorAlignmentEnum.BottomRight:
			{
				Vector2 size11 = adjustMe.Size;
				vector.y = size11.y;
				break;
			}
			}
			break;
		case AnchorAlignmentEnum.MiddleLeft:
		case AnchorAlignmentEnum.Middle:
		case AnchorAlignmentEnum.MiddleRight:
			switch (newAlignment)
			{
			case AnchorAlignmentEnum.TopLeft:
			case AnchorAlignmentEnum.TopMiddle:
			case AnchorAlignmentEnum.TopRight:
			{
				Vector2 size10 = adjustMe.Size;
				vector.y = (0f - size10.y) * 0.5f;
				break;
			}
			case AnchorAlignmentEnum.BottomLeft:
			case AnchorAlignmentEnum.BottomMiddle:
			case AnchorAlignmentEnum.BottomRight:
			{
				Vector2 size9 = adjustMe.Size;
				vector.y = size9.y * 0.5f;
				break;
			}
			}
			break;
		case AnchorAlignmentEnum.BottomLeft:
		case AnchorAlignmentEnum.BottomMiddle:
		case AnchorAlignmentEnum.BottomRight:
			switch (newAlignment)
			{
			case AnchorAlignmentEnum.TopLeft:
			case AnchorAlignmentEnum.TopMiddle:
			case AnchorAlignmentEnum.TopRight:
			{
				Vector2 size8 = adjustMe.Size;
				vector.y = 0f - size8.y;
				break;
			}
			case AnchorAlignmentEnum.MiddleLeft:
			case AnchorAlignmentEnum.Middle:
			case AnchorAlignmentEnum.MiddleRight:
			{
				Vector2 size7 = adjustMe.Size;
				vector.y = (0f - size7.y) * 0.5f;
				break;
			}
			}
			break;
		}
		adjustMe.Offset += vector;
		adjustMe.Anchor = newAlignment;
		return adjustMe;
	}
}
