using UnityEngine;

public class SphereVisualizer : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float radius;

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.3f, 0.1f, 0f, 0.6f);
		Gizmos.DrawSphere(base.transform.position, radius);
	}
}
