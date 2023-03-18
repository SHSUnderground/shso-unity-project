using UnityEngine;

public class PlaceholderTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			Gizmos.DrawWireCube(base.transform.position, new Vector3(1f, 2f, 1f));
		}
	}
}
