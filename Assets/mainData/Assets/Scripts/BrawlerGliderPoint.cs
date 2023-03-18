using UnityEngine;

public class BrawlerGliderPoint : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(base.transform.position, 0.25f);
	}
}
