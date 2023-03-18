using System;
using UnityEngine;

public class SHSChallengeUpWindow : GUIDynamicWindow
{
	protected const float START_SIZE = 10f;

	protected const float END_SIZE = 278f;

	protected const float WINDOW_SIZE = 300f;

	protected const float Y_OFFSET = 256f;

	protected const float X_OFFSET = 128f;

	protected GUIImage challengeUpBGImage;

	protected GUIImage challengeCompleteTextImage;

	protected GUIImage heroImage;

	protected GUIButton okButton;

	protected bool okButtonIsVisible;

	protected float spinSpeed = 1f;

	protected bool newChallenge;

	public SHSChallengeUpWindow(bool NewChallenge)
	{
		newChallenge = NewChallenge;
	}

	public override bool InitializeResources(bool reload)
	{
		Vector2 offset = Vector2.zero;
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		if (localPlayer != null)
		{
			offset = GUICommon.WorldToUIPoint(Camera.main, localPlayer.transform.position);
		}
		challengeUpBGImage = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(10f, 10f), new Vector2(0f, 0f));
		challengeUpBGImage.Anchor = AnchorAlignmentEnum.Middle;
		challengeUpBGImage.Docking = DockingAlignmentEnum.Middle;
		challengeUpBGImage.TextureSource = "notification_bundle|challengesystem_rotatingstar";
		Add(challengeUpBGImage);
		heroImage = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(218f, 161f), new Vector2(35f, 15f));
		heroImage.TextureSource = "notification_bundle|challengesystem_challengeicon";
		Add(heroImage);
		heroImage.AnimationAlpha = 0f;
		if (newChallenge)
		{
			challengeCompleteTextImage = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(272f, 142f), new Vector2(40f, 100f));
			challengeCompleteTextImage.TextureSource = "notification_bundle|L_challengesystem_playachallenge";
		}
		else
		{
			challengeCompleteTextImage = GUIControl.CreateControlAbsolute<GUIImage>(new Vector2(266f, 150f), new Vector2(40f, 100f));
			challengeCompleteTextImage.TextureSource = "notification_bundle|L_challengesystem_challengecomplete";
		}
		Add(challengeCompleteTextImage);
		challengeCompleteTextImage.IsVisible = false;
		SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, offset, new Vector2(300f, 300f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		AnimClip toAdd = AnimClipBuilder.Absolute.SizeXY(AnimClipBuilder.Path.Linear(10f, 278f, 1f), challengeUpBGImage) ^ AnimClipBuilder.Absolute.OffsetY(AnimClipBuilder.Path.Linear(offset.y, 190f, 1f), this) ^ AnimClipBuilder.Absolute.OffsetX(AnimClipBuilder.Path.Linear(offset.x, 350f, 1f), this);
		base.AnimationPieceManager.Add(toAdd);
		SpinLevelUp();
		okButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(38f, 101f));
		okButton.Click += delegate
		{
			onClick();
		};
		okButton.HitTestSize = new Vector2(0.5f, 0.5f);
		okButton.HitTestType = HitTestTypeEnum.Circular;
		okButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|L_mshs_button_ok");
		Add(okButton);
		okButton.Alpha = 0f;
		okButtonIsVisible = false;
		return base.InitializeResources(reload);
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
	}

	public override void Show(ModalLevelEnum modal)
	{
		AppShell.Instance.EventMgr.AddListener<GUIAutoCloseWindowMessage>(OnAutoCloseWindow);
		base.Show(modal);
	}

	public override void Hide()
	{
		AppShell.Instance.EventMgr.RemoveListener<GUIAutoCloseWindowMessage>(OnAutoCloseWindow);
		base.Hide();
	}

	protected void onClick()
	{
		Hide();
		AppShell.Instance.EventMgr.Fire(this, new ChallengeCelebrationHideMessage());
	}

	protected void SpinLevelUp()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		if (challengeUpBGImage != null)
		{
			AnimClip animClip = AnimClipBuilder.Absolute.Rotation(AnimClipBuilder.Path.Linear(0f, 360f, spinSpeed), challengeUpBGImage);
			animClip.OnFinished += (Action)(object)(Action)delegate
			{
				Vector2 size = challengeUpBGImage.Size;
				if (size.x >= 278f && !okButtonIsVisible)
				{
					AnimClip toAdd = SHSAnimations.Generic.AnimationBounceTransitionIn(new Vector2(218f, 161f), 0f, heroImage);
					base.AnimationPieceManager.Add(toAdd);
					AnimClip toAdd2 = SHSAnimations.Generic.FadeIn(okButton, 1f);
					base.AnimationPieceManager.Add(toAdd2);
					spinSpeed = 3f;
					ShowLevelUpTextImage();
					okButtonIsVisible = true;
				}
				SpinLevelUp();
			};
			base.AnimationPieceManager.Add(animClip);
		}
	}

	protected void ShowLevelUpTextImage()
	{
		if (challengeCompleteTextImage != null)
		{
			challengeCompleteTextImage.Show();
		}
	}

	protected void OnLevelUpWindowHide(LeveledUpAwardHiddenMessage e)
	{
		Hide();
	}

	protected void OnAutoCloseWindow(GUIAutoCloseWindowMessage e)
	{
		if (Id == e.Target)
		{
			if (e.AbortFlow)
			{
				Hide();
				AppShell.Instance.EventMgr.Fire(this, new ChallengeCelebrationHideMessage(false));
			}
			else
			{
				onClick();
			}
		}
	}
}
