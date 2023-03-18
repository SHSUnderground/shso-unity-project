using UnityEngine;

public class SHSMySquadChallengeChallengesWindow : SHSGadget.GadgetCenterWindow
{
	public class ChallengeListWindow : SHSSelectionWindow<SHSMySquadChallengeListItem, GUISimpleControlWindow>
	{
		public ChallengeListWindow(GUISlider slider)
			: base(slider, 290f, new Vector2(279f, 200f), 10)
		{
			SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
			slider.FireChanged();
		}

		public void Sort(string sort)
		{
		}
	}

	protected MySquadDataManager dataManager;

	public SHSMySquadChallengeHeroRewardWindow heroRewardWindow;

	private GUISlider slider;

	private ChallengeListWindow challengeWindow;

	private GUIStrokeTextLabel rewardTitle;

	private GUIImage challengeListOverlay;

	private bool stupidFirstUpdateHackForRefresh;

	public SHSMySquadChallengeChallengesWindow(MySquadDataManager dataManager)
	{
		this.dataManager = dataManager;
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, ColorUtil.FromRGB255(255, 255, 254), ColorUtil.FromRGB255(0, 65, 157), ColorUtil.FromRGB255(0, 37, 89), new Vector2(-3f, 3f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-316f, -205f), new Vector2(200f, 25f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.Text = "#SQ_MYSQUAD_ALLCHALLENGES";
		rewardTitle = new GUIStrokeTextLabel();
		rewardTitle.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, ColorUtil.FromRGB255(255, 255, 254), ColorUtil.FromRGB255(0, 65, 157), ColorUtil.FromRGB255(0, 37, 89), new Vector2(-3f, 3f), TextAnchor.MiddleCenter);
		rewardTitle.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(168f, -204f), new Vector2(200f, 25f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		rewardTitle.Text = string.Empty;
		Add(rewardTitle);
		slider = GUIControl.CreateControlAbsolute<GUISlider>(new Vector2(50f, 380f), new Vector2(281f, 155f));
		slider.Id = "MySquadChallengePageSlider";
		challengeWindow = new ChallengeListWindow(slider);
		challengeWindow.Id = "challengeWindow";
		challengeWindow.SetSize(290f, 399f);
		challengeWindow.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-300f, 25f));
		Add(challengeWindow);
		challengeListOverlay = new GUIImage();
		challengeListOverlay.SetSize(341f, 502f);
		challengeListOverlay.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-316f, 17f));
		challengeListOverlay.TextureSource = "mysquadgadget_bundle|mysquad_2panel_left_overlay";
		Add(challengeListOverlay);
		Add(slider);
		Add(gUIStrokeTextLabel);
		heroRewardWindow = new SHSMySquadChallengeHeroRewardWindow(dataManager);
		heroRewardWindow.SetPosition(QuickSizingHint.Centered);
		heroRewardWindow.SetSize(new Vector2(1000f, 700f));
		heroRewardWindow.Offset = new Vector2(200f, 0f);
		Add(heroRewardWindow);
	}

	public override void OnShow()
	{
		UpdateHeroView();
		base.OnShow();
	}

	public void UpdateHeroView()
	{
		if (dataManager.Profile is LocalPlayerProfile)
		{
			UpdateLocalHeroView();
		}
		else if (dataManager.Profile is RemotePlayerProfile)
		{
			UpdateRemoteHeroView();
		}
	}

	public void UpdateLocalHeroView()
	{
		challengeWindow.ClearItems();
		ChallengeManager challengeManager = AppShell.Instance.ChallengeManager;
		IChallenge challenge = null;
		ChallengeInfo challengeInfo = null;
		ChallengeManager.ChallengeManagerStateEnum currentState;
		challengeManager.GetChallengeManagerDisplayInfo(out currentState, out challenge, out challengeInfo);
		for (int i = 1; i <= challengeManager.ChallengeDictionary.Keys.Count; i++)
		{
			if (Singleton<Entitlements>.instance.MaxCheck(Entitlements.EntitlementFlagEnum.MaxChallengeLevel, i))
			{
				SHSMySquadChallengeListItem item = new SHSMySquadChallengeListItem(i, challengeManager, null);
				challengeWindow.AddItem(item);
			}
		}
		int currentChallenge = -1;
		if (challenge != null)
		{
			currentChallenge = challenge.Id;
		}
		else if (challengeInfo != null)
		{
			currentChallenge = challengeInfo.ChallengeId;
		}
		SetCurrentChallenge(currentChallenge);
		slider.FireChanged();
		heroRewardWindow.UpdateHeroView();
		if (currentState == ChallengeManager.ChallengeManagerStateEnum.ChallengeDisplayPending && challengeInfo.Reward.rewardType == ChallengeRewardType.Hero && challengeInfo.Reward.grantMode == ChallengeGrantMode.Manual)
		{
			rewardTitle.Text = "#SQ_MYSQUAD_PICKHERO";
		}
		else
		{
			rewardTitle.Text = "#SQ_MYSQUAD_HEROREWARDS";
		}
	}

	public void UpdateRemoteHeroView()
	{
		challengeWindow.ClearItems();
		RemotePlayerProfile remotePlayerProfile = dataManager.Profile as RemotePlayerProfile;
		if (remotePlayerProfile == null)
		{
			CspUtils.DebugLog("Current profile for this UI is not a remote player");
			return;
		}
		ChallengeManager challengeManager = AppShell.Instance.ChallengeManager;
		for (int i = 1; i <= challengeManager.ChallengeDictionary.Keys.Count; i++)
		{
			if (Singleton<Entitlements>.instance.MaxCheck(Entitlements.EntitlementFlagEnum.MaxChallengeLevel, i))
			{
				SHSMySquadChallengeListItem item = new SHSMySquadChallengeListItem(i, challengeManager, remotePlayerProfile);
				challengeWindow.AddItem(item);
			}
		}
		SetCurrentChallenge(remotePlayerProfile.CurrentChallenge);
		slider.FireChanged();
		heroRewardWindow.UpdateHeroView();
		rewardTitle.Text = "#SQ_MYSQUAD_HEROREWARDS";
	}

	private void SetCurrentChallenge(int curChallenge)
	{
		if (curChallenge > 1)
		{
			slider.Value = (float)(curChallenge - 1) * challengeWindow.ItemSize.y + 12f;
			challengeWindow.UpdateDisplay();
		}
	}

	public override void OnUpdate()
	{
		if (!stupidFirstUpdateHackForRefresh)
		{
			stupidFirstUpdateHackForRefresh = true;
			int currentChallenge = -1;
			if (dataManager.Profile is LocalPlayerProfile)
			{
				ChallengeManager challengeManager = AppShell.Instance.ChallengeManager;
				IChallenge challenge = null;
				ChallengeInfo challengeInfo = null;
				ChallengeManager.ChallengeManagerStateEnum currentState;
				challengeManager.GetChallengeManagerDisplayInfo(out currentState, out challenge, out challengeInfo);
				if (challenge != null)
				{
					currentChallenge = challenge.Id;
				}
				else if (challengeInfo != null)
				{
					currentChallenge = challengeInfo.ChallengeId;
				}
			}
			else
			{
				RemotePlayerProfile remotePlayerProfile = dataManager.Profile as RemotePlayerProfile;
				currentChallenge = remotePlayerProfile.CurrentChallenge;
			}
			SetCurrentChallenge(currentChallenge);
		}
		base.OnUpdate();
	}

	public override void OnHide()
	{
		challengeWindow.ClearItems();
		base.OnHide();
	}
}
