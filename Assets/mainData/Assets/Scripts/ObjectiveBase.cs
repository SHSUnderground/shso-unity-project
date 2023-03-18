using UnityEngine;

public abstract class ObjectiveBase : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public ObjectiveBase()
	{
	}

	public abstract bool IsMet();

	public abstract void Reset();
}
