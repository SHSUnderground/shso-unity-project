using UnityEngine;

public class HqWeeble : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Vector3 centerOfMass;

	public void Update()
	{
		if (base.gameObject.rigidbody != null)
		{
			base.gameObject.rigidbody.centerOfMass = centerOfMass;
		}
	}
}
