using System.Collections.Generic;
using UnityEngine;

public class HqReflector : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float forceValue = 1.05f;

	protected Vector3 lastVelocity = Vector3.zero;

	protected Vector3 lastIncomingVelocity = Vector3.zero;

	protected Vector3 lastCollisionPoint = Vector3.zero;

	protected Vector3 lastNormal = Vector3.zero;

	protected List<GameObject> objectsToReflect;

	public void Start()
	{
		objectsToReflect = new List<GameObject>();
	}

	public void Update()
	{
		if (lastVelocity != Vector3.zero)
		{
			Debug.DrawRay(lastCollisionPoint, lastVelocity, Color.red);
			Debug.DrawRay(lastCollisionPoint - lastIncomingVelocity, lastIncomingVelocity, Color.green);
			Debug.DrawRay(lastCollisionPoint, base.gameObject.transform.right);
		}
		Debug.DrawRay(base.gameObject.transform.position, base.gameObject.transform.right);
	}

	public void OnCollisionEnter(Collision collisionInfo)
	{
		if (HqController2.Instance.State == typeof(HqController2.HqControllerFlinga) && collisionInfo.gameObject != null && !objectsToReflect.Contains(collisionInfo.gameObject))
		{
			HqObject2 component = Utils.GetComponent<HqObject2>(collisionInfo.gameObject, Utils.SearchChildren);
			if (component != null)
			{
				ApplyForce(collisionInfo);
				objectsToReflect.Add(collisionInfo.gameObject);
			}
		}
	}

	public void OnCollisionExit(Collision collisionInfo)
	{
		if (collisionInfo.gameObject != null && objectsToReflect.Contains(collisionInfo.gameObject))
		{
			objectsToReflect.Remove(collisionInfo.gameObject);
		}
	}

	protected virtual void ApplyForce(Collision collisionInfo)
	{
		if (collisionInfo.gameObject != null && collisionInfo.contacts.Length > 0 && collisionInfo.gameObject.collider != null && collisionInfo.gameObject.collider.rigidbody != null)
		{
			Vector3 point = collisionInfo.contacts[0].point;
			Vector3 a = Vector3.Reflect(collisionInfo.relativeVelocity, base.gameObject.transform.right);
			collisionInfo.gameObject.collider.rigidbody.velocity = a * forceValue;
			lastCollisionPoint = point;
			lastVelocity = a * forceValue;
			lastIncomingVelocity = collisionInfo.relativeVelocity;
		}
	}
}
