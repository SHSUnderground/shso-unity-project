using System.Collections;
using UnityEngine;

[AddComponentMenu("Audio/SFX On Disable")]
public class SFXOnDisable : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public ShsAudioSource soundToPlay;

	private GameObject watcher;

	private void OnEnable()
	{
		InstantiateWatcher();
	}

	private void OnDisable()
	{
		if (watcher != null)
		{
			watcher.GetComponent<CoroutineContainer>().StartCoroutine(DelayedInstantiate(base.gameObject, watcher));
		}
	}

	private void InstantiateWatcher()
	{
		watcher = new GameObject(base.name + "_OnDisable");
		watcher.AddComponent<CoroutineContainer>();
		GameObject parent = (!(base.gameObject.transform.parent == null)) ? base.gameObject.transform.parent.gameObject : null;
		Utils.AttachGameObject(parent, watcher);
	}

	private IEnumerator DelayedInstantiate(GameObject originator, GameObject watcherObj)
	{
		yield return 0;
		if (originator != null)
		{
			ShsAudioSource.PlayAutoSound(soundToPlay.gameObject, originator.transform.parent);
		}
		Object.Destroy(watcherObj);
	}
}
