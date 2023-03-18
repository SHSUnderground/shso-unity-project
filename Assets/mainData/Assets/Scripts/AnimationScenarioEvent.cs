using UnityEngine;

[AddComponentMenu("ScenarioEvent/Animation")]
public class AnimationScenarioEvent : ScenarioEventHandlerEnableBase
{
	public bool rewindAnimation;

	public string animationName;

	public bool changeWrapMode;

	public WrapMode wrapMode;

	protected override void OnEnableEvent(string eventName)
	{
		string text = (!string.IsNullOrEmpty(animationName)) ? animationName : base.animation.clip.name;
		if (changeWrapMode)
		{
			base.animation[text].wrapMode = wrapMode;
		}
		base.animation.Play(text);
	}

	protected override void OnDisableEvent(string eventName)
	{
		if (rewindAnimation)
		{
			base.animation.Rewind();
			base.animation.Sample();
		}
		base.animation.Stop();
	}
}
