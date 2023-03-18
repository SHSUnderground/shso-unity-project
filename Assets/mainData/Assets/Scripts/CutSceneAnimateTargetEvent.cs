using UnityEngine;

[AddComponentMenu("Cut Scene Clips/Animate Target")]
internal class CutSceneAnimateTargetEvent : CutSceneTargetEvent
{
	public string targetAnimation = string.Empty;

	public WrapMode animationWrapMode = WrapMode.ClampForever;

	public override void StartEvent()
	{
		base.StartEvent();
		if (targetAnimation == string.Empty)
		{
			LogEventError("Animation not set for animate target event");
		}
		else if (!(base.Target == null))
		{
			Animation component = base.Target.GetComponent<Animation>();
			if (!(component == null))
			{
				component[targetAnimation].wrapMode = animationWrapMode;
				component.Play(targetAnimation, PlayMode.StopAll);
				component.CrossFadeQueued("movement_idle");
			}
		}
	}
}
