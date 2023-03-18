using UnityEngine;

public class DebugCollision : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool sendDebugMessage = true;

	public void OnTriggerEnter(Collider other)
	{
		CspUtils.DebugLog("Debug Trigger: " + other.gameObject.name);
		FireDebugMessage(other.gameObject);
	}

	public void OnCollisionEnter(Collision other)
	{
		CspUtils.DebugLog("Debug Collision: " + other.gameObject.name);
		FireDebugMessage(other.gameObject);
	}

	protected void FireDebugMessage(GameObject go)
	{
		if (sendDebugMessage)
		{
			go.BroadcastMessage("DebugCollision", null, SendMessageOptions.DontRequireReceiver);
		}
	}
}
