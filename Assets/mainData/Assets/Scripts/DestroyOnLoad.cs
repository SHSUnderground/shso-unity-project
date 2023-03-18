using UnityEngine;

public class DestroyOnLoad : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private void Start()
	{
		Object.Destroy(base.gameObject);
	}
}
