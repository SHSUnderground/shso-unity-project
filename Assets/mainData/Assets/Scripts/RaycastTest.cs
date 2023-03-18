using UnityEngine;

public class RaycastTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Vector3 direction = Vector3.down;

	public float distance = 2f;

	public bool fire;

	private void Start()
	{
	}

	private void Update()
	{
		if (fire)
		{
			fire = false;
			RaycastHit hitInfo;
			if (Physics.Raycast(base.gameObject.transform.position, direction, out hitInfo, distance, 804756969))
			{
				CspUtils.DebugLog("RaycastTest hit " + hitInfo.rigidbody + " at " + hitInfo.point);
			}
			else
			{
				CspUtils.DebugLog("RaycastTest did not hit anything");
			}
		}
	}
}
