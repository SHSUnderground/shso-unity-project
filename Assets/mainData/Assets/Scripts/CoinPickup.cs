using System;
using System.Collections;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool despawns = true;

	public float despawnTime = 30f;

	public float angleDelta = (float)Math.PI;

	public float angleDeltaAccel = 0.1f;

	public float flickerRange = 0.45f;

	public float flickerBase = 0.25f;

	public float flickerPct = 0.45f;

	public float noTriggerDuration = 0.5f;

	private double spawnTime;

	private bool flashMode;

	private float currentSinAngle = (float)Math.PI / 2f;

	private float sin = 1f;

	private MeshRenderer meshRenderer;

	private Color outerColor;

	private Color innerColor;

	private bool pickedUp;

	private double GameTime
	{
		get
		{
			if (ServerTime.Instance != null)
			{
				return ServerTime.time;
			}
			return 0.0;
		}
	}

	private void Start()
	{
		spawnTime = GameTime;
		StartCoroutine(DelayedCheckInitialCollision());
	}

	private void OnEnable()
	{
		spawnTime = GameTime;
	}

	private IEnumerator DelayedCheckInitialCollision()
	{
		yield return new WaitForSeconds(2f);
		if (pickedUp)
		{
			yield break;
		}
		SphereCollider sphere = Utils.GetComponent<SphereCollider>(base.gameObject);
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		if (localPlayer != null && sphere != null)
		{
			float playerRadius = Utils.GetComponent<CharacterController>(localPlayer).radius;
			if (Vector3.Distance(localPlayer.transform.position, base.transform.position) < sphere.radius + playerRadius)
			{
				PickUp();
			}
		}
	}

	private void Update()
	{
		if (!despawns)
		{
			return;
		}
		if (ServerTime.time - spawnTime > (double)despawnTime)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else if (ServerTime.time - spawnTime > (double)(despawnTime * flickerPct) && !flashMode)
		{
			flashMode = true;
			meshRenderer = Utils.GetComponent<MeshRenderer>(base.gameObject, Utils.SearchChildren);
			if (meshRenderer == null)
			{
				CspUtils.DebugLog("Should start flashing, but can't find mesh renderer in child.");
			}
			outerColor = meshRenderer.materials[0].color;
			if (meshRenderer.materials.Length > 1)
			{
				innerColor = meshRenderer.materials[1].color;
			}
		}
		else if (flashMode)
		{
			float num = Mathf.Abs(sin) * flickerRange + flickerBase;
			Material material = meshRenderer.materials[0];
			Color color2 = material.color = new Color(outerColor.r * num, outerColor.g * num, outerColor.b * num, 1f);
			if (meshRenderer.materials.Length > 1)
			{
				Material material2 = meshRenderer.materials[1];
				color2 = (material2.color = new Color(innerColor.r * num, innerColor.g * num, innerColor.b * num, 1f));
			}
			sin = Mathf.Sin(currentSinAngle);
			currentSinAngle += angleDelta * Time.deltaTime;
			angleDelta += angleDeltaAccel;
		}
	}

	protected void OnTriggerEnter(Collider other)
	{
		if (!(ServerTime.time - spawnTime < (double)noTriggerDuration) && other.gameObject == GameController.GetController().LocalPlayer && !pickedUp)
		{
			PickUp();
		}
	}

	protected void PickUp()
	{
		pickedUp = true;
		Collider[] components = Utils.GetComponents<Collider>(base.gameObject);
		Collider[] array = components;
		foreach (Collider collider in array)
		{
			collider.isTrigger = true;
		}
		base.rigidbody.isKinematic = true;
		base.rigidbody.useGravity = false;
		MagneticActivityObjectTriggerAdapter magneticActivityObjectTriggerAdapter = Utils.AddComponent<MagneticActivityObjectTriggerAdapter>(base.gameObject);
		magneticActivityObjectTriggerAdapter.Triggered();
	}
}
