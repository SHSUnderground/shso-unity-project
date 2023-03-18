using UnityEngine;

public class AuMultiClip : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject[] multiSoundPrefab;

	public bool attachToMe;

	private void Start()
	{
		GameObject gameObject = (GameObject)Object.Instantiate(multiSoundPrefab[Random.Range(0, multiSoundPrefab.Length)], base.transform.position, base.transform.rotation);
		if (attachToMe)
		{
			gameObject.transform.parent = base.transform;
		}
	}

	private void Update()
	{
	}
}
