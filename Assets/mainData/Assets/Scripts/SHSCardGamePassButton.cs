using CardGame;
using System;
using UnityEngine;

public class SHSCardGamePassButton : GUISimpleControlWindow
{
	private GUIButton passButton;

	private GUIButton passButtonLight;

	private GUIButton passTextButton;

	private SHSCardGamePokeTimer pokedTimer;

	private bool firstTimePoked;

	private SHSOkDialogWindow okDialog;

	public PickCardType pickCardType;

	private AnimClip pulseAnim;

	public override bool IsVisible
	{
		get
		{
			return base.IsVisible;
		}
		set
		{
			base.IsVisible = value;
			if (!value && okDialog != null)
			{
				okDialog.Hide();
			}
		}
	}

	public SHSCardGamePassButton()
	{
		okDialog = null;
		SetSize(256f, 256f);
		Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		IsVisible = false;
		passButton = new GUIButton();
		passButton.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, new Vector2(135f, 135f));
		passButton.StyleInfo = new SHSButtonStyleInfo("cardgame_bundle|mshs_pass_button");
		passButton.Traits.HitTestType = HitTestTypeEnum.Rect;
		Add(passButton);
		passButtonLight = new GUIButton();
		passButtonLight.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, new Vector2(135f, 135f));
		passButtonLight.StyleInfo = new SHSButtonStyleInfo("cardgame_bundle|mshs_pass_button_light");
		passButtonLight.Traits.HitTestType = HitTestTypeEnum.Rect;
		passButtonLight.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		Add(passButtonLight);
		passTextButton = new GUIButton();
		passTextButton.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(135f, 135f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		passTextButton.StyleInfo = new SHSButtonStyleInfo("cardgame_bundle|L_mshs_pass_button_text");
		passTextButton.Traits.HitTestType = HitTestTypeEnum.Rect;
		passTextButton.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		passTextButton.Click += delegate
		{
			AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.ClickedPass(pickCardType));
		};
		passTextButton.MouseOver += delegate
		{
			CardGameController.Instance.AudioManager.Play(CardGameAudioManager.SFX.CardHover);
		};
		Add(passTextButton);
		pokedTimer = new SHSCardGamePokeTimer(24, new Vector2(-3f, 5f));
		pokedTimer.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(49f, -3f), new Vector2(36f, 36f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		pokedTimer.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		pokedTimer.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		pokedTimer.IsVisible = false;
		pokedTimer.MouseDown += delegate
		{
		};
		Add(pokedTimer);
		firstTimePoked = true;
	}

	public void ShowRiskIt()
	{
		passButton.StyleInfo = new SHSButtonStyleInfo("cardgame_bundle|L_mshs_riskit_button");
		IsVisible = true;
		passTextButton.Offset = new Vector2(0f, 0f);
		pokedTimer.IsVisible = false;
		if (okDialog != null)
		{
			okDialog.Hide();
		}
	}

	public void ShowPass(PickCardType pickCardType, CardGameEvent.EnablePassButton.PassButtonType passButtonType)
	{
		this.pickCardType = pickCardType;
		if (passButtonType != 0 && passButtonType == CardGameEvent.EnablePassButton.PassButtonType.Done)
		{
			passTextButton.StyleInfo = new SHSButtonStyleInfo("cardgame_bundle|L_mshs_done_button_text");
		}
		else
		{
			passTextButton.StyleInfo = new SHSButtonStyleInfo("cardgame_bundle|L_mshs_pass_button_text");
		}
		StartPulse();
		IsVisible = true;
		passTextButton.Offset = new Vector2(0f, 0f);
		pokedTimer.IsVisible = false;
		if (okDialog != null)
		{
			okDialog.Hide();
		}
	}

	public override void Update()
	{
		base.Update();
	}

	public override void OnShow()
	{
		base.OnShow();
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.PlayerPoked>(OnPlayerPoked);
	}

	public override void OnHide()
	{
		base.OnHide();
		if (okDialog != null)
		{
			okDialog.Hide();
		}
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.PlayerPoked>(OnPlayerPoked);
	}

	private void OnPlayerPoked(CardGameEvent.PlayerPoked evt)
	{
		if (evt.isLocalPlayerPoked)
		{
			if (firstTimePoked)
			{
				okDialog = new SHSOkDialogWindow();
				okDialog.Text = "#CARDGAME_POKE_WARNING";
				GUIManager.Instance.ShowDynamicWindow(okDialog, ModalLevelEnum.None);
				firstTimePoked = false;
			}
			ShowTimer(evt.timer);
		}
	}

	private void ShowTimer(int seconds)
	{
		if (!pokedTimer.IsVisible)
		{
			passButton.StyleInfo = new SHSButtonStyleInfo("cardgame_bundle|mshs_pass_button");
			IsVisible = true;
			StartPulse();
			passTextButton.Offset = new Vector2(-15f, 0f);
			pokedTimer.IsVisible = true;
			pokedTimer.StartingTime = seconds;
			pokedTimer.OnTimerEvent += OnTimerComplete;
			pokedTimer.StartTimer();
		}
	}

	private void OnTimerComplete(SHSTimerEx.TimerEventType type, int data)
	{
		if (type == SHSTimerEx.TimerEventType.Completed)
		{
			pokedTimer.IsVisible = false;
			AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.PokedTimerCompleted());
			if (okDialog != null)
			{
				okDialog.Hide();
			}
		}
	}

	public void StartPulse()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		float time = 0.75f * (1f - passButtonLight.Alpha);
		float time2 = 0.75f;
		AnimClip animClip = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(passButtonLight.Alpha, 1f, time) | AnimClipBuilder.Path.Linear(1f, 0f, time2), passButtonLight);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			StartPulse();
		};
		base.AnimationPieceManager.SwapOut(ref pulseAnim, animClip);
	}

	public void EndPulse()
	{
		float time = passButtonLight.Alpha * 0.25f;
		AnimClip newPiece = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(passButtonLight.Alpha, 0f, time), passButtonLight);
		base.AnimationPieceManager.SwapOut(ref pulseAnim, newPiece);
	}
}
