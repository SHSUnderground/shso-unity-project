using System;
using System.Collections.Generic;
using UnityEngine;

public class SpinningSquadBackgroundControl : GUISimpleControlWindow
{
	protected List<GUIImage> spinningParts;

	protected AnimClip[] spinningAnims;

	protected static float[] slowAnimRange = new float[2]
	{
		20f,
		30f
	};

	protected static float[] fastAnimRange = new float[2]
	{
		7f,
		7.5f
	};

	public SpinningSquadBackgroundControl()
	{
		spinningParts = new List<GUIImage>();
		for (int num = 6; num > 0; num--)
		{
			GUIImage gUIImage = new GUIImage();
			gUIImage.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(521f, 521f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIImage.TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_p1_circle_spin" + num;
			gUIImage.Id = "SpinningPart_" + num;
			Add(gUIImage);
			spinningParts.Add(gUIImage);
		}
		spinningAnims = new AnimClip[spinningParts.Count];
	}

	public void StartSpinningAnimations()
	{
		bool flag = true;
		for (int i = 0; i < spinningParts.Count; i++)
		{
			float num = 0f;
			if (i <= 3)
			{
				num = UnityEngine.Random.Range(slowAnimRange[0], slowAnimRange[1]);
				StartSpinningAnimation(spinningParts[i], num, flag, spinningAnims[i], false);
			}
			else
			{
				num = UnityEngine.Random.Range(fastAnimRange[0], fastAnimRange[1]);
				StartSpinningAnimation(spinningParts[i], num, flag, spinningAnims[i], true);
			}
			flag = !flag;
		}
	}

	protected void StartSpinningAnimation(GUIImage img, float time, bool clockwise, AnimClip clip, bool fade)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		AnimClip spinningAnimation = GetSpinningAnimation(img, time, clockwise, fade);
		spinningAnimation.OnFinished += (Action)(object)(Action)delegate
		{
			StartSpinningAnimation(img, time, clockwise, clip, fade);
		};
		base.AnimationPieceManager.SwapOut(ref clip, spinningAnimation);
	}

	protected static AnimClip GetSpinningAnimation(GUIImage img, float time, bool clockwise, bool fade)
	{
		AnimClip result = AnimClipBuilder.Absolute.Rotation((!clockwise) ? AnimClipBuilder.Path.Linear(359f, 0f, time) : AnimClipBuilder.Path.Linear(0f, 359f, time), img);
		if (fade)
		{
			AnimClip animClip = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(1f, 0.3f, time * 0.5f), img) | AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0.3f, 1f, time * 0.5f), img);
			result ^= animClip;
		}
		return result;
	}
}
