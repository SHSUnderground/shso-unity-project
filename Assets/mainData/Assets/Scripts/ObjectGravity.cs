using UnityEngine;

[AddComponentMenu("Brawler/ObjectGravity")]
public class ObjectGravity : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float initialVelocity;

	public float acceleration = 10f;

	public GameObject landingReticle;

	public GameObject landingEffect;

	public float landingHeight;

	public bool fallForever;

	protected float landingY;

	protected GameObject reticle;

	protected float velocity;

	protected bool landed;

	protected bool unloading;

	private void Start()
	{
		Vector3 a = base.transform.position;
		RaycastHit hitInfo;
		if (Physics.Raycast(base.transform.position, Vector3.down, out hitInfo, 100f, -271086103))
		{
			a = hitInfo.point + Vector3.up * landingHeight;
		}
		else if (!fallForever)
		{
			CspUtils.DebugLog("ObjectGravity failed to find a landing spot for object " + base.gameObject.name + " at " + base.transform.position + " and will not drop");
		}
		landingY = a.y;
		if (landingReticle != null)
		{
			reticle = (Object.Instantiate(landingReticle, a + landingReticle.transform.position, landingReticle.transform.rotation) as GameObject);
		}
	}

	private void OnUnload()
	{
		unloading = true;
	}

	private void OnDisable()
	{
		if (reticle != null && !unloading)
		{
			Object.Destroy(reticle);
		}
	}

	private void Update()
	{
		if (landed)
		{
			return;
		}
		if (!fallForever)
		{
			Vector3 position = base.transform.position;
			if (!(position.y > landingY))
			{
				goto IL_009f;
			}
		}
		velocity += acceleration * Time.deltaTime;
		Vector3 position2 = base.transform.position;
		position2.y -= velocity * Time.deltaTime;
		if (position2.y < landingY)
		{
			position2.y = landingY;
		}
		base.transform.position = position2;
		goto IL_009f;
		IL_009f:
		if (fallForever)
		{
			return;
		}
		Vector3 position3 = base.transform.position;
		if (position3.y <= landingY + 0.01f)
		{
			landed = true;
			if (reticle != null)
			{
				Object.Destroy(reticle);
			}
			if (landingEffect != null)
			{
				Object.Instantiate(landingEffect, base.transform.position + landingEffect.transform.position, landingEffect.transform.rotation);
			}
		}
	}
}
