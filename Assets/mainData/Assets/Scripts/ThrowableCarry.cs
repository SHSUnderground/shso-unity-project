using UnityEngine;

public class ThrowableCarry : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float throwRange = 10f;

	public GameObject projectilePrefab;

	public string attackName = "ThrowableGeneric";

	public ThrowableGround groundComponent;

	protected Vector3 lastPosition;

	protected Vector3 currentVelocity;

	protected Vector3 lastRotation;

	protected Vector3 rotationalVelocity;

	protected GameObject pickupEffect;

	public void Initialize(ThrowableGround sourceGroundComponent, GameObject newPickupEffect)
	{
		groundComponent = sourceGroundComponent;
		pickupEffect = newPickupEffect;
	}

	private void Start()
	{
		lastPosition = base.gameObject.transform.position;
	}

	private void Update()
	{
		if (base.gameObject.transform.parent != null)
		{
			Vector3 eulerAngles = base.gameObject.transform.eulerAngles;
			rotationalVelocity = eulerAngles - lastRotation;
			if (rotationalVelocity.x > 180f)
			{
				rotationalVelocity.x -= 360f;
			}
			else if (rotationalVelocity.x < -180f)
			{
				rotationalVelocity.x += 360f;
			}
			if (rotationalVelocity.y > 180f)
			{
				rotationalVelocity.y -= 360f;
			}
			else if (rotationalVelocity.y < -180f)
			{
				rotationalVelocity.y += 360f;
			}
			if (rotationalVelocity.z > 180f)
			{
				rotationalVelocity.z -= 360f;
			}
			else if (rotationalVelocity.z < -180f)
			{
				rotationalVelocity.z += 360f;
			}
			rotationalVelocity /= Time.deltaTime;
			lastRotation = eulerAngles;
		}
		currentVelocity = (base.gameObject.transform.position - lastPosition) / Time.deltaTime;
		lastPosition = base.gameObject.transform.position;
	}

	public void ProjectileThrown()
	{
		if (pickupEffect != null)
		{
			Object.Destroy(pickupEffect);
		}
		groundComponent.PickupCharacter = null;
		if (string.IsNullOrEmpty(groundComponent.DestroyEvent))
		{
			Object.Destroy(groundComponent.gameObject);
		}
		Object.Destroy(base.gameObject);
	}

	public void DropObject(bool silent)
	{
		if (!silent && projectilePrefab != null)
		{
			ProjectileColliderController projectileColliderController = projectilePrefab.GetComponent(typeof(ProjectileColliderController)) as ProjectileColliderController;
			if (projectileColliderController != null && projectileColliderController.destruction_effect != null)
			{
				GameObject gameObject = Object.Instantiate(projectileColliderController.destruction_effect, base.transform.position, projectileColliderController.destruction_effect.transform.rotation) as GameObject;
				NetworkComponent x = gameObject.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
				CharacterGlobals characterGlobals = base.transform.root.gameObject.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
				if (gameObject != null && x != null && characterGlobals != null && characterGlobals.networkComponent != null)
				{
					characterGlobals.networkComponent.AnnounceObjectSpawn(gameObject, "CombatController", projectileColliderController.destruction_effect.name);
				}
			}
		}
		ProjectileThrown();
	}

	public Vector3 GetRotationalVelocity()
	{
		if (rotationalVelocity.magnitude > 360f)
		{
			return rotationalVelocity.normalized * 360f;
		}
		return rotationalVelocity;
	}
}
