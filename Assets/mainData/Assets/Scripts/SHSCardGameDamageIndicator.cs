using System;
using UnityEngine;

public class SHSCardGameDamageIndicator : GUISimpleControlWindow
{
	public int playerIndex = -1;

	private Vector2 fullImageSize = new Vector2(184f, 184f);

	private Vector2 halfImageSize = new Vector2(92f, 92f);

	private GUIImage background;

	private GUIDropShadowTextLabel damageCount;

	private GUIDropShadowTextLabel damageText;

	public SHSCardGameDamageIndicator()
	{
		id = "Damage Indicator";
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		SetSize(fullImageSize);
		Rotation = -10f;
		background = GUIControl.CreateControlFrameCentered<GUIImage>(fullImageSize, new Vector2(0f, 0f));
		background.TextureSource = "cardgame_bundle|mshs_damage_indicator_bg";
		Add(background);
		damageCount = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(100f, 70f), new Vector2(-5f, -7f));
		damageCount.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 70, GUILabel.GenColor(235, 54, 34), Color.white, new Vector2(-3f, 3f), TextAnchor.MiddleCenter);
		damageCount.Text = "40";
		Add(damageCount);
		damageText = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(100f, 70f), new Vector2(1f, 17f));
		damageText.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 20, GUILabel.GenColor(235, 54, 34), Color.white, new Vector2(-3f, 3f), TextAnchor.MiddleCenter);
		damageText.Text = "#CARDGAME_DAMAGE_INDICATOR";
		Add(damageText);
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.AttackResult>(OnStatusMessage);
		Hide();
	}

	private void SpinBackground()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		float finish = 360f;
		float time = 4f;
		AnimClip animClip = AnimClipBuilder.Absolute.Rotation(AnimClipBuilder.Path.Linear(0f, finish, time), background);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			SpinBackground();
		};
		base.AnimationPieceManager.Add(animClip);
	}

	public override void OnShow()
	{
		base.OnShow();
		SpinBackground();
		CardGameController.Instance.AudioManager.Play(CardGameAudioManager.SFX.DamageIndicator);
	}

	public void OnStatusMessage(CardGameEvent.AttackResult evt)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		if (playerIndex == evt.playerIndex && evt.type == CardGameEvent.AttackResultType.Damage)
		{
			int damageDone = evt.damageDone;
			damageCount.Text = damageDone.ToString();
			damageText.Alpha = 0f;
			damageCount.Alpha = 0f;
			float num = 0.3f;
			float holdTime = 2f;
			float popOutTime = 0.3f;
			Show();
			AnimClip animClip = SHSAnimations.Generic.FadeIn(background, num) ^ SHSAnimations.Generic.ChangeSize(background, fullImageSize, halfImageSize, num, 0f);
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
			animClip = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Constant(0f, num * 0.75f) | AnimClipBuilder.Path.Linear(0f, 1f, num * 0.75f) | AnimClipBuilder.Path.Constant(1f, holdTime) | AnimClipBuilder.Path.Linear(1f, 0f, popOutTime), damageCount, damageText);
			base.AnimationPieceManager.Add(animClip);
		}
	}
}
