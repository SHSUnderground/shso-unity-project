using UnityEngine;

public class DisableObjectWithAudioScenarioEvent : DisableObjectScenarioEvent
{
	public ShsAudioSource audioPrefab;

	public bool autoDestroyAudio;

	protected override void OnEnableEvent(string eventName)
	{
		PlayAudio();
		base.OnEnableEvent(eventName);
	}

	protected void PlayAudio()
	{
		if (audioPrefab != null)
		{
			GameObject g = Object.Instantiate(audioPrefab.gameObject, base.gameObject.transform.position, base.gameObject.transform.rotation) as GameObject;
			if (autoDestroyAudio)
			{
				Utils.AddComponent<SuicideOnStop>(g);
			}
			ShsAudioSource component = Utils.GetComponent<ShsAudioSource>(g);
			if (!component.PlayOnAwake)
			{
				component.Play();
			}
		}
	}
}
