using System.Collections;
using UnityEngine;

public class PrefabInstantiator : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject prefab;

	public FRange delay = new FRange(0f, 0f);

	private void Awake()
	{
		StartCoroutine(Instantiate());
	}

	public IEnumerator Instantiate()
	{
		if (delay.maximum > 0f)
		{
			yield return new WaitForSeconds(delay.RandomValue);
		}
		GameObject instance = Object.Instantiate(prefab) as GameObject;
		instance.transform.parent = base.gameObject.transform.parent;
		instance.transform.localPosition = base.gameObject.transform.localPosition;
		instance.transform.localRotation = base.gameObject.transform.localRotation;
		Object.Destroy(base.gameObject);
	}
}
