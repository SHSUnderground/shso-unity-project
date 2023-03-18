using System.Collections.Generic;
using UnityEngine;

public class EffectSpawner : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject[] effects;

	public string[] attachNodes;

	protected List<GameObject> createdEffects;

	protected bool unloading;

	private void Start()
	{
		createdEffects = new List<GameObject>();
		for (int i = 0; i < effects.Length; i++)
		{
			GameObject gameObject = Object.Instantiate(effects[i]) as GameObject;
			Transform root = base.gameObject.transform.root;
			Transform parent = Utils.FindNodeInChildren(root, attachNodes[i]);
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.zero;
			createdEffects.Add(gameObject);
		}
	}

	private void OnUnload()
	{
		unloading = true;
	}

	private void OnDisable()
	{
		if (!unloading)
		{
			foreach (GameObject createdEffect in createdEffects)
			{
				Object.Destroy(createdEffect);
			}
		}
	}
}
