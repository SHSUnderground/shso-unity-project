using System;
using System.Collections.Generic;
using UnityEngine;

public class RotatingHeroSilhouetteControl : GUISimpleControlWindow
{
	protected List<GUIImage> heroImages;

	protected GUIImage previousHeroImage;

	protected GUIImage currentHeroImage;

	protected int fadeOutIndex;

	protected int fadeInIndex;

	protected AnimClip fadeOutClip;

	protected AnimClip fadeInClip;

	public float fadeTime;

	public float constantTime;

	public RotatingHeroSilhouetteControl()
	{
		heroImages = new List<GUIImage>();
		for (int i = 0; i < 12; i++)
		{
			GUIImage gUIImage = new GUIImage();
			gUIImage.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(-17f, -78f), new Vector2(350f, 350f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			gUIImage.TextureSource = "mysquadgadget_bundle|heroes" + string.Format("{0:0000}", i);
			gUIImage.Id = "heroes_" + i.ToString();
			Add(gUIImage);
			heroImages.Add(gUIImage);
		}
	}

	public void StartAnimations()
	{
		fadeOutIndex = 0;
		fadeInIndex = 1;
		fadeTime = 1.75f;
		constantTime = 1.25f;
		FadeOut();
		FadeIn();
	}

	public override void OnShow()
	{
		base.OnShow();
		for (int i = 0; i < heroImages.Count; i++)
		{
			heroImages[i].Alpha = 0f;
		}
		heroImages[0].Alpha = 1f;
	}

	protected void FadeOut()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		AnimClip fadeOutAnimation = GetFadeOutAnimation(heroImages[fadeOutIndex], constantTime, fadeTime);
		fadeOutAnimation.OnFinished += (Action)(object)(Action)delegate
		{
			fadeOutIndex++;
			if (fadeOutIndex >= heroImages.Count)
			{
				fadeOutIndex = 0;
			}
			FadeOut();
		};
		base.AnimationPieceManager.SwapOut(ref fadeOutClip, fadeOutAnimation);
	}

	protected void FadeIn()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		AnimClip fadeInAnimation = GetFadeInAnimation(heroImages[fadeInIndex], constantTime, fadeTime);
		fadeInAnimation.OnFinished += (Action)(object)(Action)delegate
		{
			fadeInIndex++;
			if (fadeInIndex >= heroImages.Count)
			{
				fadeInIndex = 0;
			}
			FadeIn();
		};
		base.AnimationPieceManager.SwapOut(ref fadeInClip, fadeInAnimation);
	}

	protected static AnimClip GetFadeOutAnimation(GUIImage img, float ConstantTime, float FadeTime)
	{
		return AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Constant(1f, ConstantTime) | AnimClipBuilder.Path.Linear(1f, 0f, FadeTime), img);
	}

	protected static AnimClip GetFadeInAnimation(GUIImage img, float ConstantTime, float FadeTime)
	{
		return AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Constant(0f, ConstantTime) | AnimClipBuilder.Path.Linear(0f, 1f, FadeTime), img);
	}
}
