using UnityEngine;

public abstract class EmoteListener : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public abstract void OnEmoteBroadcast(sbyte emoteID, GameObject broadcaster);
}
