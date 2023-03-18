using System.Collections.Generic;
using UnityEngine;

public class SHSNewsRewardDayWindow : GUIControlWindow
{
	public enum SHSNewsRewardDayState
	{
		Normal,
		Collected,
		CollectedToday,
		CollectNow,
		UnCollected
	}

	public static readonly Vector2 WindowSize = new Vector2(96f, 110f);

	public static readonly string DayLabelTextureSource = "gameworld_bundle|L_dailyreward_label_{0}";

	public static readonly string RewardIconTextureSource = "gameworld_bundle|daily_reward_{0}_{1}";

	public static readonly string CollectedPanelTextureSource = "gameworld_bundle|dailyreward_panel_collected_background";

	public static readonly string TodayPanelTextureSource = "gameworld_bundle|dailyreward_panel_today";

	public static readonly string UnCollectedPanelTextureSource = "gameworld_bundle|dailyreward_panel_unclaimed";

	public static readonly string TodayFXTextureSource = "gameworld_bundle|dailyreward_panel_today_fx2";

	public static readonly string TodayFXCounterTextureSource = "gameworld_bundle|dailyreward_panel_today_fx1";

	public static readonly float NormalAlpha = 1f;

	public static readonly float CollectedAlpha = 0.6f;

	private List<SHSNewsRewardDayLayer> _newsRewardDayLayers;

	private SHSNewsRewardDayState _newsRewardDayState;

	public SHSNewsRewardDayWindow()
	{
		MouseOver += SHSNewsRewardDayWindow_MouseOver;
		MouseOut += SHSNewsRewardDayWindow_MouseOut;
	}

	public void CreateReward(string rewardDayTexture, string rewardCategoryTexture, string rewardTextLabel, ref Vector2 rewardCategorySize, ref Vector2 rewardCategoryOffset)
	{
		_newsRewardDayLayers = new List<SHSNewsRewardDayLayer>();
		AddRewardDayLayer(new SHSNewsRewardMainLayer(rewardDayTexture, rewardCategoryTexture, rewardTextLabel, ref rewardCategorySize, ref rewardCategoryOffset));
	}

	public void ClearReward()
	{
		if (_newsRewardDayLayers != null)
		{
			foreach (SHSNewsRewardDayLayer newsRewardDayLayer in _newsRewardDayLayers)
			{
				newsRewardDayLayer.RemoveLayerFromWindow(this);
				newsRewardDayLayer.StopLayerAnimation(base.AnimationPieceManager);
			}
			_newsRewardDayLayers.Clear();
		}
	}

	public void SetRewardState(SHSNewsRewardDayState rewardState)
	{
		if (rewardState == _newsRewardDayState)
		{
			return;
		}
		SHSNewsRewardMainLayer layer = GetLayer<SHSNewsRewardMainLayer>();
		if (layer != null)
		{
			_newsRewardDayState = rewardState;
			switch (rewardState)
			{
			case SHSNewsRewardDayState.Collected:
				AddRewardDayLayer(new SHSNewsRewardCollectedLayer(CollectedPanelTextureSource, SHSNewsRewardTableWindow.CollectedLabelKey));
				layer.SetRewardTextColor(SHSNewsRewardMainLayer.RewardLabelDayColor);
				layer.SetAlpha(CollectedAlpha);
				break;
			case SHSNewsRewardDayState.CollectedToday:
			case SHSNewsRewardDayState.CollectNow:
				AddRewardDayLayer(new SHSNewsRewardPanelLayer(TodayPanelTextureSource), layer.FirstControl());
				AddRewardDayLayer(new SHSNewsRewardFXLayer(TodayFXTextureSource, TodayFXCounterTextureSource, layer.RewardLabel), layer.FirstControl());
				layer.SetRewardTextColor(SHSNewsRewardMainLayer.RewardLabelCollectColor);
				layer.SetAlpha(NormalAlpha);
				break;
			case SHSNewsRewardDayState.UnCollected:
				AddRewardDayLayer(new SHSNewsRewardPanelLayer(UnCollectedPanelTextureSource), layer.FirstControl());
				layer.SetRewardTextColor(SHSNewsRewardMainLayer.RewardLabelDayColor);
				layer.SetAlpha(NormalAlpha);
				break;
			}
		}
	}

	public void ClearRewardState()
	{
		_newsRewardDayState = SHSNewsRewardDayState.Normal;
	}

	public SHSNewsRewardDayState GetRewardState()
	{
		return _newsRewardDayState;
	}

	public void StartAnimations()
	{
		if (_newsRewardDayState == SHSNewsRewardDayState.CollectNow)
		{
			OnCollectNow();
		}
		else if (_newsRewardDayState == SHSNewsRewardDayState.CollectedToday)
		{
			OnCollectedToday();
		}
	}

	public void OnToday()
	{
		if (_newsRewardDayLayers != null)
		{
			foreach (SHSNewsRewardDayLayer newsRewardDayLayer in _newsRewardDayLayers)
			{
				newsRewardDayLayer.OnToday(base.AnimationPieceManager);
			}
		}
	}

	public void OnCollectNow()
	{
		OnToday();
		if (_newsRewardDayLayers != null)
		{
			foreach (SHSNewsRewardDayLayer newsRewardDayLayer in _newsRewardDayLayers)
			{
				newsRewardDayLayer.OnCollectNow(base.AnimationPieceManager);
			}
		}
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("prizewheel_win"));
	}

	public void OnCollectedToday()
	{
		OnToday();
		if (_newsRewardDayLayers != null)
		{
			foreach (SHSNewsRewardDayLayer newsRewardDayLayer in _newsRewardDayLayers)
			{
				newsRewardDayLayer.OnCollectedToday(base.AnimationPieceManager);
			}
		}
	}

	private void AddRewardDayLayer(SHSNewsRewardDayLayer layer)
	{
		if (_newsRewardDayLayers != null)
		{
			layer.AddLayerToWindow(this);
			_newsRewardDayLayers.Add(layer);
		}
	}

	private void AddRewardDayLayer(SHSNewsRewardDayLayer layer, GUIControl target)
	{
		if (_newsRewardDayLayers != null)
		{
			layer.AddLayerToWindow(this, target);
			_newsRewardDayLayers.Add(layer);
		}
	}

	private void SHSNewsRewardDayWindow_MouseOut(GUIControl sender, GUIMouseEvent EventData)
	{
		if (_newsRewardDayState == SHSNewsRewardDayState.Collected && _newsRewardDayLayers != null)
		{
			foreach (SHSNewsRewardDayLayer newsRewardDayLayer in _newsRewardDayLayers)
			{
				newsRewardDayLayer.OnMouseOut(base.AnimationPieceManager);
			}
		}
	}

	private void SHSNewsRewardDayWindow_MouseOver(GUIControl sender, GUIMouseEvent EventData)
	{
		if (_newsRewardDayState == SHSNewsRewardDayState.Collected && _newsRewardDayLayers != null)
		{
			foreach (SHSNewsRewardDayLayer newsRewardDayLayer in _newsRewardDayLayers)
			{
				newsRewardDayLayer.OnMouseOver(base.AnimationPieceManager);
			}
		}
	}

	private T GetLayer<T>() where T : SHSNewsRewardDayLayer
	{
		if (_newsRewardDayLayers == null)
		{
			return (T)null;
		}
		foreach (SHSNewsRewardDayLayer newsRewardDayLayer in _newsRewardDayLayers)
		{
			if (typeof(T) == newsRewardDayLayer.GetType())
			{
				return newsRewardDayLayer as T;
			}
		}
		return (T)null;
	}
}
