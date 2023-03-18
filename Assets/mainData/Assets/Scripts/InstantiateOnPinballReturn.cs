using UnityEngine;

public class InstantiateOnPinballReturn : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject objectToInstantiate;

	public float selfDestructTimer;

	private void OnPinballReturnBegan(BehaviorAttackMultiShot owner)
	{
		InstantiateObject();
	}

	private void InstantiateObject()
	{
		GameObject gameObject = Object.Instantiate(objectToInstantiate) as GameObject;
		gameObject.transform.position = base.gameObject.transform.position;
		gameObject.transform.rotation = base.gameObject.transform.rotation;
		if (selfDestructTimer > 0f)
		{
			TimedSelfDestruct timedSelfDestruct = gameObject.AddComponent<TimedSelfDestruct>();
			timedSelfDestruct.lifetime = selfDestructTimer;
		}
	}
}
