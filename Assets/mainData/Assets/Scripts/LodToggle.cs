using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("LOD/Toggle")]
public class LodToggle : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected enum State
	{
		UNKNOWN,
		ON,
		OFF
	}

	public float maxDistance = 75f;

	public bool rendererOnly = true;

	public float pollSeconds = 0.25f;

	public float pollVariance = 0.1f;

	protected State activated;

	protected List<GameObject> deactivatedObjects;

	public void Start()
	{
		activated = State.UNKNOWN;
		StartCoroutine(PollCamaeraDistance());
	}

	protected IEnumerator PollCamaeraDistance()
	{
		yield return 0;
		float variance = UnityEngine.Random.Range(0f - pollVariance, pollVariance);
		while (true)
		{
			float d2 = (Camera.main.transform.position - base.transform.position).sqrMagnitude;
			if (d2 <= maxDistance * maxDistance)
			{
				if (activated != State.ON)
				{
					ActivateChildren();
				}
			}
			else if (activated != State.OFF)
			{
				DeactivateChildren();
			}
			yield return new WaitForSeconds(pollSeconds + variance);
		}
	}

	protected void ActivateChildren()
	{
		activated = State.ON;
		if (deactivatedObjects != null)
		{
			foreach (GameObject deactivatedObject in deactivatedObjects)
			{
				if (deactivatedObject != null)
				{
					if (rendererOnly && deactivatedObject.renderer != null)
					{
						deactivatedObject.renderer.enabled = true;
					}
					else
					{
						deactivatedObject.active = true;
					}
				}
			}
			deactivatedObjects.Clear();
		}
	}

	protected void DeactivateChildren()
	{
		activated = State.OFF;
		if (deactivatedObjects == null)
		{
			deactivatedObjects = new List<GameObject>();
		}
		Utils.ForEachTree(base.gameObject, delegate(GameObject go)
		{
			if (rendererOnly && go.renderer != null && go.renderer.enabled)
			{
				go.renderer.enabled = false;
				deactivatedObjects.Add(go);
			}
			else if (!rendererOnly && go.active)
			{
				go.active = false;
				deactivatedObjects.Add(go);
			}
		});
	}

	protected void ForAllChildren(Action<GameObject> action)
	{
		foreach (Transform item in base.transform)
		{
			Utils.ForEachTree(item.gameObject, action);
		}
	}
}
