using UnityEngine;

public class ColliderRelay : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject targetObject;

	public bool relayTriggerEnter = true;

	public bool relayTriggerExit = true;

	public bool relayTriggerStay = true;

	public bool relayCollisionEnter = true;

	public bool relayCollisionExit = true;

	public bool relayCollisionStay = true;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (relayTriggerEnter)
		{
			targetObject.SendMessage("OnTriggerEnter", other, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (relayTriggerExit)
		{
			targetObject.SendMessage("OnTriggerExit", other, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (relayTriggerStay)
		{
			targetObject.SendMessage("OnTriggerStay", other, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnCollisionEnter(Collision collisionInfo)
	{
		if (relayCollisionEnter)
		{
			targetObject.SendMessage("OnCollisionEnter", collisionInfo, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnCollisionExit(Collision collisionInfo)
	{
		if (relayCollisionExit)
		{
			targetObject.SendMessage("OnCollisionExit", collisionInfo, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnCollisionStay(Collision collisionInfo)
	{
		if (relayCollisionStay)
		{
			targetObject.SendMessage("OnCollisionStay", collisionInfo, SendMessageOptions.DontRequireReceiver);
		}
	}
}
