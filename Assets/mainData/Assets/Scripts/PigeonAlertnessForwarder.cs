using UnityEngine;

public class PigeonAlertnessForwarder : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public void OnTriggerEnter(Collider other)
	{
		AIControllerPigeon component = Utils.GetComponent<AIControllerPigeon>(base.gameObject, Utils.SearchParents);
		if (component != null)
		{
			component.OnTriggerEnter(other);
		}
	}
}
