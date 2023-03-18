using System;
using UnityEngine;

public class SHSNewsRewardFXLayer : SHSNewsRewardDayLayer
{
	private GUIImage _rewardFXTexture;

	private GUIImage _rewardFXCounterTexture;

	private AnimClip _rewardFXFade;

	private AnimClip _rewardFXRotation;

	private AnimClip _rewardFXCounterFade;

	private AnimClip _rewardFXCounterRotation;

	private string _rewardText;

	public SHSNewsRewardFXLayer(string rewardFXTextureSource, string rewardFXCounterTextureSource, string rewardTextLabel)
	{
		_rewardFXTexture = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(100f, 100f), new Vector2(0f, 0f));
		_rewardFXTexture.TextureSource = rewardFXTextureSource;
		layerControls.Add(_rewardFXTexture);
		_rewardFXCounterTexture = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(100f, 100f), new Vector2(0f, 0f));
		_rewardFXCounterTexture.TextureSource = rewardFXCounterTextureSource;
		layerControls.Add(_rewardFXCounterTexture);
		_rewardText = rewardTextLabel;
		CreateTextOutlineLabel(-1f, -1f);
		CreateTextOutlineLabel(-1f, 1f);
		CreateTextOutlineLabel(1f, -1f);
		CreateTextOutlineLabel(1f, 1f);
		_rewardFXFade = null;
		_rewardFXRotation = null;
		_rewardFXCounterFade = null;
		_rewardFXCounterRotation = null;
	}

	public override void StopLayerAnimation(AnimClipManager clipManager)
	{
		base.StopLayerAnimation(clipManager);
		if (_rewardFXFade != null)
		{
			clipManager.Remove(_rewardFXFade);
			_rewardFXFade = null;
		}
		if (_rewardFXCounterFade != null)
		{
			clipManager.Remove(_rewardFXCounterFade);
			_rewardFXCounterFade = null;
		}
		if (_rewardFXRotation != null)
		{
			clipManager.Remove(_rewardFXRotation);
			_rewardFXRotation = null;
		}
		if (_rewardFXCounterRotation != null)
		{
			clipManager.Remove(_rewardFXCounterRotation);
			_rewardFXCounterRotation = null;
		}
	}

	public override void OnToday(AnimClipManager clipManager)
	{
		base.OnToday(clipManager);
		CreateFadeAnimation(clipManager, _rewardFXFade, _rewardFXTexture, 1f, 0.7f);
		CreateFadeAnimation(clipManager, _rewardFXCounterFade, _rewardFXCounterTexture, 1f, 0.8f);
		CreateRotationAnimation(clipManager, _rewardFXRotation, _rewardFXTexture, 0f, 360f);
		CreateRotationAnimation(clipManager, _rewardFXCounterRotation, _rewardFXCounterTexture, 0f, -360f);
	}

	public override void OnCollectedToday(AnimClipManager clipManager)
	{
		base.OnCollectedToday(clipManager);
		CreateCollectedTodayAnimation(clipManager, layerAnimation, false);
	}

	private void CreateTextOutlineLabel(float xOffset, float yOffset)
	{
		GUILabel gUILabel = GUIControl.CreateControlBottomFrame<GUILabel>(new Vector2(96f, 15f), new Vector2(xOffset, yOffset - 13f));
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, GUILabel.GenColor(17, 56, 27), TextAnchor.MiddleCenter);
		gUILabel.Text = _rewardText;
		layerControls.Add(gUILabel);
	}

	private void CreateFadeAnimation(AnimClipManager clipManager, AnimClip clip, GUIImage image, float startAlpha, float endAlpha)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		image.Alpha = startAlpha;
		clip = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(startAlpha, endAlpha, 1f), image);
		clip.OnFinished += (Action)(object)(Action)delegate
		{
			CreateFadeAnimation(clipManager, clip, image, endAlpha, startAlpha);
		};
		clipManager.Add(clip);
	}

	private void CreateRotationAnimation(AnimClipManager clipManager, AnimClip clip, GUIImage image, float startAngle, float endAngle)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		image.Rotation = startAngle;
		clip = AnimClipBuilder.Absolute.Rotation(AnimClipBuilder.Path.Linear(startAngle, endAngle, 3f), image);
		clip.OnFinished += (Action)(object)(Action)delegate
		{
			CreateRotationAnimation(clipManager, clip, image, startAngle, endAngle);
		};
		clipManager.Add(clip);
	}

	private void CreateCollectedTodayAnimation(AnimClipManager clipManager, AnimClip clip, bool collectedText)
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		foreach (GUIControl layerControl in layerControls)
		{
			GUILabel gUILabel = layerControl as GUILabel;
			if (gUILabel != null)
			{
				gUILabel.Text = ((!collectedText) ? _rewardText : AppShell.Instance.stringTable[SHSNewsRewardTableWindow.CollectedLabelKey]);
			}
		}
		clip = SHSAnimations.Generic.Wait(SHSNewsRewardDayLayer.RewardTextChangeTime);
		clip.OnFinished += (Action)(object)(Action)delegate
		{
			CreateCollectedTodayAnimation(clipManager, clip, !collectedText);
		};
		clipManager.Add(clip);
	}
}
