using UnityEngine;

[AddComponentMenu("Physics/Center of Mass")]
public class CenterOfMass : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Vector3 CenterOfMassLocation;

	public void Update()
	{
		if (base.gameObject.rigidbody != null)
		{
			base.gameObject.rigidbody.centerOfMass = CenterOfMassLocation;
		}
	}
}
