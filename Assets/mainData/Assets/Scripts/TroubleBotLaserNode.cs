using UnityEngine;

public class TroubleBotLaserNode : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public static void DestroyLaser(GameObject troublebot)
	{
		TroubleBotLaserNode componentInChildren = troublebot.GetComponentInChildren<TroubleBotLaserNode>();
		if (componentInChildren != null)
		{
			Object.Destroy(componentInChildren.gameObject);
		}
	}
}
