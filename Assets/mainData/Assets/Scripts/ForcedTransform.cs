using UnityEngine;

public class ForcedTransform : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool forceRotation;

	public Vector3 rotation;

	private void Start()
	{
	}

	private void LateUpdate()
	{
		if (forceRotation)
		{
			base.transform.rotation = Quaternion.Euler(rotation);
		}
	}
}
