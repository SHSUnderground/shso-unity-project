using System.Collections;
using UnityEngine;

public class TimedSelfDestruct : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float lifetime = 5f;

	public GameObject destructObject;

	public bool log;

	protected bool started;

	private void Start()
	{
		if (!started)
		{
			Begin(destructObject);
		}
	}

	public void Begin(GameObject destructObject)
	{
		this.destructObject = destructObject;
		StartCoroutine(TimedDestruct());
	}

	public void Begin(GameObject destructObject, float lifetime)
	{
		this.lifetime = lifetime;
		Begin(destructObject);
	}

	public void Begin(GameObject destructObject, float lifetime, bool log)
	{
		this.log = log;
		this.lifetime = lifetime;
		Begin(destructObject);
	}

	private IEnumerator TimedDestruct()
	{
		started = true;
		yield return new WaitForSeconds(lifetime);
		if (destructObject != null)
		{
			if (log)
			{
				CspUtils.DebugLog("Delayed destroying <" + destructObject.name + ">");
			}
			Object.Destroy(destructObject);
		}
		Object.Destroy(base.gameObject);
	}
}
