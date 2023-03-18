using System.Collections;
using UnityEngine;

public class PlaySFXTriggerAdapter : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject audioPrefab;

	public float effectRadius;

	private GameObject sfx;

	public GameObject SFX
	{
		get
		{
			return sfx;
		}
	}

	private void Triggered()
	{
		if (sfx == null)
		{
			sfx = (Object.Instantiate(audioPrefab, base.gameObject.transform.position, Quaternion.identity) as GameObject);
			sfx.transform.parent = base.transform;
			ShsAudioSource component = Utils.GetComponent<ShsAudioSource>(sfx);
			component.Play();
			component.StartCoroutine(TrackPlayer(component, base.gameObject));
		}
	}

	private IEnumerator TrackPlayer(ShsAudioSource audioSrc, GameObject parent)
	{
		GameObject player = GameController.GetController().LocalPlayer;
		while (parent != null && player != null && sfx != null && (player.transform.position - sfx.transform.position).magnitude <= effectRadius)
		{
			yield return 0;
		}
		if (sfx != null)
		{
			Object.Destroy(sfx);
		}
	}
}
