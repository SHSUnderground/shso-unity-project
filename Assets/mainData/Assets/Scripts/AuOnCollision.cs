using UnityEngine;

public class AuOnCollision : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject collisionSoundPrefab;

	public float triggerMagnitude = 5f;

	public bool attachToMe;

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.relativeVelocity.magnitude > triggerMagnitude)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(collisionSoundPrefab, base.transform.position, base.transform.rotation);
			if (attachToMe)
			{
				gameObject.transform.parent = base.transform;
			}
		}
	}
}
