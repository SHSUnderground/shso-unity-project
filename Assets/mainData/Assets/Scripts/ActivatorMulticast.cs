using System.Collections;
using UnityEngine;

[AddComponentMenu("Lab/Activator/Multicast")]
public class ActivatorMulticast : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject[] Targets = new GameObject[0];

	private void OnEnable()
	{
		GameObject[] targets = Targets;
		foreach (GameObject gameObject in targets)
		{
			gameObject.active = true;
		}
		StartCoroutine(Finish());
	}

	private IEnumerator Finish()
	{
		yield return new WaitForEndOfFrame();
		base.gameObject.active = false;
	}
}
