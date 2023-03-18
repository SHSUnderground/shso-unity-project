using UnityEngine;

public class VOTransitionHack : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private void Update()
	{
		if (VOManager.Instance != null && VOManager.Instance.IsEmitterInUse(base.transform.root.gameObject))
		{
			Object.Destroy(base.gameObject);
		}
	}
}
