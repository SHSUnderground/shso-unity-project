using UnityEngine;

public class OrientToScreen : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Camera cameraToPointTo;

	private void Awake()
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
			Vector3 lhs = cameraToPointTo.transform.position - base.transform.position;
			Vector3 b = Vector3.Dot(lhs, cameraToPointTo.transform.forward) * cameraToPointTo.transform.forward;
			base.transform.LookAt(base.transform.position - b);
		}
	}
}
