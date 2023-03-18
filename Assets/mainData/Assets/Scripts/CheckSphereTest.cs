using UnityEngine;

public class CheckSphereTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float radius = 1.25f;

	public bool fire;

	private void Start()
	{
	}

	private void Update()
	{
		if (fire)
		{
			fire = false;
			if (Physics.CheckSphere(base.gameObject.transform.position, radius, 804756969))
			{
				CspUtils.DebugLog("CheckSphereTest collided");
			}
			else
			{
				CspUtils.DebugLog("CheckSphereTest did not collide");
			}
			RaycastHit hitInfo;
			if (Physics.Raycast(base.gameObject.transform.position, Vector3.down, out hitInfo, 1000f, 804756969))
			{
				CspUtils.DebugLog("Raycast hit " + hitInfo.collider.gameObject.name + " at " + hitInfo.point + " distance " + hitInfo.distance);
			}
			else
			{
				CspUtils.DebugLog("Raycast did not hit");
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		CspUtils.DebugLog("CheckSphereTest OnTriggerEnter: " + other.gameObject.name);
	}
}
