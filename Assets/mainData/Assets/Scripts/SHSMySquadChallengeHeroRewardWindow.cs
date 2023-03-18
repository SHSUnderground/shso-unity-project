using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSMySquadChallengeHeroRewardWindow : GUISimpleControlWindow
{
	private class HeroImageInfo
	{
		public bool owned;

		public string heroName;

		public Vector2 ownedSize;

		public Vector2 ownedPosition;

		public Vector2 buttonSize;

		public Vector2 buttonPosition;

		public int drawPosition;

		public bool autoGranted;

		public GUIControl control;
	}

	private readonly Dictionary<string, HeroImageInfo> heroButtons;

	private GUIStrokeTextLabel heroNameLabel;

	private GUIDropShadowTextLabel heroInfoLabel;

	private MySquadDataManager dataManager;

	private List<GUIImage> lockImages;

	protected AnimClip animClipFadeInOut;

	protected AnimClip animClipSelectedPulse;

	private string heroChosenTemporary;

	public SHSMySquadChallengeHeroRewardWindow(MySquadDataManager dataManager)
	{
		this.dataManager = dataManager;
		heroButtons = new Dictionary<string, HeroImageInfo>();
		Dictionary<string, HeroImageInfo> dictionary = heroButtons;
		HeroImageInfo heroImageInfo = new HeroImageInfo();
		heroImageInfo.heroName = "modok_playable";
		heroImageInfo.ownedSize = new Vector2(155f, 147f);
		heroImageInfo.ownedPosition = new Vector2(0f, 0f);
		heroImageInfo.buttonSize = new Vector2(256f, 256f);
		heroImageInfo.buttonPosition = new Vector2(-247f, -97f);
		heroImageInfo.drawPosition = 1;
		heroImageInfo.autoGranted = true;
		dictionary["modok_playable"] = heroImageInfo;
		Dictionary<string, HeroImageInfo> dictionary2 = heroButtons;
		heroImageInfo = new HeroImageInfo();
		heroImageInfo.heroName = "colossus";
		heroImageInfo.ownedSize = new Vector2(246f, 254f);
		heroImageInfo.ownedPosition = new Vector2(0f, 0f);
		heroImageInfo.buttonSize = new Vector2(512f, 512f);
		heroImageInfo.buttonPosition = new Vector2(-131f, 5f);
		heroImageInfo.drawPosition = 1;
		heroImageInfo.autoGranted = true;
		dictionary2["colossus"] = heroImageInfo;
		Dictionary<string, HeroImageInfo> dictionary3 = heroButtons;
		heroImageInfo = new HeroImageInfo();
		heroImageInfo.heroName = "iron_man_stealth";
		heroImageInfo.ownedSize = new Vector2(199f, 211f);
		heroImageInfo.ownedPosition = new Vector2(0f, 0f);
		heroImageInfo.buttonSize = new Vector2(256f, 256f);
		heroImageInfo.buttonPosition = new Vector2(39f, -24f);
		heroImageInfo.drawPosition = 2;
		heroImageInfo.autoGranted = true;
		dictionary3["iron_man_stealth"] = heroImageInfo;
		Dictionary<string, HeroImageInfo> dictionary4 = heroButtons;
		heroImageInfo = new HeroImageInfo();
		heroImageInfo.heroName = "spider_man_future";
		heroImageInfo.ownedSize = new Vector2(268f, 294f);
		heroImageInfo.ownedPosition = new Vector2(-57f, -63f);
		heroImageInfo.buttonSize = new Vector2(512f, 512f);
		heroImageInfo.buttonPosition = new Vector2(-57f, -63f);
		heroImageInfo.drawPosition = 3;
		heroImageInfo.autoGranted = true;
		dictionary4["spider_man_future"] = heroImageInfo;
		Dictionary<string, HeroImageInfo> dictionary5 = heroButtons;
		heroImageInfo = new HeroImageInfo();
		heroImageInfo.heroName = "elektra";
		heroImageInfo.ownedSize = new Vector2(208f, 118f);
		heroImageInfo.ownedPosition = new Vector2(0f, 0f);
		heroImageInfo.buttonSize = new Vector2(256f, 256f);
		heroImageInfo.buttonPosition = new Vector2(-217f, 125f);
		heroImageInfo.drawPosition = 4;
		dictionary5["elektra"] = heroImageInfo;
		Dictionary<string, HeroImageInfo> dictionary6 = heroButtons;
		heroImageInfo = new HeroImageInfo();
		heroImageInfo.heroName = "daredevil";
		heroImageInfo.ownedSize = new Vector2(165f, 128f);
		heroImageInfo.ownedPosition = new Vector2(-161f, 101f);
		heroImageInfo.buttonSize = new Vector2(256f, 256f);
		heroImageInfo.buttonPosition = new Vector2(-161f, 101f);
		heroImageInfo.drawPosition = 5;
		dictionary6["daredevil"] = heroImageInfo;
		Dictionary<string, HeroImageInfo> dictionary7 = heroButtons;
		heroImageInfo = new HeroImageInfo();
		heroImageInfo.heroName = "firestar";
		heroImageInfo.ownedSize = new Vector2(190f, 175f);
		heroImageInfo.ownedPosition = new Vector2(82f, 61f);
		heroImageInfo.buttonSize = new Vector2(256f, 256f);
		heroImageInfo.buttonPosition = new Vector2(-69f, 73f);
		heroImageInfo.drawPosition = 6;
		dictionary7["firestar"] = heroImageInfo;
		Dictionary<string, HeroImageInfo> dictionary8 = heroButtons;
		heroImageInfo = new HeroImageInfo();
		heroImageInfo.heroName = "wolverine_jeans";
		heroImageInfo.ownedSize = new Vector2(256f, 129f);
		heroImageInfo.ownedPosition = new Vector2(140f, 122f);
		heroImageInfo.buttonSize = new Vector2(512f, 512f);
		heroImageInfo.buttonPosition = new Vector2(140f, 122f);
		heroImageInfo.drawPosition = 7;
		dictionary8["wolverine_jeans"] = heroImageInfo;
		Dictionary<string, HeroImageInfo> dictionary9 = heroButtons;
		heroImageInfo = new HeroImageInfo();
		heroImageInfo.heroName = "storm_mohawk";
		heroImageInfo.ownedSize = new Vector2(159f, 169f);
		heroImageInfo.ownedPosition = new Vector2(0f, 0f);
		heroImageInfo.buttonSize = new Vector2(256f, 256f);
		heroImageInfo.buttonPosition = new Vector2(102f, 91f);
		heroImageInfo.drawPosition = 8;
		dictionary9["storm_mohawk"] = heroImageInfo;
		Dictionary<string, HeroImageInfo> dictionary10 = heroButtons;
		heroImageInfo = new HeroImageInfo();
		heroImageInfo.heroName = "sentry";
		heroImageInfo.ownedSize = new Vector2(243f, 224f);
		heroImageInfo.ownedPosition = new Vector2(0f, 0f);
		heroImageInfo.buttonSize = new Vector2(512f, 512f);
		heroImageInfo.buttonPosition = new Vector2(81f, 61f);
		heroImageInfo.drawPosition = 9;
		dictionary10["sentry"] = heroImageInfo;
		lockImages = new List<GUIImage>();
	}

	public override bool InitializeResources(bool reload)
	{
		if (!reload)
		{
			UpdateHeroView();
		}
		return base.InitializeResources(reload);
	}

	public void UpdateHeroView()
	{
		UserProfile profile = dataManager.Profile;
		if (profile == null)
		{
			return;
		}
		foreach (HeroImageInfo value in heroButtons.Values)
		{
			if (profile.AvailableCostumes.ContainsKey(value.heroName) || heroChosenTemporary == value.heroName)
			{
				value.owned = true;
			}
		}
		bool flag = false;
		ChallengeManager challengeManager = AppShell.Instance.ChallengeManager;
		IChallenge challenge = null;
		ChallengeInfo challengeInfo = null;
		if (profile is LocalPlayerProfile)
		{
			ChallengeManager.ChallengeManagerStateEnum currentState;
			challengeManager.GetChallengeManagerDisplayInfo(out currentState, out challenge, out challengeInfo);
			if (currentState == ChallengeManager.ChallengeManagerStateEnum.ChallengeDisplayPending && challengeInfo.Reward.rewardType == ChallengeRewardType.Hero && challengeInfo.Reward.grantMode == ChallengeGrantMode.Manual)
			{
				flag = true;
			}
		}
		foreach (HeroImageInfo value2 in heroButtons.Values)
		{
			if (value2.control != null)
			{
				Remove(value2.control);
				value2.control.Dispose();
				value2.control = null;
			}
		}
		foreach (GUIImage lockImage2 in lockImages)
		{
			Remove(lockImage2);
		}
		lockImages.Clear();
		controlList.ForEach(Remove);
		controlList.Clear();
		GUIImage gUIImage = new GUIImage();
		gUIImage.SetPosition(QuickSizingHint.Centered);
		gUIImage.SetSize(new Vector2(621f, 469f));
		gUIImage.Offset = new Vector2(-28f, 9f);
		gUIImage.TextureSource = "mysquadgadget_bundle|hero_rewards_noneawarded";
		Add(gUIImage);
		int num = 1;
		if (heroNameLabel == null)
		{
			heroNameLabel = new GUIStrokeTextLabel();
		}
		heroNameLabel.Id = "heroNameLabel";
		heroNameLabel.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(-45f, -144f), new Vector2(500f, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		heroNameLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 22, ColorUtil.FromRGB255(255, 242, 94), ColorUtil.FromRGB255(173, 32, 25), ColorUtil.FromRGB255(0, 15, 41), new Vector2(-3f, 4f), TextAnchor.MiddleCenter);
		heroNameLabel.Text = string.Empty;
		if (heroInfoLabel == null)
		{
			heroInfoLabel = new GUIDropShadowTextLabel();
		}
		heroInfoLabel.Id = "heroInfoLabel";
		heroInfoLabel.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(-45f, -123f), new Vector2(500f, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		heroInfoLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, ColorUtil.FromRGB255(173, 110, 11), ColorUtil.FromRGB255(1, 11, 28), new Vector2(-1f, 1f), TextAnchor.MiddleCenter);
		heroInfoLabel.Bold = true;
		heroInfoLabel.Text = string.Empty;
		heroInfoLabel.IsVisible = true;
		foreach (HeroImageInfo value3 in heroButtons.Values)
		{
			int drawPosition = value3.drawPosition;
			string heroName = value3.heroName;
			bool autoGranted = value3.autoGranted;
			GUIButton button = GUIControl.CreateControlFrameCentered<GUIButton>(value3.buttonSize, value3.buttonPosition);
			button.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|hero_rewards_" + value3.heroName, SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
			button.Id = value3.heroName;
			button.HitTestType = HitTestTypeEnum.Alpha;
			GUIImage lockImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(463f, 265f), new Vector2(-24f, 50f));
			lockImage.Id = "lockImage_" + value3.heroName;
			lockImage.TextureSource = "mysquadgadget_bundle|hero_rewards_" + heroName + "_lock_normal";
			lockImages.Add(lockImage);
			Add(button);
			if (value3.owned && IsPlayerAtHeroGrantLevel(value3))
			{
				lockImage.IsVisible = false;
				button.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|hero_rewards_" + value3.heroName, SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
				button.MouseOver += delegate
				{
					button.BringToFront();
					heroNameLabel.Text = string.Format("#CIN_{0}_EXNM", heroName.ToUpper());
					heroInfoLabel.Text = "#SQ_HERO_ALREADY_UNLOCKED";
				};
				button.MouseOut += delegate
				{
					button.Parent.Remove(button);
					if (drawPosition - 1 < 0 || drawPosition - 1 > button.Parent.ControlList.Count)
					{
						CspUtils.DebugLog("Draw Position invalid: " + (drawPosition - 1));
					}
					else
					{
						button.Parent.Add(button, button.Parent.ControlList[drawPosition - 1]);
					}
					heroNameLabel.Text = string.Empty;
					heroInfoLabel.Text = string.Empty;
					button.IsVisible = true;
				};
			}
			else if (flag)
			{
				if (autoGranted)
				{
					button.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|hero_rewards_" + value3.heroName, SHSButtonStyleInfo.SupportedStatesEnum.Normal);
				}
				button.MouseOver += delegate
				{
					if (!autoGranted)
					{
						button.BringToFront();
						lockImage.IsVisible = false;
						heroInfoLabel.Text = "#SQ_HERO_AVAILABLE";
					}
					else
					{
						ChallengeInfo heroChallenge2 = AppShell.Instance.ChallengeManager.GetHeroChallenge(heroName);
						if (heroChallenge2 != null)
						{
							heroInfoLabel.Text = string.Format(AppShell.Instance.stringTable["#SQ_HERO_UNLOCKED_CHALLENGE"], heroChallenge2.ChallengeId);
						}
					}
					heroNameLabel.Text = string.Format("#CIN_{0}_EXNM", heroName.ToUpper());
				};
				button.MouseOut += delegate
				{
					if (!autoGranted)
					{
						button.Parent.Remove(button);
						if (drawPosition - 1 < 0 || drawPosition - 1 > button.Parent.ControlList.Count)
						{
							CspUtils.DebugLog("Draw Position invalid: " + (drawPosition - 1));
						}
						else
						{
							button.Parent.Add(button, button.Parent.ControlList[drawPosition - 1]);
						}
					}
					heroNameLabel.Text = string.Empty;
					heroInfoLabel.Text = string.Empty;
					button.IsVisible = true;
					lockImage.IsVisible = true;
					lockImage.TextureSource = "mysquadgadget_bundle|hero_rewards_" + heroName + "_lock_normal";
				};
				if (!autoGranted)
				{
					button.Click += delegate
					{
						GUIManager.Instance.ShowDialog(typeof(SHSMySquadChallengeHeroSelectConfirmDialog), delegate(string Id, GUIDialogWindow.DialogState state)
						{
							if (state == GUIDialogWindow.DialogState.Ok)
							{
								AppShell.Instance.ChallengeManager.RewardSelected(challengeInfo, heroName);
								heroChosenTemporary = heroName;
							}
						}, ModalLevelEnum.Default);
					};
				}
			}
			else
			{
				button.MouseOver += delegate
				{
					button.BringToFront();
					lockImage.TextureSource = "mysquadgadget_bundle|hero_rewards_" + heroName + "_lock_highlight";
					lockImage.BringToFront();
					heroNameLabel.Text = string.Format("#CIN_{0}_EXNM", heroName.ToUpper());
					OnHeroSelected(lockImage);
					if (autoGranted)
					{
						ChallengeInfo heroChallenge = AppShell.Instance.ChallengeManager.GetHeroChallenge(heroName);
						if (heroChallenge != null)
						{
							heroInfoLabel.Text = string.Format(AppShell.Instance.stringTable["#SQ_HERO_UNLOCKED_CHALLENGE"], heroChallenge.ChallengeId);
						}
					}
					else
					{
						heroInfoLabel.Text = "#SQ_HERO_SERUM_UNLOCKED";
					}
				};
				button.MouseOut += delegate
				{
					button.Parent.Remove(button);
					if (drawPosition - 1 < 0 || drawPosition - 1 > button.Parent.ControlList.Count)
					{
						CspUtils.DebugLog("Draw Position invalid: " + (drawPosition - 1));
					}
					else
					{
						button.Parent.Add(button, button.Parent.ControlList[drawPosition - 1]);
					}
					heroNameLabel.Text = string.Empty;
					heroInfoLabel.Text = string.Empty;
					button.IsVisible = true;
					lockImage.IsVisible = true;
					lockImage.TextureSource = "mysquadgadget_bundle|hero_rewards_" + heroName + "_lock_normal";
					OnHeroUnSelected(lockImage);
				};
			}
			value3.control = button;
			num = (value3.drawPosition = num + 1);
		}
		foreach (GUIImage lockImage3 in lockImages)
		{
			Add(lockImage3);
		}
		Add(heroNameLabel);
		Add(heroInfoLabel);
	}

	private void OnHeroSelected(GUIImage lockImage)
	{
		AnimClip animClip = null;
		foreach (GUIImage lockImage2 in lockImages)
		{
			if (lockImage2 != lockImage)
			{
				if (animClip != null)
				{
					animClip ^= AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(lockImage2.Alpha, 0.4f, 0.5f), lockImage2);
				}
				else
				{
					animClip = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(lockImage2.Alpha, 0.4f, 0.5f), lockImage2);
				}
			}
			else if (animClip != null)
			{
				animClip ^= AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(lockImage2.Alpha, 1f, 0.5f), lockImage2);
			}
			else
			{
				animClip = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(lockImage2.Alpha, 1f, 0.5f), lockImage2);
			}
		}
		base.AnimationPieceManager.SwapOut(ref animClipFadeInOut, animClip);
		PulseSelectedLock(lockImage);
	}

	private void PulseSelectedLock(GUIImage lockImage)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		AnimClip animClip = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0.4f, 1f, 1f), lockImage) | AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(1f, 0.4f, 1f), lockImage);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			PulseSelectedLock(lockImage);
		};
		base.AnimationPieceManager.SwapOut(ref animClipSelectedPulse, animClip);
	}

	private void OnHeroUnSelected(GUIImage lockImage)
	{
		AnimClip animClip = null;
		foreach (GUIImage lockImage2 in lockImages)
		{
			if (lockImage2 == lockImage)
			{
				AnimClip newPiece = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(lockImage2.Alpha, 1f, 0.5f), lockImage2);
				base.AnimationPieceManager.SwapOut(ref animClipSelectedPulse, newPiece);
			}
			else if (animClip != null)
			{
				animClip ^= AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(lockImage2.Alpha, 1f, 0.5f), lockImage2);
			}
			else
			{
				animClip = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(lockImage2.Alpha, 1f, 0.5f), lockImage2);
			}
		}
		base.AnimationPieceManager.SwapOut(ref animClipFadeInOut, animClip);
	}

	private bool IsPlayerAtHeroGrantLevel(HeroImageInfo heroInfo)
	{
		UserProfile profile = dataManager.Profile;
		if (profile == null)
		{
			return false;
		}
		ChallengeManager challengeManager = AppShell.Instance.ChallengeManager;
		foreach (ChallengeInfo value in challengeManager.ChallengeDictionary.Values)
		{
			if (value.Reward.rewardType == ChallengeRewardType.Hero)
			{
				if (value.Reward.qualifier == heroInfo.heroName && value.Reward.grantMode == ChallengeGrantMode.Auto)
				{
					return challengeManager.LastViewedChallengeId >= value.ChallengeId;
				}
				return true;
			}
		}
		return false;
	}
}
