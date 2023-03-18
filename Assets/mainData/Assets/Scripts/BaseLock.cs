using System.Linq;
using UnityEngine;

public abstract class BaseLock : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string[] authorizedKeys;

	public bool MatchesKey(KeyObject key)
	{
		return authorizedKeys != null && key != null && Enumerable.Contains(authorizedKeys, key.key);
	}

	public virtual void OnKeyEnabled(KeyObject key)
	{
	}

	public virtual void OnKeyDisabled(KeyObject key)
	{
	}
}
