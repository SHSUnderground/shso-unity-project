using UnityEngine;

internal class AIPlayerPrespawner : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public void Awake()
	{
		if (BrawlerController.Instance != null)
		{
			BrawlerController.Instance.PrespawnAIPlayers = true;
		}
	}
}
