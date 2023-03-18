using System.Collections;
using UnityEngine;

public class MagneticTriggerAdapter : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float pickupDistance = 1f;

	public float acceleration = 0.3f;

	public bool useSpinEffect;

	public Vector3 spinAxis = Vector3.forward;

	public float spinDegreesPerSecond = 720f;

	public GameObject[] immobileSubObjects = new GameObject[0];

	private bool triggered;

	public void Triggered()
	{
		if (!triggered)
		{
			triggered = true;
			GameObject collectibleObject = GetCollectibleObject();
			if (collectibleObject != null)
			{
				CoroutineContainer.GetInstance(collectibleObject).StartCoroutine(TrackPlayer(collectibleObject));
			}
		}
	}

	protected virtual void OnCollected(GameObject player)
	{
		Object.Destroy(base.gameObject);
	}

	protected virtual GameObject GetCollectibleObject()
	{
		return base.gameObject;
	}

	private IEnumerator TrackPlayer(GameObject obj)
	{
		if (useSpinEffect)
		{
			Spinner spinEffect = Utils.AddComponent<Spinner>(obj);
			spinEffect.axis = spinAxis;
			spinEffect.degreesPerSecond = spinDegreesPerSecond;
		}
		GameObject player = GameController.GetController().LocalPlayer;
		float pickupDistanceSqr = pickupDistance * pickupDistance;
		float speed = 5f;
		Vector3 delta;
		do
		{
			delta = player.transform.position - obj.transform.position;
			Vector3 movement = delta.normalized * speed * Time.deltaTime;
			obj.transform.position += movement;
			if (immobileSubObjects.Length > 0)
			{
				GameObject[] array = immobileSubObjects;
				foreach (GameObject subObj in array)
				{
					subObj.transform.position -= movement;
				}
			}
			speed += speed * acceleration * Time.deltaTime;
			yield return 0;
		}
		while (player != null && delta.sqrMagnitude > pickupDistanceSqr);
		OnCollected(player);
	}
}
