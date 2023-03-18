using UnityEngine;

public class DestroyOnEmpty : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private void Start()
	{
	}

	private void Update()
	{
		if (base.transform.childCount == 0)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
