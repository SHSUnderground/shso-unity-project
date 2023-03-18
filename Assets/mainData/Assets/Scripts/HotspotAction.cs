using UnityEngine;

public abstract class HotspotAction : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public delegate void OnFinished();

	public abstract void PerformAction(CharacterGlobals player, OnFinished onFinished);
}
