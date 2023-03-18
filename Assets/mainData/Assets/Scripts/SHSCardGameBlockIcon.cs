using System;
using UnityEngine;

public class SHSCardGameBlockIcon : GUISimpleControlWindow
{
	public int playerIndex = -1;

	private Vector2 fullImageSize = new Vector2(180f, 188f);

	private Vector2 halfImageSize = new Vector2(90f, 94f);

	private GUIImage background;

	private GUIImage[] glowRings;

	public SHSCardGameBlockIcon()
	{
		SetSize(256f, 256f);
		id = "Block Indicator";
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		glowRings = new GUIImage[3];
		for (int i = 0; i < 3; i++)
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(Vector2.zero, new Vector2(5f, 10f));
			gUIImage.TextureSource = "cardgame_bundle|mshs_blocked_glowring";
			gUIImage.AnimationAlpha = 0f;
			Add(gUIImage);
			glowRings[i] = gUIImage;
		}
		background = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(100f, 100f), new Vector2(0f, 0f));
		background.TextureSource = "cardgame_bundle|L_mshs_blocked_indicator";
		background.IsVisible = false;
		background.Alpha = 0f;
		background.SetSize(halfImageSize);
		Add(background);
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.AttackResult>(OnStatusMessage);
	}

	public void OnStatusMessage(CardGameEvent.AttackResult evt)
	{
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		if (playerIndex != evt.playerIndex)
		{
			return;
		}
		if (evt.type == CardGameEvent.AttackResultType.LuckyBlock)
		{
			background.TextureSource = "cardgame_bundle|L_mshs_luckyblock_indicator";
			if (playerIndex == 0)
			{
				SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(130f, 380f));
			}
			else
			{
				SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-118f, 148f));
			}
		}
		else
		{
			if (evt.type != CardGameEvent.AttackResultType.HandBlock)
			{
				return;
			}
			background.TextureSource = "cardgame_bundle|L_mshs_blocked_indicator";
			SetPosition(QuickSizingHint.Centered);
		}
		background.Alpha = 0f;
		background.IsVisible = true;
		background.SetSize(halfImageSize);
		Show();
		float time = 0.3f;
		float holdTime = 2f;
		float popOutTime = 0.3f;
		AnimClip animClip = SHSAnimations.Generic.FadeIn(background, time) ^ SHSAnimations.Generic.ChangeSize(background, fullImageSize, halfImageSize, time, 0f);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			AnimClip animClip2 = SHSAnimations.Generic.Wait(holdTime);
			animClip2.OnFinished += (Action)(object)(Action)delegate
			{
				//IL_0055: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Expected O, but got Unknown
				AnimClip animClip3 = SHSAnimations.Generic.ChangeSizeDirect(background, halfImageSize, fullImageSize, popOutTime, 0f) ^ SHSAnimations.Generic.FadeOut(background, popOutTime);
				animClip3.OnFinished += (Action)(object)(Action)delegate
				{
					Hide();
				};
				base.AnimationPieceManager.Add(animClip3);
			};
			base.AnimationPieceManager.Add(animClip2);
		};
		base.AnimationPieceManager.Add(animClip);
		AnimClip toAdd = GenerateRingPulse(glowRings[0], 0f) ^ GenerateRingPulse(glowRings[1], 0.3f) ^ GenerateRingPulse(glowRings[2], 0.6f);
		base.AnimationPieceManager.Add(toAdd);
	}

	private AnimClip GenerateRingPulse(GUIImage ringImage, float delay)
	{
		return AnimClipBuilder.Absolute.Nothing(AnimClipBuilder.Path.Linear(0f, 0f, delay), ringImage) | (AnimClipBuilder.Absolute.SizeXY(AnimClipBuilder.Path.Linear(100f, 250f, 1.3f), ringImage) ^ (AnimClipBuilder.Absolute.AnimationAlpha(AnimClipBuilder.Path.Linear(0f, 1f, 1f), ringImage) | AnimClipBuilder.Absolute.AnimationAlpha(AnimClipBuilder.Path.Linear(1f, 0f, 0.3f), ringImage)));
	}
}
