using System.Collections;
using UnityEngine;

public class RandomSequenceInstantiator : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public EffectSequence[] selection;

	public bool attachToCharacter = true;

	public bool instantiateOnAwake;

	private void Awake()
	{
		if (instantiateOnAwake)
		{
			StartCoroutine(Instantiate());
		}
	}

	private void Start()
	{
		if (!instantiateOnAwake)
		{
			StartCoroutine(Instantiate());
		}
	}

	public IEnumerator Instantiate()
	{
		if (selection != null && selection.Length != 0)
		{
			GameObject prefab = selection[Random.Range(0, selection.Length)].gameObject;
			GameObject instance = Object.Instantiate(prefab) as GameObject;
			InitializeSequence(instance, base.gameObject);
		}
		yield break;
	}

	private void InitializeSequence(GameObject obj, GameObject parent)
	{
		EffectSequence component = obj.GetComponent<EffectSequence>();
		if (!(component != null))
		{
			return;
		}
		if (attachToCharacter)
		{
			GameObject gameObject = parent;
			while (gameObject.transform.parent != null && gameObject.GetComponent<CharacterGlobals>() == null)
			{
				gameObject = gameObject.transform.parent.gameObject;
			}
			if (gameObject.transform.parent != null)
			{
				parent = gameObject;
			}
		}
		component.Initialize(parent, null, null);
	}
}
