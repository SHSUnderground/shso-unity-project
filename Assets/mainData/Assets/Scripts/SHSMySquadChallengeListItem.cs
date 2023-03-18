using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SHSMySquadChallengeListItem : SHSSelectionItem<GUISimpleControlWindow>, IComparable<SHSMySquadChallengeListItem>
{
	public enum ChallengeState
	{
		Uninitialized,
		NotStarted,
		InProgress,
		Completed
	}

	public delegate void StateChangeDelegate();

	private const float challengeDescYOff = 68f;

	private const float challengeDescYInc = 20f;

	private Dictionary<ChallengeState, StateChangeDelegate> stateMethods;

	private ChallengeManager manager;

	private RemotePlayerProfile profile;

	private GUIImage bgd;

	private GUIImage highlightBgd;

	private GUIStrokeTextLabel title;

	private GUIStrokeTextLabel challengeName;

	private GUIDropShadowTextLabel challengeDesc;

	private GUIDrawTexture challengeCompleteImage;

	private AnimClip animClipHackRef;

	private AnimClip pulsingClip;

	[CompilerGenerated]
	private ChallengeState _003CState_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CChallengeId_003Ek__BackingField;

	public ChallengeState State
	{
		[CompilerGenerated]
		get
		{
			return _003CState_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CState_003Ek__BackingField = value;
		}
	}

	public int ChallengeId
	{
		[CompilerGenerated]
		get
		{
			return _003CChallengeId_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CChallengeId_003Ek__BackingField = value;
		}
	}

	public SHSMySquadChallengeListItem(int challengeId, ChallengeManager mgr, RemotePlayerProfile profile)
	{
		this.profile = profile;
		stateMethods = new Dictionary<ChallengeState, StateChangeDelegate>();
		stateMethods[ChallengeState.NotStarted] = NotStartedState;
		stateMethods[ChallengeState.InProgress] = InProgressState;
		stateMethods[ChallengeState.Completed] = CompletedState;
		ChallengeId = challengeId;
		manager = mgr;
		ChallengeInfo challengeInfo = mgr.ChallengeDictionary[ChallengeId];
		Initialize(challengeId, challengeInfo);
		State = ChallengeState.Uninitialized;
		SetState();
	}

	private void Initialize(int challengeId, ChallengeInfo challengeInfo)
	{
		item = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(279f, 214f), Vector2.zero);
		item.Id = "ChallengeWindow_" + challengeId;
		itemSize = new Vector2(279f, 214f);
		bgd = new GUIImage();
		bgd.SetPositionAndSize(GUIControl.DockingAlignmentEnum.Middle, GUIControl.AnchorAlignmentEnum.Middle, new Vector2(279f, 214f));
		item.Add(bgd);
		highlightBgd = new GUIImage();
		highlightBgd.SetPositionAndSize(GUIControl.DockingAlignmentEnum.Middle, GUIControl.AnchorAlignmentEnum.Middle, new Vector2(279f, 214f));
		item.Add(highlightBgd);
		title = new GUIStrokeTextLabel();
		title.SetPositionAndSize(GUIControl.DockingAlignmentEnum.TopMiddle, GUIControl.AnchorAlignmentEnum.TopMiddle, new Vector2(279f, 30f));
		title.Offset = new Vector2(0f, 8f);
		title.Bold = true;
		item.Add(title);
		challengeName = new GUIStrokeTextLabel();
		challengeName.SetPositionAndSize(GUIControl.DockingAlignmentEnum.TopLeft, GUIControl.AnchorAlignmentEnum.TopLeft, new Vector2(241f, 40f));
		challengeName.Offset = new Vector2(19f, 46f);
		challengeName.Text = AppShell.Instance.stringTable[challengeInfo.Name];
		challengeName.Bold = true;
		item.Add(challengeName);
		challengeDesc = new GUIDropShadowTextLabel();
		challengeDesc.SetPositionAndSize(GUIControl.DockingAlignmentEnum.TopLeft, GUIControl.AnchorAlignmentEnum.TopLeft, new Vector2(240f, 84f));
		challengeDesc.Offset = new Vector2(20f, 68f);
		challengeDesc.Text = AppShell.Instance.stringTable[challengeInfo.Description];
		challengeDesc.Bold = true;
		item.Add(challengeDesc);
		challengeCompleteImage = new GUIDrawTexture();
		challengeCompleteImage.SetPositionAndSize(GUIControl.DockingAlignmentEnum.Middle, GUIControl.AnchorAlignmentEnum.Middle, new Vector2(279f, 214f));
		challengeCompleteImage.TextureSource = "mysquadgadget_bundle|L_challenge_complete_banner";
		challengeCompleteImage.MouseOver += delegate
		{
			AnimClip newPiece2 = SHSAnimations.Generic.FadeOut(challengeCompleteImage, 0.5f);
			item.AnimationPieceManager.SwapOut(ref animClipHackRef, newPiece2);
		};
		challengeCompleteImage.MouseOut += delegate
		{
			AnimClip newPiece = SHSAnimations.Generic.FadeIn(challengeCompleteImage, 0.5f);
			item.AnimationPieceManager.SwapOut(ref animClipHackRef, newPiece);
		};
		item.Add(challengeCompleteImage);
	}

	private void PositionDescription()
	{
		challengeName.CalculateTextLayout();
		float y = Mathf.Max(68f, 68f + (float)(challengeName.LineCount - 1) * 20f);
		GUIDropShadowTextLabel gUIDropShadowTextLabel = challengeDesc;
		Vector2 offset = challengeDesc.Offset;
		gUIDropShadowTextLabel.Offset = new Vector2(offset.x, y);
	}

	private void SetState()
	{
		if (profile == null)
		{
			ChallengeState challengeState = (manager.LastViewedChallengeId >= ChallengeId) ? ChallengeState.Completed : ((manager.CurrentChallenge == null || manager.CurrentChallenge.Id != ChallengeId) ? ChallengeState.NotStarted : ChallengeState.InProgress);
			if (challengeState != State)
			{
				State = challengeState;
				stateMethods[challengeState]();
			}
		}
		else
		{
			ChallengeState challengeState2 = (profile.LastChallenge >= ChallengeId) ? ChallengeState.Completed : ((profile.CurrentChallenge != ChallengeId) ? ChallengeState.NotStarted : ChallengeState.InProgress);
			if (challengeState2 != State)
			{
				State = challengeState2;
				stateMethods[challengeState2]();
			}
		}
	}

	public int CompareTo(SHSMySquadChallengeListItem other)
	{
		return 0;
	}

	protected void AddSFXHandlers(GUIControl control)
	{
		control.MouseOver += PlayMouseOverSFX;
		control.MouseDown += PlayMouseDownSFX;
		control.MouseUp += PlayMouseUpSFX;
	}

	private void PlayMouseOverSFX(GUIControl sender, GUIMouseEvent EventData)
	{
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("large_hover_over"));
	}

	private void PlayMouseDownSFX(GUIControl sender, GUIMouseEvent EventData)
	{
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("large_click_down"));
	}

	private void PlayMouseUpSFX(GUIControl sender, GUIMouseEvent EventData)
	{
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("large_click_up"));
	}

	private void NotStartedState()
	{
		bgd.TextureSource = "mysquadgadget_bundle|upcoming_challenge_module_background";
		highlightBgd.TextureSource = string.Empty;
		item.AnimationPieceManager.Remove(pulsingClip);
		title.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, ColorUtil.FromRGB255(176, 222, 254), ColorUtil.FromRGB255(0, 33, 102), ColorUtil.FromRGB255(0, 19, 59), new Vector2(-2f, 2f), TextAnchor.MiddleCenter);
		title.Text = string.Format(AppShell.Instance.stringTable["#SQ_MYSQUAD_CHALLENGEITEM_TITLE"], ChallengeId);
		challengeName.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 15, ColorUtil.FromRGB255(209, 229, 253), ColorUtil.FromRGB255(0, 49, 129), ColorUtil.FromRGB255(16, 79, 163), new Vector2(-2f, 2f), TextAnchor.UpperLeft);
		challengeName.VerticalKerning = 20;
		challengeDesc.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, ColorUtil.FromRGB255(195, 223, 254), ColorUtil.FromRGB255(3, 33, 81), new Vector2(-1f, 1f), TextAnchor.UpperLeft);
		PositionDescription();
		challengeCompleteImage.IsVisible = false;
		RewardView rewardView = new RewardView(ChallengeId);
		rewardView.Id = "rewardView";
		rewardView.SetPositionAndSize(GUIControl.DockingAlignmentEnum.BottomMiddle, GUIControl.AnchorAlignmentEnum.BottomMiddle, GUIControl.OffsetType.Absolute, new Vector2(0f, 20f), new Vector2(250f, 130f), GUIControl.AutoSizeTypeEnum.Absolute, GUIControl.AutoSizeTypeEnum.Absolute);
		item.Add(rewardView, challengeCompleteImage);
	}

	private void InProgressState()
	{
		bgd.TextureSource = "mysquadgadget_bundle|current_challenge_module_background";
		highlightBgd.TextureSource = "mysquadgadget_bundle|current_challenge_module_highlight";
		AnimatePulsingChallenge(true);
		title.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, ColorUtil.FromRGB255(187, 248, 254), ColorUtil.FromRGB255(0, 72, 104), ColorUtil.FromRGB255(1, 28, 65), new Vector2(-2f, 2f), TextAnchor.MiddleCenter);
		title.Text = string.Format(AppShell.Instance.stringTable["#SQ_MYSQUAD_CHALLENGEITEMCURRENT_TITLE"], ChallengeId);
		challengeName.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 15, ColorUtil.FromRGB255(255, 255, 254), ColorUtil.FromRGB255(0, 70, 128), ColorUtil.FromRGB255(16, 96, 169), new Vector2(-2f, 2f), TextAnchor.UpperLeft);
		challengeName.VerticalKerning = 20;
		challengeDesc.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, ColorUtil.FromRGB255(228, 252, 254), ColorUtil.FromRGB255(6, 73, 122), new Vector2(-1f, 1f), TextAnchor.UpperLeft);
		PositionDescription();
		challengeCompleteImage.IsVisible = false;
		GUISimpleControlWindow gUISimpleControlWindow = new GUISimpleControlWindow();
		gUISimpleControlWindow.Id = "progressView";
		gUISimpleControlWindow.SetPositionAndSize(GUIControl.DockingAlignmentEnum.BottomMiddle, GUIControl.AnchorAlignmentEnum.BottomMiddle, GUIControl.OffsetType.Absolute, new Vector2(0f, -5f), new Vector2(250f, 65f), GUIControl.AutoSizeTypeEnum.Absolute, GUIControl.AutoSizeTypeEnum.Absolute);
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.Id = "progressLabel";
		gUIStrokeTextLabel.SetPositionAndSize(GUIControl.DockingAlignmentEnum.TopMiddle, GUIControl.AnchorAlignmentEnum.TopMiddle, GUIControl.OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(200f, 25f), GUIControl.AutoSizeTypeEnum.Absolute, GUIControl.AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(255, 217, 85), GUILabel.GenColor(0, 82, 112), GUILabel.GenColor(0, 82, 112), new Vector2(0f, 0f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel.Bold = true;
		gUIStrokeTextLabel.Text = "#SQ_CHALLENGE_PROGRESS";
		gUISimpleControlWindow.Add(gUIStrokeTextLabel);
		AppShell.Instance.ChallengeManager.GetChallengeProgressWindow(gUISimpleControlWindow, ChallengeId, false);
		item.Add(gUISimpleControlWindow);
	}

	private void anim_OnFinished()
	{
		throw new NotImplementedException();
	}

	private void CompletedState()
	{
		bgd.TextureSource = "mysquadgadget_bundle|upcoming_challenge_module_background";
		highlightBgd.TextureSource = string.Empty;
		item.AnimationPieceManager.Remove(pulsingClip);
		title.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, ColorUtil.FromRGB255(176, 222, 254), ColorUtil.FromRGB255(0, 33, 102), ColorUtil.FromRGB255(0, 19, 59), new Vector2(-2f, 2f), TextAnchor.MiddleCenter);
		title.Text = string.Format(AppShell.Instance.stringTable["#SQ_MYSQUAD_CHALLENGEITEM_TITLE"], ChallengeId);
		challengeName.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 15, ColorUtil.FromRGB255(209, 229, 253), ColorUtil.FromRGB255(0, 49, 129), ColorUtil.FromRGB255(16, 79, 163), new Vector2(-2f, 2f), TextAnchor.UpperLeft);
		challengeName.VerticalKerning = 20;
		challengeDesc.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, ColorUtil.FromRGB255(195, 223, 254), ColorUtil.FromRGB255(3, 33, 81), new Vector2(-1f, 1f), TextAnchor.UpperLeft);
		PositionDescription();
		challengeCompleteImage.IsVisible = true;
		RewardView rewardView = new RewardView(ChallengeId);
		rewardView.Id = "rewardView";
		rewardView.SetPositionAndSize(GUIControl.DockingAlignmentEnum.BottomMiddle, GUIControl.AnchorAlignmentEnum.BottomMiddle, GUIControl.OffsetType.Absolute, new Vector2(0f, 20f), new Vector2(250f, 130f), GUIControl.AutoSizeTypeEnum.Absolute, GUIControl.AutoSizeTypeEnum.Absolute);
		item.Add(rewardView, challengeCompleteImage);
	}

	private void AnimatePulsingChallenge(bool pulseIn)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		AnimClip animClip = (!pulseIn) ? AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(1f, 0.3f, 2f), highlightBgd) : AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0.3f, 1f, 2f), highlightBgd);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			AnimatePulsingChallenge(!pulseIn);
		};
		item.AnimationPieceManager.SwapOut(ref pulsingClip, animClip);
	}
}
