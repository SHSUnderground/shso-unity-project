using CardGame;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SHSCardGameCompleteWindow : GUIWindow
{
	public class SixLabelText : GUISimpleControlWindow
	{
		public GUILabel frontText;

		public GUILabel offset1;

		public GUILabel offset2;

		public GUILabel offset3;

		public GUILabel offset4;

		public GUILabel dropShadow;

		public TextAnchor TextAlignment
		{
			get
			{
				return frontText.TextAlignment;
			}
			set
			{
				frontText.TextAlignment = value;
				offset1.TextAlignment = value;
				offset2.TextAlignment = value;
				offset3.TextAlignment = value;
				offset4.TextAlignment = value;
				dropShadow.TextAlignment = value;
			}
		}

		public string Text
		{
			get
			{
				return frontText.Text;
			}
			set
			{
				frontText.Text = value;
				offset1.Text = value;
				offset2.Text = value;
				offset3.Text = value;
				offset4.Text = value;
				dropShadow.Text = value;
			}
		}

		public int FontSize
		{
			get
			{
				return frontText.FontSize;
			}
			set
			{
				frontText.FontSize = value;
				offset1.FontSize = value;
				offset2.FontSize = value;
				offset3.FontSize = value;
				offset4.FontSize = value;
				dropShadow.FontSize = value;
			}
		}

		public int VerticalKerning
		{
			get
			{
				return frontText.VerticalKerning;
			}
			set
			{
				frontText.VerticalKerning = value;
				offset1.VerticalKerning = value;
				offset2.VerticalKerning = value;
				offset3.VerticalKerning = value;
				offset4.VerticalKerning = value;
				dropShadow.VerticalKerning = value;
			}
		}

		public SixLabelText(Vector2 size, Vector2 offset)
		{
			SetSize(2000f, 2000f);
			SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, offset);
			frontText = GUIControl.CreateControlFrameCentered<GUILabel>(size, new Vector2(0f, 0f));
			offset1 = GUIControl.CreateControlFrameCentered<GUILabel>(size, new Vector2(1f, 1f));
			offset2 = GUIControl.CreateControlFrameCentered<GUILabel>(size, new Vector2(1f, -1f));
			offset3 = GUIControl.CreateControlFrameCentered<GUILabel>(size, new Vector2(-1f, 1f));
			offset4 = GUIControl.CreateControlFrameCentered<GUILabel>(size, new Vector2(-1f, -1f));
			dropShadow = GUIControl.CreateControlFrameCentered<GUILabel>(size, new Vector2(0f, 0f));
			Add(dropShadow);
			Add(offset1);
			Add(offset2);
			Add(offset3);
			Add(offset4);
			Add(frontText);
		}

		public void SetupText(GUIFontManager.SupportedFontEnum fontFace, int fontSize, Color frontColor, Color surroundColor, Color dropShadowColor, Vector2 dropShadowOffset)
		{
			frontText.SetupText(fontFace, fontSize, frontColor, TextAnchor.MiddleCenter);
			offset1.SetupText(fontFace, fontSize, surroundColor, TextAnchor.MiddleCenter);
			offset2.SetupText(fontFace, fontSize, surroundColor, TextAnchor.MiddleCenter);
			offset3.SetupText(fontFace, fontSize, surroundColor, TextAnchor.MiddleCenter);
			offset4.SetupText(fontFace, fontSize, surroundColor, TextAnchor.MiddleCenter);
			dropShadow.SetupText(fontFace, fontSize, dropShadowColor, TextAnchor.MiddleCenter);
			dropShadow.Offset = dropShadowOffset;
		}

		public int CalculateKerningAndGetLines()
		{
			frontText.CalculateTextLayout();
			return frontText.LineCount;
		}
	}

	public class CardGameRewardsWindow : GUIDynamicWindow
	{
		public class CardWindow : GUISimpleControlWindow
		{
			private const float DEFAULT_ROTATION = -8f;

			private const float DEFAULT_CARD_SIZE_PERC = 0.52f;

			public GUIImage card;

			private GUIHotSpotButton hotSpot;

			private GUIImage flash;

			private GUIStrokeTextLabel txt;

			private AnimClip growShrink;

			private float currentGrowShrinkValue;

			public CardWindow(bool recieved, string starburstText, Texture2D cardTexture)
			{
				SetSize(2000f, 2000f);
				SetPosition(QuickSizingHint.Centered);
				hotSpot = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(new Vector2(366f, 512f) * 0.52f, new Vector2(-155f, 3f));
				hotSpot.Rotation = -8f;
				hotSpot.MouseOver += delegate
				{
					MouseOverOut(true);
				};
				hotSpot.MouseOut += delegate
				{
					MouseOverOut(false);
				};
				Add(hotSpot);
				card = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(366f, 512f) * 0.52f, new Vector2(-155f, 3f));
				card.Texture = cardTexture;
				card.Rotation = -8f;
				Add(card);
				flash = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(143f, 143f), new Vector2(-282f, 71f));
				flash.TextureSource = "cardgame_bundle|card_game_rewards_prize_flash";
				Add(flash);
				int fontSize = (!recieved) ? 18 : 30;
				txt = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(100f, 180f), new Vector2(-282f, 71f));
				txt.SetupText(GUIFontManager.SupportedFontEnum.Grobold, fontSize, ColorUtil.FromRGB255(255, 255, 255), ColorUtil.FromRGB255(226, 53, 31), ColorUtil.FromRGB255(230, 53, 33), new Vector2(3f, 3f), TextAnchor.MiddleCenter);
				txt.Text = starburstText;
				txt.Rotation = -15f;
				Add(txt);
				if (cardTexture == null)
				{
					txt.IsVisible = false;
					flash.IsVisible = false;
				}
			}

			public void MouseOverOut(bool highlight)
			{
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Expected O, but got Unknown
				float num = highlight ? 1 : 0;
				float num2 = SHSAnimations.GenericFunctions.FrationalTime((!highlight) ? 1 : 0, num, currentGrowShrinkValue, 0.25f);
				AnimClip animClip = new AnimClipFunction(SHSAnimations.GenericPaths.LinearWithBounce(currentGrowShrinkValue, num, num2, Mathf.Abs(num - currentGrowShrinkValue) * 0.2f, num2), delegate(float x)
				{
					currentGrowShrinkValue = x;
					card.Rotation = UnboundLerp(-8f, 0f, x);
					card.Offset = new Vector2(-155f, UnboundLerp(3f, -50f, x));
					card.Size = new Vector2(366f, 512f) * UnboundLerp(0.52f, 1f, x);
				});
				base.AnimationPieceManager.SwapOut(ref growShrink, animClip);
				if (highlight)
				{
					ControlToFront(card);
				}
				else
				{
					animClip.OnFinished += (Action)(object)(Action)delegate
					{
						ControlToBack(card);
					};
				}
			}

			public float UnboundLerp(float start, float end, float x)
			{
				return start + (end - start) * x;
			}
		}

		public class TxtButton : GUISimpleControlWindow
		{
			public GUIButton bkgButton;

			public GUIStrokeTextLabel txt;

			public TxtButton(string buttonLoc, string text, int textSize, float rotation, Vector2 textOffset, Vector2 buttonSize)
			{
				SetSize(128f, 128f);
				SetPosition(QuickSizingHint.Centered);
				bkgButton = GUIControl.CreateControlFrameCentered<GUIButton>(buttonSize, Vector2.zero);
				bkgButton.StyleInfo = new SHSButtonStyleInfo("cardgame_bundle|cardgame_rewards_button_" + buttonLoc);
				bkgButton.HitTestType = HitTestTypeEnum.Alpha;
				Add(bkgButton);
				txt = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(80f, 60f), textOffset);
				txt.SetupText(GUIFontManager.SupportedFontEnum.Grobold, textSize, ColorUtil.FromRGB255(255, 255, 255), ColorUtil.FromRGB255(102, 129, 21), ColorUtil.FromRGB255(71, 92, 8), new Vector2(2f, 2f), TextAnchor.MiddleCenter);
				txt.Text = text;
				txt.Rotation = rotation;
				Add(txt);
				txt.VerticalKerning = 16;
			}
		}

		public class RewardWindow : GUISimpleControlWindow
		{
			private GUIImage bkg;

			private GUIStrokeTextLabel reward;

			private GUIStrokeTextLabel label;

			private float rewardTotal;

			private string bonusRewardStr = string.Empty;

			private string lastRewardText = string.Empty;

			private ShsAudioSourceList rewardsSoundList;

			[CompilerGenerated]
			private bool _003CUseLargeCounterSFX_003Ek__BackingField;

			public bool UseLargeCounterSFX
			{
				[CompilerGenerated]
				get
				{
					return _003CUseLargeCounterSFX_003Ek__BackingField;
				}
				[CompilerGenerated]
				set
				{
					_003CUseLargeCounterSFX_003Ek__BackingField = value;
				}
			}

			public RewardWindow()
			{
				SetSize(new Vector2(300f, 100f));
				SetPosition(QuickSizingHint.Centered);
				IsVisible = true;
				bkg = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(400f, 100f), Vector2.zero);
				Add(bkg);
				reward = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(300f, 100f), new Vector2(115f, -2f));
				reward.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 38, ColorUtil.FromRGB255(255, 227, 134), ColorUtil.FromRGB255(18, 50, 135), ColorUtil.FromRGB255(22, 60, 154), new Vector2(-3f, 4f), TextAnchor.MiddleCenter);
				reward.Text = "0";
				Add(reward);
				reward.TextAlignment = TextAnchor.MiddleLeft;
				label = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(300f, 100f), new Vector2(195f, 6f));
				label.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 22, ColorUtil.FromRGB255(255, 227, 134), ColorUtil.FromRGB255(18, 50, 135), ColorUtil.FromRGB255(22, 60, 154), new Vector2(-2f, 2f), TextAnchor.MiddleCenter);
				Add(label);
				label.TextAlignment = TextAnchor.MiddleLeft;
				UseLargeCounterSFX = false;
				rewardsSoundList = ShsAudioSourceList.GetList("MissionResultsScreen");
			}

			public void SetReward(float rewardTotal, string labelText, string bkgTexture)
			{
				this.rewardTotal = rewardTotal;
				bkg.TextureSource = bkgTexture;
				label.Text = labelText;
				float textWidth = reward.GetTextWidth();
				GUIStrokeTextLabel gUIStrokeTextLabel = label;
				Vector2 offset = reward.Offset;
				gUIStrokeTextLabel.Offset = new Vector2(offset.x + textWidth + 10f, 6f);
			}

			public void SetBonusString(string str)
			{
				bonusRewardStr = str;
			}

			public AnimClip AnimateIn(float duration)
			{
				AnimClip result = SHSAnimations.Generic.ChangeVisibility(true, this);
				result |= (SHSAnimations.Generic.AnimationBounceTransitionIn(new Vector2(300f, 100f), 1f, bkg) ^ SHSAnimations.Generic.AnimationFadeTransitionIn(reward, label));
				if (!UseLargeCounterSFX)
				{
					result |= (AnimClip)new AnimClipFunction(0f, delegate
					{
					});
				}
				result |= (AnimClip)new AnimClipFunction(duration, delegate(float totalTime)
				{
					reward.Text = Mathf.FloorToInt(totalTime / duration * rewardTotal).ToString();
					PlayRewardSFX(totalTime, duration);
					float textWidth = reward.GetTextWidth();
					GUIStrokeTextLabel gUIStrokeTextLabel = label;
					Vector2 offset2 = reward.Offset;
					gUIStrokeTextLabel.Offset = new Vector2(offset2.x + textWidth + 10f, 6f);
				});
				result |= (AnimClip)new AnimClipFunction(0f, delegate
				{
					Vector2 offset = new Vector2(165f, 36f);
					GUILabel gUILabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(300f, 100f), offset);
					gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 19, Utils.ColorFromBytes(byte.MaxValue, 249, 157, byte.MaxValue), TextAnchor.MiddleLeft);
					gUILabel.Text = bonusRewardStr;
					Add(gUILabel);
				});
				if (!UseLargeCounterSFX)
				{
					result |= (AnimClip)new AnimClipFunction(0f, delegate
					{
					});
				}
				return result;
			}

			protected void PlayRewardSFX(float elapsed, float duration)
			{
				if (UseLargeCounterSFX && reward.Text != lastRewardText)
				{
					ShsAudioSource.PlayAutoSound(rewardsSoundList.GetSource("counter_large"));
					lastRewardText = reward.Text;
				}
			}
		}

		private RewardWindow tickets;

		private RewardWindow silver;

		private RewardWindow xp;

		private CardWindow cardWindow;

		private GUIImage bkgFlash;

		private SHSCardGameCompleteWindow ParentWindow;

		private bool windowCreated;

		private bool buttonClicked;

		public CardGameRewardsWindow(SHSCardGameCompleteWindow ParentWindow)
		{
			this.ParentWindow = ParentWindow;
		}

		protected void CreateWindow()
		{
			if (windowCreated)
			{
				return;
			}
			SocialSpaceControllerImpl.bumpIdleTimer();
			windowCreated = true;
			if (CardGameController.Instance != null && CardGameController.Instance.IsPvPBattle())
			{
				ParentWindow.FirePvPChallengeEvent();
			}
			StartInfo start = AppShell.Instance.SharedHashTable["CardGameLevel"] as StartInfo;
			SetSize(750f, 750f);
			SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft, OffsetType.Absolute, new Vector2(-15f, 0f));
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(689f, 447f), Vector2.zero);
			gUIImage.TextureSource = "cardgame_bundle|cardgame_rewards_container";
			Add(gUIImage);
			GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(476f, 109f), new Vector2(0f, -176f));
			gUIImage2.TextureSource = "cardgame_bundle|L_cardgame_rewards_title_bar";
			Add(gUIImage2);
			bkgFlash = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(418f, 418f), new Vector2(-155f, 3f));
			bkgFlash.TextureSource = "cardgame_bundle|behind_card_flash";
			Add(bkgFlash);
			Vector2[] array = new Vector2[3]
			{
				new Vector2(116f, -85f),
				new Vector2(116f, 1f),
				new Vector2(116f, 84f)
			};
			int num = 0;
			silver = new RewardWindow();
			silver.Offset = array[num];
			silver.SetReward(ParentWindow.SilverReward, "#cardGame_fractals", "cardgame_bundle|cardgame_mc_fractal");
			Add(silver);
			num++;
			xp = new RewardWindow();
			xp.Offset = array[num];
			xp.SetReward(ParentWindow.XpReward, "#cardGame_xp", "cardgame_bundle|L_cardgame_mc_xp");
			CspUtils.DebugLog("SHSCardGameCompleteWindowRewards got xpMult of " + AppShell.Instance.Profile.xpMultiplier);
			if (AppShell.Instance.Profile.xpMultiplier > 1.0)
			{
				xp.SetBonusString("+" + (int)((double)ParentWindow.XpReward * (AppShell.Instance.Profile.xpMultiplier - 1.0) + 0.5) + " bonus XP!");
			}
			Add(xp);
			num++;
			TxtButton txtButton = new TxtButton("left", "#cardGame_Rematch", 16, -5f, new Vector2(0f, 0f), new Vector2(128f, 128f));
			txtButton.Offset = new Vector2(-103f, 173f);
			if (CardGameController.Instance != null && CardGameController.Instance.IsPvPBattle())
			{
				txtButton.bkgButton.IsEnabled = false;
			}
			else
			{
				txtButton.bkgButton.Click += delegate
				{
					if (!buttonClicked)
					{
						buttonClicked = true;
						if (start.Players[1].PlayerID == -1)
						{
							AppShell.Instance.QueueLocationInfo();
							AppShell.Instance.Matchmaker2.SoloCardGame(start.ArenaScenario, start.Players[0].Hero, start.Players[0].DeckRecipe, start.QuestNodeID);
							AppShell.Instance.Transition(GameController.ControllerType.CardGame);
						}
						else
						{
							SHSCardGameGadgetWindow sHSCardGameGadgetWindow3 = new SHSCardGameGadgetWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.InviteFriends);
							sHSCardGameGadgetWindow3.SetupForCardGameInviter((int)start.Players[1].PlayerID, start.Players[1].Name);
							sHSCardGameGadgetWindow3.CloseButton.Click += delegate
							{
								buttonClicked = false;
							};
							GUIManager.Instance.ShowDynamicWindow(sHSCardGameGadgetWindow3, ModalLevelEnum.Default);
						}
					}
				};
			}
			Add(txtButton);
			TxtButton txtButton2 = new TxtButton("middle", "#cardGame_NextBattle", 19, 0f, new Vector2(0f, 0f), new Vector2(256f, 256f));
			txtButton2.Offset = new Vector2(0f, 173f);
			Add(txtButton2);
			TxtButton txtButton3 = new TxtButton("right", "#cardGame_NewQuest", 16, 5f, new Vector2(-4f, 0f), new Vector2(128f, 128f));
			txtButton3.Offset = new Vector2(103f, 173f);
			Add(txtButton3);
			if (!ParentWindow.HaveNextQuestEnabled)
			{
				txtButton2.bkgButton.IsEnabled = false;
			}
			txtButton2.bkgButton.IsVisible = true;
			txtButton2.txt.IsVisible = true;
			txtButton2.bkgButton.Click += delegate
			{
				if (!buttonClicked)
				{
					buttonClicked = true;
					Hide();
					SHSCardGameGadgetWindow sHSCardGameGadgetWindow2 = new SHSCardGameGadgetWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.ChooseAQuest);
					CardQuestPart questPart = AppShell.Instance.CardQuestManager.GetQuestPart(start.QuestID);
					if (questPart != null)
					{
						foreach (CardQuestPart.QuestBattle node in questPart.Nodes)
						{
							if (node.Id == start.QuestNodeID)
							{
								CardQuestPart.QuestBattle questBattle = node.NextQuest;
								if (questBattle == null && questPart.Nodes.IndexOf(node) == questPart.Nodes.Count - 1)
								{
									if (questPart.ParentQuest != null)
									{
										int num2 = questPart.ParentQuest.Parts.IndexOf(questPart);
										if (num2 < questPart.ParentQuest.Parts.Count - 1)
										{
											questPart = questPart.ParentQuest.Parts[num2 + 1];
											questBattle = questPart.Nodes[0];
										}
									}
									else
									{
										CardQuest questByPartId = AppShell.Instance.CardQuestManager.GetQuestByPartId(questPart.Id);
										if (questByPartId != null)
										{
											for (int i = 0; i < questByPartId.Parts.Count; i++)
											{
												if (questByPartId.Parts[i].Id == questPart.Id)
												{
													if (i < questByPartId.Parts.Count - 1)
													{
														questPart = questByPartId.Parts[i + 1];
														questBattle = questPart.Nodes[0];
													}
													break;
												}
											}
										}
									}
								}
								if (questBattle != null)
								{
									sHSCardGameGadgetWindow2.LaunchManager.SelectedBattle = questBattle;
									sHSCardGameGadgetWindow2.LaunchManager.SelectedQuest = questBattle.ParentCardQuestPart.ParentQuest;
									sHSCardGameGadgetWindow2.LaunchManager.ShowOnlyDailyQuest = false;
									sHSCardGameGadgetWindow2.LaunchManager.SelectDailyQuest = false;
								}
								break;
							}
						}
					}
					GUIManager.Instance.ShowDynamicWindow(sHSCardGameGadgetWindow2, ModalLevelEnum.Default);
				}
			};
			txtButton3.bkgButton.Click += delegate
			{
				if (!buttonClicked)
				{
					buttonClicked = true;
					Hide();
					SHSCardGameGadgetWindow sHSCardGameGadgetWindow = new SHSCardGameGadgetWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.Main);
					sHSCardGameGadgetWindow.CloseButton.Click += delegate
					{
						AppShell.Instance.Transition(GameController.ControllerType.SocialSpace);
					};
					GUIManager.Instance.ShowDynamicWindow(sHSCardGameGadgetWindow, ModalLevelEnum.Default);
				}
			};
			bool flag = start.Players[1].Type == PlayerType.AI;
			bool playerWon = ParentWindow.PlayerWon;
			if (flag)
			{
				if (playerWon)
				{
					if (!AppShell.Instance.Profile.AvailableQuests.ContainsKey(string.Format("{0}", start.QuestID)))
					{
						cardWindow = new CardWindow(false, "#cardGame_WhatYouCouldGet", ParentWindow.GetRewardCardTexture());
						Add(cardWindow);
					}
					else
					{
						cardWindow = new CardWindow(true, "#cardGame_YouGot", ParentWindow.GetRewardCardTexture());
						Add(cardWindow);
					}
				}
				else
				{
					cardWindow = new CardWindow(false, "#cardgame_WinToGet", ParentWindow.GetRewardCardTexture());
					Add(cardWindow);
				}
			}
			else if (playerWon)
			{
				cardWindow = new CardWindow(true, "#cardGame_YouGot", ParentWindow.GetRewardCardTexture());
				Add(cardWindow);
			}
			else
			{
				GUIImage gUIImage3 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(217f, 238f), new Vector2(-172f, -6f));
				gUIImage3.TextureSource = "cardgame_bundle|cardgame_game_summary_winning_character";
				Add(gUIImage3);
			}
			GUITBCloseButton gUITBCloseButton = GUIControl.CreateControlFrameCentered<GUITBCloseButton>(new Vector2(44f, 44f), new Vector2(303f, -151f));
			gUITBCloseButton.Click += delegate
			{
				if (!buttonClicked)
				{
					buttonClicked = true;
					Hide();
					AppShell.Instance.Transition(GameController.ControllerType.SocialSpace);
				}
			};
			Add(gUITBCloseButton);
			base.AnimationOnOpen = SHSAnimations.WindowOpenCloseDelegates.GetOpenAnimation(this, new Vector2(689f, 447f), gUIImage);
			base.AnimationOnClose = SHSAnimations.WindowOpenCloseDelegates.GetCloseAnimation(this, new Vector2(689f, 447f), gUIImage);
			base.AnimationOpenFinished += delegate
			{
			};
			AnimClip toAdd = silver.AnimateIn(0.5f);
			toAdd ^= (SHSAnimations.Generic.Wait(0.5f) | xp.AnimateIn(0.5f));
			base.AnimationPieceManager.Add(toAdd);
			BeginBackgroundFlashSpin(0f);
		}

		public override void Update()
		{
			base.Update();
			if (ParentWindow.RewardsReceived)
			{
				CreateWindow();
			}
		}

		public void BeginBackgroundFlashSpin(float timeOffset)
		{
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Expected O, but got Unknown
			AnimPath path = AnimPath.Sin(0f, 5f, 8f) * 0.2f + 0.8f;
			AnimPath path2 = (AnimPath.Sin(0f, 5f, 8f) * 0.1f + 1f) * 418f;
			AnimClip ta = AnimClipBuilder.Absolute.Rotation(AnimPath.Linear(0f, 360f, 8f), bkgFlash) ^ AnimClipBuilder.Absolute.Alpha(path, bkgFlash) ^ AnimClipBuilder.Absolute.SizeXY(path2, bkgFlash);
			ta.ElapsedTime = timeOffset;
			ta.OnFinished += (Action)(object)(Action)delegate
			{
				BeginBackgroundFlashSpin(ta.TimeOver);
			};
			base.AnimationPieceManager.Add(ta);
		}
	}

	public class CardGameSummaryWindow : GUIDynamicWindow
	{
		private GUIImage bkg;

		private GUIImage heroDudeImage;

		private GUIDropShadowTextLabel EnemyName;

		private GUIDropShadowTextLabel EnemyResult;

		private GUILabel HintOrSummary;

		private GUILabel HintText;

		private GUITBCloseButton close;

		private SHSCardGameCompleteWindow ParentWindow;

		private GUIButton okButton;

		private SHSHintLabel HintOrSummaryWindow;

		public CardGameSummaryWindow(SHSCardGameCompleteWindow ParentWindow)
		{
			this.ParentWindow = ParentWindow;
			SetSize(2000f, 2000f);
			SetPosition(QuickSizingHint.Centered);
			Offset = new Vector2(0f, -46f);
			bkg = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(594f, 312f), Vector2.zero);
			bkg.TextureSource = "cardgame_bundle|cardgame_post_game_summary_bg";
			Add(bkg);
			HintOrSummaryWindow = GUIControl.CreateControlFrameCentered<SHSHintLabel>(new Vector2(354f, 500f), new Vector2(56f, 212f));
			HintOrSummaryWindow.Id = "HintOrSummaryWindow";
			HintOrSummaryWindow.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, ColorUtil.FromRGB255(78, 96, 109), TextAnchor.UpperLeft);
			Add(HintOrSummaryWindow);
			EnemyResult = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(354f, 70f), new Vector2(20f, -55f));
			EnemyResult.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 30, ColorUtil.FromRGB255(21, 153, 226), ColorUtil.FromRGB255(0, 28, 70), new Vector2(2f, 2f), TextAnchor.MiddleLeft);
			EnemyResult.BackColorAlpha = 0.1f;
			Add(EnemyResult);
			EnemyName = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(386f, 70f), new Vector2(35f, -100f));
			EnemyName.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 38, ColorUtil.FromRGB255(53, 168, 30), ColorUtil.FromRGB255(0, 28, 70), new Vector2(2f, 2f), TextAnchor.LowerLeft);
			EnemyName.BackColorAlpha = 0.1f;
			EnemyName.Overflow = false;
			Add(EnemyName);
			heroDudeImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(0f, 0f), new Vector2(-262f, -84f));
			Add(heroDudeImage);
			close = GUIControl.CreateControlFrameCentered<GUITBCloseButton>(new Vector2(44f, 44f), new Vector2(256f, -117f));
			close.Click += delegate
			{
				onClick();
			};
			Add(close);
			okButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(0f, 125f));
			okButton.Click += delegate
			{
				onClick();
			};
			okButton.HitTestSize = new Vector2(0.5f, 0.5f);
			okButton.HitTestType = HitTestTypeEnum.Circular;
			okButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|L_mshs_button_ok");
			Add(okButton);
			SetupData();
			base.AnimationOnOpen = SHSAnimations.WindowOpenCloseDelegates.GetOpenAnimation(this, new Vector2(594f, 312f), bkg);
			base.AnimationOnClose = SHSAnimations.WindowOpenCloseDelegates.GetCloseAnimation(this, new Vector2(594f, 312f), bkg);
			base.AnimationOpenFinished += delegate
			{
				AnimClip toAdd = AnimClipBuilder.Absolute.SizeX(SHSAnimations.GenericPaths.BounceTransitionInX(217f, 0f), heroDudeImage) ^ AnimClipBuilder.Absolute.SizeY(SHSAnimations.GenericPaths.BounceTransitionInY(238f, 0f), heroDudeImage);
				base.AnimationPieceManager.Add(toAdd);
			};
		}

		protected void onClick()
		{
			Hide();
			CardGameController.Instance.DisableHud();
			SquadBattleCharacterController.Instance.RewardsPresentation();
		}

		public void SetupData()
		{
			EnemyName.Text = AppShell.Instance.CharacterDescriptionManager[ParentWindow.CurrentCardQuestBattle.Enemy].CharacterName;
			if (ParentWindow.PlayerWon)
			{
				if (ParentWindow.PlayerForfeited)
				{
					EnemyResult.Text = "#cardGame_GivesUp";
				}
				else
				{
					EnemyName.Text = "#cardGame_YouWin";
					EnemyName.FontSize = 60;
					EnemyName.Offset = new Vector2(20f, -85f);
					EnemyResult.Text = "#cardGame_BattleComplete";
				}
				HintOrSummaryWindow.Text = ParentWindow.CurrentCardQuestBattle.Epilogue;
				HintOrSummaryWindow.IsVisible = true;
				heroDudeImage.TextureSource = "cardgame_bundle|cardgame_game_summary_winning_character";
			}
			else
			{
				HintText = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(354f, 30f), new Vector2(20f, -28f));
				HintText.SetupText(GUIFontManager.SupportedFontEnum.Komica, 16, ColorUtil.FromRGB255(21, 153, 226), TextAnchor.MiddleLeft);
				HintText.Text = "#cardGame_Hint";
				Add(HintText);
				EnemyResult.Text = "#cardGame_Wins";
				heroDudeImage.TextureSource = "cardgame_bundle|cardgame_game_summary_losing_character";
				ParentWindow.CurrentCardQuestBattle.TimesLost++;
				HintOrSummaryWindow.IsVisible = true;
				HintOrSummaryWindow.Text = ParentWindow.CurrentCardQuestBattle.CurrentHint;
			}
		}
	}

	public bool PlayerWon;

	public bool PlayerForfeited;

	public bool RewardsReceived;

	public int SilverReward;

	public int XpReward;

	public int TicketReward;

	public string RewardCard = string.Empty;

	public string OwnableRewards = string.Empty;

	public bool IsCardQuest;

	public CardQuestPart.QuestBattle CurrentCardQuestBattle;

	public bool HaveNextQuestEnabled;

	public SHSCardGameCompleteWindow()
	{
		RewardsReceived = false;
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.CardGameResults>(OnCardGameResults);
	}

	public void SetPlayerWon(bool playerWon, bool playerForfeited)
	{
		PlayerWon = playerWon;
		PlayerForfeited = playerForfeited;
	}

	public Texture2D GetRewardCardTexture()
	{
		Texture2D texture2D = CardManager.LoadCardTexture(RewardCard);
		if (texture2D != null)
		{
			return texture2D;
		}
		CspUtils.DebugLog("Unable to find card texture for " + RewardCard);
		return null;
	}

	private void OnCardGameResults(CardGameEvent.CardGameResults evt)
	{
		CspUtils.DebugLog("OnCardGameResults " + evt.silver + " " + evt.xp + " " + evt);
		RewardsReceived = true;
		SilverReward = evt.silver;
		XpReward = evt.xp;
		TicketReward = evt.tickets;
		if (!string.IsNullOrEmpty(evt.card))
		{
			RewardCard = evt.card;
		}
		if (!string.IsNullOrEmpty(evt.ownable))
		{
			OwnableRewards = evt.ownable;
		}
	}

	private void UpdateNextQuest(StartInfo start)
	{
		if (start == null)
		{
			return;
		}
		if (!string.IsNullOrEmpty(start.RewardCard))
		{
			RewardCard = start.RewardCard;
		}
		IsCardQuest = (start.QuestID != -1);
		if (IsCardQuest)
		{
			CardQuestPart questPart = AppShell.Instance.CardQuestManager.GetQuestPart(start.QuestID);
			if (questPart != null)
			{
				foreach (CardQuestPart.QuestBattle node in questPart.Nodes)
				{
					if (node.Id == start.QuestNodeID)
					{
						CurrentCardQuestBattle = node;
						break;
					}
				}
			}
		}
		HaveNextQuestEnabled = true;
		if (!IsCardQuest || start.Players[1].Type != PlayerType.AI || !PlayerWon)
		{
			HaveNextQuestEnabled = false;
		}
		else if (CurrentCardQuestBattle != null && CurrentCardQuestBattle.NextQuest == null)
		{
			HaveNextQuestEnabled = false;
			CardQuestPart parentCardQuestPart = CurrentCardQuestBattle.ParentCardQuestPart;
			if (parentCardQuestPart.Nodes.IndexOf(CurrentCardQuestBattle) == parentCardQuestPart.Nodes.Count - 1 && parentCardQuestPart.PartType == CardQuestPartsTypeEnum.Easy)
			{
				HaveNextQuestEnabled = true;
			}
		}
	}

	public override void Show()
	{
		base.Show();
		StartInfo startInfo = AppShell.Instance.SharedHashTable["CardGameLevel"] as StartInfo;
		if (startInfo != null)
		{
			UpdateNextQuest(startInfo);
			if (startInfo.Players[1].Type == PlayerType.AI)
			{
				CardGameSummaryWindow cardGameSummaryWindow = new CardGameSummaryWindow(this);
				GUIManager.Instance.ShowDynamicWindow(cardGameSummaryWindow, ModalLevelEnum.None);
				cardGameSummaryWindow.OnHidden += delegate
				{
					CardGameRewardsWindow dialogWindow2 = new CardGameRewardsWindow(this);
					GUIManager.Instance.ShowDynamicWindow(dialogWindow2, ModalLevelEnum.None);
				};
			}
			else
			{
				CardGameRewardsWindow dialogWindow = new CardGameRewardsWindow(this);
				CardGameController.Instance.DisableHud();
				SquadBattleCharacterController.Instance.RewardsPresentation();
				GUIManager.Instance.ShowDynamicWindow(dialogWindow, ModalLevelEnum.None);
			}
		}
		CardGameController.Instance.AudioManager.Play((!PlayerWon) ? CardGameAudioManager.SFX.YouLose : CardGameAudioManager.SFX.YouWin);
		CardGameController.Instance.AudioManager.PlayEndGameVO(PlayerWon);
	}

	private void FirePvPChallengeEvent()
	{
	}

	public bool WonRewards()
	{
		return RewardsReceived && (XpReward > 0 || SilverReward > 0 || TicketReward > 0);
	}
}
