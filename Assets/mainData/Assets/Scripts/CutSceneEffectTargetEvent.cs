using UnityEngine;

[AddComponentMenu("Cut Scene Clips/Effect Target")]
internal class CutSceneEffectTargetEvent : CutSceneTargetEvent
{
	public string targetEffect = string.Empty;

	public override void StartEvent()
	{
		base.StartEvent();
		if (targetEffect == string.Empty)
		{
			LogEventError("Effect not set for target effect event");
		}
		else if (!(base.Target == null))
		{
			CombatController component = base.Target.GetComponent<CombatController>();
			if (!(component == null) && component.createEffect(targetEffect, base.Target) == null)
			{
				CspUtils.DebugLog("CutSceneEffectTargetEvent: Failed to create effect <" + targetEffect + "> on target <" + base.Target.name + ">");
			}
		}
	}
}
