using System;
using UnityEngine;

public class SHSCardGamePokeButton : GUISimpleControlWindow
{
	private GUIButton pokeButton;

	private GUIButton pokeButtonLight;

	private GUIStrokeTextLabel pokeLabel;

	private bool firstPoke = true;

	private SHSCardGamePokeTimer pokeTimer;

	private SHSOkDialogWindow okDialog;

	private AnimClip pulseAnim;

	public bool PokingIsAllowed
	{
		set
		{
			pokeButton.IsEnabled = value;
			pokeButtonLight.IsEnabled = value;
			if (value)
			{
				pokeButtonLight.IsVisible = true;
				pokeLabel.IsVisible = true;
				StartPulse();
				if (firstPoke)
				{
					firstPoke = false;
					okDialog = new SHSOkDialogWindow();
					okDialog.Text = "#CARDGAME_POKE_MESSAGE";
					GUIManager.Instance.ShowDynamicWindow(okDialog, ModalLevelEnum.Default);
				}
			}
			else
			{
				pokeLabel.Offset = new Vector2(77f, 68f);
				pokeTimer.IsVisible = false;
				pokeButtonLight.IsVisible = false;
				pokeLabel.IsVisible = false;
				if (okDialog != null)
				{
					okDialog.Hide();
				}
				EndPulse();
			}
		}
	}

	public SHSCardGamePokeButton()
	{
		okDialog = null;
		SetSize(128f, 128f);
		Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		IsVisible = true;
		pokeButton = new GUIButton();
		pokeButton.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, new Vector2(112f, 112f));
		pokeButton.StyleInfo = new SHSButtonStyleInfo("cardgame_bundle|mshs_pass_button");
		pokeButton.Traits.HitTestType = HitTestTypeEnum.Rect;
		pokeButton.IsEnabled = false;
		pokeButton.Traits.RespectDisabledAlphaTrait = ControlTraits.RespectDisabledAlphaTraitEnum.DisrespectDisabledAlpha;
		pokeButton.Click += delegate
		{
			AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.ClickedPoke());
			PokingIsAllowed = false;
		};
		Add(pokeButton);
		pokeButtonLight = new GUIButton();
		pokeButtonLight.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, new Vector2(112f, 112f));
		pokeButtonLight.StyleInfo = new SHSButtonStyleInfo("cardgame_bundle|mshs_pass_button_light");
		pokeButtonLight.Traits.HitTestType = HitTestTypeEnum.Rect;
		pokeButtonLight.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		pokeButtonLight.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		pokeButtonLight.IsEnabled = false;
		pokeButtonLight.IsVisible = false;
		Add(pokeButtonLight);
		pokeLabel = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(200f, 35f), new Vector2(77f, 68f));
		pokeLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 22, GUILabel.GenColor(253, 255, 205), GUILabel.GenColor(74, 99, 31), GUILabel.GenColor(96, 121, 0), new Vector2(1f, 4f), TextAnchor.MiddleLeft);
		pokeLabel.Text = "#CARDGAME_POKE";
		pokeLabel.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Bubble;
		pokeLabel.IsVisible = false;
		Add(pokeLabel);
		pokeTimer = new SHSCardGamePokeTimer(20, new Vector2(-2f, 2f));
		pokeTimer.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, new Vector2(40f, 0f), new Vector2(29f, 29f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		pokeTimer.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		pokeTimer.IsVisible = false;
		Add(pokeTimer);
	}

	public override void OnShow()
	{
		base.OnShow();
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.PlayerPoked>(OnPlayerPoked);
	}

	public override void OnHide()
	{
		base.OnHide();
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.PlayerPoked>(OnPlayerPoked);
	}

	public override void Update()
	{
		base.Update();
	}

	public void StartPulse()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		float time = 0.75f * (1f - pokeButtonLight.Alpha);
		float time2 = 0.75f;
		AnimClip animClip = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(pokeButtonLight.Alpha, 1f, time) | AnimClipBuilder.Path.Linear(1f, 0f, time2), pokeButtonLight);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			StartPulse();
		};
		base.AnimationPieceManager.SwapOut(ref pulseAnim, animClip);
	}

	public void EndPulse()
	{
		float time = pokeButtonLight.Alpha * 0.25f;
		AnimClip newPiece = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(pokeButtonLight.Alpha, 0f, time), pokeButtonLight);
		base.AnimationPieceManager.SwapOut(ref pulseAnim, newPiece);
	}

	private void OnPlayerPoked(CardGameEvent.PlayerPoked evt)
	{
		if (!evt.isLocalPlayerPoked)
		{
			ShowTimer(evt.timer);
		}
	}

	private void ShowTimer(int seconds)
	{
		if (!pokeTimer.IsVisible)
		{
			pokeLabel.Offset = new Vector2(65f, 68f);
			pokeLabel.IsVisible = true;
			pokeTimer.IsVisible = true;
			pokeTimer.StartingTime = seconds;
			pokeTimer.OnTimerEvent += OnTimerComplete;
			pokeTimer.StartTimer();
		}
	}

	private void OnTimerComplete(SHSTimerEx.TimerEventType type, int data)
	{
		if (type == SHSTimerEx.TimerEventType.Completed && pokeTimer.IsVisible)
		{
			if (okDialog != null)
			{
				okDialog.Hide();
			}
			pokeLabel.Offset = new Vector2(77f, 68f);
			pokeTimer.IsVisible = false;
			AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.PokedTimerCompleted());
		}
	}
}
