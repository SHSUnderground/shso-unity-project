using UnityEngine;

public class HqToy : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected bool unloading;

	public void OnUnload()
	{
		unloading = true;
	}
}
