using System.Collections;
using UnityEngine;

public class CoroutineContainer : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public static CoroutineContainer GetInstance(GameObject obj)
	{
		if (obj != null)
		{
			CoroutineContainer coroutineContainer = obj.GetComponent<CoroutineContainer>();
			if (coroutineContainer == null)
			{
				coroutineContainer = obj.AddComponent<CoroutineContainer>();
			}
			return coroutineContainer;
		}
		return null;
	}

	public static GameObject Spawn(string objectName, IEnumerator routine)
	{
		return Spawn(objectName, routine, null);
	}

	public static GameObject Spawn(string objectName, IEnumerator routine, GameObject parent)
	{
		GameObject gameObject = new GameObject(objectName);
		if (parent != null)
		{
			gameObject.transform.parent = parent.transform;
		}
		GetInstance(gameObject).StartCoroutine(routine);
		return gameObject;
	}
}
