using UnityEngine;

[AddComponentMenu("ScenarioEvent/Effect Sequence")]
public class ScenarioEventEffectSequence : ScenarioEventHandlerEnableBase
{
	public EffectSequence sequence;

	private EffectSequence instance;

	protected override void OnEnableEvent(string eventName)
	{
		if (instance != null)
		{
			Object.Destroy(instance);
		}
		instance = EffectSequence.PlayOneShot(sequence, base.gameObject);
	}

	protected override void OnDisableEvent(string eventName)
	{
		if (instance != null)
		{
			Object.Destroy(instance);
		}
	}
}
