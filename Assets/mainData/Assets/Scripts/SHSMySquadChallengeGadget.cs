using System;
using UnityEngine;

public class SHSMySquadChallengeGadget : SHSGadget
{
	private GUIButton quitButton;

	private SHSMySquadChallengeTabStrip tabStrip;

	private SHSMySquadChallengeMySquadWindow squadWindow;

	private SHSMySquadChallengeChallengesWindow challengesWindow;

	private SHSMySquadChallengeFractalsWindow fractalsWindow;

	private ChallengeCraftingWindow craftingWindow;

	private MySquadDataManager squadDataManager;

	private AnimClip greenLightAnim;

	private GUIImage greenLightImage;

	private GUIImage challengePanel;

	private bool isMySquad;

	private static SHSMySquadChallengeTabStrip.TabType initialSelectedTab;

	private GUIImage overLay;

	public static bool InUse;

	public SHSMySquadChallengeGadget(SHSMySquadChallengeTabStrip.TabType? selectedTab)
		: this(AppShell.Instance.Profile.UserId, AppShell.Instance.Profile.PlayerName, AppShell.Instance.Profile.SquadLevel, selectedTab)
	{
		InUse = true;
	}

	public SHSMySquadChallengeGadget(long playerId, string playerName, int playerSquadLevel, SHSMySquadChallengeTabStrip.TabType? initialSelectedTab)
	{
		isMySquad = (playerId == AppShell.Instance.Profile.UserId);
		SetBackgroundSize(new Vector2(1020f, 644f));
		SetBackgroundImage("mysquadgadget_bundle|mshs_mysquad_challenge_frame");
		CloseButton.Offset = new Vector2(458f, -604f);
		CloseButton.SetSize(new Vector2(64f, 64f));
		CloseButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|button_mysquad_close");
		base.AnimationOpenFinished += SetUpGadget;
		squadDataManager = new MySquadDataManager(playerId, playerName, playerSquadLevel);
		if (initialSelectedTab.HasValue)
		{
			SHSMySquadChallengeGadget.initialSelectedTab = initialSelectedTab.Value;
		}
	}

	protected void SetUpGadget()
	{
		squadWindow = new SHSMySquadChallengeMySquadWindow(squadDataManager);
		challengesWindow = new SHSMySquadChallengeChallengesWindow(squadDataManager);
		fractalsWindow = new SHSMySquadChallengeFractalsWindow(squadDataManager);
		craftingWindow = new ChallengeCraftingWindow();
		quitButton = new GUIButton();
		quitButton.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -1f), new Vector2(325f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		quitButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|L_mshs_mysquad_challenge_quit");
		quitButton.MouseDown += delegate
		{
			CloseGadget();
		};
		Add(quitButton);
		SetCenterWindow(squadWindow);
		ControlToFront(squadWindow);
		overLay = new GUIImage();
		overLay.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(1020f, 644f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		overLay.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_overlay";
		Add(overLay);
		fade.Add(overLay);
		greenLightImage = new GUIImage();
		greenLightImage.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(-458f, -589f), new Vector2(32f, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		greenLightImage.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_greenlight_on";
		Add(greenLightImage);
		StartGreenLight();
		challengePanel = new GUIImage();
		challengePanel.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(-1f, -50f), new Vector2(974f, 502f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		challengePanel.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_panel";
		Add(challengePanel);
		fade.Add(challengePanel);
		challengePanel.IsVisible = false;
		OnTabSelected(initialSelectedTab);
	}

	protected void StartGreenLight()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		AnimClip animClip = GetGreenLightAnim(greenLightImage);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			StartGreenLight();
		};
		base.AnimationPieceManager.SwapOut(ref greenLightAnim, animClip);
	}

	protected static AnimClip GetGreenLightAnim(GUIImage img)
	{
		return AnimClipBuilder.Absolute.AnimationAlpha(AnimClipBuilder.Path.Linear(0f, 1f, 0.5f) | AnimClipBuilder.Path.Linear(1f, 0f, 0.5f), img);
	}

	protected void OnTabSelected(SHSMySquadChallengeTabStrip.TabType tabType)
	{
		initialSelectedTab = tabType;
		if (tabType == SHSMySquadChallengeTabStrip.TabType.Fractals)
		{
			SetCenterWindowImmediate(fractalsWindow);
			ControlToFront(fractalsWindow);
			challengePanel.IsVisible = false;
		}
		ControlToFront(overLay);
	}

	public override void OnShow()
	{
		AppShell.Instance.EventMgr.AddListener<ChallengeManagerStateChangedMessage>(OnChallengeManagerStateChanged);
		base.OnShow();
	}

	protected void OnChallengeManagerStateChanged(ChallengeManagerStateChangedMessage msg)
	{
		challengesWindow.UpdateHeroView();
	}

	public override void OnHide()
	{
		base.OnHide();
		AppShell.Instance.EventMgr.RemoveListener<ChallengeManagerStateChangedMessage>(OnChallengeManagerStateChanged);
		if (squadDataManager != null)
		{
			squadDataManager.ClearProfile();
		}
		InUse = false;
	}
}
