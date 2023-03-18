using UnityEngine;

public abstract class HqSwitchController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public abstract void Flip();

	public abstract bool CanUse();
}
