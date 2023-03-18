using UnityEngine;

public class Spinner : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float degreesPerSecond = 180f;

	public Vector3 axis = Vector3.up;

	public bool counterClockwise;

	public void Update()
	{
		base.transform.Rotate(axis, (float)((!counterClockwise) ? 1 : (-1)) * degreesPerSecond * Time.deltaTime);
	}
}
