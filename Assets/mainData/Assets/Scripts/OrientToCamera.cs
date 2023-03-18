using UnityEngine;

public class OrientToCamera : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Camera cameraToPointTo;

	public bool reverse;

	public bool ignoreYOffset;

	private void Start()
	{
		if (cameraToPointTo == null)
		{
			cameraToPointTo = Camera.main;
		}
		Orient();
	}

	private void Update()
	{
		Orient();
	}

	private void Orient()
	{
		if (cameraToPointTo != null)
		{
			Vector3 vector = cameraToPointTo.transform.position;
			if (reverse)
			{
				vector = base.transform.position + (base.transform.position - vector);
			}
			if (ignoreYOffset)
			{
				Vector3 position = base.transform.position;
				vector.y = position.y;
			}
			base.transform.LookAt(vector);
		}
	}
}
