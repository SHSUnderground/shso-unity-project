using System;
using UnityEngine;

public class SHSCardGamePowerIndicator : GUISimpleControlWindow
{
	private static readonly Vector2 IMAGE_SIZE = new Vector2(160f, 160f);

	private GUIImage base_c;

	private GUIImage base_b;

	private GUIImage base_b_clone;

	private GUIImage base_a;

	private GUIImage[] base_glow;

	private GUIImage power_number;

	private AnimClip currentAnim;

	public SHSCardGamePowerIndicator()
	{
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, false);
		Alpha = 0f;
		SetSize(IMAGE_SIZE * 1.2f);
		SetPosition(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, OffsetType.Absolute, new Vector2(0f, -80f));
		base_c = GUIControl.CreateControlFrameCentered<GUIImage>(IMAGE_SIZE, Vector2.zero);
		base_c.TextureSource = "cardgame_bundle|power_indicator_base_c";
		Add(base_c);
		base_b = GUIControl.CreateControlFrameCentered<GUIImage>(IMAGE_SIZE, Vector2.zero);
		base_b.TextureSource = "cardgame_bundle|power_indicator_base_b";
		base_b.AnimationAlpha = 0.1f;
		base_b.IsVisible = false;
		Add(base_b);
		base_b_clone = GUIControl.CreateControlFrameCentered<GUIImage>(IMAGE_SIZE, Vector2.zero);
		base_b_clone.TextureSource = "cardgame_bundle|power_indicator_base_b";
		base_b_clone.IsVisible = false;
		Add(base_b_clone);
		base_a = GUIControl.CreateControlFrameCentered<GUIImage>(IMAGE_SIZE, Vector2.zero);
		base_a.TextureSource = "cardgame_bundle|L_power_indicator_base_a";
		Add(base_a);
		base_glow = new GUIImage[3];
		for (int i = 0; i < 3; i++)
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(Vector2.zero, Vector2.zero);
			gUIImage.TextureSource = "cardgame_bundle|power_indicator_base_glow";
			gUIImage.AnimationAlpha = 0f;
			Add(gUIImage);
			base_glow[i] = gUIImage;
		}
		power_number = GUIControl.CreateControlFrameCentered<GUIImage>(IMAGE_SIZE, Vector2.zero);
		power_number.TextureSource = "cardgame_bundle|power_indicator_1";
		Add(power_number);
	}

	private AnimClip GenerateIdleAnimation()
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		AnimClip animClip = AnimClipBuilder.Absolute.Rotation(AnimClipBuilder.Path.Linear(base_b.Rotation, base_b.Rotation + 180f, 4f), base_b) ^ (AnimClipBuilder.Absolute.AnimationAlpha(AnimClipBuilder.Path.Linear(0.05f, 0.09f, 2f), base_b) | AnimClipBuilder.Absolute.AnimationAlpha(AnimClipBuilder.Path.Linear(0.09f, 0.05f, 2f), base_b));
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			base.AnimationPieceManager.SwapOut(ref currentAnim, GenerateIdleAnimation());
		};
		return animClip;
	}

	private AnimClip GenerateRingPulse(GUIImage ringImage, float delay)
	{
		AnimClip pieceOne = AnimClipBuilder.Absolute.Nothing(AnimClipBuilder.Path.Linear(0f, 0f, delay), ringImage);
		Vector2 iMAGE_SIZE = IMAGE_SIZE;
		return pieceOne | (AnimClipBuilder.Absolute.SizeXY(AnimClipBuilder.Path.Linear(0f, iMAGE_SIZE.x, 1f), ringImage) ^ AnimClipBuilder.Absolute.AnimationAlpha(AnimClipBuilder.Path.Linear(0f, 1f, 1f), ringImage)) | AnimClipBuilder.Absolute.AnimationAlpha(AnimClipBuilder.Path.Linear(1f, 0f, 0.3f), ringImage);
	}

	private void GenerateSpinAnimations()
	{
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Expected O, but got Unknown
		float animationAlpha = base_b.AnimationAlpha;
		AnimClip pieceTwo = GenerateRingPulse(base_glow[0], 0f) ^ GenerateRingPulse(base_glow[1], 0.3f) ^ GenerateRingPulse(base_glow[2], 0.6f);
		AnimClip pieceOne = (AnimClipBuilder.Absolute.AnimationAlpha(AnimClipBuilder.Path.Linear(animationAlpha, 1f, 0.5f), base_b) ^ AnimClipBuilder.Absolute.AnimationAlpha(AnimClipBuilder.Path.Linear(0f, 0.85f, 0.5f), base_b_clone)) | AnimClipBuilder.Absolute.Nothing(AnimClipBuilder.Path.Linear(0f, 0f, 0.5f), base_b) | (AnimClipBuilder.Absolute.AnimationAlpha(AnimClipBuilder.Path.Linear(1f, 0.05f, 1f), base_b) ^ AnimClipBuilder.Absolute.AnimationAlpha(AnimClipBuilder.Path.Linear(0.85f, 0f, 1f), base_b_clone));
		AnimClip pieceTwo2 = (AnimClipBuilder.Absolute.Rotation(AnimClipBuilder.Path.Linear(base_b.Rotation, base_b.Rotation + 180f, 1.2f), base_b) ^ AnimClipBuilder.Absolute.Rotation(AnimClipBuilder.Path.Linear(0f, -45f, 1.2f), base_b_clone)) | AnimClipBuilder.Absolute.Rotation(AnimClipBuilder.Path.Linear(base_b.Rotation, base_b.Rotation + 90f, 1f), base_b);
		AnimClip animClip = pieceOne ^ pieceTwo2 ^ pieceTwo;
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			base.AnimationPieceManager.SwapOut(ref currentAnim, GenerateIdleAnimation());
		};
		base.AnimationPieceManager.SwapOut(ref currentAnim, animClip);
	}

	public override void OnHide()
	{
		if (GUIManager.Instance.CurrentState == GUIManager.ModalStateEnum.Transition || ((CardGameController)GameController.GetController()).GameCompleted)
		{
			AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.SetPowerLevel>(OnSetPowerLevel);
			AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.ShowCardGameHud>(OnShowPowerIndicator);
		}
		base.OnHide();
	}

	public override void OnAdded(IGUIContainer addedTo)
	{
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.SetPowerLevel>(OnSetPowerLevel);
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.ShowCardGameHud>(OnShowPowerIndicator);
		base.OnAdded(addedTo);
	}

	private void OnSetPowerLevel(CardGameEvent.SetPowerLevel evt)
	{
		CspUtils.DebugLog("ON SET POWER LEVEL CALLED " + Time.frameCount);
		int num = (evt.newPower >= 20) ? 20 : evt.newPower;
		if (num < 1)
		{
			num = 1;
		}
		power_number.TextureSource = "cardgame_bundle|power_indicator_" + num.ToString();
		GenerateSpinAnimations();
		if (evt.newPower > evt.oldPower)
		{
			CardGameController.Instance.AudioManager.Play(CardGameAudioManager.Announcer.PowerUp);
		}
	}

	private void OnShowPowerIndicator(CardGameEvent.ShowCardGameHud evt)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		AnimPath path = AnimPath.Linear(0f, 1f, 1.5f);
		AnimClip animClip = new AnimClipFunction(path, delegate(float t)
		{
			Alpha = t;
		});
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			base_b.IsVisible = true;
			base_b_clone.IsVisible = true;
			base_b_clone.AnimationAlpha = 0f;
			base_b.AnimationAlpha = 0.05f;
			currentAnim = GenerateIdleAnimation();
			base.AnimationPieceManager.Add(currentAnim);
		};
		base.AnimationPieceManager.Add(animClip);
	}
}
