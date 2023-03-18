using UnityEngine;

public class CameraDriverTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float speed = 1f;

	public void Update()
	{
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
		{
			base.transform.position += Time.deltaTime * speed * base.transform.forward;
		}
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
		{
			base.transform.position -= Time.deltaTime * speed * base.transform.forward;
		}
	}
}
