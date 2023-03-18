using UnityEngine;

public class SuppressVO : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public static bool IsEmitterSuppressed(GameObject emitter)
	{
		if (emitter == null || emitter.GetComponent<SuppressVO>() != null)
		{
			return true;
		}
		return false;
	}
}
