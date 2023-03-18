using UnityEngine;

public class TriggeredForwarder : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject destination;

	public bool requireReceiver;

	protected void Triggered(object extraData)
	{
		if (destination != null)
		{
			destination.SendMessage("Triggered", extraData, (!requireReceiver) ? SendMessageOptions.DontRequireReceiver : SendMessageOptions.RequireReceiver);
		}
	}
}
