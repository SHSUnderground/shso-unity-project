using UnityEngine;

public class CenterMarker : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private void Start()
	{
		MeshRenderer component = GetComponent<MeshRenderer>();
		component.enabled = false;
	}
}
