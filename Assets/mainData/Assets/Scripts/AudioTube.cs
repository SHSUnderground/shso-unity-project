using UnityEngine;

public class AudioTube : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private void Update()
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = Camera.main.transform.position;
		position.y = position2.y;
		base.transform.position = position;
	}
}
