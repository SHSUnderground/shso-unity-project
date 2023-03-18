using System.Collections.Generic;
using UnityEngine;

public class GUIAnimatedFrame : GUIChildControl
{
	public enum CustomAnimation
	{
		None,
		Idle,
		AnimateToFinalStateAndHold,
		Loop
	}

	private List<Texture2D> animation = new List<Texture2D>();

	public float framerate = 15f;

	private string location = string.Empty;

	private int numFrames;

	private CustomAnimation currentCustomAnimation = CustomAnimation.Loop;

	private int animationCycle;

	private float lastUpdateTime;

	private bool firstTimeIn = true;

	public CustomAnimation CurrentCustomAnimation
	{
		get
		{
			return currentCustomAnimation;
		}
		set
		{
			currentCustomAnimation = value;
		}
	}

	public GUIAnimatedFrame(string location, int numberOfFrames)
	{
		this.location = location;
		numFrames = numberOfFrames;
		Traits.ResourceLoadingPhaseTrait = ControlTraits.ResourceLoadingPhaseTraitEnum.Draw;
	}

	public override bool InitializeResources(bool reload)
	{
		if (!base.InitializeResources(reload))
		{
			return false;
		}
		for (int i = 1; i <= numFrames; i++)
		{
			Texture2D texture;
			if (!((i >= 10) ? GUIManager.Instance.LoadTexture(location + i, out texture) : GUIManager.Instance.LoadTexture(location + "0" + i, out texture)))
			{
				CspUtils.DebugLog("art Asset id #" + i + " missing in location " + location);
			}
			animation.Add(texture);
		}
		return true;
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		if (!resourcesInitialized)
		{
			resourcesInitialized = InitializeResources(false);
		}
		if (firstTimeIn)
		{
			firstTimeIn = false;
			animationCycle = 0;
			lastUpdateTime = Time.time;
		}
		if (currentCustomAnimation == CustomAnimation.AnimateToFinalStateAndHold)
		{
			GUI.DrawTexture(base.rect, animation[animationCycle]);
			float time = Time.time;
			if (time > lastUpdateTime + 1f / framerate)
			{
				lastUpdateTime = time;
				animationCycle++;
				if (animationCycle >= animation.Count)
				{
					animationCycle = animation.Count - 1;
				}
			}
		}
		else if (currentCustomAnimation == CustomAnimation.Idle)
		{
			animationCycle = 0;
			GUI.DrawTexture(base.rect, animation[0]);
			firstTimeIn = true;
		}
		else
		{
			if (currentCustomAnimation != CustomAnimation.Loop)
			{
				return;
			}
			GUI.DrawTexture(base.rect, animation[animationCycle]);
			float time2 = Time.time;
			if (time2 > lastUpdateTime + 1f / framerate)
			{
				lastUpdateTime = time2;
				animationCycle++;
				if (animationCycle >= animation.Count)
				{
					animationCycle = 0;
				}
			}
		}
	}

	public void SetFramerate(float fps)
	{
		framerate = fps;
	}
}
