using System.Collections.Generic;

public class SHSNewsRewardDayLayer
{
	protected List<GUIControl> layerControls = new List<GUIControl>();

	protected AnimClip layerAnimation;

	protected static readonly float RewardTextChangeTime = 2f;

	public virtual void AddLayerToWindow(GUIWindow rewardDayWindow)
	{
		foreach (GUIControl layerControl in layerControls)
		{
			rewardDayWindow.Add(layerControl);
		}
	}

	public virtual void AddLayerToWindow(GUIWindow rewardDayWindow, GUIControl targetControl)
	{
		foreach (GUIControl layerControl in layerControls)
		{
			rewardDayWindow.Add(layerControl, targetControl);
		}
	}

	public virtual void RemoveLayerFromWindow(GUIWindow rewardDayWindow)
	{
		foreach (GUIControl layerControl in layerControls)
		{
			rewardDayWindow.Remove(layerControl);
		}
	}

	public virtual GUIControl FirstControl()
	{
		return (layerControls.Count <= 0) ? null : layerControls[0];
	}

	public virtual void SetAlpha(float alpha)
	{
		foreach (GUIControl layerControl in layerControls)
		{
			layerControl.Alpha = alpha;
		}
	}

	public virtual void StopLayerAnimation(AnimClipManager clipManager)
	{
		if (layerAnimation != null)
		{
			clipManager.Remove(layerAnimation);
			layerAnimation = null;
		}
	}

	public virtual void OnMouseOver(AnimClipManager clipManager)
	{
	}

	public virtual void OnMouseOut(AnimClipManager clipManager)
	{
	}

	public virtual void OnToday(AnimClipManager clipManager)
	{
	}

	public virtual void OnCollectNow(AnimClipManager clipManager)
	{
	}

	public virtual void OnCollectedToday(AnimClipManager clipManager)
	{
	}
}
