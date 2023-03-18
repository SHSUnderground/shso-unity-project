using UnityEngine;

public class GizmoTransformBox : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(base.transform.position, new Vector3(0.25f, 0.25f, 0.25f));
	}
}
