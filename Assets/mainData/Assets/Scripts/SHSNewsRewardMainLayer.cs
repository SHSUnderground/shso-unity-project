using System;
using UnityEngine;

public class SHSNewsRewardMainLayer : SHSNewsRewardDayLayer
{
	public static readonly Color RewardLabelDayColor = GUILabel.GenColor(246, 255, 157);

	public static readonly Color RewardLabelCollectColor = GUILabel.GenColor(255, 255, 255);

	private static readonly Vector2 RewardDayImageSize = new Vector2(60f, 46f);

	private static readonly Vector2 RewardDayImageMaxSize = new Vector2(80f, 61f);

	private static readonly int RewardDayIndex = 1;

	private static readonly int RewardIconIndex = 0;

	private GUILabel _rewardTextLabel;

	private string _rewardText;

	public string RewardLabel
	{
		get
		{
			return _rewardText;
		}
	}

	public SHSNewsRewardMainLayer(string rewardDayTexture, string rewardCategoryTexture, string rewardTextLabel, ref Vector2 rewardCategorySize, ref Vector2 rewardCategoryOffset)
	{
		GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(rewardCategorySize, rewardCategoryOffset);
		gUIImage.TextureSource = rewardCategoryTexture;
		layerControls.Add(gUIImage);
		gUIImage = GUIControl.CreateControlAbsolute<GUIImage>(RewardDayImageSize, new Vector2(-4f, -4f));
		gUIImage.TextureSource = rewardDayTexture;
		layerControls.Add(gUIImage);
		_rewardTextLabel = GUIControl.CreateControlBottomFrame<GUILabel>(new Vector2(96f, 15f), new Vector2(0f, -13f));
		_rewardTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, RewardLabelDayColor, TextAnchor.MiddleCenter);
		_rewardTextLabel.Text = rewardTextLabel;
		layerControls.Add(_rewardTextLabel);
		_rewardText = rewardTextLabel;
	}

	public void SetRewardTextColor(Color color)
	{
		_rewardTextLabel.TextColor = color;
	}

	public override void OnMouseOver(AnimClipManager clipManager)
	{
		base.OnMouseOver(clipManager);
		clipManager.SwapOut(ref layerAnimation, AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(SHSNewsRewardDayWindow.CollectedAlpha, SHSNewsRewardDayWindow.NormalAlpha, 0.2f), layerControls.ToArray()));
	}

	public override void OnMouseOut(AnimClipManager clipManager)
	{
		base.OnMouseOut(clipManager);
		clipManager.SwapOut(ref layerAnimation, AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(SHSNewsRewardDayWindow.NormalAlpha, SHSNewsRewardDayWindow.CollectedAlpha, 0.2f), layerControls.ToArray()));
	}

	public override void OnCollectNow(AnimClipManager clipManager)
	{
		base.OnCollectNow(clipManager);
		if (layerControls.Count > RewardDayIndex)
		{
			GUIControl gUIControl = layerControls[RewardDayIndex];
			gUIControl.Size = RewardDayImageMaxSize;
			layerAnimation = (SHSAnimations.Generic.Wait(0.25f) | SHSAnimations.Generic.ChangeSizeDirect(gUIControl, RewardDayImageSize, RewardDayImageMaxSize, 0.25f, 0.25f));
		}
		if (layerControls.Count > RewardIconIndex)
		{
			GUIControl gUIControl2 = layerControls[RewardIconIndex];
			Vector2 size = gUIControl2.Size;
			AnimPath path = SHSAnimations.GenericPaths.BounceTransitionInX(size.x, 0f);
			Vector2 size2 = gUIControl2.Size;
			AnimPath path2 = SHSAnimations.GenericPaths.BounceTransitionInY(size2.y, 0f);
			layerAnimation |= (AnimClipBuilder.Absolute.SizeX(path, gUIControl2) ^ AnimClipBuilder.Absolute.SizeY(path2, gUIControl2));
			gUIControl2.Size = Vector2.zero;
		}
		clipManager.Add(layerAnimation);
	}

	public override void OnCollectedToday(AnimClipManager clipManager)
	{
		base.OnCollectedToday(clipManager);
		CreateCollectedTodayAnimation(clipManager, layerAnimation, false);
	}

	private void CreateCollectedTodayAnimation(AnimClipManager clipManager, AnimClip clip, bool collectedText)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		_rewardTextLabel.Text = ((!collectedText) ? _rewardText : AppShell.Instance.stringTable[SHSNewsRewardTableWindow.CollectedLabelKey]);
		clip = SHSAnimations.Generic.Wait(SHSNewsRewardDayLayer.RewardTextChangeTime);
		clip.OnFinished += (Action)(object)(Action)delegate
		{
			CreateCollectedTodayAnimation(clipManager, clip, !collectedText);
		};
		clipManager.Add(clip);
	}
}
