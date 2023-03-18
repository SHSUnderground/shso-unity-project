using UnityEngine;

[RequireComponent(typeof(ShsAudioSource))]
[AddComponentMenu("Audio/Queue Source on Stop")]
public class QueueSourceOnStop : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public ShsAudioSource sourceToQueue;

	private void OnAudioFinished(ShsAudioSource source)
	{
		ShsAudioSource shsAudioSource = ShsAudioSource.PlayAutoSound(sourceToQueue.gameObject, base.transform);
		shsAudioSource.gameObject.transform.parent = base.transform.parent;
	}
}
