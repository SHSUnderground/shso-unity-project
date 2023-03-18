using UnityEngine;

public class RemoveEditorGameObjects : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private void Awake()
	{
		foreach (Transform item in base.transform)
		{
			Object.Destroy(item.gameObject);
		}
	}
}
