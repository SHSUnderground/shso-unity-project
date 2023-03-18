using System;
using UnityEngine;

public class SHSCardGameVersusIcon : GUISimpleControlWindow
{
	public int playerIndex = -1;

	private Vector2 fullImageSize = new Vector2(300f, 300f);

	private Vector2 halfImageSize = new Vector2(90f, 94f);

	private GUIImage background;

	public SHSCardGameVersusIcon()
	{
		SetSize(fullImageSize);
		id = "Versus Icon";
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		background = GUIControl.CreateControlFrameCentered<GUIImage>(fullImageSize, new Vector2(0f, 0f));
		background.TextureSource = "cardgame_bundle|L_mshs_versus";
		background.IsVisible = false;
		background.Alpha = 0f;
		background.SetSize(halfImageSize);
		Add(background);
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.ShowVersusIcon>(OnShowVersusIcon);
	}

	public void OnShowVersusIcon(CardGameEvent.ShowVersusIcon evt)
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		background.Alpha = 0f;
		background.IsVisible = true;
		background.SetSize(halfImageSize);
		Show();
		float time = 0.3f;
		float holdTime = 1.4f;
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
	}
}
