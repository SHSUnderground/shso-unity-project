using UnityEngine;

[AddComponentMenu("Splines/Spline Control Point")]
public class SplineControlPoint : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool DrawAxis;

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(base.transform.position, 0.25f);
		if (DrawAxis)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawRay(base.transform.position, base.transform.rotation * Vector3.forward);
			Gizmos.color = Color.red;
			Gizmos.DrawRay(base.transform.position, base.transform.rotation * Vector3.right);
			Gizmos.color = Color.green;
			Gizmos.DrawRay(base.transform.position, base.transform.rotation * Vector3.up);
		}
	}
}
