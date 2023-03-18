using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Social Space/Locks/Key")]
public class KeyObject : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string key;

	public GameObject ownerOverride;

	public GameObject Owner
	{
		get
		{
			if (ownerOverride != null)
			{
				return ownerOverride;
			}
			CharacterGlobals component = Utils.GetComponent<CharacterGlobals>(this, Utils.SearchParents);
			if (component != null)
			{
				return component.gameObject;
			}
			return base.transform.root.gameObject;
		}
		set
		{
			ownerOverride = value;
		}
	}

	private void Start()
	{
		foreach (BaseLock matchingLock in GetMatchingLocks())
		{
			matchingLock.OnKeyEnabled(this);
		}
	}

	private void OnEnable()
	{
		foreach (BaseLock matchingLock in GetMatchingLocks())
		{
			matchingLock.OnKeyEnabled(this);
		}
	}

	private void OnDisable()
	{
		foreach (BaseLock matchingLock in GetMatchingLocks())
		{
			matchingLock.OnKeyDisabled(this);
		}
	}

	private IEnumerable<BaseLock> GetMatchingLocks()
	{
		foreach (BaseLock lockObj in Utils.EnumerateObjectsOfType<BaseLock>())
		{
			if (lockObj.MatchesKey(this))
			{
				yield return lockObj;
			}
		}
	}
}
