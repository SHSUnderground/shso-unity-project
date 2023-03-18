using System.Collections;
using UnityEngine;

[AddComponentMenu("ScenarioEvent/SFX")]
public class SFXScenarioEvent : ScenarioEventHandlerEnableBase
{
	public ShsAudioSource SFX;

	public float delay;

	private ShsAudioSource instance;

	protected override void OnEnableEvent(string eventName)
	{
		Stop();
		if (SFX != null)
		{
			StartCoroutine(Play());
		}
	}

	protected override void OnDisableEvent(string eventName)
	{
		Stop();
	}

	protected void Stop()
	{
		StopAllCoroutines();
		if (instance != null)
		{
			Object.Destroy(instance.gameObject);
		}
	}

	protected IEnumerator Play()
	{
		yield return new WaitForSeconds(delay);
		instance = ShsAudioSource.PlayAutoSound(SFX.gameObject, base.transform);
	}
}
