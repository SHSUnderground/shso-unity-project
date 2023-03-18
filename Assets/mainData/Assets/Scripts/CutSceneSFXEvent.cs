using UnityEngine;

[AddComponentMenu("Cut Scene Clips/SFX")]
public class CutSceneSFXEvent : CutSceneEvent
{
	public ShsAudioSource soundEffect;

	public override void StartEvent()
	{
		base.StartEvent();
		if (soundEffect != null)
		{
			ShsAudioSource.PlayAutoSound(soundEffect.gameObject, base.gameObject.transform);
		}
	}
}
